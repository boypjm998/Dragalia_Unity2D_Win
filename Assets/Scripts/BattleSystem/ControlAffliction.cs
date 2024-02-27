using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class ControlAffliction : MonoBehaviour
{
    [SerializeField] private BasicCalculation.BattleCondition _condition;
    [SerializeField] private bool canStack = false;
    private StatusManager _statusManager;

    private void Awake()
    {
        if(canStack)
            return;

        var others = transform.parent.GetComponentsInChildren<ControlAffliction>();
        for (int i = others.Length - 1; i >= 0; i--)
        {
            if(others[i]!=this && others[i].canStack == false)
                Destroy(others[i].gameObject);
        }

        
        
    }

    private void Start()
    {
        _statusManager = GetComponentInParent<StatusManager>();
        // _statusManager.OnBuffExpiredEventDelegate += DestroyFX;
        // _statusManager.OnBuffDispelledEventDelegate += DestroyFX;
    }

    private void DestroyFX(BattleCondition condition)
    {
        if(condition.buffID == (int)_condition)
            Destroy(gameObject);
    }

    private void Update()
    {
        if(_statusManager.GetConditionStackNumber((int)_condition)<=0)
            Destroy(gameObject);
    }
}
