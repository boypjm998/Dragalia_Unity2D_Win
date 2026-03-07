using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativePositionRetainer : MonoBehaviour
{
    [SerializeField] protected Transform _parent;
    protected Vector3 _relativePosition;
    public bool isActive = true;
    
    public void SetParent(Transform _parent)
    {
        this._parent = _parent;
        _relativePosition = transform.position - _parent.position;
    }

    public Vector3 GetModifiedWorldPosition()
    {
        return _relativePosition + _parent.position;
    }

    public Transform GetParent()
    {
        return _parent;
    }

    

    // Update is called once per frame
    void Update()
    {
        if(!isActive)
            return;

        if(_parent==null)
            enabled = false;
        
        transform.position = _parent.position + _relativePosition;
    }
}
