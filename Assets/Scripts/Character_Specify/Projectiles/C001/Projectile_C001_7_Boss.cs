using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 星辰爆发
    /// </summary>
    public class Projectile_C001_7_Boss : MonoBehaviour, IEnemySealedContainer
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private GameObject hintPrefab;
        [SerializeField] private float interval;
        [SerializeField] private float hintTime = 3;

        private GameObject _enemySource;

        public void SetEnemySource(GameObject src)
        {
            _enemySource = src;
        }

        private IEnumerator Start()
        {
            var randInt = Random.Range(0, 2);
            if (randInt == 0)
            {
                GenerateHintbars(15,-105,135);
            }
            else
            {
                GenerateHintbars(-15,105,-135);
            }
            
            yield return new WaitForSeconds(hintTime);

            if (randInt == 0)
            {
                GenerateProjectiles(15,-105,135);
            }
            else
            {
                GenerateProjectiles(-15,105,-135);
            }
            
            yield return new WaitForSeconds(interval);
            
            // if (randInt == 1)
            // {
            //     GenerateHintbars(45,165,-75);
            // }
            // else
            // {
            //     
            //     GenerateHintbars(75,-45,195);
            // }
            
            yield return new WaitForSeconds(hintTime);

            if (randInt == 1)
            {
                GenerateProjectiles(45,165,-75);
                
            }
            else
            {
                GenerateProjectiles(75,-45,195);
            }

            yield return new WaitForSeconds(interval);
            Destroy(gameObject);
        }

        private void GenerateProjectiles(params float[] angles)
        {
            var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
                transform.position,Quaternion.identity, 
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            for (int i = 0; i < angles.Length; i++)
            {
                // this.InstantiateDirectionalRangedObject(projectilePrefab, transform.position,
                //     container, 1, angles[i], _enemySource.transform);
                this.InstantiateDirectionalRangedObject(projectilePrefab, transform.position,
                    container, 1, angles[i]-5f, _enemySource.transform);
                this.InstantiateDirectionalRangedObject(projectilePrefab, transform.position,
                    container, 1, angles[i]-10f, _enemySource.transform);
                this.InstantiateDirectionalRangedObject(projectilePrefab, transform.position,
                    container, 1, angles[i]-15f, _enemySource.transform);
                this.InstantiateDirectionalRangedObject(projectilePrefab, transform.position,
                    container, 1, angles[i]-20f, _enemySource.transform);
                this.InstantiateDirectionalRangedObject(projectilePrefab, transform.position,
                    container, 1, angles[i]-25f, _enemySource.transform);
                this.InstantiateDirectionalRangedObject(projectilePrefab, transform.position,
                    container, 1, angles[i]-30f, _enemySource.transform);
                this.InstantiateDirectionalRangedObject(projectilePrefab, transform.position,
                    container, 1, angles[i]-35f, _enemySource.transform);
                this.InstantiateDirectionalRangedObject(projectilePrefab, transform.position,
                    container, 1, angles[i]-40f, _enemySource.transform);
                this.InstantiateDirectionalRangedObject(projectilePrefab, transform.position,
                    container, 1, angles[i]-45f, _enemySource.transform);
                this.InstantiateDirectionalRangedObject(projectilePrefab, transform.position,
                    container, 1, angles[i]-50f, _enemySource.transform);
                this.InstantiateDirectionalRangedObject(projectilePrefab, transform.position,
                    container, 1, angles[i]-55f, _enemySource.transform);
                // this.InstantiateDirectionalRangedObject(projectilePrefab, transform.position,
                //     container, 1, angles[i]-52.5f, _enemySource.transform);
            }
            
            
        }

        private void GenerateHintbars(params float[] angles)
        {
            for (int i = 0; i < angles.Length; i++)
            {
                Instantiate(hintPrefab, transform.position, 
                    Quaternion.Euler(0, 0, angles[i]), transform);

            }
        }
        
        
        
    }

}
