using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BodyPartStatus : MonoBehaviour
{
    public PartStatusManager partStat;
    
    [SerializeField] private Slider redSlider;
    [SerializeField] private Slider yellowSlider;
    [SerializeField] private TextMeshProUGUI partName;
    [SerializeField] private GameObject uiMask;
    [SerializeField] private GameObject hpGauge;
    [SerializeField] private GameObject brokenAnimGameObject;

    private Tween _gaugeAnimationTween;
    

    public void SetBrokenText(bool active)
    {
        if (active)
        {
            partName.color = Color.white;
        }
        else
        {
            partName.color = Color.gray;
        }
        
    }
    
    private void CheckPartActive(PartStatusManager part, bool active)
    {
        if (part.hasBroken)
        {
            SetBrokenText(false);
            uiMask.SetActive(true);
            hpGauge.SetActive(false);
            brokenAnimGameObject.SetActive(true);
            //brokenAnimGameObject.GetComponent<Animator>().Rebind();
            return;
        }

        if (active)
        {
            SetBrokenText(true);
            uiMask.SetActive(false);
        }
        else
        {
            SetBrokenText(false);
            uiMask.SetActive(true);
        }

    }

    private void Start()
    {
        partStat.OnHPDecrease += DoHPDecreaseAnimation;
        partStat.OnPartActive += CheckPartActive;
        partStat.OnFakeActive += CheckPartActive;
        partStat.OnPartBroken += BrokenInactive;
        
        partName.text = partStat.displayedName;
        if (partStat.gameObject.activeInHierarchy == false)
        {
            CheckPartActive(partStat,false);
        }
    }

    private void OnDestroy()
    {
        partStat.OnHPDecrease -= DoHPDecreaseAnimation;
        partStat.OnPartActive -= CheckPartActive;
        partStat.OnFakeActive -= CheckPartActive;
        partStat.OnPartBroken -= BrokenInactive;
    }

    private void Update()
    {
        if (partStat.hasBroken == false)
        {
            redSlider.value = GetCurrentHPFraction();
        }
        else
        {
            redSlider.value = 0;
            yellowSlider.value = 0;
        }
        
        
    }

    
    
    private void DoHPDecreaseAnimation(int dmg, AttackBase atk)
    {
        
        if(partStat.hasBroken)
            return;
        
        _gaugeAnimationTween?.Kill();
        
        _gaugeAnimationTween
            = yellowSlider.DOValue(GetCurrentHPFraction(),
            Mathf.Abs(yellowSlider.value - redSlider.value))
                .SetEase(Ease.Linear).SetDelay(0.15f);
        
    }

    private float GetCurrentHPFraction()
    {
        return (float)partStat.currentHp / (float)partStat.maxHP;
    }

    private void BrokenInactive(PartStatusManager part, int id)
    {
        SetBrokenText(false);
        uiMask.SetActive(false); 
    }
}
