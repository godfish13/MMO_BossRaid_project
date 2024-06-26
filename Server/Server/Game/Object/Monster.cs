﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
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

            MonsterSkillInfo _monsterSkill = null;
            DataMgr.MonsterSkillDictionary.TryGetValue(key: ClassId, out _monsterSkill);
            MonsterSkillInfo.MergeFrom(_monsterSkill);

            PositionInfo = new PositionInfo() { State = CreatureState.Await, PosX = 0, PosY = 0 };  // 플레이어 enCounter 전 대기모드
        }

        Player _targetPlayer;

        public MonsterSkillInfo MonsterSkillInfo
        {
            get { return GameObjectInfo.MonsterSkillInfo; }
            set { GameObjectInfo.MonsterSkillInfo = value; }
        }

        float _enCounterRange = 10.0f;

        #region SkillData Property
        public int BiteDamage
        {
            get { return MonsterSkillInfo.BiteDamage; }
            set { MonsterSkillInfo.BiteDamage = value; }
        }

        public int BiteDelay
        {
            get { return MonsterSkillInfo.BiteDelay; }
            set { MonsterSkillInfo.BiteDelay = value; }
        }

        public int BiteCoolTime
        {
            get { return MonsterSkillInfo.BiteCoolTime; }
            set { MonsterSkillInfo.BiteCoolTime = value; }
        }

        public int BiteFrequency
        {
            get { return MonsterSkillInfo.BiteFrequency; }
            set { MonsterSkillInfo.BiteFrequency = value; }
        }

        public int BurnDamage
        {
            get { return MonsterSkillInfo.BurnDamage; }
            set { MonsterSkillInfo.BurnDamage = value; }
        }

        public int BurnDelay
        {
            get { return MonsterSkillInfo.BurnDelay; }
            set { MonsterSkillInfo.BurnDelay = value; }
        }

        public int BurnCoolTime
        {
            get { return MonsterSkillInfo.BurnCoolTime; }
            set { MonsterSkillInfo.BurnCoolTime = value; }
        }

        public int BurnFrequency
        {
            get { return MonsterSkillInfo.BurnFrequency; }
            set { MonsterSkillInfo.BurnFrequency = value; }
        }

        public int FireballDamage
        {
            get { return MonsterSkillInfo.FireballDamage; }
            set { MonsterSkillInfo.FireballDamage = value; }
        }

        public int FireballDelay
        {
            get { return MonsterSkillInfo.FireballDelay; }
            set { MonsterSkillInfo.FireballDelay = value; }
        }

        public int FireballCoolTime
        {
            get { return MonsterSkillInfo.FireballCoolTime; }
            set { MonsterSkillInfo.FireballCoolTime = value; }
        }

        public int FireballInstantiateTimingOffset
        {
            get { return MonsterSkillInfo.FireballInstantiateTimingOffset; }
            set { MonsterSkillInfo.FireballInstantiateTimingOffset = value; }
        }

        public int FireballFrequency
        {
            get { return MonsterSkillInfo.FireballFrequency; }
            set { MonsterSkillInfo.FireballFrequency = value; }
        }

        public int ThunderDamage
        {
            get { return MonsterSkillInfo.ThunderDamage; }
            set { MonsterSkillInfo.ThunderDamage = value; }
        }

        public int ThunderDelay
        {
            get { return MonsterSkillInfo.ThunderDelay; }
            set { MonsterSkillInfo.ThunderDelay = value; }
        }

        public int ThunderCoolTime
        {
            get { return MonsterSkillInfo.ThunderCoolTime; }
            set { MonsterSkillInfo.ThunderCoolTime = value; }
        }

        public int ThunderFrequency
        {
            get { return MonsterSkillInfo.ThunderFrequency; }
            set { MonsterSkillInfo.ThunderFrequency = value; }
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

        long _nextTick = 0; // Update로 반복실행중인 AI이므로 Timer대신 Environment.TickCount64로 계산(Timer 사용 시 Update주기마다 실행되는걸 따져줘야하므로 귀찮음)
        private bool BehaveCountTimer(long tickCycle)   // 기본 tick 측정 타이머 // 1ms기준이므로 tickCycle 1000 = 1초
        {
            if (_nextTick > Environment.TickCount64)
                return false;

            _nextTick = Environment.TickCount64 + tickCycle;
            return true;
        }
       
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

            //Console.WriteLine($"Monster target : {_targetPlayer.GameObjectId}");
            S_Move movePacket = new S_Move();
            movePacket.PositionInfo = new PositionInfo();
            movePacket.GameObjectId = GameObjectId;

            // target 플레이어쪽 바라보기
            if (_targetPlayer.PositionInfo.PosX < PositionInfo.PosX)
                PositionInfo.LocalScaleX = -1;
            else
                PositionInfo.LocalScaleX = 1;

            Random rand = new Random();
            int _patternRandom;             

            // ~~Delay : State 유지 시간
            // ~~CoolTime : 패턴 종료 후 Idle 유지시간
            if (_targetPlayer.DistanceBetweenMonster < 4.0f)    // 근접 공격 범위
            {
                _patternRandom = rand.Next(100);

                if (_targetPlayer.PositionInfo.PosY < 3.0f)
                {
                    if (_patternRandom < BiteFrequency)
                    {
                        _nextTick = Environment.TickCount64 + BiteDelay;  // State 변경되자마자 UpdateBite 내 1회 실행되는것을 방지하기 위해 미리 한번 늘려줌
                        State = CreatureState.Bite;
                    }
                    else if (_patternRandom < BiteFrequency + BurnFrequency)
                    {
                        _nextTick = Environment.TickCount64 + BurnDelay;
                        State = CreatureState.Burn;
                    }
                    else
                    {
                        _spawnProjectileOnce = true;
                        _nextTick = Environment.TickCount64 + ThunderDelay;
                        State = CreatureState.Thunder;
                    }
                }
                else
                {
                    _spawnProjectileOnce = true;
                    _nextTick = Environment.TickCount64 + ThunderDelay;
                    State = CreatureState.Thunder;
                }
            }
            else
            {
                _patternRandom = rand.Next(100);

                if (_patternRandom < FireballFrequency && _targetPlayer.PositionInfo.PosY < 3.0)
                {
                    _spawnProjectileOnce = true;
                    _nextTick = Environment.TickCount64 + FireballInstantiateTimingOffset;  // State변경된 후 투사체 생성까지 걸리는 시간
                    State = CreatureState.Fireball;
                }
                else
                {
                    _spawnProjectileOnce = true;
                    _nextTick = Environment.TickCount64 + ThunderDelay;
                    State = CreatureState.Thunder;
                }
            }

            movePacket.PositionInfo = PositionInfo;
            MyRoom.BroadCast(movePacket);
        }

        private void UpdateBite()
        {
            if (BehaveCountTimer(BiteCoolTime) == false)     // 패턴 종료 후 딜레이(Idle 상태) 유지 시간
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
            if (BehaveCountTimer(BurnCoolTime) == false)
                return;

            State = CreatureState.Idle;

            S_Move movePacket = new S_Move();
            movePacket.PositionInfo = PositionInfo;
            movePacket.GameObjectId = GameObjectId;
            movePacket.PositionInfo.State = State;
            MyRoom.BroadCast(movePacket);
        }

        private bool _spawnProjectileOnce = true;
        private void UpdateFireball()
        {
            if (_spawnProjectileOnce == true)   // 투사체 1회만 생성
            {
                if (BehaveCountTimer(FireballCoolTime) == true)     // Fireball 투사체 브레스 모션에 맞춰 생성하기 위해 한번 돌려줌
                {
                    DragonFireball Fireball = ObjectMgr.Instance.Add<DragonFireball>();
                    if (Fireball == null)
                        return;
                    Fireball.Owner = this;
                    Fireball.ProjectileType = (int)Define.ProjectileType.Dragon_Fireball;
                    Fireball.GameObjectInfo.PositionInfo.PosX = 2.3f * PositionInfo.LocalScaleX;
                    Fireball.GameObjectInfo.PositionInfo.PosY = 0;
                    Fireball.GameObjectInfo.PositionInfo.LocalScaleX = PositionInfo.LocalScaleX;

                    MyRoom.EnterGame(Fireball);
                    _spawnProjectileOnce = false;

                    _nextTick = Environment.TickCount64 + FireballDelay;    // FireballState 유지
                }
            }

            if (BehaveCountTimer(FireballCoolTime) == false)    // FireBall 후딜레이
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
            if (_spawnProjectileOnce)
            {
                foreach (Player p in MyRoom._players.Values)
                {
                    DragonThunder Thunder = ObjectMgr.Instance.Add<DragonThunder>();
                    if (Thunder == null)
                        return;
                    Thunder.Owner = this;
                    Thunder.ProjectileType = (int)Define.ProjectileType.Dragon_Thunder;
                    Thunder.GameObjectInfo.PositionInfo.PosX = p.PositionInfo.PosX;
                    Thunder.GameObjectInfo.PositionInfo.PosY = p.PositionInfo.PosY;
                    Thunder.GameObjectInfo.PositionInfo.LocalScaleX = 1;

                    MyRoom.EnterGame(Thunder);
                }

                _spawnProjectileOnce = false;
                _nextTick = Environment.TickCount64 + ThunderDelay;    // ThunderState 유지
            }

            if (BehaveCountTimer(ThunderCoolTime) == false)
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
