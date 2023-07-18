using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class ControlAffliction : MonoBehaviour
{
    [SerializeField] private BasicCalculation.BattleCondition _condition;
    private StatusManager _statusManager;

    private void Start()
    {
        _statusManager = GetComponentInParent<StatusManager>();
    }

    private void Update()
    {
        if(_statusManager.GetConditionStackNumber((int)_condition)<=0)
            Destroy(gameObject);
    }
}
