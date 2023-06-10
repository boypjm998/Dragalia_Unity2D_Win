using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class EnemyAttackTriggerController : MonoBehaviour
{
    [SerializeField] private float[] attackAwakeTime;
    [SerializeField] private float[] attackSleepTime;

    [SerializeField] private float[] nextAttackTime;
    [SerializeField] private float[] nextConditionTime;

    [SerializeField] private List<AttackProperty> changePropertyTime = new();
    
    [SerializeField] private Collider2D targetCollider;
    [SerializeField] private float destroyTime = 1;

    [SerializeField] private bool sleepAfterAnimStopped;
    private Animation anim;

    private AttackFromEnemy _attackFromEnemy;
    
    [Serializable]
    public class AttackProperty
    {
        public float time;
        public AttackFromEnemy.AvoidableProperty AvoidablePropertyType;
        public BasicCalculation.AttackType AttackType = BasicCalculation.AttackType.NONE;
    }


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
        
        if (changePropertyTime.Count > 0)
        {
            foreach (var prop in changePropertyTime)
            {
                Invoke("RenewAttack",prop.time);
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
        if (_attackFromEnemy.forcedShake)
        {
            CineMachineOperator.Instance.CamaraShake(_attackFromEnemy.hitShakeIntensity,0.1f);
        }
    }
    void AttackSleep()
    {
        targetCollider.enabled = false;
    }

    void NextCondition()
    {
        _attackFromEnemy.ResetWithConditionFlags();
    }

    void RenewAttack()
    {
        if (changePropertyTime.Count > 0)
        {
            _attackFromEnemy.ChangeAvoidability(changePropertyTime[0].AvoidablePropertyType);
            
            if(changePropertyTime[0].AttackType != BasicCalculation.AttackType.NONE)
                _attackFromEnemy.attackType = changePropertyTime[0].AttackType;
            
            changePropertyTime.RemoveAt(0);
        }
        
    }

    public void SetNextWithConditionTime(float[] times)
    {
        nextConditionTime = times;
    }

}
