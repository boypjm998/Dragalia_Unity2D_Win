using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TutorialPauseMenuCommon : UI_Tutorial_PauseMenu
{
    private StoryBattleTimelineManager _levelManager;
    private void Start()
    {
        _battleStageManager = BattleStageManager.Instance;
        _levelManager = StoryBattleTimelineManager.Instance;
        
    }
    
    protected override void DisplayPage(int targetPageID)
    {
        if(currentPage!=0)
            pagesParent.GetChild(currentPage).gameObject.SetActive(false);
        pagesParent.GetChild(targetPageID).gameObject.SetActive(true);
        currentPageObject = pagesParent.GetChild(targetPageID).gameObject;
        currentPage = targetPageID;
        //FormatTutorialText(currentPage);
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
    
    
}
