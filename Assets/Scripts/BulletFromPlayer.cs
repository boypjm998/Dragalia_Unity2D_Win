using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFromPlayer : AttackFromPlayer
{
    //speed of bullet,
    //翻滚攻击是1.5,普通攻击是2.4
    public float speed;
    //翻滚攻击的子弹维持0.45s，普通攻击0.4s
    public float lifeTime;
    //子弹长度
    public float hitDistance;
    //伤害倍率,翻滚攻击为0.23f. Dash合计0.80f
    //private float dmgModifier;

    //public GameObject hitConnectEffect;

    public LayerMask targetLayers;

    public RaycastHit2D hitinfo;

   

    private float fireRange;

    // Start is called before the first frame update
    private void Awake()
    {
        
        Invoke("DestroyProjectile", lifeTime);
        fireRange = lifeTime * lifeTime * speed * 50;
        hitFlags = SearchEnemyList();
        //print(fireRange);

    }

    private void Update()
    {
        
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        hitinfo = Physics2D.Raycast(transform.position, transform.right, 0.5f, targetLayers);
        //print(transform.forward);
        if (hitinfo.collider != null)
        {
            if (hitinfo.collider.CompareTag("Enemy") && hitFlags.Contains(hitinfo.collider.transform.parent.GetInstanceID()))
            {
                //print("Hit:" + hitinfo.collider.name);
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
        GameObject eff = Instantiate(hitConnectEffect, transform.position, Quaternion.identity);
        eff.name = "HitEffect0";
        
    }

    public virtual void BulletLinearSplitDamageCheck(float splitDistance)
    {
        RaycastHit2D[] hitsinfo = Physics2D.RaycastAll(transform.position, transform.right, splitDistance, targetLayers);
        print(transform.position+" "+transform.forward);
        
        //print(transform.position.x + splitDistance);
        //print(transform.forward);
        foreach (RaycastHit2D hitinfo in hitsinfo)
        {
            print("Split:"+hitinfo.collider.name);
            DamageCheckRaycast(hitinfo);
        }
        
    }


    /*public virtual void BulletDamageCheckRaycast(RaycastHit2D hitinfo)
    {
        if(hitinfo.collider != null)
        {
            if (hitinfo.collider.CompareTag("Enemy") && hitFlags.Contains(hitinfo.collider.transform.parent.GetInstanceID()))
            {
                //print(hitShakeIntensity);
                PlayDestroyEffect(hitShakeIntensity);
                hitFlags.Remove(hitinfo.collider.transform.parent.GetInstanceID());
                DestroyProjectile();
                hitinfo.collider.GetComponent<Enemy>().TakeDamage();

                //待优化
                GameObject damageManager = GameObject.Find("DamageManager");
                DamageNumberManager dnm = damageManager.GetComponent<DamageNumberManager>();
                int dmg;
                if (Random.Range(0, 100) < 14)
                {
                    dmg = (int)(1699 * Random.Range(0.95f, 1.05f));
                    dnm.DamagePopEnemy(hitinfo.collider.transform, dmg, 2);
                }
                else
                {
                    dmg = (int)(998 * Random.Range(0.95f, 1.05f));
                    dnm.DamagePopEnemy(hitinfo.collider.transform, dmg, 1);
                }

                hitinfo.collider.GetComponent<Enemy>().TakeDamage();

                AttackContainer container = gameObject.GetComponentInParent<AttackContainer>();
                if (container.NeedTotalDisplay())
                    container.AddTotalDamage(dmg);

            }
            
        }
    }
    */
}
