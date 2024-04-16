using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimationEventSender_C012 : AnimationEventSenderNew
{
    AttackManager_C012 _attackManagerSP;
    ActorController_c012 _actorControllerSP;
    
    protected override void Start()
    {
        base.Start();
        _attackManagerSP = _attackManager as AttackManager_C012;
        _actorControllerSP = ActorController as ActorController_c012;
    }

    protected override void AttackAction(int actionID)
    {
        switch (actionID)
        {
            case 101:
                _attackManagerSP.Combo1();
                _actorControllerSP.GeneralHorizontalMovementWithEnemyCheck(7f,3f,1.5f,0.2f);
                break;
            case 102:
                _attackManagerSP.Combo2();
                break;
            case 103:
                _attackManagerSP.Combo3();
                _actorControllerSP.GeneralHorizontalMovementWithEnemyCheck(1f,3f,1.5f,0.2f);
                break;
            case 104:
                _attackManagerSP.Combo4();
                break;
            case 105:
                _attackManagerSP.Combo5();
                break;
            
            case 2001:
                _attackManagerSP.ClearEnemyStatusAndCheckNextChain(1);
                break;
            case 2002:
                _attackManagerSP.ClearEnemyStatusAndCheckNextChain(2);
                break;
            
            case 2011:
                _attackManagerSP.Skill1_Dash();
                _actorControllerSP.GeneralHorizontalMovementWithEnemyCheck(5f,3f,1.5f,0.2f);
                break;
            case 2012:
                _attackManagerSP.Skill1_Slash();
                break;
            case 2013:
                _actorControllerSP.Skill1_BackStep();
                break;
            
            case 2021:
                _actorControllerSP.Skill2_BackFlip();
                break;
            case 2022:
                _attackManagerSP.Skill2_Attack();
                break;
            case 2023:
                _actorControllerSP.Skill2_ToGround();
                break;
            
        }
    }

    protected void Skill4(int eventID)
    {
        _attackManagerSP.Skill4();
    }
}
