using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using GameMechanics;
using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PopMenuAfterQuest : MonoBehaviour
{
    [SerializeField] private GameObject _backgroundBanner;
    
    [SerializeField] private CanvasGroup _canvasGroupSkillUpgrade;
    [SerializeField] private Image _charaIconImage;
    [SerializeField] private Image _skillIconBefore;
    [SerializeField] private Image _skillIconAfter;
    [SerializeField] private TextMeshProUGUI _title1;
    [SerializeField] private TextMeshProUGUI _fixedTextPrefix;
    [SerializeField] private TextMeshProUGUI _fixedTextCharaName;
    [SerializeField] private TextMeshProUGUI _fixedTextSkillNameBefore;
    [SerializeField] private TextMeshProUGUI _fixedTextSkillNameAfter;
    [SerializeField] private TextMeshProUGUI _textClickToContinue1;

    [SerializeField] private CanvasGroup _canvasGroupQuestClear;
    [SerializeField] private TextMeshProUGUI _title2;
    [SerializeField] private TextMeshProUGUI _textClickToContinue2;
    [SerializeField] private TextMeshProUGUI _currentQuestName;
    [SerializeField] private TextMeshProUGUI _autoFinishedQuestName;

    private JsonData _charaData;
    private JsonData _skillData;
    private JsonData _questData;
    private List<QuestSave> _playerQuestData = new();
    private AssetBundle _iconBundle;
    private Sprite[] _iconSprites;

    private Coroutine _animRoutine = null;
    private Tweener _tweener;
    private bool _flag = false;
    [SerializeField] private Button _continueButton;
    
    private Queue<(int,int)> _charaSkillLevelUpInfo = new();
    private List<string> _questClearInfo = new();
    
    public bool IsAnimating => _animRoutine != null;

    private void Awake()
    {
        if (GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
        {
            _charaData = BasicCalculation.ReadJsonDataFromStreamingAssets("CharacterInfo_EN.json");
            _skillData = BasicCalculation.ReadJsonDataFromStreamingAssets("SkillDetailedInfo_EN.json");
        }
        else if (GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
        {
            _charaData = BasicCalculation.ReadJsonDataFromStreamingAssets("CharacterInfo.json");
            _skillData = BasicCalculation.ReadJsonDataFromStreamingAssets("SkillDetailedInfo.json");
            //CharacterAbilityInfo = ReadCharacterInfoData("AbilityDetailedInfo.json");
        }

        _questData = GlobalController.Instance.QuestData;
        _playerQuestData = GlobalController.Instance.GetQuestInfo();
        _iconBundle = GlobalController.Instance.GetBundle("iconsmall");
        //_continueButton = _backgroundBanner.GetComponent<Button>();
        _iconSprites = _iconBundle.LoadAllAssets<Sprite>();

    }
    
    public void ClosePanel()
    {
        _flag = true;
    }

    private IEnumerator DisplayCharaSkillLevelUpInfo(int charaID, int skillIDFromOne)
    {
        //Reset CanvasGroup's Visibility
        _canvasGroupQuestClear.gameObject.SetActive(false);
        _canvasGroupSkillUpgrade.gameObject.SetActive(true);
        //Set Button Interactability To False
        _continueButton.interactable = false;

        //Start Banner Animation
        //Canvas Zoom
        _backgroundBanner.transform.localScale = new(1, 0.01f, 1);
        _tweener = _backgroundBanner.transform.DOScaleY(1, 0.2f);

        yield return null;
        yield return new WaitForSeconds(0.2f);

        //Read Texts

        string skillIndex1 = $"SK{skillIDFromOne}";
        string skillIndex2 = $"SK{skillIDFromOne}_LV2";

        string characterIDString =
                UI_AdventurerSelectionMenu.
                GetCharacterEntirePathUpper(charaID);
        string characterName = _charaData[characterIDString]["NAME"].ToString();
        string skillPath1 = _charaData[characterIDString][skillIndex1].ToString();
        string skillPath2 = _charaData[characterIDString][skillIndex2].ToString();
        string skillName1 = _skillData[skillPath1]["NAME"].ToString();
        string skillName2 = _skillData[skillPath2]["NAME"].ToString();
        string skillIconPath1 = _skillData[skillPath1]["ICON_PATH"].ToString();
        string skillIconPath2 = _skillData[skillPath2]["ICON_PATH"].ToString();
        Sprite skillIconSprite1 =
            _iconSprites.FirstOrDefault(x => x.name == skillIconPath1);
        Sprite skillIconSprite2 =
            _iconSprites.FirstOrDefault(x => x.name == skillIconPath2);
        string characterIDStringLowercase = BasicCalculation.ConvertID("icon_c", charaID);
        Sprite charaIconSprite = _iconSprites.ToList().Find(x => x.name == characterIDStringLowercase);
        

        //Set Contents
        if (GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
        {
            _textClickToContinue1.text = "Click To Continue";
            _fixedTextPrefix.text = "Adventurer";
            _title1.text = "Skill Upgrade";
            _fixedTextCharaName.text = $"{characterName}'s skill is upgraded!";
            _fixedTextSkillNameBefore.text = $"{skillName1}";
            _fixedTextSkillNameAfter.text = $"{skillName2}";
        }
        else
        {
            _textClickToContinue1.text = "点击继续";
            _fixedTextPrefix.text = "角色";

            _fixedTextCharaName.text = $"{characterName}的技能效果获得强化！";
            _fixedTextSkillNameBefore.text = $"{skillName1}";
            _fixedTextSkillNameAfter.text = $"{skillName2}";
        }

        _skillIconAfter.sprite = skillIconSprite2;
        _skillIconBefore.sprite = skillIconSprite1;
        _charaIconImage.sprite = charaIconSprite;

        //Canvas Contents Appear
        _tweener = _canvasGroupSkillUpgrade.DOFade(1, 0.3f);

        yield return null;
        yield return new WaitForSeconds(0.3f);

        //WaitUntil Clicked
        _continueButton.interactable = true;
        _flag = false;

        yield return new WaitUntil(() => _flag);
        _flag = false;

        _canvasGroupSkillUpgrade.alpha = 0;
        _continueButton.interactable = false;

        _backgroundBanner.transform.localScale = new(1, 1, 1);
        _tweener = _backgroundBanner.transform.DOScaleY(0.01f, 0.2f);

        yield return null;
        yield return new WaitForSeconds(0.2f);

        _canvasGroupSkillUpgrade.gameObject.SetActive(false);
        _animRoutine = null;

    }

    private IEnumerator DisplayQuestQuickClearInfo(List<string> unlockedQuestID)
    {
        //Reset CanvasGroup's Visibility
        _canvasGroupQuestClear.gameObject.SetActive(true);
        _canvasGroupSkillUpgrade.gameObject.SetActive(false);
        //Set Button Interactability To False
        _continueButton.interactable = false;

        //Start Banner Animation
        //Canvas Zoom
        _backgroundBanner.transform.localScale = new(1, 0.01f, 1);
        _tweener = _backgroundBanner.transform.DOScaleY(1, 0.2f);

        yield return null;
        yield return new WaitForSeconds(0.2f);

        string questNameCurrent = _questData[$"QUEST_{GlobalController.questID}"]["name"].ToString();

        //var questIDList = QuestSeriesInfo.GetFalldownQuests(questNameCurrent);

        //Set Contents
        if (GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
        {
            _textClickToContinue1.text = "Click To Continue";
            _title2.text = "Quest Auto Clear";
            _currentQuestName.text = "You completed all objectives in this quest.\nThe follwing quests will be considered as full-cleared.";
        }
        else
        {
            _textClickToContinue1.text = "点击继续";
            _currentQuestName.text = "由于在任务中完成了全部目标，以下关卡被视为所有目标已完成。";
        }
        
        var sb = new StringBuilder("");
        
        foreach(string questID in unlockedQuestID)
        {
            var questSave = _playerQuestData.Find(x => x.quest_id == questID);
            if(questSave != null)
            {
                if (questSave.IsFullCleared())
                {
                    continue;
                }
            }
            //print(_questData[$"QUEST_{questSave.quest_id}"]["name"].ToString());
            sb.Append(_questData[$"QUEST_{questID}"]["name"].ToString());
            sb.Append("\n");
        }

        _autoFinishedQuestName.text = sb.ToString();


        //Canvas Contents Appear
        _tweener = _canvasGroupQuestClear.DOFade(1, 0.3f);

        yield return null;
        yield return new WaitForSeconds(0.3f);

        //WaitUntil Clicked
        _continueButton.interactable = true;
        _flag = false;

        yield return new WaitUntil(() => _flag);
        _flag = false;

        _canvasGroupQuestClear.alpha = 0;
        _continueButton.interactable = false;

        _backgroundBanner.transform.localScale = new(1, 1, 1);
        _tweener = _backgroundBanner.transform.DOScaleY(0.01f, 0.2f);

        yield return null;
        yield return new WaitForSeconds(0.2f);

        _canvasGroupQuestClear.gameObject.SetActive(false);
        _animRoutine = null;

    }

    public void AddNewSkillUpgradePanel(int cid, int sid)
    {
        _charaSkillLevelUpInfo.Enqueue((cid, sid));
    }
    
    public void AddNewQuestQuickClearPanel(List<string> unlockedQuestID)
    {
        _questClearInfo.AddRange(unlockedQuestID);
        print(_questClearInfo.Count);
    }
    
    public void StartDisplayRoutine()
    {
        StartCoroutine(DisplayMenu());
    }

    private IEnumerator DisplayMenu()
    {
        if(_questClearInfo.Count > 0)
        {
            _animRoutine = StartCoroutine(DisplayQuestQuickClearInfo(_questClearInfo));
            //_questClearInfo.Clear();
            yield return new WaitUntil(()=>_animRoutine == null);
        }
        
        while (_charaSkillLevelUpInfo.Count > 0)
        {
            var (charaID, skillID) = _charaSkillLevelUpInfo.Dequeue();
            _animRoutine = StartCoroutine(DisplayCharaSkillLevelUpInfo(charaID, 
                skillID+1));
            yield return new WaitUntil(() => _animRoutine == null);
            //_charaSkillLevelUpInfo.Dequeue();
        }
        
        gameObject.SetActive(false);

    }

    
}
