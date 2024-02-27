using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class ControlDisableEffector : MonoBehaviour
{
    private ActorBase ac;
    private AttackBase atk;
    private StatusManager stat;
    private bool isActive = false;
    [SerializeField] private bool fixedOnGround = true;
    [SerializeField] private bool triggeredByAttack = false;
    [SerializeField] private float effectTime = 5f;
    [SerializeField] private float triggerTime = 0.1f;
    
    private AttackBase.AttackBaseDelegate atkDelegate = null; 
    private StatusManager.StatusManagerVoidDelegate statDelegate = null;
    
    

    private Tween effectTween;
    
    public void SetTarget(GameObject target)
    {
        ac = target.GetComponent<ActorBase>();
        stat = ac.GetComponent<StatusManager>();
    }

    private void OnDestroy()
    {
        CancelInvoke();
        if (ac != null)
        {
            stat.OnReviveOrDeath -= statDelegate;
            effectTween?.Kill();
        }
        if(atk != null)
            atk.OnAttackHit -= atkDelegate;
    }

    private void Awake()
    {
        atkDelegate = (attack, target) =>
        {
            SetTarget(target);
            SetActionUnable();
            atk.OnAttackHit -= atkDelegate;
        };
        statDelegate = () =>
        {
            effectTween?.Kill();
            stat.OnReviveOrDeath -= statDelegate;
        };
    }

    private void Start()
    {
        if (triggeredByAttack)
        {
            atk = GetComponent<AttackBase>();
            //AttackBase.AttackBaseDelegate atkDelegate = null;
            
            atk.OnAttackHit += atkDelegate;
        }
        else if (ac == null)
        {
            return;
        }
        
        

        Invoke("SetActionUnable",triggerTime);


    }

    private void Update()
    {
        if (isActive)
        {
            ac.SetActionUnable(true);
            ac.SetGravityScale(0);
            ac.rigid.velocity = Vector2.zero;
        }
    }

    private void SetActionUnable()
    {
        if ((ac as ActorController).dc)
        {
            (ac as ActorController).dc.DModeForcePurge();
        }
        ac.SetActionUnable(true);
        ac.SetGravityScale(0);
        stat.knockbackRes = 999;
        isActive = true;
        

        if (fixedOnGround)
        {
            transform.position = ac.gameObject.RaycastedPosition();
        }

        ac.transform.position = transform.position;


        effectTween = DOVirtual.DelayedCall(effectTime, () =>
        {
            if (stat.HasControlAffliction() == false)
            {
                ac.SetActionUnable(false);
            }
            isActive = false;
            ac.ResetGravityScale();
            stat.ResetKBRes();
        },false).OnKill(() =>
        {
            if (stat.HasControlAffliction() == false)
            {
                ac.SetActionUnable(false);
            }
            isActive = false;
            ac.ResetGravityScale();
            stat.ResetKBRes();
        });
        
        //StatusManager.StatusManagerVoidDelegate statDelegate = null;
        
        
        
        stat.OnReviveOrDeath += statDelegate;
    }







}
