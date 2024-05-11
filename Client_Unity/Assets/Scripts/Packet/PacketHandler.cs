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

        Managers.objectMgr.AddCreature(enterGamePacket.GameObjectInfo, myCtrl: true);   
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
        Debug.Log("S_LeaveGamePacket activated");
        Debug.Log($"{leaveGamePacket.GameObjectInfo.GameObjectId} leaved");

        Managers.objectMgr.Clear();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)   // 타 오브젝트들이 입장할때 받음
    {
        S_Spawn spawnPacket = packet as S_Spawn;

        foreach (GameObjectInfo gameObjectInfo in spawnPacket.GameObjectInfoList)
        {
            Managers.objectMgr.AddCreature(gameObjectInfo, false);
        }
    }

    public static void S_SpawnProjectileHandler(PacketSession session, IMessage packet) // projectile object 입장용
    {
        S_SpawnProjectile spawnProjectilePacket = packet as S_SpawnProjectile;
        Managers.objectMgr.AddProjectile(spawnProjectilePacket.GameObjectInfo, spawnProjectilePacket.OwnerInfo, spawnProjectilePacket.Speed);       
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;

        foreach (int Id in despawnPacket.GameObjectIdlist)
        {
            Managers.objectMgr.Remove(Id);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;

        GameObject go = Managers.objectMgr.FindGameObjectbyId(movePacket.GameObjectId);
        if (go == null) 
            return;

        GameObjectType type = Managers.objectMgr.GetGameObjectTypebyId(movePacket.GameObjectId);
        if (type == GameObjectType.Player)
        {
            
            if (Managers.objectMgr.MyPlayerBaseCtrl.GameObjectId == movePacket.GameObjectId)  // 자신 PosInfo는 클라이언트상 정보를 따름       
                return;
            
            BaseCtrl baseCtrl = go.GetComponent<BaseCtrl>();
            if (baseCtrl == null)
                return;

            baseCtrl.PositionInfo = movePacket.PositionInfo;
        }
        else if (type == GameObjectType.Monster)
        {
            MonsterCtrl monsterCtrl = go.GetComponent<MonsterCtrl>();
            if (monsterCtrl == null)
                return;

            monsterCtrl.PositionInfo = movePacket.PositionInfo;
        }
        else if (type == GameObjectType.Projectile)
        {
            ProjectileCtrl projectileCtrl = go.GetComponent<ProjectileCtrl>();
            if (projectileCtrl == null)
                return;

            projectileCtrl.PositionInfo = movePacket.PositionInfo;
        }
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        S_Skill skillPacket = packet as S_Skill;

        GameObject skillUser = Managers.objectMgr.FindGameObjectbyId(skillPacket.SkillUserId);

        if (skillUser == null) 
            return;
    }

    public static void S_MonsterTargetHandler(PacketSession session, IMessage packet)
    {
        S_MonsterTarget targetPacket = packet as S_MonsterTarget;

        GameObject monster = Managers.objectMgr.FindGameObjectbyId(targetPacket.MonsterId);
        GameObject target = Managers.objectMgr.FindGameObjectbyId(targetPacket.TargetId);

        if (target == null)
            return;
    }

    public static void S_HpdeltaHandler(PacketSession session, IMessage packet)
    {
        S_Hpdelta hpdeltaPacket = packet as S_Hpdelta;

        Debug.Log($"{hpdeltaPacket.GameObjectId} hp : {hpdeltaPacket.ChangedHp}");
        GameObjectType type = Managers.objectMgr.GetGameObjectTypebyId(hpdeltaPacket.GameObjectId);

        if (type == GameObjectType.Player)
        {
            GameObject player = Managers.objectMgr.FindGameObjectbyId(hpdeltaPacket.GameObjectId);
            player.GetComponent<BaseCtrl>().Hp = hpdeltaPacket.ChangedHp;
        }
        else if (type == GameObjectType.Monster)
        {
            GameObject monster = Managers.objectMgr.FindGameObjectbyId(hpdeltaPacket.GameObjectId);
            monster.GetComponent<MonsterCtrl>().Hp = hpdeltaPacket.ChangedHp;
        }
    }
}
