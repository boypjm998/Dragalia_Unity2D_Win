using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 5 storm
    /// </summary>
    public class Projectile_DB001_1 : MonoBehaviour, IEnemySealedContainer
    {
        private GameObject[] windGos = new GameObject[5];
        protected GameObject enemySource;
        protected bool attackHasReleased = false;

        public void SetEnemySource(GameObject source)
        {
            enemySource = source;
            source.GetComponent<EnemyController>().OnAttackInterrupt += DestroySelf;
        }

        public void DestroySelf()
        {
            if (!attackHasReleased)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            enemySource.GetComponent<EnemyController>().OnAttackInterrupt -= DestroySelf;
        }

        private void Awake()
        {
            for (int i = 0; i < 5; i++)
            {
                windGos[i] = transform.GetChild(i).gameObject;
                windGos[i].GetComponent<AttackFromEnemy>().enemySource = this.enemySource;
            }
            //Invoke("ReleaseAttack",2f);
        }

        public void ReleaseAttack()
        {
            var stunEffect = new TimerBuff((int)BasicCalculation.BattleCondition.Stun,
                1, 10, 1);
            foreach (var go in windGos)
            {
                go.SetActive(true);
                go.GetComponent<AttackFromEnemy>().enemySource = enemySource;
                
                
                go.GetComponent<AttackFromEnemy>().AddWithConditionAll(stunEffect,100);
            }
            //transform.Find("Hint").gameObject.SetActive(false);
            attackHasReleased = true;

        }
    }
}

