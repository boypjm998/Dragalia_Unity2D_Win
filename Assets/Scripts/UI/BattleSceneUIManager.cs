using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class BattleSceneUIManager : MonoBehaviour
{
    private GameObject PauseMenu;
    private GameObject MenuButton;
    private BattleStageManager _stageManager;
    private PlayerInput pi;

    private void Awake()
    {
        PauseMenu = transform.Find("PauseMenu").gameObject;
        PauseMenu.SetActive(false);
        _stageManager = FindObjectOfType<BattleStageManager>();
        MenuButton = transform.Find("MenuButton").gameObject;
    }
    

    private void Start()
    {
        
        //yield return GlobalController.currentGameState == GlobalController.GameState.Inbattle;
        // PauseMenu = transform.Find("PauseMenu").gameObject;
        // PauseMenu.SetActive(false);
        // _stageManager = FindObjectOfType<BattleStageManager>();
        // MenuButton = transform.Find("MenuButton").gameObject;
        pi = FindObjectOfType<PlayerInput>();
    }

    private void Update()
    {
        if(GlobalController.currentGameState == GlobalController.GameState.WaitForStart)
            return;
        
        if (pi.buttonEsc.OnPressed)
        {
            if (_stageManager.isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                OpenPauseMenu();
            }
        }
    }

    public void OpenPauseMenu()
    {
        if (!_stageManager.isGamePaused)
        {
            _stageManager.SetGamePause(true);
            //Time.timeScale = 0;
            MenuButton.GetComponent<Button>().enabled = false;
            PauseMenu.SetActive(true);
            return;
        }
        Debug.LogWarning("GameIsAlreadyPaused");
    }
    public void ResumeGame()
    {
        if (_stageManager.isGamePaused)
        {
            _stageManager.SetGamePause(false);
            //Time.timeScale = 1;
            MenuButton.GetComponent<Button>().enabled = true;
            PauseMenu.SetActive(false);
            return;
        }
        Debug.LogWarning("GameIsPlaying");
    }

}
