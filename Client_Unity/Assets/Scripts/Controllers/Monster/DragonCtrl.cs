using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonCtrl : MonsterCtrl
{
    private BoxCollider2D _BiteBox;
    private BoxCollider2D _BurnBox;

    protected override void Init()
    {
        base.Init();
        _BiteBox = GetComponentsInChildren<BoxCollider2D>()[0];
        _BurnBox = GetComponentsInChildren<BoxCollider2D>()[1];
    }

    #region Animation
    protected override void UpdateAnim()
    {
        if (_animator == null || _spriteRenderer == null)   // 각각이 아직 초기화 안된상태면 return
            return;

        switch (State)
        {
            case CreatureState.Await:
                _animator.Play("IDLE");
                break;
            case CreatureState.Idle:
                _animator.Play("IDLE");
                break;
            case CreatureState.Run:
                _animator.Play("RUN");
                break;
            case CreatureState.Bite:
                _animator.Play("BITE");
                break;
            case CreatureState.Burn:
                _animator.Play("BURN");
                break;
            case CreatureState.Fireball:
                _animator.Play("FIREBALL");
                break;
            case CreatureState.Thunder:
                _animator.Play("THUNDER");
                break;
            case CreatureState.Death:
                _animator.Play("DEATH");
                break;
        }
    }
    #endregion

    #region Monster_Skill
    
    // 깨물기
    protected void AnimEvent_BiteHitBoxOn()
    {
        _BiteBox.enabled = true;
        _BiteBox.transform.localPosition = new Vector2(0.1f, 0); // 충돌 감지를 위해 살짝 앞으로 이동
    }

    protected void AnimEvent_BiteFrameEnded()
    {
        _BiteBox.enabled = false;
        _BiteBox.transform.localPosition = new Vector2(0, 0);    // 위치 복귀
    }

    // 근접 불뿜기
    protected void AnimEvent_BurnHitBoxOn()
    {
        _BurnBox.enabled = true;
        _BurnBox.transform.localPosition = new Vector2(0.1f, 0); // 충돌 감지를 위해 살짝 앞으로 이동
        _BurnBox.transform.localPosition = new Vector2(0, 0);    // 위치 복귀
    }

    protected void AnimEvent_BurnSizeUp()
    {
        _BurnBox.offset = new Vector2(-2, 1.1f);    // 커져라!
        _BurnBox.size = new Vector2(3, 2.2f);
        _BurnBox.transform.localPosition = new Vector2(0.1f, 0); // 충돌 감지를 위해 살짝 앞으로 이동
        _BurnBox.transform.localPosition = new Vector2(0, 0);    // 위치 복귀
    }

    protected void AnimEvent_BurnHitBoxOFF()
    {
        _BurnBox.offset = new Vector2(-1.9f, 1.1f); // 원래 크기로 복구
        _BurnBox.size = new Vector2(2, 2.2f);
        _BurnBox.enabled = false;
    }
    #endregion
}
