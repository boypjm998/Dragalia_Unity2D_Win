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
    protected GameObject viewerPlayer;
    public Coroutine currentMoveAction = null;
    public Coroutine currentAttackAction = null;
    public Coroutine currentAction = null;
    protected bool TaskSuccess;
    public bool isAction;
    public bool playerAlive = true;
    public List<GameObject> playerList = new();

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

    protected virtual void Update()
    {
        
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
    
    protected IEnumerator Start()
    {
        yield return new WaitWhile(() => GlobalController.currentGameState == GlobalController.GameState.WaitForStart);
        SearchTarget();
        state = 0;
        substate = 0;
        yield return new WaitForSeconds(awakeTime);
        StartCoroutine(UpdateBehavior());
        //tartCoroutine(UpdateBehavior());
    }
    
    protected virtual void SearchTarget()
    {
        targetPlayer = FindObjectOfType<ActorController>().gameObject;
        viewerPlayer = targetPlayer;
        var players = GameObject.Find("Player");
        for (int i = 0; i < players.transform.childCount; i++)
        {
            this.playerList.Add(players.transform.GetChild(i).gameObject);
        }
    }
    

    protected IEnumerator UpdateBehavior()
    {
        while (true)
        {
            UpdateAttack();
            yield return null;
            yield return new WaitUntil(() => !isAction);
        }
    }

    /// <summary>
    /// 取出距离最近的玩家，如果没有参数，则取玩家控制的角色。
    /// </summary>
    /// <param name="order">1：最远玩家。0：最近玩家。其他：玩家角色</param>
    /// <returns></returns>
    protected GameObject GetTargetInDistanceOrder(int order = -1)
    {
        //返回PlayerList里第order个距离最近的物体
        //把playerList里的物体按照距离排序
        playerList.Sort((x, y) => (int) (Vector2.Distance(transform.position, x.transform.position) -
                                         Vector2.Distance(transform.position, y.transform.position)));
        //返回第order个物体
        if(order == 0)
            return playerList[0];
        if (order == 1)
            return playerList[-1];
        
        else return viewerPlayer;
    }



}
