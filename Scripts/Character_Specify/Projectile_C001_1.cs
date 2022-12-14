using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_C001_1 : ProjectileControllerTest
{
    [SerializeField]
    private GameObject destroyEffect;

    private float startVelocityY;
    public AttackFromPlayer attackSet;
    
    // Projectile of Ilia: Alchemic Grenade
    
    // Start is called before the first frame update
    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        
    }

    protected override void Start()
    {
        base.Start();
        startVelocityY = verticalVelocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DoProjectileMove();
        
        

    }

    protected override void DoProjectileMove()
    {
        var moveX = horizontalVelocity * Time.fixedDeltaTime;
        var moveY = verticalVelocity * Time.fixedDeltaTime;
        transform.position += transform.right * moveX;
        transform.position += transform.up * moveY;
        verticalVelocity -= gravScale * Time.fixedDeltaTime;
        
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.z + rotateSpeed * Time.fixedDeltaTime);
        
    }

    public override void SetContactTarget(GameObject obj)
    {
        this.contactTarget = obj;
    }
    
    
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        print("Enter "+col.gameObject.GetInstanceID());

        

        if (col.gameObject == contactTarget || col.gameObject.CompareTag("Ground"))
        {
            BurstEffect(col);
            

        }else if(col.gameObject.CompareTag("platform") && verticalVelocity < -startVelocityY )

        {
            BurstEffect(col);
        }
    }


    private void BurstEffect(Collider2D col)
    {
        Vector3 hitpoint = col.bounds.ClosestPoint(transform.position);
        GameObject burst = Instantiate(destroyEffect,new Vector3(hitpoint.x,hitpoint.y+3f,hitpoint.z),Quaternion.identity,transform.parent);
        CineMachineOperator.Instance.CamaraShake(8, .2f);
        Destroy(gameObject);
        burst.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(0,0,0,2.35f,0,firedir);
        burst.GetComponent<AttackFromPlayer>().AppendAttackSets(200, 9, 1.5f, 23.54f);
    }
}
