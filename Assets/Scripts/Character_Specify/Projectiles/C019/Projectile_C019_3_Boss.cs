using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.Pool;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C019_3_Boss : Projectile_C019_1_Boss
    {
        private ObjectPool<GameObject> pool;

        private float[] startOffsetsX = { -10, 10, -14, -5,  5, 14};
        private float[] startOffsetsY = {  -6,  6,   6,  6, -6, -6};
        private float[] intervalX = { 0, 0, 3f, 3f, -3f, -3f };
        private float[] intervalY = { 1.8f, -1.8f, 0 , 0 , 0, 0};

        private float offsetPerProjectile = 0.2f;
        private float offsetRadius = 4f;
        private readonly float[] offsetAngles = { 0, 0, 135, 45, -135, -45 };
        
        
        
        
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
            projectileCount = 6;
            
        }

        private IEnumerator Start()
        {
            
            _tracer.actor = targetPlayer.GetComponent<ActorBase>();
            _tracer.angleDeg = _tracer.actor.facedir == 1 ? 0 : 180;

            while (projectileLaunched < projectileCount)
            {
                for(int i = 0; i < 8; i ++)
                    ShootProjectiles(projectileLaunched,i);
                
                projectileLaunched++;
                yield return new WaitForSeconds(projectionInterval * 3);
            }
            
            Destroy(gameObject, destoryAfterTime);
        }

        protected void ShootProjectile(int index)
        {
            Vector2 startPos = 
                _tracer.actor.transform.position + 
                               new Vector3(startOffsetsX[projectileLaunched] + startOffsetsX[index],
                                   startOffsetsY[projectileLaunched] + startOffsetsY[index]);

            float velocityOffsetX, trueOffsetX = 0;
            float velocityOffsetY, trueOffsetY = 0;

            //var velocityOffsetLength = Mathf.Clamp(_tracer.velocity.magnitude * 5, 0, 1f);

            //velocityOffsetX = Mathf.Cos(offsetAngles[projectileLaunched] * Mathf.Deg2Rad) * velocityOffsetLength;
            //velocityOffsetY = Mathf.Sin(offsetAngles[projectileLaunched] * Mathf.Deg2Rad) * velocityOffsetLength;
            
            trueOffsetX = Mathf.Cos(offsetAngles[projectileLaunched] * Mathf.Deg2Rad) * offsetRadius;
            trueOffsetY = Mathf.Sin(offsetAngles[projectileLaunched] * Mathf.Deg2Rad) * offsetRadius;

            trueOffsetX += (intervalX[projectileLaunched] == 0 ? 0 : offsetPerProjectile);
            trueOffsetY += (intervalY[projectileLaunched] == 0 ? 0 : offsetPerProjectile);


            Vector2 predictedPosition =
                (Vector2)_tracer.actor.transform.position +
                new Vector2(trueOffsetX, trueOffsetY);

            var angle = startPos.AngleDegree(predictedPosition);

            InstantiateProjectile(startPos, angle);

        }

        protected void ShootProjectiles(int i, int j)
        {
            Vector2 startPos;
            Vector2 endPos;
            
            startPos = _tracer.actor.transform.position +
                       new Vector3(startOffsetsX[i], startOffsetsY[i]) +
                       new Vector3(j*intervalX[i], j*intervalY[i]);
            
            var velocityOffsetLength = Mathf.Clamp(_tracer.velocity.magnitude * 2, 0, 1f);

            print(velocityOffsetLength);
            var velocityOffsetX = Mathf.Cos(offsetAngles[projectileLaunched] * Mathf.Deg2Rad) * velocityOffsetLength;
            var velocityOffsetY = Mathf.Sin(offsetAngles[projectileLaunched] * Mathf.Deg2Rad) * velocityOffsetLength;

            switch (i)
            {
                case 0:
                {
                    endPos = _tracer.actor.transform.position + new Vector3(4, -1);
                    break;
                }
                case 1:
                {
                    endPos = _tracer.actor.transform.position + new Vector3(-4, -1);
                    break;
                }
                case 2:
                {
                    endPos = _tracer.actor.transform.position + new Vector3(3.7f, -4.7f);
                    break;
                }
                case 3:
                {
                    endPos = _tracer.actor.transform.position + new Vector3(-3.7f, -4.7f);
                    break;
                }
                case 4:
                {
                    endPos = _tracer.actor.transform.position + new Vector3(3.7f, 2.7f);
                    break;
                }
                case 5:
                {
                    endPos = _tracer.actor.transform.position + new Vector3(-3.7f, 2.7f);
                    break;
                }
                default:
                {
                    endPos = _tracer.actor.transform.position + new Vector3(4, 0);
                    break;
                }
            }

            endPos *= 1.1f;

            endPos += new Vector2(velocityOffsetX+intervalY[i]*0.2f, (velocityOffsetX)+intervalX[i]*0.2f) * j * 0.4f;
            
            var angle = startPos.AngleDegree(endPos);
            
            InstantiateProjectile(startPos,angle);
            




        }




        protected void InstantiateProjectile(Vector2 startPos, float direction)
        {
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
            atk.attackInfo[0].dmgModifier[0] *= 1.5f;
            SetComponents(proj, direction);
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

