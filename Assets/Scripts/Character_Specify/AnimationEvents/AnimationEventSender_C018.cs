using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C018 : AnimationEventSender
{
    protected ActorController_c018 _actorControllerSP;
    protected AttackManager_C018 _attackManagerSP;

    protected void FaceDirectionAutoFix(int index)
    {
        _actorControllerSP.FaceDirectionAutoFix(index);
    }

    protected override void Start()
    {
        base.Start();
        _actorControllerSP = ActorController as ActorController_c018;
        _attackManagerSP = _attackManager as AttackManager_C018;
    }

    protected void AirDash(int eventID)
    {
        if (eventID == 0)
        {
            _playerInput.LockDirection(1);
            var angle = _actorControllerSP.GetNearestTargetBeforeBowJumpShot();
            _actorControllerSP.BowJumpShoot(1,angle);
            _attackManagerSP.BowJumpShootAttack(angle);
        }
        



    }

    protected override void ForceStrikeEnter()
    {
        _attackManagerSP.ForceStrikeCharging();
    }

    protected override void ForceStrikeRelease()
    {
        //_attackManagerSP.ForceStrikeRelease();
    }

    protected void Combo1(int eventID)
    {
        _attackManagerSP.ComboAttack1();
    }
    
    protected void Combo2(int eventID)
    {
        if (eventID == 0)
        {
            _attackManagerSP.ComboAttack2();
        }
        else
        {
            _attackManagerSP.ComboAttack2_2();
        }
    }
    
    protected void Combo3(int eventID)
    {
        _attackManagerSP.ComboAttack3();
    }
    
    protected void Combo4(int eventID)
    {
        if (eventID == 0)
        {
            _attackManagerSP.ComboAttack4();
        }
        else
        {
            _attackManagerSP.ComboAttack4_2();
        }
    }
    
    protected void Combo5(int eventID)
    {
        _attackManagerSP.ComboAttack5();
    }
    
    protected void Combo6(int eventID)
    {
        if (eventID == 0)
        {
            _attackManagerSP.Combo6_ForcingFX();
        }else if (eventID == 1)
        {
            _attackManagerSP.Combo6Attack();
        }
    }

    protected void Skill1(int eventID)
    {
        _attackManagerSP.Skill1(eventID);
    }
    
    protected void Skill2(int eventID)
    {
        _attackManagerSP.Skill2(eventID);
    }

    protected void Skill3(int eventID)
    {
        _attackManagerSP.Skill3(eventID);
    }

    protected void Skill4(int eventID)
    {
        _attackManagerSP.Skill4(0);
    }

}
