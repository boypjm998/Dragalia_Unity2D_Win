using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_C001_2 : ProjectileControllerTest
{
    //Projectile of Ilia: Otherworld Gate.
    
    [SerializeField]
    private GameObject destroyEffect;
    private AttackFromPlayer attackSet;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        lifeTime = 1.2f;
        base.Start();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DoProjectileMove();
    }

    protected override void DoProjectileMove()
    {
        var moveX = horizontalVelocity * Time.fixedDeltaTime;
        transform.position += transform.right * moveX;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            attackSet = GetComponent<AttackFromPlayer>();
            attackSet.CauseDamage(col.gameObject);
            BurstEffect();
        }
        if (col.CompareTag("Border"))
        {
            
            BurstEffect();
        }

        
    }

    private void BurstEffect()
    {
        Vector3 hitpoint = transform.position;
        Instantiate(destroyEffect,hitpoint,Quaternion.identity,transform.parent);
        CineMachineOperator.Instance.CamaraShake(5, .2f);
        
        Destroy(gameObject);
    }

    protected override void MyDestroySelf(float time)
    {
        Invoke(nameof(BurstEffect), time);
    }
}
