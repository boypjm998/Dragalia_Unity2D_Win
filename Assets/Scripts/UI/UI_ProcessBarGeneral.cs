using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_ProcessBarGeneral : MonoBehaviour
{
    public float countdownTime = 10f;
    public bool isCountdown = false;
    public float fadeTweenTime = 1f;
    public bool fadeIn = false;
    public bool fadeOut = false;
    public Color backgroundColor = Color.white;
    public Color initialColor = Color.white;
    public Action<GameObject> onCountdownEnd;

    [Serializable]
    public class ColorGroup
    {
        public Color color;
        [Range(0,1f)]public float normalizedTime;
    }
    
    public List<ColorGroup> colorGroup = new();
    
    
    private float currentTime = 0f;
    private int progress = 0;
    private Slider _slider;
    private CanvasGroup _canvasGroup;
    private Image _image;
    private Image _fillImage;
    private Tweener _tweener;
    
    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _image = GetComponent<Image>();
        _fillImage = transform.GetChild(0).GetComponent<Image>();
    }
    
    private IEnumerator Start()
    {
        _slider.value = 0f;
        colorGroup.Sort((a, b) => a.normalizedTime.CompareTo(b.normalizedTime));
        //colorGroup.Reverse();
        if (isCountdown)
        {
            _slider.value = 1f;
        }
        if (fadeIn)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, fadeTweenTime);
            yield return new WaitForSeconds(fadeTweenTime);
        }
        

        _image.color = backgroundColor;
        progress = 1;
    }

    private void Update()
    {
        if (progress == 1)
        {
            UpdateColor();
            if (isCountdown)
            {
                _slider.value = 1 - currentTime / countdownTime;
            }
            else
            {
                _slider.value = currentTime / countdownTime;
            }
            
            currentTime += Time.deltaTime;
            
            if (currentTime >= countdownTime)
            {
                progress = 2;
                if (fadeOut)
                {
                    _tweener = _canvasGroup.DOFade(0f, fadeTweenTime).
                        OnComplete(()=>
                        {
                            enabled = false;
                            onCountdownEnd?.Invoke(gameObject);
                        });
                }
                else
                {
                    _image.color = Color.clear;
                    enabled = false;
                    onCountdownEnd?.Invoke(gameObject);
                }
                
            }
        }
    }
    
    private void UpdateColor()
    {
        if (colorGroup.Count == 0)
        {
            return;
        }
        
        //对colorGroup进行降序排序
        
        
        if(currentTime/countdownTime >= colorGroup[0].normalizedTime)
        {
            _fillImage.color = colorGroup[0].color;
            colorGroup.RemoveAt(0);
            return;
        }
    }
}
