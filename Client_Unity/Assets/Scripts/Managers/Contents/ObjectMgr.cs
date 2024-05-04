using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMgr : MonoBehaviour
{
    public MyHumanCtrl MyHumanCtrl { get; set; }    // MyHumanCtrl은 접근하기 편하게 따로 빼둠
    Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> _monsters = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> _projectiles = new Dictionary<int, GameObject>();

    public void AddCreature(GameObjectInfo gameObjectInfo, bool myCtrl = false) // MyCtrl : 내가 조종하는지 아닌지 체크
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

                MyHumanCtrl.SyncPos();  // 서버상 위치와 유니티상 위치 동기화
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
                humanCtrl.PositionInfo = gameObjectInfo.PositionInfo;
               
                humanCtrl.SyncPos();        // 서버상 위치와 유니티상 위치 동기화
            }
        }
        else if (type == GameObjectType.Monster)
        {
            // Todo
        }      
    }

    public void AddProjectile(GameObjectInfo gameObjectInfo, GameObjectInfo ownerObjectInfo) // MyCtrl : 내가 조종하는지 아닌지 체크
    {
        GameObjectType type = GetGameObjectTypebyId(gameObjectInfo.ObjectId);

        if (type != GameObjectType.Projectile)
            return;
        
        if (ownerObjectInfo.ObjectId == MyHumanCtrl.GameObjectId)
        {
            Debug.Log("My projectile added");
            GameObject Bomb = Managers.resourceMgr.Instantiate("Projectiles/MyExplosive");
            Bomb.GetComponent<ProjectileCtrl>().GameObjectId = gameObjectInfo.ObjectId;

            GameObject skillUser;
            if (_players.TryGetValue(ownerObjectInfo.ObjectId, out skillUser))
            {
                Bomb.transform.position = skillUser.transform.position + new Vector3(1.0f * skillUser.transform.localScale.x, 0.5f, 0);
                Bomb.GetComponent<Rigidbody2D>().AddForce((Vector2.up + (Vector2.right * skillUser.transform.localScale.x * 2)).normalized * 400.0f);
                _projectiles.Add(gameObjectInfo.ObjectId, Bomb);
            }     
            else
            {
                Debug.Log("Error) Cant find Skill Owner");
            }
        }
        else
        {
            Debug.Log("Others projectile added");
            GameObject Bomb = Managers.resourceMgr.Instantiate("Projectiles/Explosive");
            Bomb.GetComponent<ProjectileCtrl>().GameObjectId = gameObjectInfo.ObjectId;

            GameObject skillUser;
            if (_players.TryGetValue(ownerObjectInfo.ObjectId, out skillUser))
            {
                Bomb.transform.position = skillUser.transform.position + new Vector3(1.0f * skillUser.transform.localScale.x, 0.5f, 0);
                _projectiles.Add(gameObjectInfo.ObjectId, Bomb);
            }
            else
            {
                Debug.Log("Error) Cant find Skill Owner");
            }
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
        {
            Debug.Log($"Can't find GameObject {id}");
            return;
        }
            
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

    public void Clear()     // 현재는 DisConnect 방법이 강종뿐이니 별의미 없음 그래도 일단 둠
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
