using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using Unity.Mathematics;
using UnityEngine;

public class Projectile_C005_3 : MonoBehaviour
{
    private float lengthA = 15f;
    private float lengthB = 10f;
    private float angle;
    [SerializeField] private float startAngle;
    [SerializeField] private float rotateSpeed = 18;
    
    void Start()
    {
        //angle = Mathf.Atan(transform.localPosition.x / transform.localPosition.y);
        //GetStartAngle();
        angle = startAngle*Mathf.Deg2Rad;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        MoveAround();
        angle = (angle + Time.fixedDeltaTime * rotateSpeed * Mathf.Deg2Rad) % (4*Mathf.PI);
    }

    private void Update()
    {
        transform.parent.rotation = Quaternion.identity;
    }


    void MoveAround()
    {
        //print(Mathf.Cos(angle));
        transform.localPosition =
            new Vector3(lengthA * Mathf.Cos(angle), lengthB * Mathf.Sin(angle));
    }
    void GetStartAngle()
    {
        if (transform.localPosition.x == 0)
        {
            if (transform.localPosition.y > 0)
            {
                angle = 0;
            }else if (transform.localPosition.y < 0)
            {
                angle = Mathf.PI;
            }
        }

        else if (transform.localPosition.y == 0)
        {
            if (transform.localPosition.x > 0)
            {
                angle = Mathf.PI/2;
            }else if (transform.localPosition.x < 0)
            {
                angle = Mathf.PI/2;
            }
        }
        else
        {
            angle = Mathf.Atan(transform.localPosition.y / transform.localPosition.x);
        }


    }

}
