using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C030 : AnimationEventSenderNew
{
    protected ActorControllerRangedWithFS _actorControllerSP;
    protected AttackManager_C030 _attackManagerSP;

    protected override void Start()
    {
        base.Start();
        _actorControllerSP = ActorController as ActorControllerRangedWithFS;
        _attackManagerSP = _attackManager as AttackManager_C030;
    }

    protected override void AttackAction(int actionID)
    {
        switch (actionID)
        {
            case 2011:
                _attackManagerSP.Skill1_Muzzle();
                break;
            case 2021:
                _attackManagerSP.Skill2();
                break;


        }
    }
    
    protected override void ForceStrikeEnter()
    {
        _attackManagerSP.ForceStrikeCharging();
    }
    
    protected void AirDash(int eventID)
    {
        if (eventID == 0)
        {
            _actorControllerSP.ta.FaceDirectionAutofixWithMarking();
            _playerInput.LockDirection(1);
            var angle = _actorControllerSP.GetNearestTargetBeforeBowJumpShot();
            _actorControllerSP.BowJumpShoot(1,angle);
            _attackManagerSP.BowJumpShootAttack(angle);
        }

    }
    
    protected void Combo1(int eventID)
    {
        _attackManagerSP.BowCombo1();
    }
    
    protected void Combo2(int eventID)
    {
        if (eventID == 0)
        {
            _attackManagerSP.BowCombo2_1();
        }
        else
        {
            _attackManagerSP.BowCombo2_2();
        }
    }
    
    protected void Combo3(int eventID)
    {
        _attackManagerSP.BowCombo3();
    }
    
    protected void Combo4(int eventID)
    {
        if (eventID == 0)
        {
            _attackManagerSP.BowCombo4_1();
        }
        else
        {
            _attackManagerSP.BowCombo4_2();
        }
    }
    
    protected void Combo5(int eventID)
    {
        _attackManagerSP.BowCombo5();
    }

    private void Skill4(int eventID)
    {
        _attackManagerSP.Skill4(0);
    }
}
