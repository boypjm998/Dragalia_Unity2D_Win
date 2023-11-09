using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_DB14 : EnemyMoveManager
{

    /// <summary>
    /// Dash
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB14_Action01()
    {
        yield return _canAction;
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        
        //anim.Play("charge_enter");
        
        var hint = GenerateWarningPrefab("action01", transform.position + new Vector3(ac.facedir,1,0),
            ac.facedir == 1?Quaternion.identity : Quaternion.Euler(0,180,0),
            RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBarRect2D>();

        yield return new WaitForSeconds(hint.warningTime - 0.25f);
        
        anim.Play("action02");

        yield return new WaitForSeconds(0.35f);

        DashAttack();

        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// Water Jet
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB14_Action02()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        //anim.Play("charge_enter");
        
        var hint = GenerateWarningPrefab("action02", transform.position + Vector3.up * 2,
            Quaternion.identity, MeeleAttackFXLayer.transform).GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(2f);
        
        anim.Play("combo2");

        yield return new WaitForSeconds(0.8f);
        
        LaunchWaterJet();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();

    }

    /// <summary>
    /// 侵蚀
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB14_Action03(int effect)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB14_Action03");
        
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("charge_enter");

        yield return new WaitForSeconds(1f);
        
        CorrosionFog(effect);
        
        anim.Play("charge_exit");

        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }




    /// <summary>
    /// Double Slap
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB14_Action04()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        //anim.Play("charge_enter");
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + Vector3.up * 6);

        yield return new WaitForSeconds(1f);
        
        anim.Play("action01");

        yield return new WaitForSeconds(0.2f);
        
        DoubleSlap(1);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("action02");
        
        yield return new WaitForSeconds(0.2f);
        
        DoubleSlap(2);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();

    }
    
    /// <summary>
    /// Buff
    /// </summary>
    /// <param name="buffEffect"></param>
    /// <returns></returns>
    public IEnumerator DB14_Action05(int buffEffect = 25)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_enter");

        yield return new WaitForSeconds(0.8f);
        
        anim.Play("charge_exit");
        
        
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            buffEffect, 15);
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
            buffEffect, 15);
        
        

        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();
    }


    protected void DashAttack()
    {
        var pos = BattleStageManager.Instance.
            OutOfRangeCheck(transform.position + new Vector3(15*ac.facedir,0,0));
        _tweener = transform.DOMoveX(pos.x, 0.4f).SetEase(Ease.InOutSine);
        
        var attackGo = InstantiateRanged(GetProjectileOfFormatName("action01_2"),
            transform.position + new Vector3(ac.facedir, 1, 0), InitContainer(false),
            ac.facedir);
        
        var fx = Instantiate(GetProjectileOfFormatName("action01_1"),
            transform.position + new Vector3(0, -1, 0), Quaternion.identity,
            MeeleAttackFXLayer.transform);
        
    }

    protected void LaunchWaterJet()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action02"),
            transform.position + new Vector3(ac.facedir, 2, 0), InitContainer(false),
            ac.facedir);
        
        var projScript = proj.GetComponentInChildren<BouncingProjectile>();
        projScript.SetVelocity(new Vector2(projScript.HorizontalVelocity * ac.facedir, 0));
    }

    protected void CorrosionFog(float healNeeded)
    {
        var fx = InstantiateRanged(GetProjectileOfFormatName("action03"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,transform.position.y-1),
            InitContainer(false),1);

        var corrosionEff = new AdvancedTimerBuff((int)BasicCalculation.BattleCondition.Corrosion,
            healNeeded, 8, 0, -1, 1, -1,8);
        
        fx.GetComponent<AttackFromEnemy>().AddWithConditionAll(corrosionEff,200);



        //_statusManager.InflictCorrosion(healNeeded,1);
    }


    protected void DoubleSlap(int slapID)
    {
        string prefabName = "action04_" + slapID;
        var fx = InstantiateMeele(GetProjectileOfFormatName(prefabName),
            transform.position, InitContainer(true));
        _tweener = transform.DOMoveX(transform.position.x + ac.facedir,0.2f).SetEase(Ease.InOutSine);
    }



}
