using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyPlayerBaseCtrl : PlayerCtrl
{ 
    public override CreatureState State     // ��Ŷ ������ �κ� �����ϹǷ� override
    {
        get { return _state; }
        set
        {
            if (_state == value)    // Set�� ���ÿ� animation�����Ұ��̹Ƿ� ���������� Set�Ϸ��ϸ� return
                return;

            _state = value;
            PositionInfo.State = value;
            PacketSendingFlag = true;

            UpdateAnim();
        }
    }

    #region Server ���
    protected bool PacketSendingFlag = false; // State ��ȭ, position���� �������� �̻� ��ȭ�� true

    protected override void Update()
    {
        if (Mathf.Abs(transform.position.x - PositionInfo.PosX) > 0.05f || Mathf.Abs(transform.position.y - PositionInfo.PosY) > 0.05f)
        {
            PositionInfo.PosX = transform.position.x;
            PositionInfo.PosY = transform.position.y;
            PositionInfo.LocalScaleX = transform.localScale.x;

            PacketSendingFlag = true;
        }

        if (PacketSendingFlag)
        {
            PacketSendingFlag = false;
            C_MovePacketSend();
        }
    }

    private void C_MovePacketSend()
    {
        C_Move movePacket = new C_Move();

        movePacket.GameObjectId = GameObjectId;

        movePacket.PositionInfo = new PositionInfo();
        movePacket.PositionInfo.State = PositionInfo.State;
        movePacket.PositionInfo.PosX = PositionInfo.PosX;
        movePacket.PositionInfo.PosY = PositionInfo.PosY;
        movePacket.PositionInfo.LocalScaleX = PositionInfo.LocalScaleX;

        Managers.networkMgr.Send(movePacket);
    }
    #endregion

    #region isGround
    private Collider2D _platformCollider;    // Platform�� �����ϸ� �ش� �÷����� collider ���, ���� �ش� �ݶ��̴����� �������� �������ΰɷ� �Ǻ�

    public void OnCollisionEnter2D(Collision2D collision)   // Platform�� ��Ҵ��� üũ
    {
        // OnCollision �߻� ��
        // �浹 ������ y ��ǥ�� �÷��̾� collider �Ʒ���(y ���� �ּڰ�)���� �۰ų� ������ �ٴڿ� �����ߴٰ� ����
        // �����ؼ� �����̴µ� �÷��̾� �ݶ��̴� �翷�̳� ���ʿ� ���� ����� �� _isGrounded = true�� �Ǵ°� ����
        if (collision.contacts.All((i) => (i.point.y <= _collider.bounds.min.y)))
        {
            _platformCollider = collision.collider;
            _isGrounded = true;

            if (State == CreatureState.Rolling) // ������ ���ӵ� �ݵ� �ʱ�ȭ
            {
                _velocity.x = transform.localScale.x * StatData.MaxSpeed;
                _rigidbody.velocity = _velocity;
            }

            if (Input.GetKey(KeyCode.X) == false && State != CreatureState.Subskill)    // ��ų�� ����߿��� Land��� ��� x
                State = CreatureState.Land;
            //Debug.Log("Landed");
            _coJumpCoolTimer = StartCoroutine("CoJumpCoolTimer", SkillData.JumpCoolTime);     // ���� �� ���� 0.1�� ��Ÿ�� (�ִϸ��̼� ���� ���� ����)
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (_isGrounded && collision.collider == _platformCollider)
        {
            _isGrounded = false;
        }
    }
    #endregion
}
