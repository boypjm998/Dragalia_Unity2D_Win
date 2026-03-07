using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_DB12 : EnemyMoveManager
{
    protected VoiceControllerEnemy _voiceControllerEnemy;

    private GameObject[] _orbs = new GameObject[4];

    [SerializeField] private Collider2D rushCollider;
    [SerializeField] private GameObject markFXPrefab;

    private static List<Vector2> _orbPositionPreset = new()
    {
        new Vector2(-12f, -1.5f),
        new Vector2(12f, -1.5f),
        new Vector2(-3f, 3f),
        new Vector2(3f, 3f)
    };

    protected override void Start()
    {
        base.Start();
        _voiceControllerEnemy = GetComponentInChildren<VoiceControllerEnemy>();
        if (rushCollider != null)
        {
            rushCollider.enabled = false;
            var col1 = ac.GetComponentInChildren<StandardGroundSensor>().GetSelfCollider();
            var col2 = GetComponentInChildren<EnemyOneWayPlatformEffector>().GetComponent<Collider2D>();
            
            Physics2D.IgnoreCollision(rushCollider,
                col1,true);
            Physics2D.IgnoreCollision(rushCollider,
                col2,true);
        }

        _statusManager.OnReceiveControlAffliction += StageCameraController.SwitchMainCamera;
    }
    
    /// <summary>
    /// 侵蚀
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB12_Action01(int effect)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB12_Action01");
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("combo1");
        if(_voiceControllerEnemy != null)
            _voiceControllerEnemy.BroadCastMyVoice(3);

        yield return new WaitForSeconds(0.7f);
        
        CorrosionFog(effect);
        

        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// 虚无
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB12_Action02()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB12_Action02");
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("combo1");

        yield return new WaitForSeconds(0.7f);
        
        NihilAOE();

        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// 召唤诅咒暗影
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB12_Action03(int hp)
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB12_Action03");
        StageCameraController.SwitchOverallCamera();
        //DOVirtual.DelayedCall(8, () => StageCameraController.SwitchMainCamera(),false);
        
        yield return new WaitForSeconds(2f);
        
        anim.Play("combo1");

        yield return new WaitForSeconds(0.7f);
        
        if(_voiceControllerEnemy != null)
            _voiceControllerEnemy.BroadCastMyVoice(4);
        
        SummonOrbs(hp);
        _behavior.controllAfflictionProtect = false;

        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        DOVirtual.DelayedCall(4f, () => StageCameraController.SwitchMainCamera(),false);
        
        QuitAttack();
    }
    
    
    public IEnumerator DB12_Action04()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(1f);
        
        anim.Play("combo1");
        
        var prefab = GetProjectileOfFormatName("action04");
        TargetingMine();

        Invoke("TargetingMine",1.25f);
        Invoke("TargetingMine",2.5f);

        yield return null;
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    public IEnumerator DB12_Action05(float buffAmount)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(1f);
        
        anim.Play("buff");

        yield return null;

        _statusManager.ObtainTimerBuff(1, buffAmount, 30);
        _statusManager.ObtainTimerBuff(1, buffAmount, 30);
        _voiceControllerEnemy?.BroadCastMyVoice(3);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// Spike
    /// </summary>
    /// <param name="buffAmount"></param>
    /// <returns></returns>
    public IEnumerator DB12_Action06()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);

        yield return null;

        EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac, transform.position + new Vector3(0,1),
            MeeleAttackFXLayer.transform, 5, Vector2.zero, true, true,
            2.5f, 0.1f, 0.5f, true, true);
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position + new Vector3(ac.facedir*5, 2f),
            MeeleAttackFXLayer.transform, new Vector2(17, 6), Vector2.zero, false, 0, 2.5f,
            0, 0.5f);
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position + new Vector3(-ac.facedir*5, 2f),
            MeeleAttackFXLayer.transform, new Vector2(17, 6), Vector2.zero, false, 0, 2.5f,
            180, 0.5f);

        yield return new WaitForSeconds(2f);
        
        anim.Play("combo3",0,0.3f);

        yield return new WaitForSeconds(0.5f);
        
        _voiceControllerEnemy?.BroadCastMyVoice(5);
        SpikeAndAround();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// Slap
    /// </summary>
    /// <param name="buffAmount"></param>
    /// <returns></returns>
    public IEnumerator DB12_Action07()
    {
        yield return _canAction;
        ac.OnAttackEnter(100);
        ac.SetGravityScale(0);
        ac.TurnMove(_behavior.targetPlayer);

        yield return null;

        BattleEffectManager.Instance.SpawnExclamation(gameObject,
            transform.position + new Vector3(0, 5),true);

        yield return new WaitForSeconds(1f);
        
        anim.Play("combo3",0,0.3f);

        yield return new WaitForSeconds(0.1f);
        
        //_voiceControllerEnemy?.BroadCastMyVoice(5);
        InstantiateMeele(GetProjectileOfFormatName("action07"), transform.position + new Vector3(4*ac.facedir,0),
            InitContainer(true)).transform.localScale = new(-1,1,1);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        ac.ResetGravityScale();
        QuitAttack();
    }

    /// <summary>
    /// Deadly Dive A(Far -> Near)
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB12_Action08A()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.SetGravityScale(0);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB12_Action08");

        Tweener rushTweener = null;

        var rushInfo = GetRushEndPositionX(ac.facedir, 20);
        float rushTime = 0.5f;
        
        yield return new WaitForSeconds(0.5f);

        anim.Play("roll",0,0.05f);
        var duration = rushInfo.percentage * rushTime;
        (ac as EnemyControllerFlying).moveEnable = false;//rushCollider.enabled = true;)
        rushTweener = ac.rigid.DOMove(new Vector2(rushInfo.xPosition, transform.position.y),
            duration)
            .OnComplete(ResetRushCollider)
            .OnKill(ResetRushCollider)
            .SetEase(Ease.OutSine).OnUpdate(()=>print("isUpdating"));
        _voiceControllerEnemy.BroadCastMyVoice(5);
        
        rushTweener.Play();
        
        yield return new WaitForSeconds(duration + 0.05f);
        
        //TODO: Far Attack

        var posList = FarAttackHint();
        
        yield return new WaitForSeconds(0.8f);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("combo3",0,0.3f);
        yield return new WaitForSeconds(0.2f);
        
        FarAttack(posList);

        yield return new WaitForSeconds(1.5f);

        var distance = Mathf.Abs(transform.position.x - _behavior.targetPlayer.transform.position.x);
        if (distance < 10 && Mathf.Abs(transform.position.x) < 20)
        {
            ac.TurnMove(_behavior.targetPlayer, false);
        }
        else
        {
            ac.TurnMove(_behavior.targetPlayer);
        }
        
        rushInfo = GetRushEndPositionX(ac.facedir, 20);
        anim.Play("roll",0,0.05f); 
        duration = rushInfo.percentage * rushTime;
        rushCollider.enabled = true;
        (ac as EnemyControllerFlying).moveEnable = false;
        rushTweener = ac.rigid.DOMove(new Vector2(rushInfo.xPosition, transform.position.y),
                duration)
            .OnComplete(ResetRushCollider)
            .OnKill(ResetRushCollider)
            .SetEase(Ease.OutSine).OnUpdate(()=>print("isUpdating"));
        rushTweener.Play();
        
        yield return new WaitForSeconds(duration + 0.05f);
        //TODO: Near Attack
        
        NearAttackHint();
        
        yield return new WaitForSeconds(0.8f);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("combo3",0,0.3f);
        yield return new WaitForSeconds(0.2f);

        NearAttack();
        DarkDiveHint(true);

        yield return new WaitForSeconds(1);
        
        DarkDiveAttackFast(true);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
        
    }
    
    /// <summary>
    /// Deadly Dive A(Near -> Far)
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB12_Action08B()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.SetGravityScale(0);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB12_Action08");

        Tweener rushTweener = null;

        var rushInfo = GetRushEndPositionX(ac.facedir, 20);
        float rushTime = 0.5f;
        
        yield return new WaitForSeconds(0.5f);

        anim.Play("roll",0,0.05f);
        var duration = rushInfo.percentage * rushTime;
        rushCollider.enabled = true;
        (ac as EnemyControllerFlying).moveEnable = false;
        rushTweener = ac.rigid.DOMove(new Vector2(rushInfo.xPosition, transform.position.y),
            duration)
            .OnComplete(ResetRushCollider)
            .OnKill(ResetRushCollider)
            .SetEase(Ease.OutSine).OnUpdate(()=>print("isUpdating"));
        _voiceControllerEnemy.BroadCastMyVoice(5);
        rushTweener.Play();
        
        yield return new WaitForSeconds(duration + 0.05f);
        
        //TODO: Near Attack

        NearAttackHint();
        
        yield return new WaitForSeconds(0.8f);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("combo3",0,0.3f);
        yield return new WaitForSeconds(0.2f);
        
        NearAttack();

        yield return new WaitForSeconds(1.5f);

        var distance = Mathf.Abs(transform.position.x - _behavior.targetPlayer.transform.position.x);
        if (distance < 10 && Mathf.Abs(transform.position.x) < 20)
        {
            ac.TurnMove(_behavior.targetPlayer);
        }
        else
        {
            ac.TurnMove(_behavior.targetPlayer);
        }
        
        rushInfo = GetRushEndPositionX(ac.facedir, 20);
        anim.Play("roll",0,0.05f); 
        duration = rushInfo.percentage * rushTime;
        //rushCollider.enabled = true;
        (ac as EnemyControllerFlying).moveEnable = false;
        rushTweener = ac.rigid.DOMove(new Vector2(rushInfo.xPosition, transform.position.y),
                duration)
            .OnComplete(ResetRushCollider)
            .OnKill(ResetRushCollider)
            .SetEase(Ease.OutSine).OnUpdate(()=>print("isUpdating"));
        rushTweener.Play();
        
        yield return new WaitForSeconds(duration + 0.05f);
        //TODO: Far Attack
        
        var posList = FarAttackHint();
        
        yield return new WaitForSeconds(0.8f);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("combo3",0,0.3f);
        yield return new WaitForSeconds(0.2f);

        FarAttack(posList);
        DarkDiveHint(false);

        yield return new WaitForSeconds(1);
        
        DarkDiveAttackFast(false);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
        
    }
    
    /// <summary>
    /// twilight dance
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB12_Action09()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.SetGravityScale(0);
        ac.TurnMove(_behavior.targetPlayer);
        StageCameraController.SwitchOverallCamera();
        
        bossBanner?.PrintSkillName("DB12_Action09");

        var offset = Random.Range(0, 2) == 1 ? 0 : 22.5f;

        yield return new WaitForSeconds(1f);
        
        anim.Play("combo3",0,0.3f);
        _voiceControllerEnemy.BroadCastMyVoice(3);

        var list = TwilightDanceHint(offset);

        yield return new WaitForSeconds(1.2f);
        
        TwilightDanceAttack(list);
        yield return null;
        
        list = TwilightDanceHint(offset + 22.5f);
        
        yield return new WaitForSeconds(1.2f);
        TwilightDanceAttack(list);
        anim.Play("idle");
        
        yield return null;
        
        list = TwilightDanceHint(offset);
        yield return new WaitForSeconds(1.2f);
        TwilightDanceAttack(list);
        
        yield return null;
        
        list = TwilightDanceHint(offset + 22.5f);
        yield return new WaitForSeconds(1.2f);
        TwilightDanceAttack(list);
        
        StageCameraController.SwitchMainCamera();

        ac.ResetGravityScale();
        QuitAttack();
    }
    
    
    /// <summary>
    /// red and purple
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB12_Action10(bool single = true)
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(0.5f);
        
        var redIsUpper = Random.Range(0, 2) == 1;
        
        var posA = transform.position + new Vector3(ac.facedir * 2, 1.5f);
        var posB = transform.position + new Vector3(ac.facedir * 2, -1.5f);
        var posC = transform.position + new Vector3(-ac.facedir * 2, 1.5f);
        var posD = transform.position + new Vector3(-ac.facedir * 2, -1.5f);
        
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer, 2f);

        if (!single)
        {
            EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
                posC, RangedAttackFXLayer.transform,
                1.5f, Vector2.zero, true, true, 1);
            EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
                posD, RangedAttackFXLayer.transform,
                1.5f, Vector2.zero, true, true, 1);
        }
        EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
            posA, RangedAttackFXLayer.transform,
            1.5f, Vector2.zero, true, true, 1);
        EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
            posB, RangedAttackFXLayer.transform,
            1.5f, Vector2.zero, true, true, 1);
        
        
        yield return new WaitForSeconds(0.8f);

        anim.Play("combo3",0,0.3f);
        ac.TurnMove(_behavior.targetPlayer);
        _voiceControllerEnemy.BroadCastMyVoice(4);
        
        yield return new WaitForSeconds(0.2f);
        
        if(single)
        {
           ThrowFlameSingle(redIsUpper?posA:posB,redIsUpper?posB:posA);
        }
        else
        {
           ThrowFlameDouble(posA,posB,
               posC,posD);
        }
        
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }


    /// <summary>
    /// 转阶段
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB12_Action11()
    {
        yield return null;
        
        anim.Play("knockdown_enter");

        foreach (var orb in _orbs)
        {
            if (orb != null)
            {
                var dragonPointEnemy = orb.GetComponent<DragonPointEnemy>();
                dragonPointEnemy.DisableAll();
                var stat = orb.GetComponent<StatusManager>();
                stat.currentHp = 0;
            }
        }

        yield return new WaitForSeconds(1);
        
        var warpFXPrefab = GetProjectileOfFormatName("action11_1",true);
        
        var fx1 = Instantiate(warpFXPrefab,
            transform.position,Quaternion.identity,RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(0.1f);
        
        DisappearRenderer();
        yield return null;
        ac.SetGroundCollision(true);
        transform.position = new Vector3(0,1.85f);

        yield return new WaitUntil(()=>_voiceControllerEnemy.voice.isPlaying == false);
        yield return new WaitForSeconds(0.9f);
        
        
        var fx2 = Instantiate(warpFXPrefab,
            transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        
        StageCameraController.SwitchMainCamera();
        StageCameraController.SwitchMainCameraFollowObject(gameObject);

        yield return new WaitForSeconds(0.1f);
        
        AppearRenderer();

        yield return new WaitForSeconds(1);
        
        var transformFx = Instantiate(GetProjectileOfFormatName("action11_2"),
            transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        
        CineMachineOperator.Instance.CamaraShake(8f,2f);
        
        //todo: 改变场景
        (BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background1") as SpriteRenderer).DOColor(
            Color.clear, 1.5f);

        yield return new WaitForSeconds(2f);
        DOVirtual.DelayedCall(1.35f,()=>
            StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer),false);
        
        yield return null;
        
        QuitAttack();
        _behavior.currentMoveAction = null;



    }
    
    
    
    
    protected void CorrosionFog(float healNeeded)
    {
        var fx = InstantiateRanged(GetProjectileOfFormatName("action01"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,_behavior.viewerPlayer.transform.position.y-1),
            InitContainer(false),1);

        var corrosionEff = new AdvancedTimerBuff((int)BasicCalculation.BattleCondition.Corrosion,
            healNeeded, 8, 0, -1, 1, -1,8);
        
        fx.GetComponent<AttackFromEnemy>().AddWithConditionAll(corrosionEff,200);
        
    }
    
    protected void NihilAOE()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action02"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,
                transform.position.y,ac.ModelDepth),InitContainer(false),1);

        var nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, _behavior.difficulty == 1 ? 1:30, 1);
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(nihilDebuff,100);
        
    }

    protected void SummonOrbs(int hp)
    {
        var enemyPrefab = GetProjectileOfFormatName("minion_1");
        for(int i = 0; i < 4; i++)
        {
            if (_orbs[i] == null)
            {
                _orbs[i] = SpawnEnemyMinon(enemyPrefab, _orbPositionPreset[i], hp,
                    _statusManager.baseAtk,_orbPositionPreset[i].x > 0 ? -1 : 1);
                var controller = _orbs[i].GetComponent<Projectile_DB012_1>();
                controller.SetSource(_statusManager);
            }
        }
    }
    
    protected void TargetingMine()
    {
        var prefab = GetProjectileOfFormatName("action04");
        var pos = _behavior.targetPlayer.RaycastedPosition();
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            pos+ new Vector2(0,-0.5f) , RangedAttackFXLayer.transform,
            new Vector2(3, 7), new Vector2(0,0), false, 0, 2.5f, 
            90, 0.5f,true,
            false);

        DOVirtual.DelayedCall(2.55f, () =>
        {
            var proj = InstantiateRanged(prefab,
                pos,
                InitContainer(false),1);
        },false);
    }

    protected void SpikeAndAround()
    {
        var spikePrefab = GetProjectileOfFormatName("action06_2");
        var aroundPrefab = GetProjectileOfFormatName("action06_1");
        var container = InitContainer(true);

        var around = InstantiateMeele(aroundPrefab, transform.position + new Vector3(0,1), container);
        var spike1 = InstantiateMeele(spikePrefab,
            transform.position + new Vector3(13.5f,-1f), container);
        var spike2 = InstantiateMeele(spikePrefab,
            transform.position + new Vector3(-13.5f,-1f), container);
    }

    protected (float xPosition,float percentage) GetRushEndPositionX(int dir, float distance)
    {
        var endPos = transform.position.x + dir * distance;
        
        endPos = Mathf.Clamp(endPos, BattleStageManager.Instance.mapBorderL + 1.25f,
            BattleStageManager.Instance.mapBorderR - 1.25f);
        
        var percentage = Mathf.Abs(endPos - transform.position.x) / distance;
        
        percentage = Mathf.Clamp(percentage,0.05f,1);
        
        return (endPos,percentage);
    }

    protected void NearAttackHint()
    {
        EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,transform.position,
            RangedAttackFXLayer.transform,
            8, Vector2.zero, true,
            true,1f,.1f,0.5f,true,true);
    }

    protected void NearAttack()
    {
        InstantiateMeele(GetProjectileOfFormatName("action08_1"), transform.position, InitContainer(true));
    }

    protected List<Vector2> FarAttackHint()
    {
        var radius = 12f;
        List<Vector2> positions = new List<Vector2>();
        
        for (int i = 0; i < 8; i++)
        {
            var angle = i * 45;
            var x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            var y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            positions.Add((Vector2)transform.position + new Vector2(x,y));
            
            EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,positions[i],
                RangedAttackFXLayer.transform,
                5, Vector2.zero, true,
                true,1f,.1f,0.5f,true,true);
        }
        
        return positions;
        
    }

    protected void FarAttack(List<Vector2> posList)
    {
        var container = InitContainer(false);
        var prefab = GetProjectileOfFormatName("action08_2");

        foreach (var pos in posList)
        {
            var proj = InstantiateRanged(prefab, pos, container,1);
        }
    }
    
    protected void DarkDiveHint(bool far)
    {
        if (far)
        {
            GenerateWarningPrefab("action08", new Vector3(transform.position.x + 18, 10),Quaternion.identity,
                RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action08", new Vector3(transform.position.x - 18, 10),Quaternion.identity,
                RangedAttackFXLayer.transform);
        }else
        {
            GenerateWarningPrefab("action08", new Vector3(transform.position.x, 10),Quaternion.identity,
                RangedAttackFXLayer.transform);
        }
    }

    protected void DarkDiveAttackFast(bool far)
    {
        var prefab = GetProjectileOfFormatName("action08_3");
        var container = InitContainer(false);
        if (far)
        {
            InstantiateRanged(prefab, new Vector3(transform.position.x, 10) + new Vector3(18, 0),
                container,1);
            InstantiateRanged(prefab, new Vector3(transform.position.x, 10) + new Vector3(-18, 0),
                container,1);
        }else
        {
            InstantiateRanged(prefab, new Vector3(transform.position.x, 10) + new Vector3(0, 0),
                container,1);
        }
    }

    protected List<Quaternion> TwilightDanceHint(float offsetAngle = 22.5f)
    {
        var prefab = GetWarningPrefab("action09");

        var pos = new Vector2(0, 5);
        
        var rotList = new List<Quaternion>();
        
        for (int i = 0; i < 8; i++)
        {
            rotList.Add(Quaternion.Euler(0,0,i * 45 + offsetAngle));
            GenerateWarningPrefab(prefab, pos, rotList[i],
                RangedAttackFXLayer.transform);
        }

        return rotList;

    }

    protected void TwilightDanceAttack(List<Quaternion> rotList)
    {
        var prefab = GetProjectileOfFormatName("action09");
        
        var container = InitContainer(false);
        
        for (int i = 0; i < 8; i++)
        {
            var proj = InstantiateRanged(prefab, 
                new Vector2(0, 5), container,1);
            proj.transform.rotation = rotList[i];
        }
        
    }

    protected void ThrowFlameSingle(Vector2 redPos, Vector2 purplePos)
    {
        var redPrefab = GetProjectileOfFormatName("action10_2");
        var purplePrefab = GetProjectileOfFormatName("action10_1");
        
        var container = InitContainer(false);
        
        var red = InstantiateRanged(redPrefab, redPos, container,1);
        var purple = InstantiateRanged(purplePrefab, purplePos, container,1);

        Vector2 endPosRed = _behavior.targetPlayer.transform.position;
        Vector2 endPosPurple = endPosRed;
        
        
        bool leftOrRight = Random.Range(0, 2) == 0;

        if (leftOrRight)
        {
            endPosPurple.x = endPosRed.x + 5;
        }
        else
        {
            endPosPurple.x = endPosRed.x - 5;
        }
        
        if(endPosPurple.x < BattleStageManager.Instance.mapBorderL)
            endPosPurple.x = endPosRed.x + 5;
        else if(endPosPurple.x > BattleStageManager.Instance.mapBorderR)
            endPosPurple.x = endPosRed.x - 5;
        
        bool upOrDown = Random.Range(0, 2) == 0;
        
        if (upOrDown)
        {
            endPosPurple.y = endPosRed.y + 5;
        }
        else
        {
            endPosPurple.y = endPosRed.y - 5;
        }
        
        if(endPosPurple.y < BattleStageManager.Instance.mapBorderB)
            endPosPurple.y = endPosRed.y + 5;
        else if(endPosPurple.y > BattleStageManager.Instance.mapBorderT)
            endPosPurple.y = endPosRed.y - 5;

        var redTweener = red.GetComponent<Rigidbody2D>().DOMove(endPosRed,
            1).SetUpdate(UpdateType.Fixed).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1.5f, () =>
            {
                var proj = InstantiateRanged(GetProjectileOfFormatName("action10_4", true),
                    red.transform.position, InitContainer(false), 1);
                proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
                    (new TimerBuff((int)BasicCalculation.BattleCondition.Burn,72,12,1),100);
                Destroy(red);
            },false);

            if (_behavior.difficulty == 1)
            {
                EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    red.transform.position, RangedAttackFXLayer.transform,
                    new Vector2(50, 4), new Vector2(-25, 0), false, 1,
                    1.5f, 0, 0.3f, true, false);
            }
        });
        
        var purpleTweener = purple.GetComponent<Rigidbody2D>().DOMove(endPosPurple,
            1).SetUpdate(UpdateType.Fixed).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1.5f, () =>
            {
                var proj = InstantiateRanged(GetProjectileOfFormatName("action10_3", true),
                    purple.transform.position, InitContainer(false), 1);
                proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
                    (new TimerBuff((int)BasicCalculation.BattleCondition.ShadowBlight,44,21,1),100);
                Destroy(purple);
            },false);
            if (_behavior.difficulty == 1)
            {
                EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    purple.transform.position, RangedAttackFXLayer.transform,
                    new Vector2(50, 4), new Vector2(-25, 0), false, 1,
                    1.5f, 90, 0.3f, true, false);
            }
        });
        
    }
    protected void ThrowFlameDouble(Vector2 redPos1, Vector2 redPos2, Vector2 purplePos1, Vector2 purplePos2)
    {
        var redPrefab = GetProjectileOfFormatName("action10_2");
        var purplePrefab = GetProjectileOfFormatName("action10_1");
        
        var container = InitContainer(false);
        
        var red1 = InstantiateRanged(redPrefab, redPos1, container,1);
        var purple1 = InstantiateRanged(purplePrefab, purplePos1, container,1);
        var red2 = InstantiateRanged(redPrefab, redPos2, container,1);
        var purple2 = InstantiateRanged(purplePrefab, purplePos2, container,1);

        Vector2 endPosRed1 = _behavior.targetPlayer.transform.position + new Vector3(0, 4f);
        Vector2 endPosRed2 = _behavior.targetPlayer.transform.position + new Vector3(0, -4f);
        Vector2 endPosPurple1 = _behavior.targetPlayer.transform.position + new Vector3(2.5f, 0);
        Vector2 endPosPurple2 = _behavior.targetPlayer.transform.position + new Vector3(-2.5f, 0);


        if (endPosPurple2.x < BattleStageManager.Instance.mapBorderL)
            endPosPurple2.x = _behavior.targetPlayer.transform.position.x + 5;
        
        if(endPosPurple1.x > BattleStageManager.Instance.mapBorderR)
            endPosPurple1.x = _behavior.targetPlayer.transform.position.x - 5;


        if (endPosRed2.y < BattleStageManager.Instance.mapBorderB)
            endPosRed2.y = _behavior.targetPlayer.transform.position.y;
        if(endPosRed1.y > BattleStageManager.Instance.mapBorderT)
            endPosRed1.y = _behavior.targetPlayer.transform.position.y;

        var redTweener1 = red1.GetComponent<Rigidbody2D>().DOMove(endPosRed1,
            1).SetUpdate(UpdateType.Fixed).OnComplete(() =>
        {
            DOVirtual.DelayedCall(2f, () =>
            {
                var proj = InstantiateRanged(GetProjectileOfFormatName("action10_4", true),
                    red1.transform.position, InitContainer(false), 1);
                proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
                    (new TimerBuff((int)BasicCalculation.BattleCondition.Burn,72,12,1),100);
                Destroy(red1);
            },false);

            if (_behavior.difficulty == 1)
            {
                EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    red1.transform.position, RangedAttackFXLayer.transform,
                    new Vector2(50, 4), new Vector2(-25, 0), false, 1,
                    2, 0, 0.3f, true, false);
            }
            
        });
        
        var purpleTweener1 = purple1.GetComponent<Rigidbody2D>().DOMove(endPosPurple1,
            1).SetUpdate(UpdateType.Fixed).OnComplete(() =>
        {
            DOVirtual.DelayedCall(2f, () =>
            {
                var proj = InstantiateRanged(GetProjectileOfFormatName("action10_3", true),
                    purple1.transform.position, InitContainer(false), 1);
                proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
                    (new TimerBuff((int)BasicCalculation.BattleCondition.ShadowBlight,44,21,1),100);
                Destroy(purple1);
            },false);
            
            if(_behavior.difficulty == 1)
            {
                EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    purple1.transform.position, RangedAttackFXLayer.transform,
                    new Vector2(50, 4), new Vector2(-25, 0), false, 1,
                    2, 90, 0.3f, true, false);
            }
        });
        
        var redTweener2 = red2.GetComponent<Rigidbody2D>().DOMove(endPosRed2,
            1).SetUpdate(UpdateType.Fixed).OnComplete(() =>
        {
            DOVirtual.DelayedCall(2f, () =>
            {
                var proj = InstantiateRanged(GetProjectileOfFormatName("action10_4", true),
                    red2.transform.position, InitContainer(false), 1);
                proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
                    (new TimerBuff((int)BasicCalculation.BattleCondition.Burn,72,12,1),100);
                Destroy(red2);
            },false);
            
            if(_behavior.difficulty == 1)
            {
                EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    red2.transform.position, RangedAttackFXLayer.transform,
                    new Vector2(50, 4), new Vector2(-25, 0), false, 1,
                    2, 0, 0.3f, true, false);
            }
            
        });
        
        var purpleTweener2 = purple2.GetComponent<Rigidbody2D>().DOMove(endPosPurple2,
            1).SetUpdate(UpdateType.Fixed).OnComplete(() =>
        {
            DOVirtual.DelayedCall(2f, () =>
            {
                var proj = InstantiateRanged(GetProjectileOfFormatName("action10_3", true),
                    purple2.transform.position, InitContainer(false), 1);
                proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
                    (new TimerBuff((int)BasicCalculation.BattleCondition.ShadowBlight,44,21,1),100);
                Destroy(purple2);
            },false);
            
            if(_behavior.difficulty == 1)
            {
                EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    purple2.transform.position, RangedAttackFXLayer.transform,
                    new Vector2(50, 4), new Vector2(-25, 0), false, 1,
                    2, 90, 0.3f, true, false);
            }
        });
        
    }

    protected void ResetRushCollider()
    {
        rushCollider.enabled = false;
        (ac as EnemyControllerFlying).moveEnable = true;
    }
    
    
}
