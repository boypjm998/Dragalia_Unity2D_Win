using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Holy Crown: Faith
    /// </summary>
    public class Projectile_C003_13_Boss : MonoBehaviour
    {
        [SerializeField] private GameObject meteorL;
        [SerializeField] private GameObject meteorM;
        [SerializeField] private GameObject meteorR;
        
        [SerializeField] private GameObject starL;
        [SerializeField] private GameObject starM;
        [SerializeField] private GameObject starR;
        
        [SerializeField] private GameObject hintL;
        [SerializeField] private GameObject hintM;
        [SerializeField] private GameObject hintR;
        
        [SerializeField] private GameObject impactL;
        [SerializeField] private GameObject impactM;
        [SerializeField] private GameObject impactR;

        private GameObject enemySource;
        private int pattern;
        
        // Start is called before the first frame update
        IEnumerator Start()
        {
            pattern = Random.Range(0, 2);
            
            MeteorDrop(meteorM,starM);

            yield return new WaitForSeconds(1f);

            //pattern = 1 : 中 左 右
            if (pattern == 1)
            {
                MeteorDrop(meteorL,starL);
            }
            else
            {
                MeteorDrop(meteorR,starR);
            }
            
            yield return new WaitForSeconds(1f);
            
            if (pattern == 1)
            {
                MeteorDrop(meteorR,starR);
            }
            else
            {
                MeteorDrop(meteorL,starL);
            }

            yield return new WaitForSeconds(3f);
            
            DisplayHint(hintM);
            yield return new WaitForSeconds(1f);
            
            Burst(impactM,starM);
            
            yield return new WaitForSeconds(0.25f);
            
            if (pattern == 1)
            {
                DisplayHint(hintL);
                yield return new WaitForSeconds(1f);
                Burst(impactL,starL);
            }
            else
            {
                DisplayHint(hintR);
                yield return new WaitForSeconds(1f);
                Burst(impactR,starR);
            }
            
            yield return new WaitForSeconds(0.25f);

            if (pattern == 1)
            {
                DisplayHint(hintR);
                yield return new WaitForSeconds(1f);
                Burst(impactR,starR);
            }
            else
            {
                DisplayHint(hintL);
                yield return new WaitForSeconds(1f);
                Burst(impactL,starL);
            }

            Destroy(gameObject,1f);

        }
        

        void MeteorDrop(GameObject projectile, GameObject impact)
        {
            projectile.transform.DOMoveY(-1, 1.5f).SetEase(Ease.InSine).OnComplete(
                () =>
                {
                    projectile.SetActive(false);
                    impact.SetActive(true);
                });
        }

        void DisplayHint(GameObject hintObj)
        {
            hintObj.SetActive(true);
        }

        void Burst(GameObject burst, GameObject star)
        {
            burst.SetActive(true);
            star.SetActive(false);
        }

        public void SetEnemySource(GameObject src)
        {
            starR.GetComponent<AttackFromEnemy>().enemySource = src;
            starM.GetComponent<AttackFromEnemy>().enemySource = src;
            starL.GetComponent<AttackFromEnemy>().enemySource = src;
            
            impactR.GetComponent<AttackFromEnemy>().enemySource = src;
            impactM.GetComponent<AttackFromEnemy>().enemySource = src;
            impactL.GetComponent<AttackFromEnemy>().enemySource = src;
            
            meteorR.GetComponent<AttackFromEnemy>().enemySource = src;
            meteorM.GetComponent<AttackFromEnemy>().enemySource = src;
            meteorL.GetComponent<AttackFromEnemy>().enemySource = src;

        }


    }
}

