using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingAttack : MonoBehaviour
{
    public Transform target;
    [Header("Projectile Basic Attributes")]
    
    public Vector2 angle = Vector2.zero;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float lifeTime;
    [SerializeField]
    protected float acceleration = 0;
    [SerializeField]
    protected float homingStartTime;
    [SerializeField]
    protected float angularAcceleration;
    [SerializeField]
    protected float angularSpeed;
    
    [Header("Direction Fix")]
    [SerializeField]
    protected bool directionFix;
    [SerializeField]
    protected Vector3 smoothRotateVector;
    [SerializeField]
    protected float preAngularSpeed;

    [SerializeField] protected float stopChasingAfterTime;
    [SerializeField] protected bool stopChasingAfterMiss;
    
    
    
    [Header("Target Late Lock Settings")]
    [SerializeField]
    protected float targetLockTime;
    

    protected Vector3 smoothPoint;

    public int firedir = 1;

    protected bool stopFlag = false;
    
    
    
    void Start()
    {
        if (angle == Vector2.zero)
        {
            angle = new Vector2(firedir, 0);
        }

        SetDirection(angle);
        smoothRotateVector = new Vector3(smoothRotateVector.x * angle.x, smoothRotateVector.y * angle.y, 0);
        smoothPoint = transform.position + smoothRotateVector;
        
        Destroy(gameObject,lifeTime);
    }

    protected void FixedUpdate()
    {
        if (targetLockTime > 0)
        {
            targetLockTime -= Time.fixedDeltaTime;
        }

        
        
        
        
        if (homingStartTime > 0)
        {
            homingStartTime -= Time.fixedDeltaTime;
            
            
            if (smoothRotateVector != Vector3.zero)
            {
                transform.right =
                    Vector3.Slerp(transform.right, smoothPoint - transform.position,
                        preAngularSpeed / Vector2.Distance(transform.position,smoothPoint));
                
            }

            if (homingStartTime <= 0 && directionFix)
            {
                
                transform.right = new Vector2(firedir, 0);
               
            }
            
            
        }
        else
        {
            if (stopFlag)
            {
                transform.position += transform.right * speed * Time.fixedDeltaTime;
                return;
            }
            homingStartTime -= Time.fixedDeltaTime;
            if (homingStartTime < -stopChasingAfterTime && stopChasingAfterTime > 0)
            {
                stopFlag = true;
            }

            if (target != null)
            {
                transform.right =
                    Vector3.Slerp(transform.right, target.position - transform.position,
                        angularSpeed / Vector2.Distance(transform.position, target.transform.position));
                angularSpeed += angularAcceleration * Time.fixedDeltaTime;
                if (angularSpeed > 5)
                    angularSpeed = 5;
            }
            else
            {
                transform.right = new Vector2(firedir, 0);
            }
        }

        transform.position += transform.right * speed * Time.fixedDeltaTime;
        speed += acceleration * Time.fixedDeltaTime;
        //如果transform.right.x>0,那么firedir=1,否则firedir=-1
        firedir = transform.right.x > 0 ? 1 : -1;

        if (stopChasingAfterMiss &&  target != null)
        {
            if (firedir == 1 && transform.position.x > target.transform.position.x)
            {
                stopFlag = true;
            }

            if (firedir == -1 && transform.position.x < target.transform.position.x)
            {
                stopFlag = true;
            }
            
            // if(target.transform.Find("HitSensor").GetComponent<Collider2D>().enabled == false)
            // {
            //     stopFlag = true;
            // }
        }

    }


    protected void SetDirection(Vector2 angle)
    {
        transform.right = angle.normalized;
    }

}
