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
using UnityEngine.UI;

public class GlobalController : MonoBehaviour
{
    public static GlobalController Instance { get; protected set; }

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
    protected JsonData SettingsInfo;
    public delegate void OnGlobalControllerAwake();
    public static OnGlobalControllerAwake onGlobalControllerAwake;

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
        QuestInfo = BasicCalculation.ReadJsonData("LevelInformation/QuestInfo.json");
        SettingsInfo = BasicCalculation.ReadJsonData("savedata/PlayerSettings.json");
        
        var other = FindObjectsOfType<GlobalController>();
        if (other.Length > 1)
        {
            this.enabled = false;
            Destroy(gameObject);
        }
        Instance = this;
        this.loadedBundles = new Dictionary<string, AssetBundle>();
        LoadPlayerSettings();
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
        if (debug)
        {
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

        clickEffCD--;
        //print(loadingEnd);
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

    protected IEnumerator LoadBattleScene(string questID){
        //异步加载场景

        GlobalController.questID = questID;
        loadingEnd = false;
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
        requiredBundleList.Add("eff/eff_general");
        requiredBundleList.Add("animation/anim_common");
        requiredBundleList.Add("allin1");
        requiredBundleList.Add("soundeffect/soundeffect_common");
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
        battleStageManager.SetBGM();
        
        
        
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

    protected IEnumerator LoadMainMenu()
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

        loadingScreen.transform.position = Vector3.zero;
        
        Time.timeScale = 1;
        cameraTransform = Camera.main.transform;
        UIFXContainer = GameObject.Find("UIFXContainer");
        
        onGlobalControllerAwake?.Invoke();
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
        //print(loadedBundles.ContainsKey("Iconsmall"));
        
        
        onGlobalControllerAwake?.Invoke();
        MenuUIManager.SetMaxMenuLevel(1);
        UpdateQuestSaveData();

        yield return new WaitForSeconds(0.5f);
        currentGameState = GameState.Outbattle;
        loadingEnd = true;
        loadingRoutine = null;
        
    }

    protected IEnumerator LoadStorySceneOfPrologue()
    {
        GlobalController.questID = "100001";
        loadingEnd = false;
        var loadingScreen = Instantiate(LoadingScreen, Vector3.zero, Quaternion.identity, transform);
        var anim = loadingScreen.GetComponent<Animator>();

        //load level
        //bool levelAssetLoaded = false;
        
        //currentGameState = GameState.WaitForStart;
        AsyncOperation ao = SceneManager.LoadSceneAsync("StoryScene");
        DontDestroyOnLoad(this.gameObject);
        yield return ao;

        

        
        
        cameraTransform = GameObject.Find("Main Camera").transform;
        loadingScreen.transform.position = cameraTransform.transform.position;
        UIFXContainer = GameObject.Find("UIFXContainer");
        
        
        var voiceBundleManager = FindObjectOfType<AudioBundlesTest>();
        yield return new WaitUntil(()=>voiceBundleManager.loadingEnd);

        FindObjectOfType<StorySceneManager>().started = true;
        
        anim.SetBool("loaded",true);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("End"));
        Destroy(loadingScreen);
        
        yield return new WaitForSeconds(2.5f);
        //BattleStageManager.Instance.GetMapBorderInfo();
        //GameObject.Find("StartScreen").GetComponent<UI_StartScreen>().FadeOut();
        loadingEnd = true;
        loadingRoutine = null;
    }

    IEnumerator LoadPrologueScene()
    {
        GlobalController.questID = "100001";
        loadingEnd = false;
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
        AsyncOperation ao = SceneManager.LoadSceneAsync("BattleScenePrologue");
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
        loadingEnd = true;
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

    public List<QuestSave> GetQuestInfo()
    {
        var questSaveDataList = JsonMapper.ToObject<QuestDataList>(questSaveDataString);
        return questSaveDataList.quest_info;
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