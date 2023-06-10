using System.Collections;
using CharacterSpecificProjectiles;
using GameMechanics;
using UnityEngine;


public class EnemyMoveController_DB01 : EnemyMoveManager
{
    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerFlyingHigh>();
        
    }

    
    
    /// <summary>
    /// forward wind
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action01()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("charge_enter");
        yield return null;
        
        var hint = GenerateWarningPrefab(WarningPrefabs[0],
            transform.position, ac.facedir == 1? Quaternion.identity : Quaternion.Euler(0,180,0),
            RangedAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBarRect2D>();
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("charge_loop");
        yield return new WaitUntil(()=>hintbar.warningTimeLeft <= 0);
        
        anim.Play("charge_exit");
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1f);

        Action01_GenerateForwardWind();
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();

    }

    /// <summary>
    /// Combo
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action02()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("combo1");
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f);
        
        var container = Action02_ComboMeele();
        
        anim.Play("combo2");
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f);
        
        anim.Play("combo3");
        yield return null;

        yield return new WaitForSeconds(0.5f);
        Action02_ComboRanged(container);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();


    }

    /// <summary>
    /// Around Attack
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action03()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(100);
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,
            transform.position + new Vector3(0,4,0));

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("roar");
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.33f);
        Action03_AroundAttack();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        
        QuitAttack();

    }
    
    /// <summary>
    /// Meaningless roar
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action04()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);

        anim.Play("roar");
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// 5 winds
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action05()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action05");

        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("charge_enter");
        yield return null;

        var container = 
            Action05_GenerateWarning();
        
        var hintbar = container.GetComponentInChildren<EnemyAttackHintBarShine>();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("charge_loop");
        yield return new WaitUntil(()=>hintbar.warningTimeLeft <= 0);
        
        anim.Play("charge_exit");
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1f);

        Action05_ReleaseStart(container);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// storm wall fixed
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action06(int type)
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action06");
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("side_enter");
        
        if(type == 0)
            Action06_SetStormWallFixed();
        else
        {
            Action06_SetStormWallChasing(_behavior.targetPlayer);
        }
        
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("side_loop");

        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("side_exit");

        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();

    }

    /// <summary>
    /// bounce wind
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action07()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action07");
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("charge_enter");
        yield return null;
        
        var hint = GenerateWarningPrefab(WarningPrefabs[2], transform.position + new Vector3(2*ac.facedir, 0, 0),
            ac.facedir == 1? Quaternion.identity : Quaternion.Euler(0,180,0),RangedAttackFXLayer.transform);

        var hintbar = hint.GetComponent<EnemyAttackHintBarShine>();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("charge_loop");
        
        yield return new WaitUntil(()=>hintbar.warningTimeLeft <= 0);
        
        anim.Play("charge_exit");
        Action07_BounceWind();
        yield return null;
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();

    }

    public IEnumerator DB01_Action08()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action08");
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("side_enter");
        yield return new WaitForSeconds(0.5f);
        
        Action08_StormStrikeAttack();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("side_loop");
        
        yield return new WaitForSeconds(0.1f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("side_exit");
        
        yield return new WaitForSeconds(0.1f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();


    }

    /// <summary>
    /// 强风爆发
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action09()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action09");
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("side_enter");
        yield return new WaitForSeconds(0.5f);
        
        Action09_RendingBlasts();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("side_loop");
        
        yield return new WaitForSeconds(0.1f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("side_exit");
        
        yield return new WaitForSeconds(0.1f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();


    }

    /// <summary>
    /// 疾风弹
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action10()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action10");
        StageCameraController.SwitchOverallCamera();
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("side_enter");
        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("side_loop");
        
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("side_exit");
        
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        Action10_GaleBlasts();
        anim.Play("idle");
        Invoke(nameof(Action10_SwitchToMainCamera),6f);
        QuitAttack();


    }

    /// <summary>
    /// SummonHelp
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action11()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action11");
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("charge_enter");
        yield return null;

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("charge_loop");
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("charge_exit");
        yield return new WaitForSeconds(1f);
        
        Action11_SummonHelp();
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// Storm Rush
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action12()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action12");
        StageCameraController.SwitchOverallCamera();
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("side_enter");
        yield return new WaitForSeconds(0.5f);
        Action12_StormRush();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("side_loop");
        
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("side_exit");
        
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        
        anim.Play("idle");
        StageCameraController.SwitchMainCamera();

        QuitAttack();


    }



    protected void Action01_GenerateForwardWind()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);

        var proj = InstantiateRanged(projectile1, container.transform.position,
            container, ac.facedir);

        var windController = proj.GetComponent<DOTweenSimpleController>();
        if (ac.facedir == -1)
        {
            windController.moveDirection.x *= -1;
        }

        var atk = proj.GetComponent<AttackFromEnemy>();
        var stun = new TimerBuff((int)BasicCalculation.BattleCondition.Stun, -1, 7f,1);
        
        
        atk.AddWithConditionAll(stun,50);

    }

    protected GameObject Action02_ComboMeele()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position + new Vector3(2*ac.facedir,0,0),
            Quaternion.identity, RangedAttackFXLayer.transform);

        var proj = InstantiateRanged(projectile2, container.transform.position,
            container, ac.facedir);

        return proj;

    }

    protected void Action02_ComboRanged(GameObject container)
    {
        var proj = InstantiateRanged(projectile3, container.transform.position + new Vector3(0,1.5f,0),
            container, ac.facedir);
        
        var windController = proj.GetComponent<DOTweenSimpleController>();
        windController.moveDirection.y = 1.5f;
        // if (ac.facedir == -1)
        // {
        //     windController.moveDirection.x *= -1;
        // }
    }

    protected void Action03_AroundAttack()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position + new Vector3(0,0,0),
            Quaternion.identity, MeeleAttackFXLayer.transform);

        var proj = InstantiateMeele(projectile4, container.transform.position,
            container);
        
    }

    protected GameObject Action05_GenerateWarning()
    {
        var container = Instantiate(projectile5,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);

        var projController = container.GetComponent<IEnemySealedContainer>();
        projController.SetEnemySource(gameObject);
        
        return container;
    }
    
    protected void Action05_ReleaseStart(GameObject container)
    {
        var projController = container.GetComponent<Projectile_DB001_1>();
        projController.ReleaseAttack();
        
    }

    protected void Action06_SetStormWallFixed()
    {
        var container = 
            Instantiate(projectile6,Vector3.zero,Quaternion.identity, RangedAttackFXLayer.transform);
        var projController = container.GetComponent<IEnemySealedContainer>();
        projController.SetEnemySource(gameObject);
        
    }

    protected void Action06_SetStormWallChasing(GameObject target)
    {
        var container = 
            Instantiate(projectile7,new Vector3(target.transform.position.x,0,0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        var projController = container.GetComponent<Projectile_DB001_3>();
        projController.SetEnemySource(gameObject);
        projController.target = target;
    }

    protected void Action07_BounceWind()
    {
        var container = 
            Instantiate(attackContainer,transform.position,Quaternion.identity, RangedAttackFXLayer.transform);
        
        var proj = InstantiateRanged(projectile8,
            container.transform.position + new Vector3(2*ac.facedir,0,0),
            container, ac.facedir);
        
        
    }

    protected void Action08_StormStrikeAttack()
    {
        var container = Instantiate(projectile9,
            _behavior.targetPlayer.transform.position, Quaternion.identity, RangedAttackFXLayer.transform);

        var projController = container.GetComponentInChildren<Projectile_DB001_5>();
        projController.SetEnemySource(gameObject);
        projController.difficulty = _behavior.difficulty;
    }
    
    protected void Action09_RendingBlasts()
    {
        var container = Instantiate(projectile10,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);

        var projController = container.GetComponentInChildren<Projectile_DB001_6>();
        projController.SetEnemySource(gameObject);
        projController.target = _behavior.targetPlayer;
        
    }

    protected void Action10_GaleBlasts()
    {
        var container = Instantiate(projectilePoolEX[0],
            Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
        container.GetComponent<IEnemySealedContainer>().SetEnemySource(gameObject);
        
    }
    
    protected void Action10_SwitchToMainCamera()
    {
        StageCameraController.SwitchMainCamera();
    }
    
    protected void Action11_SummonHelp()
    {

        var enemyLayer = GameObject.Find("EnemyLayer").transform;

        Instantiate(projectilePoolEX[1], new Vector3(0, 4, 0),
            Quaternion.identity,enemyLayer);

    }

    protected void Action12_StormRush()
    {
        var container = Instantiate(projectilePoolEX[2],
            Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
        
        container.GetComponentInChildren<IEnemySealedContainer>().SetEnemySource(gameObject);
    }


}
