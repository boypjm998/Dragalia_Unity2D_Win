using System;
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
        private AttackFromEnemy attackFromEnemy;
        private Projectile_C003_14_Boss protection;
        void Start()
        {
            anim = GetComponent<Animator>();
            attackFromEnemy = GetComponent<AttackFromEnemy>();
            attackFromEnemy.BeforeAttackHit += EnemyMoveManager.PurgedShapeShiftingOfTarget;
            protection = 
                GameObject.Find("AttackFXPlayer").GetComponentInChildren<Projectile_C003_14_Boss>();
        }

        // Update is called once per frame
        public void SetEnemySource(GameObject src)
        {
            attackFromEnemy.enemySource = src;
        }

        private void OnDestroy()
        {
            attackFromEnemy.BeforeAttackHit -= EnemyMoveManager.PurgedShapeShiftingOfTarget;
        }

        public void DoLaserAnimation(int dir,int order)
        {
            attackFromEnemy.NextAttack();
            protection?.WakeProtectionZone(order);

            if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20033))
            {
                attackFromEnemy.attackInfo[0].dmgModifier[0] = 5.33f * 1.2f;
            }
            else if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20034))
            {
                attackFromEnemy.attackInfo[0].dmgModifier[0] = 5.33f * 4;
            }
            else
            {
                attackFromEnemy.attackInfo[0].dmgModifier[0] = 5.33f * 2;
            }

            if (dir == 1)
            {
                anim.Play("from_left");
                attackFromEnemy.firedir = 1;
            }
            else if (dir == -1)
            {
                anim.Play("from_right");
                attackFromEnemy.firedir = -1;
            }

        }
    }
}

