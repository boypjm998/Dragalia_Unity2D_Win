using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB01_M2_BehaviorTree : DragaliaEnemyBehavior
{
    protected EnemyMoveController_DB01_M2 enemyAttackManager;
    protected EnemyControllerHumanoid enemyController;
    private bool destroyInvoked = false;
    public float destroyTime = 20;

    protected override void Awake()
    {
        base.Awake();
        Invoke(nameof(DestroyInvoke),destroyTime);
        enemyAttackManager = GetComponent<EnemyMoveController_DB01_M2>();
        enemyController = GetComponent<EnemyControllerHumanoid>();
    }

    protected override void CheckPhase()
    {
        
    }

    protected override void DoAction(int state, int substate)
    {

        currentAction = StartCoroutine(ACT_ChaseAndBite());
    }

    protected IEnumerator ACT_ChaseAndBite()
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_M2_Action01());
        yield return new WaitUntil(()=>currentAttackAction==null);
        
        currentMoveAction = StartCoroutine
            (enemyController.MoveTowardsTarget(targetPlayer, 4, 99));

        yield return currentMoveAction;

        currentMoveAction = StartCoroutine(enemyController.JumpToTarget(targetPlayer, 3, 3.5f));
        yield return currentMoveAction;
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_M2_Action02(4f, 3f));
        yield return new WaitUntil(()=>currentAttackAction==null);

        ActionEnd();
    }

    void DestroyInvoke()
    {
        destroyInvoked = true;
    }



}
