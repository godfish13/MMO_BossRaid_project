using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
    public class GameRoom : JobSerializer
    {
        public int RoomId { get; set; }

        Dictionary<int, Player> _players = new Dictionary<int, Player>(); // 해당 룸에 접속중인 player들
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

        public Map Map { get; set; } = new Map();

        public void Init(string mapName)
        {
            Map.LoadMap(mapName);
            Console.WriteLine($"{Map.MapName} : X({Map.MinX} ~ {Map.MaxX}), Y({Map.MinY} ~ {Map.MaxY})");
        }

        public void Update()
        {

            base.Flush();
        }

        public void EnterGame(GameObject newGameObject) 
        {
            if (newGameObject == null)
            {
                Console.WriteLine("ERROR) there is no newObject : newObject is null");
                return;
            }

            GameObjectType type = ObjectMgr.GetObjectTypebyId(newGameObject.ObjectId);
            Console.WriteLine($"Type : {type} Entered to GameRoom({RoomId})");

            if (type == GameObjectType.Player)
            {
                Player newPlayer = newGameObject as Player;
                _players.Add(newPlayer.ObjectId, newPlayer);
                newPlayer.MyRoom = this;

                #region Player 입장 성공 시 입장 성공했다고 전송
                S_EnterGame EnterPacket = new S_EnterGame();
                EnterPacket.GameObjectInfo = newPlayer.GameObjectInfo;
                Console.WriteLine($"Id : {EnterPacket.GameObjectInfo.ObjectId} Enter Packet sended");
                //Console.WriteLine($"Class : {EnterPacket.GameObjectInfo.StatInfo} Enter Packet sended");
                newPlayer.MySession.Send(EnterPacket);

                S_Spawn SpawnOthersPacketToMe = new S_Spawn();    // 먼저 입장해있던 타 플레이어들 정보 전송          
                foreach (Player p in _players.Values)
                {
                    if (p != newPlayer)     // 자기자신 정보 제외
                        SpawnOthersPacketToMe.GameObjectInfoList.Add(p.GameObjectInfo);                  
                }

                newPlayer.MySession.Send(SpawnOthersPacketToMe);
                #endregion
            }
            else if (type == GameObjectType.Monster)
            {
                // Todo
            }
            else if (type == GameObjectType.Projectile)
            {
                // Todo
            }

            #region 미리 입장해있던 플레이어 모두에게 입장한 오브젝트 spawn시키라고 데이터 전송   
            S_Spawn SpawnPacketToOthers = new S_Spawn();
            SpawnPacketToOthers.GameObjectInfoList.Add(newGameObject.GameObjectInfo);

            foreach (Player p in _players.Values)
            {
                if (p.ObjectId != newGameObject.ObjectId)       // 새로 입장한 자신 제외!
                    p.MySession.Send(SpawnPacketToOthers);
            }
            #endregion
        }

        public void LeaveGame(int objectId)
        {
            Player player = null;
            if (_players.Remove(objectId, out player) == false)
                return;

            player.MyRoom = null;

            #region player 퇴장 성공시 Client의 player 본인에게 데이터 전송
            {
                S_LeaveGame leavePacket = new S_LeaveGame();
                player.MySession.Send(leavePacket);
            }
            #endregion
        }

        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
                return;
            GameObjectInfo gameObjectInfo = player.GameObjectInfo;

            /* move 패킷 정상 검증 todo
            PositionInfo movePositionInfo = movePacket.PositionInfo; 
                       
            if (movePositionInfo.PosX != gameObjectInfo.PositionInfo.PosX || movePositionInfo.PosY != gameObjectInfo.PositionInfo.PosY) // 현 좌표랑 목표좌표랑 다른지 체크
            {
                if (Map.MinX < movePositionInfo.PosX && movePositionInfo.PosX < Map.MaxX && Map.MinY < movePositionInfo.PosY && movePositionInfo.PosY < Map.MaxY)
                {
                    // if문 여러개 쓰기 귀찮아서 else쓰려고 위 조건 씀
                    // 좌표값 이상하면 else문 실행
                }
                else
                {
                    Console.WriteLine("이상한 좌표값 강제이동 얍");
                    movePositionInfo = Map.InBoundary(movePositionInfo);  // 맵범위 내 좌표로 강제이동   
                }        
            }*/

            // 서버에 저장된 자신의 좌표 변경(이동)
            gameObjectInfo.PositionInfo.State = movePacket.PositionInfo.State;
            ApplyMove(player, movePacket.PositionInfo);

            // 다른 플레이어들에게 자기위치 방송
            S_Move broadMovePkt = new S_Move(); // 방송하려고 서버측에서 보내는 M 패킷
            broadMovePkt.ObjectId = player.GameObjectInfo.ObjectId;   // 움직인 자신 Id 입력
            broadMovePkt.PositionInfo = movePacket.PositionInfo;

            BroadCast(broadMovePkt);
        }

        public void ApplyMove(Player player, PositionInfo movePositionInfo)
        {
            Player TargetPlayer;
            if (_players.TryGetValue(player.GameObjectInfo.ObjectId, out TargetPlayer))
            {              
                TargetPlayer.GameObjectInfo.PositionInfo = movePositionInfo;
            }
        }

        public void BroadCast(IMessage packet)
        {
            foreach (Player p in _players.Values)
            {
                p.MySession.Send(packet);
            }
        }
    }
}
