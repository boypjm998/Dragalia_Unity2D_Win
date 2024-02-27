using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C006 : AnimationEventSenderNew
{
    
    AttackManager_C006 _attackManagerMeeleWithFs;

    protected override void Start()
    {
        base.Start();
        _attackManagerMeeleWithFs = GetComponentInParent<AttackManager_C006>();
    }


    protected override void ForceStrikeEnter()
    {
        //_attackManagerMeeleWithFs.ForceStrikeCharging();
    }

    protected override void ForceStrikeRelease()
    {
        //_attackManagerMeeleWithFs.ForceStrikeRelease(1);
    }

    protected void Skill4()
    {
        _attackManagerMeeleWithFs.Skill4();
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
            case 1031:
                _attackManagerMeeleWithFs.Combo3();
                break;
            case 1032:
                _attackManagerMeeleWithFs.Combo3_Dash();
                break;
            case 104:
                _attackManagerMeeleWithFs.Combo4();
                break;
            case 105:
                _attackManagerMeeleWithFs.Combo5();
                break;
            case 106:
                _attackManagerMeeleWithFs.Combo6();
                break;
            case 107:
                _attackManagerMeeleWithFs.Combo7();
                break;
            case 108:
                _attackManagerMeeleWithFs.Combo8();
                break;
            case 1091:
                _attackManagerMeeleWithFs.Combo9A();
                break;
            case 1092:
                _attackManagerMeeleWithFs.Combo9B();
                break;
            
            
            case 2011:
                _attackManagerMeeleWithFs.Skill1(1);
                break;
            case 2012:
                _attackManagerMeeleWithFs.Skill1(2);
                break;
            case 202:
                _attackManagerMeeleWithFs.Skill2();
                break;


            case 401:
                (ActorController as ActorController_c006).EventRollMove();
                break;
            case 402:
                _attackManagerMeeleWithFs.RollAttack();
                break;
            
            case 801:
                _attackManagerMeeleWithFs.DragonDriveSkill(1);
                break;
            case 802:
                _attackManagerMeeleWithFs.DragonDriveSkill(2);
                break;



            default:break;
        }
        
    }

    // protected void FaceDirectionAutoFixWithManual(int typeID)
    // {
    //     if (_playerInput.buttonLeft.IsPressing && !_playerInput.buttonRight.IsPressing)
    //     {
    //         (ActorController as ActorController).SetFaceDir(-1);
    //     }
    //     else if (!_playerInput.buttonLeft.IsPressing && _playerInput.buttonRight.IsPressing)
    //     {
    //         (ActorController as ActorController).SetFaceDir(1);
    //     }
    //     else
    //     {
    //         ActorController.FaceDirectionAutoFix(typeID);
    //     }
    // }
    
}
