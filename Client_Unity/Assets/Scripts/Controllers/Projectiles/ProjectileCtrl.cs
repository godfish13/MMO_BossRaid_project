using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ProjectileCtrl : MonoBehaviour
{
    // Move, Jump �� ���ʿ��� �κ� ����� ���� baseCtrl ��� X

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

    protected Transform SpriteTransform;   
    
    protected Animator _animator;

    protected virtual void Init()
    {
        SpriteTransform = gameObject.GetComponentsInChildren<Transform>()[1];       
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        Init();
    }

    protected virtual void FixedUpdate()
    {

    }

    #region Server ���
    protected virtual void Update()
    {
        //Debug.Log($"{GameObjectId} : {PositionInfo.PosX}, {PositionInfo.LocalScaleX}");
        SyncPos();
    }

    float speed = 5.0f;     // Server �� tick = 100, �̵��� = 0.5f �̹Ƿ� 0.1�ʴ� 0.5f�̵� �� 1�ʴ� 5f �̵� �׿� ������
    public void SyncPos()
    {
        // ��ȭ ������ �������� �۵����� �ʵ��� ���� �߰�
        if (State != PositionInfo.State || transform.position.x != PositionInfo.PosX || transform.position.y != PositionInfo.PosY || transform.localScale.x != PositionInfo.LocalScaleX)
        {
            State = PositionInfo.State;
            //transform.position = new Vector2(PositionInfo.PosX, PositionInfo.PosY);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PositionInfo.PosX, PositionInfo.PosY), Time.deltaTime * speed);
            transform.localScale = new Vector2(PositionInfo.LocalScaleX, 1);
            //Debug.Log($"{GameObjectId} : {PositionInfo.PosX}, {PositionInfo.PosY}, {PositionInfo.LocalScaleX}");

        }
    }

    protected LayerMask PlayerLayerMask = (int)Define.Layer.Player;
    protected LayerMask MonsterLayerMask = (int)Define.Layer.Monster;

    protected void SendHpdeltaPacket(Collider2D collision, LayerMask layerMask, int SkillId)
    {
        if (collision.gameObject.layer == layerMask) // Collision�� Layer�� Monster�ƴϸ� ����
        {
            C_Hpdelta hpdeltaPacket = new C_Hpdelta();
            hpdeltaPacket.HittedGameObjectId = collision.GetComponent<MonsterCtrl>().GameObjectId;
            hpdeltaPacket.SkillId = SkillId;
            Managers.networkMgr.Send(hpdeltaPacket);
        }
    }
    #endregion

    protected virtual void UpdateAnim()
    {
        if (_animator == null)
            return;

        switch (State)
        {
            case CreatureState.Idle:
                _animator.Play("IDLE");
                break;
        }
    }

    protected virtual void Move()
    {
        // projectile�� ��������
        /* 1. Human - Bomb : HumanCtrl���� Instantiate ���� AddForce
         * 2. Elf - Arrow, Lizard - Magic : ArrowCtrl.FixedUpdate���� rigidbody velocity ���� -> Server�� �̵�����
         */
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Projectile {gameObject.name} : OnTrigger Enter");

        //Todo damage ���� + Packet send
    }
}
