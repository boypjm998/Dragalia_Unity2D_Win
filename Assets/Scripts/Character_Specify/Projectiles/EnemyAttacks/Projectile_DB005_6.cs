using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Shadow Linnea
    /// </summary>
    public class Projectile_DB005_6 : MonoSingleton<Projectile_DB005_6>
    {
        [SerializeField] private GameObject forcingFXPrefab;
        [SerializeField] private GameObject attackFXPrefab;
        [SerializeField] private GameObject disappearFXPrefab;
        private GameObject forcingFXInstance;
        private EnemyControllerHumanoid ac;
        
        private GameObject _targetPlayer;

        public void SetTarget(GameObject target)
        {
            _targetPlayer = target;
        }

        private IEnumerator Start()
        {
            ac = GetComponent<EnemyControllerHumanoid>();
            BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + new Vector3(0,3));
            yield return new WaitForSeconds(1);
            
            
            yield return ac.MoveTowardTargetOnGround(_targetPlayer, 5, 4, 9999, 6);

            ac.TurnMove(_targetPlayer);
            ac.anim.Play("forcing_enter");
            forcingFXInstance = Instantiate(forcingFXPrefab, gameObject.RaycastedPosition(), Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);

            yield return new WaitForSeconds(2.5f);
            
            ac.anim.Play("forcing_exit");
            
            yield return new WaitForSeconds(0.25f);

            this.InstantiateRangedObject(attackFXPrefab, forcingFXInstance.transform.position,
                Instantiate(BattleStageManager.Instance.attackContainerEnemy, Vector3.zero, Quaternion.identity,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform), 1, 1, ac);
            Destroy(forcingFXInstance);
            
            yield return new WaitForSeconds(1.75f);
            
            Instantiate(disappearFXPrefab, gameObject.RaycastedPosition(), Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            yield return new WaitForSeconds(0.15f);
            
            ac.DisappearRenderer();
            ac.SwapWeaponVisibility(false);
            
            Destroy(gameObject,0.35f);
            


        }
        
        
    }
}

