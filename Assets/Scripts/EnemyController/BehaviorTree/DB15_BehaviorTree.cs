using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

/// <summary>
/// 堕天使米迦勒
/// </summary>
public class DB15_BehaviorTree : EnemyBehaviorManager
{
    
    private EnemyControllerFlyingHigh enemyController;
    private EnemyMoveController_DB15 enemyAttackManager;
    
    [SerializeField] private GameObject phase2Prefab;
    [SerializeField] private AudioClip phase2BGM;
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyAttackManager = GetComponent<EnemyMoveController_DB15>();
        GetBehavior();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        
        if (enemyController.canDeath == false)
        {
            status.OnHPBelow0 += ToPhase2;
        }
    }
    
    protected override void DoAction(int state, int substate)
    {
        if (playerAlive == false)
            return;
        
        _currentPhase = _pattern.phasePattern[state];
        _currentActionStage = _currentPhase.action_list[substate];

        var action_name = _currentActionStage.action_name;

        if (action_name == "null")
        {
            ActionEnd();
            return;
        }
        
        DragaliaEnemyActionTypes.DB2015 actionType = 
            (DragaliaEnemyActionTypes.DB2015) Enum.Parse(typeof(DragaliaEnemyActionTypes.DB2015), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.DB2015.around:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_AroundAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.crystal_chase:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CrystalChase(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.crystal_fixed:
            {
                if (_currentActionStage.args.Length == 1)
                {
                    float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                    currentAction = StartCoroutine(ACT_CrystalFixed(interval));
                }
                else
                {
                    float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[^1]);
                    float[] arr = new float[_currentActionStage.args.Length - 1];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[i]);
                    }
                    currentAction = StartCoroutine(ACT_CrystalFixedMultiple(arr, interval));
                }
                
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.crystal_mixed:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                int type = int.Parse(_currentActionStage.args[0]);
                if (type == 1)
                {
                    currentAction = StartCoroutine(ACT_AllRangedAttackI(interval));
                }
                else if (type == 2)
                {
                    currentAction = StartCoroutine(ACT_AllRangedAttackII(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_AimingAttack(interval));
                }

                break;


            }
            case DragaliaEnemyActionTypes.DB2015.nihil:
            {
                if (_currentActionStage.args.Length == 1)
                {
                    float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                    currentAction = StartCoroutine(ACT_Nihil(interval));
                }
                else
                {
                    int effect = int.Parse(_currentActionStage.args[0]);
                    float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                    currentAction = StartCoroutine(ACT_NihilCorrosion(effect,interval)); 
                }
                
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.combo:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Combo(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.cross:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FlameCrossing(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.buff:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DefenseBuff(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.fireball:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BouncingFireball(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.wave:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_WaveAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2015.explosion:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ClearingExpolosion(interval));
                break;
            }

            default: break;

        }
        
        
        
    }
    
    protected IEnumerator ACT_CrystalChase(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        
                
        enemyController.SetKBRes(999);

        if (difficulty > 1)
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action01V());
        }
        else
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action01());
        }

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_CrystalFixed(float interval)
    {
        SetTarget(ClosestTarget);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 5, 2, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action02());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_CrystalFixedMultiple(float[] info,float interval)
    {
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);


        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action02(info));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_Nihil(float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action03());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_NihilCorrosion(float eff, float interval)
    {
        SetTarget(viewerPlayer);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action03V(eff));

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_Combo(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 4, 2, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action04());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_DefenseBuff(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 12, 2, 3));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action05());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_BouncingFireball(float interval)
    {
        ActionStart();
        SetTarget(FurthestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 6, 2, 3));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action06());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_WaveAttack(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 35, 1, 3));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action07());

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
            (targetPlayer, 3, 1, 4));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action08());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_AllRangedAttackI(float interval)
    {
        SetTarget(viewerPlayer);
        breakable = false;
        status.ImmuneToAllControlAffliction = true;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action09());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_AllRangedAttackII(float interval)
    {
        SetTarget(viewerPlayer);
        status.ImmuneToAllControlAffliction = true;
        breakable = false;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action10());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_AimingAttack(float interval)
    {
        SetTarget(viewerPlayer);
        breakable = false;
        status.ImmuneToAllControlAffliction = true;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action11());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_ClearingExpolosion(float interval)
    {
        SetTarget(viewerPlayer);
        breakable = false;
        status.ImmuneToAllControlAffliction = true;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action13());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_FlameCrossing(float interval)
    {
        
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 8, 1, 4));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB15_Action12());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private void ToPhase2()
    {
        status.OnHPBelow0 -= ToPhase2;
        
        enemyController.StopAllCoroutines();
        
        ResetBossActionsBeforeTransform();
        
        currentAction = StartCoroutine(ChangePhaseAnimationRoutine());
    }
    
    protected IEnumerator ChangePhaseAnimationRoutine()
    {
        
        ActionStart();
        
        currentMoveAction = 
            StartCoroutine(enemyAttackManager.DB15_Action14());
        yield return new WaitUntil(()=>currentMoveAction == null);
        
        
        var p2_boss = Instantiate(phase2Prefab,transform.position,Quaternion.identity,transform.parent);
        p2_boss.GetComponent<EnemyController>().TurnMove(targetPlayer);
        
        BattleStageManager.currentDisplayingBossInfo = 2;
        FindObjectOfType<UI_BossStatus>().RedirectBoss(p2_boss,1);
        p2_boss.GetComponent<StatusManager>()?.OnHPChange?.Invoke();
        BattleEffectManager.Instance.PlayBGM(false);
        AudioScheduleManager.Instance.StopTween();
        ActionEnd();
        
        yield return null;
        
        BattleEffectManager.Instance.SetBGM(phase2BGM);
        BattleEffectManager.Instance.PlayBGM(true);
        
        p2_boss.GetComponentInChildren<Animator>()?.Play("roar");
        p2_boss.GetComponentInChildren<VoiceControllerEnemy>()?.PlayIntroVoiceManually();

        DOVirtual.DelayedCall(1f, 
            () => CineMachineOperator.Instance.CamaraShake(15f, 0.4f));
        
        Destroy(gameObject);
        
    }

}
