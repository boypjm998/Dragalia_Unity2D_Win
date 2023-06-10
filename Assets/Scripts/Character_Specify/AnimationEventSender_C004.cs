using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C004 : AnimationEventSender
{
    ActorControllerDagger _actorControllerDagger;
    AttackManagerDagger _attackManagerDagger;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _attackManagerDagger = transform.parent.parent.GetComponent<AttackManagerDagger>();
        _actorControllerDagger = transform.parent.parent.GetComponent<ActorControllerDagger>();
        
    }

    public override void OnStandardAttackEnter()
    {
        //_actorControllerDagger.OnStandardAttackEnter();
    }

    public override void OnStandardAttackExit()
    {
        //_actorControllerDagger.OnStandardAttackExit();
    }
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

    protected void Skill1(int eventID)
    {
        _attackManagerDagger.Skill1(eventID);
    }
    
    protected void Skill2(int eventID)
    {
        _attackManagerDagger.Skill2(eventID);
    }
    
    protected void Skill3(int eventID)
    {
        _attackManagerDagger.Skill3(eventID);
    }
    
    protected void Skill4(int eventID)
    {
        _attackManagerDagger.Skill4(eventID);
    }

}
