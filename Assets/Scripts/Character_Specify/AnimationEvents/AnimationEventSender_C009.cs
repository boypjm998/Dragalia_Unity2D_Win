using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C009 : AnimationEventSenderNew
{
    

    protected override void AttackAction(int actionID)
    {
        var am = _attackManager as AttackManager_C009;
        switch (actionID)
        {
            case 101:
                am.ComboAttack1();
                break;
            case 102:
                am.ComboAttack2();
                break;
            case 103:
                am.ComboAttack3();
                break;
            case 104:
                am.ComboAttack4();
                break;
            case 105:
                am.ComboAttack5();
                break;
            
            case 2012:
                am.Skill1_Attack();
                break;
            case 2021:
                (ActorController as ActorController_c009).Skill_FloatInfoAir();
                break;
            case 2022:
                am.Skill2_Attack();
                break;
            case 2023:
                (ActorController as ActorController_c009).Skill_LandToGround();
                break;
            case 2031:
                am.Skill3_Attack();
                break;
            case 2042:
                am.Skill4();
                break;
        }
    }

    protected override void ForceStrikeEnter()
    {
        (_attackManager as AttackManager_C009).ForceStrikeCharging();
    }
}
