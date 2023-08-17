using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;
using UnityEngine.UI;

public class UI_DialogDisplayerStory : UI_DialogDisplayer
{
    

    void Start()
    {
        dialogQueue = new Queue<DialogFormat>();
        portraitIcon = transform.Find("Icon").GetComponent<Image>();
        _swapper = transform.Find("Icon").GetComponent<UI_DialogImageSwapper>();
        balloons = transform.Find("Balloons").gameObject;
        if (GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
        {
            questDialogInfoData = 
                BasicCalculation.ReadJsonData("LevelInformation/QuestDialogInfoStory.json");
        }
        else if(GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
        {
            questDialogInfoData = 
                BasicCalculation.ReadJsonData("LevelInformation/QuestDialogInfoStory_EN.json");
        }


    }
    
    

}
