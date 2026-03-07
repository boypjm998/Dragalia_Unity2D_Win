using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 咒影召唤
    /// </summary>
    public class Projectile_DB012_1 : MonoBehaviour
    {
        private StatusManager _statusManager;
        private StatusManager _bossStatusManager;
        private GameObject target;
        private Tween _tween;
        private AttackFromEnemy _attack;
        

        [SerializeField] private GameObject attackPrefab;

        private void Awake()
        {
            _statusManager = GetComponent<StatusManager>();
            target = DragaliaEnemyBehavior.GetPlayerList()[0];

            _statusManager.OnReviveOrDeath += ChangeSource;
        }

        private IEnumerator Start()
        {

            yield return new WaitForSeconds(1.5f);
            
            while (_statusManager.currentHp > 0)
            {
                Attack();
                yield return new WaitForSeconds(5);
            }
            
            Destroy(gameObject, 0.6f);
        }


        public void SetSource(StatusManager bossStatus)
        {
            _bossStatusManager = bossStatus;
            _statusManager.baseAtk = _bossStatusManager.baseAtk;
        }

        protected void Attack()
        {
            
            
            //计算出target和自身的角度
            var angle = Vector2.Angle(target.transform.position-transform.position,Vector2.right);

            if (transform.position.y > target.transform.position.y)
            {
                angle *= -1;
            }

            var hintBar = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(GetComponent<EnemyController>(), transform.position,
                BattleStageManager.Instance.RangedAttackFXLayer.transform, new Vector2(30, 2), Vector2.zero,
                true, 1,
                2f, angle, 1f, true, true);


            _tween = DOVirtual.DelayedCall(2f, () =>
            {
                this.InstantiateDirectionalRangedObject(attackPrefab,
                    transform.position,
                    ActorExtensions.InstantiateContainer(BattleStageManager.Instance.RangedAttackFXLayer.transform)
                        .gameObject,
                    1, angle, transform);
            });


        }

        private void ChangeSource()
        {
            if (_attack != null)
            {
                _attack.enemySource = _bossStatusManager.gameObject;
            }
        }
        
        
    }

}
