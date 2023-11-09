using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class EnemyAttackHintBarRotater : MonoBehaviour
{
    [SerializeField] private float lockTime; 
    [SerializeField] private float rotateSpeed;
    [SerializeField] private bool rotateWithoutCondition = false;
    
    public GameObject target;
    public bool freeRotate = true;

    
    
    public float center = 0;
    public float upperRange = 180;
    
    
    public float sensitivity = 1;
    
    private float currentTime = 0;
    public bool hardLock = false;

    

    public AttackBase bindingAttack;

    private bool attackbinded;
    
    

    private Vector3 currentEulerAngle;
    void Start()
    {
        currentEulerAngle = transform.eulerAngles;
        if(bindingAttack != null)
            attackbinded = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target == null)
            return;
        
        var vector = (target.transform.position - transform.position).normalized;
        var angle = Vector2.SignedAngle(Vector2.right, vector);
        
        if (currentTime < lockTime)
        {
            currentTime += Time.fixedDeltaTime;
            if (hardLock)
            {
                currentEulerAngle = new Vector3(0,0,angle);
                transform.eulerAngles = currentEulerAngle;
                return;
            }

            var angleDiff = GetAngleDiff();

            if (Mathf.Abs(angleDiff)>sensitivity)
            {
                currentEulerAngle.z += rotateSpeed * Time.fixedDeltaTime * Mathf.Sign(angleDiff);
                
                if (!freeRotate)
                {



                    if (AngleInRange(transform.right) == 1 &&
                        angleDiff > 0)
                    {
                        currentEulerAngle.z -= rotateSpeed * Time.fixedDeltaTime * Mathf.Sign(angleDiff);
                    }else if (AngleInRange(transform.right) == -1 &&
                              angleDiff < 0)
                    {
                        currentEulerAngle.z -= rotateSpeed * Time.fixedDeltaTime * Mathf.Sign(angleDiff);
                    }

                    // if (AngleInRange(vector)==1)
                    // {
                    //     currentEulerAngle.z -= rotateSpeed * Time.fixedDeltaTime * Mathf.Sign(angleDiff);
                    //     //currentEulerAngle.z = RemapedAngle(center)+RemapedAngle(upperRange);
                    // }else if (AngleInRange(vector) == -1)
                    // {
                    //     currentEulerAngle.z -= rotateSpeed * Time.fixedDeltaTime * Mathf.Sign(angleDiff);
                    //     //currentEulerAngle.z = RemapedAngle(center)+RemapedAngle(lowerRange);
                    // }
                    
                }
                
                

                transform.eulerAngles = currentEulerAngle;
            }
            else
            {
                if (freeRotate)
                {
                    currentEulerAngle.z = angle;
                }

                
                transform.eulerAngles = currentEulerAngle;
            }



            if (attackbinded)
            {
                bindingAttack.attackInfo[0].knockbackDirection = transform.right.normalized;
            }




        }
    }
    
    public void SetRotateSpeed(float speed)
    {
        rotateSpeed = speed;
    }


    private float GetAngleDiff()
    {
        var pointVector = (target.transform.position - transform.position).normalized;
        
        var angle = Vector2.SignedAngle(transform.right, pointVector);
        
        return angle;
    }

    private float RemapedAngle(float angle)
    {
        if(angle>180)
            return angle-360;
        else if(angle<-180)
            return angle+360;
        else
            return angle;
    }

    private int AngleInRange(Vector2 pointerVector)
    {
        float angleInRadians = center * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        
        print("CenterAngle="+angleInRadians);
        
        var signedAngle = Vector2.SignedAngle(direction, pointerVector);
        
        
        
        if(signedAngle>upperRange)
            return 1;
        else if(signedAngle<-upperRange)
            return -1;
        else
            return 0;
    }

}
