using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomMeeleFromPlayer : AttackFromPlayer
{
    /*
     *
     * The "Meele" word not indicate that it is an actual meele attack.
     * It only present a property that 'When Attacker is interrupt by hurt or something else,
     * this attack will be cancelled'.
     *
    */
   

    Coroutine ConnectCoroutine;
    
    //private float defaultGravity = 4;

    // Start is called before the first frame update
    private void Awake()
    {
        
        hitFlags = SearchEnemyList();
        attackCollider = GetComponent<Collider2D>();
        playerpos = GameObject.Find("PlayerHandle").transform;
        Rigidbody2D rigid = playerpos.gameObject.GetComponentInParent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
        
        //defaultGravity = rigid.gravityScale;
        self = gameObject;
    }

    public void InstantDestroySelf()
    {
        hitFlags.Clear();
        RecoverFromMeeleTimeStop(4);
        Destroy(self);

    }
  
    private void OnTriggerStay2D(Collider2D collision)
    {
        Transform enemyTrans;
        //Debug.Log(collision.gameObject.GetInstanceID());
        print("stay!!!!");
        
        if (collision.CompareTag("Enemy") && hitFlags.Contains(collision.transform.parent.GetInstanceID()))
        {

            //print(collision.name);

            hitFlags.Remove(collision.transform.parent.GetInstanceID());

            enemyTrans = collision.transform.parent;

            int dmg = battleStageManager.PlayerHit(collision.gameObject, this);
            
            

            GameObject eff = Instantiate(hitConnectEffect, new Vector2(collision.transform.position.x,transform.position.y), Quaternion.identity);
            eff.name = "HitEffect1";
            CineMachineOperator.Instance.CamaraShake(hitShakeIntensity, .1f);
            collision.gameObject.GetComponent<Enemy>().TakeDamage();
            
            AttackContainer container = gameObject.GetComponentInParent<AttackContainer>();
            container.AttackOneHit();
            if (container.NeedTotalDisplay() && dmg > 0)
                container.AddTotalDamage(dmg);
            
            
            if(ConnectCoroutine==null && isMeele==true)
            { 
                ConnectCoroutine = StartCoroutine(MeeleTimeStop(0.1f));
            }
            

        }
        
    }
    public override IEnumerator MeeleTimeStop(float time)
    {
        
        Animator animAttack = GetComponentInParent<Animator>();
        Rigidbody2D rigid = playerpos.gameObject.GetComponentInParent<Rigidbody2D>();
        Animator anim = playerpos.gameObject.GetComponentInParent<Animator>();
        //Debug.Log(parent);
        animAttack.speed = 0.5f;
        anim.speed = 0;
        float gravity = rigid.gravityScale;
        //print(gravity);
        rigid.gravityScale = 0;
        yield return new WaitForSeconds(time);

        RecoverFromMeeleTimeStop(4);

       
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(isMeele)
            RecoverFromMeeleTimeStop(4);
    }


}
