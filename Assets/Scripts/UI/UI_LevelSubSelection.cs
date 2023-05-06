using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LevelSubSelection : MonoBehaviour
{
    [SerializeField] List<UI_LevelSelection.SelectionMenuInfo> selectionMenuInfo;
    // Start is called before the first frame update
    public void EnterNextMenu(int id)
    {
        GetComponentInParent<UISortingGroup>().ToUIState(id);
    }

    private void Start()
    {
        //档UI_Levelselection.Instance的menuItems的id中不包含info中的id时，添加info
        foreach (var info in selectionMenuInfo)
        {
            if (!UI_LevelSelection.Instance.menuItems.Exists(x => x.menuID == info.menuID))
            {
                UI_LevelSelection.Instance.AddPanel(info);
            }
        }
    }
}
