using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C002 : AnimationEventSender
{
    private ActorController_c002 _actorControllerSP;
    private AttackManager_C002 _attackManagerDagger;

    protected override void Start()
    {
        base.Start();
        _actorControllerSP = ActorController as ActorController_c002;
        _attackManagerDagger = _attackManager as AttackManager_C002;
    }
    
    // public override void OnStandardAttackEnter()
    // {
    //     //_actorControllerDagger.OnStandardAttackEnter();
    // }
    //
    // public override void OnStandardAttackExit()
    // {
    //     //_actorControllerDagger.OnStandardAttackExit();
    // }
    
    protected void Combo1(int eventID)
    {
        if (eventID == 1)
        {
            _attackManagerDagger.ComboAttack_Rush(12,"combo1");
        }
        else
        {
            _attackManagerDagger.ComboAttack1();
        }
    }

    protected void Combo2(int eventID)
    {
        if (eventID == 1)
        {
            _attackManagerDagger.ComboAttack_Rush(3.5f,"combo2");
        }
        else
        {
            _attackManagerDagger.ComboAttack2();
        }
    }
    
    protected void Combo3(int eventID)
    {
        if (eventID == 1)
        {
            _attackManagerDagger.ComboAttack_Rush(3f,"combo3");
        }
        else
        {
            _attackManagerDagger.ComboAttack3();
        }
    }
    
    protected void Combo4(int eventID)
    {
        
        //_attackManagerDagger.ComboAttack_Rush(0.5f,"combo4");
        
        _attackManagerDagger.ComboAttack4();
        
    }
    
    protected void Combo5(int eventID)
    {
        
        _attackManagerDagger.ComboAttack5();
        
    }

    private void Skill1(int id)
    {
        _actorControllerSP.Skill1(id);
        _attackManagerDagger.Skill1(id);
    }

    private void Skill2(int id)
    {
        _attackManagerDagger.Skill2(id);
    }

    private void Skill3(int id)
    {
        _attackManagerDagger.Skill3(id);
        _actorControllerSP.Skill3(id);
    }

    private void Skill4(int id)
    {
        _attackManagerDagger.Skill4(id);
    }

}
