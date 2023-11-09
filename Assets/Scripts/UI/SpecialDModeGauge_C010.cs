using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController;
using UnityEngine;
using UnityEngine.UI;

public class SpecialDModeGauge_C010 : UI_DModeGauge
{
    [SerializeField] private Sprite DModeNormalImageSprite;
    [SerializeField] private Sprite DModeBWImageSprite;

    

    private void Update()
    {
        gaugeFillSlider.value = 0.23f + (statusManager.DModeGauge / statusManager.MaxDModeGauge)*0.58f;
        if (gaugeFillSlider.value >= 0.81f)
        {
            dragonGaugeImage.color = FillFullColor;
        }
        else
        {
            dragonGaugeImage.color = FillNormalColor;
        }


        gaugeCdSlider.value = (statusManager.shapeshiftingCDTimer / statusManager.shapeshiftingCD);
        if (gaugeCdSlider.value > 0 || statusManager.DModeGauge < statusManager.ReqDModeGauge)
        {
            if (statusManager.isShapeshifting)
            {
                dragonImageIcon.sprite = DModeNormalImageSprite;
            }else 
                dragonImageIcon.sprite = DModeBWImageSprite;
        }
        else
        {
            dragonImageIcon.sprite = DModeNormalImageSprite;
        }
    }
    
    
    
    
}
