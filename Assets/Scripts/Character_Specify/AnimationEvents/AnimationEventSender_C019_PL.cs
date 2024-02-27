using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C019_PL : AnimationEventSenderNew
{
    private AttackManager_C019_PL _attackManagerPL;

    protected override void Start()
    {
        base.Start();
        _attackManagerPL = _attackManager as AttackManager_C019_PL;
    }

    protected override void AttackAction(int actionID)
    {
        switch (actionID)
        {
            case 101:
                _attackManagerPL.Combo1();
                break;
            case 102:
                _attackManagerPL.Combo2();
                break;
            case 103:
                _attackManagerPL.Combo3();
                break;
            case 104:
                _attackManagerPL.Combo4();
                break;
            case 1051:
                _attackManagerPL.Combo5();
                break;
            case 1052:
                _attackManagerPL.Combo5_ShineBall();
                break;
            
            case 2011:
                _attackManagerPL.HideWeapon();
                break;
            case 2012:
                _attackManagerPL.Skill1_Muzzle();
                break;
            
            case 2021:
                _attackManagerPL.DisplayWeapon();
                break;
            case 2022:
                _attackManagerPL.Skill2_Effect();
                break;
            case 2023:
                _attackManagerPL.Skill2();
                break;
            case 2031:
                _attackManagerPL.Skill3_Float();
                break;
            case 2032:
                _attackManagerPL.Skill3();
                break;
            case 2033:
                _attackManagerPL.Skill3_FloatPurge();
                break;
            
            case 2052:
                _attackManagerPL.Skill5_Muzzle();
                break;
            case 2061:
                _attackManagerPL.Skill2_HealEffect();
                break;
            case 2062:
                _attackManagerPL.Skill2_Heal();
                break;
        }
    }

    protected void Skill4(int eventID)
    {
        _attackManagerPL.Skill4();
    }
}
