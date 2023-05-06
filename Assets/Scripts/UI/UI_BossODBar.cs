using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossODBar : MonoBehaviour
{
    Slider ODBar;
    
    public SpecialStatusManager bossStat;

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
            ODBar = GetComponentInChildren<Slider>();
        }
    }

    private void Update()
    {
        ODBar.value = bossStat.currentBreak / bossStat.baseBreak;
        if(bossStat.broken)
            transform.GetChild(2).gameObject.SetActive(true);
        else
        {
            transform.GetChild(2).gameObject.SetActive(false);
        }
    }
}
