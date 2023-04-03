using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInformation : MonoBehaviour
{
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

    public void MapClickedEvent(int panelID)
    {
        //GameObject.Find("MapView").GetComponent<UISortingGroup>().clicked("OpenMapSpot", panelID);
        var mapView = GameObject.Find("MapView");
        mapView.GetComponent<UISortingGroup>().ToUIState(1010);
        mapView.GetComponent<UI_WorldMap>().InformPanelReload(panelID);
        GlobalController.lastQuestSpot = panelID;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
