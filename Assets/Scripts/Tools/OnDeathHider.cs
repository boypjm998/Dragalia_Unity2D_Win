using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For enemy
/// </summary>
[RequireComponent(typeof(AttackFromEnemy))]
public class OnDeathHider : MonoBehaviour
{
    AttackFromEnemy _attackFromEnemy;
    StatusManager _statusManager;
    [SerializeField] bool destroyOnDeath;
    
    private void Awake()
    {
        _attackFromEnemy = GetComponent<AttackFromEnemy>();
       
    }
    
    private IEnumerator Start()
    {
        yield return null;
        _statusManager = _attackFromEnemy.enemySource?.GetComponent<StatusManager>();
        if(_statusManager!=null)
            _statusManager.OnReviveOrDeath += OnDeath;
        else
        {
            OnDeath();
        }
    }
    
    void OnDeath()
    {
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnDestroy()
    {
        if(_statusManager!=null)
            _statusManager.OnReviveOrDeath -= OnDeath;
    }
}
