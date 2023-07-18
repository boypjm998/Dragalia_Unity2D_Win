using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestDetail : MonoBehaviour
{
    [SerializeField] private RectTransform _scrollRect;
    [SerializeField] private GameObject contentParent;
    
    public static UI_QuestDetail Instance { get; private set; }
    public int currentDisplayingPageID;

    private void Awake()
    {
        Instance = this;
    }
    

    private void OnDestroy()
    {
        CancelInvoke();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }


    private void OnEnable()
    {
        
        for (int i = 0; i < contentParent.transform.childCount; i++)
        {
            Destroy(contentParent.transform.GetChild(i).gameObject);
        }
        Invoke("DisplayPage",0.1f);
        _scrollRect.localPosition = new Vector3(_scrollRect.localPosition.x, 0,
            _scrollRect.localPosition.z);
    }

    private void DisplayPage()
    {
        
        var res = Resources.Load<GameObject>
            ($"UI/OutBattle/QuestDetailedTutorial/zh/{MenuUIManager.Instance.currentQuestInfoPageID}_DetailedTutPage");
        Instantiate(res, contentParent.transform);
    }

}
