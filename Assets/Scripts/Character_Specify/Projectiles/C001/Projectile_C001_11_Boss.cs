using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 撞击BOSS的特效
    /// </summary>
    public class Projectile_C001_11_Boss : MonoBehaviour
    {
        private RelativePositionRetainer _retainer;
        private Transform _parent;

        public Vector2 velocity = Vector2.zero;

        private Vector3 _lastPosition;

        private void Awake()
        {
            _retainer = GetComponent<RelativePositionRetainer>();
        }

        private void Start()
        {
            _parent = _retainer.GetParent();
        }

        public void DisableRelativePositionScript()
        {
            _retainer.enabled = false;
        }
        
        private void FixedUpdate()
        {
            var diff = _parent.position - _lastPosition;

            velocity = (Vector2)(diff) / Time.fixedDeltaTime;

            _lastPosition = _parent.position;

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var refProj = other.GetComponent<ReflectionProjectile>();

            if (refProj == null)
            {
                return;
            }

            var refDir = transform.position -  other.transform.position;

            var magnitude = -Mathf.Max(5, velocity.magnitude);
            
            refProj.SetVelocity(1.2f * magnitude * refDir);

            refProj.GetComponent<ProjectileClamp>().enabled = false;

            other.enabled = false;

        }
    }
}

