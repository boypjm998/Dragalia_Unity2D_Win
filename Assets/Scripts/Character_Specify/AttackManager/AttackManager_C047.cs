using GameMechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class AttackManager_C047 : AttackManagerRanged
{

    TimerBuff abyssalConnectionBuff = new TimerBuff((int)BasicCalculation.BattleCondition.AbyssalConnection,
        1, 30, 1);

    private int _totalDamageReceived;
    
    private AbilityClock _abilityClock = new(15);

    protected override void Awake()
    {
        base.Awake();
        abyssalConnectionBuff.dispellable = false;
        abyssalConnectionBuff.extra_iconID = (int)BasicCalculation.BattleCondition.SkillDmgBuff;
    }

    protected override void Start()
    {
        base.Start();
        _statusManager.OnHPDecrease += AddTotalDamage;
        _statusManager.OnAfflictionInflict += CheckAfflictionInflict;
    }

    private void CheckAfflictionInflict(BattleCondition condition)
    {
        if(_abilityClock.Available == false)
            return;

        if (condition.buffID == (int)BasicCalculation.BattleCondition.Poison)
        {
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
                13, 10);
            _abilityClock.StartTick();
        }
    }
    
    private void AddTotalDamage(int amount, AttackBase atk)
    {
        amount = Mathf.Max(amount, 0);
        _totalDamageReceived += amount;
    }

    private void OnSkillEnter()
    {
        var damageDealt = Mathf.CeilToInt(_statusManager.currentHp * 0.1f);
        if (damageDealt > 0)
        {
            damageDealt = BattleStageManager.Instance.CauseIndirectDamage(_statusManager,
                damageDealt, false);
            var hpPercentage = Mathf.Clamp(100f * (float)damageDealt / (float)_statusManager.maxHP, 
                1, 10);
            (_statusManager as PlayerStatusManager).FillSP(0,(int)hpPercentage);
            (_statusManager as PlayerStatusManager).FillSP(1,(int)hpPercentage);
            (_statusManager as PlayerStatusManager).FillSP(2,(int)hpPercentage);
            (_statusManager as PlayerStatusManager).FillSP(3,(int)hpPercentage);
            _statusManager.OnSpecialBuffDelegate?.Invoke(UI_BuffLogPopManager.SpecialConditionType.SPCharge.ToString());
            //print("Damage Dealt"+hpPercentage);
        }
    }

    public void Skill3_Muzzle()
    {
        
        InstantiateBuff(skill3FX[0],gameObject.RaycastedPosition());

    }
    public void Skill3_Attack()
    {
        var boosted = _statusManager.HasCondition((int)BasicCalculation.BattleCondition.AbyssalConnection);
        
        var proj = InstantiateRanged
            (boosted?skill3FX[2]:skill3FX[1], gameObject.RaycastedPosition(), InitContainer(false, 1, true), ac.facedir);
        var atk = proj.GetComponent<AttackFromPlayer>();

        atk.AddWithConditionAll
            (new TimerBuff((int)(BasicCalculation.BattleCondition.Poison), 58, 15, 100), 120);

        var extraDamage = Mathf.Clamp(_totalDamageReceived * 15, 0, _statusManager.maxHP * 15);

        if(extraDamage > 0)
        {
            atk.attackInfo[0].dmgModifier.Add(0);
            atk.attackInfo[0].constDmg.Add(0);
            atk.attackInfo[0].constDmg.Add(extraDamage);
        }

        _totalDamageReceived = 0;


    }

    public void Skill2_Buff()
    {
        _statusManager.ObtainTimerBuff(new TimerBuff(abyssalConnectionBuff));
        InstantiateBuff(skill2FX[0],gameObject.RaycastedPosition());
    }

    public void Skill1_Heal()
    {
        _statusManager.HPRegenImmediately(36, 0, true);
    }


    public void Skill1_Attack()
    {
        var target = ta.GetNearestTargetInRangeDirection(ac.facedir, 20, 2, LayerMask.GetMask("Enemies"));
        Vector2 targetPos;
        if(target != null)
        {
            targetPos = target.gameObject.RaycastedPosition();
        }
        else
        {
            targetPos = gameObject.RaycastedPosition();
        }

        GameObject prefab;

        if(!_statusManager.HasCondition((int)BasicCalculation.BattleCondition.AbyssalConnection))
        {
            prefab = skill1FX[0];
        }
        else
        {
            prefab = skill1FX[1];
            var min = Mathf.Min(transform.position.x + 16 * ac.facedir, transform.position.x + 5 * ac.facedir);
            var max = Mathf.Max(transform.position.x + 16 * ac.facedir, transform.position.x + 5 * ac.facedir);
            targetPos.x = Mathf.Clamp(targetPos.x, min, max);
        }

        var proj = InstantiateRanged
            (prefab, targetPos, InitContainer(false, 1, true),1);
        var atk = proj.GetComponent<AttackFromPlayer>();

        int maxCap = (int)(_statusManager.maxHP * 0.1f);
        int currentHealed = 0;

        var checkConditionString = ((int)BasicCalculation.BattleCondition.Poison).ToString();
        atk.AddConditionalAttackEffect(
            new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                new string[] { "1", checkConditionString },
                new string[] { "0.5" })
        );

        //todo: if abyssal connection is active

        atk.OnAttackDealDamage += (statusManagerSelf, statusManagerTarget, attack, dmg) =>
        {
            if (currentHealed >= maxCap) return;
            var clamp = Mathf.Abs(maxCap - currentHealed);
            currentHealed += LifeStealWithReturn(statusManagerSelf,
                (int)dmg, 3, 15, clamp);
        };
    }
}
