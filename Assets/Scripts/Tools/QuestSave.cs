
using System.Collections.Generic;

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
}

public class QuestDataList
{
    public List<QuestSave> quest_info;
    
    public QuestDataList()
    {
        quest_info = new List<QuestSave>();
    }
    public QuestDataList(List<QuestSave> list)
    {
        quest_info = list;
    }
}
