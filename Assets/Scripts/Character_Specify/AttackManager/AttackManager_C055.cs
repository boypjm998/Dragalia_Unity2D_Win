using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class AttackManager_C055 : AttackManagerMeeleWithFS
{
    private bool _boosted = false;

    protected override void Start()
    {
        base.Start();
        (_statusManager as PlayerStatusManager).SetSPChargeRate(2,0);
    }

    public void Skill1()
    {
        var proj = InstantiateMeele(skillFX[0], transform.position,
            InitContainer(true, 1, true));
        
        TimerBuff bleedingBuff = new AdvancedTimerBuff(_boosted?219f:146f,30);

        if (_boosted)
        {
            bleedingBuff.dispellable = false;
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.KnockBackImmune,
                1, 15,1,-1);
        }
        
        
        TimerBuff shadowblightBuff = new TimerBuff((int)BasicCalculation.BattleCondition.ShadowBlight,
            41f, 21, 100);
        
        var atk = proj.GetComponent<AttackFromPlayer>();
        
        atk.AddWithConditionAll(bleedingBuff,_boosted?100:90);
        atk.AddWithConditionAll(shadowblightBuff,120,1);
        
        _boosted = false;
    }

    public void Skill2()
    {
        var fx = Instantiate(skillFX[1],transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);

        _statusManager.ObtainTimerBuffs((int)BasicCalculation.BattleCondition.SlumberStrikeStance,
            -1,3,3,-1,false,
            (int)BasicCalculation.BattleCondition.ForceStrikeDmgBuff);

        
        var currentBuffAmount = _statusManager.GetConditionTotalValue((int)BasicCalculation.BattleCondition.MaxHPBuff);
        var remainBuff = Mathf.Min(30 - currentBuffAmount, 10);
        if (remainBuff > 0)
        {
            _statusManager.ObtainTimerBuff(new TimerBuff((int)BasicCalculation.BattleCondition.MaxHPBuff,
                remainBuff, -1, 100),false);
        }
        _statusManager.HPRegenImmediatelyWithoutRandomDirectly(_statusManager,
            (Mathf.CeilToInt(_statusManager.maxBaseHP * 0.1f)));

        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
            40, 20);
        
        _boosted = true;

    }
    
    public override void ForceStrike_Axe()
    {
        (ac as ActorControllerMeeleWithFS).PlayAttackVoice(9);
        GameObject proj;
        
        if (_statusManager.HasCondition((int)BasicCalculation.BattleCondition.SlumberStrikeStance))
        {
            proj = InstantiateMeele(forceFX[1], transform.position, InitContainer(true));
            
            _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.SlumberStrikeStance);
            
            var atk = proj.GetComponent<AttackFromPlayer>();
            atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Sleep,
                1, Random.Range(6f,7f), 1),110);
            atk.AddWithConditionAll(new TimerBuff(999),100,1);
        }
        else
        {
            proj = InstantiateMeele(forceFX[0], transform.position, InitContainer(true));
        }

    }
    
    
}
