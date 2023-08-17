using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class HB01_BehaviorTree_Legend : DragaliaEnemyBehavior
{
    protected EnemyMoveController_HB01_Legend enemyAttackManager;
    protected EnemyControllerHumanoid enemyController;
    [SerializeField]protected int type;

    public GameObject p2_prefab;
    public AudioClip p2_bgm;
    public bool transforming = false;

    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        enemyAttackManager = GetComponent<EnemyMoveController_HB01_Legend>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        type = Random.Range(0, 2);
        //Invoke("SetEnable",awakeTime);
        status.OnHPBelow0 += ToPhase2;
        
    }

    protected override void CheckPhase()
    {
        
    }

    protected override void DoAction(int state, int substate)
    {
        switch (substate)
        {
            case 0:
            {
                currentAction = StartCoroutine(ACT_BlazingEnhancement(0f));
                break;
            }
            case 1:
            {
                currentAction = StartCoroutine(ACT_MoveToCenterGround(0f));
                break;
            }
            case 2:
            {
                currentAction = StartCoroutine(ACT_ScarletInferno(1f));
                break;
            }
            // case 2:
            // {
            //     currentAction = StartCoroutine(ACT_FlameRaidDirect(1f));
            //     break;
            // }
            // case 3:
            // {
            //     currentAction = StartCoroutine(ACT_BrightCarmineRushDirect(0f));
            //     break;
            // }
            // case 4:
            // {
            //     currentAction = StartCoroutine(ACT_SavageFlameRaidDirect(1f));
            //     break;
            // }
            case 3:
            {
                if (type == 1)
                {
                    currentAction = StartCoroutine(ACT_CarmineRushDirect(1f));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_FlameRaidDirect(1.25f));
                }
                break;
            }
            case 4:
            {
                if (type == 1)
                {
                    currentAction = StartCoroutine(ACT_SavageFlameRaidDirect(1f));
                }
                else {
                    currentAction = StartCoroutine(ACT_BrightCarmineRushDirect(0.25f));
                }

                break;
            }
            case 5:
            {
                if (type == 1)
                {
                    currentAction = StartCoroutine(ACT_BrightCarmineRushDirect(0f));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_SavageFlameRaidDirect(1f));
                }
                break;
            }
            case 6:
            {
                currentAction = StartCoroutine(ACT_CrimsonHeaven(2f));
                break;
            }
            case 7:
            {
                currentAction = StartCoroutine(ACT_ComboA(2f));
                break;
            }
            case 8:
            {
                currentAction = StartCoroutine(ACT_ComboB(1f));
                break;
            }
            case 9:
            {
                currentAction = StartCoroutine(ACT_ComboC(2f));
                break;
            }
            case 10:
            {
                currentAction = StartCoroutine(ACT_ComboD(1f));
                break;
            }
            case 11:
            {
                if (type == 1)
                {
                    currentAction = StartCoroutine(ACT_FlameRaid(3f,0f));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_CarmineRush(3f,0f));
                }
                
                break;
            }
            case 12:
            {
                if (type == 1)
                {
                    currentAction = StartCoroutine(ACT_BrightCarmineRush(0f,0f));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_SavageFlameRaid(0f,0f));
                }

                if (status.GetConditionsOfType((int)BasicCalculation.BattleCondition.BlazewolfsRush).Count <= 0)
                {
                    this.substate++;
                }

                break;
            }
            case 13:
            {
                if (type == 1)
                {
                    currentAction = StartCoroutine(ACT_SavageFlameRaid(0f,2f));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_BrightCarmineRush(0f,2f));
                }
                break;
            }
            case 14:
            {
                if(type == 1)
                    type = 0;
                else
                {
                    type = 1;
                }

                this.substate = 7;
                break;
            }
        }
        

        
    }

    protected override void Update()
    {
        if (transforming)
        {
            if(currentAction == null)
                currentAction = StartCoroutine(ChangePhaseAnimationRoutine());
        }
    }

    protected IEnumerator ACT_ComboA(float interval)
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
            yield return new WaitUntil(()=>currentAttackAction == null);
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action03());
            
        }
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_ComboB(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 3, 3.5f));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action06());
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

    protected IEnumerator ACT_ComboC(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 3, 6));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action07());
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
    
    protected IEnumerator ACT_ComboD(float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 3, 3.5f));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        enemyController.SetKBRes(999);
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action05());
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

    protected IEnumerator ACT_CarmineRush(float maxFollowTime, float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveToSameGround
                (targetPlayer, 3, 12));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        enemyController.SetKBRes(999);
        
        yield return null;
        
        if (TaskSuccess)
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action04());
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

    protected IEnumerator ACT_BrightCarmineRush(float maxFollowTime, float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);

        if (maxFollowTime > 0)
        {
            currentMoveAction = 
                StartCoroutine
                (enemyController.MoveToSameGround(
                    targetPlayer, maxFollowTime, 15));
            
            yield return new WaitUntil(() => (currentMoveAction == null));
            status.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.BlazewolfsRush, -1, true);
            enemyController.SetKBRes(999);
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action09());
            
        }
        else
        {
            enemyController.SetKBRes(999);
            TaskSuccess = enemyController.CheckTargetDistance(targetPlayer,25f, 99f);
            if (TaskSuccess)
            {
                status.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.BlazewolfsRush, -1, true);
                currentAttackAction =
                    StartCoroutine(enemyAttackManager.HB01_Action09());
            }
            else
            {
            
                currentAttackAction =
                    StartCoroutine(enemyAttackManager.HB01_Action02());
                yield return new WaitUntil(() => currentAttackAction == null);
            
                currentAttackAction =
                    StartCoroutine(enemyAttackManager.HB01_Action04());

            }
            
        }
        
        
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }



    protected IEnumerator ACT_CarmineRushDirect(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HB01_Action04_WithInferno());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_BrightCarmineRushDirect(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        status.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.BlazewolfsRush, -1, true);
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HB01_Action09_WithInferno());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    

    protected IEnumerator ACT_FlameRaidDirect(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HB01_Action08_WithInferno());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_FlameRaid(float maxFollowTime,float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveTowardTargetOnGround
                (targetPlayer, maxFollowTime, 5,6,7));
        
        yield return new WaitUntil(() => (currentMoveAction == null));
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HB01_Action08());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
        
    }

    protected IEnumerator ACT_SavageFlameRaid(float maxFollowTime, float interval)
    {
        ActionStart();
        SetTarget(ClosestTarget);
        
        

        if (maxFollowTime > 0)
        {
            currentMoveAction = 
                StartCoroutine
                (enemyController.MoveTowardTargetOnGround
                    (targetPlayer, maxFollowTime, 10,8,10));
            
            yield return new WaitUntil(() => (currentMoveAction == null));
            status.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.BlazewolfsRush, -1, true);
            enemyController.SetKBRes(999);
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action10());
            
        }
        else
        {
            enemyController.SetKBRes(999);
            TaskSuccess = enemyController.CheckTargetDistance(targetPlayer,15f, 8f);
            if (TaskSuccess)
            {
                currentAttackAction =
                    StartCoroutine(enemyAttackManager.HB01_Action10());
                status.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.BlazewolfsRush, -1, true);
            }
            else
            {
            
                currentAttackAction =
                    StartCoroutine(enemyAttackManager.HB01_Action02());
                yield return new WaitUntil(() => currentAttackAction == null);
            
                currentAttackAction =
                    StartCoroutine(enemyAttackManager.HB01_Action08());

            }
            
        }
        
        
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_SavageFlameRaidDirect(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        status.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.BlazewolfsRush, -1, true);
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HB01_Action10_WithInferno());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_BlazingEnhancement(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HB01_Action13());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_ScarletInferno(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HB01_Action11());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_CrimsonHeaven(float interval)
    {
        
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        enemyController.SetKBRes(999);
        currentAttackAction =
            StartCoroutine(enemyAttackManager.HB01_Action12());
        yield return new WaitUntil(() => currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
        
        
    }

    protected IEnumerator ACT_MoveToCenterGround(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        currentMoveAction = StartCoroutine(enemyController.
            MoveTowardsTarget(enemyAttackManager.GetAnchoredSensorOfName("GroundM"),
                0.5f,5));
        yield return new WaitUntil(()=>currentAttackAction == null &&
                                       currentMoveAction == null);
        
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ChangePhaseAnimationRoutine()
    {
        ActionStart();
        print("startPhaseChange");
        currentMoveAction = 
            StartCoroutine(enemyAttackManager.HB01_Action14());
        yield return new WaitUntil(()=>currentMoveAction == null);
        
        print("转P2");
        
        var p2_boss = Instantiate(p2_prefab,transform.position,Quaternion.identity,transform.parent);
        p2_boss.GetComponent<EnemyController>().TurnMove(targetPlayer);
        //UI_BossStatus.Instance.RedirectBoss(p2_boss);
        //BattleEffectManager.Instance.bgmVoiceSource.Stop();
        BattleStageManager.currentDisplayingBossInfo = 2;
        FindObjectOfType<UI_BossStatus>().RedirectBoss(p2_boss,1);
        p2_boss.GetComponent<StatusManager>()?.OnHPChange?.Invoke();
        BattleEffectManager.Instance.PlayBGM(false);
        ActionEnd();
        yield return null;
        BattleStageManager.Instance.RemoveFieldAbility(20081);
        BattleStageManager.Instance.RemoveFieldAbility(20091);
        BattleEffectManager.Instance.SetBGM(p2_bgm);
        BattleEffectManager.Instance.PlayBGM(true);
        

        Destroy(gameObject);
    }

    void ToPhase2()
    {
        ResetHumanActionsBeforeTransform();
        
        currentAction = StartCoroutine(ChangePhaseAnimationRoutine());
        
    }
}
