using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class DB13_BehaviorTree : EnemyBehaviorManager
{
    private EnemyMoveController_DB13 enemyAttackManager;
    private EnemyControllerFlyingHigh enemyController;
    
    [SerializeField] private GameObject uiDrasticForce;
    private bool drasticForceApplied = false;
    /// <summary>
    /// 0:Straight(a01);1:Combo(a03);2:Smash(a06);3:Mine(a04)
    /// </summary>
    private int _lastSkillUsed = 0;

    private List<int> _skillUsedTimes = new List<int>()
    {
        0,0,0,0
    };

    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_DB13>();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        OnBehaviorStart += AddUIToPlayers;
        if(drasticForceApplied == false)
            AddDrasticForceEffect();
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
        
        DragaliaEnemyActionTypes.DB2013 actionType = 
            (DragaliaEnemyActionTypes.DB2013) Enum.Parse(typeof(DragaliaEnemyActionTypes.DB2013), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.DB2013.combo:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Combo(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2013.free:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FreeAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2013.ground:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_GroundSmash(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2013.infight:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Infight(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2013.mine:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TargetingMine(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2013.nihil:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_NihilAOE(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2013.straight:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_StraightPunch(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2013.orbs:
            {
                int orbType = Convert.ToInt32(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_SummonOrbs(orbType,interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2013.orbs_tut:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonFirstOrbs(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2013.weak_point:
            {
                int hp = Convert.ToInt32(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_WeakPoint(hp,interval));
                break;
            }

        }
    }








    protected IEnumerator ACT_NihilAOE(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action01());

        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_FreeAttack(float interval)
    {
        ActionStart();
        
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt);

        var rand = UnityEngine.Random.Range(0, 100);
        
        //如果玩家距离较近
        if (Mathf.Abs(transform.position.x - targetPlayer.transform.position.x) < 4)
        {
            if ((rand < 50 && _skillUsedTimes[1] < 1) || _skillUsedTimes[2] >= 1)
            {
                _lastSkillUsed = 1;
                currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action03());
            }
            else
            {
                _lastSkillUsed = 2;
                currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action06());
            }
            ResetSkillUsedTime();
        }else if (Mathf.Abs(transform.position.x - targetPlayer.transform.position.x) < 10)
        {
            if((rand < 50 && _skillUsedTimes[2] < 1) || _skillUsedTimes[0] >= 1)
            {
                _lastSkillUsed = 2;
                currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action06());
            }
            else
            {
                _lastSkillUsed = 0;
                currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action02());
            }
            ResetSkillUsedTime();
        }
        else
        {
            if (_skillUsedTimes[0] >= 2)
            {
                _lastSkillUsed = 3;
                
                currentMoveAction = StartCoroutine
                (enemyController.FlyTowardTargetOnSamePlatform
                    (targetPlayer, 8, 1, 3));
        
                yield return _moveIsNull;
                
                currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action04());
                ResetSkillUsedTime(10);
            }
            else
            {
                _lastSkillUsed = 0;
                
                currentMoveAction = StartCoroutine
                (enemyController.FlyTowardTargetOnSamePlatform
                    (targetPlayer, 12, 2.5f, 3));
        
                yield return _moveIsNull;
        
                currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action02());
                
                ResetSkillUsedTime();
            }
        }
        
        
        
        
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        print("ActionEnd");
        
        ActionEnd();
        
        
    }
    
    protected IEnumerator ACT_StraightPunch(float interval)
    {
        ActionStart();
        
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 12, 2.5f, 3));
        
        yield return _moveIsNull;
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action02());

        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        print("ActionEnd");
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_Combo(float interval)
    {
        ActionStart();
        
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 4, 1f, 3));
        
        yield return _moveIsNull;
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action03());

        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        print("ActionEnd");
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_TargetingMine(float interval)
    {
        ActionStart();
        
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action04());

        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        //print("ActionEnd");
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_GroundSmash(float interval)
    {
        ActionStart();
        
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 6, 1f, 3));
        
        yield return _moveIsNull;

        currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action06());

        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        //print("ActionEnd");
        
        ActionEnd();
    }

    protected IEnumerator ACT_SummonOrbs(int type, float interval)
    {
        ActionStart();
        breakable = false;
        
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action05(type));

        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        breakable = true;
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_SummonFirstOrbs(float interval)
    {
        ActionStart();
        breakable = false;
        status.ImmuneToAllControlAffliction = true;
        
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        StageCameraController.SwitchMainCameraFollowObject(gameObject);

        currentMoveAction = StartCoroutine
            (enemyController.
                FlyToPoint(new Vector2(0, transform.position.y), 1, Ease.InOutSine));

        yield return _moveIsNull;

        currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action05F());

        yield return _attackIsNull;
        
        StageCameraController.SwitchMainCameraFollowObject(viewerPlayer);
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    protected IEnumerator ACT_WeakPoint(int hp, float interval)
    {
        ActionStart();
        breakable = false;
        status.ImmuneToAllControlAffliction = true;
        
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        //StageCameraController.SwitchMainCameraFollowObject(gameObject);

        currentMoveAction = StartCoroutine
        (enemyController.
            FlyToPoint(new Vector2(0, transform.position.y), 1, Ease.InOutSine));

        yield return _moveIsNull;

        currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action07(hp));

        yield return _attackIsNull;
        
        //StageCameraController.SwitchMainCameraFollowObject(viewerPlayer);
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

    protected IEnumerator ACT_Infight(float interval)
    {
        ActionStart();
        breakable = false;
        status.ImmuneToAllControlAffliction = true;
        
        SetTarget(ClosestTarget);
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        //StageCameraController.SwitchMainCameraFollowObject(gameObject);

        currentMoveAction = StartCoroutine
        (enemyController.
            FlyToPoint(new Vector2(0, transform.position.y), 1, Ease.InOutSine));

        yield return _moveIsNull;

        currentAttackAction = StartCoroutine(enemyAttackManager.DB13_Action09());

        yield return _attackIsNull;
        
        //StageCameraController.SwitchMainCameraFollowObject(viewerPlayer);
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

    private void AddUIToPlayers()
    {
        if(DrasticForce.Instance == null)
            BattleStageManager.Instance.gameObject.AddComponent<DrasticForce>();
        
        var player = BattleStageManager.Instance.GetPlayer();
        
        var buffLayer = player.transform.Find("BuffLayer");

        if (buffLayer.GetComponentInChildren<UI_DrasticForce>() == null)
        {
            Instantiate(uiDrasticForce,buffLayer.transform.position, Quaternion.identity,
                buffLayer.transform);
        }
        
    }

    private void AddDrasticForceEffect()
    {
        enemyAttackManager.AddDrasticForceEffectToStatusManager(status);
        drasticForceApplied = true;
    }

    private void ResetSkillUsedTime(int increment = 1, int decrement = 3)
    {
        for (int i = 0; i < _skillUsedTimes.Count; i++)
        {
            if (i != _lastSkillUsed)
            {
                _skillUsedTimes[i] = Mathf.Max(_skillUsedTimes[i] -decrement,0);
            }
            else
            {
                _skillUsedTimes[i] += increment;
            }
        }
    }

    private void ClearAllSkillUsed()
    {
        for (int i = 0; i < _skillUsedTimes.Count; i++)
        {
            _skillUsedTimes[i] = 0;
        }
    }
    
}
