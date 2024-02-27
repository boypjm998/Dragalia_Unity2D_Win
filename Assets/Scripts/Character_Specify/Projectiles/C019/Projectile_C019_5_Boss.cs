using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Legend：棱彩祈愿
    /// </summary>
    public class Projectile_C019_5_Boss : MonoSingleton<Projectile_C019_5_Boss>, IEnemySealedContainer
    {
        [SerializeField] private GameObject rainbowBall;

        [SerializeField] private GameObject burstFX;

        [SerializeField] private List<GameObject> colorBalls = new();

        [SerializeField] private List<GameObject> laserBeams = new();

        [SerializeField] private GameObject enemySource;

        [SerializeField] private float waitTime = 5;

        private GameObject target;

        private List<GameObject> colorballInstances = new();

        private Dictionary<int, int> colorDict = new();

        private List<int> sortedList = new();

        private GameObject removedColorBall;

        private int safePositionIndex;

        private static float[] safePositionOffset = 
            new float[] { -13, -9.75f, -6.5f, -3.25f, 0, 3.25f, 6.5f, 9.75f, 13 };

        private bool moreOrbsIsAbove = true;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1.5f);
            StartProjectileAction();
        }

        public void StartProjectileAction()
        {
            StartBurst();
            InitBasicProperties();
            SetOrder();
        }

        public void SetEnemySource(GameObject src)
        {
            enemySource = src;
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

        public void StartBurst()
        {
            rainbowBall.SetActive(false);
            burstFX.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            burstFX.SetActive(true);
        }

        private void InitBasicProperties()
        {
            removedColorBall = colorBalls[Random.Range(0, colorBalls.Count)];
            print(removedColorBall.name);
            moreOrbsIsAbove = Random.Range(0, 2) == 0 ? true : false;
        }

        private void SetOrder()
        {
            ShuffleColorBalls(colorBalls);
            ShootFirstProjectileGroup();
            ShuffleColorBalls(colorBalls);
            ShootSecondProjectileGroup();
            DOVirtual.DelayedCall(waitTime, () => LaserAttack(), false);
        }

        private void ShootFirstProjectileGroup()
        {
            float offsetY = moreOrbsIsAbove ? 4 : -4;
            
            for (int i = 0; i < colorBalls.Count; i++)
            {
                var index = i;
                colorballInstances.Add(
                    this.InstantiateRangedObject(colorBalls[i],transform.position,
                        gameObject, 1));
                colorballInstances[i].transform.
                    DOLocalMove(new Vector2(safePositionOffset[2 * index], offsetY), 0.5f);

                colorballInstances[i].name = colorBalls[i].name;
                
                if (colorBalls[i] == removedColorBall)
                {
                    safePositionIndex = i;
                }
                
            }
            
            
        }

        private void ShootSecondProjectileGroup()
        {
            float offsetY = moreOrbsIsAbove ? -4 : 4;

            var newList = new List<GameObject>();
            
            newList.AddRange(colorBalls);
            
            print(newList.Remove(removedColorBall));
            
            for (int i = 0; i < newList.Count; i++)
            {
                var index = i;
                
                colorballInstances.Add(
                    this.InstantiateRangedObject(newList[i],transform.position,
                        gameObject, 1));
                colorballInstances[i + 5].name = newList[i].name;
                colorballInstances[i + 5].transform.
                    DOLocalMove(new Vector2(safePositionOffset[2 * index + 1], offsetY), 0.5f);

            }
            
        }

        /// <summary>
        /// 将列表的顺序打乱
        /// </summary>
        /// <returns></returns>
        private List<int> ShuffleColorIndex(int length)
        {
            
            List<int> newList = new();

            //todo: 5 orbs 根据moreOrbsIsAbove 向上或者向下发射。
            
            //todo: 打乱顺序。

            List<int> numbers = new List<int>();

            for (int i = 0; i < length; i++)
            {
                numbers.Add(i);
            }
            
            
        
            System.Random random = new System.Random();
            for (int i = numbers.Count - 1; i > 0; --i)
            {
                int j = random.Next(i + 1);
            
                // 交换当前元素与随机选取的元素位置上的值
                int temp = numbers[i];
                numbers[i] = numbers[j];
                numbers[j] = temp;
            }
        
            
            foreach (var number in numbers)
            {
                newList.Add(number);
            }

            return newList;

        }


        private void ShuffleColorBalls(List<GameObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = Random.Range(0, list.Count);
                GameObject temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }


        private void LaserAttack()
        {
            for (int i = 0; i < colorballInstances.Count; i++)
            {
                var colorBall = colorballInstances[i];

                var proj = this.InstantiateRangedObject(GetLaserPrefab(colorBall.name),
                    colorBall.transform.position, gameObject, 1,1,
                    enemySource.transform);
                
                print(proj.transform.position);

                if (colorBall.name == removedColorBall.name)
                {
                    proj.transform.localRotation = Quaternion.Euler(0,0,-90);
                }

            }

            foreach (var VARIABLE in colorballInstances)
            {
                Destroy(VARIABLE,0.1f);
            }
            Destroy((gameObject),2f);
        }

        private GameObject GetLaserPrefab(string name)
        {
            var formatName = name.Split("_")[^1];

            switch (formatName)
            {
                case "blue":
                {
                    return laserBeams[0];
                }
                case "green":
                { 
                    return laserBeams[1];
                }
                case "red":
                {
                    return laserBeams[2];
                }
                case "yellow":
                {
                    return laserBeams[3];
                }
                case "purple":
                {
                    return laserBeams[4];
                }
                default:
                    return null;
            }


        }


    }
}

