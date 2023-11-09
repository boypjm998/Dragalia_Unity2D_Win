using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativePositionRetainer : MonoBehaviour
{
    [SerializeField] protected Transform _parent;
    protected Vector3 _relativePosition;
    
    public void SetParent(Transform _parent)
    {
        this._parent = _parent;
        _relativePosition = transform.position - _parent.position;
    }

    

    // Update is called once per frame
    void Update()
    {
        if(_parent==null)
            enabled = false;
        
        transform.position = _parent.position + _relativePosition;
    }
}
