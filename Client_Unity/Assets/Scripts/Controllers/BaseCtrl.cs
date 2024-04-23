using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CollectionScripts;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BaseCtrl : MonoBehaviour
{
    [SerializeField] protected CreatureState State;

    Animator _animator;
    SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Rigidbody2D _rigidbody;

    StatInfo _statInfo = new StatInfo();
    public StatInfo StatInfo
    {
        get { return _statInfo; }
        set
        {
            if (_statInfo.Equals(value))
                return;

            _statInfo.Hp = value.Hp;
            _statInfo.Acceleration = value.Acceleration;
            _statInfo.SkillPower = value.SkillPower;
            _statInfo.SubSkillPower = value.SubSkillPower;
            _statInfo.SkillCoolTime = value.SkillCoolTime;
            _statInfo.SubSkillCoolTime = value.SubSkillCoolTime;
        }
    }

    protected virtual void Init()
    {
        Vector3 pos = new Vector3(0, -3.0f, 0);
        transform.position = pos;
        _animator = gameObject.GetComponent<Animator>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();

        UpdateAnim();
    }

    void Start()
    {
        Init();
    }

    void FixedUpdate()  // Update에서 실행하면 너무 빠르게 가속도처리됨
    {
        UpdateCtrl();
    }

    protected void UpdateAnim()
    {

    }

    #region Update series
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
    
    [SerializeField] Vector2 _input = new Vector2();    // 화살표 키입력
    [SerializeField] Vector2 velocity;                  // 가속도에따른 속력
    [SerializeField] bool _isGrounded = true;           // 땅에 붙어있는지 판별
    [SerializeField] float _MaxSpeed = 10.0f;           // 최고속도 -> Json 연동 todo
    [SerializeField] float _acceleration = 40.0f;       // 가속도 -> Json 연동 todo
    
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
        if (Input.GetKey(KeyCode.UpArrow))
            _input.y = 1;
        else if (Input.GetKey(KeyCode.DownArrow))
            _input.y = -1;
        else
            _input.y = 0;

        // Idle
        if (_input.x == 0 && _input.y == 0 && _isGrounded == true)
            State = CreatureState.Idle;
    }

    #region Move
    protected void Move()
    {
        velocity = _rigidbody.velocity; // 현재 속도 tmp저장
      
        if (_input.x == 0)   // 좌우 입력 없을 시 브레이크
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, _acceleration * 2 * Time.fixedDeltaTime);
            // rg의 x속도 가속도*3만큼 0까지 감속

            if (_isGrounded == true && _input.y == -1)
                State = CreatureState.Crouch;
        }
        else  // 입력 있을 시 가속
        {
            if (_isGrounded == true)
            {
                if (_input.y == -1) // 기어다니고 있으면 이동속도 하락
                {
                    State = CreatureState.Crawl;
                    velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * _MaxSpeed * 0.3f, _acceleration * 0.3f * Time.fixedDeltaTime);
                }
                else
                {
                    State = CreatureState.Run;
                    velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * _MaxSpeed, _acceleration * Time.fixedDeltaTime);
                    // MoveTowards : rg의 x속도, 최대 _MaxSpeed까지, 시간당 가속도만큼 가속
                }
            }
            else
            {
                velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * _MaxSpeed, _acceleration * 0.5f * Time.fixedDeltaTime);
                // 체공중일 시 가속도 절반
            }       
        }
 
        _rigidbody.velocity = velocity;   // 현재 속도 조절
    }
    #endregion

    #region Jump
    float _jumpForce = 1000.0f;    // 점프력 // Json 연동 필요한가? 일단 보류
    float Gravity = 200.0f;        // 중력   // Json 연동 필요한가? 일단 보류

    protected void Jump()
    {
        if (_isGrounded == true)
        {
            if (_input.y > 0)
            {
                State = CreatureState.Jump;
                _rigidbody.AddForce(Vector2.up * _jumpForce);
            }
        }
        else
        {
            velocity.y -= Gravity * Time.fixedDeltaTime;
        }

        _rigidbody.velocity = velocity;
    }

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
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (_isGrounded == true && collision.collider == _platformCollider)
        {
            _isGrounded = false;
        }
    }
    #endregion
}
