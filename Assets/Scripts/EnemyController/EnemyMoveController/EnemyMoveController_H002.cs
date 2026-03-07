using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

/// <summary>
/// Lilith
/// </summary>
public class EnemyMoveController_H002 : EnemyMoveManager
{
    private VoiceControllerEnemy _voiceControllerEnemy;
    [SerializeField] private GameObject candyRenderer;
    [SerializeField] private GameObject weaponRenderer;

    [SerializeField] private GameObject markFXPrefab;
    [SerializeField] private GameObject partBreakFXPrefab;
    
    private Vector4 hitBoxInfo = new(2.5f, 4f, -0.25f, 0);
    private Vector4 wingBoxInfo = new(4.5f, 4f, 4.5f, 1f);
    
    [SerializeField] private GameObject partRenderer;
    
    



    enum MyVoiceGroup
    {
        Intro = 0,
        HPBelow70 = 1,
        HPBelow40 = 2,
        Defeat = 3,
        Transform = 4,
        Torture = 5,
        Buff = 6,
        Smash = 7,
        Candy = 8,
        Torture2 = 9,
        Interrupted = 10,
        CrossRush = 11,
    }

    protected override void Awake()
    {
        base.Awake();
        _voiceControllerEnemy = GetComponentInChildren<VoiceControllerEnemy>();
    }

    protected override void Start()
    {
        base.Start();
        (_statusManager as SpecialStatusManager).onBreak += () => SetCandyRenderer(true);
        SetCandyRenderer(false);
        var hitsensor = ac.HitSensor as BoxCollider2D;
        hitBoxInfo = new Vector4(hitsensor.size.x, hitsensor.size.y, hitsensor.offset.x, hitsensor.offset.y);
    }
    
    private void ToWingHitBox()
    {
        var hitsensor = ac.HitSensor as BoxCollider2D;
        hitsensor.size = new Vector2(wingBoxInfo.x, wingBoxInfo.y);
        hitsensor.offset = new Vector2(wingBoxInfo.z, wingBoxInfo.w);
        (_behavior as H002_BehaviorTree).SetWingFakeActive();
    }
    
    private void ToNormalHitBox()
    {
        var hitsensor = ac.HitSensor as BoxCollider2D;
        hitsensor.size = new Vector2(hitBoxInfo.x, hitBoxInfo.y);
        hitsensor.offset = new Vector2(hitBoxInfo.z, hitBoxInfo.w);
        (_behavior as H002_BehaviorTree).SetWingFakeActive(false);
    }

    /// <summary>
    /// 病态甜蜜
    /// </summary>
    /// <returns></returns>
    public IEnumerator H002_Action01(int effect)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H002_Action01");
        
        anim.Play("side_1");
        SetCandyRenderer(false);
        if(_voiceControllerEnemy != null)
            _voiceControllerEnemy.BroadCastMyVoice(5);
        
        yield return new WaitForSeconds(1.5f);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("side_3");

        yield return new WaitForSeconds(0.3f);
        
        SweetStockade(effect);
        
        yield return new WaitForSeconds(0.7f);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("side_5");

        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        SetCandyRenderer(true);
        anim.Play("idle");
        
        QuitAttack();
    }

    public IEnumerator H002_Action02()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position+new Vector3(0,5),true);
        
        anim.Play("swing_enter_right");
        SetCandyRenderer(false);

        yield return new WaitForSeconds(1f);
        anim.Play("swing_exit_right");

        yield return new WaitForSeconds(0.2f);
        
        ac.SetCounter(true);
        ac.currentKBRes = 100;
        InstantiateMeele(GetProjectileOfFormatName("action02_1"),
            transform.position,InitContainer(true)).
            GetComponent<AttackBase>().AddMeeleTimeStopEffect(0.25f,0.05f);
        
        
        
        yield return new WaitForSeconds(1f);
        anim.Play("swing_exit_left");
        
        yield return new WaitForSeconds(0.2f);
        InstantiateMeele(GetProjectileOfFormatName("action02_2"),
            transform.position,InitContainer(true)).
            GetComponent<AttackBase>().AddMeeleTimeStopEffect(0.25f,0.05f);;
        
        yield return new WaitForSeconds(1.25f);
        anim.Play("charge_exit");

        yield return new WaitForSeconds(0.15f);
        InstantiateMeele(GetProjectileOfFormatName("action02_3"),
            transform.position, InitContainer(true)).
            GetComponent<AttackBase>().AddMeeleTimeStopEffect(0.3f,0.05f);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        
        SetCandyRenderer(true);
        anim.Play("idle");
        
        QuitAttack();
    }

    /// <summary>
    /// 饴晶陨落
    /// </summary>
    /// <returns></returns>
    public IEnumerator H002_Action03()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H002_Action03");

        yield return new WaitForSeconds(1);
        
        SetCandyRenderer(false);
        anim.Play("side_1");

        yield return new WaitForSeconds(0.8f);
        
        Invoke("SweetShower",0.2f);
        Invoke("SweetShower",1.5f);
        Invoke("SweetShower",2.8f);

        yield return new WaitForSeconds(1.5f);
        
        anim.Play("side_3");
        
        yield return new WaitForSeconds(0.75f);
        
        anim.Play("side_5");
        
        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        SetCandyRenderer(true);
        anim.Play("idle");
        QuitAttack();


    }
    
    
    /// <summary>
    /// 虚无
    /// </summary>
    /// <returns></returns>
    public IEnumerator H002_Action04()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB12_Action02");
        
        anim.Play("side_1");
        SetCandyRenderer(false);

        yield return new WaitForSeconds(1.5f);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("side_3");

        yield return new WaitForSeconds(0.3f);
        
        Nihility();
        
        yield return new WaitForSeconds(0.7f);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("side_5");

        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        SetCandyRenderer(true);
        anim.Play("idle");
        
        QuitAttack();
    }
    
    
    /// <summary>
    /// 侵蚀
    /// </summary>
    /// <returns></returns>
    public IEnumerator H002_Action05(int effect)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB12_Action01");
        
        anim.Play("side_1");
        SetCandyRenderer(false);
        if(_voiceControllerEnemy != null)
            _voiceControllerEnemy.BroadCastMyVoice((int)MyVoiceGroup.Torture);
        
        yield return new WaitForSeconds(1.5f);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("side_3");

        yield return new WaitForSeconds(0.3f);
        
        Corrosion(effect);
        
        yield return new WaitForSeconds(0.7f);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("side_5");

        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        SetCandyRenderer(true);
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// Dash
    /// </summary>
    /// <returns></returns>
    public IEnumerator H002_Action06()
    {
        ac.SetGravityScale(0);
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        
        anim.Play("dash_1");
        SetCandyRenderer(false);
        
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            transform.position + new Vector3(-2*ac.facedir,2f),
            RangedAttackFXLayer.transform,new Vector2(24,5),Vector2.zero,false,
            0,2,ac.facedir==1?0:180,1f,true,true);

        yield return new WaitForSeconds(0.8f);
        
        var fx = DashAttack();
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("dash_3");
        
        yield return new WaitForSeconds(0.25f);

        var duration = DashAttackMove(fx);
        
        yield return new WaitForSeconds(duration - 0.1f);

        anim.Play("dash_5");

        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        SetCandyRenderer(true);
        anim.Play("idle");
        QuitAttack();
    }
    
    public IEnumerator H002_Action07(int buffEffect)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        anim.Play("buff");
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoiceGroup.Buff);
        SetCandyRenderer(false);

        yield return new WaitForSeconds(0.85f);

        Instantiate(GetProjectileOfFormatName("action07"),
            transform.position - new Vector3(0, 0.5f), Quaternion.identity,
            RangedAttackFXLayer.transform);
        _statusManager.ObtainTimerBuff(1, buffEffect, 30, 100, -1);
        _statusManager.ObtainTimerBuff(1, buffEffect, 30, 100, -1);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        
        SetCandyRenderer(true);
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// smash
    /// </summary>
    /// <param name="purpleFirst"></param>
    /// <returns></returns>
    public IEnumerator H002_Action08(bool purpleFirst)
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);

        anim.Play("smash_1");//64帧
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoiceGroup.Smash);
        SetCandyRenderer(false);

        var currentGround = gameObject.RaycastedPlatform();
        if (transform.position.x + 3 > currentGround.bounds.max.x)
        {
            ac.SetFaceDir(-1);
        }
        else if (transform.position.x - 3 < currentGround.bounds.min.x)
        {
            ac.SetFaceDir(1);
        }

        var hint1 = SmashDownFirst(purpleFirst);
        var pos = hint1.transform.position;
        
        yield return new WaitForSeconds(1.99f);
        
        anim.Play("smash_3");
        
        yield return new WaitForSeconds(0.44f);

        if ((_behavior as H002_BehaviorTree).WingIsAlive)
        {
            Instantiate(markFXPrefab,transform.position + new Vector3(ac.facedir * 4.5f,1),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }
        SmashdownCenter(purpleFirst,pos);

        yield return new WaitForSeconds(1.2f);

        var hint2 = SmashDownSecond(purpleFirst);
        pos = hint2.transform.position;
        
        yield return new WaitForSeconds(1.35f);
        
        SmashdownCenter(!purpleFirst,pos);

        yield return new WaitForSeconds(3.5f);
        
        anim.Play("smash_5");

        yield return null;

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);

        SetCandyRenderer(true);
        anim.Play("idle");
        
        QuitAttack();
    }


    public IEnumerator H002_Action09(bool redIsSafe = true)
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H002_Action09");
        
        StageCameraController.SwitchOverallCamera();
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoiceGroup.Candy);
        
        yield return new WaitForSeconds(1);
        
        anim.Play("charge_1");
        SetCandyRenderer(false);

        PrepareCrossingCandies(redIsSafe);
        yield return new WaitForSeconds(3);
        
        anim.Play("charge_3");

        yield return new WaitForSeconds(1f);
        
        anim.Play("charge_5");

        yield return null;
        
        StageCameraController.SwitchMainCamera();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);

        anim.Play("idle");
        SetCandyRenderer(true);
        
        QuitAttack();

    }

    /// <summary>
    /// 如蜜似饯
    /// </summary>
    /// <returns></returns>
    public IEnumerator H002_Action10()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H002_Action10");
        
        StageCameraController.SwitchOverallCamera();
        //_voiceControllerEnemy?.BroadCastMyVoice((int)MyVoiceGroup.Candy);

        yield return new WaitForSeconds(2);
        
        anim.Play("charge_1");
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoiceGroup.Torture2);
        SetCandyRenderer(false);
        
        var shineFlashPrefab = GetProjectileOfFormatName("action10_3",true);
        
        Instantiate(shineFlashPrefab,transform
            .position + new Vector3(-ac.facedir * 2,0),Quaternion.identity,RangedAttackFXLayer.transform);
        weaponRenderer.SetActive(false);
        
        yield return new WaitForSeconds(0.1f);

        var weaponInfo = SetFirstWeapon();

        var wing = (_behavior as H002_BehaviorTree).partWing;
        wing.transform.position = weaponInfo.trueWeapon;
        
        yield return new WaitForSeconds(0.1f);
        
        Instantiate(shineFlashPrefab,
            wing.transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        
        yield return new WaitForSeconds(0.1f);
        
        wing.gameObject.SetActive(true);
        
        var fakeWingPrefab = GetProjectileOfFormatName("action10_1",true);
        List<GameObject> fakeWingInstances = new();

        foreach (var pos in weaponInfo.fakeWeapons)
        {
            Instantiate(shineFlashPrefab,pos,Quaternion.identity,RangedAttackFXLayer.transform);
        }
        
        yield return new WaitForSeconds(0.1f);
        
        foreach (var pos in weaponInfo.fakeWeapons)
        {
            fakeWingInstances.Add(Instantiate(fakeWingPrefab,
                pos,Quaternion.identity,BattleStageManager.Instance.EnemyLayer.transform));
        }
        
        int pillarDir = Random.Range(0,2) == 1 ? 1 : -1;
        DOVirtual.DelayedCall(4, () => FirePillars(pillarDir),false);
        
        yield return new WaitForSeconds
            (SetWeaponGroupHints(weaponInfo.positionList, weaponInfo.trueID) - 0.3f);
        
        anim.Play("charge_3");

        yield return new WaitForSeconds(0.3f);

        foreach (var fake in fakeWingInstances)
        {
            Destroy(fake,0.5f);
        }
        
        
        yield return new WaitForSeconds(1.5f);

        FirePillars(-pillarDir);

        StageCameraController.SwitchMainCamera();
        
        yield return new WaitForSeconds(3f);
        
        anim.Play("charge_5");
        Instantiate(shineFlashPrefab,
            wing.transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        
        yield return new WaitForSeconds(0.1f);
        
        wing.gameObject.SetActive(false);
        Instantiate(shineFlashPrefab,transform
            .position + new Vector3(-ac.facedir * 2,0),Quaternion.identity,RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(0.1f);
        
        weaponRenderer.SetActive(true);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        SetCandyRenderer(true);
        QuitAttack();

    }


    /// <summary>
    /// 寻欢蜜意
    /// </summary>
    /// <param name="center"></param>
    /// <returns></returns>
    public IEnumerator H002_Action11(bool center)
    {
        yield return _canAction;
        ac.OnAttackEnter(999); 
        bossBanner?.PrintSkillName("H002_Action11");
        ac.SetFaceDir(transform.position.x < 0 ? 1 : -1);
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoiceGroup.CrossRush);
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(1);
        
        var shineFlashPrefab = GetProjectileOfFormatName("action10_3",true);
        Instantiate(shineFlashPrefab,transform
            .position + new Vector3(-ac.facedir * 2,0),Quaternion.identity,RangedAttackFXLayer.transform);
        
        
        yield return new WaitForSeconds(0.1f);
        
        weaponRenderer.SetActive(false);
        var wing = (_behavior as H002_BehaviorTree).partWing;
        wing.transform.position = new Vector3(-transform.position.x, 2f);
        
        Instantiate(shineFlashPrefab,
            wing.transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        
        yield return new WaitForSeconds(0.1f);
        
        wing.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        
        anim.Play("dash_1");
        ac.SetGravityScale(0);

        yield return null;
        
        SetCandyRenderer(false);
        
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            transform.position + new Vector3(-2*ac.facedir,2f),
            RangedAttackFXLayer.transform,new Vector2(24,5),Vector2.zero,false,
            0,2, ac.facedir == 1 ? 0 : 180,
            1f,true,true);

        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            wing.transform.position + new Vector3(2*ac.facedir,2f),
            RangedAttackFXLayer.transform,new Vector2(24,5),Vector2.zero,false,
            0,2,ac.facedir == 1 ? 180 : 0,
            1f,true,true);
        
        
        yield return new WaitForSeconds(0.8f);
        
        var fx = DashAttack();
        var fxWand = DashAttackWand(wing);

        yield return new WaitForSeconds(1f);
        
        anim.Play("dash_3");
        
        yield return new WaitForSeconds(0.25f);

        var duration = DashAttackMove(fx);
        DashAttackMoveWand(wing, fxWand, -ac.facedir);

        yield return new WaitForSeconds(duration - 0.1f);

        anim.Play("dash_5");
        
        yield return new WaitForSeconds(0.1f + 0.7f - duration);

        yield return null;
        
        //todo: 第二冲刺
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");

        var secondPosition = new Vector2(3f, 6.5f);
        if (!center)
        {
            secondPosition.x = 10.5f;
        }
        if(transform.position.x > 0)
        {
            secondPosition.x = -secondPosition.x;
        }
        
        Vector2 selfPosition = transform.position;
        Vector2 wingPosition = wing.transform.position;
        
        EnemyController wingAc = wing.GetComponent<EnemyController>();
        
        ac.SetFaceDir(-ac.facedir);

        yield return null;
        
        var selfAngle = selfPosition.AngleDegree(secondPosition);
        var weaponAngle = wingPosition.AngleDegree(new Vector2(-secondPosition.x,secondPosition.y));
        
        var selfDistance = Vector2.Distance(selfPosition, secondPosition);
        var wingDistance = Vector2.Distance(wingPosition, new Vector2(-secondPosition.x,secondPosition.y));

        // var hint1 = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
        //     (Vector3)selfPosition + 
        //    new Vector3(-ac.facedir * 2,2),
        //     MeeleAttackFXLayer.transform,new Vector2(selfDistance + 4,5),
        //     Vector2.zero,false,
        //     0,2, selfAngle,
        //     1f,true,true);
        //
        // var hint2 = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
        //     (Vector3)wingPosition + new Vector3(wingAc.facedir * 2,2),
        //     wingAc.transform,new Vector2(wingDistance + 4,5),
        //     Vector2.zero,false,
        //     0,2, weaponAngle,
        //     1f,true,false);
        //
        // hint1.transform.eulerAngles = new Vector3(0,0,selfAngle);
        // hint2.transform.eulerAngles = new Vector3(0,0,weaponAngle);
        
        yield return new WaitForSeconds(0.8f);
        
        anim.Play("dash_1");
        
        fx = DashAttack(gameObject,Vector3.zero);
        fx.transform.eulerAngles = new Vector3(0,0,selfAngle);
        
        fxWand = DashAttack(wing,Vector3.zero);
        fxWand.transform.eulerAngles = new Vector3(0,0,weaponAngle);
        
        //yield return new WaitForSeconds(1f);
        
        
        
        yield return new WaitForSeconds(1.25f);

        duration = DashAttackMove(gameObject, fx, secondPosition);
        DashAttackMove(wing, fxWand, new Vector2(-secondPosition.x,secondPosition.y));
        wingAc.anim.Play("spin");
        anim.Play("dash_3");
        
        yield return new WaitForSeconds(duration - 0.1f);
        
        
        
        yield return new WaitForSeconds(0.1f + 0.7f - duration);
        
        anim.Play("dash_5");

        yield return null;
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        //todo: Blast

        yield return new WaitForSeconds(1f);
        
        ac.TurnMove(Vector3.zero);
        anim.Play("buff");
        
        yield return new WaitForSeconds(0.3f);
        
        SuperRingBlast(wing,gameObject);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        SetCandyRenderer(true);

        Instantiate(shineFlashPrefab,
            wing.transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        
        yield return new WaitForSeconds(0.1f);
        
        wing.gameObject.SetActive(false);
        Instantiate(shineFlashPrefab,transform
            .position + new Vector3(-ac.facedir * 2,0),Quaternion.identity,RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(0.1f);
        
        weaponRenderer.SetActive(true);
        
        StageCameraController.SwitchMainCamera();
        ac.ResetGravityScale();
        QuitAttack();
        
    }

    public IEnumerator H002_Action12()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("side_1");
        SetCandyRenderer(false);

        var shineFlashPrefab = GetProjectileOfFormatName("action10_3", true);
        
        Instantiate(shineFlashPrefab,transform.position,
            Quaternion.identity,RangedAttackFXLayer.transform);
        
        yield return new WaitForSeconds(0.1f);

        var candyInfo = PrepareBouncingCandies(new Vector3(ac.facedir,1f));
        var candies = candyInfo.candies;

        yield return new WaitForSeconds(1f);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("side_3");

        yield return new WaitForSeconds(0.2f);
        
        for (int i = 0; i < 8; i ++)
        {
            if(i % 2 == 1)
                continue;
            
            var candy = candies[i];
            candy.GetComponent<Collider2D>().enabled = true;
            candy.GetComponent<ReflectionProjectile>().
                SetVelocity(
                    new Vector2(6*Mathf.Sin(candyInfo.angles[i]),6*Mathf.Cos(candyInfo.angles[i])));
        }
        
        yield return new WaitForSeconds(1f);
        
        for (int i = 0; i < 8; i ++)
        {
            if(i % 2 != 1)
                continue;
            
            var candy = candies[i];
            candy.GetComponent<Collider2D>().enabled = true;
            candy.GetComponent<ReflectionProjectile>().
                SetVelocity(
                    new Vector2(6*Mathf.Sin(candyInfo.angles[i]),6*Mathf.Cos(candyInfo.angles[i])));
        }

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("side_5");

        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        SetCandyRenderer(true);
        anim.Play("idle");
        
        QuitAttack();
    }


    /// <summary>
    /// smash2
    /// </summary>
    public IEnumerator H002_Action13()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(100);
        //ac.TurnMove(_behavior.targetPlayer);

        anim.Play("smash_1");//64帧
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoiceGroup.Smash);
        SetCandyRenderer(false);

        yield return new WaitForSeconds(1.5f);
        
        anim.Play("smash_3");
        
        yield return new WaitForSeconds(0.44f);
        
        GroundSmashLoop();
        if ((_behavior as H002_BehaviorTree).WingIsAlive)
        {
            Instantiate(markFXPrefab,transform.position + new Vector3(ac.facedir * 4.5f,1),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }
        
        yield return new WaitForSeconds(5f);
        
        anim.Play("smash_5");

        yield return null;

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);

        SetCandyRenderer(true);
        anim.Play("idle");
        
        QuitAttack();
    }




    public IEnumerator PartBreak()
    {
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        StageCameraController.SwitchMainCamera();
        StageCameraController.SwitchMainCameraFollowObject(gameObject);
        
        yield return new WaitForSeconds(1.5f);
        
        _behavior.breakable = false;
        _statusManager.ImmuneToAllControlAffliction = true;
        anim.Play("part_break");
        //ac.OnHurtEnter();
        BattleEffectManager.Instance.PlayReviveSoundEffect();
        
        
        Instantiate(partBreakFXPrefab,transform.position + new Vector3(-ac.facedir*2,3.5f),
            Quaternion.identity,RangedAttackFXLayer.transform);

        _statusManager.baseDef = 12;
        (_statusManager as SpecialStatusManager).counterModifier = 0.2f;
        DecreaseResistances();

        partRenderer.SetActive(false);
        (_behavior as H002_BehaviorTree).ReduceBuffEffect();
        (_behavior as H002_BehaviorTree).partWing.GetComponent<EnemyController>().
            rendererObject.SetActive((false));

        yield return new WaitForSeconds(0.1f);
        
        _voiceControllerEnemy?.BroadCastMyVoice((int)MyVoiceGroup.Interrupted);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);

        yield return new WaitForSeconds(1f);
        
        _behavior.breakable = true;
        _statusManager.ImmuneToAllControlAffliction = false;
        
        QuitAttack();

    }

    private void SetWeaponRenderer(bool active)
    {
        weaponRenderer.SetActive(active);
    }
    
    private void SetCandyRenderer(bool active)
    {
        candyRenderer.SetActive(active);
    }
    
    private void SweetStockade(float healNeeded)
    {
        var fx = InstantiateRanged(GetProjectileOfFormatName("action01"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,_behavior.viewerPlayer.transform.position.y-1),
            InitContainer(false),1);

        var corrosionEff = new AdvancedTimerBuff((int)BasicCalculation.BattleCondition.Corrosion,
            healNeeded, 8, 0, -1, 1, -1,8);
        
        var atk = fx.GetComponent<AttackFromEnemy>();
        
        atk.AddWithConditionAll(corrosionEff,200,1);
        
        var nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, 30, 1);
        
        var atkdebuff = new TimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff,
            30, 30, 100);
        atkdebuff.dispellable = false;
        
        atk.AddWithConditionAll(nihilDebuff,100);
        atk.AddWithConditionAll(atkdebuff,100,2);

        atk.BeforeAttackHit += PurgedShapeShiftingOfTarget;

    }

    private void Nihility()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action04"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,
                transform.position.y,ac.ModelDepth),InitContainer(false),1);

        var nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, 30, 1);
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(nihilDebuff,100);
    }

    private void Corrosion(int healNeeded)
    {
        var fx = InstantiateRanged(GetProjectileOfFormatName("action05"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,_behavior.viewerPlayer.transform.position.y-1),
            InitContainer(false),1);

        var corrosionEff = new AdvancedTimerBuff((int)BasicCalculation.BattleCondition.Corrosion,
            healNeeded, 8, 0, -1, 1, -1,8);
        
        fx.GetComponent<AttackFromEnemy>().AddWithConditionAll(corrosionEff,200);
    }

    private void SweetShower()
    {
        var targetPlatform = _behavior.targetPlayer.RaycastedPlatform();
        
        var targetPos = new Vector3(_behavior.targetPlayer.transform.position.x,
            targetPlatform.bounds.max.y);

        

        GameObject hintbar = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, targetPos,
            RangedAttackFXLayer.transform, new Vector2(7, 4), Vector2.zero, false,
            1, 2, 90, 1, true, false);

        EnemyAttackHintBarTopDownChaser chaser = hintbar.AddComponent<EnemyAttackHintBarTopDownChaser>();
        
        chaser.target = _behavior.targetPlayer;
        chaser.SetMoveSpeedX(20);
        chaser.SetLockTime(1.25f);
        chaser.SetHardLock(false);
        chaser.SetUseCastPlatformY(true);
        

        DOVirtual.DelayedCall(2.05f, () =>
        {
            var fx = InstantiateRanged(GetProjectileOfFormatName("action03_1"),
                chaser.transform.position, InitContainer(false), 1);
            fx.GetComponent<AttackFromEnemy>().
                AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Stun,
                    1,Random.Range(3f,5f),1),100);
            
        },false);
        
        DOVirtual.DelayedCall(2.35f, () =>
        {
            var fx = InstantiateRanged(GetProjectileOfFormatName("action03_2"),
                chaser.transform.position, InitContainer(false), 1);
            
            fx.GetComponentInChildren<GroundTrap>().SetCollider(chaser!=null?chaser.RaycastedPlatform:targetPlatform);
            
        },false);
        
    }

    private GameObject DashAttack()
    {
        var fx = InstantiateMeele(GetProjectileOfFormatName("action06"),
            transform.position,InitContainer(true));
        return fx;
    }
    
    private GameObject DashAttackWand(GameObject wand)
    {
        var fx = InstantiateRanged(GetProjectileOfFormatName("action06"),
            wand.transform.position,InitContainer(false),wand.transform.localScale.x > 0 ? 1 : -1);
        fx.AddComponent<RelativePositionRetainer>().SetParent(wand.transform);
        return fx;
    }

    private GameObject DashAttack(GameObject obj, Vector3 offset)
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            Vector3.zero, Quaternion.identity, obj.transform);
        var fx = Instantiate(GetProjectileOfFormatName("action11_1"),
            obj.transform.position+new Vector3(0,2),Quaternion.identity,
            container.transform);
        
        fx.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        
        //fx.AddComponent<RelativePositionRetainer>().SetParent(obj.transform);
        return fx;

    }
    private float DashAttackMove(GameObject atk)
    {
        
        var targetPosition = (transform.position + new Vector3(20*ac.facedir,0)).
            SafePosition(Vector2.zero);

        var distance = Mathf.Abs(targetPosition.x - transform.position.x);
        var trueDuration = distance / 45;
        
        atk.GetComponent<Collider2D>().enabled = true;
        (ac as EnemyControllerFlyingHigh).moveEnable = false;
        _tweener = ac.rigid.DOMoveX(targetPosition.x, trueDuration).
            SetEase(Ease.InOutSine).OnComplete(() => {
                (ac as EnemyControllerFlyingHigh).moveEnable = true;
                atk.GetComponent<Collider2D>().enabled = false;
            })
            .OnKill(() => {
                (ac as EnemyControllerFlyingHigh).moveEnable = true;
                atk.GetComponent<Collider2D>().enabled = false; 
            });

        return trueDuration;
    }

    private float DashAttackMoveWand(GameObject wand, GameObject atk, int faceDir)
    {
        var wandAc = wand.GetComponent<EnemyControllerFlying>();
        
        wandAc.SetFaceDir(faceDir);
        wandAc.anim.Play("spin");
        
        var targetPosition = (wand.transform.position + new Vector3(20*wandAc.facedir,0)).
            SafePosition(Vector2.zero);

        var distance = Mathf.Abs(targetPosition.x - wand.transform.position.x);
        var trueDuration = distance / 45;
        
        atk.GetComponent<Collider2D>().enabled = true;
        wandAc.moveEnable = false;
        wandAc.SetTweener(wandAc.rigid.DOMoveX(targetPosition.x, trueDuration).
            SetEase(Ease.InOutSine).OnComplete(() => {
                wandAc.moveEnable = true;
                atk.GetComponent<Collider2D>().enabled = false;
            })
            .OnKill(() => {
                wandAc.moveEnable = true;
                atk.GetComponent<Collider2D>().enabled = false; 
            }));
        return 0;
    }

    private float DashAttackMove(GameObject obj, GameObject atk, Vector2 endPos, bool isWand =false)
    {
        var wandAc = obj.GetComponent<EnemyControllerFlying>();
        
        wandAc.TurnMove(endPos);
        if (isWand)
        {
            wandAc.anim.Play("spin");
        }

        var targetPosition = endPos.
            SafePosition(Vector2.zero);

        var distance = Mathf.Abs(targetPosition.x - obj.transform.position.x);
        var trueDuration = distance / 45;
        
        atk.GetComponent<Collider2D>().enabled = true;
        wandAc.moveEnable = false;
        wandAc.SetTweener(wandAc.rigid.DOMove(targetPosition, trueDuration).
            SetEase(Ease.InOutSine).OnComplete(() => {
                wandAc.moveEnable = true;
                atk.GetComponent<Collider2D>().enabled = false;
            })
            .OnKill(() => {
                wandAc.moveEnable = true;
                atk.GetComponent<Collider2D>().enabled = false; 
            }));
        
        return trueDuration;
    }
    
    
    

    private GameObject SmashDownFirst(bool purpleFirst)
    {
        //var redPrefab = GetProjectileOfFormatName("action08_1");
        //var purplePrefab = GetProjectileOfFormatName("action08_2");
        
        var redRangedPrefab = GetProjectileOfFormatName("action08_3");
        var purpleRangedPrefab = GetProjectileOfFormatName("action08_4");
        
        var startPosition = (Vector3)gameObject.RaycastedPosition() + new Vector3(ac.facedir*3f,0);
        
        var centerHint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                startPosition,
                RangedAttackFXLayer.transform, new Vector2(14,8),Vector2.zero,(!purpleFirst) || (_behavior as H002_BehaviorTree).WingIsAlive==false,
                1, 2.43f, 90, 1f, true,true).
            GetComponent<EnemyAttackHintBarRect2D>().SetDoScale(1);

        if (purpleFirst)
        {
            var leftPosition = startPosition + new Vector3(-10.0f, 0);
            
            if (leftPosition.x < BattleStageManager.Instance.mapBorderL)
            {
                leftPosition.y = new Vector2
                (BattleStageManager.Instance.mapBorderL + 0.1f,
                    leftPosition.y).RaycastedPlatform().bounds.max.y;
            }
            else
            {
                leftPosition.y = ((Vector2)leftPosition).RaycastedPlatform().bounds.max.y;
            }
            
            var rightPosition = startPosition + new Vector3(10.0f, 0);
            
            if (rightPosition.x > BattleStageManager.Instance.mapBorderR)
            {
                rightPosition.y = new Vector2
                (BattleStageManager.Instance.mapBorderR - 0.1f,
                    rightPosition.y).RaycastedPlatform().bounds.max.y;
            }
            else
            {
                rightPosition.y = ((Vector2)rightPosition).RaycastedPlatform().bounds.max.y;
            }
            
            
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    leftPosition,
                    RangedAttackFXLayer.transform, new Vector2(14,7),Vector2.zero,true,
                    1, 2.43f, 90, 1f, true,false).
                GetComponent<EnemyAttackHintBarRect2D>().SetDoScale(1);
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    rightPosition,
                    RangedAttackFXLayer.transform, new Vector2(14,7),Vector2.zero,true,
                    1, 2.43f, 90, 1f, true,false).
                GetComponent<EnemyAttackHintBarRect2D>().SetDoScale(1);

            DOVirtual.DelayedCall(2.5f, () =>
            {
                var container = InitContainer(false);
                var fx1 = InstantiateRanged(redRangedPrefab,
                    leftPosition,container, 1);
                var fx2 = InstantiateRanged(redRangedPrefab,
                    rightPosition, container, 1);
                
            }, false);

        }
        else
        {
            var leftPosition = startPosition + new Vector3(-10.0f, 0);
            
            if (leftPosition.x < BattleStageManager.Instance.mapBorderL)
            {
                leftPosition.y = new Vector2
                (BattleStageManager.Instance.mapBorderL + 0.1f,
                    leftPosition.y).RaycastedPlatform().bounds.max.y;
            }
            else
            {
                leftPosition.y = ((Vector2)leftPosition).RaycastedPlatform().bounds.max.y;
            }
            
            var rightPosition = startPosition + new Vector3(10.0f, 0);
            
            if (rightPosition.x > BattleStageManager.Instance.mapBorderR)
            {
                rightPosition.y = new Vector2
                (BattleStageManager.Instance.mapBorderR - 0.1f,
                    rightPosition.y).RaycastedPlatform().bounds.max.y;
            }
            else
            {
                rightPosition.y = ((Vector2)rightPosition).RaycastedPlatform().bounds.max.y;
            }
            
            
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    leftPosition,
                    RangedAttackFXLayer.transform, new Vector2(14,7),Vector2.zero,false,
                    1, 2.43f, 90, 1f, true,false).
                GetComponent<EnemyAttackHintBarRect2D>().SetDoScale(1);
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    rightPosition,
                    RangedAttackFXLayer.transform, new Vector2(14,7),Vector2.zero,false,
                    1, 2.43f, 90, 1f, true,false).
                GetComponent<EnemyAttackHintBarRect2D>().SetDoScale(1);
            
            DOVirtual.DelayedCall(2.5f, () =>
            {
                var container = InitContainer(false);
                var fx1 = InstantiateRanged(purpleRangedPrefab,
                    leftPosition,container, 1);
                var fx2 = InstantiateRanged(purpleRangedPrefab,
                    rightPosition, container, 1);
                
            }, false);
        }

        return centerHint;
    }

    private GameObject SmashDownSecond(bool purpleFirst)
    {
        var redRangedPrefab = GetProjectileOfFormatName("action08_3");
        var purpleRangedPrefab = GetProjectileOfFormatName("action08_4");
        
        var startPosition = (Vector3)gameObject.RaycastedPosition() + new Vector3(ac.facedir*3f,0);
        
        var centerHint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                startPosition,
                RangedAttackFXLayer.transform, new Vector2(14,8),Vector2.zero,purpleFirst || (_behavior as H002_BehaviorTree).WingIsAlive==false,
                1, 1.25f, 90, 1f, true,true).
            GetComponent<EnemyAttackHintBarRect2D>().SetDoScale(1);

        if (!purpleFirst)
        {
            var leftPosition = startPosition + new Vector3(-10.0f, 0);
            
            if (leftPosition.x < BattleStageManager.Instance.mapBorderL)
            {
                leftPosition.y = new Vector2
                (BattleStageManager.Instance.mapBorderL + 0.1f,
                    leftPosition.y).RaycastedPlatform().bounds.max.y;
            }
            else
            {
                leftPosition.y = ((Vector2)leftPosition).RaycastedPlatform().bounds.max.y;
            }
            
            var rightPosition = startPosition + new Vector3(10.0f, 0);
            
            if (rightPosition.x > BattleStageManager.Instance.mapBorderR)
            {
                rightPosition.y = new Vector2
                (BattleStageManager.Instance.mapBorderR - 0.1f,
                    rightPosition.y).RaycastedPlatform().bounds.max.y;
            }
            else
            {
                rightPosition.y = ((Vector2)rightPosition).RaycastedPlatform().bounds.max.y;
            }
            
            
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    leftPosition,
                    RangedAttackFXLayer.transform, new Vector2(14,7),Vector2.zero,true,
                    1, 2.9f, 90, 1f, true,false).
                GetComponent<EnemyAttackHintBarRect2D>().SetDoScale(1);
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    rightPosition,
                    RangedAttackFXLayer.transform, new Vector2(14,7),Vector2.zero,true,
                    1, 2.9f, 90, 1f, true,false).
                GetComponent<EnemyAttackHintBarRect2D>().SetDoScale(1);

            DOVirtual.DelayedCall(3f, () =>
            {
                var container = InitContainer(false);
                var fx1 = InstantiateRanged(redRangedPrefab,
                    leftPosition,container, 1);
                var fx2 = InstantiateRanged(redRangedPrefab,
                    rightPosition, container, 1);
                
            }, false);


        }
        else
        {
            var leftPosition = startPosition + new Vector3(-10.0f, 0);
            
            if (leftPosition.x < BattleStageManager.Instance.mapBorderL)
            {
                leftPosition.y = new Vector2
                (BattleStageManager.Instance.mapBorderL + 0.1f,
                    leftPosition.y).RaycastedPlatform().bounds.max.y;
            }
            else
            {
                leftPosition.y = ((Vector2)leftPosition).RaycastedPlatform().bounds.max.y;
            }
            
            var rightPosition = startPosition + new Vector3(10.0f, 0);
            
            if (rightPosition.x > BattleStageManager.Instance.mapBorderR)
            {
                rightPosition.y = new Vector2
                (BattleStageManager.Instance.mapBorderR - 0.1f,
                    rightPosition.y).RaycastedPlatform().bounds.max.y;
            }
            else
            {
                rightPosition.y = ((Vector2)rightPosition).RaycastedPlatform().bounds.max.y;
            }
            
            
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    leftPosition,
                    RangedAttackFXLayer.transform, new Vector2(14,7),Vector2.zero,false,
                    1, 2.9f, 90, 1f, true,false).
                GetComponent<EnemyAttackHintBarRect2D>().SetDoScale(1);
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    rightPosition,
                    RangedAttackFXLayer.transform, new Vector2(14,7),Vector2.zero,false,
                    1, 2.9f, 90, 1f, true,false).
                GetComponent<EnemyAttackHintBarRect2D>().SetDoScale(1);
            
            DOVirtual.DelayedCall(3f, () =>
            {
                var container = InitContainer(false);
                var fx1 = InstantiateRanged(purpleRangedPrefab,
                    leftPosition,container, 1);
                var fx2 = InstantiateRanged(purpleRangedPrefab,
                    rightPosition, container, 1);
                
            }, false);
        }

        return centerHint;
    }

    private void SmashdownCenter(bool purple, Vector2 position)
    {
        var prefab = purple ? GetProjectileOfFormatName("action08_2") :
            GetProjectileOfFormatName("action08_1");
        bool broken = false;

        if ((_behavior as H002_BehaviorTree).WingIsAlive == false)
        {
            prefab = GetProjectileOfFormatName("action08_1");
            purple = false;
            broken = true;
        }
        

        InstantiateMeele(prefab, position, InitContainer(true)).
            GetComponent<AttackFromEnemy>().
            AddWithConditionAll(new TimerBuff(purple?(int)BasicCalculation.BattleCondition.ShadowBlight:
                (int)BasicCalculation.BattleCondition.Stun,purple?44:1,purple?21:3,1),
                broken?0:100);
    }

    private void PrepareCrossingCandies(bool redIsSafe)
    {
        var proj = InstantiateSealedContainer(GetProjectileOfFormatName("action09"),
            Vector3.zero, RangedAttackFXLayer.transform);

        var controller = proj.GetComponent<Projectile_H002_1>();
        controller.RedIsSafe = redIsSafe;
        controller.RandomSeed = Random.Range(1, 5);

    }

    private (Vector2 trueWeapon,List<Vector2> fakeWeapons,List<Vector2> positionList,int trueID) 
        SetFirstWeapon()
    {
        var trueWeapon = Random.Range(0, 3);
        List<Vector2> positions = new List<Vector2>()
        {
            new(13.5f, 8),
            new(-13.5f, 8),
            new(0,-3),
            new(0,18)
        };

        List<Vector2> offsets = new List<Vector2>()
        {
            new(0, -1.5f),
            new(0, -1.5f),
            new(0, 0.5f),
            new(0, -2.5f)
        };
        
        var truePosition = positions[trueWeapon] + offsets[trueWeapon];
        
        var fakeWeapons = new List<Vector2>();
        for (int i = 0; i < 4; i++)
        {
            if (i == trueWeapon)
            {
                continue;
            }

            fakeWeapons.Add(positions[i] + offsets[i]);
        }
        
        return (truePosition,fakeWeapons,positions,trueWeapon);

    }

    private float SetWeaponGroupHints(List<Vector2> posList, int trueID)
    {
        var trueHint = GenerateWarningPrefab("action10_2", posList[trueID],
            Quaternion.identity, RangedAttackFXLayer.transform);
        var trueHintBar = trueHint.GetComponent<EnemyAttackHintBarCircle>();
        trueHintBar.ease = Ease.Linear;
        //trueHintBar.warningTime += 2;
        
        var fakeHints = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            if (i == trueID)
            {
                continue;
            }
            
            var fakeHint = GenerateWarningPrefab("action10_1", posList[i],
                Quaternion.identity, RangedAttackFXLayer.transform);

            switch (i)
            {
                case 0:
                    fakeHint.GetComponent<EnemyAttackHintBarCircle>().ease = Ease.InCubic;
                    break;
                case 1:
                    fakeHint.GetComponent<EnemyAttackHintBarCircle>().ease = Ease.InSine;
                    break;
                case 2:
                    fakeHint.GetComponent<EnemyAttackHintBarCircle>().ease = Ease.InCirc;
                    break;
                case 3:
                    fakeHint.GetComponent<EnemyAttackHintBarCircle>().ease = Ease.InSine;
                    break;
                default:
                    fakeHint.GetComponent<EnemyAttackHintBarCircle>().ease = Ease.InCirc;
                    break;
            }
            
            fakeHints.Add(fakeHint);
        }
        var circleBlastPrefab = GetProjectileOfFormatName("action10_2");
        var container = InitContainer(false);
        
        var punisherAttackPillarPrefab = GetProjectileOfFormatName("action10_5",true);
        
        DOVirtual.DelayedCall(trueHintBar.warningTime - 2.8f, () =>
        {
            foreach (var fake in fakeHints)
            {
                InstantiateRanged(circleBlastPrefab, fake.transform.position,
                    container, 1);
                InstantiateRanged(punisherAttackPillarPrefab,new Vector3(0,22),
                    container, 1);
            }
            
        }, false);
        
        DOVirtual.DelayedCall(trueHintBar.warningTime + 0.2f, () =>
        {
            var container_true = InitContainer(false);
            InstantiateRanged(circleBlastPrefab, trueHint.transform.position,
                container_true, 1);
            InstantiateRanged(punisherAttackPillarPrefab,new Vector3(0,22),
                container_true, 1);
        }, false);
        
        return trueHintBar.warningTime;
        
    }


    private int FirePillars(int fixedDirection = 0)
    {
        int direction = Random.Range(0, 2) == 0 ? -1 : 1;
        
        if(fixedDirection != 0)
            direction = fixedDirection > 0 ? 1 : -1;
        
        var startPos = direction == -1 ? BattleStageManager.Instance.mapBorderL + .1f :
            BattleStageManager.Instance.mapBorderR - .1f;

        float pos = startPos;
        
        var container = InitContainer(false);
        var prefab = GetProjectileOfFormatName("action10_4",true);

        float triggerTime = 0.5f;
        int i = 0;
        
        while(pos >= BattleStageManager.Instance.mapBorderL &&
              pos <= BattleStageManager.Instance.mapBorderR)
        {
            i++;
            if(i > 10)
                break;
            
            var x = pos;

            DOVirtual.DelayedCall(triggerTime, () =>
            {
                
                var pillar = InstantiateRanged(prefab,
                    new Vector2(x, BattleStageManager.Instance.mapBorderB - 2),
                    container, 1);
                pillar.GetComponent<AttackFromEnemy>().
                    AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Burn,
                        72,12,1),80);
            }, false);
            
            triggerTime += 0.6f;
            pos -= direction * 8;
        }

        return direction;

    }

    private void SuperRingBlast(GameObject wing, GameObject self)
    {
        var container = InitContainer(false);
        
        var prefab = GetProjectileOfFormatName("action11_2",true);
        
        var fx1 = InstantiateRanged(prefab, wing.transform.position,
            container, 1);
        
        var fx2 = InstantiateRanged(prefab, self.transform.position,
            container, 1);
    }

    private (List<GameObject> candies,List<float> angles) PrepareBouncingCandies(Vector3 globalOffset)
    {
        int type = Random.Range(0, 2);
        
        var redPrefab = GetProjectileOfFormatName("action12_1",true);
        var purplePrefab = GetProjectileOfFormatName("action12_2",true);
        
        var container = InitContainer(false);

        var positionOffsetPreset = new List<Vector2>()
        {
            //顺时针
            new(0,3),
            new(1.5f,1.5f),
            new(3,0),
            new(1.5f,-1.5f),
            new(0,-3),
            new(-1.5f,-1.5f),
            new(-3,0),
            new(-1.5f,1.5f),
        };

        float[] angles = new[] { 0f, 0.785f, 1.57f, 2.355f, 3.14f, 3.925f, 4.71f, 5.495f };
        
        List<GameObject> candies = new List<GameObject>();
        
        if(transform.position.y - 3 < BattleStageManager.Instance.mapBorderB)
            globalOffset.y += 3;

        for(int i = 0; i < 8; i++)
        {
            var prefab = i % 2 == type ? redPrefab : purplePrefab;
            var offset = (Vector3)positionOffsetPreset[i];
            var angle = angles[i];
            
            var candy = InstantiateRanged(prefab, transform.position+offset+globalOffset,
                container, 1);
            
            candies.Add(candy);
        }

        return (candies,angles.ToList());
    }

    private void GroundSmashLoop()
    {
        var prefab = GetProjectileOfFormatName("action13",true);
        
        var container = InitContainer(false, 15);
        
        Destroy(container,15f);

        var currentPlatform = gameObject.RaycastedPlatform();

        if(currentPlatform.bounds.max.x - currentPlatform.bounds.min.x < 15)
        {
            if (transform.position.x > currentPlatform.bounds.center.x)
            {
                ac.SetFaceDir(-1);
            }else if (transform.position.x < currentPlatform.bounds.center.x)
            {
                ac.SetFaceDir(1);
            }
        }
        

        Vector2 positionCurrent = new Vector2(transform.position.x + ac.facedir * 3, currentPlatform.bounds.max.y);
        
        int currentDirection = ac.facedir;

        for (int i = 0; i < 15; i++)
        {
            float delay = 0.1f + i * 0.25f;
            var pos = positionCurrent;
            
            DOVirtual.DelayedCall(delay, () =>
            {
                var groundSmash = InstantiateRanged(prefab, pos,
                    container, 1);
                groundSmash.GetComponent<AttackFromEnemy>().
                    AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
                        45,21,1),99);
            }, false);
            
            if(positionCurrent.x + currentDirection * 3 > currentPlatform.bounds.max.x ||
               positionCurrent.x + currentDirection * 3 < currentPlatform.bounds.min.x)
            {
                currentDirection = -currentDirection;
            }
            
            positionCurrent.x += currentDirection * 3;
            
        }

    }

    private void DecreaseResistances()
    {
        _statusManager.PoisonRes = 
            _statusManager.PoisonRes - 40 + 
                                   (int)(_statusManager.GetConditionTotalValue
                                       ((int)BasicCalculation.BattleCondition.PoisonResDown) 
                                         - _statusManager.GetConditionTotalValue
                                             ((int)BasicCalculation.BattleCondition.PoisonRes));
        
        _statusManager.BurnRes = 
            _statusManager.BurnRes - 40 + 
                                  (int)(_statusManager.GetConditionTotalValue
                                      ((int)BasicCalculation.BattleCondition.BurnResDown) 
                                        - _statusManager.GetConditionTotalValue
                                            ((int)BasicCalculation.BattleCondition.BurnRes));
        
        
        _statusManager.ParalysisRes = 
            _statusManager.ParalysisRes - 40 + 
                                        (int)(_statusManager.GetConditionTotalValue
                                            ((int)BasicCalculation.BattleCondition.ParalysisResDown) 
                                              - _statusManager.GetConditionTotalValue
                                                  ((int)BasicCalculation.BattleCondition.ParalysisRes));
        
        _statusManager.FrostbiteRes = 
            _statusManager.FrostbiteRes - 40 + 
                                      (int)(_statusManager.GetConditionTotalValue
                                          ((int)BasicCalculation.BattleCondition.FrostbiteResDown) 
                                            - _statusManager.GetConditionTotalValue
                                                ((int)BasicCalculation.BattleCondition.FrostbiteRes));
        
        _statusManager.ScorchrendRes = 
            _statusManager.ScorchrendRes - 40 + 
                                       (int)(_statusManager.GetConditionTotalValue
                                           ((int)BasicCalculation.BattleCondition.ScorchrendResDown) 
                                             - _statusManager.GetConditionTotalValue
                                                 ((int)BasicCalculation.BattleCondition.ScorchrendRes));
        
        _statusManager.FlashburnRes = 
            _statusManager.FlashburnRes - 40 + 
                                      (int)(_statusManager.GetConditionTotalValue
                                          ((int)BasicCalculation.BattleCondition.FlashburnResDown) 
                                            - _statusManager.GetConditionTotalValue
                                                ((int)BasicCalculation.BattleCondition.FlashburnRes));
        
        _statusManager.StormlashRes = 
            _statusManager.StormlashRes - 40 + 
                                      (int)(_statusManager.GetConditionTotalValue
                                          ((int)BasicCalculation.BattleCondition.StormlashResDown) 
                                            - _statusManager.GetConditionTotalValue
                                                ((int)BasicCalculation.BattleCondition.StormlashRes));
        
        _statusManager.ShadowblightRes = 
            _statusManager.ShadowblightRes - 40 + 
                                         (int)(_statusManager.GetConditionTotalValue
                                             ((int)BasicCalculation.BattleCondition.ShadowBlightResDown) 
                                               - _statusManager.GetConditionTotalValue
                                                   ((int)BasicCalculation.BattleCondition.ShadowBlightRes));
    }
    
    
    
}
