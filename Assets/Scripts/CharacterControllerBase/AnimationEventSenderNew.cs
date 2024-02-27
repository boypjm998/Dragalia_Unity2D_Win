using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSenderNew : AnimationEventSender
{
    protected void SetWeaponVisibility(int visibility)
    {
        (ActorController as ActorController).SetWeaponVisibility(visibility == 1);
    }

    
    protected virtual void AttackAction(int actionID)
    {
        
        
        
        
        
    }
    
    protected void FaceDirectionAutoFixWithManual(int typeID)
    {
        if (_playerInput.buttonLeft.IsPressing && !_playerInput.buttonRight.IsPressing)
        {
            (ActorController as ActorController).SetFaceDir(-1);
        }
        else if (!_playerInput.buttonLeft.IsPressing && _playerInput.buttonRight.IsPressing)
        {
            (ActorController as ActorController).SetFaceDir(1);
        }
        else
        {
            ActorController.FaceDirectionAutoFix(typeID);
        }
    }
    
    
    
    
    
    #region Override Messages Process Moudles
    
    // protected void TurnToMiddle()
    // {
    //     
    // }
    //
    // protected void TurnToSide()
    // {
    //     
    // }
    //
    // public void onJumpEnter()
    // {
    //     
    // }
    //
    // public void onDoubleJumpEnter()
    // {
    //     //ActorController.onDoubleJumpEnter();
    // }
    // public void onJumpExit()
    // {
    //     //ActorController.onJumpExit();
    // }
    // public void IsGround()
    // {
    //     //ActorController.IsGround();
    // }
    // public void isNotGround()
    // {
    //     //.isNotGround();
    // }
    //
    // public virtual void onRollEnter()
    // {
    //     //ActorController.onRollEnter();
    // }
    //
    // public virtual void onRollExit()
    // {
    //     //ActorController.onRollExit();
    //     //Debug.Log("ExitRoll");
    // }
    // public void OnFall()
    // {
    //     
    // }
    //
    //
    // public virtual void OnDashEnter()
    // {
    //     
    //     
    // }
    // public virtual void OnDashExit()
    // {
    //     
    // }
    //
    //
    // public virtual void OnStandardAttackEnter()
    // {
    //     //ActorController.OnStandardAttackEnter();
    //
    // }
    // public virtual void OnStandardAttackExit()
    // {
    //     //ActorController.OnStandardAttackExit();
    // }
    //
    //
    // public virtual void OnGravityWeaken()
    // {
    //     
    //     //ActorController.OnGravityWeaken();
    // }
    //
    // public virtual void OnGravityRecover()
    // {
    //     //ActorController.OnGravityRecover();
    // }
    //
    // protected void OnHurtEnter()
    // {
    //     //ActorController?.OnHurtEnter();
    //
    // }
    //
    // protected void OnHurtExit()
    // {
    //     //ActorController?.OnHurtExit();
    // }
    
    #endregion
}
