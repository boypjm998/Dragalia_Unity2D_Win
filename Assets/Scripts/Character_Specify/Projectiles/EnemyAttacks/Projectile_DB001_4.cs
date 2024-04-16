using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_DB001_4 : ProjectileControllerTest
    {
        AttackFromEnemy attackFromEnemy;
        public float speed;

        private void Awake()
        {
            attackFromEnemy = GetComponentInParent<AttackFromEnemy>();
        }

        private void FixedUpdate()
        {
            DoProjectileMove();
        }

        protected void OnCollisionEnter2D(Collision2D other)
        {
            //如果接触的碰撞体Layer是Border，就将firedir乘以-1
            if (other.gameObject.layer == LayerMask.NameToLayer("EnemyObstacle"))
            {
                if (other.collider.GetComponentInParent<Projectile_DB001_2>() == null)
                {
                    print("not projectile");
                    return;
                }

                if (attackFromEnemy.firedir == 1 && transform.position.x < other.collider.bounds.center.x)
                {
                    attackFromEnemy.firedir = -1;
                    print("set firedir to -1");
                    return;
                }else if (attackFromEnemy.firedir == -1 && transform.position.x > other.collider.bounds.center.x)
                {
                    attackFromEnemy.firedir = 1;
                    return;
                }
            }
            
            else if (other.gameObject.layer == LayerMask.NameToLayer("Border"))
            {
                if (attackFromEnemy.firedir == 1 && transform.position.x < other.collider.bounds.center.x)
                {
                    attackFromEnemy.firedir = -1;
                    return;
                }
                
                if (attackFromEnemy.firedir == -1 && transform.position.x > other.collider.bounds.center.x)
                {
                    attackFromEnemy.firedir = 1;
                    return;
                }
            }
            else
            {
                print("not layer border");
            }
        }

        protected override void DoProjectileMove()
        {
            
            var dirSpd = new Vector3(attackFromEnemy.firedir * speed,0);
            transform.parent.position += dirSpd * Time.fixedDeltaTime;
            print(attackFromEnemy.firedir);
        }
    }
}

