using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMoveController_DB13 : EnemyMoveManager
{
    private List<GameObject> orbList = new List<GameObject>();
    protected VoiceControllerEnemy _voiceControllerEnemy;

    protected override void Start()
    {
        base.Start();
        _voiceControllerEnemy = GetComponentInChildren<VoiceControllerEnemy>();
    }

    private void Update()
    {
        if (_tweener != null)
        {
            print(_tweener.ElapsedPercentage());
        }
    }

    /// <summary>
    /// Nihil
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB13_Action01()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB15_Action03");
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("buff_enter");

        yield return new WaitForSeconds(1.2f);
        
        anim.Play("buff_exit");
        NihilAOE();

        yield return null;

        yield return _animFinished;
        
        anim.Play("idle");
        
        QuitAttack();

    }


    /// <summary>
    /// 冲拳
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB13_Action02()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        
        
        anim.Play("charge_enter");

        yield return new WaitForSeconds(0.7f);
        
        
        anim.Play("charge_exit");
        
        yield return new WaitForSeconds(0.1f);
        StraightPunch();
        
        yield return null;

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        //anim.Play("idle");
        print("QuitAttack");
        QuitAttack();
    }

    /// <summary>
    /// Combo
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB13_Action03()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + new Vector3(0,5));
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("combo1");

        yield return new WaitForSeconds(0.5f);
        ComboPunch();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
        
        anim.Play("combo2");
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.4f);
        ComboPunch();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
        
        anim.Play("combo3");
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(0.8f);

        if (Mathf.Abs(transform.position.x - _behavior.targetPlayer.transform.position.x) > 6)
        {
            StraightPunch();
        }else ComboPunch();

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// Mine
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB13_Action04()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("buff_enter");

        yield return new WaitForSeconds(0.5f);
        
        TargetingMine();

        yield return new WaitForSeconds(1f);
        
        TargetingMine();
        
        yield return new WaitForSeconds(1f);
        
        TargetingMine();
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("buff_exit");
        
        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    
    public IEnumerator DB13_Action05(int type)
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("buff_enter");

        yield return new WaitForSeconds(0.5f);

        int minionHP = _behavior.difficulty * 5000 + 5000;

        if (type == 1)
        {
            SummonOrb(2,new(10,1),minionHP,true);
            SummonOrb(2,new(-10,1),minionHP,true);
            SummonOrb(1,new(0,8),minionHP,true);
        }
        else if(type == 2)
        {
            SummonOrb(1,new(10,8),minionHP,true);
            SummonOrb(1,new(-10,8),minionHP,true);
            SummonOrb(2,new(0,1),minionHP,true);
        }else if (type == 3)
        {
            SummonOrb(1,new(5,8),minionHP,true);
            SummonOrb(2,new(-5,8),minionHP,true);
            SummonOrb(1,new(0,8),minionHP,true);
        }else if (type == 4)
        {
            SummonOrb(2,new(5,8),minionHP,true);
            SummonOrb(1,new(-5,8),minionHP,true);
            SummonOrb(2,new(0,8),minionHP,true);
        }
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("buff_exit");
        
        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();
    }

    public IEnumerator DB13_Action05F()
    {
        yield return _canActionOnFlyingGround;
        ac.SetHitSensor(false);
        ac.OnAttackEnter(999);
        
        anim.Play("buff_enter");

        yield return new WaitForSeconds(0.5f);
        
        SummonOrb(0,new Vector2(4,8),5000*_behavior.difficulty,true);
        SummonOrb(0,new Vector2(-4,8),5000*_behavior.difficulty,true);
        UI_DialogDisplayer.Instance?.
            EnqueueDialogShared(10101,
                20131,BattleEffectManager.Instance?.notteHintClips[1]);
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("buff_exit");
        
        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        ac.SetHitSensor(true);
        QuitAttack();
        
    }
    
    /// <summary>
    /// ground
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB13_Action06()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        

        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, gameObject.RaycastedPosition() + new Vector2(1.5f*ac.facedir,0),
            RangedAttackFXLayer.transform, new Vector2(5, 16), Vector2.zero, false, 0,
            1.25f, 90);

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("smash");

        yield return new WaitForSeconds(0.75f);
        
        GroundSmash();
        
        //yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    
    /// <summary>
    /// WeakPoint/Suppression
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB13_Action07(int maxHp)
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB13_Action07");
        _voiceControllerEnemy?.BroadCastMyVoice(6);
        ac.SetHitSensor(false);

        var prefab = BattleEffectManager.Instance.GetWeakPointIndicator();

        var enemy = SpawnEnemyMinon(GetProjectileOfFormatName("action07"),
            transform.position + new Vector3(ac.facedir * 2f, 0.5f), maxHp, 9999, 1);
        
        var uiRingSlider = 
            SpawnCountDownUI(prefab, 
                transform.position + new Vector3(0, 5.5f), 
                20, 1);
        var uiMinionCount = uiRingSlider.GetComponent<UI_CountdownMinon>();
        uiMinionCount.AddNewStatusManager(enemy.GetComponent<StatusManager>());

        var enemyGeneratorTween = GenerateOrbsRandomly(_behavior.difficulty * 1000 + 2000);
        
        yield return new WaitUntil(()=>uiRingSlider.currentValue <= 0 || uiMinionCount.Value <= 0);
        
        enemyGeneratorTween?.Kill();

        if (uiMinionCount.Value > 0)
        {
            uiMinionCount.KillAllMinons();
            RandomRangeAttack(1.5f,1);
            DestoryAllOrbs();
            
            yield return null;
            Destroy(uiMinionCount.gameObject);
            _voiceControllerEnemy?.BroadCastMyVoice(4);
        }
        else
        {
            _voiceControllerEnemy?.BroadCastMyVoice(7);
            Destroy(uiMinionCount.gameObject);
            ac.SetHitSensor(true);
            anim.Play("knockdown_enter");
            yield return new WaitForSeconds(10);
            anim.Play("knockdown_exit");
            yield return null;
            yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        }
        
        bossBanner?.PrintSkillName("DB13_Action08");
        ac.SetHitSensor(true);
        anim.Play("buff_enter");
        yield return new WaitForSeconds(1f);
        StrengthSuppression();
        anim.Play("buff_exit");
        yield return null;
        
        
        //yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// 唤光圣辉
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB13_Action09()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("DB13_Action09");
        _voiceControllerEnemy?.BroadCastMyVoice(5);
        
        StageCameraController.SwitchOverallCamera();

        var leftHint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector3(-16.5f, 0), RangedAttackFXLayer.transform,
            new Vector2(25, 13), Vector2.zero, true, 1, 5, 90);
        
        var rightHint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector3(16.5f, 0), RangedAttackFXLayer.transform,
            new Vector2(25, 13), Vector2.zero, true, 1, 5, 90);

        anim.Play("buff_enter");

        yield return new WaitForSeconds(5);
        
        anim.Play("buff_exit");
        PillarInvocation();

        yield return new WaitForSeconds(1);
        
        StageCameraController.SwitchMainCamera();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();

    }
    
    
    
    
    protected void NihilAOE()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action01"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,
                transform.position.y,ac.ModelDepth),InitContainer(false),1);

        var nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, 30, 1);
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(nihilDebuff,100);
        
        
    }

    protected void StraightPunch()
    {
        
        var proj = InstantiateMeele(GetProjectileOfFormatName("action02"),
            new Vector3(transform.position.x,
                transform.position.y),InitContainer(true));

        var targetPos = transform.position.x + ac.facedir * 14f;
        
        targetPos = Mathf.Clamp(targetPos,BattleStageManager.Instance.mapBorderL,
            BattleStageManager.Instance.mapBorderR);

        var distance = Mathf.Abs(targetPos - transform.position.x);

        proj.GetComponent<AttackFromEnemy>().attackInfo[0].knockbackDirection = new Vector2(ac.facedir, 0);

        var rigid = transform.GetComponent<Rigidbody2D>();
        
        print(targetPos);
        print(distance);
        
        _tweener = transform.DOMoveX(targetPos, distance * 0.45f / 30f).
            SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed).OnComplete(() =>
            {
                if (proj != null)
                {
                    Destroy(proj);
                }
            });
        
        //DrasticForce.Instance?.AddDrasticForce();

    }
    
    protected void ComboPunch()
    {
        
        var proj = InstantiateMeele(GetProjectileOfFormatName("action03"),
            new Vector3(transform.position.x,
                transform.position.y),InitContainer(true));

        var targetPos = transform.position.x + ac.facedir * 5f;
        
        targetPos = Mathf.Clamp(targetPos,BattleStageManager.Instance.mapBorderL,
            BattleStageManager.Instance.mapBorderR);

        var distance = Mathf.Abs(targetPos - transform.position.x);

        proj.GetComponent<AttackFromEnemy>().attackInfo[0].knockbackDirection = new Vector2(ac.facedir, 0);

        var rigid = transform.GetComponent<Rigidbody2D>();
        
        _tweener?.Kill();
        
        _tweener = transform.DOMoveX(targetPos, 0.1f).
            SetEase(Ease.OutSine).SetUpdate(UpdateType.Fixed).OnComplete(() =>
            {
                if (proj != null)
                {
                    Destroy(proj);
                }
            });
        
        //DrasticForce.Instance?.AddDrasticForce();

    }

    protected void TargetingMine()
    {
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            _behavior.targetPlayer.RaycastedPosition(), RangedAttackFXLayer.transform,
            new Vector2(3, 5), Vector2.zero, true, 0, 0.8f, 
            90, 0.5f,true,
            false);
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action04"),
            _behavior.targetPlayer.RaycastedPosition(),
            InitContainer(false),1);
        
        proj.GetComponent<AttackFromEnemy>().
            AddWithConditionAll
                (new TimerBuff((int)BasicCalculation.BattleCondition.Blindness,1,12,1),
                    100);
        
    }

    protected void GroundSmash()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action06"),
            gameObject.RaycastedPosition() + new Vector2(1.5f*ac.facedir,0),
            InitContainer(false),1);
        
        proj.GetComponent<AttackFromEnemy>().
            AddWithConditionAll
            (new TimerBuff((int)BasicCalculation.BattleCondition.Paralysis,41,15,1),
                100);
    }

    public void AddDrasticForceEffectToStatusManager(StatusManager statusManager)
    {
        statusManager.SpecialDamageCutEffectFunc += Ability.DrasticForceEffect;

    }

    protected void PillarInvocation()
    {
        var container = InitContainer(false);
        var proj1 = InstantiateRanged(GetProjectileOfFormatName("action09", true), new Vector3(-16.5f, 0),
            container, 1);
        var proj2 = InstantiateRanged(GetProjectileOfFormatName("action09", true), new Vector3(16.5f, 0),
            container, 1);

        var blind = new TimerBuff((int)BasicCalculation.BattleCondition.Blindness, 1, 7, 1);
        
        proj1.GetComponent<AttackFromEnemy>().AddWithConditionAll(blind,50);
        proj2.GetComponent<AttackFromEnemy>().AddWithConditionAll(blind,50);
        
        SummonOrb(1,new Vector2(-13,1),3000+_behavior.difficulty * 2000,true);
        SummonOrb(1,new Vector2(13,1),3000+_behavior.difficulty * 2000,true);
        SummonOrb(1,new Vector2(-13,10),3000+_behavior.difficulty * 2000,true);
        SummonOrb(1,new Vector2(13,10),3000+_behavior.difficulty * 2000,true);
        SummonOrb(2,new Vector2(0,6),3000+_behavior.difficulty * 2000,true);
        
    }

    protected void SummonOrb(int orbType, Vector2 position, int maxHP, bool dashOnlyAbility = false)
    {
        //todo:也要选择Prefab
        var prefab = GetProjectileOfFormatName($"minion_{orbType}");
        var instance = SpawnEnemyMinon(prefab,position, maxHP,prefab.GetComponent<StatusManager>().baseAtk);
        
        var nullList = orbList.FindAll(x => x == null);
        
        foreach (var orb in nullList)
        {
            orbList.Remove(orb);
        }

        var sameOrb = orbList.Find(x => x.transform.position == instance.transform.position);
        
        if (sameOrb != null)
        {
            orbList.Remove(sameOrb);
            Destroy(sameOrb);
        }
        
        orbList.Add(instance);

        instance.GetComponent<Projectile_DB013_EnlightmentOrb>().enemySource = transform;
        
        if (dashOnlyAbility)
        {
            instance.GetComponent<Projectile_DB013_EnlightmentOrb>().SetProtectionFXOn();
            
            
            instance.GetComponent<StatusManager>().SpecialDamageCutEffectFunc += Ability.DashAttackEffectExtraAttack;
            
            
        }
            
    }

    protected void DestoryAllOrbs()
    {
        for (int i = 0; i < orbList.Count; i++)
        {
            if (orbList[i] != null)
            {
                Destroy(orbList[i]);
                orbList.RemoveAt(i);
                i--;
            }
        }
    }
    protected Tween GenerateOrbsRandomly(int maxHP)
    {
        Tween _tween = null;

        List<Vector2> positionList = new List<Vector2>()
        {
            new(8,1),new(7,1),new(6,1),new(4,1),new(9,1),
            new(8,1),new(7,1),new(6,1),new(4,1),new(9,1)
        };

        positionList.Shuffle();
        int index = 0;

        _tween = DOVirtual.DelayedCall(3,() =>
        {
            int sign = index % 2 == 0 ? 1 : -1;
            SummonOrb(Random.Range(0,3), new Vector2(sign*positionList[index].x,positionList[index].y), maxHP);
            index++;
        },false).OnComplete(() =>
        {
            if(index < 10)
                _tween.Restart();
        });

        return _tween;

    }

    protected void StrengthSuppression()
    {
        Instantiate(GetProjectileOfFormatName("action08"), Vector3.zero, Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        DrasticForce.Instance?.RemoveAllDrasticForce();
    }

    protected Tween RandomRangeAttack(float delay, float rng)
    {
        List<Vector2> offsetList = new List<Vector2>()
        {
            new(0, 0), new(2, 2), new(-2, 2), new(3, 0), new(-3, 0),
            new(3, 3), new(-3, 3), new(5, 5), new(-5, 5),
            new(0, 3), new(4, 4), new(-4, 4), new(6, 0), new(-6, 0),
            new(0, 6), new(6, 6), new(-6, 6)
        };

        offsetList.Shuffle();

        int index = 0;
        Tween tween = null;

        tween = DOVirtual.DelayedCall(delay, () =>
        {
            var hintTime = Random.Range(delay, delay + rng);
            var size = Random.Range(0.6f, 1.2f);

            var position = _behavior.targetPlayer.transform.position + (Vector3)offsetList[index];


            EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
                position,
                RangedAttackFXLayer.transform, 5 * size, Vector2.zero, index % 2 == 0, true,
                hintTime, 0.08f * size, 0.4f, true, false);

            
            
            DOVirtual.DelayedCall(hintTime, () =>
            {
                var proj = InstantiateRanged(GetProjectileOfFormatName("action07",true),
                    position, InitContainer(false), 1);
                if (index % 2 != 0)
                {
                    proj.GetComponent<AttackFromEnemy>().
                        ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
                }

                proj.transform.localScale *= size;

            }, false);
            
            index++;

        }, false).OnComplete(() =>
        {
            if (index <= 16)
            {
                tween.Restart();
            }
        });

        return tween;

    }
    
    
}
