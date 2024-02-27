using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Dawn Circlet
    /// </summary>
    public class Projectile_C019_6_Boss : MonoBehaviour, IEnemySealedContainer
    {
        [SerializeField] private GameObject[] stars;
        [SerializeField] private float interval;

        private List<AttackFromEnemy> attacks = new();


        public void SetEnemySource(GameObject src)
        {
            foreach (var atk in attacks)
            {
                atk.enemySource = src;
            }
        }

        private void Awake()
        {
            stars = stars.Shuffle().ToArray();
            for (int i = 0; i < stars.Length; i++)
            {
                attacks.Add(stars[i].transform.GetChild(1).GetComponent<AttackFromEnemy>());
            }
                
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);

            foreach (var star in stars)
            {
                star.SetActive(true);
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(1);
            
            stars[1].transform.GetChild(1).gameObject.SetActive(true);
            stars[2].transform.GetChild(1).gameObject.SetActive(true);
            
            yield return new WaitForSeconds(2f);
            
            stars[1].transform.GetChild(1).gameObject.SetActive(false);
            stars[2].transform.GetChild(1).gameObject.SetActive(false);

            yield return new WaitForSeconds(interval);
            
            stars[0].transform.GetChild(1).gameObject.SetActive(true);
            stars[2].transform.GetChild(1).gameObject.SetActive(true);
            ResetAttack(0,2);
            
            yield return new WaitForSeconds(2f);
            
            stars[0].transform.GetChild(1).gameObject.SetActive(false);
            stars[2].transform.GetChild(1).gameObject.SetActive(false);
            
            yield return new WaitForSeconds(interval);
            
            stars[0].transform.GetChild(1).gameObject.SetActive(true);
            stars[1].transform.GetChild(1).gameObject.SetActive(true);
            ResetAttack(0,1);
            
            yield return new WaitForSeconds(2f);
            
            Destroy(gameObject);
        }

        private void ResetAttack(params int[] indexes)
        {
            foreach (var i in indexes)
            {
                if(i < attacks.Count)
                    attacks[i].GetComponent<EnemyAttackTriggerController>().Restart();
            }
        }
        

    }

}
