using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMoveController_H005 : EnemyMoveManager
{
    private VoiceControllerEnemy _voice;
    private List<Vector2> crystalPositionPresets = new();
    private List<GameObject> stoneInstances = new();

    public bool BurningOn { get; private set; } = false;
    public bool PhoenixOn { get; private set; } = false;
    private int phoenixCount = 0;
    
    private GameObject phoenixEffectInstance;

    private enum VoiceGroup
    {
        Transform,
        Intro,
        HPBelow70,
        HPBelow40,
        Defeated,
        Skill1,
        Skill2,
        Nihil,
        Corrosion,
        Rage
    }

    protected override void Start()
    {
        base.Start();
        _voice = GetComponentInChildren<VoiceControllerEnemy>();
        GetAllAnchors();
        _statusManager.ImmuneToAllControlAffliction = true;

        _statusManager.AddEffectFunction(SpecialResistanceBuff, AbilityCalculation.ProductArea.DMGCUT);
        //_statusManager.SpecialDamageCutEffectFunc += SpecialResistanceBuff;

        _statusManager.OnAfflictionGuarded += CheckResistanceBuff;
        _statusManager.OnBuffEventDelegate += CheckResistanceBuff;
        _statusManager.OnBuffEventDelegate += GainAtkBuff;
        BattleStageManager.Instance.specialEventTriggered += ClearStone;


    }

    private void OnDestroy()
    {
        BattleStageManager.Instance.specialEventTriggered -= ClearStone;
    }

    public IEnumerator H005_Action01()
    {
        yield return _canAction;
        _behavior.controllAfflictionProtect = true;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("H005_Action01");

        _voice?.BroadCastMyVoice((int)VoiceGroup.Nihil);

        yield return new WaitForSeconds(0.5f);

        anim.Play("roar");
        yield return new WaitForSeconds(1.5f);

        NihilAOE();
        _behavior.controllAfflictionProtect = false;

        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("idle");

        QuitAttack();

    }

    public IEnumerator H005_Action02(float effect)
    {
        yield return _canAction;
        _behavior.controllAfflictionProtect = true;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("H005_Action02");

        _voice?.BroadCastMyVoice((int)VoiceGroup.Corrosion);

        yield return new WaitForSeconds(0.5f);

        anim.Play("roar");
        yield return new WaitForSeconds(1.5f);

        NihilCorrosionAOE(effect);
        _behavior.controllAfflictionProtect = false;

        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("idle");

        QuitAttack();

    }

    public IEnumerator H005_Action03()
    {
        yield return _canAction;
        _behavior.controllAfflictionProtect = true;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("H005_Action03");

        _voice?.BroadCastMyVoice((int)VoiceGroup.Nihil);

        anim.Play("roar");
        yield return new WaitForSeconds(2f);

        if (!BurningOn)
        {
            FlameShieldOn();
        }
        else
        {
            _statusManager.ObtainHealOverTimeBuff(2, 15, true);
            _statusManager.ObtainHealOverTimeBuff(2, 15, true);
        }


        _behavior.controllAfflictionProtect = false;

        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("idle");

        QuitAttack();

    }
    
    public IEnumerator H005_Action04()
    {
        yield return _canAction;
        _behavior.controllAfflictionProtect = true;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("H005_Action04");

        _voice?.BroadCastMyVoice((int)VoiceGroup.Nihil);

        anim.Play("roar");
        yield return new WaitForSeconds(2f);
        
        ResistanceOn(5);
        
        _behavior.controllAfflictionProtect = false;

        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);

        anim.Play("idle");

        QuitAttack();

    }

    public IEnumerator H005_Action05(bool around = false)
    {
        yield return _canAction;

        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);

        if (_voice.voice.isPlaying == false)
        {
            _voice?.BroadCastMyVoice((int)VoiceGroup.Skill2);
        }

        anim.Play("punch");

        yield return new WaitForSeconds(1.2f);

        FireWave();

        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));

        if (around)
        {
            ac.SetKBRes(999);
            ac.SetCounter(false);
            anim.Play("roar");

            EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac, transform.position + new Vector3(0, 7),
                RangedAttackFXLayer.transform, 10, Vector2.zero, false, true, 1.2f, 0.15f, 0.85f,
                true);

            yield return new WaitForSeconds(1.25f);

            AroundBurn();

            yield return new WaitUntil
                (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
            
            anim.Play("idle");

        }

        QuitAttack();


    }

    /// <summary>
    /// Chasing Crystal
    /// </summary>
    /// <returns></returns>
    public IEnumerator H005_Action06()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H005_Action06");

        if (_voice && _voice.voiceGroups.Count > 0)
        {
            if (!_voice.voice.isPlaying)
                _voice.BroadCastMyVoice((int)VoiceGroup.Skill1);
        }

        yield return new WaitForSeconds(.5f);

        anim.Play("roar");
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer, 2f);

        yield return new WaitForSeconds(1f);


        var hint = GenerateWarningPrefab("action06_1", _behavior.targetPlayer.RaycastedPosition() + Vector2.up * 1.5f,
            Quaternion.identity, RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBarRect2D>();

        yield return new WaitForSeconds(hint.warningTime);

        var pos = hint.transform.position;
        SetFireCrystalAdvanced(hint.transform.position);

        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();


    }

    /// <summary>
    /// Cross punch
    /// </summary>
    /// <returns></returns>
    public IEnumerator H005_Action07(bool crossFirst)
    {
        yield return _canAction;

        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);

        if (_voice.voice.isPlaying == false)
        {
            //_voice?.BroadCastMyVoice((int)VoiceGroup.Skill2);
        }

        anim.Play("swing_1");
        int type = Random.Range(0, 2);
        
        if(crossFirst)
            type = 0;

        yield return null;

        if (type == 0)
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position + new Vector3(ac.facedir * 2, 2),
                RangedAttackFXLayer.transform, new Vector2(36, 8), new Vector2(-18, 0),
                false, 1, 2, 0, 0.5f, true, true, true, 0.1f,
                true);

            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position + new Vector3(ac.facedir * 2, 2),
                RangedAttackFXLayer.transform, new Vector2(36, 8), new Vector2(-18, 0),
                false, 1, 2, 90, 0.5f, true, true, true, 0.1f,
                true);

        }
        else
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position + new Vector3(ac.facedir * 2, 2),
                RangedAttackFXLayer.transform, new Vector2(36, 8),
                new Vector2(-18, 0),
                false, 1, 2, 45, 0.5f, true, true, true, 0.1f, true);

            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position + new Vector3(ac.facedir * 2, 2),
                RangedAttackFXLayer.transform, new Vector2(36, 8),
                new Vector2(-18, 0),
                false, 1, 2, -45, 0.5f, true, true, true, 0.1f, true);
        }

        yield return new WaitForSeconds(1.2f);

        anim.Play("swing_3");

        yield return new WaitForSeconds(0.8f);

        var flashburn = new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
            45, 21, 1);
        var scorchrend = new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
            45, 21, 1);

        var debuff = Random.Range(0, 2) == 0 ? flashburn : scorchrend;
        var container = InitContainer(false, 2);

        if (type == 0)
        {
            FlameCross(0, container, false, debuff);
        }
        else
        {
            FlameCross(45, container, false, debuff);
        }

        if (type == 1)
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position + new Vector3(ac.facedir * 2, 2),
                RangedAttackFXLayer.transform, new Vector2(36, 8), new Vector2(-18, 0),
                true, 1, 2, 0, 0.5f, true, true, true, 0.1f, true);

            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position + new Vector3(ac.facedir * 2, 2),
                RangedAttackFXLayer.transform, new Vector2(36, 8), new Vector2(-18, 0),
                true, 1, 2, 90, 0.5f, true, true, true, 0.1f, true);

        }
        else
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position + new Vector3(ac.facedir * 2, 2),
                RangedAttackFXLayer.transform, new Vector2(36, 8),
                new Vector2(-18, 0),
                true, 1, 2, 45, 0.5f, true, true, true, 0.1f, true);

            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position + new Vector3(ac.facedir * 2, 2),
                RangedAttackFXLayer.transform, new Vector2(36, 8),
                new Vector2(-18, 0),
                true, 1, 2, -45, 0.5f, true, true, true, 0.1f, true);
        }

        yield return new WaitForSeconds(2f);

        anim.Play("swing_5");

        if (type == 0)
        {
            FlameCross(45, container, true, debuff);
        }
        else
        {
            FlameCross(0, container, true, debuff);
        }

        yield return null;

        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("idle");

        QuitAttack();


    }



    public IEnumerator H005_Action08()
    {
        yield return _canAction;

        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        if (_voice.voice.isPlaying == false)
        {
            _voice?.BroadCastMyVoice((int)VoiceGroup.Skill1);
        }

        anim.Play("swing_1");

        ChaserFlame();

        int type = Random.Range(0, 2);

        if (type == 0)
        {
            GenerateWarningPrefab("action08_3", new Vector3(0, 10), Quaternion.identity,
                RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action08_3V", new Vector3(0, 10), Quaternion.identity,
                RangedAttackFXLayer.transform);
        }
        else
        {
            GenerateWarningPrefab("action08_2", new Vector3(12, 0), Quaternion.identity,
                RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action08_2", new Vector3(12, 16), Quaternion.identity,
                RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action08_2", new Vector3(-12, 0), Quaternion.identity,
                RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action08_2", new Vector3(-12, 16), Quaternion.identity,
                RangedAttackFXLayer.transform);
        }

        yield return new WaitForSeconds(1.5f);

        anim.Play("swing_3");

        yield return new WaitForSeconds(0.5f);

        FlameCrossAllRanged(type);
        ChaserFlame();

        if (type != 0)
        {
            GenerateWarningPrefab("action08_3", new Vector3(0, 10), Quaternion.identity,
                RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action08_3V", new Vector3(0, 10), Quaternion.identity,
                RangedAttackFXLayer.transform);
        }
        else
        {
            GenerateWarningPrefab("action08_2", new Vector3(12, 0), Quaternion.identity,
                RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action08_2", new Vector3(12, 16), Quaternion.identity,
                RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action08_2", new Vector3(-12, 0), Quaternion.identity,
                RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action08_2", new Vector3(-12, 16), Quaternion.identity,
                RangedAttackFXLayer.transform);
        }

        yield return new WaitForSeconds(2f);

        FlameCrossAllRanged(type == 0 ? 1 : 0);
        ChaserFlame();

        anim.Play("swing_5");

        yield return null;
        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);


        anim.Play("idle");

        QuitAttack();


    }

    /// <summary>
    /// 红炎烈火
    /// </summary>
    /// <returns></returns>
    public IEnumerator H005_Action09()
    {
        yield return _canAction;

        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H005_Action09");

        if (_voice.voice.isPlaying == false)
        {
            _voice?.BroadCastMyVoice((int)VoiceGroup.Skill2);
        }

        anim.Play("swing_1");

        var hintbar = GenerateWarningPrefab("action09_1", transform.position + new Vector3(0, 7),
            ac.facedir == 1 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0),
            RangedAttackFXLayer.transform);
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer, 3);

        hintbar.GetComponent<EnemyAttackHintBarRotater>().target = _behavior.targetPlayer;

        var warningTime = hintbar.GetComponent<EnemyAttackHintBar>().warningTime;

        yield return new WaitForSeconds(warningTime - 0.5f);

        anim.Play("swing_3");

        yield return new WaitForSeconds(0.5f);

        FlameBallShoot(hintbar);

        yield return new WaitForSeconds(3f);

        anim.Play("swing_5");

        yield return null;
        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);


        anim.Play("idle");

        QuitAttack();


    }


    /// <summary>
    /// 热浪焰晶
    /// </summary>
    /// <returns></returns>
    public IEnumerator H005_Action10()
    {
        yield return _canAction;

        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H005_Action10");

        _voice?.BroadCastMyVoice((int)VoiceGroup.Rage);

        SetTwoFireCrystalAndTwoStone();

        yield return new WaitForSeconds(3f);

        anim.Play("swing_1");

        var hintbar = AimingFire();
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer, 3f);
        var waitTime = hintbar.GetComponent<EnemyAttackHintBarCircle>().warningTime;

        yield return new WaitForSeconds(waitTime - 0.8f);

        anim.Play("swing_3");
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(0.8f);

        CheckOverlap(hintbar.transform.eulerAngles);

        //第二发

        yield return new WaitForSeconds(1f);

        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer, 3f);
        hintbar = AimingFire();
        anim.Play("swing_5");

        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f);

        anim.Play("swing_1");
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(hintbar.GetComponent<EnemyAttackHintBarCircle>().warningTimeLeft - 0.8f);

        anim.Play("swing_3");
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(0.8f);

        CheckOverlap(hintbar.transform.eulerAngles);

        yield return new WaitForSeconds(1f);

        anim.Play("swing_5");

        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);


        anim.Play("idle");

        QuitAttack();


    }

    /// <summary>
    /// 不灭焰晶
    /// </summary>
    /// <returns></returns>
    public IEnumerator H005_Action11()
    {
        yield return _canAction;

        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H005_Action11");
        StageCameraController.SwitchOverallCamera();

        _voice?.BroadCastMyVoice((int)VoiceGroup.Rage);

        SetFourStone();
        
        yield return new WaitForSeconds(3.5f);
        
        anim.Play("roar");
        var forcedAtk = AllRangeBlastEffect();
        StageCameraController.SwitchMainCamera();
        
        yield return new WaitForSeconds(1.6f);

        AllRangeBlast(forcedAtk);
        
        yield return new WaitForSeconds(1.5f);

        AllRangeBlast(forcedAtk);

        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        


        anim.Play("idle");

        QuitAttack();


    }

    
    /// <summary>
    /// 岩浆
    /// </summary>
    /// <returns></returns>
    public IEnumerator H005_Action12()
    {
        yield return _canAction;

        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        StageCameraController.SwitchOverallCamera();

        anim.Play("swing_1");

        int type = Random.Range(0, 2);
        
        float posY1 = 0;
        float posY2 = 7;
        float posY3 = 14;

        if (type == 1)
        {
            LavaCarpet(posY1, 1);
            LavaCarpet(posY3, 1);
        }
        else
        {
            LavaCarpet(posY1, -1);
            LavaCarpet(posY3, -1);
        }
        
        yield return new WaitForSeconds(1f);

        if (type == 1)
        {
            LavaCarpet(posY2, -1);
        }
        else
        {
            LavaCarpet(posY2, 1);
        }
        
        yield return new WaitForSeconds(2f);
        
        anim.Play("swing_3");
        
        yield return new WaitForSeconds(2f);
        
        anim.Play("swing_5");
        StageCameraController.SwitchMainCamera();

        yield return null;

        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");

        QuitAttack();


    }
    
    
    /// <summary>
    /// 冲浪
    /// </summary>
    /// <returns></returns>
    public IEnumerator H005_Action13()
    {
        yield return _canAction;

        ac.OnAttackEnter(999);
        
        BattleStageManager.Instance.TriggerSpecialEvent(1);
        bossBanner?.PrintSkillName("H005_Action13");
        StageCameraController.SwitchOverallCamera();
        
        int type = Random.Range(0, 2); //0:Stone 1:Fire
        
        var stonePos = SetOneStoneOrFireCrystal(type);

        if (type != 0)
        {
            if (Random.Range(0, 2) == 0)
            {
                ac.SetFaceDir(1);
            }else
            {
                ac.SetFaceDir(-1);
            } 
        }
        else
        {
            ac.SetFaceDir(stonePos.x > 0 ? -1 : 1);
        }
        
        
        yield return new WaitForSeconds(2);

        anim.Play("swing_1");

        if (ac.facedir == 1)
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                new Vector3(BattleStageManager.Instance.mapBorderL - 20,
                    BattleStageManager.Instance.mapBorderT + 20), RangedAttackFXLayer.transform,
                new Vector2(99, 99), Vector2.zero, false, 0, 2.5f, -30,
                1f, true, false);
        }
        else
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                new Vector3(BattleStageManager.Instance.mapBorderR + 20,
                    BattleStageManager.Instance.mapBorderT + 20), RangedAttackFXLayer.transform,
                new Vector2(99, 99), Vector2.zero, false, 0, 2.5f, -150,
                1f, true, false);
        }
        

        yield return new WaitForSeconds(1.25f);
        
        anim.Play("swing_3");

        yield return new WaitForSeconds(1.25f);
        
        LavaTsunami((int)Mathf.Sign(stonePos.x));
        
        StageCameraController.SwitchMainCamera();
        
        yield return new WaitForSeconds(0.5f);
        
        BattleStageManager.Instance.TriggerSpecialEvent(1);
        BattleStageManager.Instance.TriggerSpecialEvent(3);
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("swing_5");

        yield return null;

        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        if (type == 0)
        {
            //explosion
            anim.Play("roar");
            
            var fx = AllRangeBlastEffect("action13_4");
            
            yield return new WaitForSeconds(1.6f);
            
            AllRangeBlast(fx);
        }
        else
        {
            anim.Play("roar");
            
            RingAttack(stonePos);
            
            yield return new WaitForSeconds(1.6f);
            //ring burst
        }
        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");

        QuitAttack();


    }
    
    public IEnumerator H005_Action14()
    {
        yield return _canAction;
        _behavior.controllAfflictionProtect = true;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        if(!_voice.voice.isPlaying)
            _voice?.BroadCastMyVoice((int)VoiceGroup.Corrosion);

        yield return new WaitForSeconds(0.5f);

        anim.Play("roar");
        
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,new Vector3(0,12), RangedAttackFXLayer.transform,
            new Vector2(48,30), new Vector2(-24,0),true, 
            1, 1.25f, 0, 0.5f,true, true);
        
        yield return new WaitForSeconds(1.3f);

        Combustion();
        
        _behavior.controllAfflictionProtect = false;

        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("idle");

        QuitAttack();

    }
    
    public IEnumerator H005_Action15()
    {
        yield return _canAction;
        _behavior.controllAfflictionProtect = true;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H005_Action15");

        if(!_voice.voice.isPlaying)
            _voice?.BroadCastMyVoice((int)VoiceGroup.Rage);

        yield return new WaitForSeconds(0.5f);

        anim.Play("roar");
        
        yield return new WaitForSeconds(0.5f);
        
        SetEightFireCrystals();
        
        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("idle");

        QuitAttack();

    }
    
    public IEnumerator H005_Action16()
    {
        yield return _canAction;

        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H005_Action16");

        if (!_voice.voice.isPlaying)
        {
            _voice?.BroadCastMyVoice((int)VoiceGroup.Rage);
        }
        
        anim.Play("swing_1");
        
        var info = SetBlocks(0,1.5f);

        yield return new WaitForSeconds(3.05f);
        
        anim.Play("swing_3");
        
        BlockBlast(info.hintbars,info.avoidables);

        yield return null;
        
        info = SetBlocks(1,1.5f,info.preset == 0 ? 1 : 0);
        
        yield return new WaitForSeconds(3.05f);
        
        anim.Play("swing_5");
        
        BlockBlast(info.hintbars,info.avoidables);

        yield return null;

        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");

        QuitAttack();

    }
    
    public IEnumerator H005_Action17()
    {
        yield return _canAction;

        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("H005_Action17");
        StageCameraController.SwitchOverallCamera();

        if (!_voice.voice.isPlaying)
        {
            _voice?.BroadCastMyVoice((int)VoiceGroup.Rage);
        }
        
        anim.Play("swing_1");
        
        SetFourTallStone();
        FirePillarOn();
        
        yield return new WaitForSeconds(1f);

        anim.Play("swing_3");
        
        yield return new WaitForSeconds(1.5f);
        
        StageCameraController.SwitchMainCamera();
        
        yield return new WaitForSeconds(1.5f);
        
        ChaserFlameSpecial();
        
        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("swing_5");
        
        yield return new WaitForSeconds(1f);
        
        yield return new WaitUntil
            (() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("roar");
        
        yield return new WaitForSeconds(1f);
        
        TallStonesToCrystals();
        
        yield return new WaitForSeconds(1.5f);
        
        BattleStageManager.Instance.TriggerSpecialEvent(1);

        yield return new WaitForSeconds(0.5f);
        
        BattleStageManager.Instance.TriggerSpecialEvent(2);

        QuitAttack();

    }
    
    
    
    
    
    


    private void NihilAOE()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action01"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,
                _behavior.viewerPlayer.RaycastedPosition().y), InitContainer(false), 1);

        var nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, 30, 1);
        //nihilDebuff.dispellable = false;

        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(nihilDebuff, 200);
        
        BattleStageManager.Instance.TriggerSpecialEvent(1);

    }

    private void NihilCorrosionAOE(float healNeeded)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action02"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,
                _behavior.viewerPlayer.transform.position.y), InitContainer(false), 1);

        var nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, 30, 1);
        nihilDebuff.dispellable = false;

        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(nihilDebuff, 200);

        var corrosionEff = new AdvancedTimerBuff((int)BasicCalculation.BattleCondition.Corrosion,
            healNeeded, 8, 0, -1, 1, -1, 8);

        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(corrosionEff, 200, 1);

    }

    protected void SetFireCrystalAdvanced(Vector2 pos)
    {
        var container = Instantiate(attackContainer, RangedAttackFXLayer.transform);

        var fireCrystal = InstantiateRanged(GetProjectileOfFormatName("action06_1"),
            pos, container, 1);

        fireCrystal.GetComponentInChildren<Projectile_DB015_1>().SetEnemySource(gameObject);
    }
    
    protected void SetFireCrystalAdvancedWithoutDamage(Vector2 pos)
    {
        var container = Instantiate(attackContainer, RangedAttackFXLayer.transform);

        var fireCrystal = InstantiateRanged(GetProjectileOfFormatName("action17_2"),
            pos, container, 1);

        fireCrystal.GetComponentInChildren<Projectile_DB015_1>().SetEnemySource(gameObject);
    }
    
    protected void SetFireCrystalSpecial(Vector2 pos)
    {
        var container = Instantiate(attackContainer, RangedAttackFXLayer.transform);

        var fireCrystal = InstantiateRanged(GetProjectileOfFormatName("action13_1"),
            pos, container, 1);

        fireCrystal.GetComponentInChildren<Projectile_DB015_1>().SetEnemySource(gameObject);
    }

    private void FireWave()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action05"),
            gameObject.RaycastedPosition() - new Vector2(ac.facedir * 4, 0), InitContainer(false), ac.facedir, 0);

        Action<StatusManager, StatusManager, AttackBase, float> handler = null;

        handler = (source, target, attack, damage) =>
        {
            if (damage <= 0)
                return;
            attack.OnAttackDealDamage -= handler;
            BattleStageManager.Instance.TimeScaleEffect(0.1f, 0.2f);
        };

        proj.GetComponent<AttackFromEnemy>().OnAttackDealDamage += handler;
    }

    private void AroundBurn()
    {

        var proj = InstantiateRanged(GetProjectileOfFormatName("action07_2"),
            gameObject.RaycastedPosition() + new Vector2(0, 7),
            InitContainer(false), 1);

        var atk = proj.GetComponent<AttackFromEnemy>();
        atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
            70, 21, 1), 100);
        atk.OnAttackHit += (atk, tar) =>
        {
            _statusManager.HPRegenImmediatelyWithoutRandomDirectly(_statusManager,
                (int)(_statusManager.maxHP * 0.03f));
        };

    }

    private void FlameShieldOn()
    {
        if (BurningOn)
            return;

        _statusManager.ReliefAllDebuff();

        var enemy = SpawnEnemyMinon(GetProjectileOfFormatName("action03"),
            transform.position, (int)(_statusManager.maxBaseHP * 0.05f), 1, 1);

        enemy.AddComponent<RelativePositionRetainer>().SetParent(transform);
        enemy.GetComponent<StatusManager>().baseDef = _statusManager.baseDef;
        enemy.GetComponent<StatusManager>().OnHPDecrease += (amount,atk) =>
        {
            if (_statusManager.currentHp > amount)
            {
                _statusManager.currentHp -= amount;
                _statusManager.OnHPChange?.Invoke();
                _statusManager.OnHPDecrease?.Invoke(amount,atk);
            }
        };

        ac.SetHitSensor(false);

        var shieldUI = enemy.GetComponent<UI_H005_ShieldBar>();

        shieldUI.OnShieldDeath += FlameShieldOff;
        shieldUI.mainStat = _statusManager;

        BurningOn = true;
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(H005_BehaviorTree.BurningOn,
            new EnemyAbilityIconEvent(1),_statusManager);
    }

    private void FlameShieldOff()
    {
        BurningOn = false;
        ac.SetHitSensor(true);
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(H005_BehaviorTree.BurningOn,
            new EnemyAbilityIconEvent(0),_statusManager);
    }


    private void ResistanceOn(int count = 4)
    {
        phoenixCount = count;
        PhoenixOn = true;
        

        if (phoenixEffectInstance == null)
        {
            phoenixEffectInstance = Instantiate(GetProjectileOfFormatName("action04"),
                transform.position, Quaternion.identity, BuffFXLayer.transform);
        }
        else
        {
            phoenixEffectInstance.SetActive(true);
        }
        
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(H005_BehaviorTree.PhoenixOn,
            new EnemyAbilityIconEvent(EnemyAbilityIconEvent.EventType.SetNumber, count),
            _statusManager);
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(H005_BehaviorTree.PhoenixOn,
            new EnemyAbilityIconEvent(true),
            _statusManager);
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(H005_BehaviorTree.PhoenixOn,
            new EnemyAbilityIconEvent(1),
            _statusManager);
        
    }
    
    private void ResistanceOff()
    {
        PhoenixOn = false;

        if (phoenixEffectInstance == null)
        {
            
        }
        else
        {
            phoenixEffectInstance.SetActive(false);
        }
        
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(H005_BehaviorTree.PhoenixOn,
            new EnemyAbilityIconEvent(false), _statusManager);
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(H005_BehaviorTree.PhoenixOn,
            new EnemyAbilityIconEvent(0), _statusManager);
        
    }


    private void FlameCross(int angleZ, GameObject container, bool avoidable,
        BattleCondition debuff)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action07_1"),
            transform.position + new Vector3(ac.facedir * 2, 2),
            container, 1);

        proj.transform.eulerAngles = new Vector3(0, 0, angleZ);

        var atk = proj.GetComponent<AttackFromEnemy>();

        atk.ChangeAvoidability(avoidable
            ? AttackFromEnemy.AvoidableProperty.Red
            : AttackFromEnemy.AvoidableProperty.Purple);

        atk.AddWithConditionAll(debuff, 100);

    }

    private void ChaserFlame()
    {
        var pos = _behavior.targetPlayer.transform.position;
        EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac, pos, RangedAttackFXLayer.transform, 2, Vector2.zero,
            false, true, 2, .1f, .5f, true, false, true, 0.15f, true);

        DOVirtual.DelayedCall(2f, () =>
        {
            var proj = InstantiateRanged(GetProjectileOfFormatName("action08_1", true),
                pos, InitContainer(false), 1);

            var atk = proj.GetComponent<AttackFromEnemy>();

            atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Blindness,
                1, 10, 1), 100);
        }, false);
    }

    private void FlameCrossAllRanged(int type)
    {
        var debuff = new TimerBuff((int)BasicCalculation.BattleCondition.Stun,
            1, Random.Range(5, 7), 1);

        if (type == 0)
        {
            var proj = InstantiateRanged(GetProjectileOfFormatName("action08_3", true),
                new Vector3(0, 10), InitContainer(false), 1);
            proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(debuff, 100);
        }
        else
        {
            var container = InitContainer(false);
            var pfb = GetProjectileOfFormatName("action08_2", true);

            var proj1 = InstantiateRanged(
                pfb, new Vector3(12, 0), container, 1);
            var proj2 = InstantiateRanged(
                pfb, new Vector3(-12, 0), container, 1);
            var proj3 = InstantiateRanged(
                pfb, new Vector3(12, 16), container, 1);
            var proj4 = InstantiateRanged(
                pfb, new Vector3(-12, 16), container, 1);

            proj1.GetComponent<AttackFromEnemy>().AddWithConditionAll(debuff, 100);
            proj2.GetComponent<AttackFromEnemy>().AddWithConditionAll(debuff, 100);
            proj3.GetComponent<AttackFromEnemy>().AddWithConditionAll(debuff, 100);
            proj4.GetComponent<AttackFromEnemy>().AddWithConditionAll(debuff, 100);


        }
    }

    private void FlameBallShoot(GameObject hintbar)
    {
        Vector2 direction = hintbar.transform.right;

        RaycastHit2D hit = Physics2D.Raycast
        (hintbar.transform.position,
            direction,
            999, LayerMask.GetMask("Ground", "Border"));

        var proj = InstantiateRanged(GetProjectileOfFormatName("action09_1"),
            hintbar.transform.position, InitContainer(false), 1);

        Tweener tweener = proj.transform.DOMove(hit.point, hit.distance / 15f).SetEase(Ease.Linear).OnComplete(() =>
        {

            var angle =
                ObjectExtensions.AngleDegree(hit.point, _behavior.targetPlayer.transform.position);

            DOVirtual.DelayedCall(1.1f, () =>
            {
                proj.SetActive(false);
                var proj2 = InstantiateRanged(GetProjectileOfFormatName("action09_2"),
                    hit.point, InitContainer(false), 1);
                proj2.transform.eulerAngles = new Vector3(0, 0, angle);

                var atk = proj2.GetComponent<AttackFromEnemy>();

                atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Stun,
                    1, 5, 1), 100);
            }, false);

            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, proj.transform.position,
                RangedAttackFXLayer.transform, new Vector2(46, 12), new Vector2(-2, 0),
                false, 1, 1.1f, angle, 1f, true, false, true,
                0.1f, true);

            Destroy(proj, 2);

        });


    }

    protected void SetTwoFireCrystalAndTwoStone()
    {

        if (crystalPositionPresets.Count < 4)
        {
            crystalPositionPresets.Clear();
            var anch1 = GetAnchoredSensorOfName("LeftM");
            var anch2 = GetAnchoredSensorOfName("RightM");
            var anch3 = GetAnchoredSensorOfName("LeftG");
            var anch4 = GetAnchoredSensorOfName("RightG");

            crystalPositionPresets.Add(anch1.transform.position);
            crystalPositionPresets.Add(anch2.transform.position);
            crystalPositionPresets.Add(anch3.transform.position);
            crystalPositionPresets.Add(anch4.transform.position);
        }

        var type = Random.Range(0, 4);
        List<Vector2> crystalPos = new();
        List<Vector2> stonePos = new();

        switch (type)
        {
            case 0:
                crystalPos.Add(crystalPositionPresets[0]);
                crystalPos.Add(crystalPositionPresets[1]);
                stonePos.Add(crystalPositionPresets[2]);
                stonePos.Add(crystalPositionPresets[3]);
                break;
            case 1:
                crystalPos.Add(crystalPositionPresets[1]);
                crystalPos.Add(crystalPositionPresets[3]);
                stonePos.Add(crystalPositionPresets[0]);
                stonePos.Add(crystalPositionPresets[2]);
                break;
            case 2:
                crystalPos.Add(crystalPositionPresets[0]);
                crystalPos.Add(crystalPositionPresets[2]);
                stonePos.Add(crystalPositionPresets[3]);
                stonePos.Add(crystalPositionPresets[1]);
                break;
            case 3:
                crystalPos.Add(crystalPositionPresets[2]);
                crystalPos.Add(crystalPositionPresets[3]);
                stonePos.Add(crystalPositionPresets[1]);
                stonePos.Add(crystalPositionPresets[0]);
                break;
            default:
                crystalPos.Add(crystalPositionPresets[0]);
                crystalPos.Add(crystalPositionPresets[1]);
                stonePos.Add(crystalPositionPresets[2]);
                stonePos.Add(crystalPositionPresets[3]);
                break;
        }

        float waitTime = 0;

        for (int i = 0; i < 4; i++)
        {
            var hint = GenerateWarningPrefab("action06_1", crystalPositionPresets[i],
                Quaternion.identity, RangedAttackFXLayer.transform);

            if (waitTime <= 0)
            {
                waitTime = hint.GetComponent<EnemyAttackHintBar>().warningTime;
            }
        }

        DOVirtual.DelayedCall(waitTime, () =>
        {
            SetFireCrystalAdvanced(crystalPos[0]);
            SetFireCrystalAdvanced(crystalPos[1]);
            SetStone(stonePos[0]);
            SetStone(stonePos[1]);

        }, false);

    }

    protected void SetFourStone()
    {

        if (crystalPositionPresets.Count < 4)
        {
            crystalPositionPresets.Clear();
            var anch1 = GetAnchoredSensorOfName("LeftM");
            var anch2 = GetAnchoredSensorOfName("RightM");
            var anch3 = GetAnchoredSensorOfName("LeftG");
            var anch4 = GetAnchoredSensorOfName("RightG");

            crystalPositionPresets.Add(anch1.transform.position);
            crystalPositionPresets.Add(anch2.transform.position);
            crystalPositionPresets.Add(anch3.transform.position);
            crystalPositionPresets.Add(anch4.transform.position);
        }

        var type = Random.Range(0, 4);
        Vector2 tallStonePos = new();
        List<Vector2> stonePos = new();

        switch (type)
        {
            case 0:
                tallStonePos = crystalPositionPresets[0];
                stonePos.Add(crystalPositionPresets[1]);
                stonePos.Add(crystalPositionPresets[2]);
                stonePos.Add(crystalPositionPresets[3]);
                break;
            case 1:
                tallStonePos = crystalPositionPresets[1];
                stonePos.Add(crystalPositionPresets[2]);
                stonePos.Add(crystalPositionPresets[0]);
                stonePos.Add(crystalPositionPresets[3]);
                break;
            case 2:
                tallStonePos = crystalPositionPresets[2];
                stonePos.Add(crystalPositionPresets[3]);
                stonePos.Add(crystalPositionPresets[0]);
                stonePos.Add(crystalPositionPresets[1]);
                break;
            case 3:
                tallStonePos = crystalPositionPresets[3];
                stonePos.Add(crystalPositionPresets[0]);
                stonePos.Add(crystalPositionPresets[1]);
                stonePos.Add(crystalPositionPresets[2]);
                break;
            default:
                tallStonePos = crystalPositionPresets[0];
                stonePos.Add(crystalPositionPresets[1]);
                stonePos.Add(crystalPositionPresets[2]);
                stonePos.Add(crystalPositionPresets[3]);
                break;
        }

        float waitTime = 0;

        for (int i = 0; i < 4; i++)
        {
            var hint = GenerateWarningPrefab("action06_1", crystalPositionPresets[i],
                Quaternion.identity, RangedAttackFXLayer.transform);

            if (waitTime <= 0)
            {
                waitTime = hint.GetComponent<EnemyAttackHintBar>().warningTime;
            }
        }

        DOVirtual.DelayedCall(waitTime, () =>
        {
            SetHighStone(tallStonePos);
            SetStone(stonePos[0]);
            SetStone(stonePos[1]);
            SetStone(stonePos[2]);

        }, false);

    }

    protected void SetStone(Vector2 pos)
    {
        var container = Instantiate(attackContainer, RangedAttackFXLayer.transform);

        var stone = InstantiateRanged(GetProjectileOfFormatName("action10_1"),
            pos, container, 1);

        stoneInstances.Add(stone);
    }

    protected void SetHighStone(Vector2 pos)
    {
        var container = Instantiate(attackContainer, RangedAttackFXLayer.transform);

        var stone = InstantiateRanged(GetProjectileOfFormatName("action10_2"),
            pos, container, 1);

        stoneInstances.Add(stone);
    }

    protected ForcedAttackFromEnemy AllRangeBlastEffect(string prefabName = "action11")
    {
        return InstantiateRanged(GetProjectileOfFormatName(prefabName),
            transform.position + new Vector3(0,5), InitContainer(false), 1).GetComponent<ForcedAttackFromEnemy>();
    }

    protected void AllRangeBlast(ForcedAttackFromEnemy atk)
    {
        CineMachineOperator.Instance.CamaraShake(20f,0.3f);

        var target = _behavior.targetPlayer.transform;
        
        var hit = Physics2D.Raycast(transform.position + new Vector3(0,7), 
            target.position - (transform.position + new Vector3(0,7)), 
            999, LayerMask.GetMask("EnemyObstacle"));
        
       
        if (hit.collider == null || Mathf.Abs(_behavior.targetPlayer.transform.position.x - transform.position.x) < 7.5f)
        {
            atk.target = _behavior.viewerPlayer;
            atk.DealDamageImmediately();
        }

        BattleStageManager.Instance.TriggerSpecialEvent(3);
        
        stoneInstances.RemoveAll(x => x == null);
        
        for (int i = stoneInstances.Count - 1; i >= 0; i--)
        {
            if (stoneInstances[i] == null)
            {
                stoneInstances.RemoveAt(i);
                continue;
            }
        }

    }
    protected GameObject AimingFire()
    {
        var hint = GenerateWarningPrefab("action10", transform.position + new Vector3(0,7), Quaternion.identity, 
            RangedAttackFXLayer.transform);
        hint.GetComponent<EnemyAttackHintBarRotater>().target = _behavior.viewerPlayer;
        return hint;
    }

    protected void CheckOverlap(Vector3 euler)
    {
        var atk = InstantiateRanged(GetProjectileOfFormatName("action10_3",true),
            transform.position + new Vector3(0,7), InitContainer(false), 1).GetComponent<ForcedAttackFromEnemy>();

        atk.transform.eulerAngles = euler;

        atk.target = _behavior.viewerPlayer;
        
        DOVirtual.DelayedCall(0.01f, () =>
        {
            
            var polygonCollider2D = atk.GetComponentInChildren<PolygonCollider2D>();
            polygonCollider2D.enabled = true;
            bool playerInCollider = false;
            if(atk.target != null)
            {
                playerInCollider = polygonCollider2D.OverlapPoint(atk.target.transform.position);
            }
            
            //检测玩家
            
            Transform target = _behavior.viewerPlayer.transform;
            
            var hit = Physics2D.Raycast(transform.position + new Vector3(0,7), 
                target.position-(transform.position + new Vector3(0,7)), 
                999, LayerMask.GetMask("EnemyObstacle"));
        
            //如果碰撞到的是玩家（父节点有PlayerStatusManager），添加伤害
            
            if ((hit.collider == null ||
                 Mathf.Abs(_behavior.targetPlayer.transform.position.x - transform.position.x) < 7.5f) &&
                playerInCollider)
            {
                atk.target = _behavior.viewerPlayer;
                atk.DealDamageImmediately();
            }
            
            //检测和polygonCollider2D碰撞的所有EnemyObstacle层的碰撞体，摧毁石头

            var overlaps = new List<Collider2D>();
            ContactFilter2D contactFilter2D = new ContactFilter2D();
            contactFilter2D.SetLayerMask(LayerMask.GetMask("EnemyObstacle"));
            polygonCollider2D.OverlapCollider(contactFilter2D, overlaps);

            for (int i = overlaps.Count - 1; i >= 0; i--)
            {
                var stone = overlaps[i].GetComponent<Projectile_H005_2>();
                if(stone != null)
                {
                    stone.BreakStoneEntirely();
                    stoneInstances.Remove(stone.transform.parent.gameObject);
                }
            }
        }, false);
    }

    private void LavaCarpet(float posY, int direction)
    {
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector3(direction>0?
                BattleStageManager.Instance.mapBorderL:
                BattleStageManager.Instance.mapBorderR,
                posY + 1),
            RangedAttackFXLayer.transform,
            new Vector2(BattleStageManager.Instance.mapBorderR - BattleStageManager.Instance.mapBorderL, 6),
            Vector2.zero, false, 0, 3, 
            direction > 0 ? 0 : 180, 2, true, false,
            true, 0.2f, true);
        
        var lava = InstantiateRanged(GetProjectileOfFormatName("action12",true),
            new Vector3(direction>0?
                BattleStageManager.Instance.mapBorderL + 1:
                BattleStageManager.Instance.mapBorderR - 1,
                posY), InitContainer(false), direction>0?1:-1);

        if (direction < 0)
        {
            lava.GetComponent<DOTweenSimpleController>().moveDirection.x *= -1;
        }

    }

    private Vector2 SetOneStoneOrFireCrystal(int type)
    {
        var anch1 = GetAnchoredSensorOfName("LeftM").transform;
        var anch2 = GetAnchoredSensorOfName("RightM").transform;
        
        var pos = Random.Range(0,2) == 1 ? anch1.position : anch2.position;
        
        var hint = GenerateWarningPrefab("action06_1", 
            pos,
            Quaternion.identity, RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBarRect2D>();

        DOVirtual.DelayedCall(hint.warningTime, () =>
        {
            if (type == 0)
            {
                SetHighStone(pos);
            }
            else
            {
                SetFireCrystalSpecial(pos);
            }
        }, false);

        return pos;

    }

    private void LavaTsunami(int posX)
    {
        GameObject prefab;

        if (posX == ac.facedir)
        {
            prefab = GetProjectileOfFormatName("action13_2",true);
        }
        else
        {
            prefab = GetProjectileOfFormatName("action13_3",true);
        }

        var lava = InstantiateRanged(prefab,
            (ac.facedir > 0  ? new Vector2(-22,0) : new Vector2(22,0)),
            InitContainer(false), ac.facedir,0);
    }
    

    private void RingAttack(Vector2 pos)
    { 
        var hintbar = 
            GenerateWarningPrefab("action13_1", pos, Quaternion.identity, RangedAttackFXLayer.transform);

        var warningTime = hintbar.GetComponent<EnemyAttackHintBar>().warningTime;
        
        DOVirtual.DelayedCall(warningTime + 0.2f, () =>
        {
            var atk = InstantiateRanged(GetProjectileOfFormatName("action13_5",true),
                pos, InitContainer(false), 1);
            
        }, false);

    }

    
    /// <param name="moveType">0:横向；1:竖向</param>
    /// <returns></returns>
    private (List<GameObject> hintbars,List<bool> avoidables,int preset) SetBlocks(int moveType,float delay,int preset = -1)
    {
        List<Vector2> blockPosList = new()
        {
            //第一排
            new Vector2(-16.5f,11.5f), // 0
            new Vector2(-5.5f,11.5f),  // 1
            new Vector2(5.5f,11.5f),   // 2
            new Vector2(16.5f,11.5f),  // 3
            //第二排
            new Vector2(-16.5f,0.5f),  // 4
            new Vector2(-5.5f,0.5f),   // 5
            new Vector2(5.5f,0.5f),    // 6
            new Vector2(16.5f,0.5f),   // 7
        };

        List<bool> avoidablePreset1 = new()
        {
            true,false,false,false,
            false,false,true,false
        };
        
        List<bool> avoidablePreset2 = new()
        {
            false,true,false,false,
            false,false,false,true
        };
        
        if(preset < 0)
            preset = Random.Range(0,2);
        
        var avoidable = preset == 1 ? avoidablePreset1 : avoidablePreset2;

        List<GameObject> hintbarInstances = new();

        for (int i = 0; i < blockPosList.Count; i++)
        {
            GameObject hintbar;
            if (avoidable[i] == false)
            {
                hintbar = GenerateWarningPrefab("action16_1", 
                    blockPosList[i], Quaternion.identity, RangedAttackFXLayer.transform);
            }
            else
            {
                hintbar = GenerateWarningPrefab("action16_2", 
                    blockPosList[i], Quaternion.identity, RangedAttackFXLayer.transform);
            }
            hintbarInstances.Add(hintbar);
        }

        if (moveType == 0)
        {
            //横向移动
            hintbarInstances[0].transform.DOMoveX(blockPosList[2].x, 0.8f).SetDelay(delay-0.4f);
            hintbarInstances[1].transform.DOMoveX(blockPosList[3].x, 0.8f).SetDelay(delay-0.4f);
            hintbarInstances[2].transform.DOMoveX(blockPosList[0].x, 0.8f).SetDelay(delay-0.4f);
            hintbarInstances[3].transform.DOMoveX(blockPosList[1].x, 0.8f).SetDelay(delay-0.4f);
            
            hintbarInstances[4].transform.DOMoveX(blockPosList[6].x, 0.8f).SetDelay(delay-0.4f);
            hintbarInstances[5].transform.DOMoveX(blockPosList[7].x, 0.8f).SetDelay(delay-0.4f);
            hintbarInstances[6].transform.DOMoveX(blockPosList[4].x, 0.8f).SetDelay(delay-0.4f);
            hintbarInstances[7].transform.DOMoveX(blockPosList[5].x, 0.8f).SetDelay(delay-0.4f);
        }
        else
        {
            //纵向移动
            hintbarInstances[0].transform.DOMoveY(blockPosList[4].y, 0.8f).SetDelay(delay-0.4f);
            hintbarInstances[1].transform.DOMoveY(blockPosList[5].y, 0.8f).SetDelay(delay-0.4f);
            hintbarInstances[2].transform.DOMoveY(blockPosList[6].y, 0.8f).SetDelay(delay-0.4f);
            hintbarInstances[3].transform.DOMoveY(blockPosList[7].y, 0.8f).SetDelay(delay-0.4f);
            
            hintbarInstances[4].transform.DOMoveY(blockPosList[0].y, 0.8f).SetDelay(delay-0.4f);
            hintbarInstances[5].transform.DOMoveY(blockPosList[1].y, 0.8f).SetDelay(delay-0.4f);
            hintbarInstances[6].transform.DOMoveY(blockPosList[2].y, 0.8f).SetDelay(delay-0.4f);
            hintbarInstances[7].transform.DOMoveY(blockPosList[3].y, 0.8f).SetDelay(delay-0.4f);
        }
        
        return (hintbarInstances,avoidable,preset);

    }

    private void BlockBlast(List<GameObject> hintbarInstances, List<bool> avoidable)
    {
        var container = InitContainer(false);
        var prefab = GetProjectileOfFormatName("action16",true);
        for (int i = 0; i < hintbarInstances.Count; i++)
        {
            var fx = InstantiateRanged(prefab,
                hintbarInstances[i].transform.position, container, 1);
            
            var atk = fx.GetComponent<AttackFromEnemy>();

            if (avoidable[i] == false)
            {
                atk.ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
            }else
            {
                atk.ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Red);
            }
            
        }
    }
    
    private void Combustion()
    {
        var container = InitContainer(false);
        var prefab = GetProjectileOfFormatName("action14",true);
        var pos = new Vector2(0, 0);
        var fx = InstantiateRanged(prefab, pos, container, 1);
        BattleStageManager.Instance.TriggerSpecialEvent(1);
        BattleStageManager.Instance.TriggerSpecialEvent(2);
    }

    private void SetEightFireCrystals()
    {
        if (crystalPositionPresets.Count < 4)
        {
            crystalPositionPresets.Clear();
            var anch1 = GetAnchoredSensorOfName("LeftM");
            var anch2 = GetAnchoredSensorOfName("RightM");
            var anch3 = GetAnchoredSensorOfName("LeftG");
            var anch4 = GetAnchoredSensorOfName("RightG");

            crystalPositionPresets.Add(anch1.transform.position );
            crystalPositionPresets.Add(anch2.transform.position );
            crystalPositionPresets.Add(anch3.transform.position);
            crystalPositionPresets.Add(anch4.transform.position);
        }

        var posList = new List<Vector2>();
        
        posList.Add(crystalPositionPresets[0] + new Vector2(2,0));
        posList.Add(crystalPositionPresets[1] + new Vector2(-2,0));
        posList.Add(crystalPositionPresets[2] + new Vector2(2,0));
        posList.Add(crystalPositionPresets[3] + new Vector2(-2,0));
        
        posList.Add(crystalPositionPresets[0] + new Vector2(-6,0));
        posList.Add(crystalPositionPresets[1] + new Vector2(6,0));
        posList.Add(crystalPositionPresets[2] + new Vector2(-6,0));
        posList.Add(crystalPositionPresets[3] + new Vector2(6,0));

        float waitTime = -1;
        
        for (int i = 0; i < 8; i++)
        {
            var hint = GenerateWarningPrefab("action06_1", posList[i],
                Quaternion.identity, RangedAttackFXLayer.transform);

            if (waitTime <= 0)
            {
                waitTime = hint.GetComponent<EnemyAttackHintBar>().warningTime;
            }
        }

        DOVirtual.DelayedCall(waitTime, () =>
        {
            foreach (var pos in posList)
            {
                SetFireCrystalAdvanced(pos);
            }

        }, false);
        
    }
    
    
    
    


    private void OnBreakEnter()
    {
        ac.HitSensor.offset = new Vector2(3.25f,ac.HitSensor.offset.y);
    }

    private void OnBreakExit()
    {
        ac.HitSensor.offset = new Vector2(-0.25f, ac.HitSensor.offset.y);
    }

    private (float,float) SpecialResistanceBuff(StatusManager src, AttackBase atk, StatusManager target)
    {
        if (!PhoenixOn)
            return(0,0);
        else
        {
            if (phoenixCount >= 4)
                return (0.3f, 0);
            else if(phoenixCount >= 3)
                return (0.2f, 0);
            else if(phoenixCount >= 2)
                return (0.1f, 0);
            else
                return (0.05f, 0);
        }
    }

    private void CheckResistanceBuff(BattleCondition buff)
    {
        if(PhoenixOn == false)
            return;
        
        
        bool decrease = false;
        if (StatusManager.IsDebuff(buff.buffID) || StatusManager.IsAffliction(buff.buffID))
        {
            decrease = true;
        }

        if (decrease)
        {
            phoenixCount = Mathf.Clamp(phoenixCount - 1, 0, 8);
            
            BattleStageManager.Instance.InvokeEnemyAbilityEvent(H005_BehaviorTree.PhoenixOn,
                new EnemyAbilityIconEvent(EnemyAbilityIconEvent.EventType.SetNumber, phoenixCount),
                _statusManager);

            if (phoenixCount <= 0)
            {
                ResistanceOff();
            }
            
        }
        
    }
    
    private void FirePillarOn()
    {
        var container = InitContainer(false);
        
        var prefab = GetProjectileOfFormatName("action17_1",true);
        
        InstantiateRanged(prefab, 
            new Vector3(BattleStageManager.Instance.mapBorderL + 4,0),
            container, 1);
        
        InstantiateRanged(prefab, 
            new Vector3(BattleStageManager.Instance.mapBorderR - 4,0),
            container, 1);
    }

    protected void SetFourTallStone()
    {

        if (crystalPositionPresets.Count < 4)
        {
            crystalPositionPresets.Clear();
            var anch1 = GetAnchoredSensorOfName("LeftM");
            var anch2 = GetAnchoredSensorOfName("RightM");
            var anch3 = GetAnchoredSensorOfName("LeftG");
            var anch4 = GetAnchoredSensorOfName("RightG");

            crystalPositionPresets.Add(anch1.transform.position);
            crystalPositionPresets.Add(anch2.transform.position);
            crystalPositionPresets.Add(anch3.transform.position);
            crystalPositionPresets.Add(anch4.transform.position);
        }

        float waitTime = 0;

        for (int i = 0; i < 4; i++)
        {
            var hint = GenerateWarningPrefab("action06_1", crystalPositionPresets[i],
                Quaternion.identity, RangedAttackFXLayer.transform);

            if (waitTime <= 0)
            {
                waitTime = hint.GetComponent<EnemyAttackHintBar>().warningTime;
            }
        }

        DOVirtual.DelayedCall(waitTime, () =>
        {
            SetHighStone(crystalPositionPresets[0]);
            SetHighStone(crystalPositionPresets[1]);
            SetHighStone(crystalPositionPresets[2]);
            SetHighStone(crystalPositionPresets[3]);
        }, false);

    }
    
    private void ChaserFlameSpecial()
    {
        var pos = _behavior.targetPlayer.transform.position;
        var chaserHint = EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac, pos, RangedAttackFXLayer.transform, 2, Vector2.zero,
            false, true, 2, .1f, .5f, true, false, true, 0.15f, true);

        var chaser = chaserHint.AddComponent<EnemyAttackHintBarChaser>();
        
        chaser.hardLock = true;
        chaser.SetLockTime(1.5f);
        chaser.target = _behavior.targetPlayer;
        chaser.moveable = true;
        
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,2);
        
        
        DOVirtual.DelayedCall(2f, () =>
        {
            var proj = InstantiateRanged(GetProjectileOfFormatName("action08_1", true),
                chaser.transform.position, InitContainer(false), 1);
            
            stoneInstances.RemoveAll(x => x == null);

            foreach (var stone in stoneInstances)
            {
                if (Vector2.Distance(stone.transform.position, 
                        proj.transform.position) <= 4)
                {
                    stone.GetComponentInChildren<Projectile_H005_2>()?.BreakStone();
                }
            }
        }, false);
    }

    private void TallStonesToCrystals()
    {
        stoneInstances.RemoveAll(x => x == null);
        
        for (int i = stoneInstances.Count - 1; i >= 0; i--)
        {
            var stone = stoneInstances[i].GetComponentInChildren<Projectile_H005_2>();

            if (stone.Level == 2)
            {
                stoneInstances.RemoveAt(i);
                stone.BreakStoneEntirely();
                SetFireCrystalAdvancedWithoutDamage(stone.transform.position);
            }
        }
    }

    private void ClearStone(int id)
    {
        if (id == 2)
        {
            stoneInstances.Clear();
        }
    }

    private void GainAtkBuff(BattleCondition buff)
    {
        if (StatusManager.IsAffliction(buff.buffID))
        {
            if (buff.buffID == (int)BasicCalculation.BattleCondition.Frostbite ||
                buff.buffID == (int)BasicCalculation.BattleCondition.Poison ||
                buff.buffID == (int)BasicCalculation.BattleCondition.ShadowBlight)
            {
                _statusManager.RemoveAllConditionWithSpecialID(8202502);
            }
            else
            {
                _statusManager.ObtainTimerBuff(1, 20, 10, 1, 8202502);
            }
        }
        
        
        
    }
    
    

}
