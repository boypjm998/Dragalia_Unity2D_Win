using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cinemachine;
using LitJson;
using TMPro;
using GameMechanics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalController : MonoBehaviour
{
    public static GlobalController Instance;
    
    private Transform cameraTransform;
    private GameObject UIFXContainer;
    public int clickEffCD = 0;
    public enum Language
    {
        ZHCN,
        JP,
        EN
    }

    public enum GameState
    {
        Outbattle,
        WaitForStart,
        Inbattle,
        End //有一方判定死亡
    }

    public static GameState currentGameState { protected set; get; }
    public Dictionary<string, AssetBundle> loadedBundles;
    protected JsonData QuestInfo;
    public delegate void OnGlobalControllerAwake();
    public static OnGlobalControllerAwake onGlobalControllerAwake;

    #region GameOption

    public Language GameLanguage;
    public static int currentCharacterID = 1;
    
    public static string keyRight = "d";
    public static string keyLeft = "a";
    public static string keyDown = "s";
    public static string keySpecial = "w";
    public static string keyAttack = "j";
    public static string keyJump = "k";
    public static string keyRoll = "l";
    public static string keySkill1 = "u";
    public static string keySkill2 = "i";
    public static string keySkill3 = "o";
    public static string keySkill4 = "h";
    
    #endregion
    
    #region GameCheckpoint

    public static int lastQuestSpot = -1;
    public static string questSaveDataString;

    #endregion
    
    
    
    
    
    
    

    protected Coroutine loadingRoutine;
    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private GameObject clickEff;
    
    public static string questID = "000000";
    public static int viewerID = 0;
    public bool loadingEnd = true;
    

    private void Awake()
    {
        
        QuestInfo = BasicCalculation.ReadJsonData("LevelInformation/QuestInfo.json");
        var other = FindObjectsOfType<GlobalController>();
        if (other.Length > 1)
        {
            this.enabled = false;
            Destroy(gameObject);
        }
        Instance = this;
        this.loadedBundles = new Dictionary<string, AssetBundle>();
        UIFXContainer = GameObject.Find("UIFXContainer");
    }

    void Start()
    {
        if (GetBundle("iconsmall") == null || GetBundle("allin1") == null)
        {
            var ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/iconsmall");
            loadedBundles.Add("iconsmall",ab);
            ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/allin1");
            loadedBundles.Add("allin1",ab);
        }
        cameraTransform = GameObject.Find("Main Camera").transform;
        currentGameState = GameState.Outbattle;
        onGlobalControllerAwake?.Invoke();
        UpdateQuestSaveData();
        
        
    }

    private void Update()
    {
        
        transform.position = cameraTransform.position;
        
        
        if (Input.GetMouseButtonDown(0) && clickEffCD<0)
        {
            clickEffCD = 15;
            
            var click = Instantiate(clickEff, Camera.main.ScreenToWorldPoint(Input.mousePosition)
                , Quaternion.identity,UIFXContainer.transform);
            
        }

        clickEffCD--;
    }

    public void TestReturnMainMenu()
    {
        if(loadingRoutine!=null)
            return;
        loadingRoutine = StartCoroutine(LoadMainMenu());
    }


    // Update is called once per frame
    public void TestEnterLevel(string levelName = "010013")
    {
        if(loadingRoutine!=null)
            return;
        loadingRoutine = StartCoroutine(LoadBattleScene(levelName));
    }
    
    IEnumerator LoadBattleScene(string questID){
        //异步加载场景

        GlobalController.questID = questID;
        loadingEnd = false;
        var loadingScreen = Instantiate(LoadingScreen, Vector3.zero, Quaternion.identity, transform);
        var anim = loadingScreen.GetComponent<Animator>();
        //yield return new WaitForSecondsRealtime(2f);
        
        //load level
        bool levelAssetLoaded = false;
        
        AssetBundleCreateRequest ar = null;

        JsonData currentQuestInfo = QuestInfo[$"QUEST_{questID}"];
        var currentLevelDetailedInfo = JsonMapper.ToObject<LevelDetailedInfo>(currentQuestInfo.ToJson());

        
        List<String> requiredBundleList = new List<string>();
        
        var needLoad = SearchPlayerRelatedAssets(1);
        
        List<AssetBundle> assetBundles = new List<AssetBundle>();
        int index = 0;
        
        requiredBundleList.Add(needLoad[0]);
        requiredBundleList.Add(needLoad[1]);
        requiredBundleList.Add(needLoad[2]);
        requiredBundleList.Add("ui_general");
        requiredBundleList.Add("eff_general");
        requiredBundleList.Add("animation/anim_common");
        requiredBundleList.Add("allin1");
        requiredBundleList.Add("118effbundle");
        requiredBundleList.Add("boss_ability_icon");
        //requiredBundleList.Add("voice_c005");
        
        //print(requiredBundleList.ToArray());
        

        foreach (var abpath in requiredBundleList)
        {
            if (!loadedBundles.ContainsKey(abpath))
            {
                ar =
                    AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, abpath));
                yield return ar;
                loadedBundles.Add(abpath,ar.assetBundle);
                assetBundles.Add(ar.assetBundle);
            }
            else
            {
                assetBundles.Add(loadedBundles[abpath]);
            }

            //index++;
        }
        
        
        
        
        
        

        
        
        //Load ab file end
        
        currentGameState = GameState.WaitForStart;
        AsyncOperation ao = SceneManager.LoadSceneAsync("BattleScene");
        DontDestroyOnLoad(this.gameObject);
        yield return ao;
        
        //摄像机跟随屏幕
        cameraTransform = GameObject.Find("Main Camera").transform;
        loadingScreen.transform.position = cameraTransform.transform.position;
        UIFXContainer = GameObject.Find("UIFXContainer");
        
        //初始化battleStageManager:
        var battleStageManager = FindObjectOfType<BattleStageManager>();
        
        battleStageManager.LoadLevelDetailedInfo(currentCharacterID,currentLevelDetailedInfo);

        
        
        // battleStageManager.GetLevelInfo(currentCharacterID,
        //     currentQuestInfo["name"].ToString(),
        //     (int)currentQuestInfo["time_limit"],
        //     (int)currentQuestInfo["crown_time_limit"],
        //     (int)currentQuestInfo["total_boss_num"],
        //     (int)currentQuestInfo["revive_limit"],
        //     (int)currentQuestInfo["crown_revive_limit"]);
        
        
        
        
        
        
        
        
        //加载玩家部分
        var plr = assetBundles[0].LoadAsset<GameObject>("PlayerHandle");
        var plrlayer = GameObject.Find("Player");
        var plrclone = 
            Instantiate(plr, new Vector3(-4.5f, -2f, 0), transform.rotation, plrlayer.transform);
        plrclone.name = "PlayerHandle";
        LoadLocalizedUITest(battleStageManager,questID);
        battleStageManager.InitPlayer(plrclone);
        plrclone.GetComponent<PlayerStatusManager>().GetPlayerConditionBar();
        plrclone.GetComponent<PlayerStatusManager>().remainReviveTimes = 3;
        plrclone.GetComponent<PlayerInput>().enabled = false;
        //加载本地化相关
        yield return null;
        anim.SetBool("loaded",true);
        var Chara_UI = GameObject.Find("CharacterInfo");
        Chara_UI.SetActive(false);
        
        
        
        var enemyDependencies = battleStageManager.GetEnemyDependencies();
        
        foreach (var abpath in enemyDependencies)
        {
            if (!loadedBundles.ContainsKey(abpath))
            {
                ar =
                    AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, abpath));
                yield return ar;
                loadedBundles.Add(abpath,ar.assetBundle);
                assetBundles.Add(ar.assetBundle);
            }
            else
            {
                assetBundles.Add(loadedBundles[abpath]);
            }

            //index++;
        }
        
        battleStageManager.LinkBossStatus();
        
        

        
        
        
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("End"));
        Destroy(loadingScreen);
        loadingEnd = true;
        //Time.timeScale = 1;
        //var mainCamera = GameObject.Find("Main Camera");
        //var cinemachineVirtualCamera = mainCamera.GetComponentInChildren<CinemachineVirtualCamera>();
        //plrclone.GetComponentInChildren<TargetAimer>().enabled = true;

        yield return new WaitForSeconds(2.5f);
        GameObject.Find("StartScreen").GetComponent<UI_StartScreen>().FadeOut();
        
        yield return new WaitForSeconds(1.5f);
        currentGameState = GameState.Inbattle;
        plrclone.GetComponent<PlayerInput>().enabled = true;
        Chara_UI.SetActive(true);
        //battleStageManager

        loadingRoutine = null;
    }

    IEnumerator LoadMainMenu()
    {
        currentGameState = GameState.End;
        loadingEnd = false;
        FindObjectOfType<TargetAimer>().enabled = false;
        yield return null;

        var loadingScreen = Instantiate(LoadingScreen, Camera.main.transform.position, Quaternion.identity, transform);
        var anim = loadingScreen.GetComponent<Animator>();
        
        yield return new WaitForSecondsRealtime(2);

        
        AsyncOperation ao = SceneManager.LoadSceneAsync("MainMenu");
        DontDestroyOnLoad(this.gameObject);
        yield return ao;
        cameraTransform = GameObject.Find("Main Camera").transform;
        
        
        //AssetBundle.UnloadAllAssetBundles(true);
        var bundles = AssetBundle.GetAllLoadedAssetBundles();
        print(bundles.Count());
        foreach (var ab in bundles)
        {
            if (ab.name == "iconsmall" || ab.name == "allin1")
            {
                continue;
            }

            loadedBundles.Remove(ab.name);
            var async = ab.UnloadAsync(true);
            yield return async;
            
        }
        
        

        UpdateQuestSaveData();
        yield return null;
        
        //Resources.UnloadUnusedAssets();
        
        loadingScreen.transform.position = Vector3.zero;
        //loadedBundles.Clear();

        //print(FindObjectOfType<UI_AdventurerSelectionMenu>().
            //iconBundle.LoadAssetWithSubAssets<Sprite>("Iconsmall")[0].name);
        
        
        Time.timeScale = 1;
        cameraTransform = Camera.main.transform;
        UIFXContainer = GameObject.Find("UIFXContainer");
        
        onGlobalControllerAwake?.Invoke();
        var menuUIManager = FindObjectOfType<MenuUIManager>();
        menuUIManager.menuLevelStack.Push(101);
        yield return null;
        menuUIManager.ToNextUIState(1010);
        
        
        anim.SetBool("loaded",true);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("End"));
        Destroy(loadingScreen);
        this.enabled = true;
        //print(loadedBundles.ContainsKey("Iconsmall"));
        loadingEnd = true;
        currentGameState = GameState.Outbattle;
        onGlobalControllerAwake?.Invoke();
        MenuUIManager.SetMaxMenuLevel(1);
        UpdateQuestSaveData();


        loadingRoutine = null;
    }

    protected virtual void LoadPlayer(int characterID)
    {
        var assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "c001"));
        var plr = assetBundle.LoadAsset<GameObject>("PlayerHandle");
        var plrlayer = GameObject.Find("Player");
        var plrclone = Instantiate(plr, new Vector3(4.5f, -2.3f, 0), transform.rotation, plrlayer.transform);
        plrclone.name = "PlayerHandle";
        
    }

    public AssetBundle GetBundle(string name)
    {
        if (loadedBundles.ContainsKey(name))
        {
            return loadedBundles[name];
        }
        Debug.LogWarning($"No Bundle called {name} is loaded.");
        return null;
    }

    string[] SearchPlayerRelatedAssets(int id)
    {
        var needLoad = new string[4];
        needLoad[0] = BasicCalculation.ConvertID("model/model_c", id);
        needLoad[1] = BasicCalculation.ConvertID("voice_c", id);
        needLoad[2] = BasicCalculation.ConvertID("eff_c", id);
        needLoad[3] = BasicCalculation.ConvertID("ui_c", id);
        return needLoad;
    }

    void LoadLocalizedUITest(BattleStageManager battleStageManager,string questID)
    {
        battleStageManager.quest_id = questID;
        if (GameLanguage == Language.ZHCN)
        {
            //battleStageManager.quest_name = BasicCalculation.GetQuestNameZH(questID);
            //Load Boss Status
        }

        if (GameLanguage == Language.JP)
        {
            //battleStageManager.quest_name = BasicCalculation.GetQuestNameJP(questID);
            battleStageManager.buffLogPrefab = Resources.Load<GameObject>("UI/InBattle/BuffLogText/BuffText_JP");
        }


    }

    public static void BattleFinished(bool win)
    {
        var playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (var playerInput in playerInputs)
        {
            playerInput.enabled = false;
        }
        
        currentGameState = GameState.End;
        var battleStageManager = FindObjectOfType<BattleStageManager>();
        if(win)
        {
            battleStageManager.SetGameCleared();
        }
        else
        {
            battleStageManager.SetGameFailed();
        }
        
    }

    public static void UpdateQuestSaveData()
    {
        string path = Application.streamingAssetsPath + "/savedata/testSaveData.json";
        StreamReader sr = new StreamReader(path);
        var str = sr.ReadToEnd();
        sr.Close();
        questSaveDataString = str;
        

    }


}
