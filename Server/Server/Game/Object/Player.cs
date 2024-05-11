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

            Aggravation = 0;
            // Stat, Skill Data => GameRoom.EnterGame에서 지정된 ClassId에 따라 초기화
            // PositionInfo => GameRoom.EnterGame에서 초기화
        }

        public SkillInfo SkillInfo
        {
            get { return GameObjectInfo.SkillInfo; }
            set { GameObjectInfo.SkillInfo = value; }
        }

        #region SkillData Property
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

        #region Monster AI requirement
        public float DistanceBetweenMonster { get; set; }     // 몬스터와 player 사이의 x 거리
        public int Aggravation { get; set; }  // 몬스터와의 거리에 따라 가지는 어그로 수치
        #endregion
    }
}
