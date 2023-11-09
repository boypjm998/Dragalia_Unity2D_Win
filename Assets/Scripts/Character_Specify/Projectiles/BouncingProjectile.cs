using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 必须被安置在一个父物体上，这个父物体必须有AttackBase组件
    /// </summary>
    public class BouncingProjectile : ProjectileControllerTest
    {
        
        public float HorizontalVelocity => horizontalVelocity;
        public float VerticalVelocity => verticalVelocity;
        
        //public float speed;
        AttackBase attackBase;
        private string tag;
        public bool isEnemy = true;
        private int dirY = 1;
        public bool scaleWithAttack = true;

        private void Awake()
        {
            attackBase = GetComponentInParent<AttackBase>();
            
            if (isEnemy)
            {
                tag = "EnemyObstacle";
            }
            else
            {
                tag = "PlayerObstacle";
            }
        }

        private void FixedUpdate()
        {
            DoProjectileMove();
        }

        protected void OnCollisionEnter2D(Collision2D other)
        {

            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                if(dirY == 1 && transform.position.y < other.collider.bounds.center.y)
                {
                    dirY = -1;
                    return;
                }
                else if (dirY == -1 && transform.position.y > other.collider.bounds.center.y)
                {
                    dirY = 1;
                    return;
                }
            }


            //如果接触的碰撞体Layer是Border，就将firedir乘以-1
            if (other.gameObject.layer == LayerMask.NameToLayer(tag))
            {

                if (attackBase.firedir == 1 && transform.position.x < other.collider.bounds.center.x)
                {
                    attackBase.firedir = -1;
                    if(scaleWithAttack)
                        transform.parent.localScale = new Vector3(-1,1,1);
                    return;
                }else if (attackBase.firedir == -1 && transform.position.x > other.collider.bounds.center.x)
                {
                    attackBase.firedir = 1;
                    if(scaleWithAttack)
                        transform.parent.localScale = new Vector3(1,1,1);
                    return;
                }
            }
            
            else if (other.gameObject.layer == LayerMask.NameToLayer("Border"))
            {
                if (attackBase.firedir == 1 && transform.position.x < other.collider.bounds.center.x)
                {
                    attackBase.firedir = -1;
                    if(scaleWithAttack)
                        transform.parent.localScale = new Vector3(-1,1,1);
                    return;
                }
                
                if (attackBase.firedir == -1 && transform.position.x > other.collider.bounds.center.x)
                {
                    attackBase.firedir = 1;
                    if(scaleWithAttack)
                        transform.parent.localScale = new Vector3(1,1,1);
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
            
            var dirSpd = new Vector3(attackBase.firedir * horizontalVelocity,verticalVelocity * dirY);
            transform.parent.position += dirSpd * Time.fixedDeltaTime;
            print(attackBase.firedir);
        }

        public override void SetVelocity(Vector2 velocity)
        {
            
        }
    }

}
