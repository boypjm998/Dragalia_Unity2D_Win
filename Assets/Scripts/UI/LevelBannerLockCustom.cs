using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LevelBannerLockCustom : LevelBannerLock
{
    public Condition condition;

    [Serializable]
    public class Condition
    {
        public ConditionPresetType ConditionPresetType = ConditionPresetType.ClearSeriesDifficulty;
        public string[] customTextArray = new string[3];
        public List<string> messageList = new();
        public List<string> checkList = new();
        private string text = "";
        public bool isAny = false;

        public override string ToString()
        {
            if (text != "")
                return text;


            StringBuilder sb = new();

            if(ConditionPresetType == ConditionPresetType.ClearSeriesDifficulty)
            {
                
                var str = QuestSeriesInfo.GetQuestSetName(messageList[0]);
                if(messageList.Count > 1)
                {
                    if(messageList[1] != "")
                        str += QuestSeriesInfo.GetDifficultyName(int.Parse(messageList[1]));
                }

                if (GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
                {
                    if (messageList.Count == 2)
                        sb.Append($"通关所有{str}");
                    else
                    {
                        sb.Append($"通关{messageList[2]}个{str}");
                    }
                }
                else
                {
                    if (messageList.Count == 2)
                        sb.Append($"Clear all \"{str}\"");
                    else
                        sb.Append($"Clear {messageList[2]} \"{str}\"");
                }

            }
            else if(ConditionPresetType == ConditionPresetType.ClearQuestsMoreThan)
            {
                if (GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
                {
                    int num = int.Parse(messageList[1]);

                    
                    sb.Append($"通关{num}个任务");
                    
                }
                else
                {
                    int num = int.Parse(messageList[1]);


                    sb.Append($"Clear {num} quests");
                }
            }
            else if(ConditionPresetType == ConditionPresetType.HaveCrownMoreThan)
            {
                if (GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
                {
                    int num = int.Parse(messageList[1]);


                    sb.Append($"累计获得{num}个皇冠");

                }
                else
                {
                    int num = int.Parse(messageList[1]);


                    sb.Append($"Total crown more than {num}");
                }
            }
            else
            {
                sb.Append(customTextArray[GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN ? 0 : 1]);
            }

            if(GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
                sb.Append("后解锁");
            else
                sb.Append(" to unlock");

            text = sb.ToString();
            return text;
        }
        
    }


    [System.Serializable]
    public enum ConditionPresetType
    {
        ClearSeriesDifficulty = 1,
        ClearQuestsMoreThan = 2,
        HaveCrownMoreThan = 4,
        Custom = 8
    }
    
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

    protected override void SetElementsToLocked()
    {
        
        bannerImage.color = Color.gray;
        enterButton.enabled = false;

        // var questData = GlobalController.Instance.QuestData;
        // var needClearQuestName = questData[$"QUEST_{prequisiteLevelID}"]["name"].ToString();

        unlockText.text = condition.ToString();
        unlockText.fontSize = 18;
        unlockText.enableAutoSizing = true;
        unlockText.fontSizeMax = 18;
        unlockText.fontSizeMin = 15;
        
        unlockText.color = Color.white;
    }

    protected override bool CheckUnLock()
    {
        questSaveList = GlobalController.Instance.GetQuestInfo();
        
        if (condition.ConditionPresetType == ConditionPresetType.ClearQuestsMoreThan)
        {
            if (questSaveList.Count >= int.Parse(condition.messageList[1]))
            {
                return true;
            }
            else return false;
        }else if (condition.ConditionPresetType == ConditionPresetType.ClearSeriesDifficulty)
        {
            
            if (condition.messageList.Count >= 3)
            {
                // 任意X个
                int countRequired = int.Parse(condition.messageList[2]);
                int count = 0;
                foreach (var qid in condition.checkList)
                {
                    if (questSaveList.Exists(x => x.quest_id == qid))
                    {
                        count++;
                    }
                    if(count >= countRequired)
                    {
                        return true;
                    }
                }
                return false;
            }
            else// 默认为 AND
            {
                foreach (var qid in condition.checkList)
                {
                    if(questSaveList.Exists(x => x.quest_id == qid))
                    {
                        continue;
                    }else return false;
                }
                return true;
            }
            
            
            
        }
        else if(condition.ConditionPresetType == ConditionPresetType.HaveCrownMoreThan)
        {
            int crownCount = GlobalController.Instance.GetTotalCrownCount();
            if (crownCount >= int.Parse(condition.messageList[1]))
            {
                return true;
            }
            else return false;
        }
        else
        {
            return false;
        }
        
    }
}
