using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    ///Glorious Sanctuary: BuffZone
    public class Projectile_C003_10_Boss : MonoBehaviour
    {
        public BasicCalculation.BattleCondition buffType;
        public float buffEffect;
        public float buffDuration;

        private BattleCondition buff;
        public int spID = 8102401;

        private GameObject effect;
        private bool isUsed = false;
        public bool IsUsed => isUsed;
        private void Start()
        {
            buff = new TimerBuff((int)buffType, buffEffect, buffDuration, 100, spID);
            buff.dispellable = false;
            effect = transform.Find("laser").gameObject;
        }

        public void RecycleBuff(StatusManager statusManager)
        {
            statusManager.ObtainTimerBuff(buff);
            isUsed = true;
            effect.SetActive(true);
            Destroy(gameObject,0.5f);
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            if(isUsed || buff==null)
                return;
            
            if (col.CompareTag("Enemy"))
            {
                var ac = col.GetComponentInParent<EnemyController>();
                if(ac==null)
                    return;
                if (ac.hurt || !ac.grounded)
                {
                    return;
                }

                var enemyStat = col.GetComponentInParent<StatusManager>();
                
                if (buff.buffID == (int)BasicCalculation.BattleCondition.HealOverTime)
                {
                    buff = new TimerBuff((int)buffType, buffEffect/20, buffDuration, 100, spID);
                }

                enemyStat.ObtainTimerBuff(buff);
                isUsed = true;
                effect.SetActive(true);
                Destroy(gameObject,0.5f);
                return;
            }

            if (col.CompareTag("Player"))
            {
                var ac = col.GetComponentInParent<ActorController>();
                if(ac==null)
                    return;
                if (ac.dodging || ac.hurt || !ac.grounded)
                {
                    return;
                }

                var playerStat = col.GetComponentInParent<StatusManager>();
                isUsed = true;
                if (playerStat.GetConditionWithSpecialID(spID).Count >= 3)
                {
                    playerStat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.ManaOverloaded,
                        -1,30,1,-1);
                }
                else
                {
                    playerStat.ObtainTimerBuff(buff);
                }
                effect.SetActive(true);
                Destroy(gameObject,0.8f);
            }
        }
    }
}


