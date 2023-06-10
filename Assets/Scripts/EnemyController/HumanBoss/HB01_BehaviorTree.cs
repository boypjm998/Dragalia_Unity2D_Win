using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using GameMechanics;

public class HB01_BehaviorTree : DragaliaEnemyBehavior
{
    
    protected EnemyControllerHumanoid enemyController;
    protected EnemyMoveController_HB01 enemyAttackManager;
    
    //public int state;
    
    

    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        enemyAttackManager = GetComponent<EnemyMoveController_HB01>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        
        //Invoke("SetEnable",awakeTime);
    }

    protected void OnDestroy()
    {
        enemyController.OnMoveFinished -= FinishMove;
        enemyAttackManager.OnAttackFinished -= FinishAttack;
    }

    void SetEnable()
    {
        this.enabled = true;
    }

    protected void OnDisable()
    {
        StopAllCoroutines();
        enemyController.DisableMovement();
    }

    
    


    protected override void UpdateAttack()
    {
        CheckPhase();
        ExcutePhase();
    }

    /// <summary>
    /// Swap action phases.
    /// 当满足特定条件时切换行动阶段.
    /// </summary>
    protected override void CheckPhase()
    {
        if (status.currentHp < status.maxHP * 0.8 && state==0)
        {
            state = 1;
            substate = 0;
        }
        if (status.currentHp < status.maxHP * 0.5 && state==1)
        {
            state = 2;
            substate = 0;
        }
    }

    protected override void ExcutePhase()
    {
        if (!isAction)
        {
            DoAction(state, substate);
        }
    }

    

    protected override void DoAction(int state, int substate)
    {
        if (playerAlive == false)
            return;
        if (state == 0)
        {
            switch (substate)
            {
                case 0:
                    currentAction = StartCoroutine(ACT_CarmineRush(2));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_ComboA(2));
                    break;
                case 2:
                    currentAction = StartCoroutine(ACT_ComboB(2));
                    break;
                case 3:
                    currentAction = StartCoroutine(ACT_ComboC(2));
                    break;
                case 4:
                    currentAction = StartCoroutine(ACT_ComboD(1));
                    break;
                case 5:
                    currentAction = StartCoroutine(ACT_CarmineRush(2));
                    break;
                case 6:
                    currentAction = StartCoroutine(ACT_SingleDodgeCombo(1));
                    break;
                case 7:
                    currentAction = StartCoroutine(ACT_FlameRaid(1));
                    break;
                case 8:
                    currentAction = StartCoroutine(ACT_SingleDodgeCombo(2));
                    break;
                case 9:
                    this.substate = 0;
                    break;
                
            }
        }
        else if (state == 1)
        {
            switch (substate)
            {
                case 0:
                    currentAction = StartCoroutine(ACT_SingleDodgeCombo(2));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_FlameRaid(0));
                    break;
                case 2:
                    currentAction = StartCoroutine(ACT_BrightCarmineRush(0,0));
                    break;
                case 3:
                {
                    var checkBuff =
                        status.GetConditionsOfType((int)BasicCalculation.BattleCondition.BlazewolfsRush);
                    if (checkBuff.Count>0)
                    {
                        status.RemoveConditionWithLog(checkBuff[0]);
                        currentAction = StartCoroutine(ACT_SavageFlameRaid(1,-1));
                    }
                    else
                    {
                        this.substate++;
                    }
                    break;
                }
                case 4:
                {
                    var rand = Random.Range(0, 3);
                    if (rand == 0)
                    {
                        currentAction = StartCoroutine(ACT_ComboA(1));
                    }else if (rand == 1)
                    {
                        currentAction = StartCoroutine(ACT_ComboB(1));
                    }else if (rand == 2)
                    {
                        currentAction = StartCoroutine(ACT_ComboC(1));
                    }
                    break;
                }
                case 5:
                    currentAction = StartCoroutine(ACT_ComboD(1));
                    break;
                   
                case 6:
                    currentAction = StartCoroutine(ACT_CarmineRush(0));
                    break;
                case 7:
                    currentAction = StartCoroutine(ACT_SavageFlameRaid(0,0));
                    break;
                case 8:
                {
                    var checkBuff =
                        status.GetConditionsOfType((int)BasicCalculation.BattleCondition.BlazewolfsRush);
                    if (checkBuff.Count > 0)
                    {
                        status.RemoveConditionWithLog(checkBuff[0]);
                        currentAction = StartCoroutine(ACT_BrightCarmineRush(1,-1));
                    }
                    else
                    {
                        this.substate++;
                    }

                    break;
                }
                case 9:
                    currentAction = StartCoroutine(ACT_ComboA(2));
                    break;
                case 10:
                    currentAction = StartCoroutine(ACT_ComboB(1));
                    break;
                case 11:
                    currentAction = StartCoroutine(ACT_ComboC(1));
                    break;
                case 12:
                    currentAction = StartCoroutine(ACT_ComboD(1));
                    break;
                case 13:
                    this.substate = 0;
                    break;
                
            }
        }
        else if (state == 2)
        {
            switch (substate)
            {
                case 0:
                    currentAction = StartCoroutine(ACT_CrimsonInferno(2,0));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_ComboA(2));
                    break;
                case 2:
                    currentAction = StartCoroutine(ACT_ComboB(2));
                    break;
                case 3:
                    currentAction = StartCoroutine(ACT_ComboC(2));
                    break;
                case 4:
                    currentAction = StartCoroutine(ACT_ComboD(1));
                    break;
                case 5:
                    currentAction = StartCoroutine(ACT_BlazingEnhancement(2));
                    break;
                case 6:
                    currentAction = StartCoroutine(ACT_CarmineRush(0));
                    break;
                case 7:
                    currentAction = StartCoroutine(ACT_SavageFlameRaid(0,0));
                    break;
                case 8:
                {
                    var checkBuff =
                        status.GetConditionsOfType((int)BasicCalculation.BattleCondition.BlazewolfsRush);
                    if (checkBuff.Count > 0)
                    {
                        status.RemoveConditionWithLog(checkBuff[0]);
                        currentAction = StartCoroutine(ACT_BrightCarmineRush(1,-1));
                    }
                    else
                    {
                        this.substate++;
                    }

                    break;
                }
                case 9:
                    currentAction = StartCoroutine(ACT_ComboA(2));
                    break;
                case 10:
                    currentAction = StartCoroutine(ACT_ComboB(2));
                    break;
                case 11:
                    currentAction = StartCoroutine(ACT_ComboC(2));
                    break;
                case 12:
                    currentAction = StartCoroutine(ACT_ComboD(1));
                    break;
                case 13:
                    currentAction = StartCoroutine(ACT_BlazingEnhancement(2));
                    break;
                case 14:
                    currentAction = StartCoroutine(ACT_FlameRaid(0));
                    break;
                case 15:
                    currentAction = StartCoroutine(ACT_BrightCarmineRush(0,0));
                    break;
                case 16:
                {
                    var checkBuff =
                        status.GetConditionsOfType((int)BasicCalculation.BattleCondition.BlazewolfsRush);
                    if (checkBuff.Count>0)
                    {
                        status.RemoveConditionWithLog(checkBuff[0]);
                        currentAction = StartCoroutine(ACT_SavageFlameRaid(1,-1));
                    }
                    else
                    {
                        this.substate++;
                    }
                    break;
                }
                case 17:
                    this.substate = 1;
                    break;
                
            }
        }
    }

    protected IEnumerator ACT_CarmineRush(float interval)
    {
        SetTarget(FurthestTarget);
        
        MoveTowardTarget(targetPlayer,3,12f);
        
        yield return new WaitUntil
            (() => (currentMoveAction == null));
        
        TurnAction();
        enemyController.SetKBRes(999);
        yield return null;
        
        if (TaskSuccess)
        {
            if (interval == 0)
            { 
                enemyAttackManager.GenerateWarningPrefab(8,transform.position,
                    enemyController.facedir==1?Quaternion.identity:
                        Quaternion.Euler(0,180,0),enemyAttackManager.RangedAttackFXLayer.transform);
            }
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action04());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(() => currentAttackAction == null);
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action04());

        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        substate++;
        isAction = false;
        
    }
    
    protected IEnumerator ACT_BrightCarmineRush(float interval, float moveTime)
    {
        SetTarget(FurthestTarget);
        if (moveTime > 0)
        {
            MoveTowardTarget(targetPlayer,moveTime,15f);
        
            yield return new WaitUntil
                (() => (currentMoveAction == null));
        }
        else
        {
            isAction = true;
            if (moveTime < 0)
            {
                TaskSuccess = true;
            }
            else
            {
                TaskSuccess = enemyController.CheckTargetDistance(targetPlayer,25f, 99f);
            }
        }

        enemyController.SetKBRes(999);
        TurnAction();
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action09());
        }
        else
        {
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(() => currentAttackAction == null);
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action04());

        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        substate++;
        isAction = false;
        
    }

    protected IEnumerator ACT_ComboA(float interval)
    {
        SetTarget(ClosestTarget);
        MoveTowardTarget(targetPlayer,3,10f);
        
        
        yield return new WaitUntil
            (() => (currentMoveAction == null));
        
        TurnAction();
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
            //substate++;
            //isAction = false;
            //yield break;
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        //Wait For Attack Interval
        yield return new WaitForSeconds(interval);
        substate++;
        isAction = false;
    }

    protected IEnumerator ACT_ComboB(float interval)
    {
        SetTarget(ClosestTarget);
        MoveTowardTarget(targetPlayer,3,4f);
        
        yield return new WaitUntil
            (() => (currentMoveAction == null));
        
        TurnAction();
        enemyController.SetKBRes(999);
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action06());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        //Wait For Attack Interval
        yield return new WaitForSeconds(interval);
        substate++;
        isAction = false;
    }
    
    protected IEnumerator ACT_ComboC(float interval)
    {
        SetTarget(ClosestTarget);
        MoveTowardTarget(targetPlayer,3,6f);
        
        yield return new WaitUntil
            (() => (currentMoveAction == null));
        
        TurnAction();
        enemyController.SetKBRes(999);
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action07());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(() => currentAttackAction == null);
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action07());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        //Wait For Attack Interval
        yield return new WaitForSeconds(interval);
        substate++;
        isAction = false;
    }

    protected IEnumerator ACT_ComboD(float interval)
    {
        SetTarget(ClosestTarget);
        MoveTowardTarget(targetPlayer,3,3f);
        
        yield return new WaitUntil
            (() => (currentMoveAction == null));
        
        TurnAction();
        enemyController.SetKBRes(999);
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action05());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(() => currentAttackAction == null);
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action05());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        //Wait For Attack Interval
        yield return new WaitForSeconds(interval);
        substate++;
        isAction = false;
    }

    protected IEnumerator ACT_SingleDodgeCombo(float interval)
    {
        SetTarget(ClosestTarget);
        //Do Move Action
        MoveTowardTarget(targetPlayer,5,3.5f,2.5f,5f,false);

        yield return new WaitUntil
            (() => (currentMoveAction == null));
        
        TurnAction();
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
            //substate++;
            //isAction = false;
            //yield break;
        }

        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        //Wait For Attack Interval
        yield return new WaitForSeconds(interval);
        substate++;
        isAction = false;
    }

    protected IEnumerator ACT_FlameRaid(float interval)
    {
        SetTarget(ClosestTarget);
        MoveTowardTarget(targetPlayer,2.5f,5f,6f,7f,true);
        
        yield return new WaitUntil
            (() => (currentMoveAction == null));
        
        TurnAction();
        enemyController.SetKBRes(999);
        yield return null;
        
        if (TaskSuccess)
        {
            

            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action08());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(() => currentAttackAction == null);
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action08());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        //Wait For Attack Interval
        yield return new WaitForSeconds(interval);
        // if (interval == 0)
        // { 
        //     enemyAttackManager.GenerateWarningPrefab(9,transform.position,
        //         enemyController.facedir==1?Quaternion.identity:
        //             Quaternion.Euler(0,180,0),enemyAttackManager.RangedAttackFXLayer.transform);
        // }
        substate++;
        isAction = false;
        
    }
    protected IEnumerator ACT_SavageFlameRaid(float interval, float moveTime)
    {
        if (moveTime > 0)
        {
            MoveTowardTarget(targetPlayer,moveTime,10f,8f,10f,true);
        
            yield return new WaitUntil
                (() => (currentMoveAction == null));
        }
        else
        {
            isAction = true;
            if (moveTime < 0)
            {
                TaskSuccess = true;
            }
            else
            {
                TaskSuccess = enemyController.CheckTargetDistance(targetPlayer,15f, 8f);
            }
        }

        enemyController.SetKBRes(999);
        TurnAction();
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action10());
        }
        else
        {
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            yield return new WaitUntil(() => currentAttackAction == null);
            

            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action08());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        //Wait For Attack Interval
        yield return new WaitForSeconds(interval);
        substate++;
        isAction = false;
        
    }
    
    protected IEnumerator ACT_CrimsonInferno(float interval, float moveTime)
    {
        if (moveTime > 0)
        {
            MoveTowardTarget(targetPlayer,moveTime,99f,99f,99f,true);
        
            yield return new WaitUntil
                (() => (currentMoveAction == null));
        }
        else
        {
            isAction = true;
            if (moveTime < 0)
            {
                TaskSuccess = true;
            }
            else
            {
                TaskSuccess = enemyController.CheckTargetDistance(targetPlayer,99f, 99f);
            }
        }

        enemyController.SetKBRes(999);
        TurnAction();
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action11());
        }
        else
        {
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action11());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        //Wait For Attack Interval
        yield return new WaitForSeconds(interval);
        substate++;
        isAction = false;
        
    }
    protected IEnumerator ACT_BlazingEnhancement(float interval)
    {
        isAction = true;
        
        TaskSuccess = true;

        enemyController.SetKBRes(999);
        TurnAction();
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action13());
        }
        else
        {
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action13());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        //Wait For Attack Interval
        yield return new WaitForSeconds(interval);
        substate++;
        isAction = false;
        
    }
    
    protected IEnumerator ACT_BlazingEnhancement_2(float interval)
    {
        isAction = true;
        
        TaskSuccess = true;

        enemyController.SetKBRes(999);
        TurnAction();
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action13_2());
        }
        else
        {
            
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action13_2());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        //Wait For Attack Interval
        yield return new WaitForSeconds(interval);
        substate++;
        isAction = false;
        
    }

    void MoveTowardTarget(GameObject target, float maxfollowtime, float arriveX, float arriveY, float startDistance,bool needGround)
    {
        
        isAction = true;
        TaskSuccess = false;
        if (needGround)
        {
            currentMoveAction = 
                StartCoroutine
                (enemyController.MoveTowardTargetOnGround
                    (target, maxfollowtime, arriveX,arriveY, startDistance));
        }
        else
        {
            currentMoveAction = 
                StartCoroutine
                (enemyController.MoveTowardTarget
                    (target, maxfollowtime, arriveX,arriveY, startDistance));
        }

        
    }
    
    void MoveTowardTarget(GameObject target, float maxfollowtime, float arriveX)
    {
        
        isAction = true;
        TaskSuccess = false;
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (target, maxfollowtime, arriveX));
    }

    void TurnAction()
    {
        if (enemyController.VerticalMoveRoutine != null)
        {
            enemyController.StopCoroutine(enemyController.VerticalMoveRoutine);
            enemyController.VerticalMoveRoutine = null;
        }

        //Danger
        
        //Do Attack Action
        enemyController.TurnMove(targetPlayer);
    }


}
