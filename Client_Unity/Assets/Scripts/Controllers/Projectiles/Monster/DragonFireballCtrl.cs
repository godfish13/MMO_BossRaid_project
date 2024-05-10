using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFireballCtrl : DragonPattern
{
    private void Awake()
    {
        PatternId = (int)Define.ProjectileType.DragonFireball;
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

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);   // damage 판정, Packet Send는 base에서

        State = CreatureState.Death;      // State Change Flag
        C_MovePacketSend();                   // Death State로 변화시키라고 1회 통신
    }
}
