using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class H004_BehaviorTree : EnemyBehaviorManager
{
    private EnemyControllerFlyingHigh enemyController;
    private EnemyMoveController_H004 enemyAttackManager;
    
    [SerializeField] private GameObject capePrefab;
    private PartStatusManager _capeStatusManager;
    [SerializeField] private Collider2D capeCollider;
    private bool capeHasBroken = false;
    private bool justRecoveredFromBroken = false;
    
    public bool CapeHasBroken => capeHasBroken;
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_H004>();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        if(status is SpecialStatusManager specialStatusManager)
            specialStatusManager.OnRecoverFromBroken += () => justRecoveredFromBroken = true;
        OnBehaviorStart += InitPart;
        GetBehavior();
    }

    protected override bool CustomJumpActionConditionCheck(string[] args)
    {
        if (args[0] == "part_break")
        {
            return CapeHasBroken;
        }

        return false;
    }

    protected override void ExcutePhase()
    {
        if (!isAction)
        {
            DoAction(state,substate);
            justRecoveredFromBroken = false;
        }
    }

    protected override void Update()
    {
        if(!_capeStatusManager)
            return;
        
        if (!capeHasBroken && _capeStatusManager.currentHp<=0 && breakable &&
            (status as SpecialStatusManager).broken == false)
        {
            capeHasBroken = true;
            
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
    
    private void InitPart()
    {
        breakable = false;
        OnBehaviorStart -= InitPart;
        StageCameraController.Instance.SetMinCameraSize(9);
        _capeStatusManager = Instantiate(capePrefab, BattleStageManager.Instance.EnemyLayer.transform).
            GetComponent<PartStatusManager>();
        _capeStatusManager.Link(status);
        InvokePartEvent(_capeStatusManager.gameObject, 0,true);
        
        BattleStageManager.Instance.RemoveFieldAbility((int)BasicCalculation.EnemyAbility.Dissonance);
        BattleStageManager.Instance.RemoveFieldAbility((int)BasicCalculation.EnemyAbility.MelodyHeaven);
        BattleStageManager.Instance.RemoveFieldAbility((int)BasicCalculation.EnemyAbility.MelodyHell);

        status.OnHPDecrease += PartDecreaseHPCheck;
        status.ImmuneToAllControlAffliction = true;
        
        
        DOVirtual.DelayedCall(0.01f, 
            () =>
            {
                _capeStatusManager.gameObject.SetActive(false);
                _capeStatusManager.FakeActive(true);
            },
            false);
        
        enemyController.SetHitSensor(false);
        
        DOVirtual.DelayedCall(1, () =>
        {
            enemyController.SetHitSensor(true);
        });
        
    }

    private void PartDecreaseHPCheck(int dmg, AttackBase atk)
    {
        if(_capeStatusManager.IsFakeActive == false)
            return;
        
        if(atk == null)
            return;
        
        var atkFromPlayer = atk as AttackFromPlayer;

        if (atkFromPlayer is BulletFromPlayer || atkFromPlayer is HomingProjectile)
        {
            if (atkFromPlayer.firedir * enemyController.facedir >= 0)
            {
                print("Part Hit");
                _capeStatusManager.currentHp -= dmg;
                _capeStatusManager.OnHPDecrease?.Invoke(dmg, atk);
                _capeStatusManager.OnHPChange?.Invoke();
            }
        }
        else if (atkFromPlayer is ForcedAttackFromPlayer)
        {
            _capeStatusManager.currentHp -= dmg;
            _capeStatusManager.OnHPDecrease?.Invoke(dmg, atk);
            _capeStatusManager.OnHPChange?.Invoke();
        }
        else
        {
            if (atkFromPlayer.attackCollider)
            {
                var hitBoxCol = atkFromPlayer.attackCollider;
                if (hitBoxCol.bounds.max.x >= capeCollider.bounds.min.x &&
                    hitBoxCol.bounds.min.x <= capeCollider.bounds.max.x)
                {
                    print("Cape Hit");
                    _capeStatusManager.currentHp -= dmg;
                    _capeStatusManager.OnHPDecrease?.Invoke(dmg, atk);
                    _capeStatusManager.OnHPChange?.Invoke();
                }else print("No Hit");
            }else print("No Collider");
        }
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
        
        DragaliaEnemyActionTypes.H004 actionType = 
            (DragaliaEnemyActionTypes.H004) Enum.Parse(typeof(DragaliaEnemyActionTypes.H004), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.H004.Melody:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Melody(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.Nihil:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Nihil(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.Corrosion:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Corrosion(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.Forward:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Forward(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.Rings:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_Rings(interval,false));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_Rings(interval, true));
                }
                break;
            }
            case DragaliaEnemyActionTypes.H004.Encore:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_Encore(interval, true));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_Encore(interval,false));
                }

                break;
            }
            case DragaliaEnemyActionTypes.H004.Buff:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Buff(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.Scatter:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_ScatteredWaterball(interval, 
                        true));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_ScatteredWaterball(interval));
                }

                break;
            }
            case DragaliaEnemyActionTypes.H004.Shuffle:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Shuffle(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.DpsCheck:
            {
                int minionHp = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_DpsCheck(interval, minionHp));
                break;
            }
            case DragaliaEnemyActionTypes.H004.Follow:
            {
                //int minionHp = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Follow(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.Echo:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Echo(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.MultiWay:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SixWays(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.Targeting:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Targeting(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.WarpAttack:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_WarpAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.Teleport:
            {
                string anchorName = _currentActionStage.args[0];
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_Teleport(anchorName,interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.Dissonance:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Dissonance(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.Fan:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FanAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H004.Cross:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CrossBats(interval));
                break;
            }
            
        }
        
        
    }

    private IEnumerator ACT_Melody(float interval)
    {
        ActionStart();
        breakable = false;
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action01());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        breakable = true;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    private IEnumerator ACT_Nihil(float interval)
    {
        ActionStart();
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action02());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        //breakable = true;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    private IEnumerator ACT_Corrosion(float interval)
    {
        ActionStart();
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action03());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        //breakable = true;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    private IEnumerator ACT_Rings(float interval, bool teleport)
    {
        ActionStart();
        breakable = false;
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action05(teleport));
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        breakable = true;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    private IEnumerator ACT_Encore(float interval, bool encore)
    {
        ActionStart();
        breakable = false;
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action06(encore));
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 35, 0.5f, 2));
        
        yield return new WaitUntil(()=>currentMoveAction == null);
        
        breakable = true;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    private IEnumerator ACT_Buff(float interval)
    {
        ActionStart();
        //breakable = false;
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action07());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        //breakable = true;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    private IEnumerator ACT_Forward(float interval)
    {
        ActionStart();
        
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 6, 0.5f, 4));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action04());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    private IEnumerator ACT_PartBreak()
    {
        capeHasBroken = true;
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.PartBreak());
        yield return new WaitUntil(()=>currentAttackAction == null);

        if (justRecoveredFromBroken)
            controllAfflictionProtect = true;
        
        ActionEnd(false);
        
        _capeStatusManager.FakeActive(false);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        controllAfflictionProtect = false;
        
        status.ImmuneToAllControlAffliction = false;
    }
    
    private IEnumerator ACT_ScatteredWaterball(float interval, bool moveToCenter = false)
    {
        ActionStart();
        
        
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        var height = BattleStageManager.Instance.mapBorderB + 7;
        var posX = moveToCenter ? 0 : transform.position.x;

        currentMoveAction = StartCoroutine(enemyController.FlyToPoint
        (new Vector2(posX, height),
            Vector2.Distance(transform.position,
                new Vector2(posX, height))/20, Ease.InOutSine
        ));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action08());

        yield return new WaitUntil(()=>currentAttackAction == null);

        yield return new WaitForSeconds(0.4f);

        var raycastedPosY = gameObject.RaycastedPosition().y;
        var raycastedHeight = transform.position.y - raycastedPosY;

        if (raycastedHeight > 4)
        {
            currentMoveAction = StartCoroutine
            (enemyController.FlyTowardTargetOnSamePlatform
                (targetPlayer, 99, 0.5f, 4));
            yield return new WaitUntil(()=>currentMoveAction == null);
        }
        
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_SixWays(float interval)
    {
        ActionStart();
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action13());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        //breakable = true;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    private IEnumerator ACT_Shuffle(float interval)
    {
        breakable = false;
        ActionStart();
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action09());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        breakable = true;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    private IEnumerator ACT_DpsCheck(float interval,int hp)
    {
        ActionStart();
        breakable = false;
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action10(hp));
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        breakable = true;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    private IEnumerator ACT_Follow(float interval)
    {
        ActionStart();
        breakable = false;
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action11());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        breakable = true;

        yield return null;

        ActionEnd();
    }
    
    private IEnumerator ACT_Echo(float interval)
    {
        ActionStart();
        breakable = false;
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action12());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        breakable = true;
        
        yield return new WaitForSeconds(interval);

        yield return null;

        ActionEnd();
    }
    
    private IEnumerator ACT_Targeting(float interval)
    {
        ActionStart();
        //breakable = false;
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action14());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        //breakable = true;
        
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

    private IEnumerator ACT_WarpAttack(float interval)
    {
        ActionStart();
        //breakable = false;
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action15());
        
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 35, 0.5f, 1));

        yield return _moveIsNull;
        
        
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    private IEnumerator ACT_Teleport(string anchorName, float interval)
    {
        ActionStart();
        //breakable = false;
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action16(anchorName));
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        //breakable = true;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    private IEnumerator ACT_Dissonance(float interval)
    {
        ActionStart();
        breakable = false;
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action17());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        breakable = true;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    private IEnumerator ACT_FanAttack(float interval)
    {
        ActionStart();
        
        
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 5, 0.5f, 4));
        
                
        yield return new WaitUntil(()=>currentMoveAction == null);
                
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action18());

        yield return new WaitUntil(()=>currentAttackAction == null);
        

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    private IEnumerator ACT_CrossBats(float interval)
    {
        ActionStart();
        

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H004_Action19());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        //breakable = true;
        
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    
}