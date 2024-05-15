using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyArrowCtrl : ArrowCtrl
{
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
        base.OnTriggerEnter2D(collision);

        SendHpdeltaPacket(collision, MonsterLayerMask, (int)Define.SkillId.Elf_ArrowShot);

        State = CreatureState.Death;      
        C_MovePacketSend();               // Death State로 변화시키라고 1회 통신
    }
}
