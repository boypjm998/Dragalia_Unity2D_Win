using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C036 : AnimationEventSenderNew
{
    
    protected override void AttackAction(int actionID)
    {
        var am = _attackManager as AttackManager_C036;
        var ac = ActorController as ActorControllerMeeleWithFS;
        switch (actionID)
        {
            case 101:
            {
                am.Combo1();
                break;
            }
            case 102:
            {
                am.Combo2();
                ac.GeneralHorizontalMovementWithEnemyCheck(1f,3f,1.5f,0.2f);
                break;
            }
            case 103:
            {
                am.Combo3();
                ac.GeneralHorizontalMovementWithEnemyCheck(1f,3f,1.5f,0.2f);
                break;
            }
            case 104:
            {
                am.Combo4();
                break;
            }
            case 105:
            {
                am.Combo5_Ranged();
                break;
            }
            case 100:
            {
                ac._statusManager.ResetKBRes();
                break;
            }
            case 2011:
            {
                am.Skill1_Throw();
                break;
            }
            case 2021:
            {
                ac.GeneralHorizontalMovementWithEnemyCheck(2,3,2,0.2f);
                break;
            }
            case 2022:
            {
                am.Skill2_Slash();
                break;
            }
            case 2023:
            {
                am.Skill2_SummonAlex();
                break;
            }
            case 2031:
            {
                am.Skill3_Buff();
                break;
            }
        }
    }

    protected void Skill4()
    {
        (_attackManager as AttackManager_C036).Skill4();
    }
}
