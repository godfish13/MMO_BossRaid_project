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
    [SerializeField] private int _gameObjectId;
    public int GameObjectId { get { return _gameObjectId; } set { _gameObjectId = value; } }

    private PositionInfo _positionInfo = new PositionInfo();
    public PositionInfo PositionInfo             // State, X, Y, LocalScaleX
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))    // positionInfo�� ��ȭ�� ���涧�� Set
                return;

            _positionInfo.State = value.State; 
            _positionInfo.PosX = value.PosX;
            _positionInfo.PosY = value.PosY;
            _positionInfo.LocalScaleX = value.LocalScaleX;
        }
    }
    #endregion

    protected SpriteRenderer _spriteRenderer;
    protected Animator _animator;
    protected Rigidbody2D _rigidbody;
    protected Collider2D _collider;           // ���� ���� ���� ����� �������� Anchor, isGrounded ����    
    protected Collider2D _hitBoxCollider;     // �÷��̾� �ǰ����� HitBox

    public int ClassId = 0;

    [SerializeField] protected CreatureState _state;
    public virtual CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)    // Set�� ���ÿ� animation�����Ұ��̹Ƿ� ���������� Set�Ϸ��ϸ� return
                return;

            _state = value;
            PositionInfo.State = value;

            UpdateAnim(); 
        }
    }

    StatInfo _statInfo = new StatInfo();
    public StatInfo StatData
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

    #region Stat property
    protected UI_MyHpbar _myHpbar;

    public int MaxHp
    {
        get { return StatData.MaxHp; }
        set { StatData.MaxHp = value; }
    }

    public virtual int Hp   // Hp ���� �� Ui ǥ�� ��ġ ����
    {
        get { return StatData.Hp; }
        set
        {
            StatData.Hp = value;
        }
    }
    #endregion

    [SerializeField] protected Vector2 _input = new Vector2();    // ȭ��ǥ Ű�Է�
    protected Vector2 _velocity;                  // ���ӵ������� �ӷ�
    float Gravity = 70.0f;        // �߷� ���ӵ�
    public bool _isGrounded = true;           // ���� �پ��ִ��� �Ǻ�
    protected bool _isSkill = false; // ��ųŰ �ѹ� ������ ��ų ��뵵�߿� xŰ�� ���� �ִϸ��̼� ������ ���ǵ��� ������

    protected virtual void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        
        transform.position = new Vector2(PositionInfo.PosX, PositionInfo.PosY);

        UpdateAnim();
    }

    void Start()
    {
        Init();
    }

    #region server ���
    protected virtual void Update()
    {
        SyncPos();
    }

    public void SyncPos()
    {
        // ��ȭ ������ �������� �۵����� �ʵ��� ���� �߰�
        if (State != PositionInfo.State || transform.position.x != PositionInfo.PosX || transform.position.y != PositionInfo.PosY || transform.localScale.x != PositionInfo.LocalScaleX)
        {
            State = PositionInfo.State;

            //transform.position = Vector2.MoveTowards(transform.position, new Vector2(PositionInfo.PosX, PositionInfo.PosY), StatData.MaxSpeed * Time.deltaTime);
            // MoveToward ����� ����ȭ �ð��� �ʹ� ���� �׳� position�� �ٷ� �Ű��ִ� �ɷ�
            transform.position = new Vector2(PositionInfo.PosX, PositionInfo.PosY);
            transform.localScale = new Vector2(PositionInfo.LocalScaleX, 1);
            //Debug.Log($"{GameObjectId} : {PositionInfo.PosX}, {PositionInfo.PosY}, {PositionInfo.LocalScaleX}");
        }
    }

    protected LayerMask PlayerLayerMask = (int)Define.Layer.Player;
    protected LayerMask MonsterLayerMask = (int)Define.Layer.Monster;

    protected void SendMonsterHpdeltaPacket(Collider2D collision, LayerMask layerMask, int skillId)
    {
        if (collision.gameObject.layer == layerMask) // Collision�� Layer�� ������ Ÿ�� �ƴϸ� ����
        {
            C_Hpdelta hpdeltaPacket = new C_Hpdelta();
            hpdeltaPacket.AttackerGameObjectId = GameObjectId;
            hpdeltaPacket.HittedGameObjectId = collision.GetComponent<MonsterCtrl>().GameObjectId;
            hpdeltaPacket.SkillId = skillId;
            Managers.networkMgr.Send(hpdeltaPacket);
        }
    }
    #endregion

    void FixedUpdate()  // Update���� �����ϸ� ���ӵ�ó���� �ʹ� ����
    {
        UpdateCtrl();
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

    #region UpdateCtrl series   // ��ӹ޴� MyCtrl series���� override�� ����
    protected virtual void UpdateCtrl()
    {
    }

    protected virtual void UpdateIdle()   // �̵�, ������, MainSkill, SubSkill ����
    {
    }

    protected virtual void UpdateRun()    // �̵�, ������, MainSkill, SubSkill ����
    {
    }

    protected virtual void UpdateJump()    // �̵�, MainSkill, SubSkill ����
    {
    }

    protected virtual void UpdateFall()   // �̵�, MainSkill, SubSkill ����
    {
    }

    protected virtual void UpdateLand()   // �̵�, MainSkill ����
    {
    }

    protected virtual void UpdateCrawl()  // �̵�, ������ ����
    {
    }

    protected virtual void UpdateRolling()     // �ٸ� �ൿ �Ұ�
    {
    }

    protected virtual void UpdateSkill()  // �̵�, ��ų ����
    {
    }

    protected virtual void UpdateSubSkill()     // �ٸ� �ൿ �Ұ�
    {
    }

    protected virtual void UpdateDeath()
    {
    }
    #endregion

    #region Move
    protected void Move()
    {
        _velocity = _rigidbody.velocity; // ���� �ӵ� tmp����

        if (State == CreatureState.Rolling) // �̵� �� ������ �Է½� State ȥ�� ����
            return;

        if (_input.x == 0)   // �¿� �Է� ���� �� �극��ũ
        {
            if (_isGrounded == false)
                State = CreatureState.Fall;     // State Change flag

            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, StatData.Acceleration * 10 * Time.fixedDeltaTime);
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
                    _velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * StatData.MaxSpeed * 0.3f, StatData.Acceleration * Time.fixedDeltaTime);
                }
                else
                {
                    State = CreatureState.Run;      // State Change flag
                    _velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * StatData.MaxSpeed, StatData.Acceleration * Time.fixedDeltaTime);
                    // MoveTowards : rg�� x�ӵ�, �ִ� _MaxSpeed����, �ð��� ���ӵ���ŭ ����
                }
            }
            else if (_isGrounded == false)
            {
                if (_isSkill == false)      // ���� �� ��ų���� �ִϸ��̼��� ���� ���� �� �������� ���
                    State = CreatureState.Fall; // State Change flag
                _velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * StatData.MaxSpeed, StatData.Acceleration * 0.5f * Time.fixedDeltaTime);
                // ü������ �� ���ӵ� ����
            }
        }
       
        _rigidbody.velocity = _velocity;   // ���� �ӵ� ����
    }

    protected void MoveWhileSkill() // ��ų���� �߿� �̵� ��, ���º�ȭ ���� õõ�� ������
    {
        _velocity = _rigidbody.velocity; // ���� �ӵ� tmp����

        if (_input.x == 0)   // �¿� �Է� ���� �� �극��ũ
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, StatData.Acceleration * 2 * Time.fixedDeltaTime);
            // rg�� x�ӵ� ���ӵ�*3��ŭ 0���� ����
        }
        else  // �Է� ���� �� ����
        {
            if (_isGrounded && _jumpable)    // ���� �� ������ ��� ���� ����
            {
                _velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * StatData.MaxSpeed * 0.5f, StatData.Acceleration * 0.5f * Time.fixedDeltaTime);
                // ��ų ����� �̵��ӵ� ����
            }
            else
            {
                _velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * StatData.MaxSpeed * 0.5f, StatData.Acceleration * 0.5f * Time.fixedDeltaTime);
                // ��ų���� ü������ �� �̵��ӵ� ����
            }
        }

        _rigidbody.velocity = _velocity;   // ���� �ӵ� ����
    }

    protected void BrakeIfSubSkill()  // SubSkill ������̸� �¿��̵� ����
    {
        if (State == CreatureState.Subskill && _isGrounded)
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, StatData.Acceleration * Time.fixedDeltaTime);
            _rigidbody.velocity = _velocity;
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

        _rigidbody.velocity = _velocity;
    }

    protected void Fall()
    {
        if (_isGrounded == false)
        {
            _velocity.y -= Gravity * Time.fixedDeltaTime;
            _rigidbody.velocity = _velocity;
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
        _velocity.x = transform.localScale.x * StatData.MaxSpeed * 3;
        _rigidbody.velocity = _velocity;
    }

    protected void AnimEvent_RollingStart()   // ������ �� ����
    {
        if (_hitBoxCollider == null)    // HumanCtrl���� �����Ƿ� return
            return;

        _hitBoxCollider.enabled = false;
    }

    protected void AnimEvent_RollingEnded()
    {
        if (_hitBoxCollider == null)
            return;

        _hitBoxCollider.enabled = true;

        if (_input.y == -1) // ������ ������ �ӵ� ���¿� ���� �ʱ�ȭ(����)
        {
            _velocity.x = transform.localScale.x * StatData.MaxSpeed * 0.3f;
            _rigidbody.velocity = _velocity;
        }
        else
        {
            _velocity.x = transform.localScale.x * StatData.MaxSpeed;
            _rigidbody.velocity = _velocity;
        }

        State = CreatureState.Tmp;     // State Change flag
        _coRollingCoolTimer = StartCoroutine("CoRollingCoolTimer", SkillData.JumpCoolTime + 1.0f);
    }
    #endregion  

    #region Skills
    // SkillId / 1: HumanSlash / 2: HumanBomb / 3: Elf ArrowShot / 4: Elf Knife / 5: Furry Slash
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
    protected Coroutine _coJumpCoolTimer;
    IEnumerator CoJumpCoolTimer(float time)
    {
        _jumpable = false;
        yield return new WaitForSeconds(time);
        _jumpable = true;
        _coJumpCoolTimer = null;
    }

    // Rolling
    protected bool _isRollingOn = true;  // �������ɿ���(��Ÿ��) + ���� �� ������Ÿ�ӵ��� ��� ���� ������
    protected Coroutine _coRollingCoolTimer;
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
    #endregion
}
