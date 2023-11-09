using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C001_1_Boss : Projectile_C001_1
    {
        public GameObject enemySource;
        public Collider2D contactTargetCol;

        public void SetContactPlatform(Collider2D col)
        {
            contactTargetCol = col;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            //print("Enter "+col.gameObject.GetInstanceID());
    
            
    
            if (col.gameObject == contactTarget || col.gameObject.CompareTag("Ground"))
            {
                BurstEffect(col);
                
    
            }else if (contactTargetCol == null && col.gameObject.CompareTag("platform"))
            {
                BurstEffect(col);
            }
            
            else if(col.gameObject.CompareTag("platform") && verticalVelocity < -startVelocityY*0.1f 
                                                          && col == contactTargetCol)
            {
                BurstEffect(col);
            }
        }
        
        protected override void BurstEffect(Collider2D col)
        {
            Vector3 hitpoint = col.bounds.ClosestPoint(transform.position);
            GameObject burst = Instantiate(destroyEffect,new Vector3(hitpoint.x,hitpoint.y+3f,hitpoint.z),Quaternion.identity,transform.parent);
            CineMachineOperator.Instance.CamaraShake(8, .2f);
            
            burst.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            
            Destroy(gameObject);
            
            
        }
    }

}
