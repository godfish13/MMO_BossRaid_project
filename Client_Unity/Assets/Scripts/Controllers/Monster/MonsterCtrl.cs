using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
            if (_positionInfo.Equals(value))    // positionInfo�� ��ȭ�� ���涧�� Set
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
    //protected Rigidbody2D _rigidbody;
    //protected Collider2D _collider;           // ���� ���� ���� ����� �������� Anchor, isGrounded ����

    public int ClassId = 10;

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

    protected virtual void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        //_collider = GetComponent<Collider2D>();
        //_rigidbody = GetComponent<Rigidbody2D>();

        State = PositionInfo.State;
        transform.position = new Vector2(PositionInfo.PosX, PositionInfo.PosY);
        transform.localScale = new Vector2(-1, 1);

        _isHpbarOn = false;

        UpdateAnim();
    }

    void Start()
    {
        Init();
    }

    private bool _isHpbarOn;
    void Update()
    {
        SyncPos();
        if (State != CreatureState.Await && _isHpbarOn == false)
        {
            Managers.UIMgr.ShowSceneUI<UI_MonsterHpbar>("UI_MonsterHpbar");
            _isHpbarOn = true;
        }
    }

    public void SyncPos()
    {
        // ��ȭ ������ �������� �۵����� �ʵ��� ���� �߰�
        if (State != PositionInfo.State || transform.position.x != PositionInfo.PosX || transform.position.y != PositionInfo.PosY || transform.localScale.x != PositionInfo.LocalScaleX)
        {
            State = PositionInfo.State;
            
            transform.position = new Vector2(PositionInfo.PosX, transform.position.y);
            transform.localScale = new Vector2(PositionInfo.LocalScaleX, 1);
            //Debug.Log($"{GameObjectId} : {PositionInfo.PosX}, {PositionInfo.PosY}, {PositionInfo.LocalScaleX}");
        }
    }

    #region Animation
    protected void UpdateAnim()
    {
        if (_animator == null || _spriteRenderer == null)   // ������ ���� �ʱ�ȭ �ȵȻ��¸� return
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
    // Server���� ���� �� �۵�
    #endregion

    #region Server AI ����
    public void targetSetting(GameObject target)
    {
        //Target = target;
    }

    
    #endregion
}
