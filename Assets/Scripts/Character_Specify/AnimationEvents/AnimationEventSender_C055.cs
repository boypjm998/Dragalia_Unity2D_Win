using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C055 : AnimationEventSenderNew
{
    private AttackManager_C055 _attackManagerMeeleWithFs;
    private ActorControllerMeeleWithFS _actorControllerMeeleWithFs;

    protected override void Start()
    {
        base.Start();
        _attackManagerMeeleWithFs = _attackManager as AttackManager_C055;
        _actorControllerMeeleWithFs = ActorController as ActorControllerMeeleWithFS;
    }

    protected override void AttackAction(int actionID)
    {
        
        switch (actionID)
        {
            case 101:
                _attackManagerMeeleWithFs.Combo1();
                break;
            case 102:
                _actorControllerMeeleWithFs.GeneralHorizontalMovementWithEnemyCheck(1f,3f,1.5f,0.2f);
                _attackManagerMeeleWithFs.Combo2();
                break;
            case 103:
                _actorControllerMeeleWithFs.GeneralHorizontalMovementWithEnemyCheck(1f,3f,1.5f,0.2f);
                _attackManagerMeeleWithFs.Combo3();
                break;
            case 104:
                _actorControllerMeeleWithFs.GeneralHorizontalMovementWithEnemyCheck(1f,3f,1.5f,0.2f);
                _attackManagerMeeleWithFs.Combo4();
                break;
            case 105:
                _attackManagerMeeleWithFs.Combo5();
                break;
            
            case 2011:
                _attackManagerMeeleWithFs.Skill1();
                break;
            case 2021:
                _attackManagerMeeleWithFs.Skill2();
                break;
            
            
            
            case 301:
                _attackManagerMeeleWithFs.ForceStrike_Axe();
                break;
            
            
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

    public void Skill4()
    {
        _attackManagerMeeleWithFs.Skill4();
    }
}
