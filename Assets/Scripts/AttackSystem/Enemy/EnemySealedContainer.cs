using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AttackContainerEnemy))]
public class EnemySealedContainer : MonoBehaviour, IEnemySealedContainer
{
    [SerializeField] protected GameObject enemySource;
    [SerializeField] protected List<GameObject> attacks = new();
    protected GameObject target;
    [SerializeField] protected GameObject hint;
    [SerializeField] protected float attackAwakeTime = 0;
    [SerializeField] protected List<float> nextAttackTime = new();
    protected int firedir;

    protected IEnumerator Start()
    {
        yield return new WaitForSeconds(attackAwakeTime);
        
        if (hint != null)
        {
            var bar = hint.GetComponent<EnemyAttackHintBar>();
            yield return new WaitForSeconds(bar.warningTime);
            
        }

        

        if (nextAttackTime.Count == 0)
        {
            ReleaseAttack();yield break;
        }
        
        ReleaseAttack(0);
        
        //if(nextAttackTime.Count == 1) yield break;

        yield return new WaitForSeconds(nextAttackTime[0] - attackAwakeTime);

        for(int i = 1; i < attacks.Count ; i++)
        {
            ReleaseAttack(i);
            if(i == attacks.Count - 1) break;
            yield return new WaitForSeconds(nextAttackTime[i]-nextAttackTime[i-1]);
        }

    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    public void SetEnemySource(GameObject source)
    {
        enemySource = source;
    }
    
    public void SetFireDir(int dir)
    {
        firedir = dir;
    }
    
    public void SetTarget(GameObject target = null)
    {
        if (target == null)
        {
            this.target = enemySource.GetComponent<DragaliaEnemyBehavior>().targetPlayer;
        }
        else
        {
            this.target = target;
        }

    }

    public void ReleaseAttack(int id = -1)
    {
        if (id == -1)
        {
            foreach (var attack in attacks)
            {
                
                attack.SetActive(true);
                var atkfromenemy = attack.GetComponent<AttackFromEnemy>();
                if(atkfromenemy == null) continue;
                atkfromenemy.enemySource = enemySource;
                atkfromenemy.firedir = firedir;
                if (atkfromenemy is ForcedAttackFromEnemy)
                {
                    (atkfromenemy as ForcedAttackFromEnemy).target = target;
                }
            }
        }
        else
        {
            attacks[id].SetActive(true);
            var atkfromenemy = attacks[id].GetComponent<AttackFromEnemy>();
            if (atkfromenemy == null)
            {
                return;
            }
            atkfromenemy.enemySource = enemySource;
            atkfromenemy.firedir = firedir;
            if (atkfromenemy is ForcedAttackFromEnemy)
            {
                (atkfromenemy as ForcedAttackFromEnemy).target = target;
            }
        }
    }

}
