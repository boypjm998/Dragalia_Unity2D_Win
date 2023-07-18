using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 隐形风
    /// </summary>
    public class Projectile_DB001_5 : MonoBehaviour,IEnemySealedContainer
    {
        private GameObject enemySource;
        public int difficulty = 1;

        [SerializeField]private GameObject atkGo;
        // Start is called before the first frame update
        void Start()
        {
            var hintBar = GetComponentInChildren<EnemyAttackHintBarCircle>();
            Invoke(nameof(ReleaseAttack),hintBar.awakeTime + hintBar.warningTime);
            if(difficulty > 2)
                hintBar.gameObject.SetActive(false);
        }


        public void SetEnemySource(GameObject source)
        {
            enemySource = source;
        }

        public void ReleaseAttack()
        {
            atkGo.SetActive(true);
            atkGo.GetComponent<AttackFromEnemy>().enemySource = enemySource;

            var stormEffect = new TimerBuff((int)BasicCalculation.BattleCondition.Stormlash,
                72, 21, 1);
            
            atkGo.GetComponent<AttackFromEnemy>().AddWithConditionAll(stormEffect,100);
            
            
        }


    }
}

