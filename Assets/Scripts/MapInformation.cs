using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInformation : MonoBehaviour
{
    List<QuestSave> questSaveList = new();
    [Serializable] public class MapSpotInfo
    {
        //Map spot id is a unique id for each map spot, it is used to identify the map spot.
        //For example, a map spot's id is 1000, then when you open it, it will show a UI panel with id 1000.
        //If you click a entrance on the panel, the panel will shift to id 1000{button's id}.
        //For example, click the first entrance, the panel will shift to id 10001.
        public int mapSpotID;
        public Vector2 mapSpotPosition;
        public string mapSpotName;
    }
    
    [SerializeField] private List<MapSpotInfo> mapSpotInfoList;
    public bool tutorialCleared;

    public void MapClickedEvent(int panelID)
    {
        GlobalController.lastQuestSpot = panelID;
        //GameObject.Find("MapView").GetComponent<UISortingGroup>().clicked("OpenMapSpot", panelID);
        var mapView = GameObject.Find("MapView");
        mapView.GetComponent<UISortingGroup>().ToUIState(1010);
        mapView.GetComponent<UI_WorldMap>().InformPanelReload(panelID);
        
    }

    // Start is called before the first frame update
    void Awake()
    {
        questSaveList = GlobalController.Instance.GetQuestInfo();
        //如果questSaveList里面有元素的quest_id等于100001,令除了第一个子物体以外的子物体不可用
        if (questSaveList.Exists(x => x.quest_id == "100001"))
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }

            tutorialCleared = true;
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            for (int i = 1; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            tutorialCleared = false;
        }
    }

    public MapSpotInfo GetSpot(int id)
    {
        foreach (var VARIABLE in mapSpotInfoList)
        {
            print(VARIABLE.mapSpotID);
            if (VARIABLE.mapSpotID == id)
            {
                return VARIABLE;
            }
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
