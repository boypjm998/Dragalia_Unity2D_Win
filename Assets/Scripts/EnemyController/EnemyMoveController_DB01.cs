using System.Collections;
using CharacterSpecificProjectiles;
using GameMechanics;
using UnityEngine;


public class EnemyMoveController_DB01 : EnemyMoveManager
{
    protected VoiceControllerEnemy voice;
    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerFlyingHigh>();
        voice = GetComponentInChildren<VoiceControllerEnemy>();

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
            transform.position, ac.facedir == 1 ? Quaternion.identity : Quaternion.Euler(0, 180, 0),
            RangedAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBarRect2D>();
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("charge_loop");
        yield return new WaitUntil(() => hintbar.warningTimeLeft <= 0);

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
            transform.position + new Vector3(0, 4, 0));

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
        yield return new WaitUntil(() => hintbar.warningTimeLeft <= 0);

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

        if (type == 0)
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

        var hint = GenerateWarningPrefab(WarningPrefabs[2], transform.position + new Vector3(2 * ac.facedir, 0, 0),
            ac.facedir == 1 ? Quaternion.identity : Quaternion.Euler(0, 180, 0), RangedAttackFXLayer.transform);

        var hintbar = hint.GetComponent<EnemyAttackHintBarShine>();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("charge_loop");

        yield return new WaitUntil(() => hintbar.warningTimeLeft <= 0);

        anim.Play("charge_exit");
        Action07_BounceWind();
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("idle");
        QuitAttack();

    }

    /// <summary>
    /// Storm Strike
    /// </summary>
    /// <returns></returns>
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
        Invoke(nameof(Action10_SwitchToMainCamera), 9f);
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

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("charge_loop");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("charge_exit");
        yield return new WaitForSeconds(1f);

        Action11_SummonHelp();
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

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
        Invoke("Action10_SwitchToMainCamera",3.5f);
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

        QuitAttack();


    }

    /// <summary>
    /// Release up wind
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action13()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);

        ac.TurnMove(_behavior.targetPlayer);

        anim.Play("side_enter");
        yield return new WaitForSeconds(0.5f);
        Action13_ReleaseUpwardWind();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("side_loop");

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("side_exit");

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);


        anim.Play("idle");
        //StageCameraController.SwitchMainCamera();

        QuitAttack();
    }

    /// <summary>
    /// GroundSurgingTempest
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action14()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB01_Action14");
        voice.BroadCastMyVoice(4);

        anim.Play("charge_enter");
        yield return new WaitForSeconds(0.5f);
        Action14_SurgingTempestGround();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("charge_loop");

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("charge_exit");

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);


        anim.Play("idle");
        Action14_DisableTornadoUp();
        //StageCameraController.SwitchMainCamera();

        QuitAttack();
    }


    /// <summary>
    /// Summon Leif
    /// </summary>
    public IEnumerator DB01_Action15()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action15");
        voice.BroadCastMyVoice(1);

        anim.Play("roar");
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f);
        Action15_SummonLeif();

        yield return new WaitForSeconds(0.5f);
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff, 60, 10);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");

        QuitAttack();
    }

    public IEnumerator DB01_Action16()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action16");
        voice.BroadCastMyVoice(1);

        anim.Play("roar");
        yield return null;
        Invoke("Action16_SummonMeene", 3);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f);


        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        //Action16_SummonMeene();

        QuitAttack();
    }


    public IEnumerator DB01_Action17()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action17");
        voice.BroadCastMyVoice(1);

        anim.Play("roar");
        yield return null;
        Invoke("Action17_SummonTobias", 3);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f);


        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        //Action17_SummonTobias();

        QuitAttack();
    }

    public IEnumerator DB01_Action18()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action18");
        voice.BroadCastMyVoice(1);

        anim.Play("roar");
        yield return null;
        Invoke("Action18_SummonLathna",3);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f);


        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        //Action18_SummonLathna();

        QuitAttack();
    }

    /// <summary>
    /// SummonMelsa
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action19()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action19");
        voice.BroadCastMyVoice(1);

        anim.Play("roar");
        yield return null;
        Invoke("Action19_SummonMelsa",3);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f);


        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        

        QuitAttack();
    }
    
    /// <summary>
    /// GroundSurgingSky
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action20()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB01_Action14");
        voice.BroadCastMyVoice(4);

        anim.Play("charge_enter");
        yield return new WaitForSeconds(0.5f);
        Action20_SurgingTempestSky();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("charge_loop");

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("charge_exit");

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);


        anim.Play("idle");
        //StageCameraController.SwitchMainCamera();

        QuitAttack();
    }

    /// <summary>
    /// Gale Catastrophe
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_Action21()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("DB01_Action21");
        ac.TurnMove(_behavior.targetPlayer);

        anim.Play("charge_enter");
        yield return null;
        
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(1.5f);

        Action21_GaleCatastrophe();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("charge_loop");

        yield return new WaitForSeconds(4f);

        anim.Play("charge_exit");
        
        yield return null;

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
        var stun = new TimerBuff((int)BasicCalculation.BattleCondition.Stun, -1, 7f, 1);


        atk.AddWithConditionAll(stun, 50);

    }

    protected GameObject Action02_ComboMeele()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position + new Vector3(2 * ac.facedir, 0, 0),
            Quaternion.identity, RangedAttackFXLayer.transform);

        var proj = InstantiateRanged(projectile2, container.transform.position,
            container, ac.facedir);

        return proj;

    }

    protected void Action02_ComboRanged(GameObject container)
    {
        var proj = InstantiateRanged(projectile3, container.transform.position + new Vector3(0, 1.5f, 0),
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
            transform.position + new Vector3(0, 0, 0),
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
            Instantiate(projectile6, Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
        var projController = container.GetComponent<IEnemySealedContainer>();
        projController.SetEnemySource(gameObject);

    }

    protected void Action06_SetStormWallChasing(GameObject target)
    {
        var container =
            Instantiate(projectile7, new Vector3(target.transform.position.x, 0, 0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        var projController = container.GetComponent<Projectile_DB001_3>();
        projController.SetEnemySource(gameObject);
        projController.target = target;
    }

    protected void Action07_BounceWind()
    {
        var container =
            Instantiate(attackContainer, transform.position, Quaternion.identity, RangedAttackFXLayer.transform);

        var proj = InstantiateRanged(projectile8,
            container.transform.position + new Vector3(2 * ac.facedir, 0, 0),
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
            Quaternion.identity, enemyLayer);

    }

    protected void Action12_StormRush()
    {
        var container = Instantiate(projectilePoolEX[2],
            Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);

        container.GetComponentInChildren<IEnemySealedContainer>().SetEnemySource(gameObject);
    }

    protected void Action13_ReleaseUpwardWind()
    {
        var wind = Instantiate(projectilePoolEX[3],
            new Vector3(_behavior.targetPlayer.transform.position.x,
                -1.5f, 0), Quaternion.identity, RangedAttackFXLayer.transform);

        wind.GetComponent<Projectile_DB001_9>().target = _behavior.targetPlayer;

    }

    protected void Action14_SurgingTempestGround()
    {

        var container = Instantiate(attackContainer,
            Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);

        var wind = InstantiateRanged(projectilePoolEX[4],
            Vector3.zero, container, 1);
    }
    
    protected void Action14_DisableTornadoUp()
    {
        var tonados = FindObjectsOfType<Projectile_DB001_9>();
        
        foreach (var tornado in tonados)
        {
            tornado.SetCollisionOff();
        }

    }

    protected void Action15_SummonLeif()
    {


        var fx = InstantiateRanged(projectilePoolEX[5],
            transform.position + new Vector3(0, 3, 0)
            , RangedAttackFXLayer, ac.facedir);



    }

    protected void Action16_SummonMeene()
    {


        var fx = InstantiateRanged(projectilePoolEX[6],
            transform.position+new Vector3(0,2,0)
            , RangedAttackFXLayer, 1);


        if (_behavior.difficulty < 3)
        {
            fx.GetComponent<StatusManager>().baseAtk = 3560;
        }

        
        

    }

    protected void Action17_SummonTobias()
    {
        var targetPlatform = BasicCalculation.CheckRaycastedPlatform(_behavior.targetPlayer);
        var targetPos = targetPlatform.transform.position;
        var target = _behavior.targetPlayer;
        int dir;

        if (target.transform.position.x - targetPlatform.bounds.center.x > 3f)
        {
            targetPos.x = Mathf.Min(target.transform.position.x - 3f, targetPlatform.bounds.max.x);
            dir = 1;
        }
        else if (targetPlatform.bounds.center.x - target.transform.position.x > 3f)
        {
            targetPos.x = Mathf.Max(target.transform.position.x + 3f, targetPlatform.bounds.min.x);
            dir = -1;
        }
        else
        {
            targetPos.x = targetPlatform.bounds.center.x;
            dir = targetPos.x > target.transform.position.x ? 1 : -1;
        }

        targetPos.y = targetPlatform.bounds.max.y + 1.3f;

        


        var fx = InstantiateRanged(projectilePoolEX[7],
            targetPos
            , RangedAttackFXLayer, dir);

        fx.GetComponent<IEnemySealedContainer>().SetEnemySource(gameObject);


    }

    protected void Action18_SummonLathna()
    {
        var targetPlatform = BasicCalculation.CheckRaycastedPlatform(_behavior.targetPlayer);
        var targetPos = targetPlatform.transform.position;
        var target = _behavior.targetPlayer;

        var dir = Random.Range(0, 2) == 0 ? 1 : -1;

        if (targetPlatform.bounds.max.x - targetPlatform.bounds.min.x > 30)
        {
            if (dir == 1)
                targetPos.x = target.transform.position.x - 15;
            else if (dir == -1)
                targetPos.x = target.transform.position.x + 15;
        }
        else if (targetPlatform.bounds.max.x - targetPlatform.bounds.min.x > 20)
        {
            if (target.transform.position.x > targetPlatform.bounds.min.x + 15)
            {
                targetPos.x = target.transform.position.x - 15;
                dir = 1;
            }
            else if (target.transform.position.x < targetPlatform.bounds.max.x - 15)
            {
                targetPos.x = target.transform.position.x + 15;
                dir = -1;
            }
            else
            {
                if (dir == 1)
                    targetPos.x = targetPlatform.bounds.min.x;
                else if (dir == -1)
                    targetPos.x = targetPlatform.bounds.max.x;
            }

        }
        else
        {
            if (target.transform.position.x < targetPlatform.bounds.center.x)
            {
                targetPos.x = targetPlatform.bounds.max.x;
                dir = -1;
            }
            else
            {
                targetPos.x = targetPlatform.bounds.min.x;
                dir = 1;
            }
        }



        targetPos.y = targetPlatform.bounds.max.y + 1.3f;




        var fx = InstantiateRanged(projectilePoolEX[8],
            targetPos
            , RangedAttackFXLayer, dir);

        fx.GetComponent<EnemySealedContainer>().SetEnemySource(gameObject);
        fx.GetComponent<EnemySealedContainer>().SetFireDir(dir);


    }

    protected void Action19_SummonMelsa()
    {
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3f);
        var targetPlatform = BasicCalculation.CheckRaycastedPlatform(_behavior.targetPlayer);
        var targetPos = targetPlatform.bounds.center;
        var target = _behavior.targetPlayer;
        
        var minon = InstantiateRanged(projectilePoolEX[9],
            targetPos + new Vector3(0, 2, 0)
            , RangedAttackFXLayer, target.transform.position.x < targetPlatform.bounds.center.x ? -1 : 1);
            
        BattleEffectManager.Instance.SpawnExclamation(minon,minon.transform.position + new Vector3(0,2f,0));

        //fx.GetComponent<EnemySealedContainer>().SetEnemySource(gameObject);
    }
    
    protected void Action20_SurgingTempestSky()
    {

        var container = Instantiate(attackContainer,
            Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);

        var wind = InstantiateRanged(projectilePoolEX[10],
            new Vector3(0,29,0), container, 1);
    }
    
    protected void Action21_GaleCatastrophe()
    {
        //生成一个随机数进行四选一
        var rand = Random.Range(0, 4);
        //根据随机数，实例化projectilePoolEX中对应序号的预制体
        var wind = Instantiate(projectilePoolEX[11+rand],
            Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
        wind.GetComponent<IEnemySealedContainer>().SetEnemySource(gameObject);
        
    }


}
