using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Stone (Surtr)
    /// </summary>
    public class Projectile_DB015_2 : MonoBehaviour
    {
        [SerializeField] protected GameObject stoneSprite;
        [SerializeField] protected GameObject stoneBreakEffect;
        [SerializeField] protected Collider2D blockCollider;
        public bool IsBroken { get; protected set; }
        
        private void Start()
        {
            Invoke("BreakStone", 15f);
            BattleStageManager.Instance.specialEventTriggered += AutoDestruction;
        }

        private void OnDestroy()
        {
            CancelInvoke();
            BattleStageManager.Instance.specialEventTriggered -= AutoDestruction;
        }
        
        protected virtual void AutoDestruction(int eventID)
        {
            if(eventID == 2)
                BreakStone();
            
        }

        public virtual void BreakStone()
        {
            if (IsBroken) return;
            IsBroken = true;
            DOVirtual.DelayedCall(0.5f,()=>stoneSprite.SetActive(false),false);
            stoneBreakEffect.SetActive(true);
            Destroy(transform.parent.gameObject,1.5f);
        }
        
        
    }

}
