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
    private float speed;
    [SerializeField]
    private float lifeTime;
    [SerializeField]
    private float acceleration = 0;
    [SerializeField]
    private float homingStartTime;
    [SerializeField]
    private float angularAcceleration;
    [SerializeField]
    private float angularSpeed;
    
    [Header("Direction Fix")]
    [SerializeField]
    private bool directionFix;
    [SerializeField]
    private Vector3 smoothRotateVector;
    [SerializeField]
    private float preAngularSpeed;

    [SerializeField] private float stopChasingAfterTime;
    [SerializeField] private bool stopChasingAfterMiss;
    
    
    
    [Header("Target Late Lock Settings")]
    [SerializeField]
    private float targetLockTime;
    

    private Vector3 smoothPoint;

    public int firedir = 1;

    private bool stopFlag = false;
    
    
    
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

    private void FixedUpdate()
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


    void SetDirection(Vector2 angle)
    {
        transform.right = angle.normalized;
    }

}
