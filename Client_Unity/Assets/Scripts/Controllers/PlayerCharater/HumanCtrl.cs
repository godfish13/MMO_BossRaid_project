using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class HumanCtrl : BaseCtrl 
{
    protected ParticleSystem SlashEffect;

    protected override void Init()
    {
        ClassId = 0;
        SlashEffect = GetComponentInChildren<ParticleSystem>();

        base.Init();
    }

    #region MainSkill
    protected virtual void AnimEvent_MainSkillSlashOn()
    {
        SlashEffect.Play();
    }

    protected virtual void AnimEvent_MainSkillFrameEnded()
    {
        SlashEffect.Stop();
    }
    #endregion

    #region SubSkill      
    protected virtual void AnimEvent_SubSkillThrowBomb()
    {
    }

    protected virtual void AnimEvent_SubSkillFrameEnded()
    {
    }
    // Hit 판정 OnTriggerEnter2D는 BombCtrl에 존재
    #endregion
}