using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.Pool;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C015_1 : MonoSingleton<Projectile_C015_1>
    {
        [SerializeField] private GameObject figActive;
        [SerializeField] private GameObject figStandBy;

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject muzzlePrefab;
        [SerializeField] private GameObject hitInstance;

        private ObjectPool<GameObject> _bulletPool;
        private ObjectPool<GameObject> _muzzlePool;
        private ForcedAttackFromPlayer _forcedHit;
        private RelativePositionRetainer _retainer;
        private Animation _animation;
        private StatusManager _currentTargetStatus;
        private List<EnemyController> _targetList = new();
        private bool isActive = false;
        private bool isVisible = false;
        private AbilityClock _clock = new(3);
        private const float ShootInterval = 0.33f;
        private float _timer;
        private Tween _hitEffectTween;
        private Tweener _moveTweener;

        public AttackManager_C015 attackManager;
        public bool isOccupied = false;

        private Coroutine _delayRoutine;

        public bool HasTarget => _currentTargetStatus != null && _currentTargetStatus.currentHp > 0;



        private void Start()
        {
            _animation = GetComponent<Animation>();
            
            BattleStageManager.Instance.OnEnemyAwake += SearchAvailableTarget;
            BattleStageManager.Instance.OnEnemyEliminated += SearchAvailableTarget;
            
            _bulletPool = new ObjectPool<GameObject>
            (createFunc: CreateBullet, actionOnGet: OnGetBullet,
                actionOnRelease: OnReleaseBullet);

            _muzzlePool = new ObjectPool<GameObject>(createFunc: CreateMuzzle,
                OnGetBullet, OnReleaseMuzzle, OnDestroyBullet);

            _forcedHit = hitInstance.GetComponent<ForcedAttackFromPlayer>();
            _forcedHit.playerpos = attackManager.transform;

            _clock.StartTick();
            SearchAvailableTarget(1);

        }


        private void Update()
        {
            if(!isActive)
                return;
            
            if (_clock.Available)
            {
                SearchAvailableTarget(0);
                _clock.StartTick();
            }


            if (_timer > ShootInterval)
            {
                if (_currentTargetStatus == null || _currentTargetStatus.currentHp <= 0)
                {
                    //SetActive(false);
                    SearchAvailableTarget(0);
                    return;
                }
                _timer = 0;
                SetActive(true);
                FireBullet(_currentTargetStatus.gameObject);
            }
            _timer += Time.deltaTime;

        }


        private void SearchAvailableTarget(int instanceID)
        {
            var parent = BattleStageManager.Instance.EnemyLayer.transform;
            
            _targetList.Clear();

            float nearestDistance = 9999;
            GameObject nearestTarget = null;

            for (int i = 0; i < parent.childCount; i++)
            {
                var stat = parent.GetChild(i).GetComponent<EnemyController>();

                if (stat)
                {
                    if(!stat.HitSensor)
                        continue;
                    
                    if(!stat.gameObject.activeInHierarchy)
                        continue;
                    
                    if(!stat.HitSensor.isActiveAndEnabled)
                        continue;

                    _targetList.Add(stat);

                    var distance = Vector2.Distance(transform.position, stat.transform.position);

                    if (distance < nearestDistance)
                    {
                        nearestTarget = stat.gameObject;
                        nearestDistance = distance;
                    }
                }
            }

            if (nearestTarget != null)
            {
                if(isVisible)
                    SetActive(true);
                _currentTargetStatus = nearestTarget.GetComponent<StatusManager>();
            }
            else
            {
                _currentTargetStatus = null;
                SetActive(false);
            }

        }
        
        // 创建新子弹的回调
        private GameObject CreateBullet()
        {
            return Instantiate(bulletPrefab,transform.position,
                Quaternion.identity,BattleStageManager.Instance.RangedAttackFXLayer.transform);
        }
        
        private GameObject CreateMuzzle()
        {
            return Instantiate(muzzlePrefab,transform.position,
                Quaternion.identity,transform);
        }
    
        // 从池取出时的回调
        private void OnGetBullet(GameObject bullet)
        {
            bullet.SetActive(true);
        }
    
        // 放回池时的回调
        private void OnReleaseBullet(GameObject bullet)
        {
            bullet.SetActive(false);
            bullet.transform.position = transform.position;
            bullet.transform.GetChild(0).GetComponent<TrailRenderer>().Clear();
        }

        private void OnReleaseMuzzle(GameObject muzzle)
        {
            muzzle.SetActive(false);
        }
    
        // 对象销毁时的回调
        private void OnDestroyBullet(GameObject bullet)
        {
            Destroy(bullet);
        }
    
        // 发射子弹的公共方法
        public void FireBullet(GameObject target)
        {
            

            if (target.transform.position.x > transform.position.x)
            {
                figStandBy.transform.parent.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                figStandBy.transform.parent.localScale = new Vector3(1, 1, 1);
            }
            
            GameObject bullet = _bulletPool.Get();
            bullet.transform.position = transform.position;

            GameObject muzzle = _muzzlePool.Get();

            var targetPos = target.transform.position;

            float distance = Vector2.Distance(bullet.transform.position, targetPos);
            float duration = distance / 30f; // 速度计算
        
            // 创建移动动画并在结束后回收
            bullet.transform.DOMove(targetPos, duration)
                .SetEase(Ease.Linear)
            .OnComplete(() =>
                {
                    RecycleBullet(bullet, target);
                    _muzzlePool.Release(muzzle);
                });
            
            
        }
    
        // 直接回收方法（无额外组件）
        public void RecycleBullet(GameObject bullet,GameObject target)
        {
            hitInstance.SetActive(true);
            hitInstance.transform.position = bullet.transform.position;
            _forcedHit.target = target;
            _forcedHit.NextAttack();
            _bulletPool.Release(bullet);
            _hitEffectTween?.Kill(true);
            
            _hitEffectTween = DOVirtual.DelayedCall
                (0.1f, () => hitInstance.SetActive(false), false);

        }

        public void SetActive(bool flag)
        {
            isActive = flag;
            if (flag)
            {
                figActive.SetActive(true);
                figStandBy.SetActive(false);
            }
            else
            {
                figActive.SetActive(false);
                figStandBy.SetActive(true);
            }
        }

        public void SetRenderer(bool flag)
        {
            SetActive(flag);
            if (flag == false)
            {
                if (_delayRoutine != null)
                {
                    StopCoroutine(_delayRoutine);
                    _delayRoutine = null;
                }
                
                _delayRoutine = StartCoroutine(HideRendererRoutine());
            }
            else
            {
                figStandBy.transform.parent.gameObject.SetActive(true);
                isVisible = true;
            }
                
        }

        private IEnumerator HideRendererRoutine()
        {
            SetActive(false);
            yield return new WaitUntil(() => isOccupied == false);
            figStandBy.transform.parent.gameObject.SetActive(false);
            isVisible = false;
            _delayRoutine = null;
        }

        public void MoveToPosition(Vector2 position, float duration, Ease ease, bool disableAnim = true)
        {
            isOccupied = true;
            
            _animation.Stop();
            
            if (_retainer == null)
                _retainer = GetComponent<RelativePositionRetainer>();

            _retainer.isActive = false;

            _moveTweener?.Kill(false);

            _moveTweener = transform.
                DOMove(position, duration).SetEase(ease).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    if (!disableAnim)
                        _animation.Play();
                });
        }

        public void ReturnToPosition(float duration, Ease ease)
        {
            if(isOccupied == false)
                return;

            _animation.Stop();
            
            _moveTweener?.Kill(false);

            if (_retainer == null)
                _retainer = GetComponent<RelativePositionRetainer>();
            
            _moveTweener = transform.
                DOMove(_retainer.GetModifiedWorldPosition(), duration).
                SetEase(ease).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    _retainer.isActive = true;
                    _animation.Play();
                    isOccupied = false;
                });

        }
        
        
    }

}
