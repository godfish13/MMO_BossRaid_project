using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBoxCtrl : MonoBehaviour
{
    // 각종 피격판정 및 패킷 보내기
    protected LayerMask PlayerLayerMask = (int)Define.Layer.Player;
    protected LayerMask MonsterLayerMask = (int)Define.Layer.Monster;

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
}
