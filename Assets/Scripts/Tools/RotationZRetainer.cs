using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationZRetainer : MonoBehaviour
{
    [SerializeField][Range(0,90)] private float permitRotationX;

    [SerializeField][Range(0,90)] private float permitRotationY;
    // Update is called once per frame
    void LateUpdate()
    {
        ResetRotation();
    }

    private void ResetRotation()
    {
        var currentEulerAngle = transform.eulerAngles;

        var newAngle = new Vector3(currentEulerAngle.x, currentEulerAngle.y, currentEulerAngle.z);

        if (newAngle.x < -permitRotationX)
        {
            newAngle.x = -permitRotationX;
        }else if (newAngle.x > permitRotationX)
        {
            newAngle.x = permitRotationX;
        }
        
        if (newAngle.y < -permitRotationY)
        {
            newAngle.y = -permitRotationX;
        }else if (newAngle.y > permitRotationY)
        {
            newAngle.y = permitRotationY;
        }

        var newRotation = Quaternion.Euler(newAngle);

        transform.rotation = newRotation;



    }
    
}
