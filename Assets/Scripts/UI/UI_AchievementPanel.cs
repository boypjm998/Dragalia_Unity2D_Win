using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_AchievementPanel : MonoBehaviour
{
    private Animator _animator;
    private Queue<Achievement> _achievementQueue = new Queue<Achievement>();
    private bool animationFinished = true;
    
    [SerializeField] private TextMeshProUGUI achievementName;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void DisplayAchievement()
    {
        var achievement = _achievementQueue.Dequeue();
        animationFinished = false;
        
        //TODO: display achievement(Set text, image, etc.)
        
        _animator.Play("display",0,0);
    }
    
    public void AddAchievement(Achievement achievement)
    {
        _achievementQueue.Enqueue(achievement);
    }


}
