using Google.Protobuf.Protocol;
using Google.Protobuf;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server   
{
    class ClientSession : PacketSession // 실제로 각 상황에서 사용될 기능 구현     // Client에 앉혀둘 대리인
    {                                         
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
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"Client Session ({this.Sessionid}) OnConnected : {endPoint}");
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
            Console.WriteLine($"OnDisConnected ({this.Sessionid}) : {endPoint}");
        }

        public override void OnSend(int numOfbytes)
        {
            //  Console.WriteLine($"Transferred args byte : {numOfbytes}");
        }
    }
}
