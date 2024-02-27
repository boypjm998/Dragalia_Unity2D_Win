using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyMoveController_HB04_Legend : EnemyMoveController_HB04
{
    private TimerBuff _resistanceStd;
    private TimerBuff _resistanceDFs;
    private TimerBuff _resistanceSkl;
    private TimerBuff _resistanceOth;
    private TimerBuff _spBuff;
    
    private Color normalColor = new Color(0.4f,0.8f,1f);
    private Color nightColor = new Color(0.2f,0.4f,1f);
    private Color brightColor = new Color(0.8f, 0.8f, 0.8f);

    private const int FieldAbility_HB04 = 20151;

    private List<UI_HB004_Legend_01> controllers = new();

    public bool WorldReset => controllers[0].IsActive ? false: true;

    protected override void OnInit()
    {
        var bg = BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background1");
        (bg as SpriteRenderer).color = normalColor;
        if(_behavior.difficulty > 4)
            BattleStageManager.Instance.OnGameStart += InitFieldAbility;
        else InitFieldAbility();
        BattleStageManager.Instance.OnFieldAbilityRemove += ClearInvoke;
        _statusManager.OnReviveOrDeath += (()=>
        {
            voice?.BroadCastMyVoice((int)MyVoiceGroup.Defeat);
            var bg1 = BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background1");
            (bg1 as SpriteRenderer).DOColor( Color.clear,1);
            var bg2 = BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background2");
            (bg2 as SpriteRenderer).color = Color.white;
            var environmentRenderers = 
                BattleEnvironmentManager.Instance.GetAllEnvironmentRenderer();
            foreach (var renderer in environmentRenderers)
            {
                if (renderer.name == "Background2")
                {
                    //(renderer as SpriteRenderer).color = new Color(1, 1, 1, 0);
                }else if (renderer.name == "Background1")
                {
                    //(renderer as SpriteRenderer).color = new Color(1, 1, 1, 1);
                }else if (renderer.name.StartsWith("eff"))
                {
                    renderer.gameObject.SetActive(false);
                }
                else
                {
                    renderer.gameObject.SetActive(true);
                }
            }

        });
    }

    

    private void InitFieldAbility()
    {
        BattleStageManager.Instance.OnGameStart -= InitFieldAbility;
        DOVirtual.DelayedCall(0.1f,
            ()=>
            {
                InitTimerBuffs();
                BattleStageManager.Instance.RemoveFieldAbility(FieldAbility_HB04);
            },false);
        //BattleStageManager.Instance.AddFieldAbility(FieldAbility_HB04);
        //StartBuffTick();
    }

    private void StartBuffTick()
    {
        foreach (var controller in controllers)
        {
            controller.attachedStatus.ObtainTimerBuff(_resistanceStd);
        }

        controllers[0].OnTimer += CheckBuff;
    }
    
    //todo: 在场地结束时，添加stopBuffTick
    private void CheckBuff()
    {
        foreach (var controller in controllers)
        {
            var atkInfo = controller.CheckReceivedAttack(true);
            var atkType = atkInfo.Item1;
            var src = atkInfo.Item2;
            if (controller.IsActive && atkType >= 0)
            {
                controller.attachedStatus.RemoveAllConditionWithSpecialID(810441);
                var atkTypeEnum = (BasicCalculation.AttackType)atkType;
                int currentBuff = controller.CurrentBuff;
                bool success = false;

                switch (atkTypeEnum)
                {
                    case BasicCalculation.AttackType.STANDARD:
                    case BasicCalculation.AttackType.DSTANDARD:
                        controller.attachedStatus.ObtainTimerBuff(_resistanceStd);
                        if (currentBuff == 1)
                            success = true;
                        break;
                    case BasicCalculation.AttackType.DASH:
                    case BasicCalculation.AttackType.FORCE:
                        controller.attachedStatus.ObtainTimerBuff(_resistanceDFs);
                        if (currentBuff == 2)
                            success = true;
                        break;
                    case BasicCalculation.AttackType.SKILL:
                    case BasicCalculation.AttackType.DSKILL:
                        controller.attachedStatus.ObtainTimerBuff(_resistanceSkl);
                        if (currentBuff == 3)
                            success = true;
                        break;
                    default:
                        controller.attachedStatus.ObtainTimerBuff(_resistanceOth);
                        if (currentBuff == 4)
                            success = true;
                        break;
                }

                if (success)
                {
                    controller.attachedStatus.ObtainTimerBuff(new TimerBuff(_spBuff));
                }
                else
                {
                    atkInfo.Item2.ObtainTimerBuff(new TimerBuff(_spBuff));
                }
            }
            else
            {
                //没受伤
                //controller.attachedStatus.ObtainTimerBuff(new TimerBuff(_spBuff));
            }
        }
        //todo: 根据情况，赋予特殊buff
    }

    private void ClearInvoke(int id)
    {
        if (controllers.Count == 0)
            return;

        if(id == FieldAbility_HB04)
            controllers[0].OnTimer -= CheckBuff;

        foreach (var controller in controllers)
        {
            controller.attachedStatus.RemoveAllConditionWithSpecialID(810441);
        }
    }
    private void InitTimerBuffs()
    {
        _resistanceStd = new TimerBuff((int)BasicCalculation.BattleCondition.StandardAttackShield,
            1,-1,1,810441);
        _resistanceStd.extra_iconID = (int)BasicCalculation.BattleCondition.DamageCut;
        _resistanceStd.dispellable = false;
        
        _resistanceDFs = new TimerBuff((int)BasicCalculation.BattleCondition.DashForceShield,
            1,-1,1,810441);
        _resistanceDFs.extra_iconID = (int)BasicCalculation.BattleCondition.DamageCut;
        _resistanceDFs.dispellable = false;
        
        _resistanceSkl = new TimerBuff((int)BasicCalculation.BattleCondition.SkillShield,
            1,-1,1,810441);
        _resistanceSkl.extra_iconID = (int)BasicCalculation.BattleCondition.DamageCut;
        _resistanceSkl.dispellable = false;
        
        _resistanceOth = new TimerBuff((int)BasicCalculation.BattleCondition.OtherShield,
            1,-1,1,810441);
        _resistanceOth.extra_iconID = (int)BasicCalculation.BattleCondition.DamageCut;
        _resistanceOth.dispellable = false;

        //_statusManager.SpecialDamageCutEffectFunc += SpecialDamageCutBuffTemporaryEvent;

        AddEventToStatusManager(_statusManager);
        AddEventToStatusManager(BattleStageManager.Instance.GetPlayer().GetComponent<StatusManager>());

        controllers.Add(Instantiate(GetProjectileOfFormatName("action18_ui"),
            transform.position, Quaternion.identity,
            BuffFXLayer.transform).GetComponent<UI_HB004_Legend_01>());
        
        controllers.Add(Instantiate(GetProjectileOfFormatName("action18_ui"),
            BattleStageManager.Instance.GetPlayer().transform.position, Quaternion.identity,
            BattleStageManager.Instance.GetPlayer().transform.Find("BuffLayer")).
            GetComponent<UI_HB004_Legend_01>());

        _spBuff = new TimerBuff((int)BasicCalculation.BattleCondition.PowerOfPrayer, -1,
            -1, 100,810442);

    }
    private void AddEventToStatusManager(StatusManager statusManager)
    {
        statusManager.SpecialDamageCutEffectFunc += SpecialDamageCutBuffTemporaryEvent;
    }
    private Tuple<float, float> SpecialDamageCutBuffTemporaryEvent(StatusManager source
        , AttackBase atk, StatusManager target)
    {
        float buffModifier = 0;

        if (atk.attackType == BasicCalculation.AttackType.STANDARD ||
            atk.attackType == BasicCalculation.AttackType.DSTANDARD)
        {
            if (target.GetConditionStackNumber((int)BasicCalculation.BattleCondition.StandardAttackShield) > 0)
            {
                buffModifier = 0.3f;
            }
        }else if (atk.attackType == BasicCalculation.AttackType.FORCE)
        {
            if (target.GetConditionStackNumber((int)BasicCalculation.BattleCondition.DashForceShield) > 0)
            {
                buffModifier = 0.8f;
            }
        }else if (atk.attackType == BasicCalculation.AttackType.DASH)
        {
            if (target.GetConditionStackNumber((int)BasicCalculation.BattleCondition.DashForceShield) > 0)
            {
                buffModifier = 0.3f;
            }
        }else if (atk.attackType == BasicCalculation.AttackType.SKILL ||
                  atk.attackType == BasicCalculation.AttackType.DSKILL)
        {
            if (target.GetConditionStackNumber((int)BasicCalculation.BattleCondition.SkillShield) > 0)
            {
                buffModifier = 0.8f;
            }
        }
        else
        {
            if (target.GetConditionStackNumber((int)BasicCalculation.BattleCondition.OtherShield) > 0)
            {
                buffModifier = 0.3f;
            }
        }
        
        
        
        return new Tuple<float, float>(buffModifier, 0);
    }
    
    
    
    
    
    
    
    
    /// <summary>
    /// Blessed Wall Fixed Position Double
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action06_V2(float center, float distance)
    {
        yield return _canActionOnGround;

        bossBanner?.PrintSkillName("HB04_Action06");
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        StageCameraController.SwitchOverallCamera();
        DOVirtual.DelayedCall(4f, () => StageCameraController.SwitchMainCamera(),false);

        yield return new WaitForSeconds(1f);

        float tweenTime;

        var hints = BlessedWallHintDoubleDirection();

        yield return new WaitForSeconds(1f);
        
        var twL = BlessedWallHintDoubleDirectionL( hints[0],distance,1,
            center, out tweenTime);
        var hintR = BlessedWallHintDoubleDirectionR( hints[1],distance,1,
            center, out tweenTime);

        yield return new WaitForSeconds(tweenTime);

        anim.Play("s3");

        yield return new WaitForSeconds(1);

        BlessedWallAttack(1, hintR.transform.position.x - 2 * distance,true);
        BlessedWallAttack(-1, hintR.transform.position.x,true);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();


    }
    

    /// <summary>
    /// forward attack
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action15()
    {
        yield return _canActionOnGround;
        ac.OnAttackEnter(999);
        //bossBanner?.PrintSkillName("HB04_Action13");
        ac.TurnMove(_behavior.targetPlayer);
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + new Vector3(0,2));
        
        anim.Play("spin");
        var pos = gameObject.RaycastedPosition();

        yield return new WaitForSeconds(1.05f);

        var proj = InstantiateRanged(GetProjectileOfFormatName("action15_1"),
            pos + new Vector2(ac.facedir, 0), InitContainer(false), 1);
        
        proj.GetComponent<WandingProjectile>().SetWandingPlatform(
            AStar.GetNodeOfName
            (gameObject.RaycastedPlatform().name).platform);
        
        proj.GetComponent<WandingProjectile>().SetFiredir(ac.facedir);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();

    }

    /// <summary>
    /// Colorful Pillars
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action16()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB04_Action16");
        
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(0.5f);
        
        StageCameraController.SwitchToOtherCamera(0,true);

        var suitablePosition = transform.position;

        suitablePosition.x = Mathf.Clamp(suitablePosition.x, BattleStageManager.Instance.mapBorderL + 14,
            BattleStageManager.Instance.mapBorderR - 14);

        suitablePosition.y = Mathf.Clamp(suitablePosition.y, BattleStageManager.Instance.mapBorderB + 5,
            BattleStageManager.Instance.mapBorderT - 6);

        var hintbar = EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac, suitablePosition,
            RangedAttackFXLayer.transform, 7f, Vector2.zero, false,
            true, 2, 0.1f, 0.5f, true, false);

        yield return new WaitForSeconds(1.5f);

        anim.Play("s1");

        yield return new WaitForSeconds(0.2f);

        (ac as EnemyControllerHumanoidHigh).SwapWeaponVisibility(false);
        
        yield return new WaitForSeconds(0.3f);

        Instantiate(GetProjectileOfFormatName("action16", true), suitablePosition,
            Quaternion.identity, RangedAttackFXLayer.transform).GetComponent<IEnemySealedContainer>().
            SetEnemySource(gameObject);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.75f);
        (ac as EnemyControllerHumanoidHigh).SwapWeaponVisibility(true);


        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");
        StageCameraController.SwitchMainCamera();

        UI_DialogDisplayer.Instance?.
            EnqueueDialogShared(10101,10041,
                BattleEffectManager.Instance?.notteHintClips[1]);
        QuitAttack();
        
    }
    
    /// <summary>
    /// 分摊
    /// </summary>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    public void HB04_Action17(float waitTime)
    {
        ShareDamageAttack(waitTime);
        QuitAttack();
    }
    
    public IEnumerator HB04_Action18()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB04_Action18");
        
        anim.Play("float");

        yield return new WaitForSeconds(1f);

        

        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        SetWorld();
        anim.Play("idle");
        
        QuitAttack();

    }


    public IEnumerator HB04_Action19()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB04_Action19");
        
        anim.Play("float");

        yield return new WaitForSeconds(1f);

        var bg = 
            BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background1");

        (bg as SpriteRenderer).DOColor(normalColor, 1);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        ResetWorld();
        anim.Play("idle");
        
        QuitAttack();

    }

    public IEnumerator HB04_Action20(int dir, int fixedFacedir = 0, int avoidable = 0)
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        if (fixedFacedir != 0)
        {
            ac.SetFaceDir(fixedFacedir > 0? 1 : -1);
        }
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + new Vector3(0,2));
       
        yield return new WaitForSeconds(.75f);
        
        anim.Play("forward");

        yield return new WaitForSeconds(.5f);
        
        voice?.PlayMyVoice((int)MyVoiceGroup.Combo1);

        if (dir == 0)
        {
            dir = Random.Range(0,2) == 0 ? -1 : 1;
        }

        ShootProjectiles(dir,avoidable==1);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();

    }
    
    public IEnumerator HB04_Action21()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB04_Action21");

        StageCameraController.SwitchOverallCamera();
        anim.Play("float");
        
        
        yield return new WaitForSeconds(2f);
        
        DawnCirclet();
        
        var bg = 
            BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background1");
        (bg as SpriteRenderer).DOColor(nightColor, 1);
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("idle");
        
        DOVirtual.DelayedCall(14f, () =>
        {
            StageCameraController.SwitchMainCamera();
            // if (controllers[0].IsActive)
            // {
            //     (bg as SpriteRenderer).DOColor(brightColor, 1);
            // }else
            // {
            //     (bg as SpriteRenderer).DOColor(normalColor, 1);
            // }
        },false);
        
        yield return new WaitForSeconds(2f);
        
        QuitAttack();
    }

    public IEnumerator HB04_Action22()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB04_Action22");

        GenerateWarningPrefab("action22",transform.position,Quaternion.identity, RangedAttackFXLayer.transform);
        
        yield return new WaitForSeconds(1.8f);
        
        anim.Play("combo2");

        yield return new WaitForSeconds(.5f);
        
        HolyRing();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }

    public IEnumerator HB04_Action23()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB04_Action23");
        
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,4f);

        var hintbar =
            GenerateWarningPrefab("action23", transform.position, Quaternion.identity, RangedAttackFXLayer.transform)
                .GetComponent<EnemyAttackHintBarChaser>();
        hintbar.target = _behavior.targetPlayer;
        
        yield return new WaitForSeconds(4f);
        
        anim.Play("combo2");

        InstantiateSealedContainer(GetProjectileOfFormatName("action23", true),
            hintbar.transform.position + Vector3.down*2, RangedAttackFXLayer.transform, 1);
        Destroy(hintbar.gameObject);

        Projectile_C019_7_Boss.MoveNext = true;

        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }

    /// <summary>
    /// Light Pillar
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action24(bool avoidable = false)
    {
        yield return _canActionOnGround;
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB04_Action24");
        
        if(Mathf.Abs(_behavior.targetPlayer.transform.position.x - transform.position.x) > 16)
            StageCameraController.SwitchToOtherCamera(0,true);
        
        yield return new WaitForSeconds(1f);
        

        anim.Play("combo4");
        
        Instantiate(GetProjectileOfFormatName("action24_2", true),
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
        //yield return new WaitForSeconds(0.3f);
        var hintbarPrefab = GetWarningPrefab(avoidable?"action24_2":"action24_1");
        var attackPrefab = GetProjectileOfFormatName("action24_1", true);

        var positions = LightPillarBurst_Hint(hintbarPrefab);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.48f);

        anim.Play("combo5");
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.35f);
        
        ac.SwapWeaponVisibility(false);
        
        Instantiate(GetProjectileOfFormatName("action24_3",true),
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(0.3f);
        
        for (int i = 0; i < positions.Count; i += 2)
        {
            LightPillarBurst_Attack(avoidable, attackPrefab, positions[i],positions[1+i]);
            yield return new WaitForSeconds(0.6f);
        }
        
        print(positions);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f);
        
        ac.SwapWeaponVisibility(true);
        StageCameraController.SwitchMainCamera();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();
    }

    /// <summary>
    /// All Attack
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action25(int hp,bool celestialPrayer = false,
        bool squaredAttack = false, bool holyBurst = false, params float[] triggerTimes)
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.SetActionUnable(false);
        ac.SetHitSensor(false);
        ac.TurnMove(_behavior.targetPlayer);
        WarpEffect();
        
        yield return new WaitForSeconds(0.1f);
        
        DisappearRenderer();
        ac.SwapWeaponVisibility(false);
        
        StageCameraController.SwitchOverallCamera();

        transform.position = new Vector3(-2, 11);
        
        (BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background1") as SpriteRenderer).
            DOColor(nightColor, 4);
        
        yield return new WaitUntil(()=>voice.voice.isPlaying == false);
        
        voice?.BroadCastSpecificVoice((int)MyVoiceGroup.ChangePhase, 0);
        

        yield return new WaitForSeconds(4);
        
        WarpEffect();
        yield return new WaitForSeconds(0.2f);
        
        
        
        
        AppearRenderer();
        ac.SetFaceDir(1);
        StageCameraController.SwitchMainCamera();
        StageCameraController.SwitchMainCameraFollowObject(gameObject);
        BattleStageManager.Instance.PlayerViewEnable = false;
        var fx = Instantiate(GetProjectileOfFormatName("action25",true),
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        var fxController = fx.GetComponent<Projectile_C019_8_Boss>();
        fxController.lightPillar.SetActive(false);
        fxController.SetEnemySource(gameObject);
        
        
        yield return new WaitUntil(()=>voice.voice.isPlaying == false);
        
        
        voice?.BroadCastSpecificVoice((int)MyVoiceGroup.ChangePhase, 1);


        yield return new WaitForSeconds(2);

        Projectile_C019_7_Boss.MoveNext = false;
        var RTScene = CutsceneResourcesLoader.Instance.RTSceneGameObject;
        var fullscreenUI = CutsceneResourcesLoader.Instance.FullScreenRtuiGameObject;
        var cutsceneController = RTScene.GetComponent<CutSceneController_HB04>();
        //var director = RTScene.GetComponent<PlayableDirector>();
        var Texture = cutsceneController.rt;
        var rawImg = fullscreenUI.GetComponent<RawImage>();
        RTScene.SetActive(true);
        yield return null;
        cutsceneController.Replay();
        rawImg.texture = Texture;
        fullscreenUI.SetActive(true);
        yield return null;

        yield return new WaitForSeconds(0.5f);
        
        bossBanner?.PrintSkillName("HB04_Action25");

        yield return new WaitForSeconds(1.5f);
        
        voice?.BroadCastSpecificVoice((int)MyVoiceGroup.ChangePhase, 2);
        
        yield return new WaitForSeconds(5.5f);

        anim.Play("pray");
        cutsceneController.ResetFaceExpression();
        fullscreenUI.SetActive(false);
        rawImg.texture = null;
        RTScene.SetActive(false);
        fxController.lightPillar.SetActive(true);
        SetWorld();
        StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
        StageCameraController.SwitchToOtherCamera(0,true);
        BattleStageManager.Instance.PlayerViewEnable = true;

        var enemy = SpawnEnemyMinon(GetProjectileOfFormatName("action25_minion", true),
            transform.position + new Vector3(1, 0),
            hp, 1);

        var uiRingSlider = SpawnCountDownUI(GetProjectileOfFormatName("action25_ui", true),
            transform.position + new Vector3(1, 4.5f), 30);

        var uiMinionCount = uiRingSlider.GetComponent<UI_CountdownMinon>();
        uiMinionCount.AddNewStatusManager(enemy.GetComponent<StatusManager>());

        fxController.StartRainbowTween();

        Tween _tween = null;
        int stack = 0;
        
        
        _tween = DOVirtual.DelayedCall(7.49f, () =>
        {

            _statusManager.ObtainTimerBuff(_spBuff);
            
            for(int i = 0 ; i < stack; i++)
                _statusManager.ObtainTimerBuff(_spBuff,false);

            stack += 2;
            
            _tween?.Restart();
            
        }, false);
        
        //设置附加技能

        Projectile_C019_7_Boss.MoveNext = true;
        int extraSkills = 
            CheckExtraAttackTotalNum(celestialPrayer, squaredAttack, holyBurst);
        
        var TweenList = new List<Tween>();

        Action extraAttack1 = AttachedSkill_CelestialPrayer;
        Action extraAttack2 = AttachedSkill_SquaredAttack;
        Action extraAttack3 = AttachedSkill_HolyRing;
        
        List<Action> extraAttackList = new List<Action>();

        if(celestialPrayer)
            extraAttackList.Add(extraAttack1);
        if(squaredAttack)
            extraAttackList.Add(extraAttack2);
        if(holyBurst)
            extraAttackList.Add(extraAttack3);


        for (int i = 0; i < triggerTimes.Length; i++)
        {
            if(triggerTimes[i] < 0)
                continue;
            
            var time = triggerTimes[i];
            var action = extraAttackList[i % extraSkills];
            
            TweenList.Add(DOVirtual.DelayedCall(time,
                ()=>
                {
                    action?.Invoke();
                },
                false));
        }

        if (TweenList.Count > 0)
        {
            var bg = BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background1") as SpriteRenderer;
            bg.DOColor(Color.black, 3);
        }
        
        //给玩家一个霸体
        foreach (var controller in controllers)
        {
            if (controller.attachedStatus.GetConditionStackNumber(
                    (int)BasicCalculation.BattleCondition
                    .KnockBackImmune) <= 0)
            {
                controller.attachedStatus.ObtainTimerBuff(new TimerBuff((int)BasicCalculation.BattleCondition.KnockBackImmune,
                    -1, 30, 1),false,false);
            }
        }

        yield return new WaitUntil(()=>uiRingSlider.currentValue <= 0 || uiMinionCount.Value <= 0);

        if (uiMinionCount.Value <= 0)
        {
            _behavior.targetPlayer.GetComponent<StatusManager>().ObtainTimerBuffs(_spBuff.buffID,
                -1, 10, _spBuff.maxStackNum, _spBuff.specialID);
        }
        
        
        
        if (TweenList.Count > 0)
        {
            var bg = BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background1") as SpriteRenderer;
            bg.DOColor(brightColor, 1);
        }

        anim.Play("combo5",-1,0.4f);
        fxController.StopRainbowTween();
        _tween?.Kill();
        foreach (var tween in TweenList)
        {
            tween?.Kill();
        }
        
        if (uiMinionCount.Value > 0)
        {
            enemy.GetComponent<StatusManager>().currentHp = 0;
            enemy.GetComponent<DragonPointEnemy>()?.DisableAll();
        }
        
        Destroy(uiMinionCount.gameObject);
        StageCameraController.SwitchMainCamera();
        yield return null;
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        ac.SwapWeaponVisibility(true);
        Destroy(fx);
        anim.Play("idle");
        ac.ResetGravityScale();
        ac.SetHitSensor(true);
        

        QuitAttack();
        
    }


    /// <summary>
    /// Holy Heal
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action26()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetActionUnable(false);
        ac.SetHitSensor(false);
        
        WarpEffect();
        
        yield return new WaitForSeconds(0.1f);
        
        DisappearRenderer();
        ac.SwapWeaponVisibility(false);
        StageCameraController.SwitchToOtherCamera(0,true);

        yield return new WaitForSeconds(0.5f);
        
        (BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background1") as SpriteRenderer).
            DOColor(nightColor, 1);
        
        transform.position = new Vector3(0, 11);
        WarpEffect();
        yield return new WaitForSeconds(0.2f);

        
        AppearRenderer();
        ac.SwapWeaponVisibility(true);
        bossBanner?.PrintSkillName("HB04_Action26");
        voice?.BroadCastMyVoice((int)MyVoiceGroup.Buff);
        anim.Play("transform_1");
        
        var cnt = HolyHealProjectiles(28);
        
        yield return new WaitForSeconds(0.5f);

        ac.SetHitSensor(true);
        
        yield return new WaitForSeconds(30f);
        
        anim.SetTrigger("next");

        yield return null;
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.65f);
        
        _statusManager.HPRegenImmediately(0,10,false);
        Instantiate(GetProjectileOfName("fx_ability_recover_c019"),transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        StageCameraController.SwitchMainCamera();
        QuitAttack();

    }





    private int CheckExtraAttackTotalNum(params bool[] skls)
    {
        int sum = 0;
        foreach(var skl in skls)
        {
            if (skl)
            {
                sum++;
            }
        }
        return sum;
    }

    private int HolyHealProjectiles(int bulletCount)
    {
        var prefab = GetProjectileOfFormatName("action26_1");

        int bulletAngleCount = 90;
        float interval = 0.75f;

        List<float> angles = new();
        
        for (int i = 0; i < bulletAngleCount; i++)
        {
            angles.Add(360f / bulletAngleCount * i);
        }
        
        int launchedCount = 0;

        Tween tween = null;

        var radius = 32f;

        try
        {
            _statusManager.RemoveConditionWithLog(
                _statusManager.GetExactConditionsOfType(
                    (int)BasicCalculation.BattleCondition.DamageUp, 810443)[0]);
        }
        catch
        {
            Debug.LogWarning("No Damage Up Condition Found");
        }

        
        
        tween = DOVirtual.DelayedCall(interval,
            () =>
            {
                if (launchedCount >= bulletCount || GlobalController.currentGameState != GlobalController.GameState.Inbattle)
                {
                    tween.Kill();
                    return;
                }

                launchedCount++;
                
                int randomIndex = UnityEngine.Random.Range(0, angles.Count);
                float angle = angles[randomIndex];
                angles.RemoveAt(randomIndex);
                
                Vector2 position = transform.position + 
                                   Quaternion.Euler(0, 0, angle) *
                                   Vector2.right * radius;
                
                print(position);
                var rotateAngle = (position - (Vector2)transform.position).normalized;

                var proj = InstantiateDirectionalRanged(prefab, position,
                    InitContainer(false), 1, 0);
                var projController = proj.GetComponent<Projectile_C019_9_Boss>();
                projController.target = transform;
                projController.SetSpecialBuff(_spBuff);
                //proj.GetComponent<HomingAttack>().target = transform;
                //proj.GetComponent<HomingAttack>().angle = rotateAngle;
                
                tween.Restart();
                
            },false);

        return launchedCount;

    }
    private List<Vector2> LightPillarBurst_Hint(GameObject prefab)
    {
        var leftPosition = new Vector3(transform.position.x,
            _behavior.targetPlayer.transform.position.y) + new Vector3(-9,-6);
        var rightPosition = new Vector3(transform.position.x,
            _behavior.targetPlayer.transform.position.y) + new Vector3(9,-6);

        leftPosition.y = Mathf.Clamp(leftPosition.y,
            BattleStageManager.Instance.mapBorderB + 2,
            BattleStageManager.Instance.mapBorderT - 10);
        
        rightPosition.y = Mathf.Clamp(rightPosition.y,
            BattleStageManager.Instance.mapBorderB + 2,
            BattleStageManager.Instance.mapBorderT - 10);
        
        print(BattleStageManager.Instance.mapBorderB + " " + BattleStageManager.Instance.mapBorderT);

        // var left1 = GenerateWarningPrefab(prefab, leftPosition, Quaternion.identity,
        //     RangedAttackFXLayer.transform);
        // var right1 = GenerateWarningPrefab(prefab, rightPosition, Quaternion.identity,
        //     RangedAttackFXLayer.transform);
        
        List<Vector2> hintbarPos = new();
        
        hintbarPos.Add(leftPosition);
        hintbarPos.Add(rightPosition);

        while (leftPosition.x > BattleStageManager.Instance.mapBorderL ||
               rightPosition.x < BattleStageManager.Instance.mapBorderR)
        {
            leftPosition += Vector3.left * 14;
            rightPosition += Vector3.right * 14;

            Vector2 left;
            Vector2 right;

            if (leftPosition.x > BattleStageManager.Instance.mapBorderL)
            {
                hintbarPos.Add(leftPosition);
            }
            else
            {
                hintbarPos.Add(Vector2.positiveInfinity);
            }
            
            
            if (rightPosition.x < BattleStageManager.Instance.mapBorderR)
            {
                hintbarPos.Add(rightPosition);
            }
            else
            {
                hintbarPos.Add(Vector2.positiveInfinity);
            }
            
            
            
        }

        return hintbarPos;

    }
    
    private void LightPillarBurst_Attack(bool avoidable, GameObject prefab, params Vector2[] hintbarPos)
    {
        
        foreach (var pos in hintbarPos)
        {
            if (float.IsInfinity(pos.x))
            {
                continue;
            }
            var hintbar = GenerateWarningPrefab(avoidable?"action24_2":"action24_1",
                pos, Quaternion.identity,
                RangedAttackFXLayer.transform);
            print(hintbar.transform.position);
            DOVirtual.DelayedCall(.6f, () =>
            {
                var proj = InstantiateRanged(prefab, pos, InitContainer(false), 1);
                if(avoidable)
                    proj.GetComponent<AttackFromEnemy>().ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Red);
            }, false);
        }
    }
    
    private void HolyRing()
    {
        var proj = InstantiateRanged
        (GetProjectileOfFormatName("action22_1", true),
            transform.position, InitContainer(false), ac.facedir);

        var dt_controller = proj.AddComponent<DOTweenSimpleController>();

        dt_controller.IsAbsolutePosition = false;
        dt_controller.duration = 2;
        dt_controller.SetWaitTime(1f);
        dt_controller.moveDirection = 
            ((Vector2)(_behavior.targetPlayer.transform.position - transform.position)).normalized * 20;


    }

    private void AttachedSkill_HolyRing()
    {
        bossBanner?.PrintSkillName("HB04_Action22");
        
        GenerateWarningPrefab("action22",transform.position,Quaternion.identity, RangedAttackFXLayer.transform);
        
        DOVirtual.DelayedCall(2.3f,HolyRing,false);
    }

    private void AttachedSkill_SquaredAttack()
    {
        bossBanner?.PrintSkillName("HB04_Action23");
        
        //BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,4f);
        
        var controller = InstantiateSealedContainer(GetProjectileOfFormatName("action23", true),
            transform.position + new Vector3(2,0), RangedAttackFXLayer.transform, 1).
            GetComponent<Projectile_C019_7_Boss>();

        controller.hint = true;
        //controller.transform.GetChild(0).gameObject.SetActive(false);




    }

    private void AttachedSkill_CelestialPrayer()
    {
        var random = UnityEngine.Random.Range(0, 2);
        bossBanner?.PrintSkillName("HB04_Action11");
        
        if(random == 0)
        {
            UprisingStars(false);
        }
        else
        {
            FallingStars(false);
        }
        
    }

    private void DawnCirclet()
    {
        var container = Instantiate(GetProjectileOfFormatName("action21", true),
            Vector2.zero, Quaternion.identity,
            RangedAttackFXLayer.transform);

        var controller = container.GetComponent<IEnemySealedContainer>();
        
        controller.SetEnemySource(gameObject);


    }
    private void ShootProjectiles(int dir = 1, bool avoidable = false)
    {
        
        
        var container = InitContainer(false);
        var proj1 = InstantiateRanged(GetProjectileOfFormatName("action20_1",true),
            transform.position + new Vector3(ac.facedir * 1, 1), container, 1);
        var proj2 = InstantiateRanged(GetProjectileOfFormatName("action20_1",true),
            transform.position + new Vector3(ac.facedir * 1, 2), container, 1);
        var proj3 = InstantiateRanged(GetProjectileOfFormatName("action20_1",true),
            transform.position + new Vector3(ac.facedir * 1, -1), container, 1);
        var proj4 = InstantiateRanged(GetProjectileOfFormatName("action20_1",true),
            transform.position + new Vector3(ac.facedir * 1, -2), container, 1);
        
        var projList = new List<GameObject> {proj1, proj2, proj3, proj4};

        Vector2 targetPos = new Vector2(transform.position.x + ac.facedir * 25, transform.position.y);

        //float[] offsets = new float[] { -4,-7, 4 };
        
        Vector2[] targetPoses2 = new Vector2[] 
        {
            new(targetPos.x, targetPos.y + dir * 2), 
            new(targetPos.x - ac.facedir, targetPos.y + dir * 3),
            new(targetPos.x - ac.facedir * 2, targetPos.y + dir * 4),
            new(targetPos.x - ac.facedir * 3, targetPos.y + dir * 5) 
        };

        var positionDiff = _behavior.targetPlayer.transform.position -
                           new Vector3(targetPos.x - ac.facedir * 1.5f, targetPos.y + dir * 3.5f);
        
        var normalizedDirection = ((Vector2)positionDiff).normalized;
        
        float[] jumpPowers = new float[] { 3, 6, -3, -6 };

        for (int i = 0; i < projList.Count; i++)
        {
            Sequence seq = DOTween.Sequence();
            
            var index = i;
            
            seq.Append(projList[index].transform.DOJump
                (new Vector2(transform.position.x + ac.facedir * 8, transform.position.y),
                    jumpPowers[index],1, 1).SetEase(Ease.Linear));
            
            seq.Append(projList[index].transform.DOJump
                (targetPoses2[index], -1.5f*jumpPowers[index],1, 1.25f).SetEase(Ease.Linear));

            seq.AppendCallback(() =>
            {
                EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, projList[index].transform.position,
                    RangedAttackFXLayer.transform, new Vector2(40, 1), Vector2.zero, avoidable,
                    0, 1, Mathf.Atan2(positionDiff.y, positionDiff.x) * Mathf.Rad2Deg,
                    0.3f, true, false);
                
                if(!avoidable)
                    projList[index].GetComponent<AttackFromEnemy>().
                    ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
                
                projList[index].GetComponent<AttackBase>().NextAttack();
            });
            
            
            seq.AppendInterval(1f);
            
            seq.Append(projList[index].transform.DOMove
                (targetPoses2[index] + normalizedDirection * 40, 2f).SetEase(Ease.Linear));

            seq.AppendCallback(() => Destroy(projList[index], 0.1f));
                
            seq.Play();
        }

    }
    private void SetWorld()
    {
        if(controllers[0].IsActive)
            return;
        
        var bg = 
            BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background1");
        (bg as SpriteRenderer).DOColor(brightColor, 1);
        BattleStageManager.Instance.AddFieldAbility(FieldAbility_HB04);
        StartBuffTick();
    }

    private void ResetWorld()
    {
        if (!controllers[0].IsActive)
        {
            print("IS NOT Active");
            //return;
        }

        //todo: 解除“祈愿之光”并获得相应的攻击威力BUFF

        foreach (var controller in controllers)
        {
            var count = controller.attachedStatus.RemoveAllConditionWithSpecialID(810442);
            if(count <= 0)
                continue;

            var timerBuff =
                new TimerBuff((int)BasicCalculation.BattleCondition.DamageUp,
                    count * 10, 90, 100,
                    810443);

            timerBuff.dispellable = false;

            controller.attachedStatus.ObtainTimerBuff
            (timerBuff,true,false);
            //timerBuff.dispellable = false;
        }

        BattleStageManager.Instance.RemoveFieldAbility(FieldAbility_HB04);
    }

    private void ShareDamageAttack(float waitTime)
    {
        var hintGO = GenerateWarningPrefab("action17",
            _behavior.targetPlayer.transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform,1);

        var target = _behavior.targetPlayer;

        hintGO.GetComponent<EnemyAttackHintBarCircle>().warningTime = waitTime;
        hintGO.GetComponent<EnemyAttackHintBarShine>().warningTime = waitTime - 0.1f;
        hintGO.GetComponent<EnemyAttackHintBarChaser>().SetLockTime(waitTime);
        hintGO.GetComponent<EnemyAttackHintBarChaser>().target = target;

        _statusManager.ObtainTimerBuff(new TimerBuff((int)BasicCalculation.BattleCondition.KnockBackImmune,
            -1, waitTime + 1, 1),false);

        DOVirtual.DelayedCall(waitTime, () =>
        {
            
            var container = Instantiate
            (GetProjectileOfFormatName("action17_1", true),
                hintGO.transform.position, Quaternion.identity,RangedAttackFXLayer.transform);
            
            if(hintGO != null)
                Destroy(hintGO);

            var attackContainer = container.GetComponent<ContributoryAttackContainer>();
            
            attackContainer.SetEnemySource(gameObject);

            Action<StatusManager> handler = null;
            bool isComplete = false;

            handler = (stat) =>
            {
                if(isComplete)
                    return;
                
                BattleStageManager.Instance.CauseIndirectDamage(stat, (int)(stat.maxHP * 0.1f),
                    false);
                BattleStageManager.Instance.CauseIndirectDamageToOverdriveBar(
                    stat as SpecialStatusManager, (int)(stat.maxHP * 0.1f));
                isComplete = true;
            };

            attackContainer.SetAction(handler);

        }, false);



    }
    
    
    
    protected Tweener BlessedWallHintDoubleDirectionL
        (GameObject hintL, float distance,float stopTime,float fixedPosition, out float tweenTime)
    {
        
        var shineHint = hintL.GetComponentInChildren<EnemyAttackHintBarShine>();

        var safePosition =
            (new Vector3(fixedPosition - distance, 0)).SafePosition(Vector2.zero);
        tweenTime = 0.5f;

        var tweenerCoreL = hintL.transform.DOMoveX(safePosition.x, tweenTime);

        return tweenerCoreL;
    }
    
    protected GameObject BlessedWallHintDoubleDirectionR
        (GameObject hintR, float distance,float stopTime, float fixedPosition, out float tweenTime)
    {
        var shineHint = hintR.GetComponentInChildren<EnemyAttackHintBarShine>();
        tweenTime = 0.5f;

        var safePosition2 =
            (new Vector3(fixedPosition + distance, 0)).SafePosition(Vector2.zero);
        
        var tweenerCoreR = hintR.transform.DOMoveX(safePosition2.x, tweenTime);

        return hintR;
    }
    
    
}
