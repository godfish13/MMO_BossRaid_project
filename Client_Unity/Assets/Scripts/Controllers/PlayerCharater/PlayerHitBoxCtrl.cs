using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHitBoxCtrl : MonoBehaviour
{
    // ���� �ǰ����� �� ��Ŷ ������
    protected LayerMask PlayerLayerMask = (int)Define.Layer.Player;
    protected LayerMask MonsterLayerMask = (int)Define.Layer.Monster;
    protected LayerMask MonsterSkillLayerMask = (int)Define.Layer.MonsterSkill;
    protected LayerMask MonsterProjectileLayerMask = (int)Define.Layer.MonsterProjectile;

    protected void OnCollisionEnter2D(Collision2D collision)  // ���Ͷ� �ε���
    {
        if (collision.gameObject.layer == MonsterLayerMask)
        {
            Debug.Log("Monster�� �����ġ��!");
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
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

    /*private static Material _baseMaterial;
    private static Material _blinkMaterial;

    protected void Blink(GameObject go)
    {
        if (_baseMaterial == null) _baseMaterial = go.GetComponentInParent<BaseCtrl>().transform.GetComponent<SpriteRenderer>().sharedMaterial;
        if (_blinkMaterial == null) _blinkMaterial = new Material(Shader.Find("GUI/Text Shader"));
        
        StartCoroutine(BlinkCoroutine(go));
    }

    protected IEnumerator BlinkCoroutine(GameObject go)
    {
        go.GetComponent<SpriteRenderer>().material = _blinkMaterial;

        yield return new WaitForSeconds(0.1f);

        go.GetComponent<SpriteRenderer>().material = _baseMaterial;
    }*/
}
