using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender : MonoBehaviour
{ 
    protected IHumanActor ActorController;
    protected PlayerInput _playerInput;
    protected AttackManager _attackManager;
    protected SkinnedMeshRenderer _model;
    public bool isNPC = false;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _model = transform.Find("model/mBodyAll").GetComponent<SkinnedMeshRenderer>();
        
        ActorController = transform.parent.parent.GetComponent<IHumanActor>();
        
        if(!isNPC)
            _playerInput = transform.parent.parent.GetComponent<PlayerInput>();
        
        _attackManager = transform.parent.parent.GetComponent<AttackManager>();
    }



    #region Messages Process Moudles
    
    protected void TurnToMiddle()
    {
        //transform.rotation = Quaternion.Euler(0, 120, 0);
        if((ActorController as ActorBase)?.facedir ==1)
            transform.rotation = Quaternion.Euler(0, 120, 0);
        else if((ActorController as ActorBase)?.facedir ==-1)
        {
            transform.rotation = Quaternion.Euler(0, -120, 0);
        }
    }

    protected void TurnToSide()
    {
        //transform.rotation = Quaternion.Euler(0, 102, 0);
        if((ActorController as ActorBase)?.facedir ==1)
            transform.rotation = Quaternion.Euler(0, 102, 0);
        else if((ActorController as ActorBase)?.facedir ==-1)
        {
            transform.rotation = Quaternion.Euler(0, -102, 0);
        }
    }
    
    public void onJumpEnter()
    {
        ActorController.onJumpEnter();
    }

    public void onDoubleJumpEnter()
    {
        ActorController.onDoubleJumpEnter();
    }
    public void onJumpExit()
    {
        ActorController.onJumpExit();
    }
    public void IsGround()
    {
        ActorController.IsGround();
    }
    public void isNotGround()
    {
        ActorController.isNotGround();
    }

    public virtual void onRollEnter()
    {
        //ActorController.onRollEnter();
    }

    public virtual void onRollExit()
    {
        //ActorController.onRollExit();
        //Debug.Log("ExitRoll");
    }
    public void OnFall()
    {
        ActorController.OnFall();
    }


    public virtual void OnDashEnter()
    {
        
        ActorController.OnDashEnter();
        //Debug.Log("OndashEnter");
    }
    public virtual void OnDashExit()
    {
        ActorController.OnDashExit();
    }


    public virtual void OnStandardAttackEnter()
    {
        //ActorController.OnStandardAttackEnter();

    }
    public virtual void OnStandardAttackExit()
    {
        //ActorController.OnStandardAttackExit();
    }

    

    // public virtual void OnSkillEnter()
    // {
    //     ActorController.OnSkillEnter();
    //     
    // }
    //
    // public virtual void OnSkillExit()
    // {
    //     ActorController.OnSkillExit();
    // }

    public virtual void OnGravityWeaken()
    {
        
        ActorController.OnGravityWeaken();
    }
    
    public virtual void OnGravityRecover()
    {
        ActorController.OnGravityRecover();
    }

    protected void OnHurtEnter()
    {
        ActorController?.OnHurtEnter();

    }
    
    protected void OnHurtExit()
    {
        ActorController?.OnHurtExit();
    }
    
    #endregion

    #region Signal Modify Modules
    
    
    protected void SetInputDisabled(string command)
    {
        _playerInput?.SetInputDisabled(command);
    }
    
    protected void SetInputEnabled(string command)
    {
        _playerInput?.SetInputEnabled(command);
    }
    
    protected void SetInputMove(int flag)
    {
        _playerInput?.SetInputMove(flag);
    }

    protected void SetMoveEnabled()
    {
        _playerInput?.SetMoveEnabled();
    }
    
    protected void SetMoveDisabled()
    {
        _playerInput?.SetMoveDisabled();
    }

    protected void SetRollEnabled()
    {
        _playerInput?.SetRollEnabled();
    }
    
    protected void SetRollDisabled()
    {
        _playerInput?.SetRollDisabled();
    }

    protected void SetAttackEnabled()
    {
        _playerInput?.SetAttackEnabled();
    }
    protected void SetAttackDisabled()
    {
        _playerInput?.SetAttackDisabled();
    }

    protected void LockDirection(int flag)
    {
        _playerInput.LockDirection(flag);
    }
    
    protected void InertiaMove(float time)
    {
        ActorController.InertiaMove(time);
    }
    
    public void ClearFloatSignal(string signal)
    {
        ActorController.ClearFloatSignal(signal);
    }

    public void ClearBoolSignal(string signal)
    {
        ActorController.ClearBoolSignal(signal);
    }

    public void SetInputRoll(int signal)
    {
        _playerInput.SetInputRoll(signal);
    }
    
    public void SetInputAttack(int signal)
    {
        _playerInput.SetInputAttack(signal);
    }

    #endregion

    #region Attack/Event Modules

    /// <summary>
    /// 改变表情，0.75为受伤？
    /// </summary>
    /// <param name="offset"></param>
    public void ChangeFaceExpression(float offset = 0)
    {
        //_model.materials[1].SetTextureOffset("_SurfaceInput", new Vector2(offset, 0));
        //_model.materials[2].SetTextureOffset("_SurfaceInput", new Vector2(offset, 0));
        _model.materials[1].mainTextureOffset = new Vector2(offset, 0);
        _model.materials[2].mainTextureOffset = new Vector2(offset, 0);
    }

    protected void AirDashAttack()
    {
        _attackManager.AirDashAttack();
    }

    protected void EventRoll()
    {
        ActorController.EventRoll();
    }

    protected virtual void ForceStrikeEnter()
    {
    }

    protected virtual void ForceStrikeRelease()
    {
    }

    protected void FaceDirectionAutoFix(int moveID)
    {
        ActorController.FaceDirectionAutoFix(moveID);
    }

    protected void CancelSkill()
    {
        (ActorController as ActorController)?.SkillCancelFrame();
    }

    protected void DoShapeShifting(int type)
    {
        _attackManager.ShapeShiftingAttackWave(type);
        (ActorController as ActorController).InvokeShapeShifting();
    }

    #endregion
    
}
