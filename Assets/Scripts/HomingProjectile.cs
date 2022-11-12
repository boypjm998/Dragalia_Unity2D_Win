using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : AttackFromPlayer
{
    //public GameObject player;

    private TargetAimer ta;
    
    private Transform target;
    [Header("Projectile Basic Attributes")]
    public LayerMask targetLayers;
    public Vector2 angle;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float lifeTime;
    [SerializeField]
    private float acceleration = 0;
    [SerializeField]
    private float homingStartTime;
    [SerializeField]
    private float angularAcceleration;
    [SerializeField]
    private float angularSpeed;

    [Header("Direction Fix")]
    [SerializeField]
    private bool directionFix;
    [SerializeField]
    private Vector3 smoothRotateVector;
    [SerializeField]
    private float preAngularSpeed;

    [Header("Target Late Lock Settings")]
    [SerializeField]
    private float targetLockTime;
    public float targetRangeX;
    public float targetRangeY;

    private Vector3 smoothPoint;
    
    [Header("Damage Split Settings")]
    public float splitDistance; 
    public RaycastHit2D hitinfo;

    // Start is called before the first frame update
    private void Awake()
    {
        
        playerpos = GameObject.Find("PlayerHandle").transform;
        ta = playerpos.gameObject.GetComponentInChildren<TargetAimer>();

        hitFlags = SearchEnemyList();

    }

    protected override void Start()
    {
        base.Start();
        SetForward(angle);
        smoothRotateVector = new Vector3(smoothRotateVector.x * angle.x, smoothRotateVector.y * angle.y, 0);
        smoothPoint = transform.position + smoothRotateVector;
        //print(smoothRotateVector);
        //target = GetNearestTargetInRangeDirection(GetFaceDir(), targetRangeX, targetRangeY, targetLayers);
        //print(target);
        Invoke(nameof(DestroyProjectile), lifeTime);
    }

    

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (targetLockTime > 0)
        {
            targetLockTime -= Time.fixedDeltaTime;
            
        }
        else if(target == null) {
            target = GetNearestTargetInRangeDirection(GetFaceDir(), targetRangeX, targetRangeY, targetLayers);
        }





        hitinfo = Physics2D.Raycast(transform.position, transform.forward, 0.5f, targetLayers);
        if (hitinfo.collider != null)
        {
            
            if (hitinfo.collider.CompareTag("Enemy") && hitFlags.Contains(hitinfo.collider.transform.parent.GetInstanceID()))
            {
                
                BulletCircleSplitDamageCheck(splitDistance);

            }
        }

        if (homingStartTime > 0)
        {
            homingStartTime -= Time.fixedDeltaTime;
            
            if (smoothRotateVector != Vector3.zero)
            {
                transform.forward =
                    Vector3.Slerp(transform.forward, smoothPoint - transform.position,
                    preAngularSpeed / Vector2.Distance(transform.position,smoothPoint));
                
            }




            if (homingStartTime <= 0 && directionFix)
            {
                
               transform.forward = new Vector2(GetFaceDir(), 0);
               
            }
        }
        else
        {
            if (target != null)
            {
                transform.forward =
                    Vector3.Slerp(transform.forward, target.position - transform.position,
                    angularSpeed / Vector2.Distance(transform.position, target.transform.position));
                angularSpeed += angularAcceleration * Time.fixedDeltaTime;
                if (angularSpeed > 5)
                    angularSpeed = 5;
            }
            else
            {
                transform.forward = new Vector2(GetFaceDir(), 0);
            }
        }

        transform.position += transform.forward * speed * Time.fixedDeltaTime;
        speed += acceleration * Time.fixedDeltaTime;
    }

    void DestroyProjectile()
    {
        //print(transform.position.x);
        Destroy(gameObject);
    }

    void PlayDestroyEffect(Transform enemyTrans, float shakeIntensity)
    {
        CineMachineOperator.Instance.CamaraShake(shakeIntensity, .1f);
        GameObject eff = Instantiate(hitConnectEffect, transform.position, Quaternion.identity);
        eff.name = "HitEffect0";

    }

    public void SetForward(Vector2 forwardVector)
    {
        transform.forward = forwardVector;
    }

    public Transform GetNearestTargetInRangeDirection(int dir, float sizeX, float sizeY, LayerMask targetLayers)
    {
        float leftModifier = 0;
        float rightModifier = 0;
        if (dir == 1)
        {
            leftModifier = 0;
            rightModifier = 1;
        }
        else
        {
            leftModifier = 1;
            rightModifier = 0;
        }
        Collider2D[] AttackRangeAreaInfo =
            Physics2D.OverlapAreaAll(transform.position + new Vector3(sizeX * rightModifier, sizeY), transform.position + new Vector3(-sizeX * leftModifier, -sizeY),
            targetLayers);

        if (AttackRangeAreaInfo.Length == 0)
            return null;

        Transform target = AttackRangeAreaInfo[0].transform;
        float minDistance = Vector3.Distance(transform.position, target.position);

        for (int i = 1; i < AttackRangeAreaInfo.Length; i++)
        {
            float tempDistance = Vector3.Distance(transform.position, AttackRangeAreaInfo[i].transform.position);
            if (tempDistance < minDistance)
            {
                target = AttackRangeAreaInfo[i].transform;
                minDistance = Vector3.Distance(transform.position, target.position);
            }
        }




        return target;
    }

    public virtual void BulletCircleSplitDamageCheck(float splitRadius)
    {
        Collider2D[] hitsinfo = Physics2D.OverlapCircleAll(transform.position, splitRadius,targetLayers);
        print("Arrive1");
        foreach (Collider2D hitinfo in hitsinfo)
        {
            //print("Split:" + hitinfo.GetComponent<Collider2D>().name);
            DamageCheckCollider(hitinfo);
        }

    }



    /*public virtual void BulletDamageCheckCollider(Collider2D hitinfo)
    {
        if (hitinfo != null)
        {
            if (hitinfo.CompareTag("Enemy") && hitFlags.Contains(hitinfo.transform.parent.GetInstanceID()))
            {
                //print(hitShakeIntensity);
                PlayDestroyEffect(hitShakeIntensity);

                DestroyProjectile();
                hitinfo.GetComponent<Enemy>().TakeDamage();

                int dmg = battleStageManager.PlayerHit(hitinfo.gameObject, this);

                /*GameObject damageManager = GameObject.Find("DamageManager");
                DamageNumberManager dnm = damageManager.GetComponent<DamageNumberManager>();
                int dmg;
                if (Random.Range(0, 100) < 14)
                {
                    dmg = (int)(1699 * Random.Range(0.95f, 1.05f));
                    dnm.DamagePopEnemy(hitinfo.transform, dmg, 2);
                }
                else
                {
                    dmg = (int)(998 * Random.Range(0.95f, 1.05f));
                    dnm.DamagePopEnemy(hitinfo.transform, dmg, 1);
                }

                

                AttackContainer container = gameObject.GetComponentInParent<AttackContainer>();
                if (container.NeedTotalDisplay())
                    container.AddTotalDamage(dmg);

            }

        }
    }*/


    //private void OnDestroy()
    //{
    //    AttackContainer container = GetComponentInParent<AttackContainer>();
    //    container.FinishHit();
    //}


}
