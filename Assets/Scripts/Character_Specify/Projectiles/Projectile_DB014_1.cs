using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_DB014_1 : MonoSingleton<Projectile_DB014_1>
    {
        [SerializeField] private GameObject waterfallPrefab;
        [SerializeField] private int extraDamage = 1500;

        private List<StatusManager> minions = new();
        private Coroutine waterfallRoutine;
        private GameObject hintBarInstance;

        public int CurrentMinionCount => minions.Count;
        public float prepareTime = 20f;
        public bool IsActive => waterfallRoutine != null;
        public GameObject enemySource;

        public void AddMinion(GameObject minion)
        {
            var stat = minion.GetComponent<StatusManager>();
            stat.OnReviveOrDeath += () => minions.Remove(stat);
            minions.Add(stat);
        }

        public void KillAllMinions()
        {
            for (int i = minions.Count - 1; i >=0 ; i--)
            {
                minions[i].currentHp = 0;
                minions[i].OnHPBelow0?.Invoke();
            }
        }

        public void SetExtraDamage(int dmg)
        {
            extraDamage = dmg;
        }

        public void StartWaterFall(float time = -1)
        {
            if (time > 0)
            {
                prepareTime = time;
            }
            if (IsActive == false)
            {
                waterfallRoutine = StartCoroutine(Waterfall());
            }
        }

        /// <summary>
        /// Run After Adding All Minions
        /// </summary>
        /// <returns></returns>
        private IEnumerator Waterfall()
        {
            hintBarInstance = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar
                (null, new Vector3(0,BattleStageManager.Instance.mapBorderT),
                    BattleStageManager.Instance.RangedAttackFXLayer.transform,
                    new Vector2(BattleStageManager.Instance.mapBorderT - BattleStageManager.Instance.mapBorderB,
                        BattleStageManager.Instance.mapBorderR - BattleStageManager.Instance.mapBorderL),
                    Vector2.zero, false,0,prepareTime,-90,0.8f,
                    true,false);

            float timer = 0;

            while (timer < prepareTime)
            {
                timer += Time.deltaTime;

                if (CurrentMinionCount <= 0)
                {
                    if(hintBarInstance != null)
                        Destroy(hintBarInstance);
                    
                    waterfallRoutine = null;
                    yield break;
                }

                yield return null;
            }

            if (GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            {
                waterfallRoutine = null;
                yield break;
            }

            var container = gameObject.AddComponent<AttackContainerEnemy>();
            var count = CurrentMinionCount;
            KillAllMinions();
            var atkProj = this.InstantiateRangedObject(waterfallPrefab, new Vector3(0,BattleStageManager.Instance.mapBorderT),
                gameObject, 1, 1, enemySource.transform);
            atkProj.GetComponent<AttackFromEnemy>().AddWithConditionAll(
                new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,1,15,1),100);
            atkProj.GetComponent<AttackFromEnemy>().attackInfo[0].constDmg[0] = count * extraDamage;

            waterfallRoutine = null;
            
        }

        
    }

}
