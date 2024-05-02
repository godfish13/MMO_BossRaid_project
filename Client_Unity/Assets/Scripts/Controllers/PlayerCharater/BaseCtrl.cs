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

    [SerializeField] private bool PacketSenderFlag = false;  // PosInfo (State set, transform X Y 의 값이 변하면 true)

    private PositionInfo _positionInfo = new PositionInfo();
    public PositionInfo PositionInfo             // State, X, Y, LocalScaleX
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))    // positionInfo에 변화가 생길때만 Set
                return;

            _positionInfo.State = value.State; 
            _positionInfo.PosX = value.PosX;
            _positionInfo.PosY = value.PosY;
            _positionInfo.LocalScaleX = value.LocalScaleX;
        }
    } 
    #endregion

    SpriteRenderer _spriteRenderer;
    Animator _animator;
    protected Rigidbody2D _rigidbody;
    protected Collider2D _collider;           // 무적 도중 지형 통과를 막기위한 Anchor, isGrounded 판정
    
    public int ClassId = 0;

    [SerializeField] protected CreatureState _state;
    public virtual CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)    // Set과 동시에 animation변경할것이므로 같은값으로 Set하려하면 return
                return;

            _state = value;
            PositionInfo.State = value;

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

    [SerializeField] protected Vector2 _input = new Vector2();    // 화살표 키입력
    protected Vector2 velocity;                  // 가속도에따른 속력
    float Gravity = 70.0f;        // 중력 가속도
    public bool _isGrounded = true;           // 땅에 붙어있는지 판별
    protected bool _isSkill = false; // 스킬키 한번 누르면 스킬 사용도중에 x키를 떼도 애니메이션 끝까지 사용되도록 판정용

    protected virtual void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();

        transform.position = new Vector2(PositionInfo.PosX, PositionInfo.PosY);

        BindData(ClassId);

        UpdateAnim();
    }

    protected void BindData(int ClassId)    // Json파일 데이터 Stat과 Skill에 연결 // 기본 Human, 하위 클래스에서 ClassId 변경
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

    void FixedUpdate()  // Update에서 실행하면 가속도처리가 너무 빠름
    {
        UpdateCtrl();
    }

    #region Animation
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

    #region UpdateCtrl series   // 상속받는 MyCtrl series에서 override로 구현
    protected virtual void UpdateCtrl()
    {
    }

    protected virtual void UpdateIdle()   // 이동, 구르기, MainSkill, SubSkill 가능
    {
    }

    protected virtual void UpdateRun()    // 이동, 구르기, MainSkill, SubSkill 가능
    {
    }

    protected virtual void UpdateJump()    // 이동, MainSkill, SubSkill 가능
    {
    }

    protected virtual void UpdateFall()   // 이동, MainSkill, SubSkill 가능
    {
    }

    protected virtual void UpdateLand()   // 이동, MainSkill 가능
    {
    }

    protected virtual void UpdateCrawl()  // 이동, 구르기 가능
    {
    }

    protected virtual void UpdateRolling()     // 다른 행동 불가
    {
    }

    protected virtual void UpdateSkill()  // 이동, 스킬 가능
    {
    }

    protected virtual void UpdateSubSkill()     // 다른 행동 불가
    {
    }

    protected virtual void UpdateDeath()
    {
    }
    #endregion

    #region Move
    protected void Move()
    {
        velocity = _rigidbody.velocity; // 현재 속도 tmp저장

        if (State == CreatureState.Rolling) // 이동 중 구르기 입력시 State 혼동 방지
            return;

        if (_input.x == 0)   // 좌우 입력 없을 시 브레이크
        {
            if (_isGrounded == false)
                State = CreatureState.Fall;     // State Change flag

            velocity.x = Mathf.MoveTowards(velocity.x, 0, Stat.Acceleration * 10 * Time.fixedDeltaTime);
            // rg의 x속도 가속도*2로 0까지 감속

            if (_isGrounded && _input.y == -1)
                State = CreatureState.Crouch;   // State Change flag
        }
        else  // 입력 있을 시 가속
        {
            if (_isGrounded && _jumpable)    // 점프 후 착지시 잠깐 가속 무시 + 점프쿨타임동안 Land 모션 유지
            {
                if (_input.y == -1) // 기어다니고 있으면 이동속도 하락
                {
                    State = CreatureState.Crawl;    // State Change flag
                    velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed * 0.3f, Stat.Acceleration * Time.fixedDeltaTime);
                }
                else
                {
                    State = CreatureState.Run;      // State Change flag
                    velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed, Stat.Acceleration * Time.fixedDeltaTime);
                    // MoveTowards : rg의 x속도, 최대 _MaxSpeed까지, 시간당 가속도만큼 가속
                }
            }
            else if (_isGrounded == false)
            {
                if (_isSkill == false)      // 점프 중 스킬쓰고 애니메이션이 끝난 이후 쭉 떨어지는 경우
                    State = CreatureState.Fall; // State Change flag
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
                velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * Stat.MaxSpeed * 0.5f, Stat.Acceleration * 0.5f * Time.fixedDeltaTime);
                // 스킬쓰며 체공중일 시 이동속도 절반
            }
        }

        _rigidbody.velocity = velocity;   // 현재 속도 조절
    }

    protected void BrakeIfSubSkill()  // SubSkill 사용중이면 좌우이동 정지
    {
        if (State == CreatureState.Subskill && _isGrounded)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, Stat.Acceleration * Time.fixedDeltaTime);
            _rigidbody.velocity = velocity;
        }
        // rg의 x속도 가속도*2로 0까지 감속
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
                // AnimEvent : 점프 애니메이션 재생후 바로 체공애니메이션으로 변경
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
        // AnimEvent : 점프 애니메이션 재생후 바로 체공애니메이션으로 변경
    }
    #endregion

    #region Rolling
    protected void Rolling()
    {
        velocity.x = transform.localScale.x * Stat.MaxSpeed * 3;
        _rigidbody.velocity = velocity;
    }

    protected virtual void AnimEvent_RollingStart()   // 구르기 중 무적
    {
        
    }

    protected virtual void AnimEvent_RollingEnded()
    {
        State = CreatureState.Tmp;     // State Change flag
        _coRollingCoolTimer = StartCoroutine("CoRollingCoolTimer", SkillData.JumpCoolTime + 1.0f);
    }
    #endregion  

    #region Skills
    protected virtual void MainSkill()
    {
        // Class 별 개별구현
        if (Input.GetKey(KeyCode.X))
        {
            Debug.Log("No MainSkill implemented");
        }
    }

    protected virtual void SubSkill()
    {
        // Class 별 개별구현
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("No SubSkill implemented");
        }
    }
    #endregion

    #region CoolTimes
    // Jump
    protected bool _jumpable = true;  // 점프가능여부(쿨타임) + 착지 후 점프쿨타임동안 잠깐 가속 딜레이
    protected Coroutine _coJumpCoolTimer;
    IEnumerator CoJumpCoolTimer(float time)
    {
        _jumpable = false;
        yield return new WaitForSeconds(time);
        _jumpable = true;
        _coJumpCoolTimer = null;
    }

    // Rolling
    protected bool _isRollingOn = true;  // 점프가능여부(쿨타임) + 착지 후 점프쿨타임동안 잠깐 가속 딜레이
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
