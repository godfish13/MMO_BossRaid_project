﻿using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Data;

namespace Server.Game
{
    public class GameObject
    {
        public GameRoom MyRoom { get; set; } // Object가 입장해있는 GameRoom
        public ClientSession MySession { get; set; }

        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public GameObjectInfo GameObjectInfo { get; set; } = new GameObjectInfo()
        {
            StatInfo = new StatInfo(),
            SkillInfo = new SkillInfo(),
            MonsterSkillInfo = new MonsterSkillInfo(),
            PositionInfo = new PositionInfo()          
        };

        public int GameObjectId
        {
            get { return GameObjectInfo.GameObjectId; }
            set { GameObjectInfo.GameObjectId = value; }
        }

        public StatInfo StatInfo
        {
            get { return GameObjectInfo.StatInfo; }
            set 
            { 
                GameObjectInfo.StatInfo = value;
                Console.WriteLine($"{GameObjectInfo.StatInfo.Class} : {State}");
            }
        }

        #region StatData Property
        public int ClassId
        {
            get { return StatInfo.ClassId; }
            set { StatInfo.ClassId = value; }
        }

        public string Class
        {
            get { return StatInfo.Class; }
            set { StatInfo.Class = value; }
        }

        public int MaxHp
        {
            get { return StatInfo.MaxHp; }
            set { StatInfo.MaxHp = value; }
        }

        public int Hp       // Hp Set (변동)시 S_Hpdelta 패킷 broadCast
        {
            get { return StatInfo.Hp; }
            set 
            {
                StatInfo.Hp = value;
                S_Hpdelta hpdeltaPacket = new S_Hpdelta();
                hpdeltaPacket.GameObjectId = GameObjectId;
                hpdeltaPacket.ChangedHp = value;
                MyRoom.BroadCast(hpdeltaPacket);
            }
        }

        public float MaxSpeed
        {
            get { return StatInfo.MaxSpeed; }
            set { StatInfo.MaxSpeed = value; }
        }

        public float Acceleration
        {
            get { return StatInfo.Acceleration; }
            set { StatInfo.Acceleration = value; }
        }
        #endregion

        public PositionInfo PositionInfo
        {
            get { return GameObjectInfo.PositionInfo; }
            set { GameObjectInfo.PositionInfo = value; }
        }

        public CreatureState State
        {
            get { return GameObjectInfo.PositionInfo.State; }
            set { GameObjectInfo.PositionInfo.State = value; }
        }
    }
}
