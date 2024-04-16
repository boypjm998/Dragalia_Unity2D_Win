using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// chasing storm wall
    /// </summary>
    public class Projectile_DB001_3 : Projectile_DB001_2
    {
        protected float remainTime;
        public GameObject target;
        [SerializeField] private float speed;
        [SerializeField] private float stopChasingTime;

        [SerializeField] private GameObject attack;
        
        // Start is called before the first frame update
        void Awake()
        {
            remainTime = attackReleaseTime;
            Invoke("ReleaseAttack", attackReleaseTime);
        }

        private void FixedUpdate()
        {
            remainTime -= Time.fixedDeltaTime;
            
            if(remainTime < stopChasingTime)
                return;
            
            if(target.transform.position.x - transform.position.x > 0.5f)
                transform.position += Vector3.right * speed * Time.fixedDeltaTime;
            else if(transform.position.x - target.transform.position.x > 0.5f)
                transform.position += Vector3.left * speed * Time.fixedDeltaTime;
            
            
            
        }

        protected override void ReleaseAttack()
        {
            attack.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            attack.SetActive(true);
        }


    }
}

