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
        //Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        //Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();
        
        public void Init(int mapId)
        {
            
        }

        public void Update()
        {

            base.Flush();
        }

        public void EnterGame(CreatureObject newCreature) 
        {
            if (newCreature == null)
            {
                Console.WriteLine("ERROR) there is no newObject : newObject is null");
                return;
            }

            GameObjectType type = ObjectMgr.GetObjectTypebyId(newCreature.CreatureId);
            Console.WriteLine($"Type : {type} Entered to GameRoom({RoomId})");

            if (type == GameObjectType.Player)
            {
                Player newPlayer = newCreature as Player;
                _players.Add(newPlayer.CreatureId, newPlayer);
                newPlayer.MyRoom = this;

                #region Player 입장 성공 시 입장 성공했다고 전송
                S_EnterGame EnterPacket = new S_EnterGame();
                EnterPacket.CreatureInfo = newPlayer.CreatureInfo;
                Console.WriteLine($"Class : {EnterPacket.CreatureInfo.StatInfo} Enter Packet sended");
                newPlayer.MySession.Send(EnterPacket);

                S_Spawn SpawnOthersPacketToMe = new S_Spawn();    // 먼저 입장해있던 타 플레이어들 정보 전송          
                foreach (Player p in _players.Values)
                {
                    if (p != newPlayer)     // 자기자신 정보 제외
                        SpawnOthersPacketToMe.CreatureInfoList.Add(p.CreatureInfo);                  
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
            SpawnPacketToOthers.CreatureInfoList.Add(newCreature.CreatureInfo);

            foreach (Player p in _players.Values)
            {
                if (p.CreatureId != newCreature.CreatureId)       // 새로 입장한 자신 제외!
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


    }
}
