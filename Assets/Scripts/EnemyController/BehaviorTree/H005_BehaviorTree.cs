using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class H005_BehaviorTree : EnemyBehaviorManager
{
    private EnemyControllerFlyingHigh enemyController;
    private EnemyMoveController_H005 enemyAttackManager;

    public const int BurningOn = 20231;
    public const int PhoenixOn = 20221;
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyAttackManager = GetComponent<EnemyMoveController_H005>();
        GetBehavior();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        OnBehaviorStart += InitCameraAndAbilities;
    }

    private void InitCameraAndAbilities()
    {
        StageCameraController.Instance.SetMinCameraSize(10);
        
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(BurningOn,
            new EnemyAbilityIconEvent(0),status);
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(PhoenixOn,
            new EnemyAbilityIconEvent(0),status);

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
        
        DragaliaEnemyActionTypes.H005 actionType = 
            (DragaliaEnemyActionTypes.H005) Enum.Parse(typeof(DragaliaEnemyActionTypes.H005), action_name);


        switch (actionType)
        {
            case DragaliaEnemyActionTypes.H005.Nihil:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Nihil(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.Corrosion:
            {
                float amount = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_NihilCorrosion(amount, interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.BurningOn:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BurningOn(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.PhoenixOn:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_PhoenixOn(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.Wave:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length == 1)
                {
                    currentAction = StartCoroutine(ACT_FireWave(false,interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_FireWave(true,interval));
                }
                
                break;
            }
            case DragaliaEnemyActionTypes.H005.TargetingCrystal:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TargetingCrystal(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.Cross:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_FireCross(interval, true));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_FireCross(interval));
                }
                
                break;
            }
            case DragaliaEnemyActionTypes.H005.Chaser:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FireChaser(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.Refraction:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FireRefraction(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.MixedCrystal:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ConflagrantQuartz(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.StoneGroup:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CrystallineEmbers(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.LavaCarpet:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_LavaCarpet(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.LavaTsunami:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FlameSea(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.Explosion:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Combustion(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.CrystalInferno:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CrystalInferno(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.MovingBlocks:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_MovingBlocks(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H005.CrystalBreaker:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CrystalBreaker(interval));
                break;
            }
                
            
        }
        
        
        
    }

    private IEnumerator ACT_Nihil(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action01());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_NihilCorrosion(float amount, float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action02(amount));

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_FireWave(bool around, float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.MoveTowardTargetWithoutFlying
            (targetPlayer, 3, 25, 30));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action05(around));

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_TargetingCrystal(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action06());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_BurningOn(float interval)
    {
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action03());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_PhoenixOn(float interval)
    {
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action04());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_FireCross(float interval, bool crossFirst = false)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        if (!crossFirst)
        {
            currentMoveAction = StartCoroutine
            (enemyController.MoveTowardTargetWithoutFlying
                (targetPlayer, 3, 15, 20));
        
            yield return new WaitUntil(()=>currentMoveAction == null);
        }

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action07(crossFirst));

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    private IEnumerator ACT_FireChaser(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action08());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_FireRefraction(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action09());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_ConflagrantQuartz(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.MoveTowardTargetWithoutFlying
            (enemyAttackManager.GetAnchoredSensorOfName("MiddleM"),
                4, 0.5f, 1));
        
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action10());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_CrystallineEmbers(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.MoveTowardTargetWithoutFlying
        (enemyAttackManager.GetAnchoredSensorOfName("MiddleM"),
            4, 0.5f, 1));
        
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action11());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_LavaCarpet(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action12());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_FlameSea(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.MoveTowardTargetWithoutFlying
        (enemyAttackManager.GetAnchoredSensorOfName("MiddleM"),
            4, 0.5f, 1));
        
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action13());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_Combustion(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action14());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_CrystalInferno(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.MoveTowardTargetWithoutFlying
        (enemyAttackManager.GetAnchoredSensorOfName("MiddleM"),
            4, 0.5f, 1));
        
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action15());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    private IEnumerator ACT_MovingBlocks(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action16());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_CrystalBreaker(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.MoveTowardTargetWithoutFlying
        (enemyAttackManager.GetAnchoredSensorOfName("MiddleM"),
            4, 0.5f, 1));
        
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H005_Action17());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
}
