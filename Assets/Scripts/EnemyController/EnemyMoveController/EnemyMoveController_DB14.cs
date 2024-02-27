using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMoveController_DB14 : EnemyMoveManager
{
    private VoiceControllerEnemy _voiceController;
    protected enum MyVoiceGroup
    {
        SkillGroup1 = 4,
        SkillGroup2 = 5,
        SkillGroup3 = 6,
        SuperMove = 7
    }

    protected override void Awake()
    {
        base.Awake();
        _voiceController = GetComponentInChildren<VoiceControllerEnemy>();
    }

    /// <summary>
    /// Dash
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB14_Action01()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        
        //anim.Play("charge_enter");
        
        var hint = GenerateWarningPrefab("action01", transform.position + new Vector3(ac.facedir,1,0),
            ac.facedir == 1?Quaternion.identity : Quaternion.Euler(0,180,0),
            RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBarRect2D>();

        yield return new WaitForSeconds(hint.warningTime - 0.25f);
        
        anim.Play("action02");

        yield return new WaitForSeconds(0.35f);

        DashAttack();

        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// Water Jet
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB14_Action02()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        //anim.Play("charge_enter");
        
        var hint = GenerateWarningPrefab("action02", transform.position + Vector3.up * 2,
            Quaternion.identity, MeeleAttackFXLayer.transform).GetComponent<EnemyAttackHintBar>();

        yield return new WaitForSeconds(2f);
        
        anim.Play("combo2");

        yield return new WaitForSeconds(0.8f);
        
        LaunchWaterJet();
        
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();

    }

    /// <summary>
    /// 侵蚀
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB14_Action03(int effect)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB14_Action03");


        yield return new WaitForSeconds(1f);
        
        anim.Play("charge_enter");
        if(_voiceController != null)
            _voiceController.BroadCastMyVoice((int)EnemyMoveController_DB14.MyVoiceGroup.SkillGroup2);

        yield return new WaitForSeconds(1f);
        
        CorrosionFog(effect);
        
        anim.Play("charge_exit");

        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }




    /// <summary>
    /// Double Slap
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB14_Action04()
    {
        yield return _canAction;
        ac.OnAttackEnter(_behavior.difficulty <= 1 ? 999 : 100);
        ac.TurnMove(_behavior.targetPlayer);
        
        //anim.Play("charge_enter");
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + Vector3.up * 6);

        yield return new WaitForSeconds(1f);
        
        anim.Play("action01");

        yield return new WaitForSeconds(0.2f);
        
        DoubleSlap(1);

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("action02");
        
        yield return new WaitForSeconds(0.2f);
        
        DoubleSlap(2);
        
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();

    }
    
    /// <summary>
    /// Buff
    /// </summary>
    /// <param name="buffEffect"></param>
    /// <returns></returns>
    public IEnumerator DB14_Action05(int buffEffect = 25)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_enter");

        yield return new WaitForSeconds(0.8f);
        
        anim.Play("charge_exit");
        
        
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            buffEffect, 15);
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
            buffEffect, 15);
        if(_voiceController!=null)
            _voiceController.BroadCastMyVoice((int)EnemyMoveController_DB14.MyVoiceGroup.SkillGroup1);
        
        

        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();
    }


    /// <summary>
    /// Around
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB14_Action06()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_enter");

        var hint = GenerateWarningPrefab("action06", gameObject.RaycastedPosition(),
            Quaternion.identity, RangedAttackFXLayer.transform);

        var position = hint.transform.position;

        var waitTime = hint.GetComponent<EnemyAttackHintBar>().warningTime;

        yield return new WaitForSeconds(waitTime - 0.3f);
        
        anim.Play("charge_exit");
        
        yield return new WaitForSeconds(0.3f);

        InstantiateRanged(GetProjectileOfFormatName("action06"), position, InitContainer(false), 1).
            GetComponent<AttackFromEnemy>().
            AddWithConditionAll(
                new TimerBuff((int)BasicCalculation.BattleCondition.Bog,
                    1,8,1,-1),80);
        
        yield return new WaitUntil
            (()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();
    }

    public IEnumerator DB14_Action07()
    {
        yield return _canAction;
        ac.OnAttackEnter();
        ac.SetGravityScale(0);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.5f);
        
        _voiceController.BroadCastMyVoice((int)EnemyMoveController_DB14.MyVoiceGroup.SkillGroup3);
        ScatteredWaterBalls_Hint(2.5f);

        yield return new WaitForSeconds(2.3f);
        
        anim.Play("combo2");

        yield return new WaitForSeconds(0.2f);
        
        ScatteredWaterBalls_Attack();
        
        yield return new WaitUntil
            (()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        ac.ResetGravityScale();
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 狂涛遣唤
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB14_Action08()
    {
        yield return _canActionOnFlyingGround;
        
        bossBanner?.PrintSkillName("DB14_Action08");
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(2);

        var orbList = SummonWaterOrbs();
        var waitTime = 6f;
        OrbBlast_Hint(orbList,waitTime);
        _voiceController?.BroadCastMyVoice((int)EnemyMoveController_DB14.MyVoiceGroup.SkillGroup2);

        DOVirtual.DelayedCall(waitTime, () => { OrbBlast(orbList);},false);

        yield return new WaitForSeconds(2);
        
        StageCameraController.SwitchMainCamera();
        
        yield return new WaitForSeconds(waitTime - 4.5f);
        
        yield return new WaitUntil
            (()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        //StageCameraController.SwitchMainCamera();
        ac.ResetGravityScale();
        anim.Play("idle");
        QuitAttack();
    }

    public IEnumerator DB14_Action08_2()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter();
        anim.Play("charge_enter");

        var hint = GenerateWarningPrefab("action08", transform.position + Vector3.up, Quaternion.identity,
            RangedAttackFXLayer.transform, 1);
        var spawnPos = hint.transform.position;

        yield return new WaitForSeconds(hint.GetComponent<EnemyAttackHintBar>().warningTime);
        
        anim.Play("charge_exit");

        InstantiateRanged(GetProjectileOfFormatName("action08_3"), spawnPos, InitContainer(false),
            1);

        yield return null;
        
        yield return new WaitUntil
            (()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        QuitAttack();
        
    }
    
    /// <summary>
    /// 巨浪招来
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB14_Action09(int hp, Vector2[] positions, float time)
    {
        yield return _canActionOnFlyingGround;
        
        bossBanner?.PrintSkillName("DB14_Action09");
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        StageCameraController.SwitchOverallCamera();
        _voiceController.BroadCastMyVoice((int)EnemyMoveController_DB14.MyVoiceGroup.SkillGroup3);

        yield return new WaitForSeconds(2);
        
        SummonMinions(hp,positions,time);
        anim.Play("combo2");

        yield return new WaitForSeconds(3f);
        
        StageCameraController.SwitchMainCamera();
        //ac.ResetGravityScale();
        anim.Play("idle");
        QuitAttack();
    }

    public IEnumerator DB14_Action10()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);

        Vector2[] positions = new[] { new Vector2(0, 15), new Vector2(-16, 5), new Vector2(16, 5) };

        foreach (var position in positions)
        {
            EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
                position, RangedAttackFXLayer.transform, 10, Vector2.zero, false,
                true, 4, 0.1f, 2f, true, false);
        }
        
        anim.Play("combo2");
        _voiceController.BroadCastMyVoice((int)EnemyMoveController_DB14.MyVoiceGroup.SuperMove);

        DOVirtual.DelayedCall(4, () =>
        {
            var container = InitContainer(false);
        
            foreach (var position in positions)
            {
                InstantiateRanged(GetProjectileOfFormatName("action10", true), position,
                        container, 1).GetComponent<AttackFromEnemy>().
                    AddWithConditionAll(
                        new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,220,21,1),
                        100);
            }
        });
        
        yield return new WaitUntil
            (()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        
        anim.Play("idle");
        QuitAttack();

    }
    
    
    
    
    


    protected void DashAttack()
    {
        var pos = BattleStageManager.Instance.
            OutOfRangeCheck(transform.position + new Vector3(15*ac.facedir,0,0));
        _tweener = transform.DOMoveX(pos.x, 0.4f).SetEase(Ease.InOutSine);
        
        var attackGo = InstantiateRanged(GetProjectileOfFormatName("action01_2"),
            transform.position + new Vector3(ac.facedir, 1, 0), InitContainer(false),
            ac.facedir);
        
        var fx = Instantiate(GetProjectileOfFormatName("action01_1"),
            transform.position + new Vector3(0, -1, 0), Quaternion.identity,
            MeeleAttackFXLayer.transform);
        
    }

    protected void LaunchWaterJet()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action02"),
            transform.position + new Vector3(ac.facedir, 2, 0), InitContainer(false),
            ac.facedir);
        
        var projScript = proj.GetComponentInChildren<BouncingProjectile>();
        projScript.SetVelocity(new Vector2(projScript.HorizontalVelocity * ac.facedir, 0));
    }

    protected void CorrosionFog(float healNeeded)
    {
        var fx = InstantiateRanged(GetProjectileOfFormatName("action03"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,transform.position.y-1),
            InitContainer(false),1);

        var corrosionEff = new AdvancedTimerBuff((int)BasicCalculation.BattleCondition.Corrosion,
            healNeeded, 8, 0, -1, 1, -1,8);
        
        fx.GetComponent<AttackFromEnemy>().AddWithConditionAll(corrosionEff,200);



        //_statusManager.InflictCorrosion(healNeeded,1);
    }


    protected void DoubleSlap(int slapID)
    {
        string prefabName = "action04_" + slapID;
        var fx = InstantiateMeele(GetProjectileOfFormatName(prefabName),
            transform.position, InitContainer(true));
        _tweener = transform.DOMoveX(transform.position.x + ac.facedir,0.2f).SetEase(Ease.InOutSine);
    }
    
    
    protected void ScatteredWaterBalls_Hint(float hintTime)
    {
        float startAngle = 22.5f;

        for (int i = 0; i < 8; i++)
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                transform.position + new Vector3(0,2), MeeleAttackFXLayer.transform, new Vector2(30, 2),
                Vector2.zero, false, 1, hintTime, startAngle);

            startAngle += 45f;
        }
    }

    protected void ScatteredWaterBalls_Attack(float speed = 6)
    {
        var container = InitContainer(false, 8);

        float[] angles = new[] { 0.392f, 1.18f, 1.96f, 2.75f, 3.53f, 4.317f, 5.1f, 5.89f };
        
        for (int i = 0; i < 8; i++)
        {
            var proj = InstantiateRanged(GetProjectileOfFormatName("action07"),
                transform.position + new Vector3(0,2), container,
                1);
            proj.GetComponent<ReflectionProjectile>().
                SetVelocity(new Vector2(speed * Mathf.Sin(angles[i]),speed * Mathf.Cos(angles[i])));
        }
    }

    protected List<GameObject> SummonWaterOrbs()
    {
        List<Vector2> positions = new List<Vector2>()
        {
            new Vector2(-24, 1),
            new Vector2(24, 1),
            new Vector2(-18, 7),
            new Vector2(18, 7),
            
            new Vector2(-12, 1),
            new Vector2(12, 1),
            new Vector2(0, 1),
            new Vector2(-6, 7),
            new Vector2(6, 7)
        };

        positions.RemoveAt(Random.Range(4, positions.Count));

        List<GameObject> projList = new();

        for (int i = 0; i < 8; i++)
        {
            projList.Add(Instantiate(GetProjectileOfFormatName("action08_1"),
                positions[i],Quaternion.identity,RangedAttackFXLayer.transform));
        }

        return projList;

    }
    
    protected void OrbBlast_Hint(List<GameObject> orbList, float time)
    {
        //var container = InitContainer(false, 8);
        for (int i = 0; i < orbList.Count; i++)
        {
            EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar
            (ac, orbList[i].transform.position, RangedAttackFXLayer.transform,
                5.5f, Vector2.zero, false, true, time,
                0.1f, 0.5f,true, false);
        }
    }

    protected void OrbBlast(List<GameObject> orbList)
    {
        var container = InitContainer(false, 8);
        for (int i = 0; i < orbList.Count; i++)
        {
            InstantiateRanged(GetProjectileOfFormatName("action08_2"), orbList[i].transform.position,
                container, 1);
            Destroy(orbList[i].gameObject);
        }
        orbList.Clear();
    }

    protected void SummonMinions(int hp, Vector2[] position, float time)
    {
        var go = Instantiate(GetProjectileOfFormatName("action09"), Vector3.zero, Quaternion.identity,
            RangedAttackFXLayer.transform);
        var controller = go.GetComponent<Projectile_DB014_1>();
        controller.enemySource = gameObject;

        foreach (var VARIABLE in position)
        {
            controller.AddMinion(
                SpawnEnemyMinon(GetProjectileOfFormatName("action09_minion", true),
                    VARIABLE, hp, 1, 1, true));
        }

        controller.StartWaterFall(time);
        controller.SetExtraDamage(_behavior.difficulty * 500);
    }


}
