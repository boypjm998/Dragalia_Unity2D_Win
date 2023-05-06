using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyMoveController_E2001_M : EnemyMoveManager
{
    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerFlying>();
    }
    
    /// <summary>
    /// Blast around
    /// </summary>
    /// <returns></returns>
    public IEnumerator E20011_Action01()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter();
        ac.SetGravityScale(0);
        var hint = GenerateWarningPrefab(WarningPrefabs[0],transform.position,Quaternion.identity, RangedAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        
        Vector2 direction = (_behavior.targetPlayer.transform.position+new Vector3(0,.5f,0) - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        hintbar.transform.eulerAngles = new Vector3(0, 0, angle);
        
        yield return new WaitForSeconds(hintbar.warningTime);
        Action01_DashAttack(direction);
        yield return new WaitForSeconds(1f);
        Destroy(hint);
        yield return new WaitForSeconds(1f);
        ac.SetGravityScale(1);
        QuitAttack();
    }
    
    public IEnumerator E20011_Action02()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter();
        var hint = GenerateWarningPrefab(WarningPrefabs[1],transform.position,Quaternion.identity, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        yield return new WaitForSeconds(hintbar.warningTime);
        Action02_AroundAttack();
        Destroy(hint);
        yield return new WaitForSeconds(1f);
        QuitAttack();
    }

    void Action01_DashAttack(Vector2 direction)
    {
        //用DOTween沿着direction移动10的距离
        var moveTarget = transform.position + (Vector3)direction * 10;
        moveTarget = BattleStageManager.Instance.OutOfRangeCheck(moveTarget);
        
        DOTween.To(() => transform.position, x => transform.position = x, moveTarget, 1f);
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        var proj = InstantiateRanged(projectile1, transform.position, container,1);
        proj.transform.right = direction;
    }

    void Action02_AroundAttack()
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity, MeeleAttackFXLayer.transform);
        InstantiateMeele(projectile2, transform.position, container);
        
    }

}
