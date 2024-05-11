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
            if (_positionInfo.Equals(value))    // positionInfo에 변화가 생길때만 Set
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
    protected Collider2D _collider;           // 무적 도중 지형 통과를 막기위한 Anchor, isGrounded 판정    
    protected Collider2D _hitBoxCollider;     // 플레이어 피격판정 HitBox

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

    public virtual int Hp   // Hp 변동 시 Ui 표시 수치 변경
    {
        get { return StatData.Hp; }
        set
        {
            StatData.Hp = value;
        }
    }
    #endregion

    [SerializeField] protected Vector2 _input = new Vector2();    // 화살표 키입력
    protected Vector2 _velocity;                  // 가속도에따른 속력
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

        UpdateAnim();
    }

    void Start()
    {
        Init();
    }

    #region server 통신
    protected virtual void Update()
    {
        SyncPos();
    }

    public void SyncPos()
    {
        // 변화 없을땐 쓸데없이 작동하지 않도록 조건 추가
        if (State != PositionInfo.State || transform.position.x != PositionInfo.PosX || transform.position.y != PositionInfo.PosY || transform.localScale.x != PositionInfo.LocalScaleX)
        {
            State = PositionInfo.State;

            //transform.position = Vector2.MoveTowards(transform.position, new Vector2(PositionInfo.PosX, PositionInfo.PosY), StatData.MaxSpeed * Time.deltaTime);
            // MoveToward 방식은 동기화 시간이 너무 느림 그냥 position을 바로 옮겨주는 걸로
            transform.position = new Vector2(PositionInfo.PosX, PositionInfo.PosY);
            transform.localScale = new Vector2(PositionInfo.LocalScaleX, 1);
            //Debug.Log($"{GameObjectId} : {PositionInfo.PosX}, {PositionInfo.PosY}, {PositionInfo.LocalScaleX}");
        }
    }

    protected LayerMask PlayerLayerMask = (int)Define.Layer.Player;
    protected LayerMask MonsterLayerMask = (int)Define.Layer.Monster;

    protected void SendMonsterHpdeltaPacket(Collider2D collision, LayerMask layerMask, int skillId)
    {
        if (collision.gameObject.layer == layerMask) // Collision의 Layer가 지정한 타입 아니면 무시
        {
            C_Hpdelta hpdeltaPacket = new C_Hpdelta();
            hpdeltaPacket.AttackerGameObjectId = GameObjectId;
            hpdeltaPacket.HittedGameObjectId = collision.GetComponent<MonsterCtrl>().GameObjectId;
            hpdeltaPacket.SkillId = skillId;
            Managers.networkMgr.Send(hpdeltaPacket);
        }
    }
    #endregion

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
        _velocity = _rigidbody.velocity; // 현재 속도 tmp저장

        if (State == CreatureState.Rolling) // 이동 중 구르기 입력시 State 혼동 방지
            return;

        if (_input.x == 0)   // 좌우 입력 없을 시 브레이크
        {
            if (_isGrounded == false)
                State = CreatureState.Fall;     // State Change flag

            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, StatData.Acceleration * 10 * Time.fixedDeltaTime);
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
                    _velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * StatData.MaxSpeed * 0.3f, StatData.Acceleration * Time.fixedDeltaTime);
                }
                else
                {
                    State = CreatureState.Run;      // State Change flag
                    _velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * StatData.MaxSpeed, StatData.Acceleration * Time.fixedDeltaTime);
                    // MoveTowards : rg의 x속도, 최대 _MaxSpeed까지, 시간당 가속도만큼 가속
                }
            }
            else if (_isGrounded == false)
            {
                if (_isSkill == false)      // 점프 중 스킬쓰고 애니메이션이 끝난 이후 쭉 떨어지는 경우
                    State = CreatureState.Fall; // State Change flag
                _velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * StatData.MaxSpeed, StatData.Acceleration * 0.5f * Time.fixedDeltaTime);
                // 체공중일 시 가속도 절반
            }
        }
       
        _rigidbody.velocity = _velocity;   // 현재 속도 조절
    }

    protected void MoveWhileSkill() // 스킬쓰는 중에 이동 시, 상태변화 없이 천천히 움직임
    {
        _velocity = _rigidbody.velocity; // 현재 속도 tmp저장

        if (_input.x == 0)   // 좌우 입력 없을 시 브레이크
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, StatData.Acceleration * 2 * Time.fixedDeltaTime);
            // rg의 x속도 가속도*3만큼 0까지 감속
        }
        else  // 입력 있을 시 가속
        {
            if (_isGrounded && _jumpable)    // 점프 후 착지시 잠깐 가속 무시
            {
                _velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * StatData.MaxSpeed * 0.5f, StatData.Acceleration * 0.5f * Time.fixedDeltaTime);
                // 스킬 사용중 이동속도 절반
            }
            else
            {
                _velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * StatData.MaxSpeed * 0.5f, StatData.Acceleration * 0.5f * Time.fixedDeltaTime);
                // 스킬쓰며 체공중일 시 이동속도 절반
            }
        }

        _rigidbody.velocity = _velocity;   // 현재 속도 조절
    }

    protected void BrakeIfSubSkill()  // SubSkill 사용중이면 좌우이동 정지
    {
        if (State == CreatureState.Subskill && _isGrounded)
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, StatData.Acceleration * Time.fixedDeltaTime);
            _rigidbody.velocity = _velocity;
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
        // AnimEvent : 점프 애니메이션 재생후 바로 체공애니메이션으로 변경
    }
    #endregion

    #region Rolling
    protected void Rolling()
    {
        _velocity.x = transform.localScale.x * StatData.MaxSpeed * 3;
        _rigidbody.velocity = _velocity;
    }

    protected void AnimEvent_RollingStart()   // 구르기 중 무적
    {
        if (_hitBoxCollider == null)    // HumanCtrl에는 없으므로 return
            return;

        _hitBoxCollider.enabled = false;
    }

    protected void AnimEvent_RollingEnded()
    {
        if (_hitBoxCollider == null)
            return;

        _hitBoxCollider.enabled = true;

        if (_input.y == -1) // 구르기 끝나면 속도 상태에 맞춰 초기화(감속)
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
