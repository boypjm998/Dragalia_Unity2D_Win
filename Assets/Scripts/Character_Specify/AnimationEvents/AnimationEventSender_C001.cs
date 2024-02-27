using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C001 : AnimationEventSender
{
    AttackManager_C001 _attackManager_sp;
    ActorController_c001 _actorControllerSp;
    protected override void Start()
    {
        base.Start();
        _actorControllerSp = transform.parent.parent.GetComponent<ActorController_c001>();
        //_playerInput = transform.parent.parent.GetComponent<PlayerInput>();
        _attackManager_sp = transform.parent.parent.GetComponent<AttackManager_C001>();
    }

    protected void rollAttack()
    {
        //SendMessageUpwards("rollAttack");
        _attackManager_sp.rollAttack();
    }

    protected void ComboAttack1()
    {
        _attackManager_sp.ComboAttack1();
    }
    
    protected void Skill1()
    {
        _attackManager_sp.Skill1();
    }

    protected void Skill2()
    {
        _attackManager_sp.Skill2();
    }

    protected void Skill3(int eventID)
    {
        if(eventID == 1)
            _attackManager_sp.Skill3();
        if(eventID == 2)
            _attackManager_sp.Skill3_PushBack();
        if(eventID == 3)
            _attackManager_sp.Skill3_GateOpen();
    }

    protected void Skill4()
    {
        _attackManager_sp.Skill4();
    }

    protected void Skill5(int eventID)
    {
        if (eventID == 2)
        {
            _attackManager_sp.Skill1_Boost_PushBack();
        }
        if(eventID == 1)
        {
            _attackManager_sp.Skill1_Boost();
        }
    }

    protected void Skill6(int eventID)
    {
        if(eventID == 1)
        {
            _attackManager_sp.Skill2_Muzzle();
        }
        if(eventID == 2)
        {
            _attackManager_sp.Skill2_Boost();
        }
    }

    protected void Skill7(int eventID)
    {
        if(eventID == 1)
        {
            _attackManager_sp.Skill3_Boost_Muzzle();
        }
        if(eventID == 2)
        {
            _attackManager_sp.Skill3_Boost();
        }
    }
       
    

}
