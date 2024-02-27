using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;


namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// shadow attck warp
    /// </summary>
    public class Projectile_C005_4 : MonoBehaviour
    {
        public bool attackStart = false;
        
        private GameObject MeeleFXLayer;
        private GameObject s1Shadow;
        private GameObject s2Shadow;
        private GameObject idleShadow;

        public GameObject s1FX;
        public GameObject s2FX;

        public GameObject playerSource;
        public AttackSubContainer attackContainer;
        public float modifier = 0.8f;
        
        public static Projectile_C005_4 Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
            }

            
            
            attackContainer = GetComponentInChildren<AttackSubContainer>();
            MeeleFXLayer = transform.Find("Meele").gameObject;
            s1Shadow = transform.Find("s1").gameObject;
            s2Shadow = transform.Find("s2").gameObject;
            idleShadow = transform.Find("idle").gameObject;
        }

        private void Start()
        {
            Instance = this;
        }

        public void ReleaseAttack(int skillID,GameObject target)
        {
            
            attackStart = true;
            
            if (skillID == 1)
            {
                idleShadow.SetActive(false);
                s1Shadow.SetActive(true);
                var attack = Instantiate(s1FX, transform.position,
                    Quaternion.identity,MeeleFXLayer.transform).GetComponent<AttackFromPlayer>();

                if (target != null)
                {
                    if (target.transform.position.x < transform.position.x)
                    {
                        transform.localScale = new Vector3(-1, 1, 1);
                    }
                    else
                    {
                        transform.localScale = Vector3.one;
                    }
                }
                attack.firedir = (int)transform.localScale.x;
                attack.playerpos = playerSource.transform;
                for(int i = 0 ; i < attack.attackInfo.Count; i++)
                {
                    for(int j = 0 ; j < attack.attackInfo[i].dmgModifier.Count; j++)
                    {
                        attack.attackInfo[i].dmgModifier[j] *= modifier;
                    }
                }
                var vunlerableDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable, 15, 30,
                    1,100503);
                var evilsbaneDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                    -1, 30,
                    1);
        
                attack.AddWithConditionAll(vunlerableDebuff,100);
                attack.AddWithConditionAll(evilsbaneDebuff,100,1);
            }
            else if(skillID == 2)
            {
                idleShadow.SetActive(false);
                s2Shadow.SetActive(true);
                
                var attack = Instantiate(s2FX, transform.position,
                    Quaternion.identity,MeeleFXLayer.transform).GetComponent<AttackFromPlayer>();

                if (target != null)
                {
                    if (target.transform.position.x < transform.position.x)
                    {
                        transform.localScale = new Vector3(-1, 1, 1);
                    }
                    else
                    {
                        transform.localScale = Vector3.one;
                    }
                }
                attack.firedir = (int)transform.localScale.x;
                attack.playerpos = playerSource.transform;
                
                for(int i = 0 ; i < attack.attackInfo.Count; i++)
                {
                    for(int j = 0 ; j < attack.attackInfo[i].dmgModifier.Count; j++)
                    {
                        attack.attackInfo[i].dmgModifier[j] *= modifier;
                    }
                }
                attack.AddWithConditionAll(new TimerBuff(999),100,0);
                var burnDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Burn,
                    72.7f, 12,
                    100);
                attack.AddWithConditionAll(burnDebuff,110,1);
            }
            attackContainer.enabled = true;
            attackContainer.InitAttackContainer(1,true);

            StartCoroutine(DestroyAfterAttackFinished());



        }
        
        IEnumerator DestroyAfterAttackFinished()
        {
            yield return new WaitUntil(() => MeeleFXLayer == null);
            Destroy(gameObject,0.1f);
        }


        private void OnDestroy()
        {
            //Instance = null;
            StopAllCoroutines();
        }
    }
}

