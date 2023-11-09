using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C003_2_Boss : ProjectileControllerTest
    {
    
        public GameObject enemySource;
        public GameObject blastPrefab;
        private void Awake()
        {
            firedir = GetComponentInParent<Projectile_C003_3_Boss>().firedir;
            var targetPos = contactTarget.transform.position;
            //var topBorder = contactTarget.transform.position.y + 34f;
            if (firedir == -1)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0); 
            }
            
            var groundSensor = contactTarget.GetComponentInChildren<IGroundSensable>();
            Collider2D col = null;
            if ((col = groundSensor.GetCurrentAttachedGroundCol()) != null)
            {
                //dropDistanceY = topBorder - col.bounds.max.y;
                print("Set!");
                targetPos.y = col.bounds.max.y-1f;
                    
            }
            else
            {
                targetPos.y = contactTarget.transform.position.y;
            }
    
            transform.position = targetPos + new Vector3(-firedir * 20, 34, 0);
    
        }
    
        private void FixedUpdate()
        {
            DoProjectileMove();
        }
    
        protected override void DoProjectileMove()
        {
            transform.position += new Vector3(verticalVelocity*firedir*0.55f, -verticalVelocity, 0) * Time.fixedDeltaTime;
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Ground"))
            {
                BlastEffect(2);
                return;
            }

            if (other.CompareTag("Player"))
            {
                BlastEffect();
                return;
            }

            // var statusEnemy = other.GetComponentInParent<ActorController>();
            // var statusEnemy2 = other.GetComponentInParent<StandardCharacterController>();
            // if (statusEnemy != null || statusEnemy2 != null)
            // {
            //     
            //     BlastEffect();
            //
            // }
        }
        private void BlastEffect(float positionModifier = 0f)
        {
            var prefab = Instantiate(blastPrefab, transform.position+new Vector3(0,positionModifier,0), 
                Quaternion.identity, transform.parent.parent);
    
            var atk = prefab.GetComponent<AttackFromEnemy>();
            atk.enemySource = enemySource;
            Destroy(gameObject);
        }
        
    }
}

