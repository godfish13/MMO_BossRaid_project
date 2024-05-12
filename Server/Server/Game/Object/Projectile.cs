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

            long tick = 1000;     // 0.1초에 1회 PositionInfo.PosX 변화
            _nextMoveTick = Environment.TickCount64 + tick;    

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
