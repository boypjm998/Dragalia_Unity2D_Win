using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class E2001_minon_BehaviorTree : DragaliaEnemyBehavior
{
    protected EnemyControllerFlying enemyController;
    protected EnemyMoveController_E2001_M enemyAttackManager;
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerFlying>();
        enemyAttackManager = GetComponent<EnemyMoveController_E2001_M>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        
        //Invoke("SetEnable",awakeTime);
    }
    
    

    protected void OnDestroy()
    {
        enemyController.OnMoveFinished -= FinishMove;
        enemyAttackManager.OnAttackFinished -= FinishAttack;
    }

    protected override void UpdateAttack()
    {
        CheckPhase();
        ExcutePhase();
    }
    
    protected override void CheckPhase()
    {
        
    }

    protected override void ExcutePhase()
    {
        if (!isAction)
        {
            DoAction(state,substate);
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
                    currentAction = StartCoroutine(ACT_DashAttack(3f));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_AroundAttack(3f));
                    break;
                case 2:
                    this.substate = 0;
                    break;
            }
            
        }
        




    }

    IEnumerator ACT_DashAttack(float interval)
    {
        isAction = true;
        TaskSuccess = false;
        
        SetTarget(ClosestTarget);
        if (enemyController.CheckTargetDistance(targetPlayer, 10, 10) == false)
        {
            currentMoveAction = StartCoroutine(enemyController.FlyTowardTarget(targetPlayer,1f,Ease.InOutSine));
            yield return new WaitUntil(() => currentMoveAction == null);
        }

        currentAttackAction =
            StartCoroutine(enemyAttackManager.E20011_Action01());
        yield return new WaitUntil(() => currentAttackAction == null);
        yield return new WaitForSeconds(interval);
        isAction = false;
        substate++;
    }

    IEnumerator ACT_AroundAttack(float interval)
    {
        isAction = true;
        TaskSuccess = false;
        
        if (enemyController.CheckTargetDistance(targetPlayer, 3, 3) == false)
        {
            currentMoveAction = StartCoroutine(enemyController.FlyTowardTarget(targetPlayer,1f,Ease.InOutSine));
            yield return new WaitUntil(() => currentMoveAction == null);
        }
        
        
        currentAttackAction =
            StartCoroutine(enemyAttackManager.E20011_Action02());
        yield return new WaitUntil(() => currentAttackAction == null);
        yield return new WaitForSeconds(interval);
        isAction = false;
        substate++;
    }

}
