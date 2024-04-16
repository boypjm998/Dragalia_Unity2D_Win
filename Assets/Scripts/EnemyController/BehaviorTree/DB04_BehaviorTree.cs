using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class DB04_BehaviorTree : EnemyBehaviorManager
{
    protected EnemyMoveController_DB04 enemyAttackManager;
    protected EnemyControllerFlyingHigh enemyController;
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_DB04>();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        GetBehavior();
    }

    protected override void DoAction(int state, int substate)
    {
        if(!playerAlive)
            return;
        
        ParseAction(state, substate);
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
        
        DragaliaEnemyActionTypes.DB2004 actionType = 
            (DragaliaEnemyActionTypes.DB2004) Enum.Parse(typeof(DragaliaEnemyActionTypes.DB2004), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.DB2004.around:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Around(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.blast:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_AllRangedBlast(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.bolt:
            {
                int mine = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_BoltBarrage(mine,interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.claw:
            {
                int avoidable = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_Claw(avoidable,interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.dash:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Dash(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.dual:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DoubleThunder(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.memory1:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_MemoryZena1(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.memory2:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_MemoryZena2(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.memory3:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_MemoryZena3(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.memory4:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_MemoryZena4(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.random:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_RandomStorm(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.shells:
            {
                int direction = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_VoltaicShells(direction,interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.summon1:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonShadows1(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.summon2:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonShadows2(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.summon3:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonShadows3(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.summon4:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonShadows4(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.summon5:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonShadows5(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.summon6:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonShadows6(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.sweep:
            {
                int fixedType = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SweepingThunder(fixedType,interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.sweep_double:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SweepingThunderHigh(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2004.twist:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TwistThunder(interval));
                break;
            }
        }
        
    }

    protected IEnumerator ACT_Around(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = 
            StartCoroutine(
                enemyController.FlyTowardTargetOnSamePlatform
                    (targetPlayer, 4f, 0.5f, 3));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action01());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }
    
    protected IEnumerator ACT_Dash(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = 
            StartCoroutine(
                enemyController.FlyTowardTargetOnSamePlatform
                    (targetPlayer, 15f, 0.5f, 3));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action02());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }
    
    protected IEnumerator ACT_DoubleThunder(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = 
            StartCoroutine(
                enemyController.FlyTowardTargetOnSamePlatform
                    (targetPlayer, 16f, 0.5f, 3));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action21());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }
    
    protected IEnumerator ACT_Claw(int avoidable, float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = 
            StartCoroutine(
                enemyController.FlyTowardTargetOnSamePlatform
                    (targetPlayer, 6f, 0.5f, 3));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action03(avoidable > 0));
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }

    protected IEnumerator ACT_TwistThunder(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = 
            StartCoroutine(
                enemyController.FlyTowardTargetOnSamePlatform
                    (targetPlayer, 12f, 0.5f, 3));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action04());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_BoltBarrage(int mine, float interval)
    {
        ActionStart();

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action05(mine));
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_RandomStorm(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = 
            StartCoroutine(
                enemyController.FlyTowardTargetOnSamePlatform
                    (targetPlayer, 8f, 0.5f, 3));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action06());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_SweepingThunder(int fixedType, float interval)
    {
        ActionStart();
        breakable = false;
        status.ImmuneToAllControlAffliction = true;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint(new Vector2(0, 8.8f),
                1,Ease.InOutSine));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action07(fixedType));
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_VoltaicShells(int direction, float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint(new Vector2(0, 8.8f),
                1,Ease.InOutSine));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action08(direction));
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_MemoryZena1(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action09());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_MemoryZena2(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action10());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_MemoryZena3(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint(new Vector2(0, 8.7f),
                1,Ease.InOutSine));

        yield return _moveIsNull;

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action11());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_MemoryZena4(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint(new Vector2(0, 10f),
                1,Ease.InOutSine));

        yield return _moveIsNull;

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action12());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_SummonShadows1(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action13());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_SummonShadows2(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action15());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_SummonShadows3(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action14());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_SummonShadows4(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action16());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_SummonShadows5(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action17());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_SummonShadows6(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action18());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_AllRangedBlast(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action19());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_SweepingThunderHigh(float interval)
    {
        ActionStart();
        breakable = false;
        status.ImmuneToAllControlAffliction = true;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint(new Vector2(0, 8.8f),
                1,Ease.InOutSine));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB04_Action20());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

}
