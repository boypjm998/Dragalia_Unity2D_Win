using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class Projectile_H003_1 : Projectile_DB013_EnlightmentOrb
{
    public enum AttackType
    {
        None,
        Round,
        Projectiles,
        SingleProjectile
    }
    
    public AttackType attackType { get; private set; }
    private float firstInterval = 1;
    private float interval = 1;
    private float lifeTime = 15;
    private float warningTime = 4;
    private bool avoidable = false;
    private bool _initFinished = false;
    
    private GameObject hint1;
    private GameObject hint2;
    private GameObject hint3;

    public bool IsNegative { get; set; } = false;
    
    public void InitOrb(Transform enemySource, AttackType type,bool avoidable,float interval, float lifeTime = 15,
        float firstInterval = 1,float warningTime = 4)
    {
        this.avoidable = avoidable;
        this.interval = interval;
        this.firstInterval = firstInterval;
        this.lifeTime = lifeTime;
        attackType = type;
        this.enemySource = enemySource;
        this.warningTime = warningTime;
        
        _initFinished = true;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => _initFinished);
        
        Destroy(gameObject, lifeTime);
        float currentTime = firstInterval;
        _statusManager.OnReviveOrDeath += () =>
        {
            if (hint1 != null)
                Destroy(hint1);
            if (hint2 != null)
                Destroy(hint2);
            if (hint3 != null)
                Destroy(hint3);
            StopAllCoroutines();
        };

        yield return new WaitForSeconds(firstInterval);

        while (currentTime + warningTime < lifeTime)
        {
            if(attackType == AttackType.Round)
            {
                hint1 = EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(enemySource.GetComponent<ActorBase>(),
                    transform.position, BattleStageManager.Instance.RangedAttackFXLayer.transform, 8, Vector2.zero,
                    avoidable, true, warningTime, 0.15f, 0.4f, true, false,
                    true);
                
                yield return new WaitForSeconds(warningTime);
                
                var fx = this.InstantiateDirectionalRangedObject(attackPrefab1,
                    transform.position,
                    ActorExtensions.InstantiateContainer(BattleStageManager.Instance.RangedAttackFXLayer.transform)
                        .gameObject,
                    1, 0, enemySource);

                if (!avoidable)
                {
                    fx.GetComponent<AttackFromEnemy>().ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
                }
                
            }
            else if(attackType == AttackType.Projectiles)
            {
                target = BattleStageManager.Instance.GetPlayer();
                var angle = 
                    Vector2.Angle(target.transform.position-transform.position,Vector2.right);
                if (transform.position.y > target.transform.position.y)
                {
                    angle *= -1;
                }
                hint1 = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(GetComponent<EnemyController>(), transform.position,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform,
                    new Vector2(30, 2), Vector2.zero,
                    avoidable, 1,
                    warningTime, angle, 1f, true, true);
                hint2 = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(GetComponent<EnemyController>(), transform.position,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform,
                    new Vector2(30, 2), Vector2.zero,
                    avoidable, 1,
                    warningTime, angle + 15, 1f, true, true);
                hint3 = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(GetComponent<EnemyController>(), transform.position,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform,
                    new Vector2(30, 2), Vector2.zero,
                    avoidable, 1,
                    warningTime, angle - 15, 1f, true, true);
                
                
                yield return new WaitForSeconds(warningTime);
                
                var fx1 = this.InstantiateDirectionalRangedObject(attackPrefab2,
                    transform.position,
                    ActorExtensions.InstantiateContainer(BattleStageManager.Instance.RangedAttackFXLayer.transform)
                        .gameObject,
                    1, angle, enemySource);
                
                var fx2 = this.InstantiateDirectionalRangedObject(attackPrefab2,
                    transform.position,
                    ActorExtensions.InstantiateContainer(BattleStageManager.Instance.RangedAttackFXLayer.transform)
                        .gameObject,
                    1, angle + 15, enemySource);
                var fx3 = this.InstantiateDirectionalRangedObject(attackPrefab2,
                    transform.position,
                    ActorExtensions.InstantiateContainer(BattleStageManager.Instance.RangedAttackFXLayer.transform)
                        .gameObject,
                    1, angle - 15, enemySource);

                if (!avoidable)
                {
                    fx1.GetComponent<AttackFromEnemy>().ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
                    fx2.GetComponent<AttackFromEnemy>().ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
                    fx3.GetComponent<AttackFromEnemy>().ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
                }
                
            }
            else if(attackType == AttackType.SingleProjectile)
            {
                target = BattleStageManager.Instance.GetPlayer();
                var angle = 
                    Vector2.Angle(target.transform.position-transform.position,Vector2.right);
                if (transform.position.y > target.transform.position.y)
                {
                    angle *= -1;
                }
                hint1 = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(GetComponent<EnemyController>(), transform.position,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform,
                    new Vector2(30, 2), Vector2.zero,
                    avoidable, 1,
                    warningTime, angle, 1f, true, true);
                
                yield return new WaitForSeconds(warningTime);
                
                var fx = this.InstantiateDirectionalRangedObject(attackPrefab2,
                    transform.position,
                    ActorExtensions.InstantiateContainer(BattleStageManager.Instance.RangedAttackFXLayer.transform)
                        .gameObject,
                    1, angle, enemySource);

                if (!avoidable)
                {
                    fx.GetComponent<AttackFromEnemy>().ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Purple);
                }
            }
            else
            {
                //None, do nothing
            }

            yield return new WaitForSeconds(interval);
            currentTime += interval;
            currentTime += warningTime;

        }
        
        
    }

    

    private void OnDestroy()
    {

        _statusManager.RemoveEffectFunc(Ability.DrasticForceEffect, AbilityCalculation.ProductArea.DMGCUT);
        //_statusManager.SpecialDamageCutEffectFunc -= Ability.DrasticForceEffect;
        if (_statusManager.currentHp > 0)
        {
            _statusManager.OnReviveOrDeath -= GrantDrasticForceWhenKilled;
        }
        _tween?.Kill();
        
        if(hint1 != null)
            Destroy(hint1);
        if(hint2 != null)
            Destroy(hint2);
        if(hint3 != null)
            Destroy(hint3);
        
    }

    protected override void GrantDrasticForceWhenKilled()
    {
        if (IsNegative)
        {
            _statusManager.OnReviveOrDeath -= GrantDrasticForceWhenKilled;
            DrasticForce.Instance?.RemoveOneDrasticForce();
            _tween?.Kill();
        }
        else
        {
            base.GrantDrasticForceWhenKilled();
        }
    }
}
