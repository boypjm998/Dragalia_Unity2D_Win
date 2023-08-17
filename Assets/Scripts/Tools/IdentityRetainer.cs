using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentityRetainer : MonoBehaviour
{
    // Start is called before the first frame update
    
    private Transform _transform;

    private void Awake()
    {
        try
        {
            _transform = GetComponentInParent<StatusManager>().transform;
        }
        catch
        {
            _transform = transform.parent;
        }
    }

    void Update()
    {
        if (_transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            return;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        transform.rotation = Quaternion.identity;
    }
}
