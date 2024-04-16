using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// wind launcher
    /// </summary>
    public class Projectile_DB001_8 : MonoBehaviour,IEnemySealedContainer
    {
        [SerializeField] private GameObject launcher1;
        [SerializeField] private GameObject launcher2;

        [SerializeField] private GameObject hint1;
        [SerializeField] private GameObject hint2;

        [SerializeField] private float shootInterval;

        [SerializeField] private GameObject projectile;

        private GameObject enemySource;

        public void SetEnemySource(GameObject source)
        {
            enemySource = source;
        }
        
        private void GenerateProjectile(int angle,Vector3 position)
        {
            var atk = Instantiate(projectile,position,Quaternion.Euler(0,0,angle),
                transform);
            atk.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            
        }

        private IEnumerator Start()
        {
            var hintbar = hint1.GetComponent<EnemyAttackHintBar>();

            var time = hintbar.warningTime;

            yield return new WaitForSeconds(time);

            launcher1.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            launcher2.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            launcher1.SetActive(true);
            launcher2.SetActive(true);
            
            yield return new WaitForSeconds(shootInterval);
            
            GenerateProjectile(0,launcher1.transform.position);
            GenerateProjectile(0,launcher2.transform.position);
            
            yield return new WaitForSeconds(shootInterval);
            
            GenerateProjectile(-45,launcher1.transform.position);
            GenerateProjectile(-45,launcher2.transform.position);
            
            yield return new WaitForSeconds(shootInterval);
            
            GenerateProjectile(-90,launcher1.transform.position);
            GenerateProjectile(-90,launcher2.transform.position);
            
            yield return new WaitForSeconds(shootInterval);
            
            GenerateProjectile(-135,launcher1.transform.position);
            GenerateProjectile(-135,launcher2.transform.position);
            
            yield return new WaitForSeconds(shootInterval);
            
            GenerateProjectile(-180,launcher1.transform.position);
            GenerateProjectile(-180,launcher2.transform.position);
            
            yield return new WaitForSeconds(shootInterval);
            
            GenerateProjectile(135,launcher1.transform.position);
            GenerateProjectile(135,launcher2.transform.position);
            
            yield return new WaitForSeconds(shootInterval);
            
            GenerateProjectile(90,launcher1.transform.position);
            GenerateProjectile(90,launcher2.transform.position);
            
            yield return new WaitForSeconds(shootInterval);
            
            GenerateProjectile(45,launcher1.transform.position);
            GenerateProjectile(45,launcher2.transform.position);
            
            yield return new WaitForSeconds(shootInterval);
            
            Destroy(gameObject,0.1f);

        }
    }
}

