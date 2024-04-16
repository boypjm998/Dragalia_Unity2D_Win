using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_LevelSubSelection : MonoBehaviour
{
    

    [SerializeField] List<UI_LevelSelection.SelectionMenuInfo> selectionMenuInfo;
    // Start is called before the first frame update
    public void EnterNextMenu(int id)
    {
        GetComponentInParent<UISortingGroup>().ToUIState(id);
    }

    public List<UI_LevelSelection.SelectionMenuInfo> GetAllSubMenu()
    {
        var menu = new List<UI_LevelSelection.SelectionMenuInfo>();

        for (int i = 0; i < selectionMenuInfo.Count; i++)
        {
            var bannerLock = transform.GetChild(i).GetComponent<LevelBannerLock>();
            if (bannerLock == null)
            {
                menu.Add(selectionMenuInfo[i]);
                continue;
            }

            var preID = bannerLock.PreID;

            // print(GlobalController.Instance.GetQuestInfo().
            //     Exists(x => x.quest_id == preID));
            
            
            if (GlobalController.Instance.GetQuestInfo().Exists(x => x.quest_id == preID))
            {
                menu.Add(selectionMenuInfo[i]);
                print("ADDED A Locked");
            }
            else
            {
                print("LOCKED"+gameObject.name);
            }
        }
        
        
        return menu;
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

    private void OnEnable()
    {
        var clearedQid = GlobalController.Instance.GetAllClearedQuestID();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            var isNew = transform.GetChild(i).Find("New");
            
            if (isNew != null)
            {
                var info = selectionMenuInfo[i].menuPrefab;
                var unviewedLevels = UI_LevelSelection.GetUnviewedLevels(info);
                var cnt = unviewedLevels.Except(GlobalController.Instance.gameOptions.visitedQuest).ToList().Count;

                if (cnt > 0)
                {
                    isNew.gameObject.SetActive(true);
                }
                else
                {
                    isNew.gameObject.SetActive(false);
                }
                
            }
        }
    }
}
