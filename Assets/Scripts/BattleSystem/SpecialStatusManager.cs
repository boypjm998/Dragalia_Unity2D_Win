using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialStatusManager : StatusManager
{
    public float baseBreak = -1;
    public float currentBreak = -1;
    public float breakDefRate = 0.7f;
    public bool broken = false;
    public bool ODLock = false;
    public StatusManagerVoidDelegate onBreak;
    public float breakTime = 10f;
    public float counterModifier = 0;

    protected override void HPCheck()
    {
        base.HPCheck();
        if (broken == false && baseBreak > 0 && currentBreak < 0)
        {
            onBreak?.Invoke();
        }
    }
}
