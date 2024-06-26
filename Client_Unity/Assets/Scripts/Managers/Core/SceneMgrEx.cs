using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgrEx         // Ex : extended
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.Scene sceneType)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(sceneType));
    }

    string GetSceneName(Define.Scene sceneType)
    {
        //return sceneType.ToString();    // 걍 이거 한줄 삽가능같은데? 다른 요소들 구현(확장성)을 위해 리플렉션으로 구현하나?
                                        // => enum.GetName()식으로 리플렉션 쓰는게 ToString보다 성능상 우위라고 함!
        string name = System.Enum.GetName(typeof(Define.Scene), sceneType);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }

    public void SetResolution(int setWidth, int setHeight)
    {
        //해상도를 설정값에 따라 변경
        //3번째 파라미터는 풀스크린 모드를 설정 > true : 풀스크린, false : 창모드
        Screen.SetResolution(setWidth, setHeight, false);
    }
}
