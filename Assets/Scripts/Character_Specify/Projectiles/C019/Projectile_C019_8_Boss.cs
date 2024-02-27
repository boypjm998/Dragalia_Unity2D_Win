using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using GameMechanics;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Genesis Resplendence
    /// </summary>
    public class Projectile_C019_8_Boss : MonoBehaviour, IEnemySealedContainer
    {
        [SerializeField] GameObject _rainbowAttackPrefab;
        public GameObject lightPillar;
        private Tween _tweener;
        private float startAngle;
        [SerializeField] private GameObject enemySource;
        [SerializeField] private GameObject target;
        private int count = 0;
        

        private void Awake()
        {
            
        }

        public void SetEnemySource(GameObject src)
        {
            enemySource = src;
            target = enemySource.GetComponent<DragaliaEnemyBehavior>().targetPlayer;
        }
        
        public void StopRainbowTween()
        {
            _tweener?.Kill();
        }

        public void StartRainbowTween()
        {
            var diff = ((Vector2)(target.transform.position - enemySource.transform.position)).
                normalized;
            
            startAngle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

            _tweener = DOVirtual.DelayedCall(1.8f, () =>
            {
                var rainbowProj =
                    this.InstantiateRangedObject(_rainbowAttackPrefab,
                        transform.position, gameObject, 1,1,
                        enemySource.transform);
                
                rainbowProj.transform.eulerAngles = new Vector3(0, 0, startAngle - 90f);
                
                startAngle = (startAngle + Random.Range(80f, 100f)) % 360f;
                
                count++;
                
                if(count < 20)
                    _tweener.Restart();
                
            }, false);
        }

        private void OnDestroy()
        {
            _tweener?.Kill();
        }
    }
}

