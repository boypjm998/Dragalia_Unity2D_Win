using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C005_4_Boss : MonoBehaviour
    {
        private GameObject powerUpFX;
        private GameObject powerDownFX;
        private Transform specialHintsFX;
        private int mode = 0;

        private TimerBuff scorchrendBuffPlayer;
        private TimerBuff scorchrendBuffBoss;

        private Collider2D selfCollider;
        
        private Dictionary<StatusManager, float> afflictionCooldownDict = new();
        private Dictionary<UI_HB001_Legend_01, float>specialGaugeDict = new();
        //20081: Level Down
        //20091: Level Up
        private StatusManager bossStatusManager;
        private StatusManager viewerStatusManager;

        public void SetHintsTargets(StatusManager bossStatusManager, StatusManager viewerStatusManager)
        {
            this.bossStatusManager = bossStatusManager;
            this.viewerStatusManager = viewerStatusManager;
            bossStatusManager.OnBuffEventDelegate += CheckSpecialHints;
            viewerStatusManager.OnBuffEventDelegate += CheckSpecialHints;
        }

        private void Awake()
        {
            powerDownFX = transform.Find("power_down").gameObject;
            powerUpFX = transform.Find("power_up").gameObject;
            specialHintsFX = transform.Find("special_hints");
            selfCollider = GetComponent<Collider2D>();
            scorchrendBuffPlayer = new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
                200, 21, 1);
            scorchrendBuffBoss = new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
                10000, 21, 1, 8101402);
            InitCurrentWorld();
        }

        private void Start()
        {
            BattleStageManager.Instance.OnFieldAbilityAdd += CheckCurrentWorld;
            BattleStageManager.Instance.OnFieldAbilityRemove += CheckWorldCleared;
            OperateSpecialHints();
        }

        private void OnDestroy()
        {
            BattleStageManager.Instance.OnFieldAbilityAdd -= CheckCurrentWorld;
            BattleStageManager.Instance.OnFieldAbilityRemove -= CheckWorldCleared;
        }
        

        void CheckCurrentWorld(int id)
        {
            ReCheckOnWorldReset();
            
            if (id == 20081)
            {
                powerDownFX.SetActive(true);
                powerUpFX.SetActive(false);
                mode = 1;
                OperateSpecialHints();
                //ReCheckOnWorldReset();
            }
            else if (id == 20091)
            {
                powerDownFX.SetActive(false);
                powerUpFX.SetActive(true);
                mode = 2;
                OperateSpecialHints();
                //ReCheckOnWorldReset();
            }
        }

        void InitCurrentWorld()
        {
            ReCheckOnWorldReset();
            if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20081))
            {
                powerDownFX.SetActive(true);
                powerUpFX.SetActive(false);
                
                mode = 1;
            }
            else if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20091))
            {
                powerDownFX.SetActive(false);
                powerUpFX.SetActive(true);
                //ReCheckOnWorldReset();
                mode = 2;
            }
            
        }

        void CheckWorldCleared(int id)
        {
            if (!BattleStageManager.Instance.FieldAbilityIDList.Contains(20081) &&
                !BattleStageManager.Instance.FieldAbilityIDList.Contains(20091))
            {
                powerDownFX.SetActive(false);
                powerUpFX.SetActive(false);
                mode = 0;
                OperateSpecialHints();
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.CompareTag("Enemy") || col.CompareTag("Player"))
            {
                CheckPlayerSpecialBuffOnExit(col);
                StatusManager statusManager = col.GetComponentInParent<StatusManager>();
                if (statusManager != null)
                {
                    afflictionCooldownDict[statusManager] = 0.0f;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") || col.CompareTag("Enemy"))
            {
                CheckPlayerSpecialBuffOnEnter(col);

            }
        }


        private void OnTriggerStay2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                CheckPlayerAfflictionWhileStaying(col);
                
            }
            
            if (col.gameObject.CompareTag("Enemy"))
            { 
                CheckEnemyAfflictionWhileStaying(col);
            }
            
            
        }

        private void CheckPlayerAfflictionWhileStaying(Collider2D col)
        {
            var statusManager = col.GetComponentInParent<StatusManager>();
                
            //print(col.gameObject.name);
            if(statusManager == null)
                return;
                
            if(statusManager.GetConditionTotalValue((int)BasicCalculation.BattleCondition.Scorchrend)>=
               scorchrendBuffPlayer.effect)
                return;
                
                
                

            float nextActionTime;
            if (!afflictionCooldownDict.TryGetValue(statusManager, out nextActionTime))
            {
                nextActionTime = 0.0f;
            }
            if (Time.time >= nextActionTime)
            {
                // statusManager.ObtainTimerBuff(new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
                // 100, 5, 1));
                scorchrendBuffPlayer.lastTime = scorchrendBuffPlayer.duration;
                //statusManager.ObtainTimerBuff(scorchrendBuffPlayer);
                var condflag = BattleStageManager.Instance.
                    ObtainAfflictionDirectlyWithCheck(statusManager,scorchrendBuffPlayer,120);

                if(condflag == 1)
                    afflictionCooldownDict[statusManager] = Time.time + 1f;
                else
                {
                    afflictionCooldownDict[statusManager] = Time.time + 3f;
                }
            }
        }

        private void CheckEnemyAfflictionWhileStaying(Collider2D col)
        {
            var statusManager = col.GetComponentInParent<StatusManager>();
                
            if(statusManager == null)
                return;
                
            if(statusManager.GetConditionWithSpecialID(8101402).Count>0)
                return;
                
            scorchrendBuffBoss.lastTime = scorchrendBuffBoss.duration;
            //statusManager.ObtainTimerBuff(scorchrendBuffPlayer);
                
                
            float nextActionTime;
            if (!afflictionCooldownDict.TryGetValue(statusManager, out nextActionTime))
            {
                nextActionTime = 0.0f;
            }
            if (Time.time >= nextActionTime)
            {
                // statusManager.ObtainTimerBuff(new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
                // 100, 5, 1));
                scorchrendBuffBoss.lastTime = scorchrendBuffBoss.duration;
                //statusManager.ObtainTimerBuff(scorchrendBuffPlayer);
                var condflag = BattleStageManager.Instance.
                    ObtainAfflictionDirectlyWithCheck(statusManager,scorchrendBuffBoss,120);

                afflictionCooldownDict[statusManager] = Time.time + 3f;
            }
        }


        private void CheckPlayerSpecialBuffOnEnter(Collider2D col,bool onStay = false)
        {
            UI_HB001_Legend_01 ui;
            try
            {
                ui = col.transform.parent.Find("BuffLayer").GetComponentInChildren<UI_HB001_Legend_01>();
            }
            catch
            {
                Debug.LogWarning("Not found UI_HB001_Legend_01");
                return;
            }
            float nextBuffOperationTime;
            if(!onStay)
                ui.TriggerIncrement();
            else if (ui.colliderWithAreaNum > 0)
            {
                ui.StartTicking();
            }

            // if(specialGaugeDict.TryGetValue(ui, out nextBuffOperationTime))
            // {
            //     switch (ui.level)
            //     {
            //         case 1:
            //         {
            //             nextBuffOperationTime = Time.time;
            //             break;
            //         }
            //         case 2:
            //         {
            //             nextBuffOperationTime = Time.time + 5f;
            //             break;
            //         }
            //         case 3:
            //         {
            //             nextBuffOperationTime = Time.time + 15f;
            //             break;
            //         }
            //         default:
            //         {
            //             nextBuffOperationTime = Time.time + 30f;
            //             break;
            //         }
            //     }
            // }
            //
            // if (Time.time >= nextBuffOperationTime)
            // {
            //     switch (ui.level)
            //     {
            //         case 1:
            //         {
            //             nextBuffOperationTime = Time.time + 5f;
            //             ui.level = 2;
            //             break;
            //         }
            //         case 2:
            //         {
            //             nextBuffOperationTime = Time.time + 15f;
            //             ui.level = 3;
            //             break;
            //         }
            //         default:
            //         {
            //             nextBuffOperationTime = Time.time + 30f;
            //             break;
            //         }
            //     }
            // }
            
        }
        
        private void CheckPlayerSpecialBuffOnExit(Collider2D col)
        {
            UI_HB001_Legend_01 ui;
            try
            {
                ui = col.transform.parent.Find("BuffLayer").GetComponentInChildren<UI_HB001_Legend_01>();
            }
            catch
            {
                Debug.LogWarning("Not found UI_HB001_Legend_01");
                return;
            }
            float nextBuffOperationTime;
            ui.TriggerDecrement();
            
            
        }

        public void ReCheckOnWorldReset()
        {
            List<Collider2D> colliders = new List<Collider2D>();
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.NoFilter();
            selfCollider.OverlapCollider(contactFilter, colliders);

            // 对标签为"Player"的碰撞体执行DoAction方法
            foreach (Collider2D col in colliders)
            {
                if (col.CompareTag("Player") || col.CompareTag("Enemy"))
                {
                    CheckPlayerSpecialBuffOnEnter(col,true);
                }
            }
            
        }

        private void CheckSpecialHints(BattleCondition condition)
        {
            if(condition.buffID != (int) BasicCalculation.BattleCondition.ScorchingEnergy)
                return;

            OperateSpecialHints();



        }

        public void OperateSpecialHints()
        {
            var floatingParticles = FindObjectOfType<Projectile_C005_7_Boss>();

            if (floatingParticles == null)
            {
                specialHintsFX.GetChild(0).gameObject.SetActive(false);
                specialHintsFX.GetChild(1).gameObject.SetActive(false);
                return;
            }

            float difference =
                bossStatusManager.GetConditionTotalValue((int)BasicCalculation.BattleCondition.ScorchingEnergy) -
                viewerStatusManager.GetConditionTotalValue((int)BasicCalculation.BattleCondition.ScorchingEnergy);
            

            //difference > 0: boss > player
            //difference < 0: player > boss
            
            
            
            if (mode == 0)
            {
                specialHintsFX.GetChild(0).gameObject.SetActive(false);
                specialHintsFX.GetChild(1).gameObject.SetActive(false);
            }else if (mode == 1)
            {
                //驱散规则
                
                //当AOE惩罚buff等级较高的目标时，如果玩家的buff层数大于boss，则显示感叹号特效。
                if (floatingParticles.upward > 0 && difference <= 0)
                {
                    specialHintsFX.GetChild(0).gameObject.SetActive(true);
                    specialHintsFX.GetChild(1).gameObject.SetActive(false);
                }
                //当AOE惩罚buff等级较高的目标时，如果玩家的buff层数小于等于boss，什么都不显示。
                else if(floatingParticles.upward > 0 && difference > 0)
                {
                    specialHintsFX.GetChild(0).gameObject.SetActive(false);
                    specialHintsFX.GetChild(1).gameObject.SetActive(false);
                }
                //当AOE惩罚buff等级较低的目标时，不论buff层数，都显示禁止进入特效。
                else if(floatingParticles.upward < 0)
                {
                    specialHintsFX.GetChild(0).gameObject.SetActive(false);
                    specialHintsFX.GetChild(1).gameObject.SetActive(true);
                }

            }else if (mode == 2)
            {
                //增益规则
                
                //当AOE惩罚buff等级较高的目标时，无论玩家的buff层数如何，都显示禁止进入特效。
                if (floatingParticles.upward > 0)
                {
                    specialHintsFX.GetChild(0).gameObject.SetActive(false);
                    specialHintsFX.GetChild(1).gameObject.SetActive(true);
                }
                //当AOE惩罚buff等级较低的目标时，如果玩家的buff层数小于等于boss，则显示感叹号特效。
                else if(floatingParticles.upward < 0 && difference >= 0)
                {
                    specialHintsFX.GetChild(0).gameObject.SetActive(true);
                    specialHintsFX.GetChild(1).gameObject.SetActive(false);
                }
                //当AOE惩罚buff等级较低的目标时，如果玩家的buff层数大于boss，什么都不显示。
                else if(floatingParticles.upward < 0 && difference < 0)
                {
                    specialHintsFX.GetChild(0).gameObject.SetActive(false);
                    specialHintsFX.GetChild(1).gameObject.SetActive(false);
                }
            }
        }

    }

}
