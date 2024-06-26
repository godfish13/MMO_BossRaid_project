using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MyElfCtrl : MyPlayerBaseCtrl
{
    protected BoxCollider2D _stabBox;   // Sub Skill 판정범위 히트박스

    protected override void Init()
    {
        _hitBoxCollider = GetComponentsInChildren<Collider2D>()[1]; // 0 : Player / 1 : Player Hitbox / 2 : SlashBox
        _stabBox = GetComponentsInChildren<BoxCollider2D>()[2];     // 0 : Player / 1 : Player Hitbox / 2 : SlashBox
        SlashEffect = GetComponentInChildren<ParticleSystem>();

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

    protected override void UpdateSkill()  // 스킬 가능 (화살 발사)
    {
        //MoveWhileSkill();
        //JumpWhileSkill();
        Fall();
        //GetDirInput();
        GetSkillInput();
        GetSubSkillInput();
        BrakeIfSkill();
    }

    protected override void UpdateSubSkill()     // 이동, 스킬 가능
    {
        MoveWhileSkill();
        JumpWhileSkill();
        Fall();
        GetDirInput();
        GetSkillInput();
        GetSubSkillInput();
    }

    protected override void UpdateDeath()
    {
        _rigidbody.velocity = Vector3.zero;
        if (_hitBoxCollider.enabled == true)
            _hitBoxCollider.enabled = false;
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
        if (Input.GetKey(KeyCode.X) && _isGrounded == true) // 지면에 서있을때만 화살 발사 가능
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
        if (Input.GetKey(KeyCode.A) && _isSkill == false)
        {
            if (_isSkill == false)
                _isSkill = true;
            State = CreatureState.Subskill; 

            // Skill Packet Send Todo
        }
        else
        {
            if (State == CreatureState.Subskill && _isSkill == false)
                State = CreatureState.Tmp; 

            // Tmp Packet Send Todo
        }
    }
    #endregion

    #region MainSkill       
    protected override void AnimEvent_MainSkillSlashOn()
    {
        C_Skill skillPacket = new C_Skill();
        skillPacket.SkillId = (int)Define.SkillId.Elf_ArrowShot;
        Managers.networkMgr.Send(skillPacket);
    }

    protected override void AnimEvent_MainSkillFrameEnded()
    {
        if (_isSkill)
            _isSkill = false;
    }
    #endregion

    #region SubSkill      
    protected override void AnimEvent_SubSkill()
    {
        _stabBox.enabled = true;
        _stabBox.transform.localPosition = new Vector2(0.1f, 0); // 충돌 감지를 위해 살짝 앞으로 이동

        SlashEffect.Play();
    }

    protected override void AnimEvent_SubSkillFrameEnded()
    {
        if (_isSkill)
            _isSkill = false;
        // AnimEvent : Skill 애니메이션 끝나기 전까지 State변화 방지

        _stabBox.enabled = false;
        _stabBox.transform.localPosition = new Vector2(0, 0);    // 위치 복귀

        SlashEffect.Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == MonsterLayerMask)
        {
            Debug.Log("Stab Hitted!");
        }

        // Hpdelta Packet Send
        SendMonsterHpdeltaPacket(collision, MonsterLayerMask, (int)Define.SkillId.Elf_Knife);
    }
    // Hit 판정 OnTriggerEnter2D는 BombCtrl에 존재
    #endregion
}