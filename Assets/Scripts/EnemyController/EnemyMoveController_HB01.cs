using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;


public class EnemyMoveController_HB01 : EnemyMoveManager
{
    
    // Start is called before the first frame update

    private BattleEffectManager _effectManager;
    private Animator anim;
    private BattleStageManager _stageManager;
    private VoiceController_HB01 voice;

    [Header("Warnings")] 
    [SerializeField] private GameObject[] WarningPrefab;

    void Start()
    {
        voice = GetComponentInChildren<VoiceController_HB01>();
        _stageManager = FindObjectOfType<BattleStageManager>();
        MeeleAttackFXLayer = transform.Find("MeeleAttackFX").gameObject;
        RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");
        ac = GetComponent<EnemyControllerHumanoid>();
        anim = GetComponent<Animator>();
        _behavior = GetComponent<DragaliaEnemyBehavior>();
        _effectManager = GameObject.Find("StageManager").GetComponent<BattleEffectManager>();
        _statusManager = GetComponent<StatusManager>();
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
    public IEnumerator HB01_Action01()
    {
        QuitMove();
        
        yield return new WaitUntil(() => !ac.hurt);
        
        ac.OnAttackEnter();
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
    public IEnumerator HB01_Action02()
    {
        QuitMove();
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        
        ac.OnAttackEnter(200);

        yield return new WaitForSeconds(0.5f);
        anim.Play("warp");
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        _effectManager.SpawnTargetLockIndicator(_behavior.targetPlayer,1.2f);
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.WarpAttack);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        WarpAttack_Disappear(true);
        var dir = LockFaceDir(_behavior.targetPlayer);
        var pos = LockPosition(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(.2f);
        WarpAttack_Appear();
        anim.Play("comboDodge1");
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        SetGravityScale(0);
        BackWarp(pos,dir,3f,4.5f);
       
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.22f);
        ComboDodge2_WarpAttack1(pos);
        SetGravityScale(4);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.272f);
        ComboDodge2_WarpAttack2();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();

    }


    /// 普通的c1-c3
    public IEnumerator HB01_Action03()
    {
        QuitMove();
        
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        
        
        var hint = GenerateWarningPrefab(WarningPrefab[0], transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(hintbar.warningTime);
        Destroy(hint); 
        anim.Play("combo1");
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.Combo);
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.21f);
        Combo1();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("combo2");
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.Combo);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.1f);
        Combo2();
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        if (!anim.GetBool("isGround"))
        {
            anim.Play("fall");
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        }

        yield return new WaitUntil(() => anim.GetBool("isGround"));
        anim.Play("combo3");
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.Combo);
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f);
        Combo3();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        QuitAttack();

    }
    
    
    /// Camine Rush 焰红突袭
    public IEnumerator HB01_Action04()
    {
        QuitMove();
        
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner.PrintSkillName_ZH("焰红突袭");
        
        
        var hint = GenerateWarningPrefab(WarningPrefab[1], transform.position,transform.rotation, MeeleAttackFXLayer.transform);
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
    public IEnumerator HB01_Action05()
    {
        QuitMove();
        
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        
        
        _effectManager.SpawnExclamation(gameObject, 
            new Vector3(transform.position.x,transform.position.y+4));

        yield return new WaitForSeconds(0.8f);
        anim.Play("combo7");
        _statusManager.ObtainUnstackableTimerBuff((int)BasicCalculation.BattleCondition.BlazewolfsRush,
            -1,-1,BattleCondition.buffEffectDisplayType.None,1);
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
    public IEnumerator HB01_Action06()
    {
        QuitMove();
        
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        
        
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
    public IEnumerator HB01_Action07()
    {
        QuitMove();
        
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        
        
        var hint = GenerateWarningPrefab(WarningPrefab[2], transform.position,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(hintbar.warningTime);
        Destroy(hint);
        anim.Play("combo5");
        SetGravityScale(0);
        Combo5();
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.Combo);
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        anim.Play("combo6");
        anim.speed = 0;

        yield return new WaitForSeconds(0.15f);
        anim.speed = 1;

        yield return new WaitForSeconds(0.05f);
        var oldPos = transform.position.x;
        Combo6();
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        SetGravityScale(4);
        transform.position = new Vector3(oldPos, transform.position.y);
        QuitAttack();

    }

    /// S2: Flame Raid
    public IEnumerator HB01_Action08()
    {
        QuitMove();

        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner.PrintSkillName_ZH("红焰强袭");

        yield return new WaitForSeconds(1f);
        var hint = GenerateWarningPrefab(WarningPrefab[3], transform.position,transform.rotation, MeeleAttackFXLayer.transform);
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
    public IEnumerator HB01_Action09()
    {
        QuitMove();
        
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner.PrintSkillName_ZH("闪耀焰红突袭");
        
        var hint = GenerateWarningPrefab(WarningPrefab[1], transform.position,transform.rotation, RangedAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        var hint2 = 
            GenerateWarningPrefab(WarningPrefab[1], transform.position,
                (Quaternion.Euler(0,ac.facedir==1?0:180,20)), RangedAttackFXLayer.transform);
        var hint3 = 
            GenerateWarningPrefab(WarningPrefab[1], transform.position,
                (Quaternion.Euler(0,ac.facedir==1?0:180,-20)), RangedAttackFXLayer.transform);
        var hint4 = GenerateWarningPrefab(WarningPrefab[4], transform.position,transform.rotation, RangedAttackFXLayer.transform);
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
    public IEnumerator HB01_Action10()
    {
        QuitMove();
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner.PrintSkillName_ZH("炽烈红焰强袭");

        yield return new WaitForSeconds(.5f);
        var hint = GenerateWarningPrefab(WarningPrefab[5], transform.position+Vector3.up,transform.rotation, MeeleAttackFXLayer.transform);
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
    public IEnumerator HB01_Action11()
    {
        QuitMove();
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner.PrintSkillName_ZH("猩红咒焰");
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(.5f);
        var hint = GenerateWarningPrefab(WarningPrefab[6], transform.position+Vector3.up,transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        
        yield return new WaitForSeconds(hintbar.warningTime);
        anim.Play("buff");
        float animTime;
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.ScarletInferno);
        Destroy(hint,0.5f);
        ScarletInferno();
        StageCameraController.SwitchMainCamera();
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        QuitAttack();
    }





    #region DetailedAction

    /// <summary>
    /// 隐身
    /// </summary>
    /// <param name="invincible">是否进入无敌状态</param>
    void WarpAttack_Disappear(bool invincible)
    {
        
        if (invincible)
        {
            var col = transform.Find("HitSensor").GetComponent<Collider2D>();
            col.enabled = false;
        }
        
        var miniIcon = transform.Find("MinimapIcon").gameObject;
        miniIcon.SetActive(false);
        
        var spriteRenderer1 = GetComponent<SpriteRenderer>();
        
        //spriteRenderer;
        spriteRenderer1.color = 
            new Color(spriteRenderer1.color.r, spriteRenderer1.color.g, spriteRenderer1.color.b, 0);

        

    }
    
    void WarpAttack_Appear()
    {
        
        var spriteRenderer1 = GetComponent<SpriteRenderer>();
        
        //spriteRenderer;
        spriteRenderer1.color = 
            new Color(spriteRenderer1.color.r, spriteRenderer1.color.g, spriteRenderer1.color.b, 100);

        var miniIcon = transform.Find("MinimapIcon").gameObject;
        miniIcon.SetActive(true);

       
        var col = transform.Find("HitSensor").GetComponent<Collider2D>();
        col.enabled = true;



    }

    Vector3 LockPosition(GameObject target)
    {
        return target.transform.position;
    }

    int LockFaceDir(GameObject target)
    {
        return target.GetComponentInChildren<ActorController>().facedir;
    }

    /// <summary>
    /// 背刺
    /// </summary>
    /// <param name="position">目标位置</param>
    /// <param name="dir">X方向</param>
    /// <param name="distanceX">距离X</param>
    /// <param name="distanceY">距离Y</param>
    void BackWarp(Vector3 position, int dir, float distanceX, float distanceY)
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

    void SetGravityScale(float value)
    {
        ac.rigid.gravityScale = value;
    }

    



    void Combo1()
    {

        GameObject projectile_clone1 = Instantiate(projectile3, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        if (ac.facedir == -1)
        {
            var partical = projectile_clone1.GetComponentInChildren<ParticleSystem>().main;
            //ParticleSystem.MinMaxCurve curve;
            partical.startRotationY = new ParticleSystem.MinMaxCurve(partical.startRotationY.constant+Mathf.PI);
        }
        
        
        GameObject container = Instantiate(attackContainer, transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        GameObject projectile_clone2 = Instantiate(projectile4, transform.position, transform.rotation, container.transform);
        projectile_clone2.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        projectile_clone2.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        if (ac.facedir == -1)
        {
            projectile_clone2.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
        }
        
    }

    void Combo2()
    {
        StraightDashMove();
        //RushForward();
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        GameObject projectile_clone1 = Instantiate(projectile5, transform.position, transform.rotation, container.transform);
        projectile_clone1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        if (ac.facedir == -1)
        {
            var partical = projectile_clone1.GetComponentInChildren<ParticleSystem>().main;
            //ParticleSystem.MinMaxCurve curve;
            partical.startRotationY = new ParticleSystem.MinMaxCurve(partical.startRotationY.constant+Mathf.PI);
        }
    }

    void Combo3()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        GameObject projectile_clone1 = Instantiate(projectile6, transform.position, transform.rotation, container.transform);
        projectile_clone1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        if (ac.facedir == -1)
        {
            var partical = projectile_clone1.GetComponentInChildren<ParticleSystem>().main;
            //ParticleSystem.MinMaxCurve curve;
            partical.startRotationY = new ParticleSystem.MinMaxCurve(partical.startRotationY.constant+Mathf.PI);
        }
    }

    void Combo4_Part1()
    {
        _statusManager.ObtainTimerBuff
        ((int)BasicCalculation.BattleCondition.CritRateBuff,20,10,
            BattleCondition.buffEffectDisplayType.Value);
        _tweener = transform.DOMove(new Vector2(transform.position.x-1.5f*ac.facedir,transform.position.y + 4f),
            0.1f).SetEase(Ease.OutCubic);
        
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);

        GameObject proj = Instantiate(projectilePoolEX[1], 
            new Vector3(transform.position.x,transform.position.y+1), transform.rotation, container.transform);
        proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        proj.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        
        if (ac.facedir == -1)
        {
            var particles = proj.GetComponentsInChildren<ParticleSystem>();
            foreach (var particle in particles)
            {
                var main = particle.main;
                main.startRotationY = new ParticleSystem.MinMaxCurve(main.startRotationY.constant+Mathf.PI);
            }
        }
    }

    void Combo4_Part2()
    {
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        

        GameObject proj = Instantiate(projectilePoolEX[2], 
            new Vector3(transform.position.x,transform.position.y-2.5f),
            transform.rotation, container.transform);
        proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        proj.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        
        if (ac.facedir == -1)
        {
            var particles = proj.GetComponentsInChildren<ParticleSystem>();
            foreach (var particle in particles)
            {
                var main = particle.main;
                main.startRotationY = new ParticleSystem.MinMaxCurve(main.startRotationY.constant+Mathf.PI);
            }
        }
    }

    void Combo4_Landing()
    {
        _tweener = transform.DOMove
            (new Vector2(transform.position.x+1.5f*ac.facedir,transform.position.y-4f), 0.1f).SetEase(Ease.InCubic);
        
            
    }

    void Combo5()
    {
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        

        GameObject proj = Instantiate(projectilePoolEX[3], 
            new Vector3(transform.position.x+2f*ac.facedir,transform.position.y),
            transform.rotation, container.transform);
        proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        proj.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        
        if (ac.facedir == -1)
        {
            var particles = proj.GetComponentsInChildren<ParticleSystem>();
            foreach (var particle in particles)
            {
                var main = particle.main;
                main.startRotationY = new ParticleSystem.MinMaxCurve(main.startRotationY.constant+Mathf.PI);
            }
        }
    }
    
    void Combo6()
    {
        _tweener = transform.DOMoveX(transform.position.x + ac.facedir * 8, 0.1f);
        
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        

        GameObject proj = Instantiate(projectilePoolEX[4], 
            new Vector3(transform.position.x,transform.position.y),
            transform.rotation, container.transform);
        proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        proj.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        
        if (ac.facedir == -1)
        {
            var particles = proj.GetComponentsInChildren<ParticleSystem>();
            var shadow = particles[0].GetComponent<DOTweenSimpleController>();
            shadow.moveDirection.x *= -1;

            foreach (var particle in particles)
            {
                var main = particle.main;
                main.startRotationY = new ParticleSystem.MinMaxCurve(main.startRotationY.constant+Mathf.PI);
            }
        }
    }

    void Combo7_Part1()
    {
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);

        GameObject proj = Instantiate(projectile10, transform.position, transform.rotation, container.transform);
        if (ac.facedir == -1)
        {
            var particles = proj.GetComponentsInChildren<ParticleSystem>();
            foreach (var particle in particles)
            {
                var main = particle.main;
                main.startRotationY = new ParticleSystem.MinMaxCurve(main.startRotationY.constant+Mathf.PI);
            }
        }

    }

    void Combo7_DashDownward()
    {
        _tweener = transform.DOMoveY(transform.position.y-2.5f, 0.3f).SetEase(Ease.OutExpo);
    }

    void Combo7_Part2()
    {
        GameObject container = Instantiate(attackContainer,transform.position, transform.rotation,
            RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        

        GameObject proj = Instantiate(projectilePoolEX[0], transform.position, transform.rotation, container.transform);
        proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        proj.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        
        if (ac.facedir == -1)
        {
            var particles = proj.GetComponentsInChildren<ParticleSystem>();
            foreach (var particle in particles)
            {
                var main = particle.main;
                main.startRotationY = new ParticleSystem.MinMaxCurve(main.startRotationY.constant+Mathf.PI);
            }
        }
    }

    void ComboDodge1()
    {
        
        

        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        GameObject projectile_clone1 = Instantiate(projectile1, transform.position, transform.rotation, container.transform);

        projectile_clone1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        //projectile_clone1.GetComponent<AttackFromEnemy>().AddWithCondition(new TimerBuff(999));
        

        if (ac.facedir == -1)
        {
            var particle = projectile_clone1.GetComponentInChildren<ParticleSystem>().main;
            //ParticleSystem.MinMaxCurve curve;
            particle.startRotationY = new ParticleSystem.MinMaxCurve(Mathf.PI);
        }


    }

    void ComboDodge2_WarpAttack1(Vector3 target)
    {
        _tweener = transform.DOMove(target, 0.3f);
        _tweener.SetEase(Ease.OutExpo);
        //OnComplete(OnTweenComplete);
    }
    
    void ComboDodge2_WarpAttack2()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        GameObject projectile_clone1 = Instantiate(projectile2, transform.position, transform.rotation, container.transform);

        projectile_clone1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;

        if (ac.facedir == -1)
        {
            var partical = projectile_clone1.GetComponentInChildren<ParticleSystem>().main;
            //ParticleSystem.MinMaxCurve curve;
            partical.startRotationY = new ParticleSystem.MinMaxCurve(partical.startRotationY.constant+Mathf.PI);
        }
    }

    GameObject CamineRush_Part1()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(2);
        
        //GameObject subcontainer = Instantiate(attackSubContainer, transform.position, transform.rotation, RangedAttackFXLayer.transform);
        //subcontainer.GetComponent<AttackSubContainer>().InitAttackContainer(1,container);

        var proj1 = 
            Instantiate(projectile7, transform.position+new Vector3(ac.facedir,0), transform.rotation, container.transform);
        proj1.GetComponent<AttackFromEnemy>().
            AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,BattleCondition.buffEffectDisplayType.None,1,1));
        proj1.GetComponent<AttackFromEnemy>().
            AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,BattleCondition.buffEffectDisplayType.Value,1,1));
        
        proj1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        proj1.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        if (ac.facedir == -1)
        {
            //proj1.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
        }
        

        return container;
    }

    void CamineRush_Part2(GameObject container)
    {
        var proj2 = 
            Instantiate(projectile9, transform.position, transform.rotation, container.transform);
        //proj2.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        proj2.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        proj2.GetComponent<AttackFromEnemy>().
            AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,BattleCondition.buffEffectDisplayType.None,1,1));
        proj2.GetComponent<AttackFromEnemy>().
            AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,BattleCondition.buffEffectDisplayType.Value,1,1));

        var proj3 = Instantiate(projectile8, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        
        if (ac.facedir == -1)
        {
            var partical = proj3.GetComponentInChildren<ParticleSystem>().main;
            //ParticleSystem.MinMaxCurve curve;
            partical.startRotationY = new ParticleSystem.MinMaxCurve(partical.startRotationY.constant+Mathf.PI);
        }
    }
    
    GameObject BrightCamineRush_Part1()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(7);

        var proj1 = 
            Instantiate(projectile7, transform.position+new Vector3(ac.facedir,0), transform.rotation, container.transform);
        var attack1 = proj1.GetComponent<AttackFromEnemy>();
        attack1.AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,BattleCondition.buffEffectDisplayType.None,1,1));
        attack1.AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,BattleCondition.buffEffectDisplayType.Value,1,1));
        
        attack1.firedir = ac.facedir;
        attack1.enemySource = gameObject;
        
        var proj2 = 
            Instantiate(projectilePoolEX[9],
                transform.position+new Vector3(ac.facedir*Mathf.Cos(0.35f),Mathf.Sin(0.35f)),
                Quaternion.Euler(0,ac.facedir==1?0:180,20), container.transform);
        var attack2 = proj2.GetComponent<AttackFromEnemy>();
        attack2.AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,BattleCondition.buffEffectDisplayType.None,1,1));
        attack2.AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,BattleCondition.buffEffectDisplayType.Value,1,1));
        
        attack2.firedir = ac.facedir;
        attack2.enemySource = gameObject;
        
        var proj3 = 
            Instantiate(projectilePoolEX[9],
                transform.position+new Vector3(ac.facedir*Mathf.Cos(0.35f),-Mathf.Sin(0.35f)),
                Quaternion.Euler(0,ac.facedir==1?0:180,-20), container.transform);
        var attack3 = proj3.GetComponent<AttackFromEnemy>();
        attack3.AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,BattleCondition.buffEffectDisplayType.None,1,1));
        attack3.AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,BattleCondition.buffEffectDisplayType.Value,1,1));
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
            //proj1.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
            //projfx1.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
            //projfx2.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
        }
        
        
        return container;
    }
    
    void BrightCamineRush_Part2(GameObject container, Quaternion rotation)
    {
        
        var proj2 = 
            Instantiate(projectilePoolEX[7],
                transform.position+new Vector3(ac.facedir,0),
                rotation, container.transform);
        var attack2 = proj2.GetComponent<AttackFromEnemy>();
        attack2.AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
            -1,30,BattleCondition.buffEffectDisplayType.None,1,1));
        attack2.AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
            15,30,BattleCondition.buffEffectDisplayType.Value,1,1));
        
        var projfx1 = Instantiate(projectilePoolEX[6],
            transform.position+transform.right,
            rotation, container.transform);
        
        Debug.Log(projfx1.transform.rotation.eulerAngles);
        if(projfx1.transform.rotation.eulerAngles.z<270 && projfx1.transform.rotation.eulerAngles.z>90)
            projfx1.transform.rotation = 
                Quaternion.Euler(projfx1.transform.rotation.eulerAngles.x,
                    180,
                    (180-projfx1.transform.rotation.eulerAngles.z));

        var moveController = projfx1.GetComponent<DOTweenSimpleController>();
        var angle = Vector2.SignedAngle(Vector2.right, rotation.eulerAngles) * Mathf.Deg2Rad;
        angle = rotation.eulerAngles.z* Mathf.Deg2Rad;
        
        //print("angle"+angle);
        moveController.moveDirection.x = 30f * Mathf.Cos(angle) * ac.facedir;
        moveController.moveDirection.y = 30f * Mathf.Sin(angle);
        moveController.duration *= 1.2f;

        attack2.enemySource = gameObject;


    }
    
    void BrightCamineRush_Part3(GameObject container)
    {

        //GameObject subcontainer = Instantiate(attackSubContainer, transform.position, transform.rotation, RangedAttackFXLayer.transform);
        //subcontainer.GetComponent<AttackSubContainer>().InitAttackContainer(1,container);

        var proj1 = 
            Instantiate(projectile7, transform.position+new Vector3(ac.facedir,0), transform.rotation, container.transform);
        proj1.GetComponent<AttackFromEnemy>().
            AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                -1,30,BattleCondition.buffEffectDisplayType.None,1,1));
        proj1.GetComponent<AttackFromEnemy>().
            AddWithCondition(new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable,
                15,30,BattleCondition.buffEffectDisplayType.Value,1,1));
        
        proj1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        proj1.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        if (ac.facedir == -1)
        {
            //proj1.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
        }
        
    }

    private void FlameRaid()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        var proj1 = 
            Instantiate(projectilePoolEX[5], transform.position, Quaternion.identity, container.transform);
        proj1.GetComponent<AttackFromEnemy>().AddWithCondition(new TimerBuff(999));
        proj1.GetComponent<AttackFromEnemy>().AddWithCondition
            (new TimerBuff((int)BasicCalculation.BattleCondition.Burn,
                72.7f,12f,BattleCondition.buffEffectDisplayType.None,1));
        
        proj1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        proj1.GetComponent<AttackFromEnemy>().enemySource = gameObject;
    }
    private void SavageFlameRaid()
    {
        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        var proj1 = 
            Instantiate(projectilePoolEX[8], transform.position+Vector3.up, Quaternion.identity, container.transform);
        proj1.GetComponent<AttackFromEnemy>().AddWithCondition(new TimerBuff(999));
        proj1.GetComponent<AttackFromEnemy>().AddWithCondition
        (new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
            31.1f,21f,BattleCondition.buffEffectDisplayType.None,1));
        
        proj1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        proj1.GetComponent<AttackFromEnemy>().enemySource = gameObject;
    }

    private void ScarletInferno()
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

    void StraightDashMove()
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

        
            //var target = hitinfo.collider.gameObject.GetComponent<BoxCollider2D>();
            //var tarX = target.offset.x + (-(target.size.x + 1));
        _tweener = transform.DOMoveX(tarX, 0.2f);
        
            
        
    }

    Vector3 RushForward()
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
        _tweener.SetEase(Ease.OutExpo).OnComplete(DisappearRenderer);
        //print(_tweener.active);

        return origin;
    }
    void RushBack(Vector3 origin)
    {
        _tweener = transform.DOMove(origin, 0.1f).SetEase(Ease.OutExpo).OnComplete(AppearRenderer);
        _tweener.Play();
        
    }

    void DisappearRenderer()
    {
        var spriteRenderer1 = GetComponent<SpriteRenderer>();
        
        //spriteRenderer;
        spriteRenderer1.color = 
            new Color(spriteRenderer1.color.r, spriteRenderer1.color.g, spriteRenderer1.color.b, 0);
        
    }
    void AppearRenderer()
    {
        var spriteRenderer1 = GetComponent<SpriteRenderer>();
        
        //spriteRenderer;
        spriteRenderer1.color = 
            new Color(spriteRenderer1.color.r, spriteRenderer1.color.g, spriteRenderer1.color.b, 100);
        
    }

    

    #endregion



    /// <summary>
    /// 生成基于transform的红紫圈。
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="where"></param>
    /// <param name="_parent"></param>
    /// <returns></returns>
    GameObject GenerateWarningPrefab(GameObject prefab, Vector3 where,Quaternion rot, Transform _parent)
    {
        var clone = Instantiate(prefab, where, rot, _parent);
        clone.GetComponent<EnemyAttackHintBar>().SetSource(ac);
        return clone;
    }
    
    void QuitAttack()
    {
        _behavior.currentAttackAction = null;
        ac.OnAttackExit();
        anim.Play("idle");
        currentAttackMove = null; //只有行为树在用
        OnAttackFinished?.Invoke(true);
    }

    void QuitMove()
    {
        ac.currentKBRes = 999;
        if (ac.VerticalMoveRoutine != null)
        {
            StopCoroutine(ac.VerticalMoveRoutine);
            ac.VerticalMoveRoutine = null;
        }
    }

}
