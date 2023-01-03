using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_BossStatus : MonoBehaviour
{
    private GameObject boss;
    // Start is called before the first frame update
    private UI_BossConditionBar _conditionBar;
    private UI_BossHPBar _HPbar;
    private GameObject _abilityIcons;
    private TextMeshProUGUI _bossName;
    
    
    
    
    IEnumerator Start()
    {
        yield return new WaitUntil(() => boss != null);
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBoss(GameObject boss)
    {
        this.boss = boss;
    }

    void Init()
    {
        var bossStat = boss.GetComponentInChildren<StatusManager>();
        _HPbar = GetComponentInChildren<UI_BossHPBar>();
        _bossName = GetComponentInChildren<TextMeshProUGUI>();
        _conditionBar = GetComponentInChildren<UI_BossConditionBar>();
        _abilityIcons = transform.Find("BossAbilities").gameObject;
        //Load Ability Icon Not IMPLIED
        _bossName.text = bossStat.displayedName;
        _conditionBar.SetTargetStat(bossStat);
        bossStat.SetConditionBar(_conditionBar);
        _HPbar.SetTarget(bossStat);
        
    }
}
