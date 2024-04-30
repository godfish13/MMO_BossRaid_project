using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyHumanCtrl : HumanCtrl
{
    private float BombThrowPower = 400.0f;
    private Collider2D _hitBoxCollider;     // �÷��̾� �ǰ����� ��Ʈ�ڽ�

    protected override void Init()
    {
        SlashEffect = GetComponentInChildren<ParticleSystem>();
        SlashBox = GetComponentsInChildren<BoxCollider2D>()[2];     // 0 : Player / 1 : Player Hitbox / 2 : SlashBox
        _hitBoxCollider = GetComponentsInChildren<Collider2D>()[1];     // 0 : Player / 1 : Player Hitbox / 2 : SlashBox
        base.Init();
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 4.6f, -10);
    }

    #region UpdateCtrl series
    protected override void UpdateCtrl()
    {
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

    protected override void UpdateSubSkill()     // �ٸ� �ൿ �Ұ�
    {
        Fall();
        BrakeIfSubSkill();    // SubSkill ����ϸ� Brake
    }

    protected override void UpdateDeath()
    {

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

        if (_input.x == 0 && _input.y == 0 && _isGrounded && _jumpable && State != CreatureState.Skill && State != CreatureState.Rolling)
            State = CreatureState.Idle;     // State Change flag
    }

    // Roll Input
    protected void GetRollingInput()
    {
        // ������ �Է�
        if (_isGrounded && _isRollingOn && Input.GetKey(KeyCode.Z))
        {
            State = CreatureState.Rolling;  // State Change flag
        }
    }

    // Main Skill Input
    protected void GetSkillInput()
    {
        if (Input.GetKey(KeyCode.X))
        {
            if (_isSkill == false)
                _isSkill = true;
            State = CreatureState.Skill;    // State Change flag

            // Skill Packet Send Todo
        }
        else
        {
            if (State == CreatureState.Skill && _isSkill == false)
                State = CreatureState.Tmp;   // State Change flag

            // Tmp Packet Send Todo
        }
    }

    // Sub Skill Input
    protected void GetSubSkillInput()
    {
        if (_isSubSkillOn && Input.GetKey(KeyCode.A)) // �ѹ� ���� ��Ÿ�ӵ��� ��ų���Ұ�
        {
            _coSubSkillCoolTimer = StartCoroutine("CoSubSkillCoolTimer", SkillData.SubSkillCoolTime);
            State = CreatureState.Subskill;     // State Change flag

            // Skill Packet Send Todo
        }
    }
    #endregion

    #region MainSkill
    protected override void AnimEvent_MainSkillSlashOn()
    {
        SlashBox.enabled = true;
        SlashBox.transform.localPosition = new Vector2(0.1f, 0); // �浹 ������ ���� ��¦ ������ �̵�

        SlashEffect.Play();
    }

    protected override void AnimEvent_MainSkillFrameEnded()
    {
        if (_isSkill)
            _isSkill = false;
        // AnimEvent : Skill �ִϸ��̼� ������ ������ State��ȭ ����

        SlashBox.enabled = false;
        SlashBox.transform.localPosition = new Vector2(0, 0);    // ��ġ ����

        SlashEffect.Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //_rigidbody.AddForce(new Vector3(transform.localScale.x * -ReBoundOffset, 0));
        Debug.Log("Hitted!");

        // Todo Skill Packet Send
    }
    #endregion

    #region SubSkill      
    private void AnimEvent_SubSkillThrowBomb()
    {
        ThrowBomb();
    }

    private void ThrowBomb()
    {
        GameObject Bomb = Managers.resourceMgr.Instantiate("Projectiles/Explosive");
        Bomb.transform.position = transform.position + new Vector3(1.0f * transform.localScale.x, 0.5f, 0);
        Bomb.GetComponent<Rigidbody2D>().AddForce((Vector2.up + (Vector2.right * transform.localScale.x * 2)).normalized * BombThrowPower);
    }

    private void AnimEvent_SubSkillFrameEnded()
    {
        State = CreatureState.Tmp;  // State Change flag
        // AnimEvent : SubSkill �ִϸ��̼� ������ ������ ���º�ȭ X
    }

    // Hit ���� OnTriggerEnter2D�� BombCtrl�� ����
    #endregion

    #region Rolling
    protected void Rolling()
    {
        velocity.x = transform.localScale.x * Stat.MaxSpeed * 3;
        _rigidbody.velocity = velocity;
    }

    protected override void AnimEvent_RollingStart()   // ������ �� ����
    {
        _hitBoxCollider.enabled = false;
    }

    protected override void AnimEvent_RollingEnded()
    {
        base.AnimEvent_RollingEnded();

        _hitBoxCollider.enabled = true;

        if (_input.y == -1) // ������ ������ �ӵ� ���¿� ���� �ʱ�ȭ(����)
        {
            velocity.x = transform.localScale.x * Stat.MaxSpeed * 0.3f;
            _rigidbody.velocity = velocity;
        }
        else
        {
            velocity.x = transform.localScale.x * Stat.MaxSpeed;
            _rigidbody.velocity = velocity;
        }

        State = CreatureState.Tmp;     // State Change flag 
    }
    #endregion
}
