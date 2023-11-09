using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class AttackManager_C004 : AttackManagerDagger
{
    private Coroutine _abilityCDTick;
    private bool _abilityCDTickRunning = false;
    
    protected override void Awake()
    {
        base.Awake();
        _statusManager.OnAfflictionResist += CheckConditionAbility;

    }


    public override void Skill1(int eventID)
    {
        if(eventID!=1)
            return;
        
        

        var container = Instantiate(attackContainer, transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        var proj = InstantiateMeele(skill1FX, transform.position, container);
        var atk = proj.GetComponent<AttackFromPlayer>();
        container.GetComponent<AttackContainer>().InitAttackContainer(1,true);

        var checkConditionString = ((int)BasicCalculation.BattleCondition.Burn).ToString();
        
        var conditional_eff = new ConditionalAttackEffect
            (ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                new string[] {"1", checkConditionString},
                new string[] {"0.2"});
        
        atk.AddConditionalAttackEffect(conditional_eff);
        
        var burn = new TimerBuff((int)BasicCalculation.BattleCondition.Burn, 72, 12,100,-1);
        atk.AddWithConditionAll(burn,110);
        
    }
    
    public override void Skill2(int eventID)
    {
        if (eventID == 1)
        {
            var container = Instantiate(attackContainer, transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
            var proj = InstantiateMeele(skill2FX, transform.position, container);
            var atk = proj.GetComponent<AttackFromPlayer>();

            var checkConditionString = ((int)BasicCalculation.BattleCondition.Burn).ToString();
        
            var conditional_eff = new ConditionalAttackEffect
            (ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                new string[] {"1", checkConditionString},
                new string[] {"0.8"});
        
            atk.AddConditionalAttackEffect(conditional_eff);
            
            var defDownBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefDebuff, 5, 10, 1, -1);
            atk.AddWithConditionAll(defDownBuff,100);
            container.GetComponent<AttackContainer>().InitAttackContainer(1,true);
            var burn = new TimerBuff((int)BasicCalculation.BattleCondition.Burn, 72, 12,100,-1);
            atk.AddWithConditionAll(burn,110,1);
            if (_statusManager.comboHitCount >= 15)
            {
                atk.AddWithConditionAll(new TimerBuff(999),100,2);
            }
        }
        else
        {
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff, 25, 15);
            var buff2 = new TimerBuff((int)BasicCalculation.BattleCondition.KnockBackImmune, 100, 10, 100, -1);
            buff2.dispellable = false;
            _statusManager.ObtainTimerBuff(buff2,false);
        }

    }

    public override void Skill3(int eventID)
    {
        if (eventID == 1)
        {
            var container = Instantiate(attackContainer, transform.position, Quaternion.identity,RangedAttackFXLayer.transform);
            var proj = InstantiateMeele(skill3FX, transform.position+new Vector3(0,1.5f,0), container);
            var atk = proj.GetComponent<AttackFromPlayer>();
            
            
            var defDownBuff = new TimerBuff((int)BasicCalculation.BattleCondition.BurnResDown, 20, 30, 1, -1);
            atk.AddWithConditionAll(defDownBuff,100);
            container.GetComponent<AttackContainer>().InitAttackContainer(1,true);

            if (_statusManager.GetExactConditionsOfType(1, 1041).Count < 3)
            {
                var atkBuff = new TimerBuff((int)(BasicCalculation.BattleCondition.AtkBuff), 15f, -1, 3, 1041);
                atkBuff.dispellable = false;
                _statusManager.ObtainTimerBuff(atkBuff);
            
                var critBuff = new TimerBuff((int)(BasicCalculation.BattleCondition.CritRateBuff), 5f, -1, 3, 1041);
                critBuff.dispellable = false;
                _statusManager.ObtainTimerBuff(critBuff);
            }else
            {
                _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.AtkBuff),25,20);
                _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.BurnPunisher),10,20);

            }

            

        }
    }
    
    public override void Skill4(int eventID)
    {
        
        _statusManager.HPRegenImmediately(0,10,true);
        BattleEffectManager.Instance.SpawnHealEffect(gameObject);
        //Instantiate(healbuff, transform.position, Quaternion.identity, BuffFXLayer.transform);
        // _statusManager.ObtainTimerBuff
        // ((int)BasicCalculation.BattleCondition.HealOverTime,
        //     -10,15);
        _statusManager.ObtainHealOverTimeBuff(10,15,true);
        
    }

    private void CheckConditionAbility(BattleCondition condition)
    {
        if (condition.buffID != (int)BasicCalculation.BattleCondition.Burn)
        {
            return;
        }
        
        if (!_abilityCDTickRunning)
        {
            _abilityCDTick = StartCoroutine(AbilityTick(15f));

            var playerStat = (_statusManager as PlayerStatusManager);

            var sp_s3 = playerStat.requiredSP[2] * 0.5f;
            
            playerStat.ChargeSP(2,sp_s3);
            
            playerStat.OnSpecialBuffDelegate?.Invoke
                (UI_BuffLogPopManager.SpecialConditionType.SPCharge.ToString());
        }
    }

    private IEnumerator AbilityTick(float cd)
    {
        _abilityCDTickRunning = true;
        yield return new WaitForSeconds(cd);
        _abilityCDTickRunning = false;
    }


}
