using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentityRetainerWithRotate : MonoBehaviour
{
    public Vector3 initialScaleFactor;
    // Start is called before the first frame update

    // Update is called once per frame
    private void Start()
    {
        //判断initialScaleFactor是否和当前transform的localScale异号
        if (initialScaleFactor.x * transform.parent.localScale.x < 0)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
            transform.localScale = new Vector3(-initialScaleFactor.x, initialScaleFactor.y, initialScaleFactor.z);
        }
    }
}
