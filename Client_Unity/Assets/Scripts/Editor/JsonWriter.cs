using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class JsonWriter : EditorWindow
{
    public static Dictionary<int, Stat> TestDictionary { get; private set; } = new Dictionary<int, Stat>();
    static Loader LoadJson<Loader, key, value>(string path) where Loader : ILoader<key, value>
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/JsonBackUp/{path}");
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.ToString());
    }

    #region GUI Inputs
    string ClassIdInput;
    string ClassNameInput;
    string MaxHpInput;
    string HpInput;
    string MaxSpeedInput;
    string AccelerationInput;
    string DeleteClassIdInput;
    #endregion

    [MenuItem("Tools/CreatureStat/CreatureStatAdder")] 
    static void CreatureStatAdder()
    {
        EditorWindow StatWindow = GetWindow<JsonWriter>();    // window ���ۿ�, todo
        StatWindow.titleContent = new GUIContent("CreatureStat");
    }

    void OnGUI()
    {
        #region Add Stat Data
        // Input ��ȿ�� �˻�� ����
        int ClassId;
        int MaxHp;
        int Hp;
        float MaxSpeed;
        float Acceleration;

        ClassIdInput = EditorGUILayout.TextField("ClassId:", ClassIdInput);
        ClassNameInput = EditorGUILayout.TextField("ClassName:", ClassNameInput);
        MaxHpInput = EditorGUILayout.TextField("MaxHp:", MaxHpInput);
        HpInput = EditorGUILayout.TextField("Hp:", HpInput);
        MaxSpeedInput = EditorGUILayout.TextField("MaxSpeed:", MaxSpeedInput);
        AccelerationInput = EditorGUILayout.TextField("Acceleration:", AccelerationInput);

        if (GUILayout.Button("Add StatData"))
        {
            if (int.TryParse(ClassIdInput, out ClassId) && int.TryParse(MaxHpInput, out MaxHp) && int.TryParse(HpInput, out Hp) && float.TryParse(MaxSpeedInput, out MaxSpeed) && float.TryParse(AccelerationInput, out Acceleration))
            {
                if (TestDictionary.TryGetValue(ClassId, out Stat Target))
                {
                    Debug.Log("Error : Same ClassId is already existing");  // ClassId �ߺ� �� ����
                    return;
                }
                else
                {
                    AddStatData(ClassId, ClassNameInput, MaxHp, Hp, MaxSpeed, Acceleration);
                }
            }
            else
            {
                Debug.Log("Error : Wrong Input for StatData");  // �Է°����� ��ȿ�� ���ڰ� �ƴϰų� ������ �� �߸��� ���
            }
        }
        #endregion

        #region Delete Stat Data
        // Input ��ȿ�� �˻�� ����
        int DeleteClassId;

        GUILayout.Label("");
        GUILayout.Label("");
        DeleteClassIdInput = EditorGUILayout.TextField("Target ClassId:", DeleteClassIdInput);
        if (GUILayout.Button("Delete StatData"))
        {
            if (int.TryParse(DeleteClassIdInput, out DeleteClassId))
            {
                if (TestDictionary.TryGetValue(DeleteClassId, out Stat Target))
                {
                    DeleteStatData(DeleteClassId);
                }
                else
                {
                    Debug.Log($"Error : Cant find ClassId {DeleteClassId}");
                }
            }
            else
            {
                // �Է°��� ��ȿ�� ���ڰ� �ƴϰų� ������ �� �߸��� ���
                Debug.Log("Error : Wrong Input for StatData");
            }
        }
        #endregion
    }

    static void AddStatData(int classId, string className, int maxHp, int hp, float maxSpeed, float acceleration)
    {
        TestDictionary = LoadJson<StatData, int, Stat>("PlayerData").MakeDict();
        // Dictionary�� LoadJson�ϸ� "0" : {classId = ~, Class = "~", ... } �̷� �������� �Ľ̵�
        // �׷��� TestDictionary�� �ٷ� Json���ڿ��� ����ȭ�ϸ� ���ϴ� ���°� �ȳ��� (���۰��� ���� ����)

        Stat PlayerData = new Stat()
        {
            ClassId = classId,
            Class = className,
            MaxHp = maxHp,
            Hp = hp,
            MaxSpeed = maxSpeed,
            Acceleration = acceleration
        };

        // Dictionary�� ������ �߰�
        TestDictionary.Add(PlayerData.ClassId, PlayerData);

        // Dictionary�� key�� ClassId ������� ����
        var sortedSequence = TestDictionary.OrderBy(x => x.Key);
        TestDictionary = sortedSequence.ToDictionary(x => x.Key, x => x.Value);

        // StatData format�� Dictionary �߰�
        StatData StatDataList = new StatData { Stats = new List<Stat>() };
        foreach (Stat stat in TestDictionary.Values) 
        {
            StatDataList.Stats.Add(stat);
        }

        // Dictionary�� JSON ���ڿ��� ����ȭ
        string json = JsonConvert.SerializeObject(StatDataList, Formatting.Indented);
        System.IO.File.WriteAllText(GetJsonPath("PlayerData"), json);
    }

    static void DeleteStatData(int classId)
    {
        // Dictionary���� ������ ����
        TestDictionary.Remove(classId);

        StatData statsData = new StatData { Stats = new List<Stat>() };
        foreach (Stat stat in TestDictionary.Values)
        {
            statsData.Stats.Add(stat);
        }

        // Dictionary�� JSON ���ڿ��� ����ȭ
        string json = JsonConvert.SerializeObject(statsData, Formatting.Indented);
        System.IO.File.WriteAllText(GetJsonPath("PlayerData"), json);
    }

    static string GetJsonPath(string jsonFileName)
    {
        return string.Format($"Assets/Resources/Data/JsonBackUp/{jsonFileName}.json");
    }
}
