using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleCondition
{
    protected Sprite buffIcon;
    
    protected int buffID;
    protected int maxStackNum = 100;
    protected bool dispellable = true;
    protected float effect;
    protected buffEffectDisplayType DisplayType;
    protected bool addToTotal = true; //特殊buff，比如50加攻的地狱模式使用后，不额外显示50加攻。

    

    public enum buffEffectDisplayType
    {
        Value = 0,
        StackNumber = 1,
        None = 2
    }
    

    public virtual void GetIcon()
    {

    }

    protected virtual void OnBuffEnable()
    {
    }

    protected virtual void OnBuffDisable()
    {
    }

    protected abstract void BuffStart();

    protected abstract void BuffExpired();

    protected abstract void BuffDispell();

}
