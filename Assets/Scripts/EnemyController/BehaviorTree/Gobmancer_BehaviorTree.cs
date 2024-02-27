using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class Gobmancer_BehaviorTree : EnemyBehaviorManager
{
    protected EnemyControllerHumanoid enemyController;
    protected EnemyMoveController_Gobmancer enemyMoveController;

    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        enemyMoveController = GetComponent<EnemyMoveController_Gobmancer>();
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
        
        DragaliaEnemyActionTypes.Gobmancer actionType = 
            (DragaliaEnemyActionTypes.Gobmancer) Enum.Parse(typeof(DragaliaEnemyActionTypes.Gobmancer), action_name);


        switch (actionType)
        {
            case DragaliaEnemyActionTypes.Gobmancer.straight:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Projectile(interval));
                break;
            }
            case DragaliaEnemyActionTypes.Gobmancer.target:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ForceStrike(interval));
                break;
            }
        }


    }


    private IEnumerator ACT_Projectile(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        var myPlatfrom = gameObject.RaycastedPlatform();
        var targetPlatfrom = targetPlayer.RaycastedPlatform();
        
        
        if(targetPlatfrom.bounds.min.x - myPlatfrom.bounds.max.x > 15 ||
           myPlatfrom.bounds.min.x - targetPlatfrom.bounds.max.x > 15)
        {
            ActionEnd();
            yield return new WaitForSeconds(interval/2);
            yield break;
        }


        if(Mathf.Abs(myPlatfrom.bounds.max.y - targetPlatfrom.bounds.max.y) < 1)
        {
            currentMoveAction = StartCoroutine(enemyController.
                KeepDistanceFromTarget(targetPlayer, 2, 5, 14));

            yield return _moveIsNull;
            
            enemyController.SetKBRes(999);

            currentAttackAction = StartCoroutine(
                enemyMoveController.E9004_Action01());
        
            enemyController.ResetKBRes();

            yield return _attackIsNull;
        
            yield return new WaitForSeconds(interval);
        }
        else
        {
            yield return new WaitForSeconds(interval/2);
        }

        
        ActionEnd();
        
    }

    private IEnumerator ACT_ForceStrike(float interval)
    {
        
        ActionStart();
        SetTarget(FurthestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        enemyController.SetKBRes(999);

        currentAttackAction = StartCoroutine(
            enemyMoveController.E9004_Action02());
        
        enemyController.ResetKBRes();

        yield return _attackIsNull;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
        
        
    }



}
