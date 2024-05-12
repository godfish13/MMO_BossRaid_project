using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurryHitBoxCtrl : PlayerHitBoxCtrl
{
    public bool isGuard;
    public Transform FurryTransform;

    private void Start()
    {
        FurryTransform = GetComponentInParent<MyFurryCtrl>().transform;
    }

    private void Update()
    {
        isGuard = GetComponentInParent<MyFurryCtrl>().isGuard;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == MonsterSkillLayerMask || collision.gameObject.layer == MonsterProjectileLayerMask)
        {
            if (isGuard == true)
            {
                if (FurryTransform.localScale.x == 1 && FurryTransform.position.x < 0)
                {
                    // �����ʺ��� ������ġ(0, 0, 0) ���� ���ʿ� ������ ���强��
                    return;
                }
                else if(FurryTransform.localScale.x == -1 && FurryTransform.position.x > 0)
                {
                    // ���ʺ��� ������ġ(0, 0, 0) ���� �����ʿ� ������ ���强��
                    return;
                }
            }

            //Blink(gameObject);
            SendPlayerHpdeltaPacket(collision, collision.gameObject.GetComponent<DragonPattern>().PatternId);
        }
    }
}
