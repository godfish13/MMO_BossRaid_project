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
        // 변화 없을땐 쓸데없이 작동하지 않도록 조건 추가
        if (State != PositionInfo.State || transform.position.x != PositionInfo.PosX || transform.position.y != PositionInfo.PosY || transform.localScale.x != PositionInfo.LocalScaleX)
        {
            State = PositionInfo.State;
            transform.position = new Vector2(PositionInfo.PosX, PositionInfo.PosY);
            transform.localScale = new Vector2(PositionInfo.LocalScaleX, 1);
            Debug.Log($"{GameObjectId} : {PositionInfo.PosX}, {PositionInfo.PosY}, {PositionInfo.LocalScaleX}");
        }
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