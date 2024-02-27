using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C005 : AnimationEventSender
{
    ActorController_c005 _actorControllerDagger;
    AttackManager_C005 _attackManagerDagger;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _attackManagerDagger = transform.parent.parent.GetComponent<AttackManager_C005>();
        _actorControllerDagger = transform.parent.parent.GetComponent<ActorController_c005>();
        
    }

    // public override void onRollEnter()
    // {
    //     
    // }
    //
    // public override void onRollExit()
    // {
    //     
    // }
    //
    // public override void OnStandardAttackEnter()
    // {
    //     //_actorControllerDagger.OnStandardAttackEnter();
    // }
    //
    // public override void OnStandardAttackExit()
    // {
    //     //_actorControllerDagger.OnStandardAttackExit();
    // }

    protected void CheckSPRecharge(int skillID)
    {
        _actorControllerDagger.SkillPrepCheck(skillID);
    }


    protected void Combo1(int eventID)
    {
        switch (eventID)
        {
            default:
            {
                _attackManagerDagger.Combo1_Attack();
                break;
            }
        }
    }

    protected void Combo2(int eventID)
    {
        switch (eventID)
        {
            case 0:
            {
                //冲刺
                _attackManagerDagger.Combo2_Attack();
                _attackManagerDagger.ComboAttack_Rush(10,"combo2",8,true);
                break;
            }

        }
    }
    
    protected void Combo3(int eventID)
    {
        switch (eventID)
        {
            case 0:
            {
                _attackManagerDagger.Combo3_Attack();
                break;
            }

        }
    }
    
    protected void Combo4(int eventID)
    {

        switch (eventID)
        {
            case 0:
            {
                _actorControllerDagger.Combo4_JumpBackward();
                _attackManagerDagger.Combo4_Attack1();
                break;
            }
            case 1:
            {
                _actorControllerDagger.Combo4_JumpSmashForward();
                _attackManagerDagger.Combo4_Attack2();
                break;
            }


        }
        
    }
    
    protected void Combo5(int eventID)
    {
        
        switch (eventID)
        {
            case 0:
            {
                _attackManagerDagger.Combo5_Attack();
                break;
            }


        }
        
    }
    
    protected void Combo6(int eventID)
    {
        switch (eventID)
        {
            case 0:
            {
                _actorControllerDagger.Combo6_RecordLastPosition();
                _attackManagerDagger.Combo6_Attack();
                _attackManagerDagger.ComboAttack_RushPenetrate(8,0.2f,"combo6",true);
                break;
            }
        }
    }

    protected void Combo7(int eventID)
    {
        switch (eventID)
        {
            case 0:
            {
                _actorControllerDagger.Combo7_ReturnToLastPosition();
                _actorControllerDagger.FaceDirectionAutoFix(1);
                break;
            }
            case 1:
            {
                _attackManagerDagger.Combo7_Attack1();
                break;
            }
            case 2:
            {
                _actorControllerDagger.Combo7_WarpToHighPoint();
                break;
            }
            case 3:
            {
                _attackManagerDagger.Combo7_Attack2();
                _actorControllerDagger.Combo7_JumpSmashForward();
                break;
            }
        }
        
        
        
    }
    


    protected void Skill1(int eventID)
    {
        switch (eventID)
        {
            case 0:
            {
                //释放攻击
                _attackManagerDagger.Skill1();
                break;
            }
            case 1:
            {
                //冲刺
                _actorControllerDagger.Skill1_DashForward();
                break;
            }
        }
        
    }
    
    protected void Skill2(int eventID)
    {
        _attackManagerDagger.Skill2();
    }
    
    protected void Skill3(int eventID)
    {
        switch (eventID)
        {
            case 0:
            {
                _attackManagerDagger.Skill3(0);
                _actorControllerDagger.Skill3_WarpMove();
                break;
            }
            case 1:
            {
                _actorControllerDagger.Skill3_SlashDownward();
                _attackManagerDagger.Skill3(1);
                break;
            }
        }
    }

    protected void CallShadow(int skillID)
    {
        
    }

    protected void Skill4(int eventID)
    {
        _attackManagerDagger.Skill4(eventID);
    }

    protected void Skill5(int eventID)
    {
        switch (eventID)
        {
            case 1:
            {
                //冲刺
                _actorControllerDagger.Skill1_DashForward();
                break;
            }
            case 3:
            {
                _actorControllerDagger.SetFaceDir(_actorControllerDagger.facedir*-1);
                break;
            }
            default:
            {
                _attackManagerDagger.Skill5(eventID);
                break;
            }
        }
        
    }

    protected void Skill6(int eventID)
    {
        _attackManagerDagger.Skill6();
    }

    protected void DodgeMoveEnter()
    {
        if (_actorControllerDagger.grounded)
        {
            
        }
        else
        {
            _actorControllerDagger.QuickLanding();
            _actorControllerDagger.ResetGravityScale();
            
        }
        _actorControllerDagger.EventRollAttack();
        _attackManagerDagger.RollAttack();
        
        
    }
    
    protected void DodgeMoveExit()
    {
        _actorControllerDagger.AppearRenderer();
    }
    
    

}
