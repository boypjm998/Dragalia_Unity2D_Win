using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = System.Random;

public class AttackManager_C019_PL : AttackManager
{
    [SerializeField] private GameObject[] comboFX;
    [SerializeField] private GameObject[] skillFX;
    [SerializeField] private GameObject[] dashFX;
    private TargetAimer ta;
    private Vector2 combo4Pos;
    private TimerBuff _critDmgBuff;

    protected override void Awake()
    {
        base.Awake();
        ta = GetComponentInChildren<TargetAimer>();
        _critDmgBuff = new TimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff,
            10, 90, 3, 101901);
        _critDmgBuff.dispellable = false;
    }

    public void Combo1()
    {
        var proj = InstantiateRanged(comboFX[0],
            transform.position, InitContainer(false),
            ac.facedir);

    }

    private Vector2 GetRangedTargetPosition(float x,float y)
    {
        var targetTrans = ta.GetNearestTargetInRangeDirection(ac.facedir, x, y,
            LayerMask.GetMask("Enemies"));

        Vector2 targetPosition = new Vector2(transform.position.x + ac.facedir * 4,
            gameObject.RaycastedPosition().y);

        if (targetTrans != null)
        {
            var min = Mathf.Min(transform.position.x + ac.facedir * 4,
                transform.position.x + ac.facedir * 10);
            var max = Mathf.Max(transform.position.x + ac.facedir * 4,
                transform.position.x + ac.facedir * 10);
            
            targetPosition.x = Mathf.Clamp(targetTrans.position.x, min, max);
            targetPosition.y = targetTrans.gameObject.RaycastedPosition().y;
        }

        return targetPosition;
    }

    private Transform GetRangedTarget(float x, float y)
    {
        var targetTrans = ta.GetNearestTargetInRangeDirection(ac.facedir, x, y,
            LayerMask.GetMask("Enemies"));
        return targetTrans;
    }
    public void Combo2()
    {
        var proj = InstantiateRanged(comboFX[1],
           GetRangedTargetPosition(14.5f,3), InitContainer(false),
            ac.facedir);

    }
    
    public void Combo3()
    {
        var proj = InstantiateRanged(comboFX[2],
            GetRangedTargetPosition(14.5f,3), InitContainer(false),
            ac.facedir);

    }
    
    public void Combo4()
    {
        combo4Pos = GetRangedTargetPosition(14.5f,3);
        var proj = InstantiateRanged(comboFX[3],
            combo4Pos, InitContainer(false),
            1);

    }
    
    public void Combo5()
    {
        (ac as ActorController).SetWeaponVisibility(false);
        combo4Pos = GetRangedTargetPosition(14.5f,3);
        var proj = InstantiateRanged(comboFX[4],
            combo4Pos + new Vector2(0,-1), InitContainer(false),
           1);
    }

    public void Combo5_ShineBall()
    {
        
        var proj = Instantiate(comboFX[5], transform.position + new Vector3(ac.facedir, 0.5f),
            Quaternion.identity,RangedAttackFXLayer.transform);

        ActorBase.OnHurt onHurt = null;
        onHurt = () =>
        {
            ac.OnAttackInterrupt -= onHurt;
            Destroy(proj);
        };

        ac.OnAttackInterrupt += onHurt;

    }

    public override void DashAttack()
    {
        var container =
            Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(dashFX[0],transform.position,container);
    }

    public void HideWeapon()
    {
        (ac as ActorController).SetWeaponVisibility(false);
    }

    public void DisplayWeapon()
    {
        (ac as ActorController).SetWeaponVisibility(true);
    }

    public void Skill1_Muzzle()
    {
        var projfx = Instantiate(skillFX[0],
            transform.position + new Vector3(ac.facedir * 1.2f, 0),
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var target = GetRangedTarget(20, 10);

        DOVirtual.DelayedCall(1.6f, () =>
        {
            Vector2 targetPos;
            if (target != null)
            {
                targetPos = target.gameObject.RaycastedPosition();
            }
            else
            {
                targetPos = GetRangedTargetPosition(20, 10);
            }
            Skill1_Attack(targetPos);

        }, false);


    }
    
    public void Skill5_Muzzle()
    {
        var projfx = Instantiate(skillFX[2],
            transform.position + new Vector3(ac.facedir * 1.2f, 0),
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var target = GetRangedTarget(20, 10);

        DOVirtual.DelayedCall(1.5f, () =>
        {
            Vector2 targetPos;
            if (target != null)
            {
                targetPos = target.gameObject.RaycastedPosition();
            }
            else
            {
                targetPos = GetRangedTargetPosition(20, 10);
            }
            Skill1_Attack(targetPos,true);

        }, false);


    }

    public void Skill1_Attack(Vector2 position, bool boosted = false)
    {
        var prefab = boosted ? skillFX[3] : skillFX[1];
        
        var atk =
            InstantiateRanged(prefab, position, InitContainer(false,1,true), 1).
                GetComponent<AttackFromPlayer>();
        
        var flashBurn = new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
            41.6f, 21, 100);

        atk.AddWithConditionAll(flashBurn, 120);

        if (boosted)
        {
            atk.AddWithConditionAll(new TimerBuff(999),100,2);
        }
        
    }
    
    public void Skill2()
    {

        var bondBuff = new TimerBuff((int)BasicCalculation.BattleCondition.PowerOfBonds,
            -1, -1, 1, -1);

        _statusManager.ObtainTimerBuff(bondBuff); 
        _statusManager.ObtainTimerBuff(new TimerBuff(_critDmgBuff));

        var fx = Instantiate(skillFX[5],
            gameObject.RaycastedPosition(),
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        //(_statusManager as PlayerStatusManager).FillSP(1,100);
        
    }

    public void Skill2_HealEffect()
    {
        Instantiate(skillFX[7],
            gameObject.RaycastedPosition(),
            Quaternion.identity, RangedAttackFXLayer.transform);
    }
    
    public void Skill2_Heal()
    {

        var pob = _statusManager.
            GetConditionsOfType((int)BasicCalculation.BattleCondition.PowerOfBonds);

        if (pob.Count > 0)
        {
            _statusManager.RemoveConditionWithLog(pob[0]);
        }
        
        _statusManager.ObtainTimerBuff(new TimerBuff(_critDmgBuff));
        _statusManager.HPRegenImmediately(100,0,false);
        (ac as ActorController_c019).RecoverHealTimes();

        var fx = Instantiate(skillFX[6],
            transform.position + new Vector3(0,4.5f,0),
            Quaternion.identity, RangedAttackFXLayer.transform);
        
    }
    
    public void Skill2_Effect()
    {
        HideWeapon();
        var fx = Instantiate(skillFX[4],
            transform.position,
            Quaternion.identity, MeeleAttackFXLayer.transform);

    }

    public void Skill3_Float()
    {
        ac.SetGravityScale(0);
        ac.rigid.velocity = Vector2.zero;
        
    }

    public void Skill3_FloatPurge()
    {
        ac.ResetGravityScale();
    }

    

    public void Skill3()
    {
        var atk = InstantiateRanged(skillFX[8],
            transform.position, InitContainer(false, 1, true),
            1).GetComponent<AttackFromPlayer>();

        var retainer = atk.gameObject.AddComponent<RelativePositionRetainer>();
        retainer.SetParent(transform);

        Func<StatusManager, StatusManager, bool> conditionalFunc = (src, tar) =>
        {
            var distance = Vector2.Distance((Vector2)src.transform.position,
                (Vector2)tar.transform.position);

            if (distance <= 15)
                return true;
            return false;
        };
        
        Func<StatusManager, StatusManager, bool> conditionalFunc2 = (src, tar) =>
        {
            var distance = Vector2.Distance((Vector2)src.transform.position,
                (Vector2)tar.transform.position);

            if (distance <= 7)
                return true;
            return false;
        };

        ConditionalAttackEffect caf = new ConditionalAttackEffect(conditionalFunc,
            ConditionalAttackEffect.ExtraEffect.ExtraCritRate,new string[] {},
            new string[] {"30"});
        ConditionalAttackEffect caf2 = new ConditionalAttackEffect(conditionalFunc2,
            ConditionalAttackEffect.ExtraEffect.ExtraCritRate,new string[] {},
            new string[] {"40"});
        
        atk.AddConditionalAttackEffect(caf);
        atk.AddConditionalAttackEffect(caf2);

        // Func<StatusManager, StatusManager, bool> stunConditionalFunc = (src, tar) =>
        // {
        //     var tarAc = tar.GetComponent<ActorBase>();
        //
        //     if (tarAc.facedir == 1 && tar.transform.position.x <= src.transform.position.x ||
        //         tarAc.facedir == -1 && tar.transform.position.x >= src.transform.position.x)
        //     {
        //         return true;
        //     }
        //     else return false;
        //     
        // };

        ConditionalAttackEffect caf3 =
            new ConditionalAttackEffect(
                ConditionalAttackEffect.ConditionType.Custom,
                ConditionalAttackEffect.ExtraEffect.Custom,
                new string[] { },
                new string[] { }).
                SetEffectFunction((stats, atkStat) =>
                {
                    if ((atkStat as AttackFromPlayer).attackId > 0)
                        return 0;

                    BattleStageManager.Instance.ObtainAfflictionDirectlyWithCheck(
                        stats.targetStat, new TimerBuff((int)BasicCalculation.BattleCondition.Stun,
                            1, UnityEngine.Random.Range(4f, 7f),
                            1, -1), 200, 0,_statusManager);

                    return 0;
                }).
                SetConidtionalFunction((src, tar) =>
                {
                    var tarAc = tar.GetComponent<ActorBase>();

                    if (tarAc.facedir == 1 && tar.transform.position.x <= src.transform.position.x ||
                        tarAc.facedir == -1 && tar.transform.position.x >= src.transform.position.x)
                    {
                        return true;
                    }
                    else return false;
                }
            );
        
        atk.AddConditionalAttackEffect(caf3);
        
    }
    
    public void Skill4()
    {
        var fx = Instantiate(skillFX[9],
            transform.position + new Vector3(0,4.5f,0),
            Quaternion.identity, RangedAttackFXLayer.transform);

        var currentBuffAmount = _statusManager.GetConditionTotalValue((int)BasicCalculation.BattleCondition.MaxHPBuff);

        var remainBuff = Mathf.Min(30 - currentBuffAmount, 15);
        
        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.AtkBuff), 15,
            60,1,101902);

        if (remainBuff > 0)
        {
            _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.MaxHPBuff), remainBuff,
                -1);
            _statusManager.GetMaxHP();
        }
        else
        {
            _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.CritRateBuff), 13,
                30);
        }
        _statusManager.HPRegenImmediatelyWithoutRandomDirectly(_statusManager,
            (Mathf.CeilToInt(_statusManager.maxHP * 0.15f)));


    }
    
    
}
