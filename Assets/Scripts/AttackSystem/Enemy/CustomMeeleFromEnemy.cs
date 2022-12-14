using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMeeleFromEnemy : AttackFromEnemy
{
    // Start is called before the first frame update
    private EnemyController ac;
    
    protected override void Awake()
    {
        base.Awake();
        
    }

    void Start()
    {
        if(attackCollider==null)
            attackCollider = GetComponent<Collider2D>();
        selfpos = transform.parent.parent.parent;
        if (enemySource == null)
        {
            enemySource = selfpos?.gameObject;
        }

        //print(enemySource);
        ac = enemySource.GetComponent<EnemyController>();
        if (isMeele)
        {
            ac.OnAttackInterrupt += DestroyContainer;
        }
    }

    
    private void OnTriggerStay2D(Collider2D collision)
    {
        Transform enemyTrans;
        //Debug.Log(collision.gameObject.GetInstanceID());
        //print("stay!!!!");
        
        if (collision.CompareTag("Player") && hitFlags.Contains(collision.transform.parent.GetInstanceID()))
        {
            if (Avoidable == AvoidableProperty.Red)
            {
                if(collision.GetComponentInParent<ActorController>().dodging)
                //如果是红圈并且角色在技能中
                return;
            }

            CauseDamage(collision);
            
            if(ConnectCoroutine==null && isMeele==true)
            { 
                //ConnectCoroutine = StartCoroutine(MeeleTimeStop(0.1f));
            }
            

        }
        
    }
    

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ac.OnAttackInterrupt -= DestroyContainer;
    }
    
    public void InstantDestroySelf()
    {
        hitFlags.Clear();
        //RecoverFromMeeleTimeStop(4);
        Destroy(gameObject);
    }
    
}
