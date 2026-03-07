
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestSave
{
    public string quest_id;
    public double best_clear_time;
    public int crown_1;
    public int crown_2;
    public int crown_3;

    public QuestSave()
    {
    }

    public QuestSave(string questID)
    {
        this.quest_id = questID;
        best_clear_time = -1;
        crown_1 = 0;
        crown_2 = 0;
        crown_3 = 0;
    }
    public QuestSave(string questID, double bestClearTime, int crown1, int crown2, int crown3)
    {
        this.quest_id = questID;
        best_clear_time = bestClearTime;
        crown_1 = crown1;
        crown_2 = crown2;
        crown_3 = crown3;
    }
    
    public bool IsFullCleared()
    {
        return crown_1 == 1 && crown_2 == 1 && crown_3 == 1;
    }
}

public class MapData
{
    public int mapID;
    public int isAvailable;
}

public class QuestDataList
{
    public List<QuestSave> quest_info;
    public List<MapData> map_info;

    public QuestDataList()
    {
        quest_info = new List<QuestSave>();
        map_info = new List<MapData>();
    }
    public QuestDataList(List<QuestSave> list)
    {
        quest_info = list;
        map_info = new List<MapData>();
    }
}

public static class QuestSeriesInfo
{
    public enum QuestSetBanner
    {
        Unknown = 0,
        Auspexes = 1,
        PrimalDragons = 2,
        Demons = 3,
        EventForsakenA = 4,

        Tutorial = 99,

        Sheila = 101,
        Zena = 102,
        Ilia = 103,
        Zethia = 104,
        Origa = 105,

        Midgard = 201,
        Jupiter = 204,
        Zodiark = 205,

        FallenGabriel = 301,
        FallenRaphael = 303,
        FallenUriel = 304,

        Jaldabaoth = 311,
        Asura = 313,
        Iblis = 314

    }

    public static string GetQuestSetName(QuestSetBanner set)
    {
        string name = "???";
        if (GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
        {
            switch (set)
            {
                case QuestSetBanner.Auspexes:
                    name = "巫女的试炼";
                    break;
                case QuestSetBanner.PrimalDragons:
                    name = "传奇龙之试炼";
                    break;
                case QuestSetBanner.Demons:
                    name = "天魔封灭战";
                    break;
                case QuestSetBanner.EventForsakenA:
                    name = "圣天之陨 前篇";
                    break;
                case QuestSetBanner.Tutorial:
                    name = "终焉与伊始";
                    break;


                case QuestSetBanner.Sheila:
                    name = "席拉的试炼";
                    break;
                case QuestSetBanner.Zena:
                    name = "泽娜的试炼";
                    break;
                case QuestSetBanner.Ilia:
                    name = "伊莉雅的试炼";
                    break;
                case QuestSetBanner.Zethia:
                    name = "塞西娅的试炼";
                    break;
                case QuestSetBanner.Origa:
                    name = "奥莉加的试炼";
                    break;

                case QuestSetBanner.Midgard:
                    name = "传奇耶梦加得的试炼";
                    break;
                case QuestSetBanner.Jupiter:
                    name = "传奇朱庇特的试炼";
                    break;
                case QuestSetBanner.Zodiark:
                    name = "传奇佐迪亚克的试炼";
                    break;

                case QuestSetBanner.FallenGabriel:
                    name = "堕天使加百利封灭战";
                    break;
                case QuestSetBanner.FallenRaphael:
                    name = "堕天使拉斐尔封灭战";
                    break;
                case QuestSetBanner.FallenUriel:
                    name = "堕天使乌列封灭战";
                    break;

            }
        }
        else
        {
            switch (set)
            {
                case QuestSetBanner.Auspexes:
                    name = "Awakened Auspex's Trials";
                    break;
                case QuestSetBanner.PrimalDragons:
                    name = "Primal Dragon Trials";
                    break;
                case QuestSetBanner.Demons:
                    name = "The Rise of the Sinister Dominion";
                    break;
                case QuestSetBanner.EventForsakenA:
                    name = "Faith Forsaken (Part One)";
                    break;
                case QuestSetBanner.Tutorial:
                    name = "The Final Battle";
                    break;


                case QuestSetBanner.Sheila:
                    name = "Sheila's Trial";
                    break;
                case QuestSetBanner.Zena:
                    name = "Zena's Trial";
                    break;
                case QuestSetBanner.Ilia:
                    name = "Ilia's Trial";
                    break;
                case QuestSetBanner.Zethia:
                    name = "Zethia's Trial";
                    break;
                case QuestSetBanner.Origa:
                    name = "Origa's Trial";
                    break;

                case QuestSetBanner.Midgard:
                    name = "Primal Midgardsormr's Trial";
                    break;
                case QuestSetBanner.Jupiter:
                    name = "Primal Jupiter's Trial";
                    break;
                case QuestSetBanner.Zodiark:
                    name = "Primal Zodiark's Trial";
                    break;

                case QuestSetBanner.FallenGabriel:
                    name = "The Demon's False Love";
                    break;
                case QuestSetBanner.FallenRaphael:
                    name = "Fallen Angle of Conflict";
                    break;
                case QuestSetBanner.FallenUriel:
                    name = "Fallen Angle of Solitude";
                    break;

            }
        }

        return name;
    }

    public static string GetQuestSetName(string questID)
    {
        var set = QuestSetBanner.Unknown;
        if (questID.Length == 3)
        {
            var prefix = questID[..3];
            switch (prefix)
            {
                case "010":
                    set = QuestSetBanner.Auspexes;
                    break;
                case "020":
                    set = QuestSetBanner.PrimalDragons;
                    break;
                case "021":
                    set = QuestSetBanner.Demons;
                    break;
            }
        }
        else if (questID.Length == 5)
        {
            var prefix = questID[..4];
            switch (prefix)
            {
                case "0101":
                    set = QuestSetBanner.Sheila;
                    break;
                case "0102":
                    set = QuestSetBanner.Zena;
                    break;
                case "0103":
                    set = QuestSetBanner.Ilia;
                    break;
                case "0104":
                    set = QuestSetBanner.Zethia;
                    break;
                case "0105":
                    set = QuestSetBanner.Origa;
                    break;


                case "0201":
                    set = QuestSetBanner.Midgard;
                    break;
                case "0204":
                    set = QuestSetBanner.Jupiter;
                    break;
                case "0205":
                    set = QuestSetBanner.Zodiark;
                    break;

                case "0211":
                    set = QuestSetBanner.FallenGabriel;
                    break;
                case "0213":
                    set = QuestSetBanner.FallenRaphael;
                    break;
                case "0214":
                    set = QuestSetBanner.FallenUriel;
                    break;


            }

        }
        else if (questID.Length == 6)
        {
            var qidInt = Convert.ToInt32(questID);
            if (qidInt == 100001)
            {
                set = QuestSetBanner.Tutorial;
            }
            else if (qidInt >= 100002 && qidInt <= 1000006)
            {
                set = QuestSetBanner.EventForsakenA;
            }
        }

        return GetQuestSetName(set);
    }


    public static List<string> GetFalldownQuests(string questID)
    {
        List<string> falldownQuestIdList = new();
        if (questID.Length >= 6)
        {
            return falldownQuestIdList;
        }
        else
        {
            string falldownQuestId;
            int difficultyChar = Convert.ToInt32(questID[^1].ToString());
            for (var i = difficultyChar - 1; i >= 1; i--)
            {
                falldownQuestId = questID[..4] + i.ToString();
                try
                {
                    if (GlobalController.Instance.QuestData[$"QUEST_{falldownQuestId}"] != null)
                    {
                        falldownQuestIdList.Add(falldownQuestId);
                    }
                }
                catch
                {
                    Debug.Log(falldownQuestId);
                }
                
            }
        }

        return falldownQuestIdList;
    }

    public static string GetDifficultyName(int difficulty)
    {
        if (GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
        {
            switch (difficulty)
            {
                case 1:
                    return " 中级"; 
                    break;
                case 2:
                    return " 高级"; 
                    break;
                case 3:
                    return " 超级"; 
                    break;
                case 4:
                    return " 绝级";
                    break;
                case 5:
                    return " 绝级+";
                    break;
                default: return "";
            }
        }
        else if (GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
        {
            switch (difficulty)
            {
                case 1:
                    return ": Standard";
                    break;
                case 2:
                    return ": Expert";
                    break;
                case 3:
                    return ": Master";
                    break;
                case 4:
                    return ": Legend";
                    break;
                case 5:
                    return ": Legend+";
                    break;
                default: return "";
            }
        }
        else return "";
        
        
    }
    
    
}
