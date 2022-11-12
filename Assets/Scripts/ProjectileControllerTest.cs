using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public abstract class ProjectileControllerTest : MonoBehaviour
{
    [SerializeField] protected float rotateSpeed;
    [SerializeField] protected float gravScale;
    [SerializeField] protected float horizontalVelocity;
    [SerializeField] protected float verticalVelocity;
    [SerializeField] protected GameObject contactTarget;
    protected Collider2D _collider;
    protected int firedir;
    protected float lifeTime = 10f;
    

    private void Awake()
    {
        
    }

    void Start()
    {
        Destroy(gameObject,lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitProjectile(float horizontalVelocity, float verticalVelocity, float gravity, int direction)
    {
        this.horizontalVelocity = horizontalVelocity;
        this.verticalVelocity = verticalVelocity;
        this.gravScale = gravity;
        this.firedir = direction;
    }

    protected abstract void DoProjectileMove();

    public abstract void SetContactTarget(GameObject obj);


}
