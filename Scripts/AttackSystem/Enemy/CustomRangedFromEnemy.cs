using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRangedFromEnemy : AttackFromEnemy
{
    
    protected override void Awake()
    {
        base.Awake();
        if(attackCollider==null)
            attackCollider = GetComponent<Collider2D>();
        //selfpos = transform.parent.parent.parent;
    }
    
    

    // Update is called once per frame
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
    }
}
