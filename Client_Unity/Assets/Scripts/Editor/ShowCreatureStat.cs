using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class ShowCreatureStat : EditorWindow
{
    public static Dictionary<int, Stat> TestDictionary { get; private set; } = new Dictionary<int, Stat>();
    static Loader LoadJson<Loader, key, value>(string path) where Loader : ILoader<key, value>
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/JsonBackUp/{path}");
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.ToString());
    }

    [MenuItem("Tools/CreatureStat/ViewCreatureStat")]
    static void CreatureStatViewer()
    {
        EditorWindow StatWindow = GetWindow<ShowCreatureStat>();
        StatWindow.titleContent = new GUIContent("CreatureStat List");
    }

    private void OnEnable()
    {
        TestDictionary = LoadJson<StatData, int, Stat>("PlayerData").MakeDict();
    }

    private Vector2 scrollPosition; // 스크롤바 범위용 Vector2, 현재 스크롤 위치 저장해둠
    void OnGUI()
    {
        // 스크롤바 시작 지점 설정
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
        // 매개변수 각각 현재 스크롤 위치, 스크롤바 좌우 길이, 스크롤바 상하 길이 (alwaysShow 옵션들을 꺼놨으므로 창이 표시하는 내용이 창 길이보다 길어지면 각각 표시됨)

        if (TestDictionary.Count <= 0)
        {
            GUILayout.Label("");
            GUILayout.Label("There is no Data in PlayerData");
            GUILayout.Label("");

            if (GUILayout.Button("Renewal List"))
            {
                TestDictionary = LoadJson<StatData, int, Stat>("PlayerData").MakeDict();
                ShowList();
            }

            GUILayout.Label("");
        }
        else
        {
            ShowList();

            if (GUILayout.Button("Renewal List"))
            {
                TestDictionary = LoadJson<StatData, int, Stat>("PlayerData").MakeDict();
                ShowList();
            }

            GUILayout.Label("");
        }

        // 스크롤바 끝 지점 설정
        EditorGUILayout.EndScrollView();
    }
    
    void ShowList()
    {
        // 사전의 각 항목을 반복하여 표시
        foreach (Stat Stat in TestDictionary.Values)
        {
            GUILayout.Label("");
            GUILayout.Label($"ClassId   : {Stat.ClassId}");
            GUILayout.Label($"Class    : {Stat.Class}");
            GUILayout.Label($"MaxHp : {Stat.MaxHp}");
            GUILayout.Label($"Hp    : {Stat.Hp}");
            GUILayout.Label($"MaxSpeed  : {Stat.MaxSpeed}");
            GUILayout.Label($"Acceleration  : {Stat.Acceleration}");
            GUILayout.Label("");
        }
    }
}