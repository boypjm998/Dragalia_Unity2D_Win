using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 创世圣冠
    /// </summary>
    public class Projectile_C003_15_Boss : MonoBehaviour
    {
        private Animator anim;
        private AttackFromEnemy AttackFromEnemy;
        private Projectile_C003_14_Boss protection;
        void Start()
        {
            anim = GetComponent<Animator>();
            AttackFromEnemy = GetComponent<AttackFromEnemy>();
            protection = 
                GameObject.Find("AttackFXPlayer").GetComponentInChildren<Projectile_C003_14_Boss>();
        }

        // Update is called once per frame
        public void SetEnemySource(GameObject src)
        {
            AttackFromEnemy.enemySource = src;
        }

        public void DoLaserAnimation(int dir,int order)
        {
            AttackFromEnemy.NextAttack();
            protection?.WakeProtectionZone(order);

            if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20033))
            {
                AttackFromEnemy.attackInfo[0].dmgModifier[0] = 5.33f * 1.2f;
            }
            else if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20034))
            {
                AttackFromEnemy.attackInfo[0].dmgModifier[0] = 5.33f * 4;
            }
            else
            {
                AttackFromEnemy.attackInfo[0].dmgModifier[0] = 5.33f * 2;
            }

            if (dir == 1)
            {
                anim.Play("from_left");
                AttackFromEnemy.firedir = 1;
            }
            else if (dir == -1)
            {
                anim.Play("from_right");
                AttackFromEnemy.firedir = -1;
            }

        }
    }
}

