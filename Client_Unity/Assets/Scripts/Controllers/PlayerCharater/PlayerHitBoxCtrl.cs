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
        // Todo
        //_rigidbody.AddForce(new Vector3(transform.localScale.x * -ReBoundOffset, 0));
        if (collision.gameObject.layer == MonsterLayerMask)
        {
            Debug.Log("Monster의 몸통박치기!");
        }

        // Hpdelta Packet Send
        //SendHpdeltaPacket(collision, "Monster", (int)Define.SkillId.Human_Slash);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == MonsterSkillLayerMask)
        {
            Debug.Log("아야 맞음!!");
            Debug.Log($"pattern : {collision.gameObject.GetComponent<DragonPattern>().PatternId}");
            
        }
    }

    protected void SendPlayerHpdeltaPacket(Collider2D collision, LayerMask layerMask, int skillId)
    {
        
    }
}
