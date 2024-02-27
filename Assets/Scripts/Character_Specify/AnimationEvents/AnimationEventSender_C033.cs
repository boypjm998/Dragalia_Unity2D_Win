using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C033 : AnimationEventSenderNew
{
    ActorControllerMeeleWithFS _actorControllerMeeleWithFS;
    AttackManager_C033 _attackManagerMeeleWithFs;

    protected override void Start()
    {
        base.Start();
        _attackManagerMeeleWithFs = _attackManager as AttackManager_C033;
        _actorControllerMeeleWithFS = ActorController as ActorControllerMeeleWithFS;
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
                _actorControllerMeeleWithFS.BladeBackStep();
                break;
            
            case 104:
                _attackManagerMeeleWithFs.Combo4();
                break;
            case 105:
                _attackManagerMeeleWithFs.Combo5();
                break;
            case 1051:
                _actorControllerMeeleWithFS.BladeForwardStep();
                break;
            
            case 200:
                _attackManagerMeeleWithFs.Skill_ScreenFX();
                break;
            case 2011:
                _attackManagerMeeleWithFs.Skill1_Attack();
                break;
            case 2021:
                _attackManagerMeeleWithFs.Skill_ScreenFX(false);
                _attackManagerMeeleWithFs.Skill2_ScanEffect();
                break;
            case 2022:
                _attackManagerMeeleWithFs.Skill2_ThrowProjectile();
                break;
            case 2031:
                _attackManagerMeeleWithFs.Skill_ScreenFX(false);
                _attackManagerMeeleWithFs.Skill2_ScanEffect();
                break;
            case 2032:
                _attackManagerMeeleWithFs.Skill3_AimProjectile();
                break;
            

            default:break;
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

    protected void Skill4()
    {
        _attackManagerMeeleWithFs.Skill4_Buff();
    }
    
    


}
