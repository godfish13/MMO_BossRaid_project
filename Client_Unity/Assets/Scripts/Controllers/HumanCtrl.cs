using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    class HumanCtrl : BaseCtrl
    {
        protected override void Init()
        {
            ClassId = 0;        
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
    }
}
