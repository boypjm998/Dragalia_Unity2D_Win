using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C003 : AnimationEventSender
{
    
    AttackManager_C003 _attackManagerSP;
    ActorController_c003 _actorControllerSP;

    protected override void Start()
    {
        base.Start();
        _attackManagerSP = _attackManager as AttackManager_C003;
        _actorControllerSP = ActorController as ActorController_c003;
    }

    protected void TwilightMoon()
    {
        _attackManagerSP.TwilightMoon();
    }

    protected void TwilightMoonRelease()
    {
        //_attackManagerSP.TwilightMoonRelease();
    }

    protected void Combo1(int eventID)
    {
        if (eventID == 1)
        {
            //_attackManagerSP.ComboAttack_Rush(12,"combo1");
        }
        else
        {
            _attackManagerSP.ComboAttack1();
        }
    }

    protected void Combo2(int eventID)
    {
        if (eventID == 1)
        {
            //_attackManagerSP.ComboAttack_Rush(3.5f,"combo2");
        }
        else
        {
            _attackManagerSP.ComboAttack2();
        }
    }
    
    protected void Combo3(int eventID)
    {
        if (eventID == 1)
        {
            //_attackManagerSP.ComboAttack_Rush(3f,"combo3");
        }
        else
        {
            _attackManagerSP.ComboAttack3();
        }
    }
    
    protected void Combo4(int eventID)
    {
        
        //_attackManagerDagger.ComboAttack_Rush(0.5f,"combo4");
        
        _attackManagerSP.ComboAttack4();
        
    }
    
    protected void Combo5(int eventID)
    {
        
        _attackManagerSP.ComboAttack5();
        
    }

    public override void OnStandardAttackEnter()
    {
        
    }
    public override void OnStandardAttackExit()
    {
        
    }

    protected void Skill1(int eventID)
    {
        _attackManagerSP.Skill1(1);
        _actorControllerSP.Skill1(1);
    }
    protected void Skill2(int eventID)
    {
        _attackManagerSP.Skill2(eventID);
        _actorControllerSP.Skill2(eventID);
    }
    protected void Skill3(int eventID)
    {
        _attackManagerSP.Skill3(eventID);
        _actorControllerSP.Skill3(eventID);
    }
    protected void Skill4(int eventID)
    {
        _attackManagerSP.Skill4(eventID);
        _actorControllerSP.Skill4(eventID);
    }
    
    
}
