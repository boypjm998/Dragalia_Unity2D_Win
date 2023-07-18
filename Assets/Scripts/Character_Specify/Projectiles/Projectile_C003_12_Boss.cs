using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// twilight crown genesis
    /// </summary>
    public class Projectile_C003_12_Boss : MonoBehaviour
    {
        private AttackFromEnemy _attack;
        private List<ActorBase> caughtTargetsAc = new();
        public float speed;
        void Start()
        {
            _attack = GetComponent<AttackFromEnemy>();
            _attack.OnAttackHit += CaughtTarget;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            transform.position =
                new Vector3(transform.position.x, transform.position.y + speed * Time.fixedDeltaTime);

            foreach (var ac in caughtTargetsAc)
            {
                ac.SetActionUnable(true);
                ac.SetGravityScale(0);
                ac.transform.position =
                    new Vector3(ac.transform.position.x, ac.transform.position.y + speed * Time.fixedDeltaTime);
            }
            
        }

        private void OnDestroy()
        {
            _attack.OnAttackHit -= CaughtTarget;
            foreach (var ac in caughtTargetsAc)
            {
                ac.SetActionUnable(false);
                ac.ResetGravityScale();
            }
        }

        void CaughtTarget(AttackBase attack, GameObject target)
        {
            caughtTargetsAc.Add(target.GetComponent<ActorBase>());
            
        }



    }
    
}

