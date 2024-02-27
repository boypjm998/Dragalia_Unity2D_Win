using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C036_1 : MonoBehaviour
    {
        [SerializeField] private GameObject blashFX;
        [SerializeField] private GameObject sandcastleBlastFX;
        [SerializeField] private float range = 8;
        [SerializeField] private float rng = 2;
        [SerializeField] private float speed = 10;
        [SerializeField] private float accerlation = 1;

        public float maxRange => range + rng;

        private Tweener _moveTweener;
        private Transform _playerSource;
        private GameObject _target;
        private Vector2 _targetPos;
        private int _dir;
        private GameObject sandCastleFX;


        public void SetSource(Transform playerSource,int dir = 1)
        {
            _playerSource = playerSource;
            _dir = dir;
            transform.GetChild(0).localEulerAngles = new Vector3(0, dir * 90, 0);
        }
        

        public void SetTarget(GameObject target)
        {
            _target = target;
        }

        public void SetGoal()
        {
            Vector2 targetPos;
            
            print(_target);

            if (_target == null)
            {

                var safePosition = Mathf.Clamp(_playerSource.transform.position.x + range * _dir,
                    BattleStageManager.Instance.mapBorderL, BattleStageManager.Instance.mapBorderR);
                
                targetPos.y = BasicCalculation.GetRaycastedPlatformY(new Vector2(
                    safePosition,
                    _playerSource.position.y));
                targetPos.x = safePosition;
                
                // if (targetPos.x > BattleStageManager.Instance.mapBorderR)
                //     targetPos.x = BattleStageManager.Instance.mapBorderR;
                // if (targetPos.x < BattleStageManager.Instance.mapBorderL)
                //     targetPos.x = BattleStageManager.Instance.mapBorderL;
                
                _targetPos = targetPos;
                
                
                return;
            }
            
            targetPos = _target.RaycastedPosition();

            if (Mathf.Abs(targetPos.x - _playerSource.transform.position.x) > range + rng ||
                targetPos.y > _playerSource.transform.position.y + 6)
            {
                targetPos.x = _playerSource.transform.position.x + (range + rng)* _dir;
                targetPos.y = BasicCalculation.GetRaycastedPlatformY(new Vector2(
                    _playerSource.transform.position.x + (range + rng)* _dir,
                    _playerSource.position.y));
            }else if (Mathf.Abs(targetPos.x - _playerSource.transform.position.x) < range - rng)
            {
                var safePosition = Mathf.Clamp(_playerSource.transform.position.x + (range - rng) * _dir,
                    BattleStageManager.Instance.mapBorderL, BattleStageManager.Instance.mapBorderR);


                targetPos.x = safePosition;
                targetPos.y = BasicCalculation.GetRaycastedPlatformY(new Vector2(
                    safePosition,
                    _playerSource.position.y));
            }

            if (targetPos.x > BattleStageManager.Instance.mapBorderR)
                targetPos.x = BattleStageManager.Instance.mapBorderR;
            if (targetPos.x < BattleStageManager.Instance.mapBorderL)
                targetPos.x = BattleStageManager.Instance.mapBorderL;

            _targetPos = targetPos;
            //transform.DOMove()
        }

        public void SetSandCastle(GameObject gameObject)
        {
            sandCastleFX = gameObject;
        }

        

        private void Start()
        {
            var distance = Vector2.Distance(transform.position, _targetPos);

            var initialSpeed = speed;

            var tweenTime =
                distance / initialSpeed;
            
            print(distance);
            print(tweenTime);
            print(_targetPos);

            _moveTweener = transform.DOMove(_targetPos, tweenTime).SetEase(Ease.InSine).OnComplete(() =>
            {
                var container = new GameObject("Container");
                container.transform.position = transform.parent.position;
                container.AddComponent<AttackContainer>().InitAttackContainer(1,true);
                var atkProj = this.InstantiateRangedObject(blashFX,_targetPos,container,1,1,_playerSource);
                var atk = atkProj.GetComponent<AttackFromPlayer>();
                atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.ShadowBlight,44f,21f,100),110);
                atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Bog,1,8f,1),110,1);
                if (sandCastleFX != null)
                {
                    Instantiate(sandcastleBlastFX, sandCastleFX.transform.position,
                        Quaternion.identity);
                    Destroy(sandCastleFX);
                }
                Destroy(gameObject);

                
            });

        }
    }

}
