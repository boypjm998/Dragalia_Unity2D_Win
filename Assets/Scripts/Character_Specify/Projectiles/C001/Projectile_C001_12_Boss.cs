using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Shadow Alberius
    /// </summary>
    public class Projectile_C001_12_Boss : MonoSingleton<Projectile_C001_12_Boss>
    {
        [SerializeField] private GameObject skillPrefab;

        private StatusManager _status;
        private EnemyControllerHumanoid _ac;
        private GameObject _targetPlayer;

        public void SetTarget(GameObject target)
        {
            _targetPlayer = target;
        }
        

        private IEnumerator Start()
        {
            if (_targetPlayer == null)
            {
                _targetPlayer = BattleStageManager.Instance.GetPlayer();
            }
            _ac = GetComponent<EnemyControllerHumanoid>();
            _status = GetComponent<StatusManager>();
            var taunt = new TimerBuff((int)BasicCalculation.BattleCondition.Taunt, 1, -1, 1);
            taunt.dispellable = false;
            var kbimmune = new TimerBuff((int)BasicCalculation.BattleCondition.KnockBackImmune, 1, -1, 1);
            kbimmune.dispellable = false;
            _status.ObtainTimerBuff(taunt);
            _status.ObtainTimerBuff(kbimmune);
            
            BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + new Vector3(0,3));
            

            yield return new WaitForSeconds(1f);

            yield return _ac.MoveTowardTargetOnGround(_targetPlayer, 5, 6, 20, 8);

            _ac.TurnMove(_targetPlayer);
            
            yield return null;

            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(_ac, transform.position + new Vector3(-3*_ac.facedir, 2),
                BattleStageManager.Instance.RangedAttackFXLayer.transform, new Vector2(14, 8), Vector2.zero,
                false, 0, 1.5f, _ac.facedir == 1?0:180, 0.5f, true, true, true);

            yield return new WaitForSeconds(0.8f);
            
            _ac.anim.Play("skill_enter");

            yield return new WaitForSeconds(0.8f);

            this.InstantiateRangedObject(skillPrefab, transform.position,
                Instantiate(BattleStageManager.Instance.attackContainerEnemy, transform.position, Quaternion.identity,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform), _ac.facedir, 1, _ac)
                .GetComponent<AttackFromEnemy>().AddWithConditionAll
                    (new TimerBuff((int)BasicCalculation.BattleCondition.ShadowBlight,72,21,1),110);

            yield return new WaitForSeconds(2f);
            
            
            //再来一次
            
            
            yield return _ac.MoveTowardTargetOnGround(_targetPlayer, 5, 6, 10, 8);
            _ac.TurnMove(_targetPlayer);
            
            yield return null;
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(_ac, transform.position + new Vector3(-3*_ac.facedir, 2),
                BattleStageManager.Instance.RangedAttackFXLayer.transform, new Vector2(14, 8), Vector2.zero,
                false, 0, 1.5f, _ac.facedir == 1?0:180, 0.5f, true, true, true);

            yield return new WaitForSeconds(0.8f);
            
            _ac.anim.Play("skill_enter");

            yield return new WaitForSeconds(0.8f);

            this.InstantiateRangedObject(skillPrefab, transform.position,
                    Instantiate(BattleStageManager.Instance.attackContainerEnemy, transform.position, Quaternion.identity,
                        BattleStageManager.Instance.RangedAttackFXLayer.transform), _ac.facedir, 1, _ac)
                .GetComponent<AttackFromEnemy>().AddWithConditionAll
                    (new TimerBuff((int)BasicCalculation.BattleCondition.ShadowBlight,72,21,1),110);

            yield return new WaitForSeconds(2f);
            
            Destroy(gameObject,0.1f);
            gameObject.SetActive(false);

        }
        
        
    }
}

