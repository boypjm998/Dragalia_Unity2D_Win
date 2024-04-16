using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastProjectile : MonoBehaviour
{
    [SerializeField] private String tag;
    [SerializeField] private GameObject _blastPrefab;

    private AttackFromEnemy _attackFromEnemy;
    private AttackFromPlayer _attackFromPlayer;

    private bool _destroyed;

    private void Awake()
    {
        _attackFromEnemy = GetComponent<AttackFromEnemy>();
        _attackFromPlayer = GetComponent<AttackFromPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag(tag) || (other.CompareTag("Ground")) && !_destroyed))
        {
            _destroyed = true;
            
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        var proj = Instantiate(_blastPrefab, transform.position, Quaternion.identity,
            transform.parent);
        if (_attackFromEnemy)
        {
            proj.GetComponent<AttackFromEnemy>().enemySource = _attackFromEnemy.enemySource;
        }
        if (_attackFromPlayer)
        {
            proj.GetComponent<AttackFromPlayer>().playerpos = _attackFromPlayer.playerpos;
        }
    }
}
