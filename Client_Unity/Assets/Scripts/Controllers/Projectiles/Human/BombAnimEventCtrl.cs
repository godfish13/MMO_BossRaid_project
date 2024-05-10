using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAnimEventCtrl : MonoBehaviour
{
    private BombCtrl _bombCtrl;

    private void Start()
    {
        _bombCtrl = gameObject.GetComponentInParent<BombCtrl>();
    }

    public void AnimEvent_ExplosionFrameEnded()
    {
        _bombCtrl.State = CreatureState.Death;  // State Change Flag
    }
}
