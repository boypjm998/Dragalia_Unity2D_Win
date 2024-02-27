using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_Gobmancer : EnemyMoveManager
{
    public IEnumerator E9004_Action01()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(_statusManager.knockbackRes);
        anim.Play("action01");
        ac.TurnMove(_behavior.targetPlayer);

        yield return null;
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.33f);

        InstantiateDirectionalRanged(GetProjectileOfFormatName("action01"),
            transform.position + new Vector3(ac.facedir,0), InitContainer(false),ac.facedir,0);
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }


    /// <summary>
    /// FS
    /// </summary>
    /// <returns></returns>
    public IEnumerator E9004_Action02()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(90);
        anim.Play("action03");
        ac.TurnMove(_behavior.targetPlayer);

        var hint = GenerateWarningPrefab("action01",
            transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);

        hint.transform.DOMove(_behavior.targetPlayer.transform.position, 0.3f);

        yield return null;

        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("action04");
        
        var time = hint.GetComponent<EnemyAttackHintBar>().warningTimeLeft;
        var position = hint.transform.position;

        print(time+"Seconds left");
        yield return new WaitForSeconds(time);

        anim.Play("action05");

        yield return new WaitForSeconds(0.3f);

        var atk = InstantiateRanged(GetProjectileOfFormatName("action02"),
            position, InitContainer(false),1).GetComponent<AttackFromEnemy>();
        atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Sleep,1,
                Random.Range(3f,5f),1),
            30);

        yield return null;
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
}
