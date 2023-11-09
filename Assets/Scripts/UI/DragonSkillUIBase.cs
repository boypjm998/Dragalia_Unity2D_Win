using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSkillUIBase : SkillUIBase
{
    protected bool dModeActive => _canvasGroup.alpha > 0f;
    DragonController dc;

    protected override void Start()
    {
        base.Start();
        dc = sm.transform.Find("DModel").GetChild(0).GetComponent<DragonController>();

    }

    protected override void ShowCanvas()
    {
        base.ShowCanvas();
        if(dc == null)
            dc = sm.GetComponentInChildren<DragonController>();
            
    }

    protected override void CheckSkillCD()
    {
        if(dModeActive == false)
            return;

        

        
        
        spGaugeCDValue = 1.0f - (dc.currentDSP[sid-1] / dc.requiredDSP[sid-1]);
        if (spGaugeCDValue < 0)
        {
            spGaugeCDValue = 0;
        }
        else if (spGaugeCDValue > 1)
        {
            spGaugeCDValue = 1;
        }
        cooldownGauge.value = spGaugeCDValue;



        if (spGaugeCDValue > 0 && !unableIcon.activeSelf)
        {
            skillIcon.transform.GetChild(1).gameObject.SetActive(true);
            keyHint.SetActive(false);
        }
        else
        {
            skillIcon.transform.GetChild(1).gameObject.SetActive(false);
            keyHint.SetActive(true);
        }
    }
}
