using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedAttackFromPlayer : AttackFromPlayer
{
    private ActorController ac;
    public GameObject target;
    public List<GameObject> extraTargets = new();
    public float triggerTime;
    protected override void Awake()
    {
        base.Awake();
        // if(attackCollider==null)
        //     attackCollider = GetComponent<Collider2D>();
        //selfpos = transform.parent.parent.parent;
        if (isMeele)
        {
            ac.OnAttackInterrupt += DestroyContainer;
        }
        Invoke("CauseDamageInstantly", triggerTime);
    }

    private void CauseDamageInstantly()
    {

        var col = target.transform.Find("HitSensor")?.GetComponent<Collider2D>();
        if(col == null)
            return;
        if (col.CompareTag("Player"))
        {
            
            return;
        }
        print("FORCE TARGET:"+col.transform.parent.name);

        

        if (hitFlags.Contains(col.transform.parent.GetInstanceID()))
        {
            //print("ForcedAttackCausedAuto");
            CauseDamage(col);
        }

        
        if(extraTargets.Count > 0)
            foreach (var t in extraTargets)
            {
                col = t.transform.Find("HitSensor").GetComponent<Collider2D>();
                if(col == null)
                    continue;
                CauseDamage(col);
            }
        
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Enemy") && hitFlags.Contains(collision.transform.parent.GetInstanceID()))
        {

            print("ForcedAttackCausedByCollision");
            CauseDamage(collision);

        }
        
    }

    public override void NextAttack()
    {
        base.NextAttack();
        CauseDamageInstantly();
    }
    
}
