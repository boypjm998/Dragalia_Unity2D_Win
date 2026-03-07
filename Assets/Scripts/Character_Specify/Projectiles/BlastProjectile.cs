using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BlastProjectile : MonoBehaviour
{
    [FormerlySerializedAs("tag")] [SerializeField] protected String _tag;
    [SerializeField] protected GameObject _blastPrefab;

    protected AttackFromEnemy _attackFromEnemy;
    protected AttackFromPlayer _attackFromPlayer;

    protected bool _destroyed;

    private void Awake()
    {
        _attackFromEnemy = GetComponent<AttackFromEnemy>();
        _attackFromPlayer = GetComponent<AttackFromPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag(_tag) || (other.CompareTag("Ground")) && !_destroyed))
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
