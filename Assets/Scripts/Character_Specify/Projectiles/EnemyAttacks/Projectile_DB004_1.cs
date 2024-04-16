using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Zardin's shadow
    /// </summary>
    public class Projectile_DB004_1 : MonoSingleton<Projectile_DB004_1>
    {
        [SerializeField] private GameObject slashPrefab;
        [SerializeField] private GameObject warpPrefab;
        [SerializeField] private float intervalBefore = 3;
        [SerializeField] private float intervalAfter = 5;
        [SerializeField] private int slashTimeTotal = 3;

        public GameObject targetPlayer;
        
        private EnemyControllerHumanoid ac;
        protected override void Awake()
        {
            base.Awake();
            ac = GetComponent<EnemyControllerHumanoid>();
        }

        private IEnumerator Start()
        {
            
            
            for (int i = 0; i < slashTimeTotal; i++)
            {
                yield return new WaitForSeconds(intervalBefore);

                if (Mathf.Abs(transform.position.x - targetPlayer.transform.position.x) > 4 ||
                    gameObject.RaycastedPlatform() != targetPlayer.RaycastedPlatform())
                {
                    InstantiateWarpFX();
                    yield return new WaitForSeconds(0.1f);
                    ac.DisappearRenderer();
                    transform.position = WarpToTargetPosition();
                    InstantiateWarpFX();
                    yield return new WaitForSeconds(0.1f);
                    ac.AppearRenderer();
                }
                
                BattleEffectManager.Instance.SpawnExclamation(gameObject, 
                    transform.position + new Vector3(0, 2.5f),true);
                
                ac.TurnMove(targetPlayer);

                EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position,
                    transform.Find("MeeleAttackFX"),
                    new Vector2(5, 4), Vector2.zero, true, 0, 2, 0);
                yield return new WaitForSeconds(2);
                ac.anim.SetTrigger("action");
                yield return new WaitForSeconds(0.2f);
                InstantiateSlashFX();

                if(i != slashTimeTotal)
                    yield return new WaitForSeconds(intervalAfter);
            }
            
            Destroy(gameObject);
        }

        private Vector2 WarpToTargetPosition()
        {
            var platform = targetPlayer.RaycastedPlatform();
            var facedir = targetPlayer.transform.localScale.x;

            Vector2 position = new Vector2(targetPlayer.transform.position.x - facedir * 2.5f
                , platform.bounds.max.y);
            
            position.x = Mathf.Clamp(position.x, platform.bounds.min.x, platform.bounds.max.x);

            position.y += 1.3f;

            return position;

        }

        private void InstantiateWarpFX()
        {
            Instantiate(warpPrefab, transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
        }


        private void InstantiateSlashFX()
        {
            var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            var go =
                this.InstantiateRangedObject(slashPrefab, 
                    transform.position, container, ac.facedir,1,transform);
            
            go.GetComponent<AttackFromEnemy>().AddWithConditionAll(new TimerBuff
                ((int)BasicCalculation.BattleCondition.Stun,1,5,1),80);
            
        }
        
        
        
    }

}
