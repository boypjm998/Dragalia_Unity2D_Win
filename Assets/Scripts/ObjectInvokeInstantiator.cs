using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInvokeInstantiator : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    public float invokeTime = 1f;
    public Vector3 offset = Vector3.zero;

    public Transform _parent;
    
    private void Awake()
    {
        if (_parent == null)
            _parent = this.transform;
    }

    
    protected void GenerateInstance()
    {
        Instantiate(prefab,transform.position + offset, Quaternion.identity,_parent);
    }



}
