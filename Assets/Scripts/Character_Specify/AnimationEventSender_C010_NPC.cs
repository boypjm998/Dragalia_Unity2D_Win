using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C010_NPC : AnimationEventSender
{
    // Start is called before the first frame update
    private AttackManager_C010 _attackManagerSp;
    
    protected override void Start()
    {
        base.Start();
        _attackManagerSp = transform.parent.parent.GetComponent<AttackManager_C010>();
    }
    
    
    protected void Combo1(int eventID)
    {
        if (eventID == 2)
        {
            _attackManagerSp.ComboAttack1_Rush();
        }
        else
        {
            _attackManagerSp.ComboAttack1();
        }
    }

    protected void Combo2()
    {
        _attackManagerSp.ComboAttack2();
    }

    protected void Combo3()
    {
        _attackManagerSp.ComboAttack3();
    }

    public override void OnStandardAttackEnter()
    {
        
    }

    protected void Skill1(int eventID)
    {
        if (eventID == 1)
        {
            _attackManagerSp.Skill1_Disappear();
        }
        if(eventID == 2)
        {
            _attackManagerSp.Skill1_Wait();
        }
        if(eventID == 3)
        {
            _attackManagerSp.Skill1_Appear();
        }
    }

    protected void Skill4()
    {
        _attackManagerSp.Skill4(0);
    }

    protected void ReplayAnim()
    {
        _attackManagerSp.Skill1_Replay();
    }

    protected void Skill2()
    {
        _attackManagerSp.Skill2();
    }
}
