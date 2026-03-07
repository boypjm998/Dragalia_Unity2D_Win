using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class StoryTimelineManager_100006 : StoryBattleTimelineManager 
{
    int _orbCount = 0;
    public override void StartQuest()
    {
        UI_DialogDisplayer.Instance.LoadBasicStoryInfo("100006");

        UIElements.transform.Find("CharacterInfo").gameObject.SetActive(true);
        GlobalController.Instance.StartGame();

        if (currentCutSceneCoroutine == null)
            currentCutSceneCoroutine = StartCoroutine(CutScene_01());
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
                new Vector3(-4f, BattleStageManager.Instance.mapBorderB + 1.2f), Quaternion.identity,
                BattleStageManager.Instance.PlayerLayer.transform);
        BattleStageManager.Instance.InitPlayer(_playerGO);
        _playerGO.GetComponent<PlayerStatusManager>().remainReviveTimes = BattleStageManager.Instance.maxReviveTime;
        _playerInput = _playerGO.GetComponent<PlayerInput>();
        _playerInput.enabled = false;
        _playerGO.name = "PlayerHandle";

        DOVirtual.DelayedCall(0.01f, () => UICharaInfoClone.SetActive(false));

        //加载敌人
        if (_bossGO == null)
            _bossGO = SpawnEnemyPrefab(enemyList[0], new Vector2(0f, 0), false);

        //_bossGO.GetComponent<ActorBase>().SetFaceDir(-1);
        UI_MultiBossManager.Instance.GetComponentInChildren<UI_BossStatus>().SetBoss(_bossGO);

        BattleStageManager.Instance.loseControllTime = 1;

    }
    
    private IEnumerator CutScene_01()
    {
        _playerInput.EnableAndIdle();
        
        var BHV_Boss = _bossGO.GetComponent<EnemyBehaviorManager>();
        var AC_Boss = _bossGO.GetComponent<EnemyController>();
        var STAT_Boss = _bossGO.GetComponent<StatusManager>();
        AC_Boss.SetSummoned(true);
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
        
        BattleSceneUIManager.Instance.ReplacePauseMenu(SpawnNewPauseMenu());
        OpenTutorialHintFirstPage();

        yield return new WaitForSeconds(4);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        _orbCount = 0;
        BattleStageManager.Instance.OnEnemyEliminated += OrbDefeat;
        
        SummonGabriel();
        int summonCount = 0;

        while (STAT_Boss.currentHp > 0)
        {
            if (_orbCount >= 2)
            {
                _orbCount = 0;
                if (summonCount % 3 == 0)
                {
                    SummonRamiel();
                }else if (summonCount % 3 == 1)
                {
                    SummonUriel();
                }
                else
                {
                    SummonRaphael();
                }
                summonCount++;
            }
            
            yield return null;
        }
        
        //StageCameraController.SwitchMainCameraFollowObject(_bossGO);
        StageCameraController.SwitchOverallCamera();
        GlobalController.Instance.EndGame();
        
        yield return new WaitForSeconds(.5f);
        
        PlayStoryVoiceWithDialog(0, 2023, storyVoiceList[0]);

        yield return new WaitForSeconds(storyVoiceList[0].length);
        
        PlayStoryVoiceWithDialog(1, 1031, storyVoiceList[1]);
        
        SpawnFXPrefab(prefabList[2], _bossGO.transform.position);
        yield return new WaitForSeconds(1);
        Destroy(_bossGO);
        
        yield return new WaitForSeconds(storyVoiceList[1].length - 1);
        
        PlayStoryVoiceWithDialog(2, 1031, storyVoiceList[2]);
        CineMachineOperator.Instance.CamaraShake(5,10);
        (BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background1") as SpriteRenderer).
            DOColor(new Color(.4f,.4f,.4f,1), 1f);
        
        yield return new WaitForSeconds(storyVoiceList[2].length);
        
        BattleStageManager.Instance.SetGameClearedSimple();
        CineMachineOperator.Instance.StopCameraShake();

        currentCutSceneCoroutine = null;
        
    }

    private void OrbDefeat(int instanceID)
    {
        _orbCount++;
    }

    private void SummonGabriel()
    {
        var fx = SpawnFXPrefab(prefabList[1], _playerGO.transform.position);
        
        UI_BattleInfoCaster.Instance?.PrintSkillName("STY_Action15",2);

        var buff1 = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff, 50, -1, 1,
            201101);
        var buff2 = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff, 20, -1, 1,
            201102);
        
        buff1.dispellable = false;
        buff2.dispellable = false;

        _playerGO.GetComponent<StatusManager>().ObtainTimerBuff(buff1, false);
        _playerGO.GetComponent<StatusManager>().ObtainTimerBuff(buff2, false);

    }

    private void SummonRamiel()
    {
        var posX = _playerGO.transform.position.x < 0 ?
            Mathf.Clamp(_playerGO.transform.position.x - 2, -15, -5) :
            Mathf.Clamp(_playerGO.transform.position.x + 2, 5, 15);

        var posY = _playerGO.RaycastedPosition().y;
        
        UI_BattleInfoCaster.Instance?.PrintSkillName("STY_Action13",2);
        
        var fx = SpawnFXPrefab(prefabList[0], new Vector3(posX,BattleStageManager.Instance.mapBorderB),
            1);
        var go_Supporter = Instantiate(npcList[0], new Vector3(posX,posY),
            Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
        var ac = go_Supporter.GetComponent<StandardCharacterController>();
        ac.TurnMove(_bossGO);
        
        var controller = go_Supporter.GetComponent<SupportSkillNPC>();
        controller.GetSkillPrefab(0).onAttackCreated += (fx) =>
        {
            fx.transform.position = controller.transform.position + new Vector3(ac.facedir * 3, 3);
            var defenseDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefDebuff,
                20, 30, 1, 201201);

            fx.GetComponent<AttackFromPlayer>().AddWithConditionAll(defenseDebuff, 110);
        };


    }

    private void SummonUriel()
    {
        var posX = _playerGO.transform.position.x < 0 ?
            Mathf.Clamp(_playerGO.transform.position.x - 2, -15, -5) :
            Mathf.Clamp(_playerGO.transform.position.x + 2, 5, 15);

        var posY = _playerGO.RaycastedPosition().y;
        
        UI_BattleInfoCaster.Instance?.PrintSkillName("STY_Action12",2);
        
        var fx = SpawnFXPrefab(prefabList[0], new Vector3(posX,BattleStageManager.Instance.mapBorderB),
            1);
        var go_Supporter = Instantiate(npcList[2], new Vector3(posX,posY),
            Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
        var ac = go_Supporter.GetComponent<StandardCharacterController>();
        ac.TurnMove(_bossGO);
        
        var controller = go_Supporter.GetComponent<SupportSkillNPC>();
        controller.GetSkillPrefab(0).onAttackCreated += (fx) =>
        {
            var scor = new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
                80, 21, 1, 201401);

            fx.GetComponent<AttackFromPlayer>().AddWithConditionAll(scor, 110);

            fx.transform.position = _bossGO.RaycastedPosition();
        };
        controller.AppendAnimation(1.5f, "charge_exit");
        
    }
    
    private void SummonRaphael()
    {
        var posX = _playerGO.transform.position.x < 0 ?
            Mathf.Clamp(_playerGO.transform.position.x - 2, -15, -5) :
            Mathf.Clamp(_playerGO.transform.position.x + 2, 5, 15);

        var posY = _playerGO.RaycastedPosition().y;
        
        UI_BattleInfoCaster.Instance?.PrintSkillName("STY_Action14",2);
        
        var fx = SpawnFXPrefab(prefabList[0], new Vector3(posX,BattleStageManager.Instance.mapBorderB),
            1);
        var go_Supporter = Instantiate(npcList[1], new Vector3(posX,posY),
            Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
        var ac = go_Supporter.GetComponent<StandardCharacterController>();
        ac.TurnMove(_bossGO);
        
        var controller = go_Supporter.GetComponent<SupportSkillNPC>();
        
        controller.AppendAnimation(1.2f, "charge_exit");

        DOVirtual.DelayedCall(2.5f, () =>
        {
            controller.transform.DOMoveX
                (controller.transform.position.x + ac.facedir * 15, 0.3f).SetEase(Ease.OutSine);
        },false);
    }

    
}
