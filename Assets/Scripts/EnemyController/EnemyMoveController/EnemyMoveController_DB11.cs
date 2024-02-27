using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMoveController_DB11 : EnemyMoveManager
{

    public int minonCount => summonedChildren.Count;
    private List<StatusManager> summonedChildren = new();
    protected VoiceControllerEnemy voice;
    
    private enum MoveDialog
    {
        Intro,
        SlightWound,
        SevereWound,
        Defeated,
        DreamPillar,
        SkillGroup1,
        SkillGroup2,
        FailToForce,
        SuccessToForce
    }


    protected override void Start()
    {
        base.Start();
        voice = GetComponentInChildren<VoiceControllerEnemy>();
        GetAllAnchors();
        _statusManager.OnHPBelow0 += KillAllMinions;
    }

    private void KillAllMinions()
    {
        for(int i = summonedChildren.Count - 1; i >= 0; i--)
        {
            summonedChildren[i].currentHp = 0;
            summonedChildren[i].OnHPBelow0?.Invoke();
        }
        

        _statusManager.OnHPBelow0 -= KillAllMinions;
    }


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

    public IEnumerator DB11_Action01T(object[] summonInfo)
    {
        yield return _canActionOnFlyingGround;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        StageCameraController.SwitchOverallCamera();
        DOVirtual.DelayedCall
            (4f, () => StageCameraController.SwitchMainCamera(),
                false);
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("combo2");
        
        //ac.SetHitSensor(false);
        
        yield return new WaitForSeconds(0.5f);
        SummonChildrenOneByOne(summonInfo);
        yield return new WaitForSeconds(0.8f);
        //ac.SetHitSensor(true);
        
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
    public IEnumerator DB11_Action04(bool counter = false)
    {
        yield return _canAction;

        ac.TurnMove(_behavior.targetPlayer);
        if (counter)
        {
            ac.OnAttackEnter(100);
        }
        else
        {
            ac.OnAttackEnter();
        }
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + 
                                                                 (Vector3)(6*Vector2.up));
        yield return new WaitForSeconds(1f);
        
        anim.Play("combo2");

        yield return new WaitForSeconds(0.2f);
        
        AroundAttack();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
        
        anim.Play("combo3");
        
        if(counter)
            ac.SetCounter(false);

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
    public IEnumerator DB11_Action05(float amount,float increment)
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB11_Action05");
        yield return new WaitForSeconds(2f);
        
        anim.Play("combo2");
        
        if(increment <= 1)
            ac.SetHitSensor(false);
        yield return new WaitForSeconds(0.5f);
        
        CorrosionAOE(amount,increment);
        
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
            if(voice != null)
            {
                 voice.BroadCastMyVoice((int)MoveDialog.SuccessToForce);
            }
            yield return new WaitForSeconds(0.8f);
            BurstNihilAOE(leftMinons);
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        }else{
            anim.Play("knockdown_enter");
            if(voice != null)
            {
                voice.BroadCastMyVoice((int)MoveDialog.FailToForce); 
            }
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

    public IEnumerator DB11_Action06T(int maxHP)
    {
        yield return _canActionOnFlyingGround;
        
        StageCameraController.SwitchMainCameraFollowObject(gameObject);
        DOVirtual.DelayedCall(2f,
            ()=>StageCameraController.
                SwitchMainCameraFollowObject(_behavior.viewerPlayer),false);
        
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB11_Action06");
        ac.SetHitSensor(false);
        anim.Play("charge_enter");
        
        yield return new WaitForSeconds(1f);

        var fx = GenerateAreaFX();
        var uiController = SpawnEnergyBallsOneByOne(maxHP);
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
            if(voice != null)
            {
                voice.BroadCastMyVoice((int)MoveDialog.SuccessToForce);
            }
            yield return new WaitForSeconds(0.8f);
            BurstNihilAOE(leftMinons,30);
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        }else{
            anim.Play("knockdown_enter");
            if(voice != null)
            {
                voice.BroadCastMyVoice((int)MoveDialog.FailToForce); 
            }
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


    /// <summary>
    /// 送葬摇篮曲
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB11_Action07()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB11_Action07");
        yield return new WaitForSeconds(1f);
        
        anim.Play("combo2");
        voice?.BroadCastMyVoice((int)MoveDialog.DreamPillar);
        
        //ac.SetHitSensor(false);
        yield return new WaitForSeconds(0.5f);
        
        PillarAttack();
        
        yield return new WaitForSeconds(1f);
        //ac.SetHitSensor(true);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 罪恶牢笼
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB11_Action08()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB11_Action08");
        yield return new WaitForSeconds(1f);
        
        anim.Play("forcing_enter");
        voice?.BroadCastMyVoice((int)MoveDialog.SkillGroup1);
        
        //ac.SetHitSensor(false);
        yield return new WaitForSeconds(0.5f);
        
        PrisonOfSin();
        
        yield return new WaitForSeconds(1f);
        //ac.SetHitSensor(true);
        
        anim.Play("forcing_exit");
        yield return null;

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }


    /// <summary>
    /// 恶念狂怒
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB11_Action09()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB11_Action09");
        AllPlatformAOE();
        
        voice?.BroadCastMyVoice((int)MoveDialog.SkillGroup2);
  
        yield return new WaitForSeconds(2.5f);
        
        anim.Play("combo2");
        
        //ac.SetHitSensor(false);
        yield return new WaitForSeconds(0.5f);
        
        
        
        yield return new WaitForSeconds(1f);
        //ac.SetHitSensor(true);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    ///Buff Self
    public IEnumerator DB11_Action10(float amount)
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        //bossBanner?.PrintSkillName("DB11_Action09");

        anim.Play("combo2");
        
        
        yield return new WaitForSeconds(0.8f);
        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.AtkBuff), amount, 15f);
        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.DefBuff), amount, 15f);

        foreach (var child in summonedChildren)
        {
            child.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.AtkBuff), amount/2, 15f);
            child.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.DefBuff), amount/2, 15f);
        }
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
        
        
    }

    /// <summary>
    /// 断罪狂风Inner
    /// </summary>
    /// <param name="lastSafePointIsInner"></param>
    /// <returns></returns>
    public IEnumerator DB11_Action11A()
    {
        yield return _canAction;
        
        bossBanner?.PrintSkillName("DB11_Action11");
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        anim.Play("forcing_enter");

        GameObject hint1A = null;

        hint1A = GenerateWarningPrefab("action10_1", transform.position,
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        GameObject hint1B = GenerateWarningPrefab("action10_4", transform.position,
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        yield return new WaitForSeconds(hint1A.GetComponent<EnemyAttackHintBarShine>().warningTime + 0.1f);
        
        BoostedCycloneAroundDoubleRingOuter();
        
        GameObject hint2A = GenerateWarningPrefab("action10_2", transform.position,
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        GameObject hint2B = GenerateWarningPrefab("action10_3", transform.position,
            Quaternion.identity, 
            RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(hint2A.GetComponent<EnemyAttackHintBarShine>().warningTime + 0.1f);
        
        BoostedCycloneAroundDoubleRingInner();
        
        GameObject hint3 = GenerateWarningPrefab("action10_5", transform.position,
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        

        yield return new WaitForSeconds(hint3.GetComponent<EnemyAttackHintBarShine>().warningTime + 0.1f);
        
        BoostedCycloneMassiveRing();

        anim.Play("forcing_exit");

        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        ac.ResetGravityScale();
        anim.Play("idle");
        QuitAttack();
    }


    /// <summary>
    /// 断罪狂风Outer
    /// </summary>
    /// <param name="lastSafePointIsInner"></param>
    /// <returns></returns>
    public IEnumerator DB11_Action11B()
    {
        yield return _canAction;
        
        bossBanner?.PrintSkillName("DB11_Action11");
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        anim.Play("forcing_enter");

        GameObject hint1A = null;

        hint1A = GenerateWarningPrefab("action10_2", transform.position,
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        GameObject hint1B = GenerateWarningPrefab("action10_3", transform.position,
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        yield return new WaitForSeconds(hint1A.GetComponent<EnemyAttackHintBarShine>().warningTime + 0.1f);
        
        BoostedCycloneAroundDoubleRingInner();
        
        GameObject hint2A = GenerateWarningPrefab("action10_1", transform.position,
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        GameObject hint2B = GenerateWarningPrefab("action10_4", transform.position,
            Quaternion.identity, 
            RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(hint2A.GetComponent<EnemyAttackHintBarShine>().warningTime + 0.1f);
        
        BoostedCycloneAroundDoubleRingOuter();
        
        GameObject hint3 = GenerateWarningPrefab("action10_6", transform.position,
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        

        yield return new WaitForSeconds(hint3.GetComponent<EnemyAttackHintBarShine>().warningTime + 0.1f);
        
        BoostedCycloneMassiveCircle();

        anim.Play("forcing_exit");

        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        ac.ResetGravityScale();
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 重罪牢笼
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB11_Action12()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB11_Action12");
        yield return new WaitForSeconds(1f);
        
        anim.Play("forcing_enter");
        voice?.BroadCastMyVoice((int)MoveDialog.SkillGroup1);
        
        //ac.SetHitSensor(false);
        yield return new WaitForSeconds(0.5f);
        
        PrisonOfSinBoosted();
        
        yield return new WaitForSeconds(1f);
        //ac.SetHitSensor(true);
        
        anim.Play("forcing_exit");
        yield return null;

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
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

    protected void SummonChildrenOneByOne(object[] messages)
    {
        voice?.BroadCastMyVoice((int)MoveDialog.SkillGroup1);
        
        int index = 1;
        int childNum = Convert.ToInt32(messages[0]);

        for (int i = 0; i < childNum; i++)
        {
            if(summonedChildren.Count > 8)
                break;
            
            string prefabName = (string)messages[index++];
            float positionX = (float)Convert.ToDouble(messages[index++]);
            float positionY = (float)Convert.ToDouble(messages[index++]);
            int hp = Convert.ToInt32(messages[index++]);
            int atk = Convert.ToInt32(messages[index++]);
            
            var enemy = SpawnEnemyMinon(GetProjectileOfName(prefabName),new Vector2(positionX,positionY),
                hp,atk, ac.facedir);
            
            var fx = Instantiate(GetProjectileOfFormatName("action01_1"),
                new Vector2(positionX,positionY-1.5f),Quaternion.identity);
            
            var stat = enemy.GetComponent<StatusManager>();
            summonedChildren.Add(stat);
            
            StatusManager.StatusManagerVoidDelegate RemoveChild = null;
            RemoveChild = () =>
            {
                summonedChildren.Remove(stat);
                stat.OnReviveOrDeath -= RemoveChild;
            };
            stat.OnReviveOrDeath += RemoveChild;
            
            
            
        }






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
    
    
    protected void BoostedCycloneAroundDoubleRingOuter()
    {
        var container = InitContainer(false,2);
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action10_1"),
            transform.position,
            container,1);
        
        var proj2 = InstantiateRanged(GetProjectileOfFormatName("action10_4"),
            transform.position,
            container,1);
        
        var atk1 = proj.GetComponent<AttackFromEnemy>();
        var atk2 = proj2.GetComponent<AttackFromEnemy>();
        
        TimerBuff sleep = new TimerBuff((int)BasicCalculation.BattleCondition.Sleep,
            1,Random.Range(4f,6f),
            1);
        
        atk1.AddWithConditionAll(sleep,80);
        atk2.AddWithConditionAll(sleep,80);
        
    }

    protected void BoostedCycloneAroundDoubleRingInner()
    {
        var container = InitContainer(false,2);
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action10_2"),
            transform.position,
            container,1);
        
        var proj2 = InstantiateRanged(GetProjectileOfFormatName("action10_3"),
            transform.position,
            container,1);
        
        var atk1 = proj.GetComponent<AttackFromEnemy>();
        var atk2 = proj2.GetComponent<AttackFromEnemy>();
        
        TimerBuff sleep = new TimerBuff((int)BasicCalculation.BattleCondition.Sleep,
            1,Random.Range(4f,6f),
            1);
        
        atk1.AddWithConditionAll(sleep,80);
        atk2.AddWithConditionAll(sleep,80);
    }

    protected void BoostedCycloneMassiveRing()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action10_5"),
            transform.position,
            InitContainer(false),1);
        
        var atk = proj.GetComponent<AttackFromEnemy>();
        
        TimerBuff sleep = new TimerBuff((int)BasicCalculation.BattleCondition.Sleep,
            1,Random.Range(4f,6f),
            1);
        
        atk.AddWithConditionAll(sleep,80);
    }

    protected void BoostedCycloneMassiveCircle()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action10_6"),
            transform.position,
            InitContainer(false),1);
        
        var atk = proj.GetComponent<AttackFromEnemy>();
        
        TimerBuff sleep = new TimerBuff((int)BasicCalculation.BattleCondition.Sleep,
            1,Random.Range(4f,6f),
            1);
        
        atk.AddWithConditionAll(sleep,80);
    }




    protected void AroundAttack()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action04_1"),
            gameObject.RaycastedPosition(),
            InitContainer(false),1);
    }

    protected void CorrosionAOE(float amount,float increment)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action05_1"),
            new Vector3(_behavior.targetPlayer.transform.position.x,
                _behavior.targetPlayer.RaycastedPosition().y),
            InitContainer(false),1);

        float healTime = -1;
        if (increment == 0)
        {
            healTime = 6f;
        }

        _statusManager.ObtainHealOverTimeBuff(minonCount * 10f + 10f,healTime,false,201121);

        var atk = proj.GetComponent<ForcedAttackFromEnemy>();
        atk.attackInfo[0].dmgModifier[0] *= (1 + minonCount * 1.25f);
        
        var corrosionEff = new AdvancedTimerBuff((int)BasicCalculation.BattleCondition.Corrosion,
            (int)amount + increment * minonCount, 8, 0, -1, 1,-1,8);
        
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

    protected UI_CountdownMinon SpawnEnergyBallsOneByOne(int maxHP)
    {
        StatusManager.StatusManagerVoidDelegate handler = null;
        
        var UI = Instantiate(GetProjectileOfFormatName("action06_ui"),
            transform.position.SafePosition(new Vector2(2*ac.facedir, 4)), Quaternion.identity,
            RangedAttackFXLayer.transform);
        var UIRingSlider = UI.GetComponent<UI_RingSlider>();
        UIRingSlider.maxValue = 20;
        UIRingSlider.currentValue = 20;

        var UICountdownMinon = UI.GetComponent<UI_CountdownMinon>();
        UICountdownMinon.SetMaxCapacity(4);

        int minionCount = 0;
        
        handler = () =>
        {
            print($"MinionCount:{minionCount}, SliderValue:{UIRingSlider.currentValue}");
            if (minionCount < 4 && UIRingSlider.currentValue > 0)
            {
                DOVirtual.DelayedCall(0.3f,
                    () => 
                    {
                        if(UIRingSlider == null)
                            return;
                        
                        
                        var minion = SpawnEnemyMinon(GetProjectileOfName("e_db_2011_m1"),
                            transform.position.SafePosition(new Vector2(ac.facedir * 3, 1f)), maxHP, 1, 1);
                        var status = minion.GetComponent<StatusManager>();
                        UICountdownMinon.AddNewStatusManager(status);
                
                
                        status.OnReviveOrDeath += () =>
                        {
                            status.OnReviveOrDeath -= handler;
                            handler();
                        };
                        
                    }, false);
                

                minionCount++;
            }
        };

        handler();
        return UICountdownMinon;
    }





    protected void BurstNihilAOE(int ballLeftCount, float duration = -1)
    {
        TimerBuff attackBuff = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
            100, duration, 100);
        TimerBuff defBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            100, duration, 100);

        TimerBuff nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, 15, 1);
        
        if (ballLeftCount >= 2)
        {
            _statusManager.ObtainTimerBuff(attackBuff);
            _statusManager.ObtainTimerBuff(defBuff,false);
            foreach (var child in summonedChildren)
            {
                child.ObtainHealOverTimeBuff(10,
                    15, true);
                child.ObtainTimerBuff(attackBuff);
            }
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

    protected void PillarAttack()
    {
        ConditionalAttackEffect caf = 
            new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.Custom,
                new string []{"1",((int)BasicCalculation.BattleCondition.Sleep).ToString()},
                new string[]{}).SetEffectFunction(
                (stats,atkStat) =>
                {
                    stats.targetStat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.Nihility,
                        1, 15, 1,-1);
                    stats.targetStat.RemoveTimerBuff((int)BasicCalculation.BattleCondition.Sleep, true);
                    return stats.targetStat.currentHp - 101 > 0 ? stats.targetStat.currentHp - 101 : 0;
                });
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action07_1"),
            _behavior.targetPlayer.RaycastedPosition(),
            InitContainer(false),1);
        
        var atk = proj.GetComponent<ForcedAttackFromEnemy>();
        
        atk.AddConditionalAttackEffect(caf);
        
        atk.target = _behavior.targetPlayer;
        

    }

    protected void PrisonOfSin()
    {
        var hint = GenerateWarningPrefab("action08", _behavior.targetPlayer.transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        hint.GetComponent<EnemyAttackHintBarChaser>().target = _behavior.targetPlayer;
        
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3);

        DOVirtual.DelayedCall(hint.GetComponent<EnemyAttackHintBar>().warningTime,
            () =>
            {
                var proj = Instantiate(GetProjectileOfFormatName("action08_1"),
                    _behavior.targetPlayer.transform.position,
                    Quaternion.identity, RangedAttackFXLayer.transform);

                var controller = proj.GetComponent<ControlDisableEffector>();
                controller.SetTarget(_behavior.targetPlayer);

            }, false);

    }
    
    protected void PrisonOfSinBoosted()
    {
        var hint = GenerateWarningPrefab("action12", _behavior.targetPlayer.transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        hint.GetComponent<EnemyAttackHintBarChaser>().target = _behavior.targetPlayer;
        
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3);

        DOVirtual.DelayedCall(hint.GetComponent<EnemyAttackHintBar>().warningTime,
            () =>
            {
                var proj = Instantiate(GetProjectileOfFormatName("action12_1"),
                    _behavior.targetPlayer.transform.position,
                    Quaternion.identity, RangedAttackFXLayer.transform);

                var controller = proj.GetComponent<ControlDisableEffector>();
                controller.SetTarget(_behavior.targetPlayer);

                _behavior.targetPlayer.GetComponent<StatusManager>().ObtainTimerBuff((int)(
                    BasicCalculation.BattleCondition.DefDebuff),
                    30, 10);

            }, false);

    }

    protected void AllPlatformAOE()
    {
        var raycastedPlatform = gameObject.RaycastedPlatform();
        
        var platformLength = raycastedPlatform.bounds.size.x;

        if (platformLength < 7)
        {
            var newCol = ((Vector2)raycastedPlatform.bounds.min - new Vector2(0, 0.5f)).
                RaycastedPlatform();

            if (newCol == null)
            {
                newCol = (Vector2.zero).RaycastedPlatform();
            }
            platformLength = newCol.bounds.size.x;
            raycastedPlatform = newCol;
        }

        var hint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(
            ac,new Vector3(raycastedPlatform.bounds.center.x,raycastedPlatform.bounds.max.y),
            RangedAttackFXLayer.transform,
            new Vector2(13,platformLength),Vector2.zero,false,
            0,3f,90,1,true,false);
        
        DOVirtual.DelayedCall(3.1f, () =>
        {
            StormOfMalevolence(raycastedPlatform);
        },false);
        
    }

    protected void StormOfMalevolence(Collider2D col)
    {
        var platformWidth = col.bounds.size.x;
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action09_1"),
            new Vector3(col.bounds.center.x,
                col.bounds.max.y),
            InitContainer(false),1);

        var inPrefab = proj.transform.GetChild(0).gameObject;
        
        // 计算可以放置光柱的区域的宽度（减去两边的2.5单位）
        float effectiveWidth = platformWidth - 5f;

        // 计算光柱的数量，至少为1，如果平台宽度小于7，只能放置一个光柱
        int pillarCount = (platformWidth < 7f) ? 1 : Mathf.Max(1, Mathf.RoundToInt(effectiveWidth / 5f));

        // 如果平台宽度大于7，计算光柱之间的间距
        float pillarInterval = (pillarCount > 1) ? effectiveWidth / (pillarCount - 1) : 0;
        
        for (int i = 0; i < pillarCount; i++)
        {
            float x = col.bounds.min.x + 2.5f + i * pillarInterval;
            Instantiate(inPrefab, new Vector3(x, col.bounds.max.y, 0), inPrefab.transform.rotation,
                proj.transform).SetActive(true);
        }
        
        var atkCollider = proj.GetComponent<BoxCollider2D>();
        atkCollider.size = new Vector2(platformWidth, 13);
        atkCollider.offset = new Vector2(0, 6.5f);


    }


}
