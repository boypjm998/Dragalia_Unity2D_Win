using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C001_14_Boss : MonoSingleton<Projectile_C001_14_Boss>
    {
        [SerializeField] private GameObject summonEffectPrefab;
        [SerializeField] private GameObject bahamutAttackPrefab;
        [SerializeField] private GameObject bahamutModelPrefab;
        
        private EnemyControllerHumanoid _ac;
        private StatusManager _statusManager;

        private IEnumerator Start()
        {
            _ac = GetComponent<EnemyControllerHumanoid>();
            _statusManager = GetComponent<StatusManager>();
            
            var taunt = new TimerBuff((int)BasicCalculation.BattleCondition.Taunt, 1, -1, 1);
            taunt.dispellable = false;
            var kbimmune = new TimerBuff((int)BasicCalculation.BattleCondition.KnockBackImmune, 1, -1, 1);
            kbimmune.dispellable = false;
            _statusManager.ObtainTimerBuff(taunt);
            _statusManager.ObtainTimerBuff(kbimmune);
            
            var target = BattleStageManager.Instance.GetPlayer();

            yield return new WaitForSeconds(2);
            
            _ac.anim.Play("transform");

            yield return new WaitForSeconds(0.5f);
            
            Instantiate(summonEffectPrefab, transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);

            yield return new WaitForSeconds(1f);

            var bhmt = 
                Instantiate(bahamutModelPrefab, new Vector3(0, 2), Quaternion.identity);

            if (target.transform.position.x < 0)
            {
                bhmt.transform.localScale = new Vector3(-1, 1, 1);
            }
            
            //bhmt.transform.GetChild(0).GetComponent<Animator>().Play("combo3");

            yield return new WaitForSeconds(2f);

            var raycastedPositionY = gameObject.RaycastedPosition().y;
            var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy, Vector3.zero,
                Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);

            if (Random.Range(0, 2) == 0)
            {
                BahamutAttackPillar(new Vector2(2,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(-2,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(-6,raycastedPositionY),container,true);
                BahamutAttackPillar(new Vector2(6,raycastedPositionY),container,true);
                BahamutAttackPillar(new Vector2(-14,raycastedPositionY),container,true);
                BahamutAttackPillar(new Vector2(14,raycastedPositionY),container,true);
                BahamutAttackPillar(new Vector2(-22,raycastedPositionY),container,true);
                BahamutAttackPillar(new Vector2(22,raycastedPositionY),container,true);
                // BahamutAttackPillar(new Vector2(-24,raycastedPositionY),container,true);
                // BahamutAttackPillar(new Vector2(24,raycastedPositionY),container,true);

                yield return new WaitForSeconds(0.75f);
            
                container = Instantiate(BattleStageManager.Instance.attackContainerEnemy, Vector3.zero,
                    Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
            
                BahamutAttackPillar(new Vector2(0,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(4,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(-4,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(-10,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(10,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(-18,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(18,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(-26,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(26,raycastedPositionY),container,false);
            }
            else
            {
                BahamutAttackPillar(new Vector2(0,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(4,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(-4,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(-10,raycastedPositionY),container,true);
                BahamutAttackPillar(new Vector2(10,raycastedPositionY),container,true);
                BahamutAttackPillar(new Vector2(-18,raycastedPositionY),container,true);
                BahamutAttackPillar(new Vector2(18,raycastedPositionY),container,true);
                BahamutAttackPillar(new Vector2(-26,raycastedPositionY),container,true);
                BahamutAttackPillar(new Vector2(26,raycastedPositionY),container,true);
                
                yield return new WaitForSeconds(0.6f);
            
                container = Instantiate(BattleStageManager.Instance.attackContainerEnemy, Vector3.zero,
                    Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
                
                
                BahamutAttackPillar(new Vector2(2,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(-2,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(-6,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(6,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(-14,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(14,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(-22,raycastedPositionY),container,false);
                BahamutAttackPillar(new Vector2(22,raycastedPositionY),container,false);
                // BahamutAttackPillar(new Vector2(-24,raycastedPositionY),container,true);
                // BahamutAttackPillar(new Vector2(24,raycastedPositionY),container,true);

                
            
                
            }
            
            
            
            // BahamutAttackPillar(new Vector2(-24,raycastedPositionY),container,false);
            // BahamutAttackPillar(new Vector2(24,raycastedPositionY),container,false);
            yield return new WaitForSeconds(3);
            Destroy(gameObject,0.1f);
            gameObject.SetActive(false);

        }

        private void BahamutAttackPillar(Vector2 position, GameObject container, bool avoidable)
        {
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(_ac, position,
                BattleStageManager.Instance.RangedAttackFXLayer.transform, new Vector3(20, 4), Vector2.zero,
                avoidable, 1,
                1f, 90);

            DOVirtual.DelayedCall(1f, () =>
            {
                var atk =
                    this.InstantiateRangedObject(bahamutAttackPrefab, position, container, 1, 1, _ac)
                        .GetComponent<AttackFromEnemy>();
                if (avoidable)
                {
                    atk.ChangeAvoidability(AttackFromEnemy.AvoidableProperty.Red);
                }
            }, false);


        }
        
    }
}

