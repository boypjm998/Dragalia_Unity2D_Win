using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Summer Alex's Support Attack
    /// </summary>
    public class Projectile_C036_2 : MonoBehaviour
    {
        [SerializeField] private GameObject appearFX;
        [SerializeField] private Transform shootPointTransform;
        [SerializeField] private GameObject muzzleFX;
        [SerializeField] private GameObject projectilesFX;
        [SerializeField] private GameObject blastFX;
        [SerializeField] private float projectileSpeed;
        

        private Transform _playerSource;
        private AttackContainer _container;
        private float _buffEffect;
        private Animation _animation;

        public void SetSource(Transform src,float buffAmount)
        {
            _playerSource = src;
            _buffEffect = buffAmount;
        }

        public void SetContainer(GameObject container)
        {
            _container = container.GetComponent<AttackContainer>();
        }

        public float GetDistance(float height)
        {
            var distance2 = height / Mathf.Tan(20 * Mathf.Deg2Rad);
            return distance2;
        }
        private void Awake()
        {
            _animation = GetComponent<Animation>();
        }

        private IEnumerator Start()
        {
            var clip = _animation.clip;
            _animation[clip.name].normalizedTime = 0.35f;
            int dir = 1;

            yield return new WaitUntil(() => _animation[clip.name].normalizedTime > 0.485f);

            var muzzle = Instantiate(muzzleFX, transform.parent.position, muzzleFX.transform.rotation,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            
            if (transform.parent.localScale.x == -1)
            {
                dir = -1;
                muzzle.transform.rotation = Quaternion.Euler(0,180,muzzle.transform.localEulerAngles.z);
            }

            muzzle.transform.position = muzzle.transform.position + new Vector3(dir * 1.2f, 1.2f);
            
            yield return new WaitUntil(() => _animation[clip.name].normalizedTime > 0.5f);

            var proj1 = Instantiate(projectilesFX, transform.parent.position+ new Vector3(dir * 1.2f, 1.2f), 
                Quaternion.Euler(0, 0, -23),
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            var proj2 = Instantiate(projectilesFX, transform.parent.position+ new Vector3(dir * 1.2f, 1.2f), 
                Quaternion.Euler(0, 0, -20),
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            var proj3 = Instantiate(projectilesFX, transform.parent.position+ new Vector3(dir * 1.2f, 1.2f), 
                Quaternion.Euler(0, 0, -17),
                BattleStageManager.Instance.RangedAttackFXLayer.transform);

            var height = transform.position.y - _playerSource.gameObject.RaycastedPosition().y;
            var distance1 = height / Mathf.Tan(23 * Mathf.Deg2Rad);
            var distance2 = height / Mathf.Tan(20 * Mathf.Deg2Rad);
            var distance3 = height / Mathf.Tan(17 * Mathf.Deg2Rad);
            

            if (transform.parent.localScale.x == -1)
            {
                proj1.transform.rotation = Quaternion.Euler(0,180,proj1.transform.localEulerAngles.z);
                proj2.transform.rotation = Quaternion.Euler(0,180,proj2.transform.localEulerAngles.z);
                proj3.transform.rotation = Quaternion.Euler(0,180,proj3.transform.localEulerAngles.z);
                dir = -1;
            }

            proj1.transform.DOMove(transform.position + new Vector3(distance1 * dir, -height),
                distance2 / projectileSpeed).SetEase(Ease.Linear);
            proj2.transform.DOMove(transform.position + new Vector3(distance2 * dir, -height),
                distance2 / projectileSpeed).OnComplete(() =>
            {
                var container = new GameObject("Container2");
                var subContainer = container.AddComponent<AttackSubContainer>();
                subContainer.InitAttackContainer(1,true);
                subContainer.InitAttackContainer(1,_container.gameObject);
                var atk = this.InstantiateRangedObject(blastFX, proj2.transform.position, container,
                    1, 1, _playerSource).GetComponent<AttackFromPlayer>();
                atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
                        44,21,100,-1),110);
                
                var dmgUp = Mathf.Clamp(_buffEffect * 0.05f, 0, 0.8f);
                
                atk.AddConditionalAttackEffect(new ConditionalAttackEffect((s1, s2) => { return true; },
                    ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier, new string[] {},
                    new string[] { (dmgUp).ToString() }));
                
                
                Destroy(proj1);
                Destroy(proj2);
                Destroy(proj3);
            }).SetEase(Ease.Linear);
            proj3.transform.DOMove(transform.parent.position + new Vector3(distance3 * dir, -height),
                distance2 / projectileSpeed).SetEase(Ease.Linear);
            
            
            yield return new WaitUntil(() => _animation[clip.name].normalizedTime > 0.59f);

            transform.DOMoveY(transform.parent.position.y - height, 0.33f).SetEase(Ease.InSine);
            appearFX.SetActive(false);

            yield return new WaitUntil(() => _animation[clip.name].normalizedTime >= 0.98f);
            appearFX.transform.position = transform.position + new Vector3(0,1.3f);
            appearFX.SetActive(true);

            yield return new WaitForSeconds(0.1f);
            
            Destroy(transform.parent.gameObject);

        }
    }

}
