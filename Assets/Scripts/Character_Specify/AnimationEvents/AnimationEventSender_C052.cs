using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C052 : AnimationEventSenderNew
{
    protected override void AttackAction(int actionID)
    {
        var ac = (ActorController as ActorControllerGun);
        var am = _attackManager as AttackManager_C052;

        switch (actionID)
        {
            case 101:
                am.ComboAttack1();
                break;
            case 102:
                ac.ComboBackwardStep();
                break;
            
            case 2011:
                am.Skill1_ReleaseDrone();
                break;
            
            case 2021:
                am.Skill2_Overclock();
                break;
            
            case 2031:
                am.Skill3_Overdrive();
                break;
        }

    }
    
    protected void Skill4()
    {
        var am = _attackManager as AttackManager_C052;
        am.Skill4(0);
    }
    
}
