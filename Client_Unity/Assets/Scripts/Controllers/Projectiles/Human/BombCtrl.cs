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

    public override void SyncPos()
    {
        // 변화 없을땐 쓸데없이 작동하지 않도록 조건 추가
        if (State != PositionInfo.State || transform.position.x != PositionInfo.PosX || transform.position.y != PositionInfo.PosY || transform.localScale.x != PositionInfo.LocalScaleX)
        {
            State = PositionInfo.State;
            // Server에서 관리되는 투사체는 MoveTowards를 통해 이동
            transform.position = new Vector2(PositionInfo.PosX, PositionInfo.PosY);
            transform.localScale = new Vector2(PositionInfo.LocalScaleX, 1);
        }
    }
}
