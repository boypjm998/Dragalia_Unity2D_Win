using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimation;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.Rendering;

public class StoryTimelineManager_100002 : StoryBattleTimelineManager
{
    [SerializeField] private GameObject GO_Nevin;
    [SerializeField] private GameObject GO_Ramiel;
    [SerializeField] private GameObject GO_Pinon;
    [SerializeField] private GameObject GO_Gabriel;
    [SerializeField] private GameObject GO_Basileus;
    private GameObject GO_Harle;
    private List<GameObject> GO_FallenAngelList = new List<GameObject>();

    private StandardCharacterController AC_Nevin;
    private ActorControllerSpecial AC_Ramiel;
    private ActorControllerSpecial AC_Gabriel;
    private StandardCharacterController AC_Basileus;
    private ActorController_c018 AC_Pinon;
    private StandardCharacterController AC_Harle;

    
    [SerializeField] VolumeProfile specialVolumeProfile;
    
    private PlayerInput PI_Pinon;
    private bool moveNext = false;
    
    [SerializeField]private GameObject[] EFF_GO_Nevin = new GameObject[2];

    public bool debug;
    public bool skip;

    [SerializeField]private Sprite UI_SPR_Pinon_SP;
    private TimerBuff DBF_Nil;
    private TimerBuff DBF_HPDecrease;
    
    
    public override void InitializeScene()
    {
        UIElements = GameObject.Find("UI");
        
        Debug.Log("初始化UI中");


        var playerBundle = GlobalController.Instance.GetBundle("player/player_c018");

        var npcBundle1 = GlobalController.Instance.GetBundle("npc/npc_c026_e");
        var npcBundle2 = GlobalController.Instance.GetBundle("npc/npc_c028");
        var npcBundle3 = GlobalController.Instance.GetBundle("npc/npc_d011");
        var npcBundle4 = GlobalController.Instance.GetBundle("npc/npc_d012");
        var npcBundle5 = GlobalController.Instance.GetBundle("npc/npc_c031");

        GO_Pinon = 
            Instantiate(playerBundle.LoadAsset<GameObject>("PlayerHandle"),
                new Vector3(2.5f,-1),Quaternion.identity,
                BattleStageManager.Instance.PlayerLayer.transform);
        
        GO_Pinon.name = "PlayerHandle";
        BattleStageManager.Instance.SetPlayer(GO_Pinon);
        GO_Pinon.GetComponent<PlayerStatusManager>().remainReviveTimes = BattleStageManager.Instance.maxReviveTime;
        PI_Pinon = GO_Pinon.GetComponent<PlayerInput>();
        PI_Pinon.enabled = false;
        
        
        
        Debug.Log("玩家实例化成功");
        
        GO_Gabriel =
            Instantiate(npcBundle3.LoadAsset<GameObject>("Npc_D011"),
                new Vector3(-0.5f, -1), Quaternion.identity,
                BattleStageManager.Instance.PlayerLayer.transform);
        
        GO_Nevin =
            Instantiate(npcBundle2.LoadAsset<GameObject>("Npc_c028"),
                new Vector3(9.3f, -1), Quaternion.identity,
                BattleStageManager.Instance.PlayerLayer.transform);
        
        
        GO_Ramiel =
            Instantiate(npcBundle4.LoadAsset<GameObject>("Npc_D012"),
                new Vector3(6f, -1), Quaternion.identity,
                BattleStageManager.Instance.PlayerLayer.transform);
        
        GO_Basileus =
            Instantiate(npcBundle1.LoadAsset<GameObject>("Npc_c026_e"),
                new Vector3(18f, -1), Quaternion.identity,
                BattleStageManager.Instance.EnemyLayer.transform);

        var uiBundle = GlobalController.Instance.GetBundle("ui/ui_c018");
        var UICharaInfoAsset = uiBundle.LoadAsset<GameObject>("CharacterInfo");
        var UICharaInfoClone = Instantiate(UICharaInfoAsset, UIElements.transform);
        UICharaInfoClone.name = "CharacterInfo";
        GameObject.Find("UI").transform.Find("Minimap").gameObject.SetActive(true);
        
        InitUIElements();
        SetSkillIconAlpha(-1,0);
        SetCharacterIconAlpha(0);
        BattleStageManager.Instance.chara_id = 18;


    }

    private void Update()
    {
        if (debug)
        {
            debug = false;
            StartQuest();
        }

        if (skip)
        {
            skip = false;
            SkipCutScene();
        }
    }

    public override void StartQuest()
    {
        //InitUIElements();
        //FakeLoadStoryInfoForDebug();
        BattleStageManager.Instance.loseControllTime = 4;
        maxEnemyCount = 20;
        //BattleStageManager.Instance.SetMaxEnemy(maxEnemyCount);
        if(currentCutSceneCoroutine != null)
            return;
        currentCutSceneCoroutine = StartCoroutine(CutScene_01());
    }


    private void FakeLoadStoryInfoForDebug()
    {
        StoryLevelDetailedInfo info = new();
        GlobalController.currentCharacterID = 18;

        info.name = "恶魔的线索";
        info.revive_limit = 3;
        info.crown_revive_limit = 0;
        info.time_limit = -1;
        info.crown_time_limit = 360;
        info.clear_condition = 2;
        
        BattleStageManager.Instance.LoadStoryLevelDetailedInfo(18, info);


    }



    private IEnumerator CutScene_01()
    {
        //赋值
        BattleStageManager.Instance.GetMapBorderInfo();
        UI_DialogDisplayer.Instance.LoadBasicStoryInfo("100002");
        GlobalController.currentCharacterID = 18;
        AC_Nevin = GetNPCActor(GO_Nevin);
        AC_Ramiel = GetNPCFlyingActor(GO_Ramiel);
        AC_Gabriel = GetNPCFlyingActor(GO_Gabriel);
        AC_Basileus = GetNPCActor(GO_Basileus);
        
        PI_Pinon = GetPlayerInput();
        BattleStageManager.Instance.InitPlayer(GO_Pinon);
        
        AC_Pinon = GetPlayerActor() as ActorController_c018;
        AC_Pinon.ta.enabled = false;
        SetCharacterIconAlpha(1);
        
        EFF_GO_Nevin = GetAllEffectsInCharacter(GO_Nevin).ToArray();

        DBF_Nil = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility, -1, -1, 1);

        //初始化
        PI_Pinon.DisableAndIdle();
        SwitchCameraTo(GO_Nevin);
        SetSkillIconAlpha(-1,0);
        
        
        
        yield return new WaitForSeconds(1f);
        
        
        SwitchCameraTo(transform.Find("Anchor").gameObject);
        
        //开始过场动画
        
        PlayAnimation(AC_Nevin,"action02");
        SetVisibilityOfSkipButton(true);
        
        yield return new WaitForSeconds(0.3f);
        
        
        SpawnFXPrefab(prefabList[0],AC_Nevin.transform.position);
        
        
        yield return new WaitForSeconds(0.2f);
        
        
        PlayAnimation(AC_Basileus,"action01");
        
        
        yield return new WaitForSeconds(1.2f);
        
        
        CineMachineOperator.Instance.CamaraShake(10f,0.4f);

        
        yield return new WaitForSeconds(1.8f);
        
        
        PlayAnimation(AC_Basileus,"action08");
        
        
        yield return new WaitForSeconds(0.7f);
        
        
        SpawnFXPrefab(prefabList[1],AC_Nevin.transform.position);
        AC_Nevin.SetFaceDir(-1);
        PlayAnimation(AC_Nevin,"roll");
        PlayAnimation(AC_Ramiel,"combo1");
        
        yield return new WaitForSeconds(0.2f);
        
        CineMachineOperator.Instance.CamaraShake(10f,0.2f);
        
        yield return new WaitForSeconds(0.6f);
        
        PlayAnimation(AC_Nevin,"action05");
        
        yield return new WaitForSeconds(0.2f);
        
        PlayAnimation(AC_Basileus,"action05");
        AC_Nevin.SetFaceDir(1);

        yield return new WaitForSeconds(0.1f);
        
        //KnockBack
        GO_Basileus.transform.DOMoveX(GO_Basileus.transform.position.x + 2.5f, 0.5f).SetEase(Ease.OutSine);
        CineMachineOperator.Instance.CamaraShake(10f,0.2f);
        
        yield return new WaitForSeconds(1f);
        
        PlayStoryVoiceWithDialog(0,1026,storyVoiceList[0]);
        
        //Nevin Warp Attack
        yield return new WaitForSeconds(4f);
        
        PlayStoryVoiceWithDialog(1,1028,storyVoiceList[1]);
        
        yield return new WaitForSeconds(2.25f);
        
        AC_Nevin.SwapWeaponVisibility(true);
        AC_Nevin.DisappearRenderer();
        //PlayAnimation(AC_Nevin,"action08");
        SpawnFXPrefab(prefabList[2],AC_Nevin.transform.position);
        PlayAnimation(AC_Basileus,"action06");
        
        yield return new WaitForSeconds(0.25f);
        
        GO_Nevin.transform.position = GO_Basileus.transform.position - new Vector3(4f,0,0);
        SwitchCameraTo(GO_Nevin);
        AC_Nevin.AppearRenderer();
        PlayAnimation(AC_Nevin,"action08");
        SpawnFXPrefab(prefabList[2],GO_Nevin.transform.position);
        SpawnFXPrefab(prefabList[3],GO_Basileus.transform.position);

        yield return new WaitForSeconds(0.25f);
        
        CineMachineOperator.Instance.CamaraShake(10f,0.2f);
        
        yield return new WaitForSeconds(0.2f);
        
        GO_Nevin.transform.DOMoveX(GO_Nevin.transform.position.x - 3f, 0.5f).SetEase(Ease.OutSine);
        
        yield return new WaitForSeconds(0.5f);
        
        AC_Nevin.SetFaceDir(-1);
        PlayAnimation(AC_Nevin,"roll");
        
        yield return new WaitForSeconds(0.8f);
        
        AC_Nevin.SetFaceDir(1);
        PlayStoryVoiceWithDialog(2,1026,storyVoiceList[2]);
        
        yield return new WaitForSeconds(6.5f);
        
        PlayAnimation(AC_Basileus,"action02");
        
        yield return new WaitForSeconds(0.5f);
        
        SpawnFXPrefab(prefabList[4],new Vector3(6,0));
        var magicDeviceFX = SpawnFXPrefab(prefabList[18], AC_Basileus.transform.position - new Vector3(3, 0));
        SwitchCameraTo(GO_Ramiel);
        
        
        yield return new WaitForSeconds(2.5f);
        
        var blackSmoke1 = SpawnFXPrefab(prefabList[5],GO_Nevin.transform.position);
        PlayAnimation(AC_Nevin,"break_enter");
        
        yield return new WaitForSeconds(1.5f);
        
        SigilLockEffect(EFF_GO_Nevin,false);
        var SigilLockBuff = new TimerBuff((int)BasicCalculation.BattleCondition.LockedSigil,
            -1, -1, 1, 102801);
        AC_Nevin.GiveTimerBuff(SigilLockBuff);
        AC_Nevin.GiveTimerBuff(DBF_Nil);
        PlayStoryVoiceWithDialog(3,1028,storyVoiceList[3]);
        
        yield return new WaitForSeconds(2.5f);
        
        PlayAnimation(AC_Ramiel,"break_enter_slow");
        PlayStoryVoiceWithDialog(4,2012,storyVoiceList[4]);
        var blackSmoke2 = SpawnFXPrefab(prefabList[5],GO_Ramiel.transform.position);
        GlobalVolumeController.Instance.SetSpecialVolume(specialVolumeProfile);
        SwitchCameraTo(GO_Pinon);
        
        yield return new WaitForSeconds(6f);
        
        PlayAnimation(AC_Pinon,"break_enter");
        var blackSmoke3 = SpawnFXPrefab(prefabList[5],GO_Pinon.transform.position);
        SetCharacterIconFacialExpression(UI_SPR_Pinon_SP);
        AC_Pinon.GiveTimerBuff(SigilLockBuff);
        AC_Pinon.GiveTimerBuff(DBF_Nil);
        GlobalVolumeController.Instance.SpecialVolumeIntensityDoFade(1,2f);
        
        
        yield return new WaitForSeconds(1.5f);
        
        
        PlayAnimation(AC_Gabriel,"break_enter_slow");
        var blackSmoke4 = SpawnFXPrefab(prefabList[5],GO_Gabriel.transform.position);
        
        yield return new WaitForSeconds(1.5f);
        
        PlayStoryVoiceWithDialog(5,2011,storyVoiceList[5]);
        GlobalVolumeController.Instance.SpecialVolumeIntensityDoFade(0,10f);
        
        yield return new WaitForSeconds(7f);
        
        PlayStoryVoiceWithDialog(6,1026,storyVoiceList[6]);
        PlayStoryVoiceWithDialog(7,1026,storyVoiceList[7]);
        SwitchCameraTo(GO_Basileus);
        AC_Basileus.SetFaceDir(-1);
        
        yield return new WaitForSeconds(12f);
        
        Destroy(blackSmoke1);
        Destroy(blackSmoke2);
        Destroy(blackSmoke3);
        Destroy(blackSmoke4);
        PlayStoryVoiceWithDialog(8,1018,storyVoiceList[8]);
        SwitchCameraTo(GO_Pinon);
        
        yield return new WaitForSeconds(2.5f);
        
        GO_FallenAngelList.Add(SpawnEnemyPrefab(enemyList[0],new Vector3(40,0)));
        
        
        yield return new WaitForSeconds(2f);
        
        PlayStoryVoiceWithDialog(9,1026,storyVoiceList[9]);
        PlayStoryVoiceWithDialog(10,1026,storyVoiceList[10]);
        SwitchCameraTo(GO_Basileus);
        
        yield return new WaitForSeconds(3f);
        
        var EC_FallenGabriel = GO_FallenAngelList[0].GetComponent<EnemyControllerFlyingHigh>();
        EC_FallenGabriel.StartCoroutine(EC_FallenGabriel.
            FlyToPoint(GO_Basileus.transform.position + new Vector3(2,-1,0),
                2.5f,Ease.InOutSine));
        
        
        yield return new WaitForSeconds(3.5f);
        var EC_FallenUriel = SpawnEnemyPrefab(enemyList[1],new Vector3(-18,-1)).GetComponent<EnemyControllerFlyingHigh>();
        
        yield return new WaitForSeconds(1f);
        EC_FallenUriel.StartCoroutine(EC_FallenUriel.
            FlyToPoint(new Vector3(-10,-1,0),
                2f,Ease.InOutSine));
        
        yield return new WaitForSeconds(1f);
        SwitchCameraTo(GO_Pinon);
        
        yield return new WaitForSeconds(1f);
        EC_FallenGabriel.StartCoroutine(EC_FallenGabriel.
            FlyToPoint(new Vector3(15,-1,0),
                1.5f,Ease.InOutSine));
        
        yield return new WaitForSeconds(2.5f);
        
        
        PlayStoryVoiceWithDialog(11,1026,storyVoiceList[11]);
        
        yield return new WaitForSeconds(1.5f);
        
        PlayAnimation(EC_FallenGabriel,"charge_enter");
        PlayAnimation(EC_FallenUriel,"charge_enter");
        
        yield return new WaitForSeconds(0.5f);
        
        //堕天使开始蓄力
        var waterBallFX = SpawnFXPrefab(prefabList[6],EC_FallenUriel.transform.position + new Vector3(2,3.5f,0));
        var windBallFX = SpawnFXPrefab(prefabList[7],EC_FallenGabriel.transform.position + new Vector3(-1.5f,3.2f,0));

        var _tweenerBall_1 = waterBallFX.transform.DOScale(Vector3.one*5, 6f);
        var _tweenerBall_2 = windBallFX.transform.DOScale(Vector3.one*5, 6f);

        yield return new WaitForSeconds(3.5f);
        
        //哈尔开始攻击乌里尔
        SpawnFXPrefab(prefabList[8], EC_FallenUriel.transform.position);
        PlayAnimation(EC_FallenUriel,"knockback_enter");
        SpawnFXPrefab(prefabList[12],waterBallFX.transform.position);
        CineMachineOperator.Instance.CamaraShake(10f,0.25f);
        Destroy(waterBallFX);
        
        yield return new WaitForSeconds(0.5f);
        
        SpawnFXPrefab(prefabList[14],
            PI_Pinon.transform.position - new Vector3(1,-3f,0));
        
        yield return new WaitForSeconds(0.2f);
        
        //哈尔登场
        GO_Harle = Instantiate(npcList[0],
            new Vector3(BattleStageManager.Instance.mapBorderL, -1), Quaternion.identity,
            BattleStageManager.Instance.PlayerLayer.transform);
        AC_Harle = GetNPCActor(GO_Harle);
        AC_Harle.SetGravityScale(0);
        GO_Harle.transform.position = PI_Pinon.transform.position - new Vector3(1,-3,0);
        PlayAnimation(AC_Harle,"action04");
        
        yield return new WaitForSeconds(0.1f);
        
        //哈尔开始攻击加百列

        PlayStoryVoiceWithoutDialog(storyVoiceList[12]);
        SpawnFXPrefab(prefabList[11],GO_Harle.transform.position).transform.eulerAngles = new Vector3(0,0,-40);
        
        yield return new WaitForSeconds(0.3f);
        
        SpawnFXPrefab(prefabList[9],AC_Harle.transform.position+new Vector3(1,3,0));
        
        yield return new WaitForSeconds(0.1f);
        
        AC_Harle.ResetGravityScale();
        CineMachineOperator.Instance.CamaraShake(15f,0.3f);
        PlayAnimation(EC_FallenGabriel,"knockdown_enter");
        Destroy(windBallFX);
        SpawnFXPrefab(prefabList[13],windBallFX.transform.position);
        SpawnFXPrefab(prefabList[10],EC_FallenGabriel.transform.position + new Vector3(0,1.5f,0));

        yield return new WaitUntil(() => AC_Harle.anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        
        PlayStoryVoiceWithDialog(13,1026,storyVoiceList[13]);
        SwitchCameraTo(GO_Harle);
        PlayAnimation(AC_Harle,"action01");
        PlayAnimation(AC_Basileus,"action04");
        
        yield return null;
        yield return new WaitUntil(()=>AC_Harle.anim.GetCurrentAnimatorStateInfo(0).IsName("action03"));
        //yield return new WaitForSeconds(0.1f);
        
        SpawnFXPrefab(prefabList[14],GO_Harle.transform.position);
        
        yield return new WaitForSeconds(0.2f);
        GO_Harle.transform.position = new Vector3(GO_Basileus.transform.position.x - 3,
                GO_Basileus.transform.position.y);
        SpawnFXPrefab(prefabList[14],GO_Harle.transform.position);

        yield return null;
        
        SpawnFXPrefab(prefabList[15],GO_Harle.transform.position + Vector3.right);
        
        yield return new WaitForSeconds(0.25f);
        
        CineMachineOperator.Instance.CamaraShake(12f,0.3f);
        SpawnFXPrefab(prefabList[10], GO_Basileus.transform.position);
        GO_Basileus.transform.DOMoveX(GO_Basileus.transform.position.x + 5, 0.5f).SetEase(Ease.InOutSine)
            .OnComplete(()=>AC_Basileus.anim.Play("idle"));
        Destroy(magicDeviceFX);
        PlayStoryVoiceWithDialog(14,1031,storyVoiceList[14]);
        
        yield return new WaitForSeconds(7f);
        
        PlayStoryVoiceWithDialog(15,1026,storyVoiceList[15]);
        
        yield return new WaitForSeconds(7f);
        
        SpawnFXPrefab(prefabList[16],GO_Basileus.transform.position);
        
        yield return new WaitForSeconds(0.5f);
        
        SwitchCameraTo(GO_Pinon);
        
        PlayAnimation(AC_Pinon,"break_exit");
        PlayStoryVoiceWithDialog(16,1018,storyVoiceList[16]);
        
        yield return new WaitForSeconds(0.5f);
        GO_Basileus.SetActive(false);

        yield return new WaitUntil(() => AC_Pinon.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        PlayAnimation(AC_Pinon,"idle");
        SetCharacterIconFacialExpression(0);

        yield return null;
        
        AC_Pinon.anim.SetFloat("forward",1);
        GO_FallenAngelList.Clear();
        var EC_FallenAngleMichael = 
            SpawnEnemyPrefab(enemyList[2],new Vector3(45,0)).GetComponent<EnemyController>();
        AC_Gabriel.GetComponent<NpcControllerFlyingDragon>().enabled = false;
        GO_FallenAngelList.Add(EC_FallenAngleMichael.gameObject);
        EC_FallenAngleMichael.SetFaceDir(-1);
        //移动时间 = 距离 / 速度，速度为9

        yield return null;
        
        var moveTime = (GO_Harle.transform.position.x + 16 - GO_Pinon.transform.position.x) / 9;
        var _moveTweener = GO_Pinon.transform.DOMoveX(GO_Harle.transform.position.x + 16, moveTime).SetEase(Ease.Linear);
        
        yield return new WaitForSeconds(1f);
        
        //EC_FallenGabriel.anim.Play("break_exit");
        //EC_FallenGabriel.SetFaceDir(1);
        AC_Harle.SetFaceDir(-1);
        
        yield return new WaitUntil(() => GO_Pinon.transform.position.x >= GO_Harle.transform.position.x + 16);
        
        
        AC_Pinon.anim.SetFloat("forward",0);
        
        Destroy(EC_FallenUriel.gameObject);
        Destroy(EC_FallenGabriel.gameObject);
        
        
        yield return null;

        PlayAnimation(AC_Pinon,"idle");
        
        yield return new WaitForSeconds(0.5f);
        
        
        AC_Pinon.RemoveTimerBuff(SigilLockBuff);
        BattleSceneUIManager.Instance.ReplacePauseMenu(SpawnNewPauseMenu());
        GlobalController.Instance.StartGame();
        
        yield return null;
        
        PI_Pinon.enabled = true;
        
        OpenTutorialHintFirstPage();
        SetSkillIconAlpha(-1,1);
        currentEnemyCount = 1;
        BattleStageManager.Instance.SetMaxEnemy(maxEnemyCount);
        EC_FallenAngleMichael.GetComponent<DragaliaEnemyBehavior>().enabled = true;
        EC_FallenAngleMichael.GetComponent<StatusManager>().ImmuneToAllControlAffliction = true;
        BattleStageManager.Instance.SetLeftBorder(18,true);
        BattleStageManager.Instance.SetCameraLeftBorder(18);
        BattleStageManager.Instance.RefreshCameraBorder();
        GO_Gabriel.SetActive(false);
        GO_Nevin.SetActive(false);
        GO_Harle.SetActive(false);
        GO_Ramiel.SetActive(false);
        Destroy(GO_Basileus);

        currentCutSceneCoroutine = StartCoroutine(CutScene_02());

        //currentCutSceneCoroutine = null;
    }

    private void SkipCutScene()
    {
        StopCoroutine(currentCutSceneCoroutine);
        currentCutSceneCoroutine = null;
        
        DBF_Nil = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility, -1, -1, 1);
        SwitchCameraTo(GO_Pinon);
        PI_Pinon.EnableAndIdle();
        SetCharacterIconFacialExpression(0);
        GlobalVolumeController.Instance.SpecialVolumeIntensityDoFade(0,0.1f);

        var rangedFXLayer = BattleStageManager.Instance.RangedAttackFXLayer.transform;
        for (int i = rangedFXLayer.childCount-1; i >= 0; i--)
        {
            Destroy(rangedFXLayer.GetChild(i).gameObject);
        }


        for (int i = GO_FallenAngelList.Count - 1; i >= 0; i--)
        {
            Destroy(GO_FallenAngelList[i].gameObject);
            GO_FallenAngelList.RemoveAt(i);
        }

        if (GO_Basileus != null)
        {
            Destroy(GO_Basileus);
        }
        
        CineMachineOperator.Instance.StopCameraShake();
        BattleEffectManager.Instance.soundEffectSource.Stop();
        BattleEffectManager.Instance.sharedVoiceSource.Stop();


        var SigilLockBuff = new TimerBuff((int)BasicCalculation.BattleCondition.LockedSigil,
            -1, -1, 1, 102801);
        
        AC_Pinon.RemoveTimerBuff(SigilLockBuff);
        AC_Pinon.GiveTimerBuff(DBF_Nil);
        BattleSceneUIManager.Instance.ReplacePauseMenu(SpawnNewPauseMenu());
        GlobalController.Instance.StartGame();
        
        
        GO_Pinon.transform.position = new Vector3(33.5f, -1.5f);
        
        OpenTutorialHintFirstPage();
        SetSkillIconAlpha(-1,1);
        currentEnemyCount = 1;
        //BattleStageManager.Instance.SetMaxEnemy(maxEnemyCount);

        EnemyController EC_FallenAngleMichael = null;
        if (FindObjectOfType<DB15_BehaviorTree>() == null)
        {
            EC_FallenAngleMichael = 
                SpawnEnemyPrefab(enemyList[2],new Vector3(45,0)).GetComponent<EnemyController>();
        }

        
        AC_Gabriel.GetComponent<NpcControllerFlyingDragon>().enabled = false;
        GO_FallenAngelList.Add(EC_FallenAngleMichael.gameObject);
        
        
        EC_FallenAngleMichael.SetFaceDir(-1);
        EC_FallenAngleMichael.GetComponent<StatusManager>().ImmuneToAllControlAffliction = true;
        EC_FallenAngleMichael.GetComponent<DragaliaEnemyBehavior>().enabled = true;
        
        
        BattleStageManager.Instance.SetLeftBorder(18,true);
        BattleStageManager.Instance.SetCameraLeftBorder(18);
        BattleStageManager.Instance.RefreshCameraBorder();
        BattleStageManager.Instance.SetMaxEnemy(maxEnemyCount);
        GO_Gabriel.SetActive(false);
        GO_Nevin.SetActive(false);
        if(GO_Harle!=null)
            GO_Harle.SetActive(false);
        GO_Ramiel.SetActive(false);
        
        currentCutSceneCoroutine = StartCoroutine(CutScene_02());
    }

    public override void SkipButtonAction()
    {
        skip = true;
        skipCutSceneButtonInstance?.SetActive(false);
    }


    private IEnumerator CutScene_02()
    {
        SetVisibilityOfSkipButton(false);

        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        //BattleStageManager.Instance.RefreshCameraBorder();
        
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        
        PlayStoryVoiceWithDialog(17,1026,storyVoiceList[17]);
        PlayStoryVoiceWithDialog(18,1026,storyVoiceList[18]);
        PI_Pinon.GetComponentInChildren<TargetAimer>().enabled = true;

        yield return new WaitForSeconds(7f);
        yield return new WaitUntil(() => BattleStageManager.Instance.currentEnemyInLayerDeadAlive <= 0);

        var GO_FallenGabriel = SpawnEnemyPrefab(enemyList[0], new Vector3(21, -1));
        var BHV_FallenGabriel = GO_FallenGabriel.GetComponent<EnemyBehaviorManager>();
        var ST_FallenGabriel = GO_FallenGabriel.GetComponent<StatusManager>();
        ST_FallenGabriel.ImmuneToAllControlAffliction = true;
        SetBehavior(BHV_FallenGabriel,"enemy/db_2011","Behavior_20110_2");
        BHV_FallenGabriel.enabled = true;
        currentEnemyCount = 2;

        yield return new WaitForSeconds(3.5f);
        
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        
        
        PlayStoryVoiceWithDialog(19,1031,storyVoiceList[19]);
        PlayStoryVoiceWithDialog(20,1028,storyVoiceList[20]);
        PlayStoryVoiceWithDialog(21,1028,storyVoiceList[21]);
        PlayStoryVoiceWithDialog(22,1031,storyVoiceList[22]);
        
        ST_FallenGabriel.ImmuneToAllControlAffliction = false;

        yield return new WaitUntil(() => ST_FallenGabriel.currentHp <= 0);
        
        yield return new WaitForSeconds(1.2f);
        
        
        SpawnMinonGroup(1,70000);
        var ST_Pinon = PI_Pinon.GetComponent<StatusManager>();

        if (ST_Pinon.GetConditionStackNumber((int)BasicCalculation.BattleCondition.SigilReleased) > 0)
        {
            
        }
        else
        {
            ST_Pinon.OnBuffEventDelegate += RefreshNewCondition;
            moveNext = false;
            while (moveNext == false)
            {
                if (BattleStageManager.Instance.currentEnemyInLayerDeadAlive <= 0)
                {
                    SpawnMinonGroup(2,40000,true);
                }
                yield return null;
            }
            ST_Pinon.OnBuffEventDelegate -= RefreshNewCondition;
            
        }
        
        yield return new WaitUntil(() => BattleStageManager.Instance.currentEnemyInLayerDeadAlive <= 0);
        yield return new WaitForSeconds(1f);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();

        yield return null;
        
        GO_Gabriel.SetActive(true);
        GO_Gabriel.transform.position = new Vector3(BattleStageManager.Instance.mapBorderL + 1.5f, -1, 0);
        GO_Gabriel.GetComponent<NpcControllerFlyingDragon>().enabled = true;
        
        
        SpawnMinonGroup(2,120000);
        
        yield return new WaitUntil(() => BattleStageManager.Instance.currentEnemyInLayerDeadAlive <= 2);
        
        SpawnMinonGroup(1,110000);
        
        //实例化BOSS

        yield return new WaitForSeconds(5f);
        var ST_FallenUriel = SpawnBoss
        (1, new Vector3(BattleStageManager.Instance.mapBorderR - 1, -1),
            false,450000);

        yield return new WaitForSeconds(5f);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        int i = 0;

        while (ST_FallenUriel != null)
        {
            i++;
            
            yield return new WaitUntil(() => BattleStageManager.Instance.currentEnemyInLayerDeadAlive <= 1);
            
            if(ST_FallenUriel==null)
                break;
            
            SpawnMinonGroup(2+(i%2),100000,true);
            
            yield return null;
            
            

        }
        
        yield return new WaitUntil(() => BattleStageManager.Instance.currentEnemyInLayerDeadAlive <= 0);
        
        BattleStageManager.Instance.SetLeftBorder(-15);
        BattleStageManager.Instance.SetCameraLeftBorder(-15);
        BattleStageManager.Instance.RefreshCameraBorder();

        yield return null;

        var GoIndicator = SpawnFXPrefab(prefabList[17], Vector3.zero, 1);

        yield return new WaitUntil(() => GO_Pinon.transform.position.x < 10 &&
                                         GO_Gabriel.transform.position.x < 15);
        
        BattleStageManager.Instance.SetRightBorder(24,true);
        BattleStageManager.Instance.SetCameraRightBorder(24);
        BattleStageManager.Instance.RefreshCameraBorder();
        Destroy(GoIndicator);

        yield return null;
        
        BattleStageManager.Instance.SetMaxEnemy(3);
        
        yield return null;

        var ST_Gabriel = SpawnBoss
        (0, new Vector3(BattleStageManager.Instance.mapBorderL + 1, -1),
            false,650000,2700);
        var ST_Michael = SpawnBoss
        (2, new Vector3(BattleStageManager.Instance.mapBorderR - 1, -1),
            false,750000,2400,false);

        SetBehavior(ST_Michael.GetComponent<EnemyBehaviorManager>(),"enemy/db_2015","Behavior_20150_2");
        ST_Michael.GetComponent<EnemyBehaviorManager>().enabled = true;

        yield return new WaitForSeconds(1f);

        yield return new WaitUntil(() => ST_Michael.currentHp < 375000 || ST_Gabriel.currentHp < 325000);

        ST_FallenUriel = SpawnBoss(1, new Vector3(BattleStageManager.Instance.mapBorderR - 1, -1),
            false, 550000, 2100, false);
        SetBehavior(ST_FallenUriel.GetComponent<EnemyBehaviorManager>(),"enemy/db_2014","Behavior_20140_2");
        ST_FallenUriel.GetComponent<EnemyBehaviorManager>().enabled = true;

        yield return new WaitUntil(() => GlobalController.currentGameState == GlobalController.GameState.End);
        
        BattleStageManager.Instance.KillAllEnemy();
        PI_Pinon.DisableAndIdle();
        
        PlayStoryVoiceWithDialog(23,2011,storyVoiceList[23]);

        currentCutSceneCoroutine = null;
        yield break;
    }


    private void SigilLockEffect(GameObject[] effs, bool flag)
    {
        foreach (var eff in effs)
        {
            eff.SetActive(flag);
        }
    }

    private void RefreshNewCondition(BattleCondition condition)
    {
        if (condition.buffID == (int)BasicCalculation.BattleCondition.SigilReleased)
        {
            moveNext = true;
        }
    }

    private void SpawnMinonGroup(int type,int hp,bool summon = false)
    {
        List<StatusManager> statusManagers = new();
        switch (type)
        {
            case 1:
            {
                statusManagers.Add(
                    SpawnEnemyPrefab(enemyList[3],
                        new Vector3(BattleStageManager.Instance.mapBorderL,-1),summon).
                        GetComponent<StatusManager>());
                statusManagers.Add(
                    SpawnEnemyPrefab(enemyList[3],
                            new Vector3(BattleStageManager.Instance.mapBorderR,-1),summon).
                        GetComponent<StatusManager>());
                statusManagers.Add(
                    SpawnEnemyPrefab(enemyList[4],
                            new Vector3(BattleStageManager.Instance.mapBorderL+1,-1),summon).
                        GetComponent<StatusManager>());
                statusManagers.Add(
                    SpawnEnemyPrefab(enemyList[4],
                            new Vector3(BattleStageManager.Instance.mapBorderR-1,-1),summon).
                        GetComponent<StatusManager>());
                break;
            }
            case 2:
            {
                statusManagers.Add(
                    SpawnEnemyPrefab(enemyList[4],
                            new Vector3(BattleStageManager.Instance.mapBorderL,-1),summon).
                        GetComponent<StatusManager>());
                statusManagers.Add(
                    SpawnEnemyPrefab(enemyList[4],
                            new Vector3(BattleStageManager.Instance.mapBorderR,-1),summon).
                        GetComponent<StatusManager>());
                break;
            }
            case 3:
            {
                statusManagers.Add(
                    SpawnEnemyPrefab(enemyList[3],
                            new Vector3(BattleStageManager.Instance.mapBorderL,-1),summon).
                        GetComponent<StatusManager>());
                statusManagers.Add(
                    SpawnEnemyPrefab(enemyList[3],
                            new Vector3(BattleStageManager.Instance.mapBorderR,-1),summon).
                        GetComponent<StatusManager>());
                break;
            }
        }

        foreach (var status in statusManagers)
        {
            status.maxBaseHP = hp;
            status.maxHP = hp;
        }
    }

    private StatusManager SpawnBoss(int indexInEnemyList, Vector3 position, bool isSummon = false, int hp = -1, int atk = -1,bool enableBehavior=true)
    {
        var GO = SpawnEnemyPrefab(enemyList[indexInEnemyList],
            position);
        var ST = GO.GetComponent<StatusManager>();
        var BHV = GO.GetComponent<EnemyBehaviorManager>();
        if (hp > 0)
        {
            ST.maxBaseHP = hp;
        }

        if (atk > 0)
        {
            ST.baseAtk = atk;
        }

        BHV.enabled = enableBehavior;
        return ST;
    }




}
