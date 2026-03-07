using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastProjectileCustom : BlastProjectile
{
    //[SerializeField] private LayerMask _layerMask;
    public GameObject src;
    public Action<AttackFromPlayer> OnBlast;

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        print(other.CompareTag(tag));
        
        if (other.CompareTag(_tag) && !_destroyed)
        {
            _destroyed = true;
            Destroy(gameObject);
            //return;
        }

        
        
    }
    
    private void OnDestroy()
    {
        if (transform.parent == null)
            return;
        
        if(!_destroyed)
            return;
        
        
        var proj = Instantiate(_blastPrefab, transform.position, Quaternion.identity,
            transform.parent);
        if (src)
        {
            var atk = proj.GetComponent<AttackFromPlayer>();
            OnBlast?.Invoke(atk);
            atk.playerpos = src.transform;
        }
        
        OnBlast = null;
        
    }
    
}
