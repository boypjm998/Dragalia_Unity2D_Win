using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class Projectile_DB013_2 : Projectile_DB013_EnlightmentOrb
{
    [SerializeField] private float warningTime;
    [SerializeField] private int attackTimes;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < attackTimes; i++)
        {
            Hint();

            yield return new WaitForSeconds(warningTime - 1);
            
            Attack();
            
            yield return new WaitForSeconds(1);
        }
        
    }

    protected void Hint()
    {
        var hint = EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(GetComponent<ActorBase>(),
            transform.position, transform, 6, 
            Vector2.zero, true, true, warningTime - 1, 0.05f, 
            0.5f, true, true);
    }

    protected override void Attack()
    {
        
        this.InstantiateRangedObject(attackPrefab1,
            transform.position,
            ActorExtensions.InstantiateContainer(BattleStageManager.Instance.RangedAttackFXLayer.transform)
                .gameObject,
            1, 1, enemySource);
        
        
        
    }
}
