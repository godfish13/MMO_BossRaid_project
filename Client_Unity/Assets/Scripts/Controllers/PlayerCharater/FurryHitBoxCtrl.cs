using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurryHitBoxCtrl : PlayerHitBoxCtrl
{
    public bool isGuard;

    private void Update()
    {
        isGuard = GetComponentInParent<MyFurryCtrl>().isGuard;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == MonsterSkillLayerMask || collision.gameObject.layer == MonsterProjectileLayerMask)
        {
            if (isGuard != true)
            {
                SendPlayerHpdeltaPacket(collision, collision.gameObject.GetComponent<DragonPattern>().PatternId);
            }
        }
    }
}
