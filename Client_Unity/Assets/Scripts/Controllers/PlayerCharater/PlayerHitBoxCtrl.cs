using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBoxCtrl : MonoBehaviour
{
    // ���� �ǰ����� �� ��Ŷ ������
    protected LayerMask PlayerLayerMask = (int)Define.Layer.Player;
    protected LayerMask MonsterLayerMask = (int)Define.Layer.Monster;

    protected void OnCollisionEnter2D(Collision2D collision)  // ���Ͷ� �ε���
    {
        // Todo
        //_rigidbody.AddForce(new Vector3(transform.localScale.x * -ReBoundOffset, 0));
        if (collision.gameObject.layer == MonsterLayerMask)
        {
            Debug.Log("Monster�� �����ġ��!");
        }

        // Hpdelta Packet Send
        //SendHpdeltaPacket(collision, "Monster", (int)Define.SkillId.Human_Slash);
    }
}
