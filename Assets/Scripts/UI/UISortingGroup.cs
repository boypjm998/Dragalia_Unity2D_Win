using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISortingGroup : MonoBehaviour
{
    private MenuUIManager _menuUIManager;
    public int LayerID = 0;
    public int sortingGroupID;
    public bool isActive = false;
    public Vector2 initialPosition;
    public enum HideDirection
    {
        Free,
        Vertical,
        Horizontal,
        Zoom
    }

    

    public HideDirection _hideDirection = HideDirection.Free;
    public Vector2 _hideLocalPosition = Vector2.zero;
    private void Awake()
    {
        initialPosition = transform.position;
        if (_hideDirection == HideDirection.Vertical)
            _hideLocalPosition.x = 0;
        if (_hideDirection == HideDirection.Horizontal)
            _hideLocalPosition.y = 0;
    }

    private void Start()
    {
        _menuUIManager = GameObject.Find("UI").GetComponent<MenuUIManager>();
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
        _menuUIManager.lastDisabledUIGroup = this.sortingGroupID;
    }



    public void clicked(string name)
    {
        _menuUIManager.MenuButtonClickEvent(name);
    }

    

}
