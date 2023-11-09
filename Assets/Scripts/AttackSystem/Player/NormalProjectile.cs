using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProjectile : ProjectileControllerTest
{
    protected Rigidbody2D rigid;
    protected bool hasRigid = false;
    public Transform body;

    public bool bounceWhenAttachBorder = false;
    
    [SerializeField]
    protected float bounciness = 1;

    protected AttackBase attackBase;
    protected Vector3 scaleFactors;

    private void Awake()
    {
        rigid = GetComponentInParent<Rigidbody2D>();
        if (rigid != null)
            hasRigid = true;

        rigid.sharedMaterial.bounciness = bounciness;
        if (body == null)
            body = transform;

        scaleFactors = body.localScale;
        attackBase = GetComponentInParent<AttackBase>();
        
    }

    protected override void Start()
    {
        base.Start();
        attackBase.firedir = firedir;
    }

    protected void OnCollisionEnter2D(Collision2D other)
        {
            if(bounceWhenAttachBorder == false)
                return;
            
            //如果接触的碰撞体Layer是Border，就将firedir乘以-1
            // if (other.gameObject.layer == LayerMask.NameToLayer())
            // {
            //
            //     if (attackBase.firedir == 1 && transform.position.x < other.collider.bounds.center.x)
            //     {
            //         attackBase.firedir = -1;
            //         transform.parent.localScale = new Vector3(-scaleFactors.x,scaleFactors.y,scaleFactors.z);
            //         
            //         return;
            //     }else if (attackBase.firedir == -1 && transform.position.x > other.collider.bounds.center.x)
            //     {
            //         attackBase.firedir = 1;
            //         transform.parent.localScale = new Vector3(scaleFactors.x,scaleFactors.y,scaleFactors.z);
            //         return;
            //     }
            // }
            
            if (other.gameObject.layer == LayerMask.NameToLayer("Border"))
            {
                if (attackBase.firedir == 1 && transform.position.x < other.collider.bounds.center.x)
                {
                    attackBase.firedir = -1;
                    firedir = -1;
                    transform.parent.localScale = new Vector3(-scaleFactors.x,scaleFactors.y,scaleFactors.z);
                    return;
                }
                
                if (attackBase.firedir == -1 && transform.position.x > other.collider.bounds.center.x)
                {
                    attackBase.firedir = 1;
                    firedir = 1;
                    transform.parent.localScale = new Vector3(scaleFactors.x,scaleFactors.y,scaleFactors.z);
                    return;
                }
            }
            else
            {
                print("not layer border");
            }
        }

    

    private void FixedUpdate()
    {
        DoProjectileMove();
    }

    protected override void DoProjectileMove()
    {
        if(hasRigid)
            rigid.position += new Vector2(firedir * horizontalVelocity, verticalVelocity) * Time.fixedDeltaTime;

        else
        {
            body.position += new Vector3(firedir * horizontalVelocity, verticalVelocity) * Time.fixedDeltaTime;
        }
        
        
    }
}
