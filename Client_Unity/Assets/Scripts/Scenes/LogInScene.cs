using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogInScene : BaseScene
{
    protected override void init()
    {
        base.init();
        SceneType = Define.Scene.LogIn;

        List<GameObject> list = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            Managers.sceneMgrEx.LoadScene(Define.Scene.InGame);
        }
    }

    public override void Clear()
    {
        Debug.Log("Cleared 'LogIn' Scene");
    }
}
