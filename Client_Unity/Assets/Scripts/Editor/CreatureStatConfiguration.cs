using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using UnityEditor;
using UnityEngine;

public class CreatureStatConfiguration : EditorWindow
{
    [MenuItem("Tools/CreatureStat Configuration")]
    public static void CreatureStat()
    {
        EditorWindow StatWindow = GetWindow<CreatureStatConfiguration>();
        StatWindow.titleContent = new GUIContent("CreatureStat Configuration");
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();  // �¿������ ������ ���� LayOut ���� (������ ���η� �̾���)

        #region Window Left Side : Add/Delete Creature Stat
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 2)); // ���� ���� �������� ����

        AddButton();
        DeleteButton();

        EditorGUILayout.EndVertical();  //���� ���� ��
        #endregion

        #region Window Right Side : View Creature Stat
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 2)); // ������ ���� �������� ����

        ShowList();

        EditorGUILayout.EndVertical();  // ������ ���� ����
        #endregion

        EditorGUILayout.EndHorizontal();    // �¿������ ������ ���� LayOut ��    
    }
    
    private void OnEnable()
    {
        JsonDataDictionary = LoadJson<StatData, int, Stat>(JsonFileName).MakeDict();
    }

    string JsonFileName = "PlayerData";     // ������ Json ���� �̸�

    #region Json Data Dictionary
    public static Dictionary<int, Stat> JsonDataDictionary { get; private set; } = new Dictionary<int, Stat>();
    static Loader LoadJson<Loader, key, value>(string path) where Loader : ILoader<key, value>
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/JsonBackUp/{path}");
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.ToString());
    }
    #endregion

    #region Window Left Side : Add/Delete Creature Stat
    // GUI Inputs
    string ClassIdInput;        // Add btn
    string ClassNameInput;
    string MaxHpInput;
    string HpInput;
    string MaxSpeedInput;
    string AccelerationInput;
    string DeleteClassIdInput;  // Delete btn

    void AddButton()
    {
        // Input ��ȿ�� �˻�� ����
        int ClassId;
        int MaxHp;
        int Hp;
        float MaxSpeed;
        float Acceleration;

        GUILayout.Label("");
        ClassIdInput = EditorGUILayout.TextField("ClassId", ClassIdInput);
        ClassNameInput = EditorGUILayout.TextField("ClassName", ClassNameInput);
        MaxHpInput = EditorGUILayout.TextField("MaxHp", MaxHpInput);
        HpInput = EditorGUILayout.TextField("Hp", HpInput);
        MaxSpeedInput = EditorGUILayout.TextField("MaxSpeed", MaxSpeedInput);
        AccelerationInput = EditorGUILayout.TextField("Acceleration", AccelerationInput);

        if (GUILayout.Button("Add StatData"))
        {
            if (int.TryParse(ClassIdInput, out ClassId) && int.TryParse(MaxHpInput, out MaxHp) && int.TryParse(HpInput, out Hp) && float.TryParse(MaxSpeedInput, out MaxSpeed) && float.TryParse(AccelerationInput, out Acceleration))
            {
                if (JsonDataDictionary.TryGetValue(ClassId, out Stat Target))
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
    }

    void AddStatData(int classId, string className, int maxHp, int hp, float maxSpeed, float acceleration)
    {
        JsonDataDictionary = LoadJson<StatData, int, Stat>("PlayerData").MakeDict();
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
        JsonDataDictionary.Add(PlayerData.ClassId, PlayerData);

        // Dictionary�� key�� ClassId ������� ����
        var sortedSequence = JsonDataDictionary.OrderBy(x => x.Key);
        JsonDataDictionary = sortedSequence.ToDictionary(x => x.Key, x => x.Value);

        // StatData format�� Dictionary �߰�
        StatData StatDataList = new StatData { Stats = new List<Stat>() };
        foreach (Stat stat in JsonDataDictionary.Values)
        {
            StatDataList.Stats.Add(stat);
        }

        // Dictionary�� JSON ���ڿ��� ����ȭ �� Json���Ϸ� �ۼ�
        string json = JsonConvert.SerializeObject(StatDataList, Formatting.Indented);
        System.IO.File.WriteAllText(GetJsonPath(JsonFileName), json);

        OpenFile(JsonFileName); // json ���� ���泻�� ������ ���� �ѹ� ������ (����Ƽ ĳ�û� �������� ������ ���泻���� �ݿ����� ����)
    }

    void DeleteButton()
    {
        // Input ��ȿ�� �˻�� ����
        int DeleteClassId;

        GUILayout.Label("");
        GUILayout.Label("");
        DeleteClassIdInput = EditorGUILayout.TextField("Target ClassId", DeleteClassIdInput);
        if (GUILayout.Button("Delete StatData"))
        {
            if (int.TryParse(DeleteClassIdInput, out DeleteClassId))
            {
                if (JsonDataDictionary.TryGetValue(DeleteClassId, out Stat Target))
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
    }

    void DeleteStatData(int classId)
    {
        // Dictionary���� ������ ����
        JsonDataDictionary.Remove(classId);

        StatData statsData = new StatData { Stats = new List<Stat>() };
        foreach (Stat stat in JsonDataDictionary.Values)
        {
            statsData.Stats.Add(stat);
        }

        // Dictionary�� JSON ���ڿ��� ����ȭ �� Json���Ϸ� �ۼ�
        string json = JsonConvert.SerializeObject(statsData, Formatting.Indented);
        System.IO.File.WriteAllText(GetJsonPath(JsonFileName), json);

        OpenFile(JsonFileName); // json ���� ���泻�� ������ ���� �ѹ� ������ (����Ƽ ĳ�û� �������� ������ ���泻���� �ݿ����� ����)
    }

    string GetJsonPath(string jsonFileName)
    {
        return string.Format($"Assets/Resources/Data/JsonBackUp/{jsonFileName}.json");
    }

    void OpenFile(string path)     // Unity ������ Json ���� ���Ž����ֱ����� Json���� �ѹ� ������
    {
        // json ���� ���泻�� ������ ���� �ѹ� ������ (����Ƽ ĳ�û� �������� ������ ���泻���� �ݿ����� ����)
        string relativeJsonFilePath = $"Resources/Data/JsonBackUp/{JsonFileName}.json";
        string jsonFilePath;
        jsonFilePath = Path.Combine(Application.dataPath, relativeJsonFilePath);    // Ÿ ���α׷����� json���� ã�� �� �ְ� ������ ���� (C:Unity_Projects/~~)

        //System.Diagnostics.Process.Start("notepad.exe", jsonFilePath);    // �޸������� ����
        System.Diagnostics.Process.Start(jsonFilePath); // �⺻����(VisualStudio 2022)�� ����
    }
    #endregion

    #region Window Right Side : View Creature Stat
    private Vector2 scrollPosition; // ��ũ�ѹ� ������ Vector2, ���� ��ũ�� ��ġ �����ص�

    void ShowList()
    {
        // ��ũ�ѹ� ���� ���� ����
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width / 2 - 8f), GUILayout.Height(position.height));
        // �Ű����� ���� ���� ��ũ�� ��ġ, ��ũ�ѹ� �¿� ����, ��ũ�ѹ� ���� ���� (alwaysShow �ɼǵ��� �������Ƿ� â�� ǥ���ϴ� ������ â ���̺��� ������� ���� ǥ�õ�)

        if (JsonDataDictionary.Count <= 0)
        {
            GUILayout.Label("");
            GUILayout.Label("There is no Data in PlayerData");
            GUILayout.Label("");
        }
        else
        {
            // ������ �� �׸��� �ݺ��Ͽ� ǥ��
            foreach (Stat Stat in JsonDataDictionary.Values)
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

        EditorGUILayout.EndScrollView();    // ��ũ�ѹ� �� ���� ����
    }
    #endregion
}