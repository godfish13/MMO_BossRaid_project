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
    protected BoxCollider2D SlashBox;
    protected ParticleSystem SlashEffect;

    protected override void Init()
    {
        ClassId = 0;
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
}