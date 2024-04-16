using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_DB004_4 : MonoSingleton<Projectile_DB004_4>
    {
        [SerializeField] private GameObject hintPrefabRed;
        [SerializeField] private GameObject hintPrefabPurple;
        [SerializeField] private GameObject skillPrefabRed;
        [SerializeField] private GameObject skillPrefabPurple;
        private EnemyControllerHumanoid ac;
        private GameObject container;

        private TimerBuff resDownDebuff = 
            new((int)BasicCalculation.BattleCondition.Vulnerable, 30, 30, 100);

        private IEnumerator Start()
        {
            ac = GetComponent<EnemyControllerHumanoid>();
            
            yield return new WaitForSeconds(0.5f);
            
            container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
                transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            
            container.GetComponent<AttackContainerEnemy>().InitAttackContainer(3);
            
            
            yield return new WaitForSeconds(0.66f);
            
            GenerateHintPrefab(hintPrefabRed);
            
            DOVirtual.DelayedCall(2f, () =>
            {
                GenerateAttackPrefab(skillPrefabRed);
            },false);
            
            yield return new WaitForSeconds(0.76f);
            
            //1.33f
            DOVirtual.DelayedCall(0.55f, () => ac.anim.SetTrigger("action"), false);
            
            //yield return new WaitForSeconds(0.66f);
            //1.43f
            
            //GenerateHintPrefab(skillPrefabRed);
            
            DOVirtual.DelayedCall(2f, () =>
            {
                GenerateAttackPrefab(skillPrefabRed);
            },false);

            yield return new WaitForSeconds(1.25f);
            //2.68f
            
            GenerateHintPrefab(hintPrefabPurple);
            
            DOVirtual.DelayedCall(2f, () =>
            {
                GenerateAttackPrefab(skillPrefabPurple);
            },false);

            yield return new WaitForSeconds(3.5f);
            
            Destroy(gameObject);

        }

        private void GenerateHintPrefab(GameObject prefab)
        {
            var hint = Instantiate(prefab, transform.position, Quaternion.identity, transform);

            var hintBars = hint.GetComponents<EnemyAttackHintBar>().ToList();
            
            hintBars.ForEach(x => x.SetAc(ac));

        }
        
        private void GenerateAttackPrefab(GameObject prefab)
        {
            var attack = this.InstantiateRangedObject(prefab, transform.position, 
                container, ac.facedir,0,transform);
            attack.GetComponent<AttackFromEnemy>().AddWithConditionAll(resDownDebuff,100);
            
        }
        
        
    }

}
