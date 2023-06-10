using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class E2001_BehaviorTree : DragaliaEnemyBehavior
{
    protected EnemyControllerFlying enemyController;
    protected EnemyMoveController_E2001 enemyAttackManager;
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerFlying>();
        enemyAttackManager = GetComponent<EnemyMoveController_E2001>();
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
        if(state == 0 && status.currentHp < status.maxHP * 0.95f)
        {
            status.currentHp += 65536;
        }
        if(state == 2 && (status as SpecialStatusManager).broken)
        {
            //state = 3;
            //substate = 0;
            (status as SpecialStatusManager).ODLock = true;
        }

        if (state <= 2 && status.currentHp < status.maxHP * 0.2f)
        {
            var potency = (status.currentHp * 100f) / (status.maxHP * 1f);
            status.HPRegenImmediatelyWithoutRandom(0,30.1f-potency);
        }
        if (state == 3 && status.currentHp < status.maxHP * 0.1f)
        {
            status.currentHp = (int)(status.maxHP * 0.1f);
        }
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
        if (status is SpecialStatusManager)
        {
            if((status as SpecialStatusManager).broken)
                return;
        }


        if (state == 0)
        {
            switch (substate)
            {
                case 0:
                    currentAction = StartCoroutine(ACT_AroundAttack(4));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_LockAttack(5));
                    break;
                case 2:
                    currentAction = StartCoroutine(ACT_AroundAttack(4));
                    TutorialLevelManager.Instance.StartCutScene(2);
                    break;

                case 3:
                    currentAction = StartCoroutine(ACT_LockAttack(5));
                    //TutorialLevelManager.Instance.StartCutScene(2);
                    break;
                case 4:
                    this.substate = 5;
                    break;
                case 5:
                    TutorialLevelManager.Instance.StartCutScene(3);
                    isAction = true;
                    this.substate = 6;
                    break;
            }
        }
        else if (state == 1)
        {
            switch (substate)
            {
                case 0:
                    currentAction = StartCoroutine(ACT_PillarAttackForce(1));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_LockAttack(4));
                    break;
                case 2:
                    currentAction = StartCoroutine(ACT_AroundAttack(4));
                    break;
                case 3:
                    currentAction = StartCoroutine(ACT_AroundElectricRing(0));
                    break;
                case 4:
                    currentAction = StartCoroutine(ACT_FastApproach(2));
                    break;
                case 5:
                    currentAction = StartCoroutine(ACT_AroundAttack(5));
                    break;
                case 6:
                    this.substate = 1;
                    break;
                
            }
        }
        else if (state == 2)
        {
            switch (substate)
            {
                case 0:
                    currentAction = StartCoroutine(ACT_VoidCurseBurst(3));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_LockAttack(4));
                    break;
                case 2:
                    currentAction = StartCoroutine(ACT_AroundAttack(4));
                    break;
                case 3:
                    currentAction = StartCoroutine(ACT_AroundElectricRing(0));
                    break;
                case 4:
                    currentAction = StartCoroutine(ACT_FastApproach(1));
                    break;
                case 5:
                    currentAction = StartCoroutine(ACT_FastApproach(1));
                    break;
                case 6:
                    currentAction = StartCoroutine(ACT_LockAttack(5));
                    break;
                case 7:
                    currentAction = StartCoroutine(ACT_LockAttack(4));
                    break;
                case 8:
                    currentAction = StartCoroutine(ACT_AroundAttack(4));
                    break;
                case 9:
                    currentAction = StartCoroutine(ACT_AroundElectricRing(0));
                    break;
                case 10:
                    currentAction = StartCoroutine(ACT_FastApproach(1));
                    break;
                case 11:
                    currentAction = StartCoroutine(ACT_PillarAttack(2));
                    break;
                case 12:
                    this.substate = 7;
                    break;
                
            }
        }
        else if (state == 3)
        {
            switch (this.substate)
            {
                case 0:
                    currentAction = StartCoroutine(ACT_FullScreenBurst(2));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_SummonMinions(4));
                    break;
                case 2:
                    currentAction = StartCoroutine(ACT_LockAttack(2));
                    break;
                case 3:
                    currentAction = StartCoroutine(ACT_FastApproach(2));
                    break;
                case 4:
                    currentAction = StartCoroutine(ACT_FastAroundAttack(3));
                    break;
                case 5:
                    currentAction = StartCoroutine(ACT_AroundElectricRing(2));
                    break;
                case 6:
                    currentAction = StartCoroutine(ACT_PillarAttack(2));
                    break;
                case 7:
                    currentAction = StartCoroutine(ACT_FastAroundAttack(3));
                    break;
                case 8:
                    currentAction = StartCoroutine(ACT_LockAttack(1));
                    break;
                case 9:
                    currentAction = StartCoroutine(ACT_AroundElectric(5));
                    break;
                case 10:
                    currentAction = StartCoroutine(ACT_FastAroundAttack(3));
                    break;
                case 11:
                    currentAction = StartCoroutine(ACT_AroundElectricRing(0));
                    break;
                case 12:
                    currentAction = StartCoroutine(ACT_FastApproach(2));
                    break;
                case 13:
                    currentAction = StartCoroutine(ACT_AroundElectric(2));
                    break;
                case 14:
                    this.substate = 1;
                    break;
                
                    
            }
        }




    }
    
    IEnumerator ACT_AroundAttack(int interval)
    {
        //TODO
        isAction = true;
        TaskSuccess = false;
        currentAttackAction =
            StartCoroutine(enemyAttackManager.E2001_Action01());
        yield return new WaitUntil(() => currentAttackAction == null);
        isAction = false;
        substate++;
    }

    IEnumerator ACT_FastAroundAttack(int interval)
    {
        isAction = true;
        TaskSuccess = false;
        if (enemyController.CheckTargetDistance(targetPlayer, 999, 10) == false)
        {
            currentMoveAction = StartCoroutine(enemyController.FlyTowardTarget(targetPlayer,2f,Ease.InOutSine));
            yield return new WaitUntil(() => currentMoveAction == null);
        }
        
        currentAttackAction =
            StartCoroutine(enemyAttackManager.E2001_Action01_2());
        yield return new WaitUntil(() => currentAttackAction == null);
        yield return new WaitForSeconds(interval);
        isAction = false;
        substate++;
    }

    IEnumerator ACT_LockAttack(int interval)
    {
        //TODO
        isAction = true;
        TaskSuccess = false;
        currentAttackAction =
            StartCoroutine(enemyAttackManager.E2001_Action02());
        yield return new WaitUntil(() => currentAttackAction == null);
        isAction = false;
        substate++;
    }

    IEnumerator ACT_FastApproach(int interval)
    {
        isAction = true;
        TaskSuccess = false;
        currentMoveAction = StartCoroutine(enemyController.FlyTowardTarget(targetPlayer, 1f, Ease.InOutSine));
        yield return new WaitUntil(()=>currentMoveAction == null);
        yield return new WaitForSeconds(interval);
        isAction = false;
        substate++;
    }

    IEnumerator ACT_PillarAttackForce(int interval)
    {
        //TODO
        isAction = true;
        TaskSuccess = false;
        currentAttackAction =
            StartCoroutine(enemyAttackManager.E2001_Action03_1());
        yield return new WaitUntil(() => currentAttackAction == null);
        isAction = false;
        substate++;
    }
    
    IEnumerator ACT_PillarAttack(int interval)
    {
        //TODO
        isAction = true;
        TaskSuccess = false;
        currentAttackAction =
            StartCoroutine(enemyAttackManager.E2001_Action03_2());
        yield return new WaitUntil(() => currentAttackAction == null);
        yield return new WaitForSeconds(interval);
        isAction = false;
        substate++;
    }

    IEnumerator ACT_AroundElectricRing(int interval)
    {
        isAction = true;
        TaskSuccess = false;
        currentAttackAction =
            StartCoroutine(enemyAttackManager.E2001_Action04());
        yield return new WaitUntil(() => currentAttackAction == null);
        isAction = false;
        substate++;
    }
    
    IEnumerator ACT_VoidCurseBurst(int interval)
    {
        isAction = true;
        TaskSuccess = false;
        currentAttackAction =
            StartCoroutine(enemyAttackManager.E2001_Action05());
        yield return new WaitUntil(() => currentAttackAction == null);
        yield return new WaitForSeconds(interval);
        isAction = false;
        substate++;
    }

    IEnumerator ACT_FullScreenBurst(int interval)
    {
        isAction = true;
        TaskSuccess = false;
        currentAttackAction =
            StartCoroutine(enemyAttackManager.E2001_Action05(false));
        yield return new WaitUntil(() => currentAttackAction == null);
        yield return new WaitForSeconds(interval);
        isAction = false;
        substate++;
    }

    
    IEnumerator ACT_AroundElectric(float interval)
    {
        isAction = true;
        TaskSuccess = false;
        
        if (enemyController.CheckTargetDistance(targetPlayer, 3, 3) == false)
        {
            currentMoveAction = StartCoroutine(enemyController.FlyTowardTarget(targetPlayer,1f,Ease.InOutSine));
            yield return new WaitUntil(() => currentMoveAction == null);
        }
        
        
        currentAttackAction =
            StartCoroutine(enemyAttackManager.E2001_Action07());
        yield return new WaitUntil(() => currentAttackAction == null);
        yield return new WaitForSeconds(interval);
        isAction = false;
        substate++;
    }

    IEnumerator ACT_SummonMinions(int interval)
    {
        isAction = true;
        TaskSuccess = false;
        currentAttackAction =
            StartCoroutine(enemyAttackManager.E2001_Action06());
        yield return new WaitUntil(() => currentAttackAction == null);
        yield return new WaitForSeconds(interval);
        isAction = false;
        substate++;
    }

}
