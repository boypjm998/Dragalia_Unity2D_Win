using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UI_BossConditionBar : UI_ConditionBar
{
    //public StatusManager bossStatInfo;

    void Awake()
    {
        startPosition = -200;
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        
    }

    public void SetTargetStat(StatusManager stat)
    {
        targetStat = stat;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
