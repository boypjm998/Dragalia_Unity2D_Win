using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Laxi's Charge Gauge - 莱姬希的共鸣槽
/// </summary>
public class ChargeGauge_C015 : MonoSingleton<ChargeGauge_C015>, ICharacterSpecialGauge
{
    private const int MaxCp = 100;
    public int currentCp = 0;

    /// <summary>
    /// arg1: currentCp, arg2: delta
    /// </summary>
    public event Action<int,int> OnCharge;

    public event Action<int> OnLevelChange; 

    /// <summary>
    /// 共鸣槽充能时白色底色的充能槽
    /// </summary>
    [SerializeField] private Slider backGauge;

    /// <summary>
    /// 共鸣槽充能时的前景色充能槽
    /// </summary>
    [SerializeField] private Slider[] frontGauges;

    /// <summary>
    /// 充能完毕时，最外侧闪光动画的充能槽
    /// </summary>
    [SerializeField] private Slider gaugeFillAnimationGauge;

    private Animator _animator;


    /// <summary>
    /// 当充能槽充能时，背景色先填充，前景色后填充
    /// </summary>
    private Tween _gaugeFillTween;
    
    
    

    private void Start()
    {
        _animator = GetComponent<Animator>();
        OnCharge += GaugeAnimation;
        
        backGauge.value = 0;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnCharge -= GaugeAnimation;
    }

    private void GaugeAnimation(int cpBefore, int cpIncrement)
    {
        if(cpBefore == 100 && cpIncrement > 0)
            return;


        if (currentCp == 0)
        {
            backGauge.value = 0;
            frontGauges[0].value = 0;
            frontGauges[1].value = 0;
            frontGauges[2].value = 0;
            OnLevelChange?.Invoke(0);
            return;
        }
        
        
        // 1. 立即更新背景Slider（无动画）
        backGauge.value = currentCp;

        _gaugeFillTween = DOVirtual.DelayedCall(0.1f, () =>
        {
            if (currentCp > 66)
            {
                frontGauges[0].value = 33;
                frontGauges[1].value = 33;
                frontGauges[2].value = currentCp - 66;
            }
            else if (currentCp > 33)
            {
                frontGauges[0].value = 33;
                frontGauges[1].value = currentCp - 33;
                frontGauges[2].value = 0;
            }
            else
            {
                frontGauges[0].value = currentCp;
                frontGauges[1].value = 0;
                frontGauges[2].value = 0;
            }
        }, false);
        
        


        if (currentCp >= 33 && cpBefore < 33)
        {
            gaugeFillAnimationGauge.value = 33;
            _animator.Play("filled",0,0);
            OnLevelChange?.Invoke(1);
        }
        else if (currentCp >= 66 && cpBefore < 66)
        {
            gaugeFillAnimationGauge.value = 66;
            _animator.Play("filled",0,0);
            OnLevelChange?.Invoke(2);
        }
        else if (currentCp >= 100 && cpBefore < 100)
        {
            gaugeFillAnimationGauge.value = 100;
            _animator.Play("filled",0,0);
            OnLevelChange?.Invoke(3);
        }
        
        
    }


    
    

    public void Charge(int cp)
    {
        var beforeCp = currentCp;
        currentCp = Mathf.Clamp(currentCp + cp, 0, MaxCp);
        OnCharge?.Invoke(beforeCp, cp);
    }

    public void ChargeTo(int cp, int level = 0)
    {
        cp += level * 33;
        Charge(cp);
    }

    public void ResetGauge()
    {
        currentCp = 0;
        backGauge.value = 0;
        frontGauges[0].value = 0;
        frontGauges[1].value = 0;
        frontGauges[2].value = 0;
        OnCharge?.Invoke(currentCp, -currentCp);
        
    }

    // 辅助方法：获取指定段应该显示的值
    private float GetSegmentValue(int cp, int segmentIndex)
    {
        int segmentStart = segmentIndex == 0 ? 0 : segmentIndex == 1 ? 33 : 66;
        int segmentEnd = segmentIndex == 0 ? 33 : segmentIndex == 1 ? 66 : 100;
    
        if (cp <= segmentStart) return 0;
        if (cp >= segmentEnd) return segmentIndex == 2 ? 34 : 33;
    
        return cp - segmentStart;
    }


    // 辅助方法：检测指定段是否在满状态
    private bool IsSegmentFull(float value, int segmentIndex)
    {
        return Mathf.Approximately(value, segmentIndex == 2 ? 34 : 33);
    }


    // 辅助方法：检测指定段当前是否处于最大值
    private bool IsSegmentAtMax(int cp, int segmentIndex)
    {
        int threshold = segmentIndex == 0 ? 33 : segmentIndex == 1 ? 66 : 100;
        return cp >= threshold;
    }

    private int GetLevel(int cp)
    {
        if(cp < 33)
            return 0;
        else if (cp < 66)
            return 1;
        else if (cp < 100)
            return 2;
        else return 3;
    }
    
}
