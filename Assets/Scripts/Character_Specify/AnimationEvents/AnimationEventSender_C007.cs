using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C007 : AnimationEventSenderNew
{
    
    protected override void DoShapeShifting(int type)
    {
        (_attackManager as AttackManager_C007).InstantiateTransformWave();
        (ActorController as ActorController).InvokeShapeShifting();
    }

    protected override void AttackAction(int actionID)
    {
        var attackManager = _attackManager as AttackManager_C007;
        switch (actionID)
        {
            case 101:
                if(attackManager.IsDragondrive)
                    attackManager.Combo1_Boost();
                else
                    attackManager.Combo1();
                break;
            case 102:
                if(attackManager.IsDragondrive)
                    attackManager.Combo2_Boost();
                else
                    attackManager.Combo2();
                break;
            case 103:
                if(attackManager.IsDragondrive)
                    attackManager.Combo3_Boost();
                else attackManager.Combo3();
                break;
            case 104:
                if(attackManager.IsDragondrive)
                    attackManager.Combo4_Boost();
                else attackManager.Combo4();
                break;
            case 105:
                if(attackManager.IsDragondrive)
                    attackManager.Combo5_Boost();
                else attackManager.Combo5();
                break;
            
            case 201:
                attackManager.Skill1(0);
                break;
            case 202:
                attackManager.Skill2(0);
                break;
            // case 204:
            //     attackManager.Skill4(0);
            //     break;


            default:break;
        }
    }

    protected void Skill4()
    {
        (_attackManager as AttackManager_C007).Skill4(1);
    }
}
