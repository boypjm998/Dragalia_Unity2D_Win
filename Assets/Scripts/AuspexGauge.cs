using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AuspexGauge : MonoBehaviour,ICharacterSpecialGauge
{

    public ActorController_c003 ac;

    [SerializeField] protected Slider backGaugeSlider;
    [SerializeField] protected Slider frontGaugeSlider;
    [SerializeField] protected Image frontGaugeImage;
    [SerializeField] private Color unableColor;
    [SerializeField] private Color activeColor;
    
    Tweener _tweener;
    private Animator gaugeAnimator;
    private int currentRoutineID = 0;


    private void Start()
    {
        gaugeAnimator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        //print(ac.auspexGauge);
        backGaugeSlider.value = ac.auspexGauge;

        if (backGaugeSlider.value != frontGaugeSlider.value)
        {
            ChargeTo(ac.auspexGauge);
        }else if(ac.auspexGauge == 0 && frontGaugeSlider.value != 0)
        {
            Reset();
        }

        if (ac.auspexGauge >= 3)
        {
            frontGaugeImage.color = activeColor;
        }
        else
        {
            frontGaugeImage.color = unableColor;
        }



    }


    public void Charge(int cp)
    {
        
    }

    public void ChargeTo(int cp, int level = 0)
    {
        if(currentRoutineID == 1)
            return;

        if (_tweener != null)
        {
            if(_tweener.IsPlaying())
                _tweener.Complete();
        }

        
        
        currentRoutineID = 1;
        _tweener = frontGaugeSlider.
            DOValue(ac.auspexGauge,
                 0.2f).OnComplete(
        ()=>
        {
            currentRoutineID = 0;
            if (ac.auspexGauge >= 3)
            {
                gaugeAnimator.Play("active",0,0);
                gaugeAnimator.GetComponent<Image>().enabled = true;
            }
        });
    }

    public void Reset()
    {
        if(currentRoutineID == 2)
            return;
        
        if (_tweener != null)
        {
            if(_tweener.IsPlaying())
                _tweener.Complete();
        }
        
        currentRoutineID = 2;
        _tweener = frontGaugeSlider.
            DOValue(0,
                0.25f).OnComplete(
                ()=>
                {
                    currentRoutineID = 0;
                }).SetEase(Ease.Linear);
    }
}
