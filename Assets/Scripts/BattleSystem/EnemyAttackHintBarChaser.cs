using System;
using System.Reflection;
using UnityEngine;


public class EnemyAttackHintBarChaser : MonoBehaviour
{ 
    [SerializeField] private float lockTime; 
    [SerializeField] private float rotateSpeed;
    [SerializeField] private bool rotateWithoutCondition = false;
    [SerializeField] private float moveSpeed;
    private float currentTime = 0;
    [SerializeField] private float lag;

    public GameObject target;
    public bool isRanged = true;

    float currentRotateSpeed;
    private void Start()
    {
        currentRotateSpeed = rotateSpeed;
    }

    private void FixedUpdate()
    {


        if (transform.rotation.eulerAngles.y == 180 && !isRanged)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y + 180, transform.rotation.eulerAngles.z);
        }

        var vector = (transform.position - target.transform.position).normalized;
        //print(vector);
        if (currentTime < lockTime)
        {
            transform.position += moveSpeed * Time.fixedDeltaTime * vector;
            var angle = Vector2.SignedAngle(transform.right, vector);
            if (angle < 178 && angle>=0 && !rotateWithoutCondition)
            {
                if (angle < 120)
                {
                    currentRotateSpeed = 4 * rotateSpeed;
                }
                else
                {
                    currentRotateSpeed = rotateSpeed;
                }

                transform.Rotate(Vector3.forward*-currentRotateSpeed*Time.fixedDeltaTime);
                

            }else if (angle > -178 && angle<=0 && !rotateWithoutCondition)
            {
                if (angle > -120)
                {
                    currentRotateSpeed = 4 * rotateSpeed;
                }
                else
                {
                    currentRotateSpeed = rotateSpeed;
                }
                transform.Rotate(Vector3.forward*currentRotateSpeed*Time.fixedDeltaTime);
                //currentRotateSpeed += rotateSpeed * 0.1f * Time.fixedDeltaTime;
            }
            else
            {
                currentRotateSpeed = rotateSpeed;
            }
            //print(transform.right);
            //print(Vector2.SignedAngle(transform.right, vector));
            
            currentTime += Time.fixedDeltaTime;
        }
    }
    
    
}
