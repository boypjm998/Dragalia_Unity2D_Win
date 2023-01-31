using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cinemachine;
using LitJson;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalController : MonoBehaviour
{
    
    private Transform cameraTransform;
    private GameObject UIFXContainer;
    public int clickEffCD = 0;

    public enum Language
    {
        ZHCN,
        JP,
        EN
    }

    public Language GameLanguage;

    public enum GameState
    {
        Outbattle,
        WaitForStart,
        Inbattle,
        End //有一方判定死亡
    }

    public static GameState currentGameState { private set; get; }
    public Dictionary<string, AssetBundle> loadedBundles;
    private JsonData QuestInfo;

    private Coroutine loadingRoutine;
    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private GameObject clickEff;
    public static int currentCharacterID = 1;
    public static string questID = "000000";
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
        this.loadedBundles = new Dictionary<string, AssetBundle>();
        UIFXContainer = GameObject.Find("UIFXContainer");
    }

    void Start()
    {
        if (GetBundle("iconsmall") == null)
        {
            var ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/iconsmall");
            loadedBundles.Add("iconsmall",ab);
        }
        cameraTransform = GameObject.Find("Main Camera").transform;
        currentGameState = GameState.Outbattle;
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
    public void TestEnterLevel()
    {
        if(loadingRoutine!=null)
            return;
        loadingRoutine = StartCoroutine(LoadBattleScene("010013"));
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
        








        var needLoad = SearchPlayerRelatedAssets(1);
        
        
        AssetBundle assetBundle,assetBundle2;
        if (!loadedBundles.ContainsKey(needLoad[0]))
        {
            ar =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, needLoad[0]));
            yield return ar;
            loadedBundles.Add(needLoad[0],ar.assetBundle);
            assetBundle = ar.assetBundle;
        }
        else
        {
            assetBundle = loadedBundles[needLoad[0]];
        }
        //load UI_dependencies
        if (!loadedBundles.ContainsKey("ui_general"))
        {
            ar =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "ui_general"));
            yield return ar;
            loadedBundles.Add("ui_general",ar.assetBundle);
            //assetBundle2 = ar.assetBundle;
        }
        
        if (!loadedBundles.ContainsKey("boss_ability_icon"))
        {
            ar =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "boss_ability_icon"));
            yield return ar;
            loadedBundles.Add("boss_ability_icon",ar.assetBundle);
            //assetBundle2 = ar.assetBundle;
        }
        
        
        
        
        
        
        //Load Voices
        if (!loadedBundles.ContainsKey("voice_c005"))
        {
            ar =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "voice_c005"));
            yield return ar;
            loadedBundles.Add("voice_c005",ar.assetBundle);
            //assetBundle2 = ar.assetBundle;
        }
        
        
        
        
        
        
        if (!loadedBundles.ContainsKey(needLoad[1]))
        {
            ar =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, needLoad[1]));
            yield return ar;
            loadedBundles.Add(needLoad[1],ar.assetBundle);
            //assetBundle2 = ar.assetBundle;
        }

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
        
        battleStageManager.GetLevelInfo(currentCharacterID,
            currentQuestInfo["NAME"].ToString(),
            (int)currentQuestInfo["TIME_LIMIT"],
            (int)currentQuestInfo["CROWN_TIME_LIMIT"],
            (int)currentQuestInfo["TOTAL_BOSS_NUM"],
            (int)currentQuestInfo["REVIVE_LIMIT"],
            (int)currentQuestInfo["CROWN_REVIVE_LIMIT"]);
        
        
        
        
        
        
        
        
        //加载玩家部分
        var plr = assetBundle.LoadAsset<GameObject>("PlayerHandle");
        var plrlayer = GameObject.Find("Player");
        var plrclone = 
            Instantiate(plr, new Vector3(-4.5f, -2f, 0), transform.rotation, plrlayer.transform);
        plrclone.name = "PlayerHandle";
        LoadLocalizedUITest(battleStageManager,"010013");
        battleStageManager.InitPlayer(plrclone);
        plrclone.GetComponent<PlayerStatusManager>().GetPlayerConditionBar();
        plrclone.GetComponent<PlayerStatusManager>().remainReviveTimes = 3;
        plrclone.GetComponent<PlayerInput>().enabled = false;
        //加载本地化相关
        

        
        yield return null;
        anim.SetBool("loaded",true);
        var Chara_UI = GameObject.Find("CharacterInfo");
        Chara_UI.SetActive(false);
        
        
        
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
            if (ab.name == "iconsmall")
            {
                continue;
            }

            loadedBundles.Remove(ab.name);
            var async = ab.UnloadAsync(true);
            yield return async;
            
        }
        
        

        yield return null;
        
        //Resources.UnloadUnusedAssets();
        
        loadingScreen.transform.position = Vector3.zero;
        //loadedBundles.Clear();

        //print(FindObjectOfType<UI_AdventurerSelectionMenu>().
            //iconBundle.LoadAssetWithSubAssets<Sprite>("Iconsmall")[0].name);
        
        
        Time.timeScale = 1;
        cameraTransform = Camera.main.transform;
        UIFXContainer = GameObject.Find("UIFXContainer");
        
        anim.SetBool("loaded",true);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("End"));
        Destroy(loadingScreen);
        this.enabled = true;
        //print(loadedBundles.ContainsKey("Iconsmall"));
        loadingEnd = true;
        currentGameState = GameState.Outbattle;

        
        
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
        var needLoad = new string[3];
        needLoad[0] = BasicCalculation.ConvertID("c", id);
        needLoad[1] = BasicCalculation.ConvertID("voice_c", id);
        needLoad[2] = BasicCalculation.ConvertID("ui_c", id);
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

    
}
