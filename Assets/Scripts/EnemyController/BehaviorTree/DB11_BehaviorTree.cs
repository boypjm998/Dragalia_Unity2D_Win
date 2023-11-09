using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB11_BehaviorTree : EnemyBehaviorManager
{
    protected EnemyMoveController_DB11 enemyAttackManager;
    protected EnemyControllerFlyingHigh enemyController;

    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_DB11>();
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
        
        DragaliaBossActionTypes.DB2011 actionType = 
            (DragaliaBossActionTypes.DB2011) Enum.Parse(typeof(DragaliaBossActionTypes.DB2011), action_name);

        switch (actionType)
        {
            case DragaliaBossActionTypes.DB2011.summon_soldier:
            {
                string enemyName = _currentActionStage.args[0];
                int hp = Convert.ToInt32(_currentActionStage.args[1]);
                int atk = Convert.ToInt32(_currentActionStage.args[2]);
                int buffAmount = Convert.ToInt32(_currentActionStage.args[3]);
                float interval = float.Parse(_currentActionStage.args[4]);

                if (enemyAttackManager.minonCount > 4)
                {
                    currentAction = StartCoroutine(ACT_BuffChildren(buffAmount, interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_SummonChildren(enemyName, hp, atk, interval));
                }

                break;
            }
            case DragaliaBossActionTypes.DB2011.multi_around:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CycloneAround(interval));
                break;
            }
            case DragaliaBossActionTypes.DB2011.around:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_AroundAttack(interval));
                break;
            }
            case DragaliaBossActionTypes.DB2011.charge:
            {
                float interval = float.Parse(_currentActionStage.args[1]);
                int maxHP = Convert.ToInt32(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_HealingArea(maxHP, interval));
                break;
            }
            case DragaliaBossActionTypes.DB2011.corrosion:
            {
                float Amount = float.Parse(_currentActionStage.args[0]);
                float interval = float.Parse(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_DarkDiscipline(Amount, interval));
                break;
            }
        }
        
    }


    protected IEnumerator ACT_SummonChildren(string name,int hp, int atk,float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 10, 2, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action01(hp,atk,name));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    protected IEnumerator ACT_BuffChildren(int buffAmount,float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 10, 2, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action02(buffAmount));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    protected IEnumerator ACT_DarkDiscipline(float amount, float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        if (enemyAttackManager.minonCount > 0)
        {
            _currentActionStage.unbreakable = true;
        }

        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 15, 2, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action05(amount));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);

        if (enemyAttackManager.minonCount <= 0)
        {
            _currentActionStage.unbreakable = false;
        }
        ActionEnd(false);
        
    }

    protected IEnumerator ACT_HealingArea(int hp, float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 8, 3, 5));
        
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action06(hp));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

    protected IEnumerator ACT_CycloneAround(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 6, 3, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action03());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

    protected IEnumerator ACT_AroundAttack(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 4, 3, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action04());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }



}
