using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelBannerLock : MonoBehaviour
{
    [SerializeField] private string prequisiteLevelID;
    [SerializeField] private bool hideIfLocked = true;
    
    private List<QuestSave> questSaveList = new();
    private Image bannerImage;
    private Button enterButton;
    private TextMeshProUGUI unlockText;
    private GlobalController.Language currentLanguage;
    
    private void Awake()
    {
        currentLanguage = GlobalController.Instance.GameLanguage;
        questSaveList = GlobalController.Instance.GetQuestInfo();
        bool unlocked = CheckUnLock();

        if (hideIfLocked == false)
        {
            InitAllElements();
            if (unlocked == false)
            {
                SetElementsToLocked();
            }
            else
            {
                //unlockText.fontSize = 18;
                //unlockText.text = "";
            }
        }
        else
        {
            if(unlocked == false)
                gameObject.SetActive(false);
        }

    }

    private void InitAllElements()
    {
        bannerImage = transform.Find("Image").GetComponent<Image>();
        enterButton = GetComponentInChildren<Button>();
        unlockText = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    private void SetElementsToLocked()
    {
        bannerImage.color = Color.gray;
        enterButton.interactable = false;

        var questData = GlobalController.Instance.QuestData;
        var needClearQuestName = questData[$"QUEST_{prequisiteLevelID}"]["name"].ToString();

        if (currentLanguage == GlobalController.Language.ZHCN)
        {
            unlockText.text = $"通关“{needClearQuestName}”后解锁";
        }
        else
        {
            unlockText.text = $"Clear \"{needClearQuestName}\" to unlock";
        }
        unlockText.fontSize = 18;
        unlockText.enableAutoSizing = true;
        unlockText.fontSizeMax = 18;
        unlockText.fontSizeMin = 15;
    }
    
    private bool CheckUnLock()
    {
        if (questSaveList.Exists(x => x.quest_id == prequisiteLevelID))
        {
            return true;
        }
        else
        {
            return false;
        }
    }



}
