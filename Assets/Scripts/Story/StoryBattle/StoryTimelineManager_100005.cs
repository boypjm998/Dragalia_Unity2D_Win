using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class StoryTimelineManager_100005 : StoryBattleTimelineManager
{

    [SerializeField] private bool debug = false;
    
    public override void StartQuest()
    {
        UI_DialogDisplayer.Instance.LoadBasicStoryInfo("100005");

        UIElements.transform.Find("CharacterInfo").gameObject.SetActive(true);
        GlobalController.Instance.StartGame();

        if (currentCutSceneCoroutine == null)
            currentCutSceneCoroutine = StartCoroutine(CutScene_01());
    }

    protected void Start()
    {
        
    }

    private void Update()
    {
        if (debug)
        {
            debug = false;
            StartQuest();
        }
    }

    public override void InitializeScene()
    {
        var playerBundlePath = BasicCalculation.ConvertID($"player/player_c", GlobalController.currentCharacterID);
        var playerBundle = GlobalController.Instance.GetBundle(playerBundlePath);

        var uiBundlePath = BasicCalculation.ConvertID($"ui/ui_c", GlobalController.currentCharacterID);
        var uiBundle = GlobalController.Instance.GetBundle(uiBundlePath);
        print(uiBundle);

        UIElements = GameObject.Find("UI");
        var UICharaInfoAsset = uiBundle.LoadAsset<GameObject>("CharacterInfo");
        var UICharaInfoClone = Instantiate(UICharaInfoAsset, UIElements.transform);
        UICharaInfoClone.name = "CharacterInfo";
        //UICharaInfoClone.SetActive(false);
        UIElements.transform.Find("Minimap").gameObject.SetActive(true);

        _playerGO =
            Instantiate(playerBundle.LoadAsset<GameObject>("PlayerHandle"),
                new Vector3(-4f, -1), Quaternion.identity,
                BattleStageManager.Instance.PlayerLayer.transform);
        BattleStageManager.Instance.InitPlayer(_playerGO);
        _playerGO.GetComponent<PlayerStatusManager>().remainReviveTimes = BattleStageManager.Instance.maxReviveTime;
        _playerInput = _playerGO.GetComponent<PlayerInput>();
        _playerInput.enabled = false;
        _playerGO.name = "PlayerHandle";

        DOVirtual.DelayedCall(0.01f, () => UICharaInfoClone.SetActive(false));

        //加载敌人
        if (_bossGO == null)
            _bossGO = SpawnEnemyPrefab(enemyList[0], new Vector2(4f, -1), false);

        _bossGO.GetComponent<ActorBase>().SetFaceDir(-1);
        UI_MultiBossManager.Instance.GetComponentInChildren<UI_BossStatus>().SetBoss(_bossGO);

        BattleStageManager.Instance.loseControllTime = 1;

    }

    private IEnumerator CutScene_01()
    {
        _playerInput.EnableAndIdle();
        
        // var BHV_Boss = _bossGO.GetComponent<EnemyBehaviorManager>();
        // var AC_Boss = _bossGO.GetComponent<EnemyControllerHumanoid>();
        // var STAT_Boss = _bossGO.GetComponent<StatusManager>();
        // AC_Boss.SetHitSensor(false);
        // AC_Boss.TurnMove(_playerGO);
        // BHV_Boss.playerAlive = false;

        var charaID = GlobalController.currentCharacterID;

        yield return new WaitForSeconds(1);

        if (charaID == 33 || charaID == 18)
        {
            _playerInput.GetComponent<StatusManager>()
                .GetConditionOfTypeWithMaxEffect
                    ((int)BasicCalculation.BattleCondition.LockedSigil).lastTime = 0.1f;
        }

        yield return null;

        currentCutSceneCoroutine = null;
        
    }



}
