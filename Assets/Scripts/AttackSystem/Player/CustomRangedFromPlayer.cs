using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRangedFromPlayer : AttackFromPlayer
{
    protected ActorBase ac;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        hitFlags = SearchEnemyList();
        if(attackCollider==null)
            attackCollider = GetComponent<Collider2D>();
    }
    protected override void Start()
    {
        base.Start();
        ac = playerpos.GetComponent<ActorBase>();

        if (isMeele && ac!=null)
        {
            ac.OnAttackInterrupt += DestroyContainer;
        }
    }
  

    // Update is called once per frame
    void Update()
    {
        
    }
    
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        //Transform enemyTrans;
        //Debug.Log(collision.gameObject.GetInstanceID());
        print(collision.gameObject);
        
        if (collision.CompareTag("Enemy") && hitFlags.Contains(collision.transform.parent.GetInstanceID()))
        {
            CauseDamage(collision.gameObject);
            
            
            
            
            

        }
        
        
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (ac != null)
        {
            ac.OnAttackInterrupt -= DestroyContainer;
        }
    }
}
