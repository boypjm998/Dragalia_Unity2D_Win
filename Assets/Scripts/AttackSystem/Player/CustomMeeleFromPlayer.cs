using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMeeleFromPlayer : AttackFromPlayer
{
    protected ActorController ac;

    Coroutine ConnectCoroutine;
    
    //private float defaultGravity = 4;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        
        //hitFlags = SearchEnemyList();
        
        if(attackCollider==null)
            attackCollider = GetComponent<Collider2D>();
        
        
        
    }

    protected override void Start()
    {
        base.Start();
        playerpos = GameObject.Find("PlayerHandle").transform;
        ac = playerpos.GetComponent<ActorController>();
        if (isMeele)
        {
            ac.OnAttackInterrupt += DestroyContainer;
        }
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
        //print("stay!!!!");
        
        if (collision.CompareTag("Enemy") && hitFlags.Contains(collision.transform.parent.GetInstanceID()))
        {

            CauseDamage(collision.gameObject);
            
            if(ConnectCoroutine==null && isMeele==true)
            { 
                ConnectCoroutine = StartCoroutine(MeeleTimeStop(0.1f));
            }
            

        }
        
    }
    public override IEnumerator MeeleTimeStop(float time)
    {
        
        Animator animAttack = GetComponentInParent<Animator>();
        if(animAttack == null)
            print("animAttack is null");
        Rigidbody2D rigid = playerpos.gameObject.GetComponentInParent<Rigidbody2D>();
        Animator anim = playerpos.gameObject.GetComponentInParent<ActorController>().anim;
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
        ac.OnAttackInterrupt -= DestroyContainer;
        if(isMeele)
            RecoverFromMeeleTimeStop(4);
    }
}
