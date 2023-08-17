using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BattleInfoCasterStory : UI_BattleInfoCaster
{
    private Color EnemyColor = Color.white;
    private Color PlayerColor = new Color(0.1f, 0.9f, 1f);
    public new static UI_BattleInfoCasterStory Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        BossSkillBanner = transform.GetChild(0).gameObject;
        DialogDisplayer = transform.Find("DialogDisplayer").gameObject;
        _text = BossSkillBanner.GetComponentInChildren<TextMeshProUGUI>();
        _banner = BossSkillBanner.GetComponentInChildren<Image>();
        //_banner.gameObject.SetActive(false);
        //_text.gameObject.SetActive(false);
        _tweenerImage = null;
        _tweenerText = null;
    }

    private void OnDestory()
    {
        Instance = null;
    }

    private void Start()
    {
        _globalController = GlobalController.Instance;
        
        BossSkillNameData = BasicCalculation.ReadJsonData("LevelInformation/QuestSkillInfoStory.json");
        


    }

    public void PrintSkillName(string actionName, bool isEnemy)
    {
        if (!isEnemy)
        {
            _banner.color = PlayerColor;
        }
        else
        {
            _banner.color = EnemyColor;
        }
        base.PrintSkillName(actionName);
    }

}
