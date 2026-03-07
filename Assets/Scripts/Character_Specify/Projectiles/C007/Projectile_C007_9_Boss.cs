using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rain Effect
/// </summary>
public class Projectile_C007_9_Boss : MonoSingleton<Projectile_C007_9_Boss>
{
    [SerializeField] private ParticleSystem _rainEffect;
    private Color _startColor;

    private void Start()
    {
        _startColor = _rainEffect.main.startColor.colorMin;
    }

    public void PlayRainEffect()
    {
        _rainEffect.Play();
    }
    
    public void StopRainEffect()
    {
        _rainEffect.Stop();
    }
    
    public void ResetStartColorGradient()
    {
        var main = _rainEffect.main;
        var startColor = main.startColor;
        startColor.colorMin = _startColor;
        main.startColor = startColor;
    }
    
    public void SetStartColorGradient(Color leftColor)
    {
        var main = _rainEffect.main;
        var startColor = main.startColor;
        startColor.colorMin = leftColor;
        main.startColor = startColor;
    }
    
    public void SetRainRate(int emissionPerSecond)
    {
        var emission = _rainEffect.emission;
        emission.rateOverTime = emissionPerSecond;
    }
    
    
}
