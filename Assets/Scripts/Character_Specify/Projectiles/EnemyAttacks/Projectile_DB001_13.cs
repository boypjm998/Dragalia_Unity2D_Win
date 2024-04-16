using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  CharacterSpecificProjectiles
{
    
    
    public class Projectile_DB001_13 : MonoBehaviour,IEnemySealedContainer
    {

        [SerializeField] private List<GameObject> attacks = new();
        protected GameObject enemySource;
        protected GameObject target;
        public void SetEnemySource(GameObject source)
        {
            enemySource = source;
            target = enemySource.GetComponent<DragaliaEnemyBehavior>().targetPlayer;
        }


        private void Start()
        {
            Invoke("ActiveAttacks",0.8f);
        }

        void ActiveAttacks()
        {
            foreach (var attack in attacks)
            {
                attack.gameObject.SetActive(true);
                attack.GetComponent<AttackFromEnemy>().enemySource = enemySource;
                attack.GetComponent<HomingAttack>().target = target.transform;
            }

            Destroy(gameObject,10f);
        }
    }
}

