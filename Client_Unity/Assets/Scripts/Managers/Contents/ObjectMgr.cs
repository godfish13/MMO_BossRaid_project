using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMgr : MonoBehaviour
{
    public MyHumanCtrl MyHumanCtrl { get; set; }    // MyHumanCtrl�� �����ϱ� ���ϰ� ���� ����
    Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> _monsters = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> _projectiles = new Dictionary<int, GameObject>();
    // playerId, player

    public void Add(GameObjectInfo gameObjectInfo, bool myCtrl = false) // MyCtrl : ���� �����ϴ��� �ƴ��� üũ
    {
        GameObjectType type = GetGameObjectTypebyId(gameObjectInfo.ObjectId);

        if (type == GameObjectType.Player)
        {
            if (myCtrl == true)
            {
                GameObject go = Managers.resourceMgr.Instantiate("Character/My_Human_Adventurer");
                go.name = gameObjectInfo.StatInfo.Class;
                _players.Add(gameObjectInfo.ObjectId, go);

                MyHumanCtrl = go.GetComponent<MyHumanCtrl>();
                MyHumanCtrl.GameObjectId = gameObjectInfo.ObjectId;
                MyHumanCtrl.PositionInfo = gameObjectInfo.PositionInfo;
                MyHumanCtrl.Stat = gameObjectInfo.StatInfo;
                MyHumanCtrl.SkillData = gameObjectInfo.SkillInfo;
            }
            else
            {
                GameObject go = Managers.resourceMgr.Instantiate("Character/Human_Adventurer");
                go.name = gameObjectInfo.StatInfo.Class;
                _players.Add(gameObjectInfo.ObjectId, go);

                HumanCtrl humanCtrl = go.GetComponent<HumanCtrl>();
                humanCtrl.GameObjectId = gameObjectInfo.ObjectId;
                humanCtrl.PositionInfo = gameObjectInfo.PositionInfo;
                humanCtrl.Stat = gameObjectInfo.StatInfo;
                humanCtrl.SkillData = gameObjectInfo.SkillInfo;

                humanCtrl.SyncPos();        // ������ ��ġ�� ����Ƽ�� ��ġ ����ȭ
            }
        }
        else if (type == GameObjectType.Monster)
        {
            // Todo
        }
        else if (type == GameObjectType.Projectile)
        {
            // Todo
        }
    }

    public GameObjectType GetGameObjectTypebyId(int ObjectId)
    {
        int type = ObjectId >> 24 & 0x7F;
        return (GameObjectType)type;
    }

    public static int GetDecimalId(int id)
    {
        int DecimalId = (id) & 0xFFFFFF;
        return DecimalId;
    }

    public void Remove(int id)
    {
        GameObject go = FindGameObjectbyId(id);

        if (go == null)
            return;

        GameObjectType gameObjectType = Managers.objectMgr.GetGameObjectTypebyId(id);

        if (gameObjectType == GameObjectType.Player)
        {
            _players.Remove(id);
        }
        else if (gameObjectType == GameObjectType.Monster)
        {
            _monsters.Remove(id);
        }
        else if (gameObjectType == GameObjectType.Projectile)
        {
            _projectiles.Remove(id);
        }    
        
        Managers.resourceMgr.Destroy(go);
    }

    /*public GameObject FindGameObject(Func<GameObject, bool> condition)  // condition : go�� �Ű�������, bool�� return�ϴ� ���ٽ�
    {
        foreach (GameObject obj in _objects.Values)
        {
            BaseCtrl bc = obj.GetComponent<BaseCtrl>();
            if (bc == null)
                continue;

            if (condition.Invoke(obj))      // ���ٽ� condition�� true�� ������ obj�� ������ �ش� obj return
                return obj;
        }
        return null;
    }*/

    public GameObject FindGameObjectbyId(int id)
    {
        GameObjectType gameObjectType = Managers.objectMgr.GetGameObjectTypebyId(id);

        GameObject go = null;

        if (gameObjectType == GameObjectType.Player)
        {
            _players.TryGetValue(id, out go);
        }
        else if (gameObjectType == GameObjectType.Monster)
        {
            _monsters.TryGetValue(id, out go);
        }
        else if (gameObjectType == GameObjectType.Projectile)
        {
            _projectiles.TryGetValue(id, out go);
        }      
        
        return go;
    }

    public void Clear()
    {
        foreach (GameObject obj in _players.Values)
        {
            Managers.resourceMgr.Destroy(obj);
        }
        _players.Clear();

        foreach (GameObject obj in _monsters.Values)
        {
            Managers.resourceMgr.Destroy(obj);
        }
        _monsters.Clear();

        foreach (GameObject obj in _projectiles.Values)
        {
            Managers.resourceMgr.Destroy(obj);
        }
        _projectiles.Clear();

        MyHumanCtrl = null;
    }
}
