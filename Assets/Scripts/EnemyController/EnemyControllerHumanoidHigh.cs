using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class EnemyControllerHumanoidHigh : EnemyControllerHumanoid
{
    private EnemyMoveManager attackManager;

    protected override void Start()
    {
        base.Start();
        attackManager = GetComponent<EnemyMoveManager>();
    }

    protected void QuitMove(bool flag,int stopMove = 0)
    {
        isMove = stopMove;
        OnMoveFinished?.Invoke(flag);
        //isAction = false;
    }

    /// <summary>
    /// Target为玩家的目标，但是实际上追踪的是路线锚点。
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="target"></param>
    /// <param name="arriveDistanceX"></param>
    /// <param name="maxFollowTime"></param>
    /// <returns></returns>
    public IEnumerator MoveTowardsTargetNavigatorWithDesignedRoutine(string functionName, GameObject targetPlayer,float arriveDistanceX, float maxFollowTime = 99f)
    {
        isAction = true;
        yield return new WaitUntil(() =>
            !hurt && grounded);
        
        var cond = GetConditionByCheckTarget(functionName, targetPlayer);
        //当条件满足时，退出移动。
        
        yield return null;

        if (cond)
        {
            //如果满足条件。
            QuitMove(true);
            yield break;
        }

        GameObject target = null;

        //TODO: 根据当前位置和目标位置判断移动的路径，此为关卡特化部分，需要耦合地图上的锚点和BOSS脚本。

        var path = attackManager.GetNextRoutineNode(ref target);
        var timeCounter = Time.time;
        
        // var targetSensor = target.GetComponentInChildren<IGroundSensable>();
        // //var mySensor = GetComponentInChildren<IGroundSensable>();
        // if (targetSensor == null)
        // {
        //     targetSensor = target.transform.parent.GetComponentInChildren<IGroundSensable>();
        // }

        while(Time.time <= timeCounter + maxFollowTime)
        {
            yield return new WaitUntil(() =>
                anim.GetBool("isGround") && !hurt);
            
            cond = GetConditionByCheckTarget(functionName, targetPlayer);
            if (cond)
            {
                QuitMove(true);
                yield break;
            }
            
            path = attackManager.GetNextRoutineNode(ref target);
            print("Target:"+path.colliderName);
            
            
            // 检查NPC和目标是否处于同一个平台上
            if (CheckTargetStandOnSameGround(target) == 0) {
                // 计算两者之间的x轴距离
                float distance = Mathf.Abs(transform.position.x - target.transform.position.x);

                if (distance > arriveDistanceX)
                {
                    TurnMove(target);
                    isMove = 1;
                }
                else
                    isMove = 0;
                
            
                // 如果x轴距离小于arriveDistance，则终止协程
                if (distance <= arriveDistanceX) {
                    QuitMove(true);
                    print("Yield Break Beacuse of Distance");
                    print("Target:"+target.name);
                    yield break;
                }
            } 
            else
            {

                //var platform = attackManager.GetNextRoutineNode();

                yield return new WaitUntil(() => anim.GetBool("isGround"));
                if (path.platform.collider == _groundSensor.GetCurrentAttachedGroundCol())
                {
                    if (CheckTargetStandOnSameGround(target) == 0)
                    {
                        isMove = 0;
                        continue;
                    }
                }

                // 移动到当前平台
                if (VerticalMoveRoutine != null)
                {
                    StopCoroutine(VerticalMoveRoutine);
                    VerticalMoveRoutine = null;
                }
                    

                VerticalMoveRoutine = StartCoroutine(MoveToPlatform(path));
                
                // 等待VerticalMoveRoutine结束或被中断
                    
                yield return new WaitUntil(()=>VerticalMoveRoutine==null);
                    
                print("VerticalMoveRoutine End");

                //isMove = 0;
                isMove = 0;
                    
                yield return new WaitUntil(() =>
                    anim.GetBool("isGround"));
                    
                
            }
        }
        
        
        
        QuitMove(false);
        isAction = false;

    }
    
    
    
    




}