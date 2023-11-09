using UnityEngine;


public class BulletFromEnemy : AttackFromEnemy
{
    public float speed;
    //翻滚攻击是0.45s,普通攻击是0.4s
    public float lifeTime;
    //溅射范围
    public float hitDistance;
    //攻击倍率。
    //private float dmgModifier;
    
    public LayerMask targetLayers;

    public RaycastHit2D hitinfo;

   

    private float fireRange;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        Invoke(nameof(DestroyProjectile), lifeTime);
        fireRange = lifeTime * lifeTime * speed * 50;
        
        //print(fireRange);

    }
    


    // Update is called once per frame
    void FixedUpdate()
    {
        hitinfo = Physics2D.Raycast(transform.position, transform.right, 0.5f, targetLayers);
        


        //print(transform.forward);
        if (hitinfo.collider != null)
        {
            if (hitinfo.collider.CompareTag("Player") && hitFlags.Contains(hitinfo.collider.transform.parent.GetInstanceID()))
            {
                if (Avoidable == AvoidableProperty.Red)
                {
                    var knockbackable = hitinfo.collider.GetComponentInParent<IKnockbackable>();
                    if (knockbackable.GetDodge())
                    {
                        knockbackable.InvokeDodge(this,enemySource);
                        transform.Translate(Vector2.right * speed * lifeTime);
                        return;
                    }

                    //如果是红圈并且角色在技能中
                        
                
                    var npcController = hitinfo.collider.GetComponentInParent<NpcController>();
                    if (npcController != null)
                    {
                        if(npcController.enabled == false)
                            return;
                        if(npcController.CurrentMainRoutineType != NpcController.MainRoutineType.Attack &&
                           !npcController.subMoveRoutineIsRunning);
                        {
                            if (npcController.IsGround)
                            {
                                npcController.EvadeRoll();
                                return;
                            }
                        }
                    }
                
                }
                
                BulletLinearSplitDamageCheck(hitDistance);
                
                
                
                
            }
        }


        //transform.Translate(new Vector2(1, 0) * speed * lifeTime);
        transform.Translate(Vector2.right * speed * lifeTime);
    }
    void DestroyProjectile() {
        //print(transform.position.x);
        Destroy(gameObject);
    }
    public override void PlayDestroyEffect(float shakeIntensity)
    {
        CineMachineOperator.Instance.CamaraShake(shakeIntensity, .1f);
        if(hitConnectEffect == null)
            return;
        GameObject eff = Instantiate(hitConnectEffect, transform.position, Quaternion.identity);
        eff.name = "HitEffect0";
        
    }

    public virtual void BulletLinearSplitDamageCheck(float splitDistance)
    {
        RaycastHit2D[] hitsinfo = Physics2D.RaycastAll(transform.position, transform.right, splitDistance, targetLayers);
        //print(transform.position+" "+transform.forward);
        
        //print(transform.position.x + splitDistance);
        //print(transform.forward);
        foreach (RaycastHit2D hitinfo in hitsinfo)
        {
            
            
            
            
            //print("Split:"+hitinfo.collider.name);
            DamageCheckRaycast(hitinfo);
        }
        
    }
}
