using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ContributoryAttackContainer : AttackContainerEnemy, IEnemySealedContainer
{
    [SerializeField] private Collider2D triggerZone;
    
    private ForcedAttackFromEnemy enemyAttack;
    private Action<StatusManager> OnEnemyEnterAttack;
    private bool isTriggeredForEnemy=false;
    private int characterNum = 0;
    private List<float> _enemyDmgModifier;
    private List<AttackInfo> _enemyAttackInfo;
    [SerializeField] private float triggerTime = 0.1f;
    private List<Collider2D> _collider2Ds = new();
    private void Awake()
    {
        enemyAttack = GetComponentInChildren<ForcedAttackFromEnemy>();
        //playerAttack = GetComponentInChildren<AttackFromPlayer>();
    }

    public void SetEnemySource(GameObject src)
    {
        enemyAttack.enemySource = src;
        _enemyAttackInfo = new List<AttackInfo>();

        for (int i = 0; i < enemyAttack.attackInfo.Count; i++)
        {
            _enemyAttackInfo.Add(new AttackInfo(enemyAttack.attackInfo[i].dmgModifier));
        }

        DOVirtual.DelayedCall(triggerTime, () => Trigger(), false);

    }

    

    public void SetAction(Action<StatusManager> func)
    {
        OnEnemyEnterAttack = func;
    }

    private void OnDestroy()
    {
        OnEnemyEnterAttack = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            _collider2Ds.Add(other);
        }
        
        
        
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            _collider2Ds.Remove(other);
        }
    }

    private void Trigger()
    {
        characterNum = _collider2Ds.Count;
        
        
        
        
        for (int k = 0; k < enemyAttack.attackInfo.Count;k++)
        {
                
            //enemyAttack.attackInfo[k].dmgModifier = _enemyAttackInfo[k].dmgModifier;
                
            for (int i = 0; i < _enemyAttackInfo[k].dmgModifier.Count; i++)
            {
                enemyAttack.attackInfo[k].dmgModifier[i] = 
                    _enemyAttackInfo[k].dmgModifier[i] /
                    (characterNum > 0?characterNum:1);
            }
                
        }

        foreach (var other in _collider2Ds)
        {
            if (other.CompareTag("Player"))
            {
                enemyAttack.target = other.GetComponentInParent<PlayerStatusManager>().gameObject;
                //enemyAttack.DealDamageImmediately();
                
                print($"Characters:{characterNum}");
            }
            else if (other.CompareTag("Enemy"))
            {
                //characterNum += 1;
            
                if(isTriggeredForEnemy)
                    return;
                isTriggeredForEnemy = true;
                OnEnemyEnterAttack?.Invoke(enemyAttack.enemySource.GetComponentInParent<StatusManager>());
            }
        }


        enemyAttack.enabled = true;


    }
    
    
}
