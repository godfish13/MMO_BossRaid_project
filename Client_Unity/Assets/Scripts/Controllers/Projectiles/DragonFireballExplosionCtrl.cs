using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFireballExplosionCtrl : MonoBehaviour
{
    private ParticleSystem particleSystem;

    void Start()
    {
        // 파티클 시스템 컴포넌트 가져오기
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // 파티클 시스템이 종료되었는지 확인
        if (particleSystem.isStopped && !particleSystem.isPaused)
        {
            // 파티클 시스템이 종료되면 해당 GameObject 파괴
            Destroy(gameObject);
        }
    }
}
