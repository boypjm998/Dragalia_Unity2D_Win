using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;

public abstract class DragaliaEnemyBehavior : MonoBehaviour
{
    public float awakeTime;
    //protected int phaseNum;
    [SerializeField]protected int state = 0;
    [SerializeField]protected int substate = 0;
    //protected Enemy enemyController;
    //protected EnemyMoveManager enemyAttackManager;
    protected StatusManager status;
    public GameObject targetPlayer;
    public Coroutine currentMoveAction = null;
    public Coroutine currentAttackAction = null;
    public Coroutine currentAction = null;
    protected bool TaskSuccess;
    public bool isAction;
    
    protected abstract void UpdateAttack();
    protected abstract void ExcutePhase();
    protected abstract void CheckPhase();
    

    protected virtual void Awake()
    {
        //enemyController = GetComponent<Enemy>();
        //enemyAttackManager = GetComponent<EnemyMoveManager>();
        status = GetComponent<StatusManager>();
        isAction = false;
        
    }

    protected void FinishMove(bool distanceCheck)
    {
        
        currentMoveAction = null;
        TaskSuccess = distanceCheck;
    }
    protected void FinishAttack(bool distanceCheck)
    {
        currentAttackAction = null;
        TaskSuccess = distanceCheck;
    }

    public string GetCurrentState()
    {
        return ("State" + state + " ,Substate" + substate);
    }



}
