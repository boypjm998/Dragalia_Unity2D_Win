using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_H001_Countdown : MonoBehaviour
{
    [SerializeField] private TextMeshPro countdownTmp;
    [SerializeField] private GameObject warningPrefab;
    
    private UI_RingSlider _ringSlider;
    private void Start()
    {
        _ringSlider = GetComponentInChildren<UI_RingSlider>();
    }

    private void Update()
    {
        if(_ringSlider.currentValue>0)
            countdownTmp.text = Mathf.Ceil(_ringSlider.currentValue).ToString();
        else countdownTmp.text = "0";
        
        var fraction = _ringSlider.currentValue / _ringSlider.maxValue;

        if (fraction >= 0.75f)
        {
            countdownTmp.color = Color.green;
            warningPrefab.SetActive(false);
        }
        else if (fraction > 0.5f)
        {
            countdownTmp.color = Color.Lerp(Color.yellow, Color.green, (fraction - 0.5f) * 2);
            warningPrefab.SetActive(false);
        }
        else if(fraction > 0.3f)
        {
            countdownTmp.color = Color.Lerp(Color.red, Color.yellow, fraction * 3);
            warningPrefab.SetActive(false);
        }
        else
        {
            countdownTmp.color = Color.red;
            warningPrefab.SetActive(true);
        }
        
        
    }
}
