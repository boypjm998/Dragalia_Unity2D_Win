using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C052 : AttackManagerRanged
{
    private ActorControllerGun acGun;
    private bool enhanced;
    private ConditionalAttackEffect caf1;
    private ConditionalAttackEffect caf2;
    private ConditionalAttackEffect caf3;

    private AbilityClock _abilityClock = new(90);
    public bool Overclocked { private set; get; }

    

    public float currentSP;

    
    
    
    protected override void Awake()
    {
        base.Awake();
        acGun = GetComponent<ActorControllerGun>();
        GetComponent<PlayerStatusManager>().SetSPChargeRate(0,0);
        GetComponent<PlayerStatusManager>().SetSPChargeRate(1,0);
    }

    protected override void Start()
    {
        base.Start();
        ArmorGauge.Instance?.SetActor(this);
        _statusManager.OnBuffEventDelegate += CheckOverclock;
        _statusManager.OnBuffExpiredEventDelegate += CheckOverclock;
        _statusManager.OnBuffDispelledEventDelegate += CheckOverclock;
        BattleStageManager.Instance.OnGameStart += ResetGauge;
        
        var checkConditionString2 = ((int)BasicCalculation.BattleCondition.Flashburn).ToString();
        caf1 = (
            new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                new string[] {"1", checkConditionString2},
                new string[] {"0.2"})
        );
        
        Func<StatusManager, StatusManager, bool> conditionalFunc1 = (src, tar) =>
        {
            if (tar is SpecialStatusManager)
            {
                if((tar as SpecialStatusManager).broken)
                    return true;
            }
            return false;
        };
        
        caf2 = new ConditionalAttackEffect(conditionalFunc1,
            ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
            new string[] {},new string[] {"2"});
        
        caf3 = (
            new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                new string[] {"1", checkConditionString2},
                new string[] {"1"})
        );
        
        
        
    }

    private void OnDestroy()
    {
        BattleStageManager.Instance.OnGameStart -= ResetGauge;
    }

    private void Update()
    {
        if (_statusManager.currentHp < _statusManager.maxHP * 0.3f)
        {
            if (_abilityClock.Available)
            {
                _abilityClock.StartTick();
                if (currentSP > 2000)
                {
                    currentSP -= 2000;
                    ArmorGauge.Instance?.ChargeTo((int)currentSP, (int)currentSP/2000);
                    _statusManager.HPRegenImmediately(0, 60, false);
                }
                else
                {
                    var amount = currentSP;
                    currentSP = 0;
                    ArmorGauge.Instance?.ChargeTo(0, 0);
                    _statusManager.HPRegenImmediately(0, 20+(amount/40), false);
                }
            }
        }
        
        
        
        
        if (currentSP < 4000)
        {
            var amount = Time.deltaTime * (Overclocked ? 300 : 200);
            currentSP += amount;
            currentSP = Mathf.Clamp(currentSP, 0, 4000);
            ArmorGauge.Instance?.ChargeTo((int)currentSP, (int)currentSP/2000);
        }
        else
        {
            currentSP = 4000;
        }

        if (currentSP > 2000)
        {
            (_statusManager as PlayerStatusManager).currentSP[0] = 999999;
            (_statusManager as PlayerStatusManager).currentSP[1] = 999999;
        }
        else
        {
            (_statusManager as PlayerStatusManager).currentSP[0] = 0;
            (_statusManager as PlayerStatusManager).currentSP[1] = 0;
        }
    }

    private void ResetGauge()
    {
        currentSP = 2000;
    }

    private void OnSkillEnter()
    {
        if (ac.anim.GetCurrentAnimatorStateInfo(0).IsName("s1") ||
            ac.anim.GetCurrentAnimatorStateInfo(0).IsName("s2"))
        {
            currentSP -= 2000;
            ArmorGauge.Instance?.ChargeTo((int)currentSP, (int)currentSP/2000);
        }
        
        if(ac.anim.GetCurrentAnimatorStateInfo(0).IsName("s1"))
        {
            _statusManager.knockbackRes = 999;
        }
        
    }
    
    private void OnSkillExit()
    {
        
        _statusManager.ResetKBRes();

    }
    
    private void CheckOverclock(BattleCondition condition)
    {
        if (_statusManager.HasCondition((int)BasicCalculation.BattleCondition.Overclock))
        {
            Overclocked = true;
        }
        else 
        {
            Overclocked = false;
        }
    }

    public override void ComboAttack1()
    {
        if (Overclocked)
        {
            var container = InitContainer(false);

            var shotPoint = FindShotpointInChildren("StandardAttack");
        
            var atk = InstantiateRanged(combo1FX[1],shotPoint.position,container,ac.facedir);
            
            atk.GetComponent<AttackFromPlayer>().AddConditionalAttackEffect(caf1);
        }
        else 
            base.ComboAttack1();
    }

    public void Skill1_ReleaseDrone()
    {
        var droneA = Instantiate(skill1FX[0], 
            transform.position + new Vector3(-1.5f*ac.facedir,0), Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var droneB = Instantiate(skill1FX[0], 
            transform.position + new Vector3(1.5f*ac.facedir,0), Quaternion.identity,
            RangedAttackFXLayer.transform);

        var nearestEnemy = ta.GetNearestTargetInRangeDirection(ac.facedir, 24f, 999f,
            LayerMask.GetMask("Enemies"));

        Vector2 targetPos = new Vector2(transform.position.x, BattleStageManager.Instance.mapBorderB);
        
        if (nearestEnemy != null)
        {
            targetPos.x = nearestEnemy.transform.position.x;
        }

        if (transform.position.y > BattleStageManager.Instance.mapBorderB + 10)
        {
            targetPos.y = transform.position.y - 10;
        }

        droneA.transform.DOMove(targetPos + new Vector2(0, 28), 0.3f).SetEase(Ease.InOutSine).SetDelay(0.4f);
        droneB.transform.DOMove(targetPos + new Vector2(0, 28), 0.25f).SetEase(Ease.InOutSine).SetDelay(0.55f);

        DOVirtual.DelayedCall(1, () =>
        {
            var proj = InstantiateRanged(skill1FX[1], targetPos, 
                InitContainer(false,1,true), 1);
            var atk = proj.GetComponent<AttackFromPlayer>();
            atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,41.6f,
                    21,100),
                enhanced?200:120);
            atk.AddConditionalAttackEffect(caf1);

            if (enhanced)
            {
                enhanced = false;
                atk.attackInfo[0].dmgModifier[0] *= 1.25f;
                atk.AddConditionalAttackEffect(caf2);
                Invoke("Skill3_ClearEnhanced", 7.5f);
            }


        }, false);

        
    }

    private void Skill3_ClearEnhanced()
    {
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff, 20, 10);;
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.OverdriveAccerlerator, 50, 10);
    }

    public void Skill2_Overclock()
    {
        var buff = new TimerBuff((int)BasicCalculation.BattleCondition.Overclock, 
            1, 20, 1);
        buff.extra_iconID = (int)BasicCalculation.BattleCondition.FlashburnPunisher;
        buff.dispellable = false;

        var fx = InstantiateBuff(skill2FX[0], transform.position);

        _statusManager.ObtainTimerBuff(buff);
    }
    
    
    public void Skill3_Overdrive()
    {
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DamageCut,
            15, 10);
        
        _statusManager.OnSpecialBuffDelegate?.Invoke(UI_BuffLogPopManager.SpecialConditionType.SkillEnhanced.ToString());
        
        var fx = Instantiate(skill3FX[0], transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        
        enhanced = true;
        
    }
    
    
}
