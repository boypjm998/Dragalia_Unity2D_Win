using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Armament gauge.
/// </summary>
public class ArmorGauge : MonoBehaviour, ICharacterSpecialGauge
{
    public int MaxSPPerLevel = 2000;
    
    [Tooltip("1: Finni; 2:Eirene")]
    public int characterID;
    
    public static ArmorGauge Instance { get; private set; }
    
    [SerializeField] private Slider _gaugeSliderFront;
    [SerializeField] private Slider _gaugeSliderBack;
    private Image _gaugeSliderFrontImage;
    private Image _gaugeSliderBackImage;
    
    
    [SerializeField] private Image _gaugeCountSprite;
    [SerializeField] private Image _gaugeCountSpriteAnim;
    
    [SerializeField] private Color _fullFilledColor = new Color(1f,0.5f,0.2f);
    [SerializeField] private Color _normalColor = new Color(1,0.8f,0.2f);
    
    [SerializeField] private Sprite[] _gaugeCountSprites;
    
    private const int MaxGaugeLevel = 2;
    
    private float _currentSP;
    private AttackManager _attackManager;
    private Tweener _gaugeTween;
    private Animator _gaugeAnimator;
    
    private void Awake()
    {
        Instance = this;
        _gaugeSliderFront.value = 0;
        _gaugeSliderBack.value = 0;
        _gaugeAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        _gaugeSliderBackImage = _gaugeSliderBack.GetComponentInChildren<Image>();
        _gaugeSliderFrontImage = _gaugeSliderFront.GetComponentInChildren<Image>();
        
    }

    public void SetActor(AttackManager am)
    {
        _attackManager = am;
    }
    
    private void OnDestroy()
    {
        if(Instance == this)
            Instance = null;
    }

    public void Charge(int cp)
    {
        if(_currentSP >= MaxSPPerLevel * MaxGaugeLevel)
            return;

        _currentSP += cp;
        _currentSP = Mathf.Clamp(_currentSP, 0, MaxSPPerLevel * MaxGaugeLevel);
        
        //todo: Rewrite this line
        //_actorController.currentSP = _currentSP;
        SetActorControllerSPToGaugeSP();
        
        //将_gaugeSliderBack的值设置为当前SP，若当前SP的值大于MaxSPPerLevel，则进行换算。
        //若_currentSP的值大于MaxSPPerLevel * MaxGaugeLevel，则将_currentSP的值设置为MaxSPPerLevel * MaxGaugeLevel。
        //否则对其取模MaxSPPerLevel。
        var value = _currentSP >= MaxSPPerLevel ? _currentSP % MaxSPPerLevel : _currentSP;
        
        //如果新value小于原value，那么槽会进位，那么动画需要瞬间到0再上升。

        if (value / MaxSPPerLevel < _gaugeSliderBack.value)
        {
            _gaugeSliderFront.value = 0;
            
            _gaugeAnimator.Play("charge");
        }

        _gaugeSliderBack.value = value / MaxSPPerLevel;
        
        //根据其num数值设置_gaugeCountSprite的sprite。

        var stack = Mathf.FloorToInt(_currentSP / MaxSPPerLevel);
        if (stack >= MaxGaugeLevel)
        {
            stack = MaxGaugeLevel;
            _gaugeSliderBack.value = 1;
            _gaugeSliderFrontImage.color = _fullFilledColor;
        }
        else
        {
            _gaugeSliderFrontImage.color = _normalColor;
        }
        _gaugeCountSprite.sprite = _gaugeCountSprites[stack];
        _gaugeCountSpriteAnim.sprite = _gaugeCountSprites[stack];
        
        
        //_gaugeTween?.Kill(true);
        
        _gaugeSliderFront.value = _gaugeSliderBack.value;
        
        // _gaugeTween = DOTween.To(() => _gaugeSliderFront.value,
        //     x => _gaugeSliderFront.value = x,
        //     _gaugeSliderBack.value, 0.2f);
        
        
        
    }

    public virtual void ConsumeOneLevel()
    {
        _gaugeTween?.Kill(true);
        
        //_gaugeAnimator.Play("idle");
        
        //todo: Rewrite this line
        SyncGaugeSP();
        
        var value = _currentSP >= MaxSPPerLevel ? _currentSP % MaxSPPerLevel : _currentSP;
        
        var stack = Mathf.FloorToInt(_currentSP / MaxSPPerLevel);
        
        
        _gaugeSliderBack.value = value / MaxSPPerLevel;
        _gaugeSliderFront.value = value / MaxSPPerLevel;
        
        if (stack >= MaxGaugeLevel)
        {
            stack = MaxGaugeLevel;
            _gaugeSliderBack.value = 1;
            _gaugeSliderFrontImage.color = _fullFilledColor;
        }
        else
        {
            _gaugeSliderFrontImage.color = _normalColor;
        }
        
        _gaugeCountSprite.sprite = _gaugeCountSprites[stack];
        _gaugeCountSpriteAnim.sprite = _gaugeCountSprites[stack];
        
        
    }

    protected virtual void ClearOneSpLevel()
    {
        
    }

    public void ChargeTo(int cp, int level = 0)
    {
        if(_currentSP > MaxSPPerLevel * MaxGaugeLevel)
            return;

        var lastSP = _currentSP;
        
        _currentSP = cp;

        //todo: Rewrite this line
        //_actorController.currentSP = _currentSP;
        SetActorControllerSPToGaugeSP();
        
        //将_gaugeSliderBack的值设置为当前SP，若当前SP的值大于MaxSPPerLevel，则进行换算。
        //若_currentSP的值大于MaxSPPerLevel * MaxGaugeLevel，则将_currentSP的值设置为MaxSPPerLevel * MaxGaugeLevel。
        //否则对其取模MaxSPPerLevel。
        var value = _currentSP >= MaxSPPerLevel ? _currentSP % MaxSPPerLevel : _currentSP;
        
        var oldValue = lastSP >= MaxSPPerLevel ? lastSP % MaxSPPerLevel : lastSP;
        
        //如果新value小于原value，那么槽会进位，那么动画需要瞬间到0再上升。

        if (value / MaxSPPerLevel < oldValue / MaxSPPerLevel)
        {
            //_gaugeSliderFront.value = 0;
            
            _gaugeAnimator.Play("charge");
        }

        _gaugeSliderBack.value = value / MaxSPPerLevel;
        
        //根据其num数值设置_gaugeCountSprite的sprite。

        var stack = Mathf.FloorToInt(_currentSP / MaxSPPerLevel);
        if (stack >= MaxGaugeLevel)
        {
            stack = MaxGaugeLevel;
            _gaugeSliderBack.value = 1;
            _gaugeSliderFrontImage.color = _fullFilledColor;
        }
        else
        {
            _gaugeSliderFrontImage.color = _normalColor;
        }
        _gaugeCountSprite.sprite = _gaugeCountSprites[stack];
        _gaugeCountSpriteAnim.sprite = _gaugeCountSprites[stack];
        
        
        //_gaugeTween?.Kill(true);
        
        _gaugeSliderFront.value = _gaugeSliderBack.value;
        
        // _gaugeTween = DOTween.To(() => _gaugeSliderFront.value,
        //     x => _gaugeSliderFront.value = x,
        //     _gaugeSliderBack.value, 0.2f);
    }

    public void ResetGauge()
    {
        
    }

    public virtual void SyncGaugeSP()
    {
        if (characterID == 2)
        {
            (_attackManager as AttackManager_C052).currentSP = _currentSP;


        }
    //_actorController.currentSP = _currentSP;
    }

    public virtual void SetActorControllerSPToGaugeSP()
    {
        if (characterID == 2)
        {
            var am = _attackManager as AttackManager_C052;
            
            _currentSP = am.currentSP;
        }
        
        
    }
    
}
