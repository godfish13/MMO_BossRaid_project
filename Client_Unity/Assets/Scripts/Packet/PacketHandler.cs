using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)   // 자신이 입장할때 받음
    {
        S_EnterGame enterGamePacket = packet as S_EnterGame;

        Debug.Log("S_EnterGameHandler activated");
        Debug.Log($"class : {enterGamePacket.GameObjectInfo.StatInfo.Class} Entered Game");

        Managers.objectMgr.Add(enterGamePacket.GameObjectInfo, myCtrl: true);   
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGamePacket = packet as S_LeaveGame;


        Debug.Log("S_LeaveGamePacket activated");
        Debug.Log(leaveGamePacket.GameObjectInfo.ObjectId);
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)   // 타 오브젝트들이 입장할때 받음
    {
        S_Spawn spawnPacket = packet as S_Spawn;

        foreach (GameObjectInfo gameObjectInfo in spawnPacket.GameObjectInfoList)
        {
            Managers.objectMgr.Add(gameObjectInfo, false);
        }
        Debug.Log("S_SpawnHandler activated");
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;

        Debug.Log("S_DespawnHandler activated");
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;

        GameObject go = Managers.objectMgr.FindGameObjectbyId(movePacket.ObjectId);

        if (go == null)
            return;

        if (Managers.objectMgr.MyHumanCtrl.GameObjectId == movePacket.ObjectId)  // 자신 PosInfo는 클라이언트상 정보를 따름       
            return;
                 
        BaseCtrl baseCtrl = go.GetComponent<BaseCtrl>();
        if (baseCtrl == null)
            return;

        baseCtrl.PositionInfo = movePacket.PositionInfo;
    }
}
