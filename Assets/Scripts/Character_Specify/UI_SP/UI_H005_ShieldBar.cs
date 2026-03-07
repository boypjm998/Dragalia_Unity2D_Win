using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameMechanics;
using UnityEngine;

public class UI_H005_ShieldBar : MonoBehaviour
{
    [SerializeField] Transform shieldBarParent;
    private StatusManager shieldStat;
    private SpriteRenderer shieldBack;
    private SpriteRenderer shieldFront;
    
    [SerializeField][Range(0,0.2f)] private float depeltionRate = 0.02f;
    
    public event Action OnShieldDeath;
    public StatusManager mainStat;
    
    private void Start()
    {
        shieldStat = GetComponent<StatusManager>();
        shieldStat.DebuffResistance = 999;
        shieldStat.ImmuneToAllControlAffliction = true;
        shieldStat.ImmuneToAllDotAffliction = true;
        shieldStat.ImmuneToAllOtherAffliction = true;
        
        shieldBack = shieldBarParent.Find("Back").GetComponent<SpriteRenderer>();
        shieldFront = shieldBarParent.Find("Fill").GetComponent<SpriteRenderer>();
        
        shieldStat.OnHPChange += UpdateShield;
        shieldStat.AddEffectFunction(StandardAttackOnly, AbilityCalculation.ProductArea.DMGCUT);
        //shieldStat.SpecialDamageCutEffectFunc += StandardAttackOnly;
        shieldStat.OnTakeDirectDamageFrom += DamageReflectionCheck;
        
        InvokeRepeating("ShieldDepletion",15,1);
    }
    
    private void ShieldDepletion()
    {
        if(shieldStat.currentHp <= 0)
            return;
        
        shieldStat.currentHp -= (int)(shieldStat.maxHP * depeltionRate);
        shieldStat.OnHPChange?.Invoke();
    }

    private void OnDestroy()
    {
        OnShieldDeath?.Invoke();
        
        CancelInvoke();
        shieldStat.OnHPChange -= UpdateShield;
        shieldStat.RemoveEffectFunc(StandardAttackOnly, AbilityCalculation.ProductArea.DMGCUT);
        //shieldStat.SpecialDamageCutEffectFunc -= StandardAttackOnly;
        shieldStat.OnTakeDirectDamageFrom -= DamageReflectionCheck;
        shieldStat.OnHPDecrease = null;
    }

    private (float, float) StandardAttackOnly(StatusManager src, AttackBase atk, StatusManager tar)
    {
        if ((atk.attackType) == BasicCalculation.AttackType.STANDARD ||
            atk.attackType == BasicCalculation.AttackType.DSTANDARD ||
            atk.attackType == BasicCalculation.AttackType.OTHER)
        {
            return (0, 0);
            
        }
        else
        {
            return (10, 0);
        }


    }

    private void DamageReflectionCheck(StatusManager self, StatusManager target, AttackBase atk, float dmg)
    {
        //print("Damage Reflection Activated Before!");
        
        
        if ((atk.attackType) == BasicCalculation.AttackType.STANDARD ||
            atk.attackType == BasicCalculation.AttackType.DSTANDARD ||
            atk.attackType == BasicCalculation.AttackType.OTHER)
            return;
        
        print("Damage Reflection Activated!");
        
        
        if (target.GetExactConditionsOfType((int)BasicCalculation.BattleCondition.AtkDebuff, 8202501).Count < 5)
        {
            target.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff, 10, 10, 5,
                8202501);
        }
        
        float modifier = (atk.attackInfo[0].dmgModifier.Sum());
            
        BattleStageManager.Instance.CauseIndirectDamage(target,
            (int)(50*modifier),false,false);
        
        self.HPRegenImmediatelyWithoutRandomDirectly(mainStat, (int)(modifier * 1000));
        
    }

    private void UpdateShield()
    {
        float hpFraction = (float)(shieldStat.currentHp) / shieldStat.maxHP;
        print(hpFraction);
        print(hpFraction*shieldBack.size.x);
        
        shieldFront.size = new Vector2(hpFraction*shieldBack.size.x,shieldBack.size.y);
        if(shieldStat.currentHp <= 0)
        {
            shieldBarParent.gameObject.SetActive(false);
        }
    }
    
    
}
