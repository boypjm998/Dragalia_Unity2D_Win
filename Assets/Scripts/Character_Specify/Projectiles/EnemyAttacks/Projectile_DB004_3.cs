using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Yachiyo's shadow
    /// </summary>
    [RequireComponent(typeof(AttackContainerEnemy))]
    public class Projectile_DB004_3 : MonoSingleton<Projectile_DB004_3>,IEnemySealedContainer
    {
        private GameObject _enemySource;
        
        [SerializeField] private float waitTime = 2;

        [SerializeField] private GameObject slashFX;
        //[SerializeField] private GameObject hintFX;
        [SerializeField] private GameObject shadowFX;
        public void SetEnemySource(GameObject source)
        {
            _enemySource = source;
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(waitTime);
            
            slashFX.GetComponent<AttackFromEnemy>().enemySource = _enemySource;

            var dir = transform.localScale.x;

            shadowFX.transform.DOMoveX(transform.position.x + dir * 12, 0.15f).SetEase(Ease.OutSine);
            //hintFX.SetActive(false);
            yield return null;
            slashFX.SetActive(true);

            yield return new WaitForSeconds(2f);
            
            Destroy(gameObject);

        }
    }
}

