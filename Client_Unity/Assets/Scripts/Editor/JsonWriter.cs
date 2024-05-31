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
        //EditorWindow StatWindow = GetWindow<JsonWriter>();    // window 조작용, todo
        //StatWindow.titleContent = new GUIContent("CreatureStat");

        TestDictionary = LoadJson<StatData, int, Stat>("PlayerData").MakeDict();
        // Dictionary를 LoadJson하면 "0" : {classId = ~, Class = "~", ... } 이런 형식으로 파싱됨
        // 그래서 TestDictionary를 바로 Json문자열로 직렬화하면 원하는 형태가 안나옴 (제작과정 문서 참고)

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

        // Dictionary에 데이터 추가
        TestDictionary.Add(PlayerData.ClassId, PlayerData);

        StatData statsData = new StatData { Stats = new List<Stat>() };
        foreach (Stat stat in TestDictionary.Values) 
        {
            statsData.Stats.Add(stat);
        }

        // Dictionary를 JSON 문자열로 직렬화
        string json = JsonConvert.SerializeObject(statsData, Formatting.Indented);

        System.IO.File.WriteAllText(GetJsonPath("PlayerData"), json);
    }

    static string GetJsonPath(string jsonFileName)
    {
        return string.Format($"Assets/Resources/Data/JsonBackUp/{jsonFileName}.json");
    }
}
