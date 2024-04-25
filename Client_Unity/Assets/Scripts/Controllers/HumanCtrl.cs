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
            if (Input.GetKey(KeyCode.X))
            {
                State = CreatureState.Skill;
                _coSkillCoolTimer = StartCoroutine("CoSkillCoolTimer", SkillData.SkillCoolTime);
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
    }
}
