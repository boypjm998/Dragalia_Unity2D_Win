using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 碰到玩家时销毁
    /// </summary>
    public class Projectile_H001_1 : MonoBehaviour
    {
        [SerializeField] GameObject _blastPrefab;
        private AttackFromEnemy _attackFromEnemy;
        private bool _isUsed = false;
        
        public AttackFromEnemy attackFromEnemy => _attackFromEnemy;
        public bool useFaceDir = false;

        public GameObject enemySource;

        public List<(TimerBuff, int)> withConditions = new();


        private void Awake()
        {
            _attackFromEnemy = _blastPrefab.GetComponent<AttackFromEnemy>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_isUsed)
            {
                _isUsed = true;
                var proj = this.InstantiateRangedObject(_blastPrefab, transform.position,
                    BattleStageManager.Instance.GetNewRangedContainer(),
                    useFaceDir?(transform.localScale.x>0?1:-1):1,1,enemySource.transform);

                var atk = proj.GetComponent<AttackFromEnemy>();
                int id = 0;
                foreach (var condition in withConditions)
                {
                    atk.AddWithConditionAll(condition.Item1, condition.Item2,id++);
                }
                Destroy(gameObject,0.1f);
            }
        }
    }
}

