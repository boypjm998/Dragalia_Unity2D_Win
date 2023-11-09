using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOneWayPlatformEffector : MonoBehaviour
{
    StandardGroundSensor groundSensor;

    private int bugFix = 0;
    private Rigidbody2D rigid;
    public Collider2D platformSensor;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponentInParent<Rigidbody2D>();
        platformSensor = GetComponent<Collider2D>();
        groundSensor = transform.parent.GetComponentInChildren<StandardGroundSensor>();
    }

    private void FixedUpdate()
    {
        if (rigid.velocity.y == 0 && !groundSensor.currentPlatform && !groundSensor.currentGround)
        {
            bugFix++;
        }
        else
        {
            bugFix = 0;
        }

        if (bugFix > 300)
        {
            rigid.position += new Vector2(-transform.localScale.x, 0.01f);
            bugFix = 0;
        }



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
