using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterSpecificProjectiles
{
    ///drop ice
    public class Projectile_C007_5_Boss : MonoBehaviour, IEnemySealedContainer
    {
        [SerializeField] private GameObject blastFXInstance;
        [SerializeField] private GameObject icePillarPrefab;
        
        private GameObject _enemySource;
        private Collider2D _collider;
        private bool _flag = false;


        public void SetEnemySource(GameObject source)
        {
            _enemySource = source;
            blastFXInstance.GetComponent<AttackFromEnemy>().enemySource = source;
        }

        private IEnumerator Start()
        {
            _collider = GetComponent<Collider2D>();
            
            yield return new WaitForSeconds(1);
            
            blastFXInstance.SetActive(true);
            CheckCollider();

            yield return new WaitForSeconds(3);
            Destroy(gameObject);
        }

        private void CheckCollider()
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position,
                (_collider as CircleCollider2D).radius, LayerMask.GetMask("AttackEnemy"));

            //bool found = false;
            foreach (var col in colliders)
            {
                print("Collide with"+col);
                var controller = col.GetComponent<Projectile_C007_3_Boss>();
                if(controller == null)
                    continue;
                else
                {
                    controller.LevelUp();
                    _flag = true;
                    break;
                }
            }

            if (_flag == false)
            {
                var ice = Instantiate(icePillarPrefab, transform.position, Quaternion.identity,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform);
                _flag = true;
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            // var controller = other.GetComponent<Projectile_C007_3_Boss>();
            //
            // if (!_flag && controller != null)
            // {
            //     controller.LevelUp();
            //     _flag = true;
            // }else if (!_flag)
            // {
            //     
            // }
        }
    }
}

