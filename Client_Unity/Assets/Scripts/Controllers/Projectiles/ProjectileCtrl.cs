using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCtrl : MonoBehaviour
{
    public Transform ShooterTransform;      // 쏜 플레이어 transform, PlayerCtrl에서 projectile 생성할때 지정

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

        // Todo 위치 동기화 패킷 송수신
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
        // projectile별 개별구현
        /* 1. Human - Bomb : HumanCtrl에서 Instantiate 순간 AddForce
         * 2. Elf - Arrow, Lizard - Magic : ArrowCtrl.FixedUpdate에서 rigidbody velocity 조작 -> Server로 이동예정
         */
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Projectile {gameObject.name} : OnTrigger Enter");

        //Todo damage 판정 + Packet send
    }
}
