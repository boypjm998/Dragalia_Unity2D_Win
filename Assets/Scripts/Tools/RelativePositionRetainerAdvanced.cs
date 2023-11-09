using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativePositionRetainerAdvanced : RelativePositionRetainer
{
    public enum LockType
    {
        Both,
        X,
        Y,
        None
    }
    public LockType lockType = LockType.Both;
    public float stopTime = -1;

    private void Start()
    {
        if (stopTime > 0)
            Invoke("StopTick", stopTime);

    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    private void StopTick()
    {
        lockType = LockType.None;
    }

    // Update is called once per frame
    
    void Update()
    {
        if(_parent==null)
            enabled = false;

        if (lockType == LockType.Both)
        {
            transform.position = _parent.position + _relativePosition;
        }else if (lockType == LockType.X)
        {
            transform.position = new Vector3(_parent.position.x + _relativePosition.x, transform.position.y, transform.position.z);
        }
        else if (lockType == LockType.Y)
        {
            transform.position = new Vector3(transform.position.x, _parent.position.y + _relativePosition.y, transform.position.z);
        }
        
    }




}
