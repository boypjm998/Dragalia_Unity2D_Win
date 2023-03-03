using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISortingGroup : MonoBehaviour
{
    private GlobalController _globalController;
    private MenuUIManager _menuUIManager;
    public int LayerID = 0;
    [HideInInspector] public int sortingGroupID;
    public bool isActive = false;
    public Vector2 initialPosition;
    [HideInInspector] public Vector2 initialLocalPosition;
    public float hideTime = 0.3f;
    
    public enum HideDirection
    {
        Free,
        Vertical,
        Horizontal,
        Zoom,
        Alpha
    }

    

    public HideDirection _hideDirection = HideDirection.Free;
    public Vector2 _hideLocalPosition = Vector2.zero;
    private void Awake()
    {
        print(gameObject.name + transform.localPosition);
        initialPosition = transform.position;
        //initialLocalPosition = transform.localPosition;
        if (_hideDirection == HideDirection.Vertical)
            _hideLocalPosition.x = 0;
        if (_hideDirection == HideDirection.Horizontal)
            _hideLocalPosition.y = 0;
    }

    private void Start()
    {
        _menuUIManager = GameObject.Find("UI").GetComponent<MenuUIManager>();
        _globalController = FindObjectOfType<GlobalController>();
    }

    private void OnEnable()
    {
        if (_hideDirection == HideDirection.Zoom)
        {
            if (isActive)
            {
                transform.localPosition = initialPosition;
            }
            else
            {
                transform.localPosition= _hideLocalPosition;
            }
        }

    }

    private void OnDisable()
    {
        isActive = false;
        if(_menuUIManager==null)
            return;
        _menuUIManager.lastDisabledUIGroup = this.sortingGroupID;
    }



    public void clicked(string name)
    {
        if (_globalController.loadingEnd == false)
        {
            return;
        }

        //_menuUIManager.MenuButtonClickEvent(name);
    }
    
    public void clicked(string name,int id)
    {
        if (_globalController.loadingEnd == false)
        {
            return;
        }

        //_menuUIManager.MenuButtonClickEvent(name,id);
    }
    
    public void ToUIState(int id)
    {
        if (_globalController.loadingEnd == false)
        {
            return;
        }
        if(_menuUIManager.GUIAnimCount>0)
            return;

        _menuUIManager.ToNextUIState(id);
    }
    public void ReturnUIState()
    {
        if (_globalController.loadingEnd == false)
        {
            return;
        }
        if(_menuUIManager.GUIAnimCount>0)
            return;
        print("Clicked Return");
        _menuUIManager.ToPreviousUIState();
    }

    public void ToBattle(int questID)
    {
        if (_globalController.loadingEnd == false)
        {
            return;
        }
        if(_menuUIManager.GUIAnimCount>0)
            return;
        _menuUIManager.EnterLevel(questID);
    }




    public void HideWithoutAnimation()
    {
        GetComponent<CanvasGroup>().interactable = false;
        transform.localPosition = _hideLocalPosition;
    }
    public void AppearWithoutAnimation()
    {
        GetComponent<CanvasGroup>().interactable = true;
        transform.position = initialPosition;
    }


}
