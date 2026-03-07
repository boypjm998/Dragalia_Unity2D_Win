using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

/// <summary>
/// Agni: Void
/// </summary>
public class DB16_BehaviorTree : EnemyBehaviorManager
{
    EnemyControllerFlyingHigh enemyController;
    EnemyMoveController_DB16 enemyAttackManager;
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_DB16>();
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
              
        DragaliaEnemyActionTypes.DB2016 actionType = 
            (DragaliaEnemyActionTypes.DB2016) Enum.Parse(typeof(DragaliaEnemyActionTypes.DB2016), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.DB2016.ClawAttack:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ClawAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2016.FrontStrike:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FrontStrike(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2016.Dash:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DashAttack(interval));
                break;
            }
            
            
        }
    }
    
    
    protected IEnumerator ACT_ClawAttack(float interval)
    {
        ActionStart();
        
        //breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        if (gameObject.RaycastedPlatform() != targetPlayer.RaycastedPlatform())
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.DB16_Action01());
            yield return _attackIsNull;
            enemyController.SetKBRes(status.knockbackRes);
        }
        else
        {
            currentMoveAction = StartCoroutine(enemyController.MoveTowardTargetWithoutFlying(targetPlayer,
                3, 5, 6));
            yield return _moveIsNull;
            
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.DB16_Action02());
            yield return _attackIsNull;
        }

        yield return new WaitForSeconds(interval);
        ActionEnd();
        
    }

    protected IEnumerator ACT_FrontStrike(float interval)
    {
        ActionStart();
        
        //breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine(enemyController.MoveTowardTargetWithoutFlying(targetPlayer,
                1.5f, 5, 6));
        yield return _moveIsNull;
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB16_Action03());
        yield return _attackIsNull;

        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    
    protected IEnumerator ACT_DashAttack(float interval)
    {
        ActionStart();
        
        //breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        if (gameObject.RaycastedPlatform() != targetPlayer.RaycastedPlatform())
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.DB16_Action01());
            yield return _attackIsNull;
            
        }
        
        currentMoveAction = StartCoroutine(enemyController.MoveTowardTargetWithoutFlying(targetPlayer,
                1, 8, 6));
        yield return _moveIsNull;
            
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB16_Action05());
        yield return _attackIsNull;
        

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
        
    }
    
    
    
    
}
