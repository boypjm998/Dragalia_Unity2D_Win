using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class AttackManager_C014 : AttackManagerMeeleWithFS
{
    TimerBuff _bleedingBuff = new AdvancedTimerBuff(186f,30);

    GameObject _skill1Container;

    protected override void Start()
    {
        base.Start();
        _statusManager.AddEffectFunction(BladeFormationEffect,AbilityCalculation.ProductArea.CRITRATE);
        //_statusManager.SpecialCritEffectFunc += BladeFormationEffect;
    }

    public void Skill1_Bleeding(int eventID)
    {
        if (eventID == 0)
        {
            _skill1Container = InitContainer(true, 1, true);
        }
        

        var atk = InstantiateMeele(skillFX[eventID], transform.position,
            _skill1Container
        ).GetComponent<AttackFromPlayer>();

        atk.AddWithConditionAll(new TimerBuff(_bleedingBuff),100);
    }

    public void Skill_Flash()
    {
        InstantiateBuff(skillFX[8], transform.position);
    }

    public void Skill2()
    {
        _statusManager.
            ObtainTimerBuff((int)BasicCalculation.BattleCondition.BladeFormation, 1, 15);
        
        _statusManager.
            ObtainHealOverTimeBuff(29f, 15);

        //_statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff, 30, 15);

        Instantiate(skillFX[9], transform.position, skillFX[9].transform.rotation, RangedAttackFXLayer.transform);

    }

    public static (float, float) BladeFormationEffect
        (StatusManager src, AttackBase atk, StatusManager target)
    {
        if (src.HasCondition((int)(BasicCalculation.BattleCondition.BladeFormation)) &&
            target.HasCondition((int)(BasicCalculation.BattleCondition.Bleeding)))
        {
            return (30, 0);
        }

        return (0, 0);
    }


    public void Skill3_Attack()
    {
        var proj = InstantiateMeele(skillFX[10], transform.position, InitContainer(true,1,true));
        var atk = proj.GetComponent<AttackFromPlayer>();
        var caf = new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
            ConditionalAttackEffect.ExtraEffect.Custom,
            new []{ "1", ((int)BasicCalculation.BattleCondition.Bleeding).ToString() },
            new[]{""}).SetEffectFunction((stats, atk) =>
        {
            var totalEffect = stats.targetStat.GetConditionTotalValue((int)BasicCalculation.BattleCondition.Bleeding);
            if (totalEffect > 0)
            {
                print(totalEffect);
                return (int)(totalEffect * 0.03f);
            }
            else
            {
                //BattleStageManager.Instance.ObtainAfflictionDirectlyWithCheck
                return 0;
            }
        });

        atk.AddConditionalAttackEffect(caf);
    }
}
