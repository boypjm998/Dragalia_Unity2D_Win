using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameMechanics;

public class ConditionIcon : MonoBehaviour
{
    // Start is called before the first frame update
    public int order;

    protected UI_ConditionBar parentBar;

    [HideInInspector] public StatusManager _statusManager;
    public BattleCondition conditionInfo;
    private GameObject textObj;
    private TextMeshProUGUI _text;
    private Slider slider;
    private List<BattleCondition> list;
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        list = new List<BattleCondition>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        parentBar = GetComponentInParent<UI_ConditionBar>();
        textObj = transform.Find("Text").gameObject;
        _text = textObj.GetComponent<TextMeshProUGUI>();
        slider = transform.Find("Inner").GetComponentInChildren<Slider>();
        
    }

    // Update is called once per frame
    void Update()
    {
        DisplayConditionInfo();
        GetGaugeTimeInfo();
    }

    void DisplayConditionInfo()
    {
        if (conditionInfo.DisplayType == BattleCondition.buffEffectDisplayType.StackNumber)
        {
            int cnt = _statusManager.GetConditionStackNumber(conditionInfo.buffID);
            if (cnt == 1)
            {
                textObj.SetActive(false);
                return;
            }
            textObj.SetActive(true);
            StringBuilder sb = new StringBuilder();
            sb.Append("×");
            sb.Append(cnt.ToString());
            _text.text = sb.ToString();
        }
        else if (conditionInfo.DisplayType == BattleCondition.buffEffectDisplayType.Value)
        {
            textObj.SetActive(true);
            int val = (int)(_statusManager.GetConditionTotalValue(conditionInfo.buffID));
            if (val > BasicCalculation.BattleConditionLimit(conditionInfo.buffID))
            {
                val = (int)BasicCalculation.BattleConditionLimit(conditionInfo.buffID);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(val.ToString());
            sb.Append("%");
            _text.text = sb.ToString();
        }else if (conditionInfo.DisplayType == BattleCondition.buffEffectDisplayType.ExactValue)
        {
            textObj.SetActive(true);
            int val = (int)(_statusManager.GetConditionTotalValue(conditionInfo.buffID));
            if (val > BasicCalculation.BattleConditionLimit(conditionInfo.buffID))
            {
                val = (int)BasicCalculation.BattleConditionLimit(conditionInfo.buffID);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(val.ToString());
            _text.text = sb.ToString();
        }else if (conditionInfo.DisplayType == BattleCondition.buffEffectDisplayType.Level)
        {
            textObj.SetActive(true);
            int val = (int)(_statusManager.GetConditionTotalValue(conditionInfo.buffID));
            if (val > BasicCalculation.BattleConditionLimit(conditionInfo.buffID))
            {
                val = (int)BasicCalculation.BattleConditionLimit(conditionInfo.buffID);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Lv." + val.ToString());
            _text.text = sb.ToString();
        }
    }

    void GetGaugeTimeInfo()
    {
        //var list = _statusManager.GetConditionOfType(conditionInfo.buffID);
        
        //list.Sort(CompareTime);
        
        if (list[0].duration > 0)
        {
            float gaugeValue = (list[0].lastTime / list[0].duration);
            slider.value = gaugeValue;
        }
        else
        {
            slider.value = 1;
        }

    }

    public int OnConditionRemove()
    {
        list = _statusManager.GetConditionsOfType(conditionInfo.buffID);
        list.Sort(CompareTime);
        return _statusManager.GetConditionStackNumber(conditionInfo.buffID);
    }

    public void OnConditionAdd()
    {
        list = _statusManager.GetConditionsOfType(conditionInfo.buffID);
        list.Sort(CompareTime);
        //throw new NotImplementedException();
    }

    private int CompareTime(BattleCondition a, BattleCondition b)
    {
        if (a.lastTime > 0 && b.lastTime > 0)
        {
            if (a.lastTime > b.lastTime)
            {
                return 1;
            }else if (a.lastTime < b.lastTime)
            {
                return -1;
            }
            else return 0;
        }else if (a.lastTime < 0 && b.lastTime > 0)
        {
            return 1;
        }else if (a.lastTime > 0 && b.lastTime < 0)
        {
            return -1;
        }
        else return 0;
    }

}
