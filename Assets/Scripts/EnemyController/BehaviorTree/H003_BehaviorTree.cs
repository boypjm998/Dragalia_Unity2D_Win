using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class H003_BehaviorTree : EnemyBehaviorManager
{
    [SerializeField] private GameObject uiDrasticForce;
    private bool drasticForceApplied = false;
    
    private EnemyControllerFlyingHigh enemyController;
    private EnemyMoveController_H003 enemyAttackManager;
    
    [SerializeField] private GameObject heavenlyWingsPrefab;
    private PartStatusManager _heavenlyWingsStatusManager;
    [SerializeField] private Collider2D heavenlyWingsCollider;
    
    private bool wingHasBroken = false;
    private bool justRecoveredFromBroken = false;
    
    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_H003>();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        OnBehaviorStart += AddUIToPlayers;
        OnBehaviorStart += InitPart;
        if(status is SpecialStatusManager specialStatusManager)
            specialStatusManager.OnRecoverFromBroken += () => justRecoveredFromBroken = true;
        GetBehavior();
    }

    protected override void DoAction(int state, int substate)
    {
        if(playerAlive == false)
            return;
        
        ParseAction(state, substate);
    }

    protected override void ParseAction(int state, int substate)
    {
        _currentPhase = _pattern.phasePattern[state];

        if (substate >= _currentPhase.action_list.Count)
        {
            substate = _currentPhase.loopStartPoint;
            _currentPhase.loopCount++;
        }

        _currentActionStage = _currentPhase.action_list[substate];

        var action_name = _currentActionStage.action_name;

        if (action_name == "null")
        {
            ActionEnd();
            return;
        }
        
        DragaliaEnemyActionTypes.H003 actionType = 
            (DragaliaEnemyActionTypes.H003) Enum.Parse(typeof(DragaliaEnemyActionTypes.H003), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.H003.Orbs:
            {
                int type = Convert.ToInt32(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_SummonOrbs(interval,type));
                break;
            }
            case DragaliaEnemyActionTypes.H003.Nihil:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Nihil(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H003.Mine:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Mine(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H003.Wave:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_WaveSweep(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H003.Around:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_AroundAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H003.Line:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ThreeLines(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H003.Pizza:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FanshapedAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H003.Suppression:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_StrengthSupression(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H003.WeakPoint:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_WeakPoint(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H003.Face:
            {
                int type = Convert.ToInt32(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_FaceSwap(type,interval));
                break;
            }
            case DragaliaEnemyActionTypes.H003.Infight:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Infight(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H003.Ruin:
            {
                int type = Convert.ToInt32(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_MandalaRage(interval,type));
                break;
            }
            case DragaliaEnemyActionTypes.H003.Thunder:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_MandalaRageSingle(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H003.BalancePrepare:
            {
                int type = Convert.ToInt32(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_PreparePunishment(type, interval));
                break;
            }
            case DragaliaEnemyActionTypes.H003.Balance:
            {
                int type = Convert.ToInt32(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_Punishment(type, interval));
                break;
            }
        }
        
        
    }

    private void InitPart()
    {
        OnBehaviorStart -= InitPart;
        StageCameraController.Instance.SetMinCameraSize(9);
        _heavenlyWingsStatusManager = Instantiate(heavenlyWingsPrefab, BattleStageManager.Instance.EnemyLayer.transform).
            GetComponent<PartStatusManager>();
        _heavenlyWingsStatusManager.Link(status);
        InvokePartEvent(_heavenlyWingsStatusManager.gameObject, 0,true);
        
        BattleStageManager.Instance.RemoveFieldAbility((int)BasicCalculation.EnemyAbility.FaceOfSadness);
        BattleStageManager.Instance.RemoveFieldAbility((int)BasicCalculation.EnemyAbility.FaceOfAnger);
        StageCameraController.Instance.SetMinCameraSize(10);
        
        status.OnHPDecrease += WingsDecreaseHPCheck;
        status.ImmuneToAllControlAffliction = true;
        
        
        DOVirtual.DelayedCall(0.01f, 
            () =>
            {
                _heavenlyWingsStatusManager.gameObject.SetActive(false);
                _heavenlyWingsStatusManager.FakeActive(true);
            },
            false);
        
        enemyAttackManager.FaceSwap(1);
        
        enemyController.SetHitSensor(false);
        
        DOVirtual.DelayedCall(1, () =>
        {
            enemyController.SetHitSensor(true);
        });
        

    }
    
    protected override void ExcutePhase()
    {
        // if (_heavenlyWingsStatusManager.currentHp<=0 && breakable && !wingHasBroken)
        // {
        //     wingHasBroken = true;
        //     
        //     if (currentMoveAction != null)
        //     {
        //         StopCoroutine(currentMoveAction);
        //         currentMoveAction = null;
        //     }
        //     if(currentAttackAction != null)
        //     {
        //         StopCoroutine(currentAttackAction);
        //         currentAttackAction = null;
        //     }
        //     if(currentAction != null)
        //     {
        //         StopCoroutine(currentAction);
        //         currentAction = null;
        //     }
        //     //ActionEnd();
        //     breakable = false;
        //     status.ImmuneToAllControlAffliction = true;
        //     isAction = true;
        //     currentAction = StartCoroutine(ACT_PartBreak());
        // }
        // else
        if (!isAction)
        {
            DoAction(state,substate);
            justRecoveredFromBroken = false;
        }
        
    }
    
    protected override void Update()
    {
        if(!_heavenlyWingsStatusManager)
            return;
        
        if (!wingHasBroken && _heavenlyWingsStatusManager.currentHp<=0 && breakable &&
            (status as SpecialStatusManager).broken == false)
        {
            wingHasBroken = true;
            
            if (currentMoveAction != null)
            {
                StopCoroutine(currentMoveAction);
                currentMoveAction = null;
            }
            if(currentAttackAction != null)
            {
                StopCoroutine(currentAttackAction);
                currentAttackAction = null;
            }
            if(currentAction != null)
            {
                StopCoroutine(currentAction);
                currentAction = null;
            }
            //ActionEnd();
            breakable = false;
            status.ImmuneToAllControlAffliction = true;
            isAction = true;
            currentAction = StartCoroutine(ACT_PartBreak());
        }
    }
    
    
    private IEnumerator ACT_PartBreak()
    {
        wingHasBroken = true;
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.PartBreak());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        if (justRecoveredFromBroken)
            controllAfflictionProtect = true;
        
        ActionEnd(false);
        _heavenlyWingsStatusManager.FakeActive(false);
        controllAfflictionProtect = false;
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
    }

    private IEnumerator ACT_SummonOrbs(float interval, int summonType)
    {
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action01(summonType));
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);

        breakable = true;
        yield return new WaitForSeconds(interval);

        ActionEnd();
        
    }
    
    private IEnumerator ACT_Nihil(float interval)
    {
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action02());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);

        breakable = true;
        yield return new WaitForSeconds(interval);

        ActionEnd();
        
    }
    
    private IEnumerator ACT_Mine(float interval)
    {
        ActionStart();
        //breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action03());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);

        //breakable = true;
        yield return new WaitForSeconds(interval);

        ActionEnd();
        
    }
    
    private IEnumerator ACT_WaveSweep(float interval)
    {
        ActionStart();
        //breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action04());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);

        //breakable = true;
        yield return new WaitForSeconds(interval);

        ActionEnd();
        
    }
    
    private IEnumerator ACT_AroundAttack(float interval)
    {
        ActionStart();
        //breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action05());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);

        //breakable = true;
        yield return new WaitForSeconds(interval);

        ActionEnd();
        
    }

    private IEnumerator ACT_ThreeLines(float interval)
    {
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action06());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_FanshapedAttack(float interval)
    {
        ActionStart();
        //breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action07());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);

        //breakable = true;
        yield return new WaitForSeconds(interval);

        ActionEnd();
        
    }
    
    private IEnumerator ACT_StrengthSupression(float interval)
    {
        ActionStart();
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action08());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_WeakPoint(float interval)
    {
        ActionStart();
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action09());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        //breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_FaceSwap(int face, float interval)
    {
        ActionStart();
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action10(face));
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_Infight(float interval)
    {
        ActionStart();
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action11());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_MandalaRage(float interval, int type)
    {
        breakable = false;
        ActionStart();

        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action12(type == 2));
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }


    private IEnumerator ACT_MandalaRageSingle(float interval)
    {
        ActionStart();
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action13());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_PreparePunishment(int type, float interval)
    {
        ActionStart();
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action15(type));
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_Punishment(int type, float interval)
    {
        ActionStart();
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H003_Action16(type));
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    private void AddUIToPlayers()
    {
        if(DrasticForce.Instance == null)
            BattleStageManager.Instance.gameObject.AddComponent<DrasticForce>();

        DrasticForce.Instance.SetDuration(60);
        DrasticForce.Instance.MaxCount = 15;
        
        var player = BattleStageManager.Instance.GetPlayer();
        
        var buffLayer = player.transform.Find("BuffLayer");

        if (buffLayer.GetComponentInChildren<UI_DrasticForce>() == null)
        {
            Instantiate(uiDrasticForce,buffLayer.transform.position, Quaternion.identity,
                buffLayer.transform);
        }
        
        
        AddDrasticForceEffect();
        
    }

    private void AddDrasticForceEffect()
    {
        enemyAttackManager.AddDrasticForceEffectToStatusManager(status);
        drasticForceApplied = true;
    }
    
    private void WingsDecreaseHPCheck(int dmg, AttackBase atk)
    {
        if(_heavenlyWingsStatusManager.IsFakeActive == false)
            return;
        
        if(atk == null)
            return;
        
        var atkFromPlayer = atk as AttackFromPlayer;

        if (atkFromPlayer is BulletFromPlayer || atkFromPlayer is HomingProjectile)
        {
            if (atkFromPlayer.transform.position.y <= heavenlyWingsCollider.bounds.max.y)
            {
                print("Wings Hit");
                _heavenlyWingsStatusManager.currentHp -= dmg;
                _heavenlyWingsStatusManager.OnHPDecrease?.Invoke(dmg, atk);
                _heavenlyWingsStatusManager.OnHPChange?.Invoke();
            }
        }
        else if (atkFromPlayer is ForcedAttackFromPlayer)
        {
            _heavenlyWingsStatusManager.currentHp -= dmg;
            _heavenlyWingsStatusManager.OnHPDecrease?.Invoke(dmg, atk);
            _heavenlyWingsStatusManager.OnHPChange?.Invoke();
        }
        else
        {
            if (atkFromPlayer.attackCollider)
            {
                var hitBoxCol = atkFromPlayer.attackCollider;
                if (hitBoxCol.bounds.max.x >= heavenlyWingsCollider.bounds.min.x &&
                    hitBoxCol.bounds.min.x <= heavenlyWingsCollider.bounds.max.x &&
                    hitBoxCol.bounds.max.y >= heavenlyWingsCollider.bounds.min.y &&
                    hitBoxCol.bounds.min.y <= heavenlyWingsCollider.bounds.max.y)
                {
                    _heavenlyWingsStatusManager.currentHp -= dmg;
                    _heavenlyWingsStatusManager.OnHPDecrease?.Invoke(dmg, atk);
                    _heavenlyWingsStatusManager.OnHPChange?.Invoke();
                }else if (atkFromPlayer.transform.position.x >= heavenlyWingsCollider.bounds.min.x &&
                          atkFromPlayer.transform.position.x <= heavenlyWingsCollider.bounds.max.x &&
                          atkFromPlayer.transform.position.y >= heavenlyWingsCollider.bounds.min.y &&
                          atkFromPlayer.transform.position.y <= heavenlyWingsCollider.bounds.max.y)
                {
                    _heavenlyWingsStatusManager.currentHp -= dmg;
                    _heavenlyWingsStatusManager.OnHPDecrease?.Invoke(dmg, atk);
                    _heavenlyWingsStatusManager.OnHPChange?.Invoke();
                }
            }else print("No Collider");
        }
        
        
        /*else if (atkFromPlayer.attackCollider != null)
        {
            var filter = new ContactFilter2D();
            filter.useTriggers = true;
            filter.SetLayerMask(LayerMask.GetMask("AttackPlayer","Enemies"));
            List<Collider2D> collider2Ds = new();
            var col = Physics2D.OverlapCollider(bookCollider, filter, collider2Ds);

            if (collider2Ds.Contains(atkFromPlayer.attackCollider))
            {
                print("Book Hit");
                _bookStatusManager.currentHp -= dmg;
                _bookStatusManager.OnHPDecrease?.Invoke(dmg, atk);
                _bookStatusManager.OnHPChange?.Invoke();
            }
        }*/
        
        
        
        
    }
    
}
