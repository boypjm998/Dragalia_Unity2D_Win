using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemControllerTest : MonoBehaviour
{
    ParticleSystem ps;
    // Start is called before the first frame update
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        
        
        //遍历场景上所有碰撞体，如果其tag是platform，就将其碰撞体加入到触发器列表中
        foreach (var collider in FindObjectsOfType<Collider2D>())
        {
            if (collider.CompareTag("platform"))
            {
                ps.trigger.AddCollider(collider);
            }
        }
        
        
        
    }

    private void OnParticleTrigger()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        print(numEnter);
        for (int i = 0; i < numEnter; i++)
        {
            //let enter[i] collider enter the collision event
            print(enter[i].position);
            var p = enter[i];
            p.remainingLifetime = 0;
            p.velocity = Vector3.zero;
            
            enter[i] = p;
            
            
        }
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
    }
}
