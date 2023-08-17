using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C005_8_Boss : MonoBehaviour
    {

        
        public GameObject bossObject;
        public int damageCount = 15;
        public float awakeTime = 0.05f;
        
        
        
        [SerializeField] private float interval = 0.12f;
        private SpecialStatusManager bossStat;
        private List<StatusManager> playerStats = new();
        private List<GameObject> playerList = new();
        private List<int> punishmentList;
        private int bossPunishmentLevel;
        private bool moreIsSafe = false;

        public int Rule {
            get
            {
                if (moreIsSafe)
                    return 1;
                else return 0;
            }
            set
            {
                if(value > 0)
                    moreIsSafe = true;
                if(value < 0)
                    moreIsSafe = false;
            }
        }

        private void Awake()
        {
            playerList = DragaliaEnemyBehavior.GetPlayerList();
            punishmentList = new List<int>();
            foreach (var plr in playerList)
            {
                playerStats.Add(plr.GetComponent<StatusManager>());
            }
            
        }


        private IEnumerator Start()
        {
            bossStat = bossObject.GetComponent<SpecialStatusManager>();
            ParsePunishmentList();
            yield return new WaitForSeconds(awakeTime);
            for (int i = 0; i < damageCount; i++)
            {

                for (int j = 0 ; j < playerStats.Count ; j++)
                {
                    CauseDamageToPlayerInstantly(playerStats[j], punishmentList[j]);
                }
                CauseDamageToBossInstantly(bossStat);
                
                CineMachineOperator.Instance.CamaraShake(8,0.1f);

                yield return new WaitForSeconds(interval);

            }
            
            Destroy(gameObject,1f);
        }

        void ParsePunishmentList()
        {
            int bossScorchingLevel =
                (int)bossStat.GetConditionTotalValue((int)BasicCalculation.BattleCondition.ScorchingEnergy);
            
            foreach (var playerStat in playerStats)
            {
                int playerScorchingLevel =
                    (int)playerStat.GetConditionTotalValue((int)BasicCalculation.BattleCondition.ScorchingEnergy);
                
                if (moreIsSafe)
                {
                    bossPunishmentLevel += (playerScorchingLevel - bossScorchingLevel);
                }
                else
                {
                    bossPunishmentLevel += (bossScorchingLevel - playerScorchingLevel);
                }
                punishmentList.Add(-bossPunishmentLevel);

            }
        }



        void CauseDamageToPlayerInstantly(StatusManager target,int punishmentLevel)
        {
            if(punishmentLevel< 0)
                punishmentLevel = 0;
            var damage = (int)(target.maxHP * (0.01f + punishmentLevel * 0.06f)) - (int)target.GetDamageCutConst();
            if(damage < 0)
                damage = 0;
            
            BattleStageManager.Instance.CauseIndirectDamage(target,
                damage,
                true,false);
            
            
        }
        void CauseDamageToBossInstantly(SpecialStatusManager target)
        {
            var punishmentLevel = bossPunishmentLevel;
            if (punishmentLevel <= 0)
                punishmentLevel = 0;
            else punishmentLevel = 1;

            var damageTemp = (99 + punishmentLevel*(int)(target.maxHP * 0.01f) - target.GetDamageCutConst());
            
            if(damageTemp < 0)
                damageTemp = 0;
            
            var damage = BattleStageManager.Instance.CauseIndirectDamage(target,
            (int)damageTemp,
                false,true);

            BattleStageManager.Instance.CauseIndirectDamageToOverdriveBar(target,
                damage * (1+punishmentLevel) + 99,
                false);

        }
        
    }

}
