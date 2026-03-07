using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 蝙蝠
    /// </summary>
    public class Projectile_H004_2 : MonoSingleton<Projectile_H004_2>
    {
        [SerializeField] private GameObject waterballPrefab;
        [SerializeField] private float projectileShootInterval = 5;
        [SerializeField] private float lifeTime = 20;
        [SerializeField] private GameObject destructionPrefab;
        [SerializeField] private GameObject projectileHintPrefab;
        [SerializeField] private GameObject projectileHintPrefabAdv;

        private HomingAttackWithoutRotate _homing;
        private ActorBase _ac;
        private GameObject _bossGO;
        private StatusManager _selfStatusManager;
        private Coroutine _routine;
        private float _timer = 0;
        private float _destructTimer;
        private GameObject _hintbarInstance;
        public bool Destruct { get; private set; } = false;

        private void Start()
        {
            _homing = GetComponent<HomingAttackWithoutRotate>();
            _homing.target = BattleStageManager.Instance.GetPlayer().transform;
            _ac = GetComponent<ActorBase>();
            _selfStatusManager = GetComponent<StatusManager>();
            _destructTimer = lifeTime;
            Invoke("SetDestruction",lifeTime);
            _selfStatusManager.OnHPBelow0 += StopRoutine;
            if(_routine == null)
                _routine = StartCoroutine(ShootProjectiles());
        }

        private void Update()
        {
            if(Destruct)
                return;
            
            _timer += Time.deltaTime;
            _destructTimer -= Time.deltaTime;

            if(_destructTimer <= projectileShootInterval)
                return;

            if (_timer >= projectileShootInterval)
            {
                _timer = 0;
                
                if(_routine == null)
                    _routine = StartCoroutine(ShootProjectiles());
            }
            
            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _selfStatusManager.OnHPBelow0 -= StopRoutine;
            
        }

        private void StopRoutine()
        {
            if(_routine != null)
                StopCoroutine(_routine);
            _routine = null;
            if(_hintbarInstance != null)
                Destroy(_hintbarInstance);
        }

        private IEnumerator ShootProjectiles()
        {
            var boost = _destructTimer > 0.8f * lifeTime;
            
            var hintbarInstance = Instantiate(
                boost ? projectileHintPrefab : projectileHintPrefabAdv, 
                transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform
            );

            _hintbarInstance = hintbarInstance;

            var relativePositionRetainer = hintbarInstance.AddComponent<RelativePositionRetainer>();
            relativePositionRetainer.SetParent(transform);

            var target = BattleStageManager.Instance.GetPlayer();
            var chaser = hintbarInstance.GetComponent<EnemyAttackHintBarChaser>();
            chaser.target = target;
            chaser.SetLockTime(1.05f);
            chaser.SetRotateSpeed(150);
            chaser.isRanged = true;

            var angle = ObjectExtensions.AngleDegree(transform, target.transform);
            hintbarInstance.transform.eulerAngles = new Vector3(0, 0, angle);

            var warningBar = hintbarInstance.GetComponentInChildren<EnemyAttackHintBar>();

            yield return new WaitForSeconds(warningBar.warningTime);

            relativePositionRetainer.enabled = false;

            if (boost)
            {
                this.InstantiateDirectionalRangedObject(waterballPrefab, transform.position,
                    BattleStageManager.Instance.GetNewRangedContainer(), 1, 
                    chaser.transform.rotation.eulerAngles.z);
            }
            else
            {
                var container = BattleStageManager.Instance.GetNewRangedContainer();
                this.InstantiateDirectionalRangedObject(waterballPrefab, transform.position,
                    container, 1, 
                    chaser.transform.rotation.eulerAngles.z);
                this.InstantiateDirectionalRangedObject(waterballPrefab, transform.position,
                    container, 1, 
                    chaser.transform.rotation.eulerAngles.z+10);
                this.InstantiateDirectionalRangedObject(waterballPrefab, transform.position,
                    container, 1, 
                    chaser.transform.rotation.eulerAngles.z-10);
            }

            

            _routine = null;
        }

        public void SetBossGameObject(GameObject go)
        {
            _bossGO = go;
        }

        private void SetDestruction()
        {
            Destruct = true;
            
            EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(_ac, transform.position,
                transform, 50, Vector2.zero, false, true, 0.5f,
                0.2f, 0.5f, true, false);

            _homing.enabled = false;

            DOVirtual.DelayedCall(0.5f, ()=>FullscreenDestruction(), false);

        }

        private void FullscreenDestruction()
        {

            this.InstantiateRangedObject(destructionPrefab, transform.position,
                BattleStageManager.Instance.GetNewRangedContainer(), _ac.facedir);
            
            DamageNumberManager.Instance.IndirectDamagePop(99999,transform);
            _selfStatusManager.currentHp = 0;
            _selfStatusManager.OnHPBelow0?.Invoke();


        }

        public void DoDeath()
        {
            Destruct = true;
            _homing.enabled = false;
            //_ac.SetHitSensor(true);
            DamageNumberManager.Instance.IndirectDamagePop(99999,transform);
            _selfStatusManager.currentHp = 0;
            _selfStatusManager.OnHPBelow0?.Invoke();
        }
        
        

    }
}

