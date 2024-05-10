using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DragonThunder : ProjectileCtrl
{
    private ParticleSystem particleSystem;
    private Collider2D ThunderHitbox;

    void Start()
    {
        particleSystem = GetComponentsInChildren<ParticleSystem>()[1];  // 2¹øÂ° ³«·Ú ÀÌÆåÆ®
        ThunderHitbox = GetComponent<Collider2D>();
        ThunderHitbox.enabled = false;
        _coThunderHitBoxTimer = StartCoroutine("CoThunderHitBoxTimer", 1.7f);
    }

    protected override void Update()
    {
        if (particleSystem.isStopped && !particleSystem.isPaused)   // ³«·Ú ÀÌÆåÆ® Á¾·áµÇ¸é destroy
        {
            Destroy(gameObject);
        }
    }

    protected Coroutine _coThunderHitBoxTimer;
    IEnumerator CoThunderHitBoxTimer(float time)
    {
        yield return new WaitForSeconds(time);
        ThunderHitbox.enabled = true;
        yield return new WaitForSeconds(time - 1.5f);
        ThunderHitbox.enabled = false;
        _coThunderHitBoxTimer = null;
    }
}
