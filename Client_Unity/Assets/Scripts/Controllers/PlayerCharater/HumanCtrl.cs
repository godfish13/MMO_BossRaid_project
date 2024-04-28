using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    class HumanCtrl : BaseCtrl          // MyCtrl_Human로 이름 변경?
    {
        private BoxCollider2D SlashBox;
        private ParticleSystem SlashEffect;
        private float BombThrowPower = 400.0f;

        protected override void Init()
        {
            ClassId = 0;
            SlashEffect = GetComponentInChildren<ParticleSystem>();
            SlashBox = GetComponentsInChildren<BoxCollider2D>()[2];     // 0 : Player / 1 : Player Hitbox / 2 : SlashBox
            base.Init();
        }

        #region MainSkill
        private void AnimEvent_MainSkillSlashOn()
        {
            SlashBox.enabled = true;
            SlashBox.transform.localPosition = new Vector2(0.1f, 0);
            // 충돌 감지를 위해 살짝 앞으로 이동
            SlashEffect.Play();
        }

        private void AnimEvent_MainSkillSlashOff()
        {
            SlashBox.enabled = false;
            SlashBox.transform.localPosition = new Vector2(0, 0);
            // 위치 복귀
            SlashEffect.Stop();
        }

        private void AnimEvent_MainSkillFrameEnded()
        {
            if (_isSkill)
                _isSkill = false;
            // AnimEvent : Skill 애니메이션 끝나기 전까지 State변화 방지
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //_rigidbody.AddForce(new Vector3(transform.localScale.x * -ReBoundOffset, 0));
            Debug.Log("Hitted!");    
            
            // Todo Skill Packet Send
        }
        #endregion

        #region SubSkill      
        private void AnimEvent_SubSkillThrowBomb()
        {
            ThrowBomb();
        }

        private void ThrowBomb()
        {
            GameObject Bomb = Managers.resourceMgr.Instantiate("Projectiles/Explosive");
            Bomb.transform.position = transform.position + new Vector3(1.0f * transform.localScale.x, 0.5f, 0);
            Bomb.GetComponent<Rigidbody2D>().AddForce((Vector2.up + (Vector2.right * transform.localScale.x * 2)).normalized * BombThrowPower);
        }

        private void AnimEvent_SubSkillFrameEnded()
        {
            State = CreatureState.Tmp;  // State Change flag
            // AnimEvent : SubSkill 애니메이션 끝나기 전까지 상태변화 X
        }

        // Hit 판정 OnTriggerEnter2D는 BombCtrl에 존재
        #endregion
    }
}
