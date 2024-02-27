using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C019_9_Boss : MonoBehaviour
    {
        private TimerBuff healUpBuff;
        private TimerBuff _spBuff;
        private AttackFromEnemy _attackFromEnemy;
        public Transform target;
        public float speed;

        public void SetSpecialBuff(TimerBuff buff)
        {
            _spBuff = buff;
        }

        private void Awake()
        {
            healUpBuff = new TimerBuff((int)(BasicCalculation.BattleCondition.RecoveryBuff),
                5, 30, 100);
            _attackFromEnemy = GetComponent<AttackFromEnemy>();
        }

        private void Start()
        {
            var controller = gameObject.AddComponent<DOTweenSimpleController>();
            controller.moveDirection = (target.position - transform.position);
            controller.IsAbsolutePosition = false;
            controller.duration = controller.moveDirection.magnitude / speed;
            _attackFromEnemy.AddWithConditionAll(new TimerBuff(healUpBuff),100);
            _attackFromEnemy.AddWithConditionAll(new TimerBuff(_spBuff),100);
        }

        private void Update()
        {
            if (Mathf.Abs(transform.position.x - target.position.x) < 0.5f &&
                Mathf.Abs(transform.position.y - target.position.y) < 0.5f)
            {
                Instantiate(_attackFromEnemy.hitConnectEffect, transform.position,
                    Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
                target.GetComponent<StatusManager>().ObtainTimerBuff(new TimerBuff(healUpBuff));
                Destroy(gameObject);
            }
        }
    }
}

