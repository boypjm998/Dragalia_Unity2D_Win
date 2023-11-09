using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

[Serializable]
public class Achievement
{
    public int id;
    public int rarity;
    public string[] name = new string[3];
    public string[] description = new string[3];
    

}

public class AchievementSystem
{
    public int Id { get; set; }
    public bool isCompleted { get; set; }
    
    public Expression<Func<bool>> ConditionMethod { get; private set; }
    
    public void Init(int id, Expression<Func<bool>> conditionMethod)
    {
        Id = id;
        ConditionMethod = conditionMethod;
        ConditionMethod = () => TestConditional();
    }

    private bool TestConditional()
    {
        return false;
    }




}