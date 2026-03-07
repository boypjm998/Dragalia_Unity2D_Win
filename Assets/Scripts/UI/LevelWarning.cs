using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// 添加到关卡上，用于警告
/// </summary>
public class LevelWarning: MonoBehaviour
{
    List<AttackBase> attackList = new List<AttackBase>();
    bool initFlag = false;
    private TextMeshProUGUI questBanner;
    private Color warningColor = new Color(1, 0.5f, 0);
    private Color extremeWarningColor = new Color(1, 0, 0);
    
    //[SerializeField] private int recommendedAbilities = 0;
    [SerializeField] private int warningAbilities = -1;
    [Tooltip("需要小于warningAbilities")]
    [SerializeField] private int dangerAbilities = -1; //一定要小于warningAbilities

    private void OnEnable()
    {
        questBanner = GetComponentInChildren<TextMeshProUGUI>();
        int abilityLitUp = GetAbilityLitUp();
        if (dangerAbilities > 0 && abilityLitUp < dangerAbilities)
        {
            questBanner.color = extremeWarningColor;
        }
        else if(warningAbilities > 0 && abilityLitUp < warningAbilities)
        {
            questBanner.color = warningColor;
        }
        
    }


    private int GetAbilityLitUp()
    {
        var skillTreeInfo = GlobalController.Instance.gameOptions.skillTreeInfo;
        
        int totalLit = 0;

        foreach (var node in skillTreeInfo)
        {
            if(node == 1)
                totalLit++;
        }

        return totalLit;
    }
    
    
    
    
    
    
    
    
}

