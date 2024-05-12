using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MyFurryCtrl : MyPlayerBaseCtrl
{
    protected BoxCollider2D _slashBox;   // Main Skill �������� ��Ʈ�ڽ�
    protected BoxCollider2D _bashBox;   // Sub Skill �������� ��Ʈ�ڽ�
    protected ParticleSystem BashEffect;
    protected GameObject GuardEffect;

    public override CreatureState State     // ��Ŷ ������ �κ� �����ϹǷ� override
    {
        get { return _state; }
        set
        {
            if (_state == value)    // Set�� ���ÿ� animation�����Ұ��̹Ƿ� ���������� Set�Ϸ��ϸ� return
                return;

            _state = value;
            PositionInfo.State = value;
            PacketSendingFlag = true;

            UpdateAnim();
        }
    }
    public bool isGuard = false;    // Sub Skill (����) ���� ����
    private bool _startBrake = false;   // Bash (Rolling) �� ����

    protected override void Init()
    {
        _hitBoxCollider = GetComponentsInChildren<Collider2D>()[1]; // 0 : Player / 1 : Player Hitbox / 2 : SlashBox / 3 : BashBox
        _slashBox = GetComponentsInChildren<BoxCollider2D>()[2];     
        _bashBox = GetComponentsInChildren<BoxCollider2D>()[3];  
        SlashEffect = GetComponentsInChildren<ParticleSystem>()[0]; // 0 : SlashEffect / 1 : BashEffect
        BashEffect = GetComponentsInChildren<ParticleSystem>()[1];
        GuardEffect = GetComponentInChildren<GuardEffect>().gameObject;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();

        transform.position = new Vector2(PositionInfo.PosX, PositionInfo.PosY);

        switch (ClassId)
        {
            case 0:
                _myHpbar = Managers.UIMgr.ShowSceneUI<UI_MyHpbar>($"UI_MyHpbar_Human");
                break;
            case 1:
                _myHpbar = Managers.UIMgr.ShowSceneUI<UI_MyHpbar>($"UI_MyHpbar_Elf");
                break;
            case 2:
                _myHpbar = Managers.UIMgr.ShowSceneUI<UI_MyHpbar>($"UI_MyHpbar_Furry");
                break;
        }
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, 3.2f, -8);
    }

    public override int Hp   // Hp ���� �� UI ǥ�� ��ġ ����
    {
        get { return StatData.Hp; }
        set
        {
            StatData.Hp = value;
            _myHpbar.HpbarChange((float)Hp / (float)MaxHp);
        }
    }

    #region Server ���
    protected bool PacketSendingFlag = false; // State ��ȭ, position���� �������� �̻� ��ȭ�� true

    protected override void Update()
    {
        if (Mathf.Abs(transform.position.x - PositionInfo.PosX) > 0.05f || Mathf.Abs(transform.position.y - PositionInfo.PosY) > 0.05f)
        {
            PositionInfo.PosX = transform.position.x;
            PositionInfo.PosY = transform.position.y;
            PositionInfo.LocalScaleX = transform.localScale.x;

            PacketSendingFlag = true;
        }

        if (PacketSendingFlag)
        {
            PacketSendingFlag = false;
            C_MovePacketSend();
        } 
    }

    private void C_MovePacketSend()
    {
        C_Move movePacket = new C_Move();

        movePacket.GameObjectId = GameObjectId;

        movePacket.PositionInfo = new PositionInfo();
        movePacket.PositionInfo.State = PositionInfo.State;
        movePacket.PositionInfo.PosX = PositionInfo.PosX;
        movePacket.PositionInfo.PosY = PositionInfo.PosY;
        movePacket.PositionInfo.LocalScaleX = PositionInfo.LocalScaleX;

        Managers.networkMgr.Send(movePacket);
    }
    #endregion

    #region UpdateCtrl series
    protected override void UpdateCtrl()
    {
        if (Hp <= 0)
            State = CreatureState.Death;

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

    protected override void UpdateIdle()   // �̵�, ������, MainSkill, SubSkill ����
    {
        Move();
        Jump();
        Fall();
        GetDirInput();
        GetRollingInput();
        GetSkillInput();
        GetSubSkillInput();
    }

    protected override void UpdateRun()    // �̵�, ������, MainSkill, SubSkill ����
    {
        Move();
        Jump();
        Fall();
        GetDirInput();
        GetRollingInput();
        GetSkillInput();
        GetSubSkillInput();
    }

    protected override void UpdateJump()    // �̵�, MainSkill, SubSkill ����
    {
        Move();
        Jump();
        Fall();
        GetDirInput();
        GetSkillInput();
        GetSubSkillInput();
    }

    protected override void UpdateFall()   // �̵�, MainSkill, SubSkill ����
    {
        Move();
        Jump();
        Fall();
        GetDirInput();
        GetSkillInput();
        GetSubSkillInput();
    }

    protected override void UpdateLand()   // �̵�, MainSkill ����
    {
        Move();
        Jump();
        Fall();
        GetDirInput();
        GetSkillInput();
    }

    protected override void UpdateCrawl()  // �̵�, ������ ����
    {
        Move();
        Jump();
        Fall();
        GetDirInput();
        GetRollingInput();
    }

    protected override void UpdateRolling()     // �ٸ� �ൿ �Ұ�
    {
        Fall();
        Rolling();
    }

    protected override void UpdateSkill()  // �̵�, ��ų ����
    {
        MoveWhileSkill();
        JumpWhileSkill();
        Fall();
        GetDirInput();
        GetSkillInput();
        GetSubSkillInput();
    }

    protected override void UpdateSubSkill()     // ��ų ����
    {
        JumpWhileSkill();
        Fall();
        GetDirInput();
        GetSkillInput();
        GetSubSkillInput();
        BrakeIfSubSkill();
    }

    protected override void UpdateDeath()
    {
        _rigidbody.velocity = Vector3.zero;
        if (_hitBoxCollider.enabled == true)
        {
            GuardEffect.SetActive(false);
            _hitBoxCollider.enabled = false;
        }
    }
    #endregion

    #region Get Input // MyCtrl Series������ �Է� ����
    // Dirtection Input
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
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.C))
            _input.y = 1;
        else if (Input.GetKey(KeyCode.DownArrow))
            _input.y = -1;
        else
            _input.y = 0;

        if (_input.x == 0 && _input.y == 0 && _isGrounded && _jumpable && State != CreatureState.Skill && State != CreatureState.Subskill && State != CreatureState.Rolling)
            State = CreatureState.Idle;   
    }

    // Roll Input
    protected void GetRollingInput()
    {
        // ������ �Է�
        if (_isGrounded && _isRollingOn && Input.GetKey(KeyCode.Z))
        {
            State = CreatureState.Rolling;  
        }
    }

    // Main Skill Input
    protected void GetSkillInput()
    {
        if (Input.GetKey(KeyCode.X))
        {
            if (_isSkill == false)
                _isSkill = true;
            State = CreatureState.Skill; 

            // Skill Packet Send Todo
        }
        else
        {
            if (State == CreatureState.Skill && _isSkill == false)
                State = CreatureState.Tmp;  

            // Tmp Packet Send Todo
        }
    }

    // Sub Skill Input
    protected void GetSubSkillInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (isGuard == false)
                isGuard = true;
            State = CreatureState.Subskill;

            GuardEffect.SetActive(true);        // ��������Ʈ ON
            // Skill Packet Send Todo
        }
        else
        {
            isGuard = false;
            if (State == CreatureState.Subskill)
                State = CreatureState.Tmp;

            GuardEffect.SetActive(false);       // ��������Ʈ OFF
            // Tmp Packet Send Todo
        }
    }
    #endregion

    #region MainSkill       
    protected override void AnimEvent_MainSkillSlashOn()
    {
        _slashBox.enabled = true;
        _slashBox.transform.localPosition = new Vector2(0.1f, 0); // �浹 ������ ���� ��¦ ������ �̵�

        SlashEffect.Play();
    }

    protected override void AnimEvent_MainSkillFrameEnded()
    {
        if (_isSkill)
            _isSkill = false;
        // AnimEvent : Skill �ִϸ��̼� ������ ������ State��ȭ ����

        _slashBox.enabled = false;
        _slashBox.transform.localPosition = new Vector2(0, 0);    // ��ġ ����

        SlashEffect.Stop();
    }
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == MonsterLayerMask)
        {
            Debug.Log("Slash Hitted!");
        }

        // Hpdelta Packet Send
        SendMonsterHpdeltaPacket(collision, MonsterLayerMask, (int)Define.SkillId.Human_Slash);
    }
    #endregion

    #region SubSkill      
    protected override void AnimEvent_SubSkill()
    {

    }

    protected override void AnimEvent_SubSkillFrameEnded()
    {
        State = CreatureState.Tmp;  
        // AnimEvent : SubSkill �ִϸ��̼� ������ ������ ���º�ȭ X
    }
    // Hit ���� OnTriggerEnter2D�� BombCtrl�� ����
    #endregion

    #region Rolling        Furry Knight�� ������ ��� ���� ����   
    protected override void Rolling()   // �转�߿� ����
    {
        if (_startBrake)
        {
            _velocity.x -= transform.localScale.x * 1.0f;
            _rigidbody.velocity = _velocity;
        }
    }

    protected override void AnimEvent_RollingStart()  // �转 ���۽� ����
    {
        _velocity.x = 0;
        _rigidbody.velocity = _velocity;
    }

    protected void AnimEvent_Rolling()  // �ް��� �� ����
    {
        _velocity.x = transform.localScale.x * StatData.MaxSpeed * 3;
        _rigidbody.velocity = _velocity;
        _startBrake = true;

        _bashBox.enabled = true;
        _bashBox.transform.localPosition = new Vector2(0.1f, 0);
        BashEffect.Play();
    }

    protected override void AnimEvent_RollingEnded()
    {
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
        _startBrake = false;

        _bashBox.enabled = true;
        _bashBox.transform.localPosition = new Vector2(0, 0);
        BashEffect.Stop();
        
        State = CreatureState.Tmp;
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
                _velocity.x = transform.localScale.x * StatData.MaxSpeed;
                _rigidbody.velocity = _velocity;
            }

            if (Input.GetKey(KeyCode.X) == false && State != CreatureState.Subskill)    // ��ų�� ����߿��� Land��� ��� x
                State = CreatureState.Land; 
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
}
