using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    public class Projectile_DB001_6 : MonoBehaviour, IEnemySealedContainer
    {
        GameObject enemySource;
        public GameObject target;

        private List<Platform> mapInfo = new();

        public GameObject centerAttack;
        public GameObject leftAttack;
        public GameObject rightAttack;
        
        public List<GameObject> attackList = new();

        private void Awake()
        {
            mapInfo = BattleStageManager.InitMapInfo();
            
        }

        private void Start()
        {
            var activeTime = attackList[0].GetComponent<EnemyAttackHintBarShine>().warningTime;
            Invoke("ReleaseAttack",activeTime+0.1f);
            SetPosition();
        }


        public void SetEnemySource(GameObject source)
        {
            enemySource = source;
        }

        protected void SetPosition()
        {
            var positionL = transform.position + 
                           new Vector3(Random.Range(5f, 15f),0,0);
            
            var positionR = transform.position -
                            new Vector3(Random.Range(5f, 15f),0,0);
            
            var positionC = 
                (Mathf.Abs(target.transform.position.x - transform.position.x)) > 5 ?
                new Vector3(Random.Range(-5f, 5f),0,0) :
                new Vector3(target.transform.position.x,0,0);


            //遍历mapInfo，找到height最低的platform
            
            mapInfo = mapInfo.OrderBy(p => p.height).ToList();
            
            //var height = 999f;
            Platform platform = mapInfo[0];
            // foreach (var p in mapInfo)
            // {
            //     //print(p.height);
            //     if (p.height < height)
            //     {
            //         height = p.height;
            //         platform = p;
            //         
            //     }
            // }
            print(platform.height);
            print(platform.collider.name);

            if (target.transform.position.y < platform.height + 15f)
            {
                positionC.y = platform.height;
                positionL.y = platform.height;
                positionR.y = platform.height;
            }
            else
            {
                Platform platformC = platform;
                Platform platformL = platform;
                Platform platformR = platform;
                
                
                
                //用Linq的OrderBy方法，按照height从小到大给mapInfo排序
                mapInfo = mapInfo.OrderBy(p => p.height).ToList();
                //mapInfo.Reverse();

                foreach (var p in mapInfo)
                {
                    
                    if (platformC.height + 15 < target.transform.position.y)
                    {
                        print("check C");
                        if(p.height + 15 >= target.transform.position.y &&
                           p.leftBorderPos.x <= positionC.x &&
                           p.rightBorderPos.x >= positionC.x)
                            platformC = p;
                    }
                    
                    if (platformL.height + 15 < target.transform.position.y)
                    {
                        if(p.height + 15 >= target.transform.position.y &&
                           p.leftBorderPos.x <= positionL.x &&
                           p.rightBorderPos.x >= positionL.x)
                            platformL = p;
                    }

                    if (platformR.height + 15 < target.transform.position.y)
                    {
                        if(p.height + 15 >= target.transform.position.y &&
                           p.leftBorderPos.x <= positionR.x &&
                           p.rightBorderPos.x >= positionR.x)
                            platformR = p;
                    }

                }
                
                positionC.y = platformC.height;
                positionL.y = platformL.height;
                positionR.y = platformR.height;

            }
            
            attackList[1].transform.position = positionC - new Vector3(0,1,0);
            attackList[0].transform.position = positionL - new Vector3(0,1,0);
            attackList[2].transform.position = positionR - new Vector3(0,1,0);


        }


        protected void ReleaseAttack()
        {
            centerAttack.transform.position = attackList[1].transform.position;
            leftAttack.transform.position = attackList[0].transform.position;
            rightAttack.transform.position = attackList[2].transform.position;
            
            centerAttack.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            leftAttack.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            rightAttack.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            
            centerAttack.SetActive(true);
            leftAttack.SetActive(true);
            rightAttack.SetActive(true);
        }


    }
}


