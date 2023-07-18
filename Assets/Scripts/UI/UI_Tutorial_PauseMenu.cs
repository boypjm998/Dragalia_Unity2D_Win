using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Tutorial_PauseMenu : UI_PauseMenu
{
    public static UI_Tutorial_PauseMenu Instance { get; private set; }
    
    // Start is called before the first frame update
    private TutorialLevelManager _tutorialLevelManager;
    [SerializeField] private TextAsset tutorialTextAsset;
    private int currentMaxPages = 1;
    private int currentPage = 1;
    private Transform pagesParent;
    private GameObject leftPageButton;
    private GameObject rightPageButton;
    private GameObject currentPageObject;
    
    private void Awake()
    {
        _battleSceneUIManager = transform.parent.GetComponent<BattleSceneUIManager>();
        pagesParent = transform.Find("TutorialPages");
        currentPageObject = pagesParent.GetChild(1).gameObject;
        leftPageButton = transform.Find("Panel/PrevPage").gameObject;
        rightPageButton = transform.Find("Panel/NextPage").gameObject;
        Instance = this;
    }

    private void Start()
    {
        _battleStageManager = BattleStageManager.Instance;
        _tutorialLevelManager = TutorialLevelManager.Instance;
        //gameObject.SetActive(false);
    }

    private void OnEnable()
    {
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void OpenConfigureMenu()
    {
        transform.Find("Panel").GetComponent<CanvasGroup>().interactable = false;
        transform.Find("ConfMenu").gameObject.SetActive(true);
    }
    
    public void CloseConfigureMenu()
    {
        transform.Find("Panel").GetComponent<CanvasGroup>().interactable = true;
        transform.Find("ConfMenu").gameObject.SetActive(false);
    }

    public void NextPage()
    {
        if(currentPage < currentMaxPages)
            DisplayPage(currentPage+1);
    }

    public void PrevPage()
    {
        if(currentPage > 1)
            DisplayPage(currentPage-1);
    }

    protected void DisplayPage(int targetPageID)
    {
        if(currentPage!=0)
            pagesParent.GetChild(currentPage).gameObject.SetActive(false);
        pagesParent.GetChild(targetPageID).gameObject.SetActive(true);
        currentPageObject = pagesParent.GetChild(targetPageID).gameObject;
        currentPage = targetPageID;
        FormatTutorialText(currentPage);
        if (currentPage == 1)
        {
            leftPageButton.SetActive(false);
            if (currentMaxPages > 1)
            {
                rightPageButton.SetActive(true);
            }
            else
            {
                rightPageButton.SetActive(false);
            }
        }
        else if (currentPage == currentMaxPages)
        {
            rightPageButton.SetActive(false);
            if (currentMaxPages > 1)
            {
                leftPageButton.SetActive(true);
            }
            else
            {
                leftPageButton.SetActive(false);
            }
        }
        else
        {
            leftPageButton.SetActive(true);
            rightPageButton.SetActive(true);
        }

        
    }

    private void FormatTutorialText(int pageID)
    {
        print(pageID);
        var controller = TutorialLevelManager.Instance;
        switch (pageID)
        { 
            case 1:
            {
                var tmp = currentPageObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                var txt = tmp.text;
                tmp.text = String.Format(txt,GlobalController.keyLeft.ToString(),GlobalController.keyRight.ToString(),
                    GlobalController.keyDown.ToString());
                break;
            }
            case 2:
            {
                var tmp = currentPageObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                tmp.text = String.Format(tmp.text,GlobalController.keyAttack.ToString());
                break;
            }
            case 3:
            {
                var tmp = currentPageObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                tmp.text = String.Format(tmp.text,GlobalController.keyRoll.ToString());
                break;
            }
            case 5:
            {
                var tmp = currentPageObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                tmp.text = String.Format(tmp.text,GlobalController.keyJump.ToString());
                break;
            }
            case 9:
            {
                var tmp = currentPageObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                print(controller.keySpecial);
                tmp.text = String.Format(tmp.text,GlobalController.keySpecial.ToString());
                break;
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="args">PagesIncrement:add currentMaxPages by 1</param>
    public override void UpdatePanel(string[] args)
    {
        if (args.Length == 0)
        {
            Debug.LogWarning("No arguments passed to UpdatePanel.");
            return;
        }

        switch (args[0])
        {
            case "PagesIncrement":
                currentMaxPages++;
                DisplayPage(currentMaxPages);
                break;
            case "ShowFirstPage":
                currentMaxPages = 1;
                DisplayPage(1);
                break;
            default:
                print("No such argument");
                break;
            
        }


    }
}
