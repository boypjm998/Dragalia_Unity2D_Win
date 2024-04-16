using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class HB03_BehaviorTree_Legend : EnemyBehaviorManager
{
    protected EnemyController_HB03_Legend enemyController;
    protected EnemyMoveController_HB03_Legend enemyAttackManager;
    private DragaliaEnemyActionTypes aTypes;
    private float _abilityCDLeft = 0;
    [SerializeField] private float _abilityCoolDown = 3;
    [SerializeField] private GameObject effectWhenReceiveDash;
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyController_HB03_Legend>();
        enemyAttackManager = GetComponent<EnemyMoveController_HB03_Legend>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        status.OnTakeDirectDamageFrom += CounterEffect;
        status.ImmuneToAllControlAffliction = true;
        
        if(Projectile_C001_2_Boss.Instance != null)
            Destroy(Projectile_C001_2_Boss.Instance?.gameObject);
        
        GetBehavior();
    }
    

    protected override void DoAction(int state, int substate)
    {
        if(!playerAlive)
            return;

        ParseAction(state,substate);
    }

    protected override bool CheckCondition(string[] args, out int dest_state)
    {
        var conditionName = args[0];

        switch (conditionName)
        {
            case "has_catridge":
            {
                if (status.HasCondition((int)BasicCalculation.BattleCondition.AlchemicCatridge))
                {
                    dest_state = int.Parse(args[1],CultureInfo.InvariantCulture);
                    return true;
                }
                else
                {
                    dest_state = int.Parse(args[2],CultureInfo.InvariantCulture);
                    return false;
                }
            }
            default:
            {
                dest_state = substate + 1;
                break;
            }
        }

        
        return false;
    }

    protected override bool CustomJumpActionConditionCheck(string[] args)
    {
        if (args[0] == "break_rate")
        {
            if ((status as SpecialStatusManager).breakDefRate < ObjectExtensions.ParseInvariantFloat(args[1]))
                return true;
            
            if(status.currentHp < status.maxHP * ObjectExtensions.ParseInvariantFloat(args[1]))
                return true;
        }

        return false;
    }

    protected override void ParseAction(int state, int substate)
    {
        _currentPhase = _pattern.phasePattern[state];
        _currentActionStage = _currentPhase.action_list[substate];

        var action_name = _currentActionStage.action_name;

        if (action_name == "null")
        {
            ActionEnd();
            return;
        }
        
        DragaliaEnemyActionTypes.HB1003L actionType = 
            (DragaliaEnemyActionTypes.HB1003L) Enum.Parse(typeof(DragaliaEnemyActionTypes.HB1003L), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.HB1003L.Arrow:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ProjectileBlast(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1003L.VerticalArrow:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ArrowRain(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1003L.DriveBuster:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (status.HasCondition((int)BasicCalculation.BattleCondition.AlchemicCatridge))
                {
                    currentAction = StartCoroutine(ACT_OverdriveBuster(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_DriveBuster(interval));
                }

                break;
            }
            case DragaliaEnemyActionTypes.HB1003L.AlchemicEnhancement:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (status.HasCondition((int)BasicCalculation.BattleCondition.AlchemicCatridge))
                {
                    currentAction = StartCoroutine(ACT_AlchemicGrenade(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_AlchemicEnhancement(interval));
                }

                break;
            }
            case DragaliaEnemyActionTypes.HB1003L.OtherworldGate:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (!status.HasCondition((int)BasicCalculation.BattleCondition.AlchemicCatridge))
                {
                    currentAction = StartCoroutine(ACT_OtherworldTransportation(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_OtherworldGate(interval));
                }

                break;
            }
            case DragaliaEnemyActionTypes.HB1003L.StormShield:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (status.HasCondition((int)BasicCalculation.BattleCondition.AlchemicCatridge))
                {
                    currentAction = StartCoroutine(ACT_TempestRage(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_TempestShield(interval));
                }

                break;
            }
            case DragaliaEnemyActionTypes.HB1003L.AstralStream:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_AstralStream(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1003L.AstralSurge:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_AstralSurge(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1003L.DoomTempest:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DoomTempest(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1003L.AlchemicShield:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_AlchemicShield(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1003L.SetWorld:
            {
                int type = int.Parse(_currentActionStage.args[0], CultureInfo.InvariantCulture);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                if (type == 0)
                {
                    currentAction = StartCoroutine(ACT_ResetWorld(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_SetWorld(interval));
                }
                
                break;
            }
            case DragaliaEnemyActionTypes.HB1003L.Minion:
            {
                int type = int.Parse(_currentActionStage.args[0], CultureInfo.InvariantCulture);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                if (type == 0)
                {
                    currentAction = StartCoroutine(ACT_SummonAlberius(interval));
                }
                else if(type == 1)
                {
                    currentAction = StartCoroutine(ACT_SummonMordecai(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_SummonZethia(interval));
                }
                
                break;
            }
        }
    }


    protected IEnumerator ACT_ProjectileBlast(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        
        
        yield return new WaitUntil(() => !enemyController.hurt);
        

        currentMoveAction = StartCoroutine(
            enemyController.FlyToRelativePoint(new Vector2(9, 4),
            0.5f, 2, Ease.InOutSine));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action15());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();

        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }

    protected IEnumerator ACT_DriveBuster(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        
        yield return new WaitUntil(() => !enemyController.hurt);
        

        currentMoveAction = StartCoroutine(
            enemyController.FlyToRelativePoint(new Vector2(7, 5.5f),
                0.5f, 2, Ease.InOutSine));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action05());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    
    protected IEnumerator ACT_OverdriveBuster(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        
        yield return new WaitUntil(() => !enemyController.hurt);

        int dir = targetPlayer.transform.position.x > transform.position.x ? -1 : 1;
        var leftPoint = Mathf.Min(targetPlayer.transform.position.x + dir * 6,
            targetPlayer.transform.position.x + dir * 12);
        var rightPoint = Mathf.Max(targetPlayer.transform.position.x + dir * 6,
            targetPlayer.transform.position.x + dir * 12);
        

        currentMoveAction = StartCoroutine(
            enemyController.FlyToPointZone(leftPoint,rightPoint,
                gameObject.RaycastedPosition().y + 6, 1.5f,
                Ease.InOutSine));

        yield return _moveIsNull;
        
        
        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action06());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    protected IEnumerator ACT_AlchemicEnhancement(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPointZone(-2,2,
                gameObject.RaycastedPosition().y + 7, 1.1f,
                Ease.InOutSine));
        
        yield return _moveIsNull;
        
        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action07());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();
        breakable = true;

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    protected IEnumerator ACT_AlchemicGrenade(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        
        yield return new WaitUntil(() => !enemyController.hurt);

        int dir = targetPlayer.transform.position.x > transform.position.x ? -1 : 1;
        var leftPoint = Mathf.Min(targetPlayer.transform.position.x + dir * 8,
            targetPlayer.transform.position.x + dir * 10);
        var rightPoint = Mathf.Max(targetPlayer.transform.position.x + dir * 8,
            targetPlayer.transform.position.x + dir * 10);
        

        currentMoveAction = StartCoroutine(
            enemyController.FlyToPointZone(leftPoint,rightPoint,
                gameObject.RaycastedPosition().y + 5, 1.5f,
                Ease.InOutSine));

        yield return _moveIsNull;

        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action08());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_OtherworldTransportation(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = StartCoroutine(
            enemyController.FlyToPointZone(-1f,1f,
                6.5f, 1.25f,
                Ease.InOutSine));

        yield return _moveIsNull;

        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action09());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();
        breakable = true;

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    
    protected IEnumerator ACT_OtherworldGate(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        //breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action10(targetPlayer));

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();
        //breakable = true;

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    
    protected IEnumerator ACT_TempestShield(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        
        yield return new WaitUntil(() => !enemyController.hurt);
        

        currentMoveAction = StartCoroutine(
            enemyController.FlyToRelativePoint(new Vector2(5, 4f),
                0.5f, 2, Ease.InOutSine));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action16());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_TempestRage(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action17());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_AlchemicShield(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action18());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_AstralStream(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action19());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_AstralSurge(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = StartCoroutine(
            enemyController.FlyToPointZone(-0.1f,0.1f,
                12f, 1.5f,
                Ease.InOutSine));

        yield return _moveIsNull;

        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action20());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();
        breakable = true;

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_SetWorld(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action21());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();
        breakable = true;

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_ResetWorld(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action22());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();
        breakable = true;

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_ArrowRain(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        
        yield return new WaitUntil(() => !enemyController.hurt);
        

        currentMoveAction = StartCoroutine(
            enemyController.FlyToRelativePoint(new Vector2(8, 4f),
                0.5f, 2, Ease.InOutSine));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action26());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_DoomTempest(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action23());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();
        breakable = true;

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_SummonAlberius(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action27());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();
        breakable = true;

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_SummonMordecai(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action28());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();
        breakable = true;

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    private IEnumerator ACT_SummonZethia(float interval)
    {
        ActionStart();
        enemyController.StopExtraMove();
        enemyController.SetKBRes(999);
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action29());

        yield return _attackIsNull;
        
        enemyController.ResetKBRes();
        enemyController.StartExtraMove();
        breakable = true;

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    

    private void CounterEffect(StatusManager self, StatusManager other, AttackBase atk, float dmg)
    {
        if (_abilityCDLeft > 0)
        {
            if (enemyAttackManager.ShieldOn && atk.attackType == BasicCalculation.AttackType.DASH &&
                ((other.transform.position.x >= transform.position.x && enemyController.facedir == 1) ||
                 (other.transform.position.x <= transform.position.x && enemyController.facedir == -1)) )
            {
                Projectile_C001_6_Boss.Instance.ActiveElectricFX();
                BattleStageManager.Instance.CauseIndirectDamage(other, (int)(dmg * 0.2f), true);
                return;
            }
            return;
        }

        if ((self as SpecialStatusManager).broken)
        {
            return;
        }
        
        if (atk.attackType == BasicCalculation.AttackType.DASH)
        {
            print("Shield:"+enemyAttackManager.ShieldOn);
            if (enemyAttackManager.ShieldOn &&
                ((other.transform.position.x >= transform.position.x && enemyController.facedir == 1) ||
                (other.transform.position.x <= transform.position.x && enemyController.facedir == -1)) )
            {
                Projectile_C001_6_Boss.Instance.ActiveElectricFX();
                BattleStageManager.Instance.CauseIndirectDamage(other, (int)(dmg * 0.2f), true);
                return;
            }
            
            (status as SpecialStatusManager).currentBreak -= 500000;
            Instantiate(effectWhenReceiveDash, transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
        }else return;

        
        _abilityCDLeft = _abilityCoolDown;

        DOVirtual.DelayedCall(_abilityCoolDown, () =>
        {
            _abilityCDLeft = 0;
        }, false);
        
        
        
        
        
    }
    
    
    
}
