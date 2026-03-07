using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TutorialMenuAfterPrologue : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject pagesParent;
    [SerializeField] private Button prevBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button returnBtn;
    [SerializeField] private bool hideReturnBtnBeforeToFinalPage = true;
    private int _pageCount;
    private int _currentPage = 1;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _pageCount = pagesParent.transform.childCount - 1;
        SetPanelInactive();
        HideAllPagesInactive();
        DisplayPage(1);
        if (hideReturnBtnBeforeToFinalPage)
        {
            returnBtn.gameObject.SetActive(false);
        }
    }

    public void SetPanelActive()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        gameObject.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(returnBtn.gameObject);
    }

    public void SetPanelInactive()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        gameObject.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void NextPage()
    {
        DisplayPage(_currentPage + 1);
    }

    public void PrevPage()
    {
        DisplayPage(_currentPage - 1);
    }

    private void HideAllPagesInactive()
    {
        for (int i = 1; i < _pageCount + 1; i++)
        {
            pagesParent.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void DisplayPage(int pageNum)
    {
        pageNum = Mathf.Clamp(pageNum, 1, _pageCount);
        
        
        pagesParent.transform.GetChild(_currentPage).gameObject.SetActive(false);
        pagesParent.transform.GetChild(pageNum).gameObject.SetActive(true);
        _currentPage = pageNum;

        if (_currentPage <= 1)
        {
            prevBtn.interactable = false;
            prevBtn.image.color = Color.gray;
        }
        else
        {
            prevBtn.interactable = true;
            prevBtn.image.color = Color.white;
        }
        
        if (_currentPage >= _pageCount)
        {
            nextBtn.interactable = false;
            nextBtn.image.color = Color.gray;
            returnBtn.gameObject.SetActive(true);
        }
        else
        {
            nextBtn.interactable = true;
            nextBtn.image.color = Color.white;
            
        }
        
    }
    
    
}
