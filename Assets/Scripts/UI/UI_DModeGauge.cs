using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DModeGauge : MonoBehaviour
{
    public PlayerStatusManager statusManager;
    
    [SerializeField] protected Slider gaugeCdSlider;
    [SerializeField] protected Slider gaugeFillSlider;
    [SerializeField] protected Color FillFullColor = new Color(0.5f,1f,0.9f);
    [SerializeField] protected Color FillNormalColor = new Color(0,0.78f,0.73f);

    [SerializeField] protected Image dragonImageIcon;
    [SerializeField] protected Image dragonGaugeImage;

    protected void Start()
    {
        statusManager = GameObject.Find("PlayerHandle").GetComponent<PlayerStatusManager>();
        statusManager.OnShapeshiftingEnter += ActiveDModeStatusUI;
        statusManager.OnShapeshiftingExit += DeactiveDModeStatusUI;
    }


    protected virtual void ActiveDModeStatusUI()
    {
        
    }
    
    protected virtual void DeactiveDModeStatusUI()
    {
        
    }

}
