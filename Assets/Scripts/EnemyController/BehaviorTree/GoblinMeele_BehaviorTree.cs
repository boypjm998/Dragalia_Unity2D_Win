using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class GoblinMeele_BehaviorTree : EnemyBehaviorManager
{
    
    protected EnemyControllerHumanoid enemyController;
    protected EnemyMoveController_GoblinMeele enemyMoveController;
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        enemyMoveController = GetComponent<EnemyMoveController_GoblinMeele>();
        enemyController.OnMoveFinished += FinishMove;
        GetBehavior();
    }

    protected override void DoAction(int state, int substate)
    {
        if(!playerAlive)
            return;
        
        ParseAction(state, substate);
    }


    protected void ParseAction(int state, int substate)
    {
        _currentPhase = _pattern.phasePattern[state];
        _currentActionStage = _currentPhase.action_list[substate];

        var action_name = _currentActionStage.action_name;

        if (action_name == "null")
        {
            ActionEnd();
            return;
        }
        
        DragaliaEnemyActionTypes.Goblin actionType = 
            (DragaliaEnemyActionTypes.Goblin) Enum.Parse(typeof(DragaliaEnemyActionTypes.Goblin), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.Goblin.smash:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SmashCombo(interval));
                break;
            }
            case DragaliaEnemyActionTypes.Goblin.slash:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Slash(interval));
                break;
            }
            case DragaliaEnemyActionTypes.Goblin.buff:
            {
                if (int.Parse(_currentActionStage.args[0]) == 0)
                {
                    float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                    currentAction = StartCoroutine(ACT_Immidiate(interval));
                }
                else
                {
                    currentAttackAction = StartCoroutine(ACT_Immidiate
                        (ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0])));
                }
                break;
            }
        }
        print("ActionStart");
        
    }
    
    
    protected IEnumerator ACT_Slash(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        currentMoveAction = StartCoroutine(
            enemyController.MoveTowardsTarget(targetPlayer, 3f, 10f));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);

        currentAttackAction = StartCoroutine(
            enemyMoveController.E9003_Action01());
        
        enemyController.ResetKBRes();

        yield return _attackIsNull;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();

    }

    protected IEnumerator ACT_SmashCombo(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        currentMoveAction = StartCoroutine(
            enemyController.MoveTowardsTarget(targetPlayer, 3.5f, 10f));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);

        currentAttackAction = StartCoroutine(
            enemyMoveController.E9003_Action02());
        
        enemyController.ResetKBRes();

        yield return _attackIsNull;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }


    protected IEnumerator ACT_Immidiate(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        currentMoveAction = StartCoroutine(
            enemyController.MoveTowardsTarget(targetPlayer, 15f, 3f));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);

        currentAttackAction = StartCoroutine(
            enemyMoveController.E9003_Action03());
        
        enemyController.ResetKBRes();

        yield return _attackIsNull;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }




}
