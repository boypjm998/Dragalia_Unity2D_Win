using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_DB11 : EnemyMoveManager
{

    public int minonCount => summonedChildren.Count;
    private List<StatusManager> summonedChildren = new();
    /// <summary>
    /// SummonEnemySolider
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB11_Action01(int hp, int atk, string prefabName)
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("combo2");
        
        ac.SetHitSensor(false);
        yield return new WaitForSeconds(0.5f);
        SummonChild(hp,atk,prefabName);
        yield return new WaitForSeconds(0.8f);
        ac.SetHitSensor(true);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// Buff All allies
    /// </summary>
    /// <param name="hp"></param>
    /// <param name="atk"></param>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    public IEnumerator DB11_Action02(int buffAmount)
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("combo2");
        
        ac.SetHitSensor(false);
        yield return new WaitForSeconds(0.5f);
        
        PowerUpAllMinons(buffAmount);
        
        yield return new WaitForSeconds(0.8f);
        ac.SetHitSensor(true);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }


    /// <summary>
    /// 断罪之风
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB11_Action03(int type = 1)
    {
        yield return _canAction;
        
        bossBanner?.PrintSkillName("DB11_Action03");
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        anim.Play("forcing_enter");

        GameObject hint1 = null;

        
        hint1 = GenerateWarningPrefab("action03_1", transform.position + Vector3.up,
                Quaternion.identity,
                MeeleAttackFXLayer.transform);
        
        yield return new WaitForSeconds(hint1.GetComponent<EnemyAttackHintBarShine>().warningTime + 0.2f);

        CycloneAroundBig();
        
        GameObject hint2 = GenerateWarningPrefab("action03_2", transform.position + Vector3.up,
            Quaternion.identity,
            RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(hint2.GetComponent<EnemyAttackHintBarShine>().warningTime + 0.2f);
        
        CycloneRingBig();
        
        GameObject hint3 = GenerateWarningPrefab("action03_3", transform.position + Vector3.up,
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        GenerateWarningPrefab("action03_4", gameObject.RaycastedPosition() + new Vector2(12,0),
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        GenerateWarningPrefab("action03_4", gameObject.RaycastedPosition() + new Vector2(-12,0),
            Quaternion.identity,
            RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(hint3.GetComponent<EnemyAttackHintBarShine>().warningTime + 0.2f);
        
        CycloneAroundSmallWithPillars();
        anim.Play("forcing_exit");

        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        ac.ResetGravityScale();
        anim.Play("idle");
        QuitAttack();
        
    }

    /// <summary>
    /// Around Attack
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB11_Action04()
    {
        yield return _canAction;

        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter();
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + 
                                                                 (Vector3)(6*Vector2.up));
        yield return new WaitForSeconds(1f);
        
        anim.Play("combo2");

        yield return new WaitForSeconds(0.2f);
        
        AroundAttack();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);

        
        anim.Play("combo3");

        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.35f);

        ac.TurnMove(_behavior.targetPlayer);
        _tweener = transform.DOMoveX(
            transform.position.SafePosition(new Vector2(-ac.facedir * 7.5f, 0)).x, 
            1f);

        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// 邪恶授意
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB11_Action05(float amount)
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB11_Action05");
        yield return new WaitForSeconds(2f);
        
        anim.Play("combo2");
        
        ac.SetHitSensor(false);
        yield return new WaitForSeconds(0.5f);
        
        CorrosionAOE(amount);
        
        yield return new WaitForSeconds(1f);
        ac.SetHitSensor(true);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// 慈爱治愈
    /// </summary>
    /// <param name="maxHP"></param>
    /// <returns></returns>
    public IEnumerator DB11_Action06(int maxHP = 80000)
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB11_Action06");

        if (transform.position.x - 8 < BattleStageManager.Instance.mapBorderL)
        {
            ac.SetFaceDir(1);
        }else if (transform.position.x + 8 > BattleStageManager.Instance.mapBorderR)
        {
            ac.SetFaceDir(-1);
        }
        
        ac.SetHitSensor(false);
        anim.Play("charge_enter");
        
        yield return new WaitForSeconds(1f);

        var fx = GenerateAreaFX();
        var uiController = SpawnEnergyBalls(maxHP);
        var uiTimer = uiController.GetComponent<UI_RingSlider>();
        uiTimer.currentValue = uiTimer.maxValue;

        yield return null;
        

        yield return new WaitUntil(() => uiController.Value <= 0 || uiTimer.currentValue <= 0);

        Destroy(fx);
        if (uiController.Value > 0)
        {
            var leftMinons = uiController.KillAllMinons();
            anim.Play("combo2");
            Destroy(uiController.gameObject);
            yield return new WaitForSeconds(0.8f);
            BurstNihilAOE(leftMinons);
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        }else{
            anim.Play("knockdown_enter");
            Destroy(uiController.gameObject);
            yield return null;
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
            anim.Play("knockdown_exit");
            yield return null;
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        }
        ac.SetHitSensor(true);
        anim.Play("idle");
        QuitAttack();
    }








    protected void SummonChild(int hp, int atk, string prefabName)
    {
        var position1 = (gameObject.RaycastedPosition() + new Vector2(-5, 0)).SafePosition();
        
        var position2 = (gameObject.RaycastedPosition() + new Vector2(5, 0)).SafePosition();
        
        
        var enemy1 = SpawnEnemyMinon(GetProjectileOfName(prefabName),position1,
            hp,atk, ac.facedir);
        var enemy2 = SpawnEnemyMinon(GetProjectileOfName(prefabName),position2,
            hp,atk, ac.facedir);
        var fx1 = Instantiate(GetProjectileOfFormatName("action01_1"),position1,Quaternion.identity);
        var fx2 = Instantiate(GetProjectileOfFormatName("action01_1"),position2,Quaternion.identity);
        
        var stat1 = enemy1.GetComponent<StatusManager>();
        var stat2 = enemy2.GetComponent<StatusManager>();
        
        summonedChildren.Add(stat1);
        summonedChildren.Add(stat2);

        StatusManager.StatusManagerVoidDelegate RemoveChild1 = null;
        StatusManager.StatusManagerVoidDelegate RemoveChild2 = null;
        RemoveChild1 = () =>
        {
            summonedChildren.Remove(stat1);
            stat1.OnReviveOrDeath -= RemoveChild1;
        };
        RemoveChild2 = () =>
        {
            summonedChildren.Remove(stat2);
            stat2.OnReviveOrDeath -= RemoveChild2;
        };
        
        stat1.OnReviveOrDeath += RemoveChild1;
        stat2.OnReviveOrDeath += RemoveChild2;
        
    }

    protected void PowerUpAllMinons(int buffAmount)
    {
        foreach (var status in summonedChildren)
        {
            if (status != null)
            {
                status.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.AtkBuff), buffAmount, -1);
                status.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.DefBuff), buffAmount, -1);
                status.ObtainHealOverTimeBuff(buffAmount,15);
            }
        }
        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.AtkBuff),
            Mathf.Max(minonCount*buffAmount,buffAmount*3), -1);
        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.DefBuff),
            Mathf.Max(minonCount*buffAmount,buffAmount*3), -1);
    }


    protected void CycloneAroundBig()
    {
        var proj = InstantiateMeele(GetProjectileOfFormatName("action03_1"),
            transform.position + Vector3.up,
            InitContainer(true));
    }

    protected void CycloneRingBig()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action03_2"),
            transform.position + Vector3.up,
            InitContainer(true),1);
    }

    protected void CycloneAroundSmallWithPillars()
    {
        var proj1 = InstantiateMeele(GetProjectileOfFormatName("action03_3"),
            transform.position + Vector3.up,
            InitContainer(true));

        var container = InitContainer(false,2);

        var proj2 = InstantiateRanged(GetProjectileOfFormatName("action03_4"),
            gameObject.RaycastedPosition()+new Vector2(12,0),
            container, 1);
        
        var proj3 = InstantiateRanged(GetProjectileOfFormatName("action03_4"),
            gameObject.RaycastedPosition()+new Vector2(-12,0),
            container, 1);

    }

    protected void AroundAttack()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action04_1"),
            gameObject.RaycastedPosition(),
            InitContainer(false),1);
    }

    protected void CorrosionAOE(float amount)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action05_1"),
            new Vector3(_behavior.targetPlayer.transform.position.x,gameObject.RaycastedPosition().y),
            InitContainer(false),1);
        
        _statusManager.ObtainHealOverTimeBuff(minonCount * 10f + 10f,-1,false,201121);

        var atk = proj.GetComponent<ForcedAttackFromEnemy>();
        atk.attackInfo[0].dmgModifier[0] *= (1 + minonCount * 1.25f);
        
        var corrosionEff = new AdvancedTimerBuff((int)BasicCalculation.BattleCondition.Corrosion,
            (int)amount + 500 * minonCount, 8, 0, -1, 1,-1,8);
        
        atk.AddWithConditionAll(corrosionEff,200);
    }

    protected GameObject GenerateAreaFX()
    {
        var areaEffect = Instantiate(GetProjectileOfFormatName("action06_1"),
            gameObject.RaycastedPosition() + new Vector2(ac.facedir * 8f, 0),
            Quaternion.identity,RangedAttackFXLayer.transform);
        return areaEffect;
    }

    protected UI_CountdownMinon SpawnEnergyBalls(int maxHP)
    {
        var ballBelow = SpawnEnemyMinon(GetProjectileOfName("e_db_2011_m1"),
            transform.position.SafePosition(new Vector2(ac.facedir * 3, 0.5f)), maxHP, 1, 1);
        
        var ballAbove = SpawnEnemyMinon(GetProjectileOfName("e_db_2011_m1"),
            transform.position.SafePosition(new Vector2(ac.facedir * 3, 9.5f)), maxHP/15, 1, 1);
        
        BattleEffectManager.Instance.SpawnTargetLockParticleIndicator(ballAbove.transform.position);
        BattleEffectManager.Instance.SpawnTargetLockParticleIndicator(ballBelow.transform.position);

        var UI = Instantiate(GetProjectileOfFormatName("action06_ui"),
            transform.position.SafePosition(new Vector2(2*ac.facedir, 4)), Quaternion.identity,
            RangedAttackFXLayer.transform);

        var UIRingSlider = UI.GetComponent<UI_RingSlider>();
        UIRingSlider.maxValue = 20;

        var UICountdownMinon = UI.GetComponent<UI_CountdownMinon>();
        UICountdownMinon.SetMaxCapacity(2);
        UICountdownMinon.AddNewStatusManager(ballAbove.GetComponent<StatusManager>());
        UICountdownMinon.AddNewStatusManager(ballBelow.GetComponent<StatusManager>());

        return UICountdownMinon;
    }

    protected void BurstNihilAOE(int ballLeftCount)
    {
        TimerBuff attackBuff = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
            100, -1, 100);
        TimerBuff defBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            100, -1, 100);

        TimerBuff nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, 15, 1);
        
        if (ballLeftCount >= 2)
        {
            _statusManager.ObtainTimerBuff(attackBuff);
            _statusManager.ObtainTimerBuff(defBuff,false);
        }

        _statusManager.ObtainHealOverTimeBuff(1 + 5 * ballLeftCount * ballLeftCount,
            15, true);
        
        var fx = InstantiateRanged(GetProjectileOfFormatName("action06_2"),
            gameObject.RaycastedPosition(),
            InitContainer(false),1);

        var atk = fx.GetComponentInChildren<ForcedAttackFromEnemy>();
        atk.AddWithConditionAll(nihilDebuff,200);

        if (ballLeftCount >= 2)
        {
            atk.attackInfo[0].dmgModifier[0] *= 1.25f;
        }
    }


}
