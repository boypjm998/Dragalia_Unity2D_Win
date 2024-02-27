using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_CountdownMinon : MonoBehaviour
{
    private List<StatusManager> _statusManagers = new();

    private int capacity;
    private UI_RingSlider _ringSlider;
    private TextMeshPro tmp;

    public int Value => capacity;

    private void Awake()
    {
        _ringSlider = GetComponent<UI_RingSlider>();
        tmp = GetComponentInChildren<TextMeshPro>();
    }

    public void SetMaxCapacity(int maxCapacity)
    {
        this.capacity = maxCapacity;
        // _ringSlider.maxValue = maxCapacity;
        // _ringSlider.currentValue = maxCapacity;
        tmp.text = $"×{maxCapacity}";
    }

    public void SetCapacity(int capacity)
    {
        this.capacity = capacity;
        //_ringSlider.currentValue = this.capacity;
        tmp.text = $"×{this.capacity}";
    }

    public int KillAllMinons()
    {
        var minionsLeft = capacity;
        foreach (var _status in _statusManagers)
        {
            _status.currentHp = 0;
        }

        return minionsLeft;
    }

    public void AddNewStatusManager(StatusManager statusManager)
    {
        StatusManager.StatusManagerVoidDelegate handler = null;
        handler = () =>
        {
            _statusManagers.Remove(statusManager);
            SetCapacity(capacity - 1);
            statusManager.OnReviveOrDeath -= handler;
        };
        statusManager.OnReviveOrDeath += handler;
        _statusManagers.Add(statusManager);
    }

    

    
}
