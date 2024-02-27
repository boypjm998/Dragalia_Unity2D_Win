using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ActorController_c018 : ActorControllerRangedWithFS
{
    public bool sigilReleased;
    private float[] forcingTimeRequired = {0.5f, 2f, 2f};
    [SerializeField] private GameObject[] visualEffectsAfterSigilReleased;
    
    
    
    private TimerBuff attackRateBuff;
    
    
    
    
    protected override void Awake()
    {
        base.Awake();
        pi.buttonAttack.delayingDuration = 0.2f;
        attackRateBuff = new TimerBuff((int)BasicCalculation.BattleCondition.AttackRateUp,
            20, -1, 1, 101803);
        attackRateBuff.dispellable = false;
        _statusManager.OnTakeDirectDamageFrom += RemoveBlessingBuff;

    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GlobalController.currentGameState == GlobalController.GameState.Inbattle);
        GrantSigilLocked();
    }

    protected override void Update()
    {
        base.Update();
        CheckForceStrike();
    }

    private void GrantSigilLocked()
    {
        TimerBuff lockedSigil = new TimerBuff((int)BasicCalculation.BattleCondition.LockedSigil,
            -1, 120, 1, 101801);
        lockedSigil.dispellable = false;
        _statusManager.ObtainTimerBuff(lockedSigil);
        _statusManager.OnBuffExpiredEventDelegate += SigilReleaseCheck;
        _statusManager.OnHPChange += GrantAttackRateBuffWhenHPOver50;
        GrantAttackRateBuffWhenHPOver50();
    }

    protected void GrantAttackRateBuffWhenHPOver50()
    {
        if (_statusManager.currentHp >= _statusManager.maxHP * 0.5f)
        {
            if (_statusManager.GetExactConditionsOfType
                ((int)BasicCalculation.BattleCondition.AttackRateUp,
                    101803).Count == 0)
            {
                _statusManager.ObtainTimerBuff(attackRateBuff);
            }
            if (anim.GetBool("isAttack"))
            {
                anim.speed = 1.2f;
            }
        }
        else
        {
            _statusManager.RemoveSpecificTimerbuff
            ((int)BasicCalculation.BattleCondition.AttackRateUp,
                101803);
            if (anim.GetBool("isAttack"))
            {
                anim.speed = 1;
            }
        }

        
    }

    protected void SigilReleaseCheck(BattleCondition condition)
    {
        if (condition.buffID == (int)BasicCalculation.BattleCondition.LockedSigil)
        {
            //TODO: Sigil release
            
            TimerBuff sigilRelease = new TimerBuff((int)BasicCalculation.BattleCondition.SigilReleased,
                -1, -1, 1, 101801);
            sigilRelease.dispellable = false;
            _statusManager.ObtainTimerBuff(sigilRelease);
            _statusManager.OnBuffExpiredEventDelegate -= SigilReleaseCheck;
            sigilReleased = true;
            anim.SetBool("sigil",true);
            foreach (var eff in visualEffectsAfterSigilReleased)
            {
                eff.gameObject.SetActive(true);
            }
        }
    }

    private void RemoveBlessingBuff(StatusManager myStatus,StatusManager enemyStatus,AttackBase atk, float damage)
    {
        if (Projectile_C018_1.Instance != null)
        {
            if(Projectile_C018_1.Instance.TargetInRange())
                return;
        }
        
        _statusManager.RemoveSpecificTimerbuff
        ((int)BasicCalculation.BattleCondition.GabrielsBlessing,
            -1);


    }

    protected override void CheckForceStrike()
    {
        
        
        
        if(!pi.buttonAttack.isDelaying && pi.buttonAttack.IsPressing && pi.attackEnabled
           && !pi.hurt && grounded && !pi.isSkill && !anim.GetCurrentAnimatorStateInfo(0).IsName("walk"))
        {
            //print(forceLevel);
            if (forceLevel < 0)
            {
                forceLevel = 0;
                forcingTime = 0;
            }
            else
            {
                forcingTime += Time.deltaTime * (1+_statusManager.fsSpeedBuff);
                if(forcingTime > forcingTimeRequired[forceLevel] && forceLevel < maxForceLevel)
                {
                    forceLevel++;
                    forcingTime = 0;
                }

                if (forceLevel >= maxForceLevel)
                {
                    forcingTime = forcingRequireTime;
                }
            }
        }
        else
        {
            if(forceLevel > 0)
            {
                forceLevel = -1;
                forcingTime = 0;
                anim.Play("force_attack");
                
            }else if(forceLevel == 0)
            {
                forceLevel = -1;
                OnForceAttackExit();
                //10.25
                if(!pi.hurt)
                    anim.Play("idle");
                forcingTime = 0;
            }
        }
    }

    protected override void ResetCombo()
    {
        if (Combo >= 5)
        {
            if(!sigilReleased)
            {
                Combo = 0;
            }
            else
            {
                Combo = 6;
            }
        }
    }

    public override void OnDashEnter()
    {
        base.OnDashEnter();
        voiceController.PlayAttackVoice(0);
    }

    public override void onRollEnter()
    {
        base.onRollEnter();
        if (sigilReleased && Combo > 5)
        {
            Combo = 0;
        }
    }

    public override void OnStandardAttackEnter()
    {
        base.OnStandardAttackEnter();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("combo6") && sigilReleased)
        {
            (voiceController as VoiceControllerPlayer).PlayAttackVoice(Random.Range(5,9));
            _statusManager.knockbackRes = 999;
        }
    }

    public override void OnSkillEnter()
    {
        base.OnSkillEnter();
        if (sigilReleased && anim.GetCurrentAnimatorStateInfo(0).IsName("s1"))
        {
            print("ReconnectedCombo");
            Combo = 4;
            if (comboStageResetRoutine != null)
            {
                StopCoroutine(comboStageResetRoutine);
                comboStageResetRoutine = null;
            }
        }
    }

    public override void OnSkillExit()
    {
        base.OnSkillExit();
        if (sigilReleased && anim.GetCurrentAnimatorStateInfo(0).IsName("s1"))
        {
            Combo = 4;
        }
    }

    public override void OnStandardAttackExit()
    {
        base.OnStandardAttackExit();
        _statusManager.knockbackRes = 0;
    }
}
