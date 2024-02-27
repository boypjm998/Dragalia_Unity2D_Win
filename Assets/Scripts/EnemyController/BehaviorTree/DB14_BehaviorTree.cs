using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
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
        
        DragaliaEnemyActionTypes.DB2014 actionType = 
            (DragaliaEnemyActionTypes.DB2014) Enum.Parse(typeof(DragaliaEnemyActionTypes.DB2014), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.DB2014.water_jet:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_WaterJet(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2014.dash:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DashAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2014.slap:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DoubleSlap(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2014.buff:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                int buffAmount = 25;
                if (_currentActionStage.args.Length > 1)
                {
                    buffAmount = int.Parse(_currentActionStage.args[1]);
                }
                
                currentAction = StartCoroutine(ACT_Buff((int)buffAmount,interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2014.around:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Around(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2014.scatter:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ScatteredWater(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2014.sphere:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SurgingSpheres(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2014.corrosion:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float corrosionAmount = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CorrosionFog((int)corrosionAmount,interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2014.cascade:
            {
                int minionCount = (_currentActionStage.args.Length - 3)/2;
                var list = new List<Vector2>();
                for (int i = 0; i < minionCount; i++)
                {
                    list.Add(new Vector2(ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[i * 2]),
                        ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[i * 2+1])));
                }

                int hp = int.Parse(_currentActionStage.args[^3]);
                float waitTime = int.Parse(_currentActionStage.args[^2]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[^1]);
                currentAction = StartCoroutine(ACT_Cascade(hp, list.ToArray(), waitTime, interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2014.whirl:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FrostWhirl(interval));
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
    
    protected IEnumerator ACT_Around(float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 5, 1, 4));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB14_Action06());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_ScatteredWater(float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        // currentMoveAction = StartCoroutine
        // (enemyController.FlyTowardTargetOnSamePlatform
        //     (targetPlayer, 15, 1, 4));

        var height = BattleStageManager.Instance.mapBorderB + 8;

        currentMoveAction = StartCoroutine(enemyController.FlyToPoint(new Vector2(transform.position.x, height),
            Vector2.Distance(transform.position, new Vector2(transform.position.x, height))/20, Ease.InOutSine
        ));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB14_Action07());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_SurgingSpheres(float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB14_Action08());

        yield return _attackIsNull;

        var endPosition = targetPlayer.RaycastedPosition();
        var distance = Vector2.Distance(endPosition, transform.position);

        currentMoveAction = StartCoroutine(enemyController.FlyToPoint
            (endPosition, distance / enemyController.moveSpeed, Ease.InOutSine));

        yield return _moveIsNull;
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB14_Action08_2());

        yield return _attackIsNull;
        breakable = true;
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

    protected IEnumerator ACT_Cascade(int hp, Vector2[] positions, float waitTime, float interval)
    {
        ActionStart();
        breakable = false;
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 12, 2, 3));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB14_Action09(hp,positions,waitTime));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_FrostWhirl(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB14_Action10());

        yield return _attackIsNull;
        
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 2, 1, 3));


        yield return _moveIsNull;
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB14_Action06());
        
        yield return _attackIsNull;

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }




}
