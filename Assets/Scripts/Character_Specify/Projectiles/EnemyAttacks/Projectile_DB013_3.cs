using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Orb without attack
/// </summary>
public class Projectile_DB013_3 : Projectile_DB013_EnlightmentOrb
{
    [SerializeField] private float lifeTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,lifeTime);
    }

    
}
