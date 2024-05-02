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
        public Monster()
        {
            ObjectType = GameObjectType.Monster;
            ClassId = 10;                        // 일단 하드코딩...   10 == Dragon

            StatInfo _stat = null;                                 
            DataMgr.StatDictionary.TryGetValue(key: StatInfo.ClassId, out _stat);
            GameObjectInfo.StatInfo.MergeFrom(_stat);

            SkillInfo _skill = null;
            DataMgr.SkillDictionary.TryGetValue(key: StatInfo.ClassId, out _skill);
            GameObjectInfo.SkillInfo.MergeFrom(_skill);

            PositionInfo _posInfo = new PositionInfo() { PosX = 0, PosY = 0 };
        }

        public StatInfo StatInfo = new StatInfo();      // Json Data Loading용
        public SkillInfo SkillInfo = new SkillInfo();

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
    }
}
