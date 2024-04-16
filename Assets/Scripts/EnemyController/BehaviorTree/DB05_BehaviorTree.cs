using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class DB05_BehaviorTree : EnemyBehaviorManager
{
    EnemyMoveController_DB05 enemyAttackManager;
    EnemyControllerFlyingHigh enemyController;
    

    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_DB05>();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyAttackManager.OnAttackFinished += FinishAttack;
        enemyController.OnMoveFinished += FinishMove;
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
        
        DragaliaEnemyActionTypes.DB2005 actionType = 
            (DragaliaEnemyActionTypes.DB2005) Enum.Parse(typeof(DragaliaEnemyActionTypes.DB2005), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.DB2005.Around:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_AroundAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.Tail:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TailAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.Sprint:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_LongDash(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.PoisonSide:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_PoisonBreathSide(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.PoisonFront:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_PoisonBreathLeftCenter(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.Chaser:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_UmbralChaser(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.Spit:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_PoisonSpit(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.CursedFlame:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonFlames(interval));
                break;
            }
        }
    }
    
    
    
    private IEnumerator ACT_AroundAttack(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = 
            StartCoroutine(
                enemyController.MoveTowardTargetWithoutFlying(targetPlayer,
                    5,5,5.5f));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action01());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_TailAttack(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action02());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_LongDash(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = 
            StartCoroutine(
                enemyController.MoveTowardTargetWithoutFlying(targetPlayer,
                    5,18,25));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action03());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_PoisonBreathSide(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = 
            StartCoroutine(
                enemyController.MoveTowardTargetWithoutFlying(targetPlayer,
                    5,10,12f));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action04());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    private IEnumerator ACT_PoisonBreathLeftCenter(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        // currentMoveAction = 
        //     StartCoroutine(
        //         enemyController.MoveTowardTargetWithoutFlying(targetPlayer,
        //             5,10,12f));
        //
        // yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action08());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_UmbralChaser(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action05());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_PoisonSpit(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = 
            StartCoroutine(
                enemyController.MoveTowardTargetWithoutFlying(targetPlayer,
                    5,8,12f));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action06());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_SummonFlames(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = 
            StartCoroutine(
                enemyController.MoveTowardTargetWithoutFlying(enemyAttackManager.GetAnchor(0),
                    5,1f,1f));

        yield return _moveIsNull;

        enemyAttackManager.DB05_Action07();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
}
