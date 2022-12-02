using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemControllerTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            print("hit character");
        }
    }

    private void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
    }
}
