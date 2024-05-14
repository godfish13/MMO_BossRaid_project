using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerBaseCtrl : PlayerCtrl
{
    #region Server 통신
    protected bool PacketSendingFlag = false; // State 변화, position값이 일정수준 이상 변화시 true

    protected override void Update()
    {
        if (Mathf.Abs(transform.position.x - PositionInfo.PosX) > 0.05f || Mathf.Abs(transform.position.y - PositionInfo.PosY) > 0.05f)
        {
            PositionInfo.PosX = transform.position.x;
            PositionInfo.PosY = transform.position.y;
            PositionInfo.LocalScaleX = transform.localScale.x;

            PacketSendingFlag = true;
        }

        if (PacketSendingFlag)
        {
            PacketSendingFlag = false;
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
    #endregion
}
