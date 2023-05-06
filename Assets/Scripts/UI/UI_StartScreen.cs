using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UI_StartScreen : MonoBehaviour
{
    private BattleStageManager _battleStageManager;
    public void FadeOut()
    {
        var blackScreen = GetComponent<Image>();
        var text1 = transform.Find("Text1").GetComponent<TextMeshProUGUI>();
        var text2 = transform.Find("Text2").GetComponent<TextMeshProUGUI>();
        var text3 = transform.Find("TimeLimit").GetComponent<TextMeshProUGUI>();
        var line = transform.Find("Line").GetComponent<Image>();


        DOTween.To(() => blackScreen.color, x => blackScreen.color = x,
            new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0),
            1f);
        DOTween.To(() => text1.color, x => text1.color = x,
            new Color(text1.color.r, text1.color.g, text1.color.b, 0),
            1f);
        DOTween.To(() => text2.color, x => text2.color = x,
            new Color(text2.color.r, text2.color.g, text2.color.b, 0),
            1f);
        DOTween.To(() => text3.color, x => text3.color = x,
            new Color(text3.color.r, text3.color.g, text3.color.b, 0),
            1f);
        DOTween.To(() => line.color, x => line.color = x,
            new Color(line.color.r, line.color.g, line.color.b, 0),
            1f);

        var StartText = GameObject.Find("UIFXContainer").transform.Find("START");
        StartText.gameObject.SetActive(true);
        DestroyMe();
    }

    protected void DestroyMe()
    {
        Destroy(gameObject,1.1f);
    }

    private void OnEnable()
    {
        _battleStageManager = BattleStageManager.Instance;
        transform.Find("Text1").GetComponent<TextMeshProUGUI>().text = _battleStageManager.quest_name;
        var text2 = transform.Find("Text2").GetComponent<TextMeshProUGUI>();
        //三语本地化
        //CHINESE
        string reviveTime;
        if (_battleStageManager.maxReviveTime == 0)
        {
            reviveTime = "无法重生";
        }
        else
        {
            reviveTime = $"最多{_battleStageManager.maxReviveTime}次";
        }
        text2.text = $"通关条件: 规定时间内，打倒BOSS!\n\n重生次数: {reviveTime}";
        //ENGLISH
        //JAPANESE

        var timeLimit = transform.Find("TimeLimit").GetComponent<TextMeshProUGUI>();
        //string time;
        if (_battleStageManager.timeLimit < 0)
        {
            timeLimit.text = $"<sprite=0> ∞";
        }
        else
        {
            var min = (int)Mathf.Floor(_battleStageManager.timeLimit / 60);
            var sec = _battleStageManager.timeLimit % 60;
            timeLimit.text = $"<sprite=0> {min:D2}:{sec:D2}";
            //battleStageManager.StartCountDown(); //开始计时
        }
    }
}
