using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StoryTimelineManager_100003 : StoryBattleTimelineManager
{

    private GameObject GO_player;
    private PlayerInput PI_player;
    private PlayerStatusManager ST_Player;
    private GameObject RTScene;
    private RawImage rawImg;
    [SerializeField] private bool debug = false;
    [SerializeField] private GameObject[] borders;
    private bool[] signals = new[] { false, false, false, false };
    [SerializeField] private GameObject[] switches;
    [SerializeField] private GameObject[] doors;
    
    

    private void Update()
    {
        if (debug)
        {
            debug = false;
            StartQuest();
        }
    }
    
    private void FakeLoadStoryInfoForDebug()
    {
        StoryLevelDetailedInfo info = new();
        GlobalController.currentCharacterID = 33;

        info.name = "要争取的事物";
        info.revive_limit = 1;
        info.crown_revive_limit = 0;
        info.time_limit = -1;
        info.crown_time_limit = 300;
        info.clear_condition = 4;
        
        BattleStageManager.Instance.LoadStoryLevelDetailedInfo(33, info);


    }

    public override void StartQuest()
    {
        //todo: 在正常情况下，应该是在进入战斗场景时，调用InitializeScene()
        //InitializeScene();
        BattleStageManager.Instance.OnMapEventTriggered += OnMapEventTriggered;
        currentCutSceneCoroutine = StartCoroutine(CutScene_01());
        UI_DialogDisplayer.Instance.LoadBasicStoryInfo("100003");
    }

    private void OnMapEventTriggered(int id)
    {
        signals[id - 1] = true;
    }

    public override void InitializeScene()
    {
        UIElements = GameObject.Find("UI");
        
        Debug.Log("初始化UI中");

        //todo: 实例化玩家，并停止其操控
        var playerBundle = GlobalController.Instance.GetBundle("player/player_c033");
        
        
        GO_player = 
             Instantiate(playerBundle.LoadAsset<GameObject>("PlayerHandle"),
                 new Vector3(-15f,-1),Quaternion.identity,
                 BattleStageManager.Instance.PlayerLayer.transform);

        BattleStageManager.Instance.InitPlayer(GO_player);
        GO_player.name = "PlayerHandle";
        PI_player = GO_player.GetComponent<PlayerInput>();
        PI_player.enabled = false;
        ST_Player = GO_player.GetComponent<PlayerStatusManager>();
        ST_Player.remainReviveTimes = BattleStageManager.Instance.maxReviveTime;
        
        Debug.Log("玩家实例化成功");
        
        //todo: 实例化UI并隐藏技能栏
        var uiBundle = GlobalController.Instance.GetBundle("ui/ui_c033");
        var UICharaInfoAsset = uiBundle.LoadAsset<GameObject>("CharacterInfo");
        var UICharaInfoClone = Instantiate(UICharaInfoAsset, UIElements.transform);
        UICharaInfoClone.name = "CharacterInfo";
        GameObject.Find("UI").transform.Find("Minimap").gameObject.SetActive(true);
        
        InitUIElements();
        
        SetCharacterUIAlpha(0);
        
        SetSkillIconAlpha(1,0);
        SetSkillIconAlpha(2,0);
        SetSkillIconAlpha(3,0);
        SetSkillIconAlpha(4,0);

        PI_player.keySkill1 = KeyCode.None;
        GlobalController.gamepadMap.FindAction("Skill1").Disable();
        PI_player.keySkill2 = KeyCode.None;
        GlobalController.gamepadMap.FindAction("Skill2").Disable();
        PI_player.keySkill3 = KeyCode.None;
        GlobalController.gamepadMap.FindAction("Skill3").Disable();
        PI_player.keySkill4 = KeyCode.None;
        GlobalController.gamepadMap.FindAction("Skill4").Disable();
        
        
        
        RTScene = GameObject.Find("OtherCamera").transform.Find("RT").gameObject;
        var fullScreenUI = GameObject.Find("FullScreenEffect").transform.Find("RT").gameObject;
        

        var cutsceneController = RTScene.GetComponent<CutSceneController>();
        //var director = RTScene.GetComponent<PlayableDirector>();
        var Texture = cutsceneController.rt;

        rawImg = fullScreenUI.GetComponent<RawImage>();
        rawImg.texture = Texture;
        
        RTScene.SetActive(true);
        
        
        
        //FakeLoadStoryInfoForDebug();
        
        
        
    }

    private IEnumerator CutScene_01()
    {
        GlobalController.currentCharacterID = 33;
        
        //todo: Destory掉Loading Screen。
        var loadingScreen = GameObject.Find("LoadingScreen");
        var loadingAnim = loadingScreen.GetComponent<Animator>();
        
        loadingAnim.SetBool("loaded",true);
        
        yield return new WaitUntil(()=>loadingAnim.GetCurrentAnimatorStateInfo(0).IsName("End"));
        
        
        RTScene.transform.position = new Vector3(0, 100, 0);
        RTScene.SetActive(true);
        rawImg.gameObject.SetActive(true);
        
        
        
        Debug.Log("Loading Screen消失");
        // Destroy(loadingScreen,2.5f);
        
        //yield return new WaitForSeconds(2f);
        
        var controller = RTScene.GetComponent<CutSceneController_100003>();
        
        controller._director.Play();
        controller.start = true;
        
        yield return null;
        
        Destroy(loadingScreen);

        yield return new WaitForSeconds(7f);
        
        rawImg.DOColor(Color.black, 1f);

        yield return new WaitForSeconds(1);
        
        ActiveStartScreen(true);
        rawImg.DOColor(Color.clear, 1f).OnComplete(() =>
        {
            rawImg.gameObject.SetActive(false);
            rawImg.color = Color.white;
            //ActiveStartScreen();
        });

        yield return new WaitForSeconds(1.5f);
        
        SetCharacterUIAlpha(1);
        UIElements.transform.Find("StartScreen").GetComponent<UI_StartScreen>().FadeOut();
        
        maxEnemyCount = 3;
        BattleStageManager.Instance.SetMaxEnemy(3);
        
        var enemy1 = SpawnEnemyPrefab(enemyList[0], new Vector3(0, 2, 0)).GetComponent<StatusManager>();
        var enemy2 = SpawnEnemyPrefab(enemyList[0], new Vector3(2, 2, 0)).GetComponent<StatusManager>();

        enemy1.maxBaseHP = 22000;
        enemy1.maxHP = 22000;
        enemy2.maxBaseHP = 22000;
        enemy2.maxHP = 22000;
        
        BattleSceneUIManager.Instance.ReplacePauseMenu(SpawnNewPauseMenu());

        yield return new WaitForSeconds(2f);
        
        GlobalController.Instance.StartGame();
        
        //todo: 玩家回复控制权
        PI_player.EnableAndIdle();

        yield return null;
        
        //初始化BUFF
        var buff = ST_Player.GetExactConditionsOfType
            ((int)BasicCalculation.BattleCondition.LockedSigil,  103301);
        buff[0].SetDuration(-1);
        var healUpBuff = new TimerBuff((int)BasicCalculation.BattleCondition.RecoveryBuff,
            50, -1, 100);
        healUpBuff.dispellable = false;
        ST_Player.ObtainTimerBuff(healUpBuff);
        
        
        
        OpenTutorialHintFirstPage();
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        SetSkillIconAlpha(1,1);
        PI_player.keySkill1 = GlobalController.keySkill1;
        GlobalController.gamepadMap.FindAction("Skill1").Enable();
        

        yield return new WaitUntil(() => currentEnemyCount <= 1);
        
        borders[0].SetActive(false);
        MinimapCameraFollower.Instance.enabled = true;
        BattleStageManager.Instance.SetCameraRightBorder(30);
        BattleStageManager.Instance.SetCameraTopBorder(24);
        BattleStageManager.Instance.RefreshCameraBorder();
        
        yield return new WaitUntil(()=>signals[0]);
        
        borders[1].SetActive(true);
        
        BattleStageManager.Instance.SetCameraRightBorder(5);
        BattleStageManager.Instance.SetCameraTopBorder(27);
        BattleStageManager.Instance.RefreshCameraBorder();

        maxEnemyCount = 11;
        BattleStageManager.Instance.SetMaxEnemy(11);
        
        //todo: 弹出菜单，提示玩家被困在了这里，需要在这里坚持一定时间
        
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);

        var AC_player = GO_player.GetComponent<ActorController_c033>();

        //yield return new WaitUntil(() => AC_player.mode == 2);
        yield return new WaitForSeconds(6.5f);

        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        
        SetSkillIconAlpha(2,1);
        PI_player.keySkill2 = GlobalController.keySkill2;
        GlobalController.gamepadMap.FindAction("Skill2").Enable();
        
        //todo: 实例化一个倒计时UI

        var countDownUIRequest =
            Resources.LoadAsync<GameObject>("UI/InBattle/BattleHint/ProcessBar");

        yield return countDownUIRequest;
        
        var countDownUIInstance = 
            Instantiate(countDownUIRequest.asset as GameObject, UIElements.transform);

        var process = countDownUIInstance.GetComponent<UI_ProcessBarGeneral>();

        process.onCountdownEnd = (go) => { Destroy(go); };
        process.countdownTime = 60;
        process.fadeTweenTime = 1;
        process.fadeOut = true;
        process.fadeIn = true;
        countDownUIInstance.SetActive(true);

        yield return new WaitForSeconds(1);
        
        InvokeRepeating("SpawnTrapArrow",5,15);
        InvokeRepeating("SpawnTrapArrowHint",3.9f,15);

        
        Tween tween = null;
        tween = DOVirtual.DelayedCall(5, () =>
        {
            if(BattleStageManager.Instance.currentEnemyInLayerDeadAlive < 4)
                SummonEnemyGroup();
            tween.Restart();
        }, false);
        
        
        yield return new WaitForSeconds(60);

        CancelInvoke("SpawnTrapArrow");
        CancelInvoke("SpawnTrapArrowHint");
        tween.Kill();
        
        yield return new WaitUntil(()=>BattleStageManager.Instance.currentEnemyInLayerDeadAlive == 0);
        
        StageCameraController.SwitchMainCameraFollowObject(borders[1]);
        
        yield return new WaitForSeconds(1);
        
        borders[1].GetComponent<Animator>().Play("extro");
        borders[2].SetActive(false);
        
        yield return new WaitForSeconds(1.5f);
        
        StageCameraController.SwitchMainCameraFollowObject(GO_player);
        
        borders[1].SetActive(false);
        
        
        BattleStageManager.Instance.SetCameraRightBorder(30);
        BattleStageManager.Instance.SetCameraTopBorder(34);
        BattleStageManager.Instance.RefreshCameraBorder();
        BattleStageManager.Instance.RefreshMapInfo();
        
        yield return new WaitUntil(()=>signals[1]);
        
        borders[2].SetActive(true);
        ST_Player.SpeedUp(40,-1,true);
        
        
        
        MinimapCameraFollower.Instance.SetSize(12);
        StageCameraController.Instance.DoViewTween(10);
        BattleStageManager.Instance.SetRightBorder(88);
        BattleStageManager.Instance.SetCameraRightBorder(88);
        BattleStageManager.Instance.RefreshCameraBorder();
        BattleStageManager.Instance.RefreshMapInfo();
        
        SummonEnemyGuard(new Vector2(-10, 27),-1);
        SummonEnemyGuard(new Vector2(50, 0),-1);
        SummonEnemyGuard(new Vector2(60, 30),1);

        yield return new WaitForSeconds(0.5f);
        
        PI_player.DisableAndIdle();
        StageCameraController.SwitchMainCameraFollowObject(doors[^1]);
        
        yield return new WaitForSeconds(1.5f);
        
        StageCameraController.SwitchMainCameraFollowObject(switches[0]);
        
        yield return new WaitForSeconds(2f);

        PI_player.enabled = true;
        //todo: 弹出攻略菜单
        StageCameraController.SwitchMainCameraFollowObject(GO_player);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        
        yield return new WaitUntil(()=>signals[2]);
        
        StageCameraController.SwitchMainCameraFollowObject(GO_player);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);

        PI_player.keySkill3 = GlobalController.keySkill3;
        GlobalController.gamepadMap.FindAction("Skill3").Enable();
        SetSkillIconAlpha(3,1);
        ST_Player.SetSPChargeRate(2,800);
        
        yield return new WaitUntil(()=>signals[3]);
        
        //最后一阶段
        PlayStoryVoiceWithDialog(0,1033,storyVoiceList[0]);
        PI_player.DisableAndIdle();
        StageCameraController.Instance.DoViewTween(8,1);
        yield return new WaitForSeconds(4f);
        
        
        BattleStageManager.Instance.SetLeftBorder(54);
        BattleStageManager.Instance.SetRightBorder(94);
        BattleStageManager.Instance.SetCameraLeftBorder(54);
        BattleStageManager.Instance.SetCameraRightBorder(94);
        BattleStageManager.Instance.RefreshCameraBorder();
        BattleStageManager.Instance.RefreshMapInfo();
        PI_player.DisableAndIdle();
        foreach (var Switch in switches)
        {
            Switch.SetActive(false);
        }

        foreach (var door in doors)
        {
            door.SetActive(false);
        }

        BattleStageManager.Instance.SetMaxEnemy(3);
        var minion = SpawnEnemyPrefab(enemyList[0],
            new Vector3(53, PI_player.transform.position.y), true);
        minion.GetComponent<StatusManager>().maxBaseHP = 35000;
        var boss = SpawnEnemyPrefab(enemyList[2],
            new Vector3(55, PI_player.transform.position.y), false);
        var BHV_minion = minion.GetComponent<DragaliaEnemyBehavior>();
        var ST_BOSS = boss.GetComponent<StatusManager>();
        var BHV_boss = boss.GetComponent<DragaliaEnemyBehavior>();
        
        var EC_minion = minion.GetComponent<EnemyControllerHumanoid>();
        var EC_boss = boss.GetComponent<EnemyControllerHumanoid>();
        ST_Player.SetSPChargeRate(2,400);
        
        BHV_minion.playerAlive = false;
        BHV_boss.playerAlive = false;
        
        //todo: 播放语音
        PlayStoryVoiceWithDialog(1,9004,storyVoiceList[1]);

        EC_minion.SetFaceDir(1);
        EC_boss.SetFaceDir(1);
        EC_boss.canDeath = false;
        
        EC_boss.SetMove(1);
        EC_minion.SetMove(1);
        
        yield return new WaitForSeconds(2);
        
        
        StageCameraController.SwitchMainCameraFollowObject(EC_boss.gameObject);
        EC_boss.SetMove(0);
        
        yield return new WaitForSeconds(1);
        
        EC_minion.SetMove(0);
        
        yield return new WaitForSeconds(2);
        StageCameraController.SwitchMainCameraFollowObject(GO_player);
        
        
        PI_player.enabled = true;
        BHV_boss.playerAlive = true;
        BHV_minion.playerAlive = true;
        UI_MultiBossManager.Instance.GetComponentInChildren<UI_BossStatus>().SetBoss(boss);
        print(BattleStageManager.Instance.mapBorderL);
        print(BattleStageManager.Instance.mapBorderR);

        yield return null;
        
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        
        PlayStoryVoiceWithDialog(2,9004,storyVoiceList[2]);

        yield return new WaitUntil(()=>BHV_boss.GetState().Item1 > 0);

        PlayStoryVoiceWithDialog(5,1036,storyVoiceList[5]);
        PlayStoryVoiceWithDialog(6,1036,storyVoiceList[6]);
        
        yield return new WaitUntil(()=>ST_BOSS.currentHp <= 0);

        BHV_boss.playerAlive = false;
        BHV_boss.enabled = false;
        yield return null;
        EC_boss.anim.Play("idle");
        EC_boss.TurnMove(GO_player);
        UI_MultiBossManager.Instance.GetComponentInChildren<UI_BossStatus>().visible = false;
        
        GlobalController.Instance.EndGame();
        StageCameraController.SwitchMainCameraFollowObject(boss);
        StageCameraController.SetMainCameraSize(6);
        Time.timeScale = .5f;
        if (EC_minion != null)
        {
            EC_minion.GetComponent<StatusManager>().currentHp = 0;
        }

        yield return new WaitForSeconds(0.8f);
        
        SetCharacterUIAlpha(0);
        Time.timeScale = 1;
        StageCameraController.SetMainCameraSize(8);
        StageCameraController.SwitchMainCameraFollowObject(GO_player);
        AC_player.TurnMove(boss);
        PI_player.DisableAndIdle();
        
        PlayStoryVoiceWithDialog(3,1033,storyVoiceList[3]);
        
        yield return new WaitForSeconds(4f);
        
        PlayStoryVoiceWithDialog(4,9004,storyVoiceList[4]);
        
        yield return new WaitForSeconds(4f);
        
        PlayStoryVoiceWithDialog(7,1033,storyVoiceList[7]);
        
        yield return new WaitForSeconds(5f);
        
        PlayStoryVoiceWithDialog(8,1033,storyVoiceList[8]);
        
        yield return new WaitForSeconds(4f);
        
        PlayStoryVoiceWithDialog(9,1033,storyVoiceList[9]);
        
        yield return new WaitForSeconds(6f);
        
        PlayStoryVoiceWithDialog(10,9004,storyVoiceList[10]);
        
        yield return new WaitForSeconds(0.5f);

        BattleStageManager.Instance.SetRightBorder(98);
        BattleStageManager.Instance.SetLeftBorder(52);
        //PI_player.DisableAndIdle();
        PI_player.isMove = 1;
        AC_player.SetFaceDir(1);
        AC_player.anim.SetFloat("forward",1f);
        
        AC_player.anim.Play("walk");
        
        var RunTime = (98-PI_player.transform.position.x)/9f;
        
        AC_player.rigid.DOMoveX(98,
            (98-PI_player.transform.position.x)/9f).SetEase(Ease.Linear);
        ST_Player.OnBuffExpiredEventDelegate = null;
        ST_Player.OnBuffDispelledEventDelegate = null;

        DOVirtual.DelayedCall(RunTime, ()=>AC_player.SetGravityScale(0), false);

        yield return null;

        List<DragaliaEnemyBehavior> bhvs = new();
        List<EnemyControllerHumanoid> ecs = new();

        for(int i = 0; i < 6; i ++)
        {
            var e = SpawnEnemyPrefab(enemyList[0],
                new Vector3(53,PI_player.transform.position.y),true);
            bhvs.Add(e.GetComponent<DragaliaEnemyBehavior>());
            ecs.Add(e.GetComponent<EnemyControllerHumanoid>());
            bhvs[i].playerAlive = false;
            ecs[i].SetMove(1);
            ecs[i].SetFaceDir(1);
            yield return new WaitForSeconds(0.65f + Random.Range(-0.1f,0.1f));
        }
        
        EC_boss.SetFaceDir(1);
        EC_boss.SetMove(1);

        yield return new WaitUntil(() => ecs[^1].transform.position.x > 94);
        
        PI_player.isMove = 0;

        foreach (var ec in ecs)
        {
            Destroy((ec.gameObject));
        }

        BattleStageManager.Instance.SetGameClearedSimple();

    }

    private void SpawnTrapArrowHint()
    {
        Vector3 pos = new Vector3(-13.5f, 11);
        
        var trapFX = SpawnFXPrefab(prefabList[1], pos,1);
        
    }

    private void SpawnTrapArrow()
    {
        Vector3 pos = new Vector3(-13.5f, 11);
        
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            Vector3.zero, Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
        
        var trapFX = SpawnFXPrefab(prefabList[0], pos,1);
        
        trapFX.transform.SetParent(container.transform);

        var atk = trapFX.GetComponent<AttackFromEnemy>();

        atk.enemySource = gameObject;
        
        
        
        var poison = new TimerBuff((int)(BasicCalculation.BattleCondition.Poison),
            200, -1, 1, -1);
        
        atk.AddWithConditionAll(poison,100);
        
        
    }

    private void SummonEnemyGroup()
    {
        var enemy1 = SpawnEnemyPrefab
            (enemyList[0], new Vector3(-28f, 11),true).GetComponent<StatusManager>();
        enemy1.maxBaseHP = 65000;
    }

    private void SummonEnemyGuard(Vector2 position, int dir)
    {
        var enemy1 = SpawnEnemyPrefab
            (enemyList[1], position,true).GetComponent<StatusManager>();
        enemy1.maxBaseHP = 80000;
        enemy1.baseAtk = 1450;
        var actor = enemy1.GetComponent<ActorBase>();
        actor.SetFaceDir(dir);

        var awaker = enemy1.GetComponent<EnemyAwakeTrigger>();
        var behavior = enemy1.GetComponent<DragaliaEnemyBehavior>();
        enemy1.baseDef = 4;

        awaker.SetTriggerAction(()=>
        {
            enemy1.baseDef = 8;
            print("SetDef");
        });
        
        
        ActorBase.OnHurt onHurt = null;
        
        onHurt = () =>
        {
            actor.OnAttackInterrupt -= onHurt;
            enemy1.baseDef = 8;
            awaker.DisableCollider();
            actor.TurnMove(behavior.targetPlayer);
        };

        foreach (var door in doors)
        {
            Physics2D.IgnoreCollision((actor as EnemyControllerHumanoid)._groundSensor.box,
                door.GetComponent<Collider2D>());
        }
        

        actor.OnAttackInterrupt += onHurt;

    }


}
