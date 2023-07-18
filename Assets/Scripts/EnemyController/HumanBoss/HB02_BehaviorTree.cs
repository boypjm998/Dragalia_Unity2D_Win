using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class HB02_BehaviorTree : DragaliaEnemyBehavior
{
    protected EnemyControllerHumanoid enemyController;
    protected EnemyMoveController_HB02 enemyAttackManager;
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        enemyAttackManager = GetComponent<EnemyMoveController_HB02>();
    }

    // Start is called before the first frame update
    

    // Update is called once per frame
    protected override void UpdateAttack()
    {
        CheckPhase();
        ExcutePhase();
    }

    protected override void CheckPhase()
    {
        if (state == 0 && ((float)status.currentHp / status.maxHP) < 0.8f)
        {
            state = 1;
            substate = 0;
        }
        if (state == 1 && ((float)status.currentHp / status.maxHP) < 0.45f)
        {
            state = 2;
            substate = 0;
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
                    currentAction = StartCoroutine(ACT_ComboA(1f));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_ApproachTarget(10f, 3f));
                    break;
                case 2:
                    currentAction = StartCoroutine(ACT_GloriousSanctuary(2f));
                    break;
                case 3:
                    currentAction = StartCoroutine(ACT_TwilightMoon(1f));
                    break;
                
                case 4:
                    currentAction = StartCoroutine(ACT_ComboB(2f));
                    break;
                case 5:
                    currentAction = StartCoroutine(ACT_ComboC(1f));
                    break;
                case 6:
                    currentAction = StartCoroutine(ACT_ApproachTarget(3f, .5f, 4f, 5f));
                    break;
                case 7:
                    currentAction = StartCoroutine(ACT_DashAttack(1f));
                    break;
                case 8:
                    currentAction = StartCoroutine(ACT_KeepDistance(8f, 15f, 3f));
                    break;
                case 9:
                    currentAction = StartCoroutine(ACT_ComboA(1f));
                    break;
                case 10:
                    currentAction = StartCoroutine(ACT_TwilightCrown(0f));
                    break;
                case 11:
                    currentAction = StartCoroutine(ACT_ApproachTarget(6f, 1f, 4.5f));
                    break;
                case 12:
                    this.substate = 0;
                    break;
                    
            }
        }
        else if (state == 1)
        {
            switch (substate)
            {
                case 0:
                    currentAction = StartCoroutine(ACT_HolyCrown(0f));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_CelestialPrayer(2f));
                    break;
                case 2:
                    currentAction = StartCoroutine(ACT_KeepDistance(10f,20f,2f));
                    break;
                case 3:
                    currentAction = StartCoroutine(ACT_ComboA(1f));
                    break;
                case 4:
                    currentAction = StartCoroutine(ACT_ComboB(1f));
                    break;
                case 5:
                    currentAction = StartCoroutine(ACT_ComboC(1f));
                    break;
                case 6:
                    currentAction = StartCoroutine(ACT_TwilightMoon(1f));
                    break;
                case 7:
                    currentAction = StartCoroutine(ACT_ApproachTarget(4f, 3f));
                    break;
                case 8:
                    currentAction = StartCoroutine(ACT_TwilightCrown(2f));
                    break;
                case 9:
                    currentAction = StartCoroutine(ACT_ApproachTarget(6f, 1f, 3.5f));
                    break;
                case 10:
                    currentAction = StartCoroutine(ACT_TwilightMoon(2f));
                    break;
                case 11:
                    currentAction = StartCoroutine(ACT_HolyCrown(2f));
                    break;
                case 12:
                    currentAction = StartCoroutine(ACT_TwilightMoon(2f));
                    break;
                case 13:
                    currentAction = StartCoroutine(ACT_ApproachTarget(12f,3f));
                    break;
                case 14:
                    currentAction = StartCoroutine(ACT_GloriousSanctuary(2f));
                    break;
                case 15:
                    currentAction = StartCoroutine(ACT_TwilightMoon(1f));
                    break;
                case 16:
                    currentAction = StartCoroutine(ACT_TwilightCrown(0.25f));
                    break;
                case 17:
                    currentAction = StartCoroutine(ACT_CelestialPrayer(2f));
                    break;
                case 18:
                    this.substate = 0;
                    break;
                    


            }
        }
        else if (state == 2)
        {
            switch (substate)
            {
                case 0:
                    currentAction = StartCoroutine(ACT_PhaseShift(4.5f));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_EarthBarrier(2f));
                    break;
                case 2:
                    currentAction = StartCoroutine(ACT_FaithEnhancement(0f));
                    break;
                case 3:
                    currentAction = StartCoroutine(ACT_WarpAttack(1.75f));
                    break;
                case 4:
                    currentAction = StartCoroutine(ACT_CombinedTwilightAttack(0.75f));
                    break;
                case 5:
                    currentAction = StartCoroutine(ACT_CelestialPrayer(2f));
                    break;
                case 6:
                    currentAction = StartCoroutine(ACT_KeepDistance(10f,20f,2f));
                    break;
                case 7:
                    currentAction = StartCoroutine(ACT_TwilightCrown(1f));
                    break;
                case 8:
                    currentAction = StartCoroutine(ACT_GloriousSanctuary(0f));
                    break;
                case 9:
                    currentAction = StartCoroutine(ACT_ApproachTarget(6f, 1f, 2f));
                    break;
                case 10:
                    currentAction = StartCoroutine(ACT_HolyCrown(1f));
                    break;
                case 11:
                    currentAction = StartCoroutine(ACT_CelestialPrayer(2f));
                    break;
                case 12:
                    currentAction = StartCoroutine(ACT_ApproachTarget(8f,3f));
                    break;
                case 13:
                    currentAction = StartCoroutine(ACT_ComboA(2f));
                    break;
                case 14:
                    currentAction = StartCoroutine(ACT_ComboC(1f));
                    break;
                case 15:
                    currentAction = StartCoroutine(ACT_ApproachTarget(3f, .25f, 4f, 5f));
                    break;
                case 16:
                    currentAction = StartCoroutine(ACT_DashAttack(1f));
                    break;
                case 17:
                    currentAction = StartCoroutine(ACT_TwilightMoon(2f));
                    break;
                case 18:
                    currentAction = StartCoroutine(ACT_ComboB(1f));
                    break;
                case 19:
                    currentAction = StartCoroutine(ACT_GloriousSanctuary(3f));
                    break;
                case 20:
                    this.substate = 1;
                    break;

            }
        }
    }

    protected IEnumerator ACT_KeepDistance(float distanceMin,float distanceMax,float followTime)
    {
        ActionStart();
        currentMoveAction = 
            StartCoroutine(enemyController.KeepDistanceFromTarget(targetPlayer,followTime,distanceMin,distanceMax));
        yield return new WaitUntil(()=>currentMoveAction == null);
        ActionEnd();
    }

    /// <summary>
    /// X轴和Y轴同时靠近目标，但不考虑路线。
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ACT_ApproachTarget(float arriveDistanceX,float arriveDistanceY,float triggerDistance, float followTime)
    {
        ActionStart();
        currentMoveAction =
            StartCoroutine(enemyController.MoveTowardTarget
                (targetPlayer, followTime, arriveDistanceX,arriveDistanceY, triggerDistance));
        yield return currentMoveAction;
        currentMoveAction = null;
        ActionEnd();
    }
    
    /// <summary>
    /// 双轴追击目标，并且不会在满足条件后停下。
    /// </summary>
    /// <param name="arriveDistanceX"></param>
    /// <param name="arriveDistanceY"></param>
    /// <param name="triggerDistance"></param>
    /// <param name="followTime"></param>
    /// <returns></returns>
    protected IEnumerator ACT_ApproachTarget(float arriveDistanceX,float arriveDistanceY, float followTime)
    {
        ActionStart();
        currentMoveAction =
            StartCoroutine(enemyController.MoveTowardTarget
                (targetPlayer, followTime, arriveDistanceX,arriveDistanceY, 0,true));
        yield return currentMoveAction;
        currentMoveAction = null;
        ActionEnd();
    }
    
    /// <summary>
    /// 无视Y轴距离，只在X轴上靠近目标
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ACT_ApproachTarget(float arriveDistance,float maxFollowTime)
    {
        ActionStart();
        currentMoveAction =
            StartCoroutine(enemyController.MoveToSameGround(targetPlayer, maxFollowTime,arriveDistance));
        yield return currentMoveAction;
        currentMoveAction = null;
        ActionEnd();
    }

    protected IEnumerator ACT_ComboA(float interval)
    {
        ActionStart();
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action01());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_ComboB(float interval)
    {
        ActionStart();
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action03());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_ComboC(float interval)
    {
        ActionStart();
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action04());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_DashAttack(float interval)
    {
        ActionStart();
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action05());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_GloriousSanctuary(float interval)
    {
        ActionStart();
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action02());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_HolyCrown(float interval)
    {
        ActionStart();
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action06());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_TwilightMoon(float interval)
    {
        ActionStart();
        if (status.GetConditionsOfType((int)BasicCalculation.BattleCondition.TwilightMoon).Count > 0)
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action07());
            yield return new WaitUntil(()=>currentAttackAction == null);
            enemyController.SetKBRes(status.knockbackRes);
            yield return new WaitForSeconds(interval);
        }
        ActionEnd();
    }

    protected IEnumerator ACT_TwilightCrown(float interval)
    {
        ActionStart();
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action08());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_CelestialPrayer(float interval)
    {
        ActionStart();
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action09());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_WarpAttack(float interval)
    {
        ActionStart();
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action10());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_FaithEnhancement(float interval)
    {
        ActionStart();
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action11());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_EarthBarrier(float interval)
    {
        ActionStart();
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action12());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_CombinedTwilightAttack(float interval)
    {
        ActionStart();
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action13());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_PhaseShift(float interval)
    {
        ActionStart();
        enemyAttackManager.HB02_PhaseShift(interval);
        yield return new WaitForSeconds(interval);
        //currentAttackAction = null;
        enemyController.SetKBRes(status.knockbackRes);
        ActionEnd();
    }
}
