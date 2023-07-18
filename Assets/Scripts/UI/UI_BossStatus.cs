using System;
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
    protected GameObject boss;
    public StatusManager bossStat;
    // Start is called before the first frame update
    protected UI_BossConditionBar _conditionBar;
    protected UI_BossHPBar _HPbar;
    protected GameObject _abilityIcons;
    [SerializeField] private GameObject bossAbilityPrefab;
    private TextMeshProUGUI _bossName;

    protected CanvasGroup _canvasGroup;
    private JsonData bossAbilityDetailData;

    private GlobalController _globalController;
    protected BattleStageManager _battleStageManager;

    public int bossIndex;

    public bool visible
    {
        get => _canvasGroup.alpha > 0;
        set
        {
            _canvasGroup.alpha = value ? 1 : 0;
            _canvasGroup.blocksRaycasts = value;
        }
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }

    IEnumerator Start()
    {
        _globalController = FindObjectOfType<GlobalController>();
        _battleStageManager = FindObjectOfType<BattleStageManager>();
        
        
        yield return new WaitUntil(() => boss != null);
        _canvasGroup.alpha = 1;
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBoss(GameObject boss,int bossIndex = 0)
    {
        this.bossIndex = bossIndex;
        this.boss = boss;
    }
    
    

    protected virtual void Init()
    {
        var bossStat = boss.GetComponentInChildren<StatusManager>();

        if (bossStat is SpecialStatusManager)
        {
            var odBar = transform.Find("ODBar").gameObject;
            odBar.SetActive(true);
            odBar.GetComponent<UI_BossODBar>().bossStat = bossStat as SpecialStatusManager;
        }


        _HPbar = GetComponentInChildren<UI_BossHPBar>();
        _bossName = GetComponentInChildren<TextMeshProUGUI>();
        _conditionBar = GetComponentInChildren<UI_BossConditionBar>();
        _abilityIcons = transform.Find("BossAbilities").gameObject;

        
        bossAbilityDetailData = BasicCalculation.
            ReadJsonData("/LevelInformation/BossAbilityDetail_ZH.json");
        
        
        _bossName.text = bossStat.displayedName;
        _conditionBar.SetTargetStat(bossStat);
        bossStat.SetConditionBar(_conditionBar);
        _HPbar.SetTarget(bossStat);
        this.bossStat = bossStat;
        _HPbar.OnHPChange();
        
        
        
        var levelDetailedInfo = _battleStageManager.GetLevelDetailedInfo();
        var bossAbilities = 
            levelDetailedInfo.boss_prefab[bossIndex].boss_abilities;

        ClearBossAbility();
        
        foreach (var ability in bossAbilities)
        {
            AddBossAbility(ability);
        }
        
        // AddBossAbility("BOSS_ABILITY_20011");
        // AddBossAbility("BOSS_ABILITY_20021");
        
        
        //这里写死了 应该引入questInfo.json动态加载的！
        
        
        
        
        
        
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
        //取出bossABilityIndex字符串中最后一个下划线到结尾的字符串
        var abilityIndex = bossAbilityIndex.Substring(bossAbilityIndex.LastIndexOf('_') + 1);
        var index = int.Parse(abilityIndex);
        newIcon.GetComponent<UI_BossAbilityDisplayer>().abilityID = index;



    }

    void ClearBossAbility()
    {
        for (int i = 0; i < _abilityIcons.transform.childCount; i++)
        {
            Destroy(_abilityIcons.transform.GetChild(i).gameObject);
        }
    }

    public void RedirectBoss(GameObject boss)
    {
        SetBoss(boss);
        Init();
    }

    private void OnDestroy()
    {
        
    }
}
