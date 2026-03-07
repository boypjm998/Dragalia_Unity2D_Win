using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelBannerLock : MonoBehaviour
{
    [SerializeField] private string prequisiteLevelID;
    [SerializeField] protected bool hideIfLocked = true;
    
    protected List<QuestSave> questSaveList = new();
    protected Image bannerImage;
    protected Button enterButton;
    protected TextMeshProUGUI unlockText;
    protected GlobalController.Language currentLanguage;
    protected bool _locked = false;

    public string PreID => prequisiteLevelID;

    public bool IsLocked => !CheckUnLock();
    
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

    protected void InitAllElements()
    {
        bannerImage = transform.Find("Image").GetComponent<Image>();
        enterButton = GetComponentInChildren<Button>();
        unlockText = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    protected virtual void SetElementsToLocked()
    {
        
        bannerImage.color = Color.gray;
        enterButton.enabled = false;

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
    
    protected virtual bool CheckUnLock()
    {
        if (questSaveList == null)
        {
            questSaveList = GlobalController.Instance.GetQuestInfo();
        }
        
        
        if (questSaveList.Exists(x => x.quest_id == prequisiteLevelID))
        {
            _locked = false;
            return true;
        }
        else
        {
            _locked = true;
            print("锁住"+prequisiteLevelID);
            return false;
        }
    }

    private bool CheckLock()
    {
        if (questSaveList == null)
        {
            questSaveList = GlobalController.Instance.GetQuestInfo();
        }
        
        
        if (questSaveList.Exists(x => x.quest_id == prequisiteLevelID))
        {
            return false;
        }
        else
        {
            return true;
        }
    }



}
