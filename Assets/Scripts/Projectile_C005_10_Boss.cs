using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C005_10_Boss : MonoBehaviour,IEnemySealedContainer
    {
        GameObject enemySource;

        [SerializeField] private GameObject warningPrefab;
        [SerializeField] private GameObject attackPrefab;
        [SerializeField] private GameObject flameTrailPrefab;
        [SerializeField] private float interval = 4f;
        private List<float> heights = new List<float>();
        private float warningTime;
        private float afterFlameChaseTime = 0.5f;


        public void SetEnemySource(GameObject source)
        {
            enemySource = source;
        }

        public void InitOrderList(List<float> heights)
        {
            this.heights = heights;
        }

        private IEnumerator Start()
        {
            var warning1 = Instantiate(warningPrefab, new Vector3(transform.position.x, heights[0]+3),
                Quaternion.identity,transform);
            
            warningTime = warning1.GetComponent<EnemyAttackHintBarRect2D>().warningTime;
            
            yield return new WaitForSeconds(warningTime);
            
            GenerateAttack(0);

            yield return new WaitForSeconds(afterFlameChaseTime);
            
            GenerateFlameTrail(0);
            
            yield return new WaitForSeconds(interval - warningTime - afterFlameChaseTime);
            
            Instantiate(warningPrefab, new Vector3(transform.position.x, heights[1]+3),
                Quaternion.identity,transform);
            
            yield return new WaitForSeconds(warningTime);
            
            GenerateAttack(1);
            
            yield return new WaitForSeconds(afterFlameChaseTime);
            
            GenerateFlameTrail(1);
            
            yield return new WaitForSeconds(interval - warningTime - afterFlameChaseTime);
            
            Instantiate(warningPrefab, new Vector3(transform.position.x, heights[2]+3),
                Quaternion.identity,transform);
            
            yield return new WaitForSeconds(warningTime);
            
            GenerateAttack(2);
            
            yield return new WaitForSeconds(afterFlameChaseTime);
            
            GenerateFlameTrail(2);
            
            Destroy(gameObject,10f);

        }

        private void GenerateAttack(int index)
        {
            var attackProjectile1 = Instantiate(attackPrefab, new Vector3(transform.position.x - 2*transform.localScale.x, heights[index]+3),
                Quaternion.identity,transform);
            
            attackProjectile1.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            attackProjectile1.GetComponent<AttackFromEnemy>().firedir = (int)transform.localScale.x;
        }
        
        private void GenerateFlameTrail(int index)
        {
            var flameTrail1 = Instantiate(flameTrailPrefab, new Vector3(0, heights[index]+1.5f),
                Quaternion.identity,transform);
            
            flameTrail1.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            flameTrail1.GetComponent<AttackFromEnemy>().firedir = 0;
        }
        
        
        
    }

}
