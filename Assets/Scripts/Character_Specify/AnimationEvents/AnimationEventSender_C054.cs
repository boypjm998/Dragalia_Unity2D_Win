using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C054 : AnimationEventSenderNew
{
    private AttackManager_C054 _attackManagerMeeleWithFs;
    private ActorControllerMeeleWithFS _actorControllerMeeleWithFs;
    
    
    protected override void Start()
    {
        base.Start();
        _actorControllerMeeleWithFs = (ActorController as ActorControllerMeeleWithFS);
        _attackManagerMeeleWithFs = GetComponentInParent<AttackManager_C054>();
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
            
            case 2011:
                _attackManagerMeeleWithFs.Skill1_ForwardDash();
                _actorControllerMeeleWithFs.GeneralHorizontalMovementWithEnemyCheck(7,2,2,0.3f);
                break;
            case 2012:
                _actorControllerMeeleWithFs.FaceDirectionAutoFix(5);
                _attackManagerMeeleWithFs.Skill1_Jump();
                break;
            case 2013:
                
                _attackManagerMeeleWithFs.Skill1_Smashdown();
                break;
            
            case 2021:
                _attackManagerMeeleWithFs.Skill2_CounterStart();
                break;
            
            case 2022:
                _attackManagerMeeleWithFs.Skill2_NoCounter();
                break;
            
            case 2023:
                
                _attackManagerMeeleWithFs.Skill2_JumpAimingTarget();
                break;
            case 2024:
                _attackManagerMeeleWithFs.Skill2_Smashdown();
                break;
            
            case 2031:
                _attackManagerMeeleWithFs.Skill3_Buff();
                break;
            
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
        _attackManagerMeeleWithFs.Skill4();
    }
}
