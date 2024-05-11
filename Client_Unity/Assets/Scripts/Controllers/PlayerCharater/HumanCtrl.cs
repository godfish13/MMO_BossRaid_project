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
    public int UI_Number;

    protected override void Init()
    {
        SlashEffect = GetComponentInChildren<ParticleSystem>();

        base.Init();
        switch (ClassId)
        {
            case 0:
                _myHpbar = Managers.UIMgr.ShowSceneUI<UI_MyHpbar>($"UI_Hpbar_Human_{UI_Number}");
                break;
            case 1:
                _myHpbar = Managers.UIMgr.ShowSceneUI<UI_MyHpbar>($"UI_Hpbar_Elf_{UI_Number}");
                break;
            case 2:
                _myHpbar = Managers.UIMgr.ShowSceneUI<UI_MyHpbar>($"UI_Hpbar_Furry_{UI_Number}");
                break;
        }
    }

    protected UI_MyHpbar _hpbar;
    public override int Hp   // Hp 변동 시 Ui 표시 수치 변경
    {
        get { return StatData.Hp; }
        set
        {
            StatData.Hp = value;
            _myHpbar.HpbarChange((float)Hp / (float)MaxHp);
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