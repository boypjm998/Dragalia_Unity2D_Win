using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using UnityEngine;
using LitJson;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UI_AdventurerSelectionMenu : MonoBehaviour
{
    public int currentSelectedCharaID = 1;

    private JsonData CharacterInfo;
    private JsonData CharacterSkillInfo;
    private JsonData CharacterAbilityInfo;

    private Image Portrait;
    private TextMeshProUGUI characterName;
    private Image[] skillIcons;
    private Image[] abilityIcons;
    private TextMeshProUGUI[] skillName;
    private string[] skillPath;
    private TextMeshProUGUI[] abilityName;
    private string[] abPath;
    private TextMeshProUGUI MaxHPValue;
    private TextMeshProUGUI AttackValue;

    private GlobalController _globalController;
    private Sprite[] iconSprites1;
    private Sprite[] iconSprites2;

    private GameObject resistanceParent;

    private GameObject detailedInfoMenu;
    private Image iconInDetailedInfoMenu;
    private TextMeshProUGUI detailedMenuTitle;
    private TextMeshProUGUI detailedItemName;
    private TextMeshProUGUI description;
    private string[] descriptionString;
    private ScrollRect scrollRect;

    public AssetBundle iconBundle;

    private int deleReg=0;

    private void Awake()
    {
        //GlobalController的事件onGlobalControllerAwake添加Init()方法
        //GlobalController.onGlobalControllerAwake += Init;
        
    }

    private void Start()
    {
        deleReg = 1;
    }


    void InitAllChildren()
    {
        detailedInfoMenu = GameObject.Find("UI").transform.Find("DetailedInfoMenu").gameObject;
        iconInDetailedInfoMenu = detailedInfoMenu.transform.Find("Board").GetChild(0).GetComponent<Image>();
        detailedMenuTitle = detailedInfoMenu.transform.Find("Title").GetComponentInChildren<TextMeshProUGUI>();
        detailedItemName = detailedInfoMenu.transform.Find("Board").GetChild(1).GetComponent<TextMeshProUGUI>();
        description = detailedInfoMenu.transform.Find("Scroll View").Find("Viewport")
            .Find("Text").GetComponentInChildren<TextMeshProUGUI>();
        scrollRect = detailedInfoMenu.transform.Find("Scroll View").GetComponent<ScrollRect>();
        
        Portrait = transform.Find("Portrait").GetComponent<Image>();
        characterName = transform.Find("Banner").GetComponentInChildren<TextMeshProUGUI>();
        resistanceParent = transform.Find("Resistances").gameObject;
        
        
        skillIcons = new Image[4];
        skillName = new TextMeshProUGUI[4];
        skillPath = new string[4];
        abPath = new string[2];
        abilityIcons = new Image[2];
        abilityName = new TextMeshProUGUI[2];
        descriptionString = new string[6];
        
        for (int i = 0; i < 4; i++)
        {
            skillIcons[i] = transform.Find("SkillInfo").GetChild(i).Find("Icon").GetComponent<Image>();
            skillName[i] = transform.Find("SkillInfo").GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
        }
        for (int i = 0; i < 2; i++)
        {
            abilityIcons[i] = transform.Find("AbilityInfo").GetChild(i).Find("Icon").GetComponent<Image>();
            abilityName[i] = transform.Find("AbilityInfo").GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
        }

        MaxHPValue = transform.Find("StatInfo").Find("HP").Find("Value").GetComponent<TextMeshProUGUI>();
        AttackValue = transform.Find("StatInfo").Find("ATK").Find("Value").GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        if (iconBundle == null)
        {
            Init();
        }
        else
        {
            currentSelectedCharaID = GlobalController.currentCharacterID;
            ReloadDisplayedCharacter();
        }

    }

    private void OnDisable()
    {
        
    }

    public void Init()
    {
        //print("Init");
        InitAllChildren();
        _globalController = FindObjectOfType<GlobalController>();
        //日语！！！！！！！！！！！！！！！！！
        if (GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
        {
            CharacterInfo = ReadCharacterInfoData("CharacterInfo_EN.json");
            CharacterSkillInfo = ReadCharacterInfoData("SkillDetailedInfo_EN.json");
            CharacterAbilityInfo = ReadCharacterInfoData("AbilityDetailedInfo_EN.json");
        }
        else if (GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
        {
            CharacterInfo = ReadCharacterInfoData("CharacterInfo.json");
            CharacterSkillInfo = ReadCharacterInfoData("SkillDetailedInfo.json");
            CharacterAbilityInfo = ReadCharacterInfoData("AbilityDetailedInfo.json");
        }
        else
        {
            CharacterInfo = ReadCharacterInfoData("CharacterInfo_JP.json");
            CharacterSkillInfo = ReadCharacterInfoData("SkillDetailedInfo_JP.json");
            CharacterAbilityInfo = ReadCharacterInfoData("AbilityDetailedInfo_JP.json");
        }
        
        var bundle = AssetBundle.GetAllLoadedAssetBundles();
        
        foreach (var ab in bundle)
        {
            if (ab.name == "iconsmall")
                iconBundle = ab;
        }
        
        print(iconBundle);
        
        this.iconBundle =
            _globalController.GetBundle
                ("iconsmall");
        
        //var request = iconBundle.LoadAssetWithSubAssetsAsync<Sprite>("Icon_Skill_Sheet_pro");
        //iconSprites1 = request.allAssets;
        iconSprites1 = iconBundle.LoadAssetWithSubAssets<Sprite>("Icon_Skill_Sheet_pro");
        iconSprites2 = iconBundle.LoadAssetWithSubAssets<Sprite>("Ability_Sheet");
        //print(iconSprites1.Length);
        currentSelectedCharaID = GlobalController.currentCharacterID;
        ReloadDisplayedCharacter();
    }


    // Update is called once per frame
    

    JsonData ReadCharacterInfoData(string name)
    {
        string path = Application.streamingAssetsPath + "/"+ name;
        StreamReader sr = new StreamReader(path);
        var str = sr.ReadToEnd();
        return JsonMapper.ToObject(str);
    }

    public void ChangeCurrentSelectedCharacter(int newID)
    {
        if (newID != currentSelectedCharaID)
        {
            currentSelectedCharaID = newID;
            ReloadDisplayedCharacter();
        }
        
    }

    

    private void ReloadDisplayedCharacter()
    {
        Resources.UnloadUnusedAssets();
        
        
        currentSelectedCharaID = currentSelectedCharaID;
        
        
        Portrait.sprite = Resources.Load<Sprite>($"Portrait/portrait_c{currentSelectedCharaID}");
        var text1 = CharacterInfo[GetCharacterEntirePathUpper(currentSelectedCharaID)]["WEAPON"].ToString();
        switch (text1)
        {
            case "GUN":
                text1 = "<sprite=8>";
                break;
            case "STAFF":
                text1 = "<sprite=7>";
                break;
            case "WAND":
                text1 = "<sprite=6>";
                break;
            case "BOW":
                text1 = "<sprite=5>";
                break;
            case "LANCE":
                text1 = "<sprite=4>";
                break;
            case "DAGGER":
                text1 = "<sprite=3>";
                break;
            case "AXE":
                text1 = "<sprite=2>";
                break;
            case "BLADE":
                text1 = "<sprite=1>";
                break;
            case "SWORD":
                text1 = "<sprite=0>";
                break;
            default:
                text1 = "";
                break;
        }
        var text2 = CharacterInfo[GetCharacterEntirePathUpper(currentSelectedCharaID)]["NAME"].ToString();
        characterName.text = text1 + text2;

        MaxHPValue.text = CharacterInfo[GetCharacterEntirePathUpper(currentSelectedCharaID)]["HP"].ToString();
        AttackValue.text = CharacterInfo[GetCharacterEntirePathUpper(currentSelectedCharaID)]["ATK"].ToString();

        
        for (int i = 0; i < 4; i++)
        {
            skillPath[i] = 
                CharacterInfo[GetCharacterEntirePathUpper(currentSelectedCharaID)]["SK"+(i+1)].ToString();
            //print(skillIcons[i]);
            foreach (var vSprite in iconSprites1)
            {
                if (vSprite.name == CharacterSkillInfo[skillPath[i]]["ICON_PATH"].ToString())
                    skillIcons[i].sprite = vSprite;
            }
            
            
            
            //skillIcons[i].sprite = Array.Find(iconSprites1,
            //    item => item.name == CharacterSkillInfo[skillPath[i]]["ICON_PATH"].ToString());
            skillName[i].text = CharacterSkillInfo[skillPath[i]]["NAME"].ToString();
            descriptionString[i] = CharacterSkillInfo[skillPath[i]]["DESCRIPTION"].ToString();
        }
        
        //var abPath = new string[2];
        for (int i = 0; i < 2; i++)
        {
            abPath[i] = 
                CharacterInfo[GetCharacterEntirePathUpper(currentSelectedCharaID)]["AB"+(i+1)].ToString();
            abilityIcons[i].sprite = Array.Find(iconSprites2,
                item => item.name == CharacterAbilityInfo[abPath[i]]["ICON_PATH"].ToString());
            abilityName[i].text = CharacterAbilityInfo[abPath[i]]["NAME"].ToString();
            descriptionString[i+4] = CharacterAbilityInfo[abPath[i]]["DESCRIPTION"].ToString();
        }

        var resistances = 
            CharacterInfo[GetCharacterEntirePathUpper(currentSelectedCharaID)]["RES"];
        int indexOfResistance = 0;

        for (int i = 0; i < 4; i++)
        {
            if (i <= resistances.Count - 1)
            {
                //print(int.Parse(resistances[i].ToString()));
                resistanceParent.transform.GetChild(i).gameObject.SetActive(true);
                resistanceParent.transform.GetChild(i).GetComponent<Image>().sprite =
                    new TimerBuff(int.Parse(resistances[i].ToString()),-1,-1,100,-1).
                        GetIcon();
            }else
            {
                resistanceParent.transform.GetChild(i).gameObject.SetActive(false);
                resistanceParent.transform.GetChild(i).GetComponent<Image>().sprite = null;
            }
        }



    }

    public static string GetCharacterEntirePathUpper(int id)
    {
        if (id < 10)
        {
            return $"C00{id}";
        }

        if (id < 100)
        {
            return $"C0{id}";
        }else return $"C{id}";
    }

    public void DisplayItemDetailedMenu(int id)
    {
        //令scrollView 的bar回到顶部
        
        scrollRect.verticalNormalizedPosition = 1;
        
        if (id > 4)
        {
            switch (GlobalController.Instance.GameLanguage)
            {
                case GlobalController.Language.EN:
                    detailedMenuTitle.text = "Ability Details";
                    break;
                case GlobalController.Language.ZHCN:
                    detailedMenuTitle.text = "能力详情";
                    break;
                case GlobalController.Language.JP:
                    detailedMenuTitle.text = "アビリティ詳細";
                    break;
            }
            //detailedMenuTitle.text = "能力详情";
            //detailedMenuTitle.text = "アビリティ詳細";
            detailedItemName.text = abilityName[id - 5].text;
            iconInDetailedInfoMenu.sprite = abilityIcons[id - 5].sprite;
            description.text = descriptionString[id-1];
        }
        else
        {
            switch (GlobalController.Instance.GameLanguage)
            {
                case GlobalController.Language.EN:
                    detailedMenuTitle.text = "Skill Details";
                    break;
                case GlobalController.Language.ZHCN:
                    detailedMenuTitle.text = "技能详情";
                    break;
                case GlobalController.Language.JP:
                    detailedMenuTitle.text = "スキル詳細";
                    break;
            }
            //detailedMenuTitle.text = "技能详情";
            //detailedMenuTitle.text = "スキル詳細";
            detailedItemName.text = skillName[id - 1].text;
            iconInDetailedInfoMenu.sprite = skillIcons[id - 1].sprite;
            description.text = descriptionString[id-1];
        }



    }

    private void OnDestroy()
    {
        GlobalController.onGlobalControllerAwake -= Init;

        //Destroy(iconBundle);
    }

    
}
