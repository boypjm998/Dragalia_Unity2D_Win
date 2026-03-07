using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_DB16 : EnemyMoveManager
{
    /*
     * Action01: WarpClaw
     * Action02: ClawAttack
     * Action03: FrontStrike
     * Action04: Devastation
     * Action05: Dash
     */

    [SerializeField] private GameObject rushAttack;
    
    /// <summary>
    /// WarpClaw
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB16_Action01()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        // BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer, 2f);
        //
        // yield return new WaitForSeconds(1);

        var flashFX = GetProjectileOfFormatName("action01");
        
        Instantiate(flashFX, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.1f);
        
        DisappearRenderer();

        var targetPlatform = _behavior.targetPlayer.RaycastedPlatform();
        var targetPos = new Vector2(_behavior.targetPlayer.transform.position.x,
            targetPlatform.bounds.max.y + ac.GetActorHeight());
        targetPos.x = Mathf.Clamp(targetPos.x - ac.facedir*4,
            targetPlatform.bounds.min.x, targetPlatform.bounds.max.x);
        
        transform.position = targetPos;
        
        Instantiate(flashFX, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.1f);
        
        AppearRenderer();
        
        ac.TurnMove(_behavior.targetPlayer);

        anim.Play("claw_enter");
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position+new Vector3(0,4),
            true);

        yield return new WaitForSeconds(Random.Range(0.7f,0.95f));

        anim.Play("claw_exit");

        yield return new WaitForSeconds(0.15f);
        
        ClawSlash();

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    
    /// <summary>
    /// Claw
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB16_Action02()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        anim.Play("claw_enter");
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position+new Vector3(0,3),
            true);

        yield return new WaitForSeconds(0.8f);

        anim.Play("claw_exit");

        yield return new WaitForSeconds(0.15f);
        
        ClawSlash();

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }


    /// <summary>
    /// FrontStrike
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB16_Action03()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        if (_behavior.targetPlayer.RaycastedPlatform() != gameObject.RaycastedPlatform() ||
            Mathf.Abs(_behavior.targetPlayer.transform.position.x - transform.position.x) > 10)
        {
            var flashFX = GetProjectileOfFormatName("action01");
        
            Instantiate(flashFX, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(0.1f);
        
            DisappearRenderer();

            var targetPlatform = _behavior.targetPlayer.RaycastedPlatform();
            var targetPos = new Vector2(_behavior.targetPlayer.transform.position.x,
                targetPlatform.bounds.max.y + ac.GetActorHeight());
            targetPos.x = Mathf.Clamp(targetPos.x - ac.facedir*4,
                targetPlatform.bounds.min.x, targetPlatform.bounds.max.x);
        
            transform.position = targetPos;
        
            Instantiate(flashFX, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(0.1f);
        
            AppearRenderer();
        
            ac.TurnMove(_behavior.targetPlayer);
        }

        anim.Play("front_enter");
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position+new Vector3(0,3),
            true);

        yield return new WaitForSeconds(0.6f);

        anim.Play("front_exit");

        yield return new WaitForSeconds(0.6f);
        
        FrontStrike();

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    
    public IEnumerator DB16_Action05()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        anim.Play("dash_enter");
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position+new Vector3(0,3),
            true);

        yield return new WaitForSeconds(1f);

        anim.Play("dash_exit");

        yield return new WaitForSeconds(0.05f);
        
        Dash();

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    private void ClawSlash()
    {
        var slash = GetProjectileOfFormatName("action02");
        var slashPos = new Vector2(transform.position.x + ac.facedir * 7, transform.position.y + 1);

        var proj = InstantiateRanged(slash, slashPos, InitContainer(false),
            ac.facedir,0);
        
        var atk = proj.GetComponent<AttackFromEnemy>();

        AttackBase.AttackBaseDelegate handler = null;
        
        handler = (atk, tar) =>
        {
            atk.OnAttackHit -= handler;
            if(GlobalController.currentGameState == GlobalController.GameState.Inbattle)
                BattleStageManager.Instance.TimeScaleEffect(0.1f,0.2f);
        };
        
        atk.OnAttackHit += handler;

    }

    private void FrontStrike()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action03"),
            transform.position + new Vector3(ac.facedir * 7, 0),InitContainer(false),
            1);
    }

    private void Dash()
    {
        var endPosX = transform.position.x + ac.facedir * 16;
        endPosX = Mathf.Clamp(endPosX, BattleStageManager.Instance.mapBorderL, 
            BattleStageManager.Instance.mapBorderR);
        var timePercent = Mathf.Abs(endPosX - transform.position.x) / 16;

        _tweener = transform.DOMoveX(endPosX, timePercent * 0.7f).
            SetEase(Ease.OutSine).SetUpdate(UpdateType.Fixed).
            OnKill(() =>
            {
                rushAttack.SetActive(false);
            }).
            OnComplete(() =>
            {
                rushAttack.SetActive(false);
            });
        
        rushAttack.SetActive(true);
        rushAttack.GetComponent<AttackFromEnemy>().NextAttack();


    }
    
    
}
