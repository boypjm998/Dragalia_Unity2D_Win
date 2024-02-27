using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMechanics;

[Serializable]
public abstract class BattleCondition
{
    protected Sprite buffIcon;
    
    public int buffID { get; protected set; }
    public int maxStackNum { get; protected set; } = BasicCalculation.MAXCONDITIONSTACKNUMBER;
    public bool dispellable = true;
    public float effect { get; protected set; }
    public buffEffectDisplayType DisplayType { get; protected set; }
    public bool displayInBar { get; protected set; } = true; //是否显示在buff栏中

    public int specialID = -1;

    /// <summary>
    /// obsoleted
    /// </summary>
    public bool canStack { get; protected set; } = true;

    public float lastTime { set; get; }

    public float duration { protected set; get; } //duration is -1 means no time limit.

    

    public enum buffEffectDisplayType
    {
        Value = 0,
        StackNumber = 1,
        Level = 2,
        None = 3,
        ExactValue = 4,
        EnergyOrInspiration = 5
    }


    public abstract Sprite GetIcon();

    public void SetUniqueBuffInfo(int spID)
    {
        this.canStack = false;
        this.specialID = spID;
    }

    public void HideInspector()
    {
        this.displayInBar = false;
    }

    public void SetEffect(float newEffect)
    {
        effect = newEffect;
    }

    public void SetDuration(float value)
    {
        var diff = value - duration;
        duration = value;
        lastTime += diff;
    }



    public Action<StatusManager> OnBuffStart;
    public Action<StatusManager> OnBuffRemove;

    public abstract void BuffDispell();

}
