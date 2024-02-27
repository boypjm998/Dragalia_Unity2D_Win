using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class WandingProjectile : ProjectileControllerTest
    {
        private AttackBase attackBase;
        public float wandingEdge = 0.5f;
        private Platform wandingPlatform = null;
        private float leftEdge;
        private float rightEdge;
        public bool flip = false;
        
        public void SetWandingPlatform(Platform platform)
        {
            wandingPlatform = platform;
            SetEdges();
        }
        
        public void SetWandingPlatform(string name)
        {
            var list = BattleStageManager.InitMapInfo();
            //找到list中platform的name为name的那个
            
            foreach (var platform in list)
            {
                if (platform.collider.name == name)
                {
                    wandingPlatform = platform;
                    break;
                }
            }
            
            SetEdges();
        }

        protected void SetEdges()
        {
            leftEdge = (wandingPlatform.leftBorderPos).SafePosition(new Vector2(wandingEdge,0)).x;
            rightEdge = (wandingPlatform.rightBorderPos).SafePosition(new Vector2(-wandingEdge,0)).x;
        }

        private void Awake()
        {
            attackBase = GetComponentInParent<AttackBase>();
        }

        protected override void Start()
        {
            if(lifeTime > 0)
                Destroy(gameObject,lifeTime);
        }


        private void FixedUpdate()
        {

            DoProjectileMove();
        }


        protected override void DoProjectileMove()
        {
            transform.position = new Vector3(firedir * (horizontalVelocity * Time.fixedDeltaTime) + transform.position.x,
                transform.position.y);

            if (firedir == 1 && transform.position.x > rightEdge)
            {
                firedir = -1;
                attackBase.firedir = -1;
                if(flip)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            if (firedir == -1 && transform.position.x < leftEdge)
            {
                firedir = 1;
                attackBase.firedir = 1;
                if(flip)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                
            }


        }
    }

}
