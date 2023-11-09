using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB14_BehaviorTree : EnemyBehaviorManager
{
    protected EnemyMoveController_DB14 enemyAttackManager;
    protected EnemyControllerFlyingHigh enemyController;

    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_DB14>();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
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
        
        print("Doing: "+action_name);
        
        DragaliaBossActionTypes.DB2014 actionType = 
            (DragaliaBossActionTypes.DB2014) Enum.Parse(typeof(DragaliaBossActionTypes.DB2014), action_name);

        switch (actionType)
        {
            case DragaliaBossActionTypes.DB2014.water_jet:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_WaterJet(interval));
                break;
            }
            case DragaliaBossActionTypes.DB2014.dash:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DashAttack(interval));
                break;
            }
            case DragaliaBossActionTypes.DB2014.slap:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DoubleSlap(interval));
                break;
            }
            case DragaliaBossActionTypes.DB2014.buff:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                int buffAmount = 25;
                if (_currentActionStage.args.Length > 1)
                {
                    buffAmount = int.Parse(_currentActionStage.args[1]);
                }
                
                currentAction = StartCoroutine(ACT_Buff((int)buffAmount,interval));
                break;
            }
            case DragaliaBossActionTypes.DB2014.corrosion:
            {
                float interval = float.Parse(_currentActionStage.args[1]);
                float corrosionAmount = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CorrosionFog((int)corrosionAmount,interval));
                break;
            }

        }
    }



    protected IEnumerator ACT_DashAttack(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 12, 2, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB14_Action01());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_WaterJet(float interval)
    {
        ActionStart();
        SetTarget(FurthestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 10, 2, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB14_Action02());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_Buff(int amount, float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 12, 2, 3));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB14_Action05(amount));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_CorrosionFog(int corrosionAmount, float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 18, 2, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB14_Action03(corrosionAmount));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }


    protected IEnumerator ACT_DoubleSlap(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 3, 2, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB14_Action04());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }






}
