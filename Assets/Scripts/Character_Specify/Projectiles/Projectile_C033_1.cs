using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Regina: Trap
    /// </summary>
    public class Projectile_C033_1 : MonoBehaviour
    {
        //Singleton
        private static Projectile_C033_1 instance;
        private ForcedAttackFromPlayer attack;
        private GameObject electricFX;
        private GameObject impactFX;
        private ActorController_c033 ac;
        
        public static Projectile_C033_1 Instance => instance;

        private GameObject target;
        
        //Singleton initialization
        private void Awake()
        {
            print("Projectile_C033_1 Awake");
            
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
            
            
            impactFX = transform.GetChild(1).gameObject;
            electricFX = transform.GetChild(0).gameObject;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (target == null)
            {
                ac.SetTrap(false);
                Destroy(gameObject);
            }

        }


        private void OnDestroy()
        {
            //Singleton
            if (instance == this)
            {
                ac.SetTrap(false);
                instance = null;
            }

            
            
        }
        
        public void TriggerAttackThenDestroy(ActorController_c033 ac)
        {
            
            if(target == null)
            {
               ac.SetTrap(false);
                        
               Destroy(gameObject);
               return;
            }
            
            
            
            GetComponent<RelativePositionRetainer>().enabled = false;
            instance = null;
            attack = impactFX.GetComponent<ForcedAttackFromPlayer>();
            attack.target = target;
            electricFX.SetActive(false);
            
            //定义一个Func<>
            Func<StatusManager,StatusManager,bool> condition = (self,target) =>
            {
                if (target.GetComponent<EnemyController>().counterOn)
                {
                    return true;
                }

                return false;
            };

            var caf = new ConditionalAttackEffect(condition,
                ConditionalAttackEffect.ExtraEffect.ExtraCritRate, null,
                new string[]{"999"});
            
            attack.AddConditionalAttackEffect(caf);
            
            impactFX.transform.position = target.RaycastedPosition();
            impactFX.SetActive(true);
            ac.SetTrap(false);
            
            Destroy(gameObject,2f);

        }
        
        

        public void SetTarget(GameObject target)
        {
            this.target = target;
            GetComponent<RelativePositionRetainer>().SetParent(target.transform);
            
        }
        
        public void SetSource(ActorController_c033 ac)
        {
            this.ac = ac;
        }

        public void SetEffectFactor(Vector3 factor)
        {
            electricFX.transform.localScale = factor;
        }


    }
}

