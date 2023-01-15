using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PauseMenu : MonoBehaviour
{
    private BattleSceneUIManager _battleSceneUIManager;
    private void Start()
    {
        _battleSceneUIManager = transform.parent.GetComponent<BattleSceneUIManager>();
        var str = FindObjectOfType<BattleStageManager>().quest_name;
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
