using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    protected ActorBase ac;
    [SerializeField] protected GameObject attackContainer;
    public GameObject RangedAttackFXLayer;
    [SerializeField]
    protected GameObject MeeleAttackFXLayer;
    
    public GameObject BuffFXLayer;
    
    public GameObject healbuff;
    
    protected BattleEffectManager _effectManager;
    protected StatusManager _statusManager;
    
    // Start is called before the first frame update

    protected virtual void Awake()
    {
        _effectManager = BattleEffectManager.Instance;
        MeeleAttackFXLayer = transform.Find("MeeleAttackFX").gameObject;
        BuffFXLayer = transform.Find("BuffLayer").gameObject;
        ac=GetComponent<ActorBase>();
        _statusManager = GetComponent<StatusManager>();
    }

    protected virtual void Start()
    {
        attackContainer = BattleStageManager.Instance.attackContainer;
        RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");
    }

    public virtual void AirDashAttack()
    {
        DashAttack();
    }

    public virtual void DashAttack()
    {
        
    }

    /// <summary>
    /// 生成对应方向的攻击特效，默认翻转x轴
    /// </summary>
    /// <param name="prefab">预制体</param>
    /// <param name="facedir">人物朝向</param>
    /// <param name="axis">0:翻转x, 1:翻转y, 2:翻转z</param>
    /// <param name="rotateMode">0:euler, 1:scale</param>
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

        //prefabInstance.GetComponent<AttackFromPlayer>().playerpos = transform;

        return prefabInstance;
    }

    protected GameObject InstantiateMeele(GameObject prefab, Vector3 position, GameObject container,Transform _parent = null)
    {
        var prefabInstance = Instantiate(prefab, position, Quaternion.identity, container.transform);

        try
        {
            if(_parent == null)
                prefabInstance.GetComponent<AttackFromPlayer>().playerpos = transform;
            else
            {
                prefabInstance.GetComponent<AttackFromPlayer>().playerpos = _parent;
            }
        }
        catch
        {
            Debug.Log("No AttackFromPlayer Component");
        }
        
        return prefabInstance;
    }

    protected GameObject InstantiateBuff(GameObject prefab, Vector3 position)
    {
        var prefabInstance = Instantiate(prefab, position, Quaternion.identity, BuffFXLayer.transform);
        return prefabInstance;
    }

    protected virtual GameObject InstantiateRanged(GameObject prefab, Vector3 position, GameObject container,int facedir, int rotateMode = 1)
    {
        var prefabInstance = InstantiateDirectional(prefab, position, container.transform, facedir, 0, rotateMode);
        prefabInstance.GetComponent<AttackFromPlayer>().playerpos = transform;
        prefabInstance.GetComponent<AttackFromPlayer>().firedir = facedir;
        return prefabInstance;
    }

    protected virtual GameObject InstantiateDirectionalRanged(GameObject prefab, Vector3 position, GameObject container,
        int facedir, float angleZ)
    {
        var prefabInstance = InstantiateDirectional(prefab, position, container.transform, facedir, 0, 1);
        prefabInstance.GetComponent<AttackFromPlayer>().playerpos = transform;
        prefabInstance.GetComponent<AttackFromPlayer>().firedir = facedir;

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

    protected List<StatusManager> GetAllObjects(Transform _parent)
    {
        List<StatusManager> res = new();
        res.AddRange(_parent.GetComponentsInChildren<StatusManager>());
        return res;
    }

    /// <summary>
    /// 1:Fire,2:Water,3.Wind,4.Light,5.Dark
    /// </summary>
    /// <param name="type"></param>
    public void ShapeShiftingAttackWave(int type)
    {
        GameObject container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            MeeleAttackFXLayer.transform);
        GameObject shapeShiftingFX = BattleEffectManager.Instance.GetShapeShiftingFX(type);
        
        var prefabInstance = 
            InstantiateMeele(shapeShiftingFX, transform.position, container);
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

    protected GameObject InstantiateSealedContainer(GameObject containerPrefab, bool isMeele, bool displayDmg = false, int totalNum = 1)
    {
        var container = Instantiate(containerPrefab, transform.position,
            Quaternion.identity, isMeele ? MeeleAttackFXLayer.transform : RangedAttackFXLayer.transform);
        
        if (totalNum != 1 || displayDmg)
        {
            container.GetComponent<AttackContainer>().InitAttackContainer(totalNum, displayDmg);
        }

        return container;
    }

    protected void LifeSteal(StatusManager statusManager, int damage, int fraction, int maxHealPercentage)
    {
        float maxHealHP = statusManager.maxHP * 0.01f * maxHealPercentage;

        float healHP = Mathf.Min(damage * fraction * 0.01f,maxHealHP);
        
        statusManager.HPRegenImmediatelyWithoutRandomDirectly(statusManager,(int)healHP);


    }

    protected void AddParticleSystemSpeedModifier(GameObject fx)
    {
        var speedModifier = fx.AddComponent<PlayerParticleSystemSpeedModifier>();
        speedModifier.actorController = ac as ActorController;
    }

    protected void AddAnimationSpeedModifier(GameObject fx)
    {
        var speedModifier = fx.AddComponent<PlayerAnimationSpeedModifier>();
        speedModifier.actorController = ac as ActorController;
    }
    
    


}
