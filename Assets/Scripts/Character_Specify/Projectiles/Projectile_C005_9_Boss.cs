using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// FireSword : inheritor of Blazewolf
    /// </summary>
    public class Projectile_C005_9_Boss : EnemyMoveManager,IEnemySealedContainer
    {
        private GameObject enemySource;
        [SerializeField] private GameObject swordAura;
        [SerializeField] private GameObject crossAttack1;
        [SerializeField] private GameObject crossAttack2;
        [SerializeField] private GameObject crossAttack3;
        [SerializeField] private GameObject crossAttack4;

        public void SetEnemySource(GameObject source)
        {
            enemySource = source;
            
        }
        protected override void Awake()
        {
            MeeleAttackFXLayer = gameObject;
            RangedAttackFXLayer = BattleStageManager.Instance.RangedAttackFXLayer;


        }

        protected override void Start()
        {
            swordAura.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            CopyProjectilesToPool();
        }

        public void ReleaseAttack()
        {
            StartCoroutine(StartCrossAttack());
        }

        public IEnumerator StartCrossAttack()
        {
            var hintbar1 = 
                Instantiate(WarningPrefabs[0], MeeleAttackFXLayer.transform);
            
            var hintbarTimer1 = hintbar1.GetComponentsInChildren<EnemyAttackHintBar>();

            var warningTime = hintbarTimer1[0].warningTime;
            
            foreach (var timer in hintbarTimer1)
            {
                timer.warningTime -= 1;
            }
            
            
            yield return new WaitForSeconds(warningTime-1);
            
            crossAttack1.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            crossAttack1.SetActive(true);
            
            var hintbar2 = 
                Instantiate(WarningPrefabs[1], MeeleAttackFXLayer.transform);
            
            yield return new WaitForSeconds(warningTime);
            
            crossAttack2.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            crossAttack2.SetActive(true);
            
            var hintbar3 = 
                Instantiate(WarningPrefabs[0], MeeleAttackFXLayer.transform);
            
            yield return new WaitForSeconds(warningTime);
            
            
            crossAttack3.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            crossAttack3.SetActive(true);
            
            var hintbar4 = 
                Instantiate(WarningPrefabs[1],MeeleAttackFXLayer.transform);
            
            yield return new WaitForSeconds(warningTime);
            

            crossAttack4.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            crossAttack4.SetActive(true);
            
            Destroy(gameObject,1f);


        }

    }

}
