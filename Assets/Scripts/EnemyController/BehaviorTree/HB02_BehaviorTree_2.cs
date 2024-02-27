using System.Collections;
using GameMechanics;
using UnityEngine;


public class HB02_BehaviorTree_2 : EnemyBehaviorManager
{
    protected EnemyControllerHumanoid enemyController;
    protected EnemyMoveController_HB02 enemyAttackManager;
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        enemyAttackManager = GetComponent<EnemyMoveController_HB02>();
        GetBehavior();
        
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
    }

    

    protected override void DoAction(int state, int substate)
    {
        if (playerAlive == false)
            return;
        
        _currentPhase = _pattern.phasePattern[state];
        _currentActionStage = _currentPhase.action_list[substate];

        var action_name = _currentActionStage.action_name;

        if (action_name == "null")
        {
            ActionEnd();
            return;
        }

        switch (action_name)
        {
            case "KeepDistance":
            {
                float distanceMin = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                float distanceMax = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float followTime = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);
                currentAction = StartCoroutine(ACT_KeepDistance(distanceMin, distanceMax, followTime));
                break;
            }
            case "ApproachTarget":
            {
                if (_currentActionStage.args.Length == 4)
                {
                    float arriveDistanceX = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                    float arriveDistanceY = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                    float triggerDistance = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);
                    float followTime = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[3]);
                    currentAction = StartCoroutine(ACT_ApproachTarget(arriveDistanceX, arriveDistanceY, triggerDistance, followTime));
                }else if (_currentActionStage.args.Length == 3)
                {
                    float arriveDistanceX = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                    float arriveDistanceY = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                    float followTime = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);
                    currentAction = StartCoroutine(ACT_ApproachTarget(arriveDistanceX, arriveDistanceY, followTime));
                }else if (_currentActionStage.args.Length == 2)
                {
                    float arriveDistance = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                    float followTime = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                    currentAction = StartCoroutine(ACT_ApproachTarget(arriveDistance, followTime));
                }else throw new System.Exception("ApproachTarget参数数量错误");
                break;
            }
            case "ComboA":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboA(interval));
                break;
            }
            case "ComboB":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboB(interval));
                break;
            }
            case "ComboC":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboC(interval));
                break;
            }
            case "DashAttack":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DashAttack(interval));
                break;
            }
            case "GloriousSanctuary":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_GloriousSanctuary(interval));
                break;
            }
            case "HolyCrown":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_HolyCrown(interval));
                break;
            }
            case "TwilightCrown":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TwilightCrown(interval));
                break;
            }
            case "CelestialPrayer":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CelestialPrayer(interval));
                break;
            }
            case "TwilightMoon":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TwilightMoon(interval));
                break;
            }
            case "WarpAttack":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_WarpAttack(interval));
                break;
            }
            case "FaithEnhancement":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FaithEnhancement(interval));
                break;
            }
            case "EarthBarrier":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_EarthBarrier(interval));
                break;
            }
            case "CombinedTwilightAttack":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CombinedTwilightAttack(interval));
                break;
            }
            case "PhaseShift":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_PhaseShift(interval));
                break;
            }
            case "SpinDash":
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SpinDash(interval));
                break;
            }
            default:break;

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

    protected IEnumerator ACT_KeepDistance(float distanceMin, float distanceMax, float followTime, int fixTime)
    {
        ActionStart();
        currentMoveAction = 
            StartCoroutine(enemyController.KeepDistanceFromTarget
                (targetPlayer,followTime,distanceMin,distanceMax,fixTime==1?false:true));
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
        enemyController.SetKBRes(status.knockbackRes);
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
        enemyController.SetKBRes(status.knockbackRes);
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
        enemyController.SetKBRes(status.knockbackRes);
        ActionEnd();
    }

    protected IEnumerator ACT_ComboA(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action01());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_ComboB(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action03());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_ComboC(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action04());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_DashAttack(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action05());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_GloriousSanctuary(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action02());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_HolyCrown(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action06());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_TwilightMoon(float interval)
    {
        ActionStart();
        controllAfflictionProtect = true;
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        if (status.GetConditionsOfType((int)BasicCalculation.BattleCondition.TwilightMoon).Count > 0)
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action07());
            yield return new WaitUntil(()=>currentAttackAction == null);
            enemyController.SetKBRes(status.knockbackRes);
            yield return new WaitForSeconds(interval);
        }
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        ActionEnd();
    }

    protected IEnumerator ACT_TwilightCrown(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action08());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_CelestialPrayer(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action09());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_WarpAttack(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action10());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_FaithEnhancement(float interval)
    {
        ActionStart();
        controllAfflictionProtect = true;
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action11());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_EarthBarrier(float interval)
    {
        ActionStart();
        controllAfflictionProtect = true;
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action12());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_CombinedTwilightAttack(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action13());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_SpinDash(float interval)
    {
        ActionStart();
        controllAfflictionProtect = true;
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB02_Action15());
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
    
    

    protected override bool CheckCondition(string[] args, out int dest_state)
    {
        print(args[0]);
        switch (args[0])
        {
            case "has_attack_buff":
            {
                if (status.GetConditionTotalValue((int)BasicCalculation.BattleCondition.AtkBuff) > 0)
                {
                    dest_state = int.Parse(args[2]);
                    return false;
                }
                else
                {
                    dest_state = int.Parse(args[1]);
                    return true;
                }
            }
            case "gauge_full":
            {
                if (enemyAttackManager.auspexGauge >= 2)
                {
                    dest_state = int.Parse(args[1]);
                    return true;
                }
                else
                {
                    dest_state = int.Parse(args[2]);
                    return false;
                }
            }
            case "distance":
            {
                var distance = Mathf.Abs(targetPlayer.transform.position.x - transform.position.x);
                var cond = ObjectExtensions.ParseInvariantFloat(args[1]);
                if (distance <= cond)
                {
                    dest_state = int.Parse(args[2]);
                    return true;
                }
                else
                {
                    dest_state = int.Parse(args[3]);
                    return false;
                }
            }

            default:
            {
                dest_state = substate + 1;
                return false;
            }
        }
    }
}
