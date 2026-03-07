using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyCollisionComponent : MonoBehaviour
{
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        
        ActorController ac;
        if ((ac = other.gameObject.GetComponentInParent<ActorController>()) != null)
        {
            Debug.Log("SetVelocity");
            ac.SetVelocity(0,ac.rigid.velocity.y);
        }
        
    }
}
