using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BombCtrl : ProjectileCtrl
{
    private float rotationSpeed = 1000.0f;
    public float Xdirection = 1.0f;
    protected bool _isRotate;

    protected override void Init()
    {
        _isRotate = true;
        base.Init();  
    }

    protected override void FixedUpdate()
    {
        Rotation();
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
            case CreatureState.Explosion:
                _isRotate = false;
                _animator.Play("EXPLOSION");
                break;
        }
    }

    protected virtual void Rotation()
    {
        if (_isRotate)
            SpriteTransform.Rotate(Vector3.forward * rotationSpeed * Xdirection * Time.deltaTime);
    }
}
