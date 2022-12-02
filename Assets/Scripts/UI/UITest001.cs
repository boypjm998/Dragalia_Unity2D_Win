using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest001 : MonoBehaviour
{
    GameObject skillIcon;
    Image skillImage;
    PlayerStatusManager sm;
    float spGaugeCDValue;
    Slider cooldownGauge;
    
    // Start is called before the first frame update
    void Start()
    {
        sm = GameObject.Find("PlayerHandle").GetComponent<PlayerStatusManager>();
        cooldownGauge = transform.Find("CD").GetComponent<Slider>();
        //print(spGaugeValue);
    }

    // Update is called once per frame
    void Update()
    {
        spGaugeCDValue = 1.0f - (sm.currentSP[0] / sm.requiredSP[0]);
        if (spGaugeCDValue < 0)
        {
            spGaugeCDValue = 0;
        }
        else if (spGaugeCDValue > 1)
        {
            spGaugeCDValue = 1;
        }
        cooldownGauge.value = spGaugeCDValue;
        

        skillIcon = transform.Find("IconBody").Find("Mask").Find("skill").gameObject;
        if (spGaugeCDValue > 0)
        {
            skillIcon.transform.GetChild(1).gameObject.SetActive(true);
        }
        else {
            skillIcon.transform.GetChild(1).gameObject.SetActive(false);
        }

        
    }
}
