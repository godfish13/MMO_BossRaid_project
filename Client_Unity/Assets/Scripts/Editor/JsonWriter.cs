using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class JsonWriter : EditorWindow
{
    [MenuItem("Tools/JsonWriter/CreatureStat")] 
    static void CreatureStat()
    {
        //EditorWindow StatWindow = GetWindow<JsonWriter>();    // window ���ۿ�, todo
        //StatWindow.titleContent = new GUIContent("CreatureStat");

        TestDictionary = LoadJson<StatData, int, Stat>("PlayerData").MakeDict();
        // Dictionary�� LoadJson�ϸ� "0" : {classId = ~, Class = "~", ... } �̷� �������� �Ľ̵�
        // �׷��� TestDictionary�� �ٷ� Json���ڿ��� ����ȭ�ϸ� ���ϴ� ���°� �ȳ��� (���۰��� ���� ����)

        AddStatData();
    }

    public void CreateGUI()
    {
        rootVisualElement.Add(new Label("Hello"));
    }

    public static Dictionary<int, Stat> TestDictionary { get; private set; } = new Dictionary<int, Stat>();

    static Loader LoadJson<Loader, key, value>(string path) where Loader : ILoader<key, value>
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/JsonBackUp/{path}");
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.ToString());
    }

    static void AddStatData()
    {
        Stat PlayerData = new Stat()
        {
            ClassId = 1,
            Class = "noasd",
            MaxHp = 105,
            Hp = 9,
            MaxSpeed = 170,
            Acceleration = 460
        };

        // Dictionary�� ������ �߰�
        TestDictionary.Add(PlayerData.ClassId, PlayerData);

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
