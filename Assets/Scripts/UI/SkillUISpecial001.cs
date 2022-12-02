using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUISpecial001 : SkillUIBase
{
    //This script is the skill UI for Character001: Ilia

    private AlchemicGauge alchemicGauge;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        alchemicGauge = GameObject.Find("AlchemicGauge").GetComponentInChildren<AlchemicGauge>();
    }

    // Update is called once per frame
    void Update()
    {
        if (alchemicGauge.GetActiveCatridgeNumber() > 0)
        {
            SwapSkillIcon(1);
        }
        else {
            SwapSkillIcon(0);
        }

        if (alchemicGauge.GetCatridgeNumber() < 1 && sid == 2)
        {
            DisableSkill();
        }
        else {
            EnableSkill();
        }

        CheckSkillCD();




    }




}
