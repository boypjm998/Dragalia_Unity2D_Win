using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class Projectile_DB013_1 : Projectile_DB013_EnlightmentOrb
{
    
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        target = DragaliaEnemyBehavior.GetPlayerList()[0];
        
        yield return new WaitForSeconds(1);

        Attack();
        
        yield return new WaitForSeconds(5);
        
        Attack();
        
        yield return new WaitForSeconds(5);
        
        Attack();
        
        yield return new WaitForSeconds(5);
        
        Destroy(gameObject,0.1f);
    }

    protected override void Attack()
    {

        
        //计算出target和自身的角度
        var angle = Vector2.Angle(target.transform.position-transform.position,Vector2.right);

        if (transform.position.y > target.transform.position.y)
        {
            angle *= -1;
        }

        var hintBar = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(GetComponent<EnemyController>(), transform.position,
            BattleStageManager.Instance.RangedAttackFXLayer.transform, new Vector2(30, 1), Vector2.zero,
            true, 1,
            1.5f, angle, 1f, true, true);
        

        _tween = DOVirtual.DelayedCall(1.5f, () =>
        {
            
            
            this.InstantiateDirectionalRangedObject(attackPrefab1,
                transform.position,
                ActorExtensions.InstantiateContainer(BattleStageManager.Instance.RangedAttackFXLayer.transform)
                    .gameObject,
                1, angle, enemySource);
        }, false).OnKill(() =>
        {
            if(hintBar!=null)
                Destroy(hintBar);
        });


    }
}
