using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

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
            if (Hp <= 0)
                State = CreatureState.Death;

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
                    UpdateBite();
                    break;
                case CreatureState.Burn:
                    UpdateBurn();
                    break;
                case CreatureState.Fireball:
                    UpdateFireball();
                    break;
                case CreatureState.Thunder:
                    UpdateThunder();
                    break;
                case CreatureState.Death:
                    UpdateDeath();
                    break;
            }
        }

        long _nextTick = 0; // Update로 반복실행중인 AI이므로 Timer대신 Environment.TickCount64로 계산
        private bool BehaveCountTimer(long tickCycle)   // 기본 tick 측정 타이머 // 1ms기준이므로 tickCycle 1000 = 1초
        {
            if (_nextTick > Environment.TickCount64)
                return false;

            _nextTick = Environment.TickCount64 + tickCycle;
            return true;
        }

        bool Flag = false;

        float _enCounterRange = 10.0f;
        private void UpdateAwait()
        {
            if (BehaveCountTimer(1000) == false)
                return;

            _targetPlayer = MyRoom.SetTarget(GameObjectId);     // players 거리측정을 위해 실행

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

        Player _targetPlayer;
        protected virtual void UpdateIdle()
        {
            // 1. TickCycle마다 각 플레이어와 몬스터 간 거리측정, 자신의 거리 / 전체 거리합 = basic Aggravation
            // 2. 0~99 난수 생성 후 basic Aggravation에 합쳐서 가장 높은 Aggravation을 지닌 플레이어가 target
            // 3. target이 근접공격 범위면 Bite, Burn, Thunder 중 1 사용, 범위 밖이면 FireBall(target의 y범위가 일정수치 이하) or Thunder
            // 4. 각 패턴 후딜레이 기다린 후 반복
            if (BehaveCountTimer(1000) == false)
                return;

            _targetPlayer = MyRoom.SetTarget(GameObjectId);
            if (_targetPlayer == null)
            {
                Console.WriteLine($"Monster Cant Find target");
                return;
            }

            Console.WriteLine($"Monster target : {_targetPlayer.GameObjectId}");
            S_Move movePacket = new S_Move();
            movePacket.PositionInfo = new PositionInfo();
            movePacket.GameObjectId = GameObjectId;

            // target 플레이어쪽 바라보기
            if (_targetPlayer.PositionInfo.PosX < PositionInfo.PosX)
                PositionInfo.LocalScaleX = -1;
            else
                PositionInfo.LocalScaleX = 1;

            Random rand = new Random();
            int _patternRandom;             // 확률 범위 Json으로 뺄까?

            if (_targetPlayer.DistanceBetweenMonster < 4.0f)    // 근접 공격 범위
            {
                _patternRandom = rand.Next(100);

                if (_patternRandom < 60)
                    State = CreatureState.Bite;
                else if (_patternRandom < 85)
                    State = CreatureState.Burn;
                else
                    State = CreatureState.Thunder;
            }
            else
            {
                _patternRandom = rand.Next(100);

                if (_patternRandom < 70)
                    State = CreatureState.Fireball;
                else
                    State = CreatureState.Thunder;
            }

            movePacket.PositionInfo = PositionInfo;
            MyRoom.BroadCast(movePacket);
        }

        private void UpdateBite()
        {
            if (BehaveCountTimer(5000) == false)
                return;

            State = CreatureState.Idle;

            S_Move movePacket = new S_Move();
            movePacket.PositionInfo = PositionInfo;
            movePacket.GameObjectId = GameObjectId;
            movePacket.PositionInfo.State = State;
            MyRoom.BroadCast(movePacket);
        }

        private void UpdateBurn()
        {
            if (BehaveCountTimer(900) == false)
                return;

            State = CreatureState.Idle;

            S_Move movePacket = new S_Move();
            movePacket.PositionInfo = PositionInfo;
            movePacket.GameObjectId = GameObjectId;
            movePacket.PositionInfo.State = State;
            MyRoom.BroadCast(movePacket);
        }

        private void UpdateFireball()
        {
            if (BehaveCountTimer(2000) == false)
                return;

            State = CreatureState.Idle;

            S_Move movePacket = new S_Move();
            movePacket.PositionInfo = PositionInfo;
            movePacket.GameObjectId = GameObjectId;
            movePacket.PositionInfo.State = State;
            MyRoom.BroadCast(movePacket);
        }

        private void UpdateThunder()
        {
            if (BehaveCountTimer(2000) == false)
                return;

            State = CreatureState.Idle;

            S_Move movePacket = new S_Move();
            movePacket.PositionInfo = PositionInfo;
            movePacket.GameObjectId = GameObjectId;
            movePacket.PositionInfo.State = State;
            MyRoom.BroadCast(movePacket);
        }

        private void UpdateDeath()
        {
            S_Move movePacket = new S_Move();
            movePacket.PositionInfo = PositionInfo;
            movePacket.GameObjectId = GameObjectId;
            movePacket.PositionInfo.State = State;
            MyRoom.BroadCast(movePacket);
        }
        #endregion
    }
}
