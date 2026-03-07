using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class SkillUISpecial051 : SkillUIBase
{
    protected override void CheckSkillCD()
    {
        base.CheckSkillCD();
        if (!sm.HasCondition((int)BasicCalculation.BattleCondition.SoulSeal))
        {
            DisableSkill();
        }
        else {
            EnableSkill();
        }
    }
}
