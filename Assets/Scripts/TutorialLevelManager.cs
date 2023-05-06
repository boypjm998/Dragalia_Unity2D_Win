using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CharacterSpecificProjectiles;
using Cinemachine;
using DG.Tweening;
using GameMechanics;
using LitJson;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class TutorialLevelManager : MonoBehaviour
{
    public static TutorialLevelManager Instance{ get; private set; }
    
    private BattleSceneUIManager battleSceneUIManager;
    private UI_Tutorial_PauseMenu pauseMenu;

    private Coroutine currentRoutine;
    public int currentCutScene = 0;

    private GameObject UIElements;

    private PlayerInput _playerInput;
    public string keyDown = "";
    public string keyLeft = "";
    public string keyRight = "";
    public string keyJump = "";
    public string keyAttack = "";
    public string keyRoll = "";
    public string keySkill1 = "";
    public string keySkill2 = "";
    public string keySkill3 = "";
    public string keySkill4 = "";
    public string keySpecial = "";
    private bool moveNext = false;
    
    [Header("Debug")]
    public bool button = false;

    [SerializeField]private float buttonCD = 999.9f;
    

    [Header("Actors")] 
    public GameObject playerPool;
    public GameObject Zethia;
    public GameObject Notte;
    public StatusManager ZenaStatusManager;
    protected PlayableDirector playableDirector;
    public PlayableAsset[] playableAssets;
    protected AudioSource voiceZethia;
    protected AudioSource voiceNotte;
    protected AudioSource sharedVoice;
    public Transform lastCameraPosition;
    public GameObject BossTerminal;

    [Header("Voice")] public List<AudioClip> storyVoices = new();
    private JsonData voiceData;
    public GameObject resultPage;

    [Header("SpecialVFX")] [SerializeField]
    private GameObject vfx01;
    [SerializeField] private AudioClip newBGM;
    public GameObject enemyDeathVFX;
    public GameObject prayerFX;
    public GameObject laserFX;
    public GameObject holyCrownFX;
    
    
    private void Awake()
    {
        if(Instance==null)
            Instance = this;
        else
        {
            Destroy(this);
        }
        
        
    }

    private void Start()
    {
        playerPool = GameObject.Find("Player");
        Zethia = playerPool.transform.GetChild(1).gameObject;
        Notte = playerPool.transform.GetChild(2).gameObject;
        playableDirector = GetComponent<PlayableDirector>();
        voiceZethia = Zethia.GetComponentInChildren<AudioSource>();
        voiceNotte = Notte.GetComponentInChildren<AudioSource>();
        ZenaStatusManager = GetComponentInChildren<StatusManager>();
        
        
        battleSceneUIManager = BattleSceneUIManager.Instance;
        pauseMenu = battleSceneUIManager.PauseMenu.GetComponent<UI_Tutorial_PauseMenu>();
        FindObjectOfType<PlayerInput>().enabled = false;
        keyLeft = GlobalController.keyLeft;
        keyRight = GlobalController.keyRight;
        keyJump = GlobalController.keyJump;
        keyAttack = GlobalController.keyAttack;
        keyRoll = GlobalController.keyRoll;
        keySkill1 = GlobalController.keySkill1;
        keySkill2 = GlobalController.keySkill2;
        keySkill3 = GlobalController.keySkill3;
        keySkill4 = GlobalController.keySkill4;
        keySpecial = GlobalController.keySpecial;
        keyDown = GlobalController.keyDown;
        LoadDialogAsset();
        UIElements = GameObject.Find("UI");
        sharedVoice = BattleStageManager.Instance.transform.Find("SharedVoice").GetComponent<AudioSource>();
        

    }

    private void Update()
    {
        if (button && buttonCD >= 999)
        {
            button = false;
            buttonCD = 0;
            UseSupportSkill(14);
        }
        
    }

    public void HideCharacterUI()
    {
        UIElements.transform.Find("CharacterInfo").GetComponent<CanvasGroup>().alpha = 0;
    }

    public void ResetKeySettingsToDefault(int defaultSettingGroupID)
    {
        keyDown = "s";
        keyLeft = "a";
        keyRight = "d";
        keySpecial = "space";
        if (defaultSettingGroupID == 1)
        {
            keyJump = "w";
            keyAttack = "j";
            keyRoll = "k";
            keySkill1 = "h";
            keySkill2 = "u";
            keySkill3 = "i";
            keySkill4 = "l";
        }
        else
        {
            keyJump = "k";
            keyAttack = "j";
            keyRoll = "l";
            keySkill1 = "u";
            keySkill2 = "i";
            keySkill3 = "o";
            keySkill4 = "h";
        }
        var playerInput = FindObjectOfType<PlayerInput>();
        
        var settings = new[] {keyAttack, keySkill1, keySkill2, keySkill3, keySkill4, keyLeft, keyRight, keySpecial, keyDown, keyRoll, keyJump};
        
        playerInput.SetKeySetting(settings);
        playerInput.GetComponent<PlayerStatusManager>().remainReviveTimes = 3;
        
        GlobalController.keyAttack = keyAttack;
        GlobalController.keySkill1 = keySkill1;
        GlobalController.keySkill2 = keySkill2;
        GlobalController.keySkill3 = keySkill3;
        GlobalController.keySkill4 = keySkill4;
        GlobalController.keyLeft = keyLeft;
        GlobalController.keyRight = keyRight;
        GlobalController.keySpecial = keySpecial;
        GlobalController.keyDown = keyDown;
        GlobalController.keyRoll = keyRoll;
        GlobalController.keyJump = keyJump;
        
    }

    public void MoveNext()
    {
        moveNext = true;
    }

    public void ReadyToStartGame()
    {
        currentRoutine = StartCoroutine(StartGameRoutine());
    }

    public void StartCutScene(int sceneID)
    {
        switch (sceneID)
        {
            case 1:
            {
                if (currentRoutine != null)
                {
                    StopCoroutine(currentRoutine);
                }

                currentRoutine = StartCoroutine(CutScene_01());
                currentCutScene = 1;
                break;
            }
            case 2:
            {
                if (currentRoutine != null)
                {
                    //Debug.LogAssertion("Story Routine is busy");
                    break;
                }
                currentRoutine = StartCoroutine(CutScene_02());
                currentCutScene = 2;
                break;
            }
            case 3:
            {
                if (currentRoutine != null)
                {
                    //Debug.LogAssertion("Story Routine is busy");
                    break;
                }

                currentCutScene = 3;
                currentRoutine = StartCoroutine(CutScene_03());
                break;
            }
            case 4:
            {
                if (currentRoutine != null)
                {
                    Debug.LogWarning("Story Routine is busy");
                    break;
                }

                currentCutScene = 4;
                currentRoutine = StartCoroutine(CutScene_04());
                break;
            }
            case 5:
            {
                if (currentRoutine != null)
                {
                    StopCoroutine(currentRoutine);
                }

                currentCutScene = 5;
                //currentRoutine = StartCoroutine(CutScene_04());
                break;
            }

        }
    }


    public void SetNewPlayable(int playableID,bool play = true)
    {
        playableDirector.playableAsset = playableAssets[playableID];
        if (play)
        {
            playableDirector.Play();
        }
    }

    private void LoadDialogAsset()
    {
        voiceData = BasicCalculation.ReadJsonData("LevelInformation/QuestDialogInfoStory.json");
    }

    public void PlayStoryVoiceWithDialog(int id, int speakerID, AudioSource source)
    {
        if(source == null)
            source = sharedVoice;
        UI_DialogDisplayerStory.Instance.
            EnqueueDialog(speakerID,$"DIALOG_PROLOGUE_{id:D2}", source, storyVoices[id-1]);
    }

    public void CastSkillInfoToBanner(int quest_id,int action_id)
    {
        UI_BattleInfoCasterStory.Instance.PrintSkillName($"STY{quest_id}_Action{action_id:D2}",false);
    }

    IEnumerator StartGameRoutine()
    {
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2.5f);
        var startScr = FindObjectOfType<UI_StartScreen>();
        startScr.FadeOut();
        
        yield return new WaitForSeconds(1.5f);
        var player = FindObjectOfType<PlayerInput>();
        _playerInput = player;
        
        //GlobalController.Instance.StartGame();
        player.keyAttack = "";
        player.keySkill1 = "";
        player.keySkill2 = "";
        player.keySkill3 = "";
        player.keySkill4 = "";
        player.keyJump = "";
        player.keyRoll = "";
        player.keyDown = "";
        player.keyUp = "";
        //player.enabled = true;

        yield return new WaitForSeconds(1f);
        PlayStoryVoiceWithDialog(1,1019,voiceZethia);

        yield return new WaitForSeconds(2.5f);
        CastSkillInfoToBanner(0,1);
        //当playableZethia播放完毕后,恢复原来的动画状态
        playableDirector.Play();
        

        yield return new WaitForSeconds(1.5f);
        
        
        yield return new WaitUntil(()=>moveNext);
        moveNext = false;
        CastSkillInfoToBanner(0,2);
        yield return new WaitForSeconds(1f);
        GameObject.Find("AttackFXPlayer").transform.Find("GroundEffect").gameObject.SetActive(true);
        Zethia.GetComponentInChildren<Animator>().Play("action08");
        GiveBuffToAllPlayers(new TimerBuff((int)BasicCalculation.BattleCondition.HotRecovery,
            30,-1,100),false,9999);
        var dmgCutBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageCutConst,2000,-1,BasicCalculation.MAXCONDITIONSTACKNUMBER);
        GiveBuffToPlayer(Zethia, dmgCutBuff, true);
        
        //test buff limit 
        






        yield return new WaitForSeconds(1f);
        
        GlobalController.Instance.StartGame();

        player.enabled = true;
        UIElements.transform.Find("CharacterInfo").GetComponent<CanvasGroup>().alpha = 1;
        yield return null;
        OpenTutorialHintFirstPage();


        
        yield return null;
        
        

        currentRoutine = null;
        
    }

    IEnumerator CutScene_01()
    {
        var odbuff = new TimerBuff((int)BasicCalculation.BattleCondition.OverdriveAccerlerator,200,-1,100,-1);
        //ZenaStatusManager.ObtainTimerBuff(odbuff,false);
        //_playerInput.DisableAndIdle();
        Notte.GetComponentInChildren<NpcController>().SetSkillTimer(9999f);
        _playerInput.InvokeAttackSignal();
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        _playerInput.keyAttack = keyAttack;
        //_playerInput.EnableAndIdle();
        
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        PlayStoryVoiceWithDialog(2,1010,voiceNotte);
        

        yield return new WaitUntil(() => _playerInput.buttonAttack.OnPressed);
        print("attack pressed");
        BossTerminal.GetComponent<DragaliaEnemyBehavior>().enabled = true;

        yield return new WaitForSeconds(3f);
        
        PlayStoryVoiceWithDialog(3,1010,voiceNotte);

        yield return new WaitForSeconds(0.5f);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        _playerInput.keyRoll = keyRoll;
        
        currentRoutine = null;
    }

    IEnumerator CutScene_02()
    {
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        UIElements.transform.Find("CharacterInfo/Skill01").gameObject.SetActive(true);

        yield return null;
        Notte.GetComponent<NpcController>().SetSkillTimer(1f); 
        _playerInput.keySkill1 = keySkill1;
        
        //yield return new WaitUntil(()=>_playerInput.buttonSkill1.OnPressed);
        currentRoutine = null;

    }
    
    IEnumerator CutScene_03()
    {
        var cm = StageCameraController.MainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        //var cm = FindObjectOfType<CinemachineVirtualCamera>();
        cm.Follow = BossTerminal.transform;
        _playerInput.DisableAndIdle();
        //Notte.GetComponent<NpcController>().enabled = false;
        BossTerminal.transform.Find("HitSensor").gameObject.SetActive(false);
        BossTerminal.transform.Find("Model/effect").gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        cm.Follow = null;
        playableDirector.playableAsset = playableAssets[0];
        var anim_track = playableDirector.playableAsset.outputs.First(c => c.streamName == "Animation Track");
        playableDirector.SetGenericBinding(anim_track.sourceObject,BossTerminal.GetComponentInChildren<Animator>());
        var active_track = playableDirector.playableAsset.outputs.First(c => c.streamName == "Activation Track");
        playableDirector.SetGenericBinding(active_track.sourceObject,BossTerminal.transform.Find("Model/model").gameObject);
        var cinemachine_track = playableDirector.playableAsset.outputs.First(c => c.streamName == "Cinemachine Track");
        playableDirector.SetGenericBinding(cinemachine_track.sourceObject,GameObject.Find("Main Camera").GetComponent<CinemachineBrain>());
        var signal_track = playableDirector.playableAsset.outputs.First(c => c.streamName == "Signal Track");
        playableDirector.SetGenericBinding(signal_track.sourceObject,gameObject.GetComponent<SignalReceiver>());
        
        playableDirector.Play();
        Notte.GetComponent<NpcController>().RecieveFollowSignal();
        yield return new WaitUntil(() => moveNext);
        BossTerminal.transform.position = new Vector3(20,9,0);
        cm.Follow = BossTerminal.transform;
        
        yield return new WaitUntil(() => moveNext);
        yield return new WaitForSeconds(1f);
        
        cm.Follow = _playerInput.GetComponent<Transform>();
        BossTerminal.GetComponentInChildren<Animator>().Play("idle");
        yield return new WaitForSeconds(1f);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        _playerInput.keyJump = keyJump;
        _playerInput.keyDown = keyDown;
        _playerInput.EnableAndIdle();
        //Notte.GetComponent<NpcController>().enabled = true;
        BossTerminal.transform.Find("HitSensor").gameObject.SetActive(true);
        BossTerminal.transform.Find("Model/effect").gameObject.SetActive(true);
        yield return new WaitUntil(()=>BattleStageManager.Instance.isGamePaused==false);
        PlayStoryVoiceWithDialog(4,1010,voiceNotte);
        currentRoutine = null;

    }
    
    IEnumerator CutScene_04()
    {
        var bossBehavior = BossTerminal.GetComponent<DragaliaEnemyBehavior>();
        var attackFXPlayer = GameObject.Find("AttackFXPlayer");
        bossBehavior.isAction = false;
        bossBehavior.SetState(1);
        yield return new WaitForSeconds(1f);
        PlayStoryVoiceWithDialog(5,1001,sharedVoice);
        yield return new WaitForSeconds(1.5f);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();

        yield return new WaitForSeconds(13.5f);
        
        bossBehavior.playerAlive = false;
        yield return new WaitUntil(() => bossBehavior.isAction == false);
        _playerInput.GetComponentInChildren<TargetAimer>().enabled = false;
        var cm = StageCameraController.MainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        cm.Follow = Zethia.transform;
        cm.GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.zero;
        PlayStoryVoiceWithDialog(6,1019,sharedVoice);
        var prayerFX = Instantiate(vfx01, Zethia.transform.position, Quaternion.identity,Zethia.transform.Find("BuffLayer"));
        prayerFX.name = "PrayerFX";
        prayerFX.SetActive(true);
        
        yield return new WaitForSeconds(3f);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        _playerInput.keySkill2 = keySkill2;
        var chara_UIGroup = UIElements.transform.Find("CharacterInfo").GetComponent<CanvasGroup>();
        UIElements.transform.Find("CharacterInfo/Skill02").gameObject.SetActive(true);
        var alchemicGaugeObj = UIElements.transform.Find("CharacterInfo/AlchemicGauge");
        UIElements.transform.Find("CharacterInfo/AlchemicGauge").gameObject.SetActive(true);
        var alchemicGauge = alchemicGaugeObj.GetComponent<AlchemicGauge>();
        alchemicGauge.Reset();
        alchemicGauge.ChargeTo(33,3);
        cm.Follow = _playerInput.transform;
        var cmConfiner = cm.GetComponent<CinemachineConfiner2D>();
        _playerInput.GetComponentInChildren<TargetAimer>().enabled = true;

        yield return new WaitForSeconds(1f);
        bossBehavior.playerAlive = true;

        yield return new WaitUntil(() => alchemicGauge.IsCatridgeActive());
        yield return new WaitForSeconds(1.5f);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        var bossStatus = UIElements.transform.Find("BossStatusBarTutBoss").GetComponent<UI_BossStatus>();
        var bossStatusManager = BossTerminal.GetComponent<SpecialStatusManager>();
        bossStatus.bossStat = bossStatusManager;
        bossStatusManager.baseBreak = 1048560;
        bossStatusManager.currentBreak = 1048560;
        bossStatusManager.currentHp = (int)(bossStatusManager.maxHP * 0.98f);

        bossStatus.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        
        Zethia.transform.Find("HitSensor").gameObject.SetActive(false);
        PlayStoryVoiceWithDialog(7,8003,sharedVoice);
        yield return new WaitForSeconds(4f);
        PlayStoryVoiceWithDialog(13,1008,sharedVoice);
        yield return new WaitForSeconds(2f);
        PlayStoryVoiceWithDialog(8,1019,sharedVoice);
        yield return new WaitForSeconds(11f);
        PlayStoryVoiceWithDialog(9,1008,sharedVoice);
        yield return new WaitForSeconds(2.1f);
        PlayStoryVoiceWithDialog(10,1008,sharedVoice);
        yield return new WaitForSeconds(3.5f);
        GetComponent<AudioSource>().clip = newBGM;
        GetComponent<AudioSource>().Play();
        PlayStoryVoiceWithDialog(11,1019,sharedVoice);
        yield return new WaitForSeconds(9.5f);
        PlayStoryVoiceWithDialog(12,1019,sharedVoice);
        bossBehavior.SetState(999);
        yield return new WaitForSeconds(3f);
        yield return new WaitUntil(()=>bossBehavior.isAction==false);
        
        //开始过场动画
        BossTerminal.transform.Find("HitSensor").gameObject.SetActive(false);
        bossBehavior.isAction = true;
        _playerInput.GetComponentInChildren<TargetAimer>().enabled = false;
        chara_UIGroup.alpha = 0;

        cm.Follow = Zethia.transform;
        _playerInput.DisableAndIdle();
        yield return null;
        _playerInput.DisableAndIdle();
        yield return new WaitForSeconds(3f);
        var blessEffect = attackFXPlayer.transform.Find("BlessEffect");
        var blessEffect_cameralock = blessEffect.Find("lockpoint");
        blessEffect.gameObject.SetActive(true);
        cm.Follow = blessEffect_cameralock;
        cm.GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.zero;
        cmConfiner.enabled = false;
        
        yield return new WaitForSeconds(4.5f);
        var blackinEffectImg = UIElements.transform.Find("FullScreenEffect/BlackIn").GetComponent<Image>();
        var _tweener = blackinEffectImg.DOFade(1, 1f);
        //_tweener.Play();
        yield return new WaitForSeconds(1.5f);
        cm.Follow = null;
        yield return null;
        attackFXPlayer.transform.Find("PrologueCutscene").gameObject.SetActive(true);
        cm.Follow = attackFXPlayer.transform.Find("PrologueCutscene/Center");
        print("Start PrologueCutscene");
        _tweener = blackinEffectImg.DOFade(0, 1.5f);
        //_tweener.Play();
        yield return new WaitForSeconds(5f);
        //yield return new WaitForSeconds(3.5f);
        PlayStoryVoiceWithDialog(27,1008,sharedVoice);

        yield return new WaitForSeconds(6f);
        PlayStoryVoiceWithDialog(14,8003,sharedVoice);
        yield return new WaitForSeconds(7.5f);
        PlayStoryVoiceWithDialog(15,8003,sharedVoice);
        
        yield return new WaitForSeconds(1.5f);
        _tweener = blackinEffectImg.DOFade(1, 1.5f);
        yield return new WaitForSeconds(1.5f);
        Destroy(attackFXPlayer.transform.Find("PrologueCutscene").gameObject,1f);
        cm.Follow = _playerInput.transform;
        cmConfiner.enabled = true;
        //cm.transform.position = _playerInput.transform.position;
        _playerInput.GetComponentInChildren<TargetAimer>().enabled = true;

        yield return new WaitForSeconds(1.5f);
        _tweener = blackinEffectImg.DOFade(0, 1.5f);
        
        yield return new WaitForSeconds(1.5f);
        _playerInput.EnableAndIdle();
        BossTerminal.transform.Find("HitSensor").gameObject.SetActive(true);
        _playerInput.InvokeAttackSignal();
        chara_UIGroup.alpha = 1;
        
        yield return new WaitForSeconds(1f);
        PlayStoryVoiceWithDialog(28,8003,sharedVoice);
        yield return new WaitForSeconds(3.5f);
        bossBehavior.SetState(2);
        bossBehavior.isAction = false;
        
        var playerStat = _playerInput.GetComponent<StatusManager>();

        yield return new WaitForSeconds(3f);
        
        yield return new WaitUntil(()=>playerStat.GetConditionStackNumber(300) > 0);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();//解锁技能3
        _playerInput.keySkill3 = keySkill3;
        _playerInput.keyUp = keySpecial;
        UIElements.transform.Find("CharacterInfo/Skill03").gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2f);
        SupportSkillCutIn(4);//ljx
        PlayStoryVoiceWithDialog(16,1015,sharedVoice);
        yield return new WaitForSeconds(2.5f);
        ReliefeDebuffAllPlayers();
        EmitEffect();
        
        yield return new WaitForSeconds(3f);
        SupportSkillCutIn(5);
        PlayStoryVoiceWithDialog(17,9002,sharedVoice);
        yield return new WaitForSeconds(2.5f);
        UseSupportSkill(5);
        EmitEffect();
        
        yield return new WaitForSeconds(3f);
        SupportSkillCutIn(6);
        PlayStoryVoiceWithDialog(18,9003,sharedVoice);
        yield return new WaitForSeconds(2.5f);
        UseSupportSkill(6);
        EmitEffect();
        
        yield return new WaitForSeconds(1.5f);
        SupportSkillCutIn(7);
        PlayStoryVoiceWithDialog(19,9001,sharedVoice);
        yield return new WaitForSeconds(2.5f);
        UseSupportSkill(7);
        EmitEffect();
        
        yield return new WaitForSeconds(2f);
        SupportSkillCutIn(8);
        PlayStoryVoiceWithDialog(20,1011,sharedVoice);
        yield return new WaitForSeconds(2f);
        UseSupportSkill(8);
        EmitEffect();

        yield return new WaitForSeconds(3f);
        SupportSkillCutIn(9);
        PlayStoryVoiceWithDialog(21,1012,sharedVoice);
        yield return new WaitForSeconds(2.5f);
        UseSupportSkill(9);
        EmitEffect();
        
        yield return new WaitForSeconds(2.5f);
        SupportSkillCutIn(10);
        PlayStoryVoiceWithDialog(22,1009,sharedVoice);
        yield return new WaitForSeconds(2f);
        UseSupportSkill(10);
        
        
        yield return new WaitForSeconds(2.5f);
        SupportSkillCutIn(11);
        PlayStoryVoiceWithDialog(23,1013,sharedVoice);
        yield return new WaitForSeconds(2f);
        UseSupportSkill(11);
        EmitEffect();
        
        yield return new WaitForSeconds(2.5f);
        SupportSkillCutIn(12);
        PlayStoryVoiceWithDialog(24,1007,sharedVoice);
        yield return new WaitForSeconds(2f);
        UseSupportSkill(12);
        EmitEffect();
        
        yield return new WaitForSeconds(2.5f);
        SupportSkillCutIn(13);
        PlayStoryVoiceWithDialog(25,1005,sharedVoice);
        yield return new WaitForSeconds(2f);
        UseSupportSkill(13);
        
        
        
        yield return new WaitForSeconds(2f);
        PlayStoryVoiceWithDialog(26,1003,sharedVoice);
        StageCameraController.SwitchOverallCamera();
        yield return new WaitForSeconds(3.5f);
        SupportSkillCutIn(14);
        yield return new WaitForSeconds(2f);
        UseSupportSkill(14);
        EmitEffect();
        
        yield return new WaitForSeconds(0.5f);
        UseSupportSkill(15);
        StageCameraController.SwitchMainCamera();
        yield return new WaitForSeconds(0.5f);
        bossBehavior.SetState(999);

        yield return new WaitForSeconds(.5f);
        PlayStoryVoiceWithDialog(29,1008,sharedVoice);
        
        yield return new WaitForSeconds(4.5f);
        yield return new WaitUntil(() => !bossBehavior.isAction);
        
        bossBehavior.SetState(3);
        //bossStatusManager.baseBreak *= 2;
        //Dotween将float类型的bossStatusManager.currentBreak增加到baseBreak的数值
        DOTween.To(() => bossStatusManager.currentBreak, x => bossStatusManager.currentBreak = x, bossStatusManager.baseBreak, 1f);
        yield return new WaitForSeconds(1f);
        bossStatusManager.ODLock = false;
        bossStatusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.Scorchrend);
        bossStatusManager.baseDef = 512;
        bossStatusManager.counterModifier = 1024f;
        bossStatusManager.breakDefRate = (1 / 128f);
        var potency = (bossStatusManager.currentHp * 100f) / (bossStatusManager.maxHP * 1f);
        bossStatusManager.HPRegenImmediatelyWithoutRandom(0,30-potency);
        yield return new WaitForSeconds(2f);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        yield return new WaitUntil(() => !bossStatusManager.broken);

        yield return new WaitUntil(() => bossStatusManager.broken && bossBehavior.GetState().Item1 == 3);
        PlayStoryVoiceWithDialog(31,8003,sharedVoice);
        bossBehavior.SetState(998);
        
        yield return new WaitForSeconds(9f);
        bossStatusManager.ODLock = true;
        bossStatusManager.currentHp = 1;
        yield return new WaitForSeconds(1f);
        var enemies = FindObjectsOfType<DragaliaEnemyBehavior>();
        foreach (var enemy in enemies)
        {
            enemy.SetState(999);
            enemy.enabled = false;
        }

        currentRoutine = null;
        currentRoutine = StartCoroutine(CutScene_05());
    }

    IEnumerator CutScene_05()
    {
        //PlayStoryVoiceWithDialog(32,1008,sharedVoice);
        _playerInput.DisableAndIdle();
        BossTerminal.transform.Find("HitSensor").gameObject.SetActive(false);
        var enemyLayer = GameObject.Find("EnemyLayer").transform;
        for(int i = 0; i < enemyLayer.childCount; i++)
        {
            if(enemyLayer.GetChild(i).gameObject != BossTerminal)
                Destroy(enemyLayer.GetChild(i).gameObject);
        }
        var cm = StageCameraController.MainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        //cm.Follow = null;
        var cmConfiner = cm.GetComponent<CinemachineConfiner2D>();
        
        var attackFXLayer = GameObject.Find("AttackFXPlayer");
        UIElements.transform.Find("CharacterInfo").GetComponent<CanvasGroup>().alpha = 0;
        var bossStatus = UIElements.transform.Find("BossStatusBarTutBoss").GetComponent<UI_BossStatus>();
        bossStatus.gameObject.SetActive(false);
        var blackinEffectImg = UIElements.transform.Find("FullScreenEffect/BlackIn").GetComponent<Image>();
        var _tweener = blackinEffectImg.DOFade(1, 0.5f);
        
        
        yield return new WaitForSeconds(1f);
        PlayStoryVoiceWithDialog(32,1008,sharedVoice);
        cmConfiner.enabled = false;
        cm.Follow = lastCameraPosition;
        
        yield return new WaitForSeconds(1f);
        
        //开始播放
        var cs2 = attackFXLayer.transform.Find("PrologueCutscene2").gameObject;
        cs2.SetActive(true);
        _tweener = blackinEffectImg.DOFade(0, 0.5f);
        PlayStoryVoiceWithDialog(33,8003,sharedVoice);
        
        yield return new WaitForSeconds(3.75f);
        blackinEffectImg.color = new Color(1, 1, 1, 0);
        _tweener = blackinEffectImg.DOFade(1, 1.5f);
        
        yield return new WaitForSeconds(2f);
        Destroy(cs2);
        cmConfiner.enabled = true;
        cm.Follow = BossTerminal.transform;
        yield return new WaitForSeconds(1.5f);
        cm.Follow = null;
        _tweener = blackinEffectImg.DOFade(0, 1.5f);

        var positionBoss = BossTerminal.transform.position;
        Destroy(BossTerminal);
        ClearAllConditions();
        yield return new WaitForSeconds(1.5f);
        Instantiate(enemyDeathVFX,positionBoss,Quaternion.identity,attackFXLayer.transform);
        
        
        Zethia.GetComponentInChildren<Animator>().Play("defeat");
        attackFXLayer.transform.Find("BlessEffect").gameObject.SetActive(false);
        attackFXLayer.transform.Find("GroundEffect").gameObject.SetActive(false);
        Zethia.transform.Find("BuffLayer/PrayerFX")?.gameObject.SetActive(false);
        cm.Follow = Zethia.transform;
        
        yield return new WaitForSeconds(1.5f);
        
        yield return new WaitForSeconds(2.5f);
        cm.Follow = _playerInput.transform;
        PlayStoryVoiceWithDialog(34,1008,sharedVoice);
        var bgmPlayer = GetComponent<AudioSource>();
        //DOtween将bgmPlayer的音量从0.5f减少到0f
        DOTween.To(() => bgmPlayer.volume, x => bgmPlayer.volume = x, 0f, 15f);
        
        
        yield return new WaitForSeconds(7f);
        _tweener = blackinEffectImg.DOFade(1, 4f);
        PlayStoryVoiceWithDialog(35,1008,sharedVoice);

        GlobalController.Instance.EndGame();
        UpdatePrologueProgress();
        currentCutScene = 5;
        
        //yield return new WaitUntil(()=>sharedVoice.isPlaying == false);
        yield return new WaitForSeconds(7f);
        
        var resultPage = Instantiate(this.resultPage, UIElements.transform);
        UIElements.transform.Find("Minimap").gameObject.SetActive(false);
        UIElements.transform.Find("MenuButton").gameObject.SetActive(false);
        
        currentRoutine = null;

    }




    protected void SupportSkillCutIn(int id)
    {
        CastSkillInfoToBanner(0,id);
    }

    protected void UseSupportSkill(int id)
    {
        switch (id)
        {
            case 5:
            {
                var critDmgBuff = new TimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff, 80f, -1f, 100);
                GiveBuffToAllPlayers(critDmgBuff, true);
                break;
            }
            case 6:
            {
                var attackBuff = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff, 20f, -1f, 100);
                var deffBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff, 20f, -1f, 100);
                var hotBuff = new TimerBuff((int)BasicCalculation.BattleCondition.HotRecovery, 10f, -1f, 100);
                GiveBuffToAllPlayers(attackBuff, true,-1,false);
                GiveBuffToAllPlayers(deffBuff, true);
                GiveBuffToAllPlayers(hotBuff, true,-1,false);
                break;
            }
            case 7:
            {
                var dmgCutBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageCutConst, 1000f, -1f, 100);
                GiveBuffToAllPlayers(dmgCutBuff, true);
                break;
            }
            case 8:
            {
                var attackBuff = new TimerBuff((int)BasicCalculation.BattleCondition.HolyAccord, 30f, -1f, 100);
                GiveBuffToAllPlayers(attackBuff, true);
                break;
            }
            case 9:
            {
                var blindResBuff = new TimerBuff((int)BasicCalculation.BattleCondition.BlindnessRes, 100f, -1f, 100);
                var paralyzeResBuff = new TimerBuff((int)BasicCalculation.BattleCondition.ParalysisRes, 100f, -1f, 100);
                GiveBuffToAllPlayers(blindResBuff, true);
                GiveBuffToAllPlayers(paralyzeResBuff, true,-1,false);
                break;
            }
            case 10:
            {
                GameObject.Find("AttackFXPlayer").transform.Find("SupportCleo").gameObject.SetActive(true);
                break;
            }
            case 11:
            {
                var heartAflameBuff = new TimerBuff((int)BasicCalculation.BattleCondition.HeartAflame, 2f, -1f, 1);
                GiveBuffToAllPlayers(heartAflameBuff, false);
                var standardAttackBurnBuff = new TimerBuff((int)BasicCalculation.BattleCondition.StandardAttackBurner, 72.7f, -1f, 1);
                GiveBuffToPlayer(_playerInput.gameObject, standardAttackBurnBuff, false);
                break;
            }
            case 12:
            {
                var afflictedPunisherBuff = new TimerBuff((int)BasicCalculation.BattleCondition.AfflictedPunisher, -1f, -1f, 1);
                GiveBuffToAllPlayers(afflictedPunisherBuff, false);
                break;
            }
            case 13:
            {
                var scorchrendBuff = new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend, 41600f, 600f, 100);
                GiveBuffToPlayer(BossTerminal, scorchrendBuff,true);
                var vunerableBuff = new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable, 15f, -1f, 1);
                GiveBuffToPlayer(BossTerminal, vunerableBuff,false);
                var proj = Instantiate(laserFX, BossTerminal.transform.position,Quaternion.identity,GameObject.Find("AttackFXPlayer").transform);
                Destroy(proj,2.5f);
                break;
            }
            case 14:
            {
                var holyCrown = Instantiate(holyCrownFX,GameObject.Find("AttackFXPlayer").transform);
                holyCrown.GetComponent<AttackContainer>().InitAttackContainer(10,false);
                if (BossTerminal.transform.position.x < 0)
                {
                    holyCrown.transform.rotation = Quaternion.Euler(holyCrown.transform.rotation.eulerAngles.x,
                        180,holyCrown.transform.rotation.eulerAngles.z);
                    holyCrown.transform.GetComponent<Projectile_C003_3>().firedir = -1;
                }

                for (int i = 0; i < holyCrown.transform.childCount; i++)
                {
                    holyCrown.transform.GetChild(i).GetComponent<Projectile_C003_2>().playerPos =
                        ZenaStatusManager.gameObject;
                }
                var holyfaithBuff = new TimerBuff((int)BasicCalculation.BattleCondition.HolyFaith, -1f, -1f, 1);
                GiveBuffToAllPlayers(holyfaithBuff, false);
                break;
            }
            case 15:
            {
                var holyCrown = Instantiate(holyCrownFX,GameObject.Find("AttackFXPlayer").transform);
                holyCrown.GetComponent<AttackContainer>().InitAttackContainer(10,false);
                for (int i = 0; i < holyCrown.transform.childCount; i++)
                {
                    holyCrown.transform.GetChild(i).GetComponent<Projectile_C003_2>().playerPos =
                        ZenaStatusManager.gameObject;
                }
                if (BossTerminal.transform.position.x < 0)
                {
                    
                    holyCrown.transform.rotation = Quaternion.Euler(holyCrown.transform.rotation.eulerAngles.x,
                        180,holyCrown.transform.rotation.eulerAngles.z);
                    holyCrown.transform.GetComponent<Projectile_C003_3>().firedir = -1;
                }
                break;
            }
        }
    }


    public void ClearAllConditions()
    {
        var statusManagers = playerPool.GetComponentsInChildren<StatusManager>();
        foreach (var statusManager in statusManagers)
        {
            statusManager.ResetAllStatusForced();
        }
    }

    public void GiveBuffToAllPlayers(BattleCondition buff,bool stackable,int spID = -1, bool hasEffect = true)
    {
        var statusManagers = playerPool.GetComponentsInChildren<StatusManager>();
        foreach (var statusManager in statusManagers)
        {
            if(stackable)
                statusManager.ObtainTimerBuff(buff,hasEffect);
            else
            {
                statusManager.ObtainUnstackableTimerBuff(buff.buffID,buff.effect,spID);
            }
        }
    }
    
    public void ReliefeDebuffAllPlayers()
    {
        var statusManagers = playerPool.GetComponentsInChildren<StatusManager>();
        foreach (var statusManager in statusManagers)
        {
            statusManager.ReliefAllDebuff();
        }
    }

    public void GiveBuffToPlayer(GameObject obj, BattleCondition buff, bool stackable, int spID = -1, bool hasEffect = true)
    {
        var statusManager = obj.GetComponent<StatusManager>();
        if(stackable)
            statusManager.ObtainTimerBuff(buff,hasEffect);
        else
        {
            statusManager.ObtainUnstackableTimerBuff(buff.buffID,buff.effect,spID);
        }
    }

    protected void EmitEffect()
    {
        StoryFXEmitter_01.Instance.EmitPrefab(Notte);
        StoryFXEmitter_01.Instance.EmitPrefab(_playerInput.gameObject);
        Instantiate(prayerFX, _playerInput.transform.position-new Vector3(0,3,0), Quaternion.identity,
            _playerInput.transform.Find("BuffLayer"));
        Instantiate(prayerFX, Notte.transform.position-new Vector3(0,3,0), Quaternion.identity,
            Notte.transform.Find("BuffLayer"));
    }

    private void OpenTutorialHintPauseMenuAndTurnToNewestPage()
    {
        BattleSceneUIManager.Instance.OpenPauseMenu();
        UI_Tutorial_PauseMenu.Instance.UpdatePanel(new[]
            {
            "PagesIncrement" 
            }
        );
    }
    private void OpenTutorialHintFirstPage()
    {
        battleSceneUIManager.OpenPauseMenu();
        pauseMenu.UpdatePanel(new[]
            {
                "ShowFirstPage" 
            }
        );
    }

    public void ShakeScreen()
    {
        CineMachineOperator.Instance.CamaraShake(10f,0.2f);
    }

    public void UpdatePrologueProgress()
    {
        var clearTime = (double)Mathf.Round((float)BattleStageManager.Instance.currentTime*10f) / 10f;
        var newQuestState = new QuestSave("100001", 
            clearTime, 1, 1, 1);
        string path = Application.streamingAssetsPath + "/savedata/testSaveData.json";
        StreamReader sr = new StreamReader(path);
        var str = sr.ReadToEnd();
        sr.Close();
        var datalist = JsonMapper.ToObject<QuestDataList>(str);
        var savedata = JsonMapper.ToObject<QuestSave>(str);
        bool isFound = false;
        foreach (var data in datalist.quest_info)
        {
            if (data.quest_id == "100001")
            {
                isFound = true;
                savedata = data;
            }
        }
        if (!isFound)
        {
            datalist.quest_info.Add(newQuestState);
            savedata = datalist.quest_info[datalist.quest_info.Count - 1];
        }
        
        if (newQuestState.best_clear_time < savedata.best_clear_time || savedata.best_clear_time < 0)
        {
            savedata.best_clear_time = newQuestState.best_clear_time;
        }
        
        string jsonStr = JsonMapper.ToJson(datalist);
        string filePath = Application.streamingAssetsPath + "/savedata/testSaveData.json";
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(jsonStr);
        sw.Close();
        
    }

    public void LoadAssets()
    {
        
    }



}
