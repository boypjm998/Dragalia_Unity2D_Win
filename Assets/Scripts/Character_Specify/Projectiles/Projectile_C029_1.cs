using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C029_1 : MonoBehaviour
    {
        private Transform pointTransform;

        private Transform[] splashes = new Transform[3];

        private Transform splash_attack;
        private Transform splash_def;
        private Transform splash_crit;
        private Transform splash_res;

        private AttackFromPlayer attack;

        private List<TimerBuff> _timerBuffs;
        public bool extraAttack = false;

        private void Awake()
        {
            attack = GetComponent<AttackFromPlayer>();
            pointTransform = transform.Find("Points");
            splash_attack = transform.Find("splash_attack");
            splash_def = transform.Find("splash_def");
            splash_crit = transform.Find("splash_crit");
            splash_res = transform.Find("splash_res");
        }

        private void Start()
        {
            InitTimerBuff();
            InitSplashOrder();
        }

        private void InitTimerBuff()
        {
            var buff_atk = new TimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff,
                15, 15, 100);
            var buff_def = new TimerBuff((int)BasicCalculation.BattleCondition.DefDebuff,
                5, 15, 100);
            var buff_crit = new TimerBuff((int)BasicCalculation.BattleCondition.CritRateDebuff,
                20, 20, 100);
            var buff_res = new TimerBuff((int)BasicCalculation.BattleCondition.ParalysisResDown,
                20, 15, 100);
            
            //以随机顺序将buff加入数组
            _timerBuffs = new List<TimerBuff>();
            _timerBuffs.Add(buff_atk);
            _timerBuffs.Add(buff_def);
            _timerBuffs.Add(buff_crit);
            _timerBuffs.Add(buff_res);
            
            //打乱数组
            for (int i = 0; i < _timerBuffs.Count; i++)
            {
                int index = UnityEngine.Random.Range(0, _timerBuffs.Count);
                TimerBuff temp = _timerBuffs[i];
                _timerBuffs[i] = _timerBuffs[index];
                _timerBuffs[index] = temp;
            }
            
            attack.AddWithCondition(3,_timerBuffs[0],100,0);
            attack.AddWithCondition(4,_timerBuffs[1],100,1);
            if (extraAttack)
            {
                GetComponent<PlayerAttackTriggerController>().InvokeNextAttack(0.85f);
                var attackInfo4 = attack.attackInfo[4];
                attack.attackInfo.Add(attackInfo4);
                attack.AddWithCondition(5,_timerBuffs[2],100,2);
            }

        }

        private void InitSplashOrder()
        {
            for (int i = 0; i < splashes.Length; i++)
            {
                if (_timerBuffs[i].buffID == (int)BasicCalculation.BattleCondition.AtkDebuff)
                {
                    splashes[i] = splash_attack;
                }
                else if (_timerBuffs[i].buffID == (int)BasicCalculation.BattleCondition.DefDebuff)
                {
                    splashes[i] = splash_def;
                }
                else if (_timerBuffs[i].buffID == (int)BasicCalculation.BattleCondition.CritRateDebuff)
                {
                    splashes[i] = splash_crit;
                }
                else if (_timerBuffs[i].buffID == (int)BasicCalculation.BattleCondition.ParalysisResDown)
                {
                    splashes[i] = splash_res;
                }

                splashes[i].position = pointTransform.GetChild(i).position;
            }

            if (extraAttack)
            {
                Invoke("ThirdSplashFX",0.85f);
            }
            Invoke("FirstSplashFX",0.7f);
            Invoke("SecondSplashFX",0.75f);


        }

        private void FirstSplashFX()
        {
            splashes[0].gameObject.SetActive(true);
        }
        private void SecondSplashFX()
        {
            splashes[1].gameObject.SetActive(true);
        }
        private void ThirdSplashFX()
        {
            splashes[2].gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            CancelInvoke();
        }
    }
}

