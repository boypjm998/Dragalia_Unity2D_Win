using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingAttackWithoutRotate : HomingAttack
{
    [SerializeField][Range(0.01f,6f)] private float maxAngularSpeed = 5;
    void Start()
    {
        angle = angle.normalized;
        smoothRotateVector = new Vector3(smoothRotateVector.x * angle.x, smoothRotateVector.y * angle.y, 0);
        smoothPoint = transform.position + smoothRotateVector;
        if(lifeTime > 0)
            Destroy(gameObject,lifeTime);
    }

    private void FixedUpdate()
    {
        //angle = angle.normalized;
        
        if (targetLockTime > 0)
        {
            targetLockTime -= Time.fixedDeltaTime;
        }

        
        
        
        
        if (homingStartTime > 0)
        {
            homingStartTime -= Time.fixedDeltaTime;
            
            if (smoothRotateVector != Vector3.zero)
            {
                angle =
                    Vector3.Slerp(angle.normalized, smoothPoint - transform.position,
                        preAngularSpeed / Vector2.Distance(transform.position,smoothPoint));
                
            }

            if (homingStartTime <= 0 && directionFix)
            {
                angle = new Vector2(firedir, 0);
                //transform.right = new Vector2(firedir, 0);
               
            }
            
            
        }
        else
        {
            if (stopFlag)
            {
                var spd = speed * Time.fixedDeltaTime;
                transform.position += (Vector3)angle * spd;
                return;
            }
            homingStartTime -= Time.fixedDeltaTime;
            if (homingStartTime < -stopChasingAfterTime && stopChasingAfterTime > 0)
            {
                stopFlag = true;
            }

            if (target != null && Vector2.Distance(target.position,transform.position) > 0.5f)
            {
                angle =
                    Vector3.Slerp(angle, target.position - transform.position,
                        angularSpeed / Vector2.Distance(transform.position, target.transform.position));
                
                
                
                
                angularSpeed += angularAcceleration * Time.fixedDeltaTime;
                if (angularSpeed > maxAngularSpeed)
                    angularSpeed = maxAngularSpeed;
            }
            else
            {
                //angle = new Vector2(firedir, 0);
            }
        }

        angle = angle.normalized;
        transform.position += (Vector3)angle * speed * Time.fixedDeltaTime;
        speed += acceleration * Time.fixedDeltaTime;
        //如果transform.right.x>0,那么firedir=1,否则firedir=-1
        firedir = angle.x > 0 ? 1 : -1;

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
}
