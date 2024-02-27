using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;

public abstract class ActorBase : MonoBehaviour
{
    public delegate void OnHurt();
    public bool isAlive = true;
    protected GameObject hitSensor;
    protected Collider2D groundSensor;
    
    

    public Collider2D HitSensor
    {
        get => hitSensor?.GetComponent<Collider2D>();
    }

    public bool IsInvincible
    {
        get => (!hitSensor.activeSelf || !HitSensor.enabled);
    }

    public Animator anim;
    public Rigidbody2D rigid;
    public OnHurt OnAttackInterrupt;   
    public int facedir = 1;
    public float speedModifier = 1;
    public bool isBog { get; protected set; }
    
    public static float DefaultGravity = 4;
    protected Tweener _tweener;
    public float ModelDepth => anim.transform.position.z;

    protected virtual void Awake()
    {
        groundSensor = transform.Find("GroundSensor")?.GetComponent<Collider2D>();
    }


    public virtual void TakeDamage(float kbPower, float kbtime, float kbForce, Vector2 kbDir)
    {
    }

    public abstract void TakeDamage(AttackBase attackBase,Vector2 kbdir);
    public abstract void TakeDamage(AttackInfo attackInfo, Vector2 kbdir);

    public virtual void DisappearRenderer()
    {
    }
    
    public virtual void AppearRenderer()
    {
    }

    public virtual void SetMoveSpeed(float spd)
    {
        
    }


    protected void TurnToMiddle()
    {
        //transform.rotation = Quaternion.Euler(0, 120, 0);
        if(facedir ==1)
            anim.transform.rotation = Quaternion.Euler(0, 120, 0);
        else if(facedir ==-1)
        {
            anim.transform.rotation = Quaternion.Euler(0, -120, 0);
        }
    }

    protected void TurnToSide()
    {
        //transform.rotation = Quaternion.Euler(0, 102, 0);
        if(facedir ==1)
            anim.transform.rotation = Quaternion.Euler(0, 102, 0);
        else if(facedir ==-1)
        {
            anim.transform.rotation = Quaternion.Euler(0, -102, 0);
        }
    }
    

    public float GetActorHeight()
    {
        if (groundSensor == null)
            return transform.position.y;
        return transform.position.y - groundSensor.bounds.min.y;
        //return transform.position.y - transform.Find("GroundSensor").GetComponent<Collider2D>().bounds.min.y;
    }

    public void SetHitSensor(bool flag)
    {
        hitSensor.SetActive(flag);
    }

    public virtual void SetActionUnable(bool flag)
    {
        
    }

    public virtual void SetGravityScale(float value)
    {
        
    }

    public virtual void ResetGravityScale()
    {
        
    }
    
    public void TurnMove(GameObject target)
    {
        if (target.transform.position.x > transform.position.x)
        {
            SetFaceDir(1);
        }
        if (target.transform.position.x < transform.position.x)
        {
            SetFaceDir(-1);
        }
        
    }
    
    public virtual void SetFaceDir(int dir)
    {
        facedir = dir;
        //transform.localScale = new Vector3(facedir, 1, 1);
    }


    protected virtual void CheckBog(BattleCondition condition)
    {
        
    }
    
    #region 攻击返回指令

    public virtual void OnStandardAttackConnect()
    {
    }

    public virtual void OnSkillConnect()
    {
    }

    public virtual void OnForceConnect()
    {
    }

    public virtual void OnOtherAttackConnect()
    {
    }

    public virtual void OnStandardAttackConnect(AttackBase attack_statusManager)
    {
    }

    public virtual void OnSkillConnect(AttackBase attack_statusManager)
    {
    }

    public virtual void OnForceConnect(AttackBase attack_statusManager)
    {
    }

    public virtual void OnOtherAttackConnect(AttackBase attack_statusManager)
    {
    }
    
    
    public virtual IEnumerator HorizontalMoveFixedTime(float targetPosition, float time, string move, Ease ease = Ease.Linear)
    {
        var tweener = rigid.DOMoveX(targetPosition, time);
        tweener.SetEase(ease);
        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(move) == false)
            {
                //如果tweener还在运行，就停止它
                if (tweener.IsPlaying())
                    tweener.Kill();
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }


    #endregion

    


}
