using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class Projectile_DB005_2 : MonoBehaviour
{
    private StatusManager _statusManager;
        public List<GameObject> fxInstances = new();
    
    
    private void Awake()
    {
        _statusManager = GetComponentInParent<StatusManager>();
        _statusManager.OnBuffEventDelegate += CheckSpiteStack;
        _statusManager.OnBuffExpiredEventDelegate += CheckSpiteStack;
        _statusManager.OnBuffDispelledEventDelegate += CheckSpiteStack;
    }
    
    private void OnDestroy()
    { 
        _statusManager.OnBuffEventDelegate -= CheckSpiteStack;
        _statusManager.OnBuffExpiredEventDelegate -= CheckSpiteStack;
        _statusManager.OnBuffDispelledEventDelegate -= CheckSpiteStack;
    }
    
    
        private void CheckSpiteStack(BattleCondition cond)
        {
            int stackNum = 0;
            if(cond.buffID == (int)(BasicCalculation.BattleCondition.Spite))
            {
                stackNum = _statusManager.GetConditionStackNumber(
                    (int)(BasicCalculation.BattleCondition.Spite));
                //fxInstances[stackNum - 1].SetActive(true);
                for(int i = 0; i < fxInstances.Count; i++)
                {
                    if(i == stackNum)
                    {
                        fxInstances[i].SetActive(true);
                    }
                    else
                    {
                        fxInstances[i].SetActive(false);
                    }
                }
            }
        }
}
