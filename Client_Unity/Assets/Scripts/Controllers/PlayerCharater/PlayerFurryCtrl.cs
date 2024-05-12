using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFurryCtrl : PlayerCtrl
{
    protected ParticleSystem BashEffect;
    protected GameObject GuardEffect;

    protected override void Init()
    {
        base.Init();
        BashEffect = GetComponentsInChildren<ParticleSystem>()[1];
        GuardEffect = GetComponentInChildren<GuardEffect>().gameObject;
    }

    protected override void Update()
    {
        base.Update();
        if (State == CreatureState.Subskill)
        {
            GuardEffect.SetActive(true);
        }
        else
        {
            GuardEffect.SetActive(false);
        }
    }

    #region Rolling        Furry Knight는 구르기 대신 방패 돌진   
    protected void AnimEvent_Rolling()  // 급가속 후 감속
    {
        BashEffect.Play();
    }
    protected override void AnimEvent_RollingEnded()
    {
        BashEffect.Stop();
    }
    #endregion 
}
