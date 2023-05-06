using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_BossStatusSpecial : UI_BossStatus
{
    
    void Start()
    {
        _battleStageManager = BattleStageManager.Instance;
        Init();
    }

    // Update is called once per frame
    protected override void Init()
    {
        SetBoss(this.bossStat.gameObject);
        var bossStat = boss.GetComponentInChildren<SpecialStatusManager>();

        if (bossStat != null)
        {
            var odBar = transform.Find("ODBar").gameObject;
            odBar.SetActive(true);
            odBar.GetComponent<UI_BossODBar>().bossStat = bossStat as SpecialStatusManager;
            this.bossStat = bossStat;
        }
        else
        {
            var bossStat2 = boss.GetComponentInChildren<StatusManager>();
            this.bossStat = bossStat2;
        }
        
        
        
        _HPbar = GetComponentInChildren<UI_BossHPBar>();
        _HPbar.SetTarget(this.bossStat);
        _conditionBar = GetComponentInChildren<UI_BossConditionBar>();
        _conditionBar.SetTargetStat(this.bossStat);
        this.bossStat.SetConditionBar(_conditionBar);
        
        
        
    }
}
