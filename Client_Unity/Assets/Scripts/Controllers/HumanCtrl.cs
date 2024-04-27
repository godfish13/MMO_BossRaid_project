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

        #region MainSkill
        protected override void MainSkill()
        {
            if (Input.GetKey(KeyCode.X)) // 착지모션중에는 스킬사용불가
            {
                if (_isSkill == false)
                    _isSkill = true;
                State = CreatureState.Skill;

                // Skill Packet Send Todo
            }
            else
            {
                if (State == CreatureState.Skill && _isSkill == false)
                    State = CreatureState.Tmp;   // State Change flag

                // Tmp Packet Send Todo
            }
        }

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
        }
        #endregion

        #region SubSkill
        protected override void SubSkill()
        {
            if (_isSubSkillOn && Input.GetKey(KeyCode.A)) // 한번 사용시 쿨타임동안 스킬사용불가
            {
                _coSubSkillCoolTimer = StartCoroutine("CoSubSkillCoolTimer", SkillData.SubSkillCoolTime);
                State = CreatureState.Subskill;     // State Change flag

                // Skill Packet Send Todo
            }
        }

        private void AnimEvent_SubSkillThrowBomb()
        {
            Debug.Log("Fire in the Hole!");
            // Todo : instantiate Bomb, Bomb 피격판정 및 이펙트는 projectile 클래스로 분리해서 각 Bomb에 따로 적용
        }

        private void AnimEvent_SubSkillFrameEnded()
        {
            State = CreatureState.Tmp;  // State Change flag
            // AnimEvent : SubSkill 애니메이션 끝나기 전까지 상태변화 X
        }
        #endregion
    }
}
