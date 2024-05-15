using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class DragonFireball : Projectile
    {
        public DragonFireball()
        {
            MonsterSkillInfo _monsterSkill = null;
            DataMgr.MonsterSkillDictionary.TryGetValue(key: 10, out _monsterSkill);
            Speed = _monsterSkill.FireballSpeed;
        }

        public override void UpdateDeath()
        {
            //Console.WriteLine("Explosion");
            Projectile ExplosionEffect = ObjectMgr.Instance.Add<DragonFireballExplosion>();
            ExplosionEffect.ProjectileType = (int)Define.ProjectileType.Dragon_FireballExplosion;
            ExplosionEffect.GameObjectInfo.PositionInfo.PosX = PositionInfo.PosX;
            ExplosionEffect.GameObjectInfo.PositionInfo.PosY = 0.3f;
            ExplosionEffect.GameObjectInfo.PositionInfo.LocalScaleX = PositionInfo.LocalScaleX;
            ExplosionEffect.Owner = this;

            MyRoom.EnterGame(ExplosionEffect);  // 폭발 이펙트 재생

            MyRoom.LeaveGame(GameObjectId); // 자신은 제거
            return;
        }
    }
}
