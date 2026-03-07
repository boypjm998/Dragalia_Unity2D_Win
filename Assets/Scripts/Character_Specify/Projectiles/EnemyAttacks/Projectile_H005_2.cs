using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_H005_2 : Projectile_DB015_2
    {
        [SerializeField] private GameObject stoneSprite2;
        
        [SerializeField] protected int currentLevel = 2;
        
        public int Level => currentLevel;
        
        private void Start()
        {
            Invoke("BreakStoneEntirely", 15f);
            BattleStageManager.Instance.specialEventTriggered += AutoDestruction;
        }
        
        private void OnDestroy()
        {
            CancelInvoke();
            BattleStageManager.Instance.specialEventTriggered -= AutoDestruction;
        }
        

        public void BreakStoneEntirely()
        {
            if (IsBroken) return;
            currentLevel = 0;
            IsBroken = true;
            DOVirtual.DelayedCall(0.5f,()=>
            {
                stoneSprite.SetActive(false);
                stoneSprite2.SetActive(false);
            },false);
            stoneBreakEffect.SetActive(true);
            stoneBreakEffect.GetComponent<ParticleSystem>()?.Play(true);
            Destroy(transform.parent.gameObject,1.5f);
        }

        protected override void AutoDestruction(int eventID)
        {
            if(eventID == 2)
                BreakStoneEntirely();
            else if (eventID == 3)
            {
                BreakStone();
            }
        }
        public override void BreakStone()
        {
            if (currentLevel == 1 && IsBroken == false)
            {
                BreakStoneEntirely();
            }
            else if(currentLevel > 1)
            {
                currentLevel = 1;
                stoneBreakEffect.SetActive(true);
                stoneSprite.SetActive(false);
                stoneSprite2.SetActive(true);
            }
        }
        
        
        
    }

}
