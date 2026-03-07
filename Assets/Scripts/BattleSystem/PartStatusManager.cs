using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartStatusManager : StatusManager
{
    public int partID;
    public StatusManager mainStatus;
    public bool hpDecreaseLink;
    public bool hpIncreaseLink;
    //public bool overdriveLink;
    bool linked = false;
    public bool hasBroken { get; private set; } = false;
    public bool IsFakeActive { get; private set; } = false;


    public event Action<PartStatusManager, int> OnPartBroken;
    public event Action<PartStatusManager, bool> OnPartActive;
    public event Action<PartStatusManager, bool> OnFakeActive;


    protected override void Start()
    {
        base.Start();
        Link();
    }

    private void OnEnable()
    {
        OnPartActive?.Invoke(this, true);
    }

    private void OnDisable()
    {
        OnPartActive?.Invoke(this, false);
    }
    

    public void Link(StatusManager newStat = null)
    {
        if (newStat != null)
        {
            mainStatus = newStat;
        }
        
        if (mainStatus == null || linked)
            return;

        linked = true;
        OnHPDecrease += HPDecreaseSync;
        OnHPIncrease += HPIncreaseSync;
    }

    public void CancelLink()
    {
        if(mainStatus == null || !linked)
            return;
        
        linked = false;
        OnHPDecrease -= HPDecreaseSync;
        OnHPIncrease -= HPIncreaseSync;
        
        mainStatus = null;
    }

    private void OnDestroy()
    {
        CancelLink();
    }

    private void HPDecreaseSync(int dmg, AttackBase atk)
    {
        if(hpDecreaseLink)
        {
            if (mainStatus.currentHp <= 0)
                return;

            if(currentHp <= 0 && hasBroken == false)
            {
                hasBroken = true;
                OnPartBroken?.Invoke(this,partID);
                Debug.Log("Part Broken");
            }
        }
    }
    private void HPIncreaseSync(int dmg)
    {
        if (hpIncreaseLink && !hasBroken)
        {
            //currentHp += dmg;
            // OnHPIncrease?.Invoke(dmg);
            // OnHPChange?.Invoke();
        }
    }
    
    public void FakeActive(bool active)
    {
        OnFakeActive?.Invoke(this, active);
        IsFakeActive = active;
    }
    
    public void Repair(float hpPercent = 1)
    {
        currentHp = (int)(maxHP * hpPercent);
        hasBroken = false;
        OnHPChange?.Invoke();
    }
    
    
    
    
}
