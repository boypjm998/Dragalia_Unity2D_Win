using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PauseMenu : MonoBehaviour
{
    private UIManager _uiManager;
    private void Start()
    {
        _uiManager = transform.parent.GetComponent<UIManager>();
        var str = FindObjectOfType<BattleStageManager>().quest_name;
    }

    public void CloseMenu()
    {
        _uiManager.ResumeGame();
    }
}
