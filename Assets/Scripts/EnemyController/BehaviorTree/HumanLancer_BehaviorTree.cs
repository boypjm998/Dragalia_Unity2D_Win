using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class HumanLancer_BehaviorTree : EnemyBehaviorManager
{
    protected EnemyControllerHumanoid enemyController;
    protected EnemyMoveController_Lancer enemyAttackManager;
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        enemyAttackManager = GetComponent<EnemyMoveController_Lancer>();
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
        
        print("Doing: "+action_name);
        
        DragaliaEnemyActionTypes.HECommon actionType = 
            (DragaliaEnemyActionTypes.HECommon) Enum.Parse(typeof(DragaliaEnemyActionTypes.HECommon), action_name);

        switch (actionType)
        {
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
            case DragaliaEnemyActionTypes.HECommon.lan_hi_5:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_LAN_HI_SuperDash(interval));
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
            case DragaliaEnemyActionTypes.HECommon.phantom_1:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                string prefabName = _currentActionStage.args[1];
                int num = Convert.ToInt32(_currentActionStage.args[2]);
                int edge = Convert.ToInt32(_currentActionStage.args[3]);
                currentAction = StartCoroutine(ACT_PhantomAssultAxe(interval,prefabName,num,edge));
                break;
            }
            case DragaliaEnemyActionTypes.HECommon.phantom_2:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                string prefabName = _currentActionStage.args[1];
                float fillTime = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);
                if (_currentActionStage.args.Length == 4)
                {
                    float aimPosition = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[3]);
                    currentAction = StartCoroutine(
                        ACT_PhantomAssultBow(interval,prefabName,1,aimPosition,fillTime));
                }
                else
                {
                    currentAction = StartCoroutine(
                        ACT_PhantomAssultBow(interval,prefabName,0,0,fillTime));
                }
                
                break;
            }

        }
        
        
        
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
    
    protected IEnumerator ACT_LAN_HI_SuperDash(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 6, 25 ));
        
        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HE01_LAN_HI_Action05());

        yield return _attackIsNull;
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_PhantomAssultAxe(float interval, string prefabName, int num, int edge)
    {
        ActionStart();
        //SetTarget(ClosestTarget);

        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HE01_LAN_HI_Action06(num,prefabName,edge!=0));

        yield return _attackIsNull;
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_PhantomAssultBow(float interval, string prefabName, int aim, float posX, float fillTime)
    {
        ActionStart();
        //SetTarget(ClosestTarget);

        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HE01_LAN_HI_Action07(prefabName,aim!=0,posX,fillTime));

        yield return _attackIsNull;
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
}
