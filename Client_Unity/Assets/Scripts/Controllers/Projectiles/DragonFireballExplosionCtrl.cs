using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFireballExplosionCtrl : MonoBehaviour
{
    private ParticleSystem particleSystem;

    void Start()
    {
        // ��ƼŬ �ý��� ������Ʈ ��������
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // ��ƼŬ �ý����� ����Ǿ����� Ȯ��
        if (particleSystem.isStopped && !particleSystem.isPaused)
        {
            // ��ƼŬ �ý����� ����Ǹ� �ش� GameObject �ı�
            Destroy(gameObject);
        }
    }
}
