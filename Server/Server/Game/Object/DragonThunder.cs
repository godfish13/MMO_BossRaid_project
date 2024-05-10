using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class DragonThunder : Projectile
    {
        public DragonThunder()
        {
            MonsterSkillInfo _monsterSkill = null;
            DataMgr.MonsterSkillDictionary.TryGetValue(key: 10, out _monsterSkill);
        }

        public override void Update()
        {
            switch (State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
            }
        }

        public override void UpdateIdle()
        {


            // 피격 판정은 Client의 HitBox로 
        }

    }
}
