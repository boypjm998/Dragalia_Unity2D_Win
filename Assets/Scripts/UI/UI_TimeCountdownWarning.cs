using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_TimeCountdownWarning : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private Animation _animation;
    private TextMeshProUGUI _tmp;
    private bool[] flags = new bool[] { false, false, false };

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _animation = GetComponent<Animation>();
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
    }


    // Start is called before the first frame update
    void Start()
    {
        _canvasGroup.alpha = 0;
        if (BattleStageManager.Instance.timeLimit < 0)
        {
            enabled = false;
            return;
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        var remainedTime = BattleStageManager.Instance.timeLimit - BattleStageManager.Instance.currentTime;
        if (remainedTime <= 61 && !flags[0] && BattleStageManager.Instance.timeLimit > 60)
        {
            flags[0] = true;
            SetText(60);
            _animation.Play();
            _canvasGroup.alpha = 1;
            Invoke("DisableCanvas",2);
        }
        else if (remainedTime <= 31 && !flags[1] && BattleStageManager.Instance.timeLimit > 30)
        {
            flags[1] = true;
            SetText(30);
            _animation.Play();
            _canvasGroup.alpha = 1;
            Invoke("DisableCanvas",2);
        }else if (remainedTime <= 11 && !flags[2] && BattleStageManager.Instance.timeLimit > 10)
        {
            flags[2] = true;
            SetText(10);
            _animation.Play();
            _canvasGroup.alpha = 1;
            Invoke("DisableCanvas",2);
        }
    }

    private void DisableCanvas()
    {
        _canvasGroup.alpha = 0;
    }

    private void SetText(int timeLeft)
    {
        if (GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
        {
            _tmp.text = $"还剩 {timeLeft} 秒";
        }
        else
        {
            _tmp.text = $" {timeLeft} Seconds Left";
        }
    }
}
