using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUISpecial033 : SkillUIBase
{
    private ActorController_c033 ac;

    protected override void Start()
    {
        base.Start();
        ac = BattleStageManager.Instance.GetPlayer().GetComponent<ActorController_c033>();

        if (sid == 3)
        {
            ac.OnTrapSetDelegate += SetIcon;
        }else if (sid == 1)
        {
            ac.OnSigilReleaseDelegate += SetIconTrue;
        }




    }

    private void OnDestroy()
    {
        if(sid == 3)
            ac.OnTrapSetDelegate -= SetIcon;
        
        if (sid == 1)
            ac.OnSigilReleaseDelegate -= SetIconTrue;
    }

    private void SetIcon(bool flag)
    {
        if (flag == true)
        {
            SwapSkillIcon(1);
        }else if (flag == false)
        {
            SwapSkillIcon(0);
        }
    }
    
    private void SetIconTrue()
    {
        SwapSkillIcon(1);
    }



}
