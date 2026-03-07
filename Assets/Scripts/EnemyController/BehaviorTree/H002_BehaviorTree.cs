using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class H002_BehaviorTree : EnemyBehaviorManager
{
    private EnemyMoveController_H002 enemyAttackManager;
    private EnemyControllerFlyingHigh enemyController;

    [SerializeField] private GameObject partWingPrefab;
    public GameObject partWing;
    private PartStatusManager wingStatus;
    public bool WingHasBroken { get; private set; }
    public const int DamageTakenDownBuffID = 8212301;
    public const int DamageDealtDownDebuffID = 8212302;
    
    public bool WingIsAlive => wingStatus != null && wingStatus.currentHp > 0;
    
    //private Vector4 hitBoxInfo;
    //private Vector4 wingHitBoxInfo;
    
    private bool wingIsFakeActive = false;

    private TimerBuff dmgTakenDownDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageCut,
        10, 10, 1, DamageTakenDownBuffID);
    private TimerBuff dmgDealtDownBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageDown,
        10, 10, 1, DamageDealtDownDebuffID);
    
    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_H002>();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        OnBehaviorStart += InitWing;
        GetBehavior();
    }

    private void InitWing()
    {
        OnBehaviorStart -= InitWing;
        
        partWing = Instantiate(partWingPrefab, transform.position,
           Quaternion.identity, BattleStageManager.Instance.EnemyLayer.transform);
        var partStatus = partWing.GetComponent<PartStatusManager>();
        partStatus.Link(status);
        //partWing.SetActive(false);
        InvokePartEvent(partWing, 0,true);
        status.partList.Add(partWing);
        wingStatus = partStatus;
        status.OnHPDecrease += WingsFakeDecreaseHP;
        status.OnAfflictionInflict += IncreaseDefenseOnAfflictionConnect;
        status.OnBuffEventDelegate += DecreaseStrengthOnAfflictionReceived;

        DOVirtual.DelayedCall(0.01f, () => partWing.SetActive(false),false);

    }

    public void SetWingFakeActive(bool active = true)
    {
        wingIsFakeActive = active;
        wingStatus.FakeActive(active);
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
        
        
        
        DragaliaEnemyActionTypes.H002 actionType = 
            (DragaliaEnemyActionTypes.H002) Enum.Parse(typeof(DragaliaEnemyActionTypes.H002), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.H002.SweetStockade:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float corrosionAmount = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SweetStockade((int)corrosionAmount,interval));
                break;
            }
            case DragaliaEnemyActionTypes.H002.Combo:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_WeaponSwingCombo(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H002.TargetingCandies:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SweetShower(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H002.Nihil:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Nihility(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H002.Corrosion:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float corrosionAmount = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CorrosionFog((int)corrosionAmount,interval));
                break;
            }
            case DragaliaEnemyActionTypes.H002.Rush:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_DashForward(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_DashForward(interval,false));
                }
                
                break;
            }
            case DragaliaEnemyActionTypes.H002.Buff:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float buffAmount = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BuffSelf((int)buffAmount,interval));
                break;
            }
            case DragaliaEnemyActionTypes.H002.Smash:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    int type = int.Parse(_currentActionStage.args[1]);
                    currentAction = StartCoroutine(ACT_SmashAttack(interval,type));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_SmashAttack(interval));
                }
                break;
            }
            case DragaliaEnemyActionTypes.H002.CrossCandies:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                int type = _currentActionStage.args.Length > 1 ? int.Parse(_currentActionStage.args[1]) : 1;
                currentAction = StartCoroutine(ACT_CrossingCandies(interval,type));
                break;
            }
            case DragaliaEnemyActionTypes.H002.WandGroup:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_WandGroup(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H002.CombinedRush:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CombinedDash(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H002.BounceCandies:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BounceCandies(interval));
                break;
            }
            case DragaliaEnemyActionTypes.H002.GroundBurst:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SmashAttackII(interval));
                break;
            }
        }
    }

    protected IEnumerator ACT_Nihility(float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 18, 0.3f, 5));
        
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H002_Action04());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_CorrosionFog(int corrosionAmount, float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 18, 0.3f, 5));
        
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H002_Action05(corrosionAmount));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    protected IEnumerator ACT_SweetStockade(int corrosionAmount, float interval)
    {
        ActionStart();
        breakable = false;
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 18, 0.3f, 5));
        
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H002_Action01(corrosionAmount));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_WeaponSwingCombo(float interval)
    {
        ActionStart();
        //SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 4.5f, 0.5f, 5));
        
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H002_Action02());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_SweetShower(float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 15, 0.5f, 5));
        
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H002_Action03());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_DashForward(float interval, bool backToGround = true)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        yield return new WaitUntil(() => !enemyController.hurt);

        Vector2 targetPos = new Vector2(transform.position.x, targetPlayer.transform.position.y - 1f);
        if (targetPlayer.transform.position.x - transform.position.x > 15)
        {
            targetPos.x = targetPlayer.transform.position.x - 15;
        }else if (transform.position.x - targetPlayer.transform.position.x > 15)
        {
            targetPos.x = targetPlayer.transform.position.x + 15;
        }
        
        var distance = Vector2.Distance(transform.position, targetPos);

        if (distance > 2)
        {
            currentMoveAction = StartCoroutine
            (enemyController.FlyToPoint(new Vector2(transform.position.x,
                    targetPlayer.transform.position.y - 1f),
                Vector2.Distance(transform.position,
                    targetPlayer.transform.position) / enemyController.moveSpeed, Ease.Linear));
        
            yield return new WaitUntil(()=>currentMoveAction == null);
        }
        
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H002_Action06());

        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        if (backToGround)
        {
            currentMoveAction = StartCoroutine
            (enemyController.FlyTowardTargetOnSamePlatform
                (targetPlayer, 20, 0.4f, 5));
        
            yield return new WaitUntil(()=>currentMoveAction == null);
        }
        else
        {
            enemyController.SetGravityScale(0);
        }
        
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_BuffSelf(int buffAmount, float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H002_Action07(buffAmount));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_SmashAttack(float interval, int type = -1)
    {
        ActionStart();
        breakable = false;
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 5, 0.3f, 5));
        
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);

        if (type < 0)
        {
            type = Random.Range(0, 2);
        }

        currentAttackAction = StartCoroutine(enemyAttackManager.H002_Action08(type==1));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        
        if (WingIsAlive == false && !WingHasBroken)
        {
            WingHasBroken = true;
            currentAttackAction = StartCoroutine(enemyAttackManager.PartBreak());
            yield return new WaitUntil(()=>currentAttackAction == null);
        }
        
        
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }


    protected IEnumerator ACT_CrossingCandies(float interval, int type = 1)
    {
        ActionStart();

        yield return new WaitUntil(() => !enemyController.hurt);

        bool redIsSafe;
        enemyController.SetKBRes(999);

        if (type == 1)
        {
            redIsSafe = true;
        }else if(type == 2)
        {
            redIsSafe = false;
        }
        else
        {
            redIsSafe = Random.Range(0, 2) == 0;
        }

        currentAttackAction = StartCoroutine(enemyAttackManager.H002_Action09(redIsSafe));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_WandGroup(float interval)
    {
        ActionStart();
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H002_Action10());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);

        if (WingIsAlive == false && !WingHasBroken)
        {
            WingHasBroken = true;
            currentAttackAction = StartCoroutine(enemyAttackManager.PartBreak());
            yield return new WaitUntil(()=>currentAttackAction == null);
        }
        
        yield return new WaitForSeconds(interval);
        breakable = true;
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_CombinedDash(float interval)
    {
        ActionStart();
        breakable = false;
        status.ImmuneToAllControlAffliction = true;
        
        yield return new WaitUntil(() => !enemyController.hurt);

        int direction = transform.position.x > 0 ? -1 : 1;
        Vector2 targetPos = new Vector2(-direction*12,-2.5f);
        float timeToReach = 0.05f + Vector2.Distance(transform.position, targetPos) / enemyController.moveSpeed;
        
        currentMoveAction = StartCoroutine(enemyController.
            FlyToPoint(targetPos, timeToReach, Ease.Linear));
        
        yield return new WaitUntil(()=>currentMoveAction == null);
        

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H002_Action11(Random.Range(0,2)==1));

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);

        if (WingIsAlive == false && !WingHasBroken)
        {
            WingHasBroken = true;
            currentAttackAction = StartCoroutine(enemyAttackManager.PartBreak());
            yield return new WaitUntil(()=>currentAttackAction == null);
        }
        
        yield return new WaitForSeconds(interval);
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_BounceCandies(float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 5, 0.5f, 5));
        
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H002_Action12());

        yield return new WaitUntil(()=>currentAttackAction == null);

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_SmashAttackII(float interval)
    {
        ActionStart();
        SetTarget(viewerPlayer);
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.FlyTowardTargetOnSamePlatform
            (targetPlayer, 5, 0.3f, 5));
        
        yield return new WaitUntil(()=>currentMoveAction == null);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.H002_Action13());

        yield return new WaitUntil(()=>currentAttackAction == null);
        
        if (WingIsAlive == false && !WingHasBroken)
        {
            WingHasBroken = true;
            currentAttackAction = StartCoroutine(enemyAttackManager.PartBreak());
            yield return new WaitUntil(()=>currentAttackAction == null);
        }

        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    

    private void WingsFakeDecreaseHP(int dmg, AttackBase atk)
    {
        if(partWing.activeInHierarchy || !wingIsFakeActive)
            return;
        
        wingStatus.currentHp -= dmg;
        wingStatus.OnHPDecrease?.Invoke(dmg, atk);
        wingStatus.OnHPChange?.Invoke();
    }

    public void ReduceBuffEffect()
    {
        dmgTakenDownDebuff.SetEffect(5);
        dmgDealtDownBuff.SetEffect(15);
    }
    private void IncreaseDefenseOnAfflictionConnect(BattleCondition cond)
    {
        status.ObtainTimerBuff(new TimerBuff(dmgTakenDownDebuff));
    }
    
    private void DecreaseStrengthOnAfflictionReceived(BattleCondition cond)
    {
        if (cond.buffID == (int)BasicCalculation.BattleCondition.Flashburn ||
            cond.buffID == (int)BasicCalculation.BattleCondition.Paralysis ||
            cond.buffID == (int)BasicCalculation.BattleCondition.Frostbite)
        {
            status.ObtainTimerBuff(new TimerBuff(dmgDealtDownBuff));
        }
    }

    protected override bool CheckCondition(string[] args, out int dest_state)
    {
        var conditionName = args[0];

        switch (conditionName)
        {
            case "hp":
            {
                var hpFraction = ObjectExtensions.ParseInvariantFloat(args[1]);
                if (status.currentHp <= hpFraction * status.maxHP)
                {
                    dest_state = int.Parse(args[2], CultureInfo.InvariantCulture);
                    return true;
                }
                else
                {
                    dest_state = int.Parse(args[3], CultureInfo.InvariantCulture);
                }
                break;
            }
            default:
            {
                dest_state = substate + 1;
                break;
            }
        }

        
        return false;
    }
}
