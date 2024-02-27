using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMoveController_HB05 : EnemyMoveManager
{
    protected VoiceControllerEnemy voiceController;
    protected Tween dragondriveTween;
    protected float dragondriveGauge;
    protected GameObject dragondriveFXInstance;
    public enum MyVoiceGroup
    {
        Combo1,
        Combo3,
        Skill1,
        Skill2,
        Force,
        Transform,
        Dash,
        ExtraSkill,
        Defeat,
        Buff
    }
    protected override void Awake()
    {
        base.Awake();
        voiceController = GetComponentInChildren<VoiceControllerEnemy>();
        GetAllAnchors();
    }

    protected override void Start()
    {
        base.Start();
        InstantiateMist();
    }

    private void Update()
    {
        var behavior = _behavior as HB05_BehaviorTree;
        
        if(behavior.dragonDrive || !behavior.sealRemoved)
            return;

        if (dragondriveGauge < 40)
        {
            dragondriveGauge += 4 * Time.deltaTime;
        }
        else dragondriveGauge = 40;
    }


    public IEnumerator HB05_Action01_N()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter();

        BattleEffectManager.Instance.SpawnExclamation(gameObject, transform.position + new Vector3(0, 2.5f));

        yield return new WaitForSeconds(1f);

        anim.Play("combo1");

        yield return new WaitForSeconds(0.3f);

        voiceController?.PlayMyVoice((int)MyVoiceGroup.Combo1);
        ComboAttack(new Vector2(1,1),new Vector2(1,-1),new Vector2(0.5f,0));

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);

        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("combo2");

        yield return new WaitForSeconds(0.5f);
        
        ComboAttack(new Vector2(1,1.25f),new Vector2(1,-1.25f),new Vector2(0.75f,0.35f),new Vector2(0.75f,-0.35f));

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();

    }
    
    public IEnumerator HB05_Action01_B()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position,
            MeeleAttackFXLayer.transform, new Vector2(20, 3), Vector2.zero, false,
            0, 0.2f, 0, 1.8f, true, true, true);

        yield return new WaitForSeconds(1f);

        anim.Play("combo1");

        yield return new WaitForSeconds(0.3f);

        voiceController?.PlayMyVoice((int)MyVoiceGroup.Combo1);
        
        ComboAttackBoostedType1(1);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);

        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("combo2");

        yield return new WaitForSeconds(0.5f);
        
        ComboAttackBoostedType1(3);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();

    }
    
    public IEnumerator HB05_Action02_N()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter();

        BattleEffectManager.Instance.SpawnExclamation(gameObject, transform.position + new Vector3(0, 2.5f));

        yield return new WaitForSeconds(1f);

        anim.Play("combo3");

        yield return new WaitForSeconds(0.5f);

        voiceController?.PlayMyVoice((int)MyVoiceGroup.Combo3);
        ComboAttack(new Vector2(1,1.25f),new Vector2(1,-1.25f),new Vector2(0.75f,0.35f),new Vector2(0.75f,-0.35f));

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);

        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("combo4");

        yield return new WaitForSeconds(0.5f);
        
        ComboAttack(new Vector2(1,1.25f),new Vector2(1,-1.25f),new Vector2(0.75f,0.35f),new Vector2(0.75f,-0.35f));

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();

    }
    
    
    public IEnumerator HB05_Action02_B()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position,
            MeeleAttackFXLayer.transform, new Vector2(20, 3), Vector2.zero, false,
            0, 0.2f, 0, 1.8f, true, true, true);

        yield return new WaitForSeconds(0.5f);

        anim.Play("combo3");

        yield return new WaitForSeconds(0.5f);

        voiceController?.PlayMyVoice((int)MyVoiceGroup.Combo3);
        
        ComboAttackBoostType2();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);

        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("combo4");

        yield return new WaitForSeconds(0.5f);
        
        ComboAttackBoostType2();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();

    }


    public IEnumerator HB05_Action03_N()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action03");

        float fillTime = 1;

        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, gameObject.RaycastedPosition(),
            RangedAttackFXLayer.transform,
            new Vector2(20, 12), new Vector2(-0.5f,0f), false, 1, fillTime, 90,
            0.5f, true, true, true);

        yield return new WaitForSeconds(fillTime - 0.5f);
        
        anim.Play("s1");
        voiceController?.PlayMyVoice((int)MyVoiceGroup.Skill1);

        yield return new WaitForSeconds(0.5f);
        
        
        CocytusWhirl_Normal();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();

    }

    public IEnumerator HB05_Action03_B()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action03");
        
        BattleEffectManager.Instance.
            SpawnExclamation(gameObject,transform.position + new Vector3(0,2.5f));
        yield return new WaitForSeconds(1.2f);
        
        anim.Play("s1");
        voiceController?.PlayMyVoice((int)MyVoiceGroup.Skill1);

        yield return new WaitForSeconds(0.8f);

        CocytusWhirl_Boosted();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    
    /// <summary>
    /// 恶魔征服
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB05_Action04_N()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action04");
        
        StageCameraController.SwitchMainCameraFollowObject(gameObject);

        yield return new WaitForSeconds(1);
        
        anim.Play("s2");
        voiceController?.PlayMyVoice((int)MyVoiceGroup.Skill2);

        yield return new WaitForSeconds(1f);

        Instantiate(GetProjectileOfFormatName("action04_1"), gameObject.RaycastedPosition(),
            Quaternion.identity, RangedAttackFXLayer.transform);
        dragondriveGauge = 40;
        try
        {
            _statusManager.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.DemonSeal,
                -1, true);
        }
        catch
        {
            
        }
        
        StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }


    /// <summary>
    /// 恶魔征伐
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB05_Action04_B()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action04");
        
        yield return new WaitForSeconds(0.5f);

        if (_behavior.difficulty <= 2)
        {
            GenerateWarningPrefab("action04_2", gameObject.RaycastedPosition(), Quaternion.identity,
                RangedAttackFXLayer.transform, ac.facedir);
        }
        else
        {
            BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + new Vector3(0,2.5f));
        }
        
        yield return new WaitForSeconds(1);
        
        anim.Play("s2_boost");
        voiceController?.PlayMyVoice((int)MyVoiceGroup.Skill2);

        yield return new WaitForSeconds(0.5f);

        BanishEvilAttack();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 远程攻击
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB05_Action05(float extraInterval = 0)
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        //bossBanner?.PrintSkillName("HB05_Action03");
        

        var position = EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
            _behavior.targetPlayer.transform.position, RangedAttackFXLayer.transform,
            5, Vector2.zero, false, true, 1.2f + extraInterval, 0.1f,
            0.5f, true, false, true).transform.position;
        
        anim.Play("fs_enter");
        
        voiceController?.BroadCastMyVoice((int)MyVoiceGroup.Force);

        yield return new WaitForSeconds(.9f + extraInterval);
        
        anim.Play("fs_exit");

        yield return new WaitForSeconds(0.3f);
        
        ForceStrike(position);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();

    }

    /// <summary>
    /// 强袭
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB05_Action06()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        ac.SetHitSensor(false);
        bossBanner?.PrintSkillName("HB05_Action06");
        StageCameraController.SwitchMainCameraFollowObject(gameObject);

        anim.Play("transform");
        
        voiceController?.BroadCastMyVoice((int)MyVoiceGroup.Transform);

        yield return new WaitForSeconds(0.5f);

        Instantiate(GetProjectileOfFormatName("action06_1"), transform.position + new Vector3(0, 2),
            Quaternion.identity, RangedAttackFXLayer.transform);
        (_behavior as HB05_BehaviorTree).dragonDrive = true;
        dragondriveTween = DOVirtual.DelayedCall(Mathf.Max(15,dragondriveGauge), () =>
            HB05_Action06_C(), false);
        
        if (dragondriveFXInstance == null)
        {
            dragondriveFXInstance = Instantiate(GetProjectileOfFormatName("action06_2"),
                gameObject.RaycastedPosition(), Quaternion.identity, BuffFXLayer.transform);
        }
        else
        {
            dragondriveFXInstance.SetActive(true);
        }


        yield return new WaitForSeconds(2);
        
        StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
        ac.SetHitSensor(true);
        //anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 强袭解除
    /// </summary>
    /// <returns></returns>
    public void HB05_Action06_C()
    {
        dragondriveTween?.Kill();
        (_behavior as HB05_BehaviorTree).dragonDrive = false;
        Instantiate(GetProjectileOfFormatName("action01_2"), transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);
        dragondriveFXInstance?.SetActive(false);
    }
    
    /// <summary>
    /// 冥河涌泉
    /// </summary>
    /// <param name="enhanced"></param>
    /// <returns></returns>
    public IEnumerator HB05_Action07(bool enhanced = false)
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action07");
        
        yield return new WaitForSeconds(0.5f);

        var positionList = AcheronFountHint(10,!enhanced);
        DOVirtual.DelayedCall(2.1f, 
            () => AcheronFountAttack(positionList, !enhanced), false);
        
        yield return new WaitForSeconds(1);
        
        anim.Play("s2_boost");
        voiceController?.BroadCastMyVoice((int)MyVoiceGroup.Dash);

        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// 瞬移
    /// </summary>
    /// <param name="warpPosition"></param>
    /// <returns></returns>
    public IEnumerator HB05_Action08(Vector2 warpPosition)
    {
        yield return _canAction;
        
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        WarpEffect();
        yield return new WaitForSeconds(0.3f);
        
        ac.DisappearRenderer();
        
        transform.position = warpPosition.SafePosition(Vector2.zero);
        WarpEffect();
        yield return new WaitForSeconds(0.3f);
        
        ac.AppearRenderer();
        ac.TurnMove(_behavior.viewerPlayer);
        
        QuitAttack();
    }



    /// <summary>
    /// 魔咒
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB05_Action09()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action09");

        //yield return new WaitForSeconds(1);
        
        anim.Play("s2");
        //voiceController?.BroadCastMyVoice((int)MyVoiceGroup.Dash);

        yield return new WaitForSeconds(0.5f);
        
        SetMagicCircle();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }


    /// <summary>
    /// 落冰
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB05_Action10()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action10");

        //yield return new WaitForSeconds(1);
        
        anim.Play("s2");
        //voiceController?.BroadCastMyVoice((int)MyVoiceGroup.Dash);

        yield return new WaitForSeconds(0.5f);
        
        TopdownIce();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// Mist
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB05_Action11()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action11");
        
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(1);
        
        anim.Play("transform");
        voiceController?.BroadCastMyVoice((int)MyVoiceGroup.ExtraSkill);

        yield return new WaitForSeconds(0.1f);

        Projectile_C007_2_Boss.Instance.StartFogEffectRandom();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);

        yield return new WaitForSeconds(2);
        
        StageCameraController.SwitchMainCamera();
        anim.Play("idle");
        QuitAttack();
    }


    /// <summary>
    /// snowStorm
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB05_Action12()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action12");
        
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(1);
        
        anim.Play("transform");
        //voiceController?.BroadCastMyVoice((int)MyVoiceGroup.ExtraSkill);
        Instantiate(GetProjectileOfFormatName("action12_1"), transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(0.1f);

        Projectile_C007_2_Boss.Instance.StartStorm();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        
        yield return new WaitForSeconds(1f);
        StageCameraController.SwitchMainCamera();
        
        anim.Play("idle");
        
        
        QuitAttack();
    }
    
    
    public IEnumerator HB05_Action13()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB05_Action13");

        //yield return new WaitForSeconds(1);
        
        anim.Play("s2");
        voiceController?.BroadCastMyVoice((int)MyVoiceGroup.Buff);

        yield return new WaitForSeconds(0.5f);

        Instantiate(GetProjectileOfFormatName("action13_1", true),
            gameObject.RaycastedPosition(), Quaternion.identity, RangedAttackFXLayer.transform);

        var buff = _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff, 
            _behavior.difficulty <= 2 ? 30 : 40 * _behavior.difficulty - 20, 20,
            1, 8105201);
        buff.dispellable = false;
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            5 * _behavior.difficulty, -1, 1, 8105202);
        var kbres = _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.KnockBackImmune, 1,
            20, 1, -1);
        kbres.dispellable = false;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    

    protected void ComboAttack(params Vector2[] offset)
    {
        var muzzleFX = Instantiate(GetProjectileOfFormatName("action01_2"),
            transform.position + new Vector3(ac.facedir, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);

        var container = InitContainer(false,offset.Length);

        for (int i = 0; i < offset.Length; i++)
        {
            var proj = InstantiateRanged(GetProjectileOfFormatName("action01_1"),
                transform.position + new Vector3(offset[i].x*ac.facedir, offset[i].y), container, 1);

            if ((_behavior.targetPlayer.transform.position.x >= transform.position.x && ac.facedir == 1)||
                (_behavior.targetPlayer.transform.position.x <= transform.position.x && ac.facedir == -1))
            {
                proj.GetComponent<HomingAttackWithoutRotate>().target = _behavior.targetPlayer.transform;
                
            }
            
            proj.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0);
            
            
        }
    }

    protected void ComboAttackBoostedType1(int num = 1)
    {
        var muzzleFX = Instantiate(GetProjectileOfFormatName("action01_2"),
            transform.position + new Vector3(ac.facedir, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);

        var container = InitContainer(false,num);
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action01_3"),
            transform.position + new Vector3(1.2f*ac.facedir, 0), container,
            ac.facedir);

        if (num > 1)
        {
            var proj2 = InstantiateRanged(GetProjectileOfFormatName("action01_3"),
                transform.position + new Vector3(1f*ac.facedir, -0.75f), container,
                ac.facedir);
            var pro3 = InstantiateRanged(GetProjectileOfFormatName("action01_3"),
                transform.position + new Vector3(1f*ac.facedir, 0.75f), container,
                ac.facedir);
        }
    }

    protected void ComboAttackBoostType2()
    {
        var muzzleFX = Instantiate(GetProjectileOfFormatName("action01_2"),
            transform.position + new Vector3(ac.facedir, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);

        var container = InitContainer(false);
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action01_4"),
            transform.position + new Vector3(ac.facedir, 0), container,
            ac.facedir);
        
    }

    protected void CocytusWhirl_Normal()
    {
        var proj = InstantiateMeele(GetProjectileOfFormatName("action03_1"),
            gameObject.RaycastedPosition(), InitContainer(true));
        
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
            (new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,41,
                21,1),110);
        
    }

    protected void CocytusWhirl_Boosted()
    {
        var proj = Instantiate(GetProjectileOfFormatName("action03_2"),
            gameObject.RaycastedPosition(), Quaternion.identity,RangedAttackFXLayer.transform);

        var atk = proj.GetComponent<Projectile_C007_1>();
        atk.SetSource(gameObject);
        
    }

    protected void BanishEvilAttack()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action04_2"),
            gameObject.RaycastedPosition()+new Vector2(2*ac.facedir,-1),
            InitContainer(false),ac.facedir);
        dragondriveGauge += 2.5f;
    }

    protected void ForceStrike(Vector2 position)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action05_1"),
            position, InitContainer(false),1);
        
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
            (new TimerBuff((int)BasicCalculation.BattleCondition.Freeze,1,
                Random.Range(3f,4f),1),100);
    }

    protected List<Vector3> AcheronFountHint(float intervalDistance,bool avoidable = true)
    {
        var playerPosition = _behavior.targetPlayer.transform.position;

        var positionL1 = playerPosition - new Vector3(intervalDistance, 0);
        var positionL2 = playerPosition - new Vector3(intervalDistance * 2, 0);
        var positionR1 = playerPosition + new Vector3(intervalDistance, 0);
        var positionR2 = playerPosition + new Vector3(intervalDistance * 2, 0);

        if (positionL1.x <= BattleStageManager.Instance.mapBorderL)
        {
            positionL1 = playerPosition + new Vector3(intervalDistance * 3, 0);
            positionL2 = playerPosition + new Vector3(intervalDistance * 4, 0);
        }else if (positionL2.x <= BattleStageManager.Instance.mapBorderL)
        {
            positionL2 = playerPosition + new Vector3(intervalDistance * 3, 0);
        }else if (positionR1.x >= BattleStageManager.Instance.mapBorderR)
        {
            positionR1 = playerPosition - new Vector3(intervalDistance * 3, 0);
            positionR2 = playerPosition - new Vector3(intervalDistance * 4, 0);
        }else if (positionR2.x >= BattleStageManager.Instance.mapBorderR)
        {
            positionR2 = playerPosition - new Vector3(intervalDistance * 3, 0);
        }

        positionL1.y = BattleStageManager.Instance.mapBorderB;
        positionL2.y = BattleStageManager.Instance.mapBorderB;
        positionR1.y = BattleStageManager.Instance.mapBorderB;
        positionR2.y = BattleStageManager.Instance.mapBorderB;
        playerPosition.y = BattleStageManager.Instance.mapBorderB;
        
        var posList = new List<Vector3> { positionL1,positionL2,positionR1,positionR2,playerPosition };

        foreach (var pos in posList)
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, pos,
                RangedAttackFXLayer.transform, new Vector2(10, 4), Vector2.zero,
                avoidable, 1, 2, 90, 0.5f, true, false,
                true);
        }

        return posList;

    }

    protected void AcheronFountAttack(List<Vector3> posList,bool avoidable)
    {
        var moveDir = Random.Range(0, 2) == 0 ? 1 : -1;
        if (_behavior.targetPlayer.transform.position.x < BattleStageManager.Instance.mapBorderL + 10)
        {
            moveDir = -1;
        }
        else if (_behavior.targetPlayer.transform.position.x > BattleStageManager.Instance.mapBorderR - 10)
        {
            moveDir = 1;
        }
        
        
        var container = InitContainer(false);
        foreach (var pos in posList)
        {
            var proj = InstantiateRanged(GetProjectileOfFormatName("action07_1"),
                pos, container, 1);
            proj.GetComponent<DOTweenSimpleController>().moveDirection.x *= moveDir;
            proj.GetComponent<AttackFromEnemy>().AddWithConditionAll
                (new TimerBuff((int)BasicCalculation.BattleCondition.Bog,1,8,1),
                    100);
            if (!avoidable)
            {
                proj.GetComponent<AttackFromEnemy>().ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
            }
        }
    }

    protected void WarpEffect()
    {
        Instantiate(GetProjectileOfFormatName("action08_1", true),
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
    }

    protected void TopdownIce()
    {
        var hint = GenerateWarningPrefab("action10",_behavior.targetPlayer.transform.position,
            Quaternion.identity,RangedAttackFXLayer.transform);
        var time = hint.GetComponent<EnemyAttackHintBar>().warningTime;
        hint.GetComponent<Projectile_C007_4_Boss>().target = _behavior.targetPlayer;
        DOVirtual.DelayedCall(time, () =>
        {
            InstantiateSealedContainer(GetProjectileOfFormatName("action10_1", true),
                hint.transform.position,RangedAttackFXLayer.transform);
            Destroy(hint,0.5f);
        }, false);
        
    }

    protected void SetMagicCircle()
    {
        var fx = InstantiateSealedContainer(GetProjectileOfFormatName("action09_1", true),
            _behavior.targetPlayer.transform.position,RangedAttackFXLayer.transform);

        var controller = fx.GetComponent<Projectile_C007_6_Boss>();
        
        fx.GetComponent<RelativePositionRetainer>().SetParent(_behavior.targetPlayer.transform);
        controller.SetEnemySource(gameObject);
        controller.SetTarget(_behavior.targetPlayer);
        controller.difficulty = _behavior.difficulty;

    }

    protected void InstantiateMist()
    {
        if (Projectile_C007_2_Boss.Instance == null)
        {
            var fx = Instantiate(GetProjectileOfFormatName("action11_1", true),
                Vector3.zero, Quaternion.identity,RangedAttackFXLayer.transform);
            fx.GetComponent<Projectile_C007_2_Boss>().SetEnemySource(gameObject);
        }
        
    }
    
    
}
