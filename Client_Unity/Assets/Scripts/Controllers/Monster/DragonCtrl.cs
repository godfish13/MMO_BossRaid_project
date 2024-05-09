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
        if (_animator == null || _spriteRenderer == null)   // ������ ���� �ʱ�ȭ �ȵȻ��¸� return
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
    
    // ������
    protected void AnimEvent_BiteHitBoxOn()
    {
        _BiteBox.enabled = true;
        _BiteBox.transform.localPosition = new Vector2(0.1f, 0); // �浹 ������ ���� ��¦ ������ �̵�
    }

    protected void AnimEvent_BiteFrameEnded()
    {
        _BiteBox.enabled = false;
        _BiteBox.transform.localPosition = new Vector2(0, 0);    // ��ġ ����
    }

    // ���� �һձ�
    protected void AnimEvent_BurnHitBoxOn()
    {
        _BurnBox.enabled = true;
        _BurnBox.transform.localPosition = new Vector2(0.1f, 0); // �浹 ������ ���� ��¦ ������ �̵�
        _BurnBox.transform.localPosition = new Vector2(0, 0);    // ��ġ ����
    }

    protected void AnimEvent_BurnSizeUp()
    {
        _BurnBox.offset = new Vector2(-2, 1.1f);    // Ŀ����!
        _BurnBox.size = new Vector2(3, 2.2f);
        _BurnBox.transform.localPosition = new Vector2(0.1f, 0); // �浹 ������ ���� ��¦ ������ �̵�
        _BurnBox.transform.localPosition = new Vector2(0, 0);    // ��ġ ����
    }

    protected void AnimEvent_BurnHitBoxOFF()
    {
        _BurnBox.offset = new Vector2(-1.9f, 1.1f); // ���� ũ��� ����
        _BurnBox.size = new Vector2(2, 2.2f);
        _BurnBox.enabled = false;
    }
    #endregion
}
