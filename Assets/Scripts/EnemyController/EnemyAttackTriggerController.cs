using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTriggerController : MonoBehaviour
{
    [SerializeField] private float[] attackAwakeTime;
    [SerializeField] private float[] attackSleepTime;

    [SerializeField] private float[] nextAttackTime;
    [SerializeField] private float[] nextConditionTime;

    [SerializeField] private Collider2D targetCollider;
    [SerializeField] private float destroyTime = 1;

    [SerializeField] private bool sleepAfterAnimStopped;
    private Animation anim;

    private AttackFromEnemy _attackFromEnemy;


    private void Awake()
    {
        
        
        if (targetCollider == null)
        {
            targetCollider = GetComponent<Collider2D>();
        }

        _attackFromEnemy = GetComponent<AttackFromEnemy>();
        
        
        if (nextAttackTime.Length > 0)
        {
            foreach (var time in nextAttackTime)
            {
                Invoke("NextAttack",time);
            }
        }

        if (attackAwakeTime.Length > 0)
        {
            foreach (var time in attackAwakeTime)
            {
                Invoke("AttackAwake",time);
            }
        }

        if (attackSleepTime.Length > 0)
        {
            foreach (var time in attackSleepTime)
            {
                Invoke("AttackSleep",time);
            }
        }
        
        if (nextConditionTime.Length > 0)
        {
            foreach (var time in nextConditionTime)
            {
                Invoke("NextCondition",time);
            }
        }
        
        if(destroyTime > 0)
            Destroy(gameObject,destroyTime);

    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    void NextAttack()
    {
        _attackFromEnemy.NextAttack();
    }
    
    void AttackAwake()
    {
        targetCollider.enabled = true;
    }
    void AttackSleep()
    {
        targetCollider.enabled = false;
    }

    void NextCondition()
    {
        _attackFromEnemy.ResetWithConditionFlags();
    }

    public void SetNextWithConditionTime(float[] times)
    {
        nextConditionTime = times;
    }

}
