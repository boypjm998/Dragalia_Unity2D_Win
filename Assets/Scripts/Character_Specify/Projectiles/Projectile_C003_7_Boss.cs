using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_C003_7_Boss : MonoBehaviour
{
    public bool left;
    private BoxCollider2D box;

    private void Start()
    {
        box = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //print(collision.gameObject.layer);
        if(!collision.gameObject.layer.Equals(LayerMask.NameToLayer("Characters")) &&
           !collision.gameObject.layer.Equals(LayerMask.NameToLayer("Enemies")))
            return;
        
        if (left)
        {
            if (collision.transform.position.x < box.bounds.max.x)
            {
                collision.transform.position = new Vector3(box.bounds.max.x,0, collision.transform.position.z);
            }
        }

        if (!left)
        {
            if (collision.transform.position.x > box.bounds.min.x)
            {
                collision.transform.position = new Vector3(box.bounds.min.x, 0, collision.transform.position.z);
            }
        }



    }
}
