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
        EditorWindow StatWindow = GetWindow<JsonWriter>();    // window 조작용, todo
        StatWindow.titleContent = new GUIContent("CreatureStat");
    }

    void OnGUI()
    {
        #region Add Stat Data
        // Input 유효성 검사용 변수
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
                    Debug.Log("Error : Same ClassId is already existing");  // ClassId 중복 시 에러
                    return;
                }
                else
                {
                    AddStatData(ClassId, ClassNameInput, MaxHp, Hp, MaxSpeed, Acceleration);
                }
            }
            else
            {
                Debug.Log("Error : Wrong Input for StatData");  // 입력값들이 유효한 숫자가 아니거나 문자인 등 잘못된 경우
            }
        }
        #endregion

        #region Delete Stat Data
        // Input 유효성 검사용 변수
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
                // 입력값이 유효한 숫자가 아니거나 문자인 등 잘못된 경우
                Debug.Log("Error : Wrong Input for StatData");
            }
        }
        #endregion
    }

    static void AddStatData(int classId, string className, int maxHp, int hp, float maxSpeed, float acceleration)
    {
        TestDictionary = LoadJson<StatData, int, Stat>("PlayerData").MakeDict();
        // Dictionary를 LoadJson하면 "0" : {classId = ~, Class = "~", ... } 이런 형식으로 파싱됨
        // 그래서 TestDictionary를 바로 Json문자열로 직렬화하면 원하는 형태가 안나옴 (제작과정 문서 참고)

        Stat PlayerData = new Stat()
        {
            ClassId = classId,
            Class = className,
            MaxHp = maxHp,
            Hp = hp,
            MaxSpeed = maxSpeed,
            Acceleration = acceleration
        };

        // Dictionary에 데이터 추가
        TestDictionary.Add(PlayerData.ClassId, PlayerData);

        // Dictionary의 key값 ClassId 순서대로 정렬
        var sortedSequence = TestDictionary.OrderBy(x => x.Key);
        TestDictionary = sortedSequence.ToDictionary(x => x.Key, x => x.Value);

        // StatData format에 Dictionary 추가
        StatData StatDataList = new StatData { Stats = new List<Stat>() };
        foreach (Stat stat in TestDictionary.Values) 
        {
            StatDataList.Stats.Add(stat);
        }

        // Dictionary를 JSON 문자열로 직렬화
        string json = JsonConvert.SerializeObject(StatDataList, Formatting.Indented);
        System.IO.File.WriteAllText(GetJsonPath("PlayerData"), json);
    }

    static void DeleteStatData(int classId)
    {
        // Dictionary에서 데이터 제거
        TestDictionary.Remove(classId);

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
