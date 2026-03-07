using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class DB15V_BehaviorTree : EnemyBehaviorManager
{
    private EnemyControllerFlyingHigh enemyController;
    private EnemyMoveController_DB15V enemyAttackManager;
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyAttackManager = GetComponent<EnemyMoveController_DB15V>();
        GetBehavior();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
    }

    protected override void DoAction(int state, int substate)
    {
        if(!playerAlive)
            return;
        ParseAction(state, substate);
    }

    protected override void ParseAction(int state, int substate)
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
        
        DragaliaEnemyActionTypes.DB2015V actionType = 
            (DragaliaEnemyActionTypes.DB2015V) Enum.Parse(typeof(DragaliaEnemyActionTypes.DB2015V), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.DB2015V.Buff:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DefenseBuff(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015V.Combo:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Combo(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015V.AngeticWind:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_AngeticWind(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015V.Spike:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Convinction(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015V.Throw:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Throw(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015V.Shield:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Shield(interval));
                break;
            }

            default: break;

        }
    }
    
    protected IEnumerator ACT_DefenseBuff(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15V_Action01());

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

        enemyController.SetKBRes(999);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 18, 2, 3));
        
        yield return new WaitUntil(()=>currentMoveAction == null);
        
        if(Mathf.Abs(transform.position.x - targetPlayer.transform.position.x) < 8)
            currentAttackAction = StartCoroutine(enemyAttackManager.DB15V_Action02());
        else
            currentAttackAction = StartCoroutine(enemyAttackManager.DB15V_Action03());


        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    
    protected IEnumerator ACT_AngeticWind(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);

        if(Mathf.Abs(transform.position.x - targetPlayer.transform.position.x) < 8)
            currentAttackAction = StartCoroutine(enemyAttackManager.DB15V_Action04(true));
        else
            currentAttackAction = StartCoroutine(enemyAttackManager.DB15V_Action04(false));


        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    
    protected IEnumerator ACT_Convinction(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 18, 2, 3));
        
        yield return new WaitUntil(()=>currentMoveAction == null);
        
        if(Mathf.Abs(transform.position.x - targetPlayer.transform.position.x) < 8)
            currentAttackAction = StartCoroutine(enemyAttackManager.DB15V_Action05(true));
        else
            currentAttackAction = StartCoroutine(enemyAttackManager.DB15V_Action05(false));


        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }


    protected IEnumerator ACT_Throw(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);

        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 18, 2, 3));

        yield return new WaitUntil(() => currentMoveAction == null);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB15V_Action06());

        yield return new WaitUntil(() => currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_Shield(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);

        currentMoveAction = StartCoroutine
        (enemyController.FlyToPoint(new Vector2(0, BattleStageManager.Instance.mapBorderB + 1.4f),
            1, Ease.InOutSine));

        yield return new WaitUntil(() => currentMoveAction == null);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB15V_Action07());

        yield return new WaitUntil(() => currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        ActionEnd();
        
    }


}
