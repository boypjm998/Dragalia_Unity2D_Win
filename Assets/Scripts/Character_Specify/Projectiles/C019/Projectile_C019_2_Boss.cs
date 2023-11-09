using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C019_2_Boss : Projectile_C019_1_Boss
    {
        
       
        private ObjectPool<GameObject> pool;
        
        private bool isRotating = false;

        private int projectileCountInCurrentType = 0;
        
        private float[] rotateAngles = { 144, 108, 72, 36, 0, -36, -72, -108, -144, 180};

        private float[] movingAngles1 = { 143, -143 };
        
        private float[] movingAngles2 = { 37, -37 };
        
        private float[] modifyingModifiers = { 1, 3, 5, 6 };

        private int rotateDirection = 1;





        public void SetEnemySource(GameObject src)
        {
            enemySource = src;
        }

        private GameObject CreateObject()
        {
            return Instantiate(muzzlePrefab,transform);
        }



        private void Awake()
        {
            //_pool = GetComponent<MyObjectPool>();
            pool = new ObjectPool<GameObject>(CreateObject,
                null,null,null,true,
                20,50);
            _tracer = GetComponent<VelocityTracer>();
            
        }

        private IEnumerator Start()
        {
            _tracer.actor = targetPlayer.GetComponent<ActorBase>();
            //_tracer.angleDeg = _tracer.actor.facedir == 1 ? 0 : 180;
            
            
            while (projectileLaunched < projectileCount)
            {
                projectileLaunched++;
                ShootProjectile();
                if(projectileLaunched % 4 == 0)
                    yield return new WaitForSeconds(projectionInterval*1.5f);
                else yield return new WaitForSeconds(projectionInterval*0.5f);
            }
            
            Destroy(gameObject, destoryAfterTime);
            
        }

        protected override void ShootProjectile()
        {
            if(_tracer.velocity.magnitude < 0.1f)
            {
                if (!isRotating)
                {
                    projectileCountInCurrentType = 0;
                    rotateDirection = _tracer.actor.facedir;
                }
                isRotating = true;
            }
            else
            {
                if (isRotating)
                {
                    projectileCountInCurrentType = 0;
                }
                isRotating = false;
            }

            Vector2 startPos;


            if (_tracer.velocity.magnitude > 0.05f)
            {
                

                var predictedPosition = 
                    (Vector2)_tracer.actor.transform.position +
                    _tracer.velocity * 50.5f * waitTimeBeforeShoot;
                var modifier = modifyingModifiers[projectileCountInCurrentType % 4];
                startPos = CalculatePosition(_tracer.actor.transform.position, 10, 
                    _tracer.velocity.x > 0 ?
                        movingAngles2[projectileCountInCurrentType % 2] + modifier:
                        movingAngles1[projectileCountInCurrentType % 2] - modifier );
                
                projectileCountInCurrentType++;

                var direction = startPos.AngleDegree(predictedPosition) + Random.Range(-1f,1f);
                var proj = Instantiate(projectilePrefab, startPos,
                    Quaternion.Euler(0, 0, direction),
                    transform);
                DOVirtual.DelayedCall(waitTimeBeforeShoot, () =>
                {
                    var muzzle2 = pool.Get();
                    muzzle2.transform.position = startPos;
                },false);

                var muzzle1 = pool.Get();
                muzzle1.transform.position = startPos;
                var hint = Instantiate(hintPrefab, startPos,
                    Quaternion.Euler(0, 0, direction),
                    transform);
                var atk = proj.GetComponent<AttackFromEnemy>();

                atk.enemySource = this.enemySource;
                atk.attackInfo[0].dmgModifier[0] *= 2f;
                SetComponents(proj, direction);
            }
            else
            {
                startPos = CalculatePosition(_tracer.actor.transform.position, 10, 
                    rotateDirection * rotateAngles[projectileCountInCurrentType % 10] );
                
                projectileCountInCurrentType++;

                var direction = startPos.AngleDegree(_tracer.actor.transform.position);
                
                var proj = Instantiate(projectilePrefab, startPos,
                    Quaternion.Euler(0, 0, direction),
                    transform);

                DOVirtual.DelayedCall(waitTimeBeforeShoot, () =>
                {
                    var muzzle2 = pool.Get();
                    muzzle2.transform.position = startPos;
                },false);

                var muzzle1 = pool.Get();
                muzzle1.transform.position = startPos;
                
                var hint = Instantiate(hintPrefab, startPos,
                    Quaternion.Euler(0, 0, direction),
                    transform);

                var atk = proj.GetComponent<AttackFromEnemy>();

                atk.enemySource = this.enemySource;

                SetComponents(proj, direction);

            }




        }

        protected void SetComponents(GameObject proj, float direction)
        {
            var triggerController = proj.GetComponent<EnemyAttackTriggerController>();
            triggerController.SetAwakeTime(0,waitTimeBeforeShoot);
            triggerController.DestroyTime = moveTime + waitTimeBeforeShoot;
            

            var dtwController = proj.AddComponent<DOTweenSimpleController>();
            dtwController.duration = moveTime;
            dtwController.IsAbsolutePosition = false;
            dtwController.moveDirection = CalculateDirectionVector(direction, distance);
            dtwController.SetWaitTime(waitTimeBeforeShoot);
        }
    }
    
    
}

