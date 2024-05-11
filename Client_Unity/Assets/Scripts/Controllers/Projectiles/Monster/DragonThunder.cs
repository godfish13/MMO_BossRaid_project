using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DragonThunder : DragonPattern
{
    private ParticleSystem particleSystem;
    private Collider2D ThunderHitbox;
    private bool _sendPacketOnce;

    private void Awake()
    {
        PatternId = (int)Define.ProjectileType.Dragon_Thunder;
        PatternType = (int)Define.MonsterPatternType.Range;
        _sendPacketOnce = true;
    }

    void Start()
    {
        particleSystem = GetComponentsInChildren<ParticleSystem>()[1];  // 2¹øÂ° ³«·Ú ÀÌÆåÆ®
        ThunderHitbox = GetComponent<Collider2D>();
        ThunderHitbox.enabled = false;
        _coThunderHitBoxTimer = StartCoroutine("CoThunderHitBoxTimer", 1.6f);
    }

    protected override void Update()
    {
        if (_sendPacketOnce && particleSystem.isStopped && particleSystem.isPaused == false)   // ³«·Ú ÀÌÆåÆ® Á¾·áµÇ¸é Death
        {
            _sendPacketOnce = false;    // DespawnPacket µµÂø Àü¿¡ Á¾·áµÇ¸é ÆÐÅ¶ ¼Û½Å ±×¸¸ÇÏµµ·Ï
            State = CreatureState.Death;
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

    protected Coroutine _coThunderHitBoxTimer;
    IEnumerator CoThunderHitBoxTimer(float time)
    {
        yield return new WaitForSeconds(time);
        ThunderHitbox.enabled = true;
        yield return new WaitForSeconds(time - 1.4f);
        ThunderHitbox.enabled = false;
        _coThunderHitBoxTimer = null;
    }
}
