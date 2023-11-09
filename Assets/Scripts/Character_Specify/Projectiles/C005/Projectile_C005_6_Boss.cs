using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C005_6_Boss : MonoBehaviour
    {
        public bool attackStart = false;

        private GameObject MeeleFXLayer;
        private GameObject s1Shadow;
        private GameObject s2Shadow;
        private GameObject idleShadow;

        public GameObject s1FX;
        public GameObject s2FX;

        public GameObject enemySource;
        public AttackContainer attackContainer;

        public static Projectile_C005_6_Boss Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
            }




            

            attackContainer = GetComponentInChildren<AttackContainer>();
            MeeleFXLayer = transform.Find("Meele").gameObject;
            s1Shadow = transform.Find("s1").gameObject;
            s2Shadow = transform.Find("s2").gameObject;
            idleShadow = transform.Find("idle").gameObject;
        }

        private void Start()
        {
            Instance = this;
        }

        public void ReleaseAttack(int skillID, GameObject target)
        {

            attackStart = true;

            if (skillID == 1)
            {
                idleShadow.SetActive(false);
                s1Shadow.SetActive(true);
                var attack = Instantiate(s1FX, transform.position,
                    Quaternion.identity, MeeleFXLayer.transform).GetComponent<AttackFromEnemy>();

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
                attack.enemySource = enemySource;
                for (int i = 0; i < attack.attackInfo.Count; i++)
                {
                    for (int j = 0; j < attack.attackInfo[i].dmgModifier.Count; j++)
                    {
                        attack.attackInfo[i].dmgModifier[j] *= 0.8f;
                    }
                }

                var vunlerableDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable, 15, 30,
                    1, 1);
                var evilsbaneDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
                    -1, 30,
                    1,1);

                attack.AddWithConditionAll(vunlerableDebuff, 100);
                attack.AddWithConditionAll(evilsbaneDebuff, 100, 1);
            }
            else if (skillID == 2)
            {
                idleShadow.SetActive(false);
                s2Shadow.SetActive(true);
                transform.localScale = Vector3.one;

                var attack = Instantiate(s2FX, transform.position,
                    Quaternion.identity, MeeleFXLayer.transform).GetComponent<CustomMeeleFromEnemy>();

                // if (target != null)
                // {
                //     if (target.transform.position.x < transform.position.x)
                //     {
                //         transform.localScale = new Vector3(-1, 1, 1);
                //     }
                //     else
                //     {
                //         transform.localScale = Vector3.one;
                //     }
                // }

                attack.firedir = (int)transform.localScale.x;
                attack.enemySource = enemySource;
                attack.GetComponent<IdentityRetainer>().enabled = false;

                for (int i = 0; i < attack.attackInfo.Count; i++)
                {
                    for (int j = 0; j < attack.attackInfo[i].dmgModifier.Count; j++)
                    {
                        attack.attackInfo[i].dmgModifier[j] *= 0.8f;
                    }
                }

                attack.AddWithConditionAll(new TimerBuff(999), 100, 0);
                var burnDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Burn,
                    72.7f, 12,
                    1);
                attack.AddWithConditionAll(burnDebuff, 110, 1);
            }

            attackContainer.enabled = true;
            //attackContainer.InitAttackContainer(1, false);

            StartCoroutine(DestroyAfterAttackFinished());



        }

        IEnumerator DestroyAfterAttackFinished()
        {
            yield return new WaitUntil(() => MeeleFXLayer == null);
            Destroy(gameObject, 0.1f);
        }


        private void OnDestroy()
        {
            //Instance = null;
            StopAllCoroutines();
        }
    }
}

