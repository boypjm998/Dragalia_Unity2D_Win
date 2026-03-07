using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// summoning circle
    /// </summary>
    public class Projectile_H001_2 : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefabA;
        [SerializeField] private GameObject enemyPrefabB;

        [SerializeField] private GameObject summonEffect;

        private (int, int) _statInfoA = (1,1);
        private (int, int) _statInfoB = (1,1);

        private EnemyMoveController_H001 _spawner;
        private StatusManager _statusManager;

        [SerializeField] private float spawnInterval = 20f;
        [SerializeField] private float startDelay = 5f;
        private bool _isStarted = false;

        public void Initialize(EnemyMoveController_H001 spawner, int hp1, int atk1, int hp2, int atk2)
        {
            _spawner = spawner;
            _statInfoA = (hp1, atk1);
            _statInfoB = (hp2, atk2);
            _statusManager = GetComponent<StatusManager>();
            _statusManager.OnReviveOrDeath += () => StopAllCoroutines();
        }
        
        public void StartSpawn()
        {
            if(_isStarted) return;
            
            StartCoroutine("SpawnEnemy");
            _isStarted = true;
        }
        
        private IEnumerator SpawnEnemy()
        {
            yield return new WaitForSeconds(startDelay);
            while (true)
            {
                if (_spawner != null)
                {
                    _spawner.SummonSingleChild(enemyPrefabA,transform.position+new Vector3(0,1),
                        _statInfoA.Item1, _statInfoA.Item2,false);
                    summonEffect.SetActive(true);
                }
                
                yield return new WaitForSeconds(spawnInterval);
                
                if (_spawner != null)
                {
                    _spawner.SummonSingleChild(enemyPrefabA,transform.position+new Vector3(0,1),
                        _statInfoA.Item1, _statInfoA.Item2,false);
                    summonEffect.SetActive(true);
                }
                
                yield return new WaitForSeconds(spawnInterval);
                
                if (_spawner != null)
                {
                    _spawner.SummonSingleChild(enemyPrefabB,transform.position+new Vector3(0,1),
                        _statInfoB.Item1, _statInfoB.Item2,false);
                    summonEffect.SetActive(true);
                }
                
                yield return new WaitForSeconds(spawnInterval);
            }
        }



    }

}
