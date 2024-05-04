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
            foreach (Monster m in _monsters.Values)
            {
                //m.Update();
            }

            foreach (Projectile p in _projectiles.Values)
            {
                //p.Update();
            }

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
                newPlayer.PositionInfo = new PositionInfo() { State = CreatureState.Idle, PosX = 0, PosY = 0, LocalScaleX = 1}; 
                // 입장시킬 초기 위치 및 상태 일단 하드코딩 플레이어 입장순서에 따른 위치 초기화 todo

                #region Player 입장 성공 시 입장 성공했다고 전송 -> Client에서 MyPlayer 생성
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

                #region 미리 입장해있던 플레이어 모두에게 입장한 플레이어 spawn시키라고 데이터 전송   
                S_Spawn SpawnPacketToOthers = new S_Spawn();
                SpawnPacketToOthers.GameObjectInfoList.Add(newGameObject.GameObjectInfo);

                foreach (Player p in _players.Values)
                {
                    if (p.ObjectId != newGameObject.ObjectId)       // 플레이어가 추가됐을 시, 새로 입장한 자신 제외! // monster나 projectile은 상관없는 조건
                        p.MySession.Send(SpawnPacketToOthers);
                }
                #endregion
            }
            else if (type == GameObjectType.Monster)
            {
                // Todo
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile newProjectile = newGameObject as Projectile;
                _projectiles.Add(newProjectile.ObjectId, newProjectile);
                newProjectile.MyRoom = this;

                #region 플레이어들에게 Projectile spawn시키라고 데이터 전송   
                S_SpawnProjectile SpawnProjectilePacket = new S_SpawnProjectile();
                SpawnProjectilePacket.GameObjectInfo = newProjectile.GameObjectInfo;
                SpawnProjectilePacket.OwnerInfo = newProjectile.Owner.GameObjectInfo;

                foreach (Player p in _players.Values)
                {
                    p.MySession.Send(SpawnProjectilePacket);
                }
                #endregion
            }
        }

        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectMgr.GetObjectTypebyId(objectId);

            if (type == GameObjectType.Player)
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
            else if (type == GameObjectType.Monster)
            {
                Monster monster = null;
                if (_monsters.Remove(objectId, out monster) == false)
                    return;

                monster.MyRoom = null;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;
                if (_projectiles.Remove(objectId, out projectile) == false)
                    return;

                projectile.MyRoom = null;
            }

            #region 타인한테 Object가 퇴장했다고 데이터 전송
            {
                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.GameObjectIdlist.Add(objectId);
                foreach (Player player in _players.Values)
                {
                    if (player.GameObjectInfo.ObjectId != objectId)
                        player.MySession.Send(despawnPacket);
                }
            }
            #endregion 
        }

        public void HandleMove(GameObject gameObject, C_Move movePacket)
        {
            if (gameObject == null)
                return;
            GameObjectInfo gameObjectInfo = gameObject.GameObjectInfo;

            // move 패킷 정상 검증 todo
            
            // 서버에 저장된 자신의 좌표 변경(이동)
            gameObjectInfo.PositionInfo.State = movePacket.PositionInfo.State;
            ApplyMove(gameObject, movePacket.PositionInfo);

            // 다른 플레이어들에게 자기위치 방송
            S_Move broadMovePkt = new S_Move(); // 방송하려고 서버측에서 보내는 Move 패킷
            broadMovePkt.ObjectId = gameObject.GameObjectInfo.ObjectId;   // 움직인 자신 Id 입력
            broadMovePkt.PositionInfo = movePacket.PositionInfo;

            BroadCast(broadMovePkt);
        }

        public void HandleSkill(Player player, C_Skill skillPacket) 
        {
            if (player == null)
                return;

            // Todo 스킬 사용 가능 여부 체크

            S_Skill broadSkillPacket = new S_Skill();
            broadSkillPacket.SkillUserId = player.ObjectId;
            broadSkillPacket.SkillId = skillPacket.SkillId;
            BroadCast(broadSkillPacket);

            switch (skillPacket.SkillId)
            {
                case 1:             // Human_Slash
                    break;
                case 2:             // Human_ThrowBomb
                    {
                        Projectile Bomb = ObjectMgr.Instance.Add<Projectile>(player.MyRoom);
                        if (Bomb == null)
                            return;
                        Bomb.Owner = player;
                        Push(EnterGame, Bomb);
                    }
                    break;
                case 3:             // Elf_ArrowShot
                    break;
                case 4:             // Elf_Knife
                    break;
            }
        }

        public void ApplyMove(GameObject gameObject, PositionInfo movePositionInfo)
        {
            GameObjectType type = ObjectMgr.GetObjectTypebyId(gameObject.GameObjectInfo.ObjectId);

            if (type == GameObjectType.Player)
            {
                Player TargetPlayer;
                if (_players.TryGetValue(gameObject.GameObjectInfo.ObjectId, out TargetPlayer))
                {
                    TargetPlayer.GameObjectInfo.PositionInfo = movePositionInfo;
                }
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile TargetProjectile;
                if (_projectiles.TryGetValue(gameObject.GameObjectInfo.ObjectId, out TargetProjectile))
                {
                    TargetProjectile.GameObjectInfo.PositionInfo = movePositionInfo;
                }
            }
        }

        public void BroadCast(IMessage packet)
        {
            foreach (Player p in _players.Values)
            {
                p.MySession.Send(packet);
            }
        }

        public GameObject FindGameObjectWithId(int gameObjectId)
        {
            GameObjectType type = ObjectMgr.GetObjectTypebyId(gameObjectId);

            if(type == GameObjectType.Player)
            {
                Player TargerPlayer = null;
                if (_players.TryGetValue(gameObjectId, out TargerPlayer))
                    return TargerPlayer;
            }
            else if (type == GameObjectType.Monster)
            {
                Monster TargerMonster = null;
                if (_monsters.TryGetValue(gameObjectId, out TargerMonster))
                    return TargerMonster;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile TargetProjectile = null;
                if (_projectiles.TryGetValue(gameObjectId, out TargetProjectile))
                    return TargetProjectile;
            }

            return null;
        }
    }
}
