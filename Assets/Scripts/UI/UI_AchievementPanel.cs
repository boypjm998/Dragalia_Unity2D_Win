using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_AchievementPanel : MonoBehaviour
{
    private Animator _animator;
    private Queue<Achievement> _achievementQueue = new Queue<Achievement>();
    private bool animationFinished = true;
    private AudioSource _audioSource;
    [SerializeField] private GameObject effect;
    
    [SerializeField] private TextMeshProUGUI achievementName;
    [SerializeField] private bool debug;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

    }

    private void Start()
    {
        _animator.enabled = false;
        effect.SetActive(false);
        if(AchievementManager.Instance != null)
            AchievementManager.Instance.OnAchievementFinished += AddAchievement;
    }

    private void OnDestroy()
    {
        if(AchievementManager.Instance != null)
            AchievementManager.Instance.OnAchievementFinished -= AddAchievement;
    }

    private void Update()
    {
        if(_achievementQueue.Count > 0 && animationFinished)
            DisplayAchievement();

        // if (debug)
        // {
        //     var achievement = new Achievement(999, 3, new string[] { "测试", "Test", "测试" },
        //         new string[] { "测试", "Test", "测试" }, AchievementSystem.ProgressType.OneOffBoolean, "", 0);
        //     debug = false;
        //     //AddAchievement(achievement);
        //     AchievementManager.Instance.OnAchievementFinished?.Invoke(achievement);
        // }

    }

    private void SetAnimationFinish()
    {
        animationFinished = true;
        effect.SetActive(false);
        print("SetEffectFalse");
    }

    private void DisplayAchievement()
    {
        var achievement = _achievementQueue.Dequeue();
        animationFinished = false;
        
        //TODO: display achievement(Set text, image, etc.)

        var name = achievement.name[(int)GlobalController.Instance.GameLanguage];
        var rarity = achievement.rarity;
        effect.SetActive(true);
        print("SetEffectTrue");

        achievementName.text =  $"<sprite={rarity-1}>{name}";

        _audioSource.Play();
        _animator.enabled = true;
        _animator.Play("display",0,0);
        Debug.Log("Start Achievement Animation");
    }
    
    public void AddAchievement(Achievement achievement)
    {
        _achievementQueue.Enqueue(achievement);
    }


}
