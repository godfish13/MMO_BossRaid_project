using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class MyBombCtrl : BombCtrl
{
    protected Rigidbody2D _rigidbody;

    public override CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)    // Set과 동시에 animation변경할것이므로 같은값으로 Set하려하면 return
                return;

            _state = value;
            PositionInfo.State = value;
            PacketSendingFlag = true;       // 상태변화시에도 패킷 송신

            UpdateAnim();
        }
    }

    protected override void Init()
    {
        base.Init();
        isExplode = false;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateAnim();

        if (State == CreatureState.Death)
        {
            C_LeaveGame leavePacket = new C_LeaveGame();
            leavePacket.GameObjectId = GameObjectId;
            Managers.networkMgr.Send(leavePacket);      // Server의 Despawn 패킷을 받으면 packetHandler에서 Remove, destroy 해줌
        }
    }

    #region Server 통신
    private bool PacketSendingFlag = false;  // PosInfo (State set, transform X Y 의 값이 변하면 true)
    private bool isExplode = false; // 다른 플레이어가 던진 폭탄이 폭발 이후 Server에서 Explosion State로 응답받기 전에 C_Move가 다시 송신되어 state가 다시 idle이되는 현상 방지

    protected override void Update()
    {
        if (Mathf.Abs(transform.position.x - PositionInfo.PosX) > 0.05f || Mathf.Abs(transform.position.y - PositionInfo.PosY) > 0.05f)
        {
            PositionInfo.PosX = transform.position.x;
            PositionInfo.PosY = transform.position.y;
            PositionInfo.LocalScaleX = transform.localScale.x;

            PacketSendingFlag = true;
        }

        if (PacketSendingFlag && isExplode == false)
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

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);   // damage 판정, Packet Send는 base에서

        isExplode = true;
        State = CreatureState.Explosion;      // State Change Flag
        C_MovePacketSend();                   // Explosion State로 변화시키라고 1회 통신
        _rigidbody.velocity = new Vector2(0, 0);
        _rigidbody.gravityScale = 0;
    }
}

