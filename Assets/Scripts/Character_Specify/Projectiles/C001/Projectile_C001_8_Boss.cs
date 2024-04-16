using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 异界碎片
    /// </summary>
    public class Projectile_C001_8_Boss : MonoSingleton<Projectile_C001_8_Boss>
    {
        private bool _used = false;
        [SerializeField] private GameObject effect;
        [SerializeField] private float jumpForce = 40;
        private TimerBuff _jumpBoost = 
            new((int)BasicCalculation.BattleCondition.DoubleJumpWing,1,
                -1,1,8103402);

        public void InvokeDestroy(float time)
        {
            Destroy(gameObject,time);
        }

        private void Start()
        {
            _jumpBoost.dispellable = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(_used)
                return;


            var atk = other.GetComponent<AttackBase>();
            
            if(atk == null)
                return;

            
            
            if(atk is AttackFromPlayer)
                TriggerPlayer(atk as AttackFromPlayer);
            else if(atk is AttackFromEnemy)
                TriggerEnemy(atk as AttackFromEnemy);
            
            
        }

        private void TriggerPlayer(AttackFromPlayer atk)
        {
            if (atk.attackType != BasicCalculation.AttackType.DASH &&
                atk.attackType != BasicCalculation.AttackType.STANDARD)
            {
                return;
            }

            _used = true;
            
            if (atk.destroyAfterHit)
            {
                Destroy(atk.gameObject);
            }

            var hitEff = atk.hitConnectEffect;

            if (hitEff != null)
            {
                Instantiate(hitEff, transform.position, Quaternion.identity,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform);
            }
            
        
            CineMachineOperator.Instance.CamaraShake(atk.hitShakeIntensity, .1f);

            var playerAc = atk.playerpos.GetComponent<ActorController>();

            

            if (playerAc._statusManager.HasCondition((int)BasicCalculation.BattleCondition.DoubleJumpWing))
            {
                //playerAc._statusManager.ObtainTimerBuff((_jumpBoost));
                Destroy(gameObject);
                return;
            }
                
            
            playerAc._statusManager.ObtainTimerBuff((_jumpBoost));
            
            playerAc.BeforeJump += JumpEffect;
            playerAc.OnJump += ResetJumpEffect;




            Destroy(gameObject);

        }

        private void TriggerEnemy(AttackFromEnemy atk)
        {
            if (atk.attackType != BasicCalculation.AttackType.DASH &&
                atk.attackType != BasicCalculation.AttackType.STANDARD)
            {
                return;
            }

            _used = true;
            
            if (atk.destroyAfterHit)
            {
                Destroy(atk.gameObject);
            }

            var hitEff = atk.hitConnectEffect;

            if (hitEff != null)
            {
                Instantiate(hitEff, transform.position, Quaternion.identity,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform);
            }
            
        
            CineMachineOperator.Instance.CamaraShake(atk.hitShakeIntensity, .1f);

            var enemyStat = atk.enemySource.GetComponent<StatusManager>();

            enemyStat.ObtainTimerBuff(_jumpBoost);
        }

        
        private void JumpEffect(ActorController ac, int jumpTimes)
        {
            if (jumpTimes == 2 && ac.pi.buttonUpNew.IsPressing)
            {
                ac.jumpforce = jumpForce;
                ac.BeforeJump -= JumpEffect;
                //ac.pi.jumptime = 1;
                Instantiate(effect, ac.transform.position, Quaternion.identity,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform);
                if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20181))
                {
                    var buff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageUp,
                        30, 120, 5, 8103405);
                    buff.dispellable = false;
                    ac._statusManager.ObtainTimerBuff(buff);
                }
                ac._statusManager.RemoveConditionWithLog(_jumpBoost);
            }
        
            
        }
        
        private void ResetJumpEffect(ActorController ac, int jumpTimes)
        {
            if (ac._statusManager.HasCondition((int)BasicCalculation.BattleCondition.DoubleJumpWing) == false)
            {
                ac.ResetJumpForce();
                ac.OnJump -= ResetJumpEffect;
                //ac._statusManager.RemoveConditionWithLog(_jumpBoost);
                //ac._statusManager.RemoveAllConditionWithSpecialID(8103402);
            }
                
        }
        
    }
}

