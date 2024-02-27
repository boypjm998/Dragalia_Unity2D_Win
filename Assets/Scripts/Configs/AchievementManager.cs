using System;
using System.Collections.Generic;
using System.Linq;
using GameMechanics;
using Newtonsoft.Json;
using UnityEngine;


public class AchievementManager : MonoBehaviour
{
    [SerializeField] TextAsset achievementJson;
    public List<Achievement> Achievements;
    public List<Achievement> UnfinishedAchievements;
    private static AchievementManager _instance;
    public List<AchievementInfo> AchievementInfos;

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
    private Action<float> TickAction;
    public Action<Achievement> OnAchievementFinished;

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

    private void Start()
    {
        LoadAchievementInfos();
        AddAchievementEvents();
    }

    private void Update()
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        
        
        
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            TickAction?.Invoke(tickInterval + Mathf.Abs(_timer));
            _timer = tickInterval;
            
        }

    }

    private void AddAchievementEvents()
    {
        FilterUnfinishedAchievements();
    }

    private void LoadAchievementInfos()
    {
        AchievementInfos = GlobalController.Instance.gameOptions.achievementList;

        if (AchievementInfos == null || AchievementInfos.Count == 0)
        {
            AchievementInfos = new();
            foreach (var achievement in Achievements)
            {
                AchievementInfos.Add(new AchievementInfo(achievement.id,
                    "QUJDREVGRw=="));
            }
            
        }

        print(Achievements.Count);
        print(AchievementInfos.Count);
        foreach (var achievement in Achievements)
        {
            var achievementInfo = AchievementInfos.Find(x => x.id == achievement.id);
            if (achievementInfo == null)
            {
                achievementInfo = new AchievementInfo(achievement.id, "QUJDREVGRw==");
                AchievementInfos.Add(achievementInfo);
            }
            achievement.SetProgress(achievementInfo.progressStr);
        }
        GlobalController.Instance.WriteGameOptionToFile();
        
        
    }


    private void FilterUnfinishedAchievements()
    {
        var achievementInfos = AchievementInfos;
        var tempAchievements = new List<Achievement>();
        foreach (var achievement in Achievements)
        {
            if (achievement.IsFinished)
            {
                print($"Achievement {achievement.name[0]} is Completed.");
                continue;
            }

            
            tempAchievements.Add(achievement);
        }

        UnfinishedAchievements = tempAchievements;
    }

    public void InitializeAchievements()
    {
        print("InitAchievements");
        foreach (var achievement in UnfinishedAchievements)
        {
            ParseAchievementEventCheckInfo(achievement);
        }
    }

    private void CompleteAchievement(int id, object[] messages)
    {
        var achievement = UnfinishedAchievements.Find(x => x.id == id);

        bool finished = false;

        if (achievement.progressType == AchievementSystem.ProgressType.OneOffBoolean ||
            achievement.progressType == AchievementSystem.ProgressType.RepeatInteger)
        {
            finished = achievement.UpdateProgress(1);
            foreach (var info in AchievementInfos)
            {
                if (info.id == achievement.id)
                {
                    info.progressStr = achievement.GetProgress();
                }
            }
        }
        else if(achievement.progressType == AchievementSystem.ProgressType.DistinctArray)
        {
            finished = achievement.UpdateProgress((int)messages[0]);
            foreach (var info in AchievementInfos)
            {
                if (info.id == achievement.id)
                {
                    info.progressStr = achievement.GetProgress();
                }
            }
        }

        if (finished)
        {
            UnfinishedAchievements.Remove(achievement);
            OnAchievementFinished?.Invoke(achievement);
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
            case 4:
            {
                RegisterOnQuestClearEvent(achievement.id,"01044");
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

            case 201:
            {
                SpecialEvent_201();
                break;
            }
            case 202:
            {
                SpecialEvent_202();
                break;
            }
            case 206:
            {
                SpecialEvent_206();
                break;
            }
            case 207:
            {
                SpecialEvent_207();
                break;
            }
            case 208:
            {
                SpecialEvent_208();
                break;
            }

            
            

        }
        
        print("Registered"+achievement.id);


    }





    private void RegisterOnQuestClearEvent(int id, string questIDCheck)
    {
        Action<string> handler = null;
        
        handler = (questID) =>
        {
            if (questIDCheck == questID)
            {
                CompleteAchievement(id, new object[] { int.Parse(questID) });
                BattleStageManager.Instance.OnQuestCleared -= handler;
            }
        };
        
        BattleStageManager.Instance.OnQuestCleared += handler;
    }
    
    private void RegisterOnQuestClearEvent(int id, params string[] questIDCheck)
    {
        Action<string> handler = null;
        
        handler = (questID) =>
        {
            if (questIDCheck.Contains(questID))
            {
                CompleteAchievement(id, new object[] { int.Parse(questID) });
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
            CompleteAchievement(id, new object[] { int.Parse(questID) });
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


    /// <summary>
    /// 满血状态被一击必杀
    /// </summary>
    private void SpecialEvent_101()
    {
        var playerStatusManager = BattleStageManager.Instance.GetPlayer().
            GetComponent<PlayerStatusManager>();
        
        Action<StatusManager,StatusManager,AttackBase,float> handler = null;

        handler = (self, target,atk, damage) =>
        {
            if (self.currentHp >= self.maxHP && self.maxHP >= 3000 &&
                damage >= self.maxHP)
            {
                CompleteAchievement(101,new object[] {101});
                playerStatusManager.OnTakeDirectDamageFrom -= handler;
            }

        };

        playerStatusManager.OnTakeDirectDamageFrom += handler;
    }


    /// <summary>
    /// 进本五秒撤退
    /// </summary>
    private void SpecialEvent_102()
    {
        var manager = BattleStageManager.Instance;
        Action<string> handler = null;
        
        handler = (questID) =>
        {
            if(manager.currentTime <= 5)
                CompleteAchievement(102, new object[] {102});
            BattleStageManager.Instance.OnQuestQuit -= handler;
        };
        
        BattleStageManager.Instance.OnQuestQuit += handler;
    }

    /// <summary>
    /// 席菈试炼绝级：3分钟dot
    /// </summary>
    private void SpecialEvent_201()
    {
        if (GlobalController.questID != "01014" && GlobalController.questID != "01015")
        {
            return;
        }

        StatusManager.TestDelegate handlerBuffEvent = null;

        var playerStatusManager = BattleStageManager.Instance.GetPlayer().
            GetComponent<PlayerStatusManager>();

        int burnID = (int)BasicCalculation.BattleCondition.Burn;
        int scorchID = (int)BasicCalculation.BattleCondition.Scorchrend;

        bool isBurning = false;
        float totalTime = 0;

        handlerBuffEvent = (condition) =>
        {
            if (playerStatusManager.GetConditionStackNumber(burnID) > 0 ||
                playerStatusManager.GetConditionStackNumber(scorchID) > 0)
                isBurning = true;
            else isBurning = false;
        };

        Action<float> timerHandler = (interval) =>
        {
            if (isBurning)
                totalTime += interval;
        };

        playerStatusManager.OnBuffEventDelegate += handlerBuffEvent;
        playerStatusManager.OnBuffExpiredEventDelegate += handlerBuffEvent;
        playerStatusManager.OnBuffDispelledEventDelegate += handlerBuffEvent;
        TickAction += timerHandler;



        StatusManager.StatusManagerVoidDelegate cancleAllEventHandler = null;
        Action<string> questClearEventHandler = null;
        
        questClearEventHandler = (id) =>
        {
            if (totalTime >= 180)
            {
                CompleteAchievement(201, null);
            }
            
            playerStatusManager.OnBuffEventDelegate -= handlerBuffEvent;
            playerStatusManager.OnBuffExpiredEventDelegate -= handlerBuffEvent;
            playerStatusManager.OnBuffDispelledEventDelegate -= handlerBuffEvent;
            playerStatusManager.OnReviveOrDeath -= cancleAllEventHandler;
            TickAction -= timerHandler;

        };
        
        //清除所有事件
        BattleStageManager.Instance.OnQuestCleared += questClearEventHandler;
        
        //条件不满足时：
        cancleAllEventHandler = () =>
        {
            playerStatusManager.OnBuffEventDelegate -= handlerBuffEvent;
            playerStatusManager.OnBuffExpiredEventDelegate -= handlerBuffEvent;
            playerStatusManager.OnBuffDispelledEventDelegate -= handlerBuffEvent;
            TickAction -= timerHandler;
            BattleStageManager.Instance.OnQuestCleared -= questClearEventHandler;
        };

        playerStatusManager.OnReviveOrDeath += cancleAllEventHandler;

    }

    /// <summary>
    /// 硬抗泽娜绝级5次伤害
    /// </summary>
    private void SpecialEvent_202()
    {
        if (GlobalController.questID != "01024" && GlobalController.questID != "01025")
        {
            return;
        }
        
        print("Registered 202");

        PlayerStatusManager playerStatusManager =
            BattleStageManager.Instance.GetPlayer().GetComponent<PlayerStatusManager>();
        
        Action<StatusManager,StatusManager,AttackBase,float> receiveDamageHandler = null;
        //StatusManager.StatusManagerVoidDelegate cancleAllEventHandler = null;
        Action<string> questClearEventHandler = null;

        int count = 0;
        
        receiveDamageHandler = (self, target,atk, damage) =>
        {
            
            if(count>=5)
                return;
            
            //print(atk.name);
            
            if (atk.name.Contains("fx_hb002_action29"))
            {
                //print("count: " + count);
                if (self.GetConditionStackNumber((int)BasicCalculation.BattleCondition.Invincible) == 0)
                {
                    count++;
                    print("count: " + count);
                }
            }

        };

        questClearEventHandler = (qid) =>
        {
            playerStatusManager.OnTakeDirectDamageFrom -= receiveDamageHandler;
            //playerStatusManager.OnReviveOrDeath -= cancleAllEventHandler;
            BattleStageManager.Instance.OnQuestCleared -= questClearEventHandler;

            if (count >= 5)
            {
                CompleteAchievement(202, null);
            }

        };

        // cancleAllEventHandler = () =>
        // {
        //     playerStatusManager.OnTakeDirectDamageFrom -= receiveDamageHandler;
        //     playerStatusManager.OnReviveOrDeath -= cancleAllEventHandler;
        //     BattleStageManager.Instance.OnQuestCleared -= questClearEventHandler;
        // };
        
        playerStatusManager.OnTakeDirectDamageFrom += receiveDamageHandler;
        //playerStatusManager.OnReviveOrDeath += cancleAllEventHandler;
        BattleStageManager.Instance.OnQuestCleared += questClearEventHandler;

    }

    /// <summary>
    /// 不击败塞西娅的情况下，击败伊莉雅。
    /// </summary>
    private void SpecialEvent_206()
    {
        if (GlobalController.questID != "01033")
        {
            return;
        }

        Action<string> eventHandler = null;

        eventHandler = (qid) =>
        {
            var ZethiaController =
                BattleStageManager.Instance.EnemyLayer.
                    GetComponentInChildren<HB03_M1_BehaviorTree>();
            
            if (ZethiaController == null)
                return;

            if(ZethiaController.GetComponent<StatusManager>().currentHp > 0)
                CompleteAchievement(206, null);
            
            BattleStageManager.Instance.OnQuestCleared -= eventHandler;
        };
        
        BattleStageManager.Instance.OnQuestCleared += eventHandler;
        
        
    }
    
    /// <summary>
    /// 在塞西娅使用慈爱之环之前，过关。
    /// </summary>
    private void SpecialEvent_207()
    {
        if (GlobalController.questID != "01043")
        {
            return;
        }

        var enemyMoveManager =
            BattleStageManager.Instance.EnemyLayer.GetComponentInChildren<EnemyMoveController_HB04>();
        
        Action<string> eventHandler = null;
        Action<int> cancelEventHandler = null;
        
        eventHandler = (qid) =>
        {
            CompleteAchievement(207,null);
            BattleStageManager.Instance.OnQuestCleared -= eventHandler;
            enemyMoveManager.OnUseSkill -= cancelEventHandler;
        };

        cancelEventHandler = (sid) =>
        {
            if (sid == 4)
            {
                cancelEventHandler -= cancelEventHandler;
                BattleStageManager.Instance.OnQuestCleared -= eventHandler;
            }
        };
        
        BattleStageManager.Instance.OnQuestCleared += eventHandler;
        enemyMoveManager.OnUseSkill += cancelEventHandler;

    }


    /// <summary>
    /// 木头人
    /// </summary>
    private void SpecialEvent_208()
    {
        if (GlobalController.questID != "01024" && GlobalController.questID != "01025")
        {
            return;
        }
        
        //print("Registered 208");

        Action<int> handler = null;

        handler = (id) =>
        {
            if(!StatusManager.IsControlAffliction(id))
                return;
            BattleStageManager.Instance.specialEventTriggered -= handler;
            CompleteAchievement(208, new object[] { 208 });
        };
        
        Action<string> cancleHandler = null;
        
        cancleHandler = (id) =>
        {
            BattleStageManager.Instance.specialEventTriggered -= handler;
            BattleStageManager.Instance.OnQuestCleared -= cancleHandler;
            BattleStageManager.Instance.OnQuestQuit -= cancleHandler;
        };
        
        BattleStageManager.Instance.specialEventTriggered += handler;
        BattleStageManager.Instance.OnQuestCleared += cancleHandler;
        BattleStageManager.Instance.OnQuestQuit += cancleHandler;


    }







}
