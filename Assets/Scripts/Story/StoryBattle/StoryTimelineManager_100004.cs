using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class StoryTimelineManager_100004 : StoryBattleTimelineManager
{
    private GameObject GO_Player;
    [SerializeField] private PlayerInput PI_Player;
    [SerializeField] private GameObject GO_Boss;
    public bool debug;
    private float groundPosition;
    private StatusManager supportStatusManager;
    
    
    
    public override void StartQuest()
    {
        UI_DialogDisplayer.Instance.LoadBasicStoryInfo("100004");
        
        UIElements.transform.Find("CharacterInfo").gameObject.SetActive(true);
        GlobalController.Instance.StartGame();

        if(currentCutSceneCoroutine == null)
            currentCutSceneCoroutine = StartCoroutine(CutScene_01());
        
        
    }

    protected void Start()
    {
        supportStatusManager = GetComponent<StatusManager>();
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
        var playerBundlePath = BasicCalculation.ConvertID($"player/player_c",GlobalController.currentCharacterID);
        var playerBundle = GlobalController.Instance.GetBundle(playerBundlePath);
        
        var uiBundlePath = BasicCalculation.ConvertID($"ui/ui_c",GlobalController.currentCharacterID);
        var uiBundle = GlobalController.Instance.GetBundle(uiBundlePath);
        print(uiBundle);
        
        UIElements = GameObject.Find("UI");
        var UICharaInfoAsset = uiBundle.LoadAsset<GameObject>("CharacterInfo");
        var UICharaInfoClone = Instantiate(UICharaInfoAsset, UIElements.transform);
        UICharaInfoClone.name = "CharacterInfo";
        //UICharaInfoClone.SetActive(false);
        UIElements.transform.Find("Minimap").gameObject.SetActive(true);

        GO_Player = 
            Instantiate(playerBundle.LoadAsset<GameObject>("PlayerHandle"),
                new Vector3(-2.5f,-1),Quaternion.identity,
                BattleStageManager.Instance.PlayerLayer.transform);
        BattleStageManager.Instance.InitPlayer(GO_Player);
        GO_Player.GetComponent<PlayerStatusManager>().remainReviveTimes = BattleStageManager.Instance.maxReviveTime;
        PI_Player = GO_Player.GetComponent<PlayerInput>();
        PI_Player.enabled = false;
        GO_Player.name = "PlayerHandle";

        DOVirtual.DelayedCall(0.01f, () => UICharaInfoClone.SetActive(false));
        
        //加载敌人
        if(GO_Boss == null)
            GO_Boss = SpawnEnemyPrefab(enemyList[2], new Vector2(2.5f,-1), false);
        
        GO_Boss.GetComponent<ActorBase>().SetFaceDir(-1);
        UI_MultiBossManager.Instance.GetComponentInChildren<UI_BossStatus>().SetBoss(GO_Boss);
        
        BattleStageManager.Instance.loseControllTime = 1;
        
    }

    private IEnumerator CutScene_01()
    {
        //记得注释掉
        //UIElements = GameObject.Find("UI");
        
        

        //使徒角色直接圣痕解放
        
        
        
        PI_Player.EnableAndIdle();
        groundPosition = BattleStageManager.Instance.mapBorderB + 1.3f;
        var BHV_Boss = GO_Boss.GetComponent<EnemyBehaviorManager>();
        var AC_Boss = GO_Boss.GetComponent<EnemyControllerHumanoid>();
        var STAT_Boss = GO_Boss.GetComponent<StatusManager>();
        AC_Boss.SetHitSensor(false);
        AC_Boss.TurnMove(PI_Player.gameObject);
        BHV_Boss.playerAlive = false;

        AbilityClock clockPinon = new AbilityClock(22);
        AbilityClock clockRyz = new AbilityClock(20);
        AbilityClock clockNev = new AbilityClock(25);
        AbilityClock clockFar = new AbilityClock(12);

        yield return new WaitForSeconds(1);
        
        var charaID = GlobalController.currentCharacterID;
        if (charaID == 33 || charaID == 18)
        {
            PI_Player.GetComponent<StatusManager>()
                .GetConditionOfTypeWithMaxEffect
                    ((int)BasicCalculation.BattleCondition.LockedSigil).lastTime = 0.1f;
        }
        
        BattleSceneUIManager.Instance.ReplacePauseMenu(SpawnNewPauseMenu());
        OpenTutorialHintFirstPage();
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        OpenTutorialHintPauseMenuAndTurnToNewestPage();
        yield return new WaitUntil(() => BattleStageManager.Instance.isGamePaused == false);
        
        PlayStoryVoiceWithDialog(0,9004,storyVoiceList[0]);

        yield return new WaitForSeconds(5);
        
        SpawnEnemyPrefab(enemyList[1], new Vector2(-18,-1), true).
            GetComponent<StatusManager>().maxBaseHP = 22000;
        SpawnEnemyPrefab(enemyList[1], new Vector2(-12,-1), true).
            GetComponent<StatusManager>().maxBaseHP = 22000;
        SpawnEnemyPrefab(enemyList[1], new Vector2(-6,-1), true).
            GetComponent<StatusManager>().maxBaseHP = 22000;
        SpawnEnemyPrefab(enemyList[1], new Vector2(0,-1), true).
            GetComponent<StatusManager>().maxBaseHP = 22000;
        SpawnEnemyPrefab(enemyList[1], new Vector2(18,-1), true).
            GetComponent<StatusManager>().maxBaseHP = 22000;
        SpawnEnemyPrefab(enemyList[1], new Vector2(12,-1), true).
            GetComponent<StatusManager>().maxBaseHP = 22000;
        SpawnEnemyPrefab(enemyList[1], new Vector2(6,-1), true).
            GetComponent<StatusManager>().maxBaseHP = 22000;
        
        AC_Boss.SetHitSensor(true);
        
        yield return new WaitForSeconds(1);
        
        SummonRyszarda(true);
        clockRyz.StartTick();
        
        yield return new WaitForSeconds(2);
        
        BHV_Boss.playerAlive = true;
        yield return null;

        while (STAT_Boss.currentHp >= STAT_Boss.maxBaseHP * 0.5f)
        {
            var currentActionName = BHV_Boss.GetCurrentActionName();

            if (clockPinon.Available && currentActionName == "lan_hi_5")
            {
                if (GlobalController.currentCharacterID == 18)
                {
                    yield return new WaitForSeconds(0.75f);
                    SummonSandalphonFreeze(new Vector2(GO_Boss.transform.position.x + Random.Range(-3,3),groundPosition));
                }
                else
                {
                    SummonPinon(new Vector2(GO_Boss.transform.position.x + Random.Range(-3,3),groundPosition));
                }
                clockPinon.StartTick();
            }else if (clockNev.Available)
            {
                SummonNevin(CalculateReginaPosX());
                clockNev.StartTick();
            }else if (clockRyz.Available && currentActionName.StartsWith("summon"))
            {
                SummonRyszarda();
                clockRyz.StartTick();
            }else if (clockFar.Available)
            {
                SummonFaris();
                clockFar.StartTick();
            }
            else
            {
                yield return new WaitForSeconds(1);
                continue;
            }

            yield return new WaitForSeconds(7);

        }
        
        PlayStoryVoiceWithDialog(1,1033,storyVoiceList[6]);
        
        yield return new WaitForSeconds(9);
        
        PlayStoryVoiceWithDialog(2,1033,storyVoiceList[7]);
        
        yield return new WaitForSeconds(7);
        
        PlayStoryVoiceWithDialog(3,1033,storyVoiceList[8]);
        
        yield return new WaitForSeconds(3);
        
        if (GlobalController.currentCharacterID == 18)
        {
            yield return new WaitForSeconds(0.75f);
            SummonSandalphonFreeze(new Vector2(GO_Boss.transform.position.x + Random.Range(-3,3),groundPosition));
        }
        else
        {
            SummonPinon(new Vector2(GO_Boss.transform.position.x + Random.Range(-3,3),groundPosition));
        }

        while (STAT_Boss.currentHp > 0)
        {
            var pos = CalculateReginaPosX();
        
            yield return new WaitForSeconds(2.5f);
        
            SummonNevin(pos);
            if(STAT_Boss.currentHp <= 0)
                break;

            yield return new WaitForSeconds(3);
        
            if (GlobalController.currentCharacterID == 33)
            {
                yield return new WaitForSeconds(0.75f);
                SummonSandalphonMulti(new Vector2(GO_Boss.transform.position.x + Random.Range(-1,1),groundPosition));
            }
            else
            {
                SummonRegina(pos);
            }
            
            if(STAT_Boss.currentHp <= 0)
                break;
            
            yield return new WaitForSeconds(8);
            
            SummonRyszarda();
            if(STAT_Boss.currentHp <= 0)
                break;
        
            yield return new WaitForSeconds(8);
        
            SummonFaris();
            if(STAT_Boss.currentHp <= 0)
                break;
            
            yield return new WaitForSeconds(7);
        }
        
        PlayStoryVoiceWithDialog(4,9004,storyVoiceList[9]);

        yield return new WaitForSeconds(4);
        
        
        
        currentCutSceneCoroutine = null;
    }

    private void SummonPinon(Vector2 pos)
    {
        pos.x = Mathf.Clamp(pos.x,BattleStageManager.Instance.mapBorderL + 2,
            BattleStageManager.Instance.mapBorderR - 2);
        
        UI_BattleInfoCaster.Instance?.PrintSkillName("STY_Action01",2);
        var fx = SpawnFXPrefab(prefabList[0], pos, 1);
        var go_Pinon = Instantiate(npcList[0], pos,
            Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
        var controller = go_Pinon.GetComponent<SupportSkillNPC>();
        
        PlayStoryVoiceWithoutDialog(storyVoiceList[1]);


        controller.GetSkillPrefab(0).onAttackCreated += (atk) =>
        {
            CineMachineOperator.Instance.CamaraShake(12,0.1f);
            var freeze = new TimerBuff((int)BasicCalculation.BattleCondition.Freeze,
                -1, 8, 1, -1);
            atk.AddWithConditionAll(freeze, 200);
        };
        
        
        
    }

    private void SummonFaris()
    {
        UI_BattleInfoCaster.Instance?.PrintSkillName("STY_Action03",2);
        
        var bossPos = GO_Boss.transform.position.x;
        var playerPos = PI_Player.transform.position.x;
        var leftLimit = BattleStageManager.Instance.mapBorderL;
        var rightLimit = BattleStageManager.Instance.mapBorderR;

        float pos = 0;
        int dir = 1;

        if (playerPos < bossPos)
        {
            pos = bossPos - 12;
            dir = 1;
        }
        else
        {
            pos = bossPos + 12;
            dir = -1;
        }

        if (pos < leftLimit)
        {
            pos = bossPos + 12;
            dir = -1;
        }else if (pos > rightLimit)
        {
            pos = bossPos - 12;
            dir = 1;
        }





        var fx = SpawnFXPrefab(prefabList[0], new Vector3(pos,groundPosition),
            1);
        var go_Faris = Instantiate(npcList[1], new Vector3(pos,groundPosition),
            Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
        go_Faris.GetComponent<StandardCharacterController>().SetFaceDir(dir);
        var controller = go_Faris.GetComponent<SupportSkillNPC>();
        controller.GetSkillPrefab(1).onAttackCreated += (atk) =>
        {
            PlayStoryVoiceWithoutDialog(storyVoiceList[2]);
            CineMachineOperator.Instance.CamaraShake(15,0.1f);
            var urielsWrath = new TimerBuff((int)BasicCalculation.BattleCondition.UrielsWrath,
                1, 15, 3, 103401);
            atk.AddWithConditionAll(urielsWrath, 100);
        };
        
    }

    private void SummonRyszarda(bool around = false)
    {
        UI_BattleInfoCaster.Instance?.PrintSkillName("STY_Action04",2);
        
        var bossPos = GO_Boss.transform.position.x;
        var playerPos = PI_Player.transform.position.x;
        var leftLimit = BattleStageManager.Instance.mapBorderL;
        var rightLimit = BattleStageManager.Instance.mapBorderR;

        float pos = 0;
        int dir = 1;

        if (playerPos < bossPos)
        {
            pos = bossPos - 6;
            dir = 1;
        }
        else
        {
            pos = bossPos + 6;
            dir = -1;
        }

        if (pos < leftLimit)
        {
            pos = bossPos + 6;
            dir = -1;
        }else if (pos > rightLimit)
        {
            pos = bossPos - 6;
            dir = 1;
        }

        if (around)
        {
            pos = Mathf.Clamp(playerPos + Random.Range(-3,3),BattleStageManager.Instance.mapBorderL,
                BattleStageManager.Instance.mapBorderR);
            dir = pos > GO_Boss.transform.position.x ? -1 : 1;
        }

        var fx = SpawnFXPrefab(prefabList[0], new Vector3(pos,groundPosition),
            1);
        var go_Supporter = Instantiate(npcList[2], new Vector3(pos,groundPosition),
            Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
        go_Supporter.GetComponent<StandardCharacterController>().SetFaceDir(dir);
        var controller = go_Supporter.GetComponent<SupportSkillNPC>();
        controller.GetSkillPrefab(1).onAttackCreated += (atk) =>
        {
            PlayStoryVoiceWithoutDialog(storyVoiceList[3]);
            var debuff = new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
                42, 21, 100);
            atk.AddWithConditionAll(debuff, 120);
        };
        
    }

    private void SummonNevin(float posX)
    {
        UI_BattleInfoCaster.Instance?.PrintSkillName("STY_Action05",2);
        
        var fx = SpawnFXPrefab(prefabList[0], new Vector3(posX,groundPosition),
            1);
        var go_Supporter = Instantiate(npcList[3], new Vector3(posX,groundPosition),
            Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
        var ac = go_Supporter.GetComponent<StandardCharacterController>();
        if(posX > GO_Boss.transform.position.x)
            ac.SetFaceDir(-1);
        else
            ac.SetFaceDir(1);
        
        PlayStoryVoiceWithoutDialog(storyVoiceList[4]);
        

    }

    private void SummonRegina(float posX)
    {
        
        
        
        UI_BattleInfoCaster.Instance?.PrintSkillName("STY_Action06",2);
        
        PlayStoryVoiceWithoutDialog(storyVoiceList[5]);
        
        var fx = SpawnFXPrefab(prefabList[0], new Vector3(posX,groundPosition),
            1);
        var go_Supporter = Instantiate(npcList[4], new Vector3(posX,groundPosition),
            Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
        var ac = go_Supporter.GetComponent<StandardCharacterController>();
        
        
        var controller = go_Supporter.GetComponent<SupportSkillNPC>();
        
        ac.TurnMove(GO_Boss);
        
        controller.GetSkillPrefab(2).onObjectCreated += (fx) =>
        {

            var tweenerCore = fx.transform.DOMove
                (ac.transform.position + new Vector3(0, 6.5f, 0), 0.4f);

            var generatePos = ac.transform.position + new Vector3(0, 2.5f);

            tweenerCore.OnComplete(() =>
            {
                var proj = this.InstantiateRangedObject(prefabList[1],
                    generatePos, BattleStageManager.Instance.GetNewRangedContainer(false), 1,1,ac);

                //生成1-2的随机数,决定旋转角度
                int random = Random.Range(1, 3);
                proj.transform.GetChild(0).localRotation = Quaternion.Euler(0, random * 30, 0);

                var atk = proj.GetComponent<AttackFromPlayer>();

                //从afflictions里随机选2个不重复的值

                var frostbiteAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,
                    31, 31, 100);
                var shadowblightAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.ShadowBlight,
                    31, 31, 100);
                var flashburnAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
                    31, 31, 100);
                var scorchrendAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
                    31, 31, 100);
                var stormlashAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Stormlash,
                    31, 31, 100);

                var burnAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Burn,
                    72.7f, 22, 100);
                var paralysisAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Paralysis,
                    72.7f, 22, 100);

                var poisonAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Poison,
                    43.6f, 25, 100);

                var afflictions = (new TimerBuff[]
                {
                    burnAffliction,
                    paralysisAffliction,
                    poisonAffliction,
                    scorchrendAffliction,
                    flashburnAffliction,
                    shadowblightAffliction,
                    stormlashAffliction
                }).ToList();


                var afflictionsCopy = new List<TimerBuff>(afflictions);

                var affliction1 = afflictionsCopy[Random.Range(0, afflictionsCopy.Count)];
                afflictionsCopy.Remove(affliction1);
                var affliction2 = afflictionsCopy[Random.Range(0, afflictionsCopy.Count)];

                var chance = 110;


                atk.AddWithConditionAll(new TimerBuff(frostbiteAffliction), chance, 1);
                atk.AddWithConditionAll(new TimerBuff(affliction1), chance, 2);
                atk.AddWithConditionAll(new TimerBuff(affliction2), chance, 3);
            });
        };
        


    }

    private float CalculateReginaPosX()
    {
        float posX = 0;
        var bossPos = GO_Boss.transform.position.x;
        
        if (bossPos > 9)
        {
            posX += (bossPos - 9);
        }else if (bossPos < -9)
        {
            posX += (bossPos + 9);
        }

        posX = Mathf.Clamp(posX, -12, 12);
        return posX;
    }

    private void SummonSandalphonFreeze(Vector2 pos)
    {
        UI_BattleInfoCaster.Instance?.PrintSkillName("STY_Action07",2);
        var fx = SpawnFXPrefab(prefabList[0], pos, 1); 
        
        
        var container = BattleStageManager.Instance.GetNewRangedContainer(false);
        
        var go_Supporter = Instantiate(prefabList[2], pos,
            Quaternion.identity, container.transform);

        var atk = go_Supporter.GetComponent<AttackFromPlayer>();
        var freeze = new TimerBuff((int)BasicCalculation.BattleCondition.Freeze,
            -1, 12, 1, -1);
        atk.AddWithConditionAll(freeze, 200);
        atk.playerpos = transform;

    }
    
    private void SummonSandalphonMulti(Vector2 pos)
    {
        UI_BattleInfoCaster.Instance?.PrintSkillName("STY_Action11",2);
        var fx = SpawnFXPrefab(prefabList[0], pos, 1); 
        
        
        var container = BattleStageManager.Instance.GetNewRangedContainer(false);
        
        var go_Supporter = Instantiate(prefabList[2], pos,
            Quaternion.identity, container.transform);

        var atk = go_Supporter.GetComponent<AttackFromPlayer>();
        var aff1 = new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,
            -1, 31, 1, -1);
        var aff3 = new TimerBuff((int)BasicCalculation.BattleCondition.ShadowBlight,
            -1, 31, 1, -1);
        var aff4 = new TimerBuff((int)BasicCalculation.BattleCondition.Stormlash,
            -1, 31, 1, -1);
        
        
        atk.AddWithConditionAll(aff1, 200);
        atk.AddWithConditionAll(aff3, 200,1);
        atk.AddWithConditionAll(aff4, 200,2);
        atk.playerpos = transform;

    }
    
}
