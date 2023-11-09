using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C003_2 : ProjectileControllerTest
    {
        private float dropDistanceY;
        private float moveDistanceX;
        public GameObject blastPrefab;
        private Tweener _tweener;
        public GameObject playerPos;
        public Vector2 burstPosition;
        private float randomX;
        void Awake()
        {
            firedir = GetComponentInParent<Projectile_C003_3>().firedir;
            var targetPos = contactTarget.transform.position;
            var topBorder = BattleStageManager.Instance.mapBorderT;
            if (firedir == -1)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            
            
            var groundSensor = contactTarget.GetComponentInChildren<IGroundSensable>();
            Collider2D col = null;
            if ((col = groundSensor.GetCurrentAttachedGroundCol()) != null)
            {
                dropDistanceY = topBorder - col.bounds.max.y;
                print("Set!");
                burstPosition.y = col.bounds.max.y + 1.5f;
                
            }
            else
            {
                burstPosition.y = contactTarget.transform.position.y;
                dropDistanceY = topBorder - targetPos.y;
            }
            burstPosition.x = contactTarget.transform.position.x;
            randomX = Random.Range(-1f, 1f);
            moveDistanceX = firedir * Mathf.Abs(dropDistanceY / 1.73f) + randomX;
            DoProjectileMove();
        }

        protected override void DoProjectileMove()
        {
            var targetPos = burstPosition+ new Vector2(randomX,0);
            transform.position = new Vector3
            (contactTarget.transform.position.x - moveDistanceX + randomX, contactTarget.transform.position.y + dropDistanceY,
                transform.position.z);
            _tweener = transform.DOMove(targetPos, dropDistanceY / verticalVelocity).SetEase(Ease.InSine).OnComplete(BlastEffect);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            var statusEnemy = other.GetComponentInParent<EnemyController>();
            if (statusEnemy != null)
            {
                var groundSensor = statusEnemy.GetComponentInChildren<IGroundSensable>();
                if (groundSensor.GetCurrentAttachedGroundCol() == null)
                {
                    if (statusEnemy.gameObject == contactTarget)
                    {
                        BlastEffect();
                    }
                }
            }
        }

        public override void SetContactTarget(GameObject obj)
        {
            base.SetContactTarget(obj);
            //contactTarget = obj;
        }

        private void BlastEffect()
        {
            var prefab = Instantiate(blastPrefab, transform.position, 
                Quaternion.identity, transform.parent);

            var atk = prefab.GetComponent<ForcedAttackFromPlayer>();
            atk.playerpos = playerPos.transform;
            atk.target = contactTarget;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if(_tweener.IsPlaying())
                _tweener.Kill();
        }
    }
}