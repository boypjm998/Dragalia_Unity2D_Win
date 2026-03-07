using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AdventurerSelectionMenu2 : MonoBehaviour
{
    [SerializeField] private UI_AdventurerSelectionMenu upperMenu;

    private Transform ContentTransform;
    private Transform _currentChild;
    private ScrollRect _scrollRect;

    private void Awake()
    {
        //upperMenu = FindObjectOfType<UI_AdventurerSelectionMenu>();
        if (upperMenu == null)
        {
            upperMenu = GameObject.Find("UI").transform.Find("CharacterInfoMenu").GetComponent<UI_AdventurerSelectionMenu>();
        }
        ContentTransform = transform.Find("Scroll View/Viewport/Content");
        _scrollRect = GetComponentInChildren<ScrollRect>();
    }

    private void Start()
    {
        //RedirectSelectionArrow(GlobalController.currentCharacterID);
    }

    private void OnEnable()
    {
        
        RedirectSelectionArrow(GlobalController.currentCharacterID);
        
        
        if (_currentChild != null)
        {
            var index = _currentChild.GetSiblingIndex();
            print(index);
            _scrollRect.horizontalNormalizedPosition = 
                Mathf.Clamp(((float)index / (ContentTransform.childCount - 1)), 0, 1);
        }
    }

    public void ChooseCharacter(int id)
    {
        //Find({id}).childCount == 1
        if (upperMenu.currentSelectedCharaID == id && ContentTransform.Find($"{id}").childCount != 2)
        {
            GlobalController.currentCharacterID = id;
            RedirectSelectionArrow(id);
            print(GlobalController.currentCharacterID);
        }
        upperMenu.ChangeCurrentSelectedCharacter(id);
    }
    
    private void RedirectSelectionArrow(int id)
    {
        //id = id - 1;
        print(id);
        for(int i = 0 ; i < ContentTransform.childCount; i++)
        {
            var child = ContentTransform.GetChild(i);
            if (child.name == id.ToString() && child.childCount != 2)
            {
                print(child.Find("Light").gameObject);
                child.Find("Light").gameObject.SetActive(true);
                _currentChild = child;
                try
                {
                    
                    UnityEngine.EventSystems.EventSystem.current?.
                            SetSelectedGameObject(_currentChild.gameObject);
                    
                }
                catch(Exception e)
                {
                    print(e);
                }
            }
            else
            {
                //print(ContentTransform.GetChild(i).Find("Light").gameObject);
                child.Find("Light")?.gameObject.SetActive(false);
            }
        }
    }
}
