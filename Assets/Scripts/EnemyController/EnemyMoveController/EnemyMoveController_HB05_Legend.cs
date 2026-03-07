using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyMoveController_HB05_Legend : EnemyMoveController_HB05
{
    private int activatedFountainCount = 0;
    private List<StatusManager> fountainInstances = new();
    private List<float> fountainPositionPresets = new List<float> {0f, -10f, 10f};

    private bool hellActivated = false;
    private SpriteRenderer _backRenderer;
    private Color _normalColor = new Color(0,1,1);
    private Tweener _colorTween;
    private List<Color> _hellColors = new List<Color>
    {
        new Color(0,0.5f,0.8f),
        new Color(1,1,1),
        new Color(0.8f,0.6f,1),
        new Color(1,0.5f,1),
        new Color(1,0.3f,1),
        new Color(1,0.35f,0.45f),
        new Color(1,0.25f,0.3f),
        new Color(1,0.15f,0.15f),
        new Color(1,0,0)
    };
    private int _hellCount = 0;
    private int _hellLevel = 0;
    private bool _hellListenerSet = false;
    public int HellLevel => !hellActivated ? 0 : _hellLevel;
    

    private TimerBuff _hellDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.MaxHPDebuff,
        5, -1, 1, 8105403);
    
    private TimerBuff _devilsPact1 = new TimerBuff((int)BasicCalculation.BattleCondition.DevilsPactPositive,
        1, 30, 5, 8105404);
    
    private TimerBuff _priceOfPact1 = new TimerBuff((int)BasicCalculation.BattleCondition.DevilsPactNegative,
        1, 60, 5, 8105405);
    
    private TimerBuff _buffLegendP = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
        4, -1, 20, 8105501);
    
    
    
    

    protected override void Start()
    {
        base.Start();
        if (Projectile_C007_2_Boss.Instance != null)
        {
            Projectile_C007_2_Boss.Instance.SetEnemySource(gameObject);
        }
        
        ac?.SetHitSensor(false);

        _statusManager.OnReviveOrDeath += ResetBackground;
        _hellDebuff.dispellable = false;
        _backRenderer = BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background2") as SpriteRenderer;
    }

    public override IEnumerator HB05_Action06()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        ac.SetHitSensor(false);
        bossBanner?.PrintSkillName("HB05_Action06");
        StageCameraController.SwitchMainCameraFollowObject(gameObject);

        anim.Play("transform");
        
        voiceController?.BroadCastMyVoice((int)MyVoiceGroup.Transform);

        yield return new WaitForSeconds(0.5f);

        Instantiate(GetProjectileOfFormatName("action06_1"), transform.position + new Vector3(0, 2),
            Quaternion.identity, RangedAttackFXLayer.transform);
        (_behavior as HB05_BehaviorTree).dragonDrive = true;
        if (Projectile_C007_9_Boss.Instance == null)
        {
            RainEffect();
            yield return null;
            Projectile_C007_9_Boss.Instance.SetRainRate(20);
        }
        
        if (dragondriveFXInstance == null)
        {
            dragondriveFXInstance = Instantiate(GetProjectileOfFormatName("action06_2"),
                gameObject.RaycastedPosition(), Quaternion.identity, BuffFXLayer.transform);
        }
        else
        {
            dragondriveFXInstance.SetActive(true);
        }


        yield return new WaitForSeconds(2);
        
        StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
        ac.SetHitSensor(true);
        //anim.Play("idle");
        QuitAttack();
    }


    public IEnumerator HB05_Action15()
    {
        yield return _canAction;
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action15");
        
        if (_behavior.difficulty > 4)
        {
            _buffLegendP.dispellable = false;
            _statusManager.ObtainTimerBuff(new TimerBuff(_buffLegendP));
        }
        
        

        GameObject hint = null;

        if (Projectile_C007_7_Boss.Instance.IsActivated)
        {
            yield return null;
            anim.Play("fs_enter");
            yield return new WaitForSeconds(1);
            
            anim.Play("fs_exit");
            DestroyPlatform();
            
            yield return null;
            
            hint = GeneratePlatformHint();
            
            yield return new WaitForSeconds(1.5f);
            
            anim.Play("idle");

        }
        else
        {
            yield return new WaitForSeconds(1);
            
            hint = GeneratePlatformHint();
            
            yield return new WaitForSeconds(1.5f);
        }
        
        yield return new WaitForSeconds(1.5f);
        
        anim.Play("combo1");
        
        yield return new WaitForSeconds(1f);
        
        GeneratePlatform(hint.transform.position);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    public IEnumerator HB05_Action15_C()
    {
        yield return _canAction;
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        if (Projectile_C007_7_Boss.Instance.IsActivated)
        {
            yield return null;
            anim.Play("fs_enter");
            yield return new WaitForSeconds(1);
            
            anim.Play("fs_exit");
            DestroyPlatform();
            yield return null;
        }

        yield return new WaitForSeconds(1);
        
        if (Projectile_C007_2_Boss.Instance != null && _behavior.difficulty>4)
        {
            Projectile_C007_2_Boss.Instance.StopStorm();
            Projectile_C007_2_Boss.Instance.StopFogEffect();
        }
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    public IEnumerator HB05_Action16()
    {
        yield return _canAction;
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action16");
        
        yield return new WaitForSeconds(1);
        
        anim.Play("transform");
        
        voiceController?.BroadCastMyVoice((int)MyVoiceGroup.Heal);
        
        yield return new WaitForSeconds(1);

        if (activatedFountainCount < 3)
        {
            SummonFountain();
        }
        else
        {
            foreach (var fountainInstance in fountainInstances)
            {
                fountainInstance?.HPRegenImmediatelyWithoutRandomDirectly(fountainInstance,fountainInstance.maxHP);
            }
        }

        if (_behavior.difficulty > 4)
        {
            _buffLegendP.dispellable = false;
            _statusManager.ObtainTimerBuff(new TimerBuff(_buffLegendP));
        }


        yield return new WaitForSeconds(1);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// Hell
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB05_Action17()
    {
        yield return _canAction;
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action17");
        
        yield return new WaitForSeconds(1);
        
        anim.Play("transform");
        
        voiceController?.BroadCastMyVoice((int)MyVoiceGroup.World);

        if (!_hellListenerSet)
        {
            InitHellListener();
            _hellListenerSet = true;
        }
        
        yield return new WaitForSeconds(1);

        StartHell();

        yield return new WaitForSeconds(1);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    public IEnumerator HB05_Action18()
    {
        yield return _canAction;
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action18");
        
        yield return new WaitForSeconds(1);
        
        anim.Play("transform");
        
        
        yield return new WaitForSeconds(1);

        PauseHell();

        yield return new WaitForSeconds(1);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// Warp Attack
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB05_Action19()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        
        DisappearRenderer();
        ac.SwapWeaponVisibility(false);
        WarpEffectII();
        ac.SetHitSensor(false);
        ac.SetGravityScale(0);
        yield return new WaitForSeconds(0.2f);

        transform.position = _behavior.targetPlayer.RaycastedPosition() + new Vector2(0,8);
        anim.Play("smash");
        AppearRenderer();
        ac.SwapWeaponVisibility(true);
        WarpEffectII();
        ac.SetHitSensor(true);
        voiceController?.BroadCastMyVoice((int)MyVoiceGroup.Force);
        
        yield return new WaitForSeconds(0.3f);

        ac.ResetGravityScale();
        _tweener = transform.DOMoveY(transform.position.y - 6.5f, 0.3f).
            SetEase(Ease.OutSine);

        yield return new WaitForSeconds(0.3f);
        
        IceShardBlast();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    public IEnumerator HB05_Action20()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position+new Vector3(0,3));

        yield return new WaitForSeconds(0.5f);

        anim.Play("combo5");

        yield return new WaitForSeconds(1f);

        voiceController?.PlayMyVoice((int)MyVoiceGroup.Combo3);
        
        ComboAttackBoostType3();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();

    }
    
    public IEnumerator HB05_Action21()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(100);
        
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,transform.position+new Vector3(0,1),
            MeeleAttackFXLayer.transform, new Vector2(15,5),Vector2.zero, true,0,
            1.5f,0,0.5f,true,true);

        yield return new WaitForSeconds(0.8f);

        anim.Play("spin");

        yield return new WaitForSeconds(1f);

        voiceController?.PlayMyVoice((int)MyVoiceGroup.Combo1);
        var dir = ac.facedir;
        
        WaveForward();
        DOVirtual.DelayedCall(1f, () => IceShardForward(dir),false);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();

    }
    
    public IEnumerator HB05_Action22(int type)
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        if (type == 1)
        {
            bossBanner?.PrintSkillName("HB05_Action22");
        }
        else if (type == 2)
        {
            bossBanner?.PrintSkillName("HB05_Action23");
        }
        else
        {
            bossBanner?.PrintSkillName("HB05_Action24");
        }
        

        yield return new WaitForSeconds(0.5f);

        anim.Play("s2");
        
        PactEffect();
        if (type == 1)
        {
            SetPactBravery();
        }else if (type == 2)
        {
            SetPactTenacity();
        }
        else
        {
            SetPactVitality();
        }
        
        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();

    }
    
    
    
    public IEnumerator HB05_Action25(float delay = 10)
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action25");
        
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,2);

        yield return new WaitForSeconds(0.5f);

        anim.Play("combo5");

        yield return new WaitForSeconds(1.5f);
        
        DelayedMeteor(delay);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();

    }
    /// <summary>
    /// Around
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB05_Action26()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac, transform.position,
            MeeleAttackFXLayer.transform, 12, Vector2.zero, true, true, 2,
            0.15f, 1, true, true);

        yield return new WaitForSeconds(1.5f);
        anim.Play("float");
        yield return new WaitForSeconds(0.55f);
        
        ac.SetCounter(true);
        ac.SetKBRes(101);
        AroundIceShine();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();

    }
    
    /// <summary>
    /// Cascade
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB05_Action27()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action27");

        anim.Play("float");

        yield return new WaitForSeconds(0.5f);
        
        RandomCascade();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();

    }
    
    public IEnumerator HB05_Action28()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        ac.SetActionUnable(false);
        ac.SetHitSensor(false);
        WarpEffectII();
        
        yield return new WaitForSeconds(0.1f);
        
        DisappearRenderer();
        ac.SwapWeaponVisibility(false);
        
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(0.5f);
        
        PurgedShapeShiftingOfViewer();
        BattleStageManager.Instance.DragonBlock = true;
        
        yield return new WaitForSeconds(0.5f);

        AllRangedDebuff();
        
        yield return new WaitForSeconds(2f);

        BattleStageManager.Instance.PlayerViewEnable = false;
        var RTScene = CutsceneResourcesLoader.Instance.RTSceneGameObject;
        var fullscreenUI = CutsceneResourcesLoader.Instance.FullScreenRtuiGameObject;
        var cutsceneController = RTScene.GetComponent<CutSceneController_HB05>();
        cutsceneController.SetController(this);
        var Texture = cutsceneController.rt;
        var rawImg = fullscreenUI.GetComponent<RawImage>();
        RTScene.SetActive(true);
        yield return null;
        cutsceneController.Replay();
        rawImg.texture = Texture;
        yield return null;
        
        fullscreenUI.SetActive(true);
        
        yield return new WaitForSeconds(1.5f);
        
        bossBanner?.PrintSkillName("HB05_Action28");
        
        yield return new WaitForSeconds(1.5f);
        
        voiceController?.BroadCastMyVoice((int)(MyVoiceGroup.Ultimate));
        
        yield return new WaitForSeconds(3f);
        
        SummonOrbGroup();
        
        yield return new WaitForSeconds(1f);
        
        var orbGroup = Projectile_C007_8_Boss.Instance;
        orbGroup.SetEnemySource(gameObject);

        anim.Play("special_loop");
        
        ac.SetGravityScale(0);
        AppearRenderer();
        transform.position = new Vector2(0, 9);
        yield return null;
        TurnTo180Degree();
        
        fullscreenUI.SetActive(false);
        rawImg.texture = null;
        RTScene.SetActive(false);
        BattleStageManager.Instance.PlayerViewEnable = true;
        BattleStageManager.Instance.DragonBlock = false;

        if (Projectile_C007_7_Boss.Instance.IsActivated)
        {
            DestroyPlatform();
        }
        yield return new WaitForSeconds(1f);

        var hint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector3(0,BattleStageManager.Instance.mapBorderB),
            RangedAttackFXLayer.transform, new Vector2(14, 22), Vector2.zero, false,
            1, 4,90,0.5f, true,false);
        DOVirtual.DelayedCall(4.1f, () => GeneratePlatform
            (new Vector3(0, BattleStageManager.Instance.mapBorderB)), false);
        
        yield return new WaitForSeconds(1f);

        var rand = Projectile_C007_8_Boss.Instance.LaunchFirstTime();
        
        yield return new WaitForSeconds(5.5f);
        
        Projectile_C007_8_Boss.Instance.LaunchSecondTime(rand);
        
        yield return new WaitForSeconds(6f);
        
        Projectile_C007_8_Boss.Instance.LaunchOneByOne();
        
        yield return new WaitForSeconds(12f);

        for(int i = 0; i < 5; i++)
        {
            FlashEffect(Projectile_C007_8_Boss.Instance.transform.GetChild(i).position,2.5f);
        }

        yield return null;
        
        Destroy(Projectile_C007_8_Boss.Instance.gameObject);
        var superDamageBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageUp,
            200, 5, 1, 8105408);
        superDamageBuff.dispellable = false;
        _statusManager.ObtainTimerBuff(superDamageBuff);
        
        yield return new WaitForSeconds(.8f);

        if (Projectile_C007_7_Boss.Instance.IsActivated)
        {
            DestroyPlatform(true);
        }
        
        //todo: Add attacks
        StageCameraController.SwitchMainCamera();
        
        voiceController?.PlayMyVoice((int)(MyVoiceGroup.Combo1));
        
        WarpEffectII();
        yield return new WaitForSeconds(0.1f);
        DisappearRenderer();
        
        yield return new WaitForSeconds(0.2f);
        
        anim.Play("smash");
        AppearRenderer();
        ac.SwapWeaponVisibility(true);
        WarpEffectII();

        yield return new WaitForSeconds(0.3f);

        ac.ResetGravityScale();
        _tweener = transform.DOMoveY(BattleStageManager.Instance.mapBorderB+1.3f, 0.4f).
            SetEase(Ease.OutSine);

        yield return new WaitForSeconds(0.4f);
        
        IceShardBlast();
        ac.SetHitSensor(true);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");

        QuitAttack();

    }


    public IEnumerator HB05_Action29()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action29");

        anim.Play("s2");

        yield return new WaitForSeconds(0.5f);

        SummonSpinSpout();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();
    }
    
    public IEnumerator HB05_Action30()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action30");

        yield return new WaitForSeconds(1f);

        anim.Play("fs_enter");

        yield return new WaitForSeconds(2f);

        VitalityExchangeEffect();
        
        yield return new WaitForSeconds(2f);
        
        anim.Play("fs_exit");
        
        VitalityExchange();
        
        yield return new WaitForSeconds(0.5f);
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();
    }
    
    public IEnumerator HB05_Action31()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action31");

        yield return null;

        anim.Play("fs_enter");

        yield return new WaitForSeconds(1f);

        EternalFrost();
        
        yield return new WaitForSeconds(2f);
        
        anim.Play("fs_exit");
        
        yield return new WaitForSeconds(0.5f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();
    }
    
    
    
    
    
    
    private void DestroyPlatform(bool superBoost=false)
    {
        var destroyFx = InstantiateRanged(GetProjectileOfFormatName("action15_2"),
            Projectile_C007_7_Boss.Instance.Position-new Vector2(0,6), InitContainer(false),1);
        DOVirtual.DelayedCall(0.1f,
            ()=>Projectile_C007_7_Boss.Instance.DestroyIcePlatform(),false);

        if (superBoost)
        {
            destroyFx.GetComponent<AttackFromEnemy>().attackInfo[0].dmgModifier[0] = 4.54f;
        }
        
    }
    
    private GameObject GeneratePlatformHint()
    {
        var hint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            _behavior.targetPlayer.transform.position,
            RangedAttackFXLayer.transform, new Vector2(14, 22), Vector2.zero, false,
            1, 4,90,0.5f, true,false);

        var chaser = hint.AddComponent<EnemyAttackHintBarTopDownChaser>();
        
        chaser.target = _behavior.targetPlayer;
        chaser.SetHardLock(true);
        chaser.SetLockTime(1);
        chaser.SetUseCastPlatformY(true);
        
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,1.1f);
        
        return hint;
    }
    
    private void GeneratePlatform(Vector2 pos)
    {
        var destroyFx = InstantiateRanged(GetProjectileOfFormatName("action15_1"),
            pos, InitContainer(false),1);
        
        DOVirtual.DelayedCall(0.1f,
            ()=>Projectile_C007_7_Boss.Instance.GenerateIcePlatform(pos+new Vector2(0,6)),false);
    }

    private void SummonFountain()
    {
        var prefab = GetProjectileOfFormatName("action16_minion");
        

        var index = Random.Range(0, fountainPositionPresets.Count);
        var basePosition = fountainPositionPresets[index];
        if (fountainPositionPresets.Count > 0)
        {
            fountainPositionPresets.RemoveAt(index);
        }else
        {
            fountainPositionPresets = new List<float> {0f, -10f, 10f};
        }
        
        print(basePosition);
        
        var positionX = Mathf.Clamp(basePosition+Random.Range(-2,2),
            BattleStageManager.Instance.mapBorderL,BattleStageManager.Instance.mapBorderR);

        var minion = SpawnEnemyMinon(prefab, new Vector3(positionX,
                BattleStageManager.Instance.mapBorderB),
            _behavior.difficulty < 5 ? prefab.GetComponent<StatusManager>().maxBaseHP : 99999,
            1, 1);

        var status = minion.GetComponent<StatusManager>();
        
        fountainInstances.Add(status);
        ActivateFountain(true);

        
        
        status.OnReviveOrDeath += () =>
        {
            fountainInstances.Remove(status);
            fountainPositionPresets.Add(basePosition);
            ActivateFountain(false);
            RemoveOneBuffFromEveryTarget();
        };

    }

    private void RemoveOneBuffFromEveryTarget()
    {
        _statusManager.DispellTimerBuff();
        var targetPlayerStatus = _behavior.targetPlayer.GetComponent<StatusManager>();

        if (targetPlayerStatus.HasCondition(_priceOfPact1.buffID))
        {
            targetPlayerStatus.RemoveTimerBuff(_priceOfPact1.buffID, true, _priceOfPact1.specialID);
        }
        else targetPlayerStatus.DispellTimerBuff();
        
        
        _statusManager.ReliefDebuffExceptNilAndCorrosion();
        targetPlayerStatus.ReliefDebuffExceptNilAndCorrosion();
        
        
    }
    
    private void ActivateFountain(bool active)
    {
        if (active)
        {
            activatedFountainCount++;
        }else
        {
            activatedFountainCount--;
        }
        
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(
            HB05_BehaviorTree_Legend.ResurrectionSpring,new EnemyAbilityIconEvent(activatedFountainCount>0?1:0),_statusManager);

        if (activatedFountainCount<=0)
        {
            CancelInvoke("FountainHeal");
        }
        else if(active && activatedFountainCount == 1)//避免重复调用
        {
            InvokeRepeating("FountainHeal", 10, 10);
        }
    }
    
    private void FountainHeal()
    {
        if (GlobalController.currentGameState == GlobalController.GameState.End)
        {
            CancelInvoke("FountainHeal");
            return;
        }
        
        var targetStatus = _behavior.targetPlayer.GetComponent<StatusManager>();
        
        var dmgBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageUp,
            10, 60, 5, 8105402);
        dmgBuff.dispellable = false;

        if (targetStatus.maxHP == targetStatus.currentHp)
        {
            targetStatus.ObtainTimerBuff(new TimerBuff(dmgBuff), false,false);
        }
        
        if(_statusManager.maxHP == _statusManager.currentHp)
        {
            _statusManager.ObtainTimerBuff(new TimerBuff(dmgBuff), false,false);
        }
        
        
        fountainInstances[0].HPRegenImmediatelyWithoutRandomDirectly
            (_statusManager, (int)(_statusManager.maxBaseHP * 0.01f));
        
        fountainInstances[0].HPRegenImmediatelyWithoutRandomDirectly
            (targetStatus, 1000);
        
    }


    private void FlashEffect(Vector2 position, float scale = 1)
    {
        var go = Instantiate(GetProjectileOfFormatName("action19_1"),
            position, Quaternion.identity, 
            RangedAttackFXLayer.transform);
        go.transform.localScale = new Vector3(scale, scale);
        
    }
    private void WarpEffectII()
    {
        Instantiate(GetProjectileOfFormatName("action19_1"),
          transform.position, Quaternion.identity, 
          RangedAttackFXLayer.transform);
    }

    protected override void AcheronFountAttack(List<Vector3> posList, bool avoidable)
    {
        var moveDir = Random.Range(0, 2) == 0 ? 1 : -1;
        // if (_behavior.targetPlayer.transform.position.x < BattleStageManager.Instance.mapBorderL + 10)
        // {
        //     moveDir = -1;
        // }
        // else if (_behavior.targetPlayer.transform.position.x > BattleStageManager.Instance.mapBorderR - 10)
        // {
        //     moveDir = 1;
        // }
        
        
        var container = InitContainer(false);
        foreach (var pos in posList)
        {
            var proj = InstantiateRanged(GetProjectileOfFormatName("action07_1"),
                pos, container, 1);
            proj.GetComponent<DOTweenSimpleController>().moveDirection.x *= moveDir;
            proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
            (new TimerBuff((int)BasicCalculation.BattleCondition.Bog,1,8,1),
                100);
            if (!avoidable)
            {
                proj.GetComponent<AttackFromEnemy>().ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
            }
        }
        
        var playerPos = posList[^1];
        var newPosListX = new List<float>();
        
        List<float> typeA = new() { 23,19,15,11,7,-7,-11,-15,-19 };
        List<float> typeB = new() {    19,15,11,7,-7,-11,-15,-19,-23 };


        if (moveDir == -1)
        {
            newPosListX = typeA;
        }else
        {
            newPosListX = typeB;
        }
        
        DOVirtual.DelayedCall(5, () => AcheronFountBurst(newPosListX), false);

    }

    private void AcheronFountBurst(List<float> posListX)
    {
        var prefab = GetProjectileOfFormatName("action07_2");
        
        var container = InitContainer(false);

        foreach (var pos in posListX)
        {
            var proj = InstantiateRanged(GetProjectileOfFormatName("action07_2"),
                new Vector3(pos,BattleStageManager.Instance.mapBorderB), container, 1);
            proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
            (new TimerBuff((int)BasicCalculation.BattleCondition.Bog,1,12,1),
                100);
        }
        
    }

    private void IceShardBlast()
    {
        var container = InitContainer(false);
        
        var prefab1 = GetProjectileOfFormatName("action19_2");
        var prefab2 = GetProjectileOfFormatName("action19_3");

        InstantiateRanged(prefab1, new Vector3(transform.position.x,
            gameObject.RaycastedPlatform().bounds.max.y), container, 1);

        DOVirtual.DelayedCall(1.2f, () =>
        {
            var leftProjectile = InstantiateRanged(prefab2,
                new Vector3(transform.position.x - 6, BattleStageManager.Instance.mapBorderB),
                container, -1,0);
            
            var rightProjectile = InstantiateRanged(prefab2,
                new Vector3(transform.position.x + 6, BattleStageManager.Instance.mapBorderB),
                container, 1,0);

            leftProjectile.GetComponent<AttackFromEnemy>().attackInfo[0].knockbackDirection = new Vector2(-1, 1);

            leftProjectile.transform.GetChild(0).DOLocalMoveX(22, 0.15f);
            rightProjectile.transform.GetChild(0).DOLocalMoveX(22, 0.15f);
            
        }, false);
        
    }

    private void WaveForward()
    {
        var proj = InstantiateMeele(GetProjectileOfFormatName("action21_1", true),
            transform.position, InitContainer(true));
        
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(
            new TimerBuff((int)BasicCalculation.BattleCondition.Bog,1,8,1),
            100);
    }
    
    private void IceShardForward(int dir)
    {

        var proj = InstantiateRanged(GetProjectileOfFormatName("action19_3", true),
            new Vector3(transform.position.x, gameObject.RaycastedPosition().y),
            InitContainer(false), dir,0);

        proj.GetComponent<AttackFromEnemy>().attackInfo[0].knockbackDirection = new Vector2(dir, 1);

        proj.transform.GetChild(0).DOLocalMoveX(22, 0.15f);


    }
    
    protected override void ComboAttackBoostedType1(int num = 1)
    {
        var muzzleFX = Instantiate(GetProjectileOfFormatName("action01_2"),
            transform.position + new Vector3(ac.facedir, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);

        var container = InitContainer(false,num);
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action01_3"),
            transform.position + new Vector3(1.2f*ac.facedir, 0), container,
            ac.facedir);

        if (num > 1)
        {
            var proj2 = InstantiateRanged(GetProjectileOfFormatName("action01_3"),
                transform.position + new Vector3(1f*ac.facedir, -0.75f), container,
                ac.facedir);
            var pro3 = InstantiateRanged(GetProjectileOfFormatName("action01_3"),
                transform.position + new Vector3(1f*ac.facedir, 0.75f), container,
                ac.facedir);
        }
        
        var targetPos = _behavior.targetPlayer.transform.position;
        
        var muzzle1 = Instantiate(GetProjectileOfFormatName("action01_2"),
            targetPos + new Vector3(2*ac.facedir,8), Quaternion.identity,
            RangedAttackFXLayer.transform);

        var middleProjectile = InstantiateRanged(GetProjectileOfFormatName("action20_2"),
            targetPos + new Vector3(2*ac.facedir,8), container, 1);
        
        var directionMiddle = (targetPos - middleProjectile.transform.position).normalized;

        middleProjectile.transform.DOMove(middleProjectile.transform.position + directionMiddle * 15,
            0.5f).SetDelay(1);
        
    }
    
    protected override void ComboAttackBoostType2()
    {
        var muzzleFX = Instantiate(GetProjectileOfFormatName("action01_2"),
            transform.position + new Vector3(ac.facedir, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);

        var container = InitContainer(false);
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action01_4"),
            transform.position + new Vector3(ac.facedir, 0), container,
            ac.facedir);
        
        var targetPos = _behavior.targetPlayer.transform.position;

        var muzzle1 = Instantiate(GetProjectileOfFormatName("action01_2"),
            transform.position + new Vector3(-ac.facedir*3,6), Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var middleProjectile = InstantiateRanged(GetProjectileOfFormatName("action20_2"),
            transform.position + new Vector3(-ac.facedir*3,6), container, 1);
        
        var directionMiddle = (targetPos - middleProjectile.transform.position).normalized;

        middleProjectile.transform.DOMove(middleProjectile.transform.position + directionMiddle * 24,
            0.6f).SetDelay(1);
        
    }
    
    private void ComboAttackBoostType3()
    {
        var muzzlePrefab = GetProjectileOfFormatName("action01_2");
        
        var muzzleFX = Instantiate(muzzlePrefab,
            transform.position + new Vector3(ac.facedir, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);

        var container = InitContainer(false);
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action20_1"),
            transform.position + new Vector3(3*ac.facedir, 0), container,
            ac.facedir);
        
        var targetPos = _behavior.targetPlayer.transform.position;
        var projPrefab = GetProjectileOfFormatName("action20_2");
        
        
        
        var muzzle1 = Instantiate(muzzlePrefab,
            targetPos + new Vector3(0,8), Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var muzzle2 = Instantiate(muzzlePrefab,
            targetPos + new Vector3(-3,6), Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var muzzle3 = Instantiate(muzzlePrefab,
            targetPos + new Vector3(3,6), Quaternion.identity,
            RangedAttackFXLayer.transform);


        var middleProjectile = InstantiateRanged(projPrefab,
            targetPos + new Vector3(0,8), container, 1);
        
        var leftProjectile = InstantiateRanged(projPrefab,
            targetPos + new Vector3(-3,6), container, 1);
        
        var rightProjectile = InstantiateRanged(projPrefab, 
            targetPos + new Vector3(3,6), container, 1);
        
        var directionMiddle = (targetPos - middleProjectile.transform.position).normalized;
        var directionLeft = (targetPos - leftProjectile.transform.position).normalized;
        var directionRight = (targetPos - rightProjectile.transform.position).normalized;
        
        middleProjectile.transform.DOMove(middleProjectile.transform.position + directionMiddle * 15,
            0.5f).SetDelay(1);
        
        leftProjectile.transform.DOMove(leftProjectile.transform.position + directionLeft * 15,
            0.5f).SetDelay(1);
        
        rightProjectile.transform.DOMove(rightProjectile.transform.position + directionRight * 15,
            0.5f).SetDelay(1);

    }

    private void DelayedMeteor(float delay)
    {
        var position = _behavior.targetPlayer.RaycastedPosition();
        var target = _behavior.targetPlayer;
        var faceDir = ac.facedir;
        
        DOVirtual.DelayedCall(delay, () =>
        {
            var container = InitContainer(false);
            var proj = InstantiateRanged(GetProjectileOfFormatName("action25_1"),
                position, container, faceDir);
        },false);
        
    }

    private void StartHell()
    {
        hellActivated = true;
        
        BattleStageManager.Instance.InvokeEnemyAbilityEvent
            (HB05_BehaviorTree_Legend.CocytusTorture,new EnemyAbilityIconEvent(1),_statusManager);
        
        _hellLevel = GetHellLevel();
        RefreshInfo();

        if (Projectile_C007_9_Boss.Instance != null)
        {
            Projectile_C007_9_Boss.Instance.SetRainRate(25 + _hellLevel*5);
            Projectile_C007_9_Boss.Instance.PlayRainEffect();

            if (_hellLevel > 5)
            {
                Projectile_C007_9_Boss.Instance.SetStartColorGradient(Color.red);
            }
        }

        _colorTween = _backRenderer.DOColor(Color.black, 1)
            .OnComplete(()=>_backRenderer.DOColor(_hellColors[GetHellLevel() - 1], 1));
    }
    
    private void PauseHell()
    {
        hellActivated = false;
        
        BattleStageManager.Instance.InvokeEnemyAbilityEvent
            (HB05_BehaviorTree_Legend.CocytusTorture,new EnemyAbilityIconEvent(0),_statusManager);
        
        //_hellLevel = GetHellLevel();
        _hellLevel = 0;
        OnHellQuit();
        
        if (Projectile_C007_9_Boss.Instance != null)
        {
            Projectile_C007_9_Boss.Instance.SetRainRate(25 + _hellLevel*5);
            Projectile_C007_9_Boss.Instance.PlayRainEffect();

            if (_hellLevel > 5)
            {
                Projectile_C007_9_Boss.Instance.ResetStartColorGradient();
            }
        }

        _colorTween = _backRenderer.DOColor(Color.black, 1)
            .OnComplete(()=>_backRenderer.DOColor(_normalColor, 1));
    }

    private int GetHellLevel()
    {
        if (_hellCount < 200)
            return 1;
        else if (_hellCount < 300)
            return 2;
        else if (_hellCount < 400)
            return 3;
        else if (_hellCount < 500)
            return 4;
        else if (_hellCount < 600)
            return 5;
        else if (_hellCount < 700)
            return 6;
        else if (_hellCount < 800)
            return 7;
        else if (_hellCount < 900)
            return 8;
        else return 9;
    }
    
    private void InitHellListener()
    {
        var target = _behavior.viewerPlayer;
        
        var targetStats = target.GetComponent<StatusManager>();
        var targetActor = target.GetComponent<ActorController>();
        
        targetActor.OnJump += OnJumpListener;
        targetActor.OnRoll += OnRollListener;
        targetStats.OnBuffEventDelegate += OnInflictAfflictionListener;
        targetStats.OnTakeDirectDamageFrom += OnTakeDirectDamageListener;

        _statusManager.OnReviveOrDeath += RemoveAllListeners;
    }

    private void RefreshInfo()
    {
        StringBuilder sb = new();
        int displayedNumber = 0;

        if (_hellCount < 200)
        {
            sb.Append("1-");
            displayedNumber = Mathf.FloorToInt(_hellCount/20);
        }
        else if (_hellCount < 300)
        {
            sb.Append("2-");
            displayedNumber = Mathf.FloorToInt((_hellCount-200)/10);
        }
        else if (_hellCount < 400)
        {
            sb.Append("3-");
            displayedNumber = Mathf.FloorToInt((_hellCount-300)/10);
        }
        else if (_hellCount < 500)
        {
            sb.Append("4-");
            displayedNumber = Mathf.FloorToInt((_hellCount-400)/10);
        }
        else if (_hellCount < 600)
        {
            sb.Append("5-");
            displayedNumber = Mathf.FloorToInt((_hellCount-500)/10);
        }
        else if (_hellCount < 700)
        {
            sb.Append("6-");
            displayedNumber = Mathf.FloorToInt((_hellCount-600)/10);
        }
        else if (_hellCount < 800)
        {
            sb.Append("7-");
            displayedNumber = Mathf.FloorToInt((_hellCount-700)/10);
        }
        else if (_hellCount < 900)
        {
            sb.Append("8-");
            displayedNumber = Mathf.FloorToInt((_hellCount-800)/10);
        }
        else
        {
            sb.Append("9");
        }

        if (_hellCount < 900)
        {
            sb.Append(displayedNumber);
        }
        
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(HB05_BehaviorTree_Legend.CocytusTorture,
            new EnemyAbilityIconEvent(EnemyAbilityIconEvent.EventType.SetText, sb.ToString()),_statusManager);

        if (_hellLevel < GetHellLevel())
        {
            _hellLevel = GetHellLevel();
            _colorTween.Kill();
            _colorTween = _backRenderer.DOColor(_hellColors[_hellLevel-1], 3);
            OnHellLevelUp();
            if (_hellCount > 700)
            {
                BattleStageManager.Instance.InvokeEnemyAbilityEvent(HB05_BehaviorTree_Legend.CocytusTorture,
                    new EnemyAbilityIconEvent((ui) =>
                    {
                        ui.AbilityExtraMessage.color = Color.red;
                    }),_statusManager);
            }
            else if (_hellCount > 400)
            {
                BattleStageManager.Instance.InvokeEnemyAbilityEvent(HB05_BehaviorTree_Legend.CocytusTorture,
                    new EnemyAbilityIconEvent((ui) =>
                    {
                        ui.AbilityExtraMessage.color = Color.yellow;
                    }),_statusManager);
            }
        }
        

    }

    private void OnHellLevelUp()
    {
        int effect = 5;
        int cap = BasicCalculation.HEAL_CAP;
        
        switch(_hellLevel)
        {
            case 1:
                cap = 77777;
                break;
            case 2:
                cap = 33333;
                break;
            case 3:
                cap = 11111;
                break;
            case 4:
                cap = 7777;
                effect = 10;
                break;
            case 5:
                cap = 3333;
                effect = 15;
                break;
            case 6:
                cap = 1111;
                effect = 20;
                break;
            case 7:
                cap = 777;
                effect = 25;
                break;
            case 8:
                cap = 333;
                effect = 30;
                break;
            case 9:
                effect = 30;
                cap = 111;
                break;
            default:break;
     
        }
        
        _hellDebuff.SetEffect(effect);

        StatusManager playerStats = _behavior.viewerPlayer.GetComponent<StatusManager>();

        if (_hellLevel >= 2)
        {
            var playerDebuff = playerStats.GetConditionWithSpecialID(8102503);

            if (playerDebuff.Count == 0)
            {
                playerStats.ObtainTimerBuff(new TimerBuff(_hellDebuff),false,false);
            }
            else
            {
                playerDebuff[0].SetEffect(_hellDebuff.effect);
                playerStats.OnBuffEventDelegate?.Invoke(playerDebuff[0]);
            }
            
            var selfDebuff = _statusManager.GetConditionWithSpecialID(8102503);
            
            if (selfDebuff.Count == 0)
            {
                _statusManager.ObtainTimerBuff(new TimerBuff(_hellDebuff),false,false);
            }
            else
            {
                selfDebuff[0].SetEffect(_hellDebuff.effect);
                _statusManager.OnBuffEventDelegate?.Invoke(selfDebuff[0]);
            }
            
        }
        
        playerStats.healCap = cap;
        _statusManager.healCap = 2 * cap * Mathf.Clamp((10 - _hellLevel),1,9);
        if (_behavior.difficulty >= 5)
        {
            _statusManager.healCap *= 5;
        }
        
        
    }

    private void OnHellQuit()
    {
        StatusManager playerStats = _behavior.viewerPlayer.GetComponent<StatusManager>();

        playerStats.RemoveAllConditionWithSpecialID(_hellDebuff.specialID);
        playerStats.healCap = BasicCalculation.HEAL_CAP;
        
        _statusManager.RemoveAllConditionWithSpecialID(_hellDebuff.specialID);
        _statusManager.healCap = BasicCalculation.HEAL_CAP;
    }
    
    private void RemoveAllListeners()
    {
        _statusManager.OnReviveOrDeath -= RemoveAllListeners;
        
        var target = _behavior.viewerPlayer;
        
        var targetStats = target.GetComponent<StatusManager>();
        var targetActor = target.GetComponent<ActorController>();
        
        targetActor.OnJump -= OnJumpListener;
        targetActor.OnRoll -= OnRollListener;
        targetStats.OnBuffEventDelegate -= OnInflictAfflictionListener;
        targetStats.OnTakeDirectDamageFrom -= OnTakeDirectDamageListener;
    }

    private void OnJumpListener(ActorController ac, int count)
    {
        if (hellActivated && _hellCount < 900)
        {
            _hellCount += 5;
            RefreshInfo();
        }
    }
    private void OnRollListener(ActorController ac)
    {
        if (hellActivated && _hellCount < 900)
        {
            _hellCount += 2;
            RefreshInfo();
        }
    }

    private void OnInflictAfflictionListener(BattleCondition condition)
    {
        if (StatusManager.IsAffliction(condition.buffID))
        {
            if (hellActivated && _hellCount < 900)
            {
                _hellCount += 20;
                RefreshInfo();
            }
        }
        
        
    }
    
    private void OnTakeDirectDamageListener(StatusManager statA,StatusManager statB,AttackBase atk,float dmg)
    {
        if (hellActivated && _hellCount < 900)
        {
            if(dmg < 500)
                _hellCount += 3;
            else if (dmg < 1000)
                _hellCount += 5;
            else if (dmg < 2000)
                _hellCount += 10;
            else _hellCount += 15;
            RefreshInfo();
        }
    }

    private void PactEffect()
    {
        Instantiate(GetProjectileOfFormatName("action22_1", true),_behavior.targetPlayer.transform.position,
            Quaternion.identity, _behavior.targetPlayer.transform);
    }
    
    private void PactEffect2()
    {
        Instantiate(GetProjectileOfFormatName("action22_2", true),_behavior.targetPlayer.transform.position,
            Quaternion.identity, _behavior.targetPlayer.transform);
    }

    private void SetPactBravery()
    {
        var buff = new TimerBuff(_devilsPact1);
        buff.dispellable = false;

        var atkbuff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageUp,
            50, -1, 1, 8105406);
        atkbuff.dispellable = false;

        var debuff = new TimerBuff(_priceOfPact1);
        debuff.SetTickInterval(4.9f);
        
        var defdebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
            40, 30, 5, 8105407);
        
        debuff.OnBuffUpdate += (stat) =>
        {
            stat.ObtainTimerBuff(new TimerBuff(defdebuff),false,false);
        };
        
    
        buff.OnBuffStart += (stat) =>
        {
            stat.ObtainTimerBuff(atkbuff,false,false);
        };
        
        buff.OnBuffRemove += (stat) =>
        {
            stat.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.DamageUp,8105406);
            stat.ObtainTimerBuff(debuff,false,false);
            stat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefDebuff, 10, 5);
            PactEffect2();
        };
        
        _behavior.targetPlayer.GetComponent<StatusManager>().ObtainTimerBuff(buff,true,false);
        
        
        
    }
    
    private void SetPactTenacity()
    {
        var buff = new TimerBuff(_devilsPact1);
        buff.dispellable = false;

        var defbuff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageCutConst,
            500, -1, 1, 8105406);
        defbuff.dispellable = false;

        var debuff = new TimerBuff(_priceOfPact1);
        debuff.SetTickInterval(4.9f);
        
        var dmgdebuff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageDown,
            15, 30, 5, 8105407);
        
        debuff.OnBuffUpdate += (stat) =>
        {
            stat.ObtainTimerBuff(new TimerBuff(dmgdebuff),false,false);
        };
        
    
        buff.OnBuffStart += (stat) =>
        {
            stat.ObtainTimerBuff(defbuff,false,false);
        };
        
        buff.OnBuffRemove += (stat) =>
        {
            stat.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.DamageCutConst,8105406);
            stat.ObtainTimerBuff(debuff,false,false);
            stat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff, 10, 5);
            PactEffect2();
        };
        
        _behavior.targetPlayer.GetComponent<StatusManager>().ObtainTimerBuff(buff,true,false);
        
        
        
    }
    
    private void SetPactVitality()
    {
        var buff = new TimerBuff(_devilsPact1);
        buff.dispellable = false;

        var healbuff = new TimerBuff((int)BasicCalculation.BattleCondition.HealOverTime,
            20, -1, 1, 8105406);
        healbuff.dispellable = false;

        var debuff = new TimerBuff(_priceOfPact1);
        debuff.SetTickInterval(4.9f);

        var baseModifier = 0.5f * _behavior.difficulty;
        
        List<TimerBuff> afflictions = new List<TimerBuff>();
        
        var burn = new TimerBuff((int)BasicCalculation.BattleCondition.Burn,
            72 * baseModifier, 12, 1);
        var paralyze = new TimerBuff((int)BasicCalculation.BattleCondition.Paralysis,
            57.6f * baseModifier, 15, 1);
        var scorchrend = new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
            41.4f * baseModifier, 21, 1);
        var poison = new TimerBuff((int)BasicCalculation.BattleCondition.Poison,
            57.6f * baseModifier, 15, 1);
        var shadowblight = new TimerBuff((int)BasicCalculation.BattleCondition.ShadowBlight,
            41.4f * baseModifier, 21, 1);
        var frostbite = new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,
            41.4f * baseModifier, 21, 1);
        
        afflictions.Add(burn);
        afflictions.Add(paralyze);
        afflictions.Add(scorchrend);
        afflictions.Add(poison);
        afflictions.Add(shadowblight); 
        afflictions.Add(frostbite);


        debuff.OnBuffUpdate += (stat) =>
        {
            var affliction = afflictions[Random.Range(0, afflictions.Count)];
            stat.ObtainTimerBuff(affliction,false,false);
            
            if(afflictions.Count > 1)
                afflictions.Remove(affliction);
            else
                afflictions = new List<TimerBuff>()
                {
                    burn,
                    paralyze,
                    scorchrend,
                    poison,
                    shadowblight,
                    frostbite
                };
        };
        
    
        buff.OnBuffStart += (stat) =>
        {
            stat.ObtainTimerBuff(healbuff,false,false);
        };
        
        buff.OnBuffRemove += (stat) =>
        {
            stat.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.HealOverTime,8105406);
            stat.ObtainTimerBuff(debuff,false,false);
            stat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.FrostbiteResDown, 100, 5);
            PactEffect2();
        };
        
        _behavior.targetPlayer.GetComponent<StatusManager>().ObtainTimerBuff(buff,true,false);
        
        
        
    }

    private void AroundIceShine()
    {
        var projectile = GetProjectileOfFormatName("action26_1", true);

        var proj = InstantiateMeele(projectile, transform.position, InitContainer(true));
        
        var atk = proj.GetComponent<AttackFromEnemy>();
        atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,44,21,1),120);

    }
    
    private void RandomCascade()
    {
        var projectile = GetProjectileOfFormatName("action27_1", true);
        
        var direction = Random.Range(0, 2) == 0 ? -1 : 1;
        
        var position = direction > 0 ? new Vector3(12, 10) : new Vector3(-12, 10);

        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector3(position.x, BattleStageManager.Instance.mapBorderB),
            RangedAttackFXLayer.transform, new Vector2(20, 24), Vector2.zero,
            false, 1, 3.5f, 90, 0.5f,
            true, false);
        
        DOVirtual.DelayedCall(3.5f, () =>
        {
            var proj = InstantiateRanged(projectile, position, InitContainer(false),1);
            var atk = proj.GetComponent<AttackFromEnemy>();
            atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,44,21,1),120);
        },false);
        
    }

    private void AllRangedDebuff()
    {

        TimerBuff spDegen = new TimerBuff((int)BasicCalculation.BattleCondition.SPDegen,
            30, 9,100);
        
        spDegen.OnBuffUpdate += (stat) =>
        {
            var playerStats = stat.GetComponent<PlayerStatusManager>();
            if (playerStats)
            {
                for (int i = 0; i < 4; i++)
                {
                    playerStats.currentSP[i] -= playerStats.requiredSP[i] * 0.3f;
                }
            }
        };
        
        spDegen.SetTickInterval(2.9f);
        
        var fx = InstantiateRanged(GetProjectileOfFormatName("action28_3"),
            new Vector3(0, BattleStageManager.Instance.mapBorderB),
            InitContainer(false),1);

        fx.GetComponent<AttackFromEnemy>().attackInfo[0].constDmg[0] = _hellCount;
        
        _behavior.targetPlayer.GetComponent<StatusManager>().ObtainTimerBuff(spDegen,true,false);

    }

    private void SummonOrbGroup()
    {
        var orbGroupPrefab = GetProjectileOfFormatName("action28_1", true);

        InstantiateSealedContainer(orbGroupPrefab, new Vector3(0,-2), RangedAttackFXLayer.transform, 1);

    }

    private void SummonSpinSpout()
    {
        var position = new Vector2(transform.position.x, BattleStageManager.Instance.mapBorderB + 6.5f);
        position.x = Mathf.Clamp(position.x, -4, 4);
        
        var origin = Instantiate(GetProjectileOfFormatName("action29_1"),
            position, Quaternion.identity);

        origin.AddComponent<ObjectInvokeDestroy>().destroyTime = 15.5f;

        var angle = ObjectExtensions.AngleDegree(origin.transform, _behavior.targetPlayer.transform);
        
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,origin.transform.position,
            RangedAttackFXLayer.transform,
            new Vector2(36,3),Vector2.zero, false,1,2,angle,0.4f,
            true,false);
        
        DOVirtual.DelayedCall(2.1f, () =>
        {
            var proj = InstantiateDirectionalRanged(GetProjectileOfFormatName("action29_2"),
                origin.transform.position, InitContainer(false),1,angle);

            var angleZ = angle;
            
            //proj旋转一圈，逆时针顺时针随机,使用DoTween.TO方法
            DOTween.To(() => angleZ, 
                    x => angleZ = x, angle + (Random.Range(0,2) == 0 ? 359 : -359), 12f)
                .SetDelay(0.9f).SetEase(Ease.Linear).OnUpdate(() =>
                {
                    proj.transform.rotation = Quaternion.Euler(0,0,angleZ);
                });



            // proj.transform.DORotate(new Vector3(0,0,angle + Random.Range(0,2) == 0 ? 359 : -359),
            //     12f, RotateMode.FastBeyond360).SetDelay(0.9f).SetEase(Ease.Linear);
            proj.AddComponent<ObjectInvokeDestroy>().destroyTime = 13;
            proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
            (new TimerBuff((int)BasicCalculation.BattleCondition.Bog,1,8,1),
                100);
        },false);


    }

    private void EternalFrost()
    {
        var frostPrefab = GetProjectileOfFormatName("action31_2", true);
        var pillarPrefab = GetProjectileOfFormatName("action31_1", true);
        
        var hint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector3(_behavior.targetPlayer.transform.position.x,
                BattleStageManager.Instance.mapBorderB),
            RangedAttackFXLayer.transform, new Vector2(8, 4), Vector2.zero,
            true, 0, 2.5f, 90, 0.5f,
            true, false);

        var chaser = hint.AddComponent<EnemyAttackHintBarTopDownChaser>();

        chaser.target = _behavior.targetPlayer;
        chaser.SetMoveSpeedY(0);
        chaser.SetLockTime(1.95f);
        chaser.SetHardLock(false);
        chaser.SetMoveSpeedX(9);
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,1.9f);

        DOVirtual.DelayedCall(2.5f, () =>
        {
            var pos = hint.transform.position;
            var container = InitContainer(false, 2);
            var proj = InstantiateRanged(pillarPrefab, pos,container,1);
            var trap = InstantiateRanged(frostPrefab, pos,container,1);
        },false);


    }


    private void VitalityExchangeEffect()
    {
        var fx = GetProjectileOfFormatName("action30_1", true);

        Instantiate(fx, transform.position + new Vector3(ac.facedir * 1.5f, 1.2f), 
            Quaternion.identity,RangedAttackFXLayer.transform);
    }

    private void VitalityExchange()
    {
        if (hellActivated && _hellCount < 900)
        {
            _hellCount += 100;
            RefreshInfo();
        }
        
        var targetStat = _behavior.targetPlayer.GetComponent<PlayerStatusManager>();
        var fx= GetProjectileOfFormatName("action30_3",true);
        
        var proj = InstantiateRanged(fx,
            new Vector3(0,
                BattleStageManager.Instance.mapBorderB), InitContainer(false),1);
        
        var forcedAttack = proj.GetComponent<ForcedAttackFromEnemy>();

        forcedAttack.triggerTime = 0.2f;
        forcedAttack.target = _behavior.targetPlayer;
        forcedAttack.AddConditionalAttackEffect(
            new ConditionalAttackEffect((src, tar) => { 
                    return true; 
            },
                ConditionalAttackEffect.ExtraEffect.Custom,new string[]{},new string[]{}).
                SetEffectFunction((stats,atk) =>
                {
                    if (_statusManager.currentHp < _statusManager.maxHP * 0.1f)
                    {
                        _statusManager.HPRegenImmediatelyWithoutRandomDirectly(_statusManager,
                            (int)(_statusManager.maxHP * 0.1f)-_statusManager.currentHp);
                    }
                    
                    if (stats.targetStat.currentHp <= stats.targetStat.maxHP * 0.5f)
                    {
                        return 0;
                    }
                    else
                    {
                        print((int)(stats.targetStat.maxHP * 0.5f - stats.targetStat.currentHp));
                        return -(int)(stats.targetStat.maxHP * 0.5f - stats.targetStat.currentHp);
                    }
                }
                    )
            );
        
        
        
        DOVirtual.DelayedCall(0.5f, () =>
        {
            var selfPercentage = ((float)_statusManager.currentHp / (float)_statusManager.maxHP);
            var targetPercentage = ((float)targetStat.currentHp / (float)targetStat.maxHP);
            
            var selfHP = _statusManager.maxHP * targetPercentage;
            var targetHP = targetStat.maxHP * selfPercentage;
            
            
            var debuff = new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                1,60,1,8105409);
            debuff.dispellable = false;

            if (selfPercentage - targetPercentage > 0.5f)
            {
                debuff.SetEffect(50);
                targetStat.ObtainTimerBuff(debuff);
            }else if (selfPercentage - targetPercentage > 0.3f)
            {
                debuff.SetEffect(30);
                targetStat.ObtainTimerBuff(debuff);
            }
            else if (selfPercentage - targetPercentage > 0)
            {
                debuff.SetEffect(10);
                targetStat.ObtainTimerBuff(debuff);
            }
            else if (targetPercentage - selfPercentage > 0.5f)
            {
                debuff.SetEffect(50);
                _statusManager.ObtainTimerBuff(debuff);
            }
            else if (targetPercentage - selfPercentage > 0.3f)
            {
                debuff.SetEffect(30);
                _statusManager.ObtainTimerBuff(debuff);
            }
            else if (targetPercentage - selfPercentage >= 0)
            {
                debuff.SetEffect(10);
                _statusManager.ObtainTimerBuff(debuff);
            }
            
            if (selfHP > _statusManager.currentHp)
            {
                _statusManager.HPRegenImmediatelyWithoutRandomDirectly(_statusManager,
                    (int)selfHP - _statusManager.currentHp);
            }
            else
            {
                BattleStageManager.Instance.CauseIndirectDamage(_statusManager,
                    _statusManager.currentHp - (int)selfHP, false,false,true);
            }
            
            if (targetHP > targetStat.currentHp)
            {
                targetStat.HPRegenImmediatelyWithoutRandomDirectly(targetStat,
                    (int)targetHP - targetStat.currentHp);
            }
            else
            {
                BattleStageManager.Instance.CauseIndirectDamage(targetStat,
                    targetStat.currentHp - (int)targetHP, false,false,true);
            }
            
            var fx2 = GetProjectileOfFormatName("action30_2",true);
            var fxSelf = Instantiate(fx2,transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
            var fxTarget = Instantiate(fx2,targetStat.transform.position,
                Quaternion.identity,RangedAttackFXLayer.transform);
            
            fxSelf.AddComponent<RelativePositionRetainer>().SetParent(transform);
            fxTarget.AddComponent<RelativePositionRetainer>().SetParent(targetStat.transform);
            
            
            
        },false);
        
    }
    

    private void TurnTo180Degree()
    {
        anim.transform.localRotation = Quaternion.Euler(0, 180, 0);
    }
    
    private void TurnToNormalDegree()
    {
        anim.transform.localRotation = Quaternion.Euler(0, 102, 0);
    }

    public override void AppearRenderer()
    {
        base.AppearRenderer();

        if ((_behavior as HB05_BehaviorTree).dragonDrive && dragondriveFXInstance!= null)
        {
            dragondriveFXInstance.SetActive(true);
        }
    }
    
    public override void DisappearRenderer()
    {
        base.DisappearRenderer();

        if (dragondriveFXInstance != null)
        {
            dragondriveFXInstance.SetActive(false);
        } 
    }

    private void ResetBackground()
    {
        _statusManager.OnReviveOrDeath -= ResetBackground;
        
        foreach (var stat in fountainInstances)
        {
            stat.currentHp = 0;
        }

        var bg1 = 
            BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background1");
        
        (bg1 as SpriteRenderer).color = Color.white;
        
        var bg2 = 
            BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background2");
        
        var phase1 = BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Phase1");
        var phase2 = BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Phase2");
        
        phase1.gameObject.SetActive(true);
        phase2.gameObject.SetActive(false);

        (bg2 as SpriteRenderer).DOColor(Color.clear,1);

        if (Projectile_C007_7_Boss.Instance != null)
        {
            if (Projectile_C007_7_Boss.Instance.IsActivated)
            {
                DestroyPlatform();
            }
            
        }

    }
}
