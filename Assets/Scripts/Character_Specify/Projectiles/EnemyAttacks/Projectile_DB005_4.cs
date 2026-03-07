using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Shadow Bellina
    /// </summary>
    public class Projectile_DB005_4 : MonoSingleton<Projectile_DB005_4>
    {
        [SerializeField] private GameObject muzzleFXPrefab;
        [SerializeField] private GameObject projectileFXPrefab;
        [SerializeField] private GameObject disappearFXPrefab;
        private EnemyControllerHumanoid ac;
        private GameObject _targetPlayer;
        
        
        private IEnumerator Start()
        {
            ac = GetComponent<EnemyControllerHumanoid>();
            _targetPlayer = BattleStageManager.Instance.GetPlayer();

            ac.TurnMove(_targetPlayer);
            BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + new Vector3(0,3));
            
            yield return new WaitForSeconds(0.75f);
            
            BattleEffectManager.Instance.SpawnTargetLockIndicator(_targetPlayer,2.5f);
            
            yield return new WaitForSeconds(0.75f);
            
            
            ac.anim.Play("s1");
            

            //yield return new WaitForSeconds(0.1f);
            
            Instantiate(muzzleFXPrefab, transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            
            yield return new WaitForSeconds(0.95f);

            var container = BattleStageManager.Instance.GetNewRangedContainer();
            container.GetComponent<AttackContainerEnemy>().InitAttackContainer(5);
            CallProjectile(container,_targetPlayer.transform.position.x);
            
            yield return new WaitForSeconds(0.15f);
            
            CallProjectile(container,_targetPlayer.transform.position.x);
            
            yield return new WaitForSeconds(0.15f);
            
            CallProjectile(container,_targetPlayer.transform.position.x);
            
            yield return new WaitForSeconds(0.15f);
            
            CallProjectile(container,_targetPlayer.transform.position.x);
            
            yield return new WaitForSeconds(0.15f);
            
            CallProjectile(container,_targetPlayer.transform.position.x);

            yield return new WaitForSeconds(1.5f);

            Instantiate(disappearFXPrefab, gameObject.RaycastedPosition(), Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            
            Destroy(gameObject,0.15f);
            


        }

        private void CallProjectile(GameObject container, float position, float delay = 0.2f)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                var proj = this.InstantiateRangedObject(projectileFXPrefab,
                    new Vector3(position, BattleStageManager.Instance.mapBorderB + 12),
                    container, 1, 1, this);
                proj.transform.rotation = projectileFXPrefab.transform.rotation;
            }, false);
        }
        
        
        
    }
}

