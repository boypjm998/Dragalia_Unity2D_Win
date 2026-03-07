using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// FIRE CRYSTAL
    /// </summary>
    public class Projectile_DB015_1 : MonoBehaviour, IEnemySealedContainer
    {
        [SerializeField] private GameObject burningEffectPrefab;
        [SerializeField] private GameObject blastAttackPrefab;
        [SerializeField] private GameObject hintPrefab;
        [SerializeField] private float burningDelay = 10;
        [SerializeField] private float blastTime = 2;
        private Tween _tween;
        private bool _isBurning = false;
        private GameObject _enemySource;
        
        public void SetEnemySource(GameObject enemy)
        {
            _enemySource = enemy;
        }
        
        private void Start()
        {
            BattleStageManager.Instance.specialEventTriggered += AutoDestruction;
            
            _tween = DOVirtual.DelayedCall(burningDelay - 2, () =>
            {
                _isBurning = true;
                var blastAttack = Instantiate(burningEffectPrefab,
                    transform.position, Quaternion.identity,transform);
                
                Invoke("BlastAttack",blastTime);
                
                Instantiate(hintPrefab, transform.position,
                    Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform
                );

                
            },false);
        }

        private void OnDestroy()
        {
            BattleStageManager.Instance.specialEventTriggered -= AutoDestruction;
        }

        private void AutoDestruction(int eventID)
        {
            if (eventID == 1)
            {
                if (_isBurning == false)
                {
                    _tween?.Kill();
                    _isBurning = true;
                    var blastAttack = Instantiate(burningEffectPrefab,
                        transform.position, Quaternion.identity,transform);
                
                    Invoke("BlastAttack",blastTime);
                
                    Instantiate(hintPrefab, transform.position,
                        Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform
                    );
                }
            }
        }
        
        
        private void BlastAttack()
        {
            var blastAttack = this.InstantiateRangedObject(blastAttackPrefab,
                transform.position, BattleStageManager.Instance.GetNewRangedContainer(),1,
                1,_enemySource.transform);
            Destroy(transform.parent.gameObject,0.5f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            print(other.name);
            if (!other.CompareTag("SpecialTrigger"))
            { 
                print("Not Triggered");
               return; 
            }
            
            
            
            
            if (_isBurning == false)
            {
                _tween?.Kill();
                _isBurning = true;
                var blastAttack = Instantiate(burningEffectPrefab,
                    transform.position, Quaternion.identity,transform);
                
                Invoke("BlastAttack",blastTime);
                
                Instantiate(hintPrefab, transform.position,
                    Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform
                );
            }
        }
    }

}
