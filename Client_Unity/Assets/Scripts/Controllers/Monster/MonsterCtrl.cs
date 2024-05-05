using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCtrl : MonoBehaviour
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

    SpriteRenderer _spriteRenderer;
    Animator _animator;
    protected Rigidbody2D _rigidbody;
    protected Collider2D _collider;           // 무적 도중 지형 통과를 막기위한 Anchor, isGrounded 판정

    public int ClassId = 10;

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

    protected virtual void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        //_collider = GetComponent<Collider2D>();
        //_rigidbody = GetComponent<Rigidbody2D>();

        transform.position = new Vector2(PositionInfo.PosX, PositionInfo.PosY);

        UpdateAnim();
    }

    void Start()
    {
        Init();
    }

    #region Animation
    protected void UpdateAnim()
    {
        if (_animator == null || _spriteRenderer == null)   // 각각이 아직 초기화 안된상태면 return
            return;

        switch (State)
        {
            case CreatureState.Await:
                _animator.Play("IDLE");
                break;
            case CreatureState.Idle:
                _animator.Play("IDLE");
                break;
            case CreatureState.Run:
                _animator.Play("RUN");
                break;
            case CreatureState.Death:
                _animator.Play("DEATH");
                break;
            case CreatureState.Burn:
                _animator.Play("BURN");
                break;
            case CreatureState.Fireball:
                _animator.Play("FIREBALL");
                break;
            case CreatureState.Thunder:
                _animator.Play("THUNDER");
                break;
        }
    }
    #endregion

    #region UpdateCtrl series 
    // Server에서 연산 및 작동
    #endregion

    public void targetSetting(GameObject target)
    {
        //Target = target;
    }
}
