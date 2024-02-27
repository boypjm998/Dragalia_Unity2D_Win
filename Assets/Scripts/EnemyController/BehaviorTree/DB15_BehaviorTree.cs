using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

/// <summary>
/// 堕天使米迦勒
/// </summary>
public class DB15_BehaviorTree : EnemyBehaviorManager
{
    
    private EnemyControllerFlyingHigh enemyController;
    private EnemyMoveController_DB15 enemyAttackManager;
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyAttackManager = GetComponent<EnemyMoveController_DB15>();
        GetBehavior();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
    }
    
    protected override void DoAction(int state, int substate)
    {
        if (playerAlive == false)
            return;
        
        _currentPhase = _pattern.phasePattern[state];
        _currentActionStage = _currentPhase.action_list[substate];

        var action_name = _currentActionStage.action_name;

        if (action_name == "null")
        {
            ActionEnd();
            return;
        }
        
        DragaliaEnemyActionTypes.DB2015 actionType = 
            (DragaliaEnemyActionTypes.DB2015) Enum.Parse(typeof(DragaliaEnemyActionTypes.DB2015), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.DB2015.crystal_chase:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CrystalChase(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.crystal_fixed:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CrystalFixed(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.nihil:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Nihil(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.combo:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Combo(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.buff:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DefenseBuff(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.fireball:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BouncingFireball(interval));
                break;
            }

            default: break;

        }
        
        
        
    }
    
    protected IEnumerator ACT_CrystalChase(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);


        // currentMoveAction = StartCoroutine
        //     (enemyController.FlyTowardTargetOnSamePlatform
        //         (targetPlayer, 5, 2, 5));
        //
        //         
        // yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action01());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_CrystalFixed(float interval)
    {
        SetTarget(ClosestTarget);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 5, 2, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action02());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_Nihil(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action03());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_Combo(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 4, 2, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action04());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_DefenseBuff(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 12, 2, 3));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action05());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_BouncingFireball(float interval)
    {
        ActionStart();
        SetTarget(FurthestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 6, 2, 3));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action06());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

}
