using System.Collections;
using UnityEngine;


public class E3001_BehaviorTree : DragaliaEnemyBehavior {
    
    protected EnemyControllerTherian enemyController;
    protected EnemyMoveController_E3001 enemyAttackManager;
    protected override void Awake()
    {
        base.Awake();
        
        enemyController = GetComponent<EnemyControllerTherian>();
        enemyAttackManager = GetComponent<EnemyMoveController_E3001>();
        
    }

    protected override void DoAction(int state, int substate)
    {
        if(!playerAlive)
            return;
        
        
        currentAction = StartCoroutine(ACT_ForcingSmash());
        
    }

    protected override void CheckPhase()
    {
        
    }

    public IEnumerator ACT_ForcingSmash()
    {
        ActionStart(); 
        print("重新开始forcingSmash");
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.E3001_Action01());
        
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(1);
        ActionEnd();
    }


}
