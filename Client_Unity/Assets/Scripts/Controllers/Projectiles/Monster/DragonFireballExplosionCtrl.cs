using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFireballExplosionCtrl : DragonPattern
{
    private ParticleSystem particleSystem;
    private bool _sendPacketOnce;

    protected override void Init()
    {
        particleSystem = GetComponent<ParticleSystem>();
        _sendPacketOnce = true;
    }

    protected override void Update()    // 파티클 시스템 종료 시 Death State packet 하나 송신
    {
        if (_sendPacketOnce && particleSystem.isStopped && particleSystem.isPaused == false)   // 낙뢰 이펙트 종료되면 destroy
        {
            _sendPacketOnce = false;    // DespawnPacket 도착 전에 종료되면 패킷 송신 그만하도록
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
}
