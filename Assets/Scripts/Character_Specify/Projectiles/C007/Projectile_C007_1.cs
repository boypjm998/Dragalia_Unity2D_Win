using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C007_1 : MonoBehaviour
    {
        [SerializeField] private bool isEnemy = false;
        [SerializeField] private GameObject hitFX;
        [SerializeField] private float triggerTime = 1;



        private GameObject _source;
        private Collider2D _attackCollider;
        private List<StatusManager> _statusManagers = new();
        ///type=1:AttackfromPlayer; type=2:AttackfromEnemy
        private int _type = 1;

        public void SetSource(GameObject src)
        {
            _source = src;
        }
        private IEnumerator Start()
        {
            if (isEnemy)
                _type = 2;

            yield return new WaitForSeconds(triggerTime);
            
            TriggerAttack();
            
        }

        private void TriggerAttack()
        {
            var container = Instantiate(new GameObject(),
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            
            container.AddComponent<AttackContainer>();
            
            foreach (var stat in _statusManagers)
            {
                var fx = Instantiate(hitFX, stat.transform.position, Quaternion.identity,
                    container.transform);
                
                if (isEnemy)
                {
                    var atk = fx.GetComponent<ForcedAttackFromEnemy>();
                    atk.enemySource = _source;
                    atk.target = (stat.gameObject);
                    atk.AddWithConditionAll(
                        new TimerBuff((int)BasicCalculation.BattleCondition.Bog
                        ,1,15,1),120,0);
                    atk.AddWithConditionAll(
                        new TimerBuff((int)BasicCalculation.BattleCondition.Freeze,
                            1,Random.Range(5f,7f),1),120,1);
                    atk.AddWithConditionAll(
                        new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,
                            41,21,1),120,2);
                }
                else
                {
                    var atk = fx.GetComponent<ForcedAttackFromPlayer>();
                    container.GetComponent<AttackContainer>().InitAttackContainer(1,true);
                    atk.playerpos = _source.transform;
                    atk.target = (stat.gameObject);
                    atk.AddWithConditionAll(
                        new TimerBuff((int)BasicCalculation.BattleCondition.Bog
                            ,1,15,1),110,0);
                    atk.AddWithConditionAll(
                        new TimerBuff((int)BasicCalculation.BattleCondition.Freeze,
                            1,Random.Range(5f,7f),1),110,1);
                    atk.AddWithConditionAll(
                        new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,
                            41,21,100),120,2);
                    
                    var checkConditionString = ((int)BasicCalculation.BattleCondition.Nihility).ToString();
                    atk.AddConditionalAttackEffect(
                        new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
                            ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                            new string[] {"1", checkConditionString},
                            new string[] {"0.5"})
                    );
                }
                
            }
            
            
            
            
            
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isEnemy && other.CompareTag("Enemy"))
            {
                var stat = other.GetComponentInParent<StatusManager>();
                if(stat != null)
                    _statusManagers.Add(stat);
            }else if (isEnemy && other.CompareTag("Player"))
            {
                var stat = other.GetComponentInParent<StatusManager>();
                if(stat != null)
                    _statusManagers.Add(stat);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!isEnemy && other.CompareTag("Enemy"))
            {
                var stat = other.GetComponentInParent<StatusManager>();
                if(stat != null)
                    _statusManagers.Remove(stat);
            }else if (isEnemy && other.CompareTag("Player"))
            {
                var stat = other.GetComponentInParent<StatusManager>();
                if(stat != null)
                    _statusManagers.Remove(stat);
            }
        }
    }
}

