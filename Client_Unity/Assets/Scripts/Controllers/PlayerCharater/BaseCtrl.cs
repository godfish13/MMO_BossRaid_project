using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseCtrl : MonoBehaviour
{
    #region for Server Connection
    [SerializeField] private int _id;
    public int Id { get { return _id; } set { _id = value; } }

    private bool PacketSenderFlag = false;  // PosInfo (State, transform X Y �� ���� ���ϸ� true)

    private PositionInfo _positionInfo = new PositionInfo();
    public PositionInfo PosInfo             // State, X, Y
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))    // positionInfo�� ��ȭ�� ���涧�� Set
                return;

            _positionInfo.State = value.State; 
            _positionInfo.PosX = value.PosX;
            _positionInfo.PosY = value.PosY;
        }
    }

    public void SyncPos()
    {
        State = PosInfo.State;
        transform.position = new Vector2(PosInfo.PosX, PosInfo.PosY);
    }
    #endregion

    SpriteRenderer _spriteRenderer;
    Animator _animator;
    protected Rigidbody2D _rigidbody;
    private Collider2D _collider;           // ���� ���� ���� ����� �������� Anchor, isGrounded ����
    
    public int ClassId = 0;

    [SerializeField] protected CreatureState _state;
    public CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)    // Set�� ���ÿ� animation�����Ұ��̹Ƿ� ���������� Set�Ϸ��ϸ� return
                return;

            _state = value;
            PosInfo.State = value;
            PacketSenderFlag = true;
            UpdateAnim();
        }
    }

    StatInfo _statInfo = new StatInfo();
    public StatInfo Stat
    {
        get { return _statInfo; }
        set
        {
            if (_statInfo.Equals(value))
                return;

            _statInfo.ClassId = value.ClassId;
            _statInfo.Class = value.Class;
            _statInfo.MaxHp = value.MaxHp;
            _statInfo.Hp = value.Hp;
            _statInfo.MaxSpeed = value.MaxSpeed;
            _statInfo.Acceleration = value.Acceleration;
        }
    }

    SkillInfo _skillInfo = new SkillInfo();
    public SkillInfo SkillData
    {
        get { return _skillInfo; }
        set
        {
            if (_statInfo.Equals(value))
                return;

            _skillInfo.SkillDamage = value.SkillDamage;
            _skillInfo.SkillCoolTime = value.SkillCoolTime;
            _skillInfo.SubSkillDamage = value.SubSkillDamage;
            _skillInfo.SubSkillCoolTime = value.SubSkillCoolTime;
            _skillInfo.JumpPower = value.JumpPower;
            _skillInfo.JumpCoolTime = value.JumpCoolTime;
        }
    }

    [SerializeField] protected Vector2 _input = new Vector2();    // ȭ��ǥ Ű�Է�
    protected Vector2 velocity;                  // ���ӵ������� �ӷ�
    float Gravity = 70.0f;        // �߷� ���ӵ�
    public bool _isGrounded = true;           // ���� �پ��ִ��� �Ǻ�
    protected bool _isSkill = false; // ��ųŰ �ѹ� ������ ��ų ��뵵�߿� xŰ�� ���� �ִϸ��̼� ������ ���ǵ��� ������

    protected virtual void Init()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        transform.position = pos;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        
        BindData(ClassId);

        UpdateAnim();
    }

    protected void BindData(int ClassId)    // Json���� ������ Stat�� Skill�� ���� // �⺻ Human, ���� Ŭ�������� ClassId ����
    {
        StatInfo HumanStatData = null;
        SkillInfo HumanSkillData = null;

        if (Managers.dataMgr.StatDictionary.TryGetValue(ClassId, out HumanStatData))
        {
            Stat.ClassId = HumanStatData.ClassId;
            Stat.Class = HumanStatData.Class;
            Stat.Hp = HumanStatData.Hp;
            Stat.MaxSpeed = HumanStatData.MaxSpeed;
            Stat.Acceleration = HumanStatData.Acceleration;
        }
        if (Managers.dataMgr.SkillDictionary.TryGetValue(ClassId, out HumanSkillData))
        {
            SkillData.SkillDamage = HumanSkillData.SkillDamage;
            SkillData.SkillCoolTime = HumanSkillData.SkillCoolTime;
            SkillData.SubSkillDamage = HumanSkillData.SubSkillDamage;
            SkillData.SubSkillCoolTime = HumanSkillData.SubSkillCoolTime;
            SkillData.JumpPower = HumanSkillData.JumpPower;
            SkillData.JumpCoolTime = HumanSkillData.JumpCoolTime;
        }
    }

    void Start()
    {
        Init();
    }

    void FixedUpdate()  // Update���� �����ϸ� ���ӵ�ó���� �ʹ� ����
    {
        UpdateCtrl();

        #region Server ���
        if (PosInfo.PosX != transform.position.x || PosInfo.PosY != transform.position.y)   // ������¶� �޶��������� Set
        {
            PosInfo.PosX = transform.position.x;
            PosInfo.PosY = transform.position.y;
            PacketSenderFlag = true;
        }

        if (PacketSenderFlag && _packetCoolTime)    // PosInfo�� ��ȭ�� �ְ� PacketCoolTime on�̸� �۽�
        {
            Debug.Log("PacketSenderFlag!");
            PacketSenderFlag = false;
            C_MovePacketSend();
        }
        #endregion
    }

    public void C_MovePacketSend()
    {
        C_Move movePacket = new C_Move();

        movePacket.ObjectId = Id;
        movePacket.PositionInfo = PosInfo;

        Managers.networkMgr.Send(movePacket);
        _coPacketCoolTimer = StartCoroutine("CopacketCoolTimer", 0.25f);    // 0.25�ʸ��� ��Ŷ �۽� ���� 1�ʿ� 4��
    }

    #region Animation
    protected void UpdateAnim()
    {
        if (_animator == null || _spriteRenderer == null)   // ������ ���� �ʱ�ȭ �ȵȻ��¸� return
            return;

        switch (State)
        {
            case CreatureState.Idle:
                _animator.Play("IDLE");
                break;
            case CreatureState.Run:
                _animator.Play("RUN");
                break;
            case CreatureState.Jump:
                _animator.Play("JUMP");
                break;
            case CreatureState.Fall:
                _animator.Play("FALL");
                break;
            case CreatureState.Land:
                _animator.Play("LAND");
                break;
            case CreatureState.Crouch:
                _animator.Play("CROUCH");
                break;
            case CreatureState.Crawl:
                _animator.Play("CRAWL");
                break;
            case CreatureState.Rolling:
                _animator.Play("ROLLING");
                break;
            case CreatureState.Skill:
                _animator.Play("SKILL");
                break;
            case CreatureState.Subskill:
                _animator.Play("SUBSKILL");
                break;
            case CreatureState.Death:
                _animator.Play("DEATH");
                break;
        }
    }
    #endregion

    #region UpdateCtrl series
    protected virtual void UpdateCtrl()
    {
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
            case CreatureState.Death:   // ���
                UpdateDeath();
                break;
            case CreatureState.Tmp:  // ��ų ��� �� Idle, Move �� ���� ���·� ���ư��� animation ������Ʈ�� �����ֱ� ���� �ӽ� ����
                UpdateIdle();
                break;
        }
    }

    protected virtual void UpdateIdle()   // �̵�, ������, MainSkill, SubSkill ����
    {
        Move();
        Jump();
        Fall();
    }

    protected virtual void UpdateRun()    // �̵�, ������, MainSkill, SubSkill ����
    {
        Move();
        Jump();
        Fall();
    }

    protected virtual void UpdateJump()    // �̵�, MainSkill, SubSkill ����
    {
        Move();
        Jump();
        Fall();
    }

    protected virtual void UpdateFall()   // �̵�, MainSkill, SubSkill ����
    {
        Move();
        Jump();
        Fall();
    }

    protected virtual void UpdateLand()   // �̵�, MainSkill ����
    {
        Move();
        Jump();
        Fall();
    }

    protected virtual void UpdateCrawl()  // �̵�, ������ ����
    {
        Move();
        Jump();
        Fall();
    }

    protected virtual void UpdateRolling()     // �ٸ� �ൿ �Ұ�
    {
        Fall();
        Rolling();
    }

    protected virtual void UpdateSkill()  // �̵�, ��ų ����
    {
        MoveWhileSkill();
        JumpWhileSkill();
        Fall();
    }

    protected virtual void UpdateSubSkill()     // �ٸ� �ൿ �Ұ�
    {
        Fall();
        BrakeIfSubSkill();    // SubSkill ����ϸ� Brake
    }

    protected virtual void UpdateDeath()
    {

    }
    #endregion

    #region Move
    protected void Move()
    {
        velocity = _rigidbody.velocity; // ���� �ӵ� tmp����

        if (State == CreatureState.Rolling) // �̵� �� ������ �Է½� State ȥ�� ����
            return;

        if (_input.x == 0)   // �¿� �Է� ���� �� �극��ũ
        {
            if (_isGrounded == false)
                State = CreatureState.Fall;     // State Change flag

            velocity.x = Mathf.MoveTowards(velocity.x, 0, Stat.Acceleration * 10 * Time.fixedDeltaTime);
            // rg�� x�ӵ� ���ӵ�*2�� 0���� ����

            if (_isGrounded && _input.y == -1)
                State = CreatureState.Crouch;   // State Change flag
        }
        else  // �Է� ���� �� ����
        {
            if (_isGrounded && _jumpable)    // ���� �� ������ ��� ���� ���� + ������Ÿ�ӵ��� Land ��� ����
            {
                if (_input.y == -1) // ���ٴϰ� ������ �̵��ӵ� �϶�
                {
                    State = CreatureState.Crawl;    // State Change flag
                    velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed * 0.3f, Stat.Acceleration * Time.fixedDeltaTime);
                }
                else
                {
                    State = CreatureState.Run;      // State Change flag
                    velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed, Stat.Acceleration * Time.fixedDeltaTime);
                    // MoveTowards : rg�� x�ӵ�, �ִ� _MaxSpeed����, �ð��� ���ӵ���ŭ ����
                }
            }
            else if (_isGrounded == false)
            {
                if (_isSkill == false)      // ���� �� ��ų���� �ִϸ��̼��� ���� ���� �� �������� ���
                    State = CreatureState.Fall; // State Change flag
                velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed, Stat.Acceleration * 0.5f * Time.fixedDeltaTime);
                // ü������ �� ���ӵ� ����
            }
        }
       
        _rigidbody.velocity = velocity;   // ���� �ӵ� ����
    }

    protected void MoveWhileSkill() // ��ų���� �߿� �̵� ��, ���º�ȭ ���� õõ�� ������
    {
        velocity = _rigidbody.velocity; // ���� �ӵ� tmp����

        if (_input.x == 0)   // �¿� �Է� ���� �� �극��ũ
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, Stat.Acceleration * 2 * Time.fixedDeltaTime);
            // rg�� x�ӵ� ���ӵ�*3��ŭ 0���� ����
        }
        else  // �Է� ���� �� ����
        {
            if (_isGrounded && _jumpable)    // ���� �� ������ ��� ���� ����
            {
                velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed * 0.5f, Stat.Acceleration * 0.5f * Time.fixedDeltaTime);
                // ��ų ����� �̵��ӵ� ����
            }
            else
            {
                velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed * 0.5f, Stat.Acceleration * 0.5f * Time.fixedDeltaTime);
                // ��ų���� ü������ �� �̵��ӵ� ����
            }
        }

        _rigidbody.velocity = velocity;   // ���� �ӵ� ����
    }

    protected void BrakeIfSubSkill()  // SubSkill ������̸� �¿��̵� ����
    {
        if (State == CreatureState.Subskill && _isGrounded)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, Stat.Acceleration * Time.fixedDeltaTime);
            _rigidbody.velocity = velocity;
        }
        // rg�� x�ӵ� ���ӵ�*2�� 0���� ����
    }
    #endregion

    #region Jump
    protected void Jump()
    {
        if (_isGrounded && _jumpable)
        {
            if (_input.y > 0)
            {
                _isGrounded = false;

                State = CreatureState.Jump;     // State Change flag

                _rigidbody.AddForce(Vector2.up * SkillData.JumpPower);
                // AnimEvent : ���� �ִϸ��̼� ����� �ٷ� ü���ִϸ��̼����� ����
            }
        }
    }

    protected void JumpWhileSkill()
    {
        if (_isGrounded && _jumpable)
        {
            if (_input.y > 0)
            {
                _isGrounded = false;

                _rigidbody.AddForce(Vector2.up * SkillData.JumpPower);
            }
        }

        _rigidbody.velocity = velocity;
    }

    protected void Fall()
    {
        if (_isGrounded == false)
        {
            velocity.y -= Gravity * Time.fixedDeltaTime;
            _rigidbody.velocity = velocity;
        }
    }

    protected void AnimEvent_ChangeState2Fall()
    {
        State = CreatureState.Fall;     // State Change flag
        // AnimEvent : ���� �ִϸ��̼� ����� �ٷ� ü���ִϸ��̼����� ����
    }
    #endregion

    #region Rolling
    protected void Rolling()
    {
        velocity.x = transform.localScale.x * Stat.MaxSpeed * 3;
        _rigidbody.velocity = velocity;
    }

    protected virtual void AnimEvent_RollingStart()   // ������ �� ����
    {
        
    }

    protected virtual void AnimEvent_RollingEnded()
    {
        State = CreatureState.Tmp;     // State Change flag
        _coRollingCoolTimer = StartCoroutine("CoRollingCoolTimer", SkillData.JumpCoolTime + 1.0f);
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
                velocity.x = transform.localScale.x * Stat.MaxSpeed;
                _rigidbody.velocity = velocity;
            }

            if (Input.GetKey(KeyCode.X) == false && State != CreatureState.Subskill)    // ��ų�� ����߿��� Land��� ��� x
                State = CreatureState.Land; // State Change flag
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

    #region Skills
    protected virtual void MainSkill()
    {
        // Class �� ��������
        if (Input.GetKey(KeyCode.X))
        {
            Debug.Log("No MainSkill implemented");
        }
    }

    protected virtual void SubSkill()
    {
        // Class �� ��������
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("No SubSkill implemented");
        }
    }
    #endregion

    #region CoolTimes
    // Jump
    protected bool _jumpable = true;  // �������ɿ���(��Ÿ��) + ���� �� ������Ÿ�ӵ��� ��� ���� ������
    private Coroutine _coJumpCoolTimer;
    IEnumerator CoJumpCoolTimer(float time)
    {
        _jumpable = false;
        yield return new WaitForSeconds(time);
        _jumpable = true;
        _coJumpCoolTimer = null;
    }

    // Rolling
    protected bool _isRollingOn = true;  // �������ɿ���(��Ÿ��) + ���� �� ������Ÿ�ӵ��� ��� ���� ������
    private Coroutine _coRollingCoolTimer;
    IEnumerator CoRollingCoolTimer(float time)
    {
        _isRollingOn = false;
        yield return new WaitForSeconds(time);
        _isRollingOn = true;
        _coRollingCoolTimer = null;
    }

    // SKill
    protected bool _isSubSkillOn = true;
    protected Coroutine _coSubSkillCoolTimer;
    IEnumerator CoSubSkillCoolTimer(float time)
    {
        _isSubSkillOn = false;
        yield return new WaitForSeconds(time);
        _isSubSkillOn = true;
        _coSubSkillCoolTimer = null;
    }

    // Server ���
    protected bool _packetCoolTime = true;
    protected Coroutine _coPacketCoolTimer;
    IEnumerator CopacketCoolTimer(float time)
    {
        _packetCoolTime = false;
        yield return new WaitForSeconds(time);
        _packetCoolTime = true;
        _coPacketCoolTimer = null;
    }
    #endregion
}
