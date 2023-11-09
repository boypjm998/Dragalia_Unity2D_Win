using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that forces the enemy to attack the player. This is used for the boss. Need to distribute enemySource before using this class.
/// </summary>
public class ForcedAttackFromEnemy : AttackFromEnemy
{
    private EnemyController ac;
    public GameObject target;
    public List<GameObject> extraTargets = new();
    public float triggerTime;
    public bool isAoE = false;
    protected override void Awake()
    {
        base.Awake();
        Avoidable = AvoidableProperty.Forced;
        // if(attackCollider==null)
        //     attackCollider = GetComponent<Collider2D>();
        //selfpos = transform.parent.parent.parent;
        if (isMeele)
        {
            ac.OnAttackInterrupt += DestroyContainer;
        }
    }

    protected override void Start()
    {
        base.Start();
        if (isAoE)
        {
            extraTargets.AddRange(DragaliaEnemyBehavior.GetPlayerList());
        }


        if(triggerTime >= 0)
            Invoke("CauseDamageInstantly", triggerTime);
    }


    private void CauseDamageInstantly()
    {

        if (target != null)
        {
            var col = target.transform.Find("HitSensor")?.GetComponent<Collider2D>();
            if (col != null)
            {
                CauseDamage(col);
            }
        }

        
        
        if(extraTargets.Count > 0)
            foreach (var t in extraTargets)
            {
                var col = t.transform.Find("HitSensor").GetComponent<Collider2D>();
                if(col == null)
                    continue;
                if(t!=target)
                    CauseDamage(col);
            }
        
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Player") && hitFlags.Contains(collision.transform.parent.GetInstanceID()))
        {

            CauseDamage(collision);

        }
        
    }

    public override void NextAttack()
    {
        base.NextAttack();
        CauseDamageInstantly();
    }

    public void DealDamageImmediately()
    {
        CauseDamageInstantly();
    }
}
