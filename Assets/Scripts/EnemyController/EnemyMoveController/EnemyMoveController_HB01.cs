using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using GameMechanics;
using CharacterSpecificProjectiles;
public class EnemyMoveController_HB01 : EnemyMoveManager
{
    
    // Start is called before the first frame update

    
    
    private VoiceController_HB01 voice;
    

    protected override void Start()
    {
        voice = GetComponentInChildren<VoiceController_HB01>();
        _stageManager = FindObjectOfType<BattleStageManager>();
        MeeleAttackFXLayer = transform.Find("MeeleAttackFX").gameObject;
        RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");
        ac = GetComponent<EnemyControllerHumanoid>();
        anim = GetComponentInChildren<Animator>();
        _behavior = GetComponent<DragaliaEnemyBehavior>();
        _effectManager = GameObject.Find("StageManager").GetComponent<BattleEffectManager>();
        _statusManager = GetComponent<StatusManager>();
        CopyProjectilesToPool();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 弃用的
    /// </summary>
    /// <param name="moveID"></param>
    public override void UseMove(int moveID)
    {
        switch (moveID)
        {
            case 1:
                currentAttackMove = StartCoroutine(HB01_Action01());
                //anim.Play("comboDodge0");
                break;
            default:
                break;
        }
    }
    
    
    //Actions

    /// <summary>
    /// 普通的闪避攻击
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator HB01_Action01()
    {
        QuitMove();
        
        yield return new WaitUntil(() => !ac.hurt);
        
        ac.OnAttackEnter();
        ac.SetCounter(true);
        anim.Play("comboDodge0");
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.SingleDodgeCombo);
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.13f);
        
        ComboDodge1();
        //yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
        
        //currentAttackMove = null;OnAttackFinished?.Invoke(true);
    }

    /// <summary>
    /// 瞬移攻击
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator HB01_Action02()
    {
        QuitMove();
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        
        ac.OnAttackEnter(200);

        yield return new WaitForSeconds(0.5f);
        anim.Play("warp");
        DisappearRenderer();
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        _effectManager.SpawnTargetLockIndicator(_behavior.targetPlayer,1.2f);
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.WarpAttack);
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
        QuitAttack();

    }


    /// 普通的c1-c3
    public virtual IEnumerator HB01_Action03()
    {
        QuitMove();
        
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetCounter(true);
        
        
        var hint = GenerateWarningPrefab(WarningPrefabs[0], transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(hintbar.warningTime);
        Destroy(hint,0.5f); //4/3添加
        anim.Play("combo1");
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.Combo);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.13f);
        Combo1();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);
        anim.Play("combo2");
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.Combo);
        
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
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.Combo);
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.25f);
        Combo3();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();

    }
    
    
    /// Camine Rush 焰红突袭
    public virtual IEnumerator HB01_Action04()
    {
        QuitMove();
        
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action04");
        
        
        var hint = GenerateWarningPrefab(WarningPrefabs[1], transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(hintbar.warningTime);
        Destroy(hint);
        anim.Play("s1_rush");
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.CamineRush);
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

    /// Combo 7
    public virtual IEnumerator HB01_Action05()
    {
        QuitMove();
        
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();ac.SetCounter(true);
        ac.TurnMove(_behavior.targetPlayer);
        GenerateWarningPrefab(WarningPrefabs[7],transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        
        
        _effectManager.SpawnExclamation(gameObject, 
            new Vector3(transform.position.x,transform.position.y+4));

        yield return new WaitForSeconds(0.8f);
        anim.Play("combo7");
        _statusManager.ObtainUnstackableTimerBuff((int)BasicCalculation.BattleCondition.BlazewolfsRush,
            -1,-1,1);
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.Combo);
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
        SetGravityScale(4);
        
        //yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        QuitAttack();



    }
    
    /// Combo4
    public virtual IEnumerator HB01_Action06()
    {
        QuitMove();
        
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
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.Combo);
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
        SetGravityScale(4);
        SetGroundCollider(true);
        QuitAttack();

    }

    /// Combo5 - Combo6
    public virtual IEnumerator HB01_Action07()
    {
        QuitMove();
        
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
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.Combo);
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

    /// S2: Flame Raid
    public virtual IEnumerator HB01_Action08()
    {
        QuitMove();

        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action08");

        yield return new WaitForSeconds(1f);
        var hint = GenerateWarningPrefab(WarningPrefabs[3], transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        
        yield return new WaitForSeconds(hintbar.warningTime);
        anim.Play("s2");
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.FlameRaid);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15f);
        Destroy(hint,0.3f);
        FlameRaid();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
    }
    
    /// Bright Camine Rush
    public virtual IEnumerator HB01_Action09()
    {
        QuitMove();
        
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
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.BrightCamineRush);
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
    
    
    /// Savage Flame Raid
    public virtual IEnumerator HB01_Action10()
    {
        QuitMove();
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action10");

        yield return new WaitForSeconds(.5f);
        var hint = GenerateWarningPrefab(WarningPrefabs[5], transform.position+Vector3.up,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        
        yield return new WaitForSeconds(hintbar.warningTime);
        anim.Play("s2");
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.SavageFlameRaid);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15f);
        Destroy(hint,0.3f);
        SavageFlameRaid();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
    }
    
    /// Scarlet Inferno+ 猩红咒焰
    public virtual IEnumerator HB01_Action11()
    {
        QuitMove();
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB01_Action11");
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(.5f);
        var hint = GenerateWarningPrefab(WarningPrefabs[6], transform.position+Vector3.up,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        
        yield return new WaitForSeconds(hintbar.warningTime);
        anim.Play("buff");
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        voice.BroadcastVoice(11);
        
        Destroy(hint,0.5f);
        ScarletInferno();
        StageCameraController.SwitchMainCamera();
        _behavior.controllAfflictionProtect = false;
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        QuitAttack();
    }
    
    /// <summary>
    /// Blazing Enhancement 闪焰强化
    /// </summary>
    public virtual IEnumerator HB01_Action13()
    {
        QuitMove();
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
        voice.BroadcastVoice(13);
        BlazingEnhancementBuff();
        
        
        
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
    }
    
    
    public virtual IEnumerator HB01_Action13_2()
    {
        QuitMove();
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
        voice.BroadcastVoice(13);
        BlazingEnhancementBuff(2);
        
        
        
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();
    }





    #region DetailedAction

    protected void WarpAttack_ShadowEffect()
    {
        InstantiateDirectional(projectilePoolEX[11], transform.position, RangedAttackFXLayer.transform, ac.facedir, 0,
            1);
        
    }


    /// <summary>
    /// 隐身
    /// </summary>
    /// <param name="invincible">是否进入无敌状态</param>
    protected void WarpAttack_Disappear(bool invincible)
    {
        
        if (invincible)
        {
            var col = transform.Find("HitSensor").GetComponent<Collider2D>();
            col.enabled = false;
        }
        
        var miniIcon = transform.Find("MinimapIcon").gameObject;
        miniIcon.SetActive(false);
        DisappearRenderer();
        
    }
    
    protected void WarpAttack_Appear()
    {
        
        AppearRenderer();
        
        var miniIcon = transform.Find("MinimapIcon").gameObject;
        miniIcon.SetActive(true);

        var col = transform.Find("HitSensor").GetComponent<Collider2D>();
        col.enabled = true;
        

    }

    protected Vector3 LockPosition(GameObject target)
    {
        return target.transform.position;
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

    /// <summary>
    /// 背刺
    /// </summary>
    /// <param name="position">目标位置</param>
    /// <param name="dir">X方向</param>
    /// <param name="distanceX">距离X</param>
    /// <param name="distanceY">距离Y</param>
    protected void BackWarp(Vector3 position, int dir, float distanceX, float distanceY)
    {
        var map = GameObject.Find("StageManager").GetComponent<BattleStageManager>();
        float posX, posY;

        posX = position.x - dir * distanceX;

        if (posX <= map.mapBorderL)
        {
            posX = map.mapBorderL + 1f;
        }
        else if (posX >= map.mapBorderR)
        {
            posX = map.mapBorderR - 1f;
        }

        posY = position.y + distanceY;

        if (posY >= map.mapBorderT)
        {
            posY = map.mapBorderT - 1f;
        }

        transform.position = new Vector3(posX, posY);
        ac.SetFaceDir(dir);

    }

    protected void SetGravityScale(float value)
    {
        ac.rigid.gravityScale = value;
    }

    



    protected void Combo1()
    {

        GameObject projectile_clone1 = InstantiateDirectional(projectile3, transform.position, MeeleAttackFXLayer.transform, ac.facedir,0,1);
        if (ac.facedir == -1)
        {
            var partical = projectile_clone1.GetComponentInChildren<ParticleSystem>().main;
            //ParticleSystem.MinMaxCurve curve;
            partical.startRotationY = new ParticleSystem.MinMaxCurve(partical.startRotationY.constant+Mathf.PI);
        }
        
        
        GameObject container = Instantiate(attackContainer, transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        GameObject projectile_clone2 = InstantiateDirectional(projectile4, transform.position, container.transform, ac.facedir);
        projectile_clone2.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        projectile_clone2.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        if (ac.facedir == -1)
        {
            projectile_clone2.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
        }
        
    }

    protected void Combo2()
    {
        StraightDashMove();
        //RushForward();
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        GameObject projectile_clone1 = InstantiateDirectional
            (projectile5, transform.position, container.transform,ac.facedir,0,1);
        projectile_clone1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        if (ac.facedir == -1)
        {
            var partical = projectile_clone1.GetComponentInChildren<ParticleSystem>().main;
            //ParticleSystem.MinMaxCurve curve;
            partical.startRotationY = new ParticleSystem.MinMaxCurve(partical.startRotationY.constant+Mathf.PI);
        }
    }
    
    

    protected void Combo3()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        GameObject projectile_clone1 = InstantiateDirectional(projectile6, transform.position,container.transform,ac.facedir,0,1);
        projectile_clone1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        if (ac.facedir == -1)
        {
            var partical = projectile_clone1.GetComponentInChildren<ParticleSystem>().main;
            //ParticleSystem.MinMaxCurve curve;
            partical.startRotationY = new ParticleSystem.MinMaxCurve(partical.startRotationY.constant+Mathf.PI);
        }
    }

    protected void Combo4_Part1()
    {
        _statusManager.ObtainTimerBuff
        ((int)BasicCalculation.BattleCondition.CritRateBuff,20,10);
        _tweener = transform.DOMove(new Vector2(transform.position.x-1.5f*ac.facedir,transform.position.y + 4f),
            0.1f).SetEase(Ease.OutCubic);
        
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);

        GameObject proj = InstantiateDirectional(projectilePoolEX[1], 
            new Vector3(transform.position.x,transform.position.y+1),container.transform,ac.facedir,0,1);
        proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        proj.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        
        if (ac.facedir == -1)
        {
        }
    }

    protected void Combo4_Part2()
    {
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        

        GameObject proj = InstantiateDirectional(projectilePoolEX[2], 
            new Vector3(transform.position.x,transform.position.y-2.5f),
             container.transform,ac.facedir,0,1);
        proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        proj.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        
        if (ac.facedir == -1)
        {
        }
    }

    protected void Combo4_Landing()
    {
        _tweener = transform.DOMove
            (new Vector2(transform.position.x+1.5f*ac.facedir,transform.position.y-4f), 0.1f).SetEase(Ease.InCubic);
        
            
    }

    protected void Combo5()
    {
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        

        GameObject proj = InstantiateDirectional(projectilePoolEX[3], 
            new Vector3(transform.position.x+2f*ac.facedir,transform.position.y),
             container.transform,ac.facedir,0,1);
        proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        proj.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        
        // if (ac.facedir == -1)
        // {
        //     var particles = proj.GetComponentsInChildren<ParticleSystem>();
        //     foreach (var particle in particles)
        //     {
        //         var main = particle.main;
        //         main.startRotationY = new ParticleSystem.MinMaxCurve(main.startRotationY.constant+Mathf.PI);
        //     }
        // }
    }
    
    protected void Combo6()
    {
        _tweener = transform.DOMoveX(transform.position.x + ac.facedir * 8, 0.1f);
        
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        

        GameObject proj = InstantiateDirectional(projectilePoolEX[4], 
            new Vector3(transform.position.x,transform.position.y),
            container.transform,ac.facedir,0,1);
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

        GameObject proj = Instantiate
            (projectile10, transform.position,transform.rotation, container.transform);

    }

    protected void Combo7_DashDownward()
    {
        _tweener = transform.DOMoveY(transform.position.y-2.5f, 0.3f).SetEase(Ease.OutExpo);
    }

    protected void Combo7_Part2()
    {
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        

        GameObject proj = InstantiateDirectional
            (projectilePoolEX[0], transform.position, container.transform,ac.facedir,0,1);
        proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        proj.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        
        // if (ac.facedir == -1)
        // {
        //     var particles = proj.GetComponentsInChildren<ParticleSystem>();
        //     foreach (var particle in particles)
        //     {
        //         var main = particle.main;
        //         main.startRotationY = new ParticleSystem.MinMaxCurve(main.startRotationY.constant+Mathf.PI);
        //     }
        // }
    }

    protected void ComboDodge1()
    {
        
        

        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        GameObject projectile_clone1 = InstantiateDirectional(projectile1, transform.position,container.transform,0,1);

        projectile_clone1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
       
        

        if (ac.facedir == -1)
        {
           // var particle = projectile_clone1.GetComponentInChildren<ParticleSystem>().main;
            //ParticleSystem.MinMaxCurve curve;
            //particle.startRotationY = new ParticleSystem.MinMaxCurve(Mathf.PI);
        }


    }

    protected void ComboDodge2_WarpAttack1(Vector3 target)
    {
        _tweener = transform.DOMove(target, 0.3f);
        _tweener.SetEase(Ease.OutExpo);
        //OnComplete(OnTweenComplete);
    }
    
    protected void ComboDodge2_WarpAttack2()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        GameObject projectile_clone1 = InstantiateDirectional(projectile2, transform.position, container.transform,0,1);

        projectile_clone1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;

        if (ac.facedir == -1)
        {
            //var partical = projectile_clone1.GetComponentInChildren<ParticleSystem>().main;
            //ParticleSystem.MinMaxCurve curve;
            //partical.startRotationY = new ParticleSystem.MinMaxCurve(partical.startRotationY.constant+Mathf.PI);
        }
    }

    protected GameObject CamineRush_Part1()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(2);
        
        //GameObject subcontainer = Instantiate(attackSubContainer, transform.position, transform.rotation, RangedAttackFXLayer.transform);
        //subcontainer.GetComponent<AttackSubContainer>().InitAttackContainer(1,container);

        var proj1 = 
            InstantiateDirectional(projectile7, transform.position+new Vector3(ac.facedir,0), container.transform,ac.facedir);
        
        proj1.GetComponent<AttackFromEnemy>().
            AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,1,1),100,0);
        proj1.GetComponent<AttackFromEnemy>().
            AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,1,1),100,1);
        
        proj1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        proj1.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        if (ac.facedir == -1)
        {
            proj1.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
        }
        

        return container;
    }

    /// <summary>
    /// 直着的火焰轨迹+伤害。
    /// </summary>
    protected void CamineRush_Part2(GameObject container)
    {

        var proj2 = 
            InstantiateDirectional(projectile9, transform.position, container.transform, ac.facedir);
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
    
    protected GameObject BrightCamineRush_Part1()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(7);

        var proj1 = 
            InstantiateDirectional(projectile7, transform.position+new Vector3(ac.facedir,0), container.transform,ac.facedir);
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
            Instantiate(projectilePoolEX[9],
                transform.position+new Vector3(ac.facedir*Mathf.Cos(0.35f),Mathf.Sin(0.35f)),
                Quaternion.Euler(0,ac.facedir==1?0:180,20), container.transform);
        var attack2 = proj2.GetComponent<AttackFromEnemy>();
        attack2.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,1,1),100,0);
        attack2.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,1,1),100,1);
        
        attack2.firedir = ac.facedir;
        attack2.enemySource = gameObject;
        
        var proj3 = 
            Instantiate(projectilePoolEX[9],
                transform.position+new Vector3(ac.facedir*Mathf.Cos(0.35f),-Mathf.Sin(0.35f)),
                Quaternion.Euler(0,ac.facedir==1?0:180,-20), container.transform);
        var attack3 = proj3.GetComponent<AttackFromEnemy>();
        attack3.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,1,1),100,0);
        attack3.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,1,1),100,1);
        attack3.firedir = ac.facedir;
        attack3.enemySource = gameObject;
        
        var projfx1 = Instantiate(projectilePoolEX[6],
            transform.position+new Vector3(ac.facedir*Mathf.Cos(0.35f),Mathf.Sin(0.35f)),
            Quaternion.Euler(0,ac.facedir==1?0:180,20), container.transform);
        var projfx2 = Instantiate(projectilePoolEX[6],
            transform.position+new Vector3(ac.facedir*Mathf.Cos(0.35f),-Mathf.Sin(0.35f)),
            Quaternion.Euler(0,ac.facedir==1?0:180,-20), container.transform);
        projfx2.GetComponentInChildren<DOTweenSimpleController>().moveDirection.y *= -1;
        
        if (ac.facedir == -1)
        {
            proj1.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
            projfx1.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
            projfx2.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
        }
        
        
        return container;
    }
    
    protected void BrightCamineRush_Part2(GameObject container, Quaternion rotation)
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

        //追踪狼头+伤害
        
        
        var proj2 = 
            Instantiate(projectilePoolEX[7],
                transform.position+new Vector3(ac.facedir-ac.facedir,0),
                rotation, container.transform);//rotation->newRot
        
        var attack2 = proj2.GetComponent<AttackFromEnemy>();
        attack2.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
            -1,30,1,1),100,0);
        attack2.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
            15,30,1,1),100,1);
        
        var projfx1 = Instantiate(projectilePoolEX[6],
            transform.position+new Vector3(ac.facedir-ac.facedir,0,0),
            rotation, container.transform);
        
        if(projfx1.transform.rotation.eulerAngles.z<270 && projfx1.transform.rotation.eulerAngles.z>90)
            projfx1.transform.rotation = 
                Quaternion.Euler(projfx1.transform.rotation.eulerAngles.x,
                    180,
                    (180-projfx1.transform.rotation.eulerAngles.z));

        var moveController = projfx1.GetComponent<DOTweenSimpleController>();
        var angle = Vector2.SignedAngle(Vector2.right, rotation.eulerAngles) * Mathf.Deg2Rad;
        angle = rotation.eulerAngles.z* Mathf.Deg2Rad;
        
        //print("angle"+angle);
        moveController.moveDirection.x = 30f * Mathf.Cos(angle);
        moveController.moveDirection.y = 30f * Mathf.Sin(angle);
        moveController.duration *= 1.2f;

        attack2.enemySource = gameObject;


    }
    
    /// <summary>
    /// 第二次攻击的狼头伤害部分
    /// </summary>
    /// <param name="container"></param>
    protected void BrightCamineRush_Part3(GameObject container)
    {

        //GameObject subcontainer = Instantiate(attackSubContainer, transform.position, transform.rotation, RangedAttackFXLayer.transform);
        //subcontainer.GetComponent<AttackSubContainer>().InitAttackContainer(1,container);
        
        //狼头+伤害，第二次的攻击。
        var proj1 = 
            InstantiateDirectional(projectile7, transform.position+new Vector3(ac.facedir,0), container.transform,ac.facedir);
        
        
        //var proj1 = 
            //Instantiate(projectile7, transform.position+new Vector3(ac.facedir,0), transform.rotation, container.transform);
        proj1.GetComponent<AttackFromEnemy>().
            AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,1,1),100,0);
        proj1.GetComponent<AttackFromEnemy>().
            AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,1,1),100,1);
        
        proj1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        proj1.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        if (ac.facedir == -1)
        {
            proj1.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
        }
        
    }

    protected void FlameRaid()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        var proj1 = 
            Instantiate(projectilePoolEX[5], transform.position, Quaternion.identity, container.transform);
        proj1.GetComponent<AttackFromEnemy>().AddWithConditionAll(new TimerBuff(999),100);
        proj1.GetComponent<AttackFromEnemy>().AddWithConditionAll
            (new TimerBuff((int)BasicCalculation.BattleCondition.Burn,
                72.7f,12f,1),100,1);
        
        proj1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        proj1.GetComponent<AttackFromEnemy>().enemySource = gameObject;
    }
    protected void SavageFlameRaid()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        var proj1 = 
            Instantiate(projectilePoolEX[8], transform.position+Vector3.up, Quaternion.identity, container.transform);
        proj1.GetComponent<AttackFromEnemy>().AddWithConditionAll(new TimerBuff(999),100);
        proj1.GetComponent<AttackFromEnemy>().AddWithConditionAll
        (new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
            31.1f,21f,1),100,1);
        
        proj1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        proj1.GetComponent<AttackFromEnemy>().enemySource = gameObject;
    }

    protected virtual void ScarletInferno()
    {
        var proj = FindObjectOfType<Projectile_C005_3>();
        if (proj != null)
        {
            Destroy(proj.gameObject);
        }

        GameObject container = Instantiate(projectilePoolEX[10], transform.position+Vector3.up, Quaternion.identity,
            MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        var allAttack = container.GetComponentsInChildren<AttackFromEnemy>();
        foreach (var child in allAttack)
        {
            child.enemySource = gameObject;
        }
        
    }

    protected void BlazingEnhancementBuff(int difficulty=3)
    {
        var buffEffect1 = 200f;
        var buffEffect2 = 100f;
        if (difficulty == 2)
        {
            buffEffect1 = 50f;
            buffEffect2 = 20f;
        }
        //1001 关卡名 005 buff type 01序号
        var buff = new TimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff,
            buffEffect1/2, 10, 100, -1);
        var buff_hard = new TimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff,
            buffEffect2, -1, 100, -1);
        buff_hard.dispellable = false;
        _statusManager.ObtainTimerBuff(buff);
        _statusManager.ObtainTimerBuff(buff,false);
        _statusManager.ObtainTimerBuff(buff_hard);
    }

    protected void StraightDashMove()
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
            
                if (tarX > _stageManager.mapBorderR)
                    tarX = _stageManager.mapBorderR - 1;
            
                if (tarX < _stageManager.mapBorderL)
                    tarX = _stageManager.mapBorderL + 1;
            }
        }
        else
        {
            tarX = transform.position.x + 10 * ac.facedir;
            
            if (tarX > _stageManager.mapBorderR)
                tarX = _stageManager.mapBorderR - 1;
            
            if (tarX < _stageManager.mapBorderL)
                tarX = _stageManager.mapBorderL + 1;
            
        }

        if (tarX > _stageManager.mapBorderR)
            tarX = _stageManager.mapBorderR - 1;
            
        if (tarX < _stageManager.mapBorderL)
            tarX = _stageManager.mapBorderL + 1;
            //var target = hitinfo.collider.gameObject.GetComponent<BoxCollider2D>();
            //var tarX = target.offset.x + (-(target.size.x + 1));
        _tweener = transform.DOMoveX(tarX, 0.2f);
        
            
        
    }

    protected Vector3 RushForward()
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
        
        _tweener = ac.rigid.DOMove(target, 0.25f);
        _tweener.SetEase(Ease.OutExpo).OnComplete(DisappearRenderer);
        //print(_tweener.active);

        return origin;
    }
    protected void RushBack(Vector3 origin)
    {
        _tweener = transform.DOMove(origin, 0.1f).SetEase(Ease.OutExpo).OnComplete(AppearRenderer);
        _tweener.Play();
        
    }

    public override void DisappearRenderer()
    {
        //var spriteRenderer1 = GetComponent<SpriteRenderer>();
        ac.rendererObject.SetActive(false);
        ac.SwapWeaponVisibility(false);
        ac.minimapIcon.SetActive(false);
        //spriteRenderer;
        //spriteRenderer1.color = 
            //new Color(spriteRenderer1.color.r, spriteRenderer1.color.g, spriteRenderer1.color.b, 0);
        
    }
    public override void AppearRenderer()
    {
        //var spriteRenderer1 = GetComponent<SpriteRenderer>();
        ac.rendererObject.SetActive(true);
        ac.SwapWeaponVisibility(true);
        ac.minimapIcon.SetActive(true);
        //spriteRenderer;
        //spriteRenderer1.color = 
            //new Color(spriteRenderer1.color.r, spriteRenderer1.color.g, spriteRenderer1.color.b, 100);
        
    }

    

    #endregion



    /// <summary>
    /// 生成基于transform的红紫圈。
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="where"></param>
    /// <param name="_parent"></param>
    /// <returns></returns>
    
    
    protected override void QuitAttack()
    {
        //ac.SetCounter(false);
        _behavior.currentAttackAction = null;
        ac.OnAttackExit();
        anim.Play("idle");
        currentAttackMove = null; //只有行为树在用
        OnAttackFinished?.Invoke(true);
    }

    

    public override void PlayVoice(int id)
    {
        switch (id)
        {
            case 0:
                voice.BroadcastVoice(0);
                break;
        }
    }

}
