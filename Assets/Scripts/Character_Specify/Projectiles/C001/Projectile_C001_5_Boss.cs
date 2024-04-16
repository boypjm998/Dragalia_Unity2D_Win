using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class Projectile_C001_5_Boss : MonoSingleton<Projectile_C001_5_Boss>,IEnemySealedContainer
{
    [SerializeField] private GameObject transportationEffectPrefab;

    [SerializeField] private GameObject[] childs;

    [SerializeField] private GameObject blastEffectPrefab;

    private GameObject _enemySource;

    public int childCount => childs.Length;

    //private int _childCount;

    private void Start()
    {
        //_childCount = transform.childCount;
    }

    public void SetEnemySource(GameObject src)
    {
        _enemySource = src;
    }


    public void BlastEffect()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position, Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
        for (int i = 0; i < transform.childCount; i++)
        {
            this.InstantiateRangedObject(blastEffectPrefab, transform.GetChild(i).position,
                container, 1,1,_enemySource.transform);
        }
        
        Destroy(gameObject,0.25f);
    }

    public Transform GetChild(int index)
    {
        return childs[index].transform;
    }

    public void Transport(Transform origin,Transform child,float blastDelay)
    {
        var eff = Instantiate(transportationEffectPrefab, origin.position, Quaternion.identity);
        var eff2 = Instantiate(transportationEffectPrefab, child.transform.position, Quaternion.identity);
        eff2.GetComponent<AdventurerSpecial_PortalTrail>().InitAnim(origin);
            
        origin.position = child.position;
            
        eff.GetComponent<AdventurerSpecial_PortalTrail>().InitAnim(child);
        Destroy(child.gameObject);
        
        Invoke("BlastEffect",blastDelay);
    }
    
    
}
