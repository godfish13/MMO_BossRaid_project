using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JsonWriter : EditorWindow
{
    [MenuItem("Tools/JsonWriter/CreatureStat")]   // 따로 Class 입력 구현 못했을 시 여기서 인자넘기고 해당 인자값 = classId로 각각 빌드하도록 하자
    static void CreatureStat()
    {
        EditorWindow StatWindow = GetWindow<JsonWriter>();
        StatWindow.titleContent = new GUIContent("CreatureStat");
    }

    //[MenuItem("Tools/JsonWriter/CharacterSkill")]   // 따로 Class 입력 구현 못했을 시 여기서 인자넘기고 해당 인자값 = classId로 각각 빌드하도록 하자
    static void CharacterSkill()
    {
        //PerformWin64Build(1);
    }

    //[MenuItem("Tools/JsonWriter/MonsterSkill")]   // 따로 Class 입력 구현 못했을 시 여기서 인자넘기고 해당 인자값 = classId로 각각 빌드하도록 하자
    static void MonsterSkill()
    {
        //PerformWin64Build(1);
    }

    static void PerformWin64Build(int playerCount)
    {
        for (int i = 0; i < playerCount; i++)
        {
            BuildPipeline.BuildPlayer(GetScenePath(),
                "0.TestBuilds/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe",
                BuildTarget.StandaloneWindows64,
                BuildOptions.AutoRunPlayer);
        }   // playerCount 갯수만큼 프로젝트들을 만들고 자동실행
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];     // 프로젝트명 반환
    } // 이 프로젝트의 경로를 받아오고 '/'마다 절단해서 순서대로 넣어줌
      // 결과값 : s[0] = C:, s[1] = Unity_Projects, s[2] = MMO_GameWithServer_Project, s[3] = Client, s[4] = Assets

    static string[] GetScenePath()  // BuildSetting에 추가되어있는 scene들 코드로 긁어오기
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }
}
