using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using LitJson;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleStageManager : MonoBehaviour
{
    private Coroutine gameResultRoutine;
    
    private AssetBundle assetBundle;
    [Header("Level INFO")]
    public int chara_id;
    public string quest_name;
    public string quest_id { get; set; }
    public GameObject boss;
    public int timeLimit;
    public int totalEnemyNum;
    public int currentEnemyNum { get; private set; }
    public int maxReviveTime = 3; //最大复活次数
    public int crownReviveTime = 0;// 得到第三颗星最大复活次数
    public int crownTimeLimit = 300;//得到第二颗星所需的时间
    
    


    private DamageNumberManager damageNumberManager;
    private DamageNumberManager dnm;
    
    [Header("Common")]
    public GameObject buffLogPrefab;
    public GameObject gameFailedPrefab;
    public GameObject gameClearPrefab;
    public AudioClip gameFailedBGM;
    public AudioClip gameClearBGM;
    public GameObject resultPage;
    
    private GameObject player;
    public GameObject lastEnemyEliminated { get; private set; }
    
    public float mapBorderL { get; private set; }
    public float mapBorderR { get; private set; }
    public float mapBorderT { get; private set; }

    public bool isGamePaused { get; private set; }

    public float currentTime { get; private set; } = 0;

    private void Awake()
    {
        //LoadDependency("ui_general");
        //LoadPlayer(1);
        //FindPlayer();
        LinkBossStatus();
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
        var bossStat = GameObject.Find("UI")?.transform.Find("BossStatusBar")?.gameObject;
        if(bossStat == null)
            return;
        
        bossStat.GetComponentInChildren<UI_BossStatus>()?.SetBoss(boss);
        bossStat.SetActive(true);
        
    }

    public void SpChargeAll(GameObject playerHandle, float sp)
    {
        var playerStatusManager = playerHandle.GetComponent<PlayerStatusManager>();

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

    public virtual int PlayerHit(GameObject target, AttackFromPlayer attackStat)
    {
        

        //GameObject player = GameObject.Find("PlayerHandle");
        
        //print("进了playerhit");

        //1.If target is not in invincible state.
        if (!target.transform.Find("HitSensor").GetComponent<Collider2D>().isActiveAndEnabled) return -1;


        //Attack Callback
        switch (attackStat.attackType)
        {
            case BasicCalculation.AttackType.STANDARD:
                player.GetComponent<ActorController>().OnStandardAttackConnect(attackStat);
                break;
            case BasicCalculation.AttackType.SKILL:
                player.GetComponent<ActorController>().OnSkillConnect(attackStat);
                break;
            case BasicCalculation.AttackType.OTHER:
                player.GetComponent<ActorController>().OnOtherAttackConnect(attackStat);
                break;
        }

        var targetStat = target.GetComponentInChildren<StatusManager>();

        var damageM = new int[attackStat.GetHitCount()];

        var totalDamage = 0;

        var playerstat = player.GetComponentInChildren<PlayerStatusManager>();

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

            player.GetComponent<PlayerStatusManager>().ComboConnect();
        }

        var container = attackStat.GetComponentInParent<AttackContainer>();

        
        
        //Affliction/Debuff
        if (attackStat.withConditions.Count > 0)
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
            }

        //KnockBack 击退
        var kbtemp = attackStat.knockbackDirection;
        kbtemp = attackStat.GetKBDirection(attackStat.KBType, target);
        target.GetComponentInParent<EnemyController>().
            TakeDamage(attackStat.knockbackPower,
                attackStat.knockbackTime, 
                attackStat.knockbackForce, kbtemp);

        //5.Calculate the SP
        

        if (!container.spGained)
        {
            SpChargeAll(player, attackStat.GetSpGain());
            container.spGained = true;
        }


        //Enemy Take Damage

        targetStat.currentHp -= totalDamage;
        


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

        return damageM;
    }

    public int EnemyHit(GameObject target, GameObject self, AttackFromEnemy attackStat)
    {
        //1.If target is not in invincible state.
        if (target.transform.Find("HitSensor").GetComponent<Collider2D>().isActiveAndEnabled == false) return -1;

        //Attack Callback

        var targetStat = target.GetComponentInParent<PlayerStatusManager>();
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
        if (attackStat.withConditions.Count > 0)
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
            }
        
        //击退Knockback
        if (attackStat.knockbackPower > 99)
        {
            var kbdirtemp = attackStat.knockbackDirection;
            kbdirtemp = attackStat.GetKBDirection(attackStat.KBType, target);
            target.GetComponentInParent<ActorController>().
                TakeDamage(attackStat.knockbackTime,attackStat.knockbackForce,kbdirtemp);
        }

        

        //Damage
        targetStat.currentHp -= totalDamage;
        
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
        print(chance+"chance");
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
        yield return new WaitForSeconds(1.0f);

        Time.timeScale = 1;
        StageCameraController.SwitchMainCameraFollowObject(player);
        StageCameraController.SetMainCameraSize(8);

        yield return new WaitForSeconds(3f);
        var targetTransform = GameObject.Find("UIFXContainer").transform;
        var clearGameObject = Instantiate(gameClearPrefab,
            Camera.main.transform.position+new Vector3(0,0,0),
            Quaternion.identity,
            targetTransform);
        
        
        GameObject.Find("BossStatusBar").SetActive(false);
        var playerinput = player.GetComponent<PlayerInput>();
        playerinput.DisableAllInput();
        playerinput.SetMoveDisabled();
        playerinput.enabled = false;
        player.GetComponent<Animator>().SetFloat("forward",0);
        yield return null;
        player.GetComponent<Animator>().Play("idle");
        
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

        print(newQuestState.best_clear_time);
        
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
        print(savedata.best_clear_time);
        print(newRecord);
        //print(savedata.best_clear_time);
                
            
        

        
        
        string jsonStr = JsonMapper.ToJson(datalist);
        string filePath = Application.streamingAssetsPath + "/savedata/testSaveData.json";
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(jsonStr);
        sw.Close();


    }




}