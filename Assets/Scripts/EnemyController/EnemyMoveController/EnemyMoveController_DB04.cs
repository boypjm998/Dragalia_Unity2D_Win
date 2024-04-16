using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMoveController_DB04 : EnemyMoveManager
{
    protected VoiceControllerEnemy voice;
    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerFlyingHigh>();
        voice = GetComponentInChildren<VoiceControllerEnemy>();

    }
    
    
    public IEnumerator DB04_Action01()
    {
        yield return _canAction;
        ac.OnAttackEnter();
        anim.Play("charge_enter");
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        
        bossBanner?.PrintSkillName("DB04_Action01");
        
        int direction = Random.Range(0,2) == 0 ? -1 : 1;
        var hintTime = 2f;

        var hintBarLeft = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar
        (ac, transform.position + new Vector3(0,0), RangedAttackFXLayer.transform, new Vector2(10, 24),
            new Vector2(-0, 0), direction > 0 ? true : false, 0, hintTime, 180, 0.5f);
        
        var hintBarRight = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar
        (ac, transform.position + new Vector3(0,0), RangedAttackFXLayer.transform, new Vector2(10, 24),
            new Vector2(0, 0), direction < 0 ? true : false, 0, hintTime, 0, 0.5f);

        yield return new WaitForSeconds(hintTime - 1f);
        
        anim.Play("charge_enter");
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("charge_exit");
        AroundAttack(direction);

        yield return null;
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        ac.ResetGravityScale();
        QuitAttack();
        
    }
    
    public IEnumerator DB04_Action02()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(100);
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + new Vector3(0,5),true);

        yield return new WaitForSeconds(1);

        anim.Play("dash");

        yield return null;

        var fireDir = ac.facedir;
        var pos = DashMoveWithCollision();
        
        ac.SetKBRes(999);
        ac.SetCounter(false);
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        DashMoveTrailThunder(pos,fireDir);

        ac.ResetGravityScale();
        yield return new WaitForSeconds(1);
        anim.Play("idle");
        
        QuitAttack();
        
    }

    /// <summary>
    /// Claw
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB04_Action03(bool avoidable)
    {
        yield return _canActionOnFlyingGround;
        
        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));

        var avoidStr = avoidable ? "1" : "2";
        var hint = GenerateWarningPrefab($"action03_{avoidStr}", transform.position + new Vector3(0, 2),
            Quaternion.identity, MeeleAttackFXLayer.transform);

        yield return new WaitForSeconds(1.15f);
        
        anim.Play("claw");

        yield return new WaitForSeconds(0.9f);

        InstantiateRanged(GetProjectileOfFormatName($"action03_{avoidStr}"),
            transform.position + new Vector3(0, 2), InitContainer(false), ac.facedir);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();

    }



    /// <summary>
    /// 摇曳雷霆
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB04_Action04()
    {
        yield return _canAction;
        
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB04_Action04");

        yield return new WaitForSeconds(1);
        
        anim.Play("charge_enter");

        var posFloats = TwistingThunderHint();

        DOVirtual.DelayedCall(3, () => TwistingThunderAttack(new Vector2(posFloats.Item1, 0),
            new Vector2(posFloats.Item2, 0)),false);

        yield return new WaitForSeconds(2);
        
        anim.Play("charge_exit");

        yield return null;
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
        

    }
    
    /// <summary>
    /// 闪光射击
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB04_Action05(int mine = 0)
    {
        yield return _canAction;
        
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB04_Action05");
        anim.Play("charge_enter");
        
        yield return new WaitForSeconds(2);

        BoltBarrage(2,mine > 0);
        
        yield return new WaitForSeconds(1);
        
        BoltBarrage(2,mine > 0);
        
        yield return new WaitForSeconds(1);
        anim.Play("charge_exit");
        BoltBarrage(2,mine > 0);

        yield return null;

        yield return _animFinished;
        
        anim.Play("idle");
        
        QuitAttack();
        

    }
    
    /// <summary>
    /// 无常之怒
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB04_Action06()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB04_Action06");
        anim.Play("charge_enter");
        
        yield return new WaitForSeconds(1);

        ArcingStorm(10);
        
        yield return new WaitForSeconds(1);
        
        anim.Play("charge_exit");
        
        yield return null;

        yield return _animFinished;
        
        anim.Play("idle");
        
        QuitAttack();
        

    }
    
    /// <summary>
    /// 雷霆之雨
    /// </summary>
    public IEnumerator DB04_Action07(int type = 0)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB04_Action07");
        anim.Play("charge_enter");
        StageCameraController.SwitchOverallCamera();
        voice?.BroadCastMyVoice(0);
        
        yield return new WaitForSeconds(1);

        SweepingThunder(type);
        
        yield return new WaitForSeconds(1);
        
        anim.Play("charge_exit");
        
        yield return null;

        yield return _animFinished;
        
        anim.Play("idle");
        StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    /// <summary>
    /// 菱形雷
    /// </summary>
    public IEnumerator DB04_Action08(int direction = 0)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        if (direction == 0)
        {
            ac.TurnMove(_behavior.targetPlayer);
        }
        else
        {
            ac.SetFaceDir(direction > 0?1:-1);
        }
        
        bossBanner?.PrintSkillName("DB04_Action08");
        anim.Play("charge_enter");

        yield return new WaitForSeconds(1);

        VoltaicShellsHint();
        voice?.BroadCastMyVoice(0);
        
        yield return new WaitForSeconds(1);
        
        anim.Play("charge_exit");
        
        yield return null;

        yield return _animFinished;
        
        anim.Play("idle");
        //StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    public IEnumerator DB04_Action09()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("charge_enter");
        bossBanner?.PrintSkillName("DB04_Action09");
        voice?.BroadCastMyVoice(4);
        
        DOVirtual.DelayedCall(3, () => MemoryZenaHorizontal(),false);
        
        yield return new WaitForSeconds(2);
        
        anim.Play("charge_exit");
        
        yield return null;

        yield return _animFinished;
        
        
        anim.Play("idle");
        //StageCameraController.SwitchMainCamera();
        QuitAttack();

    }

    public IEnumerator DB04_Action10()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("charge_enter");
        bossBanner?.PrintSkillName("DB04_Action10");
        voice?.BroadCastMyVoice(4);
        
        DOVirtual.DelayedCall(3, () =>
        {
            MemoryZenaScattered();
            MemorySanzang1();
        },false);
        
        yield return new WaitForSeconds(2);
        
        anim.Play("charge_exit");
        
        yield return null;

        yield return _animFinished;
        
        
        anim.Play("idle");
        //StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    public IEnumerator DB04_Action11()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("charge_enter");
        bossBanner?.PrintSkillName("DB04_Action11");
        voice?.BroadCastMyVoice(4);
        
        DOVirtual.DelayedCall(3, () =>
        {
            MemoryZenaHorizontal();
        },false);
        DOVirtual.DelayedCall(6f, () =>
        {
            MemoryMagic1();
        },false);
        
        yield return new WaitForSeconds(2);
        
        anim.Play("charge_exit");
        
        yield return null;

        yield return _animFinished;
        
        
        anim.Play("idle");
        //StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    public IEnumerator DB04_Action12()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        anim.Play("charge_enter");
        bossBanner?.PrintSkillName("DB04_Action12");
        voice?.BroadCastMyVoice(4);
        DOVirtual.DelayedCall(3, () =>
        {
            MemoryZenaScattered();
            MemorySanzang2();
        },false);
        DOVirtual.DelayedCall(1.5f, () =>
        {
            MemoryMagic2();
        },false);
        DOVirtual.DelayedCall(6f, () =>
        {
            MemoryMagic1();
        },false);
        
        yield return new WaitForSeconds(2);
        
        anim.Play("charge_exit");
        
        yield return null;

        yield return _animFinished;
        
        
        anim.Play("idle");
        ac.ResetGravityScale();
        //StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    /// <summary>
    /// 歌姬+弗里茨
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB04_Action13()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        anim.Play("charge_enter");
        bossBanner?.PrintSkillName("DB04_Action13");
        voice?.BroadCastMyVoice(5);

        DOVirtual.DelayedCall(3, () =>
        {
            SummonFritz();
        },false);
        
        DOVirtual.DelayedCall(2, () =>
        {
            SummonLucretia();
        },false);

        yield return new WaitForSeconds(2);
        
        anim.Play("charge_exit");
        
        yield return null;

        yield return _animFinished;
        
        
        anim.Play("idle");
        ac.ResetGravityScale();
        //StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    /// <summary>
    /// 八千代+夏迪
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB04_Action14()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        anim.Play("charge_enter");
        bossBanner?.PrintSkillName("DB04_Action14");
        voice?.BroadCastMyVoice(5);

        DOVirtual.DelayedCall(3, () =>
        {
            SummonZardin();
        },false);
        
        DOVirtual.DelayedCall(4.5f, () =>
        {
            SummonYachiyo();
        },false);

        yield return new WaitForSeconds(2);
        
        anim.Play("charge_exit");
        
        yield return null;

        yield return _animFinished;
        
        
        anim.Play("idle");
        ac.ResetGravityScale();
        //StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    /// <summary>
    /// 弗里茨+夏迪
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB04_Action15()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        anim.Play("charge_enter");
        bossBanner?.PrintSkillName("DB04_Action15");
        voice?.BroadCastMyVoice(5);

        DOVirtual.DelayedCall(3, () =>
        {
            SummonFritz();
        },false);
        
        DOVirtual.DelayedCall(4.5f, () =>
        {
            SummonZardin();
        },false);

        yield return new WaitForSeconds(2);
        
        anim.Play("charge_exit");
        
        yield return null;

        yield return _animFinished;
        
        
        anim.Play("idle");
        ac.ResetGravityScale();
        //StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    /// <summary>
    /// 荒天（歌姬+弗里茨+夏迪）
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB04_Action16()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        anim.Play("charge_enter");
        bossBanner?.PrintSkillName("DB04_Action16");
        voice?.BroadCastMyVoice(5);
        
        DOVirtual.DelayedCall(2, () =>
        {
            SummonLucretia();
        },false);

        DOVirtual.DelayedCall(3, () =>
        {
            SummonYachiyo();
            SummonFritz();
        },false);
        

        yield return new WaitForSeconds(2);
        
        anim.Play("charge_exit");
        
        yield return null;

        yield return _animFinished;
        
        
        anim.Play("idle");
        ac.ResetGravityScale();
        //StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    /// <summary>
    /// 八千代+夏迪+歌姬
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB04_Action17()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        anim.Play("charge_enter");
        bossBanner?.PrintSkillName("DB04_Action17");
        voice?.BroadCastMyVoice(5);
        
        DOVirtual.DelayedCall(2, () =>
        {
            SummonLucretia();
        },false);

        DOVirtual.DelayedCall(3, () =>
        {
            SummonZardin();
        },false);
        
        DOVirtual.DelayedCall(4, () =>
        {
            SummonYachiyo();
        },false);

        yield return new WaitForSeconds(2);
        
        anim.Play("charge_exit");
        
        yield return null;

        yield return _animFinished;
        
        
        anim.Play("idle");
        ac.ResetGravityScale();
        //StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    /// <summary>
    /// 八千代+夏迪+歌姬+弗里茨
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB04_Action18()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        anim.Play("charge_enter");
        bossBanner?.PrintSkillName("DB04_Action18");
        voice?.BroadCastMyVoice(5);
        
        DOVirtual.DelayedCall(2, () =>
        {
            SummonYachiyo();
        },false);

        DOVirtual.DelayedCall(3, () =>
        {
            SummonZardin();
            SummonFritz();
        },false);
        
        DOVirtual.DelayedCall(4, () =>
        {
            SummonLucretia();
        },false);

        yield return new WaitForSeconds(2);
        
        anim.Play("charge_exit");
        
        yield return null;

        yield return _animFinished;
        
        
        anim.Play("idle");
        ac.ResetGravityScale();
        //StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    /// <summary>
    /// 核爆
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB04_Action19()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        anim.Play("charge_enter");
        bossBanner?.PrintSkillName("DB04_Action19");
        voice?.BroadCastMyVoice(6);
        
        yield return new WaitForSeconds(1.8f);
        
        anim.Play("charge_exit");
        
        yield return new WaitForSeconds(0.3f);
        
        AllRangedAttack();

        yield return _animFinished;
        
        
        anim.Play("idle");
        ac.ResetGravityScale();
        //StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    
    /// <summary>
    /// 高阶雷霆之雨
    /// </summary>
    public IEnumerator DB04_Action20()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB04_Action20");
        anim.Play("charge_enter");
        StageCameraController.SwitchOverallCamera();
        voice?.BroadCastMyVoice(6);
        
        yield return new WaitForSeconds(1);

        SweepingThunder(1);
        Invoke("SweepingThunderHorizontal",4.5f);
        
        yield return new WaitForSeconds(1);
        
        anim.Play("charge_exit");
        
        yield return new WaitForSeconds(1);

        yield return _animFinished;
        
        anim.Play("idle");
        
        yield return new WaitForSeconds(6);
        
        StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    
    public IEnumerator DB04_Action21()
    {
        yield return _canAction;
        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetGravityScale(0);
        anim.Play("charge_enter");
        //bossBanner?.PrintSkillName("DB04_Action19");
        voice?.BroadCastMyVoice(0);
        
        var pos = gameObject.RaycastedPosition();

        GenerateWarningPrefab("action21", pos + new Vector2(6f, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);
        GenerateWarningPrefab("action21", pos - new Vector2(6f, 0), Quaternion.identity,
            RangedAttackFXLayer.transform).transform.localScale = new Vector3(-1,1,1);
        
        yield return new WaitForSeconds(2.2f);
        
        anim.Play("charge_exit");
        
        yield return new WaitForSeconds(0.3f);
        
        AroundDoubleThunder(pos);

        yield return _animFinished;
        
        
        anim.Play("idle");
        ac.ResetGravityScale();
        //StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    
    

    protected void AroundAttack(int dir)
    {
        var container = InitContainer(false, 2);
        
        var proj1 = InstantiateRanged(GetProjectileOfFormatName("action01_1"),
            transform.position, container,dir);
        
        var proj2 = InstantiateRanged(GetProjectileOfFormatName("action01_2"),
            transform.position, container,dir);

        var attack1 = proj1.GetComponent<AttackFromEnemy>();

        attack1.AddWithConditionAll(
                new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
                    82,21,1),100);
        
        var attack2 = proj2.GetComponent<AttackFromEnemy>();
        
        attack2.AddWithConditionAll(
            new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
                82,21,1),100);
        

        if (dir == -1)
        {
            proj1.transform.localScale = new Vector3(-1, 1, 1);
            proj2.transform.localScale = new Vector3(-1, 1, 1);
        }

    }

    protected Vector2 DashMoveWithCollision(float tweenTime = 0.6f)
    {
        var proj = InstantiateMeele(GetProjectileOfFormatName("action02_2"),
            transform.position + new Vector3(0, 3), InitContainer(true));

        var targetPosition = new Vector2(transform.position.x + ac.facedir * 24,transform.position.y).SafePosition();

        var distance = Mathf.Abs(targetPosition.x - transform.position.x);

        _tweener = transform.DOMoveX(targetPosition.x,
            tweenTime * (distance / 24f) + 0.05f).OnComplete(() =>
        {
            anim.SetTrigger("action");
            if (proj != null)
                Destroy(proj);
        }).SetEase(Ease.InOutSine);

        return new Vector2(transform.position.x,gameObject.RaycastedPosition().y);

    }

    protected void DashMoveTrailThunder(Vector2 position, int dir)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action02_1"),
            position, InitContainer(false), dir);
        
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
            (new TimerBuff((int)BasicCalculation.BattleCondition.Paralysis,121,15,1),
                80);
        
    }

    protected (float,float) TwistingThunderHint()
    {
        var mapBorderL = BattleStageManager.Instance.mapBorderL;
        var mapBorderR = BattleStageManager.Instance.mapBorderR;
        var playerPos = _behavior.targetPlayer.transform.position.x;
        float mapWidth = mapBorderR - mapBorderL;
        float range = 3;

        // 生成第一个随机数，距离playerPos为range
        float random1 = Random.Range(playerPos - range, playerPos + range);

        // 确保第二个随机数距离playerPos大于地图总宽度的一半
        float halfMapWidth = mapWidth / 3;
        float random2;
        if (playerPos - mapBorderL > halfMapWidth)
        {
            random2 = Random.Range(mapBorderL, playerPos - halfMapWidth);
        }
        else
        {
            random2 = Random.Range(playerPos + halfMapWidth, mapBorderR);
        }

        Debug.Log("Random1: " + random1);
        Debug.Log("Random2: " + random2);

        var hint1 = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar
        (ac, new Vector3(random1, 0), RangedAttackFXLayer.transform,
            new Vector2(24, 3), Vector2.zero, true, 1, 3, 90,
            0.5f, true, false);
        var hint2 = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar
        (ac, new Vector3(random2, 0), RangedAttackFXLayer.transform,
            new Vector2(24, 3), Vector2.zero, true, 1, 3, 90,
            0.5f, true, false);

        return new (random1, random2);
    }

    protected void TwistingThunderAttack(Vector2 position1, Vector2 position2)
    {
        var container = InitContainer(false, 2);

        var proj1 = InstantiateRanged(GetProjectileOfFormatName("action04_1"),position1,
            container, 1);
        
        var proj2 = InstantiateRanged(GetProjectileOfFormatName("action04_1"),position2,
            container, 1);

        var range = 5;

        var left1 = Mathf.Clamp(position1.x - range, BattleStageManager.Instance.mapBorderL,
            BattleStageManager.Instance.mapBorderR);
        var left2 = Mathf.Clamp(position2.x - range, BattleStageManager.Instance.mapBorderL,
            BattleStageManager.Instance.mapBorderR);
        
        var right1 = Mathf.Clamp(position1.x + range, left1 + range * 2,
            BattleStageManager.Instance.mapBorderR);
        var right2 = Mathf.Clamp(position2.x + range, left2 + range * 2,
            BattleStageManager.Instance.mapBorderR);
        
        proj1.GetComponent<WandingProjectile>().SetWandingEdges(left1,right1);
        proj2.GetComponent<WandingProjectile>().SetWandingEdges(left2,right2);

        int dir = Random.Range(0, 2) == 1 ? -1 : 1;
        
        proj1.GetComponent<WandingProjectile>().SetFiredir(-dir);
        proj2.GetComponent<WandingProjectile>().SetFiredir(dir);
    }
    
    protected void BoltBarrage(float fillTime, bool mine = false)
    {
        var targetPosition = _behavior.targetPlayer.RaycastedPosition();

        if (_behavior.difficulty < 3)
        {
            var hintBar = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                targetPosition, RangedAttackFXLayer.transform,
                new Vector2(12, 4), Vector2.zero, false, 1, fillTime, 90, 0.5f,
                true, false);
        }
        

        DOVirtual.DelayedCall(fillTime, () =>
        {
            var container = InitContainer(false,mine?2:1);

            var proj = InstantiateRanged(GetProjectileOfFormatName("action05_1"),
                targetPosition, container, 1);
            
            proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
                (new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn, 84, 21,1),
                    100);

            if (mine)
            {
                InstantiateRanged(GetProjectileOfFormatName("action05_2"),
                    targetPosition, container, 1);
            }

        }, false);
    }

    protected void ArcingStorm(float radiusBase)
    {
        var radiusArray = BasicCalculation.RandomNormalDistribution(10, 0.5f);
        var angleArray = BasicCalculation.RandomNormalDistribution(10,0.5f);

        var positionList = new List<Vector2>();
        var container = InitContainer(false, 10);
        var blindness = new TimerBuff
            ((int)BasicCalculation.BattleCondition.Blindness, 1, Random.Range(8f, 11f), 1);

        var timeList = new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, .8f, .9f, 1f };
        var lockIndex = BasicCalculation.RandInt(0, 9);
        timeList.Shuffle();
        
        for (int i = 0; i < 10; i++)
        {
            float delayTime = timeList[i];
            int index = i;
            radiusArray[i] *= radiusBase;
            angleArray[i] *= Mathf.PI * 2.0f;
            positionList.
                Add(transform.position+new Vector3(radiusArray[index] * Mathf.Cos(angleArray[index]),
                    (radiusArray[index] * Mathf.Sin(angleArray[index]))).SafePosition(Vector2.zero));
            if (index == lockIndex)
            {
                print("Delaying" + positionList[index]);
                positionList[index] = _behavior.targetPlayer.transform.position;
            }
            
            
            DOVirtual.DelayedCall(delayTime, () =>
            {
                EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac, 
                    positionList[index], RangedAttackFXLayer.transform,
                    3f, Vector2.zero, true, true, 1.5f, 0.05f, 0.8f,
                    true, false);

                DOVirtual.DelayedCall(1.5f, () =>
                {
                    var proj =
                        InstantiateRanged(GetProjectileOfFormatName("action06"),
                            positionList[index], container, 1);

                    proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(blindness, 80);
                }, false);


            }, false);
            
        }
        
        

    }

    protected void SweepingThunder(int fixedType = 0)
    {
        var type = BasicCalculation.RandInt(1, 3);

        if (fixedType > 0)
        {
            type = Mathf.Clamp(fixedType,1,3);
        }

        string prefabFirst;
        string prefabSecond;

        switch (type)
        {
            case 1:
                prefabFirst = "A1";
                prefabSecond = "A2";
                break;
            case 2:
                prefabFirst = "B1";
                prefabSecond = "B2";
                break;
            case 3:
                prefabFirst = "C1";
                prefabSecond = "C2";
                break;
            default:
                prefabFirst = "A1";
                prefabSecond = "A2";
                break;
        }

        var proj1 = InstantiateSealedContainer(GetProjectileOfFormatName($"action07_{prefabFirst}", true),
            new Vector3(0,22),false,1);
        proj1.GetComponent<DOTweenSimpleController>().SetWaitTime(3.5f);

        DOVirtual.DelayedCall(3.5f, () =>
        {
            InstantiateSealedContainer(GetProjectileOfFormatName($"action07_{prefabSecond}", true),
                new Vector3(0, -6), false, 1);
        }, false);


    }

    protected void SweepingThunderHorizontal()
    {
        var proj1 = InstantiateSealedContainer(GetProjectileOfFormatName("action20_A1", true),
            new Vector3(-25.5f,0),false,1);
        proj1.GetComponent<DOTweenSimpleController>().SetWaitTime(2.5f);

        DOVirtual.DelayedCall(3f, () =>
        {
            InstantiateSealedContainer(GetProjectileOfFormatName("action20_A2", true),
                new Vector3(25.5f, 0), false, 1);
        }, false);
    }

    protected void VoltaicShellsHint()
    {
        var offsets = new List<Vector2>()
        {
            new (16,0),new(-16,0),new(0, 10), new(0,-10),
            new(8,5),new(8,-5),new(-8,5),new(-8,-5)
        };

        //offsets.ForEach(x => x += (Vector2)transform.position);

        float delay = 4f;

        foreach (var offset in offsets)
        {
            EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
                offset + (Vector2)transform.position, RangedAttackFXLayer.transform, 2, Vector2.zero, false,
                true, delay, 0.05f, 0.5f, true, false);
        }

        int dir = ac.facedir;
        Vector2 startPosition = transform.position;
        print(startPosition);
        DOVirtual.DelayedCall(delay, () => VoltaicShellsAttack(offsets,startPosition,dir),
            false);


    }

    protected void VoltaicShellsAttack(List<Vector2> positions,Vector2 startPos, int dir)
    {
        float distance1 = dir > 0
            ? BattleStageManager.Instance.mapBorderR - startPos.x
            : startPos.x - BattleStageManager.Instance.mapBorderL;

        float distance2 = BattleStageManager.Instance.mapBorderR - BattleStageManager.Instance.mapBorderL;

        float speed = 4.5f;

        var container = InitContainer(false, 8);
        container.transform.position = startPos;

        for (int i = 0; i < positions.Count; i++)
        {
            var proj = InstantiateRanged(GetProjectileOfFormatName("action08_1", true),
                container.transform.position + (Vector3)positions[i], container, 1);
            proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
                (new TimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff,6,30,100),
                    100,i);
        }

        //TODO: container进行DoTween

        float targetPos1 = dir > 0
            ? BattleStageManager.Instance.mapBorderR
            : BattleStageManager.Instance.mapBorderL;

        float targetPos2 = dir < 0
            ? BattleStageManager.Instance.mapBorderR + 10
            : BattleStageManager.Instance.mapBorderL - 10;

        float duration1 = distance1 / speed;
        float duration2 = distance2 / speed;

        Sequence mySequence = DOTween.Sequence();

        // 添加移动到targetPos1的动作
        mySequence.Append(container.transform.DOMoveX(targetPos1, duration1).SetEase(Ease.InSine)).SetUpdate(UpdateType.Fixed);

        // 添加移动到targetPos2的动作
        mySequence.Append(container.transform.DOMoveX(targetPos2, duration2).SetEase(Ease.OutSine)).SetUpdate(UpdateType.Fixed);

        // 添加销毁对象的动作
        mySequence.AppendCallback(() => {
            if(container != null)
                Destroy(container);
        });

        // 设置proj的自转
        container.transform.DOLocalRotate(new Vector3(0, 0, dir*180), 4, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);

        mySequence.Play();


    }

    protected void MemoryZenaHorizontal()
    {
        var container = InitContainer(false);

        var heights = new float[]
        {
            0,2,4,6,8,10,12,14,16,18
        };

        bool left = Random.Range(0, 2) == 0;

        for (int i = 0; i < 10; i++)
        {
            Vector2 position = Vector2.zero;
            int dir = 0;
            if ((i % 2 == 0 && left) || ((i % 2) == 1 && !left))
            {
                position = new Vector2(BattleStageManager.Instance.mapBorderL, heights[i]);
                dir = 1;
            }
            else
            {
                position = new Vector2(BattleStageManager.Instance.mapBorderR, heights[i]);
                dir = -1;
            }

            var proj =
                InstantiateRanged(GetProjectileOfFormatName("action09_1", true), position,
                    container, 1);
            proj.GetComponent<HomingAttack>().target = _behavior.targetPlayer.transform;
            proj.GetComponent<HomingAttack>().firedir = dir;



        }
        
        
    }

    protected void MemoryZenaScattered()
    {
        var borderL = BattleStageManager.Instance.mapBorderL;
        var borderR = BattleStageManager.Instance.mapBorderR;
        var borderT = BattleStageManager.Instance.mapBorderT;
        var borderB = BattleStageManager.Instance.mapBorderB - 6;

        var middleY = borderB + (borderT - borderB) / 2;

        List<Vector2> positionList = new()
        {
            new(borderL,middleY),
            new(borderR,middleY),
            new(0,borderB),
            new(0,borderT),
            new(borderL, borderT),
            new(borderL, borderB),
            new(borderR, borderT),
            new(borderR, borderB)
        };

        List<Vector2> angleList = new()
        {
            new(1, 0), new(-1, 0), new(0, 1), new(0, -1),
            new Vector2(1, -1).normalized, new Vector2(1, 1).normalized,
            new Vector2(-1, -1).normalized, new Vector2(-1, 1).normalized
        };

        var container = InitContainer(false,8);

        for (int i = 0; i < 8; i++)
        {
            var homingAttack =
                InstantiateRanged(GetProjectileOfFormatName("action09_1", true),
                    positionList[i], container, 1).GetComponent<HomingAttack>();

            homingAttack.target = _behavior.targetPlayer.transform;
            homingAttack.angle = angleList[i];
        }
        
        
    }

    protected void MemorySanzang1()
    {
        var container = InitContainer(false);

        var heights = new float[]
        {
            2,2,8,8,14,14,20,20
        };

        bool left = Random.Range(0, 2) == 0;

        for (int i = 0; i < 8; i++)
        {
            Vector2 position = Vector2.zero;
            int dir = 0;
            if ((i % 2 == 0 && left) || ((i % 2) == 1 && !left))
            {
                position = new Vector2(BattleStageManager.Instance.mapBorderL + 5, heights[i]);
                dir = 1;
            }
            else
            {
                position = new Vector2(BattleStageManager.Instance.mapBorderR - 5, heights[i]);
                dir = -1;
            }

            var hint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar
            (ac, position, RangedAttackFXLayer.transform,
                new Vector2(15, 4), Vector2.zero, false, 0, 3, 0, 0.5f,
                true, false);

            hint.transform.localScale = new Vector3(dir, 1, 1);

            DOVirtual.DelayedCall(3, () =>
            {
                var proj =
                    InstantiateRanged(GetProjectileOfFormatName("action10_1", true), 
                        position,
                        container, dir);
                proj.transform.localScale = new Vector3(dir, 1, 1);
                
            }, false);
            



        }

    }
    
    protected void MemorySanzang2()
    {
        var container = InitContainer(false);

        var heights = new float[]
        {
            2,2,8,8,14,14,20,20
        };

        bool left = Random.Range(0, 2) == 0;

        for (int i = 0; i < 8; i++)
        {
            Vector2 position = Vector2.zero;
            int dir = 0;
            if ((i % 2 == 0 && left) || ((i % 2) == 1 && !left))
            {
                position = new Vector2(BattleStageManager.Instance.mapBorderL, heights[i]);
                dir = 1;
            }
            else
            {
                position = new Vector2(BattleStageManager.Instance.mapBorderR, heights[i]);
                dir = -1;
            }

            var hint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar
            (ac, position, RangedAttackFXLayer.transform,
                new Vector2(15, 4), Vector2.zero, false, 0, 3, 0, 0.5f,
                true, false);

            hint.transform.localScale = new Vector3(dir, 1, 1);

            DOVirtual.DelayedCall(3, () =>
            {
                var proj =
                    InstantiateRanged(GetProjectileOfFormatName("action10_1", true), 
                        position,
                        container, dir);
                proj.transform.localScale = new Vector3(dir, 1, 1);
                
            }, false);
            



        }

    }

    protected void MemoryMagic1()
    {
        List<Vector2> positionList = new()
        {
            new(1,1),new(1,-1),new(-1,1),new(-1,-1)
        };

        var container = InitContainer(false);

        float interval = 0.2f;

        var prefab = GetProjectileOfFormatName("action11_2");

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                var delayTime = i * interval;
                var pos = (2 + 4 * i) * positionList[j] + (Vector2)transform.position;
                
                DOVirtual.DelayedCall(delayTime, () =>
                {
                    EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
                        pos, RangedAttackFXLayer.transform, 6, Vector2.zero, false, true,
                        1, .1f, 0.5f, true, true);
                }, false);
                
                DOVirtual.DelayedCall(delayTime + 1, () =>
                {
                    InstantiateRanged(prefab, pos, container, 1);
                }, false);

            }
            
            
        }
        
        
    }
    
    protected void MemoryMagic2()
    {
        List<Vector2> positionList = new()
        {
            new(1,0),new(-1,0),new(0,1),new(0,-1)
        };

        var container = InitContainer(false);

        float interval = 0.2f;

        var prefab = GetProjectileOfFormatName("action11_2");

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                var delayTime = i * interval;
                var pos = (2 + 4 * i) * positionList[j] + (Vector2)transform.position;
                
                DOVirtual.DelayedCall(delayTime, () =>
                {
                    EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
                        pos, RangedAttackFXLayer.transform, 6, Vector2.zero, false, true,
                        1, .1f, 0.5f, true, true);
                }, false);
                
                DOVirtual.DelayedCall(delayTime + 1, () =>
                {
                    InstantiateRanged(prefab, pos, container, 1);
                }, false);

            }
            
            
        }
        
        
    }

    protected void SummonZardin()
    {
        var enemy = SpawnEnemyMinon(GetProjectileOfFormatName("action13_1", true),
            gameObject.RaycastedPosition() + new Vector2(0,1.3f), 99999, 3000, ac.facedir, true);

        Projectile_DB004_1.Instance.targetPlayer = _behavior.targetPlayer;

    }
    protected void SummonFritz()
    {
        var enemy = SpawnEnemyMinon(GetProjectileOfFormatName("action13_2", true),
            new Vector3(0,_behavior.targetPlayer.RaycastedPlatform().bounds.max.y+1.3f),
            99999, 3000, ac.facedir, true);

        if (_behavior.targetPlayer.transform.position.x < 0)
        {
            enemy.GetComponent<EnemyController>().SetFaceDir(-1);
            enemy.transform.position = new Vector3(-Random.Range(1,4), enemy.transform.position.y, enemy.transform.position.z);
        }
        else
        {
            enemy.GetComponent<EnemyController>().SetFaceDir(1);
            enemy.transform.position = new Vector3(Random.Range(1,4), enemy.transform.position.y, enemy.transform.position.z);
        }
        
        
    }
    protected void SummonYachiyo()
    {
        var targetCollider2D = _behavior.targetPlayer.RaycastedPlatform();

        var playerPosX = _behavior.targetPlayer.transform.position.x;
        int dir;
        
        Vector2 targetPos = 
            new Vector2(targetCollider2D.bounds.center.x, targetCollider2D.bounds.max.y + 1.3f);

        if (playerPosX < targetPos.x)
        {
            targetPos.x = playerPosX + 4;
            dir = -1;
        }
        else
        {
            targetPos.x = playerPosX - 4;
            dir = 1;
        }

        var enemy = InstantiateSealedContainer(GetProjectileOfFormatName("action13_3",
            true), targetPos,RangedAttackFXLayer.transform,dir);
    }
    protected void SummonLucretia()
    {
        var targetPlatform = _behavior.targetPlayer.RaycastedPlatform();

        var targetDir = _behavior.targetPlayer.transform.localScale.x;
        var targetPosX = _behavior.targetPlayer.transform.position.x;
        
        var enemy = SpawnEnemyMinon(GetProjectileOfFormatName("action13_4", true),
            new Vector3(0,targetPlatform.bounds.max.y+1.3f),
            99999, 3300,-(int)targetDir, true);
        
        //生成在targetPosX的前方距离8的位置，并且限制在_beahvior.targetPlatform的范围内
        var posX = Mathf.Clamp(targetPosX + 9 * targetDir, targetPlatform.bounds.min.x, 
            targetPlatform.bounds.max.x);
        
        enemy.GetComponent<EnemyController>().SetFaceDir(-(int)targetDir);
        
        enemy.transform.position = new Vector3(posX, enemy.transform.position.y, enemy.transform.position.z);

    }

    protected void AllRangedAttack()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action19", true),
            transform.position + new Vector3(0, 3), InitContainer(false), 1);

        var atk = proj.GetComponent<ForcedAttackFromEnemy>();
        
        atk.AddConditionalAttackEffect(new ConditionalAttackEffect
            ((src, tar) =>
            {
                if(tar.HasCondition((int)BasicCalculation.BattleCondition.Paralysis))
                {
                    return true;
                }

                return false;
            }, ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,new string[]{},
                new string[]{ (7.5f+_behavior.difficulty*2.5f).ToString()}));




    }

    protected void AroundDoubleThunder(Vector2 raycastedPos)
    {
        var container = InitContainer(false, 2);

        var paralysis = new TimerBuff((int)BasicCalculation.BattleCondition.Paralysis, 108, 7, 1);
        
        //var raycastedPos = gameObject.RaycastedPosition();
        
        var proj1 = InstantiateRanged(GetProjectileOfFormatName("action21", true),
            raycastedPos + new Vector2(6f, 0), container, 1);
        
        var proj2 = InstantiateRanged(GetProjectileOfFormatName("action21", true),
            raycastedPos - new Vector2(6f, 0), container, -1);
        
        proj1.GetComponent<AttackFromEnemy>().AddWithConditionAll(paralysis,100);
        proj2.GetComponent<AttackFromEnemy>().AddWithConditionAll(paralysis,100);

    }
    
    
}
