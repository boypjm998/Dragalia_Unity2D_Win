using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class Projectile_GhostAssault_1 : MonoBehaviour
{
    private StatusManager _statusManager;
    private EnemyControllerHumanoid ac;
    [SerializeField] private GameObject attackPrefab;
    private IEnumerator Start()
    {
        _statusManager = GetComponent<StatusManager>();
        ac = GetComponent<EnemyControllerHumanoid>();
        
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position - new Vector3(0,1.5f),
            BattleStageManager.Instance.RangedAttackFXLayer.transform, new Vector2(3, 12), Vector2.zero, false,
            1, 2, 90, 1, true, true);
        
        yield return new WaitForSeconds(2f);
        
        ac.anim.Play("float_entire");
        
        yield return new WaitUntil(()=>
            ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);
        

        var proj = this.InstantiateRangedObject(attackPrefab,
            transform.position, BattleStageManager.Instance.GetNewRangedContainer(true),ac.facedir);
        proj.GetComponent<AttackFromEnemy>().ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
        
        yield return new WaitUntil(()=>
            ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        ac.anim.Play("idle");

        yield return new WaitForSeconds(1);
        
        Destroy(gameObject);

    }
    
    
    
    
    
}
