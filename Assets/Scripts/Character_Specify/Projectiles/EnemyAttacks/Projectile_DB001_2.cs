using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_DB001_2 : MonoBehaviour, IEnemySealedContainer
    {
        protected GameObject enemySource;
        public float attackReleaseTime;
        [SerializeField] private GameObject leftAttack;
        [SerializeField] private GameObject rightAttack;
        public virtual void SetEnemySource(GameObject source)
        {
            enemySource = source;
        }

        private void Awake()
        {
            var others = FindObjectsOfType<Projectile_DB001_2>();
            if (others.Length > 0)
            {
                for (int i = 0; i < others.Length; i++)
                {
                    if(others[i] != this)
                        Destroy(others[i].gameObject);
                }
            }
            Invoke("ReleaseAttack", attackReleaseTime);
        }

        protected virtual void ReleaseAttack()
        {
            leftAttack.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            rightAttack.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            leftAttack.SetActive(true);
            rightAttack.SetActive(true);
        }



    }
}

