using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_DB05 : EnemyMoveManager
{
    [SerializeField] private List<GameObject> _minions = new();

    protected override void Start()
    {
        base.Start();
        GetAllAnchors();
    }

    public IEnumerator DB05_Action01()
    {
        yield return _canActionOnFlyingGround;
        
        ac.OnAttackEnter();
        
        yield return new WaitForSeconds(0.5f);
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,
            transform.position + new Vector3(0,4f),true);
        
        yield return new WaitForSeconds(0.75f);
        
        anim.Play("slash");
        
        yield return new WaitForSeconds(0.25f);
        
        SlashAttack();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();

    }
    
    /// <summary>
    /// TailAttack
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action02()
    {
        yield return _canActionOnFlyingGround;
        
        ac.OnAttackEnter();
        
        yield return new WaitForSeconds(0.5f);
        
        // BattleEffectManager.Instance.SpawnExclamation(gameObject,
        //     transform.position + new Vector3(0,4f),true);
        //
        // yield return new WaitForSeconds(0.75f);
        
        anim.Play("tail");
        
        yield return new WaitForSeconds(50f/60f);
        
        TailAttack();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();

    }

    /// <summary>
    /// ForwardDash
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action03()
    {
        yield return _canActionOnFlyingGround;
        
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(0.2f);
        
        anim.Play("charge_enter");
        
        var hint = 
            GenerateWarningPrefab
                ("action03",transform.position,
                    Quaternion.identity,RangedAttackFXLayer.transform);
        hint.transform.localScale = new Vector3(ac.facedir,1f,1f);
        
        yield return new WaitForSeconds(1.5f);
        
        anim.Play("charge_exit");
        
        yield return new WaitForSeconds(0.25f);
        
        DashForward();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
        
        
    }
    
    /// <summary>
    /// Poison Sweep Side
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action04()
    {
        yield return _canActionOnFlyingGround;
        
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.1f);
        
        var hint = 
            GenerateWarningPrefab
            ("action04",transform.position + new Vector3(ac.facedir * 3.5f,2.5f),
                Quaternion.identity,RangedAttackFXLayer.transform);
        hint.transform.localScale = new Vector3(ac.facedir,1f,1f);

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("sweep_right");

        yield return new WaitForSeconds(0.55f);
        
        PoisonSide();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        //anim.Play("idle");
        yield return new WaitForSeconds(0.3f);
        
        QuitAttack();
        
        
    }
    
    /// <summary>
    /// 黑暗追逐
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action05()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB05_Action05");
        
        
        anim.Play("roar_1");
        
        
        yield return new WaitForSeconds(1.5f);
        PillarAttack();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 喷毒
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action06()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);


        anim.Play("spit");
        
        
        yield return new WaitForSeconds(1.4f);
        
        PoisonPoolSpit();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }

    public void DB05_Action07()
    {

        SpawnEnemyMinon(_minions[0], new Vector3(-12, -2), 14000 * _behavior.difficulty).
            GetComponent<Projectile_DB005_1>().enemySource = gameObject;
        SpawnEnemyMinon(_minions[0], new Vector3(12, -2), 14000 * _behavior.difficulty).
            GetComponent<Projectile_DB005_1>().enemySource = gameObject;
    }
    
    /// <summary>
    /// Poison Sweep Side
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action08()
    {
        yield return _canActionOnFlyingGround;
        
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        
        //yield return new WaitForSeconds(0.1f);
        
        // var hint = 
        //     GenerateWarningPrefab
        //     ("action04",transform.position + new Vector3(ac.facedir * 3.5f,2.5f),
        //         Quaternion.identity,RangedAttackFXLayer.transform);
        // hint.transform.localScale = new Vector3(ac.facedir,1f,1f);

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("sweep_enter");

        var modelTransform = GetComponentInChildren<AnimationEventSender_Enemy>().transform;
        _tweener = modelTransform.DOLocalRotate(new Vector3(0, 180, 0), 1f);

        yield return new WaitForSeconds(1.55f);
        
        //PoisonSide();
        
        anim.Play("sweep_left_center");
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f);
        
        
        _tweener = modelTransform.DOLocalRotate(new Vector3(0, 102, 0), 0.5f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        //anim.Play("idle");
        yield return new WaitForSeconds(0.3f);
        
        QuitAttack();
        
        
    }
    
    
    
    
    
    private void SlashAttack()
    {
        InstantiateMeele(GetProjectileOfFormatName("action01"), transform.position,
            InitContainer(true));
    }
    
    private void TailAttack()
    {
        InstantiateMeele(GetProjectileOfFormatName("action02"),
            transform.position - new Vector3(ac.facedir*5f,0),
            InitContainer(true));

        var debuff = new TimerBuff((int)BasicCalculation.BattleCondition.Bleeding);
    }

    private void DashForward()
    {
        var endPos =
            BattleStageManager.Instance.OutOfRangeCheck(transform.position 
                                                        + new Vector3(ac.facedir * 21, 0));
        var distance = Mathf.Abs(endPos.x - transform.position.x);
        
        
        var tweenTime = 0.4f * (distance / 21f);
        
        _tweener = transform.DOMoveX(endPos.x, tweenTime).SetEase(Ease.InOutSine);
        
        InstantiateMeele(GetProjectileOfFormatName("action03"), transform.position,
            InitContainer(true)).GetComponent<AttackBase>().firedir = ac.facedir;
        
    }

    private void PoisonSide()
    {
        InstantiateRanged(GetProjectileOfFormatName("action04"), 
            transform.position + new Vector3(3.5f*ac.facedir,2.5f),
            InitContainer(false),ac.facedir);
    }
    
    private void PillarAttack()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action05"),
            _behavior.targetPlayer.RaycastedPosition(),
            InitContainer(false),1);
        
        var atk = proj.GetComponent<ForcedAttackFromEnemy>();
        
        var buff = new TimerBuff((int)BasicCalculation.BattleCondition.Spite,
            1,-1,5,0);
        buff.dispellable = false;
        
        atk.AddWithConditionAll(new TimerBuff(buff),999);
        
        atk.target = _behavior.targetPlayer;
    }

    private void PoisonPoolSpit()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action06"),
            transform.position + new Vector3(ac.facedir * 12.5f,0.25f),
            InitContainer(false),ac.facedir);
    }
    
}
