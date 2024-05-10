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
        public override void Update()
        {
            switch (State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Death:
                    UpdateDeath();
                    break;
            }
        }

        public override void UpdateIdle()
        {
            // 이동 없음
        }

        public override void UpdateDeath()
        {
            base.UpdateDeath();
        }
    }
}
