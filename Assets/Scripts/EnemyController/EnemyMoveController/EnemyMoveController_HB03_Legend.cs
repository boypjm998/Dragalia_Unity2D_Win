using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyMoveController_HB03_Legend : EnemyMoveManager
{
    public const int AttackDownSPID = 8103402;
    public const int SilenceSPID = 8103403;
    public const int DamageCutSPID = 8103404;
    public bool ShieldOn { get; set; }
    public int currentWorld = 0;
    private VoiceControllerEnemy _voice;
    private float _initialGroundPosition;
    private GameObject _groundGameObject;

    private SpriteRenderer _backgroundRenderer;
    private Tween _orbGenerationTween;
    private Tweener _backgroundTweener;

    private TimerBuff _critRateBuff = 
        new TimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff,
        30, 15, 100);

    [SerializeField] private GameObject orbPrfeb;
    [SerializeField] private GameObject platformPrefab;

    [SerializeField] private GameObject minion1;
    [SerializeField] private GameObject minion2;
    [SerializeField] private GameObject minion3;

    private List<GameObject> _platformInstances = new();

    protected override void Awake()
    {
        base.Awake();
        _voice = GetComponentInChildren<VoiceControllerEnemy>();
        _groundGameObject = GameObject.FindGameObjectWithTag("Ground");
        _initialGroundPosition = _groundGameObject.transform.position.y;
    }

    /// <summary>
    /// 驱动爆破
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action05()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action05");
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3.5f);
        yield return new WaitForSeconds(1.5f);
        
        anim.SetTrigger("action");
        _voice.PlayMyVoice(3);

        yield return new WaitForSeconds(0.5f);
        
        DriveBuster();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        QuitAttack();
    }
    
    /// <summary>
    /// 超驱动爆破
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action06()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action06");
        //BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3.5f);
        yield return new WaitForSeconds(0.5f);
        
        anim.SetBool("attack",true);
        _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.AlchemicCatridge);
        _voice.PlayMyVoice(7);

        yield return new WaitForSeconds(0.35f);

        _statusManager.ObtainTimerBuff(new TimerBuff(_critRateBuff));
        OverdriveBuster();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        QuitAttack();
    }

    /// <summary>
    /// Alchemic Enhancement
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action07()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action07");
        //BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3.5f);
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("attack");
        
        _voice.PlayMyVoice(4);

        yield return new WaitForSeconds(0.35f);
        
        AlchemicEnhancement();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        QuitAttack();
    }
    
    /// <summary>
    /// 炼金榴弹炮
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action08()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action08");
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3f);
        _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.AlchemicCatridge);

        var tweenTime = FastFly(10,8,5,10);

        yield return new WaitForSeconds(tweenTime);
        
        ac.TurnMove(_behavior.targetPlayer);
        //开始跟着玩家移动
        float maxFollowTime = 2f;

        var raycastedPositionY = _behavior.targetPlayer.RaycastedPosition().y + 5;

        var playerAc = _behavior.targetPlayer.GetComponent<ActorController>();
        float vy = playerAc.jumpforce;
        float groundTime = vy / (playerAc.rigid.gravityScale * 10);
        

        Action<ActorController, int> handle = null;
        bool detectedJump = false;

        handle = (ac, jumpTime) =>
        {
            ac.OnJump -= handle;
            detectedJump = true;
        };

        playerAc.OnJump += handle;

        var tween = 
            DOVirtual.DelayedCall(4f, () => { playerAc.OnJump -= handle; }, false);


        while (maxFollowTime > 0)
        {
            maxFollowTime -= Time.fixedDeltaTime;

            var endPos = ac.facedir > 0
                ? new Vector3(Mathf.Max(_behavior.targetPlayer.transform.position.x - 8,BattleStageManager.Instance.mapBorderL), raycastedPositionY)
                : new Vector3(Mathf.Min(_behavior.targetPlayer.transform.position.x + 8,BattleStageManager.Instance.mapBorderR), raycastedPositionY)
                ;

            Vector2 direction = endPos - transform.position;

            var diffX = Mathf.Abs(transform.position.x - _behavior.targetPlayer.transform.position.x);
            var diffY = Mathf.Abs(transform.position.y - raycastedPositionY);

            if (diffX > 14f || diffX < 2f || diffY > 6f || diffY < 4f)
            {
                ac.rigid.position += (Vector2)(direction.normalized * 7) * Time.fixedDeltaTime;
            }
            else
            {
                ac.rigid.position = endPos;
            }

            yield return new WaitForFixedUpdate();
            
            if(detectedJump)
                break;

        }
        
        ac.TurnMove(_behavior.targetPlayer);
        
        if ((Mathf.Abs(transform.position.x - _behavior.targetPlayer.transform.position.x) > 8 ||
            Mathf.Abs(transform.position.x - _behavior.targetPlayer.transform.position.x) < 7.5f)
            )
        {
            
            tweenTime = FastFlyOtherSide(25, 8f, 5, 8,8,-1);
            print("重新移动");
            yield return new WaitForSeconds(Mathf.Max(0.02f,tweenTime - 0.33f));
            
            
        }
        else if(!detectedJump)
        {
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            tweenTime = groundTime - 0.35f;
            FastFlyOtherSide(25, 8f, 5, 8,8,tweenTime);
            print("检测到跳跃，不移动");
            yield return new WaitForSeconds(tweenTime);
        }
        
        //ac.TurnMove(_behavior.targetPlayer);
        
        anim.SetTrigger("action");
        _voice.PlayMyVoice(8);

        yield return new WaitForSeconds(0.35f);
        
        ac.TurnMove(_behavior.targetPlayer);
        _statusManager.ObtainTimerBuff(new TimerBuff(_critRateBuff));
        AlchemicGrenade();
        tween?.Kill();
        playerAc.OnJump -= handle;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        QuitAttack();
    }
    /// <summary>
    /// 异界传送
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action09()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        StageCameraController.SwitchOverallCamera();
        bossBanner?.PrintSkillName("HB03_Action09");
        
        yield return new WaitForSeconds(1f);

        if (Projectile_C001_5_Boss.Instance != null)
        {
            Destroy(Projectile_C001_5_Boss.Instance.gameObject);
            yield return null;
        }
        
        anim.Play("attack");
        yield return new WaitForSeconds(1f);
        OpenPortals();
        
        yield return new WaitForSeconds(1f);
        
        Transportation();
        
        yield return new WaitForSeconds(1f);
        
        StageCameraController.SwitchMainCamera();
        
        QuitAttack();

    }
    
    /// <summary>
    /// 异界之门
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action10(GameObject target)
    {
        yield return new WaitUntil(() => !ac.hurt);

        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("HB03_Action10");
        
        _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.AlchemicCatridge);
        
        var hint = GenerateWarningPrefab(WarningPrefabs[0],target.transform.position,
            Quaternion.identity,RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBar>();
        if(target != null)
            hint.GetComponent<EnemyAttackHintBarChaser>().target = target;
        

        yield return null;

        yield return new WaitForSeconds(3.5f);

        anim.Play("attack");
        
        _voice.BroadCastMyVoice(9);
        
        //StageCameraController.SwitchOverallCamera();
        //Invoke(nameof(SwitchMainCamera),4f);

        yield return new WaitForSeconds(1.5f);
        
        OtherWorldGate_Blast(hint.transform.position+new Vector3(0,2,0));
        
        _statusManager.ObtainTimerBuff(new TimerBuff(_critRateBuff));


        //yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (110f/148f));

        
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        yield return null;

        QuitAttack();
        
    }
    

    /// <summary>
    /// 疾风之箭
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action15()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        anim.Play("attack");

        yield return new WaitForSeconds(0.5f);

        List<Vector2> posList = new()
        {
            new(-2, 1), new(-4, 0), new (2, 1)
        };

        posList = posList.Shuffle().ToList();

        SpawnProjectiles((Vector2)transform.position + posList[0]);
        
        yield return new WaitForSeconds(0.2f);
        
        SpawnProjectiles((Vector2)transform.position + posList[1]);
        
        yield return new WaitForSeconds(0.2f);
        
        SpawnProjectiles((Vector2)transform.position + posList[2]);

        yield return new WaitForSeconds(1f);
        
        QuitAttack();

    }

    /// <summary>
    /// 风暴守护
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action16()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB03_Action16");
        anim.Play("attack");

        EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac, transform.position,
            MeeleAttackFXLayer.transform, 3, Vector2.zero, false, true,
            1, 0.07f, 0.3f, true, true);
        
        yield return new WaitForSeconds(1f);

        StormShield();

        yield return new WaitForSeconds(1f);
        
        var time = FlyToBorder();
        
        yield return new WaitForSeconds(time);
        
        QuitAttack();

    }
    
    public IEnumerator HB03_Action17()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        
        ac.rigid.velocity = Vector2.zero;
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action17");
        _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.AlchemicCatridge);
        
        ac.SetHitSensor(false);
        WarpFX();

        yield return new WaitForSeconds(0.2f);
        ac.DisappearRenderer();
        ac.SwapWeaponVisibility(false);

        yield return new WaitForSeconds(0.2f);

        var positionY = _behavior.targetPlayer.RaycastedPosition().y;
        transform.position = new Vector3(0, _behavior.targetPlayer.RaycastedPosition().y + 2f);
        WarpFX();

        yield return new WaitForSeconds(0.2f);
        ac.AppearRenderer();
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetHitSensor(true);
        ac.SwapWeaponVisibility(true);
        _statusManager.ObtainTimerBuff(new TimerBuff(_critRateBuff));
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("attack");

        EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac, transform.position,
            MeeleAttackFXLayer.transform, 4.5f, Vector2.zero, false, true,
            1, 0.1f, 0.3f, true, true);
        
        yield return new WaitForSeconds(1f);

        StormShieldBoost();
        SurgingGalesA(positionY,0.25f);

        yield return new WaitForSeconds(2f);
        
        ac.TurnMove(_behavior.targetPlayer);
        var dir = ac.facedir;
        var time = DashToBorder(dir,14);
        
        yield return new WaitForSeconds(time);

        time = DashToBorder(-dir,9);
        
        yield return new WaitForSeconds(0.75f);
        
        ac.TurnMove(_behavior.targetPlayer);
        
        SurgingGalesB(positionY);

        yield return new WaitForSeconds(time - 0.75f);

        QuitAttack();
    }
    
    /// <summary>
    /// Alchemic Shield
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action18()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action18");
        //BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3.5f);
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("attack");
        

        yield return new WaitForSeconds(0.35f);

        if (Projectile_C001_6_Boss.Instance != null)
        {
            Destroy(Projectile_C001_6_Boss.Instance.gameObject);
            yield return null;
        }
        
        AlchemicShield();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        QuitAttack();
    }
    
    /// <summary>
    /// 星辰射流
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action19()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action19");
        //BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3.5f);
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("attack");
        

        yield return new WaitForSeconds(0.35f);

        AstralStream();

        yield return new WaitForSeconds(1);
        
        AstralStream();

        yield return new WaitForSeconds(1);
        
        AstralStream();

        QuitAttack();
    }

    /// <summary>
    /// 星辰爆发
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action20()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action20");
        StageCameraController.SwitchOverallCamera();
        //_statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.AlchemicCatridge);

        yield return new WaitForSeconds(1);
        
        anim.Play("attack");
        //_statusManager.ObtainTimerBuff(new TimerBuff(_critRateBuff));

        AstralSurge_HintSide();
        
        yield return new WaitForSeconds(1.5f);
        
        AstralSurge_CenterCore();

        yield return new WaitForSeconds(1.5f);

        AstralSurge_Projectiles();

        yield return new WaitForSeconds(3f);
        
        AstralSurge_Projectiles();
        
        yield return new WaitForSeconds(3f);
        StageCameraController.SwitchMainCamera();
        QuitAttack();

    }

    /// <summary>
    /// 世界1
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action21()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action21");
        
        yield return new WaitForSeconds(1);
        
        anim.Play("attack");
        _voice.BroadCastMyVoice(13);
        BackgroundTweener(Color.black);
        
        yield return new WaitForSeconds(1);
        
        _orbGenerationTween?.Kill();
        GenerateOrbsNormal(1);
        BattleStageManager.Instance.AddFieldAbility((int)BasicCalculation.EnemyAbility.JumpBoostWorld);
        BackgroundTweener(Color.white);
        
        yield return new WaitForSeconds(0.5f);
        
        QuitAttack();

    }
    
    public IEnumerator HB03_Action22()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action22");
        
        yield return new WaitForSeconds(1);
        
        anim.Play("attack");
        _voice.BroadCastMyVoice(14);
        BackgroundTweener(Color.black);
        
        yield return new WaitForSeconds(1);
        
        //_orbGenerationTween?.Kill();
        GenerateOrbsNormal(0);
        BattleStageManager.Instance.RemoveFieldAbility((int)BasicCalculation.EnemyAbility.JumpBoostWorld);
        BackgroundTweener(Color.white);
        
        yield return new WaitForSeconds(0.5f);
        
        QuitAttack();

    }

    /// <summary>
    /// 终焉风暴
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action23()
    {
        yield return new WaitUntil(() => ac.hurt == false);

        


        BattleStageManager.Instance.DragonBlock = true;
        ac.rigid.velocity = Vector2.zero;
        ac.OnAttackEnter(999);
        _statusManager.ReliefAllDebuff();
        
        //bossBanner?.PrintSkillName("HB03_Action23");
        
        ac.SetHitSensor(false);
        WarpFX();

        yield return new WaitForSeconds(0.2f);
        ac.DisappearRenderer();
        ac.SwapWeaponVisibility(false);

        yield return new WaitForSeconds(0.2f);
        
        ExtendTopBorder();
        
        _voice.BroadCastSpecificVoice(15,0);

        yield return new WaitForSeconds(3.5f);

        var windArea = GenerateGroundWindArea();
        
        
        //var positionY = _behavior.targetPlayer.RaycastedPosition().y;
        transform.position = new Vector3(0, 36);
        WarpFX();

        yield return new WaitForSeconds(0.2f);
        
        _voice.BroadCastSpecificVoice(15,1);
        PurgedShapeShiftingOfViewer();
        GrantSilenceEffectToPlayer();
        
       

        yield return new WaitForSeconds(1.8f);

        BattleStageManager.Instance.PlayerViewEnable = false;
        
        //3d Cutscene
        
        var RTScene = CutsceneResourcesLoader.Instance.RTSceneGameObject;
        var fullscreenUI = CutsceneResourcesLoader.Instance.FullScreenRtuiGameObject;
        var cutsceneController = RTScene.GetComponent<CutSceneController_HB03>();
        //var director = RTScene.GetComponent<PlayableDirector>();
        var Texture = cutsceneController.rt;
        var rawImg = fullscreenUI.GetComponent<RawImage>();
        RTScene.SetActive(true);
        yield return null;
        cutsceneController.Replay();
        rawImg.texture = Texture;
        fullscreenUI.SetActive(true);
        yield return null;

        BattleStageManager.Instance.PlayerViewEnable = true;
        
        if (Projectile_C001_12_Boss.Instance != null)
        {
            Destroy(Projectile_C001_13_Boss.Instance.gameObject);
        }
        if (Projectile_C001_13_Boss.Instance != null)
        {
            Destroy(Projectile_C001_13_Boss.Instance.gameObject);
        }
        if (Projectile_C001_14_Boss.Instance != null)
        {
            Destroy(Projectile_C001_13_Boss.Instance.gameObject);
        }
        
        yield return new WaitForSeconds(1f);
        
        //bossBanner?.PrintSkillName("HB03_Action23");

        yield return new WaitForSeconds(1f);
        
        _voice.BroadCastSpecificVoice(15, 2);
        
        yield return new WaitForSeconds(3.5f);
        
        var playerInput = _behavior.targetPlayer.GetComponent<PlayerInput>();
        var actorController = playerInput.GetComponent<ActorController>();
        playerInput.DisableAndIdle();

        ac.AppearRenderer();
        anim.Play("float");
        cutsceneController.ResetFaceExpression();
        fullscreenUI.SetActive(false);
        rawImg.texture = null;
        RTScene.SetActive(false);

        //限制玩家行动


        var airWindArea = GenerateAirWindArea();
        ac.SetFaceDir(1);
        actorController.SetActionUnable(true);
        actorController.SetDefaultGravityScale(0);
        //开始赋予减伤
        GrantDamageCutEffectToPlayer();
        var dmgCutGrantTween = DOVirtual.
            DelayedCall(3f, () => GrantDamageCutEffectToPlayer(),
                false).SetLoops(4);

        yield return new WaitForSeconds(0.3f);
        
        
        
        

        var playerBurstFlyTween =
            playerInput.transform.DOMoveY(35, 1).
                SetUpdate(UpdateType.Fixed).SetEase(Ease.OutQuad);
        
        
        
        
        
        var blastFX = Instantiate(GetProjectileOfFormatName("action21_fx", true),
            playerInput.transform.position + new Vector3(0,3),
            Quaternion.identity, RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(1);

        playerInput.jumptime = 0;
        actorController.SetActionUnable(false);
        playerInput.enabled = true;

        var endHeight = BattleStageManager.Instance.mapBorderB + 1.3f;
        actorController.rigid.velocity = Vector2.zero;
        
        bossBanner?.PrintSkillName("HB03_Action24");

        DOVirtual.DelayedCall(1f, () =>
        {
            var controller = airWindArea.GetComponent<Projectile_C001_9_Boss>();
            controller.playerGO = _behavior.viewerPlayer;
            controller.SetEnemySource(gameObject);
            controller.StartShooting();
        }, false);
        
        StageCameraController.SwitchToOtherCamera(0,true);
        actorController.SetMoveSpeed(actorController._statusManager.movespeed * 1.1f);

        while (playerInput.transform.position.y > endHeight)
        {
            if (actorController.hurt == false)
            {
                actorController.rigid.velocity = Vector2.zero;
                actorController.rigid.drag = 0;
                actorController.rigid.position -= new Vector2(0, 1.7f) * Time.fixedDeltaTime;
            }

            yield return new WaitForFixedUpdate();
        }

        actorController.SetMoveSpeed(actorController._statusManager.movespeed);
        actorController.SetDefaultGravityScale(4);
        
        if (Projectile_C001_9_Boss.Instance != null)
        {
            Projectile_C001_9_Boss.Instance.StopShooting();
        }
        

        yield return new WaitForSeconds(1);
        dmgCutGrantTween?.Kill();
        
        //todo: 弹射起步

        var controller = PlayerPrepareDash();
        actorController.SetHitSensor(false);
        //成就
        BattleStageManager.Instance.
            TriggerSpecialEvent
                (actorController._statusManager.
                    GetConditionWithSpecialID(DamageCutSPID).Count);

        yield return new WaitForSeconds(1.5f);
        
        actorController.SetDefaultGravityScale(0);
        //playerInput.DisableAndIdle();

        yield return null;
        
        StageCameraController.SwitchMainCamera();

        bool tweenerFinished = false;

        playerBurstFlyTween =
            actorController.transform.DOMoveY(36, 1.2f)
                .SetUpdate(UpdateType.Fixed).SetEase(Ease.InSine).OnComplete(()=>tweenerFinished = true);
        
        // actorController.transform.DOBlendableMoveBy(new Vector3(0,36,0),1.2f)
        //     .SetUpdate(UpdateType.Fixed).SetEase(Ease.InSine).OnComplete(()=>tweenerFinished = true);
        
        yield return new WaitForSeconds(0.2f);

        playerInput.enabled = true;
        playerInput.jumptime = 0;
        //actorController.SetMoveSpeed(actorController._statusManager.movespeed * 2);


        yield return new WaitUntil(() => tweenerFinished);
        
        actorController.SetMoveSpeed(actorController._statusManager.movespeed);
        actorController.SetHitSensor(true);
        
        //todo:boss掉落
        
        //todo:可能失败
        StageCameraController.SwitchMainCameraFollowObject(gameObject);
        
        RemoveSilenceEffectFromPlayer();
        var flag = BlastImpactBoss(playerInput.transform.position);
        if (flag)
        {
            Projectile_C001_10_Boss.Instance.BreakShield();
        }
        
        controller.GetComponent<ParticleSystem>().Stop();
        controller.DisableRelativePositionScript();
        Destroy(controller.gameObject,1f);
        actorController.SetDefaultGravityScale(4);

        DOVirtual.DelayedCall(0.5f, () =>
        {
            RemoveAllDamageCutEffectFromPlayer();
        }, false);


        if (flag)
        {
            ac.SetHitSensor(true);
            _voice?.PlayMyVoice(16);
            anim.SetTrigger("action");//坠落动画
            ac.SetGravityScale(2f);
            BattleStageManager.Instance.CauseIndirectDamage(_statusManager,
                (int)(_statusManager.maxBaseHP * 0.1f), false);
            
            yield return new WaitForSeconds(0.3f);
            
            ac.SetHitSensor(false);
            BattleStageManager.Instance.SetTimeScale(0.2f);

            yield return new WaitForSeconds(0.1f);
            
            BattleStageManager.Instance.SetTimeScale();

            yield return new WaitForSeconds(0.5f);
            
            ac.SwapWeaponVisibility(true);
            ac.SetGravityScale(4);
            _voice.BroadCastMyVoice(18);
            actorController.SetHitSensor(false);
            
            yield return new WaitUntil(() =>
        
                anim.GetCurrentAnimatorStateInfo(0).IsName("idle")
            );
            //anim.Play("idle");
            ac.SetHitSensor(true);
            ac.ResetGravityScale();
            actorController.SetHitSensor(true);
            
            //todo: 起身进行下一个动作
            
            anim.Play("attack");
            
            BackgroundTweener(Color.black);
        
            yield return new WaitForSeconds(1);
        
            bossBanner?.PrintSkillName("HB03_Action21");
            _orbGenerationTween?.Kill();
            currentWorld = 1;
            BattleStageManager.Instance.AddFieldAbility((int)BasicCalculation.EnemyAbility.JumpBoostWorld);
            BackgroundTweener(Color.white);
        
            yield return new WaitForSeconds(0.5f);

            var posArray = SetPlatformPositions();
            SummonPlatform(posArray,0);
            

            UI_DialogDisplayer.Instance?.
                EnqueueDialogShared(10101,
                    10031,BattleEffectManager.Instance?.notteHintClips[0]);
            
            StageCameraController.SwitchMainCameraFollowObject
                (Projectile_C001_10_Boss.Instance.gameObject);

            yield return new WaitForSeconds(2);

            StageCameraController.SwitchMainCameraFollowObject
                (_behavior.viewerPlayer);
            
            StageCameraController.SwitchToOtherCamera(0,true);
            
            yield return new WaitForSeconds(1);
            
            
            //生成第一个珠子
            GenerateOrbManually(_behavior.transform.position +
                                new Vector3(Random.Range(-3,3),Random.Range(0,5)),10,true);

            
            
            //暴风之箭
            
            bossBanner?.PrintSkillName("HB03_Action26");

            yield return new WaitForSeconds(1f);

            int rng = Random.Range(0,2);
            for (int i = 0; i <= 17; i++)
            {
                if (i % 2 == rng)
                {
                    continue;
                }
                SpawnProjectilesVertical(GetProjectileOfFormatName("action15_1"),
                    GetProjectileOfFormatName("action15_2"),new Vector2(-24 + i*3,
                        _behavior.targetPlayer.transform.position.y + 12));
            }
            
            
            //驱动爆破
            _tweener = transform.DOMove
                (new Vector3(-1.2f*(posArray[0].x), posArray[0].y + 5), 2.5f);

            yield return new WaitForSeconds(2.5f);
            
            SummonPlatform(posArray,1);
            
            bossBanner?.PrintSkillName("HB03_Action05");
            ac.TurnMove(_behavior.targetPlayer);
            BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3.5f);
            yield return new WaitForSeconds(1.5f);
        
            anim.SetTrigger("action");
            _voice.PlayMyVoice(3);

            yield return new WaitForSeconds(0.5f);
        
            DriveBuster();
            
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
            
            _orbGenerationTween = DOVirtual.DelayedCall(7f,() =>
            {
                GenerateOrbManually(_behavior.viewerPlayer.transform.position +
                                    new Vector3(Random.Range(-1, 1), Random.Range(0, 5)), 
                    5, true);
            },false).OnComplete(()=>_orbGenerationTween.Restart());
            
            
            yield return new WaitForSeconds(1f);
            
            //星辰射流，暴风之箭

            //移动到1的上面
            _tweener = transform.DOMove(posArray[1] + new Vector2(0, 5), 1.5f);
            ac.TurnMove(_behavior.targetPlayer);
            
            bossBanner?.PrintSkillName("HB03_Action19");
            
            
            anim.Play("attack");

            yield return new WaitForSeconds(0.7f);

            AstralStream(false);

            yield return new WaitForSeconds(0.7f);
        
            AstralStream(false);

            yield return new WaitForSeconds(0.7f);
            
            bossBanner?.PrintSkillName("HB03_Action26");
            AstralStream(false);

            yield return new WaitForSeconds(1f);

            rng = Random.Range(0,2);
            for (int i = 0; i <= 17; i++)
            {
                if (i % 2 == rng)
                {
                    continue;
                }
                SpawnProjectilesVertical(GetProjectileOfFormatName("action15_1"),
                    GetProjectileOfFormatName("action15_2"),new Vector2(-24 + i*3,
                        _behavior.targetPlayer.transform.position.y + 12));
            }
            
            //SummonPlatform(posArray,2);
            
            yield return new WaitForSeconds(1.5f);

            // 异界传送
            bossBanner?.PrintSkillName("HB03_Action09");
            ac.TurnMove(_behavior.targetPlayer);
            
            yield return new WaitForSeconds(1);

            if (Projectile_C001_5_Boss.Instance != null)
            {
                Destroy(Projectile_C001_5_Boss.Instance.gameObject);
                yield return null;
            }
        
            anim.Play("attack");
            yield return new WaitForSeconds(1f);
            OpenPortals2(new Vector2(0,posArray[2].y - 12));
            
            
            
            yield return new WaitForSeconds(1f);

            Transportation(2);
            

            bool reached = false;
            
            

            yield return new WaitForSeconds(1);
            
            SummonPlatform(posArray,2);

            yield return new WaitForSeconds(1);
            
            ac.TurnMove(_behavior.targetPlayer);

            _tweener = transform.DOMove(new Vector2(0, 37), 
                2f).OnComplete(() => reached = true);

            while (reached == false)
            {
                if (Vector2.Distance(_behavior.viewerPlayer.transform.position,
                        Projectile_C001_10_Boss.Instance.transform.position) < 2f)
                {
                    _tweener.Kill();
                    break;
                }

                yield return null;
            }

            
            DestroyAllPlatforms();
            _orbGenerationTween?.Kill();
            currentWorld = 0;
            BattleStageManager.Instance.RemoveFieldAbility((int)BasicCalculation.EnemyAbility.JumpBoostWorld);

            if (reached == false)
            {
                StageCameraController.SwitchMainCamera();
                Destroy(Projectile_C001_10_Boss.Instance.gameObject,0.1f);
                controller = PlayerPrepareDash(false);
                playerInput.DisableAndIdle();
                actorController.rigid.velocity = Vector2.zero;
                actorController.SetGravityScale(0);
                actorController.SetHitSensor(false);

                yield return new WaitForSeconds(0.7f);

                var distance = Vector2.Distance(transform.position, _behavior.viewerPlayer.transform.position);
                
                playerBurstFlyTween = actorController.transform.
                    DOMove(transform.position, 0.1f).SetEase(Ease.OutSine).SetUpdate(UpdateType.Fixed);

                yield return new WaitForSeconds(0.1f);
                
                anim.Play("knockback_1");
                CineMachineOperator.Instance.CamaraShake(8,0.1f);
                
                playerBurstFlyTween = actorController.transform.
                    DOMove(new Vector3(0,BattleStageManager.Instance.mapBorderB + 1.3f),0.8f).SetEase(Ease.InCubic).SetUpdate(UpdateType.Fixed);

                _tweener = transform.
                    DOMove(new Vector3(0,BattleStageManager.Instance.mapBorderB + 1.3f),0.8f).SetEase(Ease.InCubic).SetUpdate(UpdateType.Fixed);

                yield return new WaitForSeconds(0.8f);

                BlastImpactBossGround(transform.position);
                actorController.ResetGravityScale();
                actorController.SetHitSensor(true);
                
                controller.GetComponent<ParticleSystem>().Stop();
                controller.DisableRelativePositionScript();
                Destroy(controller.gameObject,1f);

                (_statusManager as SpecialStatusManager).currentBreak = -1;
                BattleStageManager.Instance.CauseIndirectDamage(_statusManager,
                    (int)(_statusManager.maxBaseHP * 0.1f), false);
                _voice?.PlayMyVoice(16);
                playerInput.EnableAndIdle();

            }
            else
            {
                ac.SwapWeaponVisibility((false));
                anim.Play("float");
                transform.position = new Vector3(0, 36);
                
                StageCameraController.SwitchMainCamera();
                StageCameraController.SwitchMainCameraFollowObject(gameObject);
                
                yield return new WaitForSeconds(2);
            
                bossBanner?.PrintSkillName("HB03_Action23");

                //_statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff, 100, 10);
                Doomsday();
                _voice?.BroadCastSpecificVoice(17,0);
                yield return new WaitForSeconds(0.5f);
                
                Destroy(Projectile_C001_10_Boss.Instance.gameObject,0.1f);
            
                StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
            
                actorController.SetHitSensor(true);
            
                yield return new WaitForSeconds(5);
            
                ac.SetHitSensor(true);
                ac.ResetGravityScale();
                anim.Play("idle");
                transform.position = new Vector3(0, 10);
            }
            



        }
        else
        {
            bossBanner?.PrintSkillName("HB03_Action11");
            WarpFX();
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff, 150, 10);
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff, 200, 20);
            
            yield return new WaitForSeconds(2);
            
            bossBanner?.PrintSkillName("HB03_Action23");
            
            Doomsday();
            _voice?.BroadCastSpecificVoice(17,0);
            yield return new WaitForSeconds(0.5f);
            
            StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
            
            actorController.SetHitSensor(true);
            
            yield return new WaitForSeconds(5);
            
            ac.SetHitSensor(true);
            ac.ResetGravityScale();
            anim.Play("idle");
            transform.position = new Vector3(0, 10);
        }
        
        
        
        ac.SwapWeaponVisibility(true);
        
        Destroy(windArea);
        Destroy(airWindArea);
        
        
        
        StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
        ResetTopBorder();
        BattleStageManager.Instance.DragonBlock = false;
        QuitAttack();

    }
    
    /// <summary>
    /// 暴风之箭
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action26()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action26");
        //BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3.5f);
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("attack");
        

        yield return new WaitForSeconds(0.4f);

        int rng = Random.Range(0,2);
        for (int i = 0; i <= 13; i++)
        {
            if (i % 2 == rng)
            {
                continue;
            }
            SpawnProjectilesVertical(GetProjectileOfFormatName("action15_1"),
                GetProjectileOfFormatName("action15_2"),new Vector2(-24 + i*4,
                    _behavior.targetPlayer.transform.position.y + 9));
        }
        
        yield return new WaitForSeconds(1);
        
        

        QuitAttack();
    }

    /// <summary>
    /// Summon Alberius
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action27()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action27");
        //BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3.5f);
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("attack");
        
        SpawnEnemyMinon(minion1,new Vector3(Random.Range(-4,4),
            gameObject.RaycastedPosition().y),999999);
        
        yield return new WaitForSeconds(1.25f);

        QuitAttack();
    }
    
    /// <summary>
    /// Summon mordecai
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action28()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action28");
        //BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3.5f);
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("attack");
        
        SpawnEnemyMinon(minion2,new Vector3(Random.Range(-4,4),
            gameObject.RaycastedPosition().y),999999);

        yield return null;
        
        Projectile_C001_13_Boss.Instance.SetIlia(gameObject);
        
        yield return new WaitForSeconds(1.25f);

        QuitAttack();
    }
    
    /// <summary>
    /// Summon Zethia
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action29()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB03_Action29");
        //BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3.5f);
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("attack");
        
        SpawnEnemyMinon(minion3,new Vector3(0,
            gameObject.RaycastedPosition().y),999999);

        yield return null;
        
        //Projectile_C001_13_Boss.Instance.SetIlia(gameObject);
        
        yield return new WaitForSeconds(1.25f);

        QuitAttack();
    }
    
    
    
    
    
    






    protected void WarpFX()
    {
        var fx = Instantiate(GetProjectileOfFormatName("action11"),
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
    }
    
    protected void SpawnProjectiles(Vector2 pos)
    {
        
        var muzzle = Instantiate(GetProjectileOfFormatName("action15_1"), pos, Quaternion.identity);
        var angle = BasicCalculation.AimTargetAngleZ(muzzle.transform, _behavior.targetPlayer.transform);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            InstantiateDirectionalRanged(GetProjectileOfFormatName("action15_2"),
                pos, InitContainer(false), 1, angle);
        }, false);
    }
    
    protected void SpawnProjectilesVertical(GameObject muzzle, GameObject proj, Vector2 pos)
    {
        
        var muzzleInstance = Instantiate(muzzle, pos, Quaternion.identity);
        //var angle = BasicCalculation.AimTargetAngleZ(muzzle.transform, _behavior.targetPlayer.transform);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            InstantiateDirectionalRanged(proj,
                pos, InitContainer(false), 1, -90);
        }, false);
    }

    protected void DriveBuster()
    {
        var container = Instantiate(attackContainer,
            transform.position,
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var muzzlePosition = new Vector3(transform.position.x + 1.6f * ac.facedir,
            transform.position.y - 0.6f);
        
        var muzzle = 
            Instantiate(GetProjectileOfFormatName("action01_3"), muzzlePosition,
                Quaternion.Euler(0, ac.facedir==1?0:180, 0), container.transform);
        
        float[] angleX = { 0.8f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.32f, 0.24f, 0.15f };
        float[] angleY = { 0.1f, 0.0f, -0.1f, -0.18f, -0.26f, -0.34f, -0.42f, -0.5f, -0.6f, -0.7f };
        List<GameObject> projectiles = new();

        var prefab = GetProjectileOfFormatName("action05");

        for (int i = 0; i < 10; i++)
        {
            projectiles.Add(Instantiate(prefab,
                muzzlePosition,Quaternion.identity,container.transform));
            
            var attack = projectiles[i].GetComponent<AttackFromEnemy>();
            var homing = projectiles[i].GetComponent<HomingAttack>();
            
            homing.angle = new Vector2(ac.facedir*angleX[i],angleY[i]);
            homing.target = _behavior.targetPlayer.transform;
            
            attack.enemySource = gameObject;
            attack.firedir = ac.facedir;
            attack.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
                83.2f,21f,
                1),120);
        }
    }

    protected void OverdriveBuster()
    {
        var container = Instantiate(GetProjectileOfFormatName("action06_L"),
         transform.position - new Vector3(0f,-0.5f),
            ac.facedir == 1 ? Quaternion.Euler(0,0,-80) : Quaternion.Euler(0, 0, -110),
            RangedAttackFXLayer.transform);

        var sealedContainer = container.GetComponent<EnemySealedContainer>();
        sealedContainer.SetEnemySource(gameObject);
        sealedContainer.SetTarget(_behavior.targetPlayer);
        
        container.GetComponent<RelativePositionRetainer>().SetParent(transform);
        container.transform.GetChild(0).GetComponent<AttackFromEnemy>().
            AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
                83.2f,21f,
                1),120);



        DOVirtual.DelayedCall(0.25f, () =>
        {
            var tweenerEndVal = ac.facedir == 1 ? new Vector3(0, 0, 0) : new Vector3(0, 0, -180);
            var tweener = container.transform.DORotate(tweenerEndVal, 1.15f).
                SetUpdate(UpdateType.Fixed).SetEase(Ease.InOutSine);
            var tweenerX =
                transform.DOMoveX(Mathf.Clamp(transform.position.x - ac.facedir * 2, 
                        BattleStageManager.Instance.mapBorderL,BattleStageManager.Instance.mapBorderR), 1.2f).SetEase(Ease.InCubic)
                    .SetUpdate(UpdateType.Fixed);
            var tweenerY = transform.DOMoveY(transform.position.y + 1, 1.2f).
                SetEase(Ease.OutCubic)
                .SetUpdate(UpdateType.Fixed);

            


        }, false);
        
        
        StatusManager.StatusManagerVoidDelegate handler = null;

        // handler = () =>
        // {
        //     Destroy(container);
        //     _statusManager.OnReceiveControlAffliction -= handler;
        // };
        //
        // _statusManager.OnReceiveControlAffliction += handler;


    }
    
    protected void AlchemicEnhancement()
    {
        _statusManager.ObtainTimerBuffs((int)BasicCalculation.BattleCondition.AlchemicCatridge,
            -1,
            3,3,-1);

        Instantiate(GetProjectileOfFormatName("action07_L"), transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);

    }

    protected void AlchemicGrenade()
    {
        var container = Instantiate(attackContainer,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
        Instantiate(GetProjectileOfFormatName("action08_3"),
            transform.position + new Vector3(ac.facedir,-1),
            Quaternion.identity,
            MeeleAttackFXLayer.transform);

        DOVirtual.DelayedCall(0.2f, () =>
        {
            for (int i = 0; i < 3; i++)
            {
                var proj = Instantiate(GetProjectileOfFormatName("action08_L"),
                    transform.position+new Vector3(ac.facedir,-1f,0),
                    Quaternion.identity,container.transform);

                var proj_controller = proj.GetComponent<Projectile_C001_1_Boss>();
                proj_controller.enemySource = gameObject;

                
                proj_controller.SetVelocity(
                        new(ac.facedir * proj_controller.velocity.x * (0.7f + i*0.3f),proj_controller.velocity.y*(1.25f-i*0.25f)));
                
            }
            var tweener =
                ac.rigid.DOMove
                        (transform.position + new Vector3(-3*ac.facedir,3), 0.3f).SetEase(Ease.OutCubic)
                    .SetUpdate(UpdateType.Fixed);
        },false);
        
        
        
    }

    /// <summary>
    /// 迅速接近目标
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    protected float FastFly(float speed,float offsetX,float offsetY, float turnPosition,float fixedTime = -1)
    {
        float moveDistance = 0;
        Vector2 endPosition;
        if (_behavior.targetPlayer.transform.position.x > BattleStageManager.Instance.mapBorderR - turnPosition)
        {
            endPosition = _behavior.targetPlayer.RaycastedPosition() + new Vector2(-offsetX, offsetY);
            moveDistance = Vector2.Distance(transform.position,endPosition);
            print("修正1");
        }
        else if (_behavior.targetPlayer.transform.position.x < BattleStageManager.Instance.mapBorderL + turnPosition)
        {
            endPosition = _behavior.targetPlayer.RaycastedPosition() + new Vector2(offsetX, offsetY);
            moveDistance = Vector2.Distance(transform.position,endPosition);
            print("修正2");
        }
        else
        {
            endPosition = _behavior.targetPlayer.RaycastedPosition() + new Vector2(-offsetX*ac.facedir,offsetY);
            moveDistance = Vector2.Distance(transform.position,
                endPosition);
            print("修正3");
        }

        var tweenTime = moveDistance / speed;

        if (fixedTime > 0)
        {
            tweenTime = fixedTime;
        }
        
        //print(tweenTime + "S");
        print(endPosition.y);


        var tween =
            transform.DOMove(endPosition, tweenTime).
                SetUpdate(UpdateType.Fixed).SetEase(Ease.InOutSine);

        return moveDistance / speed;
    }
    
    /// <summary>
    /// 迅速接近目标
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    protected float FastFlyOtherSide(float speed,float offsetX,float offsetY,float turnPosition,float distanceX, float fixedTime)
    {
        float moveDistance = 0;
        Vector2 endPosition;
        
        //如果玩家前进方向是朝向远离自身移动，并且当前位置不在墙角，并且位置超出了某范围。那么绕背
        
        //玩家的前进方向
        int dir = (int)_behavior.targetPlayer.transform.localScale.x;
        
        //玩家位置
        float playerPosX = _behavior.targetPlayer.transform.position.x;
        
        //位置差
        float diffX = Mathf.Abs(transform.position.x - playerPosX);
        print("DiffX"+diffX);
        //正在远离
        if (diffX > distanceX )
        {
            
            //往右远离
            if (dir == 1 && playerPosX < BattleStageManager.Instance.mapBorderR - turnPosition)
            {
                endPosition =
                    _behavior.targetPlayer.RaycastedPosition() + new Vector2(offsetX*1.1f, offsetY);
                moveDistance = Vector2.Distance(transform.position,endPosition);
                print("修正1");
            }
            //往左远离
            else if (dir == -1 && playerPosX > BattleStageManager.Instance.mapBorderL + turnPosition)
            {
                endPosition =
                    _behavior.targetPlayer.RaycastedPosition() + new Vector2(-offsetX*1.1f, offsetY);
                moveDistance = Vector2.Distance(transform.position,endPosition);
                print("修正2");
            }
            //卡左边墙角了
            else if(playerPosX <= BattleStageManager.Instance.mapBorderL + turnPosition)
            {
                endPosition =
                    _behavior.targetPlayer.RaycastedPosition() +
                    new Vector2(offsetX*1.1f, offsetY);
                moveDistance = Vector2.Distance(transform.position,endPosition);
                print("修正3");
            }
            //卡右边墙角了
            else if(playerPosX >= BattleStageManager.Instance.mapBorderR - turnPosition)
            {
                endPosition =
                    _behavior.targetPlayer.RaycastedPosition() +
                    new Vector2(-offsetX*1.1f, offsetY);
                moveDistance = Vector2.Distance(transform.position,endPosition);
                print("修正4");
            }
            else
            {
                endPosition =
                    _behavior.targetPlayer.RaycastedPosition() +
                    new Vector2(-ac.facedir*offsetX, offsetY);
                moveDistance = Vector2.Distance(transform.position,endPosition);
                print("修正5");
            }
            
            
        }
        //正在接近
        else
        {
            //卡左边墙角了
            if(playerPosX <= BattleStageManager.Instance.mapBorderL + turnPosition)
            {
                endPosition =
                    _behavior.targetPlayer.RaycastedPosition() +
                    new Vector2(offsetX, offsetY);
                moveDistance = Vector2.Distance(transform.position,endPosition);
                print("修正3");
            }
            //卡右边墙角了
            else if(playerPosX >= BattleStageManager.Instance.mapBorderR - turnPosition)
            {
                endPosition =
                    _behavior.targetPlayer.RaycastedPosition() +
                    new Vector2(-offsetX, offsetY);
                moveDistance = Vector2.Distance(transform.position,endPosition);
                print("修正4");
            }
            else
            {
                if (Random.Range(0, 2) == 0)
                {
                    endPosition =
                        _behavior.targetPlayer.RaycastedPosition() +
                        new Vector2(-ac.facedir*offsetX, offsetY);
                }
                else
                {
                    endPosition =
                        _behavior.targetPlayer.RaycastedPosition() +
                        new Vector2(ac.facedir*offsetX*0.85f, offsetY);
                }
                
                moveDistance = Vector2.Distance(transform.position,endPosition);
                print("修正5");
            }
        }
        
        
        

        var tweenTime = moveDistance / speed;

        if (fixedTime > 0)
        {
            tweenTime = fixedTime;
        }
        
        //print(tweenTime + "S");
        print(endPosition.y);
        
        Ease ease = Ease.InOutSine;

        // if (Mathf.Abs(endPosition.x - transform.position.x) < offsetX)
        // {
        //     ease = Random.Range(0, 2) == 0 ? Ease.InOutSine : Ease.InOutBounce;
        //     tweenTime *= 2;
        //     print("假动作");
        // }
        

        var tween =
            transform.DOMove(endPosition, tweenTime).
                SetUpdate(UpdateType.Fixed).SetEase(ease);

        return moveDistance / speed;
    }

    protected void StormShield()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action16", true),
            transform.position, InitContainer(false), 1);
        
        proj.AddComponent<RelativePositionRetainer>().SetParent(transform);
        
    }

    protected void StormShieldBoost()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action17_1", true),
            transform.position, InitContainer(false), 1);
        
        proj.AddComponent<RelativePositionRetainer>().SetParent(transform);
    }

    protected float FlyToBorder(float spd = 12)
    {
        ac.TurnMove(_behavior.targetPlayer);
        
        var endPos = _behavior.targetPlayer.transform.position.x < 0
            ? new Vector2(BattleStageManager.Instance.mapBorderL + 1, _behavior.targetPlayer.RaycastedPosition().y + 3)
            : new Vector2(BattleStageManager.Instance.mapBorderR - 1, _behavior.targetPlayer.RaycastedPosition().y + 3);

        var distance = Vector2.Distance(endPos, transform.position);
        
        var endPos2 = _behavior.targetPlayer.transform.position.x >= 0 
            ? new Vector2(BattleStageManager.Instance.mapBorderL + 1, _behavior.targetPlayer.RaycastedPosition().y + 3)
            : new Vector2(BattleStageManager.Instance.mapBorderR - 1, _behavior.targetPlayer.RaycastedPosition().y + 3);

        var distance2 = BattleStageManager.Instance.mapBorderR - BattleStageManager.Instance.mapBorderL;

        
        _tweener = 
            (transform.DOMove(endPos, distance / spd).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed))
            .OnComplete(() =>
            {
                ac.TurnMove(_behavior.targetPlayer);
                _tweener = transform.DOMove(endPos2, distance2 * 0.8f / spd).SetUpdate(UpdateType.Fixed);
            });
        
        
        

        return (distance + distance2) / spd;


    }

    protected float DashToBorder(int dir, float spd = 10)
    {
        var endPos = dir > 0
            ? BattleStageManager.Instance.mapBorderR
            : BattleStageManager.Instance.mapBorderL;

        var distance = Mathf.Abs(endPos - transform.position.x);

        _tweener = transform.DOMoveX(endPos, distance / spd).
            SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed);

        return distance / spd;
    }

    protected float SurgingGalesA(float posY, float delay)
    {
        Vector2[] generatePositions = new Vector2[]
        {
            new(-4, posY), new(4, posY),
            //new(-6, posY), new(6, posY),
            new(-12, posY), new(12, posY),
            //new(-14, posY), new(14, posY),
            new(-20, posY), new(20, posY),
            //new(-22, posY), new(22, posY),
            new(-28,posY),new(28,posY)
        };

        var container = InitContainer(false,8);
        var prefab = GetProjectileOfFormatName("action17_2");
        
        for (int i = 0; i < 4; i++)
        {
            var delayTime = delay * i;
            var positionA = generatePositions[2 * i];
            var positionB = generatePositions[2 * i + 1];

            DOVirtual.DelayedCall(delayTime, () =>
            {
                InstantiateRanged(prefab, positionA, container, 1);
                InstantiateRanged(prefab, positionB, container, 1);
            }, false);
        }

        return delay * 4;

    }
    
    protected void SurgingGalesB(float posY)
    {
        Vector2[] generatePositions = new Vector2[]
        {
            //new(-2, posY), new(2, posY),
            new(-8, posY), new(8, posY),
            //new(-10, posY), new(10, posY),
            new(-16, posY), new(16, posY),
            //new(-18, posY), new(18, posY),
            new(-24, posY), new(24, posY),
            //new(-26,posY),new(26,posY)
            new(0,posY)
        };

        var container = InitContainer(false,8);
        var prefab = GetProjectileOfFormatName("action17_2");
        
        foreach (var position in generatePositions)
        {
            InstantiateRanged(prefab, position, container, 1);
        }

    }

    private void OpenPortals()
    {
        var container = InstantiateSealedContainer(GetProjectileOfFormatName("action09_L", true),
            Vector3.zero, RangedAttackFXLayer.transform);
        
        Projectile_C001_5_Boss.Instance.SetEnemySource(gameObject);

    }
    
    private void OpenPortals2(Vector2 position)
    {
        var container = InstantiateSealedContainer(GetProjectileOfFormatName("action09_L2", true),
            position, RangedAttackFXLayer.transform);
        
        Projectile_C001_5_Boss.Instance.SetEnemySource(gameObject);

    }
    private void Transportation(int capacity = 3)
    {
        var transPoint = Projectile_C001_5_Boss.Instance.GetChild(Random.Range(0,capacity));
        Projectile_C001_5_Boss.Instance.Transport(transform,transPoint,2.5f);
    }

    protected void OtherWorldGate_Blast(Vector3 position)
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action10_L"), position, container, 1);

        proj.GetComponentInChildren<AttackFromEnemy>().BeforeAttackHit += PurgedShapeShiftingOfTarget;

    }

    protected void AlchemicShield()
    {


        Instantiate(GetProjectileOfFormatName("action18", true), transform.position,
            Quaternion.identity, MeeleAttackFXLayer.transform);

        //ShieldOn = true;
    }

    private void AstralStream(bool avoidable = true)
    {
        var prefab = avoidable ? GetProjectileOfFormatName("action19_1", true) :
            GetProjectileOfFormatName("action19_2", true);
        var pos = _behavior.targetPlayer.RaycastedPosition();

        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, pos, RangedAttackFXLayer.transform,
            new Vector2(12, 3), Vector2.zero, avoidable, 1, 2, 90,
            0.5f, true, false);

        DOVirtual.DelayedCall(2, () =>
        {
            InstantiateRanged(prefab,
                pos, InitContainer(false), 1)
                .GetComponent<AttackFromEnemy>()
                .AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Blindness,
                    1,Random.Range(7,9),1),100);
            
        }, false);

        
    }

    private void AstralSurge_HintSide()
    {
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, new Vector3(-17, -6),
            RangedAttackFXLayer.transform, new Vector2(32, 18), Vector2.zero, false, 1,
            3, 90, 1f, true, false);
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, new Vector3(17, -6),
            RangedAttackFXLayer.transform, new Vector2(32, 18), Vector2.zero, false, 1,
            3, 90, 1f, true, false);
    }

    private void AstralSurge_Projectiles()
    {
        DOVirtual.DelayedCall(1.33f, () =>
        {
            var projCenter = InstantiateSealedContainer(GetProjectileOfFormatName("action20_1",
                true), new Vector3(0, -5),false, 1);
        },false);
        
        var projLeft = InstantiateSealedContainer(GetProjectileOfFormatName("action20_2",
            true), new Vector3(0, -6), false, 1);
        
        var projRight = InstantiateSealedContainer(GetProjectileOfFormatName("action20_2",
            true), new Vector3(0, -6), false, -1);
        
    }

    private void AstralSurge_CenterCore()
    {
        InstantiateSealedContainer(GetProjectileOfFormatName("action20_3",
            true), new Vector3(0, 10), RangedAttackFXLayer.transform);
    }

    private void BackgroundTweener(Color color)
    {
        if (_backgroundRenderer == null)
        {
            _backgroundRenderer = BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background2") as SpriteRenderer;
        }

        _backgroundTweener = _backgroundRenderer.DOColor(color, 1);
    }

    private void GenerateOrbsNormal(int flag, float interval = 20)
    {
        if (flag == 0)
        {
            _orbGenerationTween?.Kill();
            if (Projectile_C001_8_Boss.Instance != null)
            {
                Destroy(Projectile_C001_8_Boss.Instance.gameObject);
            }

            currentWorld = 0;
            return;
        }
        
        InstantiateOrb(interval);
        currentWorld = 1;


        _orbGenerationTween = DOVirtual.DelayedCall(interval, () =>
        {
            InstantiateOrb(interval);
            
        }, false).OnComplete(()=>_orbGenerationTween.Restart());


    }

    private GameObject GenerateOrbManually(Vector2 position, float existTime, bool hint = false)
    {
        var orb = Instantiate(orbPrfeb,
            BattleStageManager.Instance.OutOfRangeCheck
                (position),
            Quaternion.identity, RangedAttackFXLayer.transform
        );
            
        orb.GetComponent<Projectile_C001_8_Boss>().InvokeDestroy(existTime - 0.1f);

        if (hint)
        {
            Instantiate(GetProjectileOfFormatName("action21_hint", true),
                orb.transform.position, Quaternion.identity, orb.transform);
        }
        

        return orb;
    }

    private void InstantiateOrb(float interval)
    {
        var orb = Instantiate(orbPrfeb,
            BattleStageManager.Instance.OutOfRangeCheck
                (_behavior.viewerPlayer.RaycastedPosition() + new Vector2(Random.Range(-8,8),Random.Range(2,4))),
            Quaternion.identity, RangedAttackFXLayer.transform
        );
            
        orb.GetComponent<Projectile_C001_8_Boss>().InvokeDestroy(interval - 0.1f);
    }

    /// <summary>
    /// 扩展上边界
    /// </summary>
    private void ExtendTopBorder()
    {
        BattleStageManager.Instance.SetCameraTopBorder(42.5f);
        BattleStageManager.Instance.SetTopBorder(42.5f);
        BattleStageManager.Instance.RefreshCameraBorder();
    }
    
    private void ResetTopBorder()
    {
        BattleStageManager.Instance.SetCameraTopBorder(16);
        BattleStageManager.Instance.SetTopBorder(24);
        BattleStageManager.Instance.RefreshCameraBorder();
    }

    private GameObject GenerateGroundWindArea()
    {
        var fx = Instantiate(GetProjectileOfFormatName("action23_1", true),
            Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);

        return fx;
    }

    private void GrantSilenceEffectToPlayer()
    {
        var silenceBuff = new TimerBuff((int)BasicCalculation.BattleCondition.Silence,
            1, -1, 1, 8103403);
        

        var playerStat = _behavior.viewerPlayer.GetComponent<StatusManager>();
        
        silenceBuff.dispellable = false;
        
        silenceBuff.OnBuffStart += (stat) =>
        {
            stat.GetComponent<ActorController>().silence = true;
        };
        silenceBuff.OnBuffRemove += (stat) =>
        {
            if (stat.HasCondition((int)BasicCalculation.BattleCondition.Silence) == false)
            {
                stat.GetComponent<ActorController>().silence = false;
            }
        };

        playerStat.ObtainTimerBuff(silenceBuff);
        
    }

    private void GrantDamageCutEffectToPlayer()
    {
        var damageCutBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageCutConst,
            2000, -1, 5, DamageCutSPID);
        var playerStat = _behavior.viewerPlayer.GetComponent<StatusManager>();
        playerStat.ObtainTimerBuff(damageCutBuff);

    }

    private void RemoveAllDamageCutEffectFromPlayer()
    {
        var playerStat = _behavior.viewerPlayer.GetComponent<StatusManager>();
        playerStat.RemoveAllConditionWithSpecialID(DamageCutSPID);
    }
    
    private GameObject GenerateAirWindArea()
    {
        var fx = Instantiate(GetProjectileOfFormatName("action23_2", true),
            new Vector3(0,36), Quaternion.identity, RangedAttackFXLayer.transform);
        
        var windBall = Instantiate(GetProjectileOfFormatName("action23_5", true),
            new Vector3(0,37), Quaternion.identity, RangedAttackFXLayer.transform);
        

        return fx;
    }

    private void RemoveSilenceEffectFromPlayer()
    {
        var playerStat = _behavior.viewerPlayer.GetComponent<StatusManager>();
        playerStat.RemoveAllConditionWithSpecialID(8103403);
    }

    private Projectile_C001_11_Boss PlayerPrepareDash(bool first = true)
    {
        var trailEffect = Instantiate(GetProjectileOfFormatName("action23_3", true),
            _behavior.viewerPlayer.transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
        trailEffect.GetComponent<RelativePositionRetainer>().SetParent(_behavior.targetPlayer.transform);

        if (first)
        {
            var hint = Instantiate(GetProjectileOfFormatName("action23_hint", true),
                new Vector3(0,0), Quaternion.identity, RangedAttackFXLayer.transform);
        }


        return trailEffect.GetComponent<Projectile_C001_11_Boss>();
    }


    private void DestroyAllPlatforms()
    {
        var prefab = GetProjectileOfFormatName("action23_7");
        var container = InitContainer(false);

        for (int i = 0; i < _platformInstances.Count; i++)
        {
            InstantiateRanged(prefab, _platformInstances[i].transform.position, container, 1);
            Destroy(_platformInstances[i],0.1f);
        }
        
        _platformInstances.Clear();
        
    }
    private bool BlastImpactBoss(Vector2 position)
    {
        var blastEffect = InstantiateRanged
        (GetProjectileOfFormatName("action23_4", true), position,
            InitContainer(false), 1);

        var backForce = blastEffect.GetComponent<ForcedAttackFromEnemy>();
        backForce.target = _behavior.viewerPlayer;

        var offset = Mathf.Abs(position.x);
        
        backForce.attackInfo[0].constDmg[0] = 7999 + Mathf.Min(1500, offset * offset * 6);
        
        CineMachineOperator.Instance.CamaraShake(20,1f);
        
        var damageCut = _behavior.viewerPlayer.
            GetComponent<StatusManager>().GetConditionTotalValue((int)BasicCalculation.BattleCondition.DamageCutConst);

        bool success = true;

        if (damageCut < 2000)
        {
            return false;
        }else if (damageCut > 8000)
        {
            return true;
        }
        else
        {
            if (offset > 10)
                return false;
            else
            {
                success = true;
            }
        }

        

        return success;

    }

    private void BlastImpactBossGround(Vector2 position)
    {
        var blastEffect = InstantiateRanged
        (GetProjectileOfFormatName("action23_4", true), position,
            InitContainer(false), 1);

        var backForce = blastEffect.GetComponent<ForcedAttackFromEnemy>();
        backForce.target = _behavior.viewerPlayer;

        CineMachineOperator.Instance.CamaraShake(20,1f);

    }
    private void Doomsday()
    {
        Projectile_C001_10_Boss.Instance.BreakShield();
        Projectile_C001_10_Boss.Instance.ChargeOff();
        
        var doomsday = InstantiateRanged(GetProjectileOfFormatName("action23_6", true),
            new Vector3(0, 0), InitContainer(false), 1);
        
    }

    private Vector2[] SetPlatformPositions()
    {
        var rng = Random.Range(0, 2);
        Vector2[] positions =
        {
            new(0,4),new(0,18),new(0,32)
        };

        if (rng == 0)
        {
            positions[0].x = -10;
            positions[1].x = 10;
        }else if (rng == 1)
        {
            positions[0].x = 10;
            positions[1].x = -10;
        }

        return positions;

    }

    private void SummonPlatform(Vector2[] positions, int id)
    {
        var instance = Instantiate(platformPrefab, positions[id], Quaternion.identity,
            RangedAttackFXLayer.transform);

        if (id == 1)
        {
            var hint = Instantiate(GetProjectileOfFormatName("action25_hint", true),
                new Vector3(0, (positions[0].y + positions[1].y) / 2), Quaternion.identity,
                RangedAttackFXLayer.transform);
            var tmp = hint.GetComponentInChildren<TextMeshPro>();
            var key = PlayerInput.GetInputKeyPath("MoveU");
            if (GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
            {
                tmp.text = $"【二段跳强化】增益期间\n按下{key}键可以跳的更高";
            }
            else
            {
                tmp.text = $"Press {key} to jump higher when \n\"Double Jump Boost\" is in effect!";
            }
        }

        _platformInstances.Add(instance);
    }
    
    
}
