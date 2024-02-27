using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMoveController_HB01_Legend : EnemyMoveController_HB01
{
    VoiceControllerEnemy voiceGroup;
    
    

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
        ChangePhase = 8
    }
    
    
    
    
    protected override void Start()
    {
        voiceGroup = GetComponentInChildren<VoiceControllerEnemy>();
        _stageManager = FindObjectOfType<BattleStageManager>();
        MeeleAttackFXLayer = transform.Find("MeeleAttackFX").gameObject;
        RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");
        ac = GetComponent<EnemyControllerHumanoid>();
        anim = GetComponentInChildren<Animator>();
        _behavior = GetComponent<DragaliaEnemyBehavior>();
        _effectManager = GameObject.Find("StageManager").GetComponent<BattleEffectManager>();
        _statusManager = GetComponent<StatusManager>();
        GetAllAnchors();
        attackContainer = BattleStageManager.Instance.attackContainerEnemy;
    }

    public override IEnumerator HB01_Action01()
    {
        QuitMove();
        
        yield return new WaitUntil(() => !ac.hurt);
        
        ac.OnAttackEnter();
        ac.SetCounter(true);
        anim.Play("comboDodge0");
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.13f);
        
        ComboDodge1();
        //yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
    }

    public override IEnumerator HB01_Action02()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        
        ac.OnAttackEnter(200);

        yield return new WaitForSeconds(0.5f);
        anim.Play("warp");
        DisappearRenderer();
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,1.2f);
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.WarpAttack);
        WarpAttack_ShadowEffect();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.33f);
        WarpAttack_Disappear(true);
        
        
        yield return new WaitForSeconds(0.22f);
        var pos = LockPosition(_behavior.targetPlayer);
        var dir = LockFaceDir(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(.08f);
        WarpAttack_Appear();
        anim.Play("comboDodge1");
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        SetGravityScale(0);
        BackWarp(pos,dir,3f,4.5f);
       
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.17f);
        ComboDodge2_WarpAttack1(pos);
        var controllerHumanoid = (EnemyControllerHumanoid)ac;
        SetGravityScale(controllerHumanoid._defaultgravityscale);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.236f);
        ComboDodge2_WarpAttack2();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("idle");
        QuitAttack();
        
        
    }

    public override IEnumerator HB01_Action03()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetCounter(true);
        
        
        var hint = GenerateWarningPrefab(WarningPrefabs[0], transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(hintbar.warningTime);
        Destroy(hint,0.5f); //4/3添加
        anim.Play("combo1");
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.13f);
        Combo1();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);
        anim.Play("combo2");
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.05f);
        Combo2();
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f);
        if (!anim.GetBool("isGround"))
        {
            anim.Play("fall");
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        }

        yield return new WaitUntil(() => anim.GetBool("isGround"));
        anim.Play("combo3");
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.25f);
        Combo3();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
    }

    public override IEnumerator HB01_Action04()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action04");
        
        
        var hint = GenerateWarningPrefab(WarningPrefabs[1], transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(hintbar.warningTime);
        Destroy(hint);
        anim.Play("s1_rush");
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.CarmineRush);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);
        SetGravityScale(0);
        var container = CamineRush_Part1();
        
        yield return new WaitForSeconds(0.05f);
        Vector2 origin = RushForward();
        CamineRush_Part2(container);
        
        
        yield return new WaitForSeconds(0.2f);
        RushBack(origin);
        
        yield return new WaitForSeconds(0.1f);
        ac.SetFaceDir(-ac.facedir);
        SetGravityScale(4);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        anim.Play("s1_recover");
        
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
    }

    public IEnumerator HB01_Action04_WithInferno()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action04");

        yield return new WaitForSeconds(1f);
        
        
        var hint = GenerateWarningPrefab(WarningPrefabs[1], transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(hintbar.warningTime * 0.7f);

        SetRotateRaduisOfFireballs(18);
        AccelerateFireballs(52.5f,12);
        ExpandFireballs();
        
        yield return new WaitForSeconds(hintbar.warningTime * 0.3f);
        
        
        
        Destroy(hint);
        anim.Play("s1_rush");
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.CarmineRush);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);
        SetGravityScale(0);
        var container = CamineRush_Part1();
        
        yield return new WaitForSeconds(0.05f);
        Vector2 origin = RushForward();
        CamineRush_Part2(container);
        
        
        yield return new WaitForSeconds(0.2f);
        RushBack(origin);
        
        yield return new WaitForSeconds(0.1f);
        ac.SetFaceDir(-ac.facedir);
        SetGravityScale(4);
        
        SetRotateRaduisOfFireballs(10);
        ShrinkFireballs();
        DecelerateFireballs();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        anim.Play("s1_recover");
        
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
    }

    public override IEnumerator HB01_Action05()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();ac.SetCounter(true);
        ac.TurnMove(_behavior.targetPlayer);
        GenerateWarningPrefab(WarningPrefabs[7],transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject, 
            new Vector3(transform.position.x,transform.position.y+4));

        yield return new WaitForSeconds(0.8f);
        anim.Play("combo7");
        _statusManager.ObtainUnstackableTimerBuff((int)BasicCalculation.BattleCondition.BlazewolfsRush,
            -1,-1,1);
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= (5f/29f));
        Combo7_Part1();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= (17f/29f));
        Combo7_Part1();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("comboDodge1");
        transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f);
        SetGravityScale(0);
        
        yield return new WaitForSeconds(0.2f);
        Combo7_DashDownward();
        
        yield return new WaitForSeconds(0.05f);
        Combo7_Part2();
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        ac.ResetGravityScale();
        
        QuitAttack();
    }

    public override IEnumerator HB01_Action06()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();ac.SetCounter(true);
        ac.TurnMove(_behavior.targetPlayer);
        GenerateWarningPrefab(WarningPrefabs[7],transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        
        _effectManager.SpawnExclamation(gameObject, 
            new Vector3(transform.position.x,transform.position.y+4));

        yield return new WaitForSeconds(0.8f);
        anim.Play("combo4");
        SetGravityScale(0);
        SetGroundCollider(false);
        Combo4_Part1();
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.69f);
        anim.Play("comboDodge1");
        transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f);
        
        yield return new WaitForSeconds(0.2f);
        Combo4_Landing();
        
        yield return new WaitForSeconds(0.05f);
        Combo4_Part2();
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        ac.ResetGravityScale();
        SetGroundCollider(true);
        QuitAttack();
    }

    public override IEnumerator HB01_Action07()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();ac.SetCounter(true);
        ac.TurnMove(_behavior.targetPlayer);
        
        
        var hint = GenerateWarningPrefab(WarningPrefabs[2], transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(hintbar.warningTime);
        Destroy(hint);
        anim.Play("combo5");
        SetGravityScale(0);
        Combo5();
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.StandardAttack);
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f);
        anim.Play("combo6");
        anim.speed = 0;

        yield return new WaitForSeconds(0.15f);
        anim.speed = 1;

        yield return new WaitForSeconds(0.05f);
        var oldPos = transform.position.x;
        Combo6();
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);
        SetGravityScale(4);
        transform.position = new Vector3(oldPos, transform.position.y);
        QuitAttack();
    }

    public override IEnumerator HB01_Action08()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action08");

        yield return new WaitForSeconds(1f);
        var hint = GenerateWarningPrefab(WarningPrefabs[3], transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        
        yield return new WaitForSeconds(hintbar.warningTime);
        anim.Play("s2");
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.FlameRaid);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15f);
        Destroy(hint,0.3f);
        FlameRaid();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
    }
    
    public IEnumerator HB01_Action08_WithInferno()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action08");

        yield return new WaitForSeconds(2f);
        var hint = GenerateWarningPrefab(WarningPrefabs[3], transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        
        yield return new WaitForSeconds(hintbar.warningTime);
        anim.Play("s2");
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.FlameRaid);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        ExpandFireballs();
        SetRotateRaduisOfFireballs(6f);
        AccelerateFireballs(45);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15f);
        Destroy(hint,0.3f);
        FlameRaid();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        
        ShrinkFireballs();
        SetRotateRaduisOfFireballs(10f);
        DecelerateFireballs();
        QuitAttack();
    }

    public override IEnumerator HB01_Action09()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action09");
        
        var hint = 
            GenerateWarningPrefab
                (WarningPrefabs[1], transform.position,Quaternion.Euler(0,ac.facedir==1?0:180,0), RangedAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        var hint2 = 
            GenerateWarningPrefab(WarningPrefabs[1], transform.position,
                (Quaternion.Euler(0,ac.facedir==1?0:180,20)), RangedAttackFXLayer.transform);
        var hint3 = 
            GenerateWarningPrefab(WarningPrefabs[1], transform.position,
                (Quaternion.Euler(0,ac.facedir==1?0:180,-20)), RangedAttackFXLayer.transform);
        var hint4 = GenerateWarningPrefab(WarningPrefabs[4], transform.position,transform.rotation, RangedAttackFXLayer.transform);
        hint4.GetComponent<EnemyAttackHintBarChaser>().target = _behavior.targetPlayer;
        hint4.transform.rotation = (ac.facedir == 1 ? transform.rotation : Quaternion.Euler(0, 0, 180));
        _effectManager.SpawnTargetLockIndicator(_behavior.targetPlayer,2f);

        yield return new WaitForSeconds(hintbar.warningTime);
        
        anim.Play("s1_rush");
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.BrightCarmineRush);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);
        SetGravityScale(0);
        var container = BrightCamineRush_Part1();
        
        yield return new WaitForSeconds(0.05f);
        Vector2 origin = RushForward();
        CamineRush_Part2(container);
        
        
        yield return new WaitForSeconds(0.2f);
        RushBack(origin);
        
        yield return new WaitForSeconds(0.1f);
        ac.SetFaceDir(-ac.facedir);
        SetGravityScale(4);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        //anim.Play("s1_recover");
        ac.SetFaceDir(-ac.facedir);
        anim.Play("s1_rush",-1,0.2f);
        BrightCamineRush_Part2(container,hint4.transform.rotation);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);
        SetGravityScale(0);
        BrightCamineRush_Part3(container);
        Destroy(hint);Destroy(hint2);Destroy(hint3);
        

        yield return new WaitForSeconds(0.05f);
        RushForward();
        CamineRush_Part2(container);
        
        yield return new WaitForSeconds(0.2f);
        RushBack(origin);
        Destroy(hint4);
        
        yield return new WaitForSeconds(0.1f);
        ac.SetFaceDir(-ac.facedir);
        SetGravityScale(4);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("s1_recover");
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
    }

    
    /// <summary>
    /// 蓄力时间比较长的闪耀焰红突袭
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action09_WithInferno()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action09");
        
        var hint = 
            GenerateWarningPrefab
                (WarningPrefabs[10], transform.position,Quaternion.Euler(0,ac.facedir==1?0:180,0), RangedAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        var hint2 = 
            GenerateWarningPrefab(WarningPrefabs[10], transform.position,
                (Quaternion.Euler(0,ac.facedir==1?0:180,20)), RangedAttackFXLayer.transform);
        var hint3 = 
            GenerateWarningPrefab(WarningPrefabs[10], transform.position,
                (Quaternion.Euler(0,ac.facedir==1?0:180,-20)), RangedAttackFXLayer.transform);
        var hint4 = GenerateWarningPrefab(WarningPrefabs[11], transform.position,transform.rotation, RangedAttackFXLayer.transform);
        hint4.GetComponent<EnemyAttackHintBarChaser>().target = _behavior.targetPlayer;
        hint4.transform.rotation = (ac.facedir == 1 ? transform.rotation : Quaternion.Euler(0, 0, 180));
        _effectManager.SpawnTargetLockIndicator(_behavior.targetPlayer,2f);

        yield return new WaitForSeconds(hintbar.warningTime*0.7f);
        
        SetRotateRaduisOfFireballs(18);
        AccelerateFireballs(50,10);
        ExpandFireballs();
        
        
        yield return new WaitForSeconds(hintbar.warningTime*0.3f);
        
        anim.Play("s1_rush");
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.BrightCarmineRush);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);
        SetGravityScale(0);
        var container = BrightCamineRush_Part1_Legend();
        
        yield return new WaitForSeconds(0.05f);
        Vector2 origin = RushForward();
        CarmineRush_Part2_Legend(container);
        
        
        yield return new WaitForSeconds(0.2f);
        RushBack(origin);
        
        yield return new WaitForSeconds(0.1f);
        ac.SetFaceDir(-ac.facedir);
        ac.ResetGravityScale();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        //anim.Play("s1_recover");
        ac.SetFaceDir(-ac.facedir);
        anim.Play("s1_rush",-1,0.2f);
        BrightCamineRush_Part2(container,hint4.transform.rotation);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);
        SetGravityScale(0);
        BrightCamineRush_Part3(container);
        Destroy(hint);Destroy(hint2);Destroy(hint3);
        

        yield return new WaitForSeconds(0.05f);
        RushForward();
        CamineRush_Part2(container);
        
        yield return new WaitForSeconds(0.2f);
        RushBack(origin);
        Destroy(hint4);
        
        yield return new WaitForSeconds(0.1f);
        ac.SetFaceDir(-ac.facedir);
        SetGravityScale(4);
        
        
        SetRotateRaduisOfFireballs(10);
        ShrinkFireballs();
        DecelerateFireballs();
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("s1_recover");
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
    }

    public override IEnumerator HB01_Action10()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action10");

        yield return new WaitForSeconds(.5f);
        var hint = GenerateWarningPrefab(WarningPrefabs[5], transform.position+Vector3.up,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        
        yield return new WaitForSeconds(hintbar.warningTime);
        anim.Play("s2");
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.SavageFlameRaid);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15f);
        Destroy(hint,0.3f);
        SavageFlameRaid();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
    }

    public IEnumerator HB01_Action10_WithInferno()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action10");

        yield return new WaitForSeconds(1f);
        var hint = GenerateWarningPrefab(WarningPrefabs[13], transform.position+Vector3.up,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        
        yield return new WaitForSeconds(hintbar.warningTime);
        anim.Play("s2");
        voiceGroup.PlayMyVoice((int)VoiceGroupEnum.SavageFlameRaid);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        ExpandFireballs();
        SetRotateRaduisOfFireballs(8f);
        AccelerateFireballs(45);
        
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15f);
        Destroy(hint,0.3f);
        SavageFlameRaid();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);

        yield return new WaitForSeconds(1f);
        ShrinkFireballs();
        SetRotateRaduisOfFireballs(10f);
        DecelerateFireballs();
        QuitAttack();
    }

    public override IEnumerator HB01_Action11()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action11");
        StageCameraController.SwitchOverallCameraFollowObject(GetAnchoredSensorOfName("GroundM"));
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(.5f);
        var hint = GenerateWarningPrefab(WarningPrefabs[6], transform.position+Vector3.up,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        
        yield return new WaitForSeconds(hintbar.warningTime);
        anim.Play("buff");
        float animTime;
        
        
        voiceGroup.BroadCastMyVoice((int)VoiceGroupEnum.ScarletInferno);
        
        Destroy(hint,0.5f);
        ScarletInferno();
        StageCameraController.SwitchMainCamera();
        yield return null;
        _behavior.controllAfflictionProtect = false;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        QuitAttack();
    }

    /// <summary>
    /// Crimson Heaven
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action12()
    {
        StageCameraController.SwitchOverallCameraFollowObject(GetAnchoredSensorOfName("FloatM1"));
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        
        bossBanner?.PrintSkillName("HB01_Action12");
        GenerateWarningPrefab(WarningPrefabs[14], new Vector3(-20, -10, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(3f);
        
        anim.Play("combo1");

        yield return null;
        
        ExpandFireballs(1.5f);
        AccelerateFireballs(0.1f, 2f);

        //yield return new WaitForSeconds(0.5f);
        
        
        SprialMovementOfFireballs();
        _behavior.controllAfflictionProtect = false;
        

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        yield return new WaitForSeconds(1f);
        
        
        StageCameraController.SwitchMainCamera();
        StageCameraController.SwitchOverallCameraFollowObject(null);
        
        QuitAttack();
    }

    public override IEnumerator HB01_Action13()
    {
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action13");
        //StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(1f);
        anim.Play("buff");
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        //voice.PlayMyVoice(VoiceController_HB01.myMoveList.Roll,true);
        voiceGroup.BroadCastMyVoice((int)VoiceGroupEnum.BlazingEnhancement);
        BlazingEnhancementBuffLegend();
        
        
        
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
    }

    public IEnumerator HB01_Action14()
    {
        (_behavior as HB01_BehaviorTree_Legend).transforming = false;
        //瞬移并播放第一句语音
        var scarletInferno = Projectile_C005_3_Boss.Instance;
        

        Warp_Effect();
        ac.rigid.velocity = Vector2.zero;
        StageCameraController.SwitchMainCamera();
        yield return new WaitForSeconds(0.25f);
        
        if (scarletInferno != null)
        {
            if (scarletInferno.transformCompleted == false)
            {
                scarletInferno.DisableRotation();
                scarletInferno.DoSpiralMovements();
            }

            
        }
        
        DisappearRenderer();
        voiceGroup.BroadCastSpecificVoice((int)VoiceGroupEnum.ChangePhase,0);
        
        ac.SetGravityScale(0);
        SetGroundCollider(false);
        StageCameraController.SwitchOverallCameraFollowObject(gameObject);
        transform.position = new Vector3(0, -5, 0);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(4f);
        
        //切换镜头
        StageCameraController.SwitchOverallCamera();
        yield return new WaitUntil(()=>voiceGroup.voice.isPlaying == false);
        
        //等待第一句语音播放完成，平台开始燃烧，播放第二句语音

        ac.rigid.velocity = Vector2.zero;
        voiceGroup.BroadCastSpecificVoice((int)VoiceGroupEnum.ChangePhase,1);
        CineMachineOperator.Instance.CamaraShake(3f,10f);
        Platform_Burn_Effect(-11,2);
        yield return new WaitForSeconds(3f);
        
        //毁灭猩红咒焰的物体。
        //var scarletInferno = Projectile_C005_3_Boss.Instance;
        if(scarletInferno != null)
            Destroy(scarletInferno.gameObject);
        
        //boss现身
        transform.position = new Vector3(0, -1, 0);
        anim.Play("float");
        Appear_Effect();
        ac.TurnMove(_behavior.targetPlayer);
        yield return new WaitForSeconds(1.25f);
        AppearRenderer();
        
        _tweener = transform.DOMoveY(10f, 3.5f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.75f);
        Platform_Burn_Effect(-3);
        
        //缓慢上升到-3,5时燃烧一个平台,每秒2个单位的速度上升
        
        yield return new WaitForSeconds(0.5f);
        Appear_Platform("2nd");
        yield return new WaitForSeconds(1.5f);

        Platform_Burn_Effect(5);
        yield return new WaitForSeconds(3f);
        Appear_Platform("3rd");
        yield return new WaitUntil(()=>voiceGroup.voice.isPlaying == false);
        
        //背景淡出，播放第三条语音
        
        var backGroundGO = GameObject.Find("Background");
        _tweener = backGroundGO.transform.Find("Background1").GetComponent<SpriteRenderer>().DOColor(Color.black, 3f);
        voiceGroup.BroadCastSpecificVoice((int)VoiceGroupEnum.ChangePhase,2);
        bossBanner?.PrintSkillName("HB01_Action14");
        Appear_Platform("1st");

        FireballWithBody_Effect();

        yield return new WaitForSeconds(2.2f);
        
        CineMachineOperator.Instance.StopCameraShake();
        FireWolf_Effect();
        
        yield return new WaitForSeconds(0.3f);
        
        //向下俯冲
        anim.Play("dive");
        
        yield return new WaitForSeconds(0.5f);
        _tweener = transform.DOMoveY(-9f, 0.45f).SetEase(Ease.InOutCirc);

        yield return new WaitForSeconds(0.4f);
        
        
        CineMachineOperator.Instance.CamaraShake(25f,1.5f);
        Blast_Effect();
        ac.ResetGravityScale();
        SetGroundCollider(true);
        
        //白屏
        
        var whiteScreenImage = GameObject.Find("FullScreenEffect").transform.Find("BlackIn").GetComponent<Image>();
        whiteScreenImage.color = new Color(1, 1, 1, 0);
        //CineMachineOperator.Instance.StopCameraShake();
        
        
        yield return new WaitForSeconds(0.3f);
        whiteScreenImage.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        
        backGroundGO.transform.Find("Background1").gameObject.SetActive(false);

        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);
        anim.Play("idle");
        yield return new WaitUntil(()=>voiceGroup.voice.isPlaying == false);
        
        backGroundGO.transform.Find("GroundPic/effect").gameObject.SetActive(true);
        backGroundGO.transform.Find("GroundPic/Sprite").gameObject.SetActive(false);
        
        whiteScreenImage.DOFade(0, .4f);
        
        
        StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
        StageCameraController.SwitchMainCamera();

        _behavior.currentMoveAction = null;

    }


    protected GameObject BrightCamineRush_Part1_Legend()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(7);

        
        //狼头 有伤害的
        var proj1 = 
            InstantiateDirectional(projectile7, transform.position+new Vector3(ac.facedir,0), container.transform,ac.facedir);

        proj1.GetComponent<DOTweenSimpleController>().moveDirection.x *= (12f / 15f);
        
        
        var attack1 = proj1.GetComponent<AttackFromEnemy>();
        attack1.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,1,1),100,0);
        attack1.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,1,1),100,1);
        
        attack1.firedir = ac.facedir;
        attack1.enemySource = gameObject;
        
        
        
        //三叉火焰轨迹的一段伤害。
        
        var proj2 = 
            Instantiate(projectilePoolEX[12],
                transform.position+new Vector3(ac.facedir*Mathf.Cos(0.35f),Mathf.Sin(0.35f)),
                Quaternion.Euler(0,ac.facedir==1?0:180,20), container.transform);
        
        
        
        var attack2 = proj2.GetComponent<AttackFromEnemy>();
        attack2.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,1,1),100,0);
        attack2.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,1,1),100,1);
        
        attack2.firedir = ac.facedir;
        attack2.enemySource = gameObject;
        
        
        
        
        //三叉火焰轨迹的斜下方的一段伤害。
        
        var proj3 = 
            Instantiate(projectilePoolEX[12],
                transform.position+new Vector3(ac.facedir*Mathf.Cos(0.35f),-Mathf.Sin(0.35f)),
                Quaternion.Euler(0,ac.facedir==1?0:180,-20), container.transform);
        var attack3 = proj3.GetComponent<AttackFromEnemy>();
        attack3.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,1,1),100,0);
        attack3.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,1,1),100,1);
        attack3.firedir = ac.facedir;
        attack3.enemySource = gameObject;
        
        
        //下面都是特效
        
        var projfx1 = Instantiate(projectilePoolEX[6],
            transform.position+new Vector3(ac.facedir*Mathf.Cos(0.35f),Mathf.Sin(0.35f)),
            Quaternion.Euler(0,ac.facedir==1?0:180,20), container.transform);
        var projfx2 = Instantiate(projectilePoolEX[6],
            transform.position+new Vector3(ac.facedir*Mathf.Cos(0.35f),-Mathf.Sin(0.35f)),
            Quaternion.Euler(0,ac.facedir==1?0:180,-20), container.transform);
        projfx2.GetComponentInChildren<DOTweenSimpleController>().moveDirection.y *= -1;
        
        projfx2.GetComponent<DOTweenSimpleController>().moveDirection.x *= (12f / 15f);
        projfx1.GetComponent<DOTweenSimpleController>().moveDirection.x *= (12f / 15f);
        
        if (ac.facedir == -1)
        {
            proj1.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
            projfx1.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
            projfx2.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
        }
        
        
        return container;
    }

    protected void CarmineRush_Part2_Legend(GameObject container)
    {
        var proj2 = 
            InstantiateDirectional(projectilePoolEX[13], transform.position, container.transform, ac.facedir);
        //proj2.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        proj2.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        proj2.GetComponent<AttackFromEnemy>().
            AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,1,1),100,0);
        proj2.GetComponent<AttackFromEnemy>().
            AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,1,1),100,1);

        var proj3 = InstantiateDirectional(projectile8, transform.position, MeeleAttackFXLayer.transform,ac.facedir,0,1);
        
        if (ac.facedir == -1)
        {
            var partical = proj3.GetComponentInChildren<ParticleSystem>().main;
            //ParticleSystem.MinMaxCurve curve;
            partical.startRotationY = new ParticleSystem.MinMaxCurve(partical.startRotationY.constant+Mathf.PI);
        }
        
    }

    protected void BlazingEnhancementBuffLegend()
    {
        var buff = new TimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff,
            100, -1, 100, -1);
        var buff_hard = new TimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff,
            300, 30, 100, 8101401);
        buff_hard.dispellable = false;
        var kbresbuff = new TimerBuff((int)BasicCalculation.BattleCondition.KnockBackImmune,
            -1, 30, 100, -1);
        kbresbuff.dispellable = false;
        
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.BlazewolfsRush,
            -1,-1,1,-1);
        _statusManager.ObtainTimerBuff(kbresbuff);
        _statusManager.ObtainTimerBuff(buff,false);
        _statusManager.ObtainTimerBuff(buff_hard,false);
    }

    protected override void ScarletInferno()
    {
        GameObject container = Instantiate(projectilePoolEX[10], transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        var allAttack = container.GetComponentsInChildren<AttackFromEnemy>();
        foreach (var child in allAttack)
        {
            child.enemySource = gameObject;
        }
    }

    protected void ExpandFireballs(float expandFactor = 4)
    {
        var controller = Projectile_C005_3_Boss.Instance;
        if(controller==null)
            return;
        controller.SetScale(expandFactor,1f);
        //controller.SetAttack(8);
    }
    
    protected void ShrinkFireballs()
    {
        var controller = Projectile_C005_3_Boss.Instance;
        if(controller==null)
            return;
        controller.SetScale(1,1f);
    }

    protected void AccelerateFireballs(float speed = 50,float attack = 8)
    {
        var controller = Projectile_C005_3_Boss.Instance;
        if(controller==null)
            return;
        controller.SetRotateSpeed(speed);
        controller.SetAttack(attack);
    }
    
    protected void DecelerateFireballs()
    {
        var controller = Projectile_C005_3_Boss.Instance;
        if(controller==null)
            return;
        controller.SetRotateSpeed(18);
        controller.SetAttack(4f);
    }
    
    protected void SetRotateRaduisOfFireballs(float r)
    {
        var controller = Projectile_C005_3_Boss.Instance;
        if(controller==null)
            return;
        controller.SetRotateRadius(r,1);
    }

    /// <summary>
    /// 猩红咒焰=>赤焰漫天
    /// </summary>
    protected void SprialMovementOfFireballs()
    {
        var controller = Projectile_C005_3_Boss.Instance;
        if(controller==null)
            return;
        
        controller.DisableRotation();
        controller.ChangeAllAttackPropertiesToRed();
        controller.DoSpiralMovements();

    }
    
    protected void Warp_Effect()
    {
        
        var warpFX = Instantiate(projectilePoolEX[14],
            transform.position+new Vector3(0,1,0), Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        
    }

    protected void Platform_Burn_Effect(float posY,int extraTime = 0)
    {
        var platformBurnFX = Instantiate(projectilePoolEX[15],
            new Vector3(0,posY,0), Quaternion.identity,
            RangedAttackFXLayer.transform);
        if (extraTime > 0)
        {
            var ps = platformBurnFX.GetComponent<ParticleSystem>();
            var mainPs = ps.main;
            //ps.Stop();
            //mainPs.duration += extraTime;
            ps.Play();
        }
    }

    protected void Appear_Effect()
    {
        var appearFX = Instantiate(projectilePoolEX[16],
            transform.position, Quaternion.identity,
            MeeleAttackFXLayer.transform);
        var laserFX = Instantiate(projectilePoolEX[17],
            transform.position + new Vector3(0,1,0), Quaternion.identity,
            MeeleAttackFXLayer.transform);
    }

    protected void Appear_Platform(string name)
    {
        var pltformLayer = GameObject.Find("Platform");
        pltformLayer.transform.Find(name).gameObject.SetActive(true);
    }

    protected void FireballWithBody_Effect()
    {
        var fireballFX = Instantiate(projectilePoolEX[18],
            transform.position - new Vector3(0,2,0), Quaternion.identity,
            MeeleAttackFXLayer.transform);
    }

    protected void Blast_Effect()
    {
        var blastFX = Instantiate(projectilePoolEX[19],
            transform.position-new Vector3(0,1.5f,0), Quaternion.identity,
            RangedAttackFXLayer.transform);
        blastFX.GetComponentInChildren<ForcedAttackFromEnemy>().enemySource = gameObject;
    }
    
    protected void FireWolf_Effect()
    {
        var firewolfFX = Instantiate(projectilePoolEX[20],
            transform.position+new Vector3(ac.facedir,-2f,0),Quaternion.Euler(0,0,-ac.facedir*90),
            MeeleAttackFXLayer.transform);
    }

}
