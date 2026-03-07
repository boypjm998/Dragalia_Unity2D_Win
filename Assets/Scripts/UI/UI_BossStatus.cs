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
    [SerializeField] private GameObject bossPartUIPrefab;
    private TextMeshProUGUI _bossName;

    protected CanvasGroup _canvasGroup;
    private JsonData bossAbilityDetailData;

    private GlobalController _globalController;
    protected BattleStageManager _battleStageManager;

    public int bossIndex;

    private DragaliaEnemyBehavior _behavior;
    private UI_BodyPartStatus _bodyPartStatus;

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
            var odBarComponent = odBar.GetComponent<UI_BossODBar>();
            odBarComponent.bossStat = bossStat as SpecialStatusManager;
            UI_BossODBar.Instance = odBarComponent;
        }


        _HPbar = GetComponentInChildren<UI_BossHPBar>();
        _bossName = GetComponentInChildren<TextMeshProUGUI>();
        _conditionBar = GetComponentInChildren<UI_BossConditionBar>();
        _abilityIcons = transform.Find("BossAbilities").gameObject;

        switch (GlobalController.Instance.GameLanguage)
        {
            case GlobalController.Language.ZHCN:
                bossAbilityDetailData = BasicCalculation.
                    ReadJsonDataFromStreamingAssets("/LevelInformation/BossAbilityDetail_ZH.json");
                break;
            case GlobalController.Language.EN:
                bossAbilityDetailData = BasicCalculation.
                    ReadJsonDataFromStreamingAssets("/LevelInformation/BossAbilityDetail_EN.json");
                break;
            default:
                Debug.LogError("No such language");
                break;
        }
        // bossAbilityDetailData = BasicCalculation.
        //     ReadJsonData("/LevelInformation/BossAbilityDetail_ZH.json");
        
        
        _bossName.text = bossStat.displayedName;
        _conditionBar.SetTargetStat(bossStat);
        bossStat.SetConditionBar(_conditionBar);
        _HPbar.SetTarget(bossStat);
        this.bossStat = bossStat;
        _HPbar.OnHPChange();
        
        
        
        var levelDetailedInfo = _battleStageManager.GetLevelDetailedInfo();
        var bossAbilities = new List<string>();

        try
        {
            bossAbilities =
                levelDetailedInfo.boss_prefab[bossIndex].boss_abilities;
        }
        catch
        {
            Debug.LogWarning("No boss ability info");
            foreach (var ab in bossStat.abilityList)
            {
                bossAbilities.Add($"BOSS_ABILITY_{ab}");
                print("ADDED" + $"BOSS_ABILITY_{ab}");
            }
        }



        ClearBossAbility();
        
        foreach (var ability in bossAbilities)
        {
            AddBossAbility(ability);
        }

        _behavior = bossStat.GetComponent<DragaliaEnemyBehavior>();
        if (_behavior != null)
        {
            _behavior.OnPartAdded += AddPart;
            _behavior.OnPartRemoved += RemovePart;
            _behavior.OnPartBroken += PartBroken;
        }






    }

    /// <summary>
    /// get boss ability info
    /// </summary>
    /// <param name="bossAbilityIndex">eg:BOSS_ABILITY_0011</param>
    void AddBossAbility(string bossAbilityIndex)
    {
        //
        var bundle = _globalController.GetBundle("boss_ability_icon");
        JsonData abilityData;
        try
        {
            abilityData = bossAbilityDetailData[bossAbilityIndex];
        }
        catch
        {
            Debug.LogWarning("No boss ability info");
            return;
        }
        
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
        newIcon.GetComponent<UI_BossAbilityDisplayer>().stat = bossStat;
        



    }

    void ClearBossAbility()
    {
        for (int i = 0; i < _abilityIcons.transform.childCount; i++)
        {
            Destroy(_abilityIcons.transform.GetChild(i).gameObject);
        }
    }

    public void RedirectBoss(GameObject boss,int index = 0)
    {
        SetBoss(boss,index);
        Init();
    }

    private void OnDestroy()
    {
        if(_behavior != null)
        {
            _behavior.OnPartAdded -= AddPart;
            _behavior.OnPartRemoved -= RemovePart;
            _behavior.OnPartBroken -= PartBroken;
        }
    }

    protected void AddPart(GameObject partGameObject, int partID)
    {
        var partGO = Instantiate(bossPartUIPrefab, transform.Find("Parts"));
        _bodyPartStatus = partGO.GetComponent<UI_BodyPartStatus>();
        _bodyPartStatus.partStat = partGameObject.GetComponent<PartStatusManager>();
        //_bodyPartStatus.
    }
    
    protected void RemovePart(GameObject partGameObject, int partID)
    {
        if (_bodyPartStatus != null)
        {
            Destroy(_bodyPartStatus.gameObject);
            _bodyPartStatus = null;
        }
    }
    
    protected void PartBroken(GameObject partGameObject, int partID)
    {
        //_bodyPartStatus
    }
    
    
}


public class EnemyAbilityIconEvent
{
    public enum EventType
    {
        DisplayOrHide = 0,
        PlusOrMinus = 1,
        SetNumber = 3,
        SetText = 7,
        Custom = 15,
        IconActive = 31
    }

    public EventType Type { get; private set; }
    private string _stringInfo;
    private int _intInfo = 0;
    private Action<UI_BossAbilityDisplayer> _customAction = null;

    public string Message
    {
        get
        {
            if(Type == EventType.PlusOrMinus)
            {
                return _intInfo.ToString();
            }
            else if (Type == EventType.SetNumber || Type == EventType.DisplayOrHide || Type == EventType.IconActive)
            {
                return _intInfo.ToString();
            }
            else
            {
                return _stringInfo;
            }
        }
    }

    public void DoAction(UI_BossAbilityDisplayer displayer)
    {
        if(_customAction != null)
        {
            _customAction(displayer);
            Debug.Log("Custom action done");
        }
    }

    public EnemyAbilityIconEvent(EventType type, string stringInfo)
    {
        _stringInfo = stringInfo;
        Type = type;
    }

    public EnemyAbilityIconEvent(EventType type, int intInfo)
    {
        Type = type;
        _intInfo = intInfo;
    }

    public EnemyAbilityIconEvent(bool active)
    {
        Type = EventType.DisplayOrHide;
        _intInfo = active ? 1 : 0;
    }

    public EnemyAbilityIconEvent(int iconActive)
    {
        Type = EventType.IconActive;
        //_stringInfo = "";
        _intInfo = iconActive == 0 ? 0 : 1;
    }
    
    public EnemyAbilityIconEvent(Action<UI_BossAbilityDisplayer> customAction)
    {
        Type = EventType.Custom;
        _customAction = customAction;
        _stringInfo = customAction.ToString();
    }



}
