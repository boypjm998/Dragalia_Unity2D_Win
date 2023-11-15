using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


public class AchievementManager : MonoBehaviour
{
    [SerializeField] TextAsset achievementJson;
    public List<Achievement> Achievements;
    public List<Achievement> UnfinishedAchievements;
    private static AchievementManager _instance;

    private Dictionary<int, object[]> _achievementProgressInfo = new Dictionary<int, object[]>();
    public static AchievementManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AchievementManager>();
                if (_instance == null)
                {
                    return null;
                }

            }

            return _instance;
        }
    }
    
    private float _timer = 0;
    private Action TickAction;

    // Constants
    private const float tickInterval = 0.1f;
    
    
    

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }


        Achievements = JsonConvert.DeserializeObject<List<Achievement>>(achievementJson.text);

        _timer = tickInterval;


    }

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            _timer = tickInterval;
            TickAction?.Invoke();
        }

    }


    private void FilterUnfinishedAchievements()
    {
        var achievementInfos = GlobalController.Instance.gameOptions.achievementList;
        var tempAchievements = new List<Achievement>();
        foreach (var achievement in Achievements)
        {
            if (achievement.IsFinished)
                continue;
            tempAchievements.Add(achievement);
            ParseAchievementEventCheckInfo(achievement);
        }

        UnfinishedAchievements = tempAchievements;
    }

    private void CompleteAchievement(int id, object[] messages)
    {
        var achievement = UnfinishedAchievements.Find(x => x.id == id);

        if (achievement.progressType == AchievementSystem.ProgressType.OneOffBoolean ||
            achievement.progressType == AchievementSystem.ProgressType.RepeatInteger)
        {
            achievement.UpdateProgress(1);
        }
        else if(achievement.progressType == AchievementSystem.ProgressType.DistinctArray)
        {
            achievement.UpdateProgress((int)messages[0]);
        }


    }

    private void ParseAchievementEventCheckInfo(Achievement achievement)
    {
        switch (achievement.id)
        {
            case 1:
            {
                RegisterOnQuestClearEvent(achievement.id,"01024");
                break;
            }
            case 2:
            {
                RegisterOnQuestClearEvent(achievement.id,"01014");
                break;
            }
            case 6:
            {
                RegisterOnQuestClearEventAny(achievement.id);
                break;
            }
            case 7:
            {
                RegisterOnQuestClearWithCharacterEvent(achievement.id);
                break;
            }

            case 101:
            {
                SpecialEvent_101();
                break;
            }
            case 102:
            {
                SpecialEvent_102();
                break;
            }






        }



    }





    private void RegisterOnQuestClearEvent(int id, string questIDCheck)
    {
        Action<string> handler = null;
        
        handler = (questID) =>
        {
            if (questIDCheck == questID)
            {
                CompleteAchievement(id, new object[] { questID });
                BattleStageManager.Instance.OnQuestCleared -= handler;
            }
        };
        
        BattleStageManager.Instance.OnQuestCleared += handler;
    }

    private void RegisterOnQuestClearEventAny(int id)
    {
        Action<string> handler = null;
        
        handler = (questID) =>
        {
            CompleteAchievement(id, new object[] { questID });
            BattleStageManager.Instance.OnQuestCleared -= handler;
        };
        
        BattleStageManager.Instance.OnQuestCleared += handler;
        
    }

    private void RegisterOnQuestClearWithCharacterEvent(int id)
    {
        var currentID = GlobalController.currentCharacterID;

        Action<string> handler = null;
        
        handler = (questID) =>
        {
            CompleteAchievement(id, new object[] { currentID });
            BattleStageManager.Instance.OnQuestCleared -= handler;
        };
        
        BattleStageManager.Instance.OnQuestCleared += handler;
    }


    private void SpecialEvent_101()
    {
        var playerStatusManager = BattleStageManager.Instance.GetPlayer().
            GetComponent<PlayerStatusManager>();
        
        Action<StatusManager,StatusManager,float> handler = null;

        handler = (self, target, damage) =>
        {
            if (self.currentHp >= self.maxHP && self.maxHP >= 3000 &&
                damage >= self.maxHP)
            {
                CompleteAchievement(101,null);
            }
            
            playerStatusManager.OnTakeDirectDamageFrom -= handler;

        };

        playerStatusManager.OnTakeDirectDamageFrom += handler;
    }


    private void SpecialEvent_102()
    {
        var manager = BattleStageManager.Instance;
        Action<string> handler = null;
        
        handler = (questID) =>
        {
            if(manager.currentTime <= 5)
                CompleteAchievement(102, null);
            BattleStageManager.Instance.OnQuestCleared -= handler;
        };
        
        BattleStageManager.Instance.OnQuestCleared += handler;
    }




    public class AchievementConditionMethods
    {




    }
}
