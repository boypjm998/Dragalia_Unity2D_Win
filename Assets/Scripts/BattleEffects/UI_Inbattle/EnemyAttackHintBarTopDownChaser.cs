using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class EnemyAttackHintBarTopDownChaser : MonoBehaviour
{
    [SerializeField] private float lockTime;
    [SerializeField] private bool useCastPlatformY;
    [SerializeField] private bool hardLock;
    [SerializeField] private float moveSpeedX;
    [SerializeField] private float moveSpeedY;
    [SerializeField] private float offsetY;
    public GameObject target;
    
    private Collider2D raycastedPlatform;
    public Collider2D RaycastedPlatform => raycastedPlatform;

    public void SetLockTime(float time)
    {
        lockTime = time;
    }
    
    public void SetHardLock(bool isHardLock)
    {
        hardLock = isHardLock;
    }
    
    public void SetUseCastPlatformY(bool useCast)
    {
        useCastPlatformY = useCast;
    }
    
    public void SetMoveSpeedX(float speed)
    {
        moveSpeedX = speed;
    }
    
    public void SetMoveSpeedY(float speed)
    {
        moveSpeedY = speed;
    }
    
    public void SetOffsetY(float offset)
    {
        offsetY = offset;
    }
    
    
    private void FixedUpdate()
    {
        if(target == null)
            return;
        
        if (lockTime > 0)
        {
            lockTime -= Time.fixedDeltaTime;

            if (hardLock)
            {
                if (useCastPlatformY)
                {
                    transform.position = new Vector3(target.transform.position.x,
                        BasicCalculation.GetRaycastedPlatformY(target)+offsetY);
                    raycastedPlatform = target.RaycastedPlatform();
                }else
                {
                    transform.position = target.transform.position + new Vector3(0,offsetY);
                }
            }
            else
            {
                //对X轴进行追踪
                var distanceX = target.transform.position.x - transform.position.x;
                if (distanceX > moveSpeedX * Time.fixedDeltaTime)
                    transform.position += new Vector3(moveSpeedX, 0) * Time.fixedDeltaTime;
                else if (distanceX < -moveSpeedX * Time.fixedDeltaTime)
                    transform.position -= new Vector3(moveSpeedX, 0) * Time.fixedDeltaTime;
                else
                {
                    transform.position = new Vector3(target.transform.position.x, transform.position.y);
                }

                var distanceY = target.transform.position.y - transform.position.y - offsetY;


                if (useCastPlatformY)
                {
                    transform.position = new Vector3(transform.position.x,
                        BasicCalculation.GetRaycastedPlatformY(target) + offsetY);
                    raycastedPlatform = target.RaycastedPlatform();
                }
                else if(moveSpeedY>0)
                {
                    if (distanceY > moveSpeedY * 0.1f)
                        transform.position += new Vector3(0, moveSpeedY) * Time.fixedDeltaTime;
                    else if (distanceY < -moveSpeedY * 0.1f)
                        transform.position -= new Vector3(0, moveSpeedY) * Time.fixedDeltaTime;
                    else
                    {
                        transform.position = new Vector3(transform.position.x, target.transform.position.y + offsetY);
                    }
                }

            }

        }
        
        
        
        
    }
}
