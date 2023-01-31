using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PauseMenu : MonoBehaviour
{
    private BattleSceneUIManager _battleSceneUIManager;
    private string quest_name;

    private TextMeshProUGUI questNameText;
    private void Start()
    {
        _battleSceneUIManager = transform.parent.GetComponent<BattleSceneUIManager>();
        questNameText = transform.Find("Borders").Find("Banner1").GetComponentInChildren<TextMeshProUGUI>();
        quest_name = FindObjectOfType<BattleStageManager>().quest_name;
    }

    private void OnEnable()
    {
        //questNameText.text = quest_name;
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
        FindObjectOfType<GlobalController>().TestReturnMainMenu();
    }
}
