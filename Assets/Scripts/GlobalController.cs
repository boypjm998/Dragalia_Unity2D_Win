using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cinemachine;
using LitJson;
using TMPro;
using GameMechanics;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalController : MonoBehaviour
{
    public static GlobalController Instance { get; protected set; }

    private Transform cameraTransform;
    private GameObject UIFXContainer;
    public int clickEffCD = 0;
    public enum Language
    {
        ZHCN = 0,
        JP = 2,
        EN = 1
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
    protected JsonData SettingsInfo;
    public GameOptions gameOptions;
    protected List<StatusInformation> StatusInfoList;
    public delegate void OnGlobalControllerAwake();
    public static OnGlobalControllerAwake onGlobalControllerAwake;
    public event OnGlobalControllerAwake OnLoadFinish;

    #region GameOption

    public Language GameLanguage;
    public static int currentCharacterID = 1;
    
    
    public static KeyCode keyRight = KeyCode.D;
    public static KeyCode keyLeft = KeyCode.A;
    public static KeyCode keyDown = KeyCode.S;
    public static KeyCode keySpecial = KeyCode.Space;
    public static KeyCode keyAttack = KeyCode.J;
    public static KeyCode keyJump = KeyCode.K;
    public static KeyCode keyRoll = KeyCode.L;
    public static KeyCode keySkill1 = KeyCode.U;
    public static KeyCode keySkill2 = KeyCode.I;
    public static KeyCode keySkill3 = KeyCode.O;
    public static KeyCode keySkill4 = KeyCode.H;
    public static KeyCode keyEscape = KeyCode.Escape;
    
    
    
    #endregion
    
    #region GameCheckpoint

    public static int lastQuestSpot = -1;
    public List<int> questEnterRecordList = new();
    public static string questSaveDataString;

    #endregion

    #region GameData

    [SerializeField] protected CharacterAssetInfo CharaAssetData;

    #endregion
    
    

    public bool loadingRoutineEnd => loadingRoutine == null;
    protected Coroutine loadingRoutine;
    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private GameObject clickEff;
    
    public static string questID = "000000";
    public static int viewerID = 0;
    public bool loadingEnd = true;

    public bool debug;
    

    protected void Awake()
    {
        //读入各种文件
        if (GameLanguage == Language.ZHCN)
        {
            QuestInfo = BasicCalculation.ReadJsonData("LevelInformation/QuestInfo.json");
        }else if (GameLanguage == Language.EN)
        {
            QuestInfo = BasicCalculation.ReadJsonData("LevelInformation/QuestInfo_EN.json");
        }
        else
        {
            Debug.LogError("Language not supported");
        }

        
        SettingsInfo = BasicCalculation.ReadJsonData("savedata/PlayerSettings.json");
        
        var other = FindObjectsOfType<GlobalController>();
        if (other.Length > 1)
        {
            this.enabled = false;
            Destroy(gameObject);
            return;
        }
        Instance = this;
        this.loadedBundles = new Dictionary<string, AssetBundle>();
        LoadPlayerSettings();
        LoadGameOptionsFromFile();
        ReadStatusInformation();
        CharaAssetData?.Init();
        UIFXContainer = GameObject.Find("UIFXContainer");
        
        
        
    }

    void Start()
    {
        SceneManager.sceneLoaded += ResetAllAudioSources;
        
        ResetAllAudioSources(SceneManager.GetActiveScene(),LoadSceneMode.Single);
        
        if (GetBundle("iconsmall") == null || GetBundle("allin1") == null)
        {
            var ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/iconsmall");
            loadedBundles.Add("iconsmall",ab);
            ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/allin1");
            loadedBundles.Add("allin1",ab);
            try
            {
                ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/eff/eff_general");
                loadedBundles.Add("eff/eff_general", ab);
            }
            catch
            {
                
            }

            try
            {
                ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/animation/anim_common");
                loadedBundles.Add("animation/anim_common",ab);
            }
            catch
            {
                print("重复加载animation/anim_common");
            }

        }
        cameraTransform = GameObject.Find("Main Camera").transform;
        currentGameState = GameState.Outbattle;
        onGlobalControllerAwake?.Invoke();
        UpdateQuestSaveData();
        
        
    }

    private void Update()
    {
        if (debug)
        {
            print("currentChara:"+currentCharacterID);
            //BattleEffectManager.Instance.PlayOtherSE("SE_ACTION_GUN_001");
            debug = false;
        }

        transform.position = cameraTransform.position;
        
        
        if (Input.GetMouseButtonDown(0) && clickEffCD<0)
        {
            clickEffCD = 15;
            
            var click = Instantiate(clickEff, Camera.main.ScreenToWorldPoint(Input.mousePosition)
                , Quaternion.identity,UIFXContainer.transform);
            
        }

        if (clickEffCD<-1)
        {
            clickEffCD = -1;
        }
        else
        {
            clickEffCD--;
        }


        //print(loadingEnd);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ResetAllAudioSources;
    }

    public void TestReturnMainMenu()
    {
        if(loadingRoutine!=null)
            return;
        loadingRoutine = StartCoroutine(LoadMainMenu());
    }


    // Update is called once per frame
    public void TestEnterLevel(string levelName = "01013")
    {
        if(loadingRoutine!=null)
            return;
        loadingRoutine = StartCoroutine(LoadBattleScene(levelName));
    }

    public void EnterPrologue()
    {
        if(loadingRoutine!=null)
            return;
        loadingRoutine = StartCoroutine(LoadPrologueScene());
    }
    
    public void EnterPrologueStory()
    {
        if(loadingRoutine!=null)
            return;
        loadingRoutine = StartCoroutine(LoadStorySceneOfPrologue());
    }

    public void EnterNormalStory(string questID)
    {
        if(loadingRoutine!=null)
            return;
        loadingRoutine = StartCoroutine(LoadStorySceneOfNormalStory(questID));
    }

    public void EnterNormalStoryBattle(string questID)
    {
        if(loadingRoutine!=null)
            return;
        loadingRoutine = StartCoroutine(LoadBattleSceneOfNormalStory(questID));
    }

    protected IEnumerator LoadBattleScene(string questID)
    {
        //异步加载场景

        GlobalController.questID = questID;
        FinishLoad(false);
        var loadingScreen = Instantiate(LoadingScreen, Vector3.zero, Quaternion.identity, transform);
        var anim = loadingScreen.GetComponent<Animator>();
        //yield return new WaitForSecondsRealtime(2f);

        var clonedStack = new Stack<int>(new Stack<int>(MenuUIManager.Instance.menuLevelStack));
        //将栈顶元素一一加入到questEnterRecordList中，直到取出的栈顶元素为1010或者栈空
        questEnterRecordList.Clear();
        while (clonedStack.Count > 0)
        {
            var top = clonedStack.Pop();
            if (top == 1010)
                break;
            questEnterRecordList.Add(top);
        }




        //load level
        bool levelAssetLoaded = false;

        AssetBundleCreateRequest ar = null;

        JsonData currentQuestInfo = QuestInfo[$"QUEST_{questID}"];
        var currentLevelDetailedInfo = JsonMapper.ToObject<LevelDetailedInfo>(currentQuestInfo.ToJson());


        List<String> requiredBundleList = new List<string>();

        var needLoad = SearchPlayerRelatedAssets(currentCharacterID);

        List<AssetBundle> assetBundles = new List<AssetBundle>();
        int index = 0;

        requiredBundleList.Add(needLoad[0]);
        requiredBundleList.Add(needLoad[1]);
        requiredBundleList.Add(needLoad[2]);
        requiredBundleList.Add(needLoad[3]);
        requiredBundleList.Add("ui_general");
        //requiredBundleList.Add("eff/eff_general");
        //requiredBundleList.Add("animation/anim_common");
        //requiredBundleList.Add("allin1");
        requiredBundleList.Add("soundeffect/soundeffect_common");
        requiredBundleList.Add("118effbundle");
        requiredBundleList.Add("boss_ability_icon");

        //TODO: 加载龙的ab包

        var extraCharaAssets = GetExtraCharacterAssets(currentCharacterID);
        if (extraCharaAssets != null)
        {
            requiredBundleList.AddRange(extraCharaAssets);
        }
        // if (currentCharacterID == 10)
        // {
        //     requiredBundleList.Add("eff/eff_d010");
        //     requiredBundleList.Add("model/model_d010");
        // }
        
        
        
        


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

        
        //加载UI
        var uiBundleName = needLoad[4];
        AssetBundle uiBundle;
        if (!loadedBundles.ContainsKey(uiBundleName))
        {
            ar =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, uiBundleName));
            yield return ar;
            loadedBundles.Add(uiBundleName,ar.assetBundle);
            assetBundles.Add(ar.assetBundle);
            uiBundle = ar.assetBundle;
        }
        else
        {
            assetBundles.Add(loadedBundles[uiBundleName]);
            uiBundle = loadedBundles[uiBundleName];
        }
        
        var UICharaInfoAsset = uiBundle.LoadAsset<GameObject>("CharacterInfo");
        
        var voiceAssetReq = assetBundles[3].LoadAllAssetsAsync<AudioClip>();
        yield return voiceAssetReq;
        
        //Load ab file end
        
        string sceneName;
        switch (GameLanguage)
        {
            case Language.ZHCN:
                sceneName = "BattleScene";
                break;
            case Language.EN:
                sceneName = "BattleScene_EN";
                break;
            default:
                Debug.LogError("GameLanguage is not set");
                yield break;
        }

        currentGameState = GameState.WaitForStart;
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        DontDestroyOnLoad(this.gameObject);
        yield return ao;
        
        //摄像机跟随屏幕
        cameraTransform = GameObject.Find("Main Camera").transform;
        loadingScreen.transform.position = cameraTransform.transform.position;
        UIFXContainer = GameObject.Find("UIFXContainer");
        
        //初始化battleStageManager:
        var battleStageManager = FindObjectOfType<BattleStageManager>();

        battleStageManager.LoadLevelDetailedInfo(currentCharacterID, currentLevelDetailedInfo);
        var lvl_info = battleStageManager.GetLevelInfo();
        
        //加载场景地图
        var scenePath = lvl_info.scene_prefab_path;
        AssetBundle sceneBundle;
        if (!loadedBundles.ContainsKey(scenePath))
        {
            ar =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, scenePath));
            yield return ar;
            loadedBundles.Add(scenePath,ar.assetBundle);
            assetBundles.Add(ar.assetBundle);
            sceneBundle = ar.assetBundle;
        }
        else
        {
            assetBundles.Add(loadedBundles[scenePath]);
            sceneBundle = loadedBundles[scenePath];
        }

        var scenePrefab = sceneBundle.LoadAsset<GameObject>("Scene");
        //将其子物体的父节点全部设置到场景最高级节点
        var sceneClone = Instantiate(scenePrefab, Vector3.zero, Quaternion.identity);
        var sceneCloneChildren1 = sceneClone.transform.GetChild(0);
        var sceneCloneChildren2 = sceneClone.transform.GetChild(1);
        
        
        sceneCloneChildren1.parent = sceneClone.transform.parent;
        sceneCloneChildren2.parent = sceneClone.transform.parent;
        
        Destroy(sceneClone.gameObject);
        battleStageManager.GetMapBorderInfo();
        var othercamera = GameObject.Find("OtherCamera/TrigMap").GetComponent<Camera>();
        GameObject.Find("Main Camera").GetComponentInChildren<CinemachineConfiner2D>().m_BoundingShape2D
            = GameObject.Find("CameraRange").GetComponent<PolygonCollider2D>();
        

        var UIElements = GameObject.Find("UI");
        // othercamera.targetTexture = null;
        // yield return null;
        othercamera.targetTexture = UIElements.transform.Find("Minimap/MiniMap").GetComponent<RawImage>().texture as RenderTexture;
        


        //实例化UI
        
        var UICharaInfoClone = Instantiate(UICharaInfoAsset, UIElements.transform);
        UICharaInfoClone.name = "CharacterInfo";
        GameObject.Find("UI").transform.Find("Minimap").gameObject.SetActive(true);
        
        
        
        
        
        //加载玩家部分
        var plr = assetBundles[0].LoadAsset<GameObject>("PlayerHandle");
        var playerPositionX = (float)(currentLevelDetailedInfo.player_position[0]);
        var playerPositionY = (float)(currentLevelDetailedInfo.player_position[1]);
        var plrlayer = GameObject.Find("Player");
        var plrclone = 
            Instantiate(plr, new Vector3(playerPositionX, playerPositionY, 0), transform.rotation, plrlayer.transform);
        plrclone.name = "PlayerHandle";
        LoadLocalizedUITest(battleStageManager,questID);
        battleStageManager.InitPlayer(plrclone);
        plrclone.GetComponent<PlayerStatusManager>().GetPlayerConditionBar();
        plrclone.GetComponent<PlayerStatusManager>().remainReviveTimes = currentLevelDetailedInfo.revive_limit;
        plrclone.GetComponent<PlayerInput>().enabled = false;
        //加载本地化相关
        yield return null;

        var SEBundle = GetBundle("soundeffect/soundeffect_common");

        var soundLoadRequest = SEBundle.LoadAllAssets();
        //yield return soundLoadRequest;
        //以soundLoadRequest里面所有资源的名字为键，资源本身为值，存入字典SEClips
        // foreach (var clip in soundLoadRequest.allAssets)
        // {
        //     BattleEffectManager.Instance.SEClips.Add(clip.name, clip as AudioClip);
        // }
        
        
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
        battleStageManager.InitBGM();
        
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("End"));
        Destroy(loadingScreen);
        FinishLoad(true);
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

    protected IEnumerator LoadBattleSceneOfNormalStory(string questID)
    {
        GlobalController.questID = questID;
        FinishLoad(false);
        var loadingScreen = Instantiate(LoadingScreen, Vector3.zero, Quaternion.identity, transform);
        var anim = loadingScreen.GetComponent<Animator>();
        //yield return new WaitForSecondsRealtime(2f);
        
        // var clonedStack = new Stack<int>(new Stack<int>(MenuUIManager.Instance.menuLevelStack));
        // //将栈顶元素一一加入到questEnterRecordList中，直到取出的栈顶元素为1010或者栈空
        // questEnterRecordList.Clear();
        // while (clonedStack.Count > 0)
        // {
        //     var top = clonedStack.Pop();
        //     if (top == 1010)
        //         break;
        //     questEnterRecordList.Add(top);
        // }
        

        //load level
        bool levelAssetLoaded = false;
        print(questID);
        List<AssetBundle> assetBundles = new List<AssetBundle>();
        AssetBundleCreateRequest ar = null;

        //读取关卡信息
        string currentQuestInfo = QuestInfo[$"QUEST_{questID}"].ToString();

        currentQuestInfo = JsonMapper.ToJson(QuestInfo[$"QUEST_{questID}"]);
        print(currentQuestInfo);
        
        var currentLevelDetailedInfo = 
            JsonMapper.ToObject<StoryLevelDetailedInfo>(currentQuestInfo);
        
        
        
        
        
        //加载场景ab包
        var scenePath = currentLevelDetailedInfo.scene_prefab_path;
        AssetBundle sceneBundle;
        if (!loadedBundles.ContainsKey(scenePath))
        {
            ar =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, scenePath));
            yield return ar;
            loadedBundles.Add(scenePath,ar.assetBundle);
            assetBundles.Add(ar.assetBundle);
            sceneBundle = ar.assetBundle;
        }
        else
        {
            assetBundles.Add(loadedBundles[scenePath]);
            sceneBundle = loadedBundles[scenePath];
        }
        
        //加载恒常资源
        
        List<String> requiredBundleList = new List<string>();
        requiredBundleList.Add("ui_general");
        //requiredBundleList.Add("eff/eff_general");
        requiredBundleList.Add("soundeffect/soundeffect_common");
        requiredBundleList.Add("118effbundle");
        requiredBundleList.Add("boss_ability_icon");
        
        //加载关卡资源
        var resourcesPathList = currentLevelDetailedInfo.resources;

        foreach (var path in resourcesPathList)
        {
            requiredBundleList.Add(path);
        }
        
        
        //加载玩家资源
        var playerID = currentLevelDetailedInfo.fixed_chara_id;
        if (playerID > 0)
        {
            var requiredPlayerAssets = SearchPlayerRelatedAssets(playerID);
            requiredBundleList.AddRange(requiredPlayerAssets);
        }
        else
        {
            playerID = currentCharacterID;
            var requiredPlayerAssets = SearchPlayerRelatedAssets(currentCharacterID);
            requiredBundleList.AddRange(requiredPlayerAssets);
            
            
            
            //TODO: 加载龙的ab包
            
            
            var extraCharaAssets = GetExtraCharacterAssets(currentCharacterID);
            if (extraCharaAssets != null)
            {
                requiredBundleList.AddRange(extraCharaAssets);
            }
        }
        
        
        
        
        //提取AB包
        
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
        
        //找到AB包中的BGM_BUNDLE
        var bgmBundle = assetBundles.Find(bundle => bundle.name == currentLevelDetailedInfo.bgm_path);
        var bgm_name = currentLevelDetailedInfo.bgm_path.Split('/')[1];
        //print(bgm_name);
        var bgm = bgmBundle.LoadAsset<AudioClip>(bgm_name);



        //加载新场景
        
        string sceneName;
        switch (GameLanguage)
        {
            case Language.ZHCN:
                sceneName = "BattleScene";
                break;
            case Language.EN:
                sceneName = "BattleScene_EN";
                break;
            default:
                Debug.LogError("GameLanguage is not set");
                yield break;
        }

        currentGameState = GameState.WaitForStart;
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        DontDestroyOnLoad(this.gameObject);
        yield return ao;
        
        BattleStageManager.Instance.LoadStoryLevelDetailedInfo(playerID, currentLevelDetailedInfo);
        
        //摄像机跟随屏幕
        cameraTransform = GameObject.Find("Main Camera").transform;
        loadingScreen.transform.position = cameraTransform.transform.position;
        UIFXContainer = GameObject.Find("UIFXContainer");
        
        
        
        
        
        

        var scenePrefab = sceneBundle.LoadAsset<GameObject>("Scene");
        //将其子物体的父节点全部设置到场景最高级节点
        var sceneClone = Instantiate(scenePrefab, Vector3.zero, Quaternion.identity);
        var sceneCloneChildren1 = sceneClone.transform.GetChild(0);
        var sceneCloneChildren2 = sceneClone.transform.GetChild(1);
        
        
        sceneCloneChildren1.parent = sceneClone.transform.parent;
        sceneCloneChildren2.parent = sceneClone.transform.parent;
        
        Destroy(sceneClone.gameObject);

        var storyTimelineManager = FindObjectOfType<StoryBattleTimelineManager>();
        //把加载玩家的工作交给StoryTimelineManager
        storyTimelineManager.InitStoryQuestInfo(currentLevelDetailedInfo,this);
        print(BattleStageManager.Instance.clearConditionType);

        yield return null;
        
        storyTimelineManager.LoadPlayer(playerID);

        yield return null;



        //初始化场景参数

        
        BattleEffectManager.Instance.SetBGM(bgm);
        BattleStageManager.Instance.GetMapBorderInfo();
        var othercamera = GameObject.Find("OtherCamera/TrigMap").GetComponent<Camera>();
        GameObject.Find("Main Camera").GetComponentInChildren<CinemachineConfiner2D>().m_BoundingShape2D
            = GameObject.Find("CameraRange").GetComponent<PolygonCollider2D>();
        

        var UIElements = GameObject.Find("UI");
        // othercamera.targetTexture = null;
        // yield return null;
        othercamera.targetTexture = UIElements.transform.Find("Minimap/MiniMap").GetComponent<RawImage>().texture as RenderTexture;
        anim.SetBool("loaded",true);
        storyTimelineManager.ActiveStartScreen();
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("End"));
        Destroy(loadingScreen);
        FinishLoad(true);
        

        yield return new WaitForSeconds(2.5f);
        GameObject.Find("StartScreen").GetComponent<UI_StartScreen>().FadeOut();
        
        yield return new WaitForSeconds(1.5f);
        storyTimelineManager.StartQuest();
        

        loadingRoutine = null;
        
        
        









    }

    protected IEnumerator LoadMainMenu()
    {
        currentGameState = GameState.End;
        FinishLoad(false);
        FindObjectOfType<TargetAimer>().enabled = false;
        yield return null;

        var loadingScreen = Instantiate(LoadingScreen, Camera.main.transform.position, Quaternion.identity, transform);
        var anim = loadingScreen.GetComponent<Animator>();
        
        yield return new WaitForSecondsRealtime(2);

        string sceneName = "MainMenu";
        if (GameLanguage == Language.EN)
        {
            sceneName = "MainMenu_EN";
        }


        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        DontDestroyOnLoad(this.gameObject);
        yield return ao;
        cameraTransform = GameObject.Find("Main Camera").transform;
        
        
        //AssetBundle.UnloadAllAssetBundles(true);
        var bundles = AssetBundle.GetAllLoadedAssetBundles();
        print(bundles.Count());
        foreach (var ab in bundles)
        {
            if (ab.name == "iconsmall" || ab.name == "allin1" || ab.name == "eff/eff_general" || ab.name == "animation/anim_common")
            {
                continue;
            }

            loadedBundles.Remove(ab.name);
            Debug.Log("Unloaded:"+ab);
            var async = ab.UnloadAsync(true);
            yield return async;
            
            
        }


        foreach (var bundle in loadedBundles)
        {
            Debug.Log(bundle.Key + "," + bundle.Value);
            
        }

        UpdateQuestSaveData();
        yield return null;

        loadingScreen.transform.position = Vector3.zero;
        
        Time.timeScale = 1;
        cameraTransform = Camera.main.transform;
        UIFXContainer = GameObject.Find("UIFXContainer");
        
        //onGlobalControllerAwake?.Invoke();
        var menuUIManager = FindObjectOfType<MenuUIManager>();
        menuUIManager.menuLevelStack.Push(101);
        yield return null;
        menuUIManager.ToNextUIState(1010,true);
        
        //打印menuLevelStack里面的内容
        foreach (var VARIABLE in menuUIManager.menuLevelStack)
        {
            print(VARIABLE);
        }

        yield return new WaitForSeconds(0.5f);

        questEnterRecordList.Reverse();
        foreach (var VARIABLE in questEnterRecordList)
        {
            yield return new WaitForSeconds(0.5f);
            menuUIManager.ToNextUIState(VARIABLE);
            //yield return new WaitForSeconds(0.5f);
        }

        //yield return new WaitForSeconds(questEnterRecordList.Count * 0.3f);
        
        
        anim.SetBool("loaded",true);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("End"));
        Destroy(loadingScreen);
        this.enabled = true;
        
        
        
        onGlobalControllerAwake?.Invoke();
        MenuUIManager.SetMaxMenuLevel(1);
        UpdateQuestSaveData();

        yield return new WaitForSeconds(0.5f);
        currentGameState = GameState.Outbattle;
        FinishLoad(true);
        //loadingEnd = true;
        loadingRoutine = null;
        
    }

    protected IEnumerator LoadStorySceneOfPrologue()
    {
        GlobalController.questID = "100001";
        FinishLoad(false);
        //loadingEnd = false;
        var loadingScreen = Instantiate(LoadingScreen, Vector3.zero, Quaternion.identity, transform);
        var anim = loadingScreen.GetComponent<Animator>();

        //load level
        //bool levelAssetLoaded = false;
        questEnterRecordList.Clear();

        string sceneName;
        switch (GameLanguage)
        {
            case Language.ZHCN:
                sceneName = "StoryScene";
                break;
            case Language.EN:
                sceneName = "StoryScene_EN";
                break;
            default:
                Debug.LogError("GameLanguage is not set");
                yield break;
        }
        
        
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        DontDestroyOnLoad(this.gameObject);
        yield return ao;

        

        
        
        cameraTransform = GameObject.Find("Main Camera").transform;
        loadingScreen.transform.position = cameraTransform.transform.position;
        UIFXContainer = GameObject.Find("UIFXContainer");
        
        
        var voiceBundleManager = FindObjectOfType<AudioBundlesTest>();
        yield return new WaitUntil( () => voiceBundleManager.loadingEnd);

        FindObjectOfType<StorySceneManager>().started = true;
        
        anim.SetBool("loaded",true);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("End"));
        Destroy(loadingScreen);
        
        yield return new WaitForSeconds(2.5f);
        //BattleStageManager.Instance.GetMapBorderInfo();
        //GameObject.Find("StartScreen").GetComponent<UI_StartScreen>().FadeOut();
        FinishLoad(true);
        loadingRoutine = null;
    }
    
    protected IEnumerator LoadStorySceneOfNormalStory(string questID)
    {
        print("加载开始");
        GlobalController.questID = questID;
        //GlobalController.questID = "100001";
        FinishLoad(false);
        //loadingEnd = false;
        var loadingScreen = Instantiate(LoadingScreen, Vector3.zero, Quaternion.identity, transform);
        var anim = loadingScreen.GetComponent<Animator>();

        
        
        var clonedStack = new Stack<int>(new Stack<int>(MenuUIManager.Instance.menuLevelStack));
        //将栈顶元素一一加入到questEnterRecordList中，直到取出的栈顶元素为1010或者栈空
        questEnterRecordList.Clear();
        while (clonedStack.Count > 0)
        {
            var top = clonedStack.Pop();
            if (top == 1010)
                break;
            questEnterRecordList.Add(top);
        }
        //load level
        //bool levelAssetLoaded = false;

        string sceneName;
        switch (GameLanguage)
        {
            case Language.ZHCN:
                sceneName = "StoryScene";
                break;
            case Language.EN:
                sceneName = "StoryScene_EN";
                break;
            default:
                Debug.LogError("GameLanguage is not set");
                yield break;
        }
        
        //todo: load quest data
        //用LoadFromFileAsync读取位于story/storyTextData/{questID}路径的assetbundle

        var questDataBundlePath = "story/storytextdata/" + questID;
        TextAsset questData;
        AssetBundle questDataBundle;
        if (!loadedBundles.ContainsKey(questDataBundlePath))
        {
            var questDataBundleRequest = 
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath,questDataBundlePath));

            yield return questDataBundleRequest;
            loadedBundles.Add(questDataBundlePath, questDataBundleRequest.assetBundle);
            questDataBundle = questDataBundleRequest.assetBundle;
            questData = 
                questDataBundle.
                    LoadAsset<TextAsset>("Story_"+GameLanguage.ToString());
        }
        else
        {
            questData = loadedBundles[questDataBundlePath].
                LoadAsset<TextAsset>("Story_" + GameLanguage.ToString());
        }

        print("加载QuestData:"+GameLanguage.ToString());






        print("正在加载场景。");
        
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        DontDestroyOnLoad(this.gameObject);
        yield return ao;

        var storySceneManager = FindObjectOfType<StorySceneManager>();
        storySceneManager.SetStoryDataTextAsset(questData);
        
        
        
        
        cameraTransform = GameObject.Find("Main Camera").transform;
        loadingScreen.transform.position = cameraTransform.transform.position;
        UIFXContainer = GameObject.Find("UIFXContainer");
        
        
        var voiceBundleManager = FindObjectOfType<AudioBundlesTest>();
        yield return new WaitUntil( () => voiceBundleManager.loadingEnd);

        storySceneManager.started = true;
        
        anim.SetBool("loaded",true);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("End"));
        Destroy(loadingScreen);
        
        yield return new WaitForSeconds(1.5f);
        //BattleStageManager.Instance.GetMapBorderInfo();
        //GameObject.Find("StartScreen").GetComponent<UI_StartScreen>().FadeOut();
        FinishLoad(true);
        loadingRoutine = null;
    }

    IEnumerator LoadPrologueScene()
    {
        GlobalController.questID = "100001";
        FinishLoad(false);
        //loadingEnd = false;
        var loadingScreen = Instantiate(LoadingScreen, Vector3.zero, Quaternion.identity, transform);
        var anim = loadingScreen.GetComponent<Animator>();

        //load level
        bool levelAssetLoaded = false;
        
        AssetBundleCreateRequest ar = null;
        
        List<String> requiredBundleList = new List<string>();
        
        
        
        requiredBundleList.Add("voice/voice_c001");
        requiredBundleList.Add("voice/voice_c010");
        requiredBundleList.Add("soundeffect/soundeffect_common");
        requiredBundleList.Add("story/ms_in");
        
        
        
        
        
        //Load
        List<AssetBundle> assetBundles = new List<AssetBundle>();
        
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
        
        //var plr = assetBundles[0].LoadAsset<GameObject>("PlayerHandle");
        
        
        currentGameState = GameState.WaitForStart;

        string sceneName;
        
        if(GameLanguage == Language.ZHCN)
        {
            sceneName = "BattleScenePrologue";
        }
        else if(GameLanguage == Language.EN)
        {
            sceneName = "BattleScenePrologue_EN";
        }
        else
        {
            Debug.LogError("GameLanguage Error");
            yield break;
        }



        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        
        
        
        
        
        DontDestroyOnLoad(this.gameObject);
        yield return ao;
        
        anim.SetBool("loaded",true);
        
        cameraTransform = GameObject.Find("Main Camera").transform;
        loadingScreen.transform.position = cameraTransform.transform.position;
        UIFXContainer = GameObject.Find("UIFXContainer");
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("End"));
        Destroy(loadingScreen);
        
        yield return new WaitForSeconds(2.5f);
        BattleStageManager.Instance.GetMapBorderInfo();
        //GameObject.Find("StartScreen").GetComponent<UI_StartScreen>().FadeOut();
        FinishLoad(true);
        //loadingEnd = true;
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
        try
        {
            if (loadedBundles.ContainsKey(name))
            {
                return loadedBundles[name];
            }
        }
        catch
        {
            Debug.LogWarning($"Error when getting bundle {name}");
            return null;
        }


        Debug.LogWarning($"No Bundle called {name} is loaded.");
        return null;
    }

    string[] SearchPlayerRelatedAssets(int id)
    {
        var needLoad = new string[5];
        needLoad[0] = BasicCalculation.ConvertID("player/player_c", id);
        needLoad[1] = BasicCalculation.ConvertID("model/model_c", id);
        needLoad[2] = BasicCalculation.ConvertID("voice/voice_c", id);
        needLoad[3] = BasicCalculation.ConvertID("eff/eff_c", id);
        needLoad[4] = BasicCalculation.ConvertID("ui/ui_c", id);
        return needLoad;
    }

    /// <summary>
    /// 当前只实现了加载BuffLogText的本地化
    /// </summary>
    /// <param name="battleStageManager"></param>
    /// <param name="questID"></param>
    public void LoadLocalizedUITest(BattleStageManager battleStageManager,string questID)
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
            battleStageManager.SetGameCleared(BattleStageManager.Instance.loseControllTime);
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

    public void LoadPlayerSettings()
    {
        try
        {
            var keySettings = SettingsInfo["key_settings"];
            keyAttack = (KeyCode)Enum.Parse(typeof(KeyCode),keySettings["keyAttack"].ToString());
            keySkill1 = (KeyCode)Enum.Parse(typeof(KeyCode),keySettings["keySkill1"].ToString());
            keySkill2 = (KeyCode)Enum.Parse(typeof(KeyCode),keySettings["keySkill2"].ToString());
            keySkill3 = (KeyCode)Enum.Parse(typeof(KeyCode),keySettings["keySkill3"].ToString());
            keySkill4 = (KeyCode)Enum.Parse(typeof(KeyCode),keySettings["keySkill4"].ToString());
            keyJump = (KeyCode)Enum.Parse(typeof(KeyCode),keySettings["keyJump"].ToString());
            keyLeft = (KeyCode)Enum.Parse(typeof(KeyCode),keySettings["keyLeft"].ToString());
            keyRight = (KeyCode)Enum.Parse(typeof(KeyCode),keySettings["keyRight"].ToString());
            keySpecial = (KeyCode)Enum.Parse(typeof(KeyCode),keySettings["keySpecial"].ToString());
            keyRoll = (KeyCode)Enum.Parse(typeof(KeyCode),keySettings["keyRoll"].ToString());
            keyDown = (KeyCode)Enum.Parse(typeof(KeyCode),keySettings["keyDown"].ToString());
            
            // keySkill1 = keySettings["keySkill1"].ToString();
            // keySkill2 = keySettings["keySkill2"].ToString();
            // keySkill3 = keySettings["keySkill3"].ToString();
            // keySkill4 = keySettings["keySkill4"].ToString();
            // keyJump = keySettings["keyJump"].ToString();
            // keyLeft = keySettings["keyLeft"].ToString();
            // keyRight = keySettings["keyRight"].ToString();
            // keySpecial = keySettings["keySpecial"].ToString();
            // keyRoll = keySettings["keyRoll"].ToString();
            // keyDown = keySettings["keyDown"].ToString();
        }
        catch 
        {
            
        }
    }

    public void WritePlayerSettingsToFile()
    {
        //Use LitJson to write keySettings Into PlayerSettings.json

        SettingsInfo["key_settings"]["keyAttack"] = keyAttack.ToString();
        SettingsInfo["key_settings"]["keySkill1"] = keySkill1.ToString();
        SettingsInfo["key_settings"]["keySkill2"] = keySkill2.ToString();
        SettingsInfo["key_settings"]["keySkill3"] = keySkill3.ToString();
        SettingsInfo["key_settings"]["keySkill4"] = keySkill4.ToString();
        SettingsInfo["key_settings"]["keyJump"] = keyJump.ToString();
        SettingsInfo["key_settings"]["keyLeft"] = keyLeft.ToString();
        SettingsInfo["key_settings"]["keyRight"] = keyRight.ToString();
        SettingsInfo["key_settings"]["keySpecial"] = keySpecial.ToString();
        SettingsInfo["key_settings"]["keyRoll"] = keyRoll.ToString();
        SettingsInfo["key_settings"]["keyDown"] = keyDown.ToString();
        SettingsInfo["key_settings"]["keyEscape"] = keyEscape.ToString();
        
        var path = Application.streamingAssetsPath + "/savedata/PlayerSettings.json";
        print(keySpecial);

        var newSettings = new JsonData();
        newSettings["key_settings"] = new JsonData();
        newSettings["key_settings"]["keyAttack"] = keyAttack.ToString();
        newSettings["key_settings"]["keySkill1"] = keySkill1.ToString();
        newSettings["key_settings"]["keySkill2"] = keySkill2.ToString();
        newSettings["key_settings"]["keySkill3"] = keySkill3.ToString();
        newSettings["key_settings"]["keySkill4"] = keySkill4.ToString();
        newSettings["key_settings"]["keyJump"] = keyJump.ToString();
        newSettings["key_settings"]["keyLeft"] = keyLeft.ToString();
        newSettings["key_settings"]["keyRight"] = keyRight.ToString();
        newSettings["key_settings"]["keySpecial"] = keySpecial.ToString();
        newSettings["key_settings"]["keyRoll"] = keyRoll.ToString();
        newSettings["key_settings"]["keyDown"] = keyDown.ToString();
        newSettings["key_settings"]["keyEscape"] = keyEscape.ToString();
        print(newSettings);
        
        var jsonStr = JsonMapper.ToJson(newSettings);
        print(jsonStr);
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.Write(jsonStr);
        }


    }

    public void StartGame()
    {
        currentGameState = GameState.Inbattle;
    }

    public void EndGame()
    {
        currentGameState = GameState.End;
    }

    protected void ReadStatusInformation()
    {
        var path = Application.streamingAssetsPath + "/LevelInformation/Stat_info.json";
        
        string jsonStr;
        using (StreamReader reader = new StreamReader(path))
        {
            jsonStr = reader.ReadToEnd();
        }

        StatusInfoList = JsonUtility.FromJson<StatusInformationList>(jsonStr).statusInformationList;
    }

    public string GetNameOfID(int id)
    {
        //找到StatusInfoList中id为id的StatusInformation，返回其name
        var statusInfo = StatusInfoList.Find(x => x.id == id);
        
        
        //如果找不到，返回空字符串
        if (statusInfo == null)
        {
            return "";
        }
        else
        {
            switch (GameLanguage)
            {
                case Language.EN:
                {
                    return statusInfo.name_en;
                }
                case Language.ZHCN:
                {
                    return statusInfo.name_zh;
                }
                default:
                {
                    return String.Empty;
                }
            }
            
        }
    }

    public List<QuestSave> GetQuestInfo()
    {
        print(questSaveDataString);
        var questSaveDataList = JsonMapper.ToObject<QuestDataList>(questSaveDataString);
        return questSaveDataList.quest_info;
    }

    private void LoadGameOptionsFromFile()
    {
        //Load GameOptions from file, Use JsonUtility.FromJson<GameOptions>
        var path = Application.streamingAssetsPath + "/savedata/GameOptions.json";
        StreamReader sr = new StreamReader(path);
        var str = sr.ReadToEnd();
        sr.Close();

        try
        {
            gameOptions = JsonUtility.FromJson<GameOptions>(str);
        }
        catch
        {
            gameOptions = new();
        }

        currentCharacterID = gameOptions.last_adventurer_id;

    }

    private List<String> GetExtraCharacterAssets(int charaID)
    {
        print(CharaAssetData.Infos[0].resources_name);
        return CharaAssetData.GetResources(charaID);
    }



    public void ChangeFontSizeOfDamageNum(int size)
    {
        gameOptions.damage_font_size = size;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">0:music; 1:voice 2:SE</param>
    /// <param name="mute"></param>
    public void ChangeSoundMute(int type, bool mute)
    {
        if (mute)
        {
            gameOptions.soundSettings[type] = 0;
        }
        else
        {
            gameOptions.soundSettings[type] = 1;
        }

        if (type == 0)
        {
            ResetMusicSources();
        }
    }

    private void ResetAllAudioSources(Scene scene,LoadSceneMode mode)
    {
        var musicSources = GameObject.FindGameObjectsWithTag("Music");

        foreach (var audioObject in musicSources)
        {
            audioObject.GetComponent<AudioSource>().mute = (gameOptions.soundSettings[0] == 0);
        }

        var voiceSources = GameObject.FindGameObjectsWithTag("Voice");

        foreach (var audioObject in voiceSources)
        {
            audioObject.GetComponent<AudioSource>().mute = (gameOptions.soundSettings[1] == 0);
        }
        
        var soundEffectSources = GameObject.FindGameObjectsWithTag("SoundEffect");

        foreach (var audioObject in soundEffectSources)
        {
            audioObject.GetComponent<AudioSource>().mute = (gameOptions.soundSettings[2] == 0);
        }

    }
    
    private void ResetMusicSources()
    {
        var musicSources = GameObject.FindGameObjectsWithTag("Music");

        foreach (var audioObject in musicSources)
        {
            audioObject.GetComponent<AudioSource>().mute = (gameOptions.soundSettings[0] == 0);
        }

    }

    public void WriteGameOptionToFile()
    {
        //Write GameOptions to file of path, Use JsonUtility.ToJson
        
        var path = Application.streamingAssetsPath + "/savedata/GameOptions.json";
        
        
        //var jsonStr = JsonUtility.ToJson(gameOptions);
        var jsonStr = JsonConvert.SerializeObject(gameOptions);
        

        try
        {
            File.WriteAllText(path, jsonStr);
        }
        catch
        {
            //TODO:抛出异常
        }
        
    }

    private void FinishLoad(bool flag)
    {
        loadingEnd = flag;
        if (flag == true)
        {
            ResetAllAudioSources(SceneManager.GetActiveScene(),LoadSceneMode.Single);
            //OnLoadFinish?.Invoke();
        }
    }

    private void OnApplicationQuit()
    {
        gameOptions.last_adventurer_id = currentCharacterID;
        WriteGameOptionToFile();
    }

}
// "key_settings": {
//     "keyAttack": "j",
//     "keyJump": "k",
//     "keyLeft": "a",
//     "keyRight": "d",
//     "keySpecial": "space",
//     "keyRoll": "l",
//     "keyDown": "s",
//     "keySkill1": "u",
//     "keySkill2": "i",
//     "keySkill3": "o",
//     "keySkill4": "h"
// }

[Serializable]
public class GameOptions
{
    public int last_adventurer_id = 1;
    public int fullscreen = 1;
    public int damage_font_size = 2;

    /// <summary>
    /// Music, Voice, SoundEffect
    /// </summary>
    public int[] soundSettings = new[] { 1, 1, 1 };

    public List<AchievementInfo> achievementList = new();
}