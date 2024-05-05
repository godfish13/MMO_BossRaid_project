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

            StatInfo _stat = null;                                 
            DataMgr.StatDictionary.TryGetValue(key: ClassId, out _stat);
            GameObjectInfo.StatInfo.MergeFrom(_stat);

            SkillInfo _skill = null;
            DataMgr.SkillDictionary.TryGetValue(key: ClassId, out _skill);
            GameObjectInfo.SkillInfo.MergeFrom(_skill);

            PositionInfo = new PositionInfo() { State = CreatureState.Await, PosX = 0, PosY = 0 };  // 플레이어 enCounter 전 대기모드
        }

        public int ClassId
        {
            get { return StatInfo.ClassId; }
            set { StatInfo.ClassId = value; }
        }

        public string Class
        {
            get { return StatInfo.Class; }
            set { StatInfo.Class = value; }
        }

        #region Stat
        public int MaxHp
        {
            get { return StatInfo.MaxHp; }
            set { StatInfo.MaxHp = value; }
        }

        public int Hp
        {
            get { return StatInfo.Hp; }
            set { StatInfo.Hp = value; }
        }

        public float MaxSpeed
        {
            get { return StatInfo.MaxSpeed; }
            set { StatInfo.MaxSpeed = value; }
        }

        public float Acceleration
        {
            get { return StatInfo.Acceleration; }
            set { StatInfo.Acceleration = value; }
        }
        #endregion

        #region Skill
        public int SkillDamage
        {
            get { return SkillInfo.SkillDamage; }
            set { SkillInfo.SkillDamage = value; }
        }

        public float SkillCoolTime
        {
            get { return SkillInfo.SkillCoolTime; }
            set { SkillInfo.SkillCoolTime = value; }
        }

        public int SubSkillDamage
        {
            get { return SkillInfo.SubSkillDamage; }
            set { SkillInfo.SubSkillDamage = value; }
        }

        public float SubSkillCoolTime
        {
            get { return SkillInfo.SubSkillCoolTime; }
            set { SkillInfo.SubSkillCoolTime = value; }
        }

        public float JumpPower
        {
            get { return SkillInfo.JumpPower; }
            set { SkillInfo.JumpPower = value; }
        }

        public float JumpCoolTime
        {
            get { return SkillInfo.JumpCoolTime; }
            set { SkillInfo.JumpCoolTime = value; }
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

        Player _target; // 일단은 참조값으로 들고있기 / target 찾으면 해당 플레이어의 id를 대신 갖고있는거도 괜춘할듯

        int _searchDist = 10;
        long _nextSearchTick = 0;   // 탐색 주기 틱
        protected virtual void UpdateAwait()
        {          
            if (_nextSearchTick > Environment.TickCount64)  // 1초에 한번 작동
                return;
            _nextSearchTick = Environment.TickCount64 + 1000;   // 1ms기준이므로 1000 = 1초

            Console.WriteLine($"monster PositionInfo : {PositionInfo.PosX}, {PositionInfo.PosY}");
            Player target = MyRoom.FindPlayer(p =>
            {
                float DistancePlayerMonster = Math.Abs(p.PositionInfo.PosX - PositionInfo.PosX);
                return DistancePlayerMonster < _searchDist;
            });

            if (target == null)
            {
                Console.WriteLine($"Monster Cant Find target");
                return;
            }
                
            _target = target;
            Console.WriteLine($"Monster target setted : {_target.ObjectId}");

            //S_MonsterTarget targetPacket = new S_MonsterTarget();
            //targetPacket.MonsterId = ObjectId;
            //targetPacket.TargetId = _target.ObjectId;
            //MySession.Send(targetPacket);
        }

        #endregion
    }
}
