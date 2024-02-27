using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

/// <summary>
/// Targeting Ice Hint
/// </summary>
public class Projectile_C007_4_Boss : MonoBehaviour
{
    public GameObject target;

    private Vector2 _lockedPosition = Vector2.zero;

    private float _lockTime;

    private void Start()
    {
        _lockTime = GetComponent<EnemyAttackHintBar>().warningTime;
        if(target == null)
            return;
        _lockedPosition = new Vector2(target.transform.position.x, BattleStageManager.Instance.mapBorderT);
    }

    private void Update()
    {
        if(_lockTime < 0)
            return;

        _lockTime -= Time.deltaTime;
        
        if (target != null)
        {
            _lockedPosition = new Vector2(target.transform.position.x,
            BasicCalculation.
                GetRaycastedPlatformY(new Vector2(target.transform.position.x, 
                    BattleStageManager.Instance.mapBorderT)));
            transform.position = _lockedPosition;
        }
    }
    
    
    
    
    
}
