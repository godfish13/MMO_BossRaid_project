using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ServerSession : PacketSession
{
	public override void OnConnected(EndPoint endPoint)
	{
		Debug.Log($"OnConnected : {endPoint}");

		PacketManager.Instance.CustomHandler4Client = (packetSession, iMessage, Id) =>
		{
			PacketQueue.Instance.Push(Id, iMessage);
		};
	}

    public override void OnReceivePacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnDisConnected(EndPoint endPoint)
	{
		Debug.Log($"OnDisconnected : {endPoint}");
	}

	public override void OnSend(int numOfBytes)
	{
		//Debug.Log($"Transferred bytes: {numOfBytes}");
	}
}