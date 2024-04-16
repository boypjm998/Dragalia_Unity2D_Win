using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C017 : AttackManagerRanged
{
    private ConditionalAttackEffect crisisCaf;
    private ConditionalAttackEffect poisonCaf;
    private ConditionalAttackEffect poisonCaf2;
    private GameObject _shieldFXInstance = null;
    

    private TimerBuff _defenseBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
        25, 20, 1, 101701);
    private TimerBuff _atkBuff = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
        25, 20, 1, 101704);
    private TimerBuff doubleBuff = 
        new((int)BasicCalculation.BattleCondition.AtkBuff, 15, 15, 5, 101703);

    private AbilityClock _abilityClock = new(5);

    protected override void Awake()
    {
        base.Awake();
        
        var checkConditionString = ((int)BasicCalculation.BattleCondition.Poison).ToString();
        poisonCaf = new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
            ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
            new string[] { "1", checkConditionString },
            new string[] { "0.5" });
        poisonCaf2 = new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
            ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
            new string[] { "1", checkConditionString },
            new string[] { "0.3" });
        
        
        crisisCaf = new ConditionalAttackEffect(0.5f);
        _defenseBuff.dispellable = false;
        _atkBuff.dispellable = false;

    }

    protected override void Start()
    {
        base.Start();
        _statusManager.OnBuffEventDelegate += CheckDoublebuff;
        _statusManager.OnAfflictionInflict += CheckAfflictionInflict;
    }

    private void CheckDoublebuff(BattleCondition condition)
    {
        if (condition.buffID == (int)(BasicCalculation.BattleCondition.DefBuff))
        {
            _statusManager.ObtainTimerBuff(new TimerBuff(doubleBuff));
        }
    }

    private void CheckAfflictionInflict(BattleCondition condition)
    {
        if(_abilityClock.Available == false)
            return;

        if (condition.buffID == (int)BasicCalculation.BattleCondition.Poison)
        {
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
                8, 15);
            _abilityClock.StartTick();
        }
        
        
        
    }

    private void OpenShieldFX(StatusManager statusManager)
    {
        print("Open Shield FX");
        if (_shieldFXInstance == null)
        {
            var fx = Instantiate(skill3FX[0], 
                transform.position ,Quaternion.identity, RangedAttackFXLayer.transform);
            fx.AddComponent<RelativePositionRetainer>().SetParent(transform);
            _shieldFXInstance = fx;
        }
        else
        {
            _shieldFXInstance.SetActive(true);
        }
        
    }
    
    private void CloseShieldFX(StatusManager statusManager)
    {
        if (_shieldFXInstance != null)
        {
            _shieldFXInstance.SetActive(false);
        }
    }

    public void Skill1_Muzzle()
    {
        InstantiateBuff(skill1FX[0], transform.position);
    }

    public void Skill1_Attack()
    {
        var proj = InstantiateRanged(skill1FX[1], transform.position + new Vector3(ac.facedir,0),
            InitContainer(false,1,true),ac.facedir);
        
        var atk = proj.GetComponent<AttackFromPlayer>();
        
        atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Poison,
            58.2f,15,100),120);
        atk.AddConditionalAttackEffect(poisonCaf2);
        
        int maxCap = (int)(_statusManager.maxHP * 0.1f);
        int currentHealed = 0;
        atk.OnAttackDealDamage += (statusManagerSelf, statusManagerTarget, attack, dmg) =>
        {
            if(currentHealed >= maxCap) return;
            var clamp = Mathf.Abs(maxCap - currentHealed);
            currentHealed += LifeStealWithReturn(statusManagerSelf,
                (int)dmg,5,10,clamp);
        };
            
    }
    
    public void Skill2_Muzzle()
    {
        InstantiateBuff(skill2FX[0], transform.position + new Vector3(0,1.2f));
        DOVirtual.DelayedCall(0.7f, Skill2_Attack,false);
    }
    
    private void Skill2_Attack()
    {
        var proj = InstantiateRanged(skill2FX[1], gameObject.RaycastedPosition()
            + new Vector2(ac.facedir * 5,0),
            InitContainer(false,1,true),1);
        
        var atk = proj.GetComponent<AttackFromPlayer>();
        
        atk.AddConditionalAttackEffect(crisisCaf);
        atk.AddConditionalAttackEffect(poisonCaf);

    }

    public void Skill3_Muzzle()
    {
        InstantiateBuff(skill3FX[1], transform.position);
    }

    public void Skill3_Shield()
    {
        _statusManager.AddLifeShield((int)(_statusManager.maxHP),(int)(_statusManager.maxHP * 0.1f));

        var buff = new TimerBuff(_defenseBuff);
        buff.OnBuffStart += OpenShieldFX;
        buff.OnBuffRemove += CloseShieldFX;
        (_statusManager as PlayerStatusManager).FillSP(1,50);
        (_statusManager as PlayerStatusManager).FillSP(0,50);
        _statusManager.OnSpecialBuffDelegate?.Invoke("SPCharge");
        _statusManager.ObtainTimerBuff(buff,false);
        _statusManager.ObtainTimerBuff(new TimerBuff(_atkBuff), false);
        (_statusManager as PlayerStatusManager).SpeedUp(10,20,false);
    }

    public override void ForceStrikeRelease(int forcelevel = 0)
    {
        if(forcelevel <= 0)
            return;
        
        if (weaponType == BasicCalculation.RangedWeaponType.Bow)
        {

            var container = InitContainer(false);
            
        
            var atk = InstantiateDirectionalRanged(ForceFX[1],
                transform.position + new Vector3(ac.facedir *1f,0),
                container,ac.facedir,0);

            if (_shieldFXInstance != null)
            {
                if (_shieldFXInstance.activeSelf)
                {
                    atk.GetComponent<AttackFromPlayer>().AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.PoisonResDown,
                        20, 15, 1, 101702),100);
                }
            }
            
            (ac as ActorController)?.PlayAttackVoice(9);
        }
    }
}
