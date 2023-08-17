using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class GlobalControllerTestScene : GlobalController
{
    // Start is called before the first frame update
    public bool SetBoss;
    public GameObject boss;
    public string questIDFake;
    public int tempCurrentID;
    void Start()
    {
        //ReadStatusInformation();
        //GameLanguage = Language.ZHCN;
        currentGameState = GameState.Inbattle;
        currentCharacterID = tempCurrentID;
        AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/voice_c001");
        loadedBundles.Add("voice/voice_c001",ab);
        GameObject.Find("PlayerHandle").GetComponentInChildren<VoiceController_C001>()?.DebugLoadVoice();
        
        BattleStageManager.Instance.GetMapBorderInfo();
        FakeStart();
        
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
        // if (SetBoss)
        // {
        //     SetBoss = false;
        //     FindObjectOfType<UI_BossStatus>().SetBoss(boss);
        // }

        
    }

    // Update is called once per frame
    void Awake()
    {
        base.Awake();
    }
}
