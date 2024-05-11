using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMgr : MonoBehaviour
{
    public MyPlayerBaseCtrl MyPlayerBaseCtrl { get; set; }  // MyHumanCtrl�� �����ϱ� ���ϰ� ���� ����

    public MonsterCtrl MonsterCtrl { get; set; }
    Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> _monsters = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> _projectiles = new Dictionary<int, GameObject>();

    public void AddCreature(GameObjectInfo gameObjectInfo, bool myCtrl = false) // MyCtrl : ���� �����ϴ��� �ƴ��� üũ
    {
        GameObjectType type = GetGameObjectTypebyId(gameObjectInfo.GameObjectId);

        if (type == GameObjectType.Player)
        {
            switch (gameObjectInfo.StatInfo.ClassId)
            {
                case 0:
                    AddPlayer(gameObjectInfo, "Human_Adventurer", myCtrl);
                    break;
                case 1:
                    AddPlayer(gameObjectInfo, "Elf_Archer", myCtrl);
                    break;
                case 2:
                    AddPlayer(gameObjectInfo, "Furry_Knight", myCtrl);
                    break;
            }
        }
        else if (type == GameObjectType.Monster)
        {
            GameObject go = Managers.resourceMgr.Instantiate("Monster/Dragon");
            go.name = gameObjectInfo.StatInfo.Class;
            _monsters.Add(gameObjectInfo.GameObjectId, go);

            MonsterCtrl = go.GetComponent<MonsterCtrl>();
            MonsterCtrl.GameObjectId = gameObjectInfo.GameObjectId;
            MonsterCtrl.ClassId = gameObjectInfo.StatInfo.ClassId;
            MonsterCtrl.PositionInfo = gameObjectInfo.PositionInfo;
            MonsterCtrl.StatData = gameObjectInfo.StatInfo;
            MonsterCtrl.SkillData = gameObjectInfo.SkillInfo;
        }      
    }

    public void AddPlayer(GameObjectInfo gameObjectInfo, string className, bool myCtrl)
    {
        if (myCtrl == true)
        {
            switch (className)
            {
                case "Human_Adventurer":
                    {
                        GameObject go = Managers.resourceMgr.Instantiate($"Character/My_{className}");
                        _players.Add(gameObjectInfo.GameObjectId, go);

                        MyPlayerBaseCtrl = go.GetComponent<MyPlayerBaseCtrl>();
                        MyPlayerBaseCtrl.ClassId = gameObjectInfo.StatInfo.ClassId;
                        MyPlayerBaseCtrl.GameObjectId = gameObjectInfo.GameObjectId;
                        MyPlayerBaseCtrl.PositionInfo = gameObjectInfo.PositionInfo;
                        MyPlayerBaseCtrl.StatData = gameObjectInfo.StatInfo;
                        MyPlayerBaseCtrl.SkillData = gameObjectInfo.SkillInfo;
                        go.name = MyPlayerBaseCtrl.StatData.Class;

                        MyPlayerBaseCtrl.SyncPos();  // ������ ��ġ�� ����Ƽ�� ��ġ ����ȭ
                    }
                    break;
                case "Elf_Archer":
                    {
                        GameObject go = Managers.resourceMgr.Instantiate($"Character/My_{className}");
                        _players.Add(gameObjectInfo.GameObjectId, go);

                        MyPlayerBaseCtrl = go.GetComponent<MyPlayerBaseCtrl>();
                        MyPlayerBaseCtrl.ClassId = gameObjectInfo.StatInfo.ClassId;
                        MyPlayerBaseCtrl.GameObjectId = gameObjectInfo.GameObjectId;
                        MyPlayerBaseCtrl.PositionInfo = gameObjectInfo.PositionInfo;
                        MyPlayerBaseCtrl.StatData = gameObjectInfo.StatInfo;
                        MyPlayerBaseCtrl.SkillData = gameObjectInfo.SkillInfo;
                        go.name = MyPlayerBaseCtrl.StatData.Class;

                        MyPlayerBaseCtrl.SyncPos();  // ������ ��ġ�� ����Ƽ�� ��ġ ����ȭ
                    }
                    break;
                case "Furry_Knight":
                    {
                        GameObject go = Managers.resourceMgr.Instantiate($"Character/My_{className}");
                        _players.Add(gameObjectInfo.GameObjectId, go);

                        MyPlayerBaseCtrl = go.GetComponent<MyPlayerBaseCtrl>();
                        MyPlayerBaseCtrl.ClassId = gameObjectInfo.StatInfo.ClassId;
                        MyPlayerBaseCtrl.GameObjectId = gameObjectInfo.GameObjectId;
                        MyPlayerBaseCtrl.PositionInfo = gameObjectInfo.PositionInfo;
                        MyPlayerBaseCtrl.StatData = gameObjectInfo.StatInfo;
                        MyPlayerBaseCtrl.SkillData = gameObjectInfo.SkillInfo;
                        go.name = MyPlayerBaseCtrl.StatData.Class;

                        MyPlayerBaseCtrl.SyncPos();  // ������ ��ġ�� ����Ƽ�� ��ġ ����ȭ
                    }
                    break;
            }
        }
        else
        {
            GameObject go = Managers.resourceMgr.Instantiate($"Character/{className}");
            _players.Add(gameObjectInfo.GameObjectId, go);

            PlayerCtrl playerCtrl = go.GetComponent<PlayerCtrl>();
            playerCtrl.ClassId = gameObjectInfo.StatInfo.ClassId;
            playerCtrl.GameObjectId = gameObjectInfo.GameObjectId;
            playerCtrl.PositionInfo = gameObjectInfo.PositionInfo;
            playerCtrl.StatData = gameObjectInfo.StatInfo;
            playerCtrl.SkillData = gameObjectInfo.SkillInfo;
            playerCtrl.PositionInfo = gameObjectInfo.PositionInfo;
            playerCtrl.UI_Number = _players.Count - 1;
            go.name = playerCtrl.StatData.Class;

            playerCtrl.SyncPos();        // ������ ��ġ�� ����Ƽ�� ��ġ ����ȭ
        }
    }

    public void AddProjectile(GameObjectInfo gameObjectInfo, GameObjectInfo ownerObjectInfo, float speed) // MyCtrl : ���� �����ϴ��� �ƴ��� üũ
    {
        GameObjectType type = GetGameObjectTypebyId(gameObjectInfo.GameObjectId);

        if (type != GameObjectType.Projectile)
            return;

        switch (gameObjectInfo.ProjectileType)
        {
            case (int)Define.ProjectileType.Human_Bomb:
                BombGenerator(gameObjectInfo, ownerObjectInfo, "Human_Explosive");
                break;
            case (int)Define.ProjectileType.Elf_Arrow:
                ProjectileGenerator(gameObjectInfo, ownerObjectInfo, "Elf_Arrow", speed);
                break;
            case (int)Define.ProjectileType.Elf_ArrowHit:
                ProjectileGenerator(gameObjectInfo, ownerObjectInfo, "Elf_ArrowHit", speed);
                break;
            case (int)Define.ProjectileType.Dragon_Fireball:
                ProjectileGenerator(gameObjectInfo, ownerObjectInfo, "Dragon_Fireball", speed);
                break;
            case (int)Define.ProjectileType.Dragon_FireballExplosion:
                ProjectileGenerator(gameObjectInfo, ownerObjectInfo, "Dragon_FireballExplosion", speed);
                break;
            case (int)Define.ProjectileType.Dragon_Thunder:
                ProjectileGenerator(gameObjectInfo, ownerObjectInfo, "Dragon_Thunder", speed);
                break;
        }
    }

    // AddForce Ȱ���ϴ� case Bomb
    public void BombGenerator(GameObjectInfo gameObjectInfo, GameObjectInfo ownerObjectInfo, string projectilePrefabName)
    {
        if (ownerObjectInfo.GameObjectId == MyPlayerBaseCtrl.GameObjectId)
        {
            Debug.Log("My projectile added");
            GameObject Bomb = Managers.resourceMgr.Instantiate($"Projectiles/My{projectilePrefabName}");
            Bomb.GetComponent<ProjectileCtrl>().GameObjectId = gameObjectInfo.GameObjectId;

            GameObject skillUser;
            if (_players.TryGetValue(ownerObjectInfo.GameObjectId, out skillUser))
            {
                Bomb.transform.position = skillUser.transform.position + new Vector3(0, 0.5f, 0);
                Bomb.GetComponent<Rigidbody2D>().AddForce((Vector2.up + (Vector2.right * skillUser.transform.localScale.x * 2)).normalized * 400.0f);
                _projectiles.Add(gameObjectInfo.GameObjectId, Bomb);
            }
            else
            {
                Debug.Log("Error) Cant find Skill Owner");
            }
        }
        else
        {
            Debug.Log("Others projectile added");
            GameObject Bomb = Managers.resourceMgr.Instantiate($"Projectiles/{projectilePrefabName}");
            Bomb.GetComponent<ProjectileCtrl>().GameObjectId = gameObjectInfo.GameObjectId;

            GameObject skillUser;
            if (_players.TryGetValue(ownerObjectInfo.GameObjectId, out skillUser))
            {
                Bomb.transform.position = skillUser.transform.position + new Vector3(0, 0.5f, 0);
                _projectiles.Add(gameObjectInfo.GameObjectId, Bomb);
            }
            else
            {
                Debug.Log("Error) Cant find Skill Owner");
            }
        }
    }

    public void ProjectileGenerator(GameObjectInfo gameObjectInfo, GameObjectInfo ownerObjectInfo, string projectilePrefabName, float speed)
    {
        GameObjectType type = GetGameObjectTypebyId(ownerObjectInfo.GameObjectId);

        if (type == GameObjectType.Player) 
        {
            if (ownerObjectInfo.GameObjectId == MyPlayerBaseCtrl.GameObjectId)
            {
                //Debug.Log("My projectile added");
                GameObject Projectile = Managers.resourceMgr.Instantiate($"Projectiles/My{projectilePrefabName}");
                Projectile.GetComponent<ProjectileCtrl>().GameObjectId = gameObjectInfo.GameObjectId;
                Projectile.GetComponent<ProjectileCtrl>().Speed = speed;

                GameObject skillUser;
                if (_players.TryGetValue(ownerObjectInfo.GameObjectId, out skillUser))
                {
                    Projectile.transform.position = skillUser.transform.position;
                    _projectiles.Add(gameObjectInfo.GameObjectId, Projectile);
                }
                else
                {
                    Debug.Log("Error) Cant find Skill Owner");
                }
            }
            else
            {
                //Debug.Log("Others projectile added");
                GameObject Projectile = Managers.resourceMgr.Instantiate($"Projectiles/{projectilePrefabName}");
                Projectile.GetComponent<ProjectileCtrl>().GameObjectId = gameObjectInfo.GameObjectId;
                Projectile.GetComponent<ProjectileCtrl>().Speed = speed;

                GameObject skillUser;
                if (_players.TryGetValue(ownerObjectInfo.GameObjectId, out skillUser))
                {
                    Projectile.transform.position = skillUser.transform.position;
                    _projectiles.Add(gameObjectInfo.GameObjectId, Projectile);
                }
                else
                {
                    Debug.Log("Error) Cant find Skill Owner");
                }
            }
        }
        else if (type == GameObjectType.Monster)
        {
            GameObject Projectile = Managers.resourceMgr.Instantiate($"Projectiles/{projectilePrefabName}");
            Projectile.GetComponent<ProjectileCtrl>().GameObjectId = gameObjectInfo.GameObjectId;
            Projectile.GetComponent<ProjectileCtrl>().Speed = speed;

            GameObject skillUser;
            if (_monsters.TryGetValue(ownerObjectInfo.GameObjectId, out skillUser))
            {
                if (projectilePrefabName == "Dragon_Fireball")
                {
                    Projectile.GetComponent<ProjectileCtrl>().OwnerGameObjectId = skillUser.GetComponent<MonsterCtrl>().GameObjectId;
                    Projectile.transform.localScale = skillUser.transform.localScale;
                    Projectile.transform.position = skillUser.transform.position + new Vector3(skillUser.transform.localScale.x * 2.3f, 0, 0);
                    _projectiles.Add(gameObjectInfo.GameObjectId, Projectile);
                }
                else if (projectilePrefabName == "Dragon_Thunder")
                {
                    Projectile.GetComponent<ProjectileCtrl>().OwnerGameObjectId = skillUser.GetComponent<MonsterCtrl>().GameObjectId;
                    Projectile.transform.localScale = skillUser.transform.localScale;
                    Projectile.transform.position = new Vector3(gameObjectInfo.PositionInfo.PosX, gameObjectInfo.PositionInfo.PosY, 0);
                    _projectiles.Add(gameObjectInfo.GameObjectId, Projectile);
                }
            }
        }
        else if (type == GameObjectType.Projectile)
        {
            GameObject Projectile = Managers.resourceMgr.Instantiate($"Projectiles/{projectilePrefabName}");
            Projectile.GetComponent<ProjectileCtrl>().GameObjectId = gameObjectInfo.GameObjectId;

            GameObject skillUser;
            if (_projectiles.TryGetValue(ownerObjectInfo.GameObjectId, out skillUser))
            {
                Projectile.transform.position = new Vector2(skillUser.transform.position.x, skillUser.transform.position.y + 0.5f);
                _projectiles.Add(gameObjectInfo.GameObjectId, Projectile);
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

    public void Clear()     // ����� DisConnect ����� �������̴� ���ǹ� ���� �׷��� �ϴ� ��
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

        MyPlayerBaseCtrl = null;
    }
}
