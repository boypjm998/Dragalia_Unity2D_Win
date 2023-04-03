using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] protected GameObject attackContainer;
    public GameObject RangedAttackFXLayer;
    [SerializeField]
    protected GameObject MeeleAttackFXLayer;
    
    public GameObject BuffFXLayer;
    
    public GameObject healbuff;
    
    protected BattleEffectManager _effectManager;
    
    // Start is called before the first frame update

    protected virtual void Awake()
    {
        _effectManager = FindObjectOfType<BattleEffectManager>();
        
    }

    protected virtual void Start()
    {
        attackContainer = BattleStageManager.Instance.attackContainer;
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

    protected GameObject InstantiateMeele(GameObject prefab, Vector3 position, GameObject container)
    {
        var prefabInstance = Instantiate(prefab, position, Quaternion.identity, container.transform);
        prefabInstance.GetComponent<AttackFromPlayer>().playerpos = transform;
        return prefabInstance;
    }
}
