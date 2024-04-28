using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCtrl : MonoBehaviour
{
    public Transform ShooterTransform;      // �� �÷��̾� transform, PlayerCtrl���� projectile �����Ҷ� ����

    public void ShooterSet(Transform shooterTransform)
    {
        ShooterTransform = shooterTransform;
    }

    public CreatureState State = CreatureState.Idle;   
    protected Transform SpriteTransform;   
    protected Rigidbody2D _rigidbody;
    protected Animator _animator;

    protected virtual void Init()
    {
        SpriteTransform = gameObject.GetComponentsInChildren<Transform>()[1];
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        Init();
    }

    protected virtual void FixedUpdate()
    {
        Move(); 

        // Todo ��ġ ����ȭ ��Ŷ �ۼ���
    }

    protected virtual void UpdateAnim()
    {
        if (_animator == null)
            return;

        switch (State)
        {
            case CreatureState.Idle:
                _animator.Play("IDLE");
                break;
        }
    }

    protected virtual void Move()
    {
        // projectile�� ��������
        /* 1. Human - Bomb : HumanCtrl���� Instantiate ���� AddForce
         * 2. Elf - Arrow, Lizard - Magic : ArrowCtrl.FixedUpdate���� rigidbody velocity ���� -> Server�� �̵�����
         */
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Projectile {gameObject.name} : OnTrigger Enter");

        //Todo damage ���� + Packet send
    }
}
