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
        }

        public GameObject Owner { get; set; }
        public int ProjectileType {  get; set; }

        long _nextMoveTick = 0;
        public void Update()
        {
            if (_nextMoveTick >= Environment.TickCount64)   // TickCount64 : millisecond 기준
                return;

            long tick = 1000;   // speed = 10으로 설정해줬으므로 == 0.01
            _nextMoveTick = Environment.TickCount64 + tick;     // 0.01초당 1칸씩 움직이도록 속도 조정


            //S_Move movePacket = new S_Move();
            //movePacket.PositionInfo = new PositionInfo();
            //movePacket.GameObjectId = GameObjectId;
            //movePacket.PositionInfo.PosX = ProjectileInfo.PositionInfo.PosX + 0.5f;
            if (PositionInfo == null)
            {
                Console.WriteLine("잘못됐어");
                return;
            }
            PositionInfo.PosX += 0.5f;
            Console.WriteLine($"쓩 {PositionInfo.PosX}");
            //MyRoom.BroadCast(movePacket);

            //Console.WriteLine("Arrow moving");
            
            //피격
            //MyRoom.Push(MyRoom.LeaveGame, Id);
        }
    }   
}
