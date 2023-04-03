using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;
using UnityEngine;

public class EnemyControllerFlying : EnemyController
{
    [SerializeField] protected float isMove = 0;
    public float moveSpeed = 10f;
    
    // Start is called before the first frame update
    public override void OnAttackEnter()
    {
        isMove = 0;
        isAction = true;
    }
    public override void OnAttackExit()
    {
        isAction = false;
    }

    /// <summary>
    /// 匀速移动到目标
    /// </summary>
    /// <param name="target"></param>
    /// <param name="maxFollowTime"></param>
    /// <param name="arriveDistanceX"></param>
    /// <param name="arriveDistanceY"></param>
    /// <param name="startFollowDistance"></param>
    /// <returns></returns>
    public override IEnumerator MoveTowardTarget(GameObject target, float maxFollowTime, float arriveDistanceX, float arriveDistanceY,
        float startFollowDistance)
    {
        SetGravityScale(0);
        SetGroundCollision(false);
        TurnMove(target);
        
        
        if (CheckTargetDistance(target,arriveDistanceX,arriveDistanceY))
        {
            SetKBRes(999);
            OnMoveFinished?.Invoke(true);
            yield break;
        }
        
        //匀速移动到目标
        while (maxFollowTime > 0)
        {
            maxFollowTime -= Time.deltaTime;

            Vector2 direction = (target.transform.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.fixedDeltaTime);
            
            if (CheckTargetDistance(target, arriveDistanceX, arriveDistanceY))
            {
                /*if (VerticalMoveRoutine != null)
                {
                    StopCoroutine(VerticalMoveRoutine);
                    VerticalMoveRoutine = null;
                }*/
                TurnMove(target);
                //isMove = 0;
                currentKBRes = 999;
                OnMoveFinished?.Invoke(true);
                yield break;
            }

            yield return new WaitForFixedUpdate();

        }
        
        //isMove = 0;
        SetKBRes(999);
        OnMoveFinished?.Invoke(false);
    }
}
