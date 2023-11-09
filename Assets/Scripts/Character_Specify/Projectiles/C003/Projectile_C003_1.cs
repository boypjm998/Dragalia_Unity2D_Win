using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C003_1 : MonoBehaviour
    {
        protected bool isActive = false;
        private bool isUsed = false;
        [FormerlySerializedAs("BossGameObject")] public GameObject playerGameObject;
        [SerializeField] private float basePotency;
        private StatusManager snappedStatusManager;
        private ActorController ac;
        private void Awake()
        {
            var others = FindObjectsOfType<Projectile_C003_1>();
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
            ac = playerGameObject.GetComponent<ActorController>();
        }
        
        private void OnTriggerStay2D(Collider2D col)
        {
            if(isActive == false || isUsed == true)
                return;
                
            //print(col.gameObject.name);
                
            if (col.transform.parent.gameObject == playerGameObject && ac.grounded && !ac.dodging)
            {
                
                isUsed = true;
                playerGameObject.GetComponent<StatusManager>().HPRegenImmediately(snappedStatusManager,basePotency,0);
                //Destroy(gameObject);
                transform.Find("laser").gameObject.SetActive(true);
                Destroy(gameObject,0.5f);
            }
        
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if(other.transform.parent.gameObject == playerGameObject)
                isActive = true;
        }
    }
}

