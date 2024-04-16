using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_DB001_12 : MonoBehaviour,IEnemySealedContainer
    {
        [SerializeField] private GameObject idleFX;
        [SerializeField] private GameObject attackFX;

        [SerializeField] private GameObject attackProjectile;
        [SerializeField] private GameObject hint;

        [SerializeField] private GameObject enemySource;

        public void SetEnemySource(GameObject source)
        {
            enemySource = source;
        }

        private IEnumerator Start()
        {
            var hintBar = hint.GetComponent<EnemyAttackHintBar>();
            yield return new WaitForSeconds(0.1f);
            
            yield return new WaitUntil(()=>hintBar.warningTimeLeft <= 0);
        
            idleFX.SetActive(false);
            attackFX.SetActive(true);
            
            var attack = Instantiate(attackProjectile, transform.position, Quaternion.identity, transform);
            attack.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            // if (transform.localScale.x < 0)
            // {
            //     attack.transform.localScale = new Vector3(-attack.transform.localScale.x, attack.transform.localScale.y, attack.transform.localScale.z);
            // }

            yield return new WaitForSeconds(9f);
            Destroy(gameObject);

        }
    }
}

