using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    public AudioClip Fire;

    int i = 0;
    private void OnTriggerEnter(Collider other)
    {
        //Managers.soundMgr.Play("UnityChanVoice/univ1222");
        //Managers.soundMgr.Play("UnityChanVoice/univ0001");

        i++;
        if (i % 2 == 0)
            Managers.soundMgr.Play(Fire, Define.Sound.Bgm);
        else
            Managers.soundMgr.Play("Bgm/Evolution", Define.Sound.Bgm);
    }
}
