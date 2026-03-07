using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimation;
using TMPro;
using UnityEngine;

public class UI_DrasticForce : MonoBehaviour
{
    [SerializeField] private GameObject ringGO;
    [SerializeField] private TextMeshPro stackCountText;
    private UI_RingSlider _ringSlider;
    private PlayerStatusManager _statusManager;

    private void Start()
    {
        _ringSlider = ringGO.GetComponent<UI_RingSlider>();
        ringGO.SetActive(false);
        _ringSlider.maxValue = DrasticForce.Instance.duration;
        DrasticForce.Instance.OnResetDuration += ResetDuration;
        _statusManager = GetComponentInParent<PlayerStatusManager>();
    }

    private void Update()
    {
        if(DrasticForce.Instance == null)
            return;
        
        SetRingSliderAttributes();
    }

    private void OnDestroy()
    {
        DrasticForce.Instance.OnResetDuration -= ResetDuration;
    }

    private void ResetDuration(float duration)
    {
        _ringSlider.maxValue = duration;
    }

    private void SetRingSliderAttributes()
    {
        _ringSlider.currentValue = DrasticForce.Instance.LeftTime;
        stackCountText.text = $"×{DrasticForce.Instance.StackCount}";
        if (DrasticForce.Instance.StackCount > 0)
        {
            ringGO.SetActive(true);
            _statusManager.LifeStealBlock = true;
        }
        else
        {
            _statusManager.LifeStealBlock = false;
            ringGO.SetActive(false);
        }
            
    }
    
}
