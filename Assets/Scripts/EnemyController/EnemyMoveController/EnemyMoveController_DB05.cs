using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMoveController_DB05 : EnemyMoveManager
{
    [SerializeField] private List<GameObject> _minions = new();
    private VoiceControllerEnemy voice;

    protected override void Start()
    {
        base.Start();
        GetAllAnchors();
        voice = GetComponentInChildren<VoiceControllerEnemy>();
    }

    private void OnBreakEnter()
    {
        anim.transform.localEulerAngles = new(0, 102, 0);
    }

    public IEnumerator DB05_Action01()
    {
        yield return _canActionOnFlyingGround;
        
        ac.OnAttackEnter(100);
        
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
        
        ac.OnAttackEnter(100);
        
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
        voice?.BroadCastMyVoice(6);
        
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
        voice?.BroadCastMyVoice(6);
        
        yield return new WaitForSeconds(1.4f);
        
        PoisonPoolSpit();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }

    public void DB05_Action07()
    {
        var hp = _behavior.difficulty * 4500 + 7000;

        SpawnEnemyMinon(_minions[0], new Vector3(-13, -2), hp).
            GetComponent<Projectile_DB005_1>().enemySource = gameObject;
        SpawnEnemyMinon(_minions[0], new Vector3(13, -2), hp).
            GetComponent<Projectile_DB005_1>().enemySource = gameObject;
        
        StageCameraController.SwitchOverallCamera();
        DOVirtual.DelayedCall(3, () => StageCameraController.SwitchMainCamera(), false);
    }

    public void DB05_Action07V()
    {
        StageCameraController.SwitchOverallCamera();

        DOVirtual.DelayedCall(3, () => StageCameraController.SwitchMainCamera(), false);
        
        
        var randSeed = Random.Range(0, 2);
        var hp = _behavior.difficulty * 4500 + 7000;

        if (randSeed == 0)
        {
            SpawnEnemyMinon(_minions[0], new Vector3(-13, -2),
                    hp).
                GetComponent<Projectile_DB005_1>().enemySource = gameObject;
            DOVirtual.DelayedCall(7, () =>
            {
                SpawnEnemyMinon(_minions[0], new Vector3(13, -2),
                        hp).
                    GetComponent<Projectile_DB005_1>().enemySource = gameObject;
            }, false);
            DOVirtual.DelayedCall(14, () =>
            {
                SpawnEnemyMinon(_minions[0], new Vector3(-13, -2),
                        hp).
                    GetComponent<Projectile_DB005_1>().enemySource = gameObject;
            }, false);
            DOVirtual.DelayedCall(20, () =>
            {
                SpawnEnemyMinon(_minions[0], new Vector3(13, -2),
                        hp).
                    GetComponent<Projectile_DB005_1>().enemySource = gameObject;
            }, false);
        }
        else
        {
            SpawnEnemyMinon(_minions[0], new Vector3(13, -2),
                    hp).
                GetComponent<Projectile_DB005_1>().enemySource = gameObject;
            DOVirtual.DelayedCall(7, () =>
            {
                SpawnEnemyMinon(_minions[0], new Vector3(-13, -2),
                        hp).
                    GetComponent<Projectile_DB005_1>().enemySource = gameObject;
            }, false);
            DOVirtual.DelayedCall(14, () =>
            {
                SpawnEnemyMinon(_minions[0], new Vector3(13, -2),
                        hp).
                    GetComponent<Projectile_DB005_1>().enemySource = gameObject;
            }, false);
            DOVirtual.DelayedCall(20, () =>
            {
                SpawnEnemyMinon(_minions[0], new Vector3(-13, -2),
                        hp).
                    GetComponent<Projectile_DB005_1>().enemySource = gameObject;
            }, false);
        }
        
        
        
        
    }
    
    /// <summary>
    /// Poison Sweep Side
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action08(int direction = -1)
    {
        yield return _canActionOnFlyingGround;
        
        ac.OnAttackEnter();
        
        voice?.BroadCastMyVoice(6);

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("sweep_enter");

        var modelTransform = GetComponentInChildren<AnimationEventSender_Enemy>().transform;

        
        _tweener = modelTransform.DOLocalRotate(new Vector3(0, 180, 0), 0.5f);
        
        

        yield return new WaitForSeconds(1.5f);


        if (direction == ac.facedir)
        {
            anim.Play("sweep_left_center");
        }
        else
        {
            anim.Play("sweep_right_center");
        }
        
        
        yield return new WaitForSeconds(0.55f);
        
        PoisonFront(direction);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f);
        
        ac.TurnMove(_behavior.targetPlayer);
        _tweener = modelTransform.DOLocalRotate(new Vector3(0, 102, 0), 0.5f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        //anim.Play("idle");
        yield return new WaitForSeconds(0.3f);
        
        QuitAttack();
        
        
    }
    
    /// <summary>
    /// 奔流
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action09()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB05_Action09");
        
        
        anim.Play("roar_1");
        
        
        yield return new WaitForSeconds(1f);
        ShadowBlast();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 混沌狱炎
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action10()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        //ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB05_Action10");
        voice?.BroadCastMyVoice(6);

        var hintTime = 3f;
        var pos = ChaosFlame_Hint1(hintTime);
        var pos2 = ChaosFlame_Hint2(hintTime);

        yield return new WaitForSeconds(hintTime - 1.5f);

        anim.Play("roar_1");

        yield return new WaitForSeconds(1.5f);
        ChaosFlame(pos);
        ChaosFlame(pos2);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 征战记忆1st（海因）
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action11()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB05_Action11");
        voice?.BroadCastMyVoice(4);
        anim.Play("roar_1");

        yield return new WaitForSeconds(1.5f);
        
        Memories_Heinwald2();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 征战记忆2nd（法库+斑比）
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action12()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB05_Action12");
        voice?.BroadCastMyVoice(4);
        anim.Play("roar_1");

        yield return new WaitForSeconds(1.5f);
        
        Memories_GalaCleo();
        Memories_Vania(1);
        Memories_Vania(-1);

        DOVirtual.DelayedCall(2f, () =>
        {
            Memories_GalaCleo();
            Memories_Vania(1);
            Memories_Vania(-1);
        }, false);
        
        DOVirtual.DelayedCall(4f, () =>
        {
            Memories_Vania(1);
            Memories_Vania(-1);
        }, false);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 征战记忆3rd（法库+海因）
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action13()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB05_Action13");
        voice?.BroadCastMyVoice(4);
        anim.Play("roar_1");

        yield return new WaitForSeconds(1.5f);
        
        Memories_GalaCleo();
        Memories_Heinwald2();

        DOVirtual.DelayedCall(3f, () =>
        {
            Memories_GalaCleo();
        }, false);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 征战记忆5th（ALL）
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action14()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB05_Action14");
        voice?.BroadCastMyVoice(4);
        anim.Play("roar_1");

        yield return new WaitForSeconds(1.5f);
        
        Memories_GalaCleo();
        Memories_Vania(1);
        Memories_Vania(-1);
        
        DOVirtual.DelayedCall(1f, () =>
        {
            Memories_Heinwald1();
        }, false);

        DOVirtual.DelayedCall(2f, () =>
        {
            Memories_Vania(1);
            Memories_Vania(-1);
        }, false);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 征战创造1:（xcw+龙妹）
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action15()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB05_Action15");
        voice?.BroadCastMyVoice(5);
        anim.Play("roar_1");
        
        DOVirtual.DelayedCall(2f, () =>
        {
            Summon_Forte(-8);
            Summon_Linnea(8);
        },false);

        yield return new WaitForSeconds(1.5f);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 征战创造2:（沙音+贝姐）
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action16()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB05_Action16");
        voice?.BroadCastMyVoice(5);
        anim.Play("roar_1");
        
        DOVirtual.DelayedCall(2f, () =>
        {
            Summon_Bellina(0,2);
            Summon_GalaAlex(-8,2);
        },false);

        yield return new WaitForSeconds(1.5f);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 征战创造3:（沙音+xcw+龙妹）
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action17()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB05_Action17");
        voice?.BroadCastMyVoice(5);
        anim.Play("roar_1");
        
        DOVirtual.DelayedCall(1f, () =>
        {
            Summon_GalaAlex(-12,2);
            Summon_Linnea(12,2);
        },false);
        
        DOVirtual.DelayedCall(2.5f, () =>
        {
            Summon_Forte(0,1);
        },false);

        yield return new WaitForSeconds(1.5f);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 征战创造4:（沙音+贝姐+xcw）
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action18()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB05_Action18");
        voice?.BroadCastMyVoice(5);
        anim.Play("roar_1");
        
        DOVirtual.DelayedCall(2f, () =>
        {
            Summon_Bellina(0,1);
        },false);
        
        DOVirtual.DelayedCall(1f, () =>
        {
            Summon_Linnea(12,1);
            Summon_GalaAlex(-12,1);
        },false);

        yield return new WaitForSeconds(1.5f);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 征战创造5:（沙音+贝姐+龙妹）
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action19()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB05_Action19");
        voice?.BroadCastMyVoice(5);
        anim.Play("roar_1");
        
        DOVirtual.DelayedCall(1f, () =>
        {
            Summon_Bellina(0,1);
        },false);
        DOVirtual.DelayedCall(2.5f, () =>
        {
            Summon_Forte(0,1);
            Summon_GalaAlex(-10,1);
        },false);

        yield return new WaitForSeconds(1.5f);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 征战创造6:（ALL）
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB05_Action20()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB05_Action20");
        voice?.BroadCastMyVoice(5);
        anim.Play("roar_1");
        
        DOVirtual.DelayedCall(1f, () =>
        {
            //Summon_Forte(-16,1);
            Summon_GalaAlex(-8,1);
            //Summon_Linnea(8,2);
            Summon_Bellina(16,2);
        },false);
        
        DOVirtual.DelayedCall(2.5f, () =>
        {
            Summon_Forte(-16,1);
            //Summon_GalaAlex(-8,1);
            Summon_Linnea(8,2);
            //Summon_Bellina(16,2);
        },false);

        yield return new WaitForSeconds(1.5f);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    
    private void SlashAttack()
    {
        InstantiateMeele(GetProjectileOfFormatName("action01"), transform.position,
            InitContainer(true));
    }
    
    private void TailAttack()
    {
        var proj = InstantiateMeele(GetProjectileOfFormatName("action02"),
            transform.position - new Vector3(ac.facedir*5f,0),
            InitContainer(true));

        //var debuff = new TimerBuff((int)BasicCalculation.BattleCondition.Bleeding);
        
        var atk = proj.GetComponent<AttackFromEnemy>();

        var buff = new TimerBuff((int)BasicCalculation.BattleCondition.Spite,
            1,-1,5,0);
        buff.dispellable = false;
        
        var shadowblight = new TimerBuff((int)BasicCalculation.BattleCondition.ShadowBlight,
            44,21,1);
        
        atk.AddWithConditionAll(new TimerBuff(buff),999);
        atk.AddWithConditionAll(new TimerBuff(shadowblight),100,1);
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
        var proj = InstantiateRanged(GetProjectileOfFormatName("action04"), 
            transform.position + new Vector3(3.5f*ac.facedir,2.5f),
            InitContainer(false),ac.facedir);
        
        var atk = proj.GetComponent<AttackFromEnemy>();

        var buff = new TimerBuff((int)BasicCalculation.BattleCondition.Spite,
            1,-1,5,0);
        buff.dispellable = false;
        
        atk.AddWithConditionAll(new TimerBuff(buff),999);
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
        InstantiateRanged(GetProjectileOfFormatName("action06"),
            transform.position + new Vector3(ac.facedir * 12.5f,-0.25f),
            InitContainer(false),ac.facedir).GetComponent<AttackFromEnemy>()
            .AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Poison,
                60,15,100,-1),100);
    }

    private void PoisonFront(int direction)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action08_L"), 
            transform.position + new Vector3(0,2f),
            InitContainer(false),-direction);
        
        var atk = proj.GetComponent<AttackFromEnemy>();

        var buff = new TimerBuff((int)BasicCalculation.BattleCondition.Spite,
            1,-1,5,0);
        buff.dispellable = false;
        
        atk.AddWithConditionAll(new TimerBuff(buff),999);

    }

    private void ShadowBlast()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action09"),
            transform.position,
            InitContainer(false),1);
        
        var atk = proj.GetComponent<ForcedAttackFromEnemy>();


        var caf =
            new ConditionalAttackEffect
            ((src, tar) =>
                {
                    if (tar.HasCondition((int)BasicCalculation.BattleCondition.Spite))
                    {
                        return true;
                    }
                    return false;
                }, ConditionalAttackEffect.ExtraEffect.Custom, new string[] { },
                new string[] {});

        caf.SetEffectFunction((sms, tar) =>
        {
            var stack = 
                sms.targetStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.Spite);

            if (stack <= 0)
            {
                return 0;
            }
            else if(stack <= 4)
            {
                return stack * 500;
            }
            else
            {
                //sms.targetStat.RemoveTimerBuff((int)BasicCalculation.BattleCondition.Spite,true,0);
                sms.targetStat.RemoveTimerBuff((int)BasicCalculation.BattleCondition.Spite,false,0);
                sms.targetStat.RemoveTimerBuff((int)BasicCalculation.BattleCondition.Spite,false,0);
                sms.targetStat.RemoveTimerBuff((int)BasicCalculation.BattleCondition.Spite,false,0);
                sms.targetStat.RemoveTimerBuff((int)BasicCalculation.BattleCondition.Spite,true,0);
                return 99499;
            }
            
        });
        
        atk.AddConditionalAttackEffect(caf);


    }

    private Vector2 ChaosFlame_Hint1(float hintTime)
    {
        var randomX = UnityEngine.Random.Range(15, 20f);
        var hint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector3(randomX, _behavior.targetPlayer.RaycastedPosition().y), RangedAttackFXLayer.transform,
            new Vector2(30, 8), Vector2.zero, true, 1, hintTime, 90);
        return hint.transform.position;
    }
    
    private Vector2 ChaosFlame_Hint2(float hintTime)
    {
        var randomX = UnityEngine.Random.Range(-20f, -15f);
        var hint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector3(randomX, _behavior.targetPlayer.RaycastedPosition().y), RangedAttackFXLayer.transform,
            new Vector2(30, 8), Vector2.zero, true, 1, hintTime, 90);
        return hint.transform.position;
    }

    private void ChaosFlame(Vector2 pos)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action10"),
            pos, 
            InitContainer(false), 1);

        var wandingProjectile = proj.GetComponent<WandingProjectile>();
        wandingProjectile.SetWandingEdges(BattleStageManager.Instance.mapBorderL,
            BattleStageManager.Instance.mapBorderR);
        wandingProjectile.SetFiredir(pos.x < transform.position.x ? 1 : -1);

    }

    private void Memories_GalaCleo()
    {
        List<Vector2> positionList = new();
        var span = 0.125f*(BattleStageManager.Instance.mapBorderR - BattleStageManager.Instance.mapBorderL);
        for (int i = 0; i < 8; i++)
        {
            positionList.Add
                (new Vector2(span*i+Random.Range(span*0.1f,span*0.9f)+ BattleStageManager.Instance.mapBorderL,BattleStageManager.Instance.mapBorderB));
        }

        foreach (var position in positionList)
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, position, RangedAttackFXLayer.transform,
                new Vector2(12, 2.5f), Vector2.zero, true, 1, 0.5f, 90, 0.5f,
                true, false);
        }

        DOVirtual.DelayedCall(0.8f, () =>
        {
            var projPrefab = GetProjectileOfFormatName("action11_1");
            var container = InitContainer(false);
            foreach (var position in positionList)
            {
                InstantiateRanged(projPrefab, position, container, 1);
            }
        }, false);
    }

    private void Memories_Heinwald1()
    {
        var position1 = _behavior.targetPlayer.RaycastedPosition();
        var position2 = new Vector2(Random.Range(BattleStageManager.Instance.mapBorderL + 5,
            BattleStageManager.Instance.mapBorderR - 5),position1.y);
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, position1,
            RangedAttackFXLayer.transform, new Vector2(10, 12f), Vector2.zero, false,
            1, 2.5f, 90, 1, true, false);
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, position2,
            RangedAttackFXLayer.transform, new Vector2(10, 12f), Vector2.zero, false,
            1, 3.5f, 90, 1, true, false);
        var projPrefab = GetProjectileOfFormatName("action11_2");
        DOVirtual.DelayedCall(2.5f, () =>
        {
            InstantiateRanged(projPrefab, position1, InitContainer(false), 1);
        }, false);
        DOVirtual.DelayedCall(3.5f, () =>
        {
            InstantiateRanged(projPrefab, position2, InitContainer(false), 1);
        }, false);
    }
    
    private void Memories_Heinwald2()
    {
        List<Vector2> positionList1 = new();
        List<Vector2> positionList2 = new();
        var span = (BattleStageManager.Instance.mapBorderR - BattleStageManager.Instance.mapBorderL) / 6;
        var randInt = BasicCalculation.RandInt(0, 1);

        for (int i = 0; i < 6; i++)
        {
            if (i % 2 == randInt)
            {
                positionList1.Add(new Vector2(span * i + BattleStageManager.Instance.mapBorderL,BattleStageManager.Instance.mapBorderB));
            }
            else
            {
                positionList2.Add(new Vector2(span * i + BattleStageManager.Instance.mapBorderL,BattleStageManager.Instance.mapBorderB));
            }
        }

        for (int i = 0; i < positionList1.Count; i++)
        {
            GenerateWarningPrefab("action11_1", positionList1[i], Quaternion.identity, RangedAttackFXLayer.transform);
        }

        DOVirtual.DelayedCall(2f, () =>
        {
            for (int i = 0; i < positionList2.Count; i++)
            {
                GenerateWarningPrefab("action11_1", positionList2[i], Quaternion.identity, RangedAttackFXLayer.transform);
            }
        }, false);
        
        DOVirtual.DelayedCall(2.5f, () =>
        {
            var container = InitContainer(false);
            var prefab = GetProjectileOfFormatName("action11_2");
            for (int i = 0; i < positionList1.Count; i++)
            {
                InstantiateRanged(prefab, positionList1[i], container, 1);
            }
        }, false);
        
        DOVirtual.DelayedCall(4.5f, () =>
        {
            var container = InitContainer(false);
            var prefab = GetProjectileOfFormatName("action11_2");
            for (int i = 0; i < positionList2.Count; i++)
            {
                InstantiateRanged(prefab, positionList2[i], container, 1);
            }
        }, false);


    }

    private void Memories_Vania(int direction)
    {
        //direction *= -1;
        
        var prefab1 = GetProjectileOfFormatName("action11_3");
        var prefab2 = GetProjectileOfFormatName("action11_4");

        var positionsY = new float[] { -2, 1.25f, 4.5f, 7.75f, 11, 14.25f };

        var randSeed = BasicCalculation.RandInt(0, 2);

        var positionX = direction > 0 ?
            BattleStageManager.Instance.mapBorderL :
            BattleStageManager.Instance.mapBorderR;

        var container = InitContainer(false);

        switch (randSeed)
        {
            case 0:
            {
                InstantiateDirectionalRanged(prefab1, new (positionX, positionsY[1] + 1), container, direction,0);
                InstantiateDirectionalRanged(prefab2, new(positionX, positionsY[2] + 1), container, direction,0);
                InstantiateDirectionalRanged(prefab1, new(positionX, positionsY[3] + 1), container, direction,0);
                InstantiateDirectionalRanged(prefab2, new(positionX, positionsY[4] + 1), container, direction,0);
                InstantiateDirectionalRanged(prefab1, new(positionX, positionsY[5] + 1), container, direction,0);
                break;
            }
            case 1:
            {
                InstantiateDirectionalRanged(prefab1, new(positionX, positionsY[0]), container, direction,0);
                InstantiateDirectionalRanged(prefab1, new(positionX, positionsY[2] + 3), container, direction,0);
                InstantiateDirectionalRanged(prefab2, new(positionX, positionsY[3]+ 3), container, direction,0);
                InstantiateDirectionalRanged(prefab1, new(positionX, positionsY[4]+ 3), container, direction,0);
                InstantiateDirectionalRanged(prefab2, new(positionX, positionsY[5]+ 3), container, direction,0);
                break;
            }
            case 2:
            {
                InstantiateDirectionalRanged(prefab1, new(positionX, positionsY[0] + 1), container, direction,0);
                InstantiateDirectionalRanged(prefab2, new(positionX, positionsY[1] + 1), container, direction,0);
                InstantiateDirectionalRanged(prefab1, new(positionX, positionsY[3] + 5), container, direction,0);
                InstantiateDirectionalRanged(prefab2, new(positionX, positionsY[4] + 5), container, direction,0);
                InstantiateDirectionalRanged(prefab1, new(positionX, positionsY[5] + 5), container, direction,0);
                break;
            }
        }





    }

    private void Summon_GalaAlex(float posX, float random = 1f)
    {
        if (_behavior.targetPlayer.transform.position.x < 0)
            posX *= -1;

        var posY = gameObject.RaycastedPosition().y + 1.3f;
        
        var atk = _behavior.difficulty * 1000 + 500;
        
        var enemy = SpawnEnemyMinon(GetProjectileOfFormatName("action15_3", true),
            new Vector3(posX + Random.Range(-random,random),posY), 
            99999, atk, ac.facedir, true);
        
        enemy.GetComponent<Projectile_DB005_5>().SetTarget(_behavior.targetPlayer);

    }
    
    private void Summon_Linnea(float posX, float random = 1f)
    {
        if (_behavior.targetPlayer.transform.position.x < 0)
            posX *= -1;

        var posY = gameObject.RaycastedPosition().y + 1.3f;
        
        var atk = _behavior.difficulty * 250 + 1500;
        
        var enemy = SpawnEnemyMinon(GetProjectileOfFormatName("action15_4", true),
            new Vector3(posX + Random.Range(-random,random),posY), 
            99999, atk, ac.facedir, true);
        
        enemy.GetComponent<Projectile_DB005_6>().SetTarget(_behavior.targetPlayer);

    }

    private void Summon_Forte(float posX, float random = 1f)
    {
        if (_behavior.targetPlayer.transform.position.x < 0)
            posX *= -1;
        
        var posY = gameObject.RaycastedPosition().y + 1.3f;

        var atk = _behavior.difficulty * 500 + 1000;
        
        var enemy = SpawnEnemyMinon(GetProjectileOfFormatName("action15_1"),
            new Vector3(posX + Random.Range(-random,random),posY), 
            99999, atk, ac.facedir, true);

    }
    
    private void Summon_Bellina(float posX, float random = 1f)
    {
        if (_behavior.targetPlayer.transform.position.x < 0)
            posX *= -1;
        
        var posY = gameObject.RaycastedPosition().y + 1.3f;
        
        var atk = _behavior.difficulty * 500 + 1500;
        
        var enemy = SpawnEnemyMinon(GetProjectileOfFormatName("action15_2", true),
            new Vector3(posX + Random.Range(-random,random),posY), 
            99999, atk, ac.facedir, true);

    }
    
    
}
