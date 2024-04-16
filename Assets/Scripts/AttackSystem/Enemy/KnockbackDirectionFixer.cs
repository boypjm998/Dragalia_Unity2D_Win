using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackDirectionFixer : MonoBehaviour
{
    [SerializeField] private AttackBase _bindedAttack;

    private void Awake()
    {
        if (_bindedAttack == null)
            _bindedAttack = GetComponent<AttackBase>();
    }

    private void Update()
    {
        _bindedAttack.attackInfo[0].knockbackDirection = transform.right;
    }
}
