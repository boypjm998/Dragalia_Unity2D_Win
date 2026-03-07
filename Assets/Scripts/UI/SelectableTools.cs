using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableTools : MonoBehaviour
{

    [SerializeField] private bool findFirstSelectableOnEnable = false;
    [SerializeField] private bool scrollViewPositionAutoAdjust = false;
    
    private ScrollRect _scrollRect;
    private Rect _rect;
    private RectTransform _rectTransform;

    public bool ScrollViewAdjust
    {
        get=>scrollViewPositionAutoAdjust;
        set
        {
            scrollViewPositionAutoAdjust = value;
            InitScrollRect();
        }
    }


    private void Start()
    {
        
        if (scrollViewPositionAutoAdjust && !_scrollRect)
        {
            InitScrollRect();
        }
        
    }
    
    

    private void OnEnable()
    {
        if (findFirstSelectableOnEnable)
        {
            FindFirstSelectable();
        }
    }

    private void OnDisable()
    {
        if (findFirstSelectableOnEnable)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void LateUpdate()
    {
        // if(scrollViewPositionAutoAdjust)
        //     AutoAdjustScrollViewPosition();
    }


    private void InitScrollRect()
    {
        if (_scrollRect == null)
        {
            _scrollRect = GetComponentInParent<ScrollRect>();
            //_rect = _scrollRect.viewport.rect;
            _rectTransform = GetComponent<RectTransform>();
            
            print(_scrollRect.viewport.rect.width + " " + _scrollRect.viewport.rect.height);
        }
    }
    private void FindFirstSelectable()
    {
        UnityEngine.EventSystems.EventSystem.current.
            SetSelectedGameObject(gameObject);
        //判断当前场景是否有选中的Selectable对象
        // if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null)
        // {
        //     //如果没有
        //     UnityEngine.EventSystems.EventSystem.current.
        //         SetSelectedGameObject(gameObject);
        // }else if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.
        //               GetComponent<Selectable>().interactable == false)
        // {
        //     //如果当前选中的对象不是Button
        //     UnityEngine.EventSystems.EventSystem.current.
        //         SetSelectedGameObject(gameObject);
        // }else{print(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);}
    }
    
    private void AutoAdjustScrollViewPosition()
    {
        
            //If current selected object are out of view, adjust the scroll view position
        GameObject currentSelectedObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            
        if(currentSelectedObject != gameObject)
            return;
        
        
        
        //如果当前选中的对象还在视图内，就不调整滚动视图的位置
        
        //var currentRect = currentSelectedObject.GetComponent<RectTransform>();
        
        print(_rectTransform.anchoredPosition.x +","+_rectTransform.anchoredPosition.y);

        if (_scrollRect.horizontal)
        {
            //Use Viewport & content & currentSelectedRectTransform's AnchoredPosition to calculate the relative position.

            var currentRect = _rectTransform;
            var currentRectPosition = currentRect.anchoredPosition;

            RectTransform contentRect = _scrollRect.content;
            RectTransform viewportRect = _scrollRect.viewport;
            
            var visibleViewportLeftBorder = viewportRect.anchoredPosition.x + contentRect.anchoredPosition.x;
            //var visibleViewportRightBorder = viewportRect.anchoredPosition.x + viewportRect.rect.width + contentRect.anchoredPosition.x;
            
            print(visibleViewportLeftBorder);

            if (_rectTransform.anchoredPosition.x + visibleViewportLeftBorder < 0 ||
                _rectTransform.anchoredPosition.x + visibleViewportLeftBorder > viewportRect.rect.width)
            {
                _scrollRect.horizontalNormalizedPosition =
                    Mathf.Clamp(
                        ((float)currentSelectedObject.transform.GetSiblingIndex() /
                         (_scrollRect.content.childCount - 1)), 0, 1);
            }
        }
        else
        {
            //按照和上面一样的方法，计算纵向的滚动视图的位置
            
            var currentRect = _rectTransform;
            var currentRectPosition = currentRect.anchoredPosition;
            
            RectTransform contentRect = _scrollRect.content;
            RectTransform viewportRect = _scrollRect.viewport;
            
            var visibleViewportTopBorder = viewportRect.anchoredPosition.y + contentRect.anchoredPosition.y;
            //var visibleViewportBottomBorder = viewportRect.anchoredPosition.y + viewportRect.rect.height + contentRect.anchoredPosition.y;
            
            print(visibleViewportTopBorder);
            
            if (_rectTransform.anchoredPosition.y + visibleViewportTopBorder > 0 ||
                _rectTransform.anchoredPosition.y + visibleViewportTopBorder < -viewportRect.rect.height)
            {
                print("Adjust ScrollView Position");
                _scrollRect.verticalNormalizedPosition =
                    1 - Mathf.Clamp(
                        ((float)currentSelectedObject.transform.GetSiblingIndex() /
                         (_scrollRect.content.childCount - 1)), 0, 1);
            }
            
        }
        
        
        
        // 判断_scrollRect横向的还是纵向的
        
        // if (_scrollRect.vertical)
        //     _scrollRect.verticalNormalizedPosition = 
        //         Mathf.Clamp(((float)currentSelectedObject.transform.GetSiblingIndex() / (_scrollRect.content.childCount - 1)), 0, 1);
        // else if (_scrollRect.horizontal)
        //     _scrollRect.horizontalNormalizedPosition = 
        //         Mathf.Clamp(((float)currentSelectedObject.transform.GetSiblingIndex() / (_scrollRect.content.childCount - 1)), 0, 1);
        // else
        //     _scrollRect.verticalNormalizedPosition = 
        //         Mathf.Clamp(((float)currentSelectedObject.transform.GetSiblingIndex() / (_scrollRect.content.childCount - 1)), 0, 1);



        
            
        // _scrollRect.horizontalNormalizedPosition = 
        //     Mathf.Clamp(((float)currentSelectedObject.transform.GetSiblingIndex() / (_scrollRect.content.childCount - 1)), 0, 1);

        
    }
    
    
    
    
}
