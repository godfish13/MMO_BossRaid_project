using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class ElfArrow : Projectile
    {
        public ElfArrow()
        {
            Speed = 10.0f;
        }

        public override void UpdateDeath()
        {
            //Console.WriteLine("Explosion");
            Projectile HitEffect = ObjectMgr.Instance.Add<ElfArrowHit>();
            HitEffect.ProjectileType = (int)Define.ProjectileType.Elf_ArrowHit;
            HitEffect.GameObjectInfo.PositionInfo.PosX = PositionInfo.PosX;
            HitEffect.GameObjectInfo.PositionInfo.PosY = 0.5f;
            HitEffect.GameObjectInfo.PositionInfo.LocalScaleX = PositionInfo.LocalScaleX;
            HitEffect.Owner = this;

            MyRoom.EnterGame(HitEffect);  // 폭발 이펙트 재생

            MyRoom.LeaveGame(GameObjectId); // 자신은 제거
            return;
        }
    }
}
