using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Cinemachine;
using DG.Tweening;
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
    public int currentEnemyInLayerDeadAlive => EnemyLayer.transform.childCount;
    public int maxReviveTime = 3; //最大复活次数
    public int crownReviveTime = 0;// 得到第三颗星最大复活次数
    public int crownTimeLimit = 300;//得到第二颗星所需的时间
    public int clearConditionType;//0:击倒BOSS，1:无
    public int loseControllTime { get; set; } = 0;
    private float _currentTimeScale = 1;
    private Tween _timeScaleTween;


    //private DamageNumberManager damageNumberManager;
    protected DamageNumberManager dnm;
    protected List<Platform> platforms;

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

    private GameObject _mapBorderLGO;
    private GameObject _mapBorderRGO;
    private GameObject _mapBorderTGO;
    private GameObject _mapBorderBGO;
    
    
    
    
    
    
    protected PolygonCollider2D cameraRange;

    public bool isGamePaused { get; private set; }

    public float currentTime { get; private set; } = 0;

    public static int currentDisplayingBossInfo = 1;//正在显示的boss信息

    public Action OnMapInfoRefresh;
    public Action OnGameStart;
    
    public Action<string> OnQuestCleared;
    /// <summary>
    /// 退出任务时调用，参数为(任务ID)
    /// </summary>
    public Action<string> OnQuestQuit;
    
    public Action<int> OnMapEventTriggered;


    public delegate void StageManagerIntegerDelegate(int id);

    public StageManagerIntegerDelegate OnFieldAbilityAdd;
    public StageManagerIntegerDelegate OnFieldAbilityRemove;
    public StageManagerIntegerDelegate OnEnemyAwake;
    public StageManagerIntegerDelegate OnEnemyEliminated;
    public event Action<int> specialEventTriggered;
    public event Action<int, StatusManager, EnemyAbilityIconEvent> OnFieldAbilityEvent;

    public Action<AttackBase> OnAttackAwake;

    public event Action<int, int> OnSkillIconSwapEvent;

    // public delegate void OnMouseOverDelegate();
    // public event OnMouseOverDelegate OnPointerEnter;
    // public event OnMouseOverDelegate OnPointerExit;
    
    public bool PlayerViewEnable { get; set; } = true;
    public bool DragonBlock { get; set; } = false;
    
    public List<int> EnemyList { get; private set; } = new();


    public GameObject RangedAttackFXLayer { get; private set; }
    public GameObject EnemyLayer { get; private set; }
    
    public GameObject PlayerLayer { get; private set; }
    
    public GameObject UILayer { get; private set; }
    




    private void Awake()
    {
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
        EnemyLayer = GameObject.Find("EnemyLayer");
        PlayerLayer = GameObject.Find("Player");

        ResetEnemyList();
        
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (GlobalController.currentGameState == GlobalController.GameState.Inbattle)
        {
            if (currentTime == 0)
            {
                OnGameStart?.Invoke();
            }
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

        if (buffLayer.childCount == 0)
        {
            var bufftxt = 
                         Instantiate(buffLogPrefab, buffLayer.position + new Vector3(0, 2), Quaternion.identity, buffLayer);
        }

        

        //StartCoroutine(开场buff(player));
    }

    /// <summary>
    /// 初始化玩家对象，设置玩家远程攻击特效层引用，并初始化玩家Buff日志UI（若未存在）
    /// </summary>
    /// <param name="plr">要初始化的玩家游戏对象</param>
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

    /// <summary>
    /// 获取当前场景中的玩家游戏对象实例
    /// </summary>
    /// <returns>当前玩家的GameObject实例</returns>
    public GameObject GetPlayer()
    {
        return player;
    }
    
    /// <summary>
    /// 设置当前场景的玩家游戏对象
    /// </summary>
    /// <param name="plr">要设置为当前玩家的GameObject实例</param>
    public void SetPlayer(GameObject plr)
    {
        player = plr;
    }


    #endregion

    public LevelDetailedInfo GetLevelInfo()
    {
        return levelDetailedInfo;
    }
    
    /// <summary>
    /// 获取当前关卡的详细信息对象（LevelDetailedInfo）
    /// </summary>
    /// <returns>当前关卡的LevelDetailedInfo实例</returns>
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

    /// <summary>
    /// 加载剧情关卡的详细信息，包含角色ID、关卡名称、时间限制、复活次数等（数值做上限限制）
    /// </summary>
    /// <param name="cid">角色ID</param>
    /// <param name="info">剧情关卡详细信息对象（StoryLevelDetailedInfo）</param>
    public void LoadStoryLevelDetailedInfo(int cid, StoryLevelDetailedInfo info)
    {
        chara_id = cid;
        quest_name = info.name;
        timeLimit = (int)info.time_limit;
        maxReviveTime = info.revive_limit > 10 ? 10 : info.revive_limit;
        crownTimeLimit = (int)(info.crown_time_limit>600?600:info.crown_time_limit);
        crownReviveTime = info.crown_revive_limit>maxReviveTime?maxReviveTime:info.crown_revive_limit;
        clearConditionType = info.clear_condition;
    }

    /// <summary>
    /// 为调试场景加载关卡详细信息，逻辑同常规关卡但不激活开始界面
    /// </summary>
    /// <param name="cid">角色ID</param>
    /// <param name="info">关卡详细信息对象（LevelDetailedInfo）</param>
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

    /// <summary>
    /// 获取当前关卡中敌人（BOSS）所需的资源依赖列表，汇总BOSS预制体的resources字段
    /// </summary>
    /// <returns>包含所有敌人资源依赖路径的字符串列表</returns>
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
    /// <para>加载地图边界</para>
    /// </summary>
    public void GetMapBorderInfo()
    {
        var borderInfoL = GameObject.Find("BorderLeft");
        var borderInfoR = GameObject.Find("BorderRight");
        var borderInfoT = GameObject.Find("BorderTop");
        var borderInfoB = GameObject.FindGameObjectWithTag("Ground");

        _mapBorderBGO = borderInfoB;
        _mapBorderLGO = borderInfoL;
        _mapBorderRGO = borderInfoR;
        _mapBorderTGO = borderInfoT;

        mapBorderL = borderInfoL.GetComponent<BoxCollider2D>().offset.x +
                     borderInfoL.GetComponent<BoxCollider2D>().size.x * 0.5f + borderInfoL.transform.position.x;
        mapBorderR = borderInfoR.GetComponent<BoxCollider2D>().offset.x -
            borderInfoR.GetComponent<BoxCollider2D>().size.x * 0.5f + borderInfoR.transform.position.x;
        mapBorderT = borderInfoT.GetComponent<BoxCollider2D>().offset.y -
                     borderInfoT.GetComponent<BoxCollider2D>().size.y * 0.5f + borderInfoT.transform.position.y;
        mapBorderB = borderInfoB.GetComponent<BoxCollider2D>().bounds.max.y;
    }

    /// <summary>
    /// 设置地图左边界位置，调整边界对象坐标，可选刷新敌人行为
    /// </summary>
    /// <param name="value">新的左边界数值</param>
    /// <param name="refreshEnemyBehavior">是否通知所有敌人重新加载地图信息（默认false）</param>
    public void SetLeftBorder(float value, bool refreshEnemyBehavior = false)
    {
        var borderInfoL = _mapBorderLGO != null ? _mapBorderLGO :
            GameObject.Find("BorderLeft");
        
            // 获取BoxCollider2D组件
        BoxCollider2D boxCollider = borderInfoL.GetComponent<BoxCollider2D>();

            // 计算BoxCollider2D的宽度
        float width = boxCollider.size.x * transform.localScale.x;

            // 计算新的位置
        float newX = value - width / 2 + boxCollider.offset.x * transform.localScale.x;

            // 设置新的位置
        borderInfoL.transform.position = new Vector3(newX, borderInfoL.transform.position.y, borderInfoL.transform.position.z);
        
        if (refreshEnemyBehavior)
        {
            OnMapInfoRefresh?.Invoke();
        }

        mapBorderL = value;

    }
    
    /// <summary>
    /// 设置地图右边界位置，调整边界对象坐标，可选刷新敌人行为
    /// </summary>
    /// <param name="value">新的右边界数值</param>
    /// <param name="refreshEnemyBehavior">是否通知所有敌人重新加载地图信息（默认false）</param>
    public void SetRightBorder(float value, bool refreshEnemyBehavior = false)
    {
        var borderInfoR = _mapBorderRGO != null ? _mapBorderRGO :
            GameObject.Find("BorderRight");
        
        // 获取BoxCollider2D组件
        BoxCollider2D boxCollider = borderInfoR.GetComponent<BoxCollider2D>();

        // 计算BoxCollider2D的宽度
        float width = boxCollider.size.x * transform.localScale.x;

        // 计算新的位置
        float newX = value + width / 2 - boxCollider.offset.x * transform.localScale.x;

        // 设置新的位置
        borderInfoR.transform.position = new Vector3(newX, borderInfoR.transform.position.y, borderInfoR.transform.position.z);

        if (refreshEnemyBehavior)
        {
            OnMapInfoRefresh?.Invoke();
        }
        
        mapBorderR = value;
    }

    /// <summary>
    /// 设置地图上边界位置，调整边界对象坐标，可选刷新敌人行为
    /// </summary>
    /// <param name="value">新的上边界数值</param>
    /// <param name="refreshEnemyBehavior">是否通知所有敌人重新加载地图信息（默认false）</param>
    public void SetTopBorder(float value, bool refreshEnemyBehavior = false)
    {
        var borderInfoT = _mapBorderTGO != null ? _mapBorderTGO :
            GameObject.Find("BorderTop");
        
        BoxCollider2D boxCollider = borderInfoT.GetComponent<BoxCollider2D>();

        // 计算BoxCollider2D的高度
        float height = boxCollider.size.y * transform.localScale.y;

        // 计算新的位置
        float newY
            = value + height / 2 - boxCollider.offset.y * transform.localScale.y;

        // 设置新的位置
        borderInfoT.transform.position = new Vector3(borderInfoT.transform.position.x,newY,
            borderInfoT.transform.position.z);

        if (refreshEnemyBehavior)
        {
            OnMapInfoRefresh?.Invoke();
        }
        
        mapBorderT = value;
    }

    /// <summary>
    /// 设置相机视野左边界，调整相机范围碰撞体（PolygonCollider2D）顶点坐标
    /// </summary>
    /// <param name="value">新的相机左边界数值</param>
    /// <remarks>碰撞体顶点数超4个时会输出警告</remarks>
    public void SetCameraLeftBorder(float value)
    {
        if (cameraRange == null)
        {
            cameraRange = GameObject.Find("CameraRange").GetComponent<PolygonCollider2D>();
        }

        PolygonCollider2D polygonCollider = cameraRange;
        Vector2[] points = polygonCollider.points;
        if (points.Length > 4)
        {
            Debug.LogWarning("CameraRange has more than 4 points, use SetCameraBorder instead.");
            return;
        }
        Vector2[] sortedPoints = points.OrderBy(point => point.x).ToArray();
        int index1 = Array.IndexOf(points, sortedPoints[0]);
        int index2 = Array.IndexOf(points, sortedPoints[1]);
        points[index1].x = value;
        points[index2].x = value;
        polygonCollider.points = points;

    }
    
    /// <summary>
    /// 设置相机视野上边界，调整相机范围碰撞体（PolygonCollider2D）顶点坐标
    /// </summary>
    /// <param name="value">新的相机上边界数值</param>
    /// <remarks>碰撞体顶点数超4个时会输出警告</remarks>
    public void SetCameraTopBorder(float value)
    {
        if (cameraRange == null)
        {
            cameraRange = GameObject.Find("CameraRange").GetComponent<PolygonCollider2D>();
        }

        PolygonCollider2D polygonCollider = cameraRange;
        Vector2[] points = polygonCollider.points;
        if (points.Length > 4)
        {
            Debug.LogWarning("CameraRange has more than 4 points, use SetCameraBorder instead.");
            return;
        }
        Vector2[] sortedPoints = points.OrderBy(point => point.y).ToArray();
        int index1 = Array.IndexOf(points, sortedPoints[2]);
        int index2 = Array.IndexOf(points, sortedPoints[3]);
        points[index1].y = value;
        points[index2].y = value;
        polygonCollider.points = points;
        
        
    }
    
    /// <summary>
    /// 设置相机视野右边界，调整相机范围碰撞体（PolygonCollider2D）顶点坐标
    /// </summary>
    /// <param name="value">新的相机右边界数值</param>
    /// <remarks>碰撞体顶点数超4个时会输出警告</remarks>
    public void SetCameraRightBorder(float value)
    {
        if (cameraRange == null)
        {
            cameraRange = GameObject.Find("CameraRange").GetComponent<PolygonCollider2D>();
        }

        PolygonCollider2D polygonCollider = cameraRange;
        Vector2[] points = polygonCollider.points;

        if (points.Length > 4)
        {
            Debug.LogWarning("CameraRange has more than 4 points, use SetCameraBorder instead.");
            return;
        }

        Vector2[] sortedPoints = points.OrderBy(point => point.x).ToArray();
        int index1 = Array.IndexOf(points, sortedPoints[2]);
        int index2 = Array.IndexOf(points, sortedPoints[3]);
        points[index1].x = value;
        points[index2].x = value;
        polygonCollider.points = points;
    }

    /// <summary>
    /// 刷新相机边界缓存，重置CinemachineConfiner2D缓存，重新获取地图边界并触发信息刷新
    /// </summary>
    public void RefreshCameraBorder()
    {
        var confiners = FindObjectsOfType<CinemachineConfiner2D>();

        print(confiners.Length);

        foreach (var confiner in confiners)
        {
            print("Confiner:"+confiner.gameObject);
            confiner.InvalidateCache();
        }

        GetMapBorderInfo();
        print(mapBorderL);
        print(mapBorderR);
        
        OnMapInfoRefresh?.Invoke();
    }


    /// <summary>
    /// 初始化并关联BOSS状态：实例化开局加载的BOSS预制体、设置当前显示的BOSS信息ID、激活BOSS状态栏UI
    /// </summary>
    /// <remarks>默认取第一个实例化的BOSS，多BOSS场景需适配</remarks>
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

    /// <summary>
    /// 初始化关卡背景音乐：从指定AssetBundle加载BGM，仅当未设置过BGM时生效
    /// </summary>
    public void InitBGM()
    {
        
        var globalController = GameObject.Find("GlobalController").GetComponent<GlobalController>();
        var bundle = globalController.GetBundle(levelDetailedInfo.bgm_path);
        
        //print(GlobalController.Instance.loadedBundles.Count);
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

    /// <summary>
    /// 从指定AssetBundle加载背景音乐音频资源
    /// </summary>
    /// <param name="assetBundlePath">AssetBundle路径</param>
    /// <param name="name">BGM资源名称</param>
    /// <returns>加载到的AudioClip（未找到返回null并输出警告）</returns>
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

    /// <summary>
    /// 为玩家所有技能充能，计算技速BUFF、场地技能倍率等加成后分配SP
    /// </summary>
    /// <param name="ps">玩家状态管理器（PlayerStatusManager）实例</param>
    /// <param name="sp">基础SP充能值</param>
    public void SpChargeAll(PlayerStatusManager ps, float sp)
    {
        var playerStatusManager = ps;
        
        if(playerStatusManager == null)
            return;

        //1、计算技速BUFF

        var spBuff = playerStatusManager.skillHasteUp;
        var spAbility = AbilityCalculation
            .GetAbilityAmountInfo
                (ps, null, null, AbilityCalculation.ProductArea.SKLRATE).Item1;
        //var spAbility = BasicCalculation.CheckSpecialSkillRateEffect(playerStatusManager, null).Item1;
        //TODO: 计算技速场地效果
        var spGain = sp * (1 + spBuff);

        //2、自充sp

        for (var i = 0; i < playerStatusManager.maxSkillNum; i++) 
            playerStatusManager.SpGainInStatus(i, spGain);
    }
    
    /// <summary>
    /// 为玩家指定技能充能，仅对指定ID的技能增加SP值
    /// </summary>
    /// <param name="playerHandle">玩家游戏对象</param>
    /// <param name="sp">基础SP充能值</param>
    /// <param name="skillID">要充能的技能ID，0为起点</param>
    public void SpCharge(GameObject playerHandle, float sp, int skillID)
    {
        var playerStatusManager = playerHandle.GetComponent<PlayerStatusManager>();


        playerStatusManager.SpGainInStatus(skillID, sp);
    }

    #region DamageModule
    
    
    /// <summary>
    /// 攻击结算核心方法：计算伤害、处理无敌/护盾/生命护盾、生成伤害数字、附加异常/减益状态等
    /// </summary>
    /// <param name="target">受攻击目标游戏对象</param>
    /// <param name="player">攻击发起者游戏对象</param>
    /// <param name="attackStat">攻击属性对象（AttackBase）</param>
    /// <param name="attackType">攻击类型（0：玩家对敌人；1：敌人对玩家/NPC；2：NPC对敌人）</param>
    /// <returns>-1表示目标无敌，否则返回总伤害值</returns>
    /// <remarks>处理BOSS部位伤害同步、暴击判定、异常抗性检查等复杂逻辑</remarks>
    public int CalculateHit(GameObject target, GameObject player, AttackBase attackStat, int attackType = 0)
    {
        
        //1.If target is not in invincible state.
        if (!target.transform.Find("HitSensor").GetComponent<Collider2D>().isActiveAndEnabled) return -1;
        
        
        var targetStat = target.GetComponentInChildren<StatusManager>();

        var damageM = new int[attackStat.GetHitCountInfo()];

        var totalDamage = 0;

        var playerstat = player.GetComponentInChildren<StatusManager>();
        
        if (targetStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.Invincible) <= 0)
        {
            attackStat.BeforeAttackHit?.Invoke(attackStat,target);
        }
        
        // Check if the target is a partStatusManager(检查目标是否是boss的一个部位)
        PartStatusManager targetPartStat = null;
        bool damageSync = false;//是否把部位的伤害同步到主体上
        bool doDamage = true; //如果由于部位已经反射了伤害，就不再计算伤害
        int targetInstanceID = target.transform.GetInstanceID();
        
        // 转移目标计算
        if (attackStat is AttackFromPlayer)
        {
            AttackFromPlayer attackFromPlayer = attackStat as AttackFromPlayer;
            if (targetStat is PartStatusManager)
            {
                //如果是boss部位，转移伤害计算
                targetPartStat = targetStat as PartStatusManager;
                targetStat = targetPartStat.mainStatus;
                
                //如果boss本体已经被攻击过,同步伤害设置为false
                if(attackFromPlayer.hitConnectedFlags.Contains(targetStat.transform.GetInstanceID()))
                {
                    damageSync = false;
                }else
                {
                    //如果boss本体没有被攻击过，同步伤害设置为true
                    damageSync = true;
                    attackFromPlayer.hitConnectedFlags.Add(targetStat.transform.GetInstanceID());
                }

            }
            else if(targetStat.partList.Count > 0)
            {
                //如果是boss的主体，且有部位
                foreach (var part in targetStat.partList)
                {
                    //假设部位已经被攻击过，相当于已经反射了伤害，本次不计算伤害
                    if (attackFromPlayer.hitConnectedFlags.Contains(part.transform.GetInstanceID()))
                    {
                        doDamage = false;
                    }
                }
            }
        }
        

        

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

        var shield = targetStat.GetConditionOfTypeWithMaxEffect((int)BasicCalculation.BattleCondition.Shield);
        var lifeShield = targetStat.GetConditionOfTypeWithMaxEffect((int)BasicCalculation.BattleCondition.LifeShield);

        //2-4 : Calculate the damage in a loop.
        for (var i = 0; i < attackStat.GetHitCountInfo(); i++)
        {
            //2.Calculate the damage deal to target.

            var isCrit = false;


            int extraDamage = (int)attackStat.GetDmgConstInfo(i);
            int damageCutConst = (int)targetStat.GetDamageCutConst();

            var damage =
                BasicCalculation.CalculateDamageGeneral(
                    playerstat,
                    targetStat,
                    attackStat.GetDmgModifierInfo(i),
                    attackStat,
                    ref isCrit,
                    ref extraDamage
                );

            damageM[i] = (int)Mathf.Ceil(damage * Random.Range(0.95f, 1.05f)) +
                         extraDamage - damageCutConst;
            
            if(isCrit)
                playerstat.OnCriticalHit?.Invoke(attackStat, i);
            
            if(damageM[i]<0) damageM[i] = 0;

            //3.Special Effect(结算生命护盾、护盾)
            
            if (targetStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.Invincible) > 0)
            {
                damageM[i] = 0;
            }

            
            if (shield != null)
            {
                if (damageM[i] <= 0.01f * shield.effect * targetStat.maxHP)
                {
                    damageM[i] = 0;
                }
            }

            if (lifeShield != null)
            {
                if (damageM[i] <= lifeShield.effect)
                {
                    lifeShield.SetEffect(lifeShield.effect - damageM[i]);
                    damageM[i] = 0;
                }
                else
                {
                    damageM[i] -= (int)lifeShield.effect;
                    targetStat.RemoveTimerBuff((int)BasicCalculation.BattleCondition.LifeShield);
                    lifeShield = null;
                }
            }


            targetStat.OnTakeDirectDamage?.Invoke(targetStat);
            targetStat.OnTakeDirectDamageFrom?.Invoke(targetStat,playerstat,attackStat,damageM[i]);
            attackStat.OnAttackDealDamage?.Invoke(playerstat,targetStat,attackStat,damageM[i]);


            //4.Instantiate the damage number.

            if ((doDamage && !targetPartStat) || (targetPartStat && damageSync))
            {
                if (attackType == 0)
                {
                    if (isCrit)
                        dnm.DamagePopEnemy(target.transform, damageM[i], 2,1,
                            targetPartStat?targetPartStat:targetStat,attackStat,
                            targetPartStat?targetPartStat.height:targetStat.height);
                    else
                        dnm.DamagePopEnemy(target.transform, damageM[i], 1,1,
                            targetPartStat?targetPartStat:targetStat,attackStat,
                            targetPartStat?targetPartStat.height:targetStat.height);
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
                        dnm.DamagePopEnemy(target.transform, damageM[i], 2,0.5f,
                            targetPartStat?targetPartStat:targetStat,attackStat,
                            targetPartStat?targetPartStat.height:targetStat.height);
                    else
                        dnm.DamagePopEnemy(target.transform, damageM[i], 1,0.5f,
                            targetPartStat?targetPartStat:targetStat,attackStat,
                            targetPartStat?targetPartStat.height:targetStat.height);
                }
            }
            else
            {
                if (attackType == 0)
                {
                    dnm.DamagePopEnemy(target.transform, 0, 1,1,
                        targetPartStat?targetPartStat:targetStat,attackStat,
                            targetPartStat?targetPartStat.height:targetStat.height);
                }
                else if(attackType == 1)
                {
                    dnm.DamagePopPlayer(target.transform, 0, false);
                }
                else
                {
                    dnm.DamagePopEnemy(target.transform, 0, 1,0.5f,
                        targetPartStat?targetPartStat:targetStat,attackStat,
                            targetPartStat?targetPartStat.height:targetStat.height);
                }
            }
            
            

            totalDamage += damageM[i];

            player.GetComponent<StatusManager>()?.ComboConnect();
        }

        if (shield != null)
        {
            targetStat.RemoveConditionWithoutLog(shield);
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
                    if(doDamage == false)break;
                    if(targetPartStat && damageSync == false)break;
                    
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
                    print(attackStat.attackInfo[0].withConditions.Count);
                    
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

                        //龙化免疫异常状态
                        if (targetStat is PlayerStatusManager)
                        {
                            if ((targetStat as PlayerStatusManager).isShapeshifting &&
                                (targetStat as PlayerStatusManager).shapeShiftingImmuneToDebuff)
                            {
                                condFlag = -2;
                            }
                        }
                        
                        //0伤害免疫异常状态
                        if (totalDamage <= 0)
                            condFlag = -2;
                        
                        if (condFlag<1)
                        {
                            playerstat.OnAfflictionResist?.Invoke(withCondition.condition);
                            targetStat.OnAfflictionGuarded?.Invoke(withCondition.condition);
                            //1是成功,0是白字resist,-1是黄字resist
                            if(condFlag == 0)
                                DamageNumberManager.GenerateResistText(target.transform,0,targetStat.height);
                            else if(condFlag == -1)
                            {
                                DamageNumberManager.GenerateResistText(target.transform, 1,targetStat.height);
                            }
                        
                            continue;//检查异常抗性！不一定是异常！
                        }
                        
                        playerstat.OnAfflictionInflict?.Invoke(withCondition.condition);
                        
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
                            targetStat.DebuffResistance);
                        if (targetStat is PlayerStatusManager)
                        {
                            var psm = targetStat as PlayerStatusManager;
                            //龙化免疫大部分减益
                            if (psm.isShapeshifting && psm.shapeShiftingImmuneToDebuff && withCondition.condition.dispellable == true
                                && !StatusManager.IsBuff(withCondition.condition.buffID))
                            {
                                condFlag = -2;
                            }
                        }
                        if (condFlag < 1)
                        {
                            //failed
                            continue;
                        }
                        else
                        {
                            playerstat.OnConditionInflict?.Invoke(withCondition.condition);
                        }
                    }



                    //TODO:异常数值不完善
                    
                    var newEffect = withCondition.condition.effect;
                    //print(newEffect);
                    if (StatusManager.IsDotAffliction(withCondition.condition.buffID))
                        newEffect = 5f / 300f * newEffect *
                                    BasicCalculation.CalculateAttackInfo(attackStat,playerstat,targetStat) /
                                    BasicCalculation.CalculateDefenseInfo(targetStat, playerstat, attackStat);
                    
                    
                    

                    
                    if (withCondition.condition.maxStackNum > 1)
                    {
                        if (withCondition.condition.buffID == (int)BasicCalculation.BattleCondition.Bleeding)
                        {
                            var effects = BasicCalculation.CalculateDamage
                            (playerstat,
                                targetStat, withCondition.condition.effect, attackStat);
                            
                            //Debug.LogWarning("Effect: "+effects.standardDamage+" CritRate: "+effects.criticalRate+" CritDamage: "+effects.criticalDamage);
                            
                            targetStat.ObtainTimerBuff
                            (withCondition.condition.buffID,
                                effects.standardDamage,
                                withCondition.condition.duration,
                                withCondition.condition.maxStackNum,
                                withCondition.condition.specialID,
                                withCondition.condition.dispellable,
                                effects.criticalRate,
                                effects.criticalDamage);
                        }
                        else
                        {
                            targetStat.ObtainTimerBuff
                            (withCondition.condition.buffID,
                                newEffect,
                                withCondition.condition.duration,
                                withCondition.condition.maxStackNum,
                                withCondition.condition.specialID,
                                withCondition.condition.dispellable);
                        }
                        
                        
                    }
                    else
                    {
                        if (withCondition.condition is AdvancedTimerBuff)
                        {
                            targetStat.ObtainUnstackableTimerBuff
                            (   withCondition.condition.buffID,
                                newEffect,
                                withCondition.condition.duration,
                                withCondition.condition.specialID,
                                (withCondition.condition as AdvancedTimerBuff).effect2,
                                (withCondition.condition as AdvancedTimerBuff).effect3
                            );
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
        
        
        if(totalDamage > 0 && doDamage)
            target.GetComponentInParent<ActorBase>().
                TakeDamage(attackStat,kbtemp);

        //7. Calculate the SP
        if (playerstat is PlayerStatusManager)
        {
            if (!container.spGained)
            {
                SpChargeAll(playerstat as PlayerStatusManager, ((AttackFromPlayer)attackStat).GetSpGain());
                (playerstat as PlayerStatusManager).OnAttackGainSP?.Invoke(attackStat,((AttackFromPlayer)attackStat).GetSpGain());
                container.spGained = true;
            }
        }
        
        //8. Enemy Take Damage

        if (doDamage)
        {
            //如果不是特殊部位
            if (targetPartStat)
            {
                if (damageSync)
                {
                    targetStat.currentHp -= totalDamage;
                    targetStat.OnHPChange?.Invoke();
                    targetStat.OnHPDecrease?.Invoke(totalDamage, attackStat);
                }
            }
            else
            {
                targetStat.currentHp -= totalDamage;
                targetStat.OnHPChange?.Invoke();
                targetStat.OnHPDecrease?.Invoke(totalDamage, attackStat);
            }
        }
        
        if (targetPartStat)
        {
            targetPartStat.currentHp -= totalDamage;
            targetPartStat.OnHPChange?.Invoke();
            targetPartStat.OnHPDecrease?.Invoke(totalDamage, attackStat);
        }
        

        // 9. Reduce enemy's Overdrive Gauge
        if (targetStat is SpecialStatusManager && doDamage)
        {
            var targetSpecialStat = (SpecialStatusManager) targetStat;
            if (targetSpecialStat.baseBreak > 0)
            {
                float ODModifier = Mathf.Clamp(attackStat.extraODModifier, -0.5f,100);
                if (container.IfODCounter)
                {
                    ODModifier += (0.8f + 0.02f * Mathf.Sqrt(Mathf.Abs(attackStat.attackInfo[0].knockbackPower-100)));
                    ODModifier += targetSpecialStat.counterModifier;
                }

                //Calculate OD Gauge Punisher
                var ODpunisher = 1 + playerstat.ODAccerator +
                                 AbilityCalculation.GetAbilityAmountInfo(playerstat, targetStat, attackStat,
                                     AbilityCalculation.ProductArea.ODACC).result;
                
                //BasicCalculation.CheckSpecialODAccerleratorEffect(playerstat,targetSpecialStat,attackStat);
                if (targetSpecialStat.ODLock == false)
                {
                    targetSpecialStat.currentBreak -= totalDamage * (Random.Range(0.9f, 1.1f) + ODModifier) * ODpunisher;
                }
            }

        }
        
        //10、Special Field Effects


        if (doDamage)
        {
            CheckSpecialFieldEffect(attackStat, playerstat, targetStat, attachedConditions,damageM);
        }
        

        return totalDamage;
        
        
    }

    /// <summary>
    /// 结算间接伤害（如DOT、自烧血、强制固伤等），计算伤害数值、生成伤害数字并同步目标状态变化
    /// </summary>
    /// <param name="stat">目标的StatusManager实例</param>
    /// <param name="damage">基础伤害值</param>
    /// <param name="causeDeath">是否允许该伤害导致目标死亡</param>
    /// <param name="random">是否应用随机伤害波动（0.95-1.05倍）</param>
    /// <param name="ignoreLifeShield">是否可以无视目标的生命护盾扣血</param>
    /// <returns>实际造成的伤害值</returns>
    public int CauseIndirectDamage(StatusManager stat, int damage,bool causeDeath, bool random = false, bool ignoreLifeShield = false)
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

        var lifeShield = stat.GetConditionOfTypeWithMaxEffect((int)BasicCalculation.BattleCondition.LifeShield);
        
        if (lifeShield != null && !ignoreLifeShield)
        {
            if (damageM <= lifeShield.effect)
            {
                lifeShield.SetEffect(lifeShield.effect - damageM);
                damageM = 0;
            }
            else
            {
                damageM -= (int)lifeShield.effect;
                stat.RemoveTimerBuff((int)BasicCalculation.BattleCondition.LifeShield);
                lifeShield = null;
            }
        }
        
        
        if (stat.currentHp <= damageM && !causeDeath)
        {
            damageM = stat.currentHp - 1;
        }
        
        dnm.IndirectDamagePop(damageM,stat.transform);
        
        stat.currentHp -= damageM;
        
        stat.OnTakeIndirectDamage?.Invoke(damageM);
        stat.OnHPDecrease?.Invoke(damageM,null);
        
        if(damageM > 0)
            stat.OnHPChange?.Invoke();
        
        
        return damageM;

    }

    /// <summary>
    /// 对敌人的OD条造成间接削减
    /// </summary>
    /// <param name="stat">敌人的SpecialStatusManager实例</param>
    /// <param name="damage">基础伤害值</param>
    /// <param name="random">是否应用随机伤害波动（0.90-1.10倍）</param>
    /// <returns>实际对OD条造成的伤害值</returns>
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

    /// <summary>
    /// 为目标GameObject回血，计算回血量并处理回血上限，可应用随机波动
    /// </summary>
    /// <param name="target">要回血的目标GameObject</param>
    /// <param name="healPotency">基础回血量</param>
    /// <param name="healPotencyPercentage">基于最大HP的回血比例</param>
    /// <param name="randomRange">是否应用随机回血波动（0.95-1.05倍）</param>
    /// <param name="ignoreCap">是否忽略目标的回血上限</param>
    /// <returns>实际回血值</returns>
    public virtual int TargetHeal(GameObject target, float healPotency, float healPotencyPercentage, bool randomRange,bool ignoreCap = false)
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

        if(damageM > stat.healCap && ignoreCap == false)
            damageM = stat.healCap;


        dnm.HealPop(damageM, target.transform, stat.height);

        if (stat.currentHp < stat.maxHP)
        {
            stat.currentHp += damageM;
            stat.OnHPChange?.Invoke();
        }
        else
        {
            //stat.OnHPChange?.Invoke();
        }
        stat.OnHPIncrease?.Invoke(damageM);


        return damageM;
    }

    /// <summary>
    /// 为目标StatusManager实例回血，直接指定回血量并处理回血上限
    /// </summary>
    /// <param name="stat">目标的StatusManager实例</param>
    /// <param name="healHP">要回复的HP值</param>
    public void TargetHeal(StatusManager stat, int healHP)
    {
        if(healHP > stat.healCap)
            healHP = stat.healCap;
        
        dnm.HealPop(healHP, stat.transform, stat.height);

        if (stat.currentHp < stat.maxHP)
        {
            stat.currentHp += healHP;
            stat.OnHPChange?.Invoke();
        }else{
            stat.currentHp += healHP;
        }
        stat.OnHPIncrease?.Invoke(healHP);

        
    }

    /// <summary>
    /// 以回血者的StatusManager实例，为目标appliedStat实例回血，计算回血量并处理回血上限，可应用随机波动
    /// </summary>
    /// <param name="stat">回血者的StatusManager实例</param>
    /// <param name="healPotency">基础回血量</param>
    /// <param name="healPotencyPercentage">基于最大HP的回血比例</param>
    /// <param name="randomRange">是否应用随机回血波动（0.95-1.05倍）</param>
    /// <param name="appliedStat">实际应用回血的StatusManager实例</param>
    /// <returns>实际回血值</returns>
    public virtual int TargetHeal(StatusManager stat, float healPotency, float healPotencyPercentage,
        bool randomRange, StatusManager appliedStat)
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
        
        if(damageM > stat.healCap)
            damageM = stat.healCap;

        dnm.HealPop(damageM, stat.transform,stat.height);

        if (appliedStat.currentHp < appliedStat.maxHP)
        {
            appliedStat.currentHp += damageM;
            appliedStat.OnHPChange?.Invoke();
        }else{
            appliedStat.currentHp += damageM;
        
        }
        appliedStat.OnHPIncrease?.Invoke(damageM);

        return damageM;
    }

    /// <summary>
    /// 结算目标的DOT（持续伤害）效果，计算DOT伤害、生成DOT伤害数字并扣除目标HP
    /// </summary>
    /// <param name="target">承受DOT伤害的目标GameObject</param>
    /// <param name="condition">对应的DOT异常状态类型</param>
    /// <returns>实际造成的DOT伤害值</returns>
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

        dnm.DotPop(damageM, target.transform, condition, stat.height);

        stat.currentHp -= damageM;
        
        if(damageM > 0)
            stat.OnHPChange?.Invoke();
        stat.OnHPDecrease?.Invoke(damageM, null);

        return damageM;
    }

    /// <summary>
    /// 检查并触发场地特殊效果，处理反伤等场地能力
    /// </summary>
    /// <param name="attackStat">攻击属性配置对象</param>
    /// <param name="srcStat">攻击发起者的StatusManager实例</param>
    /// <param name="targetStat">攻击目标的StatusManager实例</param>
    /// <param name="attachedCondition">攻击附加的状态列表</param>
    /// <param name="dmg">攻击造成的伤害数组</param>
    /// <returns>实际触发的效果对应的数值（如反伤伤害值）</returns>
    public int CheckSpecialFieldEffect(AttackBase attackStat, StatusManager srcStat, StatusManager targetStat,
        List<BattleCondition> attachedCondition, int[] dmg)
    {
        //反伤
        if (FieldAbilityIDList.Contains((int)BasicCalculation.EnemyAbility.ReflectionWorld))
        {
            var reflectDamage = (int)(dmg.Sum() * 0.1f + srcStat.currentHp*0.02f);
            CauseIndirectDamage(srcStat, reflectDamage, true);
            if (attachedCondition.Count > 0)
            {
                foreach (var condition in attachedCondition)
                {
                    if (StatusManager.IsControlAffliction(condition.buffID) ||
                        StatusManager.IsDotAffliction(condition.buffID) ||
                        StatusManager.IsOtherAffliction(condition.buffID))
                    {
                        srcStat.ObtainTimerBuff(condition);
                        if (srcStat is PlayerStatusManager)
                        {
                            specialEventTriggered?.Invoke(condition.buffID);
                        }
                    }
                }
            }
        }





        return dmg[0];
    }

    #endregion

    /// <summary>
    /// 检查异常是否成功施加
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
    /// 添加场地能力，触发场地能力添加事件，记录场地能力ID
    /// </summary>
    /// <param name="id">场地能力的ID</param>
    public void AddFieldAbility(int id)
    {
        //OnFieldAbilityAdd?.Invoke(id);
        // var abilityIcons = FindObjectsOfType<UI_BossAbilityDisplayer>();
        // foreach (var ability in abilityIcons)
        // {
        //     if(ability.abilityID == id)
        //         ability.SetIconActive(true);
        // }
        if(FieldAbilityIDList.Contains(id))
            return;
        FieldAbilityIDList.Add(id);
        OnFieldAbilityAdd?.Invoke(id);
    }

    /// <summary>
    /// 移除场地能力，触发场地能力移除事件，记录场地能力ID
    /// </summary>
    /// <param name="id">场地能力的ID</param>
    public void RemoveFieldAbility(int id)
    {
        //OnFieldAbilityRemove?.Invoke(id);
        // var abilityIcons = FindObjectsOfType<UI_BossAbilityDisplayer>();
        // foreach (var ability in abilityIcons)
        // {
        //     if(ability.abilityID == id)
        //         ability?.SetIconActive(false);
        // }

        
        FieldAbilityIDList.Remove(id);
        OnFieldAbilityRemove?.Invoke(id);
    }

    /// <summary>
    /// 清除所有场地能力，触发所有场地能力的移除事件并清空场地能力ID列表
    /// </summary>
    public void ClearAllFieldAbility()
    {
        foreach (var ability in FieldAbilityIDList)
        {
            OnFieldAbilityRemove?.Invoke(ability);
        }
        FieldAbilityIDList.Clear();
    }

    /// <summary>
    /// 获取当前场景中活跃的敌人Transform列表，优先返回带有嘲讽标记的敌人
    /// </summary>
    /// <returns>活跃敌人的Transform列表</returns>
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
                var ec = ene.GetComponent<EnemyController>();

                if (ec.notTarget)
                {
                    continue;
                }
                if (ene.activeSelf && ec.HitSensor.gameObject.activeSelf)
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













    /// <summary>
    /// 设置全局慢镜头效果，可覆盖现有效果，持续指定时间后恢复正常
    /// </summary>
    /// <param name="scale">时间缩放比例</param>
    /// <param name="durationRealTime">效果持续的真实时间（秒）</param>
    /// <param name="overwrite">是否覆盖现有慢镜头效果，默认false</param>
    public void TimeScaleEffect(float scale, float durationRealTime, bool overwrite = false)
    {
        if (overwrite)
        {
            _timeScaleTween?.Kill();
        }
        else if(_currentTimeScale != 1)
        {
            return;
        }
        
        SetTimeScale(scale);
        
        var duartionGameTime = durationRealTime * Mathf.Clamp(scale, 0.02f, 1.5f);
        
        _timeScaleTween = DOVirtual.DelayedCall(duartionGameTime, () =>
        {
            SetTimeScale(1);
        },false);
        
        
        
    }

    /// <summary>
    /// 设置全局时间缩放比例，限制缩放范围在0.02到1.5之间
    /// </summary>
    /// <param name="scale">时间缩放比例，默认1（正常速度）</param>
    public void SetTimeScale(float scale = 1)
    {
        _currentTimeScale = Mathf.Clamp(scale, 0.02f, 1.5f);
        Time.timeScale = _currentTimeScale;
    }

    /// <summary>
    /// 设置游戏的暂停/继续状态，暂停时会暂停全局时间和所有音频
    /// </summary>
    /// <param name="flag">true为暂停游戏，false为继续游戏</param>
    public void SetGamePause(bool flag)
    {
        isGamePaused = flag;
        Time.timeScale = flag ? 0 : _currentTimeScale;
        var voices1 = GameObject.FindGameObjectsWithTag("Voice");
        foreach (var voice in voices1)
        {
            var audiosrc = voice.GetComponent<AudioSource>();
            
            if(audiosrc==null)
                continue;
            
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

    /// <summary>
    /// 触发游戏失败流程，禁用玩家输入、禁用所有角色的碰撞检测、播放失败BGM并返回主菜单
    /// </summary>
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
        
        BattleEffectManager.Instance.SetLoop(false);
        BattleEffectManager.Instance.SetBGM(gameFailedBGM);
        BattleEffectManager.Instance.PlayBGM();
        
        // var music = GetComponent<AudioSource>();
        // music.clip = gameFailedBGM;
        // music.Play();
        // music.loop = false;


        var targetTransform = GameObject.Find("UIFXContainer").transform;
        Instantiate(gameFailedPrefab,
            Camera.main.transform.position+new Vector3(0,2,0),
            Quaternion.identity,
            targetTransform);

        yield return new WaitForSeconds(3f);
        yield return new WaitUntil(()=>!BattleEffectManager.Instance.BGMIsPlaying || Input.GetMouseButton(0));
        
        GlobalController globalController = FindObjectOfType<GlobalController>();
        globalController.TestReturnMainMenu();
    }
    
    /// <summary>
    /// 触发游戏通关流程，处理通关动画、播放通关BGM、显示通关结算界面
    /// </summary>
    /// <param name="loseControllTime">结算菜单延迟弹出的时间（秒）</param>
    public void SetGameCleared(float loseControllTime = 0)
    {
        if(gameResultRoutine==null)
            gameResultRoutine =
                StartCoroutine(GameClearedRoutine(loseControllTime));
    }

    /// <summary>
    /// 触发简化版游戏通关流程，快速显示通关结算界面
    /// </summary>
    public void SetGameClearedSimple()
    {
        if(gameResultRoutine==null)
            gameResultRoutine =
                StartCoroutine(GameClearedRoutineSimple());
    }

    IEnumerator GameClearedRoutine(float loseControllTime = 0f)
    {
        //var fxs = GameObject.Find("AttackFXPlayer");
        OnQuestCleared?.Invoke(quest_id);
        

        var playerinput = player.GetComponent<PlayerInput>();
        player.GetComponent<StatusManager>().ResetAllStatusForced();
        
        playerinput.stdAtk = false;
        playerinput.roll = false;
        
        _timeScaleTween?.Kill();
        
        if (GameObject.Find("EnemyLayer").transform.childCount > 0)
        {
            Time.timeScale = 0.5f;
            GameObject.Find("CharacterInfo").SetActive(false);
        
            StageCameraController.SwitchMainCamera();
            StageCameraController.SwitchMainCameraFollowObject(lastEnemyEliminated,false);
            StageCameraController.SetMainCameraSize(6);
            yield return new WaitForSeconds(.8f);
        }

        Time.timeScale = 1;
        StageCameraController.SwitchMainCameraFollowObject(player,false);
        StageCameraController.SetMainCameraSize(8);

        yield return new WaitForSeconds(3f + loseControllTime);
        var targetTransform = GameObject.Find("UIFXContainer").transform;
        var clearGameObject = Instantiate(gameClearPrefab,
            Camera.main.transform.position+new Vector3(0,0,-5),
            Quaternion.identity,
            targetTransform);
        
        
        FindObjectOfType<UI_MultiBossManager>().gameObject.SetActive(false);
        
        playerinput.DisableAllInput();
        playerinput.SetMoveDisabled();
        playerinput.DisableAndIdle();
        playerinput.isMove = 0;

        var playercontroller = player.GetComponent<ActorController>();
        playercontroller.anim.ResetParameters();
        playercontroller.anim.SetFloat("forward",0);
        Debug.LogWarning("Set Idle 1");
        yield return null;
        Debug.LogWarning("Set Idle 2");
        playercontroller.anim.Play("idle");
        playercontroller.enabled = false;
        playerinput.DisableAndIdle();
        
        var attacks = FindObjectsOfType<AttackContainer>();
        foreach (var attack in attacks)
        {
            attack.DestroyInvoke();
        }
        
        BattleEffectManager.Instance.PlayBGM(false);
        BattleEffectManager.Instance.SetBGM(gameClearBGM);
        BattleEffectManager.Instance.SetLoop(false);
        BattleEffectManager.Instance.PlayBGM(true);
        // var music = GetComponent<AudioSource>();
        // music.clip = gameClearBGM;
        // music.Play();
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

    IEnumerator GameClearedRoutineSimple()
    {
        OnQuestCleared?.Invoke(quest_id);

        var playerinput = player.GetComponent<PlayerInput>();
        player.GetComponent<StatusManager>().ResetAllStatusForced();
        
        var targetTransform = GameObject.Find("UIFXContainer").transform;
        var clearGameObject = Instantiate(gameClearPrefab,
            Camera.main.transform.position+new Vector3(0,0,-5),
            Quaternion.identity,
            targetTransform);
        
        
        
        UI_MultiBossManager.Instance.gameObject.SetActive(false);
        
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
        
        BattleEffectManager.Instance.PlayBGM(false);
        BattleEffectManager.Instance.SetBGM(gameClearBGM);
        BattleEffectManager.Instance.SetLoop(false);
        BattleEffectManager.Instance.PlayBGM(true);
        // var music = GetComponent<AudioSource>();
        // music.clip = gameClearBGM;
        // music.Play();
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

    /// <summary>
    /// 处理敌人被击败的逻辑，更新当前敌人数量，当关卡剩余敌人数量为0时触发通关
    /// </summary>
    /// <param name="enemy">被击败的敌人GameObject</param>
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
    
    /// <summary>
    /// 更新关卡存档数据，记录最佳通关时间、皇冠进度，返回是否为首次通关和首次全皇冠通关
    /// </summary>
    /// <param name="newQuestState">新的关卡存档状态</param>
    /// <param name="newRecord">是否刷新了最佳时间记录（引用传递）</param>
    /// <returns>Bool元组：(firstClear:是否为首次通关; firstFullClear:是否为首次全皇冠通关)</returns>
    public (bool firstClear,bool firstFullClear) UpdateQuestSaveData(QuestSave newQuestState,ref bool newRecord)
    {
        newQuestState.best_clear_time = (double)Mathf.Round((float)newQuestState.best_clear_time*10f) / 10f;
        // 存储文件的路径  

        //print(newQuestState.best_clear_time);
        
        string path = Application.persistentDataPath + "/testSaveData.json";
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
        
        bool isFirstFullClear = false;

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
       
        if(newQuestState.crown_1 == 1 && newQuestState.crown_2 == 1 && 
           newQuestState.crown_3 == 1)
        {
            bool fullClearedBefore = savedata.crown_1 == 1 &&
                                     savedata.crown_2 == 1 && savedata.crown_3 == 1;

            if(fullClearedBefore == false)
            {
                isFirstFullClear = true;
            }
        }
            
            
        
        savedata.best_clear_time = newQuestState.best_clear_time;
        savedata.crown_1 = newQuestState.crown_1;
        savedata.crown_2 = newQuestState.crown_2;
        savedata.crown_3 = newQuestState.crown_3;


        string jsonStr = JsonMapper.ToJson(datalist);
        string filePath = Application.persistentDataPath + "/testSaveData.json";
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(jsonStr);
        sw.Close();

        return (!isFound,isFirstFullClear);

    }

    /// <summary>
    /// 更新关卡存档数据，记录最佳通关时间、皇冠进度，返回是否为首次通关和首次全皇冠通关
    /// </summary>
    /// <param name="newQuestState">新的关卡存档状态</param>
    /// <returns>元组：(是否为首次通关, 是否为首次全皇冠通关)</returns>
    public (bool firstClear,bool firstFullClear) UpdateQuestSaveData(QuestSave newQuestState)
    {
        newQuestState.best_clear_time = (double)Mathf.Round((float)newQuestState.best_clear_time*10f) / 10f;
        // 存储文件的路径  

        //print(newQuestState.best_clear_time);
        
        string path = Application.persistentDataPath + "/testSaveData.json";
        StreamReader sr = new StreamReader(path);
        var str = sr.ReadToEnd();
        sr.Close();
        var datalist = JsonMapper.ToObject<QuestDataList>(str);

        //var newData = datalist["quest_info"][quest_id];
        

        var savedata = JsonMapper.ToObject<QuestSave>(str);

        bool isFound = false;
        foreach (var data in datalist.quest_info)
        {
            if (data.quest_id == newQuestState.quest_id)
            {
                isFound = true;
                savedata = data;
            }
        }
        if (!isFound)
        {
            datalist.quest_info.Add(newQuestState);
            savedata = datalist.quest_info[datalist.quest_info.Count - 1];
            //newRecord = true;
        }
        
        
        
        if (newQuestState.best_clear_time < savedata.best_clear_time || savedata.best_clear_time < 0)
        {
            //newRecord = true;
        }
        else
        {
            newQuestState.best_clear_time = savedata.best_clear_time;
        }
        
        bool isFirstFullClear = false;

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
       
        if(newQuestState.crown_1 == 1 && newQuestState.crown_2 == 1 && 
           newQuestState.crown_3 == 1)
        {
            bool fullClearedBefore = savedata.crown_1 == 1 &&
                                     savedata.crown_2 == 1 && savedata.crown_3 == 1;

            if(fullClearedBefore == false)
            {
                isFirstFullClear = true;
            }
        }
            
            
        
        savedata.best_clear_time = newQuestState.best_clear_time;
        savedata.crown_1 = newQuestState.crown_1;
        savedata.crown_2 = newQuestState.crown_2;
        savedata.crown_3 = newQuestState.crown_3;


        string jsonStr = JsonMapper.ToJson(datalist);
        string filePath = Application.persistentDataPath + "/testSaveData.json";
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(jsonStr);
        sw.Close();

        return (!isFound,isFirstFullClear);

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

    
    /// <summary>
    /// 初始化地图信息
    /// </summary>
    /// <returns></returns>
    public static List<Platform> InitMapInfo()
    {
        if(Instance.platforms != null)
            return Instance.platforms;

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
        
        if(BattleStageManager.Instance.platforms == null)
            BattleStageManager.Instance.platforms = platformsInfo;

        return platformsInfo;
    }

    /// <summary>
    /// 刷新地图
    /// </summary>
    public void RefreshMapInfo()
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
        
        BattleStageManager.Instance.platforms = platformsInfo;
    }

    /// <summary>
    /// 获取关卡的LevelDetailedInfo信息
    /// </summary>
    public LevelDetailedInfo GetLevelDetailedInfo()
    {
        return levelDetailedInfo;
    }

    /// <summary>
    /// 设置当前敌人数量
    /// </summary>
    /// <param name="num"></param>
    public void SetMaxEnemy(int num)
    {
        currentEnemyNum = num;
    }

    /// <summary>
    /// 杀死场景所有敌人，将所有敌人的HP设为0并触发HP变化事件
    /// </summary>
    public void KillAllEnemy()
    {
        var enemyStats = EnemyLayer.GetComponentsInChildren<StatusManager>();

        foreach (var stat in enemyStats)
        {
            stat.currentHp = 0;
            stat.OnHPChange?.Invoke();
        }
    }

    /// <summary>
    /// 检查位置是否超出地图边界，修正位置在地图边界内
    /// </summary>
    /// <param name="pos">要检查的位置</param>
    /// <returns>修正后的位置</returns>
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

    /// <summary>
    /// 检查位置是否超出目标所在平台的边界，修正X轴位置在平台边界内
    /// </summary>
    /// <param name="target">目标GameObject</param>
    /// <param name="posX">要检查的X轴位置</param>
    /// <returns>修正后的X轴位置</returns>
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
    
    /// <summary>
    /// 直接为目标施加异常状态，进行成功率检查
    /// </summary>
    /// <param name="targetStat">目标的StatusManager实例</param>
    /// <param name="condition">要施加的异常状态</param>
    /// <param name="chance">成功率百分比</param>
    /// <param name="attackType">1为玩家，0为敌人攻击</param>
    /// <returns>施加结果：1为成功，0为白字抵抗，-1为黄字抵抗</returns>
    public int ObtainAfflictionDirectlyWithCheck(StatusManager targetStat,BattleCondition condition, int chance,int attackType=1,
        StatusManager srcStat = null)
    {
        var condFlag = CheckAffliction(chance,
            targetStat.GetAfflictionResistance
                ((BasicCalculation.BattleCondition)condition.buffID));
        
        if (targetStat is PlayerStatusManager)
        {
            if ((targetStat as PlayerStatusManager).isShapeshifting)
            {
                condFlag = -2;
            }
        }
        
        
        if (condFlag<1)
        {
            targetStat.OnAfflictionGuarded?.Invoke(condition);
            //1是成功,0是白字resist,-1是黄字resist
            if(condFlag == 0)
                DamageNumberManager.GenerateResistText(targetStat.transform,0,targetStat.height);
            else if(condFlag == -1)
            {
                DamageNumberManager.GenerateResistText(targetStat.transform, 1,targetStat.height);
            }
                        
            return condFlag;//检查异常抗性！不一定是异常！
        }
        targetStat.ObtainTimerBuff(condition);
        if(StatusManager.IsAffliction(condition.buffID))
        {
            if(attackType!=1)
                targetStat.IncreaseAfflictionResistance(condition.buffID);
        }
        
        
        if (srcStat &&  FieldAbilityIDList.Contains((int)BasicCalculation.EnemyAbility.ReflectionWorld) )
        {
            if (StatusManager.IsControlAffliction(condition.buffID) ||
                StatusManager.IsDotAffliction(condition.buffID))
            {
                srcStat.ObtainTimerBuff(condition);
                if (srcStat is PlayerStatusManager)
                {
                    specialEventTriggered?.Invoke(condition.buffID);
                }
                
            }
        }

        return condFlag;
    }

    /// <summary>
    /// 获取场景中所有带有ActorBase组件的StatusManager实例
    /// </summary>
    /// <returns>符合条件的StatusManager列表</returns>
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
    
    /// <summary>
    /// 触发敌人能力事件
    /// </summary>
    /// <param name="id">敌人能力ID</param>
    /// <param name="event">事件类型</param>
    /// <param name="stat">关联的StatusManager实例，默认null</param>
    public void InvokeEnemyAbilityEvent(int id, EnemyAbilityIconEvent @event, StatusManager stat = null)
    {
        OnFieldAbilityEvent?.Invoke(id, stat, @event);
    }

    /// <summary>
    /// 触发敌人唤醒事件
    /// </summary>
    /// <param name="obj">唤醒的敌人GameObject</param>
    public void InvokeEnemyOnAwake(GameObject obj)
    {
        OnEnemyAwake?.Invoke(obj.transform.GetInstanceID());
        //ResetEnemyList();
    }
    
    /// <summary>
    /// 触发敌人击败事件
    /// </summary>
    /// <param name="obj">击败的敌人GameObject</param>
    public void InvokeEnemyOnEliminated(GameObject obj)
    {
        OnEnemyEliminated?.Invoke(obj.GetInstanceID());
        //ResetEnemyList();
    }

    /// <summary>
    /// 刷新敌人列表
    /// </summary>
    protected void ResetEnemyList()
    {
        var enemyLayer = EnemyLayer;
        EnemyList.Clear();
        for (var i = 0; i < enemyLayer.transform.childCount; i++)
            if (enemyLayer.transform.GetChild(i).gameObject.activeSelf)
                EnemyList.Add(enemyLayer.transform.GetChild(i).GetInstanceID());
    }

    /// <summary>
    /// 获取新的远程攻击容器GameObject，区分敌人和玩家类型
    /// </summary>
    /// <param name="isEnemy">是否为敌人的远程攻击容器，默认true</param>
    /// <returns>远程攻击容器GameObject</returns>
    public GameObject GetNewRangedContainer(bool isEnemy = true)
    {
        GameObject go;
        if (isEnemy)
        {
            go = Instantiate(attackContainerEnemy, Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
        }
        else
        {
            go = Instantiate(attackContainer, Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
        }

        return go;
    }

    /// <summary>
    /// 触发特殊事件，一般用于boss机制或者成就触发。
    /// </summary>
    /// <param name="message">事件消息ID</param>
    public void TriggerSpecialEvent(int message)
    {
        specialEventTriggered?.Invoke(message);
    }
    
    /// <summary>
    /// 触发玩家技能图标切换事件
    /// </summary>
    /// <param name="sid">技能ID，从0开始</param>
    /// <param name="message">事件消息ID，通常为0或1</param>
    public void TriggerSkillIconEvent(int sid,int message)
    {
        OnSkillIconSwapEvent?.Invoke(sid,message);
    }
    



}