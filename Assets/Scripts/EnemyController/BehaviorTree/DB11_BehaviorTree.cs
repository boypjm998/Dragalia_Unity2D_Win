using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
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
        
        DragaliaEnemyActionTypes.DB2011 actionType = 
            (DragaliaEnemyActionTypes.DB2011) Enum.Parse(typeof(DragaliaEnemyActionTypes.DB2011), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.DB2011.summon_monster:
            {
                int buffAmount = 10;
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[^1]);
                object[] messageArray = new object[_currentActionStage.args.Length - 1];
                Array.Copy(_currentActionStage.args,
                    0, messageArray, 
                    0, _currentActionStage.args.Length - 1);
                if (enemyAttackManager.minonCount > 8)
                {
                    currentAction = StartCoroutine(ACT_BuffChildren(buffAmount, interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_SummonChildren(messageArray,interval));
                }
                break;
            }
            case DragaliaEnemyActionTypes.DB2011.summon_soldier:
            {
                string enemyName = _currentActionStage.args[0];
                int hp = Convert.ToInt32(_currentActionStage.args[1]);
                int atk = Convert.ToInt32(_currentActionStage.args[2]);
                int buffAmount = Convert.ToInt32(_currentActionStage.args[3]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[4]);

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
            case DragaliaEnemyActionTypes.DB2011.multi_around:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length == 2)
                {
                    int type = Convert.ToInt32(_currentActionStage.args[1]);
                    currentAction = StartCoroutine(ACT_CycloneAroundBoosted(type,interval));
                }else{
                     currentAction = StartCoroutine(ACT_CycloneAround(interval));
                }
                break;
            }
            case DragaliaEnemyActionTypes.DB2011.around:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_AroundAttack(interval,
                        true));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_AroundAttack(interval));
                }
                break;
            }
            case DragaliaEnemyActionTypes.DB2011.buff:
            {
                
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float amount = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BuffSelf(amount, interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2011.charge:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                int maxHP = Convert.ToInt32(_currentActionStage.args[0]);
                if(_currentActionStage.args.Length > 2)
                    currentAction = StartCoroutine(ACT_HealingAreaOrdered(maxHP, interval));
                else
                    currentAction = StartCoroutine(ACT_HealingArea(maxHP, interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2011.corrosion:
            {
                float Amount = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float increment = 500;
                if (_currentActionStage.args.Length > 2)
                {
                    increment = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);
                }
                currentAction = StartCoroutine(ACT_DarkDiscipline(Amount, interval, increment));
                break;
            }
            case DragaliaEnemyActionTypes.DB2011.target_pillar:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FuneralLullaby(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2011.prison:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if(_currentActionStage.args.Length > 1)
                    currentAction = StartCoroutine(ACT_BoostedPrison(interval));
                else
                    currentAction = StartCoroutine(ACT_Prison(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2011.platform_splash:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_PlatformAOE(interval));
                break;
            }
        }
        
    }


    protected IEnumerator ACT_SummonChildren(object[] info, float interval)
    {
        ActionStart();
        
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        
        var timeNeeded = Vector2.Distance(transform.position,
            enemyAttackManager.GetAnchoredSensorOfName("MiddleM").transform.position) / enemyController.moveSpeed;
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint
            (enemyAttackManager.GetAnchoredSensorOfName("MiddleM").transform.position,
                timeNeeded,
                Ease.Linear));
        
        yield return _moveIsNull;
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action01T(info));
        
        yield return _attackIsNull;
        
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
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
    
    protected IEnumerator ACT_DarkDiscipline(float amount, float interval, float increment = 500)
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
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action05(amount,increment));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);

        if (enemyAttackManager.minonCount <= 0)
        {
            _currentActionStage.unbreakable = false;
        }
        ActionEnd(false);
        
    }
    
    protected IEnumerator ACT_FuneralLullaby(float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 12, 2f, 5));
        
                
        yield return _moveIsNull;
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action07());

        yield return _attackIsNull;

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);

        ActionEnd();
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


    protected IEnumerator ACT_BuffSelf(float amount, float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action10(amount));

        yield return _attackIsNull;

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

    protected IEnumerator ACT_HealingAreaOrdered(int hp, float interval)
    {
        ActionStart();
        
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        
        var timeNeeded = Vector2.Distance(transform.position,
            enemyAttackManager.GetAnchoredSensorOfName("MiddleM").transform.position) / enemyController.moveSpeed;
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint
            (enemyAttackManager.GetAnchoredSensorOfName("MiddleM").transform.position,
                timeNeeded,
                Ease.Linear));
        
        yield return _moveIsNull;
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action06T(hp));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        
        breakable = true;
        
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
    
    protected IEnumerator ACT_CycloneAroundBoosted(int type, float interval)
    {
        ActionStart();
        
        breakable = false;
        status.ImmuneToAllControlAffliction = true;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        var timeNeeded = Vector2.Distance(transform.position,
            enemyAttackManager.GetAnchoredSensorOfName("MiddleM").transform.position) / enemyController.moveSpeed;
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint
            (enemyAttackManager.GetAnchoredSensorOfName("MiddleM").transform.position,
                timeNeeded,
                Ease.Linear));
        
        yield return _moveIsNull;
                
        enemyController.SetKBRes(999);

        if (type == 1)
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action11A());
        }
        else
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action11B());
        }



        yield return _attackIsNull;

        enemyController.SetKBRes(status.knockbackRes);
        
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
        
    }

    protected IEnumerator ACT_AroundAttack(float interval, bool counterable=false)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 4, 3, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action04(counterable));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    protected IEnumerator ACT_Prison(float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        status.ImmuneToAllControlAffliction = true;
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 8, 1, 5));


        yield return _moveIsNull;
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action08());

        yield return _attackIsNull;

        enemyController.SetKBRes(status.knockbackRes);
        status.ImmuneToAllControlAffliction = false;
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

    protected IEnumerator ACT_BoostedPrison(float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        status.ImmuneToAllControlAffliction = true;
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action12());

        yield return _attackIsNull;

        enemyController.SetKBRes(status.knockbackRes);
        status.ImmuneToAllControlAffliction = false;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

    protected IEnumerator ACT_PlatformAOE(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB11_Action09());

        yield return _attackIsNull;

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }



}
