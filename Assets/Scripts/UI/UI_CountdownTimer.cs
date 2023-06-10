using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameMechanics;
public class UI_CountdownTimer : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;

    [SerializeField] private Sprite countdownSprite;

    private Image backgroundImage;
    
    private bool isCountdown = false;

    private float timeLimit = 0;

    private BattleStageManager _battleStageManager;

    private TextMeshProUGUI countdownText;
    private Coroutine warningRoutine;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        
        countdownText = GetComponentInChildren<TextMeshProUGUI>();
        _battleStageManager = FindObjectOfType<BattleStageManager>();
        yield return new WaitUntil(() => GlobalController.currentGameState == GlobalController.GameState.Inbattle);
        if (_battleStageManager.timeLimit > 0)
        {
            isCountdown = true;
            timeLimit = _battleStageManager.timeLimit;
            countdownText.text = BasicCalculation.ToTimerFormat(_battleStageManager.timeLimit);
        }
        else
        {
            isCountdown = false;
            countdownText.text = BasicCalculation.ToTimerFormat(0);
        }
        backgroundImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalController.currentGameState != GlobalController.GameState.Inbattle)
        {
            //enabled = false;
            return;
        }

        if (isCountdown)
        {
            var remainedTime = timeLimit - _battleStageManager.currentTime;
            if (remainedTime <= 0)
            {
                countdownText.text = BasicCalculation.ToTimerFormat(0);
                _battleStageManager.SetGameFailed();
                enabled = false;
                return;
            }

            if (remainedTime < 30 && warningRoutine==null)
            {
                warningRoutine = StartCoroutine(WarningRoutine());
            }

            countdownText.text = BasicCalculation.ToTimerFormat(remainedTime);
        }
        else
        {
            countdownText.text = BasicCalculation.ToTimerFormat(_battleStageManager.currentTime);
        }
    }

    IEnumerator WarningRoutine()
    {
        while (GlobalController.currentGameState == GlobalController.GameState.Inbattle)
        {
            backgroundImage.sprite = countdownSprite;
            yield return new WaitForSecondsRealtime(1.5f);
            backgroundImage.sprite = normalSprite;
            yield return new WaitForSecondsRealtime(1.5f);
        }

        warningRoutine = null;
    }

}
