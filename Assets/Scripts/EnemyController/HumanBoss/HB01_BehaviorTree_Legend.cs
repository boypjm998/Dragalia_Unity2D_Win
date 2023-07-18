using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HB01_BehaviorTree_Legend : DragaliaEnemyBehavior
{
    protected EnemyMoveController_HB01 enemyAttackManager;
    protected EnemyControllerHumanoid enemyController;
    

    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        enemyAttackManager = GetComponent<EnemyMoveController_HB01>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;

        //Invoke("SetEnable",awakeTime);
    }

    protected override void CheckPhase()
    {
        
    }

    protected override void DoAction(int state, int substate)
    {
        
    }

    protected IEnumerator ComboA(float interval)
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
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator CarmineRushDirect(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HB01_Action04());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator MoveToCenterGround(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        currentMoveAction = StartCoroutine(enemyController.
            MoveTowardsTarget(enemyAttackManager.GetAnchoredSensorOfName("MiddleM"),
                0.5f,5));
        yield return new WaitUntil(()=>currentAttackAction == null &&
                                       currentMoveAction == null);
        
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
}
