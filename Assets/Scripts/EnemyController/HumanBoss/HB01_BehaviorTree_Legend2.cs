using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using GameMechanics;
using UnityEngine;

public class HB01_BehaviorTree_Legend2 : EnemyBehaviorManager
{
    protected EnemyMoveController_HB01_Legend2 enemyAttackManager;
    protected EnemyControllerHumanoid enemyController;
    public AudioClip BGM;

    public Color originColor;
    public Color catastrophicColor;
    public Color brightColor;

    public GameObject FlameLevelUI;

    public static TimerBuff brightBuff_1;
    public static TimerBuff brightBuff_2;
    public static TimerBuff brightBuff_3;
    public static TimerBuff brightBuff_4;
    
    public static TimerBuff catastrophicBuff_1;
    public static TimerBuff catastrophicBuff_2;
    //public static TimerBuff catastrophicBuff_3;
    public static TimerBuff catastrophicBuff_4;

    private bool justRecoveredFromBroken = false;

    public int currentWorld
    {
        get
        {
            if(BattleStageManager.Instance.FieldAbilityIDList.Contains(20081))
                return 1;
            else if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20091))
                return 2;
            else return 0;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        enemyAttackManager = GetComponent<EnemyMoveController_HB01_Legend2>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        (status as SpecialStatusManager).onBreak += ResetSkillBoost;
        InitAllBuff();
        InstantiateAllTargetsWithFlameUI();
        GetBehavior();
        BattleEffectManager.Instance.SetBGM(BGM);
        BattleEffectManager.Instance.PlayBGM();
        
    }

    

    protected override void DoAction(int state, int substate)
    {
        if(!playerAlive)
            return;
        
        ParseAction(state, substate);
        
        
        
        /*switch (substate)
        {
            case 0:
            {
                //currentAction = StartCoroutine(ACT_BlazingAzureFixed(3));
                currentAction = StartCoroutine(ACT_InheritorOfBlazewolf(1));
                //currentAction = StartCoroutine(ACT_SetWorld(1, 2));
                //currentAction = StartCoroutine(ACT_SavageFlameRaid(1.5f));
                //currentAction = StartCoroutine(ACT_BrightCarmineRush(1.5f));
                //currentAction = StartCoroutine(ACT_BlazingAzureFixed(3));
                //currentAction = StartCoroutine(ACT_BlazingBlitz(2));
                break;
            }
            case 1:
            {
                currentAction = StartCoroutine(ACT_ReadyForAOE(1, 2));
                //currentAction = StartCoroutine(ACT_SetWorld(2, 2));
                //currentAttackAction = StartCoroutine(ACT_CarmineRush(2));
                //currentAction = StartCoroutine(ACT_BlazingAzureFixed(3));
                //currentAction = StartCoroutine(FlameHeart(1,1,2));
                this.substate = 20;
                break;
            }
            case 2:
            {
                currentAction = StartCoroutine(ACT_JudgementFlameAOE(1,2));
                //currentAction = StartCoroutine(ACT_SetWorld(1, 2));
                //currentAction = StartCoroutine(ACT_BlazingAzureChasing(3));
                this.substate = 20;
                break;
            }
            case 3:
            {
                currentAction = StartCoroutine(ACT_ReadyForAOE(2, 2));
                //currentAction = StartCoroutine(ACT_JudgementFlameAOE(2,2));
                this.substate = 20;
                //currentAction = StartCoroutine(ACT_SetWorld(0, 2));
                //currentAction = StartCoroutine(ACT_FlameRaid(2));
                //currentAction = StartCoroutine(ACT_BrightCarmineRush(2.5f));
                //currentAction = StartCoroutine(ACT_BlazingAzureChasing(3));
                break;
            }
            case 4:
            {
                //currentAction = StartCoroutine(ACT_JudgementFlameAOE(2,2));
                //currentAction = StartCoroutine(ACT_InheritorOfBlazewolf(1));
                this.substate = 20;
                //currentAction = StartCoroutine(ACT_ComboA(1.5f));
                break;
            }
            case 5:
            {
                currentAction = StartCoroutine(ACT_ComboB(1.5f));
                break;
            }
            case 6:
            {
                currentAction = StartCoroutine(ACT_ComboC(1.5f));
                break;
            }
            case 7:
            {
                currentAction = StartCoroutine(ACT_ComboD(1.5f));
                break;
            }
            case 8:
            {
                currentAction = StartCoroutine(ACT_CarmineRush(1.5f));
                break;
            }
            case 9:
            {
                currentAction = StartCoroutine(ACT_ScarletInferno(2.5f));
                break;
            }
            default:
            {
                break;
            }
        }*/
        
        

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
        
        print("Doing: "+action_name);
        
        DragaliaBossActionTypes.HB1001 actionType = 
            (DragaliaBossActionTypes.HB1001) Enum.Parse(typeof(DragaliaBossActionTypes.HB1001), action_name);

        switch (actionType)
        {
            case DragaliaBossActionTypes.HB1001.ComboA:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboA(interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.ComboB:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboB(interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.ComboC:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboC(interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.ComboD:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboD(interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.CarmineRush:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CarmineRush(interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.FlameRaid:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FlameRaid(interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.BrightCarmineRush:
            {
                float interval = float.Parse(_currentActionStage.args[0]);

                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_BrightCarmineRush(interval, 0));
                }else 
                    currentAction = StartCoroutine(ACT_BrightCarmineRush(interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.SavageFlameRaid:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_SavageFlameRaid(interval, 0));
                }else 
                    currentAction = StartCoroutine(ACT_SavageFlameRaid(interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.Inferno:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ScarletInferno(interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.SetWorld:
            {
                int type = int.Parse(_currentActionStage.args[0]);
                float interval = float.Parse(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_SetWorld(type, interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.ScorArea:
            {
                int type = int.Parse(_currentActionStage.args[0]);
                float interval = float.Parse(_currentActionStage.args[1]);
                if(type == 2)
                    currentAction = StartCoroutine(ACT_BlazingAzureChasing(interval));
                else if(type == 1)
                    currentAction = StartCoroutine(ACT_BlazingAzureFixed2(interval));
                else 
                    currentAction = StartCoroutine(ACT_BlazingAzureFixed(interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.DashAttack:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SingleDodgeCombo(interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.Charge:
            {
                int type = int.Parse(_currentActionStage.args[0]);
                float interval = float.Parse(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_ReadyForAOE(type,interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.AOE:
            {
                int type = int.Parse(_currentActionStage.args[0]);
                float interval = float.Parse(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_JudgementFlameAOE(type, interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.Mirage:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BlazingBlitz(interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.UpdateBuff:
            {
                int buffSelf = int.Parse(_currentActionStage.args[0]);
                int buffOthers = int.Parse(_currentActionStage.args[1]);
                float interval = float.Parse(_currentActionStage.args[2]);
                currentAction = StartCoroutine(ACT_FlameHeart(buffSelf,buffOthers,interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.Counter:
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CounterBlade(interval));
                break;
            }
            case DragaliaBossActionTypes.HB1001.DualFlame:
            {
                int type = int.Parse(_currentActionStage.args[0]);
                float interval = float.Parse(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_InheritorOfBlazewolf(interval,type));
                break;
            }
        }
        
        
        
    }







    protected IEnumerator ACT_SingleDodgeCombo(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveTowardTarget
                (targetPlayer, 3.5f, 3.5f,1, 5));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        enemyController.SetKBRes(999);
        yield return null;

        if (TaskSuccess)
        {
            //TaskSuccess = false;
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action01());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
        }

        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_ComboA(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 3, 10));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action03());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(()=>currentAttackAction == null);
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action03());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_ComboB(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 3, 5));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HB01_Action06());
        
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_ComboC(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 3, 6.5f));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action07());
            yield return new WaitUntil(()=>currentAttackAction == null);
            enemyController.TurnMove(targetPlayer);
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action01());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(()=>currentAttackAction == null);
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action07());
            yield return new WaitUntil(()=>currentAttackAction == null);
            enemyController.TurnMove(targetPlayer);
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action01());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_ComboD(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 3, 5));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action05());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(()=>currentAttackAction == null);
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action05());
            
        }
        
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_CarmineRush(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 3, 15));
        
        
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action04());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(()=>currentAttackAction == null);
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action04());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        justRecoveredFromBroken = false;
        
        yield return new WaitForSeconds(interval);
        ActionEnd();

    }

    protected IEnumerator ACT_FlameRaid(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 3.5f, 5));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        //yield return null;
        
        if (TaskSuccess)
        {
            print("FlameRaid_In");
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action08());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(()=>currentAttackAction == null);
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action08());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        justRecoveredFromBroken = false;
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_BrightCarmineRush(float interval,int move = 1)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        if (move == 0)
        {
            status.RemoveTimerBuff((int)BasicCalculation.BattleCondition.BlazewolfsRush,true,1);
        }
        
        
        currentMoveAction = 
                StartCoroutine
                (enemyController.MoveToSameGround
                    (targetPlayer, 0, 15));
        
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        

        //yield return null;
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);

        if (TaskSuccess || move==0)
        {
            if (justRecoveredFromBroken)
            {
                currentAttackAction =
                    StartCoroutine(enemyAttackManager.HB01_Action04());
            }
            else
            {
                currentAttackAction =
                    StartCoroutine(enemyAttackManager.HB01_Action09());
            }
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(()=>currentAttackAction == null);
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action04());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        justRecoveredFromBroken = false;
        
        yield return new WaitForSeconds(interval);
        ActionEnd();

    }

    protected IEnumerator ACT_SavageFlameRaid(float interval,int move = 1)
    {
        ActionStart();
        SetTarget(ClosestTarget);

        if (move == 0)
        {
            status.RemoveTimerBuff((int)BasicCalculation.BattleCondition.BlazewolfsRush,true,1);
        }



        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 0, 9));
            
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        
        
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        yield return null;
        
        if (TaskSuccess || move == 0)
        {
            if (justRecoveredFromBroken)
            {
                currentAttackAction =
                    StartCoroutine(enemyAttackManager.HB01_Action08());
            }
            else
            {
                currentAttackAction =
                    StartCoroutine(enemyAttackManager.HB01_Action10());
            }
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(()=>currentAttackAction == null);
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action08());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        justRecoveredFromBroken = false;
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }


    protected IEnumerator ACT_ScarletInferno(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action11());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_BlazingAzureFixed(float interval)
    {
        breakable = false;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action15_1());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        breakable = true;
        ActionEnd();
    }
    
    protected IEnumerator ACT_BlazingAzureChasing(float interval)
    {
        breakable = false;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action15_2());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        breakable = true;
        ActionEnd();
    }
    
    protected IEnumerator ACT_BlazingAzureFixed2(float interval)
    {
        breakable = false;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action15_3());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        breakable = true;
        ActionEnd();
    }

    protected IEnumerator ACT_FlameHeart(int lvSelf, int lvOthers,float interval)
    {
        breakable = false;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action16(lvSelf,lvOthers));
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_SetWorld(int world,float interval)
    {
        breakable = false;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);

        if (world == 1)
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action17());
        }else if (world == 2)
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action18());
        }
        else
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action19());
            
        }
        
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        ActionEnd();
        
    }

    public IEnumerator ACT_BlazingBlitz(float interval)
    {
        
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 3, 6.5f));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action20());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(()=>currentAttackAction == null);
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action20());

        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
        
        
    }
    
    protected IEnumerator ACT_CounterBlade(float interval)
    {
        breakable = false;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action21());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">1:高的胜利 0:低的胜利</param>
    /// <param name="interval"></param>
    /// <returns></returns>
    protected IEnumerator ACT_ReadyForAOE(int type, float interval)
    {
        breakable = false;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        if (type == 1)
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action22());
        }
        else
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action23());
        }

        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_JudgementFlameAOE(int type, float interval)
    {
        breakable = false;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        if (type == 1)
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action24());
        }
        else
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action25());
        }

        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interval"></param>
    /// <param name="type">)0是反向</param>
    /// <returns></returns>
    protected IEnumerator ACT_InheritorOfBlazewolf(float interval, int type=1)
    {
        breakable = false;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB01_Action26(type==1?true:false));
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        ActionEnd();
        
    }















    protected void InitAllBuff()
    {
        catastrophicBuff_1 = new TimerBuff((int)BasicCalculation.BattleCondition.DamageUp,
            10,-1,1,8101402);
        catastrophicBuff_2 = new TimerBuff((int)BasicCalculation.BattleCondition.SkillHasteBuff,
            14,-1,1,8101402);
        // catastrophicBuff_3 = new TimerBuff((int)BasicCalculation.BattleCondition.BreakPunisher,
        //     20,-1,1,8101402);
        catastrophicBuff_4 = new TimerBuff((int)BasicCalculation.BattleCondition.RecoveryDebuff,
            30,-1,1,8101402);
        
        catastrophicBuff_1.dispellable = false;
        catastrophicBuff_2.dispellable = false;
        //catastrophicBuff_3.dispellable = false;
        catastrophicBuff_4.dispellable = false;
        
        brightBuff_1 = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            10,-1,1,8101402);
        // brightBuff_2 = new TimerBuff((int)BasicCalculation.BattleCondition.DamageCut,
        //     10,-1,1,8101402);
        
        brightBuff_2 = new TimerBuff((int)BasicCalculation.BattleCondition.BreakPunisher,
            60,-1,1,8101402);
        
        brightBuff_3 = new TimerBuff((int)BasicCalculation.BattleCondition.OverdriveAccerlerator,
            160,-1,1,8101402);
        brightBuff_4 = new TimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff,
            10,-1,1,8101402);
        
        brightBuff_1.dispellable = false;
        brightBuff_2.dispellable = false;
        brightBuff_3.dispellable = false;
        brightBuff_4.dispellable = false;


    }

    protected void InstantiateAllTargetsWithFlameUI()
    {
        var targets = BattleStageManager.GetAllStatusManagers();
        foreach (var target in targets)
        {
            //target.OnBuffEventDelegate += BuffWithFlameLevelCheckOnBuffEvent;
            var _parent = target.transform.Find("BuffLayer");

            float depth;
            try
            {
                depth = target.transform.GetChild(0).position.z;
            }
            catch
            {
                depth = 0;
            }

            var ui = Instantiate(FlameLevelUI, _parent.position + new Vector3(0,2.5f,depth),
                Quaternion.identity,_parent);
            var uiController = ui.GetComponent<UI_HB001_Legend_01>();
            uiController.levels = new List<float>(){1,4,16};
        }
        var mybufflayer = transform.Find("BuffLayer");
        
        var myUIController = mybufflayer.GetComponentInChildren<UI_HB001_Legend_01>();
        myUIController.levels = new List<float>(){0.1f,9.9f,999};
        
        BattleStageManager.Instance.OnFieldAbilityAdd += BuffWithFlameLevelCheck;
        BattleStageManager.Instance.OnFieldAbilityRemove += BuffWithFlameLevelCheckWhenRemoved;


    }

    protected void BuffWithFlameLevelCheckWhenRemoved(int id)
    {
        var targets = BattleStageManager.GetAllStatusManagers();
        if (!BattleStageManager.Instance.FieldAbilityIDList.Contains(20081) &&
            !BattleStageManager.Instance.FieldAbilityIDList.Contains(20091))
        {
            foreach (var target in targets)
            {
                target.RemoveAllConditionWithSpecialID(8101402);
            }
        }


    }

    protected void BuffWithFlameLevelCheck(int id)
    {
        var targets = BattleStageManager.GetAllStatusManagers();
        if (id == 20081)
        {
            foreach (var target in targets)
            {
                target.RemoveAllConditionWithSpecialID(8101402);
                var scorchingLevel =
                    target.GetConditionTotalValue((int)BasicCalculation.BattleCondition.ScorchingEnergy) - 1;
                if(scorchingLevel<0)
                    continue;
                
                
                var buff1 = new TimerBuff(brightBuff_1);
                buff1.SetEffect(buff1.effect + scorchingLevel * 5);
                var buff2 = new TimerBuff(brightBuff_2);
                buff2.SetEffect(buff2.effect + scorchingLevel * 5);
                var buff3 = new TimerBuff(brightBuff_3);
                buff3.SetEffect(buff3.effect + scorchingLevel * 20);
                var buff4 = new TimerBuff(brightBuff_4);
                buff4.SetEffect(buff4.effect + scorchingLevel * 5);
                
                target.ObtainTimerBuff(buff1);
                target.ObtainTimerBuff(buff2,false);
                target.ObtainTimerBuff(buff3,false);
                target.ObtainTimerBuff(buff4,false);
            }

        }else if (id == 20091)
        {
            
            foreach (var target in targets)
            {
                target.RemoveAllConditionWithSpecialID(8101402);
                var scorchingLevel =
                    target.GetConditionTotalValue((int)BasicCalculation.BattleCondition.ScorchingEnergy) - 1;
                if(scorchingLevel<0)
                    continue;
                
                var buff1 = new TimerBuff(catastrophicBuff_1);
                buff1.SetEffect(buff1.effect + scorchingLevel * 5);
                var buff2 = new TimerBuff(catastrophicBuff_2);
                buff2.SetEffect(buff2.effect + scorchingLevel * 3);
                // var buff3 = new TimerBuff(catastrophicBuff_3);
                // buff3.SetEffect(buff3.effect + scorchingLevel * 5);
                var buff4 = new TimerBuff(catastrophicBuff_4);
                buff4.SetEffect(buff4.effect + scorchingLevel * 5);
                
                target.ObtainTimerBuff(buff1);
                target.ObtainTimerBuff(buff2,false);
                //target.ObtainTimerBuff(buff3,false);
                target.ObtainTimerBuff(buff4,false);
            }
        }

    }

    protected override bool CheckCondition(string[] args, out int dest_state)
    {
        var conditionName = args[0];

        switch (conditionName)
        {
            //arg为0，惩罚多的。arg为1，惩罚少的。
            //向上漂，惩罚少的。向下漂，惩罚多的。
            //所以，向上漂，arg为1。向下漂，arg为0。
            case "loop_count":
            {
                if (_currentPhase.loopCount % 2 == 0)
                {
                    dest_state = int.Parse(args[1]);
                    return true;
                }
                else
                {
                    dest_state = int.Parse(args[2]);
                    return false;
                }
            }
            case "has_buff":
            {
                if (status.GetConditionStackNumber((int)BasicCalculation.BattleCondition.BlazewolfsRush) > 0)
                {
                    dest_state = int.Parse(args[1]);
                    return true;
                }
                else
                {
                    dest_state = int.Parse(args[2]);
                    return false;
                }
            }
            case "aoe_released":
            {
                var particle = FindObjectOfType<Projectile_C005_7_Boss>();
                if (particle == null)
                {
                    dest_state = int.Parse(args[1]);
                    return true;
                }
                else
                {
                    dest_state = int.Parse(args[2]);
                    return false;
                }
            }
            case "aoe_type_upward":
            {
                var particle = FindObjectOfType<Projectile_C005_7_Boss>();
                if (particle == null)
                {
                    dest_state = int.Parse(args[1]);
                    Debug.LogWarning("particle is null");
                    return false;
                }

                if (particle.upward > 0)
                {
                    dest_state = int.Parse(args[1]);
                    return true;
                }
                else
                {
                    dest_state = int.Parse(args[2]);
                    return false;
                }

            }
            default:
            {
                dest_state = (substate + 1);
                return false;
            }

        }
        
        
    }

    

    private void ResetSkillBoost()
    {

        justRecoveredFromBroken = true;

    }
}
