using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMgr : MonoBehaviour
{
    public MyHumanCtrl MyHumanCtrl { get; set; }
    Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> _monsters = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
    // playerId, player

    public static GameObjectType GetGameObjectTypebyId(int ObjectId)
    {
        int type = ObjectId >> 24 & 0x7F;
        return (GameObjectType)type;
    }

    public void Add(CreatureInfo CreatureInfo, bool myCtrl = false) // MyCtrl : 내가 조종하는지 아닌지 체크
    {
        GameObjectType type = GetGameObjectTypebyId(CreatureInfo.CreatureId);

        if (type == GameObjectType.Player)
        {
            if (myCtrl == true)
            {
                GameObject go = Managers.resourceMgr.Instantiate("Character/My_Human_Adventurer");
                go.name = CreatureInfo.StatInfo.Class;
                _players.Add(CreatureInfo.CreatureId, go);

                MyHumanCtrl = go.GetComponent<MyHumanCtrl>();
                MyHumanCtrl.Id = CreatureInfo.CreatureId;
                MyHumanCtrl.PosInfo = CreatureInfo.PosInfo;
                MyHumanCtrl.Stat = CreatureInfo.StatInfo;
                MyHumanCtrl.SkillData = CreatureInfo.SkillInfo;

                MyHumanCtrl.SyncPos();     // 서버상 위치와 유니티상 위치 동기화
            }
            else
            {
                GameObject go = Managers.resourceMgr.Instantiate("Character/Human_Adventurer");
                go.name = CreatureInfo.StatInfo.Class;
                _players.Add(CreatureInfo.CreatureId, go);

                HumanCtrl hc = go.GetComponent<HumanCtrl>();
                hc.Id = CreatureInfo.CreatureId;
                hc.PosInfo = CreatureInfo.PosInfo;
                hc.Stat = CreatureInfo.StatInfo;
                hc.SkillData = CreatureInfo.SkillInfo;

                hc.SyncPos();        // 서버상 위치와 유니티상 위치 동기화
            }
        }
        else if (type == GameObjectType.Monster)
        {
            // Todo
        }
    }

    public static int GetDecimalId(int id)
    {
        int DecimalId = (id) & 0xFFFFFF;
        return DecimalId;
    }

    public void Remove(int Id)
    {
        GameObject go = FindGameObjectbyId(Id);
        if (go == null)
            return;

        _objects.Remove(Id);
        Managers.resourceMgr.Destroy(go);
    }

    public GameObject FindGameObject(Func<GameObject, bool> condition)  // condition : go를 매개변수로, bool을 return하는 람다식
    {
        foreach (GameObject obj in _objects.Values)
        {
            BaseCtrl bc = obj.GetComponent<BaseCtrl>();
            if (bc == null)
                continue;

            if (condition.Invoke(obj))      // 람다식 condition이 true를 리턴한 obj가 있으면 해당 obj return
                return obj;
        }
        return null;
    }

    public GameObject FindGameObjectbyId(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public void Clear()
    {
        foreach (GameObject obj in _objects.Values)
        {
            Managers.resourceMgr.Destroy(obj);
        }
        _objects.Clear();
        MyHumanCtrl = null;
    }
}
