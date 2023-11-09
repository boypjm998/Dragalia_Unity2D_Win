using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


public class AchievementManager : MonoBehaviour
{
    [SerializeField] TextAsset achievementJson;
    public List<Achievement> Achievements;

    private void Awake()
    {
        Achievements = JsonConvert.DeserializeObject<List<Achievement>>(achievementJson.text);
    }
}
