using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Fog
    /// </summary>
    public class Projectile_C007_2_Boss : MonoSingleton<Projectile_C007_2_Boss>
    {
        //static fields
        public static float PolygonSize = 24;

        public int lastTime = 15;
        public int inflictFreezeChance = 20;

        //SerializeFields
        [SerializeField] private GameObject fogInstanceL;
        [SerializeField] private GameObject fogInstanceR;

        [SerializeField] private GameObject stormInstanceL;
        [SerializeField] private GameObject stormInstanceR;

        [SerializeField] private bool debugCollider;

        //public Properties
        public bool FogStarted => _startFog;
        public bool FromLeft => _direction == 1;
        public bool FromRight => _direction == -1;


        //private fields
        private bool _startFog = false;
        private int _direction;
        private PolygonCollider2D _collider2D;
        private List<Projectile_C007_3_Boss> icePillarList = new();
        private AttackFromEnemy _stormAttack;
        private GameObject _enemySource;
        private Tween _resetTween = null;
        private Tween _stormTween = null;


        private void Start()
        {
            fogInstanceL.SetActive(false);
            fogInstanceR.SetActive(false);
            _collider2D = GetComponentInChildren<PolygonCollider2D>();
            _stormAttack = _collider2D.GetComponent<AttackFromEnemy>();
            _stormAttack.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Freeze,
                1, 3, 1), inflictFreezeChance);
            _stormAttack.enemySource = _enemySource;
            _stormAttack.BeforeAttackHit += EnemyMoveManager.PurgedShapeShiftingOfTarget;
        }

        private void Update()
        {
            if (debugCollider)
            {
                StartStorm();
                debugCollider = false;
            }
        }

        public void SetEnemySource(GameObject src)
        {
            _enemySource = src;
        }

        public void StopFogEffect()
        {
            _resetTween?.Kill();
            if (FromLeft)
            {
                fogInstanceL.GetComponent<ParticleSystem>().Stop();
                _resetTween = DOVirtual.DelayedCall(9, () => fogInstanceL.SetActive(false), false);
            }

            if (FromRight)
            {
                fogInstanceR.GetComponent<ParticleSystem>().Stop();
                _resetTween = DOVirtual.DelayedCall(9, () => fogInstanceR.SetActive(false), false);
            }

            _startFog = false;
        }

        public void StartFogEffectRandom()
        {
            _resetTween?.Kill();
            if (Random.Range(0, 2) == 0)
            {
                fogInstanceL.SetActive(true);
                _direction = 1;
            }
            else
            {
                fogInstanceR.SetActive(true);
                _direction = -1;
            }

            _startFog = true;
        }

        public void AddIcePillar(Projectile_C007_3_Boss pillar)
        {
            icePillarList.Add(pillar);
        }

        public void RemoveIcePillar(Projectile_C007_3_Boss pillar)
        {
            icePillarList.Remove(pillar);
        }

        public void StartStorm()
        {
            if (!FogStarted)
                return;
            
            _stormTween?.Kill(true);

            List<Vector2> points = new List<Vector2>();

            var pillars = PreprocessObstacles(icePillarList);

            _collider2D.SetPath(0, pillars.ToArray());

            if (FromLeft)
            {
                stormInstanceL.SetActive(true);
            }

            if (FromRight)
            {
                stormInstanceR.SetActive(true);
            }

            _stormTween = DOVirtual.DelayedCall(1, () =>
            {
                _collider2D.enabled = true;
            }, false);


            StopFogEffect();
            DOVirtual.DelayedCall(lastTime, () => StopStorm(), false);
        }

        public void StopStorm()
        {
            _stormTween?.Kill(true);
            
            stormInstanceL.GetComponent<ParticleSystem>().Stop();
            stormInstanceR.GetComponent<ParticleSystem>().Stop();

            _collider2D.enabled = false;
            
            //StopFogEffect();

            _stormTween = DOVirtual.DelayedCall(1.5f, () =>
            {
                stormInstanceL.SetActive(false);
                stormInstanceR.SetActive(false);
            }, false);

            for (int i = icePillarList.Count - 1; i >= 0; i--)
            {
                icePillarList[i].DestroyInstance();
            }
            


        }
        
        
        private List<Vector2> PreprocessObstacles(List<Projectile_C007_3_Boss> obstacles)
        {
            var processedObstacles = new List<Vector2>();

            var obstaclePointList = new List<(Vector2 point, Projectile_C007_3_Boss instance)>();
            
            
            foreach (var obstacle in obstacles)
            {
                obstaclePointList.Add((obstacle.LowerPoint, obstacle));
                //obstaclePointList.Add((obstacle.UpperPoint, obstacle));
            }
            
            obstaclePointList.Sort((a, b) => a.point.y.CompareTo(b.point.y));

            for (int i = 0; i < obstaclePointList.Count - 1; i++)
            {
                if (obstaclePointList[i].point.y == obstaclePointList[i + 1].point.y)
                {
                    if (FromRight && obstaclePointList[i].point.x < obstaclePointList[i+1].point.x
                        && obstaclePointList[i].instance.Level > obstaclePointList[i+1].instance.Level)
                    {
                        var temp = obstaclePointList[i];
                        obstaclePointList[i] = obstaclePointList[i + 1];
                        obstaclePointList[i + 1] = temp;
                    }
                    else if (FromLeft && obstaclePointList[i].point.x > obstaclePointList[i + 1].point.x
                                      && obstaclePointList[i].instance.Level > obstaclePointList[i+1].instance.Level)
                    {
                        var temp = obstaclePointList[i];
                        obstaclePointList[i] = obstaclePointList[i + 1];
                        obstaclePointList[i + 1] = temp;
                    }
                }
            }
            
            int direction = FromLeft ? 1 : -1;

            Vector2 lowestPoint;
            Vector2 nextLowestPoint;

            if (FromLeft)
            {
                processedObstacles.Add(new Vector2(-PolygonSize,-PolygonSize));
                processedObstacles.Add(new Vector2(PolygonSize,-PolygonSize));
                processedObstacles.Add(new Vector2(PolygonSize,obstaclePointList[0].point.y));
            }
            else
            {
                processedObstacles.Add(new Vector2(PolygonSize,-PolygonSize));
                processedObstacles.Add(new Vector2(-PolygonSize,-PolygonSize));
                processedObstacles.Add(new Vector2(-PolygonSize,obstaclePointList[0].point.y));
                
            }
            
            
            direction *= -1;//代表多边形碰撞体添加点的方向

            for (int i = 0; i < obstaclePointList.Count - 1; i++)
            {
                lowestPoint = obstaclePointList[i].point;
                nextLowestPoint = obstaclePointList[i + 1].point;

                if (direction == -1)
                {
                    var upperPoint = (obstaclePointList[i].instance.UpperPoint);
                    if (upperPoint.y > nextLowestPoint.y && upperPoint.x < nextLowestPoint.x)
                    {
                        bool covered = false;
                        //如果下一个障碍物被完全遮挡
                        while (i < obstaclePointList.Count - 1 &&
                               obstaclePointList[i+1].instance.UpperPoint.y < upperPoint.y)
                        {
                            i++;
                            covered = true;
                            break;
                        }
                        
                        if(covered)
                            continue;
                        
                        if(i == 0)
                            processedObstacles.Add(lowestPoint);
                        print("Add"+lowestPoint);
                        processedObstacles.Add(upperPoint);
                        print("Add"+upperPoint);
                        processedObstacles.Add(new Vector2(nextLowestPoint.x, upperPoint.y));
                        print("Add"+new Vector2(nextLowestPoint.x, upperPoint.y));
                        
                    }
                    else if (upperPoint.y > nextLowestPoint.y && upperPoint.x >= nextLowestPoint.x)
                    {
                        if(i == 0)
                            processedObstacles.Add(lowestPoint);
                        print("Add"+lowestPoint);
                        processedObstacles.Add(new Vector2(lowestPoint.x, nextLowestPoint.y));
                        print("Add"+new Vector2(lowestPoint.x, nextLowestPoint.y));
                        processedObstacles.Add(nextLowestPoint);
                        print("Add"+nextLowestPoint);
                    }
                    else
                    {
                        if(i == 0 && upperPoint.y != nextLowestPoint.y)
                            processedObstacles.Add(lowestPoint);
                        print("Add"+lowestPoint);
                        processedObstacles.Add(upperPoint);
                        print("Add"+upperPoint);

                        if (i + 1 < obstaclePointList.Count)
                        {
                            processedObstacles.Add(new Vector2(PolygonSize,upperPoint.y));
                            processedObstacles.Add(new Vector2(PolygonSize,nextLowestPoint.y));
                            processedObstacles.Add(nextLowestPoint);
                        }
                        
                    }

                }
                else
                {
                    var upperPoint = (obstaclePointList[i].instance.UpperPoint);
                    if (upperPoint.y > nextLowestPoint.y && upperPoint.x > nextLowestPoint.x)
                    {
                        bool covered = false;
                        //如果下一个障碍物被完全遮挡
                        while (i < obstaclePointList.Count - 1 &&
                               obstaclePointList[i+1].instance.UpperPoint.y < upperPoint.y)
                        {
                            i++;
                            covered = true;
                            print("Covered");
                            break;
                        }
                        
                        if(covered)
                            continue;
                        
                        if(i == 0)
                            processedObstacles.Add(lowestPoint);
                        processedObstacles.Add(upperPoint);
                        processedObstacles.Add(new Vector2(nextLowestPoint.x, upperPoint.y));
                        
                    }
                    else if (upperPoint.y > nextLowestPoint.y && upperPoint.x <= nextLowestPoint.x)
                    {
                        if(i == 0)
                            processedObstacles.Add(lowestPoint);
                        processedObstacles.Add(new Vector2(lowestPoint.x, nextLowestPoint.y));
                        processedObstacles.Add(nextLowestPoint);
                    }
                    else
                    {
                        if(i == 0 && upperPoint.y != nextLowestPoint.y)
                            processedObstacles.Add(lowestPoint);
                        processedObstacles.Add(upperPoint);
                        if (i + 1 < obstaclePointList.Count)
                        {
                            processedObstacles.Add(new Vector2(-PolygonSize,upperPoint.y));
                            processedObstacles.Add(new Vector2(-PolygonSize,nextLowestPoint.y));
                            processedObstacles.Add(nextLowestPoint);
                        }
                    }
                }
                
                
                
                
            }

            if (FromLeft)
            {
                var highestPoint = obstaclePointList[^1].instance.UpperPoint;
                if(obstaclePointList[^1].point.y >= processedObstacles[^1].y)
                    processedObstacles.Add(obstaclePointList[^1].point);
                processedObstacles.Add(highestPoint);
                processedObstacles.Add(new Vector2(PolygonSize,highestPoint.y));
                processedObstacles.Add(new Vector2(PolygonSize,PolygonSize));
                processedObstacles.Add(new Vector2(-PolygonSize,PolygonSize));
            }else if (FromRight)
            {
                var highestPoint = obstaclePointList[^1].instance.UpperPoint;
                if(obstaclePointList[^1].point.y >= processedObstacles[^1].y)
                    processedObstacles.Add(obstaclePointList[^1].point);
                processedObstacles.Add(highestPoint);
                processedObstacles.Add(new Vector2(-PolygonSize,highestPoint.y));
                processedObstacles.Add(new Vector2(-PolygonSize,PolygonSize));
                processedObstacles.Add(new Vector2(PolygonSize,PolygonSize));
            }

            for (int i = processedObstacles.Count - 1; i >= 1; i--)
            {
                if (processedObstacles[i].x == processedObstacles[i - 1].x &&
                    processedObstacles[i].y == processedObstacles[i - 1].y)
                {
                    processedObstacles.RemoveAt(i);
                }
            }
            
            
            
            
            
            

            return processedObstacles;
        }
        
        
        
        
        
        
    }
}