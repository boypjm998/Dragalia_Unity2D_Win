using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class HumanMeele_BehaviorTree : EnemyBehaviorManager
{
    protected EnemyControllerHumanoid enemyController;
    protected EnemyMoveController_HumanMeele enemyAttackManager;
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        enemyAttackManager = GetComponent<EnemyMoveController_HumanMeele>();
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
        
        DragaliaEnemyActionTypes.HECommon actionType = 
            (DragaliaEnemyActionTypes.HECommon) Enum.Parse(typeof(DragaliaEnemyActionTypes.HECommon), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.HECommon.swd_1:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                int element = 0;
                if(_currentActionStage.args.Length > 1)
                    element = Convert.ToInt32(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_SWD_CMB_01(interval,element));
                break;
            }
            case DragaliaEnemyActionTypes.HECommon.swd_3:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                int element = 0;
                if(_currentActionStage.args.Length > 1)
                    element = Convert.ToInt32(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_SWD_CMB_03(interval,element));
                break;
            }
            case DragaliaEnemyActionTypes.HECommon.lan_hi_1:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                int element = 0;
                if(_currentActionStage.args.Length > 1)
                    element = Convert.ToInt32(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_LAN_HI_CMB_01(interval,element));
                break;
            }
            case DragaliaEnemyActionTypes.HECommon.lan_hi_2:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                int element = 0;
                if(_currentActionStage.args.Length > 1)
                    element = Convert.ToInt32(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_LAN_HI_CMB_02(interval,element));
                break;
            }
            case DragaliaEnemyActionTypes.HECommon.lan_hi_3:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                int element = 0;
                if(_currentActionStage.args.Length > 1)
                    element = Convert.ToInt32(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_LAN_HI_CMB_03(interval,element));
                break;
            }
            case DragaliaEnemyActionTypes.HECommon.lan_hi_4:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                int element = 0;
                if(_currentActionStage.args.Length > 1)
                    element = Convert.ToInt32(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_LAN_HI_WARP(interval,element));
                break;
            }
            case DragaliaEnemyActionTypes.HECommon.summon_1:
            {
                object[] args = new object[]
                {
                    _currentActionStage.args[0],
                    ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]),
                    ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]),
                    int.Parse(_currentActionStage.args[3]),
                    int.Parse(_currentActionStage.args[4]),
                    _currentActionStage.args[5],
                    ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[6]),
                    ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[7]),
                    int.Parse(_currentActionStage.args[8]),
                    int.Parse(_currentActionStage.args[9])
                };
                currentAction = StartCoroutine(ACT_SMN_DOUBLE(
                    ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[10]),args));
                break;
            }

            case DragaliaEnemyActionTypes.HECommon.fx_wroth:
            {
                string fx_name = _currentActionStage.args[0];
                enemyAttackManager.FX_Wroth();
                ActionEnd();
                break;
            }

        }
        
        
        
    }
    
    // Element: 0: Fire, 1: Water, 2: Wind, 3: Light, 4: Shadow

    protected IEnumerator ACT_SWD_CMB_01(float interval, int element = 0)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 99, 3 + Random.Range(-0.25f, 0.25f)));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HE01_SWD_Action01());
        
        yield return new WaitUntil(()=>(currentAttackAction == null));
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_SWD_CMB_03(float interval, int element = 0)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 99, 3.5f+ Random.Range(-0.5f, 0f)) );
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HE01_SWD_Action03());
        
        yield return new WaitUntil(()=>(currentAttackAction == null));
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    
    protected IEnumerator ACT_LAN_HI_CMB_01(float interval, int element = 0)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 6, 7f+ Random.Range(-0.5f, 0f)) );
        
        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HE01_LAN_HI_Action01());

        yield return _attackIsNull;
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }


    protected IEnumerator ACT_LAN_HI_CMB_02(float interval, int element = 0)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 6, 10f+ Random.Range(-0.5f, 0f)) );
        
        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HE01_LAN_HI_Action02());

        yield return _attackIsNull;
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_LAN_HI_CMB_03(float interval, int element = 0)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 6, 5f+ Random.Range(-0.5f, 0f)) );
        
        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HE01_LAN_HI_Action03());

        yield return _attackIsNull;
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_LAN_HI_WARP(float interval, int element = 0)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HE01_LAN_HI_Action04());

        yield return _attackIsNull;
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_SMN_DOUBLE(float interval, object[] msg)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 99, 10f + Random.Range(-0.5f, 0.5f)) );
        
        yield return _moveIsNull;
        

        if (BattleStageManager.Instance.currentEnemyInLayerDeadAlive < 8)
        {
            enemyController.SetKBRes(999);
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HE01_SummonMinons(msg));

            yield return _attackIsNull;
            
            enemyController.SetKBRes(status.knockbackRes);
            yield return new WaitForSeconds(interval);
        }

        
        ActionEnd();
    }

}
