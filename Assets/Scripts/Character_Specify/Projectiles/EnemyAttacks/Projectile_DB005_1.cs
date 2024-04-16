using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

/// <summary>
/// 黑暗冤魂
/// </summary>
public class Projectile_DB005_1 : MonoBehaviour
{
    [SerializeField] private GameObject hintbarPrefab;
    [SerializeField] private GameObject blastFXPrefab;

    public GameObject enemySource;
    
    private GameObject _hintbar;
    private GameObject _blastFX;
    private StatusManager _sm;

    private void Awake()
    {
        _sm = GetComponent<StatusManager>();
        _sm.OnReviveOrDeath += DestroyHintBar;
    }

    private IEnumerator Start()
    {
        var waitTime = GenerateHintBar();

        yield return new WaitForSeconds(waitTime);

        if (_sm.currentHp > 0)
        {
            Blast();
            _sm.currentHp = 0;
            _sm.OnHPBelow0?.Invoke();
        }

    }

    private void OnDestroy()
    {
        DestroyHintBar();
    }
    
    private void DestroyHintBar()
    {
        if (_hintbar != null)
        {
            Destroy(_hintbar.gameObject);
        }
    }

    private float GenerateHintBar()
    {
        _hintbar = Instantiate(hintbarPrefab,gameObject.RaycastedPosition(),Quaternion.identity,
            BattleStageManager.Instance.RangedAttackFXLayer.transform);
        _hintbar.GetComponent<EnemyAttackHintBarRect2D>().SetAc(GetComponent<EnemyController>());

        return _hintbar.GetComponent<EnemyAttackHintBarRect2D>().warningTime;
    }

    private void Blast()
    {
        this.InstantiateRangedObject(blastFXPrefab,gameObject.RaycastedPosition(),
            Instantiate(BattleStageManager.Instance.attackContainerEnemy,
                Vector3.zero,Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform),1,1,transform
            ).GetComponent<AttackFromEnemy>().enemySource = enemySource;
    }
    
}
