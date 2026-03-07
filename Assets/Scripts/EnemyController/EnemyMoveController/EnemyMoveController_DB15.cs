using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMoveController_DB15 : EnemyMoveManager
{
    private VoiceControllerEnemy _voice;
    
    private List<Vector2> crystalPositionPresets = new();
    private List<GameObject> stoneInstances = new();

    private enum VoiceGroup
    {
        Intro,
        HPBelow70,
        HPBelow40,
        Defeated,
        Skill1,
        Skill2,
        OverHeal,
        Skill3,
        Tortune,
        AllRanged
    }

    protected override void Start()
    {
        base.Start();
        _voice = GetComponentInChildren<VoiceControllerEnemy>();
        GetAllAnchors();
    }

    /// <summary>
    /// Chasing Crystal
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action01()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB15_Action01");

        if (_voice && _voice.voiceGroups.Count > 0)
        {
            _voice.BroadCastMyVoice((int)VoiceGroup.Skill1);
        }
        
        yield return new WaitForSeconds(1f);

        anim.Play("charge_1");
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,2f);

        
        var hint = GenerateWarningPrefab("action01_1", _behavior.targetPlayer.RaycastedPosition() + Vector2.up*1.5f,
            Quaternion.identity, RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBarRect2D>();
        
        yield return new WaitForSeconds(hint.warningTime);
        
        anim.Play("charge_3");
        
        var pos = hint.transform.position;
        var container = SetFireCrystal(hint.transform.position);
        
        DOVirtual.DelayedCall(8f, () =>
        {
            SetFireCrystalFireFX(pos);
            GenerateWarningPrefab("action01_2", pos,
                Quaternion.identity, RangedAttackFXLayer.transform);
        },false);
        
        DOVirtual.DelayedCall(10f, () =>
        {
            FireCrystalExplode(pos,container);
        },false);
        
        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
        && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();


    }
    
    
    /// <summary>
    /// Chasing Crystal
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action01V()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB15_Action01");

        if (_voice && _voice.voiceGroups.Count > 0)
        {
            if(!_voice.voice.isPlaying)
                _voice.BroadCastMyVoice((int)VoiceGroup.Skill1);
        }
        
        yield return new WaitForSeconds(.5f);

        anim.Play("charge_1");
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,2f);
        
        yield return new WaitForSeconds(1.25f);

        
        var hint = GenerateWarningPrefab("action01_1", _behavior.targetPlayer.RaycastedPosition() + Vector2.up*1.5f,
            Quaternion.identity, RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBarRect2D>();
        
        yield return new WaitForSeconds(hint.warningTime);
        
        anim.Play("charge_3");
        
        var pos = hint.transform.position;
        SetFireCrystalAdvanced(hint.transform.position);

        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();


    }

    /// <summary>
    /// Fixed Crystal
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action02()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        
        anim.Play("charge_1");

        var raycastedCol = gameObject.RaycastedPlatform();
        float leftBorder, rightBorder, height;

        if (raycastedCol == null)
        {
            height = transform.position.y;
            leftBorder = transform.position.x - 3;
            rightBorder = transform.position.x + 3;
        }
        else
        {
            height = raycastedCol.bounds.max.y;
            leftBorder = raycastedCol.bounds.min.x;
            rightBorder = raycastedCol.bounds.max.x;
        }

        var leftCrystalPosition = new Vector2
            (Mathf.Max(transform.position.x - 3,leftBorder), height);
        var rightCrystalPosition = new Vector2
            (Mathf.Min(transform.position.x + 3,rightBorder), height);
        

        var hint = GenerateWarningPrefab("action01_1", leftCrystalPosition + Vector2.up*1.5f,
            Quaternion.identity, RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBarRect2D>();
        var hint2GO = GenerateWarningPrefab("action01_1", rightCrystalPosition + Vector2.up * 1.5f,
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        
        
        yield return new WaitForSeconds(hint.warningTime);
        
        
        
        anim.Play("charge_3");
        var pos1 = hint.transform.position;
        var pos2 = hint2GO.transform.position;
        

        if (_behavior.difficulty >= 2)
        {
            SetFireCrystalAdvanced(pos1);
            SetFireCrystalAdvanced(pos2);
        }
        else
        {
            var container1 = SetFireCrystal(hint.transform.position);
            var container2 = SetFireCrystal(hint2GO.transform.position);
            DOVirtual.DelayedCall(8f, () =>
            {
                SetFireCrystalFireFX(pos1);
                SetFireCrystalFireFX(pos2);
                GenerateWarningPrefab("action01_2", pos1,
                    Quaternion.identity, RangedAttackFXLayer.transform);
                GenerateWarningPrefab("action01_2", pos2,
                    Quaternion.identity, RangedAttackFXLayer.transform);
            },false);
        
            DOVirtual.DelayedCall(10f, () =>
            {
                FireCrystalExplode(pos1,container1);
                FireCrystalExplode(pos2,container2);
            },false);
        }
        
        
        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();
        
        
    }
    
    
    /// <summary>
    /// Fixed Crystal
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action02(float[] positionArray)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        
        anim.Play("charge_1");
        
        List<Vector2> positionList = new List<Vector2>();

        for (int i = 0; i < positionArray.Length-1; i += 2)
        {
            positionList.Add( new Vector2(positionArray[i],positionArray[i+1]));
        }

        yield return new WaitForSeconds(1);
        EnemyAttackHintBar hint = null;
        
        foreach (var pos in positionList)
        {
            if (hint == null)
            {
                hint = GenerateWarningPrefab("action01_1", pos,
                    Quaternion.identity, RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBarRect2D>();
            }
            else
            {
                GenerateWarningPrefab("action01_1", 
                    pos,
                    Quaternion.identity, RangedAttackFXLayer.transform);
            }
        }

        yield return new WaitForSeconds(hint.warningTime);
        
        anim.Play("charge_3");

        foreach (var posVec in positionList)
        {
            SetFireCrystalAdvanced(posVec);
        }

        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();
        
        
    }
    

    /// <summary>
    /// Nihil
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action03(float nihilTime = 5f)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB15_Action03");
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_1");

        yield return new WaitForSeconds(1.2f);
        
        anim.Play("charge_3");
        NihilAOE(nihilTime);

        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();

    }
    
    public IEnumerator DB15_Action03V(float effect)
    {
        yield return _canAction;
        _behavior.controllAfflictionProtect = true;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("DB15_Action04");
        
        _voice?.BroadCastMyVoice((int)VoiceGroup.Tortune);

        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_1");

        yield return new WaitForSeconds(1.2f);
        
        anim.Play("charge_3");
        
        NihilCorrosionAOE(effect);
        _behavior.controllAfflictionProtect = false;

        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();

    }

    /// <summary>
    /// Punch Attack
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action04()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + 5*Vector3.up);
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("combo1");

        ComboPunch();
        
        yield return null;
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.45f);
        
        anim.Play("combo2");

        yield return new WaitForSeconds(0.6f);
        var pos = BattleStageManager.Instance.OutOfRangeCheck(new Vector2(transform.position.x + 4 * (ac.facedir),
            transform.position.y));
        _tweener = transform.DOMoveX(pos.x, 0.5f).SetEase(Ease.OutSine);
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.45f);
        
        anim.Play("combo3");
        
        yield return new WaitForSeconds(0.6f);
        pos = BattleStageManager.Instance.OutOfRangeCheck(new Vector2(transform.position.x + 5 * (ac.facedir),
            transform.position.y));
        _tweener = transform.DOMoveX(pos.x, 0.8f).SetEase(Ease.OutSine);
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();
    }

    /// <summary>
    /// Buff Defense
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action05(int buffEffect = 20)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_1");

        yield return new WaitForSeconds(1.2f);
        
        anim.Play("charge_3");
        
        if (_behavior.difficulty != 1)
        {
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
                buffEffect, 15);
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
                buffEffect, 15);
        }
        else
        {
            if (_statusManager.GetConditionTotalValue((int)BasicCalculation.BattleCondition.DefBuff) < 200)
            {
                _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
                    buffEffect, -1);
                _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
                    buffEffect, -1);
                _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
                    buffEffect, 15);
            }
        }


        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();
    }

    public IEnumerator DB15_Action06()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_1");

        EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
            transform.position + new Vector3(0, 4f), RangedAttackFXLayer.transform,
            2f, Vector2.zero,false, 
            true, 1.8f, 0.05f, 0.4f,true, true);

        yield return new WaitForSeconds(0.5f);

        var fx = Instantiate(GetProjectileOfFormatName("action05_1"),
            transform.position + new Vector3(0, 4f),
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        yield return new WaitForSeconds(1.5f);
        
        anim.Play("charge_3");
        
        BouncingFireball();

        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();
    }


    /// <summary>
    /// Wave
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action07()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        
        yield return new WaitForSeconds(0.1f);
        
        if(Mathf.Abs(_behavior.targetPlayer.transform.position.x - transform.position.x) > 22)
        {
            ac.TurnMove(_behavior.targetPlayer);
            anim.Play("roll");
            yield return new WaitForSeconds(0.3f);
            _tweener = transform.DOMoveX(transform.position.x +ac.facedir * 12,
                0.45f).SetEase(Ease.InOutSine);
            
            yield return new WaitForSeconds(0.7f);
        }else if (Mathf.Abs(_behavior.targetPlayer.transform.position.x - transform.position.x) < 8)
        {
            if (transform.position.x < 0)
            {
                ac.SetFaceDir(1);
                anim.Play("roll");
                yield return new WaitForSeconds(0.3f);
                _tweener = transform.DOMoveX(transform.position.x +ac.facedir * 12,
                    0.45f).SetEase(Ease.InOutSine);
            }
            else
            {
                ac.SetFaceDir(-1);
                anim.Play("roll");
                yield return new WaitForSeconds(0.3f);
                _tweener = transform.DOMoveX(transform.position.x +ac.facedir * 12,
                    0.45f).SetEase(Ease.InOutSine);
            }
            
            yield return new WaitForSeconds(0.7f);
            ac.TurnMove(_behavior.targetPlayer);
        }
        
        anim.Play("punch");
        
        ac.SetCounter(true);
        ac.currentKBRes = 100;

        if (_voice.voice.isPlaying == false)
        {
            _voice?.BroadCastMyVoice((int)VoiceGroup.Skill2);
        }
        
        yield return new WaitForSeconds(0.67f);
        
        WaveSlow();

        if (Random.Range(0, 2) == 0)
        {
            yield return new WaitUntil
                (()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
            ac.TurnMove(_behavior.targetPlayer);
            anim.Play("combo2");
            yield return new WaitForSeconds(0.8f);
            if(Random.Range(0, 2) == 0)
                WaveSlow();
            else
                WaveFast();
        }
        
        ac.SetCounter(false);
        ac.currentKBRes = 999;
        
        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();
    }



    public IEnumerator DB15_Action08()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.SetGravityScale(0);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_1");

        EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac,
            transform.position + new Vector3(0, 1f), RangedAttackFXLayer.transform,
            5f, Vector2.zero,true, 
            true, 2f, 0.05f, 0.5f,true, true);

        if (!_voice.voice.isPlaying)
        {
            _voice?.BroadCastMyVoice((int)VoiceGroup.Skill3);
        }
        
        yield return new WaitForSeconds(2f);
        
        anim.Play("charge_3l");

        int currentHP = _statusManager.currentHp;
        
        ac.SetCounter(true);
        ac.currentKBRes = 100;
        
        BurnAround();
        
        yield return new WaitForSeconds(2f);
        
        anim.Play("charge_5");

        yield return null;
        
        ac.ResetGravityScale();

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        if (_statusManager.currentHp - currentHP >= _statusManager.maxHP * 0.15f)
        {
            _voice?.BroadCastMyVoice((int)VoiceGroup.OverHeal);
        }
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    
    /// <summary>
    /// 1石头+3炎晶
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action09()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("DB15_Action09");
        
        yield return new WaitForSeconds(0.5f);

        SetThreeFireCrystalAndAStone();

        yield return null;
        
        ac.TurnMove(new Vector3(0,0));
        anim.SetFloat("forward", 1);
        var middleM = GetAnchoredSensorOfName("MiddleM");
        _tweener = transform.DOMove(middleM.transform.position, 1.25f).SetEase(Ease.InOutSine);

        
        
        yield return new WaitForSeconds(1f);
        anim.SetFloat("forward", 0);
        
        yield return new WaitForSeconds(0.25f);
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(2f);
        
        anim.Play("charge_1");
        
        
        _voice?.BroadCastMyVoice((int)VoiceGroup.AllRanged);
        
        
        yield return new WaitForSeconds(0.5f);

        var fx = AllRangedDestructionForcing();

        yield return new WaitForSeconds(1f);

        anim.Play("charge_3l");

        yield return new WaitForSeconds(0.55f);
        
        AllRangeDestruction(fx);
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("charge_5");

        yield return null;
        
        if(fx != null)
            Destroy(fx);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// 5炎晶
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action10()
    {
        yield return _canAction;
        _behavior.controllAfflictionProtect = true;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("DB15_Action10");
        StageCameraController.SwitchOverallCamera();
        
        yield return new WaitForSeconds(0.5f);

        SetFiveFireCrystals();

        yield return null;
        
        ac.TurnMove(new Vector3(0,0));
        anim.SetFloat("forward", 1);
        var middleM = GetAnchoredSensorOfName("MiddleM");
        _tweener = transform.DOMove(middleM.transform.position, 1.25f).SetEase(Ease.InOutSine);
        
        
        yield return new WaitForSeconds(1f);
        anim.SetFloat("forward", 0);
        
        yield return new WaitForSeconds(0.25f);
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(1f);
        
        anim.Play("charge_1");
        
        _voice?.BroadCastMyVoice((int)VoiceGroup.Skill1);
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,new Vector3(0,12), RangedAttackFXLayer.transform,
            new Vector2(48,30), new Vector2(-24,0),true, 
            1, 2.55f, 0, 0.5f,true, true);
        
        yield return new WaitForSeconds(2f);

        anim.Play("charge_3l");

        yield return new WaitForSeconds(0.55f);
        
        AllRangedDestructionAvoidable();
        StageCameraController.SwitchMainCamera();
        _behavior.controllAfflictionProtect = false;
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("charge_5");

        yield return null;

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");
        
        QuitAttack();
    }
    
    
    /// <summary>
    /// 2石头+2炎晶
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action11()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("DB15_Action11");
        
        yield return new WaitForSeconds(0.5f);

        SetTwoFireCrystalAndTwoStone();

        yield return null;
        
        ac.TurnMove(new Vector3(0,0));
        anim.SetFloat("forward", 1);
        var middleM = GetAnchoredSensorOfName("MiddleM");
        _tweener = transform.DOMove(middleM.transform.position - new Vector3(0,1),
            1.25f).SetEase(Ease.InOutSine);

        
        
        yield return new WaitForSeconds(1f);
        anim.SetFloat("forward", 0);
        
        yield return new WaitForSeconds(0.25f);
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(1f);
        
        anim.Play("charge_1");
        
        
        _voice?.BroadCastMyVoice((int)VoiceGroup.AllRanged);
        
        
        yield return new WaitForSeconds(0.5f);

        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3f);
        var hintbar = AimingFire();
        var waitTime = hintbar.GetComponent<EnemyAttackHintBarCircle>().warningTime;

        yield return new WaitForSeconds(waitTime);

        anim.Play("charge_3l");

        yield return new WaitForSeconds(0.1f);
        
        ac.TurnMove(_behavior.targetPlayer);
        CheckOverlap(hintbar.transform.eulerAngles);
        
        yield return new WaitForSeconds(0.5f);
        
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3f);
        hintbar = AimingFire();
        anim.Play("charge_5");
        
        yield return new WaitForSeconds(0.5f);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("charge_1");

        yield return new WaitForSeconds(hintbar.GetComponent<EnemyAttackHintBarCircle>().warningTimeLeft);
        
        anim.Play("charge_3l");

        yield return new WaitForSeconds(0.1f);
        
        ac.TurnMove(_behavior.targetPlayer);
        CheckOverlap(hintbar.transform.eulerAngles);
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_5");

        yield return null;
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");
        
        QuitAttack();
    }
    
    /// <summary>
    /// Cross
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action12()
    {
        yield return _canActionOnFlyingGround;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("DB15_Action12");
        
        if (!_voice.voice.isPlaying)
        {
            _voice?.BroadCastMyVoice((int)VoiceGroup.Skill2);
        }
        
        yield return new WaitForSeconds(0.5f);

        anim.Play("charge_1");

        GenerateWarningPrefab("action12_1", transform.position + new Vector3(0, 1), Quaternion.identity,
            RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(2f);

        anim.Play("charge_3l");
        
        FireRing();
        int type = Random.Range(0, 2);
        if (type == 0)
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position+new Vector3(0,1),
                RangedAttackFXLayer.transform, new Vector2(28, 6),new Vector2(-14,0),
                true,1,2,0,0.5f,true,true);
            
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position+new Vector3(0,1),
                RangedAttackFXLayer.transform, new Vector2(28, 6),new Vector2(-14,0),
                true,1,2,90,0.5f,true,true);
            
        }
        else
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position+new Vector3(0,1),
                RangedAttackFXLayer.transform, new Vector2(28, 6),
                new Vector2(-14,0),
                true,1,2,45,0.5f,true,true);
            
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position+new Vector3(0,1),
                RangedAttackFXLayer.transform, new Vector2(28, 6),
                new Vector2(-14,0),
                true,1,2,-45,0.5f,true,true);
        }
        
        yield return new WaitForSeconds(2f);
        
        FireCrossRandom(type);

        if (type == 0)
        {
            GenerateWarningPrefab("action12_3", new Vector3(0,10), Quaternion.identity,
                RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action12_3V", new Vector3(0,10), Quaternion.identity,
                RangedAttackFXLayer.transform);
        }
        else
        {
            GenerateWarningPrefab("action12_2", new Vector3(12,0), Quaternion.identity,
                RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action12_2", new Vector3(12,16), Quaternion.identity,
                RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action12_2", new Vector3(-12,0), Quaternion.identity,
                RangedAttackFXLayer.transform);
            GenerateWarningPrefab("action12_2", new Vector3(-12,16), Quaternion.identity,
                RangedAttackFXLayer.transform);
        }
        
        yield return new WaitForSeconds(2f);
        
        FireCrossAllRanged(type);
        
        anim.Play("charge_5");

        yield return null;

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");
        
        QuitAttack();
    }
    
    
    /// <summary>
    /// Clearing Explosion
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15_Action13()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        anim.Play("charge_1");
        
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,new Vector3(0,12), RangedAttackFXLayer.transform,
            new Vector2(48,30), new Vector2(-24,0),true, 
            1, 1.55f, 0, 0.5f,true, true);

        yield return new WaitForSeconds(1f);

        anim.Play("charge_3l");

        yield return new WaitForSeconds(0.55f);
        
        AllRangedDestructionAvoidable();
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("charge_5");

        yield return null;

        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");
        
        QuitAttack();
    }
    
    
    public IEnumerator DB15_Action14()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);

        ac.TurnMove(new Vector3(0,0));
        anim.SetFloat("forward", 1);
        var middleM = GetAnchoredSensorOfName("MiddleM");
        var leftG = GetAnchoredSensorOfName("LeftG");
        _tweener = transform.DOMove(new Vector3(middleM.transform.position.x, 
                leftG.transform.position.y),
            1.25f).SetEase(Ease.InOutSine);
        
        yield return new WaitForSeconds(1f);
        anim.SetFloat("forward", 0);
        
        yield return new WaitForSeconds(0.25f);
        ac.TurnMove(_behavior.targetPlayer);

        anim.Play("charge_1");
        
        PhaseChangeEffect1();
        StageCameraController.SwitchOverallCamera();

        yield return new WaitForSeconds(1f);

        anim.Play("charge_3l");

        yield return new WaitForSeconds(2.5f);
        
        PhaseChangeEffect2();
        CineMachineOperator.Instance.CamaraShake(6f,3);

        (BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background1") as SpriteRenderer).DOColor(
            Color.clear, 1.5f);
        
        yield return new WaitForSeconds(2f);
        
        DOVirtual.DelayedCall(1.35f,()=>
            StageCameraController.SwitchMainCamera(),false);
        
        yield return null;
        
        QuitAttack();
        _behavior.currentMoveAction = null;
    }
    
    


    protected GameObject SetFireCrystal(Vector2 pos)
    {
        var container = Instantiate(attackContainer, RangedAttackFXLayer.transform);
        
        var fireCrystal = InstantiateRanged(GetProjectileOfFormatName("action01_1"),
            pos,container,1);
        
        return container;
    }

    protected void SetFireCrystalFireFX(Vector2 pos)
    {
        Instantiate(GetProjectileOfFormatName("action01_2"),
            pos,Quaternion.identity,
            RangedAttackFXLayer.transform);
    }
    
    protected void FireCrystalExplode(Vector2 pos, GameObject container)
    {
        InstantiateRanged(GetProjectileOfFormatName("action01_3"),
            pos,container,1);
    }

    protected void SetFireCrystalAdvanced(Vector2 pos)
    {
        var container = Instantiate(attackContainer, RangedAttackFXLayer.transform);
        
        var fireCrystal = InstantiateRanged(GetProjectileOfFormatName("action01_1"),
            pos,container,1);
        
        fireCrystal.GetComponentInChildren<Projectile_DB015_1>().SetEnemySource(gameObject);
    }

    protected void SetStoneAdvanced(Vector2 pos)
    {
        var container = Instantiate(attackContainer, RangedAttackFXLayer.transform);
        
        var stone = InstantiateRanged(GetProjectileOfFormatName("action09_1"),
            pos,container,1);

        
        
        stoneInstances.Add(stone);
        
    }

    protected void NihilAOE(float time)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action03"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,
                transform.position.y,ac.ModelDepth),InitContainer(false),1);

        var nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, time, 1);
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(nihilDebuff,100);

    }
    
    protected void NihilCorrosionAOE(float healNeeded)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action03_2"),
            new Vector3(_behavior.viewerPlayer.transform.position.x,
                transform.position.y,ac.ModelDepth),InitContainer(false),1);

        var nihilDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            -1, 30, 1);
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(nihilDebuff,100);
        
        var corrosionEff = new AdvancedTimerBuff((int)BasicCalculation.BattleCondition.Corrosion,
            healNeeded, 8, 0, -1, 1, -1,8);
        
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(corrosionEff,200,1);

    }


    protected void ComboPunch()
    {
        var proj = InstantiateMeele(GetProjectileOfFormatName("action04"),
            transform.position,InitContainer(true));
    }

    protected void BouncingFireball()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action05_2"),
            transform.position + new Vector3(0, 4), InitContainer(false), 1);
        
        proj.GetComponentInChildren<NormalProjectile>().SetFiredir(ac.facedir);
        
    }

    protected void WaveSlow()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action07_1"),
            gameObject.RaycastedPosition()+new Vector2(0,1.5f), InitContainer(false), ac.facedir);

        Action<StatusManager, StatusManager, AttackBase, float> handler = null;
        
        handler = (source, target, attack, damage) =>
        {
            if(damage <= 0)
                return;
            attack.OnAttackDealDamage -= handler;
            BattleStageManager.Instance.TimeScaleEffect(0.1f,0.2f);
        };

        proj.GetComponent<AttackFromEnemy>().OnAttackDealDamage += handler;
    }
    
    protected void WaveFast()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action07_2"),
            gameObject.RaycastedPosition()+new Vector2(0,1.5f), InitContainer(false),
            ac.facedir);
        
        Action<StatusManager, StatusManager, AttackBase, float> handler = null;
        
        handler = (source, target, attack, damage) =>
        {
            if(damage <= 0)
                return;
            attack.OnAttackDealDamage -= handler;
            BattleStageManager.Instance.TimeScaleEffect(0.1f,0.2f);
        };

        proj.GetComponent<AttackFromEnemy>().OnAttackDealDamage += handler;
    }

    protected void BurnAround()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action08_1"),
            gameObject.RaycastedPosition()+new Vector2(0,1f),
            InitContainer(false), 1);
        var atk = proj.GetComponent<AttackFromEnemy>();
        atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
            70, 21, 1),100);
        _statusManager.ObtainTimerBuff(new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
            _statusManager.maxHP*0.01f, 21, 1));
        atk.OnAttackHit += HPDrain;

    }

    protected void HPDrain(AttackBase atk, GameObject tar)
    {
        _statusManager.HPRegenImmediatelyWithoutRandomDirectly(_statusManager,
            (int)(_statusManager.maxHP*0.075f));
    }
    
    protected void SetThreeFireCrystalAndAStone()
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
            crystalPositionPresets.Add(anch4.transform.position);
            crystalPositionPresets.Add(anch3.transform.position);
        }

        float waitTime = 0;

        for (int i = 0; i < 4; i++)
        {
            var hint = GenerateWarningPrefab("action01_1", crystalPositionPresets[i] ,
                Quaternion.identity, RangedAttackFXLayer.transform);

            if (waitTime <= 0)
            {
                waitTime = hint.GetComponent<EnemyAttackHintBar>().warningTime;
            }
        }
        
        int stoneIndex = Random.Range(0, 4);
        Vector2 stonePos = crystalPositionPresets[stoneIndex];
        List<Vector2> fireCrystalPos = new();
        for (int i = 0; i < 4; i++)
        {
            if (i == stoneIndex)
            {
                continue;
            }
            fireCrystalPos.Add(crystalPositionPresets[i]);
        }

        DOVirtual.DelayedCall(waitTime, () =>
        {
            SetFireCrystalAdvanced(fireCrystalPos[0]);
            SetFireCrystalAdvanced(fireCrystalPos[1]);
            SetFireCrystalAdvanced(fireCrystalPos[2]);
            SetStoneAdvanced(stonePos);
            
        },false);

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
            crystalPositionPresets.Add(anch4.transform.position);
            crystalPositionPresets.Add(anch3.transform.position);
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
                crystalPos.Add(crystalPositionPresets[2]);
                stonePos.Add(crystalPositionPresets[0]);
                stonePos.Add(crystalPositionPresets[3]);
                break;
            case 2:
                crystalPos.Add(crystalPositionPresets[2]);
                crystalPos.Add(crystalPositionPresets[3]);
                stonePos.Add(crystalPositionPresets[0]);
                stonePos.Add(crystalPositionPresets[1]);
                break;
            case 3:
                crystalPos.Add(crystalPositionPresets[0]);
                crystalPos.Add(crystalPositionPresets[3]);
                stonePos.Add(crystalPositionPresets[1]);
                stonePos.Add(crystalPositionPresets[2]);
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
            var hint = GenerateWarningPrefab("action01_1", crystalPositionPresets[i] ,
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
            SetStoneAdvanced(stonePos[0]);
            SetStoneAdvanced(stonePos[1]);
            
        },false);
        
    }

    protected void SetFiveFireCrystals()
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
            crystalPositionPresets.Add(anch4.transform.position);
            crystalPositionPresets.Add(anch3.transform.position);
        }
        
        float waitTime = 0;
        List<Vector2> fireCrystalPos = new();
        
        fireCrystalPos.Add(new Vector2(crystalPositionPresets[0].x - 4,crystalPositionPresets[0].y));
        fireCrystalPos.Add(new Vector2(crystalPositionPresets[1].x + 4,crystalPositionPresets[1].y));
        fireCrystalPos.Add(new Vector2(crystalPositionPresets[2].x - 2,crystalPositionPresets[2].y));
        fireCrystalPos.Add(new Vector2(crystalPositionPresets[3].x + 2,crystalPositionPresets[3].y));
        

        if (Random.Range(0, 2) == 0)
        {
            fireCrystalPos.Add(crystalPositionPresets[3] + new Vector2(-8,0));
        }
        else
        {
            fireCrystalPos.Add(crystalPositionPresets[2] + new Vector2(8,0));
        }
        
        fireCrystalPos.Add(new Vector2(0,crystalPositionPresets[0].y));
        
        
        
        
        for (int i = 0; i < fireCrystalPos.Count; i++)
        {
            var hint = GenerateWarningPrefab("action01_1", fireCrystalPos[i] ,
                Quaternion.identity, RangedAttackFXLayer.transform);

            if (waitTime <= 0)
            {
                waitTime = hint.GetComponent<EnemyAttackHintBar>().warningTime;
            }
        }

        DOVirtual.DelayedCall(waitTime, () =>
        {
            SetFireCrystalAdvanced(fireCrystalPos[0]);
            SetFireCrystalAdvanced(fireCrystalPos[1]);
            SetFireCrystalAdvanced(fireCrystalPos[2]);
            SetFireCrystalAdvanced(fireCrystalPos[3]);
            SetFireCrystalAdvanced(fireCrystalPos[4]);
            SetFireCrystalAdvanced(fireCrystalPos[5]);
        },false);
        
    }

    protected GameObject AllRangedDestructionForcing()
    {
        return InstantiateRanged(GetProjectileOfFormatName("action09_2",true),
            transform.position, InitContainer(false), 1);
    }

    protected void AllRangeDestruction(GameObject proj)
    {
        CineMachineOperator.Instance.CamaraShake(20f,0.2f);
        
        
        var atk = proj.GetComponent<ForcedAttackFromEnemy>();

        var target = _behavior.viewerPlayer.transform;
        
        //从自身到目标发出一道射线，返回命中的Characters层的第一个碰撞点
        
        var hit = Physics2D.Raycast(transform.position, 
            target.position-transform.position, 
            999, LayerMask.GetMask("EnemyObstacle"));
        
        //如果碰撞到的是玩家（父节点有PlayerStatusManager），添加伤害
        
        //删除所有stoneInstances中为null的元素
        
        
        
        if (hit.collider == null || Mathf.Abs(_behavior.targetPlayer.transform.position.x - transform.position.x) < 7.5f)
        {
            atk.target = _behavior.viewerPlayer;
            atk.DealDamageImmediately();
        }

        for (int i = stoneInstances.Count - 1; i >= 0; i--)
        {
            if (stoneInstances[i] == null)
            {
                stoneInstances.RemoveAt(i);
                continue;
            }
            stoneInstances[i].GetComponentInChildren<Projectile_DB015_2>().BreakStone();
            stoneInstances.RemoveAt(i);
        }
        

        atk.GetComponentInChildren<CircleCollider2D>().enabled = true;
    }

    protected void AllRangedDestructionAvoidable()
    {
        InstantiateRanged(GetProjectileOfFormatName("action13_1", true),
            Vector3.zero, InitContainer(false), 1);
        
        BattleStageManager.Instance.TriggerSpecialEvent(1);
        BattleStageManager.Instance.TriggerSpecialEvent(2);

        var buff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageUp,
            10, -1,100);
        buff.dispellable = false;

        _statusManager.ObtainTimerBuff(buff);


    }

    protected GameObject AimingFire()
    {
        var hint = GenerateWarningPrefab("action11", transform.position, Quaternion.identity, 
            RangedAttackFXLayer.transform);
        hint.GetComponent<EnemyAttackHintBarRotater>().target = _behavior.viewerPlayer;
        return hint;
    }

    protected void CheckOverlap(Vector3 euler)
    {
        var atk = InstantiateRanged(GetProjectileOfFormatName("action11_1",true),
            transform.position, InitContainer(false), 1).GetComponent<ForcedAttackFromEnemy>();

        atk.transform.eulerAngles = euler;

        atk.target = _behavior.viewerPlayer;
        
        DOVirtual.DelayedCall(0.1f, () =>
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
            
            var hit = Physics2D.Raycast(transform.position, 
                target.position-transform.position, 
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
                var stone = overlaps[i].GetComponent<Projectile_DB015_2>();
                if(stone != null)
                {
                    stone.BreakStone();
                    stoneInstances.Remove(stone.transform.parent.gameObject);
                }
            }
            
            
            


        }, false);
    }

    protected void FireRing()
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action12_1",true),
            transform.position + new Vector3(0,1), InitContainer(false), 1);
        
        var atk = proj.GetComponent<AttackFromEnemy>();
        
        atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Stun,
            1,Random.Range(4,6),1),80);

    }

    protected void FireCrossRandom(int type)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action12_2",true),
            transform.position + new Vector3(0,1), InitContainer(false), 1);
        
        var atk = proj.GetComponent<AttackFromEnemy>();
        
        atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Stun,
            1,Random.Range(4,6),1),80);

        if (type != 0)
        {
            proj.transform.eulerAngles = new Vector3(0,0,45);
        }
        
        
    }

    protected void FireCrossAllRanged(int type)
    {
        var debuff = new TimerBuff((int)BasicCalculation.BattleCondition.Stun,
            1, Random.Range(4, 6), 1);
        
        if (type == 0)
        {
            var proj = InstantiateRanged(GetProjectileOfFormatName("action12_3",true),
                new Vector3(0,10), InitContainer(false), 1);
            proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(debuff,100);
        }
        else
        {
            var container = InitContainer(false);
            var pfb = GetProjectileOfFormatName("action12_4",true);
            
            var proj1 = InstantiateRanged(
                pfb ,new Vector3(12,0), container, 1);
            var proj2 = InstantiateRanged(
                pfb ,new Vector3(-12,0), container, 1);
            var proj3 = InstantiateRanged(
                pfb ,new Vector3(12,16), container, 1);
            var proj4 = InstantiateRanged(
                pfb ,new Vector3(-12,16), container, 1);
            
            proj1.GetComponent<AttackFromEnemy>().AddWithConditionAll(debuff,100);
            proj2.GetComponent<AttackFromEnemy>().AddWithConditionAll(debuff,100);
            proj3.GetComponent<AttackFromEnemy>().AddWithConditionAll(debuff,100);
            proj4.GetComponent<AttackFromEnemy>().AddWithConditionAll(debuff,100);
            
            
        }
    }

    private void PhaseChangeEffect1()
    {
        Instantiate(GetProjectileOfFormatName("action14_1",true),
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
        BattleStageManager.Instance.TriggerSpecialEvent(1);
        BattleStageManager.Instance.TriggerSpecialEvent(2);
    }
    
    private void PhaseChangeEffect2()
    {
        Instantiate(GetProjectileOfFormatName("action14_2",true),
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
    }


}
