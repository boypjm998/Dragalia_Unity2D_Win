using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrasticForce : MonoSingleton<DrasticForce>
{
    public float duration = 40f;
    
    public static int MaxCount = 10;
    
    public int StackCount => leftTimeList.Count;

    public float LeftTime
    {
        //找到最小的时间
        get
        {
            if (leftTimeList.Count == 0)
                return 0;
            return Mathf.Clamp(leftTimeList.Min(),0,duration);
        }
    }
    
    private List<float> leftTimeList = new List<float>();
    private void Update()
    {
        TickLeftTime();
    }

    public void AddDrasticForce()
    {
        leftTimeList.Add(duration);
        if (leftTimeList.Count > MaxCount)
        {
            leftTimeList.RemoveAt(0);
        }
    }

    public void RemoveAllDrasticForce()
    {
        leftTimeList.Clear();
    }
    private void TickLeftTime()
    {
        for (int i = 0; i < leftTimeList.Count; i++)
        {
            leftTimeList[i] -= Time.deltaTime;
            if (leftTimeList[i] <= 0)
            {
                leftTimeList.RemoveAt(i);
                i--;
            }
        }
    }
    
}
