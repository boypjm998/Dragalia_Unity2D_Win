using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

/// <summary>
/// Drastic Force Ball
/// </summary>
public abstract class Projectile_DB013_EnlightmentOrb : MonoBehaviour
{
    private StatusManager _statusManager;
    [SerializeField] protected GameObject protectionFX;
    [SerializeField] protected GameObject attackPrefab1;
    [SerializeField] protected GameObject attackPrefab2;
    [SerializeField] protected GameObject attackPrefab3;

    
    protected GameObject target;
    protected Tween _tween;

    public Transform enemySource;

    private void Awake()
    {
        _statusManager = GetComponent<StatusManager>();
        _statusManager.OnReviveOrDeath += GrantDrasticForceWhenKilled;
        _statusManager.SpecialDamageCutEffectFunc += Ability.DrasticForceEffect;
    }

    private void GrantDrasticForceWhenKilled()
    {
        _statusManager.OnReviveOrDeath -= GrantDrasticForceWhenKilled;
        DrasticForce.Instance?.AddDrasticForce();
        _tween?.Kill();
    }

    public void SetProtectionFXOn()
    {
        protectionFX.SetActive(true);
        
        var debuff = new TimerBuff((int)BasicCalculation.BattleCondition.DashAttackVunerable,
            500,-1,1,0);
        debuff.extra_iconID = (int)BasicCalculation.BattleCondition.Vulnerable;


        DOVirtual.DelayedCall(0.5f, () =>
        {
            GetComponent<StatusManager>().ObtainTimerBuff(debuff);
            print("GrantDebuff");
        }, false);
        
    }

    private void OnDestroy()
    {
        _statusManager.SpecialDamageCutEffectFunc -= Ability.DrasticForceEffect;
        if (_statusManager.currentHp > 0)
        {
            _statusManager.OnReviveOrDeath -= GrantDrasticForceWhenKilled;
        }
        _tween?.Kill();
    }
    
    protected virtual void Hint(){}
    protected virtual void Attack(){}
}
