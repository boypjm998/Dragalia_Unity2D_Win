using System.Collections;
using DG.Tweening;
using UnityEngine;

public interface ICharacterSpecialGauge
{
    public void Reset();
    public void Charge(int cp);
    public void ChargeTo(int cp, int level = 0);
}

public interface IKnockbackable
{
    public bool GetDodge();
    public void TakeDamage(float kbPower, float kbtime, float kbForce, Vector2 kbDir);
}

public interface IHumanActor
{
    public virtual void OnHurtExit()
    {
    }
    public virtual void OnHurtEnter()
    {
    }
    
    protected virtual void OnDeath()
    {
    }
    public void onJumpEnter()
    {
    }

    public void onDoubleJumpEnter()
    {
    }
    public void onJumpExit()
    {
    }
    public void IsGround()
    {
    }
    public void isNotGround()
    {
    }

    public virtual void onRollEnter()
    {
    }

    public virtual void onRollExit()
    {
    }
    public void OnFall()
    {
    }


    public virtual void OnDashEnter()
    {
        
    }
    public virtual void OnDashExit()
    {
    }


    public virtual void OnStandardAttackEnter()
    {

    }
    public virtual void OnStandardAttackExit()
    {
    }

    

    public virtual void OnSkillEnter()
    {

    }

    public virtual void OnSkillExit()
    {
       
    }

    public virtual void OnGravityWeaken()
    {
        
    }
    
    public virtual void OnGravityRecover()
    {
    }
    
    public void ClearFloatSignal(string signal)
    {
    }

    public void ClearBoolSignal(string signal)
    {
    }
    
    public void InertiaMove(float time)
    {
    }
    
    public virtual void FaceDirectionAutoFix(int moveID)
    {
        
    }

    public void EventRoll()
    {
    }
    
    public IEnumerator HorizontalMoveFixedTime(float targetPosition, float time, string move, Ease ease = Ease.Linear)
    {
        yield break;
    }
    public IEnumerator HorizontalMove(float speed, float time, string move)
    {
        yield break;
    }
    
    public IEnumerator HorizontalMove(float speed, float acceration, float time, string move)
    {
        yield break;
    }

    

    


}