using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_GoblinMeele : EnemyMoveManager
{
    /// <summary>
    /// Slash
    /// </summary>
    /// <returns></returns>
    public IEnumerator E9003_Action01()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(_statusManager.knockbackRes);
        anim.Play("action01");
        ac.TurnMove(_behavior.targetPlayer);

        yield return null;
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f);

        InstantiateMeele(GetProjectileOfFormatName("action01"),
            transform.position, InitContainer(true));
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
        
        
    }
    
    /// <summary>
    /// Smash * 3
    /// </summary>
    /// <returns></returns>
    public IEnumerator E9003_Action02()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(80);
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position +
                                                                 new Vector3(0,2f,0));

        yield return new WaitForSeconds(1);

        anim.Play("action02");
        ac.TurnMove(_behavior.targetPlayer);

        yield return null;
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("action03");
        
        //0.1-0.3移动，0.2发出第一次攻击，0.55-0.8移动第二次。
        
        yield return new WaitForSeconds(0.1f);
        JumpMove(2, 0.2f);
        yield return new WaitForSeconds(0.1f);
        InstantiateMeele(GetProjectileOfFormatName("action02_1"),transform.position, InitContainer(true));
        yield return new WaitForSeconds(0.35f);
        JumpMove(2.5f, 0.25f);
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        
        anim.Play("action04");
        yield return null;
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f);
        InstantiateMeele(GetProjectileOfFormatName("action02_2"),
            transform.position, InitContainer(true));
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
        
        
    }
    
    /// <summary>
    /// Meaningless Immidiate
    /// </summary>
    /// <returns></returns>
    public IEnumerator E9003_Action03()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(_statusManager.knockbackRes);
        
        anim.Play("action05");
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();

    }

    
    
    private void JumpMove(float distance, float duration)
    {
        var currentPlatform = gameObject.RaycastedPlatform();

        var endPos = Mathf.Clamp(transform.position.x + distance * ac.facedir,
            currentPlatform.bounds.min.x,
            currentPlatform.bounds.max.x);
        
        
        _tweener = transform.DOMoveX(endPos, duration).SetEase(Ease.InOutSine);
        
    }



}
