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
    
    public bool IsSleeping { get; private set; }
    public float DestroyTime
    {
        get => destroyTime;
        set => destroyTime = value;
    }

    [SerializeField] private bool sleepAfterAnimStopped;
    private Animation anim;

    private AttackFromEnemy _attackFromEnemy;
    
    [Serializable]
    public class AttackProperty
    {
        public float time;
        public AttackFromEnemy.AvoidableProperty AvoidablePropertyType;
        public BasicCalculation.AttackType AttackType = BasicCalculation.AttackType.STANDARD;
    }


    private void Awake()
    {
        

        _attackFromEnemy = GetComponent<AttackFromEnemy>();
    }



    public void Restart()
    {
        NextAttack();
        InitInvokes();
    }


    private void Start()
    {
        if (targetCollider == null)
        {
            targetCollider = GetComponent<Collider2D>();
        }
        InitInvokes();
    }

    private void InitInvokes()
    {
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
        IsSleeping = false;
    }
    void AttackSleep()
    {
        targetCollider.enabled = false;
        IsSleeping = true;
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
            
            
            _attackFromEnemy.attackType = changePropertyTime[0].AttackType;
            
            changePropertyTime.RemoveAt(0);
        }
        
    }

    public void SetNextWithConditionTime(float[] times)
    {
        nextConditionTime = times;
    }
    
    public void SetAwakeTimes(List<float> times)
    {
        attackAwakeTime = times.ToArray();
    }

    public void SetAwakeTime(int index, float time)
    {
        attackAwakeTime[index] = time;
    }

    public void SetSleepTimes(List<float> times)
    {
        attackSleepTime = times.ToArray();
    }

}
