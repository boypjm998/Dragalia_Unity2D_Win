using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleSystemSpeedModifier : MonoBehaviour, ISpeedControllable
{
    protected List<ParticleSystem.MainModule> _particleSystemsMain = new();
    public ActorController actorController;
    //protected AttackFromPlayer _attackBase;

    public float testRate = 1;
    public bool isFixed = false;

    public void GetTargetComponents()
    {
        var _particleSystems = GetComponentsInChildren<ParticleSystem>();
        
        foreach (var system in _particleSystems)
        {
            _particleSystemsMain.Add(system.main);
        }
        
    }

    public void SetRate(int id, float rate)
    {
        if (rate > 1.5f)
            rate = 1.5f;

        var main = _particleSystemsMain[id];

        main.simulationSpeed = rate;


    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => actorController != null || isFixed);
        //actorController = _attackBase.ac as ActorController;
        GetTargetComponents();
        
    }

    private void Update()
    {
        

        for (int i = 0; i < _particleSystemsMain.Count; i++)
        {
            float rate;
            if (isFixed)
            {
                rate = testRate;
            }
            else
            {
                rate = 1 + actorController.attackRate * 0.01f;
            }
            //var rate = testRate;
            SetRate(i,rate);
        }
        
    }
}
