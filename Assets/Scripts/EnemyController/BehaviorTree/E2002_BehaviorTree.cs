using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2002_BehaviorTree : DragaliaEnemyBehavior
{
    protected EnemyControllerFlying enemyController;
    protected EnemyMoveController_E2002 enemyAttackManager;
    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager = GetComponent<EnemyMoveController_E2002>();
        enemyController = GetComponent<EnemyControllerFlying>();
    }

    protected override void CheckPhase()
    {
        
    }

    protected override void DoAction(int state, int substate)
    {
        currentAction = StartCoroutine(ACT_ApproachGate());

    }

    protected IEnumerator ACT_ApproachGate()
    {
        ActionStart();
        currentAction = StartCoroutine(enemyAttackManager.E2002_Action01());
        yield return new WaitUntil(()=>currentAction == null);
        ActionEnd();
    }
}
