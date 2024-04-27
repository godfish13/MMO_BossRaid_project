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
    class HumanCtrl : BaseCtrl
    {
        private BoxCollider2D[] BoxCollider2Ds;
        private BoxCollider2D SlashBox;
        private ParticleSystem SlashEffect;

        protected override void Init()
        {
            ClassId = 0;
            SlashEffect = GetComponentInChildren<ParticleSystem>();
            BoxCollider2Ds = GetComponentsInChildren<BoxCollider2D>();
            SlashBox = BoxCollider2Ds[1];
            base.Init();
        }

        protected override void MainSkill()
        {

            if (Input.GetKey(KeyCode.X)) // 착지모션중에는 스킬사용불가
            {
                if (_isSkill == false)
                    _isSkill = true;
                State = CreatureState.Skill;
            }
            else
            {
                if (State == CreatureState.Skill && _isSkill == false)
                    State = CreatureState.Tmp;   // State Change flag
            }
        }

        protected override void SubSkill()
        {

        }

        protected void AnimEvent_SkillFrameEnded()
        {
            if (_isSkill)
                _isSkill = false;
            // AnimEvent : Skill 애니메이션 끝나기 전까지 State변화 방지
        }

        private void AnimEvent_SlashOn()
        {
            SlashBox.enabled = true;
            SlashBox.transform.localPosition = new Vector2(0.1f, 0);
            // 충돌 감지를 위해 살짝 앞으로 이동
            SlashEffect.Play();
        }

        private void AnimEvent_SlashOff()
        {
            SlashBox.enabled = false;
            SlashBox.transform.localPosition = new Vector2(0, 0);
            // 위치 복귀
            SlashEffect.Stop();
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            //_rigidbody.AddForce(new Vector3(transform.localScale.x * -ReBoundOffset, 0));
            Debug.Log("Hitted!");       
        }
    }
}
