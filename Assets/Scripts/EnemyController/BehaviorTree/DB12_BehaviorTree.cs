using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class DB12_BehaviorTree : EnemyBehaviorManager
{
    private EnemyMoveController_DB12 enemyAttackManager;
    private EnemyControllerFlyingHigh enemyController;
    
    [SerializeField] private GameObject phase2Prefab;
    [SerializeField] private AudioClip phase2BGM;
    
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_DB12>();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        
        GetBehavior();

        if (enemyController.canDeath == false)
        {
            status.OnHPBelow0 += ToPhase2;
        }
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
        
        
        
        DragaliaEnemyActionTypes.DB2012 actionType = 
            (DragaliaEnemyActionTypes.DB2012) Enum.Parse(typeof(DragaliaEnemyActionTypes.DB2012), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.DB2012.corrosion:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float corrosionAmount = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CorrosionFog((int)corrosionAmount,interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2012.nihil:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Nihility(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2012.sphere:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float hp = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonSphere((int)hp, interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2012.chasing:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TargetingMine(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2012.buff:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float buffAmount = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BuffSelf(buffAmount,interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2012.spike:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SpikeAndAround(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2012.slap:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Slap(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2012.multi_dash:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DarkDive(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2012.pizza:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TwilightDance(interval));
                break;
            }
            case DragaliaEnemyActionTypes.DB2012.jalapeno:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_CrossFlames(interval,false));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_CrossFlames(interval));
                }
                break;
            }
                
        }
    }
    
    
    protected IEnumerator ACT_CorrosionFog(int corrosionAmount, float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 18, 2, 5));
        
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB12_Action01(corrosionAmount));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_Nihility(float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 18, 2, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB12_Action02());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_SummonSphere(int hp, float interval)
    {
        ActionStart();
        breakable = false;
        controllAfflictionProtect = true;
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB12_Action03(hp));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    
    protected IEnumerator ACT_TargetingMine(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 12, 2, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB12_Action04());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_BuffSelf(float buffAmount, float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB12_Action05(buffAmount));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_SpikeAndAround(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 12, 1, 5));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB12_Action06());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_Slap(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 4, 1, 5));

        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB12_Action07());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_DarkDive(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        var type = UnityEngine.Random.Range(0, 2);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 24, 1, 5));

        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        if(type == 0)
            currentAttackAction = StartCoroutine(enemyAttackManager.DB12_Action08B());
        else
            currentAttackAction = StartCoroutine(enemyAttackManager.DB12_Action08A());

        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.ResetGravityScale();

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_TwilightDance(float interval)
    {
        ActionStart();
        breakable = false;
        status.ImmuneToAllControlAffliction = true;
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
            (enemyController.FlyToPoint(new Vector2(0, 5), 1, Ease.InOutSine));

        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB12_Action09());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_CrossFlames(float interval, bool single = true)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 12, 1, 5));

        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB12_Action10(single));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ChangePhaseAnimationRoutine()
    {
        
        ActionStart();
        
        currentMoveAction = 
            StartCoroutine(enemyAttackManager.DB12_Action11());
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
        
        p2_boss.GetComponentInChildren<Animator>()?.Play("intro");
        p2_boss.GetComponentInChildren<VoiceControllerEnemy>()?.PlayIntroVoiceManually();

        DOVirtual.DelayedCall(0.3f, 
            () => CineMachineOperator.Instance.CamaraShake(15f, 0.4f));
        
        Destroy(gameObject);
        
    }

    private void ToPhase2()
    {
        status.OnHPBelow0 -= ToPhase2;
        
        enemyController.StopAllCoroutines();
        
        ResetBossActionsBeforeTransform();
        
        currentAction = StartCoroutine(ChangePhaseAnimationRoutine());
    }
    
}
