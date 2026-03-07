using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This script should be attached to a child of a game object with a ForcedAttackFromEnemy script attached.
/// </summary>
public class GroundTrap : MonoBehaviour
{
    private ForcedAttackFromEnemy _forcedAttackFromEnemy;
    
    [SerializeField] private Collider2D _attachedCollider;
    
    private void Start()
    {
        _forcedAttackFromEnemy = GetComponentInParent<ForcedAttackFromEnemy>();
        //_attachedCollider = GetComponent<Collider2D>();
    }
    
    public void SetCollider(Collider2D collider)
    {
        _attachedCollider = collider;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _forcedAttackFromEnemy.hitFlags.Contains(other.transform.parent.GetInstanceID()))
        {
            var sensor = other.transform.parent.GetComponentInChildren<IGroundSensable>();
            var anim = other.transform.parent.GetComponent<ActorController>().anim;
            if (sensor != null)
            {
                if(sensor.GetCurrentAttachedGroundCol() == _attachedCollider && anim.GetBool("isGround"))
                    _forcedAttackFromEnemy.CauseDamage(other);
            }
        }
    }
}
