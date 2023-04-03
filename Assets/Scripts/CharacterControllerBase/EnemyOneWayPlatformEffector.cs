using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOneWayPlatformEffector : MonoBehaviour
{
    StandardGroundSensor groundSensor;
    // Start is called before the first frame update
    void Start()
    {
        groundSensor = transform.parent.GetComponentInChildren<StandardGroundSensor>();
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collision) {

        if (collision.gameObject.CompareTag("platform"))
        {
            groundSensor.currentPlatform = collision.gameObject;
        }


    }
    private void OnCollisionExit2D(Collision2D collision) {

        if (collision.gameObject.CompareTag("platform"))
            groundSensor.currentPlatform = null;

    }
}
