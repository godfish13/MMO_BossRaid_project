using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MultiPlayerBuildandRun : MonoBehaviour
{
    [MenuItem("Tools/Run MultiPlayer/1 players")]   // 따로 Class 입력 구현 못했을 시 여기서 인자넘기고 해당 인자값 = classId로 각각 빌드하도록 하자
    static void Run1player()
    {
        PerformWin64Build(1);
    }

    [MenuItem("Tools/Run MultiPlayer/2 players")]
    static void Run2players()
    {
        PerformWin64Build(2);
    }

    [MenuItem("Tools/Run MultiPlayer/3 players")]
    static void Run3players()
    {
        PerformWin64Build(3);
    }

    [MenuItem("Tools/Run MultiPlayer/4 players")]
    static void Run4players()
    {
        PerformWin64Build(4);
    }

    static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        // window 설정으로 빌드

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
