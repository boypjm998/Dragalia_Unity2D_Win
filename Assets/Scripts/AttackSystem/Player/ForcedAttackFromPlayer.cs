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
        if(col.CompareTag("Player"))
            return;
        
        CauseDamage(col.gameObject);
        
        if(extraTargets.Count > 0)
            foreach (var t in extraTargets)
            {
                col = t.transform.Find("HitSensor").GetComponent<Collider2D>();
                if(col == null)
                    continue;
                CauseDamage(col.gameObject);
            }
        
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Enemy") && hitFlags.Contains(collision.transform.parent.GetInstanceID()))
        {

            CauseDamage(collision.gameObject);

        }
        
    }

    public override void NextAttack()
    {
        base.NextAttack();
        CauseDamageInstantly();
    }
    
}
