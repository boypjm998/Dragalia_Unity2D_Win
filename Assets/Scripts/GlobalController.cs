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
    public static int currentCharacterID = 4;
    
    
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
    public List<int> questEnterRecordList = new();
    public static string questSaveDataString;

    #endregion
    
    

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
            BattleEffectManager.Instance.PlayOtherSE("SE_ACTION_GUN_001");
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

    IEnumerator LoadBattleScene(string questID){
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
        
        battleStageManager.LoadLevelDetailedInfo(currentCharacterID,currentLevelDetailedInfo);

        //实例化UI
        var UIElements = GameObject.Find("UI");
        var UICharaInfoClone = Instantiate(UICharaInfoAsset, UIElements.transform);
        UICharaInfoClone.name = "CharacterInfo";
        
        
        
        
        
        
        
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
        plrclone.GetComponent<PlayerStatusManager>().remainReviveTimes = 3;
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
        loadingEnd = true;
        currentGameState = GameState.Outbattle;
        onGlobalControllerAwake?.Invoke();
        MenuUIManager.SetMaxMenuLevel(1);
        UpdateQuestSaveData();


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
        
        // requiredBundleList.Add("player/player_c001");
        //
        // requiredBundleList.Add("npc/npc_prologue_01");
        // requiredBundleList.Add("npc/npc_prologue_02");
        //
        // requiredBundleList.Add("model/model_c001");
        // requiredBundleList.Add("model/model_c010");
        // requiredBundleList.Add("model/model_c019");
        //
        // requiredBundleList.Add("ui_general");
        //
        // requiredBundleList.Add("eff/eff_general");
        // requiredBundleList.Add("eff/eff_c001");
        // requiredBundleList.Add("eff/eff_c010");
        // requiredBundleList.Add("eff/eff_c019");
        //
        // requiredBundleList.Add("animation/anim_common");
        // requiredBundleList.Add("allin1");
        // requiredBundleList.Add("118effbundle");
        // requiredBundleList.Add("boss_ability_icon");
        
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
        if (loadedBundles.ContainsKey(name))
        {
            return loadedBundles[name];
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
            keyAttack = keySettings["keyAttack"].ToString();
            keySkill1 = keySettings["keySkill1"].ToString();
            keySkill2 = keySettings["keySkill2"].ToString();
            keySkill3 = keySettings["keySkill3"].ToString();
            keySkill4 = keySettings["keySkill4"].ToString();
            keyJump = keySettings["keyJump"].ToString();
            keyLeft = keySettings["keyLeft"].ToString();
            keyRight = keySettings["keyRight"].ToString();
            keySpecial = keySettings["keySpecial"].ToString();
            keyRoll = keySettings["keyRoll"].ToString();
            keyDown = keySettings["keyDown"].ToString();
        }
        catch 
        {
            
        }
    }

    public void WritePlayerSettingsToFile()
    {
        //Use LitJson to write keySettings Into PlayerSettings.json

        SettingsInfo["key_settings"]["keyAttack"] = keyAttack;
        SettingsInfo["key_settings"]["keySkill1"] = keySkill1;
        SettingsInfo["key_settings"]["keySkill2"] = keySkill2;
        SettingsInfo["key_settings"]["keySkill3"] = keySkill3;
        SettingsInfo["key_settings"]["keySkill4"] = keySkill4;
        SettingsInfo["key_settings"]["keyJump"] = keyJump;
        SettingsInfo["key_settings"]["keyLeft"] = keyLeft;
        SettingsInfo["key_settings"]["keyRight"] = keyRight;
        SettingsInfo["key_settings"]["keySpecial"] = keySpecial;
        SettingsInfo["key_settings"]["keyRoll"] = keyRoll;
        SettingsInfo["key_settings"]["keyDown"] = keyDown;
        var path = Application.streamingAssetsPath + "/savedata/PlayerSettings.json";
        print(keySpecial);

        var newSettings = new JsonData();
        newSettings["key_settings"] = new JsonData();
        newSettings["key_settings"]["keyAttack"] = keyAttack;
        newSettings["key_settings"]["keySkill1"] = keySkill1;
        newSettings["key_settings"]["keySkill2"] = keySkill2;
        newSettings["key_settings"]["keySkill3"] = keySkill3;
        newSettings["key_settings"]["keySkill4"] = keySkill4;
        newSettings["key_settings"]["keyJump"] = keyJump;
        newSettings["key_settings"]["keyLeft"] = keyLeft;
        newSettings["key_settings"]["keyRight"] = keyRight;
        newSettings["key_settings"]["keySpecial"] = keySpecial;
        newSettings["key_settings"]["keyRoll"] = keyRoll;
        newSettings["key_settings"]["keyDown"] = keyDown;
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