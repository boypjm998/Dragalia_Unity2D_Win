using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using LitJson;
using UnityEngine;
using Random = UnityEngine.Random;
using GameMechanics;
public class BattleStageManager : MonoBehaviour
{
    public static BattleStageManager Instance;
    
    
    private Coroutine gameResultRoutine;
    protected LevelDetailedInfo levelDetailedInfo;
    private AssetBundle assetBundle;
    [Header("Level INFO")]
    public int chara_id;
    public string quest_name;
    public string quest_id { get; set; }
    public List<int> FieldAbilityIDList = new();
    public GameObject boss;
    public int timeLimit;
    public int totalEnemyNum;
    public int currentEnemyNum { get; private set; }
    public int maxReviveTime = 3; //最大复活次数
    public int crownReviveTime = 0;// 得到第三颗星最大复活次数
    public int crownTimeLimit = 300;//得到第二颗星所需的时间
    public int clearConditionType;//0:击倒BOSS，1:无
    
    
    //private DamageNumberManager damageNumberManager;
    protected DamageNumberManager dnm;

    [Header("Common")]
    public GameObject attackContainer;
    public GameObject attackContainerEnemy;
    public GameObject attackSubContainer;
    public GameObject simpleHealthBar;
    
    public GameObject buffLogPrefab;
    public GameObject gameFailedPrefab;
    public GameObject gameClearPrefab;
    public AudioClip gameFailedBGM;
    public AudioClip gameClearBGM;
    public GameObject resultPage;
    
    [SerializeField]private GameObject player;
    public GameObject lastEnemyEliminated { get; private set; }
    
    public float mapBorderL { get; private set; }
    public float mapBorderR { get; private set; }
    public float mapBorderT { get; private set; }
    
    public float mapBorderB { get; private set; }

    public bool isGamePaused { get; private set; }

    public float currentTime { get; private set; } = 0;

    public static int currentDisplayingBossInfo = 1;//正在显示的boss信息


    public delegate void StageManagerIntegerDelegate(int id);

    public StageManagerIntegerDelegate OnFieldAbilityAdd;
    public StageManagerIntegerDelegate OnFieldAbilityRemove;
    
    public delegate void OnMouseOverDelegate();
    public event OnMouseOverDelegate OnPointerEnter;
    public event OnMouseOverDelegate OnPointerExit;
    
    public bool debugAddFieldAbility1 = false;
    public bool debugRemoveFieldAbility1 = false;
    public bool debugAddFieldAbility2 = false;
    public bool debugRemoveFieldAbility2 = false;
    
    
    public GameObject RangedAttackFXLayer { get; private set; }
    
    
    

    private void Awake()
    {
        //LoadDependency("ui_general");
        //LoadPlayer(1);
        //FindPlayer();
        //LinkBossStatus();
        if(Instance == null)
            Instance = this;
    }


    private void Start()
    {
        //GetMapBorderInfo();
        //player = GameObject.Find("PlayerHandle");

        var damageManager = GameObject.Find("DamageManager");
        dnm = damageManager.GetComponent<DamageNumberManager>();
        RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (GlobalController.currentGameState == GlobalController.GameState.Inbattle)
        {
            currentTime += Time.deltaTime;
        }
        if(debugAddFieldAbility1)
        {
            debugAddFieldAbility1 = false;
            AddFieldAbility(20081);
        }
        if(debugRemoveFieldAbility1)
        {
            debugRemoveFieldAbility1 = false;
            RemoveFieldAbility(20081);
        }
        if(debugAddFieldAbility2)
        {
            debugAddFieldAbility2 = false;
            AddFieldAbility(20091);
        }
        if(debugRemoveFieldAbility2)
        {
            debugRemoveFieldAbility2 = false;
            RemoveFieldAbility(20091);
        }
        
    }

    #region LoadAssets

    protected void FindPlayer()
    {
        player = GameObject.Find("PlayerHandle");
    }

    protected virtual void LoadDependency(string name)
    {
        AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, name));
        
    }

    protected virtual void LoadPlayer(int characterID)
    {
        assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "c001"));
        var plr = assetBundle.LoadAsset<GameObject>("PlayerHandle");
        var plrlayer = GameObject.Find("Player");
        var plrclone = Instantiate(plr, new Vector3(4.5f, -6.5f, 0), transform.rotation, plrlayer.transform);
        plrclone.name = "PlayerHandle";
        
        
        player = plrclone;


        player.GetComponent<AttackManager>().RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");

        var buffLayer = player.transform.Find("BuffLayer");

        if (buffLayer.childCount == 0)
        {
            var bufftxt = 
                         Instantiate(buffLogPrefab, buffLayer.position + new Vector3(0, 2), Quaternion.identity, buffLayer);
        }

        

        //StartCoroutine(开场buff(player));
    }

    public void InitPlayer(GameObject plr)
    {
        player = plr;
        
        player.GetComponent<AttackManager>().RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");

        var buffLayer = player.transform.Find("BuffLayer");
        
        if(buffLayer.GetComponentInChildren<UI_BuffLogPopManager>()!= null)
            return;
        
        var bufftxt = 
            Instantiate(buffLogPrefab, buffLayer.position + new Vector3(0, 2), Quaternion.identity, buffLayer);

        //StartCoroutine(开场buff(player));
    }

    public GameObject GetPlayer()
    {
        return player;
    }



    #endregion

    public LevelDetailedInfo GetLevelInfo()
    {
        return levelDetailedInfo;
    }
    public void LoadLevelDetailedInfo(int cid, LevelDetailedInfo info)
    {
        levelDetailedInfo = info;
        
        chara_id = cid;
        quest_name = info.name;
        timeLimit = info.time_limit>3600?3600:info.time_limit;
        maxReviveTime = info.revive_limit>10?10:info.revive_limit;
        crownTimeLimit = info.crown_time_limit>600?600:info.crown_time_limit;
        crownReviveTime = info.crown_revive_limit>maxReviveTime?maxReviveTime:info.crown_revive_limit;
        totalEnemyNum = info.total_boss_num;
        currentEnemyNum = totalEnemyNum;
        clearConditionType = info.clear_condition;

        GameObject.Find("UI").transform.Find("StartScreen").gameObject.SetActive(true);
    }
    
    public void LoadLevelDetailedInfoDebugScene(int cid, LevelDetailedInfo info)
    {
        levelDetailedInfo = info;
        
        chara_id = cid;
        quest_name = info.name;
        timeLimit = info.time_limit>3600?3600:info.time_limit;
        maxReviveTime = info.revive_limit>10?10:info.revive_limit;
        crownTimeLimit = info.crown_time_limit>600?600:info.crown_time_limit;
        crownReviveTime = info.crown_revive_limit>maxReviveTime?maxReviveTime:info.crown_revive_limit;
        totalEnemyNum = info.total_boss_num;
        currentEnemyNum = totalEnemyNum;
        clearConditionType = info.clear_condition;
    }

    public List<string> GetEnemyDependencies()
    {
        var boss_prefab_list = levelDetailedInfo.boss_prefab;
        List<string> dependencies = new List<string>();
        if (boss_prefab_list.Count > 0)
        {
            foreach (var boss_prefab in boss_prefab_list)
            {
                //将boss_prefab中resources列表追加到dependencies中
                dependencies.AddRange(boss_prefab.resources);
                
            }
        }

        return dependencies;

    }

    /// <summary>
    ///   <para>获得地图边界</para>
    /// </summary>
    public void GetMapBorderInfo()
    {
        var borderInfoL = GameObject.Find("BorderLeft");
        var borderInfoR = GameObject.Find("BorderRight");
        var borderInfoT = GameObject.Find("BorderTop");
        var borderInfoB = GameObject.FindGameObjectWithTag("Ground");

        mapBorderL = borderInfoL.GetComponent<BoxCollider2D>().offset.x +
                     borderInfoL.GetComponent<BoxCollider2D>().size.x * 0.5f + borderInfoL.transform.position.x;
        mapBorderR = borderInfoR.GetComponent<BoxCollider2D>().offset.x -
            borderInfoR.GetComponent<BoxCollider2D>().size.x * 0.5f + borderInfoR.transform.position.x;
        mapBorderT = borderInfoT.GetComponent<BoxCollider2D>().offset.y -
                     borderInfoT.GetComponent<BoxCollider2D>().size.y * 0.5f + borderInfoT.transform.position.y;
        mapBorderB = borderInfoB.GetComponent<BoxCollider2D>().bounds.max.y;
    }

    /// <summary>
    /// test link
    /// </summary>
    public virtual void LinkBossStatus()
    {
        //summon boss
        List<GameObject> bossList = new List<GameObject>();
        foreach (var single_boss in levelDetailedInfo.boss_prefab)
        {
            if (single_boss.load_at_start == 1)
            {
                bossList.Add(InstantiateBossResources(single_boss));
            }
        }
        
        boss = bossList[0];
        //切换的时候要改掉！！！
        currentDisplayingBossInfo = 1;
        
        print("boss name: " + boss.name);
        //TODO:如果不是boss，就不要显示boss状态栏

        var bossStat = GameObject.Find("UI")?.transform.GetComponentInChildren<UI_BossStatus>().gameObject;
        if(bossStat == null)
            return;
        
        bossStat.GetComponentInChildren<UI_BossStatus>()?.SetBoss(boss);
        bossStat.SetActive(true);
        
    }

    public void InitBGM()
    {
        
        var globalController = GameObject.Find("GlobalController").GetComponent<GlobalController>();
        var bundle = globalController.GetBundle(levelDetailedInfo.bgm_path);
        
        print(GlobalController.Instance.loadedBundles.Count);
        foreach (var VARIABLE in GlobalController.Instance.loadedBundles)
        {
            print(VARIABLE.Value.name);
        }
        
        
        
        var bgm_name = levelDetailedInfo.bgm_path.Split('/')[1];
        //print(bgm_name);
        var bgm = bundle.LoadAsset<AudioClip>(bgm_name);
        if (bgm == null)
        {
            Debug.LogWarning("BGM not found!");
            return;
        }

        
        if (BattleEffectManager.Instance.BGMHasSet == false)
        {
            BattleEffectManager.Instance.SetBGM(bgm);
        }
    }

    public AudioClip LoadBGMFromAssetBundle(string assetBundlePath,string name)
    {
        var bundle = GlobalController.Instance.GetBundle(assetBundlePath);
        var bgm = bundle.LoadAsset<AudioClip>(name);
        
        if (bgm == null)
        {
            Debug.LogWarning("BGM not found!");
            return null;
        }

        return bgm;
    }

    public void SpChargeAll(GameObject playerHandle, float sp)
    {
        var playerStatusManager = playerHandle.GetComponent<PlayerStatusManager>();
        
        if(playerStatusManager == null)
            return;

        //1、计算技速BUFF

        var spBuff = playerStatusManager.skillHasteUp;
        //TODO: 计算技速被动
        //TODO: 计算技速场地效果
        var spGain = sp * (1 + spBuff);

        //2、自充sp

        for (var i = 0; i < playerStatusManager.maxSkillNum; i++) playerStatusManager.SpGainInStatus(i, spGain);
    }

    public void SpCharge(GameObject playerHandle, float sp, int skillID)
    {
        var playerStatusManager = playerHandle.GetComponent<PlayerStatusManager>();


        playerStatusManager.SpGainInStatus(skillID, sp);
    }

    #region DamageModule

    public virtual int PlayerHit(GameObject target,GameObject player, AttackFromPlayer attackStat)
    {
        
        //1.If target is not in invincible state.
        if (!target.transform.Find("HitSensor").GetComponent<Collider2D>().isActiveAndEnabled) return -1;

        
        //Attack Callback
        switch (attackStat.attackType)
        {
            case BasicCalculation.AttackType.STANDARD:
                player.GetComponent<ActorController>()?.OnStandardAttackConnect(attackStat);
                break;
            case BasicCalculation.AttackType.SKILL:
                player.GetComponent<ActorController>()?.OnSkillConnect(attackStat);
                break;
            case BasicCalculation.AttackType.OTHER:
                player.GetComponent<ActorController>()?.OnOtherAttackConnect(attackStat);
                break;
        }

        var targetStat = target.GetComponentInChildren<StatusManager>();

        var damageM = new int[attackStat.GetHitCount()];

        var totalDamage = 0;

        var playerstat = player.GetComponentInChildren<StatusManager>();

        for (var i = 0; i < attackStat.GetHitCount(); i++)
        {
            //2.Calculate the damage deal to target.

            var isCrit = false;
            


            // var damage =
            //     BasicCalculation.CalculateDamagePlayer(
            //         playerstat,
            //         targetStat,
            //         attackStat.GetDmgModifier(i),
            //         attackStat,
            //         ref isCrit
            //     );

            //damageM[i] = (int)Mathf.Ceil(damage * Random.Range(0.95f, 1.05f));

            //3.Special Effect


            //4.Instantiate the damage number.


            if (isCrit)
                dnm.DamagePopEnemy(target.transform, damageM[i], 2);
            else
                dnm.DamagePopEnemy(target.transform, damageM[i], 1);

            totalDamage += damageM[i];

            player.GetComponent<PlayerStatusManager>()?.ComboConnect();
        }

        var container = attackStat.GetComponentInParent<AttackContainer>();

        
        
        //Affliction/Debuff
        /*if (attackStat.withConditions.Count > 0)
            if (!container.conditionCheckDone.Contains(target.GetInstanceID()))
            {
                for (var i = 0; i < attackStat.withConditionNum[0]; i++)
                {
                    var condFlag = CheckAffliction(attackStat.withConditionChance[i],
                        targetStat.GetAfflictionResistance
                            ((BasicCalculation.BattleCondition)attackStat.withConditions[i].buffID));
                    if (condFlag<1)
                    {
                        //1是成功,0是白字resist,-1是黄字resist
                        DamageNumberManager.GenerateResistText(target.transform);
                        continue;//检查异常抗性！不一定是异常！
                    }
                    if(StatusManager.IsDotAffliction(attackStat.withConditions[i].buffID)
                            ||StatusManager.IsControlAffliction(attackStat.withConditions[i].buffID))
                    {
                        targetStat.IncreaseAfflictionResistance(attackStat.withConditions[i].buffID);
                    }


                    var newEffect = attackStat.withConditions[i].effect;
                    print(newEffect);
                    if (StatusManager.IsDotAffliction(attackStat.withConditions[i].buffID))
                        newEffect = 5f / 300f * newEffect * BasicCalculation.CalculateAttackInfo(playerstat) /
                                    BasicCalculation.CalculateDefenseInfo(targetStat);
                    

                    
                    if (attackStat.withConditions[i].maxStackNum > 1)
                    {
                        targetStat.ObtainTimerBuff
                        (attackStat.withConditions[i].buffID,
                            newEffect,
                            attackStat.withConditions[i].duration,
                            attackStat.withConditions[i].DisplayType,
                            attackStat.withConditions[i].maxStackNum,
                            attackStat.withConditions[i].specialID);
                    }
                    else
                    {
                        targetStat.ObtainUnstackableTimerBuff
                        (attackStat.withConditions[i].buffID,
                            newEffect,
                            attackStat.withConditions[i].duration,
                            attackStat.withConditions[i].DisplayType,
                            attackStat.withConditions[i].specialID
                        );
                    }
                }

                container.conditionCheckDone.Add(target.GetInstanceID());
            }*/

        //KnockBack 击退
        // print("击退发生");
        // var kbtemp = attackStat.attackInfo[0].knockbackDirection;
        // kbtemp = attackStat.GetKBDirection(attackStat.attackInfo[0].KBType, target);
        // target.GetComponentInParent<EnemyController>().
        //     TakeDamage(attackStat.knockbackPower,
        //         attackStat.knockbackTime, 
        //         attackStat.knockbackForce, kbtemp);

        //5.Calculate the SP
        

        if (!container.spGained)
        {
            SpChargeAll(player, attackStat.GetSpGain());
            container.spGained = true;
        }


        //Enemy Take Damage

        targetStat.currentHp -= totalDamage;
        targetStat.OnHPChange?.Invoke();

        return totalDamage;
    }
    
    /// <summary>
    /// 攻击结算的主要函数
    /// </summary>
    /// <param name="target">目标</param>
    /// <param name="player">攻击发起者</param>
    /// <param name="attackStat">攻击属性</param>
    /// <param name="attackType">攻击类型，0代表玩家对敌人，1代表敌人对玩家或NPC，2代表NPC玩家对敌人</param>
    /// <returns></returns>
    public int CalculateHit(GameObject target, GameObject player, AttackBase attackStat, int attackType = 0)
    {
        
        //1.If target is not in invincible state.
        if (!target.transform.Find("HitSensor").GetComponent<Collider2D>().isActiveAndEnabled) return -1;
        
        
        var targetStat = target.GetComponentInChildren<StatusManager>();

        var damageM = new int[attackStat.GetHitCountInfo()];

        var totalDamage = 0;

        var playerstat = player.GetComponentInChildren<StatusManager>();

        



        switch (attackStat.attackType)
        {
            //Attack Callback
            case BasicCalculation.AttackType.STANDARD:
                player.GetComponent<ActorBase>()?.OnStandardAttackConnect(attackStat);
                break;
            case BasicCalculation.AttackType.SKILL:
                player.GetComponent<ActorBase>()?.OnSkillConnect(attackStat);
                break;
            case BasicCalculation.AttackType.OTHER:
                player.GetComponent<ActorBase>()?.OnOtherAttackConnect(attackStat);
                break;
        }

        

        //2-4 : Calculate the damage in a loop.
        for (var i = 0; i < attackStat.GetHitCountInfo(); i++)
        {
            //2.Calculate the damage deal to target.

            var isCrit = false;



            var damage =
                BasicCalculation.CalculateDamageGeneral(
                    playerstat,
                    targetStat,
                    attackStat.GetDmgModifierInfo(i),
                    attackStat,
                    ref isCrit
                );

            damageM[i] = (int)Mathf.Ceil(damage * Random.Range(0.95f, 1.05f)) +
                         (int)attackStat.GetDmgConstInfo(i) - (int)targetStat.GetDamageCutConst();
            
            if(damageM[i]<0) damageM[i] = 0;

            //3.Special Effect
            
            if (targetStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.Invincible) > 0)
            {
                damageM[i] = 0;
            }
            
            targetStat.OnTakeDirectDamage?.Invoke(targetStat);
            targetStat.OnTakeDirectDamageFrom?.Invoke(targetStat,playerstat);


            //4.Instantiate the damage number.
            
            if (attackType == 0)
            {
                if (isCrit)
                    dnm.DamagePopEnemy(target.transform, damageM[i], 2);
                else
                    dnm.DamagePopEnemy(target.transform, damageM[i], 1);
            }
            else if(attackType == 1)
            {
                if (isCrit)
                    dnm.DamagePopPlayer(target.transform, damageM[i], true);
                else
                    dnm.DamagePopPlayer(target.transform, damageM[i], false);
            }
            else
            {
                if (isCrit)
                    dnm.DamagePopEnemy(target.transform, damageM[i], 2,0.5f);
                else
                    dnm.DamagePopEnemy(target.transform, damageM[i], 1,0.5f);
            }
            

            totalDamage += damageM[i];

            player.GetComponent<StatusManager>()?.ComboConnect();
        }

        var container = attackStat.GetComponentInParent<AttackContainer>();


        if (targetStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.Invincible) > 0)
        {
            return -1;
        }
        
        //5. Affliction/Debuff
        List<BattleCondition> attachedConditions = new();
        if (attackStat.attackInfo[0].withConditions.Count > 0)
        {
            for (var i = 0; i < attackStat.attackInfo[0].withConditions.Count; i++)
                {
                    if (container.checkedConditions.Contains
                            (new Tuple<int, int>(target.GetInstanceID(),
                                attackStat.attackInfo[0].withConditions[i].identifier))) //改成字典判断
                    {
                        continue;
                    }
                    else
                    {
                        container.AddNewCheckedCondition(target.GetInstanceID(),
                            attackStat.attackInfo[0].withConditions[i].identifier);
                    }

                    var withCondition = attackStat.attackInfo[0].withConditions[i];
                    
                    //驱散
                    if (withCondition.condition.buffID == 999)
                    {
                        var dispellCheck = targetStat.DispellTimerBuff();
                        if (dispellCheck)
                        {
                            continue;
                        }
                        else
                        {
                            container.RemoveCheckedCondition(target.GetInstanceID(),
                                attackStat.attackInfo[0].withConditions[i].identifier);
                            continue;
                        }
                    }
                    
                    //如果是异常状态！
                    if (StatusManager.IsAffliction(withCondition.condition.buffID))
                    {
                        var condFlag = CheckAffliction(withCondition.withConditionChance +
                                                       playerstat.GetConditionRateBuff((BasicCalculation.BattleCondition)(withCondition.condition.buffID)),
                            targetStat.GetAfflictionResistance
                                ((BasicCalculation.BattleCondition)withCondition.condition.buffID));
                        if (condFlag<1)
                        {
                            playerstat.OnAfflictionResist?.Invoke(withCondition.condition);
                            //1是成功,0是白字resist,-1是黄字resist
                            if(condFlag == 0)
                                DamageNumberManager.GenerateResistText(target.transform);
                            else
                            {
                                DamageNumberManager.GenerateResistText(target.transform, 1);
                            }
                        
                            continue;//检查异常抗性！不一定是异常！
                        }
                        if(StatusManager.IsAffliction(withCondition.condition.buffID))
                        {
                            if(attackType!=1)
                                targetStat.IncreaseAfflictionResistance(withCondition.condition.buffID);
                        }
                    }
                    else
                    {
                        var condFlag = CheckAffliction(
                            (withCondition.withConditionChance + playerstat.GetConditionRateBuff((BasicCalculation.BattleCondition)(withCondition.condition.buffID)))*
                            (1+BasicCalculation.CheckSpecialDebuffRateEffect(playerstat,targetStat,attackStat).Item1),
                            0);
                        if (condFlag < 1)
                        {
                            //failed
                            continue;
                        }
                    }



                    //TODO:异常数值重做！！
                    
                    var newEffect = withCondition.condition.effect;
                    //print(newEffect);
                    if (StatusManager.IsDotAffliction(withCondition.condition.buffID))
                        newEffect = 5f / 300f * newEffect * BasicCalculation.CalculateAttackInfo(attackStat,playerstat,targetStat) /
                                    BasicCalculation.CalculateDefenseInfo(targetStat);
                    
                    
                    

                    
                    if (withCondition.condition.maxStackNum > 1)
                    {
                        targetStat.ObtainTimerBuff
                        (withCondition.condition.buffID,
                            newEffect,
                            withCondition.condition.duration,
                            withCondition.condition.maxStackNum,
                            withCondition.condition.specialID);
                    }
                    else
                    {
                        targetStat.ObtainUnstackableTimerBuff
                        (   withCondition.condition.buffID,
                            newEffect,
                            withCondition.condition.duration,
                            withCondition.condition.specialID
                        );
                    }
                    attachedConditions.Add(new TimerBuff(withCondition.condition.buffID,
                        newEffect,
                        withCondition.condition.duration,
                        withCondition.condition.maxStackNum,
                        withCondition.condition.specialID));
                    
                }

        }

        

        //6. KnockBack 击退
        var kbtemp = attackStat.attackInfo[0].knockbackDirection;
        kbtemp = attackStat.GetKBDirection(attackStat.attackInfo[0].KBType, target);
        
        //print("击退的力度为"+attackStat.attackInfo[0].knockbackForce);
        
        // target.GetComponentInParent<ActorBase>().
        //     TakeDamage(attackStat.attackInfo[0].knockbackPower,
        //         attackStat.attackInfo[0].knockbackTime, 
        //         attackStat.attackInfo[0].knockbackForce, kbtemp);
        target.GetComponentInParent<ActorBase>().
            TakeDamage(attackStat,kbtemp);

        //7. Calculate the SP
        if (player.GetComponent<PlayerStatusManager>() != null)
        {
            if (!container.spGained)
            {
                SpChargeAll(player, ((AttackFromPlayer)attackStat).GetSpGain());
                container.spGained = true;
            }
        }
        
        //8. Enemy Take Damage

        targetStat.currentHp -= totalDamage;
        targetStat.OnHPChange?.Invoke();
        targetStat.OnHPDecrease?.Invoke(totalDamage);

        // 9. Reduce enemy's Overdrive Gauge
        if (targetStat is SpecialStatusManager)
        {
            var targetSpecialStat = (SpecialStatusManager) targetStat;
            if (targetSpecialStat.baseBreak > 0)
            {
                float ODModifier = 0;
                if (container.IfODCounter)
                {
                    ODModifier += (0.8f + 0.02f * Mathf.Sqrt(Mathf.Abs(attackStat.attackInfo[0].knockbackPower-100)));
                    ODModifier += targetSpecialStat.counterModifier;
                }

                //Calculate OD Gauge Punisher
                var ODpunisher = 1 + playerstat.ODAccerator + 
                                 BasicCalculation.CheckSpecialODAccerleratorEffect
                                     (playerstat,targetSpecialStat,attackStat);
                if (targetSpecialStat.ODLock == false)
                {
                    targetSpecialStat.currentBreak -= totalDamage * (Random.Range(0.9f, 1.1f) + ODModifier) * ODpunisher;
                }
            }

        }
        
        //10、Special Field Effects
        
        CheckSpecialFieldEffect(attackStat, playerstat, targetStat,attachedConditions,damageM);

        return totalDamage;
        
        
    }

    public int CauseIndirectDamage(StatusManager stat, int damage,bool causeDeath, bool random = false)
    {
        if (stat.GetComponent<ActorBase>().IsInvincible)
        {
            return 0;
        }

        int damageM = damage;
        if (random)
        {
            damageM = (int)Mathf.Ceil(damage * Random.Range(0.95f, 1.05f));
        }
        
        
        if (stat.currentHp <= damageM && !causeDeath)
        {
            damageM = stat.currentHp - 1;
        }
        
        dnm.IndirectDamagePop(damageM,stat.transform);
        
        if(damageM > 0)
            stat.OnHPChange?.Invoke();

        stat.currentHp -= damageM;
        
        return damageM;

    }

    public int CauseIndirectDamageToOverdriveBar(SpecialStatusManager stat, int damage,
        bool random = false)
    {
        if (stat.GetComponent<ActorBase>().IsInvincible)
        {
            return 0;
        }

        int damageM = damage;
        if (random)
        {
            damageM = (int)Mathf.Ceil(damage * Random.Range(0.9f, 1.1f));
        }

        if (stat.baseBreak <= 0 || stat.ODLock)
            return 0;
        
        
        if (stat.currentBreak <= damageM)
        {
            damageM = (int)(stat.currentBreak - 1);
        }

        stat.currentBreak -= damageM;
        
        return damageM;
    }



    public virtual int TargetHeal(GameObject target, float healPotency, float healPotencyPercentage, bool randomRange)
    {
        var stat = target.GetComponent<StatusManager>();
        //1. 计算回血Part1
        var damage = BasicCalculation.CalculateHPRegenGeneral(stat, healPotency, healPotencyPercentage);

        int damageM;
        if (randomRange)
        { 
            damageM = (int)Mathf.Ceil(damage * Random.Range(0.95f, 1.05f));
        }
        else
        {
            damageM = (int)Mathf.Ceil(damage);
        }



        dnm.HealPop(damageM, target.transform);

        if(stat.currentHp < stat.maxHP)
            stat.OnHPChange?.Invoke();
        
        stat.currentHp += damageM;

        return damageM;
    }

    public virtual int TargetHeal(StatusManager stat, float healPotency, float healPotencyPercentage,
        bool randomRange)
    {
        //1. 计算回血Part1
        var damage = BasicCalculation.CalculateHPRegenGeneral(stat, healPotency, healPotencyPercentage);

        int damageM;
        if (randomRange)
        { 
            damageM = (int)Mathf.Ceil(damage * Random.Range(0.95f, 1.05f));
        }
        else
        {
            damageM = (int)Mathf.Ceil(damage);
        }



        dnm.HealPop(damageM, stat.transform);

        if(stat.currentHp < stat.maxHP)
            stat.OnHPChange?.Invoke();
        
        stat.currentHp += damageM;

        return damageM;
    }

    public int TargetDot(GameObject target, BasicCalculation.BattleCondition condition)
    {
        var stat = target.GetComponent<StatusManager>();

        var modifier = stat.GetConditionTotalValue((int)condition);


        //Check target.HP

        var damage = (int)modifier;

        var damageM = (int)Mathf.Ceil(damage * Random.Range(0.95f, 1.05f));
        
        if (stat.currentHp <= damageM)
        {
            damageM = stat.currentHp - 1;
        }

        dnm.DotPop(damageM, target.transform, condition);


        if(damageM > 0)
            stat.OnHPChange?.Invoke();
        stat.OnHPDecrease?.Invoke(damageM);

        stat.currentHp -= damageM;
        
        
        return damageM;
    }

    public int CheckSpecialFieldEffect(AttackBase attackStat, StatusManager srcStat, StatusManager targetStat,
        List<BattleCondition> attachedCondition, int[] dmg)
    {
        //反伤
        if (FieldAbilityIDList.Contains(20033))
        {
            var reflectDamage = (int)(dmg.Sum() * 0.1f + srcStat.currentHp*0.02f);
            CauseIndirectDamage(srcStat, reflectDamage, true);
            if (attachedCondition.Count > 0)
            {
                foreach (var condition in attachedCondition)
                {
                    if (StatusManager.IsControlAffliction(condition.buffID) ||
                        StatusManager.IsDotAffliction(condition.buffID))
                    {
                        srcStat.ObtainTimerBuff(condition);
                    }
                }
            }
        }





        return dmg[0];
    }

    #endregion

    /// <summary>
    /// 检查异常上去没
    /// </summary>
    /// <param name="chance"></param>
    /// <param name="resistance"></param>
    /// <returns>-1:黄字,0:白字</returns>
    protected int CheckAffliction(int chance, int resistance)
    {
        //print(chance+"chance");
        //检测异常状态是不是上的去
        if (resistance >= 100 || chance<=resistance)
        {
            return -1;
        }

        var p = Random.Range(0, 100);
        {
            if (p < chance-resistance)
            {
                return 1;
            }
            else
            {
                //DamageNumberManager.GenerateResistText();
                return 0;
            }
        }


    }

    public void AddFieldAbility(int id)
    {
        //OnFieldAbilityAdd?.Invoke(id);
        var abilityIcons = FindObjectsOfType<UI_BossAbilityDisplayer>();
        foreach (var ability in abilityIcons)
        {
            if(ability.abilityID == id)
                ability.SetIconActive(true);
        }
        if(FieldAbilityIDList.Contains(id))
            return;
        FieldAbilityIDList.Add(id);
        OnFieldAbilityAdd?.Invoke(id);
    }

    public void RemoveFieldAbility(int id)
    {
        //OnFieldAbilityRemove?.Invoke(id);
        var abilityIcons = FindObjectsOfType<UI_BossAbilityDisplayer>();
        foreach (var ability in abilityIcons)
        {
            if(ability.abilityID == id)
                ability?.SetIconActive(false);
        }

        
        FieldAbilityIDList.Remove(id);
        OnFieldAbilityRemove?.Invoke(id);
    }

    public void ClearAllFieldAbility()
    {
        foreach (var ability in FieldAbilityIDList)
        {
            OnFieldAbilityRemove?.Invoke(ability);
        }
        FieldAbilityIDList.Clear();
    }

    public List<Transform> GetEnemyList()
    {
        var hitFlags = new List<Transform>();
    
        var enemyLayer = GameObject.Find("EnemyLayer");
        
        var markedEnemies = GetEnemyWithMarking(enemyLayer);

        if (markedEnemies.Count > 0)
        {
            for (var i = 0; i < markedEnemies.Count; i++)
            {
                var ene = markedEnemies[i].gameObject;
                if (ene.activeSelf && ene.GetComponent<ActorBase>().HitSensor.gameObject.activeSelf)
                {
                    hitFlags.Add(markedEnemies[i]);
                }
            }
        }
        else
        {
            for (var i = 0; i < enemyLayer.transform.childCount; i++)
            {
                var ene = enemyLayer.transform.GetChild(i).gameObject;
                if (ene.activeSelf && ene.GetComponent<ActorBase>().HitSensor.gameObject.activeSelf)
                {
                    hitFlags.Add(enemyLayer.transform.GetChild(i));
                }
            }
        }

        

        return hitFlags;
    }

    protected List<Transform> GetEnemyWithMarking(GameObject enemyPool)
    {
        List<Transform> markedEnemies = new();
        for(int i = 0; i < enemyPool.transform.childCount; i++)
        {
            var ene = enemyPool.transform.GetChild(i).gameObject;
            if (ene.activeSelf && ene.GetComponent<ActorBase>().HitSensor.gameObject.activeSelf)
            {
                if (ene.GetComponent<StatusManager>().
                        GetConditionsOfType
                            ((int)BasicCalculation.BattleCondition.Taunt).
                        Count>0)
                {
                    markedEnemies.Add(ene.transform);
                }
            }
        }

        return markedEnemies;
    }

















    public void SetGamePause(bool flag)
    {
        isGamePaused = flag;
        Time.timeScale = flag ? 0 : 1;
        var voices1 = GameObject.FindGameObjectsWithTag("Voice");
        foreach (var voice in voices1)
        {
            var audiosrc = voice.GetComponent<AudioSource>();
            
            if (flag)
            {
                if(audiosrc.isPlaying)
                    audiosrc.Pause();
            }
            else
            {
                if(!audiosrc.isPlaying)
                    audiosrc.UnPause();
            }
        }
    }

    public void SetGameFailed()
    {
        if(gameResultRoutine==null)
            gameResultRoutine =
                StartCoroutine(GameFailedRoutine());
    }

    IEnumerator GameFailedRoutine()
    {
        var playerController = FindObjectOfType<PlayerInput>();
        playerController.DisableAndIdle(false);

        var actors = FindObjectsOfType<ActorBase>();
        foreach (var ac in actors)
        {
            ac.SetHitSensor(false);
            ac.enabled = false;
        }
        
        var enemybehaviors = FindObjectsOfType<DragaliaEnemyBehavior>();
        foreach (var behavior in enemybehaviors)
        {
            behavior.playerAlive = false;
            behavior.enabled = false;
        }
        
        
        
        var music = GetComponent<AudioSource>();
        music.clip = gameFailedBGM;
        music.Play();
        music.loop = false;


        var targetTransform = GameObject.Find("UIFXContainer").transform;
        Instantiate(gameFailedPrefab,
            Camera.main.transform.position+new Vector3(0,2,0),
            Quaternion.identity,
            targetTransform);

        yield return new WaitForSeconds(3f);
        yield return new WaitUntil(()=>!music.isPlaying || Input.GetMouseButton(0));
        
        GlobalController globalController = FindObjectOfType<GlobalController>();
        globalController.TestReturnMainMenu();
    }
    
    public void SetGameCleared()
    {
        if(gameResultRoutine==null)
            gameResultRoutine =
                StartCoroutine(GameClearedRoutine());
    }

    IEnumerator GameClearedRoutine()
    {
        //var fxs = GameObject.Find("AttackFXPlayer");

        var playerinput = player.GetComponent<PlayerInput>();
        
        playerinput.stdAtk = false;
        playerinput.roll = false;
        
        if (GameObject.Find("EnemyLayer").transform.childCount > 0)
        {
            Time.timeScale = 0.5f;
            GameObject.Find("CharacterInfo").SetActive(false);
        
            StageCameraController.SwitchMainCamera();
            StageCameraController.SwitchMainCameraFollowObject(lastEnemyEliminated);
            StageCameraController.SetMainCameraSize(6);
            yield return new WaitForSeconds(.8f);
        }

        Time.timeScale = 1;
        StageCameraController.SwitchMainCameraFollowObject(player);
        StageCameraController.SetMainCameraSize(8);

        yield return new WaitForSeconds(3f);
        var targetTransform = GameObject.Find("UIFXContainer").transform;
        var clearGameObject = Instantiate(gameClearPrefab,
            Camera.main.transform.position+new Vector3(0,0,-5),
            Quaternion.identity,
            targetTransform);
        
        
        FindObjectOfType<UI_MultiBossManager>().gameObject.SetActive(false);
        
        playerinput.DisableAllInput();
        playerinput.SetMoveDisabled();
        playerinput.DisableAndIdle();
        
        var playercontroller = player.GetComponent<ActorController>();
        playercontroller.anim.SetFloat("forward",0);
        yield return null;
        playercontroller.anim.Play("idle");
        playercontroller.enabled = false;
        playerinput.DisableAndIdle();
        
        var attacks = FindObjectsOfType<AttackContainer>();
        foreach (var attack in attacks)
        {
            attack.DestroyInvoke();
        }
        
        //fxs?.SetActive(false);
        var music = GetComponent<AudioSource>();
        music.clip = gameClearBGM;
        music.Play();
        //music.loop = false;
        yield return new WaitForSeconds(1.5f);
        var waitTime = 0f;
        while (!Input.GetMouseButton(0) && waitTime<1.5f)
        {
            waitTime += 0.02f;
            yield return new WaitForSeconds(0.02f);
        }
        
        playercontroller.anim.SetFloat("forward",0);
        playercontroller.anim.Play("idle");
        Destroy(clearGameObject);
        var UILayer = GameObject.Find("UI");
        var resultPage = Instantiate(this.resultPage, UILayer.transform);


    }

    public void EnemyEliminated(GameObject enemy)
    {
        currentEnemyNum--;
        if (currentEnemyNum <= 0)
        {
            lastEnemyEliminated = enemy;
            //宣告游戏结束
            GlobalController.BattleFinished(true);
        }
    }
    


    public void UpdateQuestSaveData(QuestSave newQuestState,ref bool newRecord)
    {
        newQuestState.best_clear_time = (double)Mathf.Round((float)newQuestState.best_clear_time*10f) / 10f;
        // 存储文件的路径  

        //print(newQuestState.best_clear_time);
        
        string path = Application.streamingAssetsPath + "/savedata/testSaveData.json";
        StreamReader sr = new StreamReader(path);
        var str = sr.ReadToEnd();
        sr.Close();
        var datalist = JsonMapper.ToObject<QuestDataList>(str);

        //var newData = datalist["quest_info"][quest_id];
        

        var savedata = JsonMapper.ToObject<QuestSave>(str);

        bool isFound = false;
        foreach (var data in datalist.quest_info)
        {
            if (data.quest_id == quest_id)
            {
                isFound = true;
                savedata = data;
            }
        }
        if (!isFound)
        {
            datalist.quest_info.Add(newQuestState);
            savedata = datalist.quest_info[datalist.quest_info.Count - 1];
            newRecord = true;
        }
        
        
        
        if (newQuestState.best_clear_time < savedata.best_clear_time || savedata.best_clear_time < 0)
        {
            newRecord = true;
        }
        else
        {
            newQuestState.best_clear_time = savedata.best_clear_time;
        }

        if (savedata.crown_1 == 1)
        {
            newQuestState.crown_1 = 1;
        }
        if (savedata.crown_2 == 1)
        {
            newQuestState.crown_2 = 1;
        }
        if (savedata.crown_3 == 1)
        {
            newQuestState.crown_3 = 1;
        }

        //var isFound = false;
       
        
            
            
        
        savedata.best_clear_time = newQuestState.best_clear_time;
        savedata.crown_1 = newQuestState.crown_1;
        savedata.crown_2 = newQuestState.crown_2;
        savedata.crown_3 = newQuestState.crown_3;
        //print(savedata.best_clear_time);
        //print(newRecord);
        //print(savedata.best_clear_time);
                
            
        

        
        
        string jsonStr = JsonMapper.ToJson(datalist);
        string filePath = Application.streamingAssetsPath + "/savedata/testSaveData.json";
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(jsonStr);
        sw.Close();


    }

    protected GameObject InstantiateBossResources(LevelDetailedInfo.BossPrefabInfo prefabInfo)
    {
        var globalController = GameObject.Find("GlobalController").GetComponent<GlobalController>();
        var enemyBundle = globalController.GetBundle(prefabInfo.bundle_name);
        //从enemyBundle中加载出名为prefabInfo.prefab_name的预制体
        var prefab = enemyBundle.LoadAsset<GameObject>(prefabInfo.prefab_name);
        Vector3 startPos = new Vector3
            ((float)prefabInfo.start_position[0], (float)prefabInfo.start_position[1], 0);
        var _parent = GameObject.Find("EnemyLayer").transform;
        var boss = Instantiate(prefab, startPos, Quaternion.identity, _parent);
        return boss;
    }
    
    public static List<Platform> InitMapInfo()
    {
        //获取场景上所有tag为platform或Ground的物体和其碰撞体
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("platform");
        GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");
        GameObject[] all = new GameObject[platforms.Length + grounds.Length];
        platforms.CopyTo(all, 0);
        grounds.CopyTo(all, platforms.Length);
        print(all.Length);
        //将all中所有碰撞体存入mapInfo列表。
        List<Platform> platformsInfo = new List<Platform>();
        foreach (GameObject go in all)
        {
            print(go.name);
            var platform = new Platform(go);
            platformsInfo.Add(platform);
        }

        return platformsInfo;
    }

    public LevelDetailedInfo GetLevelDetailedInfo()
    {
        return levelDetailedInfo;
    }

    public Vector2 OutOfRangeCheck(Vector2 pos)
    {
        if (pos.y > mapBorderT)
        {
            pos.y = mapBorderT;
        }
        if (pos.y < mapBorderB)
        {
            pos.y = mapBorderB;
        }
        if (pos.x > mapBorderR)
        {
            pos.x = mapBorderR;
        }
        if (pos.x < mapBorderL)
        {
            pos.x = mapBorderL;
        }

        return new Vector2(pos.x, pos.y);
    }

    public float OutOfPlatformBoundsCheck(GameObject target, float posX)
    {
        var targetPlatform = BasicCalculation.CheckRaycastedPlatform(target);
        if (targetPlatform == null)
        {
            return posX;
        }
        
        if(posX > targetPlatform.bounds.max.x)
        {
            posX = Mathf.Max(target.transform.position.x, targetPlatform.bounds.max.x);
        }else if(posX < targetPlatform.bounds.min.x)
        {
            posX = Mathf.Min(target.transform.position.x, targetPlatform.bounds.min.x);
        }
        
        //pos.y = targetPlatform.bounds.max.y;

        return posX;


    }
    public int ObtainAfflictionDirectlyWithCheck(StatusManager targetStat,BattleCondition condition, int chance,int attackType=1)
    {
        var condFlag = CheckAffliction(chance,
            targetStat.GetAfflictionResistance
                ((BasicCalculation.BattleCondition)condition.buffID));
        if (condFlag<1)
        {
            //sourceStat.OnAfflictionResist?.Invoke(condition);
            //1是成功,0是白字resist,-1是黄字resist
            if(condFlag == 0)
                DamageNumberManager.GenerateResistText(targetStat.transform);
            else
            {
                DamageNumberManager.GenerateResistText(targetStat.transform, 1);
            }
                        
            return condFlag;//检查异常抗性！不一定是异常！
        }
        targetStat.ObtainTimerBuff(condition);
        if(StatusManager.IsAffliction(condition.buffID))
        {
            if(attackType!=1)
                targetStat.IncreaseAfflictionResistance(condition.buffID);
        }

        return condFlag;
    }

    public static List<StatusManager> GetAllStatusManagers()
    {
        var list =  FindObjectsOfType<StatusManager>().ToList();
        var result = new List<StatusManager>();
        foreach(var status in list)
        {
            if(status.GetComponent<ActorBase>() != null)
            {
                result.Add(status);
            }
        }

        return result;

    }
    
    public void InvokePointerEvent(bool flag)
    {
        if (flag)
        {
            OnPointerEnter?.Invoke();
        }
        else
        {
            OnPointerExit?.Invoke();
        }
    }



}