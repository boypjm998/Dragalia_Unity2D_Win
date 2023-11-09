using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_DB15 : EnemyMoveManager
{
    
    /// <summary>
    /// Chasing Crystal
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action01()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB15_Action01");
        
        yield return new WaitForSeconds(1f);

        anim.Play("charge_1");
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,2f);

        
        var hint = GenerateWarningPrefab("action01_1", _behavior.targetPlayer.RaycastedPosition() + Vector2.up*1.5f,
            Quaternion.identity, RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBarRect2D>();
        
        yield return new WaitForSeconds(hint.warningTime);
        
        anim.Play("charge_3");
        
        var pos = hint.transform.position;
        var container = SetFireCrystal(hint.transform.position);
        
        DOVirtual.DelayedCall(8f, () =>
        {
            SetFireCrystalFireFX(pos);
            GenerateWarningPrefab("action01_2", pos,
                Quaternion.identity, RangedAttackFXLayer.transform);
        },false);
        
        DOVirtual.DelayedCall(10f, () =>
        {
            FireCrystalExplode(pos,container);
        },false);
        
        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
        && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();


    }

    /// <summary>
    /// Fixed Crystal
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action02()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        
        anim.Play("charge_1");

        var raycastedCol = gameObject.RaycastedPlatform();
        float leftBorder, rightBorder, height;

        if (raycastedCol == null)
        {
            height = transform.position.y;
            leftBorder = transform.position.x - 3;
            rightBorder = transform.position.x + 3;
        }
        else
        {
            height = raycastedCol.bounds.max.y;
            leftBorder = raycastedCol.bounds.min.x;
            rightBorder = raycastedCol.bounds.max.x;
        }

        var leftCrystalPosition = new Vector2
            (Mathf.Max(transform.position.x - 3,leftBorder), height);
        var rightCrystalPosition = new Vector2
            (Mathf.Min(transform.position.x + 3,rightBorder), height);
        

        var hint = GenerateWarningPrefab("action01_1", leftCrystalPosition + Vector2.up*1.5f,
            Quaternion.identity, RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBarRect2D>();
        var hint2GO = GenerateWarningPrefab("action01_1", rightCrystalPosition + Vector2.up * 1.5f,
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        
        
        yield return new WaitForSeconds(hint.warningTime);
        
        
        
        anim.Play("charge_3");
        var pos1 = hint.transform.position;
        var pos2 = hint2GO.transform.position;
        var container1 = SetFireCrystal(hint.transform.position);
        var container2 = SetFireCrystal(hint2GO.transform.position);
        
        DOVirtual.DelayedCall(8f, () =>
        {
            SetFireCrystalFireFX(pos1);
            SetFireCrystalFireFX(pos2);
            GenerateWarningPrefab("action01_2", pos1,
                Quaternion.identity, RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action01_2", pos2,
                Quaternion.identity, RangedAttackFXLayer.transform);
        },false);
        
        DOVirtual.DelayedCall(10f, () =>
        {
            FireCrystalExplode(pos1,container1);
            FireCrystalExplode(pos2,container2);
        },false);
        
        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();
        
        
    }

    /// <summary>
    /// Nihil
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action03(float nihilTime = 5f)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB15_Action03");
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_1");

        yield return new WaitForSeconds(1.2f);
        
        anim.Play("charge_3");
        NihilAOE(nihilTime);

        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();

    }

    /// <summary>
    /// Punch Attack
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action04()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + 5*Vector3.up);
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("combo1");

        ComboPunch();
        
        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.45f);
        
        anim.Play("combo2");

        yield return new WaitForSeconds(0.6f);
        var pos = BattleStageManager.Instance.OutOfRangeCheck(new Vector2(transform.position.x + 4 * (ac.facedir),
            transform.position.y));
        _tweener = transform.DOMoveX(pos.x, 0.5f).SetEase(Ease.OutSine);
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.45f);
        
        anim.Play("combo3");
        
        yield return new WaitForSeconds(0.6f);
        pos = BattleStageManager.Instance.OutOfRangeCheck(new Vector2(transform.position.x + 5 * (ac.facedir),
            transform.position.y));
        _tweener = transform.DOMoveX(pos.x, 0.8f).SetEase(Ease.OutSine);
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();
    }

    /// <summary>
    /// Buff Defense
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action05(int buffEffect = 20)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_1");

        yield return new WaitForSeconds(1.2f);
        
        anim.Play("charge_3");

        if (_statusManager.GetConditionTotalValue((int)BasicCalculation.BattleCondition.DefBuff) < 200)
        {
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
                buffEffect, -1);
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
                buffEffect, -1);
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
                buffEffect, 15);
        }


        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();
    }

    public IEnumerator DB15_Action06()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_1");

        yield return new WaitForSeconds(0.5f);

        var fx = Instantiate(GetProjectileOfFormatName("action05_1"),
            transform.position + new Vector3(0, 4f),
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        yield return new WaitForSeconds(1.5f);
        
        anim.Play("charge_3");
        
        BouncingFireball();

        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();
    }







    protected GameObject SetFireCrystal(Vector2 pos)
    {
        var container = Instantiate(attackContainer, RangedAttackFXLayer.transform);
        
        var fireCrystal = InstantiateRanged(GetProjectileOfFormatName("action01_1"),
            pos,container,1);
        
        return container;
    }

    protected void SetFireCrystalFireFX(Vector2 pos)
    {
        Instantiate(GetProjectileOfFormatName("action01_2"),
            pos,Quaternion.identity,
            RangedAttackFXLayer.transform);
    }
    
    protected void FireCrystalExplode(Vector2 pos, GameObject container)
    {
        InstantiateRanged(GetProjectileOfFormatName("action01_3"),
            pos,container,1);
    }

    protected void NihilAOE(float time)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action03"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,
                transform.position.y,ac.ModelDepth),InitContainer(false),1);

        var nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, time, 1);
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(nihilDebuff,100);
        
        
    }

    protected void ComboPunch()
    {
        var proj = InstantiateMeele(GetProjectileOfFormatName("action04"),
            transform.position,InitContainer(true));
    }

    protected void BouncingFireball()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action05_2"),
            transform.position + new Vector3(0, 4), InitContainer(false), 1);
        
        proj.GetComponentInChildren<NormalProjectile>().SetFiredir(ac.facedir);
        
    }



}
