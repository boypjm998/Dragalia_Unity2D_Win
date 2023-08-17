using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Ground Smash Of Combo7 for Boss HB001 Legend
    /// </summary>
    public class Projectile_C005_5_Boss : MonoBehaviour
    {
        AttackFromEnemy attackFromEnemy;
        public GameObject groundAttaching;
        [SerializeField] private GameObject hitEffect;
        
        public float groundSmashTime;
        private List<GameObject> playerList = new();

        private void Awake()
        {
            attackFromEnemy = GetComponent<AttackFromEnemy>();
            playerList = DragaliaEnemyBehavior.GetPlayerList();
            Invoke("HitOnGround", groundSmashTime);
        }

        void HitOnGround()
        {
            foreach (var player in playerList)
            {
                if (player.GetComponentInChildren<IGroundSensable>().GetCurrentAttachedGroundInfo() ==
                    groundAttaching)
                {
                    var hitSensor = player.GetComponent<ActorBase>().HitSensor;
                    try
                    {
                        attackFromEnemy.CauseDamage(hitSensor);
                        Instantiate(hitEffect, 
                            new Vector3(player.transform.position.x,
                                groundAttaching.transform.position.y+3), Quaternion.identity,
                            BattleStageManager.Instance.RangedAttackFXLayer.transform);
                    }
                    catch
                    {
                        print("Error Occurs");
                    }
                }

        }
            
            
        }

    }
}

