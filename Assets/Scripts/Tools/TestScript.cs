using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoSingleton<GameManager>
{
    public Action<EnemyController> OnEnemyDeath;
}

/// <summary>
/// 比如这是你的敌人脚本
/// </summary>
public class TestScript
{
    private void TestLambdaMethod()
    {
        
        
        string someThingYouWantToPrint = "Hello World";
        
        GameManager.Instance.OnEnemyDeath += (enemy) =>
        {
            Debug.Log("Enemy Kill" + someThingYouWantToPrint);
            someThingYouWantToPrint += "!";
        };

    }

    private void Test()
    {
        Debug.Log("不带参数的函数");
    }
    
    private void Test2(string myVariable)
    {
        Debug.Log("带参数的函数" + myVariable);
    }
    
    
    
    
}

