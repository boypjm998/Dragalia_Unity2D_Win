using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using TMPro;
using UnityEngine;

public class HB03_M1_BehaviorTree : DragaliaEnemyBehavior
{
    private EnemyControllerHumanoid enemyController;
    private EnemyMoveController_HB03_M1 attackManager;
    
    bool taunt = false;
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        attackManager = GetComponent<EnemyMoveController_HB03_M1>();
        enemyController.OnMoveFinished += FinishMove;
        enemyController.OnBeingCountered += CounterPunish;
        print("Awaken");

    }

    


    protected override void DoAction(int state, int substate)
    {

        if(playerAlive == false)
            return;

        if (!taunt)
        {
            taunt = true;
            GrantTaunt();
        }


        switch (substate)
        {
            case 0:
            {
                
                currentAction = StartCoroutine(ACT_GenesisCirclet(1.5f,true));
                break;
            }
            case 1:
            {
                currentAction = StartCoroutine(ACT_SuperForceStrike(2f));
                break;
            }
            case 2:
            {
                currentAction = StartCoroutine(ACT_ForceStrike(1f));
                break;
            }
            case 3:
            {
                currentAction = StartCoroutine(ACT_EntireComboTest(1f));
                break;
            }
            case 4:
            {
                currentAction = StartCoroutine(ACT_GenesisCirclet(2f));
                break;
            }
            case 5:
            {
                currentAction = StartCoroutine(ACT_ForceStrike(2f));
                break;
            }
            case 6:
            {
                currentAction = StartCoroutine(ACT_EntireComboTest(2f));
                break;
            }
            case 7:
            {
                currentAction = StartCoroutine(ACT_ForceStrike(1f));
                break;
            }
            case 8:
            {
                currentAction = StartCoroutine(ACT_SuperForceStrike(2f));
                break;
            }
            case 9:
            {
                this.substate = 3;
                break;
            }
        }
        
    }

    protected override void CheckPhase()
    {
        
    }

    public IEnumerator ACT_EntireComboTest(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        currentMoveAction = StartCoroutine
        (enemyController.MoveToSameGround(
            targetPlayer,4,3.5f));
        
        yield return new WaitUntil(() => currentMoveAction == null);
        
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(attackManager.HB03M1_Action02_Test());
        
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
        
    }
    
    public IEnumerator ACT_SuperForceStrike(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        currentMoveAction = StartCoroutine
        (enemyController.MoveToSameGround(
            targetPlayer,4,6f));
        
        yield return new WaitUntil(() => currentMoveAction == null);
        
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(attackManager.HB03M1_Action01(true));
        
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
        
    }
    
    public IEnumerator ACT_ForceStrike(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        currentMoveAction = StartCoroutine
        (enemyController.MoveToSameGround(
            targetPlayer,4,6f));
        
        yield return new WaitUntil(() => currentMoveAction == null);
        
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(attackManager.HB03M1_Action01(false));
        
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
        
    }

    public IEnumerator ACT_GenesisCirclet(float interval,bool start = false)
    {
        ActionStart();

        if (start)
        {
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        currentMoveAction = StartCoroutine
        (enemyController.MoveToSameGround(
            targetPlayer,5,5f));
        
        yield return new WaitUntil(() => currentMoveAction == null);
        
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(attackManager.HB03M1_Action04());
        
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    public void GrantTaunt()
    {
        status.ObtainTimerBuff((int)BasicCalculation.BattleCondition.Taunt, -1f,-1f,1,-1);
        attackManager.PlayVoice(1);
    }

    protected void CounterPunish()
    {
        status.ObtainTimerBuff((int)BasicCalculation.BattleCondition.Vulnerable, 10f, -1,
            3, 8103301);
    }




}
