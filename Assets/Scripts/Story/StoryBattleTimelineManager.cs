using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameMechanics;
using UnityEngine;
using UnityEngine.UI;

public abstract class StoryBattleTimelineManager : MonoBehaviour
{
    [SerializeField] protected int fixedCharacterID = -1;
    [SerializeField] protected GameObject pauseMenuPrefab;
    [SerializeField] protected GameObject pauseMenuPrefabEN;
    [SerializeField] protected List<GameObject> prefabList = new();
    [SerializeField] protected List<GameObject> enemyList = new();
    [SerializeField] protected List<GameObject> npcList = new();
    [SerializeField] protected bool fadeStartScreen = true;
    
    
    protected Coroutine currentCutSceneCoroutine;
    protected GameObject UIElements;
    private CanvasGroup CharacterCanvasGroup;
    private CanvasGroup CharacterIconCanvasGroup;
    private UI_AdventurerStatusInfo adventurerStatusInfo;
    private CanvasGroup Skill1CanvasGroup;
    private CanvasGroup Skill2CanvasGroup;
    private CanvasGroup Skill3CanvasGroup;
    private CanvasGroup Skill4CanvasGroup;
    
    [SerializeField] protected List<AudioClip> storyVoiceList = new();
    [SerializeField] protected GameObject skipCutSceneButtonPrefab;
    [SerializeField] protected GameObject skipCutSceneButtonPrefabEN;
    protected GameObject skipCutSceneButtonInstance;

    protected int maxEnemyCount;
    protected int currentEnemyCount;
    protected List<AssetBundle> bossBundles = new();
    public enum QuestClearCondition
    {
        DefeatBoss = 0,
        None = 1,
        ClearAllEnemies = 2,
        DefeatBossInTime = 3,
        Custom = 4
    }
    
    public QuestClearCondition questClearCondition;
    public static StoryBattleTimelineManager Instance { get; protected set; }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //todo:需要被动调用
        
    }

    private void OnDestroy()
    {
        if(Instance == this)
            Instance = null;
    }


    /// <summary>
    /// 把场景里的东西放进去
    /// </summary>
    public abstract void InitializeScene();
    public abstract void StartQuest();

    public bool ActiveStartScreen(bool force = false)
    {
        UIElements.transform.Find("StartScreen").gameObject.SetActive(fadeStartScreen || force);
        return fadeStartScreen;
    }

    public void SetVisibilityOfSkipButton(bool visibility)
    {
        GameObject instance;
        if (skipCutSceneButtonInstance == null)
        {
            if (GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
            {
                instance = Instantiate(skipCutSceneButtonPrefabEN, UIElements.transform);
            }
            else
            {
                instance = Instantiate(skipCutSceneButtonPrefab, UIElements.transform);
            }


            skipCutSceneButtonInstance = instance;
            skipCutSceneButtonInstance.GetComponentInChildren<Button>().onClick.AddListener(()=>SkipButtonAction());
        }
        
        skipCutSceneButtonInstance.gameObject.SetActive(visibility);
        
    }

    public virtual void SkipButtonAction()
    {
        
    }

    public void InitStoryQuestInfo(StoryLevelDetailedInfo info,GlobalController controller)
    {
        bossBundles.Clear();
        FindObjectOfType<BattleStageManager>().clearConditionType = (int)(info.clear_condition);
        foreach (var path in info.boss_bundles)
        {
            bossBundles.Add(controller.GetBundle(path));
        }
        
    }

    public virtual void LoadPlayer(int playerID)
    {
        var playerAssetPath = BasicCalculation.ConvertID($"player/player_c",playerID);
        InitializeScene();
        GlobalController.Instance.LoadLocalizedUITest(BattleStageManager.Instance,
            GlobalController.questID);
    }
    
    protected StandardCharacterController GetNPCActor(GameObject go)
    {
        return go.GetComponent<StandardCharacterController>();
    }
    
    protected ActorControllerSpecial GetNPCFlyingActor(GameObject go)
    {
        return go.GetComponent<ActorControllerSpecial>();
    }
    
    protected ActorController GetPlayerActor()
    {
        return FindObjectOfType<ActorController>();
    }
    
    protected PlayerInput GetPlayerInput()
    {
        return FindObjectOfType<PlayerInput>();
    }
    
    protected void PlayAnimation(ActorBase actor, string animName, float normalizedTime = 0)
    {
        actor.anim.Play(animName,-1,normalizedTime);
    }
    
    public void PlayStoryVoiceWithDialog(int id, int speakerID, AudioClip clip, AudioSource source = null)
    {
        if(source == null)
            source = BattleEffectManager.Instance.sharedVoiceSource;
        UI_DialogDisplayerStory.Instance.
            EnqueueDialogSharedInBasicStory
                (speakerID,id, clip);
    }

    public void PlayStoryVoiceWithoutDialog(AudioClip clip)
    {
        BattleEffectManager.Instance.sharedVoiceSource.PlayOneShot(clip);
    }

    protected List<GameObject> GetAllEffectsInCharacter(GameObject go)
    {
        List<GameObject> foundObjects = new List<GameObject>();
        foreach (Transform child in go.transform)
        {
            if (child.CompareTag("EffectInCharacter"))
            {
                foundObjects.Add(child.gameObject);
            }
            foundObjects.AddRange(GetAllEffectsInCharacter(child.gameObject));
        }
        return foundObjects;

    }

    

    protected void SetBehavior(EnemyBehaviorManager behavior, string bossBundleName, string behaviorTextName)
    {
        var bundle = bossBundles.Find(x => x.name == bossBundleName);
        if (bundle == null)
        {
            try
            {
                
                var assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bossBundleName));
                GlobalController.Instance.loadedBundles.Add(bossBundleName, assetBundle);
                bundle = assetBundle;
            }
            catch
            {
                Debug.LogError("No Bundle Found");
                return;
            }
        }
        var textAsset = bundle.LoadAsset<TextAsset>(behaviorTextName);
        if (textAsset == null)
        {
            Debug.LogError("TextAsset not found");
            return;
        }
        behavior.SetBehavior(textAsset);
        
    }


    protected void OpenTutorialHintPauseMenuAndTurnToNewestPage()
    {
        BattleSceneUIManager.Instance.OpenPauseMenu();
        UI_Tutorial_PauseMenu.Instance.UpdatePanel(new[]
            {
                "PagesIncrement" 
            }
        );
    }
    protected void OpenTutorialHintFirstPage()
    {
        BattleSceneUIManager.Instance.OpenPauseMenu();
        UI_Tutorial_PauseMenu.Instance.UpdatePanel(new[]
            {
                "ShowFirstPage" 
            }
        );
    }

    protected void InitUIElements()
    {
        
        CharacterIconCanvasGroup = UIElements.transform.Find("CharacterInfo/CharacterStatus").GetComponent<CanvasGroup>();
        adventurerStatusInfo = UIElements.transform.Find("CharacterInfo/CharacterStatus").GetComponent<UI_AdventurerStatusInfo>();
        Skill1CanvasGroup = UIElements.transform.Find("CharacterInfo/Skill01").GetComponent<CanvasGroup>();
        Skill2CanvasGroup = UIElements.transform.Find("CharacterInfo/Skill02").GetComponent<CanvasGroup>();
        Skill3CanvasGroup = UIElements.transform.Find("CharacterInfo/Skill03").GetComponent<CanvasGroup>();
        Skill4CanvasGroup = UIElements.transform.Find("CharacterInfo/Skill04").GetComponent<CanvasGroup>();
        CharacterCanvasGroup = UIElements.transform.Find("CharacterInfo").GetComponent<CanvasGroup>();
    }

    protected void SetCharacterIconFacialExpression(int ID)
    {
        adventurerStatusInfo.SetImage(ID);
    }
    
    protected void SetCharacterIconFacialExpression(Sprite sprite)
    {
        adventurerStatusInfo.SetImage(sprite);
    }
    
    protected void SetCharacterUIAlpha(float alpha)
    {
        CharacterCanvasGroup.alpha = alpha;
    }

    protected void SetCharacterIconAlpha(float alpha)
    {
        CharacterIconCanvasGroup.alpha = alpha;
    }

    protected void SetSkillIconAlpha(int skillId, float alpha)
    {
        switch (skillId)
        {
            case 1:
                Skill1CanvasGroup.alpha = alpha;
                break;
            case 2:
                Skill2CanvasGroup.alpha = alpha;
                break;
            case 3:
                Skill3CanvasGroup.alpha = alpha;
                break;
            case 4:
                Skill4CanvasGroup.alpha = alpha;
                break;
            case -1:
            {
                Skill1CanvasGroup.alpha = alpha;
                Skill2CanvasGroup.alpha = alpha;
                Skill3CanvasGroup.alpha = alpha;
                Skill4CanvasGroup.alpha = alpha;
                break;
            }
        }
    }

    protected GameObject SpawnNewPauseMenu()
    {
        GameObject menu;

        if (GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
            menu = Instantiate(pauseMenuPrefab, UIElements.transform);
        else menu = Instantiate(pauseMenuPrefabEN, UIElements.transform);



            return menu;
    }

    protected GameObject SpawnFXPrefab(GameObject prefab, Vector3 position, int direction = 1)
    {
        var go = Instantiate(prefab, position, Quaternion.identity,
            BattleStageManager.Instance.RangedAttackFXLayer.transform);
        go.transform.localScale = new Vector3(go.transform.localScale.x * direction, go.transform.localScale.y,
            go.transform.localScale.z);
        return go;
    }
    
    protected GameObject SpawnEnemyPrefab(GameObject prefab, Vector3 position, bool summon = false)
    {
        var go = Instantiate(prefab, position, Quaternion.identity,
            BattleStageManager.Instance.EnemyLayer.transform);

        var enemyStat = go.GetComponent<StatusManager>();

        if (summon)
        {
            prefab.GetComponent<EnemyController>().SetSummoned(true);
            return go;
        }
        
        currentEnemyCount++;

        StatusManager.StatusManagerVoidDelegate handle = null;
        enemyStat.OnHPBelow0 += () =>
        {
            enemyStat.OnHPBelow0 -= handle;
            EnemyEliminated();
        };
        
        return go;
    }

    protected void SwitchCameraTo(GameObject go)
    {
        StageCameraController.SwitchMainCameraFollowObject(go);
    }
    
    private void EnemyEliminated()
    {
        currentEnemyCount--;
        maxEnemyCount--;
    }

}
