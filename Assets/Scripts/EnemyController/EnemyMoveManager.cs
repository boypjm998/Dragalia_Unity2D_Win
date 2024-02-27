using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using GameMechanics;

public abstract class EnemyMoveManager : MonoBehaviour
{
    public GameObject MeeleAttackFXLayer;
    public GameObject RangedAttackFXLayer;
    public GameObject BuffFXLayer;
    public GameObject attackContainer;
    public GameObject attackSubContainer;
    protected EnemyController ac;
    public EnemyController.OnTask OnAttackFinished;
    protected DragaliaEnemyBehavior _behavior;
    public Coroutine currentAttackMove;//只有行为树在用
    public Tweener _tweener;
    protected UI_BattleInfoCaster bossBanner;
    
    [SerializeField] protected GameObject[] WarningPrefabs;

    [Header("Moves")]
    [SerializeField] protected GameObject projectile1;
    [SerializeField] protected GameObject projectile2;
    [SerializeField] protected GameObject projectile3;
    [SerializeField] protected GameObject projectile4;
    [SerializeField] protected GameObject projectile5;
    [SerializeField] protected GameObject projectile6;
    [SerializeField] protected GameObject projectile7;
    [SerializeField] protected GameObject projectile8;
    [SerializeField] protected GameObject projectile9;
    [SerializeField] protected GameObject projectile10;

    [SerializeField] protected GameObject[] projectilePoolEX;
    protected GameObject[] projectilePool = new GameObject[10];

    protected StatusManager _statusManager;
    protected BattleStageManager _stageManager;
    protected BattleEffectManager _effectManager;
    protected Animator anim;

    protected List<NPCNavigateAnchorSensor> _navigateAnchorSensors = new();
    protected IEnumerator _canAction;
    protected IEnumerator _canActionOnGround;
    protected IEnumerator _canActionOnFlyingGround;
    
    public event Action<int> OnUseSkill;
    
    protected void InvokeOnUseSkill(int skillID)
    {
        OnUseSkill?.Invoke(skillID);
    }

    public virtual void UseMove(int moveID)
    {
    }


    protected virtual void Awake()
    {
        bossBanner = GameObject.Find("BattleInfoCaster")?.GetComponent<UI_BattleInfoCaster>();
        ac = GetComponent<EnemyController>();
        _canAction = new WaitUntil(()=>ac.hurt == false);
        _canActionOnGround = new WaitUntil(()=>ac.hurt == false && ac.grounded);
        _canActionOnFlyingGround = new WaitUntil(()=>ac.hurt == false && (ac as EnemyControllerFlyingHigh).flyingGrounded);
    }

    // Start is called before the first frame update

    protected virtual void Start()
    {
        MeeleAttackFXLayer = transform.Find("MeeleAttackFX")?.gameObject;
        RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");
        BuffFXLayer = transform.Find("BuffLayer")?.gameObject;
        _behavior = GetComponent<DragaliaEnemyBehavior>();
        anim = GetComponentInChildren<Animator>();
        _statusManager = GetComponent<StatusManager>();
        _stageManager = BattleStageManager.Instance;
        attackContainer = _stageManager.attackContainerEnemy;
        CopyProjectilesToPool();
    }

    // Update is called once per frame

    protected void GetAllAnchors()
    {
        _navigateAnchorSensors = FindObjectsOfType<NPCNavigateAnchorSensor>().ToList();
    }


    public void SetGroundCollider(bool flag)
    {
        if (flag)
        {
            var sensor = transform.Find("GroundSensor").GetComponent<BoxCollider2D>();
            sensor.enabled = true;
        }
        else
        {
            var sensor = transform.Find("GroundSensor").GetComponent<BoxCollider2D>();
            sensor.enabled = false;
        }
    }

    public virtual void PlayVoice(int id)
    {

    }
    
    protected GameObject InstantiateDirectional(GameObject prefab, Vector3 position, Transform _parent, int facedir, int axis = 0, int rotateMode = 0)
    {
        var prefabInstance = Instantiate(prefab, position, Quaternion.identity, _parent);
        if(facedir == -1)
        {
            if (rotateMode == 0)
            {
                prefabInstance.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                var initScale = prefabInstance.transform.localScale;
                if(axis == 0)
                    prefabInstance.transform.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);
                else if(axis == 1)
                    prefabInstance.transform.localScale = new Vector3(initScale.x, -initScale.y, initScale.z);
                else if(axis == 2)
                    prefabInstance.transform.localScale = new Vector3(initScale.x, initScale.y, -initScale.z);
                
            }
        }

        return prefabInstance;
    }

    public virtual void AppearRenderer()
    {
        ac.rendererObject.SetActive(true);
        ac.shadowCaster?.gameObject.SetActive(true);
        ac.minimapIcon?.gameObject.SetActive(true);
    }

    public virtual void DisappearRenderer()
    {
        ac.rendererObject.SetActive(false);
        ac.shadowCaster?.gameObject.SetActive(false);
        ac.minimapIcon?.gameObject.SetActive(false);
    }
    
    protected GameObject GenerateWarningPrefab(GameObject prefab, Vector3 where,Quaternion rot, Transform _parent)
    {
        var clone = Instantiate(prefab, where, rot, _parent);
        clone.GetComponent<EnemyAttackHintBar>()?.SetSource(ac);
        return clone;
    }
    public GameObject GenerateWarningPrefab(int prefabId, Vector3 where,Quaternion rot, Transform _parent)
    {
        var clone = Instantiate(WarningPrefabs[prefabId], where, rot, _parent);
        clone.GetComponent<EnemyAttackHintBar>()?.SetSource(ac);
        return clone;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prefabName">最后一个横线后的后缀名</param>
    /// <param name="where"></param>
    /// <param name="rot"></param>
    /// <param name="_parent"></param>
    /// <returns></returns>
    public GameObject GenerateWarningPrefab(string prefabName, Vector3 where,Quaternion rot, Transform _parent,int dir = 1)
    {
        GameObject prefab;
        
        try
        {
            prefab = WarningPrefabs.ToList().Find(
                x =>
                    x.name.EndsWith("_" + prefabName))?.gameObject;
        }
        catch
        {
            return null;
        }

        var clone = Instantiate(prefab, where, rot, _parent);
        clone.GetComponent<EnemyAttackHintBar>()?.SetSource(ac);

        if (dir != 1)
        {
            clone.transform.localScale =
                new Vector3(dir*transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }


        return clone;
    }

    public GameObject GetWarningPrefab(string prefabName)
    {
        GameObject prefab;
        try
        {
            prefab = WarningPrefabs.ToList().Find(
                x =>
                    x.name.EndsWith("_" + prefabName))?.gameObject;
        }
        catch
        {
            return null;
        }

        return prefab;
    }
    protected virtual void QuitAttack()
    {
        _behavior.currentAttackAction = null;
        ac.OnAttackExit();
        OnAttackFinished?.Invoke(true);
    }
    
    protected GameObject InstantiateRanged(GameObject prefab, Vector3 position, GameObject container,int facedir, int rotateMode = 1)
    {
        var prefabInstance = InstantiateDirectional(prefab, position, container.transform, facedir, 0, rotateMode);
        try
        {
            prefabInstance.GetComponent<AttackFromEnemy>().enemySource = gameObject;
            prefabInstance.GetComponent<AttackBase>().firedir = facedir;
        }
        catch
        {
            print("No AttackFromEnemy Component");
        }
        return prefabInstance;
    }
    
    protected GameObject InstantiateDirectionalRanged(GameObject prefab, Vector3 position, GameObject container,
        int facedir, float angleZ)
    {
        var prefabInstance = InstantiateDirectional(prefab, position, container.transform, facedir, 0, 1);
        prefabInstance.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        prefabInstance.GetComponent<AttackFromEnemy>().firedir = facedir;

        prefabInstance.transform.eulerAngles = new Vector3(0, 0, angleZ);
        //用TryGetComponent来尝试获取DoTweenSimpleController组件，如果有就设置朝向
        if (prefabInstance.TryGetComponent(out DOTweenSimpleController controller))
        {
            var magnitude = controller.moveDirection.magnitude;
            controller.moveDirection = 
                new Vector2(magnitude * facedir * Mathf.Cos(angleZ*Mathf.Deg2Rad),
                    magnitude * Mathf.Sin(angleZ*Mathf.Deg2Rad) * facedir);
        }
        
        
        return prefabInstance;
    }
    
    protected GameObject InstantiateMeele(GameObject prefab, Vector3 position, GameObject container)
    {
        var prefabInstance = Instantiate(prefab, position, Quaternion.identity, container.transform);
        prefabInstance.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        return prefabInstance;
    }

    protected GameObject InstantiateSealedContainer(GameObject prefab, Vector3 position, bool isMeele, int facedir = 1)
    {
        Transform layer = isMeele ? MeeleAttackFXLayer.transform : RangedAttackFXLayer.transform;

        var containerInstance = 
            Instantiate
                (prefab, position, Quaternion.identity, layer.transform);

        var atkFromEnemies = containerInstance.GetComponentsInChildren<AttackFromEnemy>();

        foreach (var atkFromEnemy in atkFromEnemies)
        {
            atkFromEnemy.enemySource = gameObject;
        }

        if (!isMeele && facedir == -1)
        {
            containerInstance.transform.localScale = new Vector3(-containerInstance.transform.localScale.x,
                containerInstance.transform.localScale.y, containerInstance.transform.localScale.z);
        }

        return containerInstance;

    }

    protected GameObject InstantiateSealedContainer(GameObject prefab, Vector3 position,
        Transform _parent, int facedir = 1)
    {
        var layer = _parent;
        
        var containerInstance = 
            Instantiate
                (prefab, position, Quaternion.identity, layer.transform);
        
        containerInstance.GetComponent<IEnemySealedContainer>().SetEnemySource(gameObject);
        
        if (facedir == -1 && _parent == RangedAttackFXLayer.transform)
        {
            containerInstance.transform.localScale = new Vector3(-containerInstance.transform.localScale.x,
                containerInstance.transform.localScale.y, containerInstance.transform.localScale.z);
        }

        return containerInstance;

    }
    protected void QuitMove()
    {
        ac.currentKBRes = 999;
        if (ac.VerticalMoveRoutine != null)
        {
            StopCoroutine(ac.VerticalMoveRoutine);
            ac.VerticalMoveRoutine = null;
        }
    }

    public virtual List<APlatformNode> GetDesignedRoutine()
    {
        return null;
    }

    public virtual APlatformNode GetNextRoutineNode(ref GameObject anchorTarget)
    {
        return null;
    }

    public GameObject GetAnchoredSensorOfName(string name)
    {
        return _navigateAnchorSensors.Find
        (x =>
            x.gameObject.name.Equals(name))?.gameObject;
    }

    protected void CopyProjectilesToPool()
    {
        //projectilePool = new GameObject[10];
        projectilePool[0] = projectile1;
        projectilePool[1] = projectile2;
        projectilePool[2] = projectile3;
        projectilePool[3] = projectile4;
        projectilePool[4] = projectile5;
        projectilePool[5] = projectile6;
        projectilePool[6] = projectile7;
        projectilePool[7] = projectile8;
        projectilePool[8] = projectile9;
        projectilePool[9] = projectile10;
    }

    /// <summary>
    /// 寻找格式化名字的projectile
    /// </summary>
    /// <param name="nameFromAction">例如fx_hb001_action14_1,该参数输入"action14_1"</param>
    /// <returns></returns>
    public GameObject GetProjectileOfFormatName(string nameFromAction,bool ignoreArray = false)
    {
        //查找尾部是_{nameFromAction}的projectile
        
        foreach (var projectile in projectilePool)
        {
            if(ignoreArray)
                break;
            if(projectile == null)
                continue;
            if (projectile.name.EndsWith("_" + nameFromAction))
            {
                return projectile;
            }
        }
        
        
        return projectilePoolEX.ToList().Find(
            x =>
                x.name.EndsWith("_" + nameFromAction))?.gameObject;
    }

    public GameObject GetProjectileOfName(string name)
    {
        //遍历projectilePool，找到名字相同的projectile
        foreach (var projectile in projectilePool)
        {
            if(projectile == null)
                continue;
            if (projectile.name.Equals(name))
            {
                return projectile;
            }
        }

        return projectilePoolEX.ToList().Find
        (x =>
            x.name.Equals(name))?.gameObject;
    }
    
    public GameObject GetProjectileStartWithName(string prefix)
    {
        //遍历projectilePool，找到名字相同的projectile
        foreach (var projectile in projectilePool)
        {
            if(projectile == null)
                continue;
            if (projectile.name.StartsWith(prefix))
            {
                return projectile;
            }
        }

        return projectilePoolEX.ToList().Find
        (x =>
            x.name.StartsWith(prefix))?.gameObject;
    }

    protected void PurgedShapeShiftingOfViewer()
    {
        var actor = _behavior.viewerPlayer.GetComponent<ActorController>();
        if (actor.DModeIsOn && actor.dc)
        {
            actor.dc.DModeForcePurge();
            actor._statusManager.InvokeShapeshiftingPurged();
        }

    }
    
    public static void PurgedShapeShiftingOfTarget(AttackBase atkBase, GameObject target)
    {
        var actor = target.GetComponent<ActorController>();
        if(actor == null)
            return;
        
        if (actor.DModeIsOn && actor.dc)
        {
            actor.dc.DModeForcePurge();
            actor._statusManager.InvokeShapeshiftingPurged();
        }

    }
    
    protected GameObject InitContainer(bool isMeele, int totalNum = 1, bool displayDmg = false)
    {
        var container = Instantiate(attackContainer, transform.position,
            Quaternion.identity, isMeele ? MeeleAttackFXLayer.transform : RangedAttackFXLayer.transform);

        if (totalNum != 1 || displayDmg)
        {
            container.GetComponent<AttackContainer>().InitAttackContainer(totalNum, displayDmg);
        }

        return container;
    }

    /// <summary>
    /// 未完成的方法
    /// </summary>
    /// <param name="attackPrefabInfo"></param>
    /// <param name="position"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    protected GameObject SpawnEmptyAttack(AttackPrefabInfo attackPrefabInfo, Vector3 position, Transform parent)
    {
        //Instantiate a new GameObject
        //Instantiate a new empty GameObject
        
        //实例化一个什么都没有的GameObject，不包含任何组件
        var go = new GameObject();
        AttackFromEnemy atk = null;
        if (attackPrefabInfo.attackType == AttackPrefabInfo.AttackType.Meele)
        {
            //设置父物体为MeeleAttackFXLayer
            go.transform.SetParent(MeeleAttackFXLayer.transform);
            atk = go.AddComponent<CustomMeeleFromEnemy>();
        }
        else if(attackPrefabInfo.attackType == AttackPrefabInfo.AttackType.Ranged)
        {
            //go.transform.SetParent(RangedAttackFXLayer.transform);
            atk = go.AddComponent<CustomRangedFromEnemy>();
        }else if (attackPrefabInfo.attackType == AttackPrefabInfo.AttackType.Bullet)
        {
            //go.transform.SetParent(RangedAttackFXLayer.transform);
            atk = go.AddComponent<BulletFromEnemy>();
        }
        else
        {
            //go.transform.SetParent(RangedAttackFXLayer.transform);
            atk = go.AddComponent<ForcedAttackFromEnemy>();
            var fatk = atk as ForcedAttackFromEnemy;
            fatk.triggerTime = attackPrefabInfo.awakeTimes[0];
        }
        
        go.transform.SetParent(parent);
        go.transform.position = position;

        if (attackPrefabInfo.colliderType == AttackPrefabInfo.ColliderType.Box)
        {
            var boxCollider = go.AddComponent<BoxCollider2D>();
            boxCollider.offset = new Vector2(attackPrefabInfo.colliderInfo[0],
                attackPrefabInfo.colliderInfo[1]);
            boxCollider.size = new Vector2(attackPrefabInfo.colliderInfo[2],
                attackPrefabInfo.colliderInfo[3]);
            boxCollider.isTrigger = true;
        }else if (attackPrefabInfo.colliderType == AttackPrefabInfo.ColliderType.Circle)
        {
            var circleCollider = go.AddComponent<CircleCollider2D>();
            circleCollider.radius = attackPrefabInfo.colliderInfo[0];
            circleCollider.offset = new Vector2(attackPrefabInfo.colliderInfo[1],
                attackPrefabInfo.colliderInfo[2]);
        }else if (attackPrefabInfo.colliderType == AttackPrefabInfo.ColliderType.Polygon)
        {
            var polygonCollider = go.AddComponent<PolygonCollider2D>();
            List<Vector2> points = new List<Vector2>();
            var pointNum = attackPrefabInfo.colliderInfo[0];
            for (int i = 1; i < attackPrefabInfo.colliderInfo.Count; i+=2)
            {
                points.Add(new Vector2(attackPrefabInfo.colliderInfo[i],
                    attackPrefabInfo.colliderInfo[i + 1]));
            }
            polygonCollider.points = points.ToArray();
        }


        atk.hitShakeIntensity = attackPrefabInfo.shakeIntensity;
        atk.attackInfo = attackPrefabInfo.attackInfos;
        

        if (attackPrefabInfo.awakeTimes.Count > 0 && !(atk is ForcedAttackFromEnemy))
        {
            var triggerController = go.AddComponent<EnemyAttackTriggerController>();
            triggerController.SetAwakeTimes(attackPrefabInfo.awakeTimes);
            if(attackPrefabInfo.sleepTimes.Count > 0)
                triggerController.SetSleepTimes(attackPrefabInfo.sleepTimes);

            triggerController.DestroyTime = attackPrefabInfo.invokeDestroyTime;
        }
        else
        {
            var invokeDestroy = go.AddComponent<ObjectInvokeDestroy>();
            invokeDestroy.destroyTime = attackPrefabInfo.invokeDestroyTime;
        }
        
        return go;
        

    }

    protected GameObject SpawnEnemyMinon(GameObject prefab, Vector3 position, int maxHP, int attack,int facedir = 1, bool isSummon = true)
    {
        var go = Instantiate(prefab, position, Quaternion.identity, BattleStageManager.Instance.EnemyLayer.transform);
        var enemy = go.GetComponent<EnemyController>();
        var statusManager = go.GetComponent<StatusManager>();
        statusManager.maxBaseHP = maxHP;
        statusManager.maxHP = maxHP;
        statusManager.baseAtk = attack;
        enemy.SetSummoned(isSummon);
        enemy.SetFaceDir(facedir);
        var behavior = go.GetComponent<DragaliaEnemyBehavior>();
        if (behavior != null)
        {
            behavior.enabled = true;
        }

        
        return go;
    }

    protected UI_RingSlider SpawnCountDownUI(GameObject UIPrefab,Vector2 position, float countdownTime,
        int minionTotalCount = 1)
    {
        var UI = Instantiate(UIPrefab,
            position.SafePosition(Vector2.zero), Quaternion.identity,
            RangedAttackFXLayer.transform);
        var UIRingSlider = UI.GetComponent<UI_RingSlider>();
        UIRingSlider.maxValue = countdownTime;
        UIRingSlider.currentValue = countdownTime;
        
        var UICountdownMinon = UI.GetComponent<UI_CountdownMinon>();
        UICountdownMinon.SetMaxCapacity(Mathf.Clamp(minionTotalCount, 1, 10));
        
        return UIRingSlider;
    }
    
    
}
