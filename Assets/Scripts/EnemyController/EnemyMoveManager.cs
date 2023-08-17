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

    public virtual void UseMove(int moveID)
    {
    }


    protected virtual void Awake()
    {
        bossBanner = GameObject.Find("BattleInfoCaster")?.GetComponent<UI_BattleInfoCaster>();
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
    }

    public virtual void DisappearRenderer()
    {
        ac.rendererObject.SetActive(false);
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
    public GameObject GenerateWarningPrefab(string prefabName, Vector3 where,Quaternion rot, Transform _parent)
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
        return clone;
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
    
    protected GameObject InstantiateMeele(GameObject prefab, Vector3 position, GameObject container)
    {
        var prefabInstance = Instantiate(prefab, position, Quaternion.identity, container.transform);
        prefabInstance.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        return prefabInstance;
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
    public GameObject GetProjectileOfFormatName(string nameFromAction)
    {
        //查找尾部是_{nameFromAction}的projectile
        
        foreach (var projectile in projectilePool)
        {
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
   
}
