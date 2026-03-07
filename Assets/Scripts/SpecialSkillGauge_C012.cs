using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SpecialSkillGauge_C012 : MonoBehaviour, ICharacterSpecialGauge
{
    public static SpecialSkillGauge_C012 Instance { get; private set; }
    
    [SerializeField] private Slider _gaugeSliderFront;
    [SerializeField] private Slider _gaugeSliderBack;
    private Image _gaugeSliderFrontImage;
    private Image _gaugeSliderBackImage;
    
    
    [SerializeField] private Image _gaugeCountSprite;
    [SerializeField] private Image _gaugeCountSpriteAnim;
    
    [SerializeField] private Color _fullFilledColor = new Color(1f,0.5f,0.2f);
    [SerializeField] private Color _normalColor = new Color(1,0.8f,0.2f);
    
    [SerializeField] private Sprite[] _gaugeCountSprites;
    
    public const float MaxSPPerLevel = 1129;
    private const int MaxGaugeLevel = 3;
    
    private float _currentSP;
    private ActorController_c012 _actorController;
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

    private void OnDestroy()
    {
        if(Instance == this)
            Instance = null;
    }

    public void SetActor(ActorController_c012 ac)
    {
        _actorController = ac;
    }


    public void Charge(int cp)
    {
        if(_currentSP >= MaxSPPerLevel * MaxGaugeLevel)
            return;

        _currentSP += cp;
        _currentSP = Mathf.Clamp(_currentSP, 0, MaxSPPerLevel * MaxGaugeLevel);
        _actorController.currentSP = _currentSP;
        
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
        
        
        _gaugeTween?.Kill(true);
        
        _gaugeTween = DOTween.To(() => _gaugeSliderFront.value,
            x => _gaugeSliderFront.value = x,
            _gaugeSliderBack.value, 0.2f);
        
        
        
    }

    public void ConsumeOneLevel()
    {
        _gaugeTween?.Kill(true);
        
        //_gaugeAnimator.Play("idle");
        
        if (_actorController.currentSP < MaxSPPerLevel)
        {
            _currentSP = 0;
        }else
        {
            _currentSP -= MaxSPPerLevel;
        }
        
        _actorController.currentSP = _currentSP;
        
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

    public void ChargeTo(int cp, int level = 0)
    {
        
    }

    public void ResetGauge()
    {
        
    }
}
