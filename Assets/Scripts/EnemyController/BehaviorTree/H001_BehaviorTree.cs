using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class H001_BehaviorTree : EnemyBehaviorManager
{
    private EnemyControllerFlyingHigh enemyController;
    private EnemyMoveController_H001 enemyAttackManager;
    private PartStatusManager _bookStatusManager;
    private bool _isBookActive = false;

    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private Collider2D bookCollider;

    public int destructionCount;
    private const int DestructionBook = 20211;
    private const int GenesisBook = 20201;

    public bool bookHasBroken;
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_H001>();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        OnBehaviorStart += InitPart;
        GetBehavior();
    }

    protected override void ExcutePhase()
    {
        if (_bookStatusManager.currentHp<=0 && breakable && !bookHasBroken)
        {
            bookHasBroken = true;
            
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
        else if (!isAction)
        {
            DoAction(state,substate);
        }
        
    }

    protected override bool CustomJumpActionConditionCheck(string[] args)
    {
        if (args[0] == "part_break")
        {
            if (bookHasBroken)
            {
                return true;
            }
        }

        return false;
    }

    private void InitPart()
    {
        OnBehaviorStart -= InitPart;
        _bookStatusManager = Instantiate(bookPrefab, BattleStageManager.Instance.EnemyLayer.transform).
            GetComponent<PartStatusManager>();
        _bookStatusManager.Link(status);
        InvokePartEvent(_bookStatusManager.gameObject, 0,true);
        
        BattleStageManager.Instance.RemoveFieldAbility(GenesisBook);

        status.OnHPDecrease += BookDecreaseHPCheck;
        status.AddEffectFunction(DestructionBookDamageAbility, AbilityCalculation.ProductArea.DMG);
        //status.SpecialDamageEffectFunc += DestructionBookDamageAbility;
        
        DOVirtual.DelayedCall(0.01f, () => _bookStatusManager.gameObject.SetActive(false),
            false);

        switch (difficulty)
        {
            case 3:
                destructionCount = 8;
                break;
            case 4:
                destructionCount = 5;
                break;
            default:
                destructionCount = 8;
                break;
        }
        
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(DestructionBook,
            new EnemyAbilityIconEvent(EnemyAbilityIconEvent.EventType.SetNumber, destructionCount),status);

    }
    
    public void SetBookFakeActive(bool active = true)
    {
        _isBookActive = active;
        _bookStatusManager.FakeActive(active);
    }


    private void BookDecreaseHPCheck(int dmg, AttackBase atk)
    {
        if(_isBookActive == false)
            return;
        
        var atkFromPlayer = atk as AttackFromPlayer;

        if (atkFromPlayer is BulletFromPlayer || atkFromPlayer is HomingProjectile)
        {
            if (atkFromPlayer.firedir * enemyController.facedir < 0)
            {
                print("Book Hit");
                _bookStatusManager.currentHp -= dmg;
                _bookStatusManager.OnHPDecrease?.Invoke(dmg, atk);
                _bookStatusManager.OnHPChange?.Invoke();
            }
        }
        else if (atkFromPlayer is CustomRangedFromPlayer)
        {
            if (atkFromPlayer.attackInfo[0].KBType == BasicCalculation.KnockBackType.FaceDirection &&
                atkFromPlayer.DestroyOnActorInterrupt == false)
            {
                if (atkFromPlayer.firedir * enemyController.facedir < 0)
                {
                    print("Book Hit");
                    _bookStatusManager.currentHp -= dmg;
                    _bookStatusManager.OnHPDecrease?.Invoke(dmg, atk);
                    _bookStatusManager.OnHPChange?.Invoke();
                }
            }
            else
            {
                if (atkFromPlayer.attackCollider)
                {
                    var hitBoxCol = atkFromPlayer.attackCollider;
                    if (hitBoxCol.bounds.max.x >= bookCollider.bounds.min.x &&
                        hitBoxCol.bounds.min.x <= bookCollider.bounds.max.x)
                    {
                        print("Book Hit");
                        _bookStatusManager.currentHp -= dmg;
                        _bookStatusManager.OnHPDecrease?.Invoke(dmg, atk);
                        _bookStatusManager.OnHPChange?.Invoke();
                    }
                }
            }
        }
        else if (atkFromPlayer is CustomMeeleFromPlayer)
        {
            if (atkFromPlayer.attackCollider)
            {
                var hitBoxCol = atkFromPlayer.attackCollider;
                if (hitBoxCol.bounds.max.x >= bookCollider.bounds.min.x &&
                    hitBoxCol.bounds.min.x <= bookCollider.bounds.max.x)
                {
                    print("Book Hit");
                    _bookStatusManager.currentHp -= dmg;
                    _bookStatusManager.OnHPDecrease?.Invoke(dmg, atk);
                    _bookStatusManager.OnHPChange?.Invoke();
                }else print("No Hit");
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
        else
        {
            _bookStatusManager.currentHp -= dmg;
            _bookStatusManager.OnHPDecrease?.Invoke(dmg, atk);
            _bookStatusManager.OnHPChange?.Invoke();
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
              
        DragaliaEnemyActionTypes.H001 actionType = 
            (DragaliaEnemyActionTypes.H001) Enum.Parse(typeof(DragaliaEnemyActionTypes.H001), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.H001.Buff:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float buffAmount = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BuffAllChildren((int)buffAmount,interval));
                break;
            }
            case DragaliaEnemyActionTypes.H001.SummonChild:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[^1]);
                object[] messageArray = new object[_currentActionStage.args.Length - 1];
                Array.Copy(_currentActionStage.args,
                    0, messageArray, 
                    0, _currentActionStage.args.Length - 1);
                currentAction = StartCoroutine(ACT_SummonChildren(messageArray,interval));
                break;
            }
            case DragaliaEnemyActionTypes.H001.SummonElite:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonElite(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H001.Executioners:
            {
                int hp1 = int.Parse(_currentActionStage.args[0], CultureInfo.InvariantCulture);
                int atk1 = int.Parse(_currentActionStage.args[1], CultureInfo.InvariantCulture);
                
                int hp2 = int.Parse(_currentActionStage.args[2], CultureInfo.InvariantCulture);
                int atk2 = int.Parse(_currentActionStage.args[3], CultureInfo.InvariantCulture);

                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[4]);
                currentAction = StartCoroutine(ACT_Executioners(hp1,atk1,hp2,atk2,interval));
                break;
            }
            case DragaliaEnemyActionTypes.H001.HealOnebyOne:
            {
                int hpOrb = int.Parse(_currentActionStage.args[0], CultureInfo.InvariantCulture);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_AmplificationAnthem(hpOrb, interval));
                break;
            }
            case DragaliaEnemyActionTypes.H001.HealSimultaneously:
            {
                int hpOrb = int.Parse(_currentActionStage.args[0], CultureInfo.InvariantCulture);
                int hpM1 = int.Parse(_currentActionStage.args[1], CultureInfo.InvariantCulture);
                int atkM1 = int.Parse(_currentActionStage.args[2], CultureInfo.InvariantCulture);
                int hpM2 = int.Parse(_currentActionStage.args[3], CultureInfo.InvariantCulture);
                int atkM2 = int.Parse(_currentActionStage.args[4], CultureInfo.InvariantCulture);
                
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[5]);
                
                currentAction = StartCoroutine(ACT_SacrificeCeremony(hpOrb,hpM1,atkM1,hpM2,atkM2,interval));
                break;
            }
            case DragaliaEnemyActionTypes.H001.TargetingPillar:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TargetingPillar(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H001.BouncingOrb:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BouncingProjectiles(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H001.Laser:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_LaserAttack(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H001.ChasingPillar:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_PillarPunishment(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H001.Corrosion:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float amount = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Corrosion((int)amount,interval));
                break;
            }
            case DragaliaEnemyActionTypes.H001.ElementSwitch:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ElementSwitch(interval));
                break;
            }
            
        }
    }



    private IEnumerator ACT_BuffAllChildren(int buffAmount, float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action01());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        if (destructionCount <= 0)
        {
            enemyController.SetKBRes(999);
            breakable = false;
            currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action12());
            
            yield return _attackIsNull;
            
            SetDestructionCount(difficulty <= 3?8:5);
            breakable = true;
            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(1);
        }
        
        ActionEnd();
    }
    
    private IEnumerator ACT_SummonChildren(object[] info, float interval)
    {
        ActionStart();
        
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action05(info));
        
        yield return _attackIsNull;
        
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        if (destructionCount <= 0)
        {
            enemyController.SetKBRes(999);
            breakable = false;
            currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action12());
            
            yield return _attackIsNull;
            
            SetDestructionCount(difficulty <= 3?8:5);
            breakable = true;
            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(1);
        }
        
        ActionEnd();
    }
    
    private IEnumerator ACT_SummonElite(float interval)
    {
        breakable = false;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        if (!(enemyAttackManager as EnemyMoveController_H001).IsInvincible)
        {
            BattleStageManager.Instance.AddFieldAbility(GenesisBook);
            currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action06());
            yield return _attackIsNull;
            breakable = true;
            yield return new WaitForSeconds(interval);
        }
        else
        {
            breakable = true;
        }
        
        currentMoveAction = StartCoroutine(enemyController.FlyTowardTargetOnSamePlatform(targetPlayer,
            99, 0.5f, 4));

        yield return _moveIsNull;

        if (destructionCount <= 0)
        {
            enemyController.SetKBRes(999);
            breakable = false;
            currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action12());
            
            yield return _attackIsNull;
            
            SetDestructionCount(difficulty <= 3?8:5);
            breakable = true;
            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(1);
        }
        
        ActionEnd();
    }
    
    private IEnumerator ACT_Executioners(int hp1, int atk1, int hp2, int atk2, float interval)
    {
        ActionStart();
        
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.
            H001_Action08(hp1,hp2,atk1,atk2));
        
        yield return _attackIsNull;
        
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        if (destructionCount <= 0)
        {
            enemyController.SetKBRes(999);
            breakable = false;
            currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action12());
            
            yield return _attackIsNull;
            
            SetDestructionCount(difficulty <= 3?8:5);
            breakable = true;
            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(1);
        }
        
        ActionEnd();
    }
    
    private IEnumerator ACT_AmplificationAnthem(int hp, float interval)
    {
        ActionStart();
        
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine
        (enemyAttackManager.
            H001_Action09(hp));
        
        yield return _attackIsNull;
        
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        if (destructionCount <= 0)
        {
            enemyController.SetKBRes(999);
            breakable = false;
            currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action12());
            
            yield return _attackIsNull;
            
            SetDestructionCount(difficulty <= 3?8:5);
            breakable = true;
            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(1);
        }
        
        ActionEnd();
    }
    
    private IEnumerator ACT_SacrificeCeremony(int hp, int hp1, int atk1, int hp2, int atk2, float interval)
    {
        ActionStart();
        
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.
            H001_Action10(hp, hp1,atk1,hp2,atk2));
        
        yield return _attackIsNull;
        
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        if (destructionCount <= 0)
        {
            enemyController.SetKBRes(999);
            breakable = false;
            currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action12());
            
            yield return _attackIsNull;
            
            SetDestructionCount(difficulty <= 3?8:5);
            breakable = true;
            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(1);
        }
        
        ActionEnd();
    }
    
    private IEnumerator ACT_TargetingPillar(float interval)
    {
        ActionStart();
        
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action02());
        
        yield return _attackIsNull;
        
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        if (destructionCount <= 0)
        {
            enemyController.SetKBRes(999);
            breakable = false;
            currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action12());
            
            yield return _attackIsNull;
            
            SetDestructionCount(difficulty <= 3?8:5);
            breakable = true;
            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(1);
        }
        
        ActionEnd();
    }
    
    private IEnumerator ACT_BouncingProjectiles(float interval)
    {
        ActionStart();

        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = StartCoroutine(enemyController.FlyTowardTargetOnSamePlatform(targetPlayer,
            18, 0.3f, 4));

        yield return _moveIsNull;
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action03());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        
        if (destructionCount <= 0)
        {
            enemyController.SetKBRes(999);
            breakable = false;
            currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action12());
            
            yield return _attackIsNull;
            
            SetDestructionCount(difficulty <= 3?8:5);
            breakable = true;
            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(1);
        }
        
        ActionEnd();
    }
    
    private IEnumerator ACT_LaserAttack(float interval)
    {
        ActionStart();

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action04());
        
        yield return _attackIsNull;
        
        currentMoveAction = StartCoroutine(enemyController.FlyTowardTargetOnSamePlatform(targetPlayer,
            99, 0.5f, 4));

        yield return _moveIsNull;

        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        
        if (destructionCount <= 0)
        {
            enemyController.SetKBRes(999);
            breakable = false;
            currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action12());
            
            yield return _attackIsNull;
            
            SetDestructionCount(difficulty <= 3?8:5);
            breakable = true;
            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(1);
        }
        
        ActionEnd();
    }
    
    private IEnumerator ACT_PillarPunishment(float interval)
    {
        ActionStart();

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action11());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);

        if (destructionCount <= 0)
        {
            enemyController.SetKBRes(999);
            breakable = false;
            currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action12());
            
            yield return _attackIsNull;
            
            SetDestructionCount(difficulty <= 3?8:5);
            breakable = true;
            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(1);
        }

        ActionEnd();
    }
    
    private IEnumerator ACT_Corrosion(int amount, float interval)
    {
        ActionStart();

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action07(amount));
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);

        if (destructionCount <= 0)
        {
            enemyController.SetKBRes(999);
            breakable = false;
            currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action12());
            
            yield return _attackIsNull;
            
            SetDestructionCount(difficulty <= 3?8:5);
            breakable = true;
            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(1);
        }

        ActionEnd();
    }
    
    private IEnumerator ACT_ElementSwitch(float interval)
    {
        ActionStart();
        breakable = false;

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action13());
        
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        
        breakable = true;

        yield return new WaitForSeconds(interval);

        if (destructionCount <= 0)
        {
            enemyController.SetKBRes(999);
            breakable = false;
            currentAttackAction = StartCoroutine(enemyAttackManager.H001_Action12());
            
            yield return _attackIsNull;
            
            SetDestructionCount(difficulty <= 3?8:5);
            breakable = true;
            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(1);
        }

        ActionEnd();
    }
    

    private IEnumerator ACT_PartBreak()
    {
        bookHasBroken = true;
        enemyController.SetKBRes(999);
        BattleStageManager.Instance.RemoveFieldAbility(GenesisBook);
        currentAttackAction = StartCoroutine(enemyAttackManager.PartBreak());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        ActionEnd();
        SetBookFakeActive(false);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
    }

    public void SetDestructionCount(int res)
    {
        destructionCount = res;
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(DestructionBook,
            new EnemyAbilityIconEvent(EnemyAbilityIconEvent.EventType.SetNumber,destructionCount),status);
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(DestructionBook,
            new EnemyAbilityIconEvent((ui) =>
            {
                if (destructionCount < 5)
                {
                    ui.AbilityExtraMessage.color = Color.red;
                }
                else
                {
                    ui.AbilityExtraMessage.color = Color.white;
                }
            }),status);

    }

    private (float,float) DestructionBookDamageAbility(StatusManager source, AttackBase atk, StatusManager target)
    {
        var buff = 0f;
        if (difficulty >= 4)
        {
            buff = (5 - destructionCount) * 0.1f;
        }
        else
        {
            buff = (8 - destructionCount) * 0.05f;
        }
        
        return (Mathf.Clamp(buff,0,0.4f),0);
        
    }
    
    


}
