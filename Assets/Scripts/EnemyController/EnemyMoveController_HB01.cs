using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using DG.Tweening;
using UnityEngine;

public class EnemyMoveController_HB01 : EnemyMoveManager
{
    
    // Start is called before the first frame update

   
    private Animator anim;
    
    
    void Start()
    {
        MeeleAttackFXLayer = transform.Find("MeeleAttackFX").gameObject;
        RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");
        ac = GetComponent<EnemyControllerHumanoid>();
        anim = GetComponent<Animator>();
        Behavior = GetComponent<DragaliaEnemyBehavior>();
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
        anim.Play("comboDodge0");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.13f);
        
        ComboDodge1();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));

        currentAttackMove = null;
        
        OnAttackFinished?.Invoke(true);
    }

    /// <summary>
    /// 瞬移攻击
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB01_Action02()
    {
        if (ac.VerticalMoveRoutine != null)
        {
            ac.StopCoroutine(ac.VerticalMoveRoutine);
        }

        yield return new WaitUntil(() => anim.GetBool("isGround"));
        
        ac.OnAttackEnter();
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("warp");
        var animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        
        Disappear(true);
        var dir = LockFaceDir(Behavior.targetPlayer);
        var pos = LockPosition(Behavior.targetPlayer);
        yield return new WaitForSeconds(.2f);
        
        Appear();
        anim.Play("comboDodge1");
        animTime = BasicCalculation.GetLastAnimationNormalizedTime(anim);
        SetGravityScale(0);
        BackWarp(pos,dir,3f,4.5f);
        //pos = LockPosition(Behavior.targetPlayer);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.22f);
        
        ComboDodge2_WarpAttack1(pos);
        SetGravityScale(4);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.272f);

        ComboDodge2_WarpAttack2();
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= animTime);
        
        QuitAttack();

    }

    /// <summary>
    /// 隐身
    /// </summary>
    /// <param name="invincible">是否进入无敌状态</param>
    void Disappear(bool invincible)
    {
        
        var spriteRenderer1 = GetComponent<SpriteRenderer>();
        
        //spriteRenderer;
        spriteRenderer1.color = 
            new Color(spriteRenderer1.color.r, spriteRenderer1.color.g, spriteRenderer1.color.b, 0);

        var miniIcon = transform.Find("MinimapIcon").gameObject;
        miniIcon.SetActive(false);

        if (invincible)
        {
            var col = transform.Find("HitSensor").GetComponent<Collider2D>();
            col.enabled = false;
        }


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
        currentAttackMove = null;
        OnAttackFinished?.Invoke(true);
    }



    void ComboDodge1()
    {
        
        

        GameObject container = Instantiate(attackContainer, transform.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        
        GameObject projectile_clone1 = Instantiate(projectile1, transform.position, transform.rotation, container.transform);

        projectile_clone1.GetComponent<AttackFromEnemy>().firedir = ac.facedir;

        if (ac.facedir == -1)
        {
            var partical = projectile_clone1.GetComponentInChildren<ParticleSystem>().main;
            //ParticleSystem.MinMaxCurve curve;
            partical.startRotationY = new ParticleSystem.MinMaxCurve(Mathf.PI);
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

}
