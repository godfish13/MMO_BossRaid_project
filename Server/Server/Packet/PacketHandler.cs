﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        C_LeaveGame leavePacket = packet as C_LeaveGame;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.myPlayer;     // clientSession.myPlayer == null 체크는 멀티스레드 환경 상 제대로 작동안할 가능성이 존재함
        if (player == null)             // 이를 방지하기 위해 player로 한번 꺼내고 null체크하면 다른 스레드에서 myPlayer을 건들더라도  
            return;                     // player는 그대로 남아있으므로 비교적 안전해짐

        GameRoom myRoom = player.MyRoom;  // player랑 마찬가지
        if (myRoom == null)               // 얘는 GameRoom.LeaveGame에서 실제로 null로 밀어주기도 함으로 더위험함!! 더 주의해야하는부분
            return;

        myRoom.Push(myRoom.LeaveGame, leavePacket.GameObjectId);
    }

    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        GameObjectType type = ObjectMgr.GetObjectTypebyId(movePacket.GameObjectId);

        if (type == GameObjectType.Player)
        {
            Player player = clientSession.myPlayer;     // clientSession.myPlayer == null 체크는 멀티스레드 환경 상 제대로 작동안할 가능성이 존재함
            if (player == null)             // 이를 방지하기 위해 player로 한번 꺼내고 null체크하면 다른 스레드에서 myPlayer을 건들더라도  
            {
                Console.WriteLine("NO Player");
                return;                     // player는 그대로 남아있으므로 비교적 안전해짐
            }

            GameRoom myRoom = player.MyRoom;  // player랑 마찬가지
            if (myRoom == null)               // 얘는 GameRoom.LeaveGame에서 실제로 null로 밀어주기도 함으로 더위험함!! 더 주의해야하는부분
            {
                Console.WriteLine("NO GameRoom");
                return;
            }

            //Todo 검증(클라이언트가 잘못된 정보 보내는지?)

            myRoom.Push(myRoom.HandleMove, player, movePacket);
        }
        else if (type == GameObjectType.Projectile)
        {
            Player player = clientSession.myPlayer;
            if (player == null) 
            {
                Console.WriteLine("NO Player");
                return;
            }

            GameRoom myRoom = player.MyRoom;
            if (myRoom == null) 
            {
                Console.WriteLine("NO GameRoom");
                return;
            }

            GameObject projectile = myRoom.FindGameObjectWithId(movePacket.GameObjectId);
            myRoom.Push(myRoom.HandleMove, projectile, movePacket);
        }
    }

    public static void C_SkillHandler(PacketSession session, IMessage packet) 
    {
        C_Skill skillPacket = packet as C_Skill;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.myPlayer;     // clientSession.myPlayer == null 체크는 멀티스레드 환경 상 제대로 작동안할 가능성이 존재함
        if (player == null)             // 이를 방지하기 위해 player로 한번 꺼내고 null체크하면 다른 스레드에서 myPlayer을 건들더라도  
            return;                     // player는 그대로 남아있으므로 비교적 안전해짐

        GameRoom myRoom = player.MyRoom;  // player랑 마찬가지
        if (myRoom == null)               // 얘는 GameRoom.LeaveGame에서 실제로 null로 밀어주기도 함으로 더위험함!! 더 주의해야하는부분
            return;

        myRoom.Push(myRoom.HandleSkill, player, skillPacket);
    }

    public static void C_HpdeltaHandler(PacketSession session, IMessage packet)
    {
        C_Hpdelta hpdeltaPacket = packet as C_Hpdelta;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.myPlayer;     // clientSession.myPlayer == null 체크는 멀티스레드 환경 상 제대로 작동안할 가능성이 존재함
        if (player == null)             // 이를 방지하기 위해 player로 한번 꺼내고 null체크하면 다른 스레드에서 myPlayer을 건들더라도  
            return;                     // player는 그대로 남아있으므로 비교적 안전해짐

        GameRoom myRoom = player.MyRoom;  // player랑 마찬가지
        if (myRoom == null)               // 얘는 GameRoom.LeaveGame에서 실제로 null로 밀어주기도 함으로 더위험함!! 더 주의해야하는부분
            return;

        myRoom.Push(myRoom.HandleHp, player, hpdeltaPacket);
    }

    public static void C_SelectCharacterHandler(PacketSession session, IMessage packet)
    {
        C_SelectCharacter selectCharacterPacket = packet as C_SelectCharacter;
        ClientSession clientSession = session as ClientSession;
    }
}