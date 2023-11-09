using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyMoveController_HB01_Legend2 : EnemyMoveManager
{
    private VoiceControllerEnemy voice;

    protected Vector2 firstPositionOfFlameArea = Vector2.zero;

    protected bool counterFang = false;
    protected SpriteRenderer backGroundSpriteRenderer;

    [SerializeField] private GameObject weaponLeft;
    [SerializeField] private GameObject weaponRight;
    
    private bool hint1Displayed = false;
    private bool hint2Displayed = false;
    private bool hint3Displayed = false;
    private bool hint4Displayed = false;

    protected enum VoiceGroupEnum
    {
        StandardAttack = 0,
        WarpAttack = 1,
        CarmineRush = 2,
        FlameRaid = 3,
        BrightCarmineRush = 4,
        SavageFlameRaid = 5,
        ScarletInferno = 6,
        BlazingEnhancement = 7,
        FlameHeart = 8,
        Counter = 9,
        Inheritor = 10
    }

    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerHumanoid>();
        voice = GetComponentInChildren<VoiceControllerEnemy>();
        GetAllAnchors();
        attackContainer = BattleStageManager.Instance.attackContainerEnemy;
        _statusManager.OnTakeDirectDamage += CounterFangActive;
        (_statusManager as SpecialStatusManager).onBreak += BlackScreenFadeOut;
        backGroundSpriteRenderer = GameObject.Find("Background2").GetComponent<SpriteRenderer>();
        _statusManager.OnHPBelow0 += WorldBreakEffect;
        BattleStageManager.Instance.RefreshMapInfo();
    }


    /// <summary>
    /// Single dodge combo
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action01()
    {
        yield return new WaitUntil(() => !ac.hurt);

        ac.OnAttackEnter();
        //ac.SetCounter(true);
        anim.Play("comboDodge0");
        voice.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.13f);

        ComboDodge1();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("idle");

        QuitAttack();
    }



    /// <summary>
    /// WarpAttack
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action02()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);

        ac.OnAttackEnter(999);

        yield return new WaitForSeconds(0.5f);
        anim.Play("warp");
        DisappearRenderer();
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer, 1.2f);
        voice.PlayMyVoice((int)VoiceGroupEnum.WarpAttack);
        WarpAttack_ShadowEffect();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.33f);
        WarpAttack_Disappear(true);


        yield return new WaitForSeconds(0.22f);
        var pos = _behavior.targetPlayer.transform.position;
        var dir = LockFaceDir(_behavior.targetPlayer);

        yield return new WaitForSeconds(.2f);
        WarpAttack_Appear();
        anim.Play("comboDodge1");
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        ac.SetGravityScale(0);
        BackWarp(pos, dir, 3f, 4.5f);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.17f);
        WarpAttack_Move(pos);

        ac.ResetGravityScale();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.236f);
        WarpAttack_Attack();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// Combo1-Combo3(CMB_A)
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action03()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetCounter(true);


        var hint = GenerateWarningPrefab("action03_1", transform.position, transform.rotation,
            MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(hintbar.warningTime);
        Destroy(hint, 0.5f);
        anim.Play("combo1");
        voice.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.13f);
        Combo1();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);
        anim.Play("combo2");
        voice.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.05f);
        Combo2_Attack();
        Combo2_StraightDashMove();
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);

        yield return new WaitForSeconds(0.25f);

        GenerateWarningPrefab("action03_2",
            transform.position,
            transform.rotation,
            MeeleAttackFXLayer.transform);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f);
        // if (!anim.GetBool("isGround"))
        // {
        //     anim.Play("fall");
        //     yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        // }

        //yield return new WaitUntil(() => anim.GetBool("isGround"));
        anim.Play("combo3");
        voice.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.25f);
        Combo3();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// Carmine Rush
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action04()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action04");


        var hint = GenerateWarningPrefab("action04", transform.position, transform.rotation,
            MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(hintbar.warningTime);
        Destroy(hint);
        anim.Play("s1_rush");
        voice.PlayMyVoice((int)VoiceGroupEnum.CarmineRush);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);
        ac.SetGravityScale(0);
        var container = CarmineRush_Attack();
        ActiveShadowSkill1(1);

        yield return new WaitForSeconds(0.05f);
        Vector2 origin = CarmineRush_Forward();

        yield return new WaitForSeconds(0.2f);
        CarmineRush_Back(origin);

        yield return new WaitForSeconds(0.1f);
        ac.SetFaceDir(-ac.facedir);
        ac.ResetGravityScale();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        anim.Play("s1_recover");

        yield return null;


        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("idle");
        QuitAttack();

    }

    /// <summary>
    /// Combo7(CMB_D)
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action05()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        //GenerateWarningPrefab(WarningPrefabs[7],transform.position,transform.rotation, MeeleAttackFXLayer.transform);


        BattleEffectManager.Instance.SpawnExclamation(gameObject,
            new Vector3(transform.position.x, transform.position.y + 4));

        yield return new WaitForSeconds(0.8f);
        anim.Play("combo7");
        _statusManager.ObtainUnstackableTimerBuff((int)BasicCalculation.BattleCondition.BlazewolfsRush,
            -1, -1, 1);
        voice.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);


        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= (5f / 29f));
        Combo7_Part1();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= (17f / 29f));
        Combo7_Part1();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("comboDodge1");
        transform.position = new Vector3(transform.position.x, transform.position.y + 6f);
        ac.SetGravityScale(0);
        SetGroundCollider(false);

        yield return new WaitForSeconds(0.2f);
        _tweener = transform.DOMoveY(transform.position.y - 6f, 0.3f).SetEase(Ease.OutExpo);
        SetGroundCollider(true);

        yield return new WaitForSeconds(0.15f);
        Combo7_Part2();
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        ac.ResetGravityScale();

        anim.Play("idle");

        //yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        QuitAttack();


    }


    /// <summary>
    /// Combo4(CMB_B)
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action06()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);


        BattleEffectManager.Instance.SpawnExclamation(gameObject,
            new Vector3(transform.position.x, transform.position.y + 4));

        yield return new WaitForSeconds(1.5f);
        anim.Play("combo4");
        ac.SetGravityScale(0);
        SetGroundCollider(false);
        Combo4_Part1();
        voice.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.65f);

        bool needWarp = false;
        var distance = ac.facedir * (_behavior.targetPlayer.transform.position.x - transform.position.x);

        if ((distance > 8 || distance < 0) ||
            BasicCalculation.CheckRaycastedPlatform(gameObject) !=
            BasicCalculation.CheckRaycastedPlatform(_behavior.targetPlayer))
        {
            needWarp = true;
            Combo4_WarpIn();
            ac.SetCounter(false);
        }

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.69f);
        anim.Play("comboDodge1");
        if (needWarp)
        {
            Combo4_WarpOut();
            ac.SetCounter(true);
        }

        yield return new WaitForSeconds(0.1f);

        var targetPos = new Vector2(transform.position.x + 1.5f * ac.facedir, transform.position.y - 4f);
        targetPos = BattleStageManager.Instance.OutOfRangeCheck(targetPos);
        _tweener = transform.DOMove
            (targetPos, 0.1f).SetEase(Ease.InCubic);

        yield return new WaitForSeconds(0.05f);
        Combo4_Part2();
        ac.ResetGravityScale();
        SetGroundCollider(true);
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);



        anim.Play("idle");
        QuitAttack();
    }


    /// <summary>
    /// Combo5-6(CMB_C)
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action07()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);


        var hint = GenerateWarningPrefab("action07", transform.position, transform.rotation,
            MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(hintbar.warningTime);
        Destroy(hint);
        anim.Play("combo5");
        ac.SetGravityScale(0);
        Combo5();
        voice.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f);
        anim.Play("combo6");
        anim.speed = 0;

        yield return new WaitForSeconds(0.15f);
        anim.speed = 1;

        yield return new WaitForSeconds(0.05f);

        var oldPos = transform.position;
        Combo6();
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);
        ac.ResetGravityScale();


        if (Mathf.Abs(_behavior.targetPlayer.transform.position.x - transform.position.x) >
            Mathf.Abs(oldPos.x - _behavior.targetPlayer.transform.position.x))
        {
            transform.position = oldPos;
        }
        else
        {
            transform.position = BattleStageManager.Instance.OutOfRangeCheck(transform.position);
        }

        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// Flame Raid
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action08()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action08");

        yield return new WaitForSeconds(0.5f);
        var hint = GenerateWarningPrefab("action08", transform.position, transform.rotation,
            MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(hintbar.warningTime);
        anim.Play("s2");
        
        voice.PlayMyVoice((int)VoiceGroupEnum.FlameRaid);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15f);
        Destroy(hint, 0.3f);
        FlameRaid();
        ActiveShadowSkill1(2);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// Bright Carmine Rush
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action09()
    {
        yield return new WaitUntil(() => ac.grounded && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action09");

        var hint =
            GenerateWarningPrefab
            ("action04", transform.position, Quaternion.Euler(0, ac.facedir == 1 ? 0 : 180, 0),
                RangedAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        var hint2 =
            GenerateWarningPrefab("action04", transform.position,
                (Quaternion.Euler(0, ac.facedir == 1 ? 0 : 180, 20)), RangedAttackFXLayer.transform);
        var hint3 =
            GenerateWarningPrefab("action04", transform.position,
                (Quaternion.Euler(0, ac.facedir == 1 ? 0 : 180, -20)), RangedAttackFXLayer.transform);

        var hint4 = GenerateWarningPrefab("action09", transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        hint4.GetComponent<EnemyAttackHintBarChaser>().target = _behavior.targetPlayer;
        hint4.transform.rotation = (ac.facedir == 1 ? transform.rotation : Quaternion.Euler(0, 0, 180));
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer, 2f);

        yield return new WaitForSeconds(hintbar.warningTime);

        anim.Play("s1_rush");
        voice.PlayMyVoice((int)VoiceGroupEnum.BrightCarmineRush);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);
        ac.SetGravityScale(0);
        var container = BrightCarmineRush_Attack1();

        yield return new WaitForSeconds(0.05f);
        Vector2 origin = CarmineRush_Forward();
        //CamineRush_Part2(container);

        yield return new WaitForSeconds(0.2f);
        CarmineRush_Back(origin);

        yield return new WaitForSeconds(0.1f);
        ac.SetFaceDir(-ac.facedir);
        ac.ResetGravityScale();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        //anim.Play("s1_recover");
        ac.SetFaceDir(-ac.facedir);
        anim.Play("s1_rush", -1, 0.2f);
        BrightCarmineRush_Attack2(container, hint4.transform.eulerAngles);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);
        ac.ResetGravityScale();
        BrightCarmineRush_Attack3(container);
        Destroy(hint);
        Destroy(hint2);
        Destroy(hint3);


        yield return new WaitForSeconds(0.05f);
        CarmineRush_Forward();
        //CarmineRush_Attack();

        yield return new WaitForSeconds(0.2f);
        CarmineRush_Back(origin);
        Destroy(hint4);

        yield return new WaitForSeconds(0.1f);
        ac.SetFaceDir(-ac.facedir);
        ac.ResetGravityScale();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("s1_recover");
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("idle");
        QuitAttack();

    }
    
    

    /// <summary>
    /// Savage Flame Raid
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action10()
    {
        yield return new WaitUntil(() => ac.grounded && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action10");

        yield return new WaitForSeconds(.5f);
        var hint = GenerateWarningPrefab("action10", transform.position+Vector3.up,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        
        yield return new WaitForSeconds(hintbar.warningTime);
        anim.Play("s2");
        voice.PlayMyVoice((int)VoiceGroupEnum.SavageFlameRaid);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15f);
        Destroy(hint,0.3f);
        SavageFlameRaid();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("idle");
        QuitAttack();
    }


    public IEnumerator HB01_Action11()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action11");
        StageCameraController.SwitchOverallCameraFollowObject(gameObject);
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(.5f);
        var hint = GenerateWarningPrefab("action11", transform.position+Vector3.up,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        
        yield return new WaitForSeconds(hintbar.warningTime);
        anim.Play("buff");
        float animTime;
        
        
        voice.BroadCastMyVoice((int)VoiceGroupEnum.ScarletInferno);
        
        Destroy(hint,0.5f);
        ScarletInferno();
        Invoke(nameof(ScarletInferno_Aggerate),2f);
        Invoke(nameof(ScarletInferno_Absorb),5.5f);
        Invoke(nameof(ScarletInferno_Buff),7f);
        StageCameraController.SwitchMainCamera();
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        
        anim.Play("idle");
        QuitAttack();
        
        
    }

    /// <summary>
    /// blazing azure fixed
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action15_1()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        var SecondFloorWatcher = GetAnchoredSensorOfName("M2F");
        StageCameraController.SwitchOverallCameraFollowObject(SecondFloorWatcher);
        yield return new WaitForSeconds(0.5f);
        StageCameraController.SwitchOverallCamera();
        
        bossBanner?.PrintSkillName("HB01_Action15");
        var random = Random.Range(0, 2);
        if (random == 1)
        {
            GenerateWarningPrefab("action15_1", new Vector3(12,-2),Quaternion.identity, RangedAttackFXLayer.transform);
            firstPositionOfFlameArea = new Vector2(12f, -2);
        }
        else
        {
            GenerateWarningPrefab("action15_1", new Vector3(-12,-2),Quaternion.identity, RangedAttackFXLayer.transform);
            firstPositionOfFlameArea = new Vector2(-12f, -2);
        }

        yield return new WaitForSeconds(2.5f);
        StageCameraController.SwitchMainCamera();
        yield return new WaitForSeconds(1.3f);
        
        anim.Play("side");
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.79f);
        
        GenerateFlameArea(firstPositionOfFlameArea);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        
        

        QuitAttack();
        
    }

    public IEnumerator HB01_Action15_2()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        
        bossBanner?.PrintSkillName("HB01_Action15");
        
        yield return new WaitForSeconds(2f);
        
        var hint = GenerateWarningPrefab("action15_2",
            _behavior.targetPlayer.transform.position,Quaternion.identity, RangedAttackFXLayer.transform);
        hint.GetComponent<EnemyAttackHintBarChaser>().target = _behavior.targetPlayer;
        
        
        yield return new WaitForSeconds(3.8f);
        var pos = hint.transform.position;
        
        anim.Play("side");
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.79f);

        GenerateFlameArea(pos);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");

        QuitAttack();


    }

    
    public IEnumerator HB01_Action15_3()
    {
        if (hint3Displayed == false)
        {
            hint3Displayed = true;
            UI_DialogDisplayer.Instance?.
                EnqueueDialogShared(10101,10017,BattleEffectManager.Instance?.notteHintClips[1]);
        }

        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        var SecondFloorWatcher = GetAnchoredSensorOfName("M2F");
        StageCameraController.SwitchOverallCameraFollowObject(SecondFloorWatcher);
        yield return new WaitForSeconds(0.5f);
        StageCameraController.SwitchOverallCamera();
        
        bossBanner?.PrintSkillName("HB01_Action15");
        var random = Random.Range(0, 2);
        Vector2 flameArea;
        if (random == 1)
        {
            if (firstPositionOfFlameArea.x > 0)
            {
                GenerateWarningPrefab("action15_1", new Vector3(6,6),Quaternion.identity, RangedAttackFXLayer.transform);
                flameArea = new Vector2(6, 6);
            }
            else
            {
                GenerateWarningPrefab("action15_1", new Vector3(-6,6),Quaternion.identity, RangedAttackFXLayer.transform);
                flameArea = new Vector2(-6, 6);
            }
        }
        else
        {
            if (firstPositionOfFlameArea.x > 0)
            {
                GenerateWarningPrefab("action15_1", new Vector3(6,-10),Quaternion.identity, RangedAttackFXLayer.transform);
                flameArea = new Vector2(6, -10);
            }
            else
            {
                GenerateWarningPrefab("action15_1", new Vector3(-6,-10),Quaternion.identity, RangedAttackFXLayer.transform);
                flameArea = new Vector2(-6, -10);
            }
        }

        yield return new WaitForSeconds(2.5f);
        StageCameraController.SwitchMainCamera();
        yield return new WaitForSeconds(1.3f);
        
        anim.Play("side");
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.79f);
        
        GenerateFlameArea(flameArea);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        
        

        QuitAttack();
        
    }
    
    
    /// <summary>
    /// fiery soul
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action16(int lvSelf,int lvOthers)
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("HB01_Action16");

        yield return new WaitForSeconds(0.5f);
        
        voice?.BroadCastMyVoice((int)VoiceGroupEnum.FlameHeart);
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("float_entire");
        
        yield return null;
        
        SetFlameBuffToAllTargets(lvSelf,lvOthers);

        // if (lvSelf == 0 && _behavior.GetState().Item1 == 2 && _behavior.GetState().Item2 <= 5)
        // {
        //     UI_DialogDisplayer.Instance?.
        //         EnqueueDialogShared(10101,10019,BattleEffectManager.Instance?.notteHintClips[0]);
        // }

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
        
    }

    /// <summary>
    /// Rule of creation: Baptism...
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action17()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("HB01_Action17");

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("float_entire");
        
        yield return null;
        
        SetWorld_BlackScreenFade();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        SetWorld(1);
        
        anim.Play("idle");
        
        QuitAttack();
        
    }


    /// <summary>
    /// Rule of creation: Judgment...
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action18()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("HB01_Action18");

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("float_entire");
        
        yield return null;
        
        SetWorld_BlackScreenFade();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        SetWorld(2);
        
        anim.Play("idle");
        
        QuitAttack();
        
    }
    
    /// <summary>
    /// Reset
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action19()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("HB01_Action19");

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("float_entire");
        
        yield return null;
        
        SetWorld_BlackScreenFade();
        Invoke("BlackScreenFadeOut",1f);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        SetWorld(0);
        

        anim.Play("idle");
        
        QuitAttack();
        
    }



    /// <summary>
    /// Blazing Blitz
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action20()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("HB01_Action20");

        yield return new WaitForSeconds(0.5f);
        
        var hint = GenerateWarningPrefab("action20_1",transform.position,Quaternion.identity, MeeleAttackFXLayer.transform);
        
        yield return new WaitForSeconds(1.6f);
        
        Destroy(hint);
        
        anim.Play("comboDodge2");
        voice?.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);

        yield return null;
        
        BlazingBlitzAndLeaveAShadow();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }

    /// <summary>
    /// Counter Blade
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action21()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        ac.OnAttackEnter(999);

        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action21");
        yield return new WaitForSeconds(1f);

        anim.Play("counter_enter");
        _statusManager.OnSpecialBuffDelegate?.Invoke("CounterReady");

        var counterTime = 0f;
        counterFang = false;
        while (counterFang == false && counterTime < 2f)
        {
            counterTime += Time.deltaTime;
            yield return null;
        }

        GameObject hintbar;
        //反击成功
        if (counterFang)
        {
            if (Mathf.Abs(_behavior.targetPlayer.transform.position.x - transform.position.x) > 6)
            {
                WarpEffect();
                WarpAttack_Disappear(false);

                yield return null;

                CounterBlade_WarpMove();
                WarpAttack_Appear();
                WarpEffect();
                ac.TurnMove(_behavior.targetPlayer);
            }


            hintbar=GenerateWarningPrefab("action21_2", transform.position + new Vector3(0, 1.5f, 0),
                Quaternion.identity, MeeleAttackFXLayer.transform);
            yield return new WaitForSeconds(0.5f);
            anim.Play("backflip");
            voice?.PlayMyVoice((int)VoiceGroupEnum.Counter);
            yield return null;
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.15f);

            CounterBlade_Attack(true);

        }
        else
        {
            hintbar=GenerateWarningPrefab("action21_1", transform.position + new Vector3(0, 1.5f, 0),
                Quaternion.identity, MeeleAttackFXLayer.transform);
            yield return new WaitForSeconds(0.9f);
            anim.Play("backflip");
            voice?.PlayMyVoice((int)VoiceGroupEnum.Counter);
            yield return null;
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.15f);

            CounterBlade_Attack(false);
        }
        Destroy(hintbar);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        QuitAttack();

    }

    /// <summary>
    /// 烈焰蓄能 天
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action22()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        ac.OnAttackEnter(999);

        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action22");
        UI_DialogDisplayer.Instance?.
            EnqueueDialogShared(10101,10015,BattleEffectManager.Instance?.notteHintClips[0]);
        
        anim.Play("float_entire");
        GenerateParticles(false);
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        if (!hint2Displayed)
        {
            hint2Displayed = true;
            UI_DialogDisplayer.Instance?.
                EnqueueDialogShared(10101,10011,BattleEffectManager.Instance?.notteHintClips[1]);
        }
        QuitAttack();
    }
    
    /// <summary>
    /// 烈焰蓄能 地
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action23()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        ac.OnAttackEnter(999);

        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action23");
        UI_DialogDisplayer.Instance?.
            EnqueueDialogShared(10101,10012,BattleEffectManager.Instance?.notteHintClips[0]);
        
        anim.Play("float_entire");
        GenerateParticles(true);
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");

        if (!hint1Displayed)
        {
            hint1Displayed = true;
            // UI_DialogDisplayer.Instance?.
            //     EnqueueDialogShared(10101,10013,BattleEffectManager.Instance?.notteHintClips[1]);
            // UI_DialogDisplayer.Instance?.
            //     EnqueueDialogShared(10101,10016,null);
            UI_DialogDisplayer.Instance?.
                EnqueueDialogShared(10101,10014,BattleEffectManager.Instance?.notteHintClips[0]);
        }

        
        
        QuitAttack();
    }
    
    /// <summary>
    /// 核爆（Sky）多的安全
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action24()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        ac.OnAttackEnter(999);

        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action24");
        
        yield return new WaitForSeconds(0.5f);
        
        PurgedShapeShiftingOfViewer();
        OpenParticleForcefield();
        SetWorld_BlackScreenFade();

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("float_entire");
        
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.49f);
        
        if(Projectile_C005_7_Boss.Instance!=null) 
            Destroy(Projectile_C005_7_Boss.Instance.gameObject);
        
        AllRangedAttackBurst(1);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        //ModifyFlameBuffToToAllTargets(0);
        
        Invoke("BlackScreenFadeOut",1f);
        yield return new WaitForSeconds(1.5f);
        SetFlameBuffToAllTargets(0,0);
        
        
        QuitAttack();
    }
    public IEnumerator HB01_Action25()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        ac.OnAttackEnter(999);

        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action25");
        
        yield return new WaitForSeconds(0.5f);
        
        PurgedShapeShiftingOfViewer();
        OpenParticleForcefield();
        SetWorld_BlackScreenFade();

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("float_entire");
        
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.49f);
        
        if(Projectile_C005_7_Boss.Instance.gameObject!=null)
            Destroy(Projectile_C005_7_Boss.Instance.gameObject);
        
        AllRangedAttackBurst(-1);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        
        Invoke("BlackScreenFadeOut",1f);
        yield return new WaitForSeconds(1.5f);
        SetFlameBuffToAllTargets(0,0);
        
        QuitAttack();
    }

    /// <summary>
    /// Inheritor of Blazewolf
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action26(bool towardsFirstArea = true)
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        if (!hint4Displayed)
        {
            hint4Displayed = true;
            UI_DialogDisplayer.Instance?.
                EnqueueDialogShared(10101,10018,BattleEffectManager.Instance?.notteHintClips[0]);
        }

        
        ac.OnAttackEnter(999);
        PurgedShapeShiftingOfViewer();

        ac.TurnMove(_behavior.targetPlayer);
        //bossBanner?.PrintSkillName("HB01_Action25");
        
        yield return new WaitForSeconds(0.5f);
        
        WarpEffect();
        
        yield return new WaitForSeconds(0.2f);
        
        WarpAttack_Disappear(true);
        StageCameraController.SwitchOverallCameraFollowObject(gameObject);
        transform.position = new Vector3(0, -1);
        StageCameraController.SwitchOverallCamera();
        yield return new WaitForSeconds(0.1f);
        
        WarpEffect();
        ac.SetFaceDir(1);
        WarpAttack_Appear();
        ac.SetHitSensor(false);

        yield return new WaitForSeconds(2f);
        
        voice?.BroadCastSpecificVoice((int)VoiceGroupEnum.Inheritor,0);
        
        if(firstPositionOfFlameArea.x >= 0)
            ac.SetFaceDir(1);
        else ac.SetFaceDir(-1);
        
        if(towardsFirstArea == false)
        {
            ac.SetFaceDir(-ac.facedir);
        }
        
        
        
        //设置特写UI
        
        
        var RTScene = GameObject.Find("OtherCamera").transform.Find("RT").gameObject;
        var fullScreenUI = GameObject.Find("FullScreenEffect").transform.Find("RT").gameObject;
        

        var cutsceneController = RTScene.GetComponent<CutSceneController_HB01>();
        //var director = RTScene.GetComponent<PlayableDirector>();
        var Texture = cutsceneController.rt;

        var rawImg = fullScreenUI.GetComponent<RawImage>();
        
        RTScene.SetActive(true);
        
        yield return null;
        cutsceneController.Replay();
        cutsceneController.SetSkyBoxColor((_behavior as HB01_BehaviorTree_Legend2).currentWorld);
        cutsceneController.SetSkyBoxDirection(ac.facedir);
        
        rawImg.texture = Texture;
        yield return null;
        
        
        fullScreenUI.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        
        bossBanner?.PrintSkillName("HB01_Action26");
        
        yield return new WaitForSeconds(3.8f);
        
        //结束特写分镜UI
        
        ac.SwapWeaponVisibility(false);
        weaponLeft.SetActive(true);
        weaponRight.SetActive(true);
        
        //开始扔剑
        
        
        
        anim.Play("throw_enter");
        ac.SetHitSensor(true);
        
        cutsceneController.ResetFaceExpression();
        fullScreenUI.SetActive(false);
        rawImg.texture = null;
        RTScene.SetActive(false);
        InheritorOfBlazewolf_Charging();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("throw_loop"));

        voice?.BroadCastSpecificVoice((int)VoiceGroupEnum.Inheritor,1);
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("throw_exit");
        yield return new WaitForSeconds(0.2f);
        
        InheritorOfBlazewolf_ThrowTwinSword();
        ac.SwapWeaponVisibility(true);
        weaponLeft.SetActive(false);
        weaponRight.SetActive(false);
        

        yield return null;
        
        //StageCameraController.SwitchMainCamera();
        StageCameraController.SwitchOverallCameraFollowObject(_behavior.viewerPlayer);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");

        yield return new WaitForSeconds(1.5f);
        
        //开始进行突刺
        
        
        //WarpAttack_Disappear(true);
        ac.SwapWeaponVisibility(false);
        

        float chaserInterval = 3f;//动画1.5s左右
        float animTime = 1.45f;
        float interval = 0.5f;
        var firstChaser = InheritorOfBlazewolf_GenerateChaseHint();

        //横扫平面
        
        yield return new WaitForSeconds(0.5f);
        
        var listOrder = 
            InheritorOfBlazewolf_SwipeFlameOrderCheck(_behavior.targetPlayer.transform.position);

        InheritorOfBlazewolf_StartSwipingBlades(listOrder,!towardsFirstArea);
        
        yield return new WaitForSeconds(chaserInterval - 0.5f);
        
        
        weaponRight.SetActive(true);
        //WarpEffect();
        InheritorOfBlazewolf_TopDownAttack(firstChaser);
        //yield return null;
        
        yield return new WaitForSeconds(interval);
        
        
        var secondChaser = InheritorOfBlazewolf_GenerateChaseHint();
        
        yield return new WaitForSeconds(animTime);
        
        weaponRight.SetActive(false);
        WarpEffect();
        WarpAttack_Disappear(true);
        
        
        yield return new WaitForSeconds(chaserInterval-animTime);
        
        InheritorOfBlazewolf_TopDownAttack(secondChaser);
        
        yield return new WaitForSeconds(interval);
        
        var thirdChaser = InheritorOfBlazewolf_GenerateChaseHint();
        
        yield return new WaitForSeconds(animTime);
        weaponRight.SetActive(false);
        WarpEffect();
        WarpAttack_Disappear(true);
        
        yield return new WaitForSeconds(chaserInterval-animTime);
        
        
        InheritorOfBlazewolf_TopDownAttack(thirdChaser);
        yield return null;
        
        //结束
        yield return new WaitForSeconds(animTime);
        
        WarpEffect();
        WarpAttack_Disappear(true);
        
        //结束攻击并瞬移到中场。
        transform.position = new Vector3(0, -1);
        WarpEffect();
        ac.SetFaceDir(1);
        WarpAttack_Appear();
        ac.SwapWeaponVisibility(true);
        weaponRight.SetActive(false);
        ac.ResetGravityScale();
        
        anim.Play("idle");

        yield return new WaitForSeconds(2f);
        Invoke("SwitchToMainCamera",10f);



        QuitAttack();

    }
    
    private void WorldBreakEffect()
    {
        var backGroundGO = GameObject.Find("Background");
        
        backGroundGO.transform.GetChild(0).gameObject.SetActive(true);
        backGroundGO.transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.white, 0.5f);
        
        backGroundSpriteRenderer.color = Color.clear;
        _statusManager.OnHPBelow0 -= WorldBreakEffect;
    }























    #region specific moves
    
    private void SwitchToMainCamera()
    {
        StageCameraController.SwitchMainCamera();
    }
    
    protected void ComboDodge1()
    {
        
        GameObject container = Instantiate(attackContainer, transform.position, Quaternion.identity, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        GameObject projectile_clone1 =
            InstantiateMeele(GetProjectileOfFormatName("action01"),
                transform.position,container);
        
        projectile_clone1.GetComponent<AttackBase>().firedir = ac.facedir;
        
    }
    
    protected void WarpAttack_ShadowEffect()
    {
        InstantiateDirectional(GetProjectileOfFormatName("action02_2"), transform.position, RangedAttackFXLayer.transform, ac.facedir, 0,
            1);
        
    }
    
    protected void WarpAttack_Disappear(bool invincible)
    {
        
        ac.SetHitSensor(!invincible);
        DisappearRenderer();
        
    }

    protected void WarpAttack_Appear()
    {
        ac.SetHitSensor(true);
        AppearRenderer();
    }

    protected int LockFaceDir(GameObject target)
    {
        var ac = target.GetComponentInChildren<ActorController>();
        if (ac == null)
        {
            var sac = target.GetComponentInChildren<StandardCharacterController>();
            return sac.facedir;
        }

        return ac.facedir;
    }
    
    protected void BackWarp(Vector3 position, int dir, float distanceX, float distanceY)
    {
        float posX, posY;

        posX = position.x - dir * distanceX;

        posY = position.y + distanceY;

        var pos = BattleStageManager.Instance.OutOfRangeCheck(new Vector2(posX, posY));

        transform.position = pos;
        ac.SetFaceDir(dir);

    }
    
    protected void WarpAttack_Move(Vector3 target)
    {
        _tweener = transform.DOMove(target, 0.3f);
        _tweener.SetEase(Ease.OutExpo);
        //OnComplete(OnTweenComplete);
    }
    
    protected void WarpAttack_Attack()
    {
        GameObject container = Instantiate(attackContainer, transform.position, Quaternion.identity,
            MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        GameObject projectile_clone1 = 
            InstantiateMeele(GetProjectileOfFormatName("action02_1"),
                transform.position, container);
        
    }

    private void Combo1()
    {
        
        var muzzleFx = Instantiate(GetProjectileOfFormatName("action03_1"),
            transform.position,Quaternion.identity, MeeleAttackFXLayer.transform);
        GameObject container = 
            Instantiate(attackContainer,
                transform.position,
                Quaternion.identity, RangedAttackFXLayer.transform);
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action03_2"),
            transform.position,container,ac.facedir);
        
        
        if (ac.facedir == -1)
        {
            proj.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
        }
    }

    private void Combo2_Attack()
    {
        GameObject container = Instantiate(attackContainer,
            transform.position,
            Quaternion.identity,
            MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        GameObject projectile_clone1 = InstantiateMeele
            (GetProjectileOfFormatName("action03_3"), transform.position, container);
        projectile_clone1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        // if (ac.facedir == -1)
        // {
        //     var partical = projectile_clone1.GetComponentInChildren<ParticleSystem>().main;
        //     //ParticleSystem.MinMaxCurve curve;
        //     partical.startRotationY = new ParticleSystem.MinMaxCurve(partical.startRotationY.constant+Mathf.PI);
        // }
    }
    
    protected void Combo2_StraightDashMove()
    {
        var target = _behavior.targetPlayer;
        
        if(Mathf.Abs(target.transform.position.x- transform.position.x) < 2 && Mathf.Abs(target.transform.position.y - transform.position.y) < 1f)
            return;
        
        float tarX;
        if (Mathf.Abs(target.transform.position.y - transform.position.y) < 1f)
        {
            if (target.transform.position.x - transform.position.x < 10 && ac.facedir==1 && target.transform.position.x > transform.position.x)
            {
                tarX = target.transform.position.x - ac.facedir;
            }
            else if(transform.position.x - target.transform.position.x < 10 && ac.facedir==-1 && target.transform.position.x < transform.position.x)
            {
                tarX = target.transform.position.x - ac.facedir;
            }
            else
            {
                tarX = transform.position.x + 10 * ac.facedir;
            }
        }
        else
        {
            tarX = transform.position.x + 10 * ac.facedir;

        }

        tarX = BattleStageManager.Instance.OutOfRangeCheck(new Vector2(tarX,transform.position.y)).x;
        _tweener = transform.DOMoveX(tarX, 0.2f);

    }

    protected void Combo3()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        GameObject projectile_clone1 = InstantiateMeele(GetProjectileOfFormatName("action03_4"), transform.position,container);
        projectile_clone1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
    }

    /// <summary>
    /// 后跳斩+加buff
    /// </summary>
    protected void Combo4_Part1()
    {
        _statusManager.ObtainTimerBuff
            ((int)BasicCalculation.BattleCondition.CritRateBuff, 20, 10);
        _tweener = transform.DOMove(new Vector2(transform.position.x - 2f * ac.facedir, transform.position.y + 4f),
            0.1f).SetEase(Ease.OutCubic);

        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);

        GameObject proj = InstantiateRanged(GetProjectileOfFormatName("action06_1"),
            new Vector3(transform.position.x, transform.position.y + 1), container, ac.facedir);

    }

    protected void Combo4_WarpIn()
    {
        var warpFX =
            Instantiate(GetProjectileOfName("fx_c005_04"), transform.position,
                quaternion.identity, RangedAttackFXLayer.transform);
        DisappearRenderer();
        ac.SetHitSensor(false);

    }
    
    protected void Combo4_WarpOut()
    {
        var initialPosition = transform.position;
        
        if(_behavior.targetPlayer.transform.position.x < transform.position.x && ac.facedir == 1)
            ac.SetFaceDir(-1);
        else if(_behavior.targetPlayer.transform.position.x > transform.position.x && ac.facedir == -1)
            ac.SetFaceDir(1);
        
        var targetPlatform = BasicCalculation.CheckRaycastedPlatform(_behavior.targetPlayer);
        var targetPositionY = targetPlatform.bounds.max.y + 5.5f;
        var targetPositionX = _behavior.targetPlayer.transform.position.x - ac.facedir * 2;

        transform.position = BattleStageManager.Instance.OutOfRangeCheck(
             new Vector2(targetPositionX, targetPositionY));
        
        
        
        
        var warpFX =
            Instantiate(GetProjectileOfName("fx_c005_04"), transform.position,
                quaternion.identity, RangedAttackFXLayer.transform);
        AppearRenderer();
        ac.SetHitSensor(true);
        
    }

    protected void Combo4_Part2()
    {
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        GameObject proj = InstantiateRanged(GetProjectileOfFormatName("action06_2"), 
            new Vector3(transform.position.x,transform.position.y-2.5f),
            container,ac.facedir);
        
    }
    
    protected void Combo5()
    {
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        

        GameObject proj = InstantiateRanged(GetProjectileOfFormatName("action07_1"), 
            new Vector3(transform.position.x+2f*ac.facedir,transform.position.y),
            container,ac.facedir);
        proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        proj.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        
    }
    protected void Combo6()
    {
        _tweener = transform.DOMoveX(transform.position.x + ac.facedir * 8, 0.1f);
        
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        var warpFX =
            Instantiate(GetProjectileOfName("fx_c005_04"), transform.position,
                quaternion.identity, RangedAttackFXLayer.transform);
        

        GameObject proj = InstantiateRanged(GetProjectileOfFormatName("action07_2"), 
            new Vector3(transform.position.x,transform.position.y),
            container,ac.facedir);
        proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        proj.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        
        if (ac.facedir == -1)
        {
            var particles = proj.GetComponentsInChildren<ParticleSystem>();
            var shadow = particles[0].GetComponent<DOTweenSimpleController>();
            shadow.moveDirection.x *= -1;
            
        }
    }

    protected void Combo7_Part1()
    {
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);

        GameObject proj = InstantiateMeele
            (GetProjectileOfFormatName("action05_1"),
                transform.position,container);

    }
    
    protected void Combo7_Part2()
    {
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        GameObject proj = InstantiateRanged
            (GetProjectileOfFormatName("action05_2"),
                transform.position, container,ac.facedir);
        proj.GetComponent<Projectile_C005_5_Boss>().groundAttaching = 
            BasicCalculation.CheckRaycastedPlatform(gameObject).gameObject;

    }




    private GameObject CarmineRush_Attack()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        

        var proj1 = 
            InstantiateRanged(GetProjectileOfFormatName("action04_1"),
                transform.position, container,ac.facedir);
        
        proj1.GetComponent<AttackFromEnemy>().
            AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,1,1),100,0);
        proj1.GetComponent<AttackFromEnemy>().
            AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,1,1),100,1);
        
        
        
        var trail = proj1.GetComponentInChildren<TrailRenderer>().gameObject;
        
        trail.transform.SetParent(MeeleAttackFXLayer.transform);

        return container;

    }
    
    protected Vector3 CarmineRush_Forward()
    {
        //Sequence sequence = DOTween.Sequence();
        //SetGravityScale(0);
        var origin = transform.position;
        var target = transform.position + new Vector3(ac.facedir * 15, 0);
        
        if (target.x > _stageManager.mapBorderR)
            target.x = _stageManager.mapBorderR - 1;
            
        if (target.x < _stageManager.mapBorderL)
            target.x = _stageManager.mapBorderL + 1;

        //print(target);
        
        _tweener = transform.DOMove(target, 0.25f);
        _tweener.SetEase(Ease.OutQuad).OnComplete(DisappearRenderer);
        //print(_tweener.active);

        return origin;
    }
    protected void CarmineRush_Back(Vector3 origin)
    {
        _tweener = transform.DOMove(origin, 0.1f).SetEase(Ease.OutExpo).OnComplete(AppearRenderer);
        _tweener.Play();
        
    }

    protected void FlameRaid()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        var proj1 = 
            Instantiate(GetProjectileOfFormatName("action08"), transform.position, Quaternion.identity, container.transform);
        proj1.GetComponent<AttackFromEnemy>().AddWithConditionAll(new TimerBuff(999),100);
        proj1.GetComponent<AttackFromEnemy>().AddWithConditionAll
        (new TimerBuff((int)BasicCalculation.BattleCondition.Burn,
            72.7f,12f,1),110,1);
        
        proj1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        proj1.GetComponent<AttackFromEnemy>().enemySource = gameObject;
    }

    private GameObject BrightCarmineRush_Attack1()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(5);

        var proj1 = 
            InstantiateRanged(GetProjectileOfFormatName("action04_1"),
                transform.position, container,ac.facedir);
        //var proj1 = 
            //Instantiate(projectile7, transform.position+new Vector3(ac.facedir,0), transform.rotation, container.transform);
        var attack1 = proj1.GetComponent<AttackFromEnemy>();
        attack1.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,1,1),100,0);
        attack1.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,1,1),100,1);
        
        attack1.firedir = ac.facedir;
        attack1.enemySource = gameObject;
        
        var proj2 = 
            Instantiate(GetProjectileOfFormatName("action09_2"),
                transform.position,
                Quaternion.Euler(0,0,20), container.transform);
        var attack2 = proj2.GetComponent<AttackFromEnemy>();
        attack2.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,1,1),100,0);
        attack2.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,1,1),100,1);
        
        if(ac.facedir == -1)
            proj2.transform.localScale = new Vector3(-1,1,1);
        
        attack2.firedir = ac.facedir;
        attack2.enemySource = gameObject;
        
        var proj3 = 
            Instantiate(GetProjectileOfFormatName("action09_2"),
                transform.position+new Vector3(0*Mathf.Cos(0.35f),-0*Mathf.Sin(0.35f)),
                Quaternion.Euler(0,0,-20), container.transform);
        
        if(ac.facedir == -1)
            proj3.transform.localScale = new Vector3(-1,1,1);
        
        var attack3 = proj3.GetComponent<AttackFromEnemy>();
        attack3.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,1,1),100,0);
        attack3.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,1,1),100,1);
        attack3.firedir = ac.facedir;
        attack3.enemySource = gameObject;
        
        var controllerDown = attack2.GetComponentInChildren<DOTweenSimpleController>();
        var controllerUp = attack3.GetComponentInChildren<DOTweenSimpleController>();
        
        
        
        return container;
    }

    private void BrightCarmineRush_Attack2(GameObject container,Vector3 euler)
    {
        Quaternion newRot;
        if(ac.facedir == 1)
        {
            newRot = Quaternion.Euler(0,0,0);
        }
        else
        {
            newRot = Quaternion.Euler(0,180,0);
        }

        GameObject projfx1 = Instantiate(GetProjectileOfFormatName("action09_1"),
            transform.position+new Vector3(ac.facedir-ac.facedir,0,0),
            Quaternion.Euler(euler), container.transform);
    
        int modifierY = 1;
        int modifierX = 1;
        
        if (euler.z < 270 && euler.z > 90)
        {
            projfx1.transform.localScale = new Vector3(-1,1,1);
            var newEuler = euler + new Vector3(0,0,180);
            projfx1.transform.eulerAngles = newEuler;
            modifierX = -1;
        }
        else
        {
            modifierX = -1;
        }
        print("Euler"+euler.z);


        var moveController = projfx1.GetComponentInChildren<DOTweenSimpleController>();
        var angle = Vector2.SignedAngle(Vector2.right, euler) * Mathf.Deg2Rad;
        angle = angle* Mathf.Deg2Rad;
        
        //print("angle"+angle);
        
        moveController.moveDirection.x = 
            30f * Mathf.Cos(angle * Mathf.Deg2Rad) * (-modifierX);
        moveController.moveDirection.y =
            30f * Mathf.Sin(angle * Mathf.Deg2Rad)* (-modifierX);
        
        


        //moveController.duration *= 1.2f;

        //attack2.enemySource = gameObject;
        projfx1.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        var attack2 = projfx1.GetComponent<AttackFromEnemy>();
        attack2.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
            -1,30,1,1),100,0);
        attack2.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
            15,30,1,1),100,1);
    }
    
    protected void BrightCarmineRush_Attack3(GameObject container)
    {

        //狼头+伤害，第二次的攻击。
        var proj1 = 
            InstantiateRanged(GetProjectileOfFormatName("action04_1"),
                transform.position, container,ac.facedir);
        
        proj1.GetComponentInChildren<TrailRenderer>().gameObject.SetActive(false);
        
        
        //var proj1 = 
        //Instantiate(projectile7, transform.position+new Vector3(ac.facedir,0), transform.rotation, container.transform);
        proj1.GetComponent<AttackFromEnemy>().
            AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,1,1),100,0);
        proj1.GetComponent<AttackFromEnemy>().
            AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,1,1),100,1);
    }
    
    protected void SavageFlameRaid()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        var proj1 = 
            InstantiateMeele(GetProjectileOfFormatName("action10"),
                transform.position+Vector3.up, container);
        
        proj1.GetComponent<AttackFromEnemy>().AddWithConditionAll(new TimerBuff(999),110);
        proj1.GetComponent<AttackFromEnemy>().AddWithConditionAll
        (new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
            31.1f,21f,1),100,1);
        
    }



    private void ScarletInferno()
    {
        GameObject container = Instantiate(GetProjectileOfFormatName("action11"), transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        var allAttack = container.GetComponentsInChildren<AttackFromEnemy>();
        foreach (var child in allAttack)
        {
            child.enemySource = gameObject;
        }
        container.GetComponent<RelativePositionRetainer>().SetParent(transform);
    }

    private void ScarletInferno_Aggerate()
    {
        var container = Projectile_C005_3_Boss.Instance;
        container.SetRotateRadius(15,3);
        container.SetScale(1.5f,3);
    }
    
    private void ScarletInferno_Absorb()
    {
        var container = Projectile_C005_3_Boss.Instance;
        container.SetRotateRadius(1,1.5f);
        container.SetScale(1,1);
        
    }
    
    private void ScarletInferno_Buff()
    {
        Destroy(Projectile_C005_3_Boss.Instance.gameObject);
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            5,15);
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            5,15);
        var scorPunisher = new TimerBuff((int)BasicCalculation.BattleCondition.ScorchrendPunisher,
            10, -1, 100, 8101404);
        _statusManager.ObtainTimerBuff(scorPunisher,false);

    }

    private void SetWorld_BlackScreenFade()
    {
        backGroundSpriteRenderer.DOColor(Color.black, 0.5f);
        //Invoke("BlackScreenFadeOut", 2.5f);;
        // BattleStageManager.Instance.RemoveFieldAbility(20091);
        // BattleStageManager.Instance.RemoveFieldAbility(20081);
    }
    
    
    private void BlackScreenFadeOut()
    {
        var bossBehavior = _behavior as HB01_BehaviorTree_Legend2;
        var currentWorld = bossBehavior.currentWorld;
        if(currentWorld == 0)
        {
            _tweener = backGroundSpriteRenderer.DOColor(bossBehavior.originColor, 0.5f).
                OnKill(()=>backGroundSpriteRenderer.color = bossBehavior.originColor);
        }
        else if (currentWorld == 1)
        {
            _tweener = backGroundSpriteRenderer.DOColor(bossBehavior.brightColor, 0.5f).
                OnKill(()=>backGroundSpriteRenderer.color = bossBehavior.brightColor);
        }else if (currentWorld == 2)
        {
            _tweener = backGroundSpriteRenderer.DOColor(bossBehavior.catastrophicColor, 0.5f)
                .OnKill(()=>backGroundSpriteRenderer.color = bossBehavior.catastrophicColor);
        }

        // BattleStageManager.Instance.RemoveFieldAbility(20091);
        // BattleStageManager.Instance.RemoveFieldAbility(20081);
    }

    private void SetWorld(int worldID)
    {
        switch (worldID)
        {
            case 1:
            {
                _tweener = backGroundSpriteRenderer.DOColor(
                    (_behavior as HB01_BehaviorTree_Legend2).brightColor,
                    0.5f).OnKill(() =>
                {
                    backGroundSpriteRenderer.color = (_behavior as HB01_BehaviorTree_Legend2).brightColor;
                });
                BattleStageManager.Instance.RemoveFieldAbility(20091);
                BattleStageManager.Instance.AddFieldAbility(20081);
                break;
            }
            case 2:
            {
                _tweener = backGroundSpriteRenderer.DOColor(
                    (_behavior as HB01_BehaviorTree_Legend2).catastrophicColor,
                    0.5f).
                    OnKill(()=>backGroundSpriteRenderer.color = (_behavior as HB01_BehaviorTree_Legend2).catastrophicColor);;
                BattleStageManager.Instance.RemoveFieldAbility(20081);
                BattleStageManager.Instance.AddFieldAbility(20091);
                break;
            }
            default:
            {
                _tweener = backGroundSpriteRenderer.DOColor(
                    (_behavior as HB01_BehaviorTree_Legend2).originColor,
                    0.5f).
                    OnKill(() => backGroundSpriteRenderer.color = (_behavior as HB01_BehaviorTree_Legend2).originColor);
                BattleStageManager.Instance.RemoveFieldAbility(20091);
                BattleStageManager.Instance.RemoveFieldAbility(20081);
                BattleStageManager.Instance.AddFieldAbility(-1);
                BattleStageManager.Instance.RemoveFieldAbility(-1);
                break;
            }

        }
        
        
        
    }

    private void BlazingBlitzAndLeaveAShadow()
    {
        var endPos = transform.position + new Vector3(ac.facedir * 7.5f, 0);
        endPos = BattleStageManager.Instance.OutOfRangeCheck(endPos);
        
        var container = Instantiate(attackContainer, transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        var proj1 = 
            InstantiateRanged(GetProjectileOfFormatName("action20_2"),
                transform.position, container,ac.facedir);
        
        var projFX = Instantiate(GetProjectileOfFormatName("action20_1"),
            transform.position,Quaternion.identity, MeeleAttackFXLayer.transform);
        
        
        _tweener = transform.DOMove(endPos, 0.1f).SetEase(Ease.OutSine);
        
        var shadow = Instantiate(GetProjectileOfFormatName("action20_3"),
            transform.position+new Vector3(0,0.1f),
            Quaternion.identity, RangedAttackFXLayer.transform);

        var shadowController =
            shadow.GetComponent<Projectile_C005_6_Boss>();
        shadowController.enemySource = gameObject;
        shadowController.s1FX = GetProjectileOfFormatName("action04_1");
        shadowController.s2FX = GetProjectileOfFormatName("action08");
        if(ac.facedir == -1)
            shadow.transform.localScale = new Vector3(-1,1,1);

    }

    private void ActiveShadowSkill1(int sid)
    {
        if (Projectile_C005_6_Boss.Instance != null)
        {
            if(Projectile_C005_6_Boss.Instance.attackStart)
                return;
            Projectile_C005_6_Boss.Instance.ReleaseAttack(sid,_behavior.targetPlayer);
        }
    }

    private void WarpEffect()
    {
        var warpFX =
            Instantiate(GetProjectileOfName("fx_c005_04"), transform.position,
                quaternion.identity, RangedAttackFXLayer.transform);
    }

    private void CounterBlade_WarpMove()
    {
        if (transform.position.x < _behavior.targetPlayer.transform.position.x)
        {
            transform.position = BattleStageManager.Instance.OutOfRangeCheck(
                _behavior.targetPlayer.transform.position
                - new Vector3(4, 0));
        }
        else
        {
            transform.position = BattleStageManager.Instance.OutOfRangeCheck(
                _behavior.targetPlayer.transform.position
                + new Vector3(4, 0));
        }
    }

    private void CounterBlade_Attack(bool success)
    {
        var container = Instantiate(attackContainer, transform.position,
            Quaternion.identity, MeeleAttackFXLayer.transform);
        
        var proj = 
            InstantiateMeele(GetProjectileOfFormatName("action21"),
                transform.position+new Vector3(0,1.5f), container);

        if (success)
        {
            proj.GetComponent<AttackFromEnemy>().
                ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
        }


    }
    private void CounterFangActive(StatusManager self)
    {
        counterFang = true;
    }

    private void GenerateParticles(bool upward)
    {
        var particleInstance = Projectile_C005_7_Boss.Instance;
        if (particleInstance != null)
        {
            Destroy(particleInstance.gameObject);
        }

        if (upward)
        {
            Instantiate(GetProjectileOfFormatName("action23"),
                RangedAttackFXLayer.transform);
        }
        else
        {
            Instantiate(GetProjectileOfFormatName("action22"),
                RangedAttackFXLayer.transform);
        }

    }

    private void OpenParticleForcefield()
    {
        var particleForceField = transform.GetComponentInChildren<ParticleSystemForceField>();
        var particleInstance = Projectile_C005_7_Boss.Instance;
        if(particleInstance == null)
            return;
        particleInstance.AddInflunce(particleForceField);
    }

    private void AllRangedAttackBurst(int rule)
    {
        var go = Instantiate(GetProjectileOfFormatName("action24_1"),
            RangedAttackFXLayer.transform);
        
        var fx = Instantiate(GetProjectileOfFormatName("action24_2"),
            transform.position, Quaternion.identity, MeeleAttackFXLayer.transform);

        var controller = go.GetComponent<Projectile_C005_8_Boss>();
        controller.bossObject = gameObject;
        controller.Rule = rule;

    }

    private void InheritorOfBlazewolf_Charging()
    {
        var fx = Instantiate(GetProjectileOfFormatName("action26_3"),
            transform.position, Quaternion.identity, MeeleAttackFXLayer.transform);
    }

    private void InheritorOfBlazewolf_ThrowTwinSword()
    {
        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.StandardAttackBurner),
            72.7f,45);
        
        var container = Instantiate(attackContainer, transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        var sword1 = 
            InstantiateRanged(GetProjectileOfFormatName("action26_1"),
                transform.position+new Vector3(0.5f*ac.facedir,0),
                container,ac.facedir);
        
        var sword2 =
            InstantiateRanged(GetProjectileOfFormatName("action26_2"),
                transform.position+new Vector3(0.5f*ac.facedir,0),
                container,ac.facedir);
        
        if(ac.facedir == -1)
        {
            sword1.transform.localScale = new Vector3(-1,1,1);
            sword2.transform.localScale = new Vector3(-1,1,1);
            sword1.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
            sword2.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
        }

    }

    private GameObject InheritorOfBlazewolf_GenerateChaseHint()
    {
        var chaserHint = GenerateWarningPrefab("action26_6",
            _behavior.targetPlayer.transform.position + new Vector3(0,6,0),
            Quaternion.identity, RangedAttackFXLayer.transform);

        var chaserController = chaserHint.GetComponent<EnemyAttackHintBarTopDownChaser>();
        
        chaserController.target = _behavior.targetPlayer;

        return chaserHint;
    }

    private void InheritorOfBlazewolf_TopDownAttack(GameObject chaserHint)
    {
        WarpEffect();
        weaponRight.SetActive(true);
        ac.SwapWeaponVisibility(false);
        anim.Play("dive_out",-1,0.1f);
        transform.position = new Vector3(chaserHint.transform.position.x,chaserHint.transform.position.y+8f);
        WarpAttack_Appear();
        WarpEffect();
        //BurningEffect();
        ac.SetGravityScale(1);
        var currentPlatform = BasicCalculation.GetRaycastedPlatformY(chaserHint.transform.position-new Vector3(0,12,0));
        _tweener = transform.DOMoveY(currentPlatform+1.5f, 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            var sword = Instantiate(GetProjectileOfFormatName("action26_7"),
                new Vector3(transform.position.x, currentPlatform+0.5f), Quaternion.identity,
                RangedAttackFXLayer.transform);

            sword.GetComponent<Projectile_C005_9_Boss>().SetEnemySource(gameObject);
            ac.ResetGravityScale();
            sword.GetComponent<Projectile_C005_9_Boss>().ReleaseAttack();
            weaponRight.SetActive(false);
        });
        
    }

    private List<float> InheritorOfBlazewolf_SwipeFlameOrderCheck(Vector2 targetPosition)
    {
        bossBanner?.PrintSkillName("HB01_Action27");
        List<float> platformBottoms = new List<float>();
        var platforms = AStar.GetAllNodes();
        foreach (var platformNode in platforms)
        {
            platformBottoms.Add(platformNode.platform.height);
        }
        //降序排列
        platformBottoms.Sort((a,b)=>b.CompareTo(a));
        print("第二个平台高度"+platformBottoms[1]);
        
        //判断targetPosition介于哪两个平台之间
        var targetPlatform = platformBottoms.FindIndex(x => x < targetPosition.y);
        print("目标平台"+targetPlatform);

        int direction;
        //如果目标平台是第一个或是第二个平台,让direction为-1
        if (targetPlatform < 2)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

        

        if (direction == 1)
        {
            return new List<float>()
            {
                platformBottoms[0],
                platformBottoms[1],
                platformBottoms[2]
            };
        }
        else
        {
            return new List<float>()
            {
                platformBottoms[3],
                platformBottoms[2],
                platformBottoms[1]
            };
        }




    }

    private void InheritorOfBlazewolf_StartSwipingBlades(List<float> heights,bool reversed = false)
    {
        GameObject container;
        if (!reversed)
        {
            if (firstPositionOfFlameArea.x >= 0)
            {
                container = Instantiate(GetProjectileOfFormatName("action26_8"),
                    new Vector3(-19,0), Quaternion.identity, RangedAttackFXLayer.transform);
            }
            else
            {
                container = Instantiate(GetProjectileOfFormatName("action26_8"),
                    new Vector3(19,0), Quaternion.identity, RangedAttackFXLayer.transform);
                container.transform.localScale = new Vector3(-1,1,1);
            }
        }
        else
        {
            if (firstPositionOfFlameArea.x <= 0)
            {
                container = Instantiate(GetProjectileOfFormatName("action26_8"),
                    new Vector3(-19,0), Quaternion.identity, RangedAttackFXLayer.transform);
            }
            else
            {
                container = Instantiate(GetProjectileOfFormatName("action26_8"),
                    new Vector3(19,0), Quaternion.identity, RangedAttackFXLayer.transform);
                container.transform.localScale = new Vector3(-1,1,1);
            }
        }



        var controller = container.GetComponent<Projectile_C005_10_Boss>();
        controller.SetEnemySource(gameObject);
        controller.InitOrderList(heights);
    }







    private void GenerateFlameArea(Vector2 position)
    {
        var area = Instantiate(GetProjectileOfFormatName("action15"),
            position, Quaternion.identity, RangedAttackFXLayer.transform);
        area.GetComponent<Projectile_C005_4_Boss>().SetHintsTargets(_statusManager,
            _behavior.viewerPlayer.GetComponent<StatusManager>());
    }

    private void BurningEffect()
    {
        Instantiate(GetProjectileOfFormatName("action16_1"),
            transform.position + Vector3.down, Quaternion.identity, MeeleAttackFXLayer.transform);
    }
    
    
    
    
    

    /// <summary>
    /// 全部目标重置buff和buff等级
    /// </summary>
    /// <param name="levelSelf"></param>
    /// <param name="levelOthers"></param>
    private void SetFlameBuffToAllTargets(int levelSelf, int levelOthers)
    {
        Instantiate(GetProjectileOfFormatName("action16_1"),
            transform.position + Vector3.down, Quaternion.identity, MeeleAttackFXLayer.transform);
        
        
        
        _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.ScorchingEnergy,levelSelf>0?false:true,
        8101401);

        if (levelSelf > 0)
        {
            _statusManager.ObtainUnstackableTimerBuff((int)BasicCalculation.BattleCondition.ScorchingEnergy,
                        levelSelf,-1,8101401);
            
        }
        _statusManager.GetComponentInChildren<UI_HB001_Legend_01>().BuffWithFlameLevelCheck(levelSelf-1);
        
        var targets = DragaliaEnemyBehavior.GetPlayerListDeadAlive();
        foreach (var target in targets)
        {
            Instantiate(GetProjectileOfFormatName("action16_1"),
                target.transform.position+ Vector3.down, Quaternion.identity, target.transform.Find("MeeleAttackFX"));
            
            target.GetComponent<StatusManager>().
                RemoveTimerBuff((int)BasicCalculation.BattleCondition.ScorchingEnergy,levelOthers>0?false:true,
                8101401);
            if (levelOthers > 0)
            {
                target.GetComponent<StatusManager>().
                                ObtainUnstackableTimerBuff((int)BasicCalculation.BattleCondition.ScorchingEnergy,
                                levelOthers,-1,8101401);
            }

            target.GetComponentInChildren<UI_HB001_Legend_01>().BuffWithFlameLevelCheck(levelOthers-1);
        }
        
    }
    
    private void ModifyFlameBuffToToAllTargets(int modifyLevel)
    {
        
        var targets = BattleStageManager.GetAllStatusManagers();
        foreach (var target in targets)
        {
            target.RemoveTimerBuff((int)BasicCalculation.BattleCondition.ScorchingEnergy);
            
            var currentLvl = 
                target.GetConditionTotalValue((int)BasicCalculation.BattleCondition.ScorchingEnergy);
            if(currentLvl + modifyLevel > 3)
                modifyLevel = 3;
            else if (currentLvl + modifyLevel <= 0)
            {
                return;
            }else{
                modifyLevel = (int)(modifyLevel + currentLvl);
            }
            
            target.ObtainTimerBuff((int)BasicCalculation.BattleCondition.ScorchingEnergy,
                modifyLevel,-1,1,8101401);
            
            target.GetComponentInChildren<UI_HB001_Legend_01>().BuffWithFlameLevelCheck(modifyLevel-1);
        }
    }
    
    

    #endregion
}
