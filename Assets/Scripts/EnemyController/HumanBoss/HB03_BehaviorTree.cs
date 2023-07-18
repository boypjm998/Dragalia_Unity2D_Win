using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using GameMechanics;
using UnityEngine;

public class HB03_BehaviorTree : EnemyBehaviorManager
{
    protected EnemyControllerHumanoidHigh enemyController;
    protected EnemyMoveController_HB03 enemyAttackManager;

    protected float skill1CD = 5;//20
    protected float skill2CD = 30;//30
    protected float skill3CD = 15;//20
    protected float skill7CD = 0;

    public float skill1CDMax = 15;
    public float skill2CDMax = 30;
    public float skill3CDMax = 15;
    public float skill7CDMax = 20;

    protected float counterPunish = 3;
    public float counterPunishCD = 10;
    
    protected bool debuffRelived = false;
    public bool summoned = true;

    protected int catridgeCount =>
        status.GetConditionStackNumber((int)BasicCalculation.BattleCondition.AlchemicCatridge);
    
    
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoidHigh>();
        enemyAttackManager = GetComponent<EnemyMoveController_HB03>();
        enemyController.OnMoveFinished += FinishMove;
        enemyController.OnBeingCountered += CounterPunish;
        counterPunish = 0;
        GetBehavior();

    }

    protected override void Update()
    {
        base.Update();
        skill3CD -= Time.deltaTime;
        skill2CD -= Time.deltaTime;
        skill1CD -= Time.deltaTime;
        counterPunish -= Time.deltaTime;

        if (debuffRelived == false && status.currentHp < status.maxHP * (0.45f + (difficulty==2?0:0.05f)))
        {
            if (status.GetConditionStackNumber((int)BasicCalculation.BattleCondition.Stun) > 0)
            {
                status.ReliefAllAfflication();
                debuffRelived = true;
            }
            
            
        }

    }

    protected override void CheckPhase()
    {
        var phase = _currentPhase;
        
        if(_currentActionStage.lock_phase)
            return;
        
        
        if (difficulty == 2)
        {
            if (status.currentHp < status.maxHP * 0.85 && state==0)
            {
                state = 1;
                substate = 0;
                skill1CDMax = 15;
                skill2CD = 0;
                skill3CD = 15;
            }
            if (status.currentHp < status.maxHP * 0.45 && state==1)
            {
                state = 2;
                skill2CD = 0;
                skill1CD = 0;
                substate = 0;
            }
        }
        else
        {
            if (status.currentHp < status.maxHP * 0.9 && state==0)
            {
                state = 1;
                substate = 0;
                skill1CDMax = 10;
                skill2CD = 0;
                skill3CD = 15;
            }
            if (status.currentHp < status.maxHP * 0.5 && state==1)
            {
                state = 2;
                skill2CD = 0;
                skill1CD = 0;
                substate = 0;
            }

            if (summoned == false && state == 2)
            {
                state = 3;
                skill2CD = 0;
                substate = 0;
            }
        }


    }

    // protected override void DoAction(int state, int substate)
    // {
    //     
    //     if (playerAlive == false)
    //         return;
    //
    //     if (state == 0)
    //     {
    //         switch (substate)
    //         {
    //              case 0:
    //              {
    //                  currentAction = StartCoroutine(ACT_FreeMoveThenStandardAttack(5,0.1f));
    //                  break;
    //              }
    //              case 1:
    //              {
    //                  currentAction = StartCoroutine(ACT_FreeMoveThenDriveBuster(3,1));
    //                  break;
    //              }
    //              case 2:
    //              {
    //                  this.substate = 0;
    //                  break;
    //              }
    //         
    //         }
    //     }
    //     else if (state == 1)
    //     {
    //         switch (substate)
    //         {
    //             case 0:
    //             {
    //                 currentAction = StartCoroutine(ACT_AlchemicEnhancement(3,0.1f));
    //                 break;
    //             }
    //             case 1:
    //             {
    //                 currentAction = StartCoroutine(ACT_FreeMoveThenDriveBuster(3,0.1f));
    //                 break;
    //             }
    //             case 2:
    //             {
    //                 currentAction = StartCoroutine(ACT_FreeMoveThenStandardAttack(3,0.1f));
    //                 break;
    //             }
    //             case 3:
    //             {
    //                 currentAction = StartCoroutine(ACT_OtherworldPortal(4,0.1f));
    //                 break;
    //             }
    //             case 4:
    //             {
    //                 this.substate = 0;
    //                 break;
    //             }
    //         }
    //     }
    //     else if(state == 2)
    //     {
    //         switch (substate)
    //         {
    //             case 0:
    //             {
    //                 currentAction = StartCoroutine(ACT_BlessingOfGale(3));
    //                 break;
    //             }
    //             case 1:
    //             {
    //                 currentAction = StartCoroutine(ACT_MoveToCenter(0));
    //                 break;
    //             }
    //             case 2:
    //             {
    //                 currentAction = StartCoroutine(ACT_OtherworldGateFixed(0));
    //                 break;
    //             }
    //             case 3:
    //             {
    //                 currentAction = StartCoroutine(ACT_FreeMoveThenDriveBuster(3,0f));
    //                 break;
    //             }
    //             case 4:
    //             {
    //                 currentAction = StartCoroutine(ACT_SummonZethia(0.5f));
    //                 break;
    //             }
    //             case 5:
    //             {
    //                 currentAction = StartCoroutine(ACT_ForceFieldEnhancement(0));
    //                 break;
    //             }
    //             
    //             case 6:
    //             {
    //                 currentAction = StartCoroutine(ACT_AlchemicEnhancement(2,0f));
    //                 break;
    //             }
    //             case 7:
    //             {
    //                 currentAction = StartCoroutine(ACT_FreeMoveThenDriveBuster(3,0.1f));
    //                 break;
    //             }
    //             
    //             case 8:
    //             {
    //                 currentAction = StartCoroutine(ACT_OtherworldPortal(3,2f));
    //                 break;
    //             }
    //             case 9:
    //             {
    //                 currentAction = StartCoroutine(ACT_FreeMoveThenStandardAttack(3,2f));
    //                 break;
    //             }
    //             case 10:
    //             {
    //                 currentAction = StartCoroutine(ACT_AlchemicEnhancement(2,2f));
    //                 break;
    //             }
    //             case 11:
    //             {
    //                 currentAction = StartCoroutine(ACT_FreeMoveThenDriveBuster(3,2f));
    //                 break;
    //             }
    //             case 12:
    //             {
    //                 this.substate = 8;
    //                 break;
    //             }
    //         }
    //     }
    //     else if (state == 3)
    //     {
    //         switch (substate)
    //         {
    //             case 0:
    //             {
    //                 if(catridgeCount<=0)
    //                 {
    //                     currentAction = StartCoroutine(ACT_AlchemicEnhancement(0.1f,0));
    //                     break;
    //                 }
    //
    //                 this.substate = 1;
    //                 break;
    //             }
    //             case 1:
    //             {
    //                 currentAction = StartCoroutine(ACT_OtherworldGateChasing(2.5f));
    //                 break;
    //             }
    //             case 2:
    //             {
    //                 if (FindObjectOfType<Projectile_C001_4_Boss>() == null)
    //                 {
    //                     this.substate = 3;
    //                     break;
    //                 }
    //
    //                 currentAction = StartCoroutine(ACT_ForceFieldEnhancement(0));
    //                 break;
    //             }
    //             case 3:
    //             {
    //                 currentAction = StartCoroutine(ACT_AlchemicEnhancement(4,2f));
    //                 break;
    //             }
    //             case 4:
    //             {
    //                 currentAction = StartCoroutine(ACT_FreeMoveThenDriveBuster(4,0.2f));
    //                 break;
    //             }
    //             case 5:
    //             {
    //                 currentAction = StartCoroutine(ACT_FreeMoveThenStandardAttack(5,0.1f));
    //                 break;
    //             }
    //             case 6:
    //             {
    //                 currentAction = StartCoroutine(ACT_OtherworldPortal(4,0.2f));
    //                 break;
    //             }
    //             case 7:
    //             {
    //                 currentAction = StartCoroutine(ACT_FreeMoveThenStandardAttack(5,0.1f));
    //                 break;
    //             }
    //             case 8:
    //             {
    //                 currentAction = StartCoroutine(ACT_FreeMoveThenDriveBuster(4,0.2f));
    //                 break;
    //             }
    //             case 9:
    //             {
    //                 currentAction = StartCoroutine(ACT_AlchemicEnhancement(4, 2f));
    //                 break;
    //             }
    //             case 10:
    //             {
    //                 this.substate = 1;
    //                 break;
    //             }
    //         }
    //     }
    //
    //
    //
    //
    //
    //
    // }

    protected override void DoAction(int state, int substate)
    {
        if(playerAlive == false)
        {
            return;
        }
        _currentPhase = _pattern.phasePattern[state];
        _currentActionStage = _currentPhase.action_list[substate];
        
        var action_name = _currentActionStage.action_name;
    
        switch (action_name)
        {
            case "free":
            {
                float type = float.Parse(_currentActionStage.args[0]);
                float moveTime = float.Parse(_currentActionStage.args[1]);
                float interval = float.Parse(_currentActionStage.args[2]);
                
                if (type == 0)
                {
                    currentAction = StartCoroutine(ACT_FreeMoveThenStandardAttack(moveTime,interval));
                    break;
                }
                else if (type == 1)
                {
                    currentAction = StartCoroutine(ACT_FreeMoveThenDriveBuster(moveTime,interval));
                    break;
                }else if (type == 2)
                {
                    currentAction = StartCoroutine(ACT_AlchemicEnhancement(moveTime,interval));
                    break;
                }else if (type == 3)
                {
                    currentAction = StartCoroutine(ACT_OtherworldPortal(moveTime,interval));
                    break;
                }
                else
                {
                    currentAction = StartCoroutine(ACT_FreeMoveThenStandardAttack(moveTime,interval));
                    break;
                }
    
    
            }
            case "gate":
            {
                float type = float.Parse(_currentActionStage.args[0]);
                float interval = float.Parse(_currentActionStage.args[1]);
                
                if(type == 0)
                    currentAction = StartCoroutine(ACT_OtherworldGateFixed(interval));
                else if (type == 1)
                    currentAction = StartCoroutine(ACT_OtherworldGateChasing(interval));
                else
                    currentAction = StartCoroutine(ACT_MoveToCenter(0));
                break;
                
            }
            case "buff":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BlessingOfGale(interval));
                break;
            }
            case "summon":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonZethia(interval));
                break;
            }
            case "force":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ForceFieldEnhancement(interval));
                break;
            }
        }
        
        
    }

    protected override bool CheckCondition(string[] args, out int dest_state)
    {
        print(args[0]);
        switch (args[0])
        {
            case "has_catridge":
            {
                if (status.GetConditionStackNumber((int)BasicCalculation.BattleCondition.AlchemicCatridge) > 0)
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
            case "gate_exist":
            {
                if (FindObjectOfType<Projectile_C001_4_Boss>() == null)
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
            case "distance":
            {
                var distance = Mathf.Abs(targetPlayer.transform.position.x - transform.position.x);
                var cond = float.Parse(args[1]);
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

    protected IEnumerator ACT_FreeMoveThenStandardAttack(float moveTime,float interval)
    {
        
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        
        if (enemyAttackManager.transportable)
        {
            if (Vector2.Distance(Projectile_C001_2_Boss.Instance.transform.position,
                    targetPlayer.transform.position) > 10)
            {
                currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action09T());
                yield return new WaitUntil(()=>currentAttackAction == null);
                TaskSuccess = false;
                enemyController.SetKBRes(status.knockbackRes);
                currentMoveAction = StartCoroutine((enemyController).MoveTowardsTargetNavigatorWithDesignedRoutine(
                    nameof(enemyAttackManager.COND_StandardAttackAimCheck),
                    targetPlayer,1,moveTime));
                yield return new WaitUntil(()=>currentMoveAction == null);
                    
            }

        }

        currentMoveAction = StartCoroutine((enemyController).MoveTowardsTargetNavigatorWithDesignedRoutine(
             nameof(enemyAttackManager.COND_StandardAttackAimCheck),
             targetPlayer,1,moveTime));
        
        yield return new WaitUntil(()=>currentMoveAction == null);
        print("Task0");
        enemyController.SetKBRes(999);

        if (TaskSuccess)
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action01());
            yield return new WaitUntil(()=>currentAttackAction == null);
        }
        else
        {
            enemyController.SetKBRes(999);
            if (TaskSuccess)
            {
                print("Task1");
                currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action01());
                yield return new WaitUntil(()=>currentAttackAction == null);
            }
            else if (CheckDistanceReachable(5))
            {
                print("Task2");
                currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action03());
                yield return new WaitUntil(()=>currentAttackAction == null);
            }else if (CheckPlayerAbove(10))
            {
                print("Task3");
                enemyController.SetKBRes(status.knockbackRes);
                currentMoveAction =
                    StartCoroutine(enemyController.MoveTowardTargetOnGround
                        (targetPlayer, 2.5f, 3,1, 3));
                yield return new WaitUntil(()=>currentMoveAction == null);
                print("Task4");
                enemyController.SetKBRes(999);
                currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action03());
                yield return new WaitUntil(()=>currentAttackAction == null);
            }
            else
            {
                currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action02());
                yield return new WaitUntil(()=>currentAttackAction == null);
            }
        }

        if (CheckDistanceReachable(9))
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action04());
            yield return new WaitUntil(()=>currentAttackAction == null);
        }
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_FreeMoveThenDriveBuster(float moveTime, float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        if (skill1CD <= 0)
        {
            skill1CD = skill1CDMax;
            if (catridgeCount > 0)
            {
                currentMoveAction = StartCoroutine
                (enemyController.MoveToSameGround(
                    targetPlayer,moveTime,13));
                
                yield return new WaitUntil(()=>currentMoveAction == null);
                
                yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
                enemyController.SetKBRes(999);
                currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action06());
                
                yield return new WaitUntil(()=>currentAttackAction == null);
                if (catridgeCount <= 0)
                    skill2CD = skill2CDMax;
            }
            else
            {
                currentMoveAction = StartCoroutine((enemyController).MoveTowardsTargetNavigatorWithDesignedRoutine(
                    nameof(enemyAttackManager.COND_StandardAttackAimCheck),
                    targetPlayer,1,moveTime));
        
                yield return new WaitUntil(()=>currentMoveAction == null);
                
                

                if (TaskSuccess)
                {
                    print("Task1");
                    currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action05());
                    yield return new WaitUntil(()=>currentAttackAction == null);
                }
                
                else 
                {
                    if (CheckDistanceReachable(6))
                    {
                        print("Task2");
                        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action03());
                        yield return new WaitUntil(()=>currentAttackAction == null);
                    }else if (CheckPlayerAbove(10))
                    {
                        print("Task3");
                        enemyController.SetKBRes(status.knockbackRes);
                    currentMoveAction =
                        StartCoroutine(enemyController.MoveTowardTargetOnGround
                            (targetPlayer, 2.5f, 3,1, 3));
                        yield return new WaitUntil(()=>currentMoveAction == null);
                        print("Task4");
                        enemyController.SetKBRes(999);
                        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action03());
                        yield return new WaitUntil(()=>currentAttackAction == null);
                    }
                    else
                    {
                        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action05());
                        yield return new WaitUntil(()=>currentAttackAction == null);
                    }
                    
                }
            }

        }
        if (CheckDistanceReachable(9))
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action04());
            yield return new WaitUntil(()=>currentAttackAction == null);
            enemyController.SetKBRes(status.knockbackRes);
        }
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_AlchemicEnhancement(float moveTime, float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        
        if (skill2CD <= 0)
        {
            if(catridgeCount <= 0)
            {
                currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action07());
                yield return new WaitUntil(()=>currentAttackAction == null);
                if(catridgeCount > 0)
                {
                    skill2CD = skill2CDMax/3;
                }
                else
                {
                    skill2CD = skill2CDMax;
                }
            }
            else
            {
                skill2CD = skill2CDMax + 2;
                currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action08());
                yield return new WaitUntil(()=>currentAttackAction == null);
                
            }
        }
        
        if (CheckDistanceReachable(9))
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action04());
            yield return new WaitUntil(()=>currentAttackAction == null);
            enemyController.SetKBRes(status.knockbackRes);
        }
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_OtherworldPortal(float moveTime,float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        if (catridgeCount <= 0 && skill3CD <= 0)
        {
            
            currentMoveAction = StartCoroutine
            (enemyController.MoveToSameGround(
                targetPlayer,moveTime,7));
                
            yield return new WaitUntil(()=>currentMoveAction == null);
                
            enemyController.SetKBRes(999);
            skill3CD = skill3CDMax;
            currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action09());

            yield return new WaitUntil(()=>currentAttackAction == null);
        }
        
        if (CheckDistanceReachable(9))
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action04());
            yield return new WaitUntil(()=>currentAttackAction == null);
            enemyController.SetKBRes(status.knockbackRes);
        }
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
        
    }

    protected IEnumerator ACT_OtherworldGateFixed(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);


        if(currentMoveAction!=null)
            StopCoroutine(currentMoveAction);    
    
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action10());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_OtherworldGateChasing(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);


        if(currentMoveAction!=null)
            StopCoroutine(currentMoveAction);

        if (skill7CD <= 0 && catridgeCount > 0)
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action10(targetPlayer));
            yield return new WaitUntil(()=>currentAttackAction == null);
            enemyController.SetKBRes(status.knockbackRes);
            skill7CD = skill7CDMax;
        
            yield return new WaitForSeconds(interval);
        }

        
        ActionEnd();
    }

    protected IEnumerator ACT_BlessingOfGale(float interval)
    {
        ActionStart();
        status.RemoveTimerBuff((int)(BasicCalculation.BattleCondition.Stun), true);
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action11());
        yield return new WaitUntil(()=>currentAttackAction == null);
        skill2CD = 0;
        skill1CD = 0;
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_ForceFieldEnhancement(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action13());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        skill1CD = 0;
        skill2CD = 0;
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_MoveToCenter(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        currentMoveAction = StartCoroutine(enemyController.
            MoveTowardsTarget(enemyAttackManager.GetAnchoredSensor("MiddleM"),0.5f,5));
        yield return new WaitUntil(()=>currentAttackAction == null &&
                                       currentMoveAction == null);
        
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_SummonZethia(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.HB03_Action12());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }




    protected bool CheckDistanceReachable(float distance)
    {
        //检查以当前位置为中心，距离distance的范围内是否有玩家
        
        for(int i = 0; i < playerList.Count; i++)
        {
            if (Vector2.Distance(transform.position,
                    playerList[i].transform.position) <= distance)
            {
                return true;
            }
        }
        return false;
    }

    protected bool CheckPlayerAbove(float maxDistance)
    {
        //检查targetPlayer与vector2.right的signedAngle是否在50-130之间或者-130-50之间
        var vector = targetPlayer.transform.position - transform.position;
        var angle = Vector2.SignedAngle(vector, Vector2.right);
        if (angle > 50 && angle < 130 || angle < -50 && angle > -130)
        {
            if (Vector2.Distance(targetPlayer.transform.position, transform.position) <= maxDistance)
            {
                return true;
            }
            else return false;
        }
        return false;
        
    }

    protected void CounterPunish()
    {
        if(counterPunish > 0)
            return;
        status.ObtainTimerBuff((int)BasicCalculation.BattleCondition.Stun,
            -1,6+Random.Range(0f,3f),1,-1);
        counterPunish = counterPunishCD;
    }

}

