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
    public bool displayInBar { get; protected set; } = true; //是否显示在buff栏中，已经废弃

    public int specialID = -1;

    /// <summary>
    /// 累计值
    /// </summary>
    protected float tickTime = 0;

    public float TickInterval { get; protected set; } = -1;

    /// <summary>
    /// 状态的剩余时间
    /// </summary>
    public float lastTime { set; get; }

    /// <summary>
    /// 状态的总持续时间
    /// </summary>
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
        //this.canStack = false;
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
    
    public void SetTickInterval(float value = 2.9f)
    {
        TickInterval = value;
    }

    
    public Action<StatusManager> OnBuffStart;
    public Action<StatusManager> OnBuffRemove;
    public event Action<StatusManager> OnBuffUpdate;
    
    
    public void Tick(float deltaTime, StatusManager statusManager)
    {
        tickTime += deltaTime;
        if (tickTime >= TickInterval)
        {
            tickTime -= TickInterval;
            OnBuffUpdate?.Invoke(statusManager);
        }
    }

    public abstract void BuffDispell();

}
