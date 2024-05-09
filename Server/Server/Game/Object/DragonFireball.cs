using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class DragonFireball : Projectile
    {
        public override void Update()
        {
            switch (State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Explosion:
                    UpdateExplosion();
                    break;
            }
        }

        public override void UpdateIdle()
        {
            if (_nextMoveTick >= Environment.TickCount64)   // TickCount64 : millisecond 기준
                return;

            long tick = 100;
            _nextMoveTick = Environment.TickCount64 + tick;     // 0.1초당 움직임

            PositionInfo.PosX += PositionInfo.LocalScaleX * 0.5f;

            S_Move movePacket = new S_Move();
            movePacket.PositionInfo = new PositionInfo();
            movePacket.GameObjectId = GameObjectId;
            movePacket.PositionInfo.State = CreatureState.Idle;
            movePacket.PositionInfo.PosX = PositionInfo.PosX;
            movePacket.PositionInfo.PosY = PositionInfo.PosY;
            movePacket.PositionInfo.LocalScaleX = PositionInfo.LocalScaleX;

            MyRoom.BroadCast(movePacket);

            // 피격 판정은 Client의 HitBox로 
        }

        public void UpdateExplosion()
        {
            //Console.WriteLine("Explosion");
            Projectile ExplosionEffect = ObjectMgr.Instance.Add<Projectile>();
            ExplosionEffect.ProjectileType = (int)Define.ProjectileType.DragonFireballExplosion;
            ExplosionEffect.GameObjectInfo.PositionInfo.PosX = PositionInfo.PosX;
            ExplosionEffect.GameObjectInfo.PositionInfo.PosY = 0;
            ExplosionEffect.GameObjectInfo.PositionInfo.LocalScaleX = PositionInfo.LocalScaleX;
            ExplosionEffect.Owner = this;

            MyRoom.EnterGame(ExplosionEffect);  // 폭발 이펙트 재생

            MyRoom.LeaveGame(GameObjectId); // 자신은 제거
            return;
        }
    }
}
