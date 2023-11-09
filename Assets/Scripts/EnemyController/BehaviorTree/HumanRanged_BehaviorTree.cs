using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HumanRanged_BehaviorTree : EnemyBehaviorManager
{
    protected EnemyControllerHumanoid enemyController;
    protected EnemyMoveController_HumanRanged enemyAttackManager;
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        enemyAttackManager = GetComponent<EnemyMoveController_HumanRanged>();
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
        
        DragaliaBossActionTypes.HECommon actionType = 
            (DragaliaBossActionTypes.HECommon) Enum.Parse(typeof(DragaliaBossActionTypes.HECommon), action_name);

        switch (actionType)
        {
            case DragaliaBossActionTypes.HECommon.rod_1:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                int element = 0;
                if(_currentActionStage.args.Length > 1)
                    element = Convert.ToInt32(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_ROD_CMB_01(interval,element));
                break;
            }
            case DragaliaBossActionTypes.HECommon.rod_2:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                int element = 0;
                if(_currentActionStage.args.Length > 1)
                    element = Convert.ToInt32(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_ROD_FS(interval,element));
                break;
            }
            case DragaliaBossActionTypes.HECommon.fx_wroth:
            {
                string fx_name = _currentActionStage.args[0];
                enemyAttackManager.FX_Wroth();
                ActionEnd();
                break;
            }

        }
        
        
        
    }
    
    protected IEnumerator ACT_ROD_CMB_01(float interval, int element = 0)
    {
        ActionStart();
        SetTarget(ClosestTarget);

        currentMoveAction =
            StartCoroutine
                (enemyController.KeepDistanceFromTarget(targetPlayer, 3f,
                    5f + Random.Range(0,2f), 11f + Random.Range(0,2f)));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HE01_ROD_Action01());
        
        yield return new WaitUntil(()=>(currentAttackAction == null));
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_ROD_FS(float interval, int element = 0)
    {
        ActionStart();
        SetTarget(FurthestTarget);

        currentMoveAction =
            StartCoroutine
                (enemyController.KeepDistanceFromTarget(targetPlayer, 1f,
                    9f + Random.Range(0,2f), 17f + Random.Range(0,2f)));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HE01_ROD_Action02());
        
        yield return new WaitUntil(()=>(currentAttackAction == null));
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
}
