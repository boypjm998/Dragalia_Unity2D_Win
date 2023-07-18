using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;

public class UI_KeySelectionInTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        
    }

    public void SelectNewKeySettings(int settingsGroupID)
    {
        if(GlobalController.Instance.loadingEnd==false)
            return;
        
        canvasGroup.interactable = false;
        TutorialLevelManager.Instance.ResetKeySettingsToDefault(settingsGroupID);
    }

    public void FadeOut()
    {
        if(GlobalController.Instance.loadingEnd==false)
            return;
        
        TutorialLevelManager.Instance.HideCharacterUI();
        canvasGroup.interactable = false;
        var _tweener = canvasGroup.DOFade(0, 1f).OnComplete
        (() =>
        {
            TutorialLevelManager.Instance.ReadyToStartGame();
            gameObject.SetActive(false);
            Destroy(gameObject,0.5f);
        }
            );
    }


}
