using System.Collections;
using DG.Tweening;
using UnityEngine;

public interface ICharacterSpecialGauge
{
    public void Reset();
    public void Charge(int cp);
    public void ChargeTo(int cp, int level = 0);
}

public interface IEnemySealedContainer
{
    public void SetEnemySource(GameObject source);
    
}

public interface IForceAttackable
{
    /// <summary>
    /// 在进入爆发攻击蓄力动画时调用
    /// </summary>
    public void OnForceEnter();
    /// <summary>
    /// 爆发攻击可以选择方向的执行方法
    /// </summary>
    public void OnForcingPointerSelectable();
    /// <summary>
    /// 爆发攻击可移动的执行方法
    /// </summary>
    public void OnForcingMoveable();
    /// <summary>
    /// 开始爆发攻击
    /// </summary>
    public void OnForceAttackEnter();
    /// <summary>
    /// 退出爆发攻击阶段
    /// </summary>
    public void OnForceAttackExit();
    //public void ForceAttackRelease();
}

public interface IKnockbackable
{
    public bool GetDodge();

    public virtual void InvokeDodge(AttackBase atk, GameObject source)
    {
    }

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

public interface ISpeedControllable
{
    public void SetRate(int ComponentID, float rate);
    public void GetTargetComponents();
}