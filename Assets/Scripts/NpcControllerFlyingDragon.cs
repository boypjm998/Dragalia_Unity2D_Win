using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class NpcControllerFlyingDragon : MonoBehaviour
{
    protected bool isAction;
    public enum ActionMode
    {
        /// 在该模式下，NPC会主动寻找玩家锁定的敌人进行攻击，如果没有可以攻击到的敌人，则转为FollowMode。
        AttackMode,
        /// 在该模式下，NPC会时刻跟随玩家，在该模式下，只要玩家没有进行攻击指令，npc就不会主动攻击。玩家进行攻击指令后，npc会转为AttackMode。
        FollowMode
    }
    
    public ActionMode actionMode = ActionMode.FollowMode;
    protected GameObject currentTarget;
    [SerializeField]protected GameObject playerGameObject;
    protected ActorControllerSpecial ac;
    protected DragonController dc;
    
    protected TargetAimer playerTargetAimer;
    [SerializeField] protected float moveSpeed = 10;

    public Action<bool> OnMoveFinished;
    protected Coroutine currentMoveCoroutine;
    
    [SerializeField] protected float maxAttackActiveDistanceX = 10;
    [SerializeField] protected float maxAttackActiveDistanceY = 4;
    [SerializeField] protected float minAttackActiveDistance = 3;
    
    
    void Start()
    {
        ac = GetComponent<ActorControllerSpecial>();
        playerGameObject = GameObject.Find("PlayerHandle");
        playerTargetAimer = playerGameObject.GetComponentInChildren<TargetAimer>();
        PlayerInput.OnPressAttack += ReceiveAttackSignal;
        GetComponent<StatusManager>().DebuffResistance = 999;
        //ac.OnAttackInterrupt += StopCurrentMoveAction;

        // mapInfo = BattleStageManager.InitMapInfo();
        // _aStar = new AStar(ac._defaultgravityscale,ac.jumpforce,ac.movespeed);
        // _aStar.Init(mapInfo);
        
        // if(currentTarget != null)
        //     GetPath(currentTarget);
        
        OnMoveFinished += StopCurrentAction;
        GetComponent<StatusManager>().ImmuneToAllControlAffliction = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAction)
        {
            if (actionMode == ActionMode.FollowMode)
            {
                currentTarget = playerGameObject;
                
                if (Mathf.Abs(currentTarget.transform.position.x - transform.position.x) > maxAttackActiveDistanceX)
                {
                    if (currentMoveCoroutine == null)
                    {
                        currentMoveCoroutine = StartCoroutine(FlyTowardTargetOnSamePlatform(playerGameObject, maxAttackActiveDistanceX/2
                            ,0.5f,999));
                    }

                    
                    //currentMainRoutineType = MainRoutineType.MoveTowardTarget;
                }
                else return;
            }
            else if(actionMode == ActionMode.AttackMode)
            {
                
                
        
        
                if (currentTarget!=null && currentTarget!=playerGameObject)
                {
                    print("enterActionMode");
                    if (CheckDistanceXDoubleDirection(maxAttackActiveDistanceX))
                    {
                        currentMoveCoroutine = StartCoroutine(DoAttack(currentTarget));
                        //currentMainRoutineType = MainRoutineType.Attack;
                    }
                    //else if(CheckDistanceXDoubleDirection( maxAttackActiveDistance))
                    else if(playerTargetAimer.ReachableEnemies.Contains(currentTarget))
                    {
                        currentMoveCoroutine = 
                            StartCoroutine(FlyTowardTargetOnSamePlatform(currentTarget,
                                maxAttackActiveDistanceX/2,0.5f, 999));
                        //currentMainRoutineType = MainRoutineType.MoveTowardTarget;
                    }
                    else
                    {
                        actionMode = ActionMode.FollowMode;
                        currentTarget = null;
                        if (currentMoveCoroutine != null)
                        {
                            StopCoroutine(currentMoveCoroutine);
                            currentMoveCoroutine = null;
                            //currentMainRoutineType = MainRoutineType.None;
                        }
                    }
                }
                else
                {
                    currentTarget = playerGameObject;
                    if (Mathf.Abs(currentTarget.transform.position.x - transform.position.x) > maxAttackActiveDistanceX)
                    {
                        currentMoveCoroutine = StartCoroutine(FlyTowardTargetOnSamePlatform(currentTarget,
                            maxAttackActiveDistanceX/2,0.5f ,999));
                        //currentMainRoutineType = MainRoutineType.MoveTowardTarget;
                    }
        
                    
                    //else currentTarget = null;
                }
        
                //如果周边没有敌人
                if (currentTarget == null || !playerTargetAimer.ReachableEnemies.Contains(currentTarget))
                {
                    //print(currentTarget.name + " is not reachable");
                    //ac.anim.SetFloat("forward",0);
                    currentTarget = null;
                    actionMode = ActionMode.FollowMode;
                }
            }
            
        }
    }
    public IEnumerator FlyTowardTargetOnSamePlatform(GameObject target, float arriveDistanceX, float allowDistanceY, float maxFollowTime)
      {
          isAction = true;
          print(ac.GetActorHeight());
            ac.SetGravityScale(0);
            print(target.gameObject.name);
            var targetCollider = BasicCalculation.CheckRaycastedPlatform(target);
            float targetHeight;
            try
            {
                targetHeight = target.GetComponent<ActorBase>().GetActorHeight();
            }
            catch
            {
                targetHeight = target.GetComponentInParent<ActorBase>().GetActorHeight();
            }



            //var myCollider = BasicCalculation.CheckRaycastedPlatform(gameObject);
            Vector2 endPoint;

            float time = 0;
            
            
            
            if (Mathf.Abs(target.transform.position.x - transform.position.x) < arriveDistanceX &&
                Mathf.Abs(target.transform.position.y - targetHeight - transform.position.y + ac.GetActorHeight()) < -allowDistanceY)
            { 
                  OnMoveFinished?.Invoke(true);
                  //ac.SetGroundCollision(true);
                  //ac.anim.Play("idle");
                  ac.anim.SetFloat("forward",0);
                  ac.ResetGravityScale();
                  yield break;
            }
            
            ac.anim.SetFloat("forward",1);
            //ac.anim.Play("move");

            while (time < maxFollowTime)
            {
                  
                  targetCollider = BasicCalculation.CheckRaycastedPlatform(target);
                  
                  if (((transform.position.y - ac.GetActorHeight()) -
                       target.transform.position.y - targetHeight) > allowDistanceY)
                  {
                        endPoint.y = targetCollider.bounds.max.y + ac.GetActorHeight();
                        //endPoint.y = transform.position.y + 0.5f;
                  }
                  else if (((target.transform.position.y) - targetHeight) - (transform.position.y - ac.GetActorHeight()) >
                             allowDistanceY)
                   {
                         endPoint.y = targetCollider.bounds.max.y + ac.GetActorHeight();
                   }
                  else
                  {
                        //endPoint.y = targetCollider.bounds.max.y + GetActorHeight() + 0.5f;
                        endPoint.y = transform.position.y;
                        print("set y with target collider");
                  }


                  if (Mathf.Abs(target.transform.position.x - transform.position.x) < arriveDistanceX)
                  {
                        if (target.transform.position.x > transform.position.x)
                        {
                              print("目标在右边，且x轴到达范围");
                              endPoint.x = Mathf.Max(transform.position.x, targetCollider.bounds.min.x);
                        }
                        else
                        {
                              endPoint.x = Mathf.Min(transform.position.x, targetCollider.bounds.max.x);
                              print("目标在左边，且x轴到达范围");
                        }
                  }
                  else if(target.transform.position.x > transform.position.x)
                  {
                        //目标在右边
                        endPoint.x = Mathf.Max(target.transform.position.x - arriveDistanceX,targetCollider.bounds.min.x) + 0.5f;
                  }else
                  {
                        //目标在左边
                        endPoint.x = Mathf.Min(target.transform.position.x + arriveDistanceX,targetCollider.bounds.max.x) - 0.5f;
                  }
                  
                  //以moveSpeed的速度向endPoint移动
                  Vector2 direction = (endPoint - (Vector2)transform.position).normalized;
                  transform.Translate(direction * moveSpeed * Time.deltaTime);
                  
                  //如果距离目标点的距离小于0.5f,break.
                  
                  
                  ac.TurnMove(target);
                  
                  if (Vector2.Distance(transform.position, endPoint) < 0.5f)
                  { 
                        OnMoveFinished?.Invoke(true);
                        //SetGroundCollision(true);
                        //ac.anim.Play("idle");
                        ac.TurnMove(target);
                        ac.anim.SetFloat("forward",0);
                        ac.ResetGravityScale();
                        yield break;
                  }
                  
                  yield return null;
                  
                  time += Time.deltaTime;
                  //print(endPoint);
            }

            //SetGroundCollision(true);
            //ac.anim.Play("idle");
            ac.anim.SetFloat("forward",0);
            ac.ResetGravityScale();
            OnMoveFinished?.Invoke(false);
            ac.TurnMove(target);
            
            yield break;

      }
    
    protected bool CheckDistanceXDoubleDirection(float distance, float distanceY = 999f)
    {
        //射线检测，找到射线和目标的交点,检测左右两边
        var hit = Physics2D.Raycast(transform.position, Vector2.right,
            distance,
            LayerMask.GetMask("Enemies"));
        
        var hit2 = Physics2D.Raycast(transform.position, Vector2.left,
            distance,
            LayerMask.GetMask("Enemies"));
        
        if(hit.collider == null && hit2.collider == null)
            return false;
        return true;

    }

    protected virtual IEnumerator DoAttack(GameObject target)
    {
        yield break;
    }

    protected void StopCurrentAction(bool flag)
    {
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
            //currentMainRoutineType = MainRoutineType.None;
        }

        
        isAction = false;
        
        //currentTarget = null;
    }

    void ReceiveAttackSignal()
    {
        if (playerTargetAimer.EnemyWatched != null)
        {
            
            actionMode = ActionMode.AttackMode;
            
            
            if(currentTarget == playerGameObject || !playerTargetAimer.ReachableEnemies.Contains(currentTarget))
                currentTarget = playerTargetAimer.EnemyWatched;
        }
    }
    
    protected bool CheckDistanceX(GameObject target, float distance, float distanceY = 999f)
    {
        //射线检测，找到射线和目标的交点
        var hit = Physics2D.Raycast(transform.position, ac.facedir*Vector2.right,
            distance,
            LayerMask.GetMask("Enemies"));
        
        var hit2 = Physics2D.Raycast(transform.position + new Vector3(0,-1.5f,0), ac.facedir*Vector2.right,
            distance,
            LayerMask.GetMask("Enemies"));
        //交点坐标
        
        if(hit.collider == null && hit2.collider == null)
            return false;

        float distanceX;

        if (hit != null)
        {
            distanceX = Mathf.Abs(hit.point.x - transform.position.x);
        }
        else
        {
            distanceX = Mathf.Abs(hit2.point.x - transform.position.x);
        }


        //print(distanceX);
        
        
        
        return distanceX < distance;
    }

    protected GameObject CheckNearsetTargetDoubleDirection(float distance, float distanceY = 999f)
    {
        //射线检测，找到射线和目标的交点,检测左右两边
        var hit = Physics2D.Raycast(transform.position, Vector2.right,
            distance,
            LayerMask.GetMask("Enemies"));
        
        var hit2 = Physics2D.Raycast(transform.position, Vector2.left,
            distance,
            LayerMask.GetMask("Enemies"));

        if (hit.collider == null && hit2.collider == null)
            return null;
        else if (hit.collider != null)
            return hit.collider.gameObject;
        else return hit2.collider.gameObject;

    }
}
