using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_D010_1 : MonoBehaviour
{
    private GameObject projectileSmash;

    private void Awake()
    {
        projectileSmash = transform.parent.Find("Smash").gameObject;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("platform") || other.CompareTag("Ground"))
        {
            projectileSmash.SetActive(true);
            //Destroy(gameObject);
        }
    }
}
