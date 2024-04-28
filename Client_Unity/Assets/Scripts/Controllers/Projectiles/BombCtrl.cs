using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BombCtrl : ProjectileCtrl
{
    public float rotationSpeed = 200.0f;
    public float Xdirection = 1.0f;
    private bool _isRotate = true;

    protected override void Init()
    {
        base.Init();
        ResetState();      
    }
    private void ResetState()
    {
        State = CreatureState.Idle;
        _isRotate = true;
        _rigidbody.gravityScale = 1;
    }

    protected override void UpdateAnim()
    {
        if (_animator == null)
            return;

        switch (State)
        {
            case CreatureState.Idle:
                _animator.Play("IDLE");
                break;
            case CreatureState.Rolling:         // Explosion State
                _animator.Play("EXPLOSION");
                break;
        }
    }
    
    protected override void FixedUpdate()
    {
        Rotation();
        UpdateAnim();

        if (State == CreatureState.Death)
        {
            transform.localPosition = Vector3.zero;
            Managers.resourceMgr.Destroy(gameObject);
        }
    }

    protected virtual void Rotation()
    {
        if (_isRotate)
            SpriteTransform.Rotate(Vector3.forward * rotationSpeed * Xdirection * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);   // damage 판정, Packet Send는 base에서

        _isRotate = false;
        _rigidbody.velocity = new Vector2(0, 0);
        _rigidbody.gravityScale = 0;
        State = CreatureState.Rolling;      // Explosion State
    }
}
