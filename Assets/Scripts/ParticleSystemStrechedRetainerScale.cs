using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemStrechedRetainerScale : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.MainModule psmain;
    [SerializeField]private Transform _parent;
    
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        psmain = ps.main;
        

    }
    private void OnEnable()
    {
        
        var attackBase = _parent.GetComponent<AttackBase>();
        float fireDir;
        if(attackBase != null)
            fireDir = attackBase.firedir;
        else
        {
            fireDir = _parent.transform.lossyScale.x > 0? 1:-1;
        }
        
        
        //var dir = (_parent.GetComponent<AttackBase>().firedir);
        var dir = fireDir;
        print(dir);
        if (dir < 0)
        {
            var shape = ps.shape;
            shape.scale = new Vector3(-1, 1, -1);
            //("shaped");
        }
        else
        {
            var shape = ps.shape;
            shape.scale = new Vector3(1, 1, 1);
            //("shaped");
        }
    }

    
}
