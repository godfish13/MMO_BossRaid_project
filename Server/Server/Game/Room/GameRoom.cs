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

        public Dictionary<int, Player> _players = new Dictionary<int, Player>(); // 해당 룸에 접속중인 player들
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

        public Map Map { get; set; } = new Map();

        public void Init(string mapName)
        {
            Map.LoadMap(mapName);
            Console.WriteLine($"{Map.MapName} : X({Map.MinX} ~ {Map.MaxX}), Y({Map.MinY} ~ {Map.MaxY})");

            // tmp monster init
            Monster monster = ObjectMgr.Instance.Add<Monster>();
            monster.MonsterName = monster.StatInfo.Class;
            monster.PositionInfo.PosX = 0;
            monster.PositionInfo.PosY = 0;
            monster.PositionInfo.LocalScaleX = -1;
            Push(EnterGame, monster);
        }

        public void Update()
        {
            foreach (Monster m in _monsters.Values)
            {
                m.Update();
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

            GameObjectType type = ObjectMgr.GetObjectTypebyId(newGameObject.GameObjectId);
            //Console.WriteLine($"Type : {type} Entered to GameRoom({RoomId})");

            if (type == GameObjectType.Player)
            {
                Player newPlayer = newGameObject as Player;
                _players.Add(newPlayer.GameObjectId, newPlayer);
                newPlayer.MyRoom = this;
                newPlayer.PositionInfo = new PositionInfo() { State = CreatureState.Idle, PosX = -15, PosY = 6.5f, LocalScaleX = 1}; 
                // 입장시킬 초기 위치 및 상태 일단 하드코딩 플레이어 입장순서에 따른 위치 초기화 todo

                #region Player 입장 성공 시 입장 성공했다고 전송 -> Client에서 MyPlayer 생성
                S_EnterGame EnterPacket = new S_EnterGame();
                EnterPacket.GameObjectInfo = newPlayer.GameObjectInfo;
                Console.WriteLine($"Id : {EnterPacket.GameObjectInfo.GameObjectId} Enter Packet sended");
                //Console.WriteLine($"Class : {EnterPacket.GameObjectInfo.StatInfo} Enter Packet sended");
                newPlayer.MySession.Send(EnterPacket);
                #endregion

                #region Player가 입장하기 전 미리 GameRoom에 입장해있던 존재들 Spawn Packet 전송
                S_Spawn SpawnOthersPacketToMe = new S_Spawn();    // 먼저 입장해있던 타 플레이어들 정보 전송          
                foreach (Player p in _players.Values)
                {
                    if (p != newPlayer)     // 자기자신 정보 제외
                        SpawnOthersPacketToMe.GameObjectInfoList.Add(p.GameObjectInfo);                  
                }

                foreach (Monster m in _monsters.Values)         // 먼저 입장해있던 몬스터 정보 전송
                    SpawnOthersPacketToMe.GameObjectInfoList.Add(m.GameObjectInfo);

                newPlayer.MySession.Send(SpawnOthersPacketToMe);
                #endregion

                #region 미리 입장해있던 플레이어 모두에게 입장한 플레이어 spawn시키라고 데이터 전송   
                S_Spawn SpawnPacketToOthers = new S_Spawn();
                SpawnPacketToOthers.GameObjectInfoList.Add(newPlayer.GameObjectInfo);

                foreach (Player p in _players.Values)
                {
                    if (p.GameObjectId != newPlayer.GameObjectId) 
                        p.MySession.Send(SpawnPacketToOthers);
                }
                #endregion
            }
            else if (type == GameObjectType.Monster)
            {
                Monster newMonster = newGameObject as Monster;
                _monsters.Add(newMonster.GameObjectId, newMonster);
                newMonster.MyRoom = this;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile newProjectile = newGameObject as Projectile;
                _projectiles.Add(newProjectile.GameObjectId, newProjectile);
                newProjectile.MyRoom = this;

                #region 플레이어들에게 Projectile spawn시키라고 데이터 전송   
                S_SpawnProjectile SpawnProjectilePacket = new S_SpawnProjectile();
                SpawnProjectilePacket.GameObjectInfo = newProjectile.GameObjectInfo;
                SpawnProjectilePacket.OwnerInfo = newProjectile.Owner.GameObjectInfo;

                foreach (Player p in _players.Values)
                {
                    p.MySession.Send(SpawnProjectilePacket);
                }

                // 특정 플레이어가 접속 이전에 사용해둔 projectile 생성파트는 일단 접속전 미리 사용할일이 없도록 게임설계예정이므로 보류 
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
                    if (player.GameObjectInfo.GameObjectId != objectId)
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
            broadMovePkt.GameObjectId = gameObject.GameObjectInfo.GameObjectId;   // 움직인 자신 Id 입력
            broadMovePkt.PositionInfo = movePacket.PositionInfo;

            BroadCast(broadMovePkt);
        }

        public void HandleSkill(Player player, C_Skill skillPacket)     // 스킬 사용 판정만, Hp 변동은 아래 HandleHp 에서
        {
            if (player == null)
                return;

            // Todo 스킬 사용 가능 여부 체크       

            switch (skillPacket.SkillId)
            {
                case (int)Define.SkillId.Human_Slash:             // Human_Slash
                    break;
                case (int)Define.SkillId.Human_ThrowBomb:             // Human_ThrowBomb
                    {
                        Projectile Bomb = ObjectMgr.Instance.Add<Projectile>();
                        if (Bomb == null)
                            return;
                        Bomb.Owner = player;
                        Push(EnterGame, Bomb);
                    }
                    break;
                case (int)Define.SkillId.Elf_ArrowShot:             // Elf_ArrowShot
                    break;
                case (int)Define.SkillId.Elf_Knife:             // Elf_Knife
                    break;
                case (int)Define.SkillId.Furry_Slash:             // Furry_Slash
                    break;
            }

            //S_Skill broadSkillPacket = new S_Skill();
            //broadSkillPacket.SkillUserId = player.ObjectId;
            //broadSkillPacket.SkillId = skillPacket.SkillId;
            //BroadCast(broadSkillPacket);
        }

        public void HandleHp(Player player, C_Hpdelta hpdeltaPacket)
        {
            if (player == null)
                return;  

            switch (hpdeltaPacket.SkillId)
            {
                case (int)Define.SkillId.Human_Slash:             // Human_Slash
                    {
                        Console.WriteLine($"hitted {hpdeltaPacket.HittedGameObjectId} by Human_Slash");
                        _monsters.TryGetValue(hpdeltaPacket.HittedGameObjectId, out Monster Monster);
                        Monster.Hp -= player.SkillDamage;
                    }
                    break;
                case (int)Define.SkillId.Human_ThrowBomb:             // Human_ThrowBomb
                    {
                        Console.WriteLine($"hitted {hpdeltaPacket.HittedGameObjectId} by Human_ThrowBomb");
                        _monsters.TryGetValue(hpdeltaPacket.HittedGameObjectId, out Monster Monster);
                        Monster.Hp -= player.SubSkillDamage;
                    }
                    break;
                case (int)Define.SkillId.Elf_ArrowShot:             // Elf_ArrowShot
                    break;
                case (int)Define.SkillId.Elf_Knife:             // Elf_Knife
                    break;
                case (int)Define.SkillId.Furry_Slash:             // Furry_Slash
                    break;
                case (int)Define.SkillId.Dragon_Bite:             // Dragon_Bite    // 플레이어 체력 깎아주기
                    {
                        //player.Hp -= _monsters.
                    }
                    break;
            }
        }

        public void ApplyMove(GameObject gameObject, PositionInfo movePositionInfo)
        {
            GameObjectType type = ObjectMgr.GetObjectTypebyId(gameObject.GameObjectInfo.GameObjectId);

            if (type == GameObjectType.Player)
            {
                Player TargetPlayer;
                if (_players.TryGetValue(gameObject.GameObjectInfo.GameObjectId, out TargetPlayer))
                {
                    TargetPlayer.GameObjectInfo.PositionInfo = movePositionInfo;
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster TargetMonster;
                if (_monsters.TryGetValue(gameObject.GameObjectInfo.GameObjectId, out TargetMonster))
                {
                    TargetMonster.GameObjectInfo.PositionInfo = movePositionInfo;
                }
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile TargetProjectile;
                if (_projectiles.TryGetValue(gameObject.GameObjectInfo.GameObjectId, out TargetProjectile))
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

        #region MonsterAI requirement
        public void DistanceCalculater(int monsterObjectId)     // 몬스터와 player간 x 거리 저장
        {
            Monster monster = null;
            if (_monsters.TryGetValue(monsterObjectId, out monster) == false)
            {
                Console.WriteLine("there is no Monster in Server");
                return;
            }
               
            if (_players.Count == 0)
            {
                Console.WriteLine("there is no Player in Server");
                return;
            }
                
            foreach (Player p in _players.Values)
            {
                p.DistanceBetweenMonster = Math.Abs(p.PositionInfo.PosX - monster.PositionInfo.PosX);    // 연산효율을 위해 X값만 따짐
                //Console.WriteLine($"player {p.ObjectId} / monster {monster.ObjectId} distance : {p.DistanceBetweenMonster}");
            }
        }

        public Player FindPlayer(Func<Player, bool> condition)  // 원시적으로 플레이어 전부 탐색, condition에 맞는 player return
        {
            foreach (Player p in _players.Values)
            {
                if (condition.Invoke(p))
                    return p;
            }
            return null;
        }
        #endregion
    }
}
