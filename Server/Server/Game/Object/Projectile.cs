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
        }

        public GameObject Owner { get; set; }
    }   
}
