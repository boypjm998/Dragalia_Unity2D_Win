using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PauseMenu : MonoBehaviour
{
    protected BattleSceneUIManager _battleSceneUIManager;
    protected string quest_name;
    protected BattleStageManager _battleStageManager;
    protected TextMeshProUGUI questNameText;
    
    private void Awake()
    {
        _battleSceneUIManager = transform.parent.GetComponent<BattleSceneUIManager>();
        questNameText = transform.Find("Borders").Find("Banner1").GetComponentInChildren<TextMeshProUGUI>();
        //quest_name = FindObjectOfType<BattleStageManager>().quest_name;
        _battleStageManager = FindObjectOfType<BattleStageManager>();
    }

    private void OnEnable()
    {
        quest_name = _battleStageManager.quest_name;
        questNameText.text = quest_name;
    }

    public void CloseMenu()
    {
        _battleSceneUIManager.ResumeGame();
    }

    public void ReturnToMainMenu()
    {
        var buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
        _battleStageManager.OnQuestQuit?.Invoke(_battleStageManager.quest_id);
        FindObjectOfType<GlobalController>().TestReturnMainMenu();
    }

    public virtual void UpdatePanel(string[] args)
    {
        
    }

}
