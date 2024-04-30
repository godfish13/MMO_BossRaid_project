using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Server;

namespace Server.Game
{
    public class CreatureObject
    {
        public GameRoom MyRoom { get; set; } // Object가 입장해있는 GameRoom
        public ClientSession MySession { get; set; }

        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public CreatureInfo CreatureInfo { get; set; } = new CreatureInfo()
        {
            CreatureId = 0,
            StatInfo = new StatInfo(),
            SkillInfo = new SkillInfo(),
            PosInfo = new PositionInfo() { PosX = 0, PosY = 0 }
        };

        public int CreatureId
        {
            get { return CreatureInfo.CreatureId; }
            set { CreatureInfo.CreatureId = value; }
        }
    }
}
