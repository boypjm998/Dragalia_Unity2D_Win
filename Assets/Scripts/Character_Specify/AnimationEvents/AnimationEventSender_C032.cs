using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C032 : AnimationEventSenderNew
{
    AttackManager_C032 _attackManagerMeeleWithFs;
    private ActorControllerMeeleWithFS _actorControllerMeeleWithFs;
    
    protected override void Start()
    {
        base.Start();
        
        _attackManagerMeeleWithFs = GetComponentInParent<AttackManager_C032>();
    }
    protected override void AttackAction(int actionID)
    {
        switch (actionID)
        {
            case 101:
                _attackManagerMeeleWithFs.Combo1();
                break;
            case 102:
                _attackManagerMeeleWithFs.Combo2();
                break;
            case 103:
                _attackManagerMeeleWithFs.Combo3();
                break;
            
            case 104:
                _attackManagerMeeleWithFs.Combo4();
                break;
            case 105:
                _attackManagerMeeleWithFs.Combo5();
                break;
            // case 106:
            //     _attackManagerMeeleWithFs.Combo6();
            //     break;
            // case 107:
            //     _attackManagerMeeleWithFs.Combo7();
            //     break;
            // case 108:
            //     _attackManagerMeeleWithFs.Combo8();
            //     break;
            // case 1091:
            //     _attackManagerMeeleWithFs.Combo9A();
            //     break;
            // case 1092:
            //     _attackManagerMeeleWithFs.Combo9B();
            //     break;
            //
            //
            case 2011:
                _attackManagerMeeleWithFs.Skill1_AroundAttack();
                break;
            case 2012:
                _attackManagerMeeleWithFs.Skill1_BackFlip();
                break;
            case 2013:
                _attackManagerMeeleWithFs.Skill1_ThrowWeapon();
                break;
            case 2014:
                _attackManagerMeeleWithFs.Skill1_ToGround();
                break;
            case 2015:
                _attackManagerMeeleWithFs.Skill1_RespawnWeapon();
                break;
            case 2021:
                _attackManagerMeeleWithFs.Skill2_Buff();
                break;
            case 2022:
                _attackManagerMeeleWithFs.Skill2_Stab();
                break;
            case 2031:
                _attackManagerMeeleWithFs.Skill3();
                break;
            case 2032:
                _attackManagerMeeleWithFs.Skill3_Appear();
                break;
            //
            //
            // case 401:
            //     (ActorController as ActorController_c006).EventRollMove();
            //     break;
            // case 402:
            //     _attackManagerMeeleWithFs.RollAttack();
            //     break;
            //
            // case 801:
            //     _attackManagerMeeleWithFs.DragonDriveSkill(1);
            //     break;
            // case 802:
            //     _attackManagerMeeleWithFs.DragonDriveSkill(2);
            //     break;



            default:break;
        }
        

    }

    private void Skill4()
    {
        _attackManagerMeeleWithFs.Skill4_Super();
    }

}
