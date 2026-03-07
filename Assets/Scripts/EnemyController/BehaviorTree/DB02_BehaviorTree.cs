using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class DB02_BehaviorTree : EnemyBehaviorManager
{
    protected EnemyMoveController_DB02 enemyAttackManager;
    protected EnemyControllerFlyingHigh enemyController;
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_DB02>();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        GetBehavior();
        OnBehaviorStart += InitCameraAndAbilities;
    }
    
    private void InitCameraAndAbilities()
    {
        StageCameraController.Instance.SetMinCameraSize(9);

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
        
        DragaliaEnemyActionTypes.DB2002 actionType = 
            (DragaliaEnemyActionTypes.DB2002) Enum.Parse(typeof(DragaliaEnemyActionTypes.DB2002), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.DB2002.Claw:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Claw(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2002.Pillar:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Meltdown(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2002.Inferno:
            {
                float interval;
                if (_currentActionStage.args.Length > 1)
                {
                    interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                    //switch args-0
                    currentAction = StartCoroutine(ACT_CrimsonInfernoOrDash(interval));
                }
                else
                {
                    interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                    currentAction = StartCoroutine(ACT_CrimsonInferno(interval));
                }
                break;
            }
            case DragaliaEnemyActionTypes.DB2002.Fireball:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Fireball(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2002.Breathe:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Breathe(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2002.Whirlwind:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Whirlwind(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2002.Chaser:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FlameChaser(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2002.Muspelheim:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Muspelheim(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2002.Memories:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                int type = int.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonMaribelle(interval));
                break;
            }
        }
        
    }
    
    
    protected IEnumerator ACT_Claw(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = 
            StartCoroutine(
                enemyController.FlyTowardTargetOnSamePlatform
                    (targetPlayer, 5f, 0.5f, 3));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB02_Action01());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }
    
    
    protected IEnumerator ACT_CrimsonInferno(float interval)
    {
        ActionStart();
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB02_Action03());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }
    
    protected IEnumerator ACT_Meltdown(float interval)
    {
        ActionStart();
        //breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB02_Action02());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        //breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }
    
    
    protected IEnumerator ACT_CrimsonInfernoOrDash(float interval)
    {
        ActionStart();
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        if(Mathf.Abs(transform.position.x - targetPlayer.transform.position.x)<13 &&
           (targetPlayer.transform.position.y - transform.position.y < 10 &&
            targetPlayer.transform.position.y - transform.position.y > 0.5f))
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB02_Action03M());
        else currentAttackAction = StartCoroutine(enemyAttackManager.DB02_Action03());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }
    
    protected IEnumerator ACT_Fireball(float interval)
    {
        ActionStart();
        //breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB02_Action04());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        //breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }
    
    protected IEnumerator ACT_Breathe(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        breakable = false;

        if (Mathf.Abs(transform.position.x - targetPlayer.transform.position.x) < 7.5f)
        {
            var targetPos = GetAvoidPosition(15f,0.75f);
            float distance = Vector2.Distance(transform.position, targetPos);
            currentMoveAction = StartCoroutine(enemyController.FlyToPoint(targetPos,
                distance / enemyController.moveSpeed, Ease.InOutSine));
        }
        else
        {
            currentMoveAction = 
                StartCoroutine(
                    enemyController.FlyTowardTargetOnSamePlatform
                        (targetPlayer, 99f, 0.5f, 1.5f));
        }
        yield return _moveIsNull;

        breakable = true;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB02_Action05());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }
    
    protected IEnumerator ACT_Whirlwind(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        breakable = false;

        if (Mathf.Abs(transform.position.x - BattleStageManager.Instance.mapBorderL) < 6f)
        {
            var targetPos = GetAvoidPosition(10f,0.75f);
            float distance = Vector2.Distance(transform.position, targetPos);
            currentMoveAction = StartCoroutine(enemyController.FlyToPoint(targetPos,
                distance / enemyController.moveSpeed, Ease.InOutSine));
        }
        else if (Mathf.Abs(transform.position.x - BattleStageManager.Instance.mapBorderR) < 6f)
        {
            var targetPos = GetAvoidPosition(10f,0.75f);
            float distance = Vector2.Distance(transform.position, targetPos);
            currentMoveAction = StartCoroutine(enemyController.FlyToPoint(targetPos,
                distance / enemyController.moveSpeed, Ease.InOutSine));
        }
        else
        {
            currentMoveAction = 
                StartCoroutine(
                    enemyController.FlyTowardTargetOnSamePlatform
                        (targetPlayer, 12f, 0.5f, 1.5f));
        }
        yield return _moveIsNull;

        breakable = true;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB02_Action06());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }
    
    protected IEnumerator ACT_FlameChaser(float interval)
    {
        ActionStart();
        //breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB02_Action07());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        //breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }
    
    protected IEnumerator ACT_Muspelheim(float interval)
    {
        ActionStart();
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB02_Action08());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }
    
    protected IEnumerator ACT_SummonMaribelle(float interval)
    {
        ActionStart();
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB02_Action10());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();

    }
    

    private Vector2 GetAvoidPosition(float idealDistance, float height = 1.5f)
    {
        float enemyX = transform.position.x;
        float playerX = targetPlayer.transform.position.x;
        float borderL = BattleStageManager.Instance.mapBorderL;
        float borderR = BattleStageManager.Instance.mapBorderR;
        bool isPlayerOnRight = playerX > enemyX;
        float targetX = 0;
        bool foundPos = false;
        // 1. 优先尝试向远离玩家的方向移动（X轴逻辑不变）
        float retreatDir = isPlayerOnRight ? -1f : 1f;
        float desiredPosX = playerX + retreatDir * idealDistance;
        if (desiredPosX >= borderL && desiredPosX <= borderR)
        {
            targetX = desiredPosX;
            foundPos = true;
        }
        // 2. 若远离方向不可行（撞墙），则尝试穿过玩家到另一侧
        if (!foundPos)
        {
            float advanceDir = isPlayerOnRight ? 1f : -1f;
            desiredPosX = playerX + advanceDir * idealDistance;
            targetX = Mathf.Clamp(desiredPosX, borderL, borderR);
        }
        // 3. 计算二维目标坐标
        float targetY = targetPlayer.RaycastedPosition().y + height;
        Vector2 targetPos = new Vector2(targetX, targetY);
        return targetPos;
    }
    
}
