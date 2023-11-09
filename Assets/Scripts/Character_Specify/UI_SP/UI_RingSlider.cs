using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RingSlider : MonoBehaviour
{
    public float currentValue = 0;
    public float maxValue = 1;
    //public float totalTime = 20;
    MaterialPropertyBlock block;
    SpriteRenderer frontGauge;
    private SpriteRenderer backGauge;
    
    public Color normalColor = Color.yellow;
    public Color warningColor = Color.red;

    private void Awake()
    {
        block = new MaterialPropertyBlock();
        frontGauge = transform.Find("front").GetComponent<SpriteRenderer>();
        backGauge = transform.Find("back").GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        UpdateGauge();
        
    }
    
    private void UpdateGauge()
    {
        float fillAmount = currentValue / maxValue;
        frontGauge.GetPropertyBlock(block);
        block.SetFloat("_FillAmount", fillAmount);
        frontGauge.SetPropertyBlock(block);
        
        currentValue -= Time.deltaTime;
        if (currentValue < 0)
            currentValue = 0;
        
        if(fillAmount < 0.25f)
            frontGauge.color = warningColor;
        else
            frontGauge.color = normalColor;
    }
}
