using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyPlayerBaseCtrl : PlayerCtrl
{ 
    public override CreatureState State     // 패킷 보내는 부분 존재하므로 override
    {
        get { return _state; }
        set
        {
            if (_state == value)    // Set과 동시에 animation변경할것이므로 같은값으로 Set하려하면 return
                return;

            _state = value;
            PositionInfo.State = value;
            PacketSendingFlag = true;

            UpdateAnim();
        }
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, 3.2f, -8);
    }

    public override int Hp   // Hp 변동 시 UI 표시 수치 변경
    {
        get { return StatData.Hp; }
        set
        {
            StatData.Hp = value;
            _myHpbar.HpbarChange((float)Hp / (float)MaxHp);
        }
    }

    #region Server 통신
    protected bool PacketSendingFlag = false; // State 변화, position값이 일정수준 이상 변화시 true

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

    #region UpdateCtrl
    protected override void UpdateCtrl()
    {
        if (Hp <= 0)
            State = CreatureState.Death;

        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Run:
                UpdateRun();
                break;
            case CreatureState.Jump:
                UpdateJump();
                break;
            case CreatureState.Fall:
                UpdateFall();
                break;
            case CreatureState.Land:
                UpdateLand();
                break;
            case CreatureState.Crouch:
                UpdateCrawl();
                break;
            case CreatureState.Crawl:
                UpdateCrawl();
                break;
            case CreatureState.Rolling:
                UpdateRolling();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Subskill:
                UpdateSubSkill();
                break;
            case CreatureState.Death:   // 사망
                UpdateDeath();
                break;
            case CreatureState.Tmp:  // 스킬 사용 후 Idle, Move 등 원래 상태로 돌아가되 animation 업데이트는 안해주기 위한 임시 상태
                UpdateIdle();
                break;
        }
    }
    #endregion

    #region isGround
    private Collider2D _platformCollider;    // Platform에 착지하면 해당 플랫폼의 collider 기억, 이후 해당 콜라이더에서 떨어지면 점프중인걸로 판별

    public void OnCollisionEnter2D(Collision2D collision)   // Platform에 닿았는지 체크
    {
        // OnCollision 발생 시
        // 충돌 지점의 y 좌표가 플레이어 collider 아랫면(y 축의 최솟값)보다 작거나 같으면 바닥에 접촉했다고 판정
        // 점프해서 움직이는데 플레이어 콜라이더 양옆이나 위쪽에 뭐가 닿았을 시 _isGrounded = true가 되는것 방지
        if (collision.contacts.All((i) => (i.point.y <= _collider.bounds.min.y)))
        {
            _platformCollider = collision.collider;
            _isGrounded = true;

            if (State == CreatureState.Rolling) // 구르기 가속도 반동 초기화
            {
                _velocity.x = transform.localScale.x * StatData.MaxSpeed;
                _rigidbody.velocity = _velocity;
            }

            if (Input.GetKey(KeyCode.X) == false && State != CreatureState.Subskill)    // 스킬들 사용중에는 Land모션 재생 x
                State = CreatureState.Land;
            //Debug.Log("Landed");
            _coJumpCoolTimer = StartCoroutine("CoJumpCoolTimer", SkillData.JumpCoolTime);     // 착지 후 점프 0.1초 쿨타임 (애니메이션 꼬임 문제 방지)
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
