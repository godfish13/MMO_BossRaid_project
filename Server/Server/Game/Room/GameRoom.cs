using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
    public class GameRoom : JobSerializer
    {
        public int RoomId { get; set; }

        //Dictionary<int, Player> _players = new Dictionary<int, Player>(); // 해당 룸에 접속중인 player들
        //Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        //Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();
        
        public void Init()
        {

        }
    }
}
