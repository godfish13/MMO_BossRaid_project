using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Player : GameObject
    {
        public Player()
        {
            ObjectType = GameObjectType.Player;
            ClassId = 0;                        // 따로 입력하는것 못넣을 시 Client쪽 Multi Build and Run Editor에서 1~4로 순서대로 넣자

            StatInfo _stat = null;                                  // StatJson으로 불러놓은 player정보 넣기
            DataMgr.StatDictionary.TryGetValue(key: StatInfo.ClassId, out _stat); 
            GameObjectInfo.StatInfo.MergeFrom(_stat);

            SkillInfo _skill = null;
            DataMgr.SkillDictionary.TryGetValue(key: StatInfo.ClassId, out _skill);
            GameObjectInfo.SkillInfo.MergeFrom(_skill);

            PositionInfo _posInfo = new PositionInfo() { PosX = 0, PosY = 0, LocalScaleX = 1 };
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
