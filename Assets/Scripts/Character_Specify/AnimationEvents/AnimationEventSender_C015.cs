using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C015 : AnimationEventSenderNew
{
    protected override void ForceStrikeEnter()
    {
        (_attackManager as AttackManager_C015).SpecialForceStrikeCharging();
        (ActorController as ActorController_c015).CancelDelay();
    }

    protected override void ForceStrikeRelease()
    {
        (_attackManager as AttackManager_C015).ForceStrikeRelease(1);
    }


    protected override void AttackAction(int actionID)
    {
        var am = _attackManager as AttackManager_C015;
        var ac = ActorController as ActorController_c015;
        switch (actionID)
        {
            case 1000:
            {
                var delayed = ac.StartDelayTween();
                if (delayed)
                {
                    am.DelayedShine();
                }
                break;
            }
            case 1001:
            {
                var delayed = ac.StartDelayTween(0.5f);
                if (delayed)
                {
                    am.DelayedShine();
                }
                break;
            }
            case 1002:
            {
                ac.StopDelayTween();
                break;
            }
            case 1010:
            {
                am.Combo1_Muzzle();
                break;
            }
            case 1011:
            {
                am.Combo1();
                break;
            }
            case 10120:
            {
                am.Combo2_Muzzle();
                break;
            }
            case 1012:
            {
                am.Combo2();
                break;
            }
            case 1013:
            {
                am.Combo3();
                break;
            }
            case 10141:
            {
                ac.Combo4A_Jump();
                am.ComboFlash();
                break;
            }
            case 10142:
            {
                ac.Combo4A_Smash();
                am.ComboFlash();
                break;
            }
            case 1014:
            {
                am.Combo4();
                break;
            }
            case 10143:
            {
                am.Combo4_Muzzle();
                break;
            }
            case 10144:
            {
                am.Combo4D();
                break;
            }

            case 2011:
            {
                am.Skill1_Muzzle(1);
                break;
            }
            case 2012:
            {
                am.Skill1_Muzzle(2);
                break;
            }
            case 2013:
            {
                am.Skill1_Muzzle(3);
                break;
            }
            case 2014:
            {
                am.Skill1_Proj_Normal(true);
                break;
            }
            case 2015:
            {
                am.Skill1_Proj_Normal();
                break;
            }

            case 2021:
            {
                ac.Skill2_Jump();
                break;
            }
            case 2022:
            {
                am.Skill2();
                break;
            }
            case 2023:
            {
                ac.Skill2_Land();
                break;
            }

            case 2030:
            {
                ac.Skill3_AutoCheckSkillType();
                am.CheckEnhanced();
                break;
            }
            case 2031:
            {
                if (!ac.Skill3IsForward)
                {
                    am.Skill3_Backward_Slash();
                }
                else
                {
                    am.Skill3_Forward_Wave();
                    am.Combo4_Muzzle();
                }
                break;
            }
            case 2032:
            {
                if (!ac.Skill3IsForward && !ac.Skill3IsStatic)
                {
                    ac.DisappearRenderer();
                    am.ComboFlash();
                }
                else
                {
                    am.ComboFlash();
                }
                break;
            }
            case 2033:
            {
                if (!ac.Skill3IsForward)
                {
                    ac.AppearRenderer();
                    am.ComboFlash();
                }
                break;
            }
            case 2034:
            {
                if (!ac.Skill3IsForward && !ac.Skill3IsStatic)
                    ac.Skill3_BackwardWarp();
                break;
            }
            case 2035:
            {
                if (!ac.Skill3IsForward)
                    am.Skill3_Backward_Wave();
                break;
            }
            case 2036:
            {
                if (ac.Skill3IsForward)
                {
                    am.Skill3_Forward_Slash();
                    am.Skill2_FigShine(true);
                }
                    
                break;
            }
            case 2037:
            {
                if (!ac.Skill3IsForward)
                {
                    ac.Skill3_AutoCheckStatic();
                }
                break;
            }
            case 2038:
            {
                if (ac.Skill3IsForward)
                    am.ComboFlash();
                break;
            }
            case 2039:
            {
                if (!ac.Skill3IsForward)
                {
                    am.Skill1_Muzzle_Boost();
                    am.Skill1_FigShine(true);
                }
                else
                {
                    ac.Skill3_ForwardWarp();
                    am.Skill3_Forward_Muzzle();
                }
                break;
            }
            
            
            case 2051:
            {
                am.Skill1_Muzzle(1);
                am.Skill1_FigShine(false);
                break;
            }
            case 2052:
            {
                am.Skill1_Muzzle(2);
                am.Skill1_FigShine(false);
                break;
            }
            case 2053:
            {
                am.Skill1_Muzzle(3);
                am.Skill1_FigShine(false);
                break;
            }
            case 20541:
            {
                am.Skill1_Proj_Boost(1,true);
                break;
            }
            case 20542:
            {
                am.Skill1_Proj_Boost(1,false);
                break;
            }
            case 20543:
            {
                am.Skill1_Proj_Boost(2);
                break;
            }
            case 20544:
            {
                am.Skill1_Proj_Boost(3);
                break;
            }
            case 2055:
            {
                am.Skill1_Muzzle_Boost();
                am.Skill1_FigShine(true);
                break;
            }
            case 2056:
            {
                am.Skill1_MoveFig();
                break;
            }
            case 2057:
            {
                am.Skill1_ReturnFig();
                break;
            }

            case 2061:
            {
                am.Skill2_FigActive(true);
                break;
            }
            case 2062:
            {
                am.Skill2_FigActive(false);
                break;
            }
            case 2063:
            {
                am.Skill2_FigShine(true);
                break;
            }
            case 2064:
            {
                am.Skill2_FigShine(false);
                break;
            }
            case 2065:
            {
                am.Skill2_Slash();
                am.ComboFlash();
                break;
            }
            case 2066:
            {
                ac.Skill2_Boost_Dash();
                break;
            }
            
            

            case 9000:
            {
                am.TurnToForceDirection();
                break;
            }

        }
    }

    protected void Skill4()
    {
        var am = _attackManager as AttackManager_C015;
        am.Skill4(0);
    }
}
