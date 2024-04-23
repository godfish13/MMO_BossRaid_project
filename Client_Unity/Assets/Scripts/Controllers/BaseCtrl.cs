using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseCtrl : MonoBehaviour
{
    Animator _animator;
    SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Rigidbody2D _rigidbody;

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
            _statInfo.Hp = value.Hp;
            _statInfo.MaxSpeed = value.MaxSpeed;
            _statInfo.Acceleration = value.Acceleration;
        }
    }

    SkillInfo _skillInfo = new SkillInfo();
    public SkillInfo Skill
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

    [SerializeField] Vector2 _input = new Vector2();    // ȭ��ǥ Ű�Է�
    Vector2 velocity;                  // ���ӵ������� �ӷ�
    float Gravity = 70.0f;        // �߷� ���ӵ�
    bool _isGrounded = true;           // ���� �پ��ִ��� �Ǻ�
    bool _jumpable = true;  // �������ɿ��� + ���� �� ������Ÿ�ӵ��� ��� ���� ������

    protected virtual void Init()
    {
        Vector3 pos = new Vector3(0, -3.0f, 0);
        transform.position = pos;
        _animator = gameObject.GetComponentInChildren<Animator>();
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        BindData(ClassId);

        UpdateAnim();
    }

    protected void BindData(int ClassId)    // Json���� ������ Stat�� Skill�� ���� // �⺻ Human, ���� Ŭ�������� ClassId �����ؼ� ����
    {
        Stat HumanStatData = null;
        Skill HumanSkillData = null;

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
            Skill.SkillDamage = HumanSkillData.SkillDamage;
            Skill.SkillCoolTime = HumanSkillData.SkillCoolTime;
            Skill.SubSkillDamage = HumanSkillData.SubSkillDamage;
            Skill.SubSkillCoolTime = HumanSkillData.SubSkillCoolTime;
            Skill.JumpPower = HumanSkillData.JumpPower;
            Skill.JumpCoolTime = HumanSkillData.JumpCoolTime;
        }
    }

    void Start()
    {
        Init();
    }

    void FixedUpdate()  // Update���� �����ϸ� ���ӵ�ó���� �ʹ� ����
    {
        UpdateCtrl();
    }

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

    #region UpdateCtrl series
    protected virtual void UpdateCtrl()
    {
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                UpdateIdle();
                break;
            case CreatureState.Run:
                GetDirInput();
                UpdateRun();
                break;
            case CreatureState.Jump:
                GetDirInput();
                UpdateJump();
                break;
            case CreatureState.Land:
                GetDirInput();
                UpdateJump();
                break;
            case CreatureState.Crouch:
                GetDirInput();
                UpdateCrawl();
                break;
            case CreatureState.Crawl:
                GetDirInput();
                UpdateCrawl();
                break;
            case CreatureState.Rolling:
                UpdateRolling();
                break;
            case CreatureState.Skill:
                GetDirInput();
                UpdateSkill();
                break;
            case CreatureState.Subskill:
                GetDirInput();
                UpdateSubSkill();
                break;
            case CreatureState.Death:
                UpdateDeath();
                break;
        }
    }

    private void UpdateIdle()
    {
        Move();
        Jump();
    }

    private void UpdateRun()
    {
        Move();
        Jump();
    }

    private void UpdateJump()
    {
        Move();
        Jump();
    }

    private void UpdateCrawl()
    {
        Move();
        Jump();
    }

    private void UpdateRolling()
    {

    }

    private void UpdateSkill()
    {

    }

    private void UpdateSubSkill()
    {

    }

    private void UpdateDeath()
    {

    }
    #endregion
    
    protected void GetDirInput()  // Ű �Է� �� ���� ����
    {
        // �¿��̵� �Է�
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _input.x = -1;
            transform.localScale = new Vector2(-1, 1);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _input.x = 1;
            transform.localScale = new Vector2(1, 1);
        }
        else
            _input.x = 0;

        // ���� �Է�
        if (Input.GetKey(KeyCode.UpArrow))
            _input.y = 1;
        else if (Input.GetKey(KeyCode.DownArrow))
            _input.y = -1;
        else
            _input.y = 0;
      
        if (_input.x == 0 && _input.y == 0 && _isGrounded && _jumpable)
            State = CreatureState.Idle;     // State Change flag
    }

    #region Move
    protected void Move()
    {
        velocity = _rigidbody.velocity; // ���� �ӵ� tmp����

        if (_input.x == 0)   // �¿� �Է� ���� �� �극��ũ
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, Stat.Acceleration * 2 * Time.fixedDeltaTime);
            // rg�� x�ӵ� ���ӵ�*3��ŭ 0���� ����

            if (_isGrounded && _input.y == -1)
                State = CreatureState.Crouch;   // State Change flag
        }
        else  // �Է� ���� �� ����
        {
            if (_isGrounded && _jumpable)    // ���� �� ������ ��� ���� ����
            {
                if (_input.y == -1) // ���ٴϰ� ������ �̵��ӵ� �϶�
                {
                    State = CreatureState.Crawl;    // State Change flag
                    velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed * 0.3f, Stat.Acceleration * 0.3f * Time.fixedDeltaTime);
                }
                else
                {
                    State = CreatureState.Run;      // State Change flag
                    velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed, Stat.Acceleration * Time.fixedDeltaTime);
                    // MoveTowards : rg�� x�ӵ�, �ִ� _MaxSpeed����, �ð��� ���ӵ���ŭ ����
                }
            }
            else
            {
                velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed, Stat.Acceleration * 0.5f * Time.fixedDeltaTime);
                // ü������ �� ���ӵ� ����
            }       
        }

        _rigidbody.velocity = velocity;   // ���� �ӵ� ����
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
                _rigidbody.AddForce(Vector2.up * Skill.JumpPower);
            }
        }
        else
        {
            velocity.y -= Gravity * Time.fixedDeltaTime;
        }

        _rigidbody.velocity = velocity;
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

            State = CreatureState.Land; // State Change flag
            _coSkillCoolTimer = StartCoroutine("CoJumpCoolTimer", Skill.JumpCoolTime);     // ���� �� ���� 0.1�� ��Ÿ�� (�ִϸ��̼� ���� ���� ����)
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

    #region CoolTimes
    // Jump
    Coroutine _coSkillCoolTimer;

    IEnumerator CoJumpCoolTimer(float time)
    {
        _jumpable = false;
        yield return new WaitForSeconds(time);
        _jumpable = true;
        _coSkillCoolTimer = null;
    }

    // SKill
    // Todo
    #endregion
}
