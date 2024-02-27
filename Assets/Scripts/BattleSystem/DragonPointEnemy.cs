using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonPointEnemy : MonoBehaviour
{
    StatusManager _statusManager;
    public float totalDModeGain;
    [Range(1,40)]public int sensitivity = 10;
    private int checkedCount = 0;
    private List<float> checkedList;
    PlayerStatusManager playerStatusManager;

    private void Awake()
    {
        _statusManager = GetComponent<StatusManager>();
        InitCheckedList();
        
        _statusManager.OnHPChange += AddDModeGaugeToPlayer;
    }

    private void InitCheckedList()
    {
        //根据sensitivity初始化checkedList，将100分成sensitivity份，最后一份为0（也就是不包括100）
        checkedList = new List<float>();
        for (int i = 1; i <= sensitivity; i++)
        {
            checkedList.Add( 100f - (100f / sensitivity) * i);
        }

    }

    public void DisableAll()
    {
        checkedList.Clear();
    }

    private void AddDModeGaugeToPlayer()
    {
        float currentHPInPercent = 100 * _statusManager.currentHp / _statusManager.maxBaseHP;
        
        if (currentHPInPercent <= checkedList[checkedCount])
        {
            checkedCount++;
            if (playerStatusManager == null)
            {
                var viewerplayerStat = FindObjectOfType<PlayerStatusManager>();
                playerStatusManager = viewerplayerStat;
            }
            
            playerStatusManager.ChargeDP(totalDModeGain / sensitivity, false);

            if (checkedCount >= checkedList.Count)
            {
                _statusManager.OnHPChange -= AddDModeGaugeToPlayer;
            }
        }
    }



}
