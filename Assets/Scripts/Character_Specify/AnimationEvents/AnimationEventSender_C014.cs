using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C014 : AnimationEventSenderNew
{
    ActorControllerMeeleWithFS _actorControllerMeeleWithFs;
    AttackManager_C014 _attackManagerMeeleWithFs;

    protected override void Start()
    {
        base.Start();
        _attackManagerMeeleWithFs = _attackManager as AttackManager_C014;
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
                _attackManagerMeeleWithFs.Combo2();
                break;
            case 103:
                _attackManagerMeeleWithFs.Combo3();
                break;
            case 1041:
                _actorControllerMeeleWithFs.BladeBackStep();
                break;

            case 104:
                _attackManagerMeeleWithFs.Combo4();
                break;
            case 105:
                _attackManagerMeeleWithFs.Combo5();
                break;
            case 1051:
                _actorControllerMeeleWithFs.BladeForwardStep();
                break;
            
            case 2010:
                _attackManagerMeeleWithFs.Skill_Flash();
                break;
            
            case 2011:
                _attackManagerMeeleWithFs.Skill1_Bleeding(0);
                break;
            case 2012:
                _attackManagerMeeleWithFs.Skill1_Bleeding(1);
                break;
            case 2013:
                _attackManagerMeeleWithFs.Skill1_Bleeding(2);
                _actorControllerMeeleWithFs.GeneralHorizontalMovementWithEnemyCheck(1f,3f,1.5f,0.2f);
                break;
            case 2014:
                _attackManagerMeeleWithFs.Skill1_Bleeding(3);
                _actorControllerMeeleWithFs.GeneralHorizontalMovementWithEnemyCheck(1f,3f,1.5f,0.2f);
                break;
            case 2015:
                _attackManagerMeeleWithFs.Skill1_Bleeding(4);
                break;
            case 2016:
                _attackManagerMeeleWithFs.Skill1_Bleeding(5);
                break;
            case 2017:
                _attackManagerMeeleWithFs.Skill1_Bleeding(6);
                break;
            case 2018:
                _attackManagerMeeleWithFs.Skill1_Bleeding(7);
                break;
            
            case 2021:
                _attackManagerMeeleWithFs.Skill2();
                break;
            
            case 2031:
                _attackManagerMeeleWithFs.Skill3_Attack();
                break;

        }
    }


    protected void OnStandardAttackEnter()
    {
        SetWeaponVisibility(1);
    }

    protected void OnStandardAttackExit()
    {
        SetWeaponVisibility(0);
    }
    
    public void OnForceAttackExit()
    {
        SetWeaponVisibility(0);
    }

    protected void onRollEnter()
    {
        SetWeaponVisibility(0);
    }

    protected void OnHurtExit()
    {
        SetWeaponVisibility(0);
    }

    protected void OnSkillEnter()
    {
        SetWeaponVisibility(1);
    }

    protected void OnSkillExit()
    {
        SetWeaponVisibility(0);
    }
    protected void Skill4()
    {
        _attackManagerMeeleWithFs.Skill4();
    }
}
