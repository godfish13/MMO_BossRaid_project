using Google.Protobuf.Protocol;
using Google.Protobuf;
using ServerCore;
using System.Net;
using Server.Game;
using Server.Data;

namespace Server   
{
    public class ClientSession : PacketSession // 실제로 각 상황에서 사용될 기능 구현     // Client에 앉혀둘 대리인
    {
        public Player myPlayer { get; set; }	// 이 Session을 가진 Player
        public int Sessionid { get; set; }

        public void Send(IMessage packet)
        {
            string MsgName = packet.Descriptor.Name.Replace("_", string.Empty); // Descriptor.Name : 패킷의 이름 꺼내옴 / "_"는 실제 실행시 무시되기때문에 없애줌
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), MsgName);	// Enum.Parse(Type, string) : string과 같은 이름을 지닌 Type을 뱉어줌

            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];	// 제일 앞에 패킷크기, 다음에 패킷 Id 넣어줄 공간 4byte(ushort 2개) 추가
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));	// 패킷 크기 // GetBytes(ushort)로 쬐꼼이라도 성능향상...
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));	// 패킷 Id
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);						// 패킷 내용

            Send(new ArraySegment<byte>(sendBuffer));
            //Console.WriteLine($"Send {packet}");
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"Client Session ({this.Sessionid}) OnConnected : {endPoint}");

            GameRoom gameRoom = RoomMgr.Instance.Find(1);           // GameRoom 1번방만 하드코딩
            myPlayer = ObjectMgr.Instance.Add<Player>();    // 접속한 플레이어 생성 및 Id generate 후 기록
            myPlayer.MySession = this;                              // 자신이 대변하는 플레이어 기록
            gameRoom.Push(gameRoom.EnterGame, myPlayer);            // GameRoom에 플레이어 입장
        }
        
        public override void OnReceivePacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
            //Console.WriteLine("ClentSession received Packet");
            // 싱글톤으로 구현해둔 PacketManager에 연결
        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            SessionManager.instance.Remove(this);

            GameRoom gameRoom = RoomMgr.Instance.Find(1);
            gameRoom.Push(gameRoom.LeaveGame, myPlayer.ObjectId);

            Console.WriteLine($"OnDisConnected ({this.Sessionid}) : {endPoint}");
        }

        public override void OnSend(int numOfbytes)
        {
            //  Console.WriteLine($"Transferred args byte : {numOfbytes}");
        }
    }
}
