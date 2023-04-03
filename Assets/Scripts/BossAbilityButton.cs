using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossAbilityButton : Button
{
    private UI_BossAbilityDisplayer info;
    protected override void Start()
    {
        base.Start();
        //info = GetComponent<UI_BossAbilityDisplayer>();
    }

    protected override void DoStateTransition(SelectionState state, bool instant) 
    {
 
        base.DoStateTransition(state, true); 
        info = GetComponent<UI_BossAbilityDisplayer>();
        switch (state) 
        {
            case SelectionState.Pressed:
                info.OnMouseEnter();
                break;
            
            case SelectionState.Highlighted: 
                Debug.Log("鼠标移到button上！");
                info.OnMouseEnter();
                break;
            default: 
                info.OnMouseExit();
                break; 
        } 
    } 
}
