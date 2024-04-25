using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
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
            if (_state == value)    // Set과 동시에 animation변경할것이므로 같은값으로 Set하려하면 return
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

    [SerializeField] Vector2 _input = new Vector2();    // 화살표 키입력
    Vector2 velocity;                  // 가속도에따른 속력
    float Gravity = 70.0f;        // 중력 가속도
    bool _isGrounded = true;           // 땅에 붙어있는지 판별
    bool _jumpable = true;  // 점프가능여부 + 착지 후 점프쿨타임동안 잠깐 가속 딜레이

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

    protected void BindData(int ClassId)    // Json파일 데이터 Stat과 Skill에 연결 // 기본 Human, 하위 클래스에서 ClassId 변경해서 쓰기
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

    void FixedUpdate()  // Update에서 실행하면 가속도처리가 너무 빠름
    {
        UpdateCtrl();
    }

    protected void UpdateAnim()
    {
        if (_animator == null || _spriteRenderer == null)   // 각각이 아직 초기화 안된상태면 return
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
            case CreatureState.Tmp:  // 스킬 사용 후 Idle, Move 등 원래 상태로 돌아가되 animation 업데이트는 안해주기 위한 임시 상태
                GetDirInput();
                UpdateRun();
                break;
        }
    }

    private void UpdateIdle()
    {
        Move();
        Jump();
        MainSkill();
        SubSkill();
    }

    private void UpdateRun()
    {
        Move();
        Jump();
        MainSkill();
        SubSkill();
    }

    private void UpdateJump()
    {
        Move();
        Jump();
        MainSkill();
        SubSkill();
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
        MoveWhileSkill();
        Jump();
        MainSkill();
        SubSkill();
    }

    private void UpdateSubSkill()
    {
        MoveWhileSkill();
        MainSkill();
        SubSkill();
    }

    private void UpdateDeath()
    {

    }
    #endregion

    protected void GetDirInput()  // 키 입력 시 상태 지정
    {
        // 좌우이동 입력
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

        // 점프 입력
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.C))
            _input.y = 1;
        else if (Input.GetKey(KeyCode.DownArrow))
            _input.y = -1;
        else
            _input.y = 0;

        if (_input.x == 0 && _input.y == 0 && _isGrounded && _jumpable && State != CreatureState.Skill)
            State = CreatureState.Idle;     // State Change flag
    }

    #region Move
    protected void Move()
    {
        velocity = _rigidbody.velocity; // 현재 속도 tmp저장
        
        if (_input.x == 0)   // 좌우 입력 없을 시 브레이크
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, Stat.Acceleration * 2 * Time.fixedDeltaTime);
            // rg의 x속도 가속도*3만큼 0까지 감속

            if (_isGrounded && _input.y == -1)
                State = CreatureState.Crouch;   // State Change flag
        }
        else  // 입력 있을 시 가속
        {
            if (_isGrounded && _jumpable)    // 점프 후 착지시 잠깐 가속 무시
            {
                if (_input.y == -1) // 기어다니고 있으면 이동속도 하락
                {
                    State = CreatureState.Crawl;    // State Change flag
                    velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed * 0.3f, Stat.Acceleration * 0.3f * Time.fixedDeltaTime);
                }
                else
                {
                    State = CreatureState.Run;      // State Change flag
                    velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed, Stat.Acceleration * Time.fixedDeltaTime);
                    // MoveTowards : rg의 x속도, 최대 _MaxSpeed까지, 시간당 가속도만큼 가속
                }
            }
            else
            {
                velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed, Stat.Acceleration * 0.5f * Time.fixedDeltaTime);
                // 체공중일 시 가속도 절반
            }
        }
       
        _rigidbody.velocity = velocity;   // 현재 속도 조절
    }

    protected void MoveWhileSkill() // 스킬쓰는 중에 이동 시, 상태변화 없이 천천히 움직임
    {
        velocity = _rigidbody.velocity; // 현재 속도 tmp저장

        if (_input.x == 0)   // 좌우 입력 없을 시 브레이크
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, Stat.Acceleration * 2 * Time.fixedDeltaTime);
            // rg의 x속도 가속도*3만큼 0까지 감속
        }
        else  // 입력 있을 시 가속
        {
            if (_isGrounded && _jumpable)    // 점프 후 착지시 잠깐 가속 무시
            {
                velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed * 0.5f, Stat.Acceleration * 0.5f * Time.fixedDeltaTime);
                // 스킬 사용중 이동속도 절반
            }
            else
            {
                velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed, Stat.Acceleration * 0.5f * Time.fixedDeltaTime);
                // 체공중일 시 가속도 절반
            }
        }

        _rigidbody.velocity = velocity;   // 현재 속도 조절
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

            State = CreatureState.Land; // State Change flag
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

    #region Skills
    protected virtual void MainSkill()
    {
        // Class 별 개별구현
    }

    protected virtual void SubSkill()
    {
        // Class 별 개별구현
    }
    #endregion

    #region CoolTimes
    // Jump
    private Coroutine _coJumpCoolTimer;
    protected Coroutine _coSkillCoolTimer;

    IEnumerator CoJumpCoolTimer(float time)
    {
        _jumpable = false;
        yield return new WaitForSeconds(time);
        _jumpable = true;
        _coJumpCoolTimer = null;
    }

    // SKill
    protected bool _isSkill = false;    // 스킬키 한번 누르면 스킬 사용도중에 x키를 떼도 애니메이션 끝까지 사용되도록 체크하는 플래그
    IEnumerator CoSkillCoolTimer(float time)
    {
        _isSkill = true;   
        yield return new WaitForSeconds(time);

        if (Input.GetKey(KeyCode.X) == false)
            _isSkill = false;
    }
    #endregion
}
