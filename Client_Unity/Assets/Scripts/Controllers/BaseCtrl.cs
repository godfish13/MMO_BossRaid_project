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

    void FixedUpdate()  // Update���� �����ϸ� �ʹ� ������ ���ӵ�ó����
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
    
    [SerializeField] Vector2 _input = new Vector2();    // ȭ��ǥ Ű�Է�
    [SerializeField] Vector2 velocity;                  // ���ӵ������� �ӷ�
    [SerializeField] bool _isGrounded = true;           // ���� �پ��ִ��� �Ǻ�
    [SerializeField] float _MaxSpeed = 10.0f;           // �ְ�ӵ� -> Json ���� todo
    [SerializeField] float _acceleration = 40.0f;       // ���ӵ� -> Json ���� todo
    
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

        // Idle
        if (_input.x == 0 && _input.y == 0 && _isGrounded == true)
            State = CreatureState.Idle;
    }

    #region Move
    protected void Move()
    {
        velocity = _rigidbody.velocity; // ���� �ӵ� tmp����
      
        if (_input.x == 0)   // �¿� �Է� ���� �� �극��ũ
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, _acceleration * 2 * Time.fixedDeltaTime);
            // rg�� x�ӵ� ���ӵ�*3��ŭ 0���� ����

            if (_isGrounded == true && _input.y == -1)
                State = CreatureState.Crouch;
        }
        else  // �Է� ���� �� ����
        {
            if (_isGrounded == true)
            {
                if (_input.y == -1) // ���ٴϰ� ������ �̵��ӵ� �϶�
                {
                    State = CreatureState.Crawl;
                    velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * _MaxSpeed * 0.3f, _acceleration * 0.3f * Time.fixedDeltaTime);
                }
                else
                {
                    State = CreatureState.Run;
                    velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * _MaxSpeed, _acceleration * Time.fixedDeltaTime);
                    // MoveTowards : rg�� x�ӵ�, �ִ� _MaxSpeed����, �ð��� ���ӵ���ŭ ����
                }
            }
            else
            {
                velocity.x = Mathf.MoveTowards(_rigidbody.velocity.x, _input.x * _MaxSpeed, _acceleration * 0.5f * Time.fixedDeltaTime);
                // ü������ �� ���ӵ� ����
            }       
        }
 
        _rigidbody.velocity = velocity;   // ���� �ӵ� ����
    }
    #endregion

    #region Jump
    float _jumpForce = 1000.0f;    // ������ // Json ���� �ʿ��Ѱ�? �ϴ� ����
    float Gravity = 200.0f;        // �߷�   // Json ���� �ʿ��Ѱ�? �ϴ� ����

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
