
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 疾风弹
    /// </summary>
    public class Projectile_DB001_7 : MonoBehaviour,IEnemySealedContainer
    {
        [SerializeField] private GameObject[] projectiles;
        private List<GameObject> waitlists = new();

        private int type;
        [SerializeField]private GameObject enemySource;

        public void SetEnemySource(GameObject source)
        {
            enemySource = source;
        }

        private void RandomSelectPatternType()
        {
            //type随机为1-3任意一个
            type = Random.Range(1, 4);
        }

        private IEnumerator Start()
        {
            RandomSelectPatternType();
            print(type);
            yield return new WaitUntil(()=>enemySource!=null);
            PrepareAttack(1, type);

            yield return null;
            ActiveDust();
            
            yield return new WaitForSeconds(4f);
            StartProjectileMove();

            yield return null;
            PrepareAttack(2,type);
            
            yield return null;
            ActiveDust();

            yield return new WaitForSeconds(2.25f);
            if(type == 1)
                yield return new WaitForSeconds(0.75f);
            StartProjectileMove();
            
            Destroy(gameObject,3f);
        }


        private void PrepareAttack(int order, int type)
        {
            foreach (var proj in projectiles)
            {
                proj.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            }
            waitlists.Clear();
            if (order == 1)
            {
                switch (type)
                {
                    case 1:
                    {
                        waitlists.Add(projectiles[1]);
                        waitlists.Add(projectiles[3]);
                        break;
                    }
                    case 2:
                    {
                        waitlists.Add(projectiles[1]);
                        waitlists.Add(projectiles[2]);
                        break;
                    }
                    case 3:
                    {
                        waitlists.Add(projectiles[2]);
                        waitlists.Add(projectiles[3]);
                        break;
                    }
                }
                
                
            }
            
            else if (order == 2)
            {
                switch (type)
                {
                    case 1:
                    {
                        waitlists.Add(projectiles[0]);
                        waitlists.Add(projectiles[2]);
                        waitlists.Add(projectiles[4]);
                        break;
                    }
                    case 2:
                    {
                        waitlists.Add(projectiles[0]);
                        waitlists.Add(projectiles[3]);
                        waitlists.Add(projectiles[4]);
                        break;
                    }
                    case 3:
                    {
                        waitlists.Add(projectiles[0]);
                        waitlists.Add(projectiles[1]);
                        waitlists.Add(projectiles[4]);
                        break;
                    }
                }
            }
        }

        private void StartProjectileMove()
        {
            foreach (var proj in waitlists)
            {
                proj.GetComponent<DOTweenSimpleController>().enabled = true;
            }

        }

        private void ActiveDust()
        {
            foreach (var proj in waitlists)
            {
                
                proj.transform.GetChild(0).Find("dust").gameObject.SetActive(true);
            }
        }





    }
}

