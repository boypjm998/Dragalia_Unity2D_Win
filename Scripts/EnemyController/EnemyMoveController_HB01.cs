using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

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
                currentAttackMove = StartCoroutine(HB01_Action01(0));
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
    public IEnumerator HB01_Action01(float interval)
    {
        QuitMove();
        
        yield return new WaitUntil(() => !ac.hurt);
        
        ac.OnAttackEnter();
        yield return new WaitForSeconds(interval);

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
    public IEnumerator HB01_Action02(float interval)
    {
        QuitMove();
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        
        ac.OnAttackEnter();
        
        yield return new WaitForSeconds(interval);
        anim.Play("warp");
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        _effectManager.SpawnTargetLockIndicator(_behavior.targetPlayer,1.2f);
        voice.PlayMyVoice(VoiceController_HB01.myMoveList.WarpAttack);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        Disappear(true);
        var dir = LockFaceDir(_behavior.targetPlayer);
        var pos = LockPosition(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(.2f);
        Appear();
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

    public IEnumerator HB01_Action03(float interval)
    {
        QuitMove();
        
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(interval);
        var hint = GenerateWarningPrefab(WarningPrefab[0], transform, MeeleAttackFXLayer.transform);
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
    public IEnumerator HB01_Action04(float interval)
    {
        QuitMove();
        
        yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(200);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(interval);
        var hint = GenerateWarningPrefab(WarningPrefab[1], transform, MeeleAttackFXLayer.transform);
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




    /// <summary>
    /// 隐身
    /// </summary>
    /// <param name="invincible">是否进入无敌状态</param>
    void Disappear(bool invincible)
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
    
    void Appear()
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

    void QuitAttack()
    {
        ac.OnAttackExit();
        anim.Play("idle");
        currentAttackMove = null; //只有行为树在用
        _behavior.currentAttackAction = null;
        
        OnAttackFinished?.Invoke(true);
    }

    void QuitMove()
    {
        if (ac.VerticalMoveRoutine != null)
        {
            StopCoroutine(ac.VerticalMoveRoutine);
            ac.VerticalMoveRoutine = null;
        }
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

    void ComboDodge1()
    {
        
        

        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        GameObject projectile_clone1 = Instantiate(projectile1, transform.position, transform.rotation, container.transform);

        projectile_clone1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;

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
            proj1.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= -1;
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



    GameObject GenerateWarningPrefab(GameObject prefab, Transform where, Transform _parent)
    {
        var clone = Instantiate(prefab, where.position, where.rotation, _parent);
        return clone;
    }

}
