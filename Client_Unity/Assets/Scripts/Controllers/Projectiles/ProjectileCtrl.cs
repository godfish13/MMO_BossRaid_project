using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileCtrl : MonoBehaviour
{
    // Move, Jump 등 불필요한 부분 덜어내기 위해 baseCtrl 상속 X

    #region for Server Connection
    [SerializeField] private int _gameObjectId;
    public int GameObjectId { get { return _gameObjectId; } set { _gameObjectId = value; } }
    public int OwnerGameObjectId { get; set; }

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

    #region Server 통신
    protected virtual void Update()
    {
        //Debug.Log($"{GameObjectId} : {PositionInfo.PosX}, {PositionInfo.LocalScaleX}");
        SyncPos();
    }

    public float Speed { get; set; } = 1.0f;     // Server 상 projectile update tick(1000 == 1초) * 1tick당 이동량(Speed) // 각 파생 class에서 조절 기본값 1
                            // ex. DragonFireball : Server에서 1000틱당 5이동 => Speed = 5
    public virtual void SyncPos()
    {
        // 변화 없을땐 쓸데없이 작동하지 않도록 조건 추가
        if (State != PositionInfo.State || transform.position.x != PositionInfo.PosX || transform.position.y != PositionInfo.PosY || transform.localScale.x != PositionInfo.LocalScaleX)
        {
            State = PositionInfo.State;
            // Server에서 관리되는 투사체는 MoveTowards를 통해 이동
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PositionInfo.PosX, PositionInfo.PosY), Time.deltaTime * Speed);
            transform.localScale = new Vector2(PositionInfo.LocalScaleX, 1);
        }
    }

    protected LayerMask PlayerLayerMask = (int)Define.Layer.Player;
    protected LayerMask MonsterLayerMask = (int)Define.Layer.Monster;

    protected void SendHpdeltaPacket(Collider2D collision, LayerMask layerMask, int SkillId)
    {
        if (collision.gameObject.layer == layerMask) // Collision의 Layer가 Monster아니면 무시
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
        // projectile별 개별구현
        /* 1. Human - Bomb : HumanCtrl에서 Instantiate 순간 AddForce
         * 2. Elf - Arrow, Lizard - Magic : ArrowCtrl.FixedUpdate에서 rigidbody velocity 조작 -> Server로 이동예정
         */
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Projectile {gameObject.name} : OnTrigger Enter");
    }
}
