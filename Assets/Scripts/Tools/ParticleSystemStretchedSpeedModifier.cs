using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemStretchedSpeedModifier : MonoBehaviour
{
    [SerializeField] private Transform _transformParent;
    
    private ParticleSystem _particleSystem;
    private ParticleSystem.VelocityOverLifetimeModule _velocityOverLifetimeModule;
    
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _velocityOverLifetimeModule = _particleSystem.velocityOverLifetime;
    }

    private void Start()
    {
        if (_transformParent.localScale.x < 0)
        {
            // set the speed modifier to negative to make the particles go in the opposite direction
            // Note that we need to process multiple situations here,
            // 1. Constant speed
            // 2. Between two constants
            // 3. Curve
            
            // Only modify the x axis
            
            // 1. Constant speed
            if (_velocityOverLifetimeModule.x.mode == ParticleSystemCurveMode.Constant)
            {
                _velocityOverLifetimeModule.x = new ParticleSystem.MinMaxCurve(-_velocityOverLifetimeModule.x.constant);
            }
            
            // 2. Between two constants
            else if (_velocityOverLifetimeModule.x.mode == ParticleSystemCurveMode.TwoConstants)
            {
                _velocityOverLifetimeModule.x = new ParticleSystem.MinMaxCurve(-_velocityOverLifetimeModule.x.constantMin, -_velocityOverLifetimeModule.x.constantMax);
            }
            
            // 3. Curve
            else if (_velocityOverLifetimeModule.x.mode == ParticleSystemCurveMode.Curve)
            {
                AnimationCurve curve = _velocityOverLifetimeModule.x.curve;
                float multiplier = _velocityOverLifetimeModule.xMultiplier;
                for (int i = 0; i < curve.keys.Length; i++)
                {
                    curve.keys[i].value = -curve.keys[i].value;
                }

                _velocityOverLifetimeModule.x = new ParticleSystem.MinMaxCurve(multiplier, curve);
            }
            
            
            
            
            
            
            
        }
    }
}
