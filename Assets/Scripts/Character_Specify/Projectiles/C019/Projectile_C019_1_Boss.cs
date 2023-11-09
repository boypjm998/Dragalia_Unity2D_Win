using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterSpecificProjectiles;
using DG.Tweening;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 裁决 Type 1
    /// </summary>
    public class Projectile_C019_1_Boss : MonoBehaviour, IEnemySealedContainer
    {
        protected VelocityTracer _tracer;
        protected GameObject enemySource;
        public GameObject targetPlayer;
        protected int projectileLaunched = 0;
        [SerializeField] protected GameObject projectilePrefab;
        [SerializeField] protected GameObject muzzlePrefab;
        [SerializeField] protected GameObject hintPrefab;
        [SerializeField] protected float distance = 90;
        [SerializeField] protected float moveTime = 3;
        [SerializeField] protected int projectileCount = 1;
        [SerializeField] protected float waitTimeBeforeShoot = 1.5f;
        [SerializeField][Range(0.05f,0.5f)] protected float projectionInterval = 0.1f;
        public float destoryAfterTime = 5f;
        public void SetEnemySource(GameObject source)
        {
            enemySource = source;
        }
        

        private void Awake()
        {
            _tracer = GetComponent<VelocityTracer>();
        }


        private IEnumerator Start()
        {
            _tracer.actor = targetPlayer.GetComponent<ActorBase>();
            _tracer.angleDeg = _tracer.actor.facedir == 1 ? 0 : 180;

            while (projectileLaunched < projectileCount)
            {
                projectileLaunched++;
                ShootProjectile();
                yield return new WaitForSeconds(projectionInterval);
            }
            
            Destroy(gameObject, destoryAfterTime);
        }


        protected virtual void ShootProjectile()
        {
            var direction = _tracer.angleDeg;
            
            print(_tracer.angleDeg);
            

            var startPos = _tracer.actor.transform.position -
                           (Vector3)CalculateDirectionVector(direction, 10);
            
            Debug.Log(startPos);

            var proj = Instantiate(projectilePrefab, startPos,
                Quaternion.Euler(0, 0, direction),
                BattleStageManager.Instance.RangedAttackFXLayer.transform);

            DOVirtual.DelayedCall(waitTimeBeforeShoot, () =>
            {
                var muzzle2 = Instantiate(muzzlePrefab, startPos,
                    Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
            },false); 
            
            var muzzle1 = Instantiate(muzzlePrefab, startPos,
                Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
            
            var hint = Instantiate(hintPrefab, startPos,
                Quaternion.Euler(0, 0, direction),
                BattleStageManager.Instance.RangedAttackFXLayer.transform);

            var atk = proj.GetComponent<AttackFromEnemy>();

            atk.enemySource = this.enemySource;

            var triggerController = proj.GetComponent<EnemyAttackTriggerController>();
            triggerController.SetAwakeTime(0,waitTimeBeforeShoot);
            triggerController.DestroyTime = moveTime + waitTimeBeforeShoot;
            

            var dtwController = proj.AddComponent<DOTweenSimpleController>();
            dtwController.duration = moveTime;
            dtwController.IsAbsolutePosition = false;
            dtwController.moveDirection = CalculateDirectionVector(direction, distance);
            dtwController.SetWaitTime(waitTimeBeforeShoot);


        }

        protected Vector2 CalculateDirectionVector(float angle, float distance)
        {
            
            Debug.Log("angle:"+angle);
            float radian = angle * Mathf.Deg2Rad;

            Vector2 vec = new Vector2(distance * Mathf.Cos(radian),
                distance * Mathf.Sin(radian));
            
            //Debug.Log(vec);

            return vec.normalized * distance;
        }

        public Vector2 CalculatePosition(Vector2 endPoint, float distance, float angle)
        {
            // 将角度转换为弧度
            float radian = angle * Mathf.Deg2Rad;

            // 计算新的位置
            Vector2 position = new Vector2(endPoint.x - distance * Mathf.Cos(radian), 
                endPoint.y - distance * Mathf.Sin(radian));
            
            //Debug.Log(endPoint);

            return position;
        }






    }
}

