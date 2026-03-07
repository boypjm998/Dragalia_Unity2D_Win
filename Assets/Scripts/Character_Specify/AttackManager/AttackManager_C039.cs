using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C039 : AttackManagerRanged
{
    private bool _abilityReady = true;
    private float _abilityCD = 5;
    private Tween _abilityCDTween = null;
    protected override void Start()
    {
        base.Start();
        _abilityReady = true;
        _statusManager.OnAfflictionInflict += ParalysisTriggered;
    }

    private void ParalysisTriggered(BattleCondition condition)
    {
        if(_abilityReady == false)
            return;
        
        print("Triggered");
        
        if (condition.buffID == (int)BasicCalculation.BattleCondition.Paralysis)
        {
            _abilityReady = false;
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff, 13, 10);
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff, 13, 10);
            _abilityCDTween = DOVirtual.DelayedCall(_abilityCD, () => _abilityReady = true,
                false);
        }
    }

    public void Skill1_Muzzle()
    {
        var fx = Instantiate(skill1FX[1], transform.position + new Vector3(ac.facedir*1.5f,3f),
            Quaternion.identity, RangedAttackFXLayer.transform);
    }

    public void Skill1()
    {
        var proj = InstantiateRanged(skill1FX[0],transform.position + new Vector3(2*ac.facedir,-0.1f),
            InitContainer(false,1,true),ac.facedir,0);
        
        var atk = proj.GetComponent<AttackFromPlayer>();
        atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Paralysis,
            48.6f,15,100),120);
        
        var checkConditionString = ((int)BasicCalculation.BattleCondition.Paralysis).ToString();
        atk.AddConditionalAttackEffect(
            new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                new string[] {"1", checkConditionString},
                new string[] {"0.2"})
        );
    }

    public void Skill2()
    {
        InstantiateBuff(skill2FX[0], transform.position);
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.ParalysisRateUp, 20, 20);
        _statusManager.EnergyLevelUp(2);
        _statusManager.AddLifeShield((int)(_statusManager.maxHP*0.5f), (int)(_statusManager.maxHP*0.3f));
        
        
        //todo: 根据生命盾提升技能伤害
        var lifeShield =
            _statusManager.GetConditionOfTypeWithMaxEffect((int)(BasicCalculation.BattleCondition.LifeShield));

        if (lifeShield != null)
        {
            print(lifeShield.effect);
            print((lifeShield.effect / (float)_statusManager.maxHP));

            int amount = Mathf.CeilToInt(10 * (lifeShield.effect / (float)_statusManager.maxHP)) + 15;
            
            //print(amount);

            amount = Mathf.Clamp(amount, 10, 20);

            var specialBuff = new TimerBuff((int)BasicCalculation.BattleCondition.SkillDmgBuff,
                amount, 15, 1, 103902);
            specialBuff.dispellable = false;
            
            _statusManager.ObtainTimerBuff(specialBuff);
        }
    }
    
    
    public void Skill3_Muzzle()
    {
        InstantiateBuff(skill3FX[0], transform.position);
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff, 20, 5);
        
    }

    public void Skill3_Burst()
    {
        var proj = InstantiateMeele
            (skill3FX[1], transform.position - new Vector3(0,1.5f), InitContainer(true, 1, true));
        var atk = proj.GetComponent<AttackFromPlayer>();

        int maxCap = (int)(_statusManager.maxHP * 0.3f);
        int currentHealed = 0;
        
        var checkConditionString = ((int)BasicCalculation.BattleCondition.Paralysis).ToString();
        atk.AddConditionalAttackEffect(
            new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                new string[] {"1", checkConditionString},
                new string[] {"0.3"})
        );
        
        atk.OnAttackDealDamage += (statusManagerSelf, statusManagerTarget, attack, dmg) =>
        {
            if(currentHealed >= maxCap) return;
            var clamp = Mathf.Abs(maxCap - currentHealed);
            currentHealed += LifeStealWithReturn(statusManagerSelf,
                (int)dmg,5,30,clamp);
        };
    }

    public override void Skill4(int eventID)
    {
        _statusManager.HPRegenImmediately(130,0,true);
        InstantiateBuff(skill4FX[0], transform.position);
        // _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff, 
        //     15, 60,1,103901);
        _statusManager.ReliefOneDoTAffliction();
        
        
        



    }
}
