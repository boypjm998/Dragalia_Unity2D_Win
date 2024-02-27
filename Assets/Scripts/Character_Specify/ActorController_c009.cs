using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.Pool;

public class ActorController_c009 : ActorControllerRangedWithFS
{
    [SerializeField]
    private GameObject specialReflectionDamageFX;

    private ObjectPool<GameObject> reflectionFXPool;
    private void Start()
    {
        canPerformInAir[3] = false;
        reflectionFXPool = new ObjectPool<GameObject>(() => Instantiate(specialReflectionDamageFX,
            BattleStageManager.Instance.RangedAttackFXLayer.transform));
        _statusManager.OnTakeDirectDamageFrom += ReflectionDamage;
    }

    protected override void Update()
    {
        base.Update();
        CheckForceStrike();
    }

    private void ReflectionDamage(StatusManager myStat, StatusManager targetStat, AttackBase atk, float dmg)
    {
        if(targetStat.GetAttackBuff(2) <= 0)
            return;
        
        var atkBuff = _statusManager.attackBuff + 
                      AbilityCalculation.GetAbilityAmountInfo(myStat,targetStat,atk,AbilityCalculation.ProductArea.ATK).Item1;

        var modifier = atk.attackInfo[0].dmgModifier.Sum();

        var damage = (1+atkBuff) * myStat.baseAtk * 0.3f * modifier;

        BattleStageManager.Instance.CauseIndirectDamage(targetStat, 
            (int)Mathf.Ceil(damage), false, false);
        reflectionFXPool.Get().transform.position = targetStat.transform.position;
        
        targetStat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefDebuff, 5, -1,
            1, 100905);
    }

    protected override void CheckForceStrike()
    {
        if(_statusManager.GetConditionStackNumber((int)BasicCalculation.BattleCondition.AlteredStrikeCleo)<=0)
            return;
        base.CheckForceStrike();
    }

    public void Skill_FloatInfoAir()
    {
        _tweener = rigid.DOMoveY(transform.position.y + 4f, 0.4f).SetEase(Ease.OutSine);
        SetGravityScale(0);
    }
    
    public void Skill_LandToGround()
    {
        _tweener = rigid.DOMoveY(gameObject.RaycastedPosition().y + 1.3f, 0.7f).
            SetEase(Ease.InOutSine).OnKill(ResetGravityScale).OnComplete(ResetGravityScale);
        
    }
    
}
