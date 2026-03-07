using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMoveController_H001 : EnemyMoveManager
{
    private VoiceControllerEnemy _voiceControllerEnemy;
    [SerializeField] private GameObject partRenderer;
    [SerializeField] private GameObject partBreakFXPrefab;
    [SerializeField] private GameObject countdownUIPrefab;
    
    [SerializeField][Tooltip("哥布林")] private GameObject minionPrefabA;
    [SerializeField][Tooltip("哥布林法师")] private GameObject minionPrefabB;
    [SerializeField][Tooltip("阿格尼")] private GameObject elitePrefabA;
    [SerializeField][Tooltip("尼德霍格")] private GameObject elitePrefabB;
    
    [SerializeField] private GameObject summonCirclePrefab;
    [SerializeField] private GameObject orbPrefab;
    
    private List<StatusManager> minionInstanceList = new();
    private GameObject summonCircleInstanceLeft;
    private GameObject summonCircleInstanceRight;
    private GameObject shieldInstance;
    
    public bool IsInvincible => shieldInstance != null && shieldInstance.activeSelf;
    private bool IsBoosted => (_behavior as H001_BehaviorTree).destructionCount <= 4;
    /// <summary>
    /// type = 
    /// </summary>
    private int type = 1;
    
    /*
     * Action01: Buff all children
     * Action02: Targeting Pillar (action02_1: Pillar)
     * Action03: Bouncing Projectiles
     * Action04: Warp Laser Attack
     * Action05: Summon Child (action05_1: Summon FX)
     * Action06: Summon Elite
     * Action07: Corrosion
     * Action08: Summon Executioners
     * Action09: Weak Points One by One
     * Action10: Weak Points Simultaneously
     * Action11: Chasing Pillar
     * Action12: Absolutely Truth
     */

    private enum MyVoice
    {
        Transform = 0,
        Intro = 1,
        HPBelow70 = 2,
        HPBelow40 = 3,
        Defeat = 4,
        Buff = 5,
        Punishment = 6,
        AbsoluteTruth = 7,
        SummonElite = 8,
        Executioners = 9,
        SuccessToForce = 10,
        FailToForce = 11,
        Healing = 12,
        ElementSwitch = 13
    }

    protected override void Start()
    {
        base.Start();
        _voiceControllerEnemy = GetComponentInChildren<VoiceControllerEnemy>();
        _statusManager.OnReviveOrDeath += KillAllMinions;
    }


    public IEnumerator PartBreak()
    {
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        // StageCameraController.SwitchMainCamera();
        // StageCameraController.SwitchMainCameraFollowObject(gameObject);
        
        yield return new WaitForSeconds(0.5f);
        _behavior.breakable = false;
        _statusManager.ImmuneToAllControlAffliction = true;
        anim.Play("part_break");
        //ac.OnHurtEnter();
        BattleEffectManager.Instance.PlayReviveSoundEffect();
        Instantiate(partBreakFXPrefab,transform.position + new Vector3(ac.facedir*2,1.5f),
            Quaternion.identity,RangedAttackFXLayer.transform);

        //_statusManager.baseDef = 12;
        
        (_statusManager as SpecialStatusManager).counterModifier = 0.2f;
        //DecreaseResistances();

        partRenderer.SetActive(false);
        

        yield return new WaitForSeconds(0.1f);
       

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        //StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);

        yield return new WaitForSeconds(1f);
        _behavior.breakable = true;
        _statusManager.ImmuneToAllControlAffliction = false;
        
        QuitAttack();
    }
    
    /// <summary>
    /// Buff all children
    /// </summary>
    /// <param name="buffAmount"></param>
    /// <returns></returns>
    public IEnumerator H001_Action01(int buffAmount = 20)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        anim.Play("buff");

        yield return new WaitForSeconds(0.5f);

        BuffAllChildren(buffAmount);
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoice.Buff);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    
    public IEnumerator H001_Action02()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer, 2f);
        bossBanner?.PrintSkillName("H001_Action02");
        
        anim.Play("skill1_1");

        yield return new WaitForSeconds(0.8f);
        
        PillarAttack();

        anim.Play("skill1_3");

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// 弹弹乐
    /// </summary>
    /// <returns></returns>
    public IEnumerator H001_Action03()
    {
        yield return _canAction;
        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);

        anim.Play("summon");

        yield return new WaitForSeconds(0.2f);
        
        if(type == 1)
            GenerateProjectiles(GetProjectileOfFormatName("action03_1"));
        else
            GenerateProjectiles(GetProjectileOfFormatName("action03_2"));

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }

    
    /// <summary>
    /// laser
    /// </summary>
    /// <returns></returns>
    public IEnumerator H001_Action04()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);

        var warpFX = GetProjectileOfFormatName("action01");

        yield return new WaitForSeconds(0.5f);
        
        InstantiateWarpEffect(warpFX);

        yield return new WaitForSeconds(0.1f);
        
        DisappearRenderer();
        ac.SetHitSensor((false));
        
        yield return new WaitForSeconds(0.2f);

        Vector2 pos = transform.position;
        
        if (Mathf.Abs(transform.position.x) < 20)
        {
            if (_behavior.targetPlayer.transform.position.x < 0)
            {
                pos = new Vector2(_behavior.targetPlayer.transform.position.x - 15,
                    transform.position.y);
            }
            else
            {
                pos = new Vector2(_behavior.targetPlayer.transform.position.x + 15,
                    transform.position.y);
            }
            
            pos.x = Mathf.Clamp(pos.x, BattleStageManager.Instance.mapBorderL,
                BattleStageManager.Instance.mapBorderR);
            pos.y = _behavior.targetPlayer.RaycastedPosition().y + ac.GetActorHeight();
        }
        else
        {
            pos = new Vector2(0, transform.position.y);
            pos.y = _behavior.targetPlayer.RaycastedPosition().y + ac.GetActorHeight();
        }
        
        transform.position = pos;
        AppearRenderer();
        if (!IsInvincible)
        {
            ac.SetHitSensor(true);
        }
        
        ac.SetCounter(true);
        ac.SetKBRes(100);
        ac.TurnMove(_behavior.targetPlayer);

        yield return null;
        
        var muzzleFX = Instantiate(GetProjectileOfFormatName("action04_1"),
            new Vector3(transform.position.x + ac.facedir*4,transform.position.y + 2),
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        bool boosted = false;
        if ((_behavior as H001_BehaviorTree).destructionCount < 5)
        {
            boosted = true;
            var muzzleFX2 = Instantiate(GetProjectileOfFormatName("action04_1"),
                new Vector3(transform.position.x + ac.facedir*4,transform.position.y + 7),
                Quaternion.identity, RangedAttackFXLayer.transform);
            var muzzleFX3= Instantiate(GetProjectileOfFormatName("action04_1"),
                new Vector3(transform.position.x + ac.facedir*4,transform.position.y - 3),
                Quaternion.identity, RangedAttackFXLayer.transform);
            Destroy(muzzleFX2,2.7f);
            Destroy(muzzleFX3,2.7f);
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                transform.position + new Vector3(2*ac.facedir,7f), 
                RangedAttackFXLayer.transform, new Vector2(32,4),Vector2.zero, true,
                1, 2f, ac.facedir==1?0:180,0.5f,true,true);
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                transform.position + new Vector3(2*ac.facedir,-3f), 
                RangedAttackFXLayer.transform, new Vector2(32,4),Vector2.zero, true,
                1, 2f, ac.facedir==1?0:180,0.5f,true,true);
        }
        
        var hintbar = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            transform.position + new Vector3(2*ac.facedir,2f), 
            RangedAttackFXLayer.transform, new Vector2(32,4),Vector2.zero, !boosted,
            1, 2f, ac.facedir==1?0:180,0.5f,true,true);

        yield return new WaitForSeconds(0.7f);
        
        anim.Play("summon");
        
        yield return new WaitForSeconds(1.5f);
        
        if(muzzleFX!= null)
            Destroy(muzzleFX,0.5f);
        
        LaserAttack(type==1?GetProjectileOfFormatName("action04_2"):GetProjectileOfFormatName("action04_3"),boosted);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        ac.ResetGravityScale();
        QuitAttack();
    }


    /// <summary>
    /// 召唤小怪
    /// </summary>
    /// <param name="summonInfo"></param>
    /// <returns></returns>
    public IEnumerator H001_Action05(object[] summonInfo)
    {
        yield return _canActionOnFlyingGround;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        StageCameraController.SwitchOverallCamera();
        DOVirtual.DelayedCall
        (4f, () => StageCameraController.SwitchMainCamera(),
            false);
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("skill1_1");

        yield return new WaitForSeconds(1f);
        
        anim.Play("skill1_3");
        
        yield return new WaitForSeconds(0.5f);
        SummonChildrenOneByOne(summonInfo);
        //yield return new WaitForSeconds(0.8f);
        //ac.SetHitSensor(true);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 召唤Agni
    /// </summary>
    /// <param name="summonInfo"></param>
    /// <returns></returns>
    public IEnumerator H001_Action06()
    {
        GrantInvincible();
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        StageCameraController.SwitchOverallCamera();
        DOVirtual.DelayedCall
        (4f, () => StageCameraController.SwitchMainCamera(),
            false);
        
        var warpFX = GetProjectileOfFormatName("action01");

        yield return new WaitForSeconds(0.5f);
        
        InstantiateWarpEffect(warpFX);

        yield return new WaitForSeconds(0.1f);
        
        DisappearRenderer();

        yield return new WaitForSeconds(0.2f);

        Vector2 pos = new Vector2(0,6);
        transform.position = pos;

        yield return null;
        
        InstantiateWarpEffect(warpFX);
        
        yield return new WaitForSeconds(0.1f);

        AppearRenderer();
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoice.SummonElite);

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("skill1_1");

        yield return new WaitForSeconds(1f);
        
        anim.Play("skill1_3");
        
        yield return new WaitForSeconds(0.5f);
        
        SummonElite(elitePrefabA, new Vector2(0,-1));

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    

    /// <summary>
    /// corrosion
    /// </summary>
    /// <param name="corrosionAmount"></param>
    /// <returns></returns>
    public IEnumerator H001_Action07(int corrosionAmount)
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("H001_Action07");
        yield return new WaitForSeconds(0.7f);
        
        anim.Play("skill3_1");

        yield return new WaitForSeconds(1f);
        
        anim.Play("skill3_3");
        
        yield return new WaitForSeconds(0.6f);

        CorrosionAttack(corrosionAmount);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 处刑召唤
    /// </summary>
    /// <returns></returns>
    public IEnumerator H001_Action08(int hp1, int hp2, int atk1, int atk2)
    {
        yield return _canActionOnFlyingGround;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("H001_Action08");
        StageCameraController.SwitchOverallCamera();
        DOVirtual.DelayedCall
        (4f, () => StageCameraController.SwitchMainCamera(),
            false);
        
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoice.Executioners);
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("skill1_1");

        yield return new WaitForSeconds(1f);
        
        anim.Play("skill1_3");
        
        yield return new WaitForSeconds(0.5f);
        
        SummonCircleStart(hp1, hp2, atk1, atk2);
        //yield return new WaitForSeconds(0.8f);
        //ac.SetHitSensor(true);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    
    /// <summary>
    /// 黑暗圣歌
    /// </summary>
    /// <returns></returns>
    public IEnumerator H001_Action09(int hp)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetHitSensor(false);
        bossBanner?.PrintSkillName("H001_Action09");
        
        StageCameraController.SwitchOverallCamera();
        DOVirtual.DelayedCall
        (3, () => StageCameraController.SwitchMainCamera(),
            false);
        
        var warpFX = GetProjectileOfFormatName("action01");

        yield return new WaitForSeconds(0.5f);
        
        InstantiateWarpEffect(warpFX);

        yield return new WaitForSeconds(0.1f);
        
        DisappearRenderer();

        yield return new WaitForSeconds(0.2f);

        Vector2 pos = new Vector2(0,5);
        transform.position = pos;

        yield return null;
        
        InstantiateWarpEffect(warpFX);
        
        yield return new WaitForSeconds(0.1f);

        AppearRenderer();
        ac.TurnMove(_behavior.targetPlayer);
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoice.Healing);

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("skill2_1");

        yield return new WaitForSeconds(1f);
        
        //anim.Play("skill1_3");
        var fx = GenerateAreaFX();
        
        var uiController = SpawnEnergyBallsOneByOne(hp);
        var uiTimer = uiController.GetComponent<UI_RingSlider>();
        uiTimer.currentValue = uiTimer.maxValue;

        yield return null;
        

        yield return new WaitUntil(() => uiController.Value <= 0 || uiTimer.currentValue <= 0);
        Destroy(fx);
        
        if (uiController.Value > 0)
        {
            var leftMinons = uiController.KillAllMinons();
            
            _statusManager.ReliefAllDebuff();
            _statusManager.ObtainHealOverTimeBuff(5,10,true);
            if (leftMinons >= 1)
            {
                _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff, 100, 30);
            }
            if (leftMinons >= 2)
            {
                _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff, 100, 30);
            }
            
            anim.Play("skill2_3");
            Destroy(uiController.gameObject);
            
            if(_voiceControllerEnemy != null)
            {
                _voiceControllerEnemy.BroadCastMyVoice((int)MyVoice.SuccessToForce);
            }
            yield return new WaitForSeconds(0.8f);

            NihilBurstAOE(leftMinons);
            
            ac.SetHitSensor(true);
            
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        }
        else
        {
            anim.Play("knockdown_1");
            if(_voiceControllerEnemy != null)
            {
                _voiceControllerEnemy.BroadCastMyVoice((int)MyVoice.FailToForce);
            }
            Destroy(uiController.gameObject);
            yield return null;
            ac.SetHitSensor(true);
            
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
            anim.Play("knockdown_3");
            yield return null;

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        }
        anim.Play("idle");
        QuitAttack();
    }


    /// <summary>
    /// 灵魂献祭
    /// </summary>
    /// <returns></returns>
    public IEnumerator H001_Action10(int hp, int minionHP1,int minionAtk1, int minionHP2, int minionAtk2)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetHitSensor(false);
        bossBanner?.PrintSkillName("H001_Action10");
        
        StageCameraController.SwitchOverallCamera();
        DOVirtual.DelayedCall
        (3, () => StageCameraController.SwitchMainCamera(),
            false);
        
        var warpFX = GetProjectileOfFormatName("action01");

        yield return new WaitForSeconds(0.5f);
        
        InstantiateWarpEffect(warpFX);

        yield return new WaitForSeconds(0.1f);
        
        DisappearRenderer();

        yield return new WaitForSeconds(0.2f);

        Vector2 pos = new Vector2(0,0);
        transform.position = pos;

        yield return null;
        
        InstantiateWarpEffect(warpFX);
        
        yield return new WaitForSeconds(0.1f);

        AppearRenderer();
        //_voiceControllerEnemy?.BroadCastMyVoice((int)MyVoice.SummonElite);

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("skill2_1");

        yield return new WaitForSeconds(1f);
        
        //anim.Play("skill1_3");
        var fx = GenerateAreaFX();
        SpawnEnemies(minionHP1, minionHP2, minionAtk1, minionAtk2);
        var uiController = SpawnEnergyBalls(hp);
        var uiTimer = uiController.GetComponent<UI_RingSlider>();
        uiTimer.currentValue = uiTimer.maxValue;

        yield return null;
        
        yield return new WaitUntil(() => uiController.Value <= 0 || uiTimer.currentValue <= 0);

        Destroy(fx);
        
        if (uiController.Value > 0)
        {
            var leftMinons = uiController.KillAllMinons();
            
            AbsorbMinions(leftMinons);
            _statusManager.ReliefAllDebuff();
            if (leftMinons >= 2)
            {
                _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff, 100, 30);
            }
            if (leftMinons >= 3)
            {
                _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff, 100, 30);
            }
            
            anim.Play("skill2_3");
            Destroy(uiController.gameObject);
            
            if(_voiceControllerEnemy != null)
            {
                _voiceControllerEnemy.BroadCastMyVoice((int)MyVoice.SuccessToForce);
            }
            
            yield return new WaitForSeconds(0.1f);
            ac.SetHitSensor(true);

            yield return new WaitForSeconds(0.7f);

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        }
        else
        {
            anim.Play("knockdown_1");
            if(_voiceControllerEnemy != null)
            {
                _voiceControllerEnemy.BroadCastMyVoice((int)MyVoice.FailToForce);
            }
            Destroy(uiController.gameObject);
            yield return null;
            
            ac.SetHitSensor(true);
            
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
            anim.Play("knockdown_3");
            yield return null;
            
            
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        }
        
        
        anim.Play("idle");
        QuitAttack();
    }

    
    /// <summary>
    /// 聒噪之刑
    /// </summary>
    /// <returns></returns>
    public IEnumerator H001_Action11()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("H001_Action11");
        yield return new WaitForSeconds(0.7f);
        
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoice.Punishment);
        
        var hintbar = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector3(_behavior.targetPlayer.transform.position.x,-1), 
            RangedAttackFXLayer.transform, new Vector2(35,2.5f),Vector2.zero, false,
            1, 3f, 90,0.5f,true,false);

        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3);
        var chaser = hintbar.AddComponent<EnemyAttackHintBarTopDownChaser>();
        chaser.SetMoveSpeedX(10);
        chaser.SetHardLock(false);
        chaser.SetUseCastPlatformY((false));
        chaser.SetLockTime(2.5f);
        chaser.target = _behavior.targetPlayer;

        DOVirtual.DelayedCall(3f, () =>
        {
           PillarPunishment(hintbar.transform.position);
        },false);
        
        anim.Play("skill3_1");

        yield return new WaitForSeconds(2.4f);
        
        anim.Play("skill3_3");
        
        yield return new WaitForSeconds(0.6f);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// 绝对真理
    /// </summary>
    /// <returns></returns>
    public IEnumerator H001_Action12()
    {
        yield return _canActionOnFlyingGround;
        
        ac.SetHitSensor(false);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("H001_Action12");
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoice.AbsoluteTruth);
        
        yield return new WaitForSeconds(0.7f);
        
        anim.Play("skill2_1");
        CineMachineOperator.Instance.CamaraShake(8f,1.5f);

        yield return new WaitForSeconds(1f);
        
        anim.Play("skill2_3");
        
        //PurgedShapeShiftingOfViewer();
        
        yield return new WaitForSeconds(0.5f);

        AbsoluteTruth();

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        if (!IsInvincible)
        {
            ac.SetHitSensor(true);
        }
        anim.Play("idle");
        QuitAttack();
    }
    
    
    /// <summary>
    /// 聒噪之刑
    /// </summary>
    /// <returns></returns>
    public IEnumerator H001_Action13()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.5f);
        
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoice.ElementSwitch);
        
        
        anim.Play("skill3_1");

        yield return new WaitForSeconds(1f);
        ElementSwitch();
        
        anim.Play("skill3_3");
        
        yield return new WaitForSeconds(0.6f);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    

    private void BuffAllChildren(int buffEffect)
    {
        var bhv = _behavior as H001_BehaviorTree;
        foreach (var stat in minionInstanceList)
        {
            if (stat.currentHp > 0)
            {
                stat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
                    buffEffect, 20);
                if(!bhv.bookHasBroken)
                    stat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
                        buffEffect, 20);
            }
        }
        
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
            buffEffect, 20);
        
        if(!bhv.bookHasBroken)
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
                buffEffect, 20);
    }
    
    private void PillarAttack()
    {
        ConditionalAttackEffect caf = 
            new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.Custom,
                new string []{"1",((int)BasicCalculation.BattleCondition.Sleep).ToString()},
                new string[]{}).SetEffectFunction(
                (stats,atkStat) =>
                {
                    stats.targetStat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.Nihility,
                        1, 30, 1,-1,false);
                    stats.targetStat.RemoveTimerBuff((int)BasicCalculation.BattleCondition.Sleep, true);
                    stats.sourceStat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
                        50, 30);
                    return stats.targetStat.currentHp - 101 > 0 ? stats.targetStat.currentHp - 101 : 0;
                });
        
        ConditionalAttackEffect caf2 = 
            new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.Custom,
                new string []{"1",((int)BasicCalculation.BattleCondition.Bog).ToString()},
                new string[]{}).SetEffectFunction(
                (stats,atkStat) =>
                {
                    stats.targetStat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.Nihility,
                        1, 30, 1,-1,false);
                    //stats.targetStat.RemoveTimerBuff((int)BasicCalculation.BattleCondition.Sleep, true);
                    stats.sourceStat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
                        50, 30);
                    return stats.targetStat.currentHp - 101 > 0 ? stats.targetStat.currentHp - 101 : 0;
                });
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action02_1"),
            _behavior.targetPlayer.RaycastedPosition(),
            InitContainer(false),1);
        
        var atk = proj.GetComponent<ForcedAttackFromEnemy>();
        
        atk.AddConditionalAttackEffect(caf);
        atk.AddConditionalAttackEffect(caf2);
        
        atk.target = _behavior.targetPlayer;
        

    }

    private void CorrosionAttack(float amount)
    {
        var isBoosted = IsBoosted;
        var container = InitContainer(false,isBoosted?2:1);
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action07_1"),
            new Vector3(_behavior.targetPlayer.transform.position.x,
                _behavior.targetPlayer.RaycastedPosition().y),
            container,1);

        var atk = proj.GetComponent<ForcedAttackFromEnemy>();
        
        var corrosionEff = new AdvancedTimerBuff((int)BasicCalculation.BattleCondition.Corrosion,
            (int)amount, 8, 0, -1, 1,-1,8);
        
        atk.AddWithConditionAll(corrosionEff,200);

        if (isBoosted)
        {
            DOVirtual.DelayedCall(2f, () => CorrosionAttackSecond(amount,container),
                false);
        }
        
    }

    private void CorrosionAttackSecond(float amount,GameObject container)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action07_1"),
            new Vector3(_behavior.targetPlayer.transform.position.x,
                _behavior.targetPlayer.RaycastedPosition().y),
            container,1);

        var atk = proj.GetComponent<ForcedAttackFromEnemy>();
        
        var corrosionEff = new AdvancedTimerBuff((int)BasicCalculation.BattleCondition.Corrosion,
            (int)amount, 8, 0, -1, 1,-1,8);
        
        atk.AddWithConditionAll(corrosionEff,200);
    }

    public void SummonSingleChild(GameObject prefab,Vector2 position,int hp,int atk,bool dragonPoint = true)
    {
        var enemy = SpawnEnemyMinon(prefab,position,hp,atk,position.x > 0?-1:1);
        var fx = Instantiate(GetProjectileOfFormatName("action05_1"),
            position - new Vector2(0,1.5f), Quaternion.identity);
        var stat = enemy.GetComponent<StatusManager>();
        minionInstanceList.Add(stat);

        StatusManager.StatusManagerVoidDelegate RemoveChild = null;
        RemoveChild = () =>
        {
            minionInstanceList.Remove(stat);
            stat.OnReviveOrDeath -= RemoveChild;
        };
        stat.OnReviveOrDeath += RemoveChild;

        var countDownUI = Instantiate(countdownUIPrefab,
            enemy.transform.position + new Vector3(0, 3), Quaternion.identity,
            RangedAttackFXLayer.transform);
        countDownUI.AddComponent<RelativePositionRetainer>().SetParent(enemy.transform);
        var countDownUIComponent = countDownUI.GetComponentInChildren<UI_RingSlider>();

        countDownUIComponent.maxValue = 30;
        countDownUIComponent.currentValue = 30;

        if (dragonPoint == false)
        {
            stat.GetComponent<DragonPointEnemy>()?.DisableAll();
        }
            
        stat.OnReviveOrDeath += () =>
        {
            Destroy(countDownUI);
        };

        countDownUIComponent.OnCountdownEnd += (ring) =>
        {
            if (stat.currentHp > 0)
            {
                stat.GetComponent<DragonPointEnemy>().DisableAll();
                (_behavior as H001_BehaviorTree).SetDestructionCount( 
                    Mathf.Clamp((_behavior as H001_BehaviorTree).destructionCount - 1,
                        0, 10));
                stat.currentHp = 0;
            }
        };
    }
    private void SummonChildrenOneByOne(object[] messages)
    {
        int index = 1;
        int childNum = Convert.ToInt32(messages[0]);

        for (int i = 0; i < childNum; i++)
        {
            if (minionInstanceList.Count > 8)
                break;

            int prefabID = Convert.ToInt32(messages[index++]);
            float positionX = (float)Convert.ToDouble(messages[index++]);
            float positionY = (float)Convert.ToDouble(messages[index++]);
            int hp = Convert.ToInt32(messages[index++]);
            int atk = Convert.ToInt32(messages[index++]);

            var enemy = SpawnEnemyMinon(prefabID == 1 ? minionPrefabA : minionPrefabB,
                new Vector2(positionX, positionY),
                hp, atk, ac.facedir);

            var fx = Instantiate(GetProjectileOfFormatName("action05_1"),
                new Vector2(positionX, positionY - 1.5f), Quaternion.identity);

            var stat = enemy.GetComponent<StatusManager>();
            minionInstanceList.Add(stat);

            StatusManager.StatusManagerVoidDelegate RemoveChild = null;
            RemoveChild = () =>
            {
                minionInstanceList.Remove(stat);
                stat.OnReviveOrDeath -= RemoveChild;
            };
            stat.OnReviveOrDeath += RemoveChild;

            var countDownUI = Instantiate(countdownUIPrefab,
                enemy.transform.position + new Vector3(0, 3), Quaternion.identity,
                RangedAttackFXLayer.transform);
            countDownUI.AddComponent<RelativePositionRetainer>().SetParent(enemy.transform);
            var countDownUIComponent = countDownUI.GetComponentInChildren<UI_RingSlider>();

            countDownUIComponent.maxValue = 30;
            countDownUIComponent.currentValue = 30;
            
            stat.OnReviveOrDeath += () =>
            {
                Destroy(countDownUI);
            };

            countDownUIComponent.OnCountdownEnd += (ring) =>
            {
                if (stat.currentHp > 0)
                {
                    stat.GetComponent<DragonPointEnemy>().DisableAll();
                    (_behavior as H001_BehaviorTree).SetDestructionCount( 
                        Mathf.Clamp((_behavior as H001_BehaviorTree).destructionCount - 1,
                            0, 10));
                    stat.currentHp = 0;
                }
            };


        }
    }

    private void GenerateProjectiles(GameObject prefab)
    {
        var container = InitContainer(false);
        
        List<GameObject> projList = new List<GameObject>();

        int count = (_behavior as H001_BehaviorTree).destructionCount < 5 ? 5 : 3;

        for (int i = 0; i < count; i++)
        {
            var proj = InstantiateRanged(prefab,
                new Vector3(transform.position.x +ac.facedir*2,
                    transform.position.y + 2),
                container,1);
            
            var projPrefab = proj.GetComponent<Projectile_H001_1>();
            projPrefab.attackFromEnemy.enemySource = gameObject;
            projPrefab.enemySource = gameObject;
            //projPrefab.attackFromEnemy.RemoveAllWithConditions();
            
            projPrefab.withConditions.Clear();

            if (type == 1)
            {
                projPrefab.withConditions.Add((
                    new TimerBuff((int)BasicCalculation.BattleCondition.Sleep,1,
                        Random.Range(4,6),1),100));
                //projPrefab.enemySource = gameObject;
            }
            else
            {
                projPrefab.withConditions.Add((
                    new TimerBuff((int)BasicCalculation.BattleCondition.Bog,1,
                        Random.Range(8,12),1),100));
                //projPrefab.enemySource = gameObject;
            }
            
            
            projList.Add(proj);
        }
        
        DOVirtual.DelayedCall(1.3f, () => LaunchProjectiles(projList), false);
        
    }
    
    private void LaunchProjectiles(List<GameObject> projList)
    {
        
        for(int i = 0; i < projList.Count; i++)
        {
            var proj = projList[i];
            var reflection = proj.GetComponent<ReflectionProjectile>();
            if (i == 1)
            {
                reflection.SetVelocity(new Vector2(10*ac.facedir,0));
            }
            else if (i == 2)
            {
                reflection.SetVelocity(new Vector2(ac.facedir*8.66f,5));
            }
            else if(i == 0)
            {
                reflection.SetVelocity(new Vector2(ac.facedir*8.66f,-5));
            }else if (i == 4)
            {
                reflection.SetVelocity(new Vector2(ac.facedir*9.65f,2.59f));
            }
            else
            {
                reflection.SetVelocity(new Vector2(ac.facedir*9.65f,-2.59f));
            }
            proj.GetComponent<Collider2D>().enabled = true;
        }
        
    }

    private void InstantiateWarpEffect(GameObject effPrefab)
    {
        var eff = Instantiate(effPrefab, transform.position, Quaternion.identity,RangedAttackFXLayer.transform);
        
    }

    private void LaserAttack(GameObject prefab,bool boosted)
    {
        var container = InitContainer(false);
        
        var fx = InstantiateRanged(prefab,
            new Vector3(transform.position.x + ac.facedir*2,
                transform.position.y + 2),
            container,ac.facedir,0);
        
        var atk = fx.GetComponent<AttackFromEnemy>();

        if (boosted)
        {
            atk.ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
            atk.attackInfo[0].dmgModifier[0] *= 1.25f;
            
            var atk2 = InstantiateRanged(prefab,
                new Vector3(transform.position.x + ac.facedir*2,
                    transform.position.y + 7),
                container,ac.facedir,0).GetComponent<AttackFromEnemy>();
            
            var atk3 = InstantiateRanged(prefab,
                new Vector3(transform.position.x + ac.facedir*2,
                    transform.position.y - 3),
                container,ac.facedir,0).GetComponent<AttackFromEnemy>();
            
            if (type == 1)
            {
                atk2.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Sleep,
                    1,6,
                    1),80);
                atk3.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Sleep,
                    1,6,
                    1),80);
            }
            else
            {
                atk2.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Bog,
                    1,12,
                    1),80);
                atk3.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Bog,
                    1,12,
                    1),80);
            }
            
            
            
            
        }

        if (type == 1)
        {
            atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Sleep,
                1,6,
                1),80);
        }
        else
        {
            atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Bog,
                1,12,
                1),80);
        }
        
        
    }

    private void PillarPunishment(Vector2 pos)
    {
        var chaser =
            InstantiateRanged(GetProjectileOfFormatName("action11_1", true),
                pos, InitContainer(false), 1).GetComponent<EnemyAttackHintBarTopDownChaser>();
        chaser.target = _behavior.targetPlayer;
        if ((_behavior as H001_BehaviorTree).destructionCount < 5)
        {
            chaser.SetMoveSpeedX(5.25f);
        }
    }


    private void AbsoluteTruth()
    {
        var forceAttack = InstantiateRanged(GetProjectileOfFormatName("action12_1"),
            Vector3.zero,
            InitContainer(false),1).GetComponent<ForcedAttackFromEnemy>();
        
        PurgedShapeShiftingOfViewer();

        forceAttack.BeforeAttackHit += PurgedShapeShiftingOfTarget;
        forceAttack.BeforeAttackHit += (atk, target) =>
        {
            //前置解除信赖之力
            target.GetComponent<StatusManager>()
                .RemoveTimerBuff((int)BasicCalculation.BattleCondition.PowerOfBonds, true);
        };

        Destroy(forceAttack.gameObject,5f);

    }

    private void GrantInvincible()
    {
        ac.SetHitSensor(false);
        _behavior.breakable = false;
        (_statusManager as SpecialStatusManager).ODLock = true;
        if ((_statusManager as SpecialStatusManager).currentBreak <= 0)
        {
            (_statusManager as SpecialStatusManager).currentBreak = 0.1f;
        }
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.Invincible,
            1, -1, 1,8202101,false);
        if (shieldInstance == null)
        {
            shieldInstance = Instantiate(GetProjectileOfFormatName("action06_1"),
                transform.position, Quaternion.identity, BuffFXLayer.transform);
        }
        else
        {
            shieldInstance.SetActive(true);
        }
        
        
    }
    
    private void PurgeInvincible()
    {
        ac.SetHitSensor(true);
        _behavior.breakable = true;
        (_statusManager as SpecialStatusManager).ODLock = false;
        _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.Invincible, true,
            8202101);
        if (shieldInstance != null)
        {
            shieldInstance.SetActive(false);
        }
        (_behavior as H001_BehaviorTree).SetBookFakeActive();
    }
    
    private void SummonElite(GameObject prefab, Vector2 pos)
    {
        _statusManager.ReliefAllAfflication();

        var enemy = Instantiate(prefab, pos, Quaternion.identity,
            BattleStageManager.Instance.EnemyLayer.transform);

        var fx = Instantiate(GetProjectileOfFormatName("action06_2"),
                pos - new Vector2(0,1), Quaternion.identity);

        var stat = enemy.GetComponent<StatusManager>();
        minionInstanceList.Add(stat);

        StatusManager.StatusManagerVoidDelegate RemoveChild = null;
        RemoveChild = () =>
        {
            minionInstanceList.Remove(stat);
            stat.OnReviveOrDeath -= RemoveChild;
        };
        stat.OnReviveOrDeath += RemoveChild;

        var countDownUI = Instantiate(countdownUIPrefab,
                enemy.transform.position + new Vector3(0, 6f), Quaternion.identity,
                RangedAttackFXLayer.transform);
        countDownUI.AddComponent<RelativePositionRetainer>().SetParent(enemy.transform);
        var countDownUIComponent = countDownUI.GetComponentInChildren<UI_RingSlider>();

        countDownUIComponent.maxValue = 45;
        countDownUIComponent.currentValue = 45;
            
        stat.OnReviveOrDeath += () =>
        {
            Destroy(countDownUI);
            PurgeInvincible();
        };

        countDownUIComponent.OnCountdownEnd += (ring) =>
        {
            if (stat.currentHp > 0)
            {
                stat.GetComponent<DragonPointEnemy>().DisableAll();
                (_behavior as H001_BehaviorTree).SetDestructionCount( 
                        Mathf.Clamp((_behavior as H001_BehaviorTree).destructionCount - 5,
                            0, 10));
                stat.currentHp = 0;
            }
        };


        
    }

    private void SummonCircleStart(int hp1, int hp2, int atk1, int atk2)
    {
        if(summonCircleInstanceLeft == null)
        {
            summonCircleInstanceLeft = Instantiate(summonCirclePrefab,
                new Vector3(-15, -1), Quaternion.identity,
                BattleStageManager.Instance.EnemyLayer.transform);

            var controller = summonCircleInstanceLeft.GetComponent<Projectile_H001_2>();
            controller.Initialize(this, hp1, atk1, hp2, atk2);
            controller.StartSpawn();
        }
        
        if(summonCircleInstanceRight == null)
        {
            summonCircleInstanceRight = Instantiate(summonCirclePrefab,
                new Vector3(15, -1), Quaternion.identity,
                BattleStageManager.Instance.EnemyLayer.transform);

            var controller = summonCircleInstanceRight.GetComponent<Projectile_H001_2>();
            controller.Initialize(this, hp1, atk1, hp2, atk2);
            controller.StartSpawn();
        }
    }
    
    private void KillAllMinions()
    {
        foreach (var minion in minionInstanceList)
        {
            if (minion.currentHp > 0)
            {
                minion.GetComponent<DragonPointEnemy>().DisableAll();
                minion.currentHp = 0;
                //minion.OnHPBelow0?.Invoke();
            }
        }
        
        if(summonCircleInstanceLeft != null)
        {
            Destroy(summonCircleInstanceLeft);
        }
        
        if(summonCircleInstanceRight != null)
        {
            Destroy(summonCircleInstanceRight);
        }
    }

    private GameObject GenerateAreaFX()
    {
        return Instantiate(GetProjectileOfFormatName("action10_1"),
            new Vector3(0,-1), Quaternion.identity, RangedAttackFXLayer.transform);
    }

    private void SpawnEnemies(int hp1, int hp2, int atk1, int atk2)
    {
        SummonSingleChild(minionPrefabA, new Vector2(-2, 0), hp1, atk1);
        SummonSingleChild(minionPrefabA, new Vector2(2, 0), hp1, atk1);
        SummonSingleChild(minionPrefabB, new Vector2(-10, 0), hp2, atk2);
        SummonSingleChild(minionPrefabB, new Vector2(10, 0), hp2, atk2);
        SummonSingleChild(minionPrefabA, new Vector2(-8, 6), hp1, atk1);
        SummonSingleChild(minionPrefabA, new Vector2(8, 6), hp1, atk1);
    }
    private UI_CountdownMinon SpawnEnergyBalls(int hp)
    {
        var ball1 = SpawnEnemyMinon(orbPrefab,
            new Vector3(-6, 0), hp);
        
        var ball2 = SpawnEnemyMinon(orbPrefab,
            new Vector3(6, 0), hp);
        
        var ball3 = SpawnEnemyMinon(orbPrefab,
            new Vector3(-6,6), hp);
        
        var ball4 = SpawnEnemyMinon(orbPrefab,
            new Vector3(6,6), hp);
        
        var UI = Instantiate(GetProjectileOfFormatName("action09_ui"),
            transform.position.SafePosition(new Vector2(0, 6f)), Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var UIRingSlider = UI.GetComponent<UI_RingSlider>();
        UIRingSlider.maxValue = 20;

        var UICountdownMinon = UI.GetComponent<UI_CountdownMinon>();
        UICountdownMinon.SetMaxCapacity(4);
        UICountdownMinon.AddNewStatusManager(ball1.GetComponent<StatusManager>());
        UICountdownMinon.AddNewStatusManager(ball2.GetComponent<StatusManager>());
        UICountdownMinon.AddNewStatusManager(ball3.GetComponent<StatusManager>());
        UICountdownMinon.AddNewStatusManager(ball4.GetComponent<StatusManager>());

        return UICountdownMinon;
        
    }
    
    private UI_CountdownMinon SpawnEnergyBallsOneByOne(int maxHP)
    {
        StatusManager.StatusManagerVoidDelegate handler = null;
        
        var UI = Instantiate(GetProjectileOfFormatName("action09_ui"),
            transform.position.SafePosition(new Vector2(2*ac.facedir, 5.5f)), Quaternion.identity,
            RangedAttackFXLayer.transform);
        var UIRingSlider = UI.GetComponent<UI_RingSlider>();
        UIRingSlider.maxValue = 20;
        UIRingSlider.currentValue = 20;

        var UICountdownMinon = UI.GetComponent<UI_CountdownMinon>();
        UICountdownMinon.SetMaxCapacity(4);

        int minionCount = 0;
        
        handler = () =>
        {
            //print($"MinionCount:{minionCount}, SliderValue:{UIRingSlider.currentValue}");
            if (minionCount < 4 && UIRingSlider.currentValue > 0)
            {
                DOVirtual.DelayedCall(0.3f,
                    () => 
                    {
                        if(UIRingSlider == null)
                            return;
                        
                        
                        var minion = SpawnEnemyMinon(orbPrefab,
                            transform.position.SafePosition(new Vector2(ac.facedir * 3, 1.5f)), maxHP,
                            1, 1);
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

    private void NihilBurstAOE(int ballLeftCount)
    {
        
        TimerBuff attackBuff = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
            100, 30, 100);
        TimerBuff defBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            100, 30, 100);

        foreach (var child in minionInstanceList)
        {
            child.ObtainHealOverTimeBuff(10,
                    15, true);
            if (ballLeftCount >= 2)
            {
                child.ObtainTimerBuff(defBuff,false);
            }
            if(ballLeftCount >= 3)
            {
                child.ObtainTimerBuff(attackBuff,false);
            }
        }
        
        TimerBuff nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, 15, 1);
        
        var fx = InstantiateRanged(GetProjectileOfFormatName("action09"),
            gameObject.RaycastedPosition(),
            InitContainer(false),1);

        var atk = fx.GetComponentInChildren<ForcedAttackFromEnemy>();
        atk.AddWithConditionAll(nihilDebuff,200);

        if (ballLeftCount >= 2 || IsBoosted)
        {
            atk.attackInfo[0].dmgModifier[0] *= 1.5f;
        }
        
    }

    private void AbsorbMinions(int extra = 0)
    {
        (_behavior as H001_BehaviorTree).SetDestructionCount( 
            Mathf.Clamp((_behavior as H001_BehaviorTree).destructionCount - extra,
                0, 10));
        foreach (var minion in minionInstanceList)
        {
            if(minion.currentHp > 0)
            {
                minion.currentHp = 0;
                minion.GetComponent<DragonPointEnemy>().DisableAll();
                (_behavior as H001_BehaviorTree).SetDestructionCount( 
                    Mathf.Clamp((_behavior as H001_BehaviorTree).destructionCount - 1,
                        0, 10));
            }
        }
    }

    private void ElementSwitch()
    {
        if (type == 1)
        {
            type = 2;
            Instantiate(GetProjectileOfFormatName("action13_2",true),
                transform.position,Quaternion.identity, RangedAttackFXLayer.transform);
        }
        else if (type == 2)
        {
            type = 1;
            Instantiate(GetProjectileOfFormatName("action13_1",true),
                transform.position,Quaternion.identity, RangedAttackFXLayer.transform);
        }
    }
    

}
