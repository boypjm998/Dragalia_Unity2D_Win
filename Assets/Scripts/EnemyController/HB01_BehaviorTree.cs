using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class HB01_BehaviorTree : DragaliaEnemyBehavior
{
    
    protected EnemyControllerHumanoid enemyController;
    protected EnemyMoveController_HB01 enemyAttackManager;
    
    //public int state;
    
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        enemyAttackManager = GetComponent<EnemyMoveController_HB01>();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;
        
        //Invoke("SetEnable",awakeTime);
    }

    void SetEnable()
    {
        this.enabled = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        enemyController.DisableMovement();
    }

    IEnumerator Start()
    {
        SearchTarget();
        state = 0;
        substate = 0;
        yield return new WaitForSeconds(awakeTime);
        StartCoroutine(UpdateBehavior());
        //tartCoroutine(UpdateBehavior());
    }
    

    IEnumerator UpdateBehavior()
    {
        while (true)
        {
            UpdateAttack();
            yield return null;
            yield return new WaitUntil(() => !isAction);
        }
    }

    private void Update()
    {
        //UpdateAttack();
    }


    protected override void UpdateAttack()
    {
        CheckPhase();
        ExcutePhase();
    }

    /// <summary>
    /// Swap action phases.
    /// 当满足特定条件时切换行动阶段.
    /// </summary>
    protected override void CheckPhase()
    {
        //if (status.currentHp < status.maxHP * 0.1)
        //{
            //Unimplemented
        //}
    }

    protected override void ExcutePhase()
    {
        if (!isAction)
        {
            DoAction(state, substate);
        }
    }

    protected void SearchTarget()
    {
        targetPlayer = FindObjectOfType<ActorController>().gameObject;
    }

    private void DoAction(int state, int substate)
    {
        if (state == 0)
        {
            switch (substate)
            {
                case 0:
                    currentAction = StartCoroutine(SingleDodgeCombo());
                    break;
                case 1:
                    currentAction = StartCoroutine(SingleDodgeCombo());
                    break;
                case 2:
                    this.substate = 0;
                    break;
            }
        }
    }

    private IEnumerator SingleDodgeCombo()
    {
        //Do Move Action
        MoveTowardTarget(targetPlayer,5,3.5f,2.5f,5f);

        yield return new WaitUntil
            (() => (currentMoveAction == null));
        
        TurnAction();

        if (TaskSuccess)
        {
            //TaskSuccess = false;
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action01());
        }
        else
        {
            currentAttackAction =
                StartCoroutine(enemyAttackManager.HB01_Action02());
            //substate++;
            //isAction = false;
            //yield break;
        }

        yield return new WaitUntil(() => currentAttackAction == null);
        
        

        //Wait For Attack Interval
        yield return new WaitForSeconds(0.2f);
        substate++;
        isAction = false;
    }

    void MoveTowardTarget(GameObject target, float maxfollowtime, float arriveX, float arriveY, float startDistance)
    {
        isAction = true;
        TaskSuccess = false;
        currentMoveAction = 
            StartCoroutine
            (enemyController.MoveTowardTarget
                (target, maxfollowtime, arriveX,arriveY, startDistance));
    }

    void TurnAction()
    {
        if(enemyController.VerticalMoveRoutine!=null)
            enemyController.StopCoroutine(enemyController.VerticalMoveRoutine);//Danger
        
        //Do Attack Action
        enemyController.TurnMove(targetPlayer);
    }


}
