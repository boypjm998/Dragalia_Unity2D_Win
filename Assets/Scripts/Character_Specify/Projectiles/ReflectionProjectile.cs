using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    public class ReflectionProjectile : ProjectileControllerTest
    {
        public Vector2 velocity
        {
            get => new Vector2(horizontalVelocity, verticalVelocity);
        }
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private bool useRigid = false;
        [SerializeField] private LayerMask contactLayerMasks;
        [SerializeField] private bool rotate;
        [SerializeField][Range(0,15f)] private float randomRange = 0;
        
        protected override void DoProjectileMove()
        {
            
            
            transform.position += new Vector3(horizontalVelocity, verticalVelocity) * Time.fixedDeltaTime;
            
            
            var magnitude = velocity.magnitude;
            var dir = (velocity).normalized;
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position,
                dir, magnitude * Time.fixedDeltaTime,
                contactLayerMasks);
            
            if (hit.collider != null)
            {
                dir = Vector2.Reflect(dir, hit.normal);

                if(randomRange != 0)
                    dir = Quaternion.Euler(0, 0, Random.Range(-randomRange, randomRange)) * dir;
                
                horizontalVelocity = dir.x * magnitude;
                verticalVelocity = dir.y * magnitude;
                transform.right = velocity;
            }
            
        }

        protected override void Start()
        {
            base.Start();
            if (rotate)
            {
                transform.right = velocity;
            }

            
        }

        private void FixedUpdate()
        {
            DoProjectileMove();
        }
    }
}

