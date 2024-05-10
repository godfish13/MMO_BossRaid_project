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

    protected override void Update()    // ��ƼŬ �ý��� ���� �� Death State packet �ϳ� �۽�
    {
        if (_sendPacketOnce && particleSystem.isStopped && particleSystem.isPaused == false)   // ���� ����Ʈ ����Ǹ� destroy
        {
            _sendPacketOnce = false;    // DespawnPacket ���� ���� ����Ǹ� ��Ŷ �۽� �׸��ϵ���
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
