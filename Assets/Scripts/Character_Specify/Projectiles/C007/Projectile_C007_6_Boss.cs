using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C007_6_Boss : MonoBehaviour,IEnemySealedContainer
    {

        public static int totalNum = 0;
        
        [SerializeField] private GameObject blastFX;
        [SerializeField] private GameObject magicCircleFX;
        [SerializeField] private float lifeTime = 20;

        public int difficulty = 2;

        private AttackFromEnemy _attackFromEnemy;
        private GameObject _enemySource;
        private StatusManager _attachedTargetStat;
        private bool _hasTriggered = false;


        private void Awake()
        {
            totalNum++;
            if(totalNum > 1)
                Destroy(gameObject);
        }

        private IEnumerator Start()
        {
            _attackFromEnemy = blastFX.GetComponent<AttackFromEnemy>();
            _attackFromEnemy.enemySource = _enemySource;

            var caf = new ConditionalAttackEffect
            (ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.Custom,
                new string[] { "1", ((int)BasicCalculation.BattleCondition.Freeze).ToString() },
                new string[] { }).
                SetEffectFunction(
                    (stats,atkStat) =>
                    {
                        stats.targetStat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.Nihility,
                            1, 30, 1,-1);
                        stats.targetStat.RemoveTimerBuff((int)BasicCalculation.BattleCondition.Freeze, true);
                        return difficulty * 500 + 499;
                    });
            
            _attackFromEnemy.AddConditionalAttackEffect(caf);


            if (_attachedTargetStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.Freeze) > 0)
            {
                ActiveAttack();
            }
            

            _attachedTargetStat.OnBuffEventDelegate += CheckCondition;

            yield return new WaitForSeconds(lifeTime);
            
            if(!_hasTriggered)
                Destroy(gameObject);

        }

        private void OnDestroy()
        {
            totalNum--;
            _attachedTargetStat.OnBuffEventDelegate -= CheckCondition;
        }

        public void SetTarget(GameObject target)
        {
            _attachedTargetStat = target.GetComponent<StatusManager>();
        }

        public void SetEnemySource(GameObject source)
        {
            _enemySource = source;
        }

        private void ActiveAttack()
        {
            GetComponent<RelativePositionRetainer>().enabled = false;
            _hasTriggered = true;
            blastFX.SetActive(true);
            magicCircleFX.SetActive(false);
            Destroy(gameObject,2f);
        }

        private void CheckCondition(BattleCondition condition)
        {
            if (_hasTriggered)
                return;

            if (condition.buffID == (int)BasicCalculation.BattleCondition.Freeze)
            {
                ActiveAttack();
            }
            
        }
        
        
    }

}
