using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRangedFromEnemy : AttackFromEnemy
{
    //protected EnemyController ac;
    protected override void Awake()
    {
        base.Awake();
        if(attackCollider==null)
            attackCollider = GetComponent<Collider2D>();
        //selfpos = transform.parent.parent.parent;
        
    }

    protected override void Start()
    {
        
        if (isMeele)
        {
            if(ac == null)
                ac = enemySource?.GetComponent<EnemyController>();
            ac.OnAttackInterrupt += DestroyContainer;
        }

        try
        {
            ac = enemySource?.GetComponent<EnemyController>();
            base.Start();
        }
        catch
        {
            
        }
        
    }


    // Update is called once per frame
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Transform enemyTrans;
        //Debug.Log(collision.gameObject.GetInstanceID());
        //print("stay!!!!");
        
        if (collision.CompareTag("Player") && hitFlags.Contains(collision.transform.parent.GetInstanceID()))
        {
            if (Avoidable == AvoidableProperty.Red)
            {
                var knockbackable = collision.GetComponentInParent<IKnockbackable>();
                if (knockbackable.GetDodge())
                {
                    knockbackable.InvokeDodge(this, enemySource);
                    //如果是红圈并且角色在技能中
                    return;
                }

                
                
                var npcController = collision.GetComponentInParent<NpcController>();
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

        if (ac != null)
        {
            ac.OnAttackInterrupt -= DestroyContainer;
        }

    }
}
