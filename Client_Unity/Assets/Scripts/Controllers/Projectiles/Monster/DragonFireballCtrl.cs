using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFireballCtrl : DragonPattern
{
    private void Awake()
    {
        PatternId = (int)Define.ProjectileType.Dragon_Fireball;
        PatternType = (int)Define.MonsterPatternType.Range;
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
        base.OnTriggerEnter2D(collision); 

        State = CreatureState.Death; 
        C_MovePacketSend();                   // Death State�� ��ȭ��Ű��� 1ȸ ���
    }
}
