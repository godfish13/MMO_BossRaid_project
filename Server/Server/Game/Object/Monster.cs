using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Monster : GameObject
    {
        public string MonsterName;

        public Monster()
        {
            ObjectType = GameObjectType.Monster;
            ClassId = 10;                        // 일단 하드코딩...   10 == Dragon

            StatInfo _stat = null;                                  // Json으로 불러놓은 Monster Data 넣기
            DataMgr.StatDictionary.TryGetValue(key: ClassId, out _stat);
            StatInfo.MergeFrom(_stat);

            SkillInfo _skill = null;
            DataMgr.SkillDictionary.TryGetValue(key: ClassId, out _skill);
            SkillInfo.MergeFrom(_skill);

            PositionInfo = new PositionInfo() { State = CreatureState.Await, PosX = 0, PosY = 0 };  // 플레이어 enCounter 전 대기모드
        }    

        #region SkillData Property
        public int BiteDamage
        {
            get { return SkillInfo.BiteDamage; }
            set { SkillInfo.BiteDamage = value; }
        }

        public float BiteCoolTime
        {
            get { return SkillInfo.BiteCoolTime; }
            set { SkillInfo.BiteCoolTime = value; }
        }

        public int BurnDamage
        {
            get { return SkillInfo.BurnDamage; }
            set { SkillInfo.BurnDamage = value; }
        }

        public float BurnCoolTime
        {
            get { return SkillInfo.BurnCoolTime; }
            set { SkillInfo.BurnCoolTime = value; }
        }

        public int FireBallDamage
        {
            get { return SkillInfo.FireBallDamage; }
            set { SkillInfo.FireBallDamage = value; }
        }

        public float FireBallCoolTime
        {
            get { return SkillInfo.FireBallCoolTime; }
            set { SkillInfo.FireBallCoolTime = value; }
        }

        public int ThunderDamage
        {
            get { return SkillInfo.ThunderDamage; }
            set { SkillInfo.ThunderDamage = value; }
        }

        public float ThunderCoolTime
        {
            get { return SkillInfo.ThunderCoolTime; }
            set { SkillInfo.ThunderCoolTime = value; }
        }
        #endregion

        #region Monster Ai
        // FSM 방식 AI
        public void Update()
        {
            switch (State)
            {
                case CreatureState.Await:
                    UpdateAwait();
                    break;
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Run:
                    break;
                case CreatureState.Bite:
                    break;
                case CreatureState.Burn:
                    break;
                case CreatureState.Fireball:
                    break;
                case CreatureState.Thunder:
                    break;
                case CreatureState.Death:
                    break;
            }
        }

        long _nextTick = 0;
        private bool BehaveCountTimer(long tickCycle)   // 기본 tick 측정 타이머 // 1ms기준이므로 tickCycle 1000 = 1초
        {
            if (_nextTick > Environment.TickCount64)
                return false;
            _nextTick = Environment.TickCount64 + tickCycle; 

            MyRoom.DistanceCalculater(GameObjectId);    // 1사이클마다 플레이어들과의 거리계산, Aggravation 수치 입력

            return true;
        }

        float _enCounterRange = 10.0f;
        private void UpdateAwait()
        {
            if (BehaveCountTimer(1000) == false)
                return;

            if (MyRoom._players.Count == 0) 
                return;

            foreach (Player p in MyRoom._players.Values)
            {
                if (p.DistanceBetweenMonster < _enCounterRange)
                {
                    State = CreatureState.Idle;

                    S_Move movePacket = new S_Move();
                    movePacket.PositionInfo = new PositionInfo();

                    movePacket.GameObjectId = GameObjectId;
                    movePacket.PositionInfo.State = State;
                    movePacket.PositionInfo.LocalScaleX = -1;
                    MyRoom.BroadCast(movePacket);
                }
            }
            Console.WriteLine($"{StatInfo.Class} is awaiting");
        }

        int _targetID; // 일단은 참조값으로 들고있기 / target 찾으면 해당 플레이어의 id를 대신 갖고있는거도 괜춘할듯

        int _searchDist = 15;
        protected virtual void UpdateIdle()
        {
            // 1. TickCycle마다 각 플레이어와 몬스터 간 거리측정, 자신의 거리 / 전체 거리합 = basic Aggravation
            // 2. 0~99 난수 생성 후 basic Aggravation에 합쳐서 가장 높은 Aggravation을 지닌 플레이어가 target
            // 3. target이 근접공격 범위면 Bite, Burn, Thunder 중 1 사용, 범위 밖이면 FireBall(target의 y범위가 일정수치 이하) or Thunder
            // 4. 각 패턴 후딜레이 기다린 후 반복
            if (BehaveCountTimer(1000) == false)
                return;

            MyRoom.SetTarget();

            //Console.WriteLine($"monster PositionInfo : {PositionInfo.PosX}, {PositionInfo.PosY}");
            Player target = MyRoom.FindPlayer(p =>
            {
                return p.DistanceBetweenMonster < _searchDist;
            });

            if (target == null)
            {
                Console.WriteLine($"Monster Cant Find target");
                return;
            }

            _targetID = target.GameObjectId;
            Console.WriteLine($"Monster target : {_targetID}");

            //S_MonsterTarget targetPacket = new S_MonsterTarget();
            //targetPacket.MonsterId = ObjectId;
            //targetPacket.TargetId = _target.ObjectId;
            //MySession.Send(targetPacket);
        }
        #endregion
    }
}
