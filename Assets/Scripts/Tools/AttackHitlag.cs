using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

public class AttackHitlag : MonoBehaviour
{
    [SerializeField] private bool particleSystemHitlag = false;
    [SerializeField] private bool animationHitlag = false;
    [SerializeField] private bool visualEffectHitlag = false;
    [SerializeField] private bool attackMotionLag = false;
    
    private AttackBase _attackBase;
    private int actionTween = 0;

    private ParticleSystem _particleSystem;
    private Animator _animator;
    private VisualEffect _visualEffect;
    private Animator _attackerAnimator;
    private Tween _tween;
    private Tween _tweenVFX;
    private Tween _tweenPS;
    
    private void Awake()
    {
        _attackBase = GetComponentInParent<AttackBase>();
        if (visualEffectHitlag)
        {
            _visualEffect = GetComponent<VisualEffect>();
        }

        if (particleSystemHitlag)
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        _attackBase.OnAttackHit += DoLag;

    }

    private void Start()
    {
        if (attackMotionLag)
        {
            _attackerAnimator = (_attackBase is AttackFromPlayer) ? 
                (_attackBase as AttackFromPlayer).ac.anim : 
                (_attackBase as AttackFromEnemy).GetComponent<EnemyController>().anim;
        }

        


    }

    private void OnDestroy()
    {
        _tween?.Complete();
        _tweenVFX?.Complete();
        _tweenPS?.Complete();
    }


    private void DoLag(AttackBase attackBase, GameObject other)
    {
        if(actionTween > 0)
            return;

        if (visualEffectHitlag)
        {
            _tweenVFX?.Complete();
            actionTween++;
            _visualEffect.playRate = 0f;
            _tweenVFX = DOVirtual.DelayedCall(0.1f, () =>
            {
                _visualEffect.playRate = 1f;
            },false).OnComplete(()=>actionTween--);
        }

        if (attackMotionLag)
        {
            _tween?.Complete();
            actionTween++;
            var initialSpeed = _attackerAnimator.speed;
            _attackerAnimator.speed = 0.1f;
            _tween = DOVirtual.DelayedCall(0.1f, () =>
            {
                _attackerAnimator.speed = initialSpeed;
            },false).OnComplete(()=>actionTween--);;
        }

        if (particleSystemHitlag)
        {
            _tweenPS?.Complete();
            actionTween++;
            var initialSpeed = _particleSystem.main.simulationSpeed;
            var psMain = _particleSystem.main;
            _tweenPS = DOVirtual.DelayedCall(0.1f, () =>
            {
                psMain.simulationSpeed = initialSpeed;
            },false).OnComplete(()=>actionTween--);;

        }


    }







}
