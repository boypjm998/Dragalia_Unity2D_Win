using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C012 : AttackManagerMeeleWithFS
{
    private GameObject _skill1Container;
    private GameObject _skill2Container;

    private ActorController_c012 acSP;
    private List<StatusManager> _lastHitEnemyStatus = new();

    private TimerBuff _defDebuff = new((int)BasicCalculation.BattleCondition.DefDebuff,
        5, 15, 1, 101201);
    
    private const int SkillChainTime = 4;
    private Tween _skillChainTimer;

    /// <summary>
    /// 10%降防特攻
    /// </summary>
    private ConditionalAttackEffect skillChainEffect1;
    /// <summary>
    /// 10%中毒特攻
    /// </summary>
    private ConditionalAttackEffect skillChainEffect2;
    /// <summary>
    /// 破防特攻
    /// </summary>
    private ConditionalAttackEffect skillChainEffect3;


    private Tween poisonTriggerTimer;
    private const float PoisonTriggerTime = 20f;
    private bool posionTriggerActive = true;
    
    private Tween defDebuffTriggerTimer;
    private const float DefDebuffTriggerTime = 20f;
    private bool defDebuffTriggerActive = true;
    
    TimerBuff _timerBuff = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
        15, 30, 2,101202);
    
    TimerBuff _forceBuff = new TimerBuff((int)BasicCalculation.BattleCondition.AlteredStrikeAlex,
        1, -1, 1,101203);
    
    TimerBuff _overdriveAccBuff = new TimerBuff((int)BasicCalculation.BattleCondition.OverdriveAccerlerator,
        10, 90,1,101204);
    
    
    

    protected override void Start()
    {
        base.Start();
        acSP = ac as ActorController_c012;
        InitCAF();
        _statusManager.OnAfflictionInflict += TriggerBuffPoison;
        _statusManager.OnConditionInflict += TriggerBuffDefdown;
        _timerBuff.dispellable = false;
        _forceBuff.extra_iconID = (int)BasicCalculation.BattleCondition.ForceStrikeDmgBuff;
        UpdateSkillInfo(3);
        (_statusManager as PlayerStatusManager).SetSPChargeRate(2,0);
    }

    private void InitCAF()
    {
        var checkConditionString1 = ((int)BasicCalculation.BattleCondition.DefDebuff).ToString();
        skillChainEffect1 = (
            new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                new string[] {"1", checkConditionString1},
                new string[] {"0.1"})
        );
        
        var checkConditionString2 = ((int)BasicCalculation.BattleCondition.Poison).ToString();
        skillChainEffect2 = (
            new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                new string[] {"1", checkConditionString2},
                new string[] {"0.1"})
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
        
        skillChainEffect3 = new ConditionalAttackEffect(conditionalFunc1,
            ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
            new string[] {},new string[] {"0.5"});

        


    }

    private void TriggerBuffPoison(BattleCondition condition)
    {
        if(posionTriggerActive == false)
            return;
        
        
        if (condition.buffID == (int)BasicCalculation.BattleCondition.Poison)
        {
            posionTriggerActive = false;
            _statusManager.ObtainTimerBuff(new TimerBuff(_timerBuff));
            poisonTriggerTimer = DOVirtual.DelayedCall(PoisonTriggerTime, () =>
            {
                posionTriggerActive = true;
            },false);
        }
    }
    
    private void TriggerBuffDefdown(BattleCondition condition)
    {
        if(defDebuffTriggerActive == false)
            return;
        
        
        if (condition.buffID == (int)BasicCalculation.BattleCondition.DefDebuff)
        {
            defDebuffTriggerActive = false;
            _statusManager.ObtainTimerBuff(new TimerBuff(_timerBuff));
            defDebuffTriggerTimer = DOVirtual.DelayedCall(DefDebuffTriggerTime, () =>
            {
                defDebuffTriggerActive = true;
            },false);
        }
    }
    
    protected void OnStandardAttackEnter()
    {
        
    }

    public void Combo5()
    {
        InstantiateRanged(comboFX[4], transform.position, InitContainer(false), ac.facedir);
    }

    public void Skill1_Dash()
    {
        _skill1Container = InitContainer(true, 2, true);
        
        var proj = InstantiateMeele(skillFX[0], transform.position, _skill1Container);
        var atk = proj.GetComponent<AttackFromPlayer>();

        atk.OnAttackDealDamage += AddLastHitEnemyStatus;

        if (acSP._skill1EffectCurrent == ActorController_c012.SkillChainState.None)
        {
            atk.AddWithConditionAll(new TimerBuff(_defDebuff),100);
        }else if (acSP._skill1EffectCurrent == ActorController_c012.SkillChainState.NormalChain)
        {
            //普通技能链，降防特攻
            atk.AddConditionalAttackEffect(skillChainEffect1);
        }else if (acSP._skill1EffectCurrent == ActorController_c012.SkillChainState.DispelChain)
        {
            //驱散技能链
            atk.AddWithConditionAll(new TimerBuff(999),100);
        }else if(acSP._skill1EffectCurrent == ActorController_c012.SkillChainState.BreakChain)
        {
            //破防特效
            atk.attackInfo[0].dmgModifier[0] *= 1.2f;
            atk.AddConditionalAttackEffect(skillChainEffect3);
        }

    }
    
    public void Skill1_Slash()
    {
        var proj = InstantiateMeele(skillFX[1], transform.position, _skill1Container);
        var atk = proj.GetComponent<AttackFromPlayer>();
        
        atk.OnAttackDealDamage += AddLastHitEnemyStatus;

        
        
        Action<StatusManager, StatusManager, AttackBase, float> handler = null;
        
        if (acSP._skill1EffectCurrent == ActorController_c012.SkillChainState.None)
        {
            atk.AddWithConditionAll(new TimerBuff(_defDebuff),100);
        }else if (acSP._skill1EffectCurrent == ActorController_c012.SkillChainState.NormalChain)
        {
            //普通技能链，降防特攻
            atk.AddConditionalAttackEffect(skillChainEffect1);
            List<StatusManager> attackedEnemy = new List<StatusManager>();
            //追加一个额外hit
            handler = (self, enemy, atkstat, dmg) =>
            {
                if(attackedEnemy.Contains(enemy))
                    return;
                InstantiateAnExtraHitForSkill1(enemy, ActorController_c012.SkillChainState.NormalChain);
                attackedEnemy.Add(enemy);
            };
            
            atk.OnAttackDealDamage += handler;

        }else if (acSP._skill1EffectCurrent == ActorController_c012.SkillChainState.DispelChain)
        {
            //驱散技能链
            atk.AddWithConditionAll(new TimerBuff(999),100);
            List<StatusManager> attackedEnemy = new List<StatusManager>();
            handler = (self, enemy, atkstat, dmg) =>
            {
                if(attackedEnemy.Contains(enemy))
                    return;
                InstantiateAnExtraHitForSkill1(enemy, ActorController_c012.SkillChainState.DispelChain);
                attackedEnemy.Add(enemy);
            };
            atk.OnAttackDealDamage += handler;

        }else if(acSP._skill1EffectCurrent == ActorController_c012.SkillChainState.BreakChain)
        {
            //破防特效
            atk.attackInfo[0].dmgModifier[0] *= 1.2f;
            atk.AddConditionalAttackEffect(skillChainEffect3);
            List<StatusManager> attackedEnemy = new List<StatusManager>();
            handler = (self, enemy, atkstat, dmg) =>
            {
                if(attackedEnemy.Contains(enemy))
                    return;
                InstantiateAnExtraHitForSkill1(enemy, ActorController_c012.SkillChainState.BreakChain);
                attackedEnemy.Add(enemy);
            };
            atk.OnAttackDealDamage += handler;
        }
        
        
        
    }

    public void Skill2_Attack()
    {
        
        var skill2Container = InitContainer(false,1,true);
        var proj = InstantiateRanged(skillFX[2], transform.position,
            skill2Container, ac.facedir);
        _skill2Container = skill2Container;
        
        var atk = proj.GetComponent<AttackFromPlayer>();
        
        atk.OnAttackDealDamage += AddLastHitEnemyStatus;
        
        Action<StatusManager, StatusManager, AttackBase, float> handler = null;
        
        if (acSP._skill2EffectCurrent == ActorController_c012.SkillChainState.None)
        {
            atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Poison,58.2f,
                15,100),120 + 30);
        }else if (acSP._skill2EffectCurrent == ActorController_c012.SkillChainState.NormalChain)
        {
            //普通技能链，中毒特攻
            atk.AddConditionalAttackEffect(skillChainEffect2);
            //追加一个额外hit
            handler = (self, enemy, atkstat, dmg) =>
            {
                InstantiateAnExtraHitForSkill2(enemy, ActorController_c012.SkillChainState.NormalChain,skill2Container);
            };
            
            atk.OnAttackDealDamage += handler;

        }else if (acSP._skill2EffectCurrent == ActorController_c012.SkillChainState.DispelChain)
        {
            //驱散技能链
            atk.AddWithConditionAll(new TimerBuff(999),100);
            //追加一个额外hit
            handler = (self, enemy, atkstat, dmg) =>
            {
                InstantiateAnExtraHitForSkill2(enemy, ActorController_c012.SkillChainState.DispelChain,skill2Container);
            };
            atk.OnAttackDealDamage += handler;

        }else if(acSP._skill2EffectCurrent == ActorController_c012.SkillChainState.BreakChain)
        {
            //破防特效
            atk.attackInfo[0].dmgModifier[0] *= 1.2f;
            atk.AddConditionalAttackEffect(skillChainEffect3);
            //追加一个额外hit
            handler = (self, enemy, atkstat, dmg) =>
            {
                InstantiateAnExtraHitForSkill2(enemy, ActorController_c012.SkillChainState.BreakChain,skill2Container);
            };
            atk.OnAttackDealDamage += handler;
        }
    }

    public override void Skill4()
    {
        if (skillUpgradeInfo[3])
        {
            base.Skill4();
            _forceBuff.dispellable = false;
            _statusManager.ObtainTimerBuff(new TimerBuff(_forceBuff), false);
            _statusManager.ObtainTimerBuff(new TimerBuff(_overdriveAccBuff), false);
        }
        else
        {
            base.Skill4();
        }
    }

    public override GameObject ForceStrikeRelease(int currentFSLV)
    {
        var go = base.ForceStrikeRelease(currentFSLV);

        if (skillUpgradeInfo[3] && _statusManager.HasCondition((int)BasicCalculation.BattleCondition.AlteredStrikeAlex))
        {
            go.GetComponent<AttackFromPlayer>().BeforeAttackHit += ForceStrikeAddExtraEffects;
            
        }

        return go;
    }

    private void AddLastHitEnemyStatus(StatusManager self,
        StatusManager enemy, AttackBase atk, float damage)
    {
        if(_lastHitEnemyStatus.Contains(enemy) == false)
            _lastHitEnemyStatus.Add(enemy);
    }

    private void InstantiateAnExtraHitForSkill1(StatusManager enemy, ActorController_c012.SkillChainState state)
    {

        var proj = 
            InstantiateMeele(skillFX[3], enemy.transform.position, _skill1Container);
        
        var forcedAtk = proj.GetComponent<ForcedAttackFromPlayer>();
        forcedAtk.target = enemy.gameObject;

        switch (state)
        {
            case ActorController_c012.SkillChainState.NormalChain:
            {
                forcedAtk.attackInfo[0].dmgModifier[0] = 4.85f;
                forcedAtk.AddConditionalAttackEffect(skillChainEffect1);
                break;
            }
            case ActorController_c012.SkillChainState.DispelChain:
            {
                forcedAtk.attackInfo[0].dmgModifier[0] = 4.95f;
                forcedAtk.AddConditionalAttackEffect(skillChainEffect1);
                break;
            }
            case ActorController_c012.SkillChainState.BreakChain:
            {
                forcedAtk.attackInfo[0].dmgModifier[0] = 6.68f;
                forcedAtk.AddConditionalAttackEffect(skillChainEffect3);
                break;
            }
            
        }

    }
    
    
    private void InstantiateAnExtraHitForSkill2(StatusManager enemy,
        ActorController_c012.SkillChainState state, GameObject container)
    {

        var proj = 
            InstantiateRanged(skillFX[3], enemy.transform.position, container,1);
        
        var forcedAtk = proj.GetComponent<ForcedAttackFromPlayer>();
        forcedAtk.target = enemy.gameObject;

        switch (state)
        {
            case ActorController_c012.SkillChainState.NormalChain:
            {
                forcedAtk.attackInfo[0].dmgModifier[0] = 4.42f;
                forcedAtk.AddConditionalAttackEffect(skillChainEffect2);
                break;
            }
            case ActorController_c012.SkillChainState.DispelChain:
            {
                forcedAtk.attackInfo[0].dmgModifier[0] = 4.52f;
                forcedAtk.AddConditionalAttackEffect(skillChainEffect2);
                break;
            }
            case ActorController_c012.SkillChainState.BreakChain:
            {
                forcedAtk.attackInfo[0].dmgModifier[0] = 6.58f;
                forcedAtk.AddConditionalAttackEffect(skillChainEffect3);
                break;
            }
            
        }

    }
    
    
    
    private void OnSkillEnter()
    {
        _lastHitEnemyStatus.Clear();

        if (ac.anim.GetCurrentAnimatorStateInfo(0).IsName("s1") ||
            ac.anim.GetCurrentAnimatorStateInfo(0).IsName("s2"))
        {
            if (_skillChainTimer != null)
            {
                _skillChainTimer.Kill();
            }
        
            acSP._skill1EffectCurrent = acSP._skill1EffectNext;
            acSP._skill2EffectCurrent = acSP._skill2EffectNext;
            acSP._skill1EffectNext = ActorController_c012.SkillChainState.None;
            acSP._skill2EffectNext = ActorController_c012.SkillChainState.None;
        }

        
    }

    public void ClearEnemyStatusAndCheckNextChain(int sid)
    {
        foreach (var status in _lastHitEnemyStatus)
        {
            if (status.HasDispellableBuff())
            {
                if (sid == 1)
                {
                    acSP._skill2EffectNext = ActorController_c012.SkillChainState.DispelChain;
                    acSP._skill1EffectNext = ActorController_c012.SkillChainState.NormalChain;
                }else if (sid == 2)
                {
                    acSP._skill1EffectNext = ActorController_c012.SkillChainState.DispelChain;
                    acSP._skill2EffectNext = ActorController_c012.SkillChainState.NormalChain;
                }
                break;
            }
            else if (status is SpecialStatusManager)
            {
                if ((status as SpecialStatusManager).broken)
                {
                    if (sid == 1)
                    {
                        acSP._skill2EffectNext = ActorController_c012.SkillChainState.BreakChain;
                        acSP._skill1EffectNext = ActorController_c012.SkillChainState.NormalChain;
                    }else if (sid == 2)
                    {
                        acSP._skill1EffectNext = ActorController_c012.SkillChainState.BreakChain;
                        acSP._skill2EffectNext = ActorController_c012.SkillChainState.NormalChain;
                    }
                    break;
                }
            }
        }

        if (acSP.currentSP < SpecialSkillGauge_C012.MaxSPPerLevel)
        {
            acSP._skill1EffectNext = acSP._skill2EffectNext = ActorController_c012.SkillChainState.None;
        }
        else
        {
            if(sid == 1)
                acSP._skill1EffectNext = ActorController_c012.SkillChainState.NormalChain;
            else if(sid == 2)
                acSP._skill2EffectNext = ActorController_c012.SkillChainState.NormalChain;
            
            _statusManager.OnSpecialBuffDelegate?.Invoke(
                UI_BuffLogPopManager.SpecialConditionType.SkillChain.ToString());
            _skillChainTimer = DOVirtual.DelayedCall(SkillChainTime,
                () => ResetSkillChain(), false);
        }
    }

    private void ResetSkillChain()
    {
        acSP._skill1EffectNext = acSP._skill2EffectNext = ActorController_c012.SkillChainState.None;
    }

    private void ForceStrikeAddExtraEffects(AttackBase atk, GameObject enemy)
    {
        var ssm = enemy.GetComponent<SpecialStatusManager>();
        if (ssm)
        {
            if (ssm.broken && 
                _statusManager.HasCondition((int)BasicCalculation.BattleCondition.AlteredStrikeAlex))
            {
                _statusManager.RemoveAllConditionWithSpecialID(101203);
                atk.AddWithConditionAll(new TimerBuff(_defDebuff),100);
                atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Poison,58.2f,
                    15,100),120 + 30,1);
                (atk as AttackFromPlayer).SetSpGain(1129);
                atk.BeforeAttackHit -= ForceStrikeAddExtraEffects;
            }
        }
        
            
    }
    
}
