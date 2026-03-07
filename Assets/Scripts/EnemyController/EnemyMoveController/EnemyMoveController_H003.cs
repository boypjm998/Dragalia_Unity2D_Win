using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMoveController_H003 : EnemyMoveManager
{
    [SerializeField] private GameObject partRenderer;
    [SerializeField] private GameObject partBreakFXPrefab;

    [SerializeField] private GameObject orbPrefab;
    [SerializeField] private GameObject antiOrbPrefab;
    private Dictionary<GameObject, StatusManager> _orbInstances = new();
    
    [SerializeField] private GameObject weakPointPrefab;
    
    [SerializeField] private GameObject faceOfNothingnessRenderer;
    [SerializeField] private GameObject faceOfSadnessRenderer;
    [SerializeField] private GameObject faceOfAngerRenderer;

    [SerializeField] private int MinionHPUnit = 8000;

    public int Face { get; private set; } = 0;
    private bool abilitiesCleared = false;

    private List<StatusManager.SpecialEffectFunc> _playerAtkAbilities = new();
    private List<StatusManager.SpecialEffectFunc> _playerCritRateAbilities = new();
    private List<StatusManager.SpecialEffectFunc> _playerCritDmgAbilities = new();
    
    private List<StatusManager.SpecialEffectFunc> _playerRecoveryAbilities = new();
    
    private List<StatusManager.SpecialEffectFunc> _playerDefAbilities = new();
    private List<StatusManager.SpecialEffectFunc> _playerDamageCutAbilities = new();

    private Tween _angerBuffTween;
    private bool _sadnessActionOn = false;
    private bool _initiated = false;

    enum VoiceGroup
    {
        Transform,
        HPBelow70,
        HPBelow40,
        Defeated,
        Intro,
        Skill1,
        Allranged,
        SadnessFace,
        AngerFace,
        Rage,
        FailToForce
    }

    protected override void Start()
    {
        base.Start();
        _voiceController = GetComponentInChildren<VoiceControllerEnemy>();
        GetAllAnchors();
        foreach (var anchor in _navigateAnchorSensors)
        {
            _orbInstances.Add(anchor.gameObject,null);
        }

        _statusManager.OnReviveOrDeath += () =>
        {
            if (_angerBuffTween != null) _angerBuffTween.Kill();
            DestroyAllRecordedOrbs();
        };


    }

    public void AddDrasticForceEffectToStatusManager(StatusManager statusManager)
    {
        var dmgCutEffects = statusManager.GetInvocationList(AbilityCalculation.ProductArea.DMGCUT);

        foreach (var effectFunc in dmgCutEffects)
        {
            var method = ((StatusManager.SpecialEffectFunc)effectFunc);
            
            if (method == Ability.DrasticForceEffect)
            {
                return;
            }
        }

        print("Add Drastic Force Effect to Status Manager");
        statusManager.AddEffectFunction(Ability.DrasticForceEffect, AbilityCalculation.ProductArea.DMGCUT);
        
    }
    
    public IEnumerator PartBreak()
    {
        ac.OnAttackEnter(999);
        ac.SetCounter(false);
        
        yield return new WaitForSeconds(0.5f);
        _behavior.breakable = false;
        _statusManager.ImmuneToAllControlAffliction = true;
        anim.Play("part_break");
        //ac.OnHurtEnter();
        BattleEffectManager.Instance.PlayReviveSoundEffect();
        Instantiate(partBreakFXPrefab,transform.position + new Vector3(0,1f),
            Quaternion.identity,RangedAttackFXLayer.transform);

        var defDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefDebuff,
            10, -1, 1, 8202301);
        defDebuff.dispellable = false;
        //(_statusManager as SpecialStatusManager).counterModifier = 0.2f;
        _statusManager.ObtainTimerBuff(defDebuff);

        partRenderer.SetActive(false);
        

        yield return new WaitForSeconds(0.1f);
       

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        //StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);

        yield return new WaitForSeconds(1f);
        _behavior.breakable = true;
        _statusManager.ImmuneToAllControlAffliction = false;
        
        QuitAttack();
    }


    public IEnumerator H003_Action01(int summonType)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("buff_1");
        
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(1f);

        if (summonType == 1)
        {
            SummonOrbTypeA();
        }else if (summonType == 2)
        {
            SummonOrbTypeB();
        }else if (summonType == 3)
        {
            SummonOrbTypeC();
        }else if (summonType == 4)
        {
            SummonOrbTypeD();
        }else if (summonType == 5)
        {
            SummonOrbTypeE();
        }else if (summonType == 6)
        {
            SummonOrbTypeF();
        }

        anim.Play("buff_3");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }
    
    
    public IEnumerator H003_Action02()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("H003_Action02");
        
        anim.Play("buff_1");

        yield return new WaitForSeconds(1f);

        NihilAOE();

        anim.Play("buff_3");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }
    
    
    public IEnumerator H003_Action03()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);

        anim.Play("buff_1");

        yield return new WaitForSeconds(1f);

        TargetingMine();
        Invoke("TargetingMine",1);
        Invoke("TargetingMine",2);
        Invoke("TargetingMine",3);

        anim.Play("buff_3");
        
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        //StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }
    
    
    
    public IEnumerator H003_Action04()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);

        anim.Play("buff_1");

        yield return new WaitForSeconds(1f);

        var randomDirection = Random.Range(0, 2) == 0 ? 1 : -1;
        WaveSweeping(randomDirection);

        anim.Play("buff_3");
        
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        //StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }
    
    
    public IEnumerator H003_Action05()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);

        anim.Play("buff_1");

        var hint = EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
            transform.position + new Vector3(0, 8), MeeleAttackFXLayer.transform,
            12, Vector2.zero, false, true, 3.5f, 0.15f);
        
        yield return new WaitForSeconds(3f);

        anim.Play("buff_3");
        
        yield return new WaitForSeconds(0.5f);
        AroundAttack();
        

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        //StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }
    
    public IEnumerator H003_Action06()
    {
        yield return _canAction;
        ac.OnAttackEnter(100);
        StageCameraController.SwitchOverallCamera();
        bossBanner?.PrintSkillName("H003_Action06");

        anim.Play("buff_1");

        if (Face == 1)
        {
            _voiceController?.BroadCastSpecificVoice((int)VoiceGroup.Skill1, 0);
        }else if (Face == 2)
        {
            _voiceController?.BroadCastSpecificVoice((int)VoiceGroup.Skill1, 1);
        }
        else
        {
            _voiceController?.BroadCastSpecificVoice((int)VoiceGroup.Skill1, 2);
        }

        var hintL = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector2(0,BattleStageManager.Instance.mapBorderB),
            RangedAttackFXLayer.transform,
            new Vector2(30, 16), Vector2.zero, true, 1, 7.8f, 
            90, 0.5f,true,
            true);
        ThreeLineSummon();

        yield return new WaitForSeconds(7.5f);

        anim.Play("buff_3");

        yield return new WaitForSeconds(0.5f);
        
        ThreeLineMain();
        StageCameraController.SwitchMainCamera();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        //StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }
    
    
    public IEnumerator H003_Action07()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);

        anim.Play("buff_1");

        var fanType = Random.Range(0, 2) == 0 ? 1 : 2;
        FanshapedAttackHint(fanType);

        yield return new WaitForSeconds(2.5f);

        anim.Play("buff_3");
        
        yield return new WaitForSeconds(0.5f);
        
        FanshapedAttack(fanType);
        

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        //StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// Strength Suppression
    /// </summary>
    /// <returns></returns>
    public IEnumerator H003_Action08()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        //StageCameraController.SwitchOverallCamera();
        bossBanner?.PrintSkillName("H003_Action08");

        anim.Play("buff_1");
        
        yield return new WaitForSeconds(1f);
        
        StrengthSuppression();

        anim.Play("buff_3");

        yield return new WaitForSeconds(0.5f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        //StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }
    
    
    /// <summary>
    /// 梵天灭相
    /// </summary>
    /// <returns></returns>
    public IEnumerator H003_Action09()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        StageCameraController.SwitchOverallCamera();
        bossBanner?.PrintSkillName("H003_Action09");
        
        _voiceController?.BroadCastMyVoice((int)VoiceGroup.Allranged);
        ac.SetHitSensor(false);

        var prefab = BattleEffectManager.Instance.GetWeakPointIndicator();

        var enemy = SpawnEnemyMinon(weakPointPrefab,
            new Vector3(0, 9f), weakPointPrefab.GetComponent<StatusManager>().maxBaseHP,
            9999, 1);

        var enemyStat = enemy.GetComponent<StatusManager>();
        AddDrasticForceEffectToStatusManager(enemyStat);
        var rangeAttackTween = RandomRangeAttack(3,0.5f);
        
        var uiRingSlider = 
            SpawnCountDownUI(prefab, 
                new Vector3(0, 13f), 
                25, 1);
        var uiMinionCount = uiRingSlider.GetComponent<UI_CountdownMinon>();
        uiMinionCount.AddNewStatusManager(enemyStat);

        var enemyGeneratorTween = GenerateOrbsRandomly( MinionHPUnit );

        yield return new WaitUntil(()=>uiRingSlider.currentValue <= 0 || uiMinionCount.Value <= 0);
        
        enemyGeneratorTween?.Kill();
        rangeAttackTween?.Kill();
        
        if (uiMinionCount.Value > 0)
        {
            uiMinionCount.KillAllMinons();
            WeakPointAOE();
            
            anim.Play("charge_3");

            yield return null;
            Destroy(uiMinionCount.gameObject);
        }
        else
        {
            Destroy(uiMinionCount.gameObject);
            ac.SetHitSensor(true);
            anim.Play("part_break");
            yield return new WaitForSeconds(3);
        }
        
        StageCameraController.SwitchMainCamera();
        
        yield return new WaitForSeconds(0.5f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        
        anim.Play("idle");
        QuitAttack();
    }
    
    
    /// <summary>
    /// Face
    /// </summary>
    /// <returns></returns>
    public IEnumerator H003_Action10(int face)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        StageCameraController.SwitchMainCameraFollowObject(GetAnchoredSensorOfName("M3"));
        bossBanner?.PrintSkillName("H003_Action10");
        ac.SetHitSensor(false);

        //anim.Play("buff_1");
        
        yield return new WaitForSeconds(1f);

        if (Face == 1)
        {
            
            faceOfNothingnessRenderer.SetActive(false);
            BattleStageManager.Instance.RemoveFieldAbility((int)BasicCalculation.EnemyAbility.FaceOfNothingness);
        }
        else if (Face == 2)
        {
            
            faceOfSadnessRenderer.SetActive(false);
            BattleStageManager.Instance.RemoveFieldAbility((int)BasicCalculation.EnemyAbility.FaceOfSadness);
        }
        else if (Face == 3)
        {
            
            faceOfAngerRenderer.SetActive(false);
            BattleStageManager.Instance.RemoveFieldAbility((int)BasicCalculation.EnemyAbility.FaceOfAnger);
        }
        
        
        FaceSwap(face);
        Instantiate(GetProjectileOfFormatName($"action10_{face}"),
            transform.position + new Vector3(0,16), Quaternion.identity, RangedAttackFXLayer.transform);

        if (face == 1)
        {
            _voiceController.BroadCastSpecificVoice((int)VoiceGroup.Skill1,0);
            faceOfNothingnessRenderer.SetActive(true);
            BattleStageManager.Instance.AddFieldAbility((int)BasicCalculation.EnemyAbility.FaceOfNothingness);
        }
        else if (face == 2)
        {
            _voiceController.BroadCastMyVoice((int)VoiceGroup.SadnessFace);
            faceOfSadnessRenderer.SetActive(true);
            BattleStageManager.Instance.AddFieldAbility((int)BasicCalculation.EnemyAbility.FaceOfSadness);
        }
        else if (face == 3)
        {
            _voiceController.BroadCastMyVoice((int)VoiceGroup.AngerFace);
            faceOfAngerRenderer.SetActive(true);
            BattleStageManager.Instance.AddFieldAbility((int)BasicCalculation.EnemyAbility.FaceOfAnger);
        }

        SetFaceAction();

        if (_behavior.difficulty > 1)
        {
            SummonNegativeOrbs();
            UI_DialogDisplayer.Instance?.
                EnqueueDialogShared(10101,
                    20132,BattleEffectManager.Instance?.notteHintClips[1]);
        }
        

        yield return new WaitForSeconds(1.5f);
        ac.SetHitSensor(true);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
        anim.Play("idle");
        
        //yield return new WaitForSeconds(1f);
        
        QuitAttack();
    }
    
    
    public IEnumerator H003_Action11()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        StageCameraController.SwitchOverallCamera();
        bossBanner?.PrintSkillName("H003_Action11");
        anim.Play("buff_1");
        
        ForbiddenArea();
        _voiceController?.BroadCastMyVoice((int)VoiceGroup.Allranged);
        Invoke("SummonOrbForbidden", 5);
        
        yield return new WaitForSeconds(4.5f);

        anim.Play("buff_3");

        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 尽灭曼荼罗+降魔
    /// </summary>
    /// <returns></returns>
    public IEnumerator H003_Action12(bool downgrade)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        StageCameraController.SwitchOverallCamera();
        bossBanner?.PrintSkillName("H003_Action14");

        yield return new WaitForSeconds(1);
        
        anim.Play("buff_1");
        
        DestroyAllRecordedOrbs();
        
        yield return null;
        
        SummonOrbGroups(downgrade);
        
        yield return new WaitForSeconds(1f);
        
        Convergence(13);
        bossBanner?.PrintSkillName("H003_Action13");

        yield return new WaitForSeconds(1);
        
        MandalaChaser();
        
        yield return new WaitForSeconds(7f);

        anim.Play("buff_3");
        if (Face == 2)
        {
            _voiceController.BroadCastSpecificVoice((int)VoiceGroup.Skill1,1);
        }else if (Face == 3)
        {
            _voiceController.BroadCastSpecificVoice((int)VoiceGroup.Rage,0);
        }
        else
        {
            _voiceController.BroadCastSpecificVoice((int)VoiceGroup.Rage,0);
        }

        yield return new WaitForSeconds(0.5f);
        
        bossBanner?.PrintSkillName("H003_Action12");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// 尽灭曼荼罗
    /// </summary>
    /// <returns></returns>
    public IEnumerator H003_Action13()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);

        yield return new WaitForSeconds(1);
        
        anim.Play("buff_1");

        MandalaChaser();
        
        yield return new WaitForSeconds(7f);

        anim.Play("buff_3");
        if (Face == 2)
        {
            _voiceController.BroadCastSpecificVoice((int)VoiceGroup.Skill1,1);
        }else if (Face == 3)
        {
            _voiceController.BroadCastSpecificVoice((int)VoiceGroup.Rage,0);
        }
        else
        {
            _voiceController.BroadCastSpecificVoice((int)VoiceGroup.Rage,0);
        }

        yield return new WaitForSeconds(0.5f);
        
        bossBanner?.PrintSkillName("H003_Action12");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// Karmic Scale Revealed
    /// </summary>
    /// <returns></returns>
    public IEnumerator H003_Action15(int type)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(1);
        bossBanner?.PrintSkillName("H003_Action15");
        
        anim.Play("buff_1");

        yield return new WaitForSeconds(1.5f);

        anim.Play("buff_3");
        if (type == 1)
        {
            Instantiate(GetProjectileOfFormatName("action15_1", true),
                new Vector3(0, 11.5f), Quaternion.identity, RangedAttackFXLayer.transform);
        }
        else
        {
            Instantiate(GetProjectileOfFormatName("action15_2", true),
                new Vector3(0, 11.5f), Quaternion.identity, RangedAttackFXLayer.transform);
        }

        yield return new WaitForSeconds(0.5f);
        
        _statusManager.HPRegenImmediatelyWithoutRandomDirectly(_statusManager, 100000);
        _statusManager.HPRegenImmediatelyWithoutRandomDirectly(
            _behavior.viewerPlayer.GetComponent<PlayerStatusManager>(), DrasticForce.Instance.StackCount * 1000);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// Punishment
    /// </summary>
    /// <returns></returns>
    public IEnumerator H003_Action16(int type)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(1);
        bossBanner?.PrintSkillName("H003_Action16");
        
        anim.Play("buff_1");

        yield return new WaitForSeconds(1f);
        
        PunishimentEffect();
        
        yield return new WaitForSeconds(0.5f);

        anim.Play("buff_3");

        yield return new WaitForSeconds(0.5f);

        var extraMsg = PunishmentAttack(type == 2);
        
        if (extraMsg.punishPlayer == false)
        {
            yield return new WaitForSeconds(0.1f);
            anim.Play("part_break");
            _voiceController.BroadCastMyVoice((int)VoiceGroup.FailToForce);

            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        GenerateOrbBonus(extraMsg.extraOrb);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }
    
    
    
    

    /// <summary>
    /// 两个Projectile，位于L2和R2
    /// </summary>
    private void SummonOrbTypeA()
    {
        var orb1 = SummonOrb("L2", MinionHPUnit * 2,true);
        var orb2 = SummonOrb("R2", MinionHPUnit * 2,true);
        
        orb1.InitOrb(transform, Projectile_H003_1.AttackType.Projectiles, false, 5,
            20, 1, 2);
        orb2.InitOrb(transform, Projectile_H003_1.AttackType.Projectiles, false, 5,
            20, 1, 2);
    }
    
    /// <summary>
    /// 4个Round，位于L3和R3和M1,M3
    /// </summary>
    private void SummonOrbTypeB()
    {
        var orb1 = SummonOrb("L3", MinionHPUnit * 2,true);
        var orb2 = SummonOrb("R3", MinionHPUnit * 2,true);
        var orb3 = SummonOrb("M1", MinionHPUnit * 2);
        var orb4 = SummonOrb("M3", MinionHPUnit * 2,true);
        
        orb1.InitOrb(transform, Projectile_H003_1.AttackType.Round, true, 2,
            20, 1, 5);
        orb2.InitOrb(transform, Projectile_H003_1.AttackType.Round, true, 2,
            20, 1, 5);
        orb3.InitOrb(transform, Projectile_H003_1.AttackType.Round, true, 2,
            20, 1, 5);
        orb4.InitOrb(transform, Projectile_H003_1.AttackType.Round, true, 2,
            20, 1, 5);
    }
    
    /// <summary>
    /// 4个SingleProjectile，位于L4和R4，L1和R1
    /// </summary>
    private void SummonOrbTypeC()
    {
        var orb1 = SummonOrb("L4", MinionHPUnit * 2,true);
        var orb2 = SummonOrb("R4", MinionHPUnit * 2,true);
        var orb3 = SummonOrb("L1", MinionHPUnit * 2,true);
        var orb4 = SummonOrb("R1", MinionHPUnit * 2,true);
        
        orb1.InitOrb(transform, Projectile_H003_1.AttackType.SingleProjectile, false, 5,
            20, 1, 2);
        orb2.InitOrb(transform, Projectile_H003_1.AttackType.SingleProjectile, false, 5,
            20, 1, 2);
        orb3.InitOrb(transform, Projectile_H003_1.AttackType.SingleProjectile, false, 5,
            20, 1, 2);
        orb4.InitOrb(transform, Projectile_H003_1.AttackType.SingleProjectile, false, 5,
            20, 1, 2);
        
    }
    
    /// <summary>
    /// Projectiles, T2和T3
    /// </summary>
    private void SummonOrbTypeD()
    {
        var orb1 = SummonOrb("T2", MinionHPUnit * 2,true);
        var orb2 = SummonOrb("T3", MinionHPUnit * 2,true);
        
        orb1.InitOrb(transform, Projectile_H003_1.AttackType.Projectiles, false, 5,
            20, 1, 2);
        orb2.InitOrb(transform, Projectile_H003_1.AttackType.Projectiles, false, 5,
            20, 1, 2);
    }
    
    /// <summary>
    /// 两个Projectiles，位于T1和T4, 附加一个ROund，位于M2
    /// </summary>
    private void SummonOrbTypeE()
    {
        var orb1 = SummonOrb("T1", MinionHPUnit * 2,true);
        var orb2 = SummonOrb("T4", MinionHPUnit * 2,true);
        var orb3 = SummonOrb("M2", MinionHPUnit * 2,true);
        
        orb1.InitOrb(transform, Projectile_H003_1.AttackType.Projectiles, false, 5,
            20, 1, 2);
        orb2.InitOrb(transform, Projectile_H003_1.AttackType.Projectiles, false, 5,
            20, 1, 2);
        orb3.InitOrb(transform, Projectile_H003_1.AttackType.Round, true, 2,
            20, 1, 5);
        
    }
    
    /// <summary>
    /// 2个SingleProjectile，位于L1和R1，附加2个Round，位于L3和R3
    /// </summary>
    private void SummonOrbTypeF()
    {
        var orb1 = SummonOrb("L1", MinionHPUnit * 2);
        var orb2 = SummonOrb("R1", MinionHPUnit * 2);
        var orb3 = SummonOrb("L3", MinionHPUnit * 2,true);
        var orb4 = SummonOrb("R3", MinionHPUnit * 2,true);
        
        orb1.InitOrb(transform, Projectile_H003_1.AttackType.SingleProjectile, false, 5,
            20, 1, 2);
        orb2.InitOrb(transform, Projectile_H003_1.AttackType.SingleProjectile, false, 5,
            20, 1, 2);
        orb3.InitOrb(transform, Projectile_H003_1.AttackType.Round, true, 2,
            20, 1, 5);
        orb4.InitOrb(transform, Projectile_H003_1.AttackType.Round, true, 2,
            20, 1, 5);
        
        
    }
    
    
    
    private void TargetingMine()
    {
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            _behavior.targetPlayer.RaycastedPosition(), RangedAttackFXLayer.transform,
            new Vector2(3, 5), Vector2.zero, false, 0, 1.4f, 
            90, 0.5f,true,
            false);
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action03"),
            _behavior.targetPlayer.RaycastedPosition(),
            InitContainer(false),1);
        
        proj.GetComponent<AttackFromEnemy>().
            AddWithConditionAll
            (new TimerBuff((int)BasicCalculation.BattleCondition.Blindness,1,12,1),
                100);
        
    }

    private void WaveSweeping(int direction)
    {
        var prefab = GetProjectileOfFormatName("action04");
        var proj = InstantiateRanged(prefab,
            direction == 1
                ? new Vector3(BattleStageManager.Instance.mapBorderL,BattleStageManager.Instance.mapBorderB) 
                : new Vector3(BattleStageManager.Instance.mapBorderR,BattleStageManager.Instance.mapBorderB),
            InitContainer(false),1);
        
        var boxCollider2D = proj.GetComponent<BoxCollider2D>();
        var particleSystem = proj.transform.GetChild(0).GetComponent<ParticleSystem>();
        
        var colliderSizeX = boxCollider2D.size.x;
        var colliderOffsetY = boxCollider2D.offset.y;
        
        //startSize的Y在5秒内从4加到12
        //var main = particleSystem.main;
        var size = particleSystem.main.startSizeY;

        DOTween.To(() => size.constant, x => size.constant = x, 12, 4).
            OnUpdate(() =>
            {
                var main = particleSystem.main;
                main.startSizeY = size;
            }).SetEase(Ease.Linear).SetDelay(1);

        DOTween.To(() => boxCollider2D.size, x => boxCollider2D.size = x,
            new Vector2(colliderSizeX, 15), 4).SetEase(Ease.Linear).SetDelay(1);
        
        DOTween.To(() => boxCollider2D.offset, x => boxCollider2D.offset = x,
            new Vector2(0, colliderOffsetY*3), 4).SetEase(Ease.Linear).SetDelay(1);
        
        proj.transform.DOMoveX(direction == 1 ?
                BattleStageManager.Instance.mapBorderR : BattleStageManager.Instance.mapBorderL, 5).
            SetEase(Ease.Linear);
    }

    private void AroundAttack()
    {
        var prefab = GetProjectileOfFormatName("action05");
        
        var proj = InstantiateMeele(prefab,
            transform.position + new Vector3(0,8),
            InitContainer(true));
    }

    private void ThreeLineSummon()
    {
        var orbR = SummonOrb("LineR", (int)(MinionHPUnit * 3.2f), false);
        var orbL = SummonOrb("LineL", (int)(MinionHPUnit * 3.2f), false);
        
        orbR.InitOrb(transform, Projectile_H003_1.AttackType.None, false, 999, 8);
        orbL.InitOrb(transform, Projectile_H003_1.AttackType.None, false, 999, 8);
        
        var hintL = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector2(-16,BattleStageManager.Instance.mapBorderB),
            RangedAttackFXLayer.transform,
            new Vector2(30, 16), Vector2.zero, false, 1, 7.8f, 
            90, 0.5f,true,
            false);
        
        var hintR = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector2(16,BattleStageManager.Instance.mapBorderB),
            RangedAttackFXLayer.transform,
            new Vector2(30, 16), Vector2.zero, false, 1, 7.8f, 
            90, 0.5f,true,
            false);
        
        var prefabAttack1 = GetProjectileOfFormatName("action06_1");


        var tweenL = DOVirtual.DelayedCall(8, () =>
        {
            InstantiateRanged(prefabAttack1,
                new Vector3(-16,BattleStageManager.Instance.mapBorderB),
                InitContainer(false),1);
        }, false);
        
        var tweenR = DOVirtual.DelayedCall(8, () =>
        {
            InstantiateRanged(prefabAttack1,
                new Vector3(16,BattleStageManager.Instance.mapBorderB),
                InitContainer(false),1);
        }, false);
        
        var orbStatL = orbL.GetComponent<StatusManager>();
        var orbStatR = orbR.GetComponent<StatusManager>();
        
        StatusManager.StatusManagerVoidDelegate cancleDelegateL = () =>
        {
            tweenL?.Kill();
            Destroy(hintL);
        };
        
        StatusManager.StatusManagerVoidDelegate cancleDelegateR = () =>
        {
            tweenR?.Kill();
            Destroy(hintR);
        };
        
        orbStatL.OnReviveOrDeath += cancleDelegateL;
        orbStatR.OnReviveOrDeath += cancleDelegateR;


    }

    private void ThreeLineMain()
    {
        var prefab = GetProjectileOfFormatName("action06_2");
        
        var proj = InstantiateMeele(prefab,
            new Vector3(0,BattleStageManager.Instance.mapBorderB),
            InitContainer(true));
    }


    private void FanshapedAttackHint(int fanType)
    {
        
        
        GenerateWarningPrefab("action07", transform.position + new Vector3(0, 8),
            Quaternion.Euler(0,0,fanType == 1 ? 0 : 45), RangedAttackFXLayer.transform);
        
        GenerateWarningPrefab("action07", transform.position + new Vector3(0, 8),
            Quaternion.Euler(0,0,fanType == 1 ? 90 : 135), RangedAttackFXLayer.transform);
        
        GenerateWarningPrefab("action07", transform.position + new Vector3(0, 8),
            Quaternion.Euler(0,0,fanType == 1 ? 180 : -135), RangedAttackFXLayer.transform);
        
        GenerateWarningPrefab("action07", transform.position + new Vector3(0, 8),
            Quaternion.Euler(0,0,fanType == 1 ? -90 : -45), RangedAttackFXLayer.transform);
        
    }

    private void FanshapedAttack(int fanType)
    {
        var prefab = GetProjectileOfFormatName("action07");
        
        var container = InitContainer(false);
        
        var proj1 = InstantiateDirectionalRanged(prefab, transform.position + new Vector3(0, 8),
            container,1,fanType == 1 ? 0 : 45);
        
        var proj2 = InstantiateDirectionalRanged(prefab, transform.position + new Vector3(0, 8),
            container,1,fanType == 1 ? 90 : 135);
        
        var proj3 = InstantiateDirectionalRanged(prefab, transform.position + new Vector3(0, 8),
            container,1,fanType == 1 ? 180 : -135);
        
        var proj4 = InstantiateDirectionalRanged(prefab, transform.position + new Vector3(0, 8),
            container,1,fanType == 1 ? -90 : -45);
        
    }


    private void DestroyAllRecordedOrbs()
    {
        //遍历_orbInstances，如果其value不为空，则销毁,并将其value置空
        //不能用foreach，因为foreach会在遍历过程中改变字典
        
        var keys = _orbInstances.Keys.ToList();
        
        for (int i = 0; i < keys.Count; i++)
        {
            var key = keys[i];
            if (_orbInstances[key] != null)
            {
                Destroy(_orbInstances[key].gameObject);
                _orbInstances[key] = null;
            }
        }

    }

    private Projectile_H003_1 SummonOrb(string positionName, int hp, bool dashBoost = false)
    {
        var position = GetAnchoredSensorOfName(positionName);
        
        if (position == null)
        {
            Debug.LogError("No such position: " + positionName);
            return null;
        }

        if (_orbInstances[position] != null)
        {
            Destroy(_orbInstances[position].gameObject);
            _orbInstances[position] = null;
        }

        var orb = SpawnEnemyMinon(orbPrefab, position.transform.position,
            hp, 1, 1);
        
        var statusManager = orb.GetComponent<StatusManager>();
        var moveController = orb.GetComponent<Projectile_H003_1>();

        if (dashBoost)
        {
            moveController.SetProtectionFXOn();
            statusManager.AddEffectFunction
                (Ability.DashAttackEffectExtraAttack,AbilityCalculation.ProductArea.DMGCUT);
        }

        _orbInstances[position] = statusManager;
        
        return moveController;




    }
    
    private Projectile_H003_1 SummonOrb(Vector2 position, int hp, bool dashBoost = false)
    {
        var orb = SpawnEnemyMinon(orbPrefab, position,
            hp, 1, 1);
        var statusManager = orb.GetComponent<StatusManager>();
        var moveController = orb.GetComponent<Projectile_H003_1>();

        if (dashBoost)
        {
            moveController.SetProtectionFXOn();
            statusManager.AddEffectFunction
                (Ability.DashAttackEffectExtraAttack,AbilityCalculation.ProductArea.DMGCUT);
        }

        return moveController;
        
    }
    
    private Projectile_H003_1 SummonAntiOrb(string positionName, int hp)
    {
        var position = GetAnchoredSensorOfName(positionName);
        
        if (position == null)
        {
            Debug.LogError("No such position: " + positionName);
            return null;
        }
        if (_orbInstances[position] != null)
        {
            Destroy(_orbInstances[position].gameObject);
            _orbInstances[position] = null;
        }
        
        var orb = SpawnEnemyMinon(antiOrbPrefab, position.transform.position,
            hp, 1, 1);
        var statusManager = orb.GetComponent<StatusManager>();
        var moveController = orb.GetComponent<Projectile_H003_1>();
        orb.GetComponent<EnemyController>().notTarget = true;
        
        moveController.IsNegative = true;
        
        _orbInstances[position] = statusManager;

        moveController.SetProtectionFXOn();
        statusManager.AddEffectFunction
            (Ability.DashAttackEffectExtraAttack,AbilityCalculation.ProductArea.DMGCUT);
        
        
        

        return moveController;
        
    }
    
    private Projectile_H003_1 SummonAntiOrb(Vector2 position, int hp, bool dashBoost = false)
    {
        var orb = SpawnEnemyMinon(antiOrbPrefab, position,
            hp, 1, 1);
        var statusManager = orb.GetComponent<StatusManager>();
        var moveController = orb.GetComponent<Projectile_H003_1>();
        moveController.IsNegative = true;

        if (dashBoost)
        {
            moveController.SetProtectionFXOn();
            statusManager.AddEffectFunction
                (Ability.DashAttackEffectExtraAttack,AbilityCalculation.ProductArea.DMGCUT);
        }

        return moveController;
        
    }
    
    protected void StrengthSuppression()
    {
        Instantiate(GetProjectileOfFormatName("action08"), Vector3.zero, Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        DrasticForce.Instance?.RemoveAllDrasticForce();
    }
    
    private void PunishimentEffect()
    {
        Instantiate(GetProjectileOfFormatName("action16_1"), Vector3.zero, Quaternion.identity,
            RangedAttackFXLayer.transform);
    }

    private (int extraOrb, bool punishPlayer) PunishmentAttack(bool downgrade)
    {
        var drasticForceLevel = DrasticForce.Instance.StackCount;

        bool punishPlayer = false;
        int punishLevel = 0;
        
        if (downgrade && drasticForceLevel > 8)
        {
            punishPlayer = true;
            punishLevel = drasticForceLevel - 8;
        }
        else if (!downgrade && drasticForceLevel < 8)
        {
            punishPlayer = true;
            punishLevel = 8 - drasticForceLevel;
        }

        int orbBonus = 0;
        if (drasticForceLevel < 8)
        {
            orbBonus = 8 - drasticForceLevel;
        }

        orbBonus = Mathf.Clamp(orbBonus, 0, 6);
        
        var prefab = GetProjectileOfFormatName("action16_2");

        var atk = InstantiateRanged(prefab,
            punishPlayer ? _behavior.targetPlayer.transform.position : transform.position,
            InitContainer(false), 1);

        if (!punishPlayer)
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                CineMachineOperator.Instance.CamaraShake(18,.2f);
                BattleStageManager.Instance.CauseIndirectDamage(_statusManager,
                    (int)(_statusManager.maxBaseHP * 0.05f), false, true);
            }, false);
        }
        else
        {
            CineMachineOperator.Instance.CamaraShake(18,.2f);
            atk.GetComponent<ForcedAttackFromEnemy>().target = _behavior.targetPlayer;
            atk.GetComponent<ForcedAttackFromEnemy>().attackInfo[0].constDmg[0] = punishLevel * 500;
        }

        return (orbBonus,punishPlayer);

    }

    private void GenerateOrbBonus(int count)
    {
        if(count <= 0)
            return;
        
        if (count == 1)
        {
            var orb = SummonOrb(new Vector2(0,9), 1);
            orb.InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
        }else if (count == 2)
        {
            SummonOrb(new Vector2(-3,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
            SummonOrb(new Vector2(3,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
        }else if (count == 3)
        {
            SummonOrb(new Vector2(-3,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
            SummonOrb(new Vector2(0,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
            SummonOrb(new Vector2(3,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
        }else if (count == 4)
        {
            SummonOrb(new Vector2(-3,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
            SummonOrb(new Vector2(-5,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
            SummonOrb(new Vector2(5,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
            SummonOrb(new Vector2(3,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
        }else if (count == 5)
        {
            SummonOrb(new Vector2(-3, 9), 1).InitOrb(transform, Projectile_H003_1.AttackType.None, true, 1, 10);
            SummonOrb(new Vector2(-5, 9), 1).InitOrb(transform, Projectile_H003_1.AttackType.None, true, 1, 10);
            SummonOrb(new Vector2(0, 9), 1).InitOrb(transform, Projectile_H003_1.AttackType.None, true, 1, 10);
            SummonOrb(new Vector2(5, 9), 1).InitOrb(transform, Projectile_H003_1.AttackType.None, true, 1, 10);
            SummonOrb(new Vector2(3, 9), 1).InitOrb(transform, Projectile_H003_1.AttackType.None, true, 1, 10);
        }
        else
        {
            SummonOrb(new Vector2(-3,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
            SummonOrb(new Vector2(-5,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
            SummonOrb(new Vector2(5,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
            SummonOrb(new Vector2(3,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
            SummonOrb(new Vector2(-7,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
            SummonOrb(new Vector2(7,9), 1).InitOrb(transform,Projectile_H003_1.AttackType.None,true,1,10);
        }


    }
    
    private Tween GenerateOrbsRandomly(int maxHP)
    {
        Tween _tween = null;

        List<Vector2> positionList = new List<Vector2>()
        {
            new(8,9),new(7,9),new(6,9),new(4,9),new(9,9),new(3,9),
            new(8,9),new(7,9),new(6,9),new(4,9),new(9,9),new(3,9)
        };

        positionList.Shuffle();
        int index = 0;

        _tween = DOVirtual.DelayedCall(3,() =>
        {
            int sign = index % 2 == 0 ? 1 : -1;
            var orbController = 
                SummonOrb(new Vector2(sign*positionList[index].x,positionList[index].y), maxHP);
            orbController.InitOrb(transform, Projectile_H003_1.AttackType.None,
                true, 1,5.5f);
            
            index++;
        },false).OnComplete(() =>
        {
            if(index < 12)
                _tween.Restart();
        });

        return _tween;

    }
    
    private Tween RandomRangeAttack(float delay, float rng)
    {
        List<Vector2> offsetList = new List<Vector2>()
        {
            new(0, 0), new(2, 2), new(-2, 2), new(3, 0), new(-3, 0),
            new(3, 3), new(-3, 3), new(5, 5), new(-5, 5),
            new(0, 3), new(4, 4), new(-4, 4), new(6, 0), new(-6, 0),
            new(0, 6), new(6, 6), new(-6, 6)
        };

        offsetList.Shuffle();

        int index = 0;
        Tween tween = null;

        tween = DOVirtual.DelayedCall(delay, () =>
        {
            var hintTime = Random.Range(delay, delay + rng);
            var size = Random.Range(0.4f, 1f);

            var position = _behavior.targetPlayer.transform.position + (Vector3)offsetList[index];

            var avoidable = index % 2 == 0
                ? false
                : true;

            EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
                position,
                RangedAttackFXLayer.transform, 5 * size, Vector2.zero,
                avoidable, true,
                hintTime, 0.08f * size, 0.4f, true, false);

            
            
            DOVirtual.DelayedCall(hintTime, () =>
            {
                var proj = InstantiateRanged(GetProjectileOfFormatName("action09_1"),
                    position, InitContainer(false), 1);
                if (!avoidable)
                {
                    proj.GetComponent<AttackFromEnemy>().
                        ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
                }else
                {
                    proj.GetComponent<AttackFromEnemy>().
                        ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Red);
                }

                proj.transform.localScale *= size;

            }, false);
            
            index++;

        }, false).OnComplete(() =>
        {
            if (index <= 16)
            {
                tween.Restart();
            }
        });

        return tween;

    }

    private void ForbiddenArea()
    {
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar
        (ac, new Vector3(-18f, BattleStageManager.Instance.mapBorderB - 1),
            RangedAttackFXLayer.transform, new Vector2(30, 12), Vector2.zero,
            false, 1, 5, 90, 1, true, false);
        
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar
        (ac, new Vector3(18f, BattleStageManager.Instance.mapBorderB - 1),
            RangedAttackFXLayer.transform, new Vector2(30, 12), Vector2.zero,
            false, 1, 5, 90, 1, true, false);
        
        var prefab = GetProjectileOfFormatName("action11");
        
        DOVirtual.DelayedCall(5, () =>
        {
            var proj = InstantiateRanged(prefab,
                new Vector3(-18f, BattleStageManager.Instance.mapBorderB - 1), InitContainer(false), 1);
            proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
                (new TimerBuff((int)BasicCalculation.BattleCondition.Blindness,
                    1,5,1),30);
            
        }, false);
        
        DOVirtual.DelayedCall(5, () =>
        {
            var proj = InstantiateRanged(prefab,
                new Vector3(18f, BattleStageManager.Instance.mapBorderB - 1), InitContainer(false), 1);
            proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
            (new TimerBuff((int)BasicCalculation.BattleCondition.Blindness,
                1,5,1),30);
        }, false);
    }

    private void SummonOrbForbidden()
    {
        var orb1 = SummonOrb("T2", MinionHPUnit * 2,true);
        var orb2 = SummonOrb("T3", MinionHPUnit * 2,true);
        var orb3 = SummonOrb("B2", MinionHPUnit * 2,true);
        var orb4 = SummonOrb("B3", MinionHPUnit * 2,true);
        
        orb1.InitOrb(transform, Projectile_H003_1.AttackType.Projectiles, false, 5,
            22, 2, 2);
        orb2.InitOrb(transform, Projectile_H003_1.AttackType.Projectiles, false, 5,
            22, 2, 2);
        orb3.InitOrb(transform, Projectile_H003_1.AttackType.Projectiles, false, 5,
            22, 2, 2);
        orb4.InitOrb(transform, Projectile_H003_1.AttackType.Projectiles, false, 5,
            22, 2, 2);
        
    }

    private void SummonOrbGroups(bool downgrade)
    {
        int lifeTime = 15;
        
        Vector2[] groupR = new Vector2[]
        {
            new(6, 9), new(18, 9), new(12, 2.5f), new(12, 15)
        };
        
        Vector2[] groupL = new Vector2[]
        {
            new(-6, 9), new(-18, 9), new(-12, 2.5f), new(-12, 15)
        };
        
        var leftIsDowngrade = Random.Range(0, 2) == 0;
        List<Projectile_H003_1> orbListNormal = new List<Projectile_H003_1>();
        List<Projectile_H003_1> orbListAnti = new List<Projectile_H003_1>();

        if (leftIsDowngrade)
        {
            foreach (var pos in groupL)
            {
                orbListAnti.Add(SummonAntiOrb(pos, MinionHPUnit * 10));
            }
            
            foreach (var pos in groupR)
            {
                orbListNormal.Add(SummonOrb(pos, MinionHPUnit * 10));
            }
        }
        else
        {
            foreach (var pos in groupL)
            {
                orbListNormal.Add(SummonOrb(pos, MinionHPUnit * 10));
            }
            
            foreach (var pos in groupR)
            {
                orbListAnti.Add(SummonAntiOrb(pos, MinionHPUnit * 10));
            }
        }

       
        foreach (var orb in orbListNormal)
        {
            orb.InitOrb(transform, Projectile_H003_1.AttackType.SingleProjectile, 
                downgrade ? false : true,
                5, lifeTime, 1, 2);
        }

        foreach (var orb in orbListAnti)
        {
            orb.InitOrb(transform, Projectile_H003_1.AttackType.SingleProjectile, 
                downgrade ? true : false,
                5, lifeTime, 2, 2);
        }
        
    }

    private void MandalaChaser()
    {
        float chaseTime = 4.5f;
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer, chaseTime);
        
        var hint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar
            (ac, new Vector3(_behavior.targetPlayer.transform.position.x,
                    BattleStageManager.Instance.mapBorderB),
                RangedAttackFXLayer.transform, new Vector2(30, 24), Vector2.zero,
                false, 1, chaseTime + 3, 90,
                1f, true, false);

        var chaser = hint.AddComponent<EnemyAttackHintBarTopDownChaser>();
        chaser.SetHardLock(false);
        chaser.SetMoveSpeedY(0);
        chaser.SetMoveSpeedX(15);
        chaser.target = _behavior.targetPlayer;
        chaser.SetLockTime(chaseTime);
        
        DOVirtual.DelayedCall(chaseTime + 3.5f, () =>
        {
            var proj = InstantiateRanged(GetProjectileOfFormatName("action12_2"),
                new Vector3(chaser.transform.position.x,
                    BattleStageManager.Instance.mapBorderB), InitContainer(false), 1);
        }, false);

    }

    private void Convergence(float chaseTime)
    {
        var hint = GenerateWarningPrefab("action12_1", _behavior.targetPlayer.transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform, 1);

        hint.GetComponent<EnemyAttackHintBarCircle>().warningTime = chaseTime;
        hint.GetComponent<EnemyAttackHintBarShine>().warningTime = chaseTime - 0.1f;
        
        var chaser = hint.GetComponent<EnemyAttackHintBarChaser>();
        
        chaser.SetLockTime(chaseTime);
        chaser.target = _behavior.targetPlayer;
        
        DOVirtual.DelayedCall(chaseTime, () =>
        {
            
            var container = Instantiate
            (GetProjectileOfFormatName("action12_1", true),
                hint.transform.position, Quaternion.identity,RangedAttackFXLayer.transform);
            
            if(hint != null)
                Destroy(hint);

            var attackContainer = container.GetComponent<ContributoryAttackContainer>();
            
            attackContainer.SetEnemySource(gameObject);

            //var forcedAttackFromEnemy = attackContainer.GetComponentInChildren<ForcedAttackFromEnemy>();
            
            Action<StatusManager> handler = null;
            bool isComplete = false;

            handler = (stat) =>
            {
                // if(isComplete)
                //     return;
                
                if(stat is PartStatusManager)
                    return;
                
                BattleStageManager.Instance.CauseIndirectDamage(stat, 
                    (int)(MinionHPUnit * 
                          11),
                    true, true);
                
                isComplete = true;
            };

            attackContainer.SetAction(handler);

        }, false);



    }

    private void NihilAOE()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action01"),
            new Vector3(0,BattleStageManager.Instance.mapBorderB),InitContainer(false),1);

        var nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, 5, 1);
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(nihilDebuff,100);
    }

    private void WeakPointAOE()
    {
        var fx = InstantiateRanged(GetProjectileOfFormatName("action09_2"),
            transform.position + new Vector3(0,8), InitContainer(false), 1);
    }

    private void SummonNegativeOrbs()
    {
        var orbL = SummonAntiOrb("B1", MinionHPUnit * 9);
        var orbR = SummonAntiOrb("B4", MinionHPUnit * 9);
        
        orbL.InitOrb(transform, Projectile_H003_1.AttackType.Round, 
            true, 10, 120, 5, 5);
        orbR.InitOrb(transform, Projectile_H003_1.AttackType.Round, 
            true, 10, 120, 5, 5);
    }


    
    
    

    public void FaceSwap(int newFace)
    {
        if (Face == 1)
        {
            ResetPlayerStrAbilities();
        }else if (Face == 2)
        {
            ResetPlayerRecoveryAbilities();
        }else if (Face == 3)
        {
            ResetPlayerDefAbilities();
        }

        if (newFace == 1)
        {
            ClearPlayerStrAbilities();
        }else if (newFace == 2)
        {
            ClearPlayerRecoveryAbilities();
        }else if (newFace == 3)
        {
            ClearPlayerDefAbilities();
        }
        
        Face = newFace;



    }
    

    private void ClearPlayerStrAbilities()
    {
        abilitiesCleared = true;
        
        var playerStat = BattleStageManager.Instance.GetPlayer().GetComponent<PlayerStatusManager>();

        var originPlayerAtkAbilities = playerStat.GetInvocationList(AbilityCalculation.ProductArea.ATK);
        var originPlayerCritRateAbilities = playerStat.GetInvocationList(AbilityCalculation.ProductArea.CRITRATE);
        var originPlayerCritDmgAbilities = playerStat.GetInvocationList(AbilityCalculation.ProductArea.CRITDMG);

        _playerAtkAbilities = new List<StatusManager.SpecialEffectFunc>
            (playerStat.GetInvocationList(AbilityCalculation.ProductArea.ATK));
        _playerCritRateAbilities = new List<StatusManager.SpecialEffectFunc>
            (playerStat.GetInvocationList(AbilityCalculation.ProductArea.CRITRATE));
        _playerCritDmgAbilities = new List<StatusManager.SpecialEffectFunc>
            (playerStat.GetInvocationList(AbilityCalculation.ProductArea.CRITDMG));

        for (int i = originPlayerAtkAbilities.Count - 1; i >= 0; i--)
        {
            var delegateTarget = originPlayerAtkAbilities[i].Target;
            if (delegateTarget == null)
            {
                var delegateDeclaringType = originPlayerAtkAbilities[i].Method.DeclaringType;
                if (delegateDeclaringType.FullName == "GameMechanics.Ability")
                {
                    playerStat.RemoveEffectFunc(originPlayerAtkAbilities[i],AbilityCalculation.ProductArea.ATK);
                }
                
            }
        }
        
        for (int i = originPlayerCritRateAbilities.Count - 1; i >= 0; i--)
        {
            var delegateTarget = originPlayerCritRateAbilities[i].Target;
            if (delegateTarget == null)
            {
                var delegateDeclaringType = originPlayerCritRateAbilities[i].Method.DeclaringType;
                if (delegateDeclaringType.FullName == "GameMechanics.Ability")
                {
                    playerStat.RemoveEffectFunc(originPlayerCritRateAbilities[i],AbilityCalculation.ProductArea.CRITRATE);
                }
                
            }
        }
        
        for (int i = originPlayerCritDmgAbilities.Count - 1; i >= 0; i--)
        {
            var delegateTarget = originPlayerCritDmgAbilities[i].Target;
            if (delegateTarget == null)
            {
                var delegateDeclaringType = originPlayerCritDmgAbilities[i].Method.DeclaringType;
                print(delegateDeclaringType.FullName);
                if (delegateDeclaringType.FullName == "GameMechanics.Ability")
                {
                    playerStat.RemoveEffectFunc(originPlayerCritDmgAbilities[i],AbilityCalculation.ProductArea.CRITDMG);
                }
                
            }
        }
        

    }

    private void ResetPlayerStrAbilities()
    {
        var playerStat = BattleStageManager.Instance.GetPlayer().GetComponent<PlayerStatusManager>();

        if (abilitiesCleared)
        {
            playerStat.UpdateEffectFuncs(new List<StatusManager.SpecialEffectFunc>(_playerAtkAbilities), AbilityCalculation.ProductArea.ATK);
            playerStat.UpdateEffectFuncs(new List<StatusManager.SpecialEffectFunc>(_playerCritRateAbilities), AbilityCalculation.ProductArea.CRITRATE);
            playerStat.UpdateEffectFuncs(new List<StatusManager.SpecialEffectFunc>(_playerCritDmgAbilities), AbilityCalculation.ProductArea.CRITDMG);
            abilitiesCleared = false;
        }
        
    }

    private void ClearPlayerRecoveryAbilities()
    {
        abilitiesCleared = true;
        
        var playerStat = BattleStageManager.Instance.GetPlayer().GetComponent<PlayerStatusManager>();
        
        var originPlayerRecoveryAbilities = playerStat.GetInvocationList(AbilityCalculation.ProductArea.RCV);
        
        _playerRecoveryAbilities = new List<StatusManager.SpecialEffectFunc>
            (playerStat.GetInvocationList(AbilityCalculation.ProductArea.RCV));

        for (int i = originPlayerRecoveryAbilities.Count - 1; i >= 0; i--)
        {
            var delegateTarget = originPlayerRecoveryAbilities[i].Target;
            if (delegateTarget == null)
            {
                var delegateDeclaringType = originPlayerRecoveryAbilities[i].Method.DeclaringType;
                if (delegateDeclaringType.FullName == "GameMechanics.Ability")
                {
                    playerStat.RemoveEffectFunc(originPlayerRecoveryAbilities[i],AbilityCalculation.ProductArea.RCV);
                }
                
            }
        }
    }
    
    private void ResetPlayerRecoveryAbilities()
    {
        var playerStat = BattleStageManager.Instance.GetPlayer().GetComponent<PlayerStatusManager>();

        if (abilitiesCleared)
        {
            playerStat.UpdateEffectFuncs(new List<StatusManager.SpecialEffectFunc>(_playerRecoveryAbilities), AbilityCalculation.ProductArea.RCV);
            abilitiesCleared = false;
        }
        
    }
    
    private void ClearPlayerDefAbilities()
    {
        abilitiesCleared = true;
        
        var playerStat = BattleStageManager.Instance.GetPlayer().GetComponent<PlayerStatusManager>();
        
        var originPlayerDefAbilities = playerStat.GetInvocationList(AbilityCalculation.ProductArea.DEF);
        var originPlayerDamageCutAbilities = playerStat.GetInvocationList(AbilityCalculation.ProductArea.DMGCUT);
        
        _playerDefAbilities = new List<StatusManager.SpecialEffectFunc>
            (playerStat.GetInvocationList(AbilityCalculation.ProductArea.DEF));
        _playerDamageCutAbilities = new List<StatusManager.SpecialEffectFunc>
            (playerStat.GetInvocationList(AbilityCalculation.ProductArea.DMGCUT));

        for (int i = originPlayerDefAbilities.Count - 1; i >= 0; i--)
        {
            var delegateTarget = originPlayerDefAbilities[i].Target;
            if (delegateTarget == null)
            {
                var delegateDeclaringType = originPlayerDefAbilities[i].Method.DeclaringType;
                if (delegateDeclaringType.FullName == "GameMechanics.Ability")
                {
                    playerStat.RemoveEffectFunc(originPlayerDefAbilities[i],AbilityCalculation.ProductArea.DEF);
                }
                
            }
        }
        
        for (int i = originPlayerDamageCutAbilities.Count - 1; i >= 0; i--)
        {
            var delegateTarget = originPlayerDamageCutAbilities[i].Target;
            if (delegateTarget == null)
            {
                var delegateDeclaringType = originPlayerDamageCutAbilities[i].Method.DeclaringType;
                if (delegateDeclaringType.FullName == "GameMechanics.Ability")
                {
                    playerStat.RemoveEffectFunc(originPlayerDamageCutAbilities[i],AbilityCalculation.ProductArea.DMGCUT);
                }
                
            }
        }
        
    }
    
    private void ResetPlayerDefAbilities()
    {
        var playerStat = BattleStageManager.Instance.GetPlayer().GetComponent<PlayerStatusManager>();

        if (abilitiesCleared)
        {
            playerStat.UpdateEffectFuncs(new List<StatusManager.SpecialEffectFunc>(_playerDefAbilities), AbilityCalculation.ProductArea.DEF);
            playerStat.UpdateEffectFuncs(new List<StatusManager.SpecialEffectFunc>(_playerDamageCutAbilities), AbilityCalculation.ProductArea.DMGCUT);
            abilitiesCleared = false;
        }
        
    }

    private void SetFaceAction()
    {
        if (!_initiated)
        {
            SetPlayerSadnessEffect();
        }
        
        
        if (Face != 3)
        {
            _angerBuffTween?.Kill();
        }
        else
        {
            _angerBuffTween = DOVirtual.DelayedCall(5, () =>
            {
                _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
                    20, 10);
            }, false).OnComplete(()=>_angerBuffTween?.Restart());
        }

        if (Face == 2)
        {
            _sadnessActionOn = true;
        }
        else
        {
            _sadnessActionOn = false;
        }
        
        
    }
    
    public void SadnessCorrosionEffect(StatusManager playerStat, StatusManager enemyStat,
        AttackBase atk, float dmg)
    {
        if (_sadnessActionOn)
        {
            if (atk.attackType != BasicCalculation.AttackType.OTHER)
            {
                playerStat.ObtainUnstackableTimerBuff((int)BasicCalculation.BattleCondition.Corrosion,
                    500,-1,0, 5,0);
            }
        }
        
    }

    private void SetPlayerSadnessEffect()
    {
        var player = BattleStageManager.Instance.GetPlayer();
        var playerStat = player.GetComponent<PlayerStatusManager>();
        playerStat.OnTakeDirectDamageFrom += SadnessCorrosionEffect;
        
    }

    private void OnDestroy()
    {
        var player = BattleStageManager.Instance.GetPlayer();
        var playerStat = player.GetComponent<PlayerStatusManager>();
        playerStat.OnTakeDirectDamageFrom -= SadnessCorrosionEffect;
    }
}
