﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public abstract class ActorBase : MonoBehaviour
{
    public delegate void OnHurt();


    public Animator anim;
    public Rigidbody2D rigid;
    public OnHurt OnAttackInterrupt;   
    public int facedir = 1;
    public static float DefaultGravity = 4;


    public abstract void TakeDamage(float kbPower, float kbtime, float kbForce, Vector2 kbDir);

    public virtual void DisappearRenderer()
    {
    }
    
    public virtual void AppearRenderer()
    {
    }

    public float GetActorHeight()
    {
        return transform.position.y - transform.Find("GroundSensor").GetComponent<Collider2D>().bounds.min.y;
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
        var tweener = transform.DOMoveX(targetPosition, time);
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