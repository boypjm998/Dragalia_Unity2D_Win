using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossODBar : MonoBehaviour
{
    Slider ODBar;

    private bool displayBroken = false;
    
    public SpecialStatusManager bossStat;

    public static UI_BossODBar Instance; 

    private void Awake()
    {
        Instance = this;
        ODBar = GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        var statUI = GetComponentInParent<UI_BossStatus>();

        var statTemp = statUI.bossStat;

        if (statTemp is not SpecialStatusManager)
        {
            gameObject.SetActive(false);
            return;
        }
        
        bossStat = statTemp as SpecialStatusManager;
        
        
        if(bossStat.baseBreak == -1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            //ODBar = GetComponentInChildren<Slider>();
        }
    }

    private void Update()
    {
        if (!bossStat.broken && displayBroken == false)
        {
            ODBar.value = bossStat.currentBreak / bossStat.baseBreak;
        }


        if (bossStat.broken)
        {
            transform.GetChild(2).gameObject.SetActive(true);
            displayBroken = true;
        }
        else
        {
            transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    public void ODBarRecharge()
    {
        var twc = DOTween.To(() => ODBar.value,
            x => ODBar.value = x,
            bossStat.currentBreak / bossStat.baseBreak, 1f);
        twc.OnComplete(() =>
        {
            displayBroken = false;
        });
    }

    public void ODBarClear()
    {
        ODBar.value = 0;
    }

}
