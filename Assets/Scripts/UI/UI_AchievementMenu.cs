using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AchievementMenu : MonoBehaviour
{
    [SerializeField] private Transform achievementParent;
    /// <summary>
    /// 0: ZHCN, 1: ENUS, 2: JAJP
    /// </summary>
    [SerializeField] private GameObject[] singleAchievementPrefab;
    [SerializeField] private Sprite[] rarityImages;
    
    private List<AchievementInfo> achievementInfos = new List<AchievementInfo>();

    private void Start()
    {
        achievementInfos = GlobalController.Instance.gameOptions.achievementList;

        InstantiateAchievementListUI();


    }

    private void InstantiateAchievementListUI()
    {
        var achievementData = AchievementManager.Instance.Achievements;

        foreach (var data in achievementData)
        {
            var singleUI = Instantiate(singleAchievementPrefab
                [(int)GlobalController.Instance.GameLanguage], achievementParent);

            var icon = singleUI.transform.GetChild(1);
            var title = singleUI.transform.GetChild(2);
            var description = singleUI.transform.GetChild(3);
            var progress = singleUI.transform.GetChild(4);
            
            int rarity = (int)data.rarity;
            icon.GetComponent<Image>().sprite = rarityImages[rarity-1];
            
            title.GetComponent<TextMeshProUGUI>().text =
                data.name[(int)GlobalController.Instance.GameLanguage];
            
            
            
            var trueAchievement =
                achievementInfos.Find(x => x.id == data.id);
            
            var trueProgress =
                trueAchievement == null ? String.Empty : trueAchievement.progressStr;
            
            print(data.id);
            
            var tempAchievement = new Achievement(data.id,data.rarity,data.name,data.description,data.progressType,
                trueProgress,data.hideCondition,data.maxProgress);
            
            bool finished = false;
            
            progress.GetComponent<TextMeshProUGUI>().text =
                ParseAchievementProgress(tempAchievement,ref finished);
            
            description.GetComponent<TextMeshProUGUI>().text =
                !finished && data.hideCondition == 1 ? "???" :
                data.description[(int)GlobalController.Instance.GameLanguage];

            if (finished)
            {
                icon.GetComponent<Image>().color = Color.white;
            }
            



        }
        
        
        
        
    }
    
    /// <summary>
    /// ScrollView
    /// - ViewPort
    /// -- Content
    /// </summary>
    /// <param name="achievement"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>


    private string ParseAchievementProgress(Achievement achievement, ref bool finished)
    {
        var plainText = AchievementSystem.
            DecryptAchievementProcessString(achievement);
        
        //一次性布尔值标志：FINISHED=
        //重复整数标志：CURRENT=
        //不重复数组标志：LIST=/
        
        string displayText = "";
        
        if(achievement.maxProgress == 0)
            achievement.maxProgress = 1;
        
        switch (achievement.progressType)
        {
            case AchievementSystem.ProgressType.OneOffBoolean:
                //如果plainText不包含标志FINISHED，强行设置为FINISHED:FALSE
                if (!plainText.Contains("FINISHED=TRUE"))
                {
                    plainText = "FINISHED=FALSE";
                    displayText = "0/1";
                }
                else
                {
                    plainText = "FINISHED=TRUE";
                    displayText = "1/1";
                    finished = true;
                }
                //AchievementSystem.EncryptAchievementProcessString(achievement,plainText);
                break;
            
            case AchievementSystem.ProgressType.DistinctArray:
                if (!plainText.Contains("LIST=/"))
                {
                    plainText = "LIST=//";
                    displayText = $"0/{achievement.maxProgress}";
                }
                
                int count = plainText.Count(ch => ch == ',') + 1;
                if (plainText.Contains("//"))
                {
                    count = 0;
                }
                displayText = $"{count}/{achievement.maxProgress}";
                if (count >= achievement.maxProgress)
                {
                    finished = true;
                }
                break;
            
            case AchievementSystem.ProgressType.RepeatInteger:
                if (!plainText.Contains("CURRENT="))
                {
                    plainText = "CURRENT=0";
                    displayText = $"0/{achievement.maxProgress}";
                }
                else
                {
                    int current = int.Parse(plainText.Split('=')[1]);
                    displayText = $"{current}/{achievement.maxProgress}";
                    if (current >= achievement.maxProgress)
                    {
                        finished = true;
                    }
                }

                
                break;


            default:
                throw new ArgumentOutOfRangeException();
        }
        
        AchievementSystem.EncryptAchievementProcessString(achievement,plainText);
        WriteAchievementProcess(achievement);
        
        print(displayText);
        
        return displayText;
        
    }


    private void WriteAchievementProcess(Achievement achievement)
    {
        //在achievementInfos中找到对应的achievementInfo，然后修改其中的progressStr
        var achievementInfo = achievementInfos.Find(x => x.id == achievement.id);

        if (achievementInfo == null)
        {
            achievementInfos.Add(new AchievementInfo(achievement.id,achievement.GetProgress()));
        }
        else
        {
            achievementInfo.progressStr = achievement.GetProgress();
        }


        
    }



}
