using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Projectile : GameObject
    {
        public Projectile()
        {
            ObjectType = GameObjectType.Projectile;
            PositionInfo = new PositionInfo();
            PositionInfo.LocalScaleX = 1;
            Speed = 0.1f;
        }

        public GameObject Owner { get; set; }
        public int ProjectileType {  get; set; }
        public float Speed { get; set; }

        protected long _nextMoveTick = 0;

        public virtual void Update()
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

        public virtual void UpdateIdle()
        {
            if (_nextMoveTick >= Environment.TickCount64)   // TickCount64 : millisecond 기준
                return;

            long tick = 100;   // speed = 10으로 설정해줬으므로 == 0.01
            _nextMoveTick = Environment.TickCount64 + tick;     // 0.01초당 1칸씩 움직이도록 속도 조정

            PositionInfo.PosX += PositionInfo.LocalScaleX * Speed;

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

        public virtual void UpdateDeath()
        {
            MyRoom.LeaveGame(GameObjectId);
        }
    }   
}
