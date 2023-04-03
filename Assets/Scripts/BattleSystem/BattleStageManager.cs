using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public List<int> FieldAbilityIDList;
    public GameObject boss;
    public int timeLimit;
    public int totalEnemyNum;
    public int currentEnemyNum { get; private set; }
    public int maxReviveTime = 3; //最大复活次数
    public int crownReviveTime = 0;// 得到第三颗星最大复活次数
    public int crownTimeLimit = 300;//得到第二颗星所需的时间
    
    
    //private DamageNumberManager damageNumberManager;
    protected DamageNumberManager dnm;

    [Header("Common")]
    public GameObject attackContainer;
    public GameObject attackContainerEnemy;
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

    public bool isGamePaused { get; private set; }

    public float currentTime { get; private set; } = 0;

    public static int currentDisplayingBossInfo = 1;//正在显示的boss信息

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
        GetMapBorderInfo();
        //player = GameObject.Find("PlayerHandle");

        var damageManager = GameObject.Find("DamageManager");
        dnm = damageManager.GetComponent<DamageNumberManager>();
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (GlobalController.currentGameState == GlobalController.GameState.Inbattle)
        {
            currentTime += Time.deltaTime;
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
        var bufftxt = 
            Instantiate(buffLogPrefab, buffLayer.position + new Vector3(0, 2), Quaternion.identity, buffLayer);

        //StartCoroutine(开场buff(player));
    }

    public void InitPlayer(GameObject plr)
    {
        player = plr;
        
        player.GetComponent<AttackManager>().RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");

        var buffLayer = player.transform.Find("BuffLayer");
        
        var bufftxt = 
            Instantiate(buffLogPrefab, buffLayer.position + new Vector3(0, 2), Quaternion.identity, buffLayer);

        //StartCoroutine(开场buff(player));
    }



    #endregion

    public void GetLevelInfo(int cid, string qname,int timeLimit,int crownTime,
        int totalEnemyNum, int maxRevive, int crownRevive)
    {
        chara_id = cid;
        quest_name = qname;
        this.timeLimit = timeLimit;
        this.maxReviveTime = maxRevive;
        
        this.crownTimeLimit = crownTime;
        this.crownReviveTime = crownRevive;
        this.totalEnemyNum = totalEnemyNum;
        currentEnemyNum = totalEnemyNum;
        GameObject.Find("UI").transform.Find("StartScreen").gameObject.SetActive(true);
    }
    public void LoadLevelDetailedInfo(int cid, LevelDetailedInfo info)
    {
        levelDetailedInfo = info;
        
        chara_id = cid;
        quest_name = info.name;
        timeLimit = info.time_limit;
        maxReviveTime = info.revive_limit;
        crownTimeLimit = info.crown_time_limit;
        crownReviveTime = info.crown_revive_limit;
        totalEnemyNum = info.total_boss_num;
        currentEnemyNum = totalEnemyNum;
        
        
        
        GameObject.Find("UI").transform.Find("StartScreen").gameObject.SetActive(true);
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

        mapBorderL = borderInfoL.GetComponent<BoxCollider2D>().offset.x +
                     borderInfoL.GetComponent<BoxCollider2D>().size.x * 0.5f + borderInfoL.transform.position.x;
        mapBorderR = borderInfoR.GetComponent<BoxCollider2D>().offset.x -
            borderInfoR.GetComponent<BoxCollider2D>().size.x * 0.5f + borderInfoR.transform.position.x;
        mapBorderT = borderInfoT.GetComponent<BoxCollider2D>().offset.y -
                     borderInfoT.GetComponent<BoxCollider2D>().size.y * 0.5f + borderInfoT.transform.position.y;
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

        var bossStat = GameObject.Find("UI")?.transform.Find("BossStatusBar")?.gameObject;
        if(bossStat == null)
            return;
        
        bossStat.GetComponentInChildren<UI_BossStatus>()?.SetBoss(boss);
        bossStat.SetActive(true);
        
    }

    public void SpChargeAll(GameObject playerHandle, float sp)
    {
        var playerStatusManager = playerHandle.GetComponent<PlayerStatusManager>();
        
        if(playerStatusManager == null)
            return;

        //1、计算技速BUFF


        //2、自充sp

        for (var i = 0; i < playerStatusManager.maxSkillNum; i++) playerStatusManager.SpGainInStatus(i, sp);
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
            


            var damage =
                BasicCalculation.CalculateDamagePlayer(
                    playerstat,
                    targetStat,
                    attackStat.GetDmgModifier(i),
                    attackStat,
                    ref isCrit
                );

            damageM[i] = (int)Mathf.Ceil(damage * Random.Range(0.95f, 1.05f));

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

        
        //Attack Callback
        switch (attackStat.attackType)
        {
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

        var targetStat = target.GetComponentInChildren<StatusManager>();

        var damageM = new int[attackStat.GetHitCountInfo()];

        var totalDamage = 0;

        var playerstat = player.GetComponentInChildren<StatusManager>();

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
                         (int)attackStat.GetDmgConstInfo(i);

            //3.Special Effect


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

            player.GetComponent<PlayerStatusManager>()?.ComboConnect();
        }

        var container = attackStat.GetComponentInParent<AttackContainer>();

        
        
        //5. Affliction/Debuff
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
                    
                    if (withCondition.condition.buffID == 999)
                    {
                        targetStat.DispellTimerBuff();
                        continue;
                    }
                    
                    var condFlag = CheckAffliction(withCondition.withConditionChance,
                        targetStat.GetAfflictionResistance
                            ((BasicCalculation.BattleCondition)withCondition.condition.buffID));
                    if (condFlag<1)
                    {
                        //1是成功,0是白字resist,-1是黄字resist
                        DamageNumberManager.GenerateResistText(target.transform);
                        continue;//检查异常抗性！不一定是异常！
                    }
                    if(StatusManager.IsDotAffliction(withCondition.condition.buffID)
                            ||StatusManager.IsControlAffliction(withCondition.condition.buffID))
                    {
                        targetStat.IncreaseAfflictionResistance(withCondition.condition.buffID);
                    }


                    var newEffect = withCondition.condition.effect;
                    print(newEffect);
                    if (StatusManager.IsDotAffliction(withCondition.condition.buffID))
                        newEffect = 5f / 300f * newEffect * BasicCalculation.CalculateAttackInfo(playerstat) /
                                    BasicCalculation.CalculateDefenseInfo(targetStat);
                    

                    
                    if (withCondition.condition.maxStackNum > 1)
                    {
                        targetStat.ObtainTimerBuff
                        (withCondition.condition.buffID,
                            newEffect,
                            withCondition.condition.duration,
                            withCondition.condition.DisplayType,
                            withCondition.condition.maxStackNum,
                            withCondition.condition.specialID);
                    }
                    else
                    {
                        targetStat.ObtainUnstackableTimerBuff
                        (   withCondition.condition.buffID,
                            newEffect,
                            withCondition.condition.duration,
                            withCondition.condition.DisplayType,
                            withCondition.condition.specialID
                        );
                    }
                    
                }

        }

        

        //6. KnockBack 击退
        var kbtemp = attackStat.attackInfo[0].knockbackDirection;
        kbtemp = attackStat.GetKBDirection(attackStat.attackInfo[0].KBType, target);
        
        //print("击退的力度为"+attackStat.attackInfo[0].knockbackForce);
        
        target.GetComponentInParent<ActorBase>().
            TakeDamage(attackStat.attackInfo[0].knockbackPower,
                attackStat.attackInfo[0].knockbackTime, 
                attackStat.attackInfo[0].knockbackForce, kbtemp);

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

        return totalDamage;
        
        
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

        stat.currentHp += damageM;

        stat.OnHPChange?.Invoke();
        
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


        stat.currentHp -= damageM;
        
        stat.OnHPChange?.Invoke();

        return damageM;
    }

    public virtual int EnemyHit(GameObject target, GameObject self, AttackFromEnemy attackStat)
    {
        //1.If target is not in invincible state.
        if (target.transform.Find("HitSensor").GetComponent<Collider2D>().isActiveAndEnabled == false) return -1;

        //Attack Callback

        var targetStat = target.GetComponentInParent<StatusManager>();
        var damageM = new int[attackStat.GetHitCount()];
        var totalDamage = 0;
        
        //var selfstat = attackStat.enemySource;
        //if (selfstat == null)
        var selfstat = self.GetComponentInChildren<StatusManager>();
        //}
        
        


        for (var i = 0; i < attackStat.GetHitCount(); i++)
        {
            //2.Calculate the damage deal to target.

            var isCrit = false;
            var damage =
                BasicCalculation.CalculateDamageEnemy(
                    selfstat,
                    targetStat,
                    attackStat.GetDmgModifier(i),
                    attackStat,
                    ref isCrit
                );

            damageM[i] = (int)Mathf.Ceil(damage * Random.Range(0.95f, 1.05f));

            //3.Special Effect


            //4.Instantiate the damage number.


            if (isCrit)
                dnm.DamagePopPlayer(target.transform, damageM[i], true);
            else
                dnm.DamagePopPlayer(target.transform, damageM[i], false);

            totalDamage += damageM[i];
        }

        var container = attackStat.GetComponentInParent<AttackContainer>();

        //Affliction/Debuff
        /*if (attackStat.withConditions.Count > 0)
            if (!container.conditionCheckDone.Contains(target.GetInstanceID()))
            {
                for (var i = 0; i < attackStat.withConditionNum[0]; i++)
                {
                    if (attackStat.withConditions[i].buffID == 999)
                    {
                        targetStat.DispellTimerBuff();
                        continue;
                    }
                    
                    var condFlag = CheckAffliction(attackStat.withConditionChance[i],
                        targetStat.GetAfflictionResistance
                            ((BasicCalculation.BattleCondition)attackStat.withConditions[i].buffID));
                    if (condFlag<1)
                    {
                        DamageNumberManager.GenerateResistText(target.transform);
                        continue;//检查异常抗性，这里还没有获取Resistance！！不一定是异常！
                    }


                    var newEffect = attackStat.withConditions[i].effect;
                    //print(newEffect);
                    if (StatusManager.IsDotAffliction(attackStat.withConditions[i].buffID))
                        newEffect = 5f / 300f * newEffect * BasicCalculation.CalculateAttackInfo(selfstat) /
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
        
        //击退Knockback
        // if (attackStat.knockbackPower > 0)
        // {
        //     var kbdirtemp = attackStat.knockbackDirection;
        //     kbdirtemp = attackStat.GetKBDirection(attackStat.KBType, target);
        //     target.GetComponentInParent<IKnockbackable>().
        //         TakeDamage(attackStat.knockbackPower,attackStat.knockbackTime,attackStat.knockbackForce,kbdirtemp);
        // }

        

        //Damage
        targetStat.currentHp -= totalDamage;
        targetStat.OnHPChange?.Invoke();
        
        return totalDamage;
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

    /// <summary>
    ///   <para>返回obj脚下踩着的地面</para>
    /// </summary>
    public Collider2D GetContactGround(GameObject obj)
    {
        return null;
    }

    public void SetGamePause(bool flag)
    {
        isGamePaused = flag;
        Time.timeScale = flag ? 0 : 1;
        
    }

    public void SetGameFailed()
    {
        if(gameResultRoutine==null)
            gameResultRoutine =
                StartCoroutine(GameFailedRoutine());
    }

    IEnumerator GameFailedRoutine()
    {
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
        
        
        Time.timeScale = 0.5f;
        GameObject.Find("CharacterInfo").SetActive(false);
        //
        StageCameraController.SwitchMainCamera();
        StageCameraController.SwitchMainCameraFollowObject(lastEnemyEliminated);
        StageCameraController.SetMainCameraSize(6);
        yield return new WaitForSeconds(.8f);

        Time.timeScale = 1;
        StageCameraController.SwitchMainCameraFollowObject(player);
        StageCameraController.SetMainCameraSize(8);

        yield return new WaitForSeconds(3f);
        var targetTransform = GameObject.Find("UIFXContainer").transform;
        var clearGameObject = Instantiate(gameClearPrefab,
            Camera.main.transform.position+new Vector3(0,0,-5),
            Quaternion.identity,
            targetTransform);
        
        
        GameObject.Find("BossStatusBar").SetActive(false);
        var playerinput = player.GetComponent<PlayerInput>();
        playerinput.DisableAllInput();
        playerinput.SetMoveDisabled();
        playerinput.enabled = false;
        var playercontroller = player.GetComponent<ActorController>();
        playercontroller.anim.SetFloat("forward",0);
        yield return null;
        playercontroller.anim.Play("idle");
        playercontroller.enabled = false;
        
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

}