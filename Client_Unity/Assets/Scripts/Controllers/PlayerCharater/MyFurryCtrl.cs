using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MyFurryCtrl : MyPlayerBaseCtrl
{
    protected BoxCollider2D _slashBox;   // Main Skill 판정범위 히트박스
    protected BoxCollider2D _bashBox;   // Sub Skill 판정범위 히트박스
    protected ParticleSystem BashEffect;
    protected GameObject GuardEffect;

    public bool isGuard = false;    // Sub Skill (가드) 유지 판정
    private bool _startBrake = false;   // Bash (Rolling) 중 감속

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
            case CreatureState.Death:   // 사망
                UpdateDeath();
                break;
            case CreatureState.Tmp:  // 스킬 사용 후 Idle, Move 등 원래 상태로 돌아가되 animation 업데이트는 안해주기 위한 임시 상태
                UpdateIdle();
                break;
        }
    }

    protected override void UpdateIdle()   // 이동, 구르기, MainSkill, SubSkill 가능
    {
        Move();
        Jump();
        Fall();
        GetDirInput();
        GetRollingInput();
        GetSkillInput();
        GetSubSkillInput();
    }

    protected override void UpdateRun()    // 이동, 구르기, MainSkill, SubSkill 가능
    {
        Move();
        Jump();
        Fall();
        GetDirInput();
        GetRollingInput();
        GetSkillInput();
        GetSubSkillInput();
    }

    protected override void UpdateJump()    // 이동, MainSkill, SubSkill 가능
    {
        Move();
        Jump();
        Fall();
        GetDirInput();
        GetSkillInput();
        GetSubSkillInput();
    }

    protected override void UpdateFall()   // 이동, MainSkill, SubSkill 가능
    {
        Move();
        Jump();
        Fall();
        GetDirInput();
        GetSkillInput();
        GetSubSkillInput();
    }

    protected override void UpdateLand()   // 이동, MainSkill 가능
    {
        Move();
        Jump();
        Fall();
        GetDirInput();
        GetSkillInput();
    }

    protected override void UpdateCrawl()  // 이동, 구르기 가능
    {
        Move();
        Jump();
        Fall();
        GetDirInput();
        GetRollingInput();
    }

    protected override void UpdateRolling()     // 다른 행동 불가
    {
        Fall();
        Rolling();
    }

    protected override void UpdateSkill()  // 이동, 스킬 가능
    {
        MoveWhileSkill();
        JumpWhileSkill();
        Fall();
        GetDirInput();
        GetSkillInput();
        GetSubSkillInput();
    }

    protected override void UpdateSubSkill()     // 스킬 가능
    {
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

    #region Get Input // MyCtrl Series에서만 입력 가능
    // Dirtection Input
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

        if (_input.x == 0 && _input.y == 0 && _isGrounded && _jumpable && State != CreatureState.Skill && State != CreatureState.Subskill && State != CreatureState.Rolling)
            State = CreatureState.Idle;   
    }

    // Roll Input
    protected void GetRollingInput()
    {
        // 구르기 입력
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

            GuardEffect.SetActive(true);        // 가드이펙트 ON
            // Skill Packet Send Todo
        }
        else
        {
            isGuard = false;
            if (State == CreatureState.Subskill)
                State = CreatureState.Tmp;

            GuardEffect.SetActive(false);       // 가드이펙트 OFF
            // Tmp Packet Send Todo
        }
    }
    #endregion

    #region MainSkill       
    protected override void AnimEvent_MainSkillSlashOn()
    {
        _slashBox.enabled = true;
        _slashBox.transform.localPosition = new Vector2(0.1f, 0); // 충돌 감지를 위해 살짝 앞으로 이동

        SlashEffect.Play();
    }

    protected override void AnimEvent_MainSkillFrameEnded()
    {
        if (_isSkill)
            _isSkill = false;
        // AnimEvent : Skill 애니메이션 끝나기 전까지 State변화 방지

        _slashBox.enabled = false;
        _slashBox.transform.localPosition = new Vector2(0, 0);    // 위치 복귀

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
        // AnimEvent : SubSkill 애니메이션 끝나기 전까지 상태변화 X
    }
    // Hit 판정 OnTriggerEnter2D는 BombCtrl에 존재
    #endregion

    #region Rolling        Furry Knight는 구르기 대신 방패 돌진   
    protected override void Rolling()   // 배쉬중에 감속
    {
        if (_startBrake)
        {
            _velocity.x -= transform.localScale.x * 1.0f;
            _rigidbody.velocity = _velocity;
        }
    }

    protected override void AnimEvent_RollingStart()  // 배쉬 시작시 정지
    {
        _velocity.x = 0;
        _rigidbody.velocity = _velocity;
    }

    protected void AnimEvent_Rolling()  // 급가속 후 감속
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
        _startBrake = false;

        _bashBox.enabled = true;
        _bashBox.transform.localPosition = new Vector2(0, 0);
        BashEffect.Stop();
        
        State = CreatureState.Tmp;
        _coRollingCoolTimer = StartCoroutine("CoRollingCoolTimer", SkillData.JumpCoolTime + 1.0f);
    }
    #endregion 
}
