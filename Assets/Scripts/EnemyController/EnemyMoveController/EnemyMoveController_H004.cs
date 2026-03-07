using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Image = UnityEngine.UI.Image;

public class EnemyMoveController_H004 : EnemyMoveManager
{
    [SerializeField] private GameObject partRenderer;
    [SerializeField] private GameObject partBreakFXPrefab;
    [SerializeField] private GameObject playerTuneFXPrefab;

    [SerializeField] private GameObject hellTuneInstance;
    [SerializeField] private GameObject heavenTuneInstance;
    
    [SerializeField] private GameObject warpFXAppearPrefab;
    [SerializeField] private GameObject warpFXDisappearPrefab;
    [SerializeField] private GameObject warpFXDisappearPrefab2;

    [SerializeField] private GameObject playerMelodyUIPrefab;
    [FormerlySerializedAs("hellUIImage")] [SerializeField] private Sprite hellUISprite;
    [FormerlySerializedAs("heavenUIImage")] [SerializeField] private Sprite heavenUISprite;

    private bool _dissonance = false;

    public int CurrentTune { get; private set; } = 0;
    public int TuneCountdown { get; private set; }

    private GameObject _melodyFXInstance;
    private GameObject _playerTuneFXInstance;
    private int _playerMelodyType = 0;

    private int _damageCounter = 0;
    private bool _isFirstMelody = true;

    private Image _uiInstance;

    private TimerBuff _partBreakDebuff = new((int)BasicCalculation.BattleCondition.DamageDown,
        5, -1, 1, 8202401);
    private TimerBuff _partBreakDebuff2 = new((int)BasicCalculation.BattleCondition.Vulnerable,
        5, -1, 1, 8202401);
    
    private enum MyVoiceGroup
    {
        /// <summary>
        /// Corrosion
        /// </summary>
        SkillGroup1 = 4,
        /// <summary>
        /// Buff
        /// </summary>
        SkillGroup2 = 5,
        /// <summary>
        /// Scattered / Rings
        /// </summary>
        SkillGroup3 = 6,
        /// <summary>
        /// WarpAttack
        /// </summary>
        SkillGroup4 = 7,
        /// <summary>
        /// BING WU协奏曲
        /// </summary>
        SkillGroup5 = 8,
        /// <summary>
        /// FOLLOW
        /// </summary>
        SkillGroup6 = 9,
        /// <summary>
        /// Encore
        /// </summary>
        SkillGroup7 = 10,
        DpsCheck = 11,
        FailToForce = 12,
        SkillGroup8 = 13
    }
    

    protected override void Start()
    {
        base.Start();
        BattleStageManager.Instance.specialEventTriggered += OnMelodyHit;
        _statusManager.OnHPDecrease += OnReceiveDamageEvent;
        _statusManager.OnBuffEventDelegate += OnAfflictionEvent;
        _statusManager.OnAfflictionGuarded += OnAfflictionEvent;
        _statusManager.OnBuffExpiredEventDelegate += CheckDissonanceAvability;
        _statusManager.OnBuffDispelledEventDelegate += CheckDissonanceAvability;
        _statusManager.OnTakeDirectDamageFrom += DissonanceReflection;
        _statusManager.OnHPBelow0 += HidePlayerMelodyUI;
        
        GetAllAnchors();
    }

    private void DissonanceReflection(StatusManager src, StatusManager tar, AttackBase atk, float dmg)
    {
        if(!_dissonance)
            return;
        
        if (CurrentTune == _playerMelodyType && atk.attackType != BasicCalculation.AttackType.SKILL)
        {
            src.RemoveAllConditionOfType((int)BasicCalculation.BattleCondition.Dissonance);
            BattleEffectManager.Instance.SpawnEffect(gameObject,BasicCalculation.BattleCondition.Dispell);
        }
        else
        {
            BattleStageManager.Instance.CauseIndirectDamage(tar, (int)(dmg * 0.1f), true);
        }
    }

    private void CheckDissonanceAvability(BattleCondition condition)
    {
        if (condition.buffID == (int)BasicCalculation.BattleCondition.Dissonance)
        {
            _dissonance = false;
            BattleStageManager.Instance.InvokeEnemyAbilityEvent
            ((int)BasicCalculation.EnemyAbility.Dissonance,
                new EnemyAbilityIconEvent(0),_statusManager);
        }
    }
    
    /// <summary>
    /// Hell:1 ; Heaven:2
    /// </summary>
    private void OnMelodyHit(int type)
    {
        if (type == 1)
        {
            _playerMelodyType = 1;
            if (_playerTuneFXInstance)
            {
                _playerTuneFXInstance.transform.GetChild(0).gameObject.SetActive(true);
                _playerTuneFXInstance.transform.GetChild(1).gameObject.SetActive(false);
            }

            if (_uiInstance)
            {
                _uiInstance.sprite = hellUISprite;
            }
        }
        else if (type == 2)
        {
            _playerMelodyType = 2;
            if (_playerTuneFXInstance)
            {
                _playerTuneFXInstance.transform.GetChild(0).gameObject.SetActive(false);
                _playerTuneFXInstance.transform.GetChild(1).gameObject.SetActive(true);
            }
            if (_uiInstance)
            {
                _uiInstance.sprite = heavenUISprite;
            }
        }
            
    }

    private void HidePlayerMelodyUI()
    {
        _uiInstance?.gameObject.SetActive(false);
    }

    public IEnumerator H004_Action01()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);

        int type;
        bool swap = true;
        if (CurrentTune <= 0)
        {
            type = Random.Range(1, 3);
            if (type == 1)
            {
                bossBanner?.PrintSkillName("H004_Action01A");
            }
            else
            {
                bossBanner?.PrintSkillName("H004_Action01B");
            }
        }
        else
        {
            if (TuneCountdown > 0)
            {
                swap = false;
                type = CurrentTune;
                bossBanner?.PrintSkillName("H004_Action01C");
            }
            else
            {
                type = CurrentTune == 1 ? 2 : 1;
                bossBanner?.PrintSkillName("H004_Action01");
            }
        }
        
        anim.Play("charge_enter");

        yield return new WaitForSeconds(1);
        
        SetSelfMelody(type);
        if (swap)
        {
            InitPlayerMelody();
        }
        else
        {
            var atk = FrigidElegance(false).GetComponent<ForcedAttackFromEnemy>();
            atk.attackInfo[0].constDmg[0] = 2000;
            BuffSelf();
        }
        
        
        anim.Play("charge_exit");

        yield return null;

        if (_isFirstMelody)
        {
            _isFirstMelody = false;
            StageCameraController.SwitchMainCameraFollowObject(GetAnchoredSensorOfName("TopM"));
            UI_DialogDisplayer.Instance?.
                EnqueueDialogShared(10101,20141,BattleEffectManager.Instance?.notteHintClips[0]);
            yield return new WaitForSeconds(2);
            StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
        }
        

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        QuitAttack();

    }

    /// <summary>
    /// Nihil
    /// </summary>
    /// <returns></returns>
    public IEnumerator H004_Action02()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("H004_Action02");
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("buff_1");

        yield return new WaitForSeconds(0.85f);

        anim.Play("buff_3");
        
        yield return new WaitForSeconds(0.5f);
        NihilAOE();

        yield return null;
        anim.Play("buff_5");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        //StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// Corrosion
    /// </summary>
    /// <returns></returns>
    public IEnumerator H004_Action03()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H004_Action03");


        yield return new WaitForSeconds(0.5f);
        
        anim.Play("buff_1");
        
        if(_voiceController != null)
            _voiceController.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup1);
        
        yield return new WaitForSeconds(0.85f);
        
        anim.Play("buff_3");
        
        yield return new WaitForSeconds(0.5f);
        
        CorrosionFog(1000 + (_behavior.difficulty - 1) * 250);
        
        anim.Play("buff_5");

        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    
    /// <summary>
    /// Around
    /// </summary>
    /// <returns></returns>
    public IEnumerator H004_Action04()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(0.5f);

        var col1 = ((Vector2)transform.position + new Vector2(ac.facedir * 3,0)).RaycastedPlatform();
        var col2 = ((Vector2)transform.position + new Vector2(ac.facedir * 6,0)).RaycastedPlatform();

        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector3(transform.position.x + ac.facedir * 3, col1.bounds.max.y),
            RangedAttackFXLayer.transform,
            new Vector2(6, 6), new Vector2(-1, 0), true, 0, 1, 90,
            0.4f);
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector3(transform.position.x + ac.facedir * 12, col2.bounds.max.y),
            RangedAttackFXLayer.transform,
            new Vector2(6, 6), new Vector2(-1, 0), false, 0, 1, 90,
            0.4f);

        anim.Play("buff_1");

        yield return new WaitForSeconds(0.8f);
        
        anim.Play("buff_3");
        
        yield return new WaitForSeconds(0.2f);
        
        ac.SetCounter(true);
        ac.SetKBRes(100);
        WaveForward(new Vector3(transform.position.x + ac.facedir * 3, col1.bounds.max.y),
            new Vector3(transform.position.x + ac.facedir * 12, col1.bounds.max.y));
        var raycastedPos = gameObject.RaycastedPosition();
        
        yield return new WaitForSeconds(0.5f);
        
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, raycastedPos + new Vector2(15,0),
            RangedAttackFXLayer.transform,
            new Vector2(6, 6), new Vector2(-1, 0), false, 0, 1, 90,
            0.4f);
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, raycastedPos + new Vector2(-15,0), 
            RangedAttackFXLayer.transform,
            new Vector2(6, 6), new Vector2(-1, 0), false, 0, 1, 90,
            0.4f);
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, raycastedPos, 
            RangedAttackFXLayer.transform,
            new Vector2(6, 18), new Vector2(-1, 0), true, 0, 1, 90,
            0.4f);
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("buff_5");

        yield return null;
        
        WaveAround();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    
    public IEnumerator H004_Action05(bool teleport = true)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H004_Action05");

        if (teleport)
        {
            WarpFXIn();
            ac.SetGravityScale(0);

            yield return new WaitForSeconds(0.2f);
        
            DisappearRenderer();
            transform.position = GetAnchoredSensorOfName("MiddleM").transform.position;
        
            yield return null;
        
            WarpFXOut(false);
            StageCameraController.SwitchOverallCamera();
        
            yield return new WaitForSeconds(0.2f);
        
            AppearRenderer();
            ac.SetHitSensor(true);
        }
        else
        {
            StageCameraController.SwitchOverallCamera();
            yield return new WaitForSeconds(0.2f);
        }
        

        anim.Play("charge_enter");

        yield return new WaitForSeconds(1);

        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup3);

        var batR = SummonBatEffect(GetAnchoredSensorOfName("MiddleR").transform.position);
        var batL = SummonBatEffect(GetAnchoredSensorOfName("MiddleL").transform.position);
        
        WhirlRingsHint(batL.transform.position, batR.transform.position);

        yield return new WaitForSeconds(2);
        
        WhirlRingsAttack(batL.transform.position, 
            batR.transform.position, 1);
        
        yield return new WaitForSeconds(1f);
        
        WhirlRingsAttack(batL.transform.position, 
            batR.transform.position, 2);
        
        yield return new WaitForSeconds(1f);
        
        WhirlRingsAttack(batL.transform.position, 
            batR.transform.position, 3);

        anim.Play("charge_exit");

        yield return null;
        
        StageCameraController.SwitchMainCamera();
        Destroy(batL);
        Destroy(batR);
        ac.ResetGravityScale();
        WarpFXIn();

        yield return new WaitForSeconds(0.2f);
        
        DisappearRenderer();
        transform.position = gameObject.RaycastedPosition() + new Vector2(0,1);
        
        yield return null;
        
        WarpFXOut();
        
        yield return new WaitForSeconds(0.2f);
        AppearRenderer();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        QuitAttack();

    }

    /// <summary>
    /// Encore
    /// </summary>
    /// <returns></returns>
    public IEnumerator H004_Action06(bool encore = false)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);

        bossBanner?.PrintSkillName("H004_Action06");

        WarpFXIn();
        ac.SetGravityScale(0);

        yield return new WaitForSeconds(0.2f);

        DisappearRenderer();
        transform.position = GetAnchoredSensorOfName("MiddleM").transform.position;
        
        var batR = SummonBatEffect(new Vector2(10,10));
        var batL = SummonBatEffect(new Vector2(-10,10));
        var batB = SummonBatEffect(new Vector2(0, 0));
        var batT = SummonBatEffect(new Vector2(0, 20));

        yield return null;

        WarpFXOut(false);
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(0.2f);

        AppearRenderer();
        ac.SetHitSensor(true);

        anim.Play("buff_1");

        //声明
        var typeArray = new int[3] { 1, 2, 3 };
        var typeList = typeArray.Shuffle().ToList();

        if (!encore)
        {
            typeList.RemoveAt(0);
        }
        
        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup7);
        
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < typeList.Count; i++)
        {
            yield return new WaitForSeconds(EncorePrepareHint(typeList[i]));
            
            EncorePrepareAttack(typeList[i]);
            if (i == 0)
            {
                anim.Play("buff_3");
            }
            
            yield return new WaitForSeconds(0.75f);
        }
        
        //添加点什么特效
        ShineEffect(batL,batT,batB,batR);

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < typeList.Count; i++)
        {
            EncoreAccentHint(typeList[i],batL,batR,batB,batT);
            yield return new WaitForSeconds(0.5f);
            EncoreAccentAttack(typeList[i],batL,batR,batB,batT);
            yield return new WaitForSeconds(1.5f);
        }
        
        anim.Play("buff_5");
        StageCameraController.SwitchMainCamera();

        yield return null;
        
        Destroy(batL);
        Destroy(batB);
        Destroy(batT);
        Destroy(batR);
        ac.ResetGravityScale();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");
        QuitAttack();

    }

    /// <summary>
    /// BUFF
    /// </summary>
    /// <returns></returns>
    public IEnumerator H004_Action07()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);

        anim.Play("charge_enter");

        yield return new WaitForSeconds(1);
        
        BuffSelf();
        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup2);
        anim.Play("charge_exit");

        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        QuitAttack();

    }

    /// <summary>
    /// Scatter
    /// </summary>
    public IEnumerator H004_Action08()
    {
        yield return _canAction;
        ac.OnAttackEnter();
        ac.SetGravityScale(0);
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("buff_1");
        
        yield return new WaitForSeconds(0.5f);
        
        var startAngle = Random.Range(0, 2) == 1 ? 30 : 0;
        var avoidablity = (_behavior as H004_BehaviorTree).CapeHasBroken;
        ScatteredWaterballsHint(avoidablity, startAngle);

        yield return new WaitForSeconds(2.3f);
        
        anim.Play("buff_3");

        yield return new WaitForSeconds(0.2f);
        
        ScatteredWaterballsAttack(avoidablity,6.5f, startAngle - 30);
        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup3);
        
        startAngle = (startAngle == 0 ? 30 : 0);
        ScatteredWaterballsHint(avoidablity, startAngle);
        
        yield return new WaitForSeconds(2.3f);
        
        anim.Play("buff_5");
        
        yield return new WaitForSeconds(0.2f);
        
        ScatteredWaterballsAttack(avoidablity,6.5f, startAngle - 30);
        
        yield return new WaitUntil
            (()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        ac.ResetGravityScale();
        anim.Play("idle");
        QuitAttack();
        
        
    }


    public IEnumerator H004_Action09()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("H004_Action09");
        
        WarpFXIn();
        _statusManager.ReliefAllDebuffAndAfflictions();
        ac.SetGravityScale(0);

        yield return new WaitForSeconds(0.2f);
        
        SetActiveMelody(false);
        DisappearRenderer();
        transform.position = new Vector3(0, 20);
        StageCameraController.SwitchOverallCamera();
        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup8);
        
        yield return new WaitForSeconds(1);
        
        WarpFXOutWithBat();
        
        yield return new WaitForSeconds(0.7f);
        
        List<GameObject> batInstances = null; // 局部变量存储蝙蝠引用
        bool isComplete = false; // 局部完成标志

        // 调用交换方法并传入完成回调
        BatShuffle(0.75f,1f, (bats) => 
        {
            batInstances = bats; 
            isComplete = true;
        });
        
        yield return new WaitUntil(() => isComplete);

        var hintbar = DestructionPrepare(10);
        var batStats = StartTickAfterShuffle(batInstances);
        var trueBat = batStats[0];
        var timeOver = false;
        var tween = DOVirtual.DelayedCall(10, () => timeOver = true,false);

        yield return new WaitUntil(() => timeOver || trueBat.currentHp <= 0);
        StageCameraController.SwitchMainCamera();
        transform.position = new Vector3(0, BattleStageManager.Instance.mapBorderB + 2);
        KillAllBats(batStats);
        if (timeOver)
        {
            FrigidElegance();
            yield return new WaitForSeconds(2f);
            
        }
        else
        {
            Destroy(hintbar);
        }
        WarpFXOutWithBat();
        SetActiveMelody(true);
        yield return new WaitForSeconds(0.8f);
        AppearRenderer();
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetHitSensor(true);
        ac.ResetGravityScale();
        anim.Play("idle");
        QuitAttack();
        
    }

    /// <summary>
    /// Dps Check
    /// </summary>
    public IEnumerator H004_Action10(int minionHP)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("H004_Action10");
        
        WarpFXIn();
        ac.SetGravityScale(0);

        yield return new WaitForSeconds(0.2f);
        
        DisappearRenderer();
        transform.position = new Vector3(0, BattleStageManager.Instance.mapBorderB + 1.5f);
        
        yield return null;
        
        WarpFXOut(false);
        StageCameraController.SwitchMainCameraFollowObject(gameObject);
        
        yield return new WaitForSeconds(0.2f);
        
        AppearRenderer();
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("charge_enter");
        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.DpsCheck);
        ac.ResetGravityScale();
        
        yield return new WaitForSeconds(0.8f);

        var uiController = SpawnBatMinionsDpsCheck(minionHP);
        var uiTimer = uiController.GetComponent<UI_RingSlider>();
        uiTimer.currentValue = uiTimer.maxValue;

        yield return null;
        
        StageCameraController.SwitchMainCameraFollowObject(BattleStageManager.Instance.GetPlayer());
        
        yield return new WaitUntil(() => uiController.Value <= 0 || uiTimer.currentValue <= 0);

        if (uiController.Value > 0)
        {
            uiController.KillAllMinons();
            Destroy(uiController.gameObject);
            
            FrigidElegance();

            yield return new WaitForSeconds(1.5f);
            
            ac.SetHitSensor(true);
            
            anim.Play("charge_exit");
            
            yield return new WaitForSeconds(0.5f);

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);

        }
        else
        {
            anim.Play("knockdown_enter");

            _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.FailToForce);
            
            Destroy(uiController.gameObject);
            yield return null;
            
            ac.SetHitSensor(true);
            
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
            anim.Play("knockdown_exit");
            yield return null;
            
            
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        }
        
        anim.Play("idle");
        QuitAttack();
        
    }
    
    
    /// <summary>
    /// Follow
    /// </summary>
    /// <returns></returns>
    public IEnumerator H004_Action11()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("H004_Action11");
        ac.TurnMove(_behavior.targetPlayer);
        EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
            new Vector3(15, BattleStageManager.Instance.mapBorderB + 3),
            RangedAttackFXLayer.transform, 3, Vector2.zero, false, true, 1);
        EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
            new Vector3(-15, BattleStageManager.Instance.mapBorderB + 3),
            RangedAttackFXLayer.transform, 3, Vector2.zero, false, true, 1);
        
        StageCameraController.SwitchOverallCamera();
        anim.Play("charge_enter");
        
        yield return new WaitForSeconds(1);
        
        var gates = GateOpen(15,BattleStageManager.Instance.mapBorderB + 3);
        
        yield return new WaitForSeconds(0.5f);
        
        GenerateFollowBat();
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,2);
        gates.gateL.enabled = true;
        gates.gateR.enabled = true;
        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup6);
        
        yield return new WaitForSeconds(1.5f);
        
        StageCameraController.SwitchMainCamera();
        
        anim.Play("charge_exit");

        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        QuitAttack();

    }
    
    
    /// <summary>
    /// Echo
    /// </summary>
    public IEnumerator H004_Action12()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H004_Action12");
        StageCameraController.SwitchOverallCamera();
        WarpFXIn();
        ac.SetGravityScale(0);

        yield return new WaitForSeconds(0.2f);
        
        DisappearRenderer();
        transform.position = new Vector3(0, BattleStageManager.Instance.mapBorderB + 1.5f);
        
        yield return null;
        
        WarpFXOut(true);

        yield return new WaitForSeconds(0.2f);
        
        AppearRenderer();
        ac.ResetGravityScale();
        anim.Play("charge_enter");
        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup3);
        
        yield return new WaitForSeconds(0.8f);

        EchoStart(Random.Range(0,2) == 1, new Vector2(0,2));
        
        yield return new WaitForSeconds(3f);
       
        anim.Play("charge_exit");
        
        StageCameraController.SwitchMainCamera();
        
        yield return new WaitForSeconds(0.2f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        QuitAttack();
        
    }
    
    
    /// <summary>
    /// 齐奏交响曲
    /// </summary>
    /// <returns></returns>
    public IEnumerator H004_Action13()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("H004_Action13");
        ac.TurnMove(_behavior.targetPlayer);

        anim.Play("charge_enter");

        yield return new WaitForSeconds(1);
        
        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup7);
        SixWayBatAttack(true);

        yield return new WaitForSeconds(3.5f);
        
        anim.Play("charge_exit");

        yield return null;
        
        SixWayBatAttack();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        QuitAttack();

    }
    
    /// <summary>
    /// Swelling Concerto
    /// </summary>
    public IEnumerator H004_Action14()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("H004_Action14");
        ac.TurnMove(_behavior.targetPlayer);

        anim.Play("buff_1");

        yield return new WaitForSeconds(0.5f);

        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer, 1.5f);
        ChaserAttack();
        
        yield return new WaitForSeconds(2.75f);
        
        anim.Play("buff_3");
        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup5);
        RingDestruction();
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("buff_5");

        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        QuitAttack();

    }
    
    
    /// <summary>
    /// Warp attack
    /// </summary>
    /// <returns></returns>
    public IEnumerator H004_Action15()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);

        WarpFXIn();
        ac.SetGravityScale(0);

        yield return new WaitForSeconds(0.2f);
        
        DisappearRenderer();
        
        yield return new WaitForSeconds(Random.Range(0.4f,0.8f));

        var pos1 = WarpAttackSetPosition();
        transform.position = pos1;
        

        yield return null;
        
        WarpFXOutWithBat();
        yield return new WaitForSeconds(0.75f);
        
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetHitSensor(true);
        ac.SetCounter(true);
        ac.SetKBRes(100);
        AppearRenderer();
        anim.Play("buff_3");

        yield return new WaitForSeconds(0.25f);
        
        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup4);
        WarpAttack();
        
        yield return new WaitForSeconds(1f);

        WarpFXIn();
        var targetPosX = _behavior.targetPlayer.transform.position.x;
        ac.SetCounter(false);

        yield return new WaitForSeconds(0.2f);
        
        DisappearRenderer();
        
        yield return new WaitForSeconds(Random.Range(0.9f,1f));

        
        if (Mathf.Abs(targetPosX - _behavior.targetPlayer.transform.position.x) <= 6)
        {
            transform.position = WarpAttackSetPosition2();
            WarpFXOutWithBat();
            yield return new WaitForSeconds(0.75f);
        
            ac.TurnMove(_behavior.targetPlayer);
            ac.SetHitSensor(true);
            ac.SetCounter(true);
            ac.SetKBRes(100);
            AppearRenderer();
            anim.Play("buff_3");
            
            yield return new WaitForSeconds(0.2f);
        
            _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup4);
            WarpAttackRanged();
        }
        else
        {
            transform.position = WarpAttackSetPosition();
            WarpFXOutWithBat();
            yield return new WaitForSeconds(0.75f);
        
            ac.TurnMove(_behavior.targetPlayer);
            ac.SetHitSensor(true);
            ac.SetCounter(true);
            ac.SetKBRes(100);
            AppearRenderer();
            anim.Play("buff_3");
            
            yield return new WaitForSeconds(0.25f);
        
            _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup4);
            WarpAttack();
        }
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("buff_5");

        yield return null;
        
        ac.ResetGravityScale();
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        QuitAttack();

    }

    /// <summary>
    /// teleport
    /// </summary>
    public IEnumerator H004_Action16(string anchorName)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        WarpFXIn();
        ac.SetGravityScale(0);

        yield return new WaitForSeconds(0.2f);
        
        DisappearRenderer();
        transform.position = GetAnchoredSensorOfName(anchorName).transform.position;
        
        yield return null;
        
        WarpFXOut(false);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.2f);
        
        AppearRenderer();
        ac.SetHitSensor(true);
        ac.ResetGravityScale();
        QuitAttack();
    }
    
    
    public IEnumerator H004_Action17()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("charge_enter");
        bossBanner?.PrintSkillName("H004_Action17");

        yield return new WaitForSeconds(1);
        
        OnMelodyHit(CurrentTune == 1 ? 2 : 1);
        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup2);
        anim.Play("charge_exit");

        yield return null;
        
        StartDissonance();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        QuitAttack();

    }
    
    public IEnumerator H004_Action18()
    {
        yield return _canAction;
        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("buff_1");

        yield return new WaitForSeconds(0.5f);

        GenerateWarningPrefab("action18", transform.position, Quaternion.identity,
            MeeleAttackFXLayer.transform);

        yield return new WaitForSeconds(0.9f);
        
        anim.Play("buff_3");
        
        yield return new WaitForSeconds(0.2f);
        
        FanAttack();
        
        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup4);
        anim.Play("buff_5");

        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        QuitAttack();

    }
    
    /// <summary>
    /// 魔翼合奏曲
    /// </summary>
    /// <returns></returns>
    public IEnumerator H004_Action19()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("buff_1");
        bossBanner?.PrintSkillName("H004_Action19");

        yield return new WaitForSeconds(0.5f);

        var type = Random.Range(0, 2) == 0;
        CrossBatAttack(type,2);
        
        yield return new WaitForSeconds(0.3f);
        
        anim.Play("buff_3");
        
        yield return new WaitForSeconds(2f);
        
        CrossBatAttack(!type,2.25f);
        
        _voiceController?.BroadCastMyVoice((int)MyVoiceGroup.SkillGroup5);
        anim.Play("buff_5");

        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        QuitAttack();

    }
    
    
    
    public IEnumerator PartBreak()
    {
        ac.SetCounter(false);
        ac.SetKBRes(999);
        ac.ResetGravityScale();
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        _behavior.breakable = false;
        _statusManager.ImmuneToAllControlAffliction = true;
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("part_break");
        //ac.OnHurtEnter();
        BattleEffectManager.Instance.PlayReviveSoundEffect();
        Instantiate(partBreakFXPrefab,transform.position + new Vector3(-ac.facedir,1.5f),
            Quaternion.identity,RangedAttackFXLayer.transform);

     
        (_statusManager as SpecialStatusManager).counterModifier = 0.3f;
        _partBreakDebuff.dispellable = false;
        _partBreakDebuff2.dispellable = false;
        _statusManager.ObtainTimerBuff(_partBreakDebuff);
        _statusManager.ObtainTimerBuff(_partBreakDebuff2);
        
        //DecreaseResistances();

        partRenderer.SetActive(false);
        

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        

        yield return new WaitForSeconds(1f);
        _behavior.breakable = true;
        _statusManager.ImmuneToAllControlAffliction = false;
        
        QuitAttack();
    }

    private void OnAfflictionEvent(BattleCondition condition)
    {
        if(TuneCountdown <= 0)
            return;
        
        if (CurrentTune == 1)
        {
            if (condition.buffID == (int)BasicCalculation.BattleCondition.Flashburn ||
                condition.buffID == (int)BasicCalculation.BattleCondition.Stormlash)
            {
                TuneCountdown--;
                BattleStageManager.Instance.InvokeEnemyAbilityEvent((int)BasicCalculation.EnemyAbility.MelodyHell,
                    new EnemyAbilityIconEvent(EnemyAbilityIconEvent.EventType.SetNumber, TuneCountdown),_statusManager);
            }
        }else if (CurrentTune == 2)
        {
            if (condition.buffID == (int)BasicCalculation.BattleCondition.Poison ||
                condition.buffID == (int)BasicCalculation.BattleCondition.Paralysis)
            {
                TuneCountdown--;
                BattleStageManager.Instance.InvokeEnemyAbilityEvent((int)BasicCalculation.EnemyAbility.MelodyHeaven,
                    new EnemyAbilityIconEvent(EnemyAbilityIconEvent.EventType.SetNumber, TuneCountdown),_statusManager);
            }
        }
        
        
    }
    
    
    
    private void OnReceiveDamageEvent(int dmg, AttackBase atk)
    {
        if(_playerMelodyType == CurrentTune)
            return;
        
        if(_playerMelodyType == 0 || TuneCountdown <= 0)
            return;

        if (atk.attackType == BasicCalculation.AttackType.SKILL)
        {
            dmg = (int)(dmg*1.2f);
        }

        _damageCounter += dmg;

        if (_damageCounter > _statusManager.maxBaseHP * 0.02f)
        {
            _damageCounter = 0;
            TuneCountdown--;

            if (CurrentTune == 1)
            {
                BattleStageManager.Instance.InvokeEnemyAbilityEvent((int)BasicCalculation.EnemyAbility.MelodyHell,
                    new EnemyAbilityIconEvent(EnemyAbilityIconEvent.EventType.SetNumber, TuneCountdown),_statusManager);
            }else if (CurrentTune == 2)
            {
                BattleStageManager.Instance.InvokeEnemyAbilityEvent((int)BasicCalculation.EnemyAbility.MelodyHeaven,
                    new EnemyAbilityIconEvent(EnemyAbilityIconEvent.EventType.SetNumber, TuneCountdown),_statusManager);
            }
        }
        

    }
    
    private void InitPlayerMelody()
    {
        if (_melodyFXInstance == null)
        {
            _melodyFXInstance = Instantiate(GetProjectileOfFormatName("action01"),
                new Vector3(0,0),Quaternion.identity,RangedAttackFXLayer.transform);
        }

        if (_playerTuneFXInstance == null)
        {
            var player = BattleStageManager.Instance.GetPlayer();
            _playerTuneFXInstance = Instantiate(playerTuneFXPrefab,
                player.transform.position,Quaternion.identity,player.transform.Find("BuffLayer"));
            
            _playerTuneFXInstance.transform.GetChild(0).gameObject.SetActive(false);
            _playerTuneFXInstance.transform.GetChild(1).gameObject.SetActive(false);

            _uiInstance = Instantiate(playerMelodyUIPrefab,
                BattleSceneUIManager.Instance.transform).GetComponent<Image>();
        }

        if (_playerMelodyType == 0)
        {
            OnMelodyHit(CurrentTune==1?1:2);
        }
        
    }

    private void SetSelfMelody(int type)
    {
        if (type == 1)
        {
            CurrentTune = 1;
            TuneCountdown = 8;
            hellTuneInstance.SetActive(true);
            heavenTuneInstance.SetActive(false);
            BattleStageManager.Instance.InvokeEnemyAbilityEvent((int)BasicCalculation.EnemyAbility.MelodyHeaven,
                new EnemyAbilityIconEvent(EnemyAbilityIconEvent.EventType.SetText, ""),_statusManager);
            BattleStageManager.Instance.InvokeEnemyAbilityEvent((int)BasicCalculation.EnemyAbility.MelodyHell,
                new EnemyAbilityIconEvent(EnemyAbilityIconEvent.EventType.SetNumber, TuneCountdown),_statusManager);
            BattleStageManager.Instance.InvokeEnemyAbilityEvent((int)BasicCalculation.EnemyAbility.MelodyHeaven,
                new EnemyAbilityIconEvent(0),_statusManager);
            BattleStageManager.Instance.InvokeEnemyAbilityEvent((int)BasicCalculation.EnemyAbility.MelodyHell,
                new EnemyAbilityIconEvent(1),_statusManager);
        }else if (type == 2)
        {
            CurrentTune = 2;
            TuneCountdown = 8;
            hellTuneInstance.SetActive(false);
            heavenTuneInstance.SetActive(true);
            BattleStageManager.Instance.InvokeEnemyAbilityEvent((int)BasicCalculation.EnemyAbility.MelodyHell,
                new EnemyAbilityIconEvent(EnemyAbilityIconEvent.EventType.SetText, ""),_statusManager);
            BattleStageManager.Instance.InvokeEnemyAbilityEvent((int)BasicCalculation.EnemyAbility.MelodyHell,
                new EnemyAbilityIconEvent(0),_statusManager);
            BattleStageManager.Instance.InvokeEnemyAbilityEvent((int)BasicCalculation.EnemyAbility.MelodyHeaven,
                new EnemyAbilityIconEvent(1),_statusManager);
            BattleStageManager.Instance.InvokeEnemyAbilityEvent((int)BasicCalculation.EnemyAbility.MelodyHeaven,
                new EnemyAbilityIconEvent(EnemyAbilityIconEvent.EventType.SetNumber, TuneCountdown),_statusManager);
        }
        
        
    }

    private void StartDissonance()
    {
        var dissonance = new TimerBuff((int)BasicCalculation.BattleCondition.Dissonance,
            1, -1, 1);
        dissonance.dispellable = true;
        _dissonance = true;
        BattleStageManager.Instance.InvokeEnemyAbilityEvent
        ((int)BasicCalculation.EnemyAbility.Dissonance,
            new EnemyAbilityIconEvent(1),_statusManager);

        _statusManager.ObtainTimerBuff(dissonance);
        
        var fx = Instantiate(GetProjectileOfFormatName("action17"),transform.position,
            Quaternion.identity,RangedAttackFXLayer.transform);
    }

    private void BuffSelf()
    {
        var fx = Instantiate(GetProjectileOfFormatName("action07"),transform.position,
            Quaternion.identity,RangedAttackFXLayer.transform);

        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
            25,15);
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            25,15);

    }
    private void NihilAOE()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action02"),
            new Vector3(_behavior.targetPlayer.transform.position.x,BattleStageManager.Instance.mapBorderB),InitContainer(false),1);

        var nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, 5, 1);
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(nihilDebuff,100);
    }
    private void CorrosionFog(float healNeeded)
    {
        var fx = InstantiateRanged(GetProjectileOfFormatName("action03"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,
                _behavior.viewerPlayer.transform.position.y-1),
            InitContainer(false),1);

        var corrosionEff = new AdvancedTimerBuff((int)BasicCalculation.BattleCondition.Corrosion,
            healNeeded, 8, 0, -1, 1, -1,8);
        
        fx.GetComponent<AttackFromEnemy>().AddWithConditionAll(corrosionEff,200);

    }

    private void WaveForward(Vector2 pos1, Vector2 pos2)
    {
        var container = InitContainer(false);
        var fx1 = InstantiateRanged(GetProjectileOfFormatName("action04_1"),
            pos1, container, 1);
        
        var fx2 = InstantiateRanged(GetProjectileOfFormatName("action04_2"),
            pos2, container, 1);

    }

    private void WaveAround()
    {
        var container = InitContainer(false);
        
        var fxAround = InstantiateRanged(GetProjectileOfFormatName("action04_3"),
            new Vector3(transform.position.x,
                gameObject.RaycastedPosition().y),container,1);
        
        var fxLeft = InstantiateRanged(GetProjectileOfFormatName("action04_2"),
            new Vector3(transform.position.x - 15f,
                fxAround.transform.position.y),container,1);
        
        var fxRight = InstantiateRanged(GetProjectileOfFormatName("action04_2"),
            new Vector3(transform.position.x + 15f,
                fxAround.transform.position.y),container,1);
        
        var bogEff = new TimerBuff((int)BasicCalculation.BattleCondition.Bog, 1, 9, 1);

        fxAround.GetComponent<AttackFromEnemy>().AddWithConditionAll(bogEff,100);
        
    }

    private void ScatteredWaterballsHint(bool avoidability, float startAngle = 0)
    {
        for (int i = 0; i < 6; i++)
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                transform.position + new Vector3(0,2), 
                MeeleAttackFXLayer.transform, new Vector2(30, 2),
                Vector2.zero, avoidability, 1, 2.5f, startAngle, 1);
            
            print("HintAngle: " + startAngle + " degrees");

            startAngle += 60f;
            
        }
    }
    
    private void ScatteredWaterballsAttack(bool avoidability, float speed, float startAngle = 0)
    {
        var container = InitContainer(false, 6);  // 容器调整为6个弹幕

        // 从起始角度开始的6个方向，每60度一个（匹配UI提示）
        float baseAngle = startAngle * Mathf.Deg2Rad;  // 转换为弧度
        const float angleStep = 60 * Mathf.Deg2Rad;    // 60度间隔（弧度值）
        
        var projPrefab = GetProjectileOfFormatName("action08");

        for (int i = 0; i < 6; i++)
        {
            // 计算当前角度
            float currentAngle = baseAngle + i * angleStep;
        
            // 创建弹幕（与提示相同的位置：头顶+2单位）
            var proj = InstantiateRanged(
                projPrefab,
                transform.position + new Vector3(0, 2),
                container,
                1
            );

            // 设置速度（使用弧度计算方向向量）
            Vector2 direction = new Vector2(
                Mathf.Sin(currentAngle),
                Mathf.Cos(currentAngle)
            );
        
            proj.GetComponent<ReflectionProjectile>().SetVelocity(direction * speed);
            if (avoidability)
            {
                proj.GetComponent<AttackFromEnemy>().ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Red);
            }
        }
    }

    private void WarpFXIn(bool hideHitbox = true)
    {
        var fx = Instantiate(warpFXAppearPrefab, transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);

        if (hideHitbox)
        {
            ac.SetHitSensor(false);
        }
        
    }
    
    private void WarpFXOut(bool activateHitbox = true)
    {
        var fx = Instantiate(warpFXDisappearPrefab, transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);
        if(activateHitbox)
        {
            ac.SetHitSensor(true);
        }
    }
    
    private GameObject WarpFXOutWithBat()
    {
        var fx = Instantiate(warpFXDisappearPrefab2, transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);
        return fx;
    }

    private void SetActiveMelody(bool active)
    {
        if (active)
        {
            BattleStageManager.Instance.TriggerSpecialEvent(Projectile_H004_1.Active);
        }
        else
        {
            BattleStageManager.Instance.TriggerSpecialEvent(Projectile_H004_1.Disable);
        }
        
    }
    
    
    
    private GameObject SummonBatEffect(Vector2 pos)
    {
        var fx = Instantiate(GetProjectileOfFormatName("action05"), pos,
            Quaternion.identity, RangedAttackFXLayer.transform);

        return fx;
    }

    private void WhirlRingsHint(Vector2 posL, Vector2 posR)
    {
        if ((_behavior as H004_BehaviorTree).CapeHasBroken)
        {
            GenerateWarningPrefab("action05_1",posL,Quaternion.identity,RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action05_1",posR,Quaternion.identity,RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action05_2",transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        }
        else
        {
            GenerateWarningPrefab("action05_1",posL,Quaternion.identity,RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action05_1",posR,Quaternion.identity,RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action05_1",transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        }



    }

    private void WhirlRingsAttack(Vector2 posL, Vector2 posR, int size)
    {
        GameObject prefab;
        if (size <= 1)
        {
            prefab = GetProjectileOfFormatName("action05_1");
        }else if (size == 2)
        {
            prefab = GetProjectileOfFormatName("action05_2");
        }
        else
        {
            prefab = GetProjectileOfFormatName("action05_3");
        }
        
        var container = InitContainer(false, 3);
        
        var fxL = InstantiateRanged(prefab, posL, container, 1);
        var fxR = InstantiateRanged(prefab, posR, container, -1);
        var fxM = InstantiateRanged(prefab, transform.position, container, ac.facedir);

        if ((_behavior as H004_BehaviorTree).CapeHasBroken)
        {
            fxM.GetComponent<AttackFromEnemy>().ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Red);
            //fxL.GetComponent<AttackFromEnemy>().ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Red);
            //fxR.GetComponent<AttackFromEnemy>().ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Red);
        }

    }


    /// <param name="type">1: LowerT 2:LowerW 3:UpperW</param>
    private float EncorePrepareHint(int type)
    {
        float warningTime = 0;
        switch (type)
        {
            case 1:
            {
                warningTime = GenerateWarningPrefab("action06_1", transform.position, Quaternion.Euler(0, 0, -90),
                    RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBar>().warningTime;
                break;
            }
            case 2:
            {
                warningTime = GenerateWarningPrefab("action06_3", transform.position, Quaternion.Euler(0, 0, -90),
                    RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBar>().warningTime;
                break;
            }
            case 3:
            {
                warningTime = GenerateWarningPrefab("action06_3", transform.position, Quaternion.Euler(0, 0, 90),
                    RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBar>().warningTime;
                break;
            }
        }

        return warningTime;
    }
    
    /// <param name="type">1: LowerT 2:LowerW 3:UpperW</param>
    private void EncorePrepareAttack(int type)
    {
        
        switch (type)
        {
            case 1:
            {
                InstantiateDirectionalRanged(GetProjectileOfFormatName("action06_1"),
                    transform.position, InitContainer(false), 1,-90);
                break;
            }
            case 2:
            {
                InstantiateDirectionalRanged(GetProjectileOfFormatName("action06_3"),
                    transform.position, InitContainer(false), 1,-90);
                break;
            }
            case 3:
            {
                InstantiateDirectionalRanged(GetProjectileOfFormatName("action06_3"),
                    transform.position, InitContainer(false), 1,90);
                break;
            }
        }
    }

    /// <param name="type">1: LowerT 2:LowerW 3:UpperW</param>
    private void EncoreAccentHint(int type, GameObject batL, GameObject batR, GameObject batB, GameObject batT)
    {
        //没加方向
        switch (type)
        {
            case 1:
            {
                GenerateWarningPrefab("action06_2", batT.transform.position,
                        Quaternion.Euler(0, 0, -90),
                    RangedAttackFXLayer.transform);
                GenerateWarningPrefab("action06_2", batB.transform.position,
                    Quaternion.Euler(0, 0, 90),
                    RangedAttackFXLayer.transform);
                GenerateWarningPrefab("action06_2", batR.transform.position,
                    Quaternion.Euler(0, 0, 180),
                    RangedAttackFXLayer.transform);
                GenerateWarningPrefab("action06_2", batL.transform.position,
                    Quaternion.Euler(0, 0, 0),
                    RangedAttackFXLayer.transform);
                break;
            }
            case 2: 
            {
                GenerateWarningPrefab("action06_4", batT.transform.position,
                    Quaternion.Euler(0, 0, -90),
                    RangedAttackFXLayer.transform);
                GenerateWarningPrefab("action06_4", batB.transform.position,
                    Quaternion.Euler(0, 0, 90),
                    RangedAttackFXLayer.transform);
                GenerateWarningPrefab("action06_4", batR.transform.position,
                    Quaternion.Euler(0, 0, 180),
                    RangedAttackFXLayer.transform);
                GenerateWarningPrefab("action06_4", batL.transform.position,
                    Quaternion.Euler(0, 0, 0),
                    RangedAttackFXLayer.transform);
                break;
            }
            case 3: 
            {
                GenerateWarningPrefab("action06_4", batT.transform.position,
                    Quaternion.Euler(0, 0, 90),
                    RangedAttackFXLayer.transform);
                GenerateWarningPrefab("action06_4", batB.transform.position,
                    Quaternion.Euler(0, 0, -90),
                    RangedAttackFXLayer.transform);
                GenerateWarningPrefab("action06_4", batR.transform.position,
                    Quaternion.Euler(0, 0, 0),
                    RangedAttackFXLayer.transform);
                GenerateWarningPrefab("action06_4", batL.transform.position,
                    Quaternion.Euler(0, 0, 180),
                    RangedAttackFXLayer.transform);
                break;
            }
        }
        
    }
    
    /// <param name="type">1: LowerT 2:LowerW 3:UpperW</param>
    private void EncoreAccentAttack(int type, GameObject batL, GameObject batR, GameObject batB, GameObject batT)
    {

        var container = InitContainer(false, 4);
        var prefab = type == 1 ? 
            GetProjectileOfFormatName("action06_2") : GetProjectileOfFormatName("action06_4");

        switch (type)
        {
            case 1:
            {
                InstantiateDirectionalRanged(prefab, batL.transform.position,
                    container, 1, 0);
                InstantiateDirectionalRanged(prefab, batR.transform.position,
                    container, 1, 180);
                InstantiateDirectionalRanged(prefab, batB.transform.position,
                    container, 1, 90);
                InstantiateDirectionalRanged(prefab, batT.transform.position,
                    container, 1, -90);
                break;
            }
            case 2:
            {
                InstantiateDirectionalRanged(prefab, batL.transform.position,
                    container, 1, 0);
                InstantiateDirectionalRanged(prefab, batR.transform.position,
                    container, 1, 180);
                InstantiateDirectionalRanged(prefab, batB.transform.position,
                    container, 1, 90);
                InstantiateDirectionalRanged(prefab, batT.transform.position,
                    container, 1, -90);
                break;
            }
            case 3:
            {
                InstantiateDirectionalRanged(prefab, batL.transform.position,
                    container, 1, 180);
                InstantiateDirectionalRanged(prefab, batR.transform.position,
                    container, 1, 0);
                InstantiateDirectionalRanged(prefab, batB.transform.position,
                    container, 1, -90);
                InstantiateDirectionalRanged(prefab, batT.transform.position,
                    container, 1, 90);
                break;
            }
        }
        
    }

    private (Projectile_H004_3 gateL,Projectile_H004_3 gateR) GateOpen(float posX, float posY)
    {
        var gatePrefab = GetProjectileOfFormatName("action11_1",false);

        var container = InitContainer(false);

        var gateL = InstantiateRanged(gatePrefab, new Vector2(-posX, posY),
            container,1, 1).GetComponent<Projectile_H004_3>();
        
        var gateR =  InstantiateRanged(gatePrefab, new Vector2(posX, posY),
            container,1, 1).GetComponent<Projectile_H004_3>();

        return (gateL, gateR);

    }

    private void GenerateFollowBat()
    {
        var batPrefab = GetProjectileOfFormatName("minion_2",false);

        var bat = SpawnEnemyMinon(batPrefab, new Vector3(0, 10), 99999, 
            _statusManager.baseAtk);
        
        var fx = Instantiate(GetProjectileOfFormatName("action06"),bat.transform.position,
            Quaternion.identity,RangedAttackFXLayer.transform);

        bat.GetComponent<Projectile_H004_2>().SetBossGameObject(gameObject);
        
    }
    
    private void SixWayBatAttack(bool startWithPurple = false)
    {
        var positions = new int[]
        {
            -24, -20, -16, -12, -8, -4, 0, 4, 8, 12, 16, 20, 24
        };
        
        var barLength = (BattleStageManager.Instance.mapBorderT - 
                         BattleStageManager.Instance.mapBorderB) + 2;
        bool avoidable = !startWithPurple;
        float fillTime = 1.5f;
        bool capeHasBroken = (_behavior as H004_BehaviorTree).CapeHasBroken;

        foreach (var position in positions)
        {
            if (capeHasBroken && !avoidable)
            {
                avoidable = !avoidable;
                continue;
            }

            var bar = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                new Vector3(position, BattleStageManager.Instance.mapBorderT - 2),
                RangedAttackFXLayer.transform, new Vector2(barLength, 3), Vector2.zero,
                avoidable, 1, fillTime, -90, 1);

            bar.transform.localScale = new Vector3(1, 0.1f, 1);
            bar.transform.DOScaleY(1, fillTime/5);

            avoidable = !avoidable;
        }
        
        var prefab = GetProjectileOfFormatName("action13");
        var container = InitContainer(false);
        var atkList = new List<AttackFromEnemy>();
        avoidable = !startWithPurple;

        foreach (var position in positions)
        {
            if (capeHasBroken && !avoidable)
            {
                avoidable = !avoidable;
                continue;
            }
            
            var atk = InstantiateRanged(prefab, 
                new Vector3(position,BattleStageManager.Instance.mapBorderT - 2),
                container, 1).GetComponent<AttackFromEnemy>();

            if (!avoidable)
            {
                atk.ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
            }
            else
            {
                atk.ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Red);
            }
            atkList.Add(atk);
            
            avoidable = !avoidable;
        }

        DOVirtual.DelayedCall(fillTime, () =>
        {
            foreach (var atk in atkList)
            {
                var duration = atk.GetAvoidableProperty() == AttackFromEnemy.AvoidableProperty.Purple ? 
                    1.45f : 1.4f;
                atk.attackCollider.enabled = true;
                atk.GetComponent<Rigidbody2D>().
                    DOMoveY(BattleStageManager.Instance.mapBorderB - 2, duration).OnComplete(() =>
                    {
                        Destroy(atk.gameObject);
                    }).SetEase(Ease.Linear);
            }
        }, false);
    }

    private void ShineEffect(params GameObject[] posArray)
    {
        var prefab = GetProjectileOfFormatName("action06");
        foreach(var pos in posArray)
        {
            Instantiate(prefab, pos.transform.position, Quaternion.identity,
                RangedAttackFXLayer.transform);
        }
    }

    private UI_CountdownMinon SpawnBatMinionsDpsCheck(int hp)
    {
        var minionPrefab = GetProjectileOfFormatName("minion_1");
        
        var minon1 = SpawnEnemyMinon(minionPrefab,
                new Vector3(-9, BattleStageManager.Instance.mapBorderB + 1), hp);
        var minon2 = SpawnEnemyMinon(minionPrefab,
                new Vector3(9, BattleStageManager.Instance.mapBorderB + 1), hp);
        var minon3 = SpawnEnemyMinon(minionPrefab,
                new Vector3(0,BattleStageManager.Instance.mapBorderB + 1), hp);
        
        var UI = Instantiate(GetProjectileOfFormatName("action10_ui",true),
                transform.position.SafePosition(new Vector2(0, 8f)), Quaternion.identity,
                RangedAttackFXLayer.transform);
        
        var UIRingSlider = UI.GetComponent<UI_RingSlider>();
        UIRingSlider.maxValue = 15;

        var UICountdownMinon = UI.GetComponent<UI_CountdownMinon>();
        UICountdownMinon.SetMaxCapacity(3);
        UICountdownMinon.AddNewStatusManager(minon1.GetComponent<StatusManager>());
        UICountdownMinon.AddNewStatusManager(minon2.GetComponent<StatusManager>());
        UICountdownMinon.AddNewStatusManager(minon3.GetComponent<StatusManager>());

        return UICountdownMinon;
    }

    private GameObject FrigidElegance(bool printSkillName = true)
    {
        if(printSkillName)
            bossBanner?.PrintSkillName("H004_Action16");
        
        PurgedShapeShiftingOfViewer();

        return InstantiateRanged(GetProjectileOfFormatName("action16", true),
            new Vector3(0, BattleStageManager.Instance.mapBorderB), InitContainer(false), 1);
    }

    private void BatShuffle(float animTime, float interval, Action<List<GameObject>> onComplete)
    {
        Vector2[] positionPresets = new Vector2[]
        {
            new(0, 20),
            new(12, 15),
            new(12, 7),
            new(0, 2),
            new(-12, 7),
            new(-12, 15)
        };

        var batPrefab = GetProjectileOfFormatName("minion_3");
        var batStat = batPrefab.GetComponent<StatusManager>();
        var batInstances = new List<GameObject>();
        for (int i = 0; i < positionPresets.Length; i++)
        {
            batInstances.Add(SpawnEnemyMinon(batPrefab, positionPresets[i], batStat.maxBaseHP,
                batStat.baseAtk));
        }
        

        List<List<int[]>> swapPatterns = new List<List<int[]>>()
        {
            // 模式0（2次交换）：0-4-2
            new List<int[]>
            {
                //        { 0, 1, 2, 3, 4, 5 }
                new int[] { 4, 3, 5, 0, 2, 1 }, 
                new int[] { 2, 0, 1, 4, 5, 3 } 
            },
            // 模式1（2次交换）：0-3-0
            new List<int[]>
            {
                //        { 0, 1, 2, 3, 4, 5 }
                new int[] { 3, 5, 4, 0, 2, 1 }, 
                new int[] { 0, 2, 1, 4, 5, 3 } 
            },
            // 模式1（2次交换）：0-3-4
            new List<int[]>
            {
                //        { 0, 1, 2, 3, 4, 5 }
                new int[] { 3, 5, 4, 0, 2, 1 }, 
                new int[] { 4, 2, 1, 3, 0, 5 } 
            },
            // 模式3（3次交换）：0-4-3-1
            new List<int[]>
            {
                //        { 0, 1, 2, 3, 4, 5 }
                new int[] { 4, 3, 5, 2, 0, 1 },
                new int[] { 3, 5, 1, 0, 2, 4 }, 
                new int[] { 1, 4, 5, 2, 0, 3 } 
            },
            // 模式4（3次交换）：0-4-2-5
            new List<int[]>
            {
                //        { 0, 1, 2, 3, 4, 5 }
                new int[] { 4, 3, 5, 0, 2, 1 }, 
                new int[] { 2, 0, 1, 3, 4, 5 }, 
                new int[] { 5, 2, 3, 0, 1, 4 } 
            },
            // 模式5（3次交换）：0-4-1-3
            new List<int[]>
            {
                //        { 0, 1, 2, 3, 4, 5 }
                new int[] { 4, 3, 5, 0, 1, 2 }, 
                new int[] { 1, 0, 3, 2, 5, 4 }, 
                new int[] { 3, 1, 5, 0, 4, 2 } 
            }
        };

        // 随机选择一种模式（0-4）
        int selectedPattern = Random.Range(0, 5);
        var patternSteps = swapPatterns[selectedPattern];

        // 构建动画序列（含第一次交换前的间隔）
        Sequence dtSequence = DOTween.Sequence()
            .AppendInterval(interval * 2); // 第一次交换前等待interval秒

        // 遍历每个交换步骤
        for (int step = 0; step < patternSteps.Count; step++)
        {
            int[] targetIndices = patternSteps[step]; // 当前步骤目标位置索引

            // 创建并行动画组（所有蝙蝠同时移动）
            Sequence stepSequence = DOTween.Sequence();
            for (int i = 0; i < batInstances.Count; i++)
            {
                Vector2 targetPos = positionPresets[targetIndices[i]];
                stepSequence.Join(batInstances[i].transform.DOMove(targetPos, animTime));
            }

            // 将本次交换动画添加到主序列
            dtSequence.Append(stepSequence);

            // 非最后一步时添加交换后的间隔（最后一步无间隔）
            if (step < patternSteps.Count - 1)
            {
                dtSequence.AppendInterval(interval);
            }
        }

        dtSequence.OnComplete(() => onComplete?.Invoke(batInstances));

    }

    private GameObject DestructionPrepare(float fillTime)
    {
        var mapWidth = Mathf.Abs((BattleStageManager.Instance.mapBorderR -
                                  BattleStageManager.Instance.mapBorderL));
        var mapHeight = BattleStageManager.Instance.mapBorderT - BattleStageManager.Instance.mapBorderB;
        
        return EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, 
            new Vector3(0,BattleStageManager.Instance.mapBorderT),
            RangedAttackFXLayer.transform, new Vector2(mapHeight,mapWidth), Vector2.zero, 
            false, 0, fillTime,-90,1,true,false);
    }
    private List<StatusManager> StartTickAfterShuffle(List<GameObject> batList)
    {
        
        List<EnemyController> batControllerList = new();
        List<StatusManager> batStatList = new();

        for (int i = 0; i < batList.Count; i++)
        {
            var bat = batList[i];
            var ac = bat.GetComponent<EnemyController>();
            var stat = ac.GetComponent<StatusManager>();
            ac.SetHitSensor(true);
            stat.AddEffectFunction(SpecialDamageCut, AbilityCalculation.ProductArea.DMGCUT);
            batControllerList.Add(ac);
            batStatList.Add(stat);
        }

        return batStatList;

    }

    private void KillAllBats(List<StatusManager> batList)
    {
        for (int i = 0; i < batList.Count; i++)
        {
            batList[i].currentHp = 0;
            batList[i].OnHPBelow0?.Invoke();
        }
    }

    private static (float, float) SpecialDamageCut(StatusManager src, AttackBase atk, StatusManager tar)
    {
        if (atk.attackType == BasicCalculation.AttackType.DASH)
        {
            return (0, 0.5f);
        }else if (atk.attackType == BasicCalculation.AttackType.STANDARD)
        {
            return (0.9f, 0);
        }
        else return (0.99f, 0);
    }

    private void EchoStart(bool clockWise, Vector2 basePosition)
    {
        var hintbarSetPrefab = clockWise ? GetWarningPrefab("action12_1") :
            GetWarningPrefab("action12_4");

        var hintbarInstance = Instantiate(hintbarSetPrefab,
            basePosition, Quaternion.identity, RangedAttackFXLayer.transform);

        List<EnemyAttackHintBar> barComponentList = new();
        barComponentList.AddRange(hintbarInstance.GetComponentsInChildren<EnemyAttackHintBar>());
        float warningTime = -1;

        foreach (var bar in barComponentList)
        {
            bar.warningTime *= 2;
            bar.SetAc(ac);
            if (warningTime < 0)
                warningTime = bar.warningTime;
        }

        var batPrefab = GetProjectileOfFormatName("action12_1");
        var rotationHintPrefab = GetWarningPrefab("action12_5");

        List<GameObject> batList = new();
        List<float> angleZList = new();

        for (int i = 0; i < hintbarInstance.transform.childCount; i++)
        {
            var bat = Instantiate(batPrefab, hintbarInstance.transform.GetChild(i).position,
                Quaternion.identity, RangedAttackFXLayer.transform);
            batList.Add(bat);
            angleZList.Add(hintbarInstance.transform.GetChild(i).eulerAngles.z);
            var rotateHint = Instantiate(rotationHintPrefab, bat.transform.position,
                Quaternion.identity, RangedAttackFXLayer.transform);
            if (!clockWise)
            {
                rotateHint.transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        var atkPrefab = GetProjectileOfFormatName("action12_2");
        var container = InitContainer(false,12);

        int clockwiseModifier = clockWise ? -1 : 1;

        
        DOVirtual.DelayedCall(warningTime, () =>
        {
            for (int i = 0; i < batList.Count(); i++)
            {
                InstantiateDirectionalRanged(atkPrefab, batList[i].transform.position,
                        container, 1, angleZList[i]);
            }
                
            var hintbarInstance = Instantiate(clockWise ? 
                    GetWarningPrefab("action12_2") : GetWarningPrefab("action12_3"),
                basePosition, Quaternion.identity, RangedAttackFXLayer.transform);
                
        }, false);
        
        DOVirtual.DelayedCall(warningTime * 1.5f, () =>
        {
            for (int i = 0; i < batList.Count(); i++)
            {
                InstantiateDirectionalRanged(atkPrefab, batList[i].transform.position,
                    container, 1, angleZList[i] + clockwiseModifier * 45);
            }
                
            var hintbarInstance = Instantiate(clockWise ? 
                    GetWarningPrefab("action12_3") : GetWarningPrefab("action12_2"),
                basePosition, Quaternion.identity, RangedAttackFXLayer.transform);
                
        }, false);


        DOVirtual.DelayedCall(warningTime * 2f, () =>
        {
            for (int i = 0; i < batList.Count(); i++)
            {
                InstantiateDirectionalRanged(atkPrefab, batList[i].transform.position,
                    container, 1, angleZList[i] + clockwiseModifier * 90);
            }
                
            var hintbarInstance = Instantiate(clockWise ? 
                    GetWarningPrefab("action12_4") : GetWarningPrefab("action12_1"),
                basePosition, Quaternion.identity, RangedAttackFXLayer.transform);
                
        }, false);
        
        DOVirtual.DelayedCall(warningTime * 2.5f, () =>
        {
            for (int i = 0; i < batList.Count(); i++)
            {
                InstantiateDirectionalRanged(atkPrefab, batList[i].transform.position,
                    container, 1, angleZList[i] + clockwiseModifier * 135);
            }

        }, false);

        foreach (var bat in batList)
        {
            Destroy(bat, warningTime * 3f);
        }

    }

    private Vector2 WarpAttackSetPosition()
    {
        Vector2 pos;
        ActorBase playerActor = _behavior.targetPlayer.GetComponent<ActorBase>();
        if (playerActor.transform.position.x < BattleStageManager.Instance.mapBorderL + 6)
        {
            pos = new Vector2(BattleStageManager.Instance.mapBorderL + 4, playerActor.transform.position.y - 0.3f);
        }
        else if (playerActor.transform.position.x > BattleStageManager.Instance.mapBorderR - 6)
        {
            pos = new Vector2(BattleStageManager.Instance.mapBorderR - 4, playerActor.transform.position.y- 0.3f);
        }
        else
        {
            if (playerActor.facedir == 1)
            {
                pos = new Vector2(playerActor.transform.position.x - 2, playerActor.transform.position.y- 0.3f);
            }
            else
            {
                pos = new Vector2(playerActor.transform.position.x + 2, playerActor.transform.position.y- 0.3f);
            }
        }

        return pos;
    }
    
    private Vector2 WarpAttackSetPosition2()
    {
        Vector2 pos;
        ActorBase playerActor = _behavior.targetPlayer.GetComponent<ActorBase>();
        if (playerActor.transform.position.x < BattleStageManager.Instance.mapBorderL + 6)
        {
            pos = new Vector2(BattleStageManager.Instance.mapBorderL + 8, playerActor.transform.position.y- 0.3f);
        }
        else if (playerActor.transform.position.x > BattleStageManager.Instance.mapBorderR - 6)
        {
            pos = new Vector2(BattleStageManager.Instance.mapBorderR - 8, playerActor.transform.position.y- 0.3f);
        }
        else
        {
            if (playerActor.facedir == 1)
            {
                pos = new Vector2(playerActor.transform.position.x - 6, playerActor.transform.position.y- 0.3f);
            }
            else
            {
                pos = new Vector2(playerActor.transform.position.x + 6, playerActor.transform.position.y- 0.3f);
            }
        }

        pos = BattleStageManager.Instance.OutOfRangeCheck(pos);
        
        

        return pos;
    }

    private void WarpAttack()
    {
        InstantiateMeele(GetProjectileOfFormatName("action15_1", true),
            transform.position + new Vector3(0.5f, 1), InitContainer(true)).
            GetComponent<AttackFromEnemy>().AddMeeleTimeStopEffect(0.25f);
    }
    
    private void WarpAttackRanged()
    {
        InstantiateRanged(GetProjectileOfFormatName("action15_2", true),
            transform.position + new Vector3(0, 2), 
            InitContainer(false), ac.facedir,0).GetComponent<AttackFromEnemy>().
            AddMeeleTimeStopEffect(0.25f);;
    }

    private void FanAttack()
    {
        InstantiateRanged(GetProjectileOfFormatName("action18", true),
            transform.position, InitContainer(false),ac.facedir,0);
    }

    private void ChaserAttack()
    {
        var chaseBar = EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
            _behavior.targetPlayer.transform.position, RangedAttackFXLayer.transform,
            8, Vector2.zero, false, true, 3, .1f, 0.5f,
            true);
        var chaser = chaseBar.AddComponent<EnemyAttackHintBarChaser>();
        chaser.hardLock = true;
        chaser.target = _behavior.targetPlayer;
        chaser.SetLockTime(1.5f);
        chaser.moveable = true;

        DOVirtual.DelayedCall(3, () =>
        {
            var atk = InstantiateRanged(GetProjectileOfFormatName("action14_1", true),
                chaseBar.transform.position,InitContainer(false), 1).GetComponent<AttackFromEnemy>();

            atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Freeze,1,5,1),
                100);
        }, false);
    }

    private void RingDestruction()
    {
        var hint = GenerateWarningPrefab("action14_2", transform.position + new Vector3(0,1),
            Quaternion.identity, RangedAttackFXLayer.transform);

        DOVirtual.DelayedCall(1, () =>
        {
            var atk = InstantiateRanged(GetProjectileOfFormatName("action14_2", true),
                transform.position, InitContainer(false), 1).GetComponent<AttackFromEnemy>();

            atk.AddMeeleTimeStopEffect(0.3f);

            atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite, 120,
                21, 1), 100);
        },false);

    }

    
    /// <param name="type">为true时，横向为红色，竖向为紫色</param>
    private void CrossBatAttack(bool type, float fillTime = 1.5f)
    {
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,fillTime);
        var hintHorizontal = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector2(BattleStageManager.Instance.mapBorderL,
                _behavior.targetPlayer.transform.position.y), RangedAttackFXLayer.transform,
            new Vector2(BattleStageManager.Instance.mapBorderR - BattleStageManager.Instance.mapBorderL, 3),
            Vector2.zero, type, 1, fillTime,0,0.5f,true,
            false);
        
        var hintVertical = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector2(_behavior.targetPlayer.transform.position.x,BattleStageManager.Instance.mapBorderT), RangedAttackFXLayer.transform,
            new Vector2(BattleStageManager.Instance.mapBorderT - BattleStageManager.Instance.mapBorderB,3),
            Vector2.zero, !type, 1, fillTime,-90,0.5f,true,
            false);

        var chaserH = hintHorizontal.AddComponent<RelativePositionRetainerAdvanced>();
        var chaserV = hintVertical.AddComponent<RelativePositionRetainerAdvanced>();

        chaserH.lockType = RelativePositionRetainerAdvanced.LockType.Y;
        chaserV.lockType = RelativePositionRetainerAdvanced.LockType.X;
        
        chaserH.SetParent(_behavior.targetPlayer.transform);
        chaserV.SetParent(_behavior.targetPlayer.transform);
        chaserH.stopTime = fillTime - 0.25f;
        chaserV.stopTime = fillTime - 0.25f;

        DOVirtual.DelayedCall(fillTime + 0.1f, () =>
        {
            var batHPrefab = type
                ? GetProjectileOfFormatName("action19_1", true)
                : GetProjectileOfFormatName("action19_2", true);
            
            var batVPrefab = type
                ? GetProjectileOfFormatName("action19_2", true)
                : GetProjectileOfFormatName("action19_1", true);

            var container = InitContainer(false);
            int direction = Random.Range(0, 2) == 1 ? -1 : 1;
            float lengthX = BattleStageManager.Instance.mapBorderR - BattleStageManager.Instance.mapBorderL;
            float lengthY = BattleStageManager.Instance.mapBorderT - BattleStageManager.Instance.mapBorderB;
            
            var batH = InstantiateRanged(batHPrefab,
                direction < 0
                    ? new Vector2(BattleStageManager.Instance.mapBorderR, chaserH.transform.position.y)
                    : new Vector2(BattleStageManager.Instance.mapBorderL, chaserH.transform.position.y),
                container, direction);
            var batV = InstantiateRanged(batVPrefab,
                new Vector2(chaserV.transform.position.x,BattleStageManager.Instance.mapBorderT),
                container,1);

            batH.GetComponent<Rigidbody2D>()
                .DOMoveX(
                    direction < 0 ? BattleStageManager.Instance.mapBorderL : BattleStageManager.Instance.mapBorderR,
                    lengthX / 12).OnComplete(()=>Destroy(batH)).SetEase(Ease.Linear);

            batV.GetComponent<Rigidbody2D>()
                .DOMoveY(
                    BattleStageManager.Instance.mapBorderB - 3,
                    (3+lengthY) / 12).OnComplete(()=>Destroy(batV)).SetEase(Ease.Linear);

        }, false);


    }
    
    
    
    public override void DisappearRenderer()
    {
        base.DisappearRenderer();
        hellTuneInstance.SetActive(false);
        heavenTuneInstance.SetActive(false);
    }

    public override void AppearRenderer()
    {
        base.AppearRenderer();
        if (CurrentTune == 1)
        {
            hellTuneInstance.SetActive(true);
            heavenTuneInstance.SetActive(false);
        }
        else
        {
            hellTuneInstance.SetActive(false);
            heavenTuneInstance.SetActive(true);
        }
    }
}
