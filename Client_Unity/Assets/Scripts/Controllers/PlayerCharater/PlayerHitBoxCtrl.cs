using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBoxCtrl : MonoBehaviour
{
    // 각종 피격판정 및 패킷 보내기
    protected LayerMask PlayerLayerMask = (int)Define.Layer.Player;
    protected LayerMask MonsterLayerMask = (int)Define.Layer.Monster;
    protected LayerMask MonsterSkillLayerMask = (int)Define.Layer.MonsterSkill;
    protected LayerMask MonsterProjectileLayerMask = (int)Define.Layer.MonsterProjectile;

    protected void OnCollisionEnter2D(Collision2D collision)  // 몬스터랑 부딪힘
    {
        if (collision.gameObject.layer == MonsterLayerMask)
        {
            Debug.Log("Monster의 몸통박치기!");
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == MonsterSkillLayerMask || collision.gameObject.layer == MonsterProjectileLayerMask)
        {
            SendPlayerHpdeltaPacket(collision, collision.gameObject.GetComponent<DragonPattern>().PatternId);
        }
    }

    protected void SendPlayerHpdeltaPacket(Collider2D collision, int monsterPatternId)
    {
        C_Hpdelta hpdeltaPacket = new C_Hpdelta();
        if (collision.GetComponent<DragonPattern>().PatternType == (int)Define.MonsterPatternType.Melee)
        {
            hpdeltaPacket.AttackerGameObjectId = collision.gameObject.GetComponentInParent<MonsterCtrl>().GameObjectId;
        }
        else if (collision.GetComponent<DragonPattern>().PatternType == (int)Define.MonsterPatternType.Range)
        {
            hpdeltaPacket.AttackerGameObjectId = collision.GetComponent<ProjectileCtrl>().OwnerGameObjectId;
        }
        hpdeltaPacket.HittedGameObjectId = gameObject.GetComponentInParent<BaseCtrl>().GameObjectId;
        hpdeltaPacket.SkillId = monsterPatternId;
        Managers.networkMgr.Send(hpdeltaPacket);
    }
}
