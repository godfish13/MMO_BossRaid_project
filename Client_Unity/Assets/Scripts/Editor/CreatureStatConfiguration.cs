using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        EditorGUILayout.BeginHorizontal();

        #region Window Left Side : Add/Delete Creature Stat
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 2)); // 왼쪽 범위 시작지점 지정

        AddButton();

        DeleteButton();

        EditorGUILayout.EndVertical();  //왼쪽 범위 끝
        #endregion

        #region Window Right Side : View Creature Stat
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 2)); // 오른쪽 범위 시작지점 지정

        ShowList();

        EditorGUILayout.EndVertical();  // 오른쪽 범위 종료
        #endregion

        EditorGUILayout.EndHorizontal();    
    }
    
    private void OnEnable()
    {
        TestDictionary = LoadJson<StatData, int, Stat>("PlayerData").MakeDict();
    }

    #region Json Data Dictionary
    public static Dictionary<int, Stat> TestDictionary { get; private set; } = new Dictionary<int, Stat>();
    static Loader LoadJson<Loader, key, value>(string path) where Loader : ILoader<key, value>
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/JsonBackUp/{path}");
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.ToString());
    }
    #endregion

    #region Window Left Side : Add/Delete Creature Stat
    // GUI Inputs
    string ClassIdInput;
    string ClassNameInput;
    string MaxHpInput;
    string HpInput;
    string MaxSpeedInput;
    string AccelerationInput;
    string DeleteClassIdInput;

    void AddButton()
    {
        // Input 유효성 검사용 변수
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
    }

    void AddStatData(int classId, string className, int maxHp, int hp, float maxSpeed, float acceleration)
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

        OpenFile(); // json 파일 변경내역 갱신을 위해 한번 열어줌 (유니티 캐시상 열어주지 않으면 변경내용이 반영되지 않음)
    }

    void DeleteButton()
    {
        // Input 유효성 검사용 변수
        int DeleteClassId;

        GUILayout.Label("");
        GUILayout.Label("");
        DeleteClassIdInput = EditorGUILayout.TextField("Target ClassId", DeleteClassIdInput);
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
    }

    void DeleteStatData(int classId)
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

        OpenFile(); // json 파일 변경내역 갱신을 위해 한번 열어줌 (유니티 캐시상 열어주지 않으면 변경내용이 반영되지 않음)
    }

    string GetJsonPath(string jsonFileName)
    {
        return string.Format($"Assets/Resources/Data/JsonBackUp/{jsonFileName}.json");
    }

    void OpenFile()     // Unity 내에서 Json 변동 갱신시켜주기위해 Json파일 한번 열어줌
    {
        string relativeJsonFilePath = "Resources/Data/JsonBackUp/PlayerData.json";
        string jsonFilePath;
        jsonFilePath = Path.Combine(Application.dataPath, relativeJsonFilePath);    // 타 프로그램에서 json파일 찾을 수 있게 절대경로 설정 (C:Unity_Projects/~~)

        //System.Diagnostics.Process.Start("notepad.exe", jsonFilePath);    // 메모장으로 열기
        System.Diagnostics.Process.Start(jsonFilePath); // 기본설정(VisualStudio 2022)로 열기
    }
    #endregion

    #region Window Right Side : View Creature Stat
    private Vector2 scrollPosition; // 스크롤바 범위용 Vector2, 현재 스크롤 위치 저장해둠

    void ShowList()
    {
        // 스크롤바 시작 지점 설정
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width / 2 - 8f), GUILayout.Height(position.height));
        // 매개변수 각각 현재 스크롤 위치, 스크롤바 좌우 범위, 스크롤바 상하 길이 (alwaysShow 옵션들을 꺼놨으므로 창이 표시하는 내용이 창 길이보다 길어지면 각각 표시됨)

        if (TestDictionary.Count <= 0)
        {
            GUILayout.Label("");
            GUILayout.Label("There is no Data in PlayerData");
            GUILayout.Label("");
        }
        else
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

        EditorGUILayout.EndScrollView();    // 스크롤바 끝 지점 설정
    }
    #endregion
}
