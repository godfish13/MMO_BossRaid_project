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

    protected virtual void Update()
    {
        SyncPos();
    }

    public void SyncPos()
    {
        State = PositionInfo.State;
        transform.position = new Vector2(PositionInfo.PosX, PositionInfo.PosY);
        transform.localScale = new Vector2(PositionInfo.LocalScaleX, 1);
    }

    #region UpdateCtrl series   // 아무 기능 없게 override
    protected override void UpdateCtrl()
    {
    }
              
    protected override void UpdateIdle()   // 이동, 구르기, MainSkill, SubSkill 가능
    {
    }
              
    protected override void UpdateRun()    // 이동, 구르기, MainSkill, SubSkill 가능
    {
    }
              
    protected override void UpdateJump()    // 이동, MainSkill, SubSkill 가능
    {
    }
              
    protected override void UpdateFall()   // 이동, MainSkill, SubSkill 가능
    {
    }
              
    protected override void UpdateLand()   // 이동, MainSkill 가능
    {
    }
              
    protected override void UpdateCrawl()  // 이동, 구르기 가능
    {
    }
              
    protected override void UpdateRolling()     // 다른 행동 불가
    {
    }
              
    protected override void UpdateSkill()  // 이동, 스킬 가능
    {
    }
              
    protected override void UpdateSubSkill()     // 다른 행동 불가
    {
    }
              
    protected override void UpdateDeath()
    {
    }
    #endregion

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