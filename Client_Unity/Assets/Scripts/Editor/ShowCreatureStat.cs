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

    private Vector2 scrollPosition; // ��ũ�ѹ� ������ Vector2, ���� ��ũ�� ��ġ �����ص�
    void OnGUI()
    {
        // ��ũ�ѹ� ���� ���� ����
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
        // �Ű����� ���� ���� ��ũ�� ��ġ, ��ũ�ѹ� �¿� ����, ��ũ�ѹ� ���� ���� (alwaysShow �ɼǵ��� �������Ƿ� â�� ǥ���ϴ� ������ â ���̺��� ������� ���� ǥ�õ�)

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

        // ��ũ�ѹ� �� ���� ����
        EditorGUILayout.EndScrollView();
    }
    
    void ShowList()
    {
        // ������ �� �׸��� �ݺ��Ͽ� ǥ��
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