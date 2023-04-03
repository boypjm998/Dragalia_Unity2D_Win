using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class UI_LevelEnterButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnClick(int id)
    {
        GetComponentInParent<UISortingGroup>().ToBattle(id);
    }

    private void OnEnable()
    {
        var questSaveData = GlobalController.questSaveDataString;
        if(questSaveData == null)
            return;
        var questSaveDataList = JsonMapper.ToObject<QuestDataList>(questSaveData);
        var questInfoList = questSaveDataList.quest_info;
        //遍历transform下所有子物体
        foreach (Transform child in transform)
        {
            //如果子物体下子物体数大于2
            if (child.childCount > 2)
            {
                int quest_id = Convert.ToInt32(child.name);
                //如果questInfoList中有QuestSave内quest_id为子物体名字的元素
                var matchedInfo = questInfoList.Find(x => x.quest_id == child.name);
                if (matchedInfo != null)
                {
                    //读取matchedInfo中的crown_1,crown_2,crown_3
                    var crown_1 = matchedInfo.crown_1;
                    var crown_2 = matchedInfo.crown_2;
                    var crown_3 = matchedInfo.crown_3;
                    //获取child下第三个子物体
                    var crown = child.GetChild(2);
                    //如果crown_1为1，显示crown下第一个子物体中的第一个子物体
                    if (crown_1 == 1)
                    {
                        crown.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    }
                    if (crown_2 == 1)
                    {
                        crown.GetChild(1).GetChild(0).gameObject.SetActive(true);
                    }
                    if (crown_3 == 1)
                    {
                        crown.GetChild(2).GetChild(0).gameObject.SetActive(true);
                    }
                }


            }

        }

    }
}
