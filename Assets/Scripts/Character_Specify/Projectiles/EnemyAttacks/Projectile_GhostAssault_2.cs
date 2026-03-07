using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class Projectile_GhostAssault_2 : MonoBehaviour
{
    private StatusManager _statusManager;
    private EnemyControllerHumanoid ac;
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private GameObject muzzlePrefab;

    public bool fixedPosition;
    public float fixedPositionValue;
    public Vector2 attackRange;
    public float fillTime = 2;
    
    private IEnumerator Start()
    {
        _statusManager = GetComponent<StatusManager>();
        ac = GetComponent<EnemyControllerHumanoid>();
        attackRange = attackPrefab.GetComponent<BoxCollider2D>().size;
        
        ac.anim.Play("action06");

        var position = fixedPosition
            ? new Vector2(fixedPositionValue, gameObject.RaycastedPosition().y)
            : BattleStageManager.Instance.GetPlayer().RaycastedPosition();
        
        
        yield return null;
        yield return new WaitUntil(()=>
            ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            position,
            BattleStageManager.Instance.RangedAttackFXLayer.transform, 
            new Vector2(attackRange.y,attackRange.x), Vector2.zero, false,
            1, fillTime, 90, 1, true, true);
        
        yield return new WaitForSeconds(fillTime - 0.3f);

        ac.anim.Play("action08");
        
        Instantiate(muzzlePrefab, transform.position + new Vector3(0,2), Quaternion.identity,transform);
        
        yield return new WaitForSeconds(0.5f);

        this.InstantiateRangedObject(attackPrefab,position
            ,
            BattleStageManager.Instance.GetNewRangedContainer(true),1);
        
        yield return new WaitUntil(()=>
            ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        ac.anim.Play("idle");

        yield return new WaitForSeconds(1);
        
        Destroy(gameObject);

    }
    
    
    
    
}
