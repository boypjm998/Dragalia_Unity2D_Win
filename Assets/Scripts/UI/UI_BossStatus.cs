using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using GameMechanics;

public class UI_BossStatus : MonoBehaviour
{
    private GameObject boss;
    // Start is called before the first frame update
    private UI_BossConditionBar _conditionBar;
    private UI_BossHPBar _HPbar;
    private GameObject _abilityIcons;
    [SerializeField] private GameObject bossAbilityPrefab;
    private TextMeshProUGUI _bossName;

    private JsonData bossAbilityDetailData;

    private GlobalController _globalController;
    private BattleStageManager _battleStageManager;
    
    
    
    IEnumerator Start()
    {
        _globalController = FindObjectOfType<GlobalController>();
        _battleStageManager = FindObjectOfType<BattleStageManager>();
        yield return new WaitUntil(() => boss != null);
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBoss(GameObject boss)
    {
        this.boss = boss;
    }

    void Init()
    {
        var bossStat = boss.GetComponentInChildren<StatusManager>();
        _HPbar = GetComponentInChildren<UI_BossHPBar>();
        _bossName = GetComponentInChildren<TextMeshProUGUI>();
        _conditionBar = GetComponentInChildren<UI_BossConditionBar>();
        _abilityIcons = transform.Find("BossAbilities").gameObject;

        
        bossAbilityDetailData = BasicCalculation.
            ReadJsonData("/LevelInformation/BossAbilityDetail_ZH.json");
        var levelDetailedInfo = _battleStageManager.GetLevelDetailedInfo();
        var bossAbilities = 
            levelDetailedInfo.boss_prefab[BattleStageManager.currentDisplayingBossInfo - 1].boss_abilities;

        foreach (var ability in bossAbilities)
        {
            AddBossAbility(ability);
        }
        
        // AddBossAbility("BOSS_ABILITY_20011");
        // AddBossAbility("BOSS_ABILITY_20021");
        
        
        //这里写死了 应该引入questInfo.json动态加载的！
        
        
        
        
        _bossName.text = bossStat.displayedName;
        _conditionBar.SetTargetStat(bossStat);
        bossStat.SetConditionBar(_conditionBar);
        _HPbar.SetTarget(bossStat);
        
    }

    /// <summary>
    /// get boss ability info
    /// </summary>
    /// <param name="bossAbilityIndex">eg:BOSS_ABILITY_0011</param>
    void AddBossAbility(string bossAbilityIndex)
    {
        //
        var bundle = _globalController.GetBundle("boss_ability_icon");
        var abilityData = bossAbilityDetailData[bossAbilityIndex];
        var imageSprite = bundle.LoadAsset<Sprite>(abilityData["ICON_PATH"].ToString());
        var newIcon = Instantiate(bossAbilityPrefab, _abilityIcons.transform);

        newIcon.GetComponent<Image>().sprite = imageSprite;
        newIcon.transform.Find("Info").Find("Content").Find("Text").GetComponent<TextMeshProUGUI>().text =
            abilityData["DESCRIPTION"].ToString();
        newIcon.transform.Find("Info").Find("Banner").Find("Text").GetComponent<TextMeshProUGUI>().text =
            abilityData["NAME"].ToString();


    }


}
