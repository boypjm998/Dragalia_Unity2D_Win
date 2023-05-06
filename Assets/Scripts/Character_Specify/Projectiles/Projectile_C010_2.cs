using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C010_2 : ProjectileControllerTest
    {
        // Start is called before the first frame update

        private AttackBase _attackBase;
        ParticleSystem _particleSystem;
        
        protected override void Start()
        {
            _particleSystem = GetComponentInChildren<ParticleSystem>();
            _attackBase = GetComponent<AttackBase>();
            firedir = _attackBase.firedir;
            InvokeRepeating("CheckDirection", 1, 1);
            Invoke("StopParticles", 4.2f);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            DoProjectileMove();
        }

        private void OnDestroy()
        {
            CancelInvoke();
        }

        protected override void DoProjectileMove()
        {
            var horizontal = horizontalVelocity * Time.deltaTime * firedir;
            transform.position += new Vector3(1, 0, 0) * horizontal;
        }

        private void CheckDirection()
        {
            //检测以自身为中心高为7长为4.5的矩形内是否有LayerMask为Enemies的物体
            Collider2D[] colliders = Physics2D.OverlapBoxAll
            (transform.position-new Vector3(0,1,0), 
                new Vector2(10f, 7f), 0,
                LayerMask.GetMask("Enemies"));
            //计算出colliders中gameObject的x坐标和自身x坐标相减的值，存入数组
            
            if(colliders.Length==0)
                return;
            
            float[] x = new float[colliders.Length];
            for (int i = 0; i < colliders.Length; i++)
            {
                x[i] = Mathf.Abs(colliders[i].gameObject.transform.position.x - transform.position.x);
            }

            float minDistance = 1000;
            
            for (int i = 0; i < x.Length; i++)
            {
                if (!(x[i] < minDistance)) continue;
                minDistance = x[i];
                contactTarget = colliders[i].gameObject;
            }
            
            if(contactTarget.transform.position.x-transform.position.x>0)
                firedir = 1;
            else
                firedir = -1;
            
            _attackBase.firedir = firedir;

        }

        private void StopParticles()
        {
            _particleSystem.Stop();
            CancelInvoke();
        }


    }
}

