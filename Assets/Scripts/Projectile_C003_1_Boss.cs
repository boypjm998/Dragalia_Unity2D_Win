using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C003_1_Boss : MonoBehaviour
    {
        // Glorious Sanctuary(BOSS)
        protected bool isActive = false;
        private bool isUsed = false;
        public GameObject BossGameObject;
        [SerializeField] private float basePotency;
        private StatusManager snappedStatusManager;
    
        private void Awake()
        {
            var others = FindObjectsOfType<Projectile_C003_1_Boss>();
            foreach (var other in others)
            {
                if (other != this)
                {
                    Destroy(other.gameObject);
                }
            }
        }
    
        public void InitPotencyInfo(StatusManager statusManager)
        {
            snappedStatusManager = gameObject.AddComponent<StatusManager>();
            //将statusManager的所有属性复制到snappedStatusManager
            snappedStatusManager.conditionList = statusManager.conditionList;
            snappedStatusManager.baseAtk = statusManager.baseAtk;
            snappedStatusManager.maxHP = statusManager.maxHP;
            snappedStatusManager.maxBaseHP = statusManager.maxBaseHP;
            snappedStatusManager.currentHp = statusManager.currentHp;
            snappedStatusManager.abilityList = statusManager.abilityList;
            snappedStatusManager.enabled = false;
        }
    
        private void OnTriggerEnter2D(Collider2D col)
        {
            if(isActive == false || isUsed == true)
                return;
            
            //print(col.gameObject.name);
            
            if (col.transform.parent.gameObject == BossGameObject)
            {
                isUsed = true;
                BossGameObject.GetComponent<StatusManager>().HPRegenImmediately(snappedStatusManager,basePotency,0);
                Destroy(gameObject);
            }
    
        }
    
        private void OnTriggerExit2D(Collider2D other)
        {
            if(other.transform.parent.gameObject == BossGameObject)
                isActive = true;
        }
    }
}

