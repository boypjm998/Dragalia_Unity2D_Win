using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResultPageSpecial : UI_ResultPage
{
    // Start is called before the first frame update
    private static string zhcn_levelname1 = "终焉与伊始";
    private static string zhcn_levelname2 = "起始之“人”";
    
    private static string en_levelname1 = "The Final Battle";
    private static string en_levelname2 = "A New Beginning";
    public float backspaceTotalTime = 1.5f;
    
    void Awake()
    {
        portraits = transform.Find("Portrait").gameObject;
        portraitImage = portraits.transform.Find("Portrait").GetComponent<Image>();
        shadowImage = portraits.transform.Find("Shadow").GetComponent<Image>();

        levelName = transform.Find("Upper").GetComponentInChildren<TextMeshProUGUI>();
        clearTime = transform.Find("ClearTime").Find("TimeText").GetComponent<TextMeshProUGUI>();
        
        missionList = transform.Find("MissionList").gameObject;
        mission1 = missionList.transform.GetChild(0).gameObject;
        mission2 = missionList.transform.GetChild(1).gameObject;
        mission3 = missionList.transform.GetChild(2).gameObject;

        returnButton = transform.Find("ReturnButton").gameObject;

        battleManager = FindObjectOfType<BattleStageManager>();
    }

    // Update is called once per frame
    private void OnEnable()
    {
        portraits.transform.localPosition = new Vector3(900, 0);
        portraits.transform.DOLocalMoveX(0, 0.5f);
        //加载相应角色的立绘
        
        //加载关卡名from其他脚本
        //levelName.text = battleManager.quest_name;
        
        //统计过关时间
        var min = (int)Mathf.Floor(battleManager.currentTime / 60);
        var sec = (int)battleManager.currentTime % 60;
        clearTime.text = $"{min:D2}:{sec:D2}";
        //clearTime.text = "00:00";
        //new 判定
        clearTime.transform.parent.localPosition =
            new Vector3(-1100, clearTime.transform.parent.localPosition.y, 0);
        clearTime.transform.parent.DOLocalMoveX(-720, 0.5f);
        

        // missionList.transform.localPosition =
        //     new Vector3(-1200, missionList.transform.localPosition.y, 0);
        // missionList.transform.DOLocalMoveX(-470, 0.5f).
        //     OnComplete(DoCrownAnimation);
        
        //获取关卡过关条件
        // condition1.text = $"使用{reviveLimit}次以下重生的情况下过关";
        // condition2.text = $"{getCrownLimitTime}秒内过关";
        // if (getCrownReviveTime <= 0)
        // {
        //     condition3.text = "不使用重生的情况下过关";
        // }
        // else
        // {
        //     condition3.text = $"使用{getCrownReviveTime}次以下重生的情况下过关";
        // }
        //位移
        StartCoroutine(LevelNameAnimation());
        
        missionList.transform.localPosition =
            new Vector3(-1200, missionList.transform.localPosition.y, 0);
        missionList.transform.DOLocalMoveX(-470, 0.5f).
            OnComplete(DoCrownAnimation);
        
        returnButton.transform.localPosition =
            new Vector3(-1000, returnButton.transform.localPosition.y);

        bool isNewRecord = false;
        
        
        // battleManager.UpdateQuestSaveData
        //     (new QuestSave(battleManager.quest_id,battleManager.currentTime,
        //         ConditionCheck(1)?1:0,
        //         ConditionCheck(2)?1:0,
        //         ConditionCheck(3)?1:0),
        //         ref isNewRecord);
        
    }
    
    private void OnDisable()
    {
        SetButtonDisabled();
    }

    private IEnumerator LevelNameAnimation()
    {
        string levelNameText1;
        string levelNameText2;
        if (GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
        {
            levelNameText1 = zhcn_levelname1;
            levelNameText2 = zhcn_levelname2;
        }
        else if(GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
        {
            levelNameText1 = en_levelname1;
            levelNameText2 = en_levelname2;
        }
        else yield break;
        
        //blink
        int blinkTime = 2;
        var blinkInterval = 0.25f;
        while (blinkTime > 0)
        {
            levelName.text = levelNameText1 + "|";
            yield return new WaitForSeconds(blinkInterval);
            levelName.text = levelNameText1;
            yield return new WaitForSeconds(blinkInterval);
            blinkTime--;
        }

        var backspaceInterval = 0.1f;
        levelName.text = levelNameText1 + "|";
        while (levelName.text.Length > 1)
        {
            levelName.text = levelName.text.Substring(0, levelName.text.Length - 2) + "|";
            yield return new WaitForSeconds(backspaceInterval);
        }

        blinkTime = 2;
        while (blinkTime > 0)
        {
            levelName.text = "|";
            yield return new WaitForSeconds(blinkInterval);
            levelName.text = "";
            yield return new WaitForSeconds(blinkInterval);
            blinkTime--;
        }
        yield return new WaitForSeconds(blinkInterval);
        levelName.text = "|";
        for (int i = 0; i < levelNameText2.Length; i++)
        {
            levelName.text = levelName.text.Substring(0, levelName.text.Length - 1) + levelNameText2[i] + "|";
            yield return new WaitForSeconds(backspaceInterval);
        }

        yield return new WaitForSeconds(backspaceInterval);
        levelName.text = levelNameText2;
        
        
        returnButton.transform.DOLocalMoveX(-600, 0.5f).
            OnComplete(SetButtonEnabled);

    }

    protected override IEnumerator CrownAnimationRoutine()
    {
        if (true)
        {
            crown1 = mission1.transform.Find("Crown").gameObject;
            crown1.SetActive(true);
            crown1.transform.localScale = new Vector3(2, 2, 2);
            crown1.transform.DOScale(Vector3.one, .5f);
            yield return new WaitForSeconds(.4f);
        }
        if (true)
        {
            crown2 = mission2.transform.Find("Crown").gameObject;
            crown2.SetActive(true);
            crown2.transform.localScale = new Vector3(2, 2, 2);
            crown2.transform.DOScale(Vector3.one, .5f);
            yield return new WaitForSeconds(.4f);
        }
        if (true)
        {
            crown3 = mission3.transform.Find("Crown").gameObject;
            crown3.SetActive(true);
            crown3.transform.localScale = new Vector3(2, 2, 2);
            crown3.transform.DOScale(Vector3.one, .5f);
            yield return new WaitForSeconds(.4f);
        }
    }
}
