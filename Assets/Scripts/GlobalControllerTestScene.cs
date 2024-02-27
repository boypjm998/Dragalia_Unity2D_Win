using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalControllerTestScene : GlobalController
{
    // Start is called before the first frame update
    public bool SetBoss;
    public GameObject boss;
    public string questIDFake;
    public int tempCurrentID;

    public GameState FakeState;
    void Start()
    {
        currentGameState = FakeState;
        currentCharacterID = tempCurrentID;
        AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/voice_c001");
        loadedBundles.Add("voice/voice_c001",ab);
        //GameObject.Find("PlayerHandle").GetComponentInChildren<VoiceController_C001>()?.DebugLoadVoice();
        
        BattleStageManager.Instance.GetMapBorderInfo();
        FakeStart();
        LoadGamepadOption();
        gamepadEnable = true;

    }

    public static Tuple<float, float> TestDelegateFunc1(StatusManager src, AttackBase atk, StatusManager tar)
    {
        print("TestDelegateFunc1");
        return new Tuple<float, float>(0.2f,0.2f);
    }
    
    public static Tuple<float, float> TestDelegateFunc2(StatusManager src, AttackBase atk, StatusManager tar)
    {
        print("TestDelegateFunc1");
        return new Tuple<float, float>(0.3f,0);
    }
    
    public static Tuple<float, float> TestDelegateFunc3(StatusManager src, AttackBase atk, StatusManager tar)
    {
        print("TestDelegateFunc1");
        return new Tuple<float, float>(0.4f,0.1f);
    }

    void FakeStart()
    {
        var ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/boss_ability_icon");
        loadedBundles.Add("boss_ability_icon",ab);
        FindObjectOfType<UI_BossStatus>().SetBoss(boss);
        JsonData currentQuestInfo = QuestInfo[$"QUEST_{questIDFake}"];
        var currentLevelDetailedInfo = JsonMapper.ToObject<LevelDetailedInfo>(currentQuestInfo.ToJson());
        BattleStageManager.Instance.LoadLevelDetailedInfoDebugScene(3,currentLevelDetailedInfo);
    }

    void Update()
    {
        currentGameState = FakeState;


    }

    // Update is called once per frame
    void Awake()
    {
        base.Awake();
    }
}
