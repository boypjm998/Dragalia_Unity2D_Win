using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Fritz's shadow
    /// </summary>
    public class Projectile_DB004_2 : MonoSingleton<Projectile_DB004_2>
    {
        private EnemyControllerHumanoid ac;
        [SerializeField] private float forcingTime = 3;
        [SerializeField] private GameObject forcingPrefab;
        [SerializeField] private GameObject slashPrefab;
        [SerializeField] private GameObject daggerProjectilePrefab;

        private IEnumerator Start()
        {
            ac = GetComponent<EnemyControllerHumanoid>();

            yield return new WaitForSeconds(1f);
            
            Instantiate(forcingPrefab,transform.position,Quaternion.identity,transform);
            
            var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            container.GetComponent<AttackContainerEnemy>().InitAttackContainer(9);
            
            GenerateWaringPrefabs();
            ac.anim.SetTrigger("action");
            
            yield return new WaitForSeconds(forcingTime - 0.2f);
            
            ac.anim.SetTrigger("action");
            
            yield return new WaitForSeconds(0.2f);
            
            SlashAttack(container);
            
            yield return new WaitForSeconds(0.25f);
            
            ProjectileAttack(container);
            
            yield return new WaitForSeconds(2f);
            
            Destroy(gameObject);


        }

        private void GenerateWaringPrefabs()
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                transform.position + new Vector3(0,-2.5f), BattleStageManager.Instance.RangedAttackFXLayer.transform,
                new Vector2(4, 8), Vector2.zero, false, 1, forcingTime,
                90, 0.5f, true);

            for (int i = 0; i < 8; i++)
            {
                EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
                    transform.position, BattleStageManager.Instance.RangedAttackFXLayer.transform,
                    new Vector2(20,1),Vector2.zero, true,0,
                    forcingTime+0.25f,
                    45 * i, 0.5f, true);
            }

        }

        private void SlashAttack(GameObject container)
        {
            

            this.InstantiateRangedObject(slashPrefab, transform.position, container, ac.facedir);



        }

        private void ProjectileAttack(GameObject container)
        {
            TimerBuff stunAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Stun, 1, 3, 1);
            
            ac.SwapWeaponVisibility(false);
            
            for (int i = 0; i < 8; i++)
            {
                var projectile = this.InstantiateRangedObject(daggerProjectilePrefab,
                    transform.position, container, i < 4 ? 1 : -1);
                
                //让投掷物按照一定的角度飞出20个单位

                var rigid = projectile.GetComponent<Rigidbody2D>();
                
                projectile.GetComponent<AttackFromEnemy>().AddWithConditionAll(stunAffliction,80);
                
                var angle = i * 45 * Mathf.Deg2Rad;
                
                rigid.DOMove(new Vector2
                        (transform.position.x + 20 * Mathf.Cos(angle),
                            transform.position.y + 20 * Mathf.Sin(angle)),
                    0.7f);

            }
        }
        
        
        
        
        
    }

}
