using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DB01_M1_BehaviorTree : DragaliaEnemyBehavior
{
    protected EnemyControllerFlyingHigh enemyController;
    protected EnemyMoveController_DB01_M1 enemyAttackManager;
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyAttackManager = GetComponent<EnemyMoveController_DB01_M1>();
    }

    protected override void CheckPhase()
    {
        
    }

    protected override void DoAction(int state, int substate)
    {
        if(!playerAlive)
            return;

        switch (substate)
        {
            case 0:
                currentAction = StartCoroutine(ACT_KeepDistance(10f));
                break;
            case 1:
                currentAction = StartCoroutine(ACT_Combo(4f));
                break;
            case 2:
                currentAction = StartCoroutine(ACT_KeepDistance(10f));
                break;
            case 3:
                currentAction = StartCoroutine(ACT_Combo(4f));
                break;
            case 4:
                currentAction = StartCoroutine(ACT_KeepDistance(10f));
                break;
            case 5:
                currentAction = StartCoroutine(ACT_Combo(3f));
                break;
            case 6:
                currentAction = StartCoroutine(ACT_KeepDistance(1f));
                break;
            case 7:
                currentAction = StartCoroutine(ACT_ButterflyFade());
                break;
            case 8:
                this.substate = 99;
                break;
        }
        
    }


    protected IEnumerator ACT_KeepDistance(float distanceX)
    {
        ActionStart();

        Vector2 endPoint;
        var randDistance = Random.Range(1f, 3f);

        if (transform.position.x < targetPlayer.transform.position.x)
        {
            endPoint.x = targetPlayer.transform.position.x - distanceX - randDistance;
        }else
        {
            endPoint.x = targetPlayer.transform.position.x + distanceX + randDistance;
        }
        endPoint.y = targetPlayer.transform.position.y;

        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint(endPoint,
                Vector2.Distance(endPoint,transform.position)/7,
                Ease.Linear));

        yield return currentMoveAction;
        
        ActionEnd();
    }

    protected IEnumerator ACT_Combo(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_M1_Action01());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_ButterflyFade()
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_M1_Action02());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        ActionEnd();
        Destroy(gameObject);
        
    }
}
