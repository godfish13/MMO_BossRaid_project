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
                p.Update();
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
                newPlayer.ClassId = _players.Count - 1;              // 입장 순서대로 0(Human) 1(Elf) 2(Furry)

                StatInfo _stat = null;                                  // Json으로 불러놓은 player Data 넣기
                DataMgr.StatDictionary.TryGetValue(key: newPlayer.ClassId, out _stat);
                newPlayer.StatInfo.MergeFrom(_stat);
                SkillInfo _skill = null;
                DataMgr.SkillDictionary.TryGetValue(key: newPlayer.ClassId, out _skill);
                newPlayer.SkillInfo.MergeFrom(_skill);

                newPlayer.MyRoom = this;
                newPlayer.PositionInfo = new PositionInfo() { State = CreatureState.Idle, PosX = -15, PosY = 6.5f, LocalScaleX = 1}; 
                // 입장시킬 초기 위치 및 상태 일단 하드코딩 플레이어 입장순서에 따른 위치 초기화 todo

                #region Player 입장 성공 시 입장 성공했다고 전송 -> Client에서 MyPlayer 생성
                S_EnterGame EnterPacket = new S_EnterGame();
                EnterPacket.GameObjectInfo = newPlayer.GameObjectInfo;
                Console.WriteLine($"Id : {EnterPacket.GameObjectInfo.GameObjectId} Enter Packet sended");
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
                SpawnProjectilePacket.GameObjectInfo.GameObjectId = newProjectile.GameObjectId;
                SpawnProjectilePacket.GameObjectInfo.ProjectileType = newProjectile.ProjectileType;
                SpawnProjectilePacket.OwnerInfo = newProjectile.Owner.GameObjectInfo;
                SpawnProjectilePacket.Speed = newProjectile.Speed;

                foreach (Player p in _players.Values)
                {
                    p.MySession.Send(SpawnProjectilePacket);
                }

                // 특정 플레이어가 접속 이전에 사용해둔 projectile 생성파트는 일단 접속전 미리 사용할일이 없도록 게임설계예정이므로 보류 
                #endregion
            }
        }

        public void LeaveGame(int gameObjectId)
        {
            GameObjectType type = ObjectMgr.GetObjectTypebyId(gameObjectId);

            if (type == GameObjectType.Player)
            {
                Player player = null;
                if (_players.Remove(gameObjectId, out player) == false)
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
                if (_monsters.Remove(gameObjectId, out monster) == false)
                    return;

                monster.MyRoom = null;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;
                if (_projectiles.Remove(gameObjectId, out projectile) == false)
                    return;

                projectile.MyRoom = null;
            }

            #region 타인한테 Object가 퇴장했다고 데이터 전송
            {
                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.GameObjectIdlist.Add(gameObjectId);
                foreach (Player player in _players.Values)
                {
                    if (player.GameObjectInfo.GameObjectId != gameObjectId)
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
                        Bomb Bomb = ObjectMgr.Instance.Add<Bomb>();
                        if (Bomb == null)
                            return;
                        Bomb.Owner = player;
                        Bomb.ProjectileType = (int)Define.ProjectileType.Human_Bomb;
                        Push(EnterGame, Bomb);
                    }
                    break;
                case (int)Define.SkillId.Elf_ArrowShot:             // Elf_ArrowShot
                    {
                        ElfArrow elfArrow = ObjectMgr.Instance.Add<ElfArrow>();
                        if (elfArrow == null)
                            return;
                        elfArrow.Owner = player;
                        elfArrow.PositionInfo.PosX = elfArrow.Owner.PositionInfo.PosX;
                        elfArrow.PositionInfo.PosY = elfArrow.Owner.PositionInfo.PosY;
                        elfArrow.PositionInfo.LocalScaleX = elfArrow.Owner.PositionInfo.LocalScaleX;
                        elfArrow.ProjectileType = (int)Define.ProjectileType.Elf_Arrow;
                        Push(EnterGame, elfArrow);
                    }
                    break;
                case (int)Define.SkillId.Elf_Knife:             // Elf_Knife
                    break;
                case (int)Define.SkillId.Furry_Slash:             // Furry_Slash
                    break;
            }
        }

        public void HandleHp(Player player, C_Hpdelta hpdeltaPacket)
        {
            if (player == null)
                return;  

            switch (hpdeltaPacket.SkillId)
            {
                case (int)Define.SkillId.Human_Slash:      
                    {
                        _monsters.TryGetValue(hpdeltaPacket.HittedGameObjectId, out Monster Monster);
                        Monster.Hp -= player.SkillDamage;
                    }
                    break;
                case (int)Define.SkillId.Human_ThrowBomb:      
                    {
                        _monsters.TryGetValue(hpdeltaPacket.HittedGameObjectId, out Monster Monster);
                        Monster.Hp -= player.SubSkillDamage;
                    }
                    break;
                case (int)Define.SkillId.Elf_ArrowShot:    
                    {
                        _monsters.TryGetValue(hpdeltaPacket.HittedGameObjectId, out Monster Monster);
                        Monster.Hp -= player.SkillDamage;
                    }
                    break;
                case (int)Define.SkillId.Elf_Knife:      
                    {
                        _monsters.TryGetValue(hpdeltaPacket.HittedGameObjectId, out Monster Monster);
                        Monster.Hp -= player.SubSkillDamage;
                    }
                    break;
                case (int)Define.SkillId.Furry_Slash:
                    {
                        _monsters.TryGetValue(hpdeltaPacket.HittedGameObjectId, out Monster Monster);
                        Monster.Hp -= player.SkillDamage;
                    }
                    break;
                case (int)Define.SkillId.Dragon_Bite:            
                    {
                        Monster monster;
                        _monsters.TryGetValue(hpdeltaPacket.AttackerGameObjectId, out monster);
                        Player hittedPlayer;
                        _players.TryGetValue(hpdeltaPacket.HittedGameObjectId, out hittedPlayer);
                        hittedPlayer.Hp -= monster.BiteDamage;
                    }
                    break;
                case (int)Define.SkillId.Dragon_Burn:            
                    {
                        Monster monster;
                        _monsters.TryGetValue(hpdeltaPacket.AttackerGameObjectId, out monster);
                        Player hittedPlayer;
                        _players.TryGetValue(hpdeltaPacket.HittedGameObjectId, out hittedPlayer);
                        hittedPlayer.Hp -= monster.BurnDamage;
                    }
                    break;
                case (int)Define.SkillId.Dragon_Fireball:            
                    {
                        Monster monster;
                        _monsters.TryGetValue(hpdeltaPacket.AttackerGameObjectId, out monster);
                        Player hittedPlayer;
                        _players.TryGetValue(hpdeltaPacket.HittedGameObjectId, out hittedPlayer);
                        hittedPlayer.Hp -= monster.FireballDamage;
                    }
                    break;
                case (int)Define.SkillId.Dragon_Thunder:         
                    {
                        Monster monster;
                        _monsters.TryGetValue(hpdeltaPacket.AttackerGameObjectId, out monster);
                        Player hittedPlayer;
                        _players.TryGetValue(hpdeltaPacket.HittedGameObjectId, out hittedPlayer);
                        hittedPlayer.Hp -= monster.ThunderDamage;
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
        public Player SetTarget(int monsterObjectId)     // 몬스터의 타겟 연산
        {
            Monster monster = null;
            if (_monsters.TryGetValue(monsterObjectId, out monster) == false)
            {
                Console.WriteLine("there is no Monster in Server");
                return null;
            }
               
            if (_players.Count == 0)
            {
                Console.WriteLine("there is no Player in Server");
                return null;
            }

            bool _allDeath = true;
            foreach (Player p in _players.Values)       // 플레이어들 모두 Death State면 return null
            {
                if (p.State != CreatureState.Death)
                    _allDeath = false;
            }
            if (_allDeath)
                return null;

            Player TargetPlayer = new Player();
            foreach (Player p in _players.Values)
            {
                if (p.State != CreatureState.Death)
                {
                    // 각 플레이어와 거리 계산 연산효율을 위해 X값만 따짐, packet으로 보내는 x단위 0.05f
                    p.DistanceBetweenMonster = Math.Abs(p.PositionInfo.PosX - monster.PositionInfo.PosX);

                    // 가까울수록 Aggravation 수치 up
                    p.Aggravation = (int)(100 / p.DistanceBetweenMonster);

                    // 난수값 적당히 추가해줘서 무조건 가깝다고 target되지 않게 조정
                    Random rand = new Random();
                    int randomNumber = rand.Next(_players.Count * 20);
                    p.Aggravation += randomNumber;

                    if (p.Aggravation > TargetPlayer.Aggravation)
                        TargetPlayer = p;
                }
            }
            return TargetPlayer;
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
