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
    /// Topdown Projectile Controller
    /// (终末风暴 从上往下发射的子弹控制器)
    /// </summary>
    public class Projectile_C001_9_Boss : MonoSingleton<Projectile_C001_9_Boss>, IEnemySealedContainer
    {
        public GameObject playerGO;
        
        [SerializeField] private GameObject projectilePrefab;

        [SerializeField] private GameObject crossProjectilePrefab;

        [SerializeField] private GameObject laserPrefab;

        [SerializeField][Range(0.5f,2f)] private float shootInterval = 1f;

        [SerializeField] private float playerVelocity = -1.7f;
        
        private float _projectileSpeedX = 1f;
        
        private float _projectileSpeedY = 5f;

        private Coroutine _attackCoroutine = null;

        private int _safePoint = 12;

        private GameObject _attackContainer;

        private GameObject _enemySource;

        private const int MaxLeftPosition = 0;
        
        private const int MaxRightPosition = 24;

        private readonly float[] Positions = 
            {-24.5f,-21.5f,-20,-18,-16,-14,-12,-10,-8,-6,-4,-2,0,2,4,6,8,10,12,14,16,18,20,21.5f,24.5f };
        
        private readonly float[] Positions2 = 
            {-24,-21,-18,-15,-12,-9,-6,-3,0,3,6,9,12,15,18,21f,24f };

        private TimerBuff _punishBuff = 
            new TimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff,10,30,5,
                EnemyMoveController_HB03_Legend.AttackDownSPID);

        
        private List<int[]> _patterns = new();
        private int index;
        
        //偶数从右往左。奇数从左往右
        
        private int[] _pattern0 = {1,1,1,1,1,1,0,1, 1, 1,0,1,1,1,1,1,1};
        
        private int[] _pattern1 = {1,1,1,1,0,1,1,0, 1, 0,1,1,0,1,1,1,1};
        
        private int[] _pattern2 = {1,1,0,1,1,0,1,1, 0, 1,1,0,1,1,0,1,1};
        
        private int[] _pattern3 = {1,1,1,1,0,1,0,1, 1, 1,0,1,0,1,1,1,1};
        
        private int[] _pattern4 = {1,1,0,1,1,0,1,1, 0, 1,1,0,1,1,0,1,1};
        
        private int[] _pattern5 = {1,1,1,1,0,1,0,1, 1, 1,0,1,0,1,1,1,1};
        
        private int[] _pattern6 = {1,1,1,0,1,0,1,1, 0, 1,1,0,1,0,1,1,1};
        
        // private int[] _pattern0 = {1,1,1,1,1,1,1,1,1,0,0,1, 1, 1,0,0,1,1,1,1,1,1,1,1,1};
        //
        // private int[] _pattern1 = {1,1,0,0,1,1,1,1,1,1,0,0, 1, 0,0,1,1,1,1,1,1,0,0,1,1};
        //
        // private int[] _pattern2 = {1,1,1,1,0,0,1,1,0,0,1,1, 1, 1,1,0,0,1,1,0,0,1,1,1,1};
        //
        // private int[] _pattern3 = {1,1,0,0,1,1,0,0,1,1,1,1, 0, 1,1,1,1,0,0,1,1,0,0,1,1};
        //
        // private int[] _pattern4 = {1,0,1,1,0,0,1,1,0,1,1,1, 0, 1,1,1,1,0,1,0,0,1,1,0,1};
        //
        // private int[] _pattern5 = {1,1,0,0,1,1,1,1,0,0,1,1, 0, 1,1,0,0,1,1,1,1,0,0,1,1};
        //
        // private int[] _pattern6 = {1,1,1,1,0,0,1,1,1,1,0,0, 1, 0,0,1,1,1,1,0,0,1,1,1,1};

        private void Start()
        {
            _attackContainer = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
                transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);

            _punishBuff.dispellable = false;

            _projectileSpeedX = crossProjectilePrefab.GetComponent<ProjectileControllerTest>().velocity.x;
            _projectileSpeedY = crossProjectilePrefab.GetComponent<ProjectileControllerTest>().velocity.y;
            
            _patterns.Add(_pattern0);
            _patterns.Add(_pattern1);
            _patterns.Add(_pattern2);
            _patterns.Add(_pattern3);
            _patterns.Add(_pattern4);
            _patterns.Add(_pattern5);
            _patterns.Add(_pattern6);
            

        }

        public void SetEnemySource(GameObject enemySource)
        {
            _enemySource = enemySource;
        }
        
        
        public void StartShooting()
        {
            if(_attackCoroutine == null)
                _attackCoroutine = StartCoroutine(ShootingProjectileRoutine());
        }
        
        public void StopShooting()
        {
            if(_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
        }



        private IEnumerator ShootingProjectileRoutine()
        {
            index = 0;
            bool flag = false;
            yield return new WaitForSeconds(shootInterval);
            
            GenerateProjectile();

            if (Random.Range(0, 2) == 0)
            {
                DOVirtual.DelayedCall(4,() =>
                {
                    LaserProjectile(-1);
                },false);
                DOVirtual.DelayedCall(13,() =>
                {
                    LaserProjectile(1);
                },false);
            }
            else
            {
                DOVirtual.DelayedCall(4,() =>
                {
                    LaserProjectile(1);
                },false);
                DOVirtual.DelayedCall(12,() =>
                {
                    LaserProjectile(-1);
                },false);
            }
            
            yield return new WaitForSeconds(shootInterval);

            while (true)
            {
                GenerateCrossProjectile();

                yield return new WaitForSeconds(shootInterval);

                if (!flag)
                {
                    _patterns.Remove(_pattern0);
                    index = Random.Range(0, _patterns.Count);
                    flag = true;
                }
            }
            
            
        }

        private void LaserProjectile(int dir)
        {
            print("Laser");
            
            var projectile =
                this.InstantiateRangedObject(laserPrefab,
                    new Vector3(dir > 0 ? -24 : 24, playerGO.transform.position.y - 20), _attackContainer, 1
                    ,1,_enemySource.transform);

            if (dir < 0)
            {
                var controller = projectile.GetComponent<DOTweenSimpleController>();
                controller.moveDirection = Vector2.left * controller.moveDirection.x;
            }
            
            projectile.GetComponent<AttackFromEnemy>().OnAttackHit += RemoveOneSpecificDamageCut;
        }
        private void GenerateProjectile()
        {
            _safePoint = Mathf.Clamp(_safePoint, MaxLeftPosition, MaxRightPosition);
            
            
            int leftSafePosition = _safePoint - Random.Range(1, 3);
            int rightSafePosition = _safePoint + Random.Range(1, 3);

            int rng = Random.Range(0, 2);

            int anotherSafePoint = rng == 0 ? leftSafePosition : rightSafePosition;
            
            anotherSafePoint = Mathf.Clamp(anotherSafePoint, MaxLeftPosition, MaxRightPosition);
            
            
            for (int i = 0; i < Positions.Length; i++)
            {
                if(i == anotherSafePoint || i == _safePoint)
                    continue;

                var projectile =
                    this.InstantiateRangedObject(projectilePrefab,
                        new Vector3(Positions[i], transform.position.y + Random.Range(2.5f,3.5f)), _attackContainer, 1
                        ,1,_enemySource.transform);

                projectile.GetComponent<AttackFromEnemy>().OnAttackHit += RemoveOneSpecificDamageCut;
            }

            _safePoint = rng == 1 ? leftSafePosition : rightSafePosition;
            


        }

        private void GenerateCrossProjectile()
        {
            print("Pattern"+index+" is selected");
            var pattern = _patterns[index];
            
            index = (index + 1) % _patterns.Count;
            
            for (int i = 0; i < Positions2.Length; i++)
            {
                if(pattern[i] == 0)
                    continue;

                int shootDir = 0;
                if(i % 2 == 0 && i > 8)
                    shootDir = -1;
                else if(i % 2 == 1 && i < 8)
                    shootDir = -1;
                else if (i == 8)
                {
                    shootDir = 0;
                }
                else
                {
                    shootDir = 1;
                }
                
                
                
                var position = PredictShootPosition(Positions2[i], shootDir);

                var projectile =
                    this.InstantiateRangedObject(crossProjectilePrefab,
                        new Vector3(position, playerGO.transform.position.y + 16), _attackContainer, 1
                        ,1,_enemySource.transform);

                projectile.GetComponent<AttackFromEnemy>().OnAttackHit += RemoveOneSpecificDamageCut;
                var controller = projectile.GetComponent<ProjectileControllerTest>();
                
                controller.SetVelocity(new Vector2(shootDir * controller.velocity.x,controller.velocity.y));
                


            }
            
            
        }

        private float PredictShootPosition(float targetPosX, int direction)
        {
            // 玩家的速度
            float v1 = playerVelocity;
            // 子弹的速度
            float v2 = _projectileSpeedY;
            // 子弹的发射位置的y坐标
            float startY = playerGO.transform.position.y + 16;
            // 子弹的水平速度
            float velocityX = _projectileSpeedX;

            // 计算子弹从发射位置到玩家所在位置的时间
            float time = (playerGO.transform.position.y - startY) / (v2 - v1);
            print("Time: "+time);
            // 计算子弹在水平方向上的位移
            float displacement = time * velocityX * direction;
            // 计算子弹的发射位置
            float startX = targetPosX - displacement;

            return startX;

        }

        private void RemoveOneSpecificDamageCut(AttackBase attackBase, GameObject target)
        {
            attackBase.OnAttackHit -= RemoveOneSpecificDamageCut;
            
            var flag = target.GetComponent<StatusManager>().RemoveSpecificTimerbuff(
                (int)BasicCalculation.BattleCondition.DamageCutConst,
                EnemyMoveController_HB03_Legend.DamageCutSPID,true);

            if (flag)
                BattleEffectManager.Instance.SpawnEffect(target,BasicCalculation.BattleCondition.Dispell);
            else
            {
                target.GetComponent<StatusManager>().ObtainTimerBuff(new TimerBuff(_punishBuff));
            }
        }



    }

}
