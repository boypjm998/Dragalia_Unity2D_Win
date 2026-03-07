using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C051 : AttackManagerMeeleWithFS
{
    private TargetAimer ta;
    private TimerBuff _soulSealBuff = new((int)BasicCalculation.BattleCondition.SoulSeal,
        1, -1, 4);
    private TimerBuff _atkDebuff = new((int)BasicCalculation.BattleCondition.AtkDebuff,
        10, 10,1,105101);
    private TimerBuff _specialScorchrend = 
        new((int)BasicCalculation.BattleCondition.Scorchrend, 1, 21, 1, 105103);
    private TimerBuff _selfScorchrend = 
        new((int)BasicCalculation.BattleCondition.Scorchrend, 100, 21, 1, 105102);

    private ConditionalAttackEffect _crisisCAF;
    private ConditionalAttackEffect _breakPunisherCAF;
    private AbilityClock _clock = new(5);
    private GameObject _fireShieldInstance;
    
    private int _attackConnectedEnemieCount = 0;
    private Tween _soulSealFXTween;

    protected override void Start()
    {
        base.Start();
        ta = GetComponentInChildren<TargetAimer>();
        (ac as ActorController).SkillConditionCheck = CheckSkill;
        (ac as ActorController).SetSkillAirPerformProperty(4,false);
        _statusManager.OnBuffEventDelegate += HealDoubleBuff;
        _statusManager.OnTakeDirectDamageFrom += AfflictionReflection;
        _selfScorchrend.SetEffect(_statusManager.maxHP * 0.02f);
        _crisisCAF = new ConditionalAttackEffect(1.5f, 0.5f, 1, 0.4f);
        _breakPunisherCAF = new ConditionalAttackEffect((src, tar) =>
        {
            if (tar is SpecialStatusManager)
            {
                if ((tar as SpecialStatusManager).broken)
                {
                    return true;
                }
            }

            return false;
        }, ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
            new string[] {},new string[] {"0.5"});
    }

    public void Skill1()
    {
        HpDeplete();
        
        var proj = InstantiateMeele(skillFX[0], transform.position + new Vector3(ac.facedir, 1),
            InitContainer(true, 1, true));
        
        var atk = proj.GetComponent<AttackFromPlayer>();

        _statusManager.HPRegenImmediately(0, 5, true);
        
        atk.AddWithConditionAll(_atkDebuff,100);

        _attackConnectedEnemieCount = 0;
        atk.BeforeAttackHit += GrantSoulSeal;
        _soulSealFXTween = DOVirtual.DelayedCall(1.1f, () =>
        {
            if(atk == null)
                return;

            atk.SetMeeleProperty(false);
            
            var ps = atk.GetComponent<ParticleSystem>();
            var em = ps.emission;
            em.SetBurst(0, new ParticleSystem.Burst(0, _attackConnectedEnemieCount));

            if (_attackConnectedEnemieCount == 0)
            {
                atk.transform.Find("flash").gameObject.SetActive(false);
            }

        },false);

    }
    
    public void Skill2_FlameShield()
    {
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff, 60, 5);
        var specialDefBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            20, 15, 100);
        specialDefBuff.dispellable = false;
        
        
        _statusManager.ObtainTimerBuff(specialDefBuff);

        if (_statusManager.GetConditionsOfType((int)BasicCalculation.BattleCondition.Scorchrend).Count > 0)
        {
            var scorchrend = _statusManager.GetConditionsOfType((int)BasicCalculation.BattleCondition.Scorchrend)[0];
            if(scorchrend.specialID != _selfScorchrend.specialID)
                _statusManager.RemoveConditionWithoutLog(scorchrend);
        }
        

        if (_statusManager.GetExactConditionsOfType(
                (int)BasicCalculation.BattleCondition.Scorchrend, 105102).Count <= 0)
        {
            var selfScor = new TimerBuff(_selfScorchrend);
            selfScor.dispellable = false;
            selfScor.OnBuffStart = GrantImmunityToScorchrend;
            selfScor.OnBuffRemove = RemoveImmunityToScorchrend;
            selfScor.dispellable = false;
            _statusManager.ObtainTimerBuff(selfScor, false, false);
        }
        else
        {
            _statusManager.GetExactConditionsOfType(
                    (int)BasicCalculation.BattleCondition.Scorchrend, 105102)[0].lastTime
                = _selfScorchrend.duration;
            _statusManager.OnBuffEventDelegate?.Invoke(new TimerBuff(_selfScorchrend));
        }
        
        InstantiateBuff(skillFX[4], transform.position);
        
        (_statusManager as PlayerStatusManager).FillSP(0,10);
        _statusManager.OnSpecialBuffDelegate?.
            Invoke(UI_BuffLogPopManager.SpecialConditionType.SPCharge.ToString());
        
        
    }

    private void AfflictionReflection(StatusManager src, StatusManager tar, AttackBase atk, float dmg)
    {
        
        
        if (!src.HasBuffWithSPID(105102))
            return;

        //敌人持有特殊的劫火时，被攻击回复生命值。
        if (tar.HasBuffWithSPID(105103) && _clock.Available)
        {
            _clock.StartTick();
            src.HPRegenImmediatelyWithoutRandom(0, 3);
        }
        
        //如果目标是自身
        if(tar == _statusManager)
            return;
        

        //敌人持有特殊的劫火时，不再叠加。
        if (tar.HasBuffWithSPID(105103))
            return;


        var scorchrend = new TimerBuff(_specialScorchrend);
        var defEffect = 
            AbilityCalculation.GetAbilityAmountInfo(tar, src, atk, AbilityCalculation.ProductArea.DEF);
        
        var totalEffect = scorchrend.effect * (1.5f+defEffect.buffPart+_statusManager.GetDefenseBuff(1));
        
        scorchrend.SetEffect(totalEffect * src.baseAtk);
        print("防御增益:"+totalEffect);

        BattleStageManager.Instance.ObtainAfflictionDirectlyWithCheck(tar, scorchrend, 200, 1, null);

    }
    
    private void GrantImmunityToScorchrend(StatusManager stat)
    {
        stat.ScorchrendRes = 999;

        if (_fireShieldInstance == null)
        {
            _fireShieldInstance = InstantiateBuff(skillFX[5],
                transform.position - new Vector3(0,1.4f));
        }else
        {
            _fireShieldInstance.SetActive(true);
        }
        
        
    }
    
    private void RemoveImmunityToScorchrend(StatusManager stat)
    {
        stat.ScorchrendRes = 0;
        if(_fireShieldInstance != null)
            _fireShieldInstance.SetActive(false);
    }

    public void Skill3_Muzzle()
    {
        var starCount = _statusManager.GetConditionStackNumber(_soulSealBuff.buffID);
        
        for(int i = 0; i < starCount; i++)
        {
            _statusManager.ObtainTimerBuff(
                new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff, 15, 15, 100, -1),
                i == 0);
        }

        _statusManager.RemoveAllConditionOfType(_soulSealBuff.buffID);
        
        var fx = Instantiate(skillFX[2], transform.position, Quaternion.identity,RangedAttackFXLayer.transform);
        
        var ps = fx.GetComponent<ParticleSystem>();
        
        var em = ps.emission;
        em.SetBurst(0, new ParticleSystem.Burst(0,
            (short)starCount, (short)starCount, 1, 0.01f));
        
        
        ps.Play();
        print(ps.emission.burstCount);
        
        
        DOVirtual.DelayedCall(.5f,() =>
        {
            Skill3_Attack(starCount);
        },false);

    }
    
    private void Skill3_Attack(int starCount)
    {
        
        
        var enemiesInRange = ta.GetAllEnemiesWithMarkingCheck();
        enemiesInRange.Sort((a, b) =>
        {
            var distance1 = Vector2.Distance(a.position, transform.position);
            var distance2 = Vector2.Distance(b.position, transform.position);

            if (distance1 > distance2)
                return 1;
            else if (distance1 < distance2)
                return -1;
            else return 0;

        });

        var targetList = enemiesInRange.GetMultipleTargetDistributionList(starCount);
        
        //print(targetList.Count);

        //List<Tween> tweens = new();
        float interval = .25f;

        var container = InitContainer(false,starCount,true);

        for (int i = 0; i < starCount; i++)
        {
            var index = i;
            DOVirtual.DelayedCall(interval * (index + 1),() =>
            {
                if (targetList == null)
                {
                    var proj = InstantiateRanged(skillFX[1], 
                        transform.position, container, 1);
                    
                    proj.GetComponent<ForcedAttackFromPlayer>().AddConditionalAttackEffect(_crisisCAF);
                    
                    return;
                }
                
                var target = targetList[index];
                
                
                if (target != null)
                {
                    var proj = InstantiateRanged(skillFX[1], target.transform.position, container, 1);
                    proj.GetComponent<ForcedAttackFromPlayer>().target = target;
                    proj.GetComponent<ForcedAttackFromPlayer>().AddConditionalAttackEffect(_crisisCAF);
                }
                else
                {
                    var proj = InstantiateRanged(skillFX[1], transform.position, container, 1);
                    proj.GetComponent<ForcedAttackFromPlayer>().AddConditionalAttackEffect(_crisisCAF);
                }
                
                
                
                
            },false);
        }

        

    }

    public void Skill4_Shield()
    {
        _statusManager.ReliefOneDebuff();
        
        Instantiate(skillFX[3],transform.position - new Vector3(0,1.6f),Quaternion.identity);

        if (_statusManager.currentHp <= _statusManager.maxHP * 0.4f)
        {
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff, 20, 15);
            return;
        }
        else
        {
            int dmg = (int)(_statusManager.currentHp - _statusManager.maxHP * 0.3f);
            
            //dmg = Mathf.Clamp(dmg, 1, _statusManager.maxHP);
            print(dmg);
            BattleStageManager.Instance.CauseIndirectDamage(_statusManager, dmg, false, false,true);
            _statusManager.AddLifeShield(_statusManager.maxHP,dmg);
        }

    }
    
    
    

    private bool CheckSkill(int sid)
    {
        if (sid == 2 && _statusManager.HasCondition(_soulSealBuff.buffID) == false)
        {
            return false;
        }

        return true;
        
    }
    private void GrantSoulSeal(AttackBase @base, GameObject target)
    {
        
        Instantiate(skillFX[6], target.transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);
        _attackConnectedEnemieCount++;
        
        if (_statusManager.GetConditionStackNumber(_soulSealBuff.buffID) >= 4)
        {
            return;
        }

        _statusManager.ObtainTimerBuff(new TimerBuff(_soulSealBuff));
    }
    
    private void HpDeplete()
    {
        if (_statusManager.GetConditionStackNumber(_soulSealBuff.buffID) >= 4)
        {
            BattleStageManager.Instance.CauseIndirectDamage(
                _statusManager, (int)(_statusManager.maxHP * 0.02f), false, false);
        }
    }
    
    private void HealDoubleBuff(BattleCondition condition)
    {
        if (condition.buffID == (int)BasicCalculation.BattleCondition.DefBuff)
        {
            _statusManager.ObtainHealOverTimeBuff(5,25);
        }
    }

    protected override void OnStandardAttackEnter()
    {
        base.OnStandardAttackEnter();
        HpDeplete();
    }

    public override void DashAttack()
    {
        base.DashAttack();
        HpDeplete();
    }

    public override GameObject ForceStrikeRelease(int currentFSLV)
    {
        HpDeplete();
        var proj = base.ForceStrikeRelease(currentFSLV);
        
        if(_statusManager.HasBuffWithSPID(_selfScorchrend.specialID))
        {
            proj.GetComponent<AttackFromPlayer>().AddWithConditionAll(new TimerBuff(999),100);
        }
        
        return proj;
    }
    


}


