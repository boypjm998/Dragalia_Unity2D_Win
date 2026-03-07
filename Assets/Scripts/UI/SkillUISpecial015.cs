using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class SkillUISpecial015 : SkillUIBase
{
    private ChargeGauge_C015 _gauge;
    
    protected override void Start()
    {
        base.Start();
        BattleStageManager.Instance.OnGameStart += GetGauge;
    }

    private void GetGauge()
    {
        _gauge = ChargeGauge_C015.Instance;
        BattleStageManager.Instance.OnGameStart -= GetGauge;
    }

    protected override void CheckSkillCD()
    {
        base.CheckSkillCD();

        if (_gauge)
        {
            if (sm.HasCondition((int)BasicCalculation.BattleCondition.EdenMode))
            {
                EnableSkill();
            }
            else if(_gauge.currentCp < 33)
            {
                DisableSkill();
            }
            else
            {
                EnableSkill();
            }
        }
    }
}
