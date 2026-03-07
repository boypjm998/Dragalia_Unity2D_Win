using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C047 : AnimationEventSenderNew
{
    private AttackManager_C047 _attackManagerRanged;

    protected override void Start()
    {
        base.Start();
        _attackManagerRanged = _attackManager as AttackManager_C047;
    }

    protected override void AttackAction(int actionID)
    {
        switch (actionID)
        {
            case 101:
                _attackManagerRanged.ComboAttack1();
                break;
            case 102:
                _attackManagerRanged.ComboAttack2();
                break;
            case 103:
                _attackManagerRanged.ComboAttack3();
                break;
            case 104:
                _attackManagerRanged.ComboAttack4();
                break;
            case 105:
                _attackManagerRanged.ComboAttack5();
                break;


            
            case 2011:
                _attackManagerRanged.Skill1_Heal();
                break;
            case 2012:
                _attackManagerRanged.Skill1_Attack();
                break;
            
            case 2021:
                _attackManagerRanged.Skill2_Buff();
                break;
            
            case 2031:
                _attackManagerRanged.Skill3_Muzzle();
                break;
            case 2032:
                _attackManagerRanged.Skill3_Attack();
                break;
           
            //
            //
            // case 301:
            //     _attackManagerMeeleWithFs.ForceStrike_Axe();
            //     break;



            default: break;
        }
    }

    private void Skill4()
    {
        _attackManagerRanged.Skill4(0);
    }
}
