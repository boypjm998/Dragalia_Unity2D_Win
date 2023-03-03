using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_ResultPage : MonoBehaviour
{
    private GameObject portraits;
    private Image portraitImage;
    private Image shadowImage;
    
    
    private TextMeshProUGUI levelName;
    private TextMeshProUGUI clearTime;

    private GameObject missionList;
    private GameObject mission1;
    private GameObject mission2;
    private GameObject mission3;
    private TextMeshProUGUI condition1;
    private TextMeshProUGUI condition2;
    private TextMeshProUGUI condition3;
    private GameObject crown1;
    private GameObject crown2;
    private GameObject crown3;

    private GameObject returnButton;

    private int reviveLimit;
    private int getCrownReviveTime = 0;
    private int getCrownLimitTime = 300;

    private BattleStageManager battleManager;

    private void Awake()
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
        condition1 = mission1.GetComponentInChildren<TextMeshProUGUI>();
        condition2 = mission2.GetComponentInChildren<TextMeshProUGUI>();
        condition3 = mission3.GetComponentInChildren<TextMeshProUGUI>();

        returnButton = transform.Find("ReturnButton").gameObject;

        battleManager = FindObjectOfType<BattleStageManager>();
        getCrownLimitTime = battleManager.crownTimeLimit;
        getCrownReviveTime = battleManager.crownReviveTime;
        reviveLimit = battleManager.maxReviveTime;
    }

    void Start()
    {
        
    }

    private void OnEnable()
    {
        portraits.transform.localPosition = new Vector3(900, 0);
        portraits.transform.DOLocalMoveX(0, 0.5f);
        //加载相应角色的立绘
        
        //加载关卡名from其他脚本
        levelName.text = battleManager.quest_name;
        
        //统计过关时间
        var min = (int)Mathf.Floor(battleManager.currentTime / 60);
        var sec = (int)battleManager.currentTime % 60;
        clearTime.text = $"{min:D2}:{sec:D2}";
        //clearTime.text = "00:00";
        //new 判定
        clearTime.transform.parent.localPosition =
            new Vector3(-1100, clearTime.transform.parent.localPosition.y, 0);
        clearTime.transform.parent.DOLocalMoveX(-720, 0.5f);
        

        missionList.transform.localPosition =
            new Vector3(-1200, missionList.transform.localPosition.y, 0);
        missionList.transform.DOLocalMoveX(-470, 0.5f).
            OnComplete(DoCrownAnimation);
        
        //获取关卡过关条件
        condition1.text = $"使用{reviveLimit}次以下重生的情况下过关";
        condition2.text = $"{getCrownLimitTime}秒内过关";
        if (getCrownReviveTime <= 0)
        {
            condition3.text = "不使用重生的情况下过关";
        }
        else
        {
            condition3.text = $"使用{getCrownReviveTime}次以下重生的情况下过关";
        }
        //位移
        returnButton.transform.localPosition =
            new Vector3(-1000, returnButton.transform.localPosition.y);
        returnButton.transform.DOLocalMoveX(-600, 0.5f).
            OnComplete(SetButtonEnabled);

        bool isNewRecord = false;
        
        
        battleManager.UpdateQuestSaveData
            (new QuestSave(battleManager.quest_id,battleManager.currentTime,
                ConditionCheck(1)?1:0,
                ConditionCheck(2)?1:0,
                ConditionCheck(3)?1:0),
                ref isNewRecord);

        if (!isNewRecord)
        {
            clearTime.transform.parent.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            clearTime.transform.parent.GetChild(2).gameObject.SetActive(true);
        }
    }

    private void DoCrownAnimation()
    {
        StartCoroutine(CrownAnimationRoutine());
    }


    IEnumerator CrownAnimationRoutine()
    {
        if (ConditionCheck(1))
        {
            crown1 = mission1.transform.Find("Crown").gameObject;
            crown1.SetActive(true);
            crown1.transform.localScale = new Vector3(2, 2, 2);
            crown1.transform.DOScale(Vector3.one, .5f);
            yield return new WaitForSeconds(.4f);
        }
        if (ConditionCheck(2))
        {
            crown2 = mission2.transform.Find("Crown").gameObject;
            crown2.SetActive(true);
            crown2.transform.localScale = new Vector3(2, 2, 2);
            crown2.transform.DOScale(Vector3.one, .5f);
            yield return new WaitForSeconds(.4f);
        }
        if (ConditionCheck(3))
        {
            crown3 = mission3.transform.Find("Crown").gameObject;
            crown3.SetActive(true);
            crown3.transform.localScale = new Vector3(2, 2, 2);
            crown3.transform.DOScale(Vector3.one, .5f);
            yield return new WaitForSeconds(.4f);
        }
        yield break;
    }

    bool ConditionCheck(int crownID)
    {
        switch (crownID)
        {
            case 1:
                return true;
            case 3:
            {
                var plrstat = FindObjectOfType<PlayerStatusManager>();
                if (plrstat.remainReviveTimes >= reviveLimit - getCrownReviveTime)
                {
                    return true;
                }
                break;
            }
            case 2:
            {
                if (battleManager.currentTime <= getCrownLimitTime)
                    return true;
                break;
            }
                default: break;
        }
        //检查每个星能否得到、
        return false;
    }

    void SetButtonEnabled()
    {
        Button rtnbtn = returnButton.GetComponent<Button>();
        rtnbtn.interactable = true;
    }
    void SetButtonDisabled()
    {
        Button rtnbtn = returnButton.GetComponent<Button>();
        rtnbtn.interactable = false;
    }

    private void OnDisable()
    {
        SetButtonDisabled();
        crown1?.SetActive(false);
        crown2?.SetActive(false);
        crown3?.SetActive(false);
        
    }

    public void ReturnToMainMenu()
    {
        var buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
        FindObjectOfType<GlobalController>().TestReturnMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
