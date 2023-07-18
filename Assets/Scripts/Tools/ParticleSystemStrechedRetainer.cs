using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemStrechedRetainer : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.MainModule psmain;
    [SerializeField]private Transform _parent;
    private Vector3 startRotation;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        psmain = ps.main;
        

    }

    private void Start()
    {
        startRotation = ps.shape.rotation;
        var attackBase = _parent.GetComponent<AttackBase>();
        float fireDir;
        if(attackBase != null)
            fireDir = attackBase.firedir;
        else
        {
            fireDir = _parent.transform.localScale.x > 0? 1:-1;
        }
        
        
        //var dir = (_parent.GetComponent<AttackBase>().firedir);
        var dir = fireDir;
        print(dir);
        if (dir < 0)
        {
            var shape = ps.shape;
            shape.rotation = new Vector3(startRotation.x-180, 0, 0);
            print("shaped");
        }
        else
        {
            
        }
    }

    private void OnEnable()
    {
        
    }
}
