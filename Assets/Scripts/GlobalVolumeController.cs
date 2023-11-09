using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class GlobalVolumeController : MonoBehaviour
{
    Volume volume;
    private Volume specialVolume;
    Tweener specialVolumeTweener;

    public static GlobalVolumeController Instance { get; private set; }

    void Start()
    {
        volume = GetComponent<Volume>();
        specialVolume = transform.parent.Find("SpecialVolume").GetComponent<Volume>();
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    // Update is called once per frame
    public void SetVolumeIntensity(float intensity)
    {
        

    }

    public void SpecialVolumeIntensityDoFade(float endValue, float time)
    {
        specialVolumeTweener = DOTween.To(() => specialVolume.weight,
            x => specialVolume.weight = x, endValue, time);
    }

    public void SetSpecialVolume(VolumeProfile profile)
    {
        specialVolume.profile = profile;
    }
}
