using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class ComboIndicatorUI : MonoBehaviour
{
    [SerializeField] private GameObject comboRemainGauge;
    [SerializeField] private GameObject comboCount;
    [SerializeField] private GameObject hitText;
    [SerializeField] private GameObject hitsText;

    private Slider gaugeSlider;
    private PlayerStatusManager _playerStatusManager;
    private TextMeshProUGUI comboTMP;
    
    void Start()
    {
        gaugeSlider = comboRemainGauge.GetComponentInChildren<Slider>();
        _playerStatusManager = GameObject.Find("PlayerHandle").GetComponentInChildren<PlayerStatusManager>();
        _playerStatusManager._comboIndicator = this;
        comboTMP = comboCount.GetComponentInChildren<TextMeshProUGUI>();
        comboTMP.text = "";
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        GaugeControl();
    }

    public void PrintComboNum()
    {
        gameObject.SetActive(true);
        comboTMP.text = Number2TMP(_playerStatusManager.comboHitCount);

    }

    public void HideComboNum()
    {
        gameObject.SetActive(false);
    }

    private void GaugeControl()
    {
        var value = _playerStatusManager.lastComboRemainTime / _playerStatusManager.comboConnectMaxInterval;
        if (value > 0.8f)
        {
            gaugeSlider.value = 1;
        }
        else
        {
            gaugeSlider.value = value / 0.8f;
        }

        
    }

    private string Number2TMP(int count)
    {
        StringBuilder sb = new StringBuilder(10);
        if (count > 999)
            count = 999;

        if (count == 1)
        {
            hitsText.SetActive(false);
            hitText.SetActive(true);
        }
        else
        {
            hitText.SetActive(false);
            hitsText.SetActive(true);
        }

        
        
        string str = count.ToString();
        int len = str.Length;
        for (int i = 0; i < len; i++)
        {
            sb.Append($"<sprite={str[i]}>");
        }
        return sb.ToString();
        
    }

}
