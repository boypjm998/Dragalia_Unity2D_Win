using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Shadow Alex
    /// </summary>
    public class Projectile_DB005_5 : MonoSingleton<Projectile_DB005_5>
    {
        [SerializeField] private GameObject skillFXPrefab;
        [SerializeField] private GameObject disappearFXPrefab;
        private EnemyControllerHumanoid ac;
        
        private GameObject _targetPlayer;

        public void SetTarget(GameObject target)
        {
            _targetPlayer = target;
        }

        private IEnumerator Start()
        {
            ac = GetComponent<EnemyControllerHumanoid>();
            
            
            yield return ac.KeepDistanceFromTarget(_targetPlayer, 
                3, 4, 7,false);

            ac.TurnMove(_targetPlayer);
            BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + new Vector3(0,3));

            yield return new WaitForSeconds(1);
            
            ac.anim.Play("s2");
            

            yield return new WaitForSeconds(0.125f);
            
            BackFlip();
            
            yield return new WaitForSeconds(0.3f);

            Attack();
            
            yield return new WaitForSeconds(0.33f);
            
            ToGround();

            yield return new WaitForSeconds(1);
            
            Instantiate(disappearFXPrefab, gameObject.RaycastedPosition(), Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            
            Destroy(gameObject,0.15f);
            


        }

        private void BackFlip()
        {
            ac.SetGravityScale(0);
        
            var targetPos = new Vector2(transform.position.x - ac.facedir*2,
                transform.position.y + 1.5f).SafePosition();

            var currentPlatform = gameObject.RaycastedPlatform();

            targetPos.x = 
                Mathf.Clamp(targetPos.x, currentPlatform.bounds.min.x, currentPlatform.bounds.max.x);
        
            var _tweener1 = 
                transform.DOMoveX(targetPos.x, 0.5f).SetEase(Ease.InSine).SetUpdate(UpdateType.Fixed);
        
            var _tweener2 = 
                transform.DOMoveY(targetPos.y,0.35f).SetEase(Ease.InOutCubic).SetUpdate(UpdateType.Fixed);
        }

        private void Attack()
        {
            this.InstantiateRangedObject(skillFXPrefab, transform.position,
                Instantiate(BattleStageManager.Instance.attackContainerEnemy, Vector3.zero, Quaternion.identity,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform), ac.facedir, 1, ac);
        }
        
        public void ToGround()
        {
            ac.ResetGravityScale();
        
            var targetPos = new Vector2(transform.position.x - ac.facedir*1,
                gameObject.RaycastedPosition().y + 1.2f).SafePosition();

            var currentPlatform = gameObject.RaycastedPlatform();

            targetPos.x = 
                Mathf.Clamp(targetPos.x, currentPlatform.bounds.min.x, currentPlatform.bounds.max.x);

            //var distance = transform.position.y - gameObject.RaycastedPosition().y - 1.2f;
        
            var _tweener1 = 
                transform.DOMoveY(targetPos.y,
                    0.4f).SetEase(Ease.OutCubic);
        
            var _tweener2 = 
                transform.DOMoveX(targetPos.x, 0.4f).
                    SetEase(Ease.OutSine).SetUpdate(UpdateType.Fixed);

        }
        
    }

}
