using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class DB05_BehaviorTree : EnemyBehaviorManager
{
    EnemyMoveController_DB05 enemyAttackManager;
    EnemyControllerFlyingHigh enemyController;

    [SerializeField] private GameObject fxFlameUI;
    

    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_DB05>();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyAttackManager.OnAttackFinished += FinishAttack;
        enemyController.OnMoveFinished += FinishMove;
        OnBehaviorStart += SetFlameUI;
        //BattleStageManager.Instance.OnGameStart += SetFlameUI;
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
        
        DragaliaEnemyActionTypes.DB2005 actionType = 
            (DragaliaEnemyActionTypes.DB2005) Enum.Parse(typeof(DragaliaEnemyActionTypes.DB2005), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.DB2005.Around:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_AroundAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.Tail:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TailAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.Sprint:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_LongDash(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.PoisonSide:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_PoisonBreathSide(interval,false));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_PoisonBreathSide(interval));
                }
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.PoisonFront:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                int direction = int.Parse(_currentActionStage.args[0]) > 0 ? 1 : -1;
                if (direction > 0)
                {
                    currentAction = StartCoroutine(ACT_PoisonBreathRightCenter(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_PoisonBreathLeftCenter(interval));
                }
                
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.Chaser:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_UmbralChaser(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.Spit:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_PoisonSpit(interval,false));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_PoisonSpit(interval));
                }
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.CursedFlame:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_SummonFlamesOneByOne(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_SummonFlames(interval));
                }
                
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.Blast:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ShadowBlast(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.GroundFire:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_GroundFire(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.Memories:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                int type = int.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Memories(interval,type));
                break;
            }
            case DragaliaEnemyActionTypes.DB2005.Summon:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                int type = int.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonShadows(interval,type));
                break;
            }
        }
    }

    private void SetFlameUI()
    {
        OnBehaviorStart -= SetFlameUI;
        var player = BattleStageManager.Instance.GetPlayer();
        var ui = Instantiate(fxFlameUI,player.transform.position,Quaternion.identity,
            player.transform);
    }
    
    private IEnumerator ACT_AroundAttack(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = 
            StartCoroutine(
                enemyController.MoveTowardTargetWithoutFlying(targetPlayer,
                    5,5,5.5f));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action01());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_TailAttack(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action02());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_LongDash(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = 
            StartCoroutine(
                enemyController.MoveTowardTargetWithoutFlying(targetPlayer,
                    5,18,25));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action03());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_PoisonBreathSide(float interval, bool move = true)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        if (move)
        {
            currentMoveAction = 
                StartCoroutine(
                    enemyController.MoveTowardTargetWithoutFlying(targetPlayer,
                        5,10,12f));

            yield return _moveIsNull;
        }

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action04());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    private IEnumerator ACT_PoisonBreathLeftCenter(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action08());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_PoisonBreathRightCenter(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action08(1));
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_UmbralChaser(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action05());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_PoisonSpit(float interval, bool move = true)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        if (move)
        {
            currentMoveAction = 
                StartCoroutine(
                    enemyController.MoveTowardTargetWithoutFlying(targetPlayer,
                        5,8,12f));

            yield return _moveIsNull;
        }

        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action06());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_SummonFlames(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = 
            StartCoroutine(
                enemyController.MoveTowardTargetWithoutFlying(enemyAttackManager.GetAnchor(0),
                    5,1f,1f));

        yield return _moveIsNull;

        enemyAttackManager.DB05_Action07();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    private IEnumerator ACT_SummonFlamesOneByOne(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = 
            StartCoroutine(
                enemyController.MoveTowardTargetWithoutFlying(enemyAttackManager.GetAnchor(0),
                    5,1f,1f));

        yield return _moveIsNull;

        enemyAttackManager.DB05_Action07V();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    private IEnumerator ACT_ShadowBlast(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        
        enemyController.SetKBRes(999);
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.DB05_Action09());
        
        yield return _attackIsNull;
        enemyController.ResetKBRes();
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    private IEnumerator ACT_GroundFire(float interval)
    {
        ActionStart();

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.DB05_Action10());

        yield return _attackIsNull;
        enemyController.ResetKBRes();

        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

    private IEnumerator ACT_Memories(float interval, int type)
    {
        ActionStart();
        breakable = false;

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);

        if (type == 1)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.DB05_Action11());
        }else if (type == 2)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.DB05_Action12());
        }
        else if (type == 3)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.DB05_Action13());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.DB05_Action14());
        }

        yield return _attackIsNull;
        enemyController.ResetKBRes();
        breakable = true;

        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

    private IEnumerator ACT_SummonShadows(float interval, int type)
    {
        ActionStart();
        //breakable = false;

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);

        if (type == 1)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.DB05_Action15());
        }
        else if (type == 2)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.DB05_Action16());
        }
        else if (type == 3)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.DB05_Action17());
        }
        else if (type == 4)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.DB05_Action18());
        }
        else if (type == 5)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.DB05_Action19());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.DB05_Action20());
        }

        yield return _attackIsNull;
        enemyController.ResetKBRes();
        breakable = true;

        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

}
