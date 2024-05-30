using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JsonWriter : EditorWindow
{
    [MenuItem("Tools/JsonWriter/CreatureStat")]   // ���� Class �Է� ���� ������ �� ���⼭ ���ڳѱ�� �ش� ���ڰ� = classId�� ���� �����ϵ��� ����
    static void CreatureStat()
    {
        EditorWindow StatWindow = GetWindow<JsonWriter>();
        StatWindow.titleContent = new GUIContent("CreatureStat");
    }

    //[MenuItem("Tools/JsonWriter/CharacterSkill")]   // ���� Class �Է� ���� ������ �� ���⼭ ���ڳѱ�� �ش� ���ڰ� = classId�� ���� �����ϵ��� ����
    static void CharacterSkill()
    {
        //PerformWin64Build(1);
    }

    //[MenuItem("Tools/JsonWriter/MonsterSkill")]   // ���� Class �Է� ���� ������ �� ���⼭ ���ڳѱ�� �ش� ���ڰ� = classId�� ���� �����ϵ��� ����
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
        }   // playerCount ������ŭ ������Ʈ���� ����� �ڵ�����
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];     // ������Ʈ�� ��ȯ
    } // �� ������Ʈ�� ��θ� �޾ƿ��� '/'���� �����ؼ� ������� �־���
      // ����� : s[0] = C:, s[1] = Unity_Projects, s[2] = MMO_GameWithServer_Project, s[3] = Client, s[4] = Assets

    static string[] GetScenePath()  // BuildSetting�� �߰��Ǿ��ִ� scene�� �ڵ�� �ܾ����
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }
}
