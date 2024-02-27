using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class HB04_BehaviorTree : EnemyBehaviorManager
{
    protected EnemyMoveController_HB04 enemyAttackManager;
    protected EnemyControllerHumanoidHigh enemyController;
    protected int totalRevived = 0;
    protected int breakState = -1;
    
    //public int currentDebugVal;
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoidHigh>();
        enemyAttackManager = GetComponent<EnemyMoveController_HB04>();
        GetBehavior();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        status.BeforeReviveOrDeath += CheckPowerOfBonds;

    }

    

    protected override void DoAction(int state, int substate)
    {
        if(playerAlive == false)
            return;
        
        ParseAction(state,substate);

        // if(substate == 0)
        //     currentAction = StartCoroutine(ACT_DivineRadiance(2));
        // else if(substate == 1)
        //     currentAction = StartCoroutine(ACT_ConsecratedBrilliance(20,100,2));
        // else if(substate == 2)
        //     currentAction = StartCoroutine(ACT_ProjectilesAttackIII(1));
        // else if(substate == 3)
        //     currentAction = StartCoroutine(ACT_VertiStars(1));
        // else if(substate == 4)
        //     currentAction = StartCoroutine(ACT_BlazingFount(2));
        // else if(substate == 5)
        //     currentAction = StartCoroutine(ACT_RingOfAffection(2));
        // else if(substate == 6)
        //     currentAction = StartCoroutine(ACT_BlessedWall(1,2));
        // else if(substate == 7)
        //     currentAction = StartCoroutine(ACT_BlessedWall(-1,0));
        // else if(substate == 8)
        //     currentAction = StartCoroutine(ACT_ProjectilesAttackII(2));
        // // else if(substate == 8)
        // //     currentAction = StartCoroutine(ACT_ComboB(2));
        // else if(substate == 9)
        //     currentAction = StartCoroutine(ACT_BlazingFount(3));
        // else if(substate == 10)
        //     currentAction = StartCoroutine(ACT_ComboA(1));
        // else if(substate == 11)
        //     currentAction = StartCoroutine(ACT_ComboB(1));
        // else
        // {
        //     this.substate = 4;
        // }
        
    }

    protected override bool CustomJumpActionConditionCheck(string[] args)
    {
        if (args[0] == "revive")
        {
            if (totalRevived >= int.Parse(args[1]))
                return true;
        }

        return false;
    }

    protected override bool CheckCondition(string[] args, out int dest_state)
    {
        var conditionName = args[0];

        switch (conditionName)
        {
            case "wall_stucked":
            {
                var borders = enemyAttackManager.blessedWalls;
                if (borders.Count == 0)
                {
                    dest_state = int.Parse(args[2]);
                    return false;
                }
                else if (borders.Count == 1)
                {
                    if (targetPlayer.transform.position.x > borders[0].transform.position.x &&
                        borders[0].transform.position.x >= 0)
                    {
                        dest_state = int.Parse(args[1]);
                        return true;
                    }

                    else if (targetPlayer.transform.position.x < borders[0].transform.position.x &&
                             borders[0].transform.position.x < 0)
                    {
                        dest_state = int.Parse(args[1]);
                        return true;
                    }

                    else
                    {
                        dest_state = int.Parse(args[2]);
                        return true;
                    }

                }
                else if (borders.Count >= 2)
                {
                    var min = Mathf.Min(borders[0].transform.position.x, borders[1].transform.position.x);
                    var max = Mathf.Max(borders[0].transform.position.x, borders[1].transform.position.x);
                    
                    if(targetPlayer.transform.position.x > min && targetPlayer.transform.position.x < max)
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
                else
                {
                    dest_state = int.Parse(args[2]);
                    return false;
                }
            }
            case "no_bond":
            {
                var bond = status.GetConditionStackNumber((int)BasicCalculation.BattleCondition.PowerOfBonds);

                if (bond == 0)
                {
                    dest_state = int.Parse(args[1]);

                    if (args.Length >= 3)
                    {
                        var rand = UnityEngine.Random.Range(2, args.Length);
                        breakState = int.Parse(args[rand]);
                    }
                    else
                    {
                        breakState = substate + 1;
                    }

                    return true;
                }
                else
                {
                    if (args.Length == 3)
                    {
                        dest_state = int.Parse(args[2]);
                        return false;
                    }
                    else if (args.Length >= 4)
                    {
                        var rand = UnityEngine.Random.Range(2, args.Length);
                        dest_state = int.Parse(args[rand]);
                        return false;
                    }
                    else
                    {
                        dest_state = this.substate + 1;
                        return false;
                    }
                }

            }
            case "to_break_point":
            {
                if (breakState >= 0)
                {
                    dest_state = breakState;
                    //breakState = -1;
                    return true;
                }
                else
                {
                    dest_state = int.Parse(args[1]);
                    return false;
                }
            }
            default:
            {
                dest_state = substate + 1;
                break;
            }
        }

        
        return false;

    }


    protected virtual void ParseAction(int state, int substate)
    {
        _currentPhase = _pattern.phasePattern[state];
        _currentActionStage = _currentPhase.action_list[substate];

        var action_name = _currentActionStage.action_name;

        if (action_name == "null")
        {
            ActionEnd();
            return;
        }
        
        DragaliaEnemyActionTypes.HB1004 actionType = 
            (DragaliaEnemyActionTypes.HB1004) Enum.Parse(typeof(DragaliaEnemyActionTypes.HB1004), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.HB1004.comboA:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboA(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.comboB:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboB(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.blazingFount:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BlazingFount(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.affectionRing:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_RingOfAffection(interval,false));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_RingOfAffection(interval,true));
                }
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.warp:
            {
                if (_currentActionStage.args.Length == 3)
                {
                    float posX = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                    float posY = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                    float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);
                    currentAction = StartCoroutine(ACT_WarpToPosition(new Vector2(posX,posY),interval));
                }else if (_currentActionStage.args.Length == 1)
                {
                    float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                    currentAction = StartCoroutine(ACT_WarpToNear(interval));
                }else Debug.LogError("Warp action args error");
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.wall:
            {
                int type = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                if (type == 1)
                {
                    currentAction = StartCoroutine(ACT_BlessedWall(1, interval));
                }else if (type == -1)
                {
                    currentAction = StartCoroutine(ACT_BlessedWall(-1, interval));
                }
                else
                {
                    float distance = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);
                    currentAction = StartCoroutine(ACT_BlessedWallDoubleDirection(distance, interval));
                }

                break;
            }
            case DragaliaEnemyActionTypes.HB1004.buff:
            {
                float atk = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                float dmg = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);
                currentAction = StartCoroutine(ACT_ConsecratedBrilliance(atk,dmg,interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.projectiles:
            {
                int type = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                if (type == 1)
                {
                    currentAction = StartCoroutine(ACT_ProjectilesAttackI(interval));
                }else if (type == 2)
                {
                    currentAction = StartCoroutine(ACT_ProjectilesAttackII(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_ProjectilesAttackIII(interval));
                }
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.celestial:
            {
                int type = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_VertiStars(type, interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.ball:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DivineRadiance(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.glare:
            {
                int needGround = int.Parse(_currentActionStage.args[0]);
                int fix = int.Parse(_currentActionStage.args[1]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);

                if (fix == 1)
                {
                    currentAction = StartCoroutine(ACT_StunningGlareFixed(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_StunningGlare(0,needGround,interval));
                }
                break;
            }

        }
        
        
        
        
    }

    


    protected IEnumerator ACT_ComboA(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        if (transform.DistanceX(targetPlayer) > 35f ||
            transform.DistanceY(targetPlayer) > 12f)
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action07(true,true));
            yield return new WaitUntil(() => currentAttackAction == null);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action01());
        }
        else
        {
            currentMoveAction = StartCoroutine
            (enemyController.MoveToSameGround(
                targetPlayer,4,8));
            yield return new WaitUntil(() => currentMoveAction == null);
            enemyController.SetKBRes(999);
            if (TaskSuccess)
            {
                currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action01());
            }
            else
            {
                currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action07(true));
            }
        }
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        ActionEnd();
        //currentDebugVal = 4;
    }
    
    protected IEnumerator ACT_ComboB(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        if (transform.DistanceX(targetPlayer) > 35f ||
            transform.DistanceY(targetPlayer) > 12f)
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action07(true,true));
            yield return new WaitUntil(() => currentAttackAction == null);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action02());
        }
        else
        {
            currentMoveAction = StartCoroutine
            (enemyController.MoveToSameGround(
                targetPlayer,4,8));
            yield return new WaitUntil(() => currentMoveAction == null);
            enemyController.SetKBRes(999);
            if (TaskSuccess)
            {
                currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action02());
            }
            else
            {
                currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action07(true));
            }
        }
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_BlazingFount(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        currentMoveAction = StartCoroutine
        (enemyController.KeepDistanceFromTarget(
            targetPlayer,6,5,35));
        
        
        yield return new WaitUntil(() => currentMoveAction == null);

        enemyController.SetKBRes(999);

        if (status.GetConditionStackNumber((int)BasicCalculation.BattleCondition.PowerOfBonds) > 0)
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action05());
        }
        else
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action03());
        }


        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
        //currentDebugVal = 4;
    }
    
    protected IEnumerator ACT_RingOfAffection(float interval, bool cameraFollow)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);


        if (status.GetConditionStackNumber((int)BasicCalculation.BattleCondition.PowerOfBonds) <= 0)
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action04(cameraFollow));
            yield return new WaitUntil(() => currentAttackAction == null);
            enemyController.SetKBRes(status.knockbackRes);
            yield return new WaitForSeconds(interval);
        }

        
        ActionEnd();
    }

    
    protected IEnumerator ACT_BlessedWall(int dir, float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action06(dir));
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
    }
    
    protected IEnumerator ACT_BlessedWallDoubleDirection(float distance, float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action06_V(distance));
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
    }
    

    /// <summary>
    /// 惩罚用
    /// </summary>
    /// <param name="interval"></param>
    /// <returns></returns>
    protected IEnumerator ACT_StunningGlareFixed(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action07_V());
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
    }
    
    
    protected IEnumerator ACT_StunningGlare(int punishment, int needGround, float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine
        (enemyAttackManager.HB04_Action07(punishment == 0 ? false : true,
            needGround == 0 ? false : true));
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
    }


    protected IEnumerator ACT_WarpToPosition(Vector2 position, float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        if (Mathf.Abs(transform.position.x - position.x) > 1)
        {

            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action11(position));
            yield return new WaitUntil(() => currentAttackAction == null);


            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(interval);
        }

        ActionEnd();
    }
    
    protected IEnumerator ACT_WarpToNear(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action11(Vector2.zero, true));
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
    }
    
    protected IEnumerator ACT_WarpToAvoid(float xpos,float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action11_V(xpos));
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
    }


    protected IEnumerator ACT_ProjectilesAttackIII(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action08());
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
    }
    
    protected IEnumerator ACT_ProjectilesAttackII(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);


        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action10());
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
    }
    
    protected IEnumerator ACT_ProjectilesAttackI(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);


        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action09());
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
    }
    
    protected IEnumerator ACT_VertiStars(int up, float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action11(up == 0 ? false : true));
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
    }


    protected IEnumerator ACT_ConsecratedBrilliance(float atkBuff, float dmgBuff, float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action12(atkBuff,dmgBuff));
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
    }


    protected IEnumerator ACT_DivineRadiance(float interval)
    {
        
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action13());
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
        
    }

    protected virtual void CheckPowerOfBonds()
    {
        var cond =
            status.GetConditionsOfType((int)BasicCalculation.BattleCondition.PowerOfBonds);
        if (cond.Count > 0)
        {
            status.currentHp = 1;
            status.RemoveConditionWithLog(cond[0]);
            status.HPRegenImmediately(0,20 + difficulty * 5);
            status.ImmuneToAllControlAffliction = false;
            totalRevived++;
        }
    }

}
