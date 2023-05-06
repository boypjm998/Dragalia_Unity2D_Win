using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using LitJson;
using UnityEngine.UI;

public class UI_WorldMap : MonoBehaviour
{
    private GameObject content;
    private GlobalController _globalController;
    private MapInformation _mapInformation;
    private int lastQuestID = -1;
    private ScrollRect _scrollRect;
    private Vector3 startPosition;
    protected UI_LevelSelection _levelSelection;

    // Start is called before the first frame update
    private void Awake()
    {
        content = transform.Find("Viewport/Content").gameObject;
        startPosition = content.transform.localPosition;
        _scrollRect = GetComponent<ScrollRect>();
        _levelSelection = transform.parent.Find("LevelSelection").GetComponent<UI_LevelSelection>();
        //print(content);
        _globalController = FindObjectOfType<GlobalController>();
        _mapInformation = transform.Find("Viewport/Content/MapContainer").GetComponentInChildren<MapInformation>();
    }

    private void OnEnable()
    {
        MapInformation.MapSpotInfo mapSpotInfo;
        lastQuestID = GlobalController.lastQuestSpot;
        if (lastQuestID == -1)
        {
            mapSpotInfo = _mapInformation?.GetSpot(100);
            SetScrollRectDisable();
            StartCoroutine(ZoomToQuestSpot(mapSpotInfo));
        }
        else
        {
            mapSpotInfo = _mapInformation?.GetSpot(GlobalController.lastQuestSpot);
            //SetScrollRectDisable();
            content.transform.localPosition = (startPosition - 2 * (Vector3)mapSpotInfo.mapSpotPosition);
            content.transform.localScale = new Vector3(2, 2, 1);
        }
    }

    private IEnumerator ZoomToQuestSpot(MapInformation.MapSpotInfo spot,float time = 0.5f)
    {

        yield return new WaitForSecondsRealtime(time);
        
        var tweener_1 = content.transform.DOScale(2f, 0.5f);
        
        //将spot的位置移到屏幕中心
        //_scrollRect.content.transform.position = -spot.mapSpotPosition*2;
        //print();
        content.transform.DOLocalMove(startPosition - 2*(Vector3)spot.mapSpotPosition, 0.5f);

        //print(-spot.mapSpotPosition);



        //var tweener_2 = content.transform.DOMove(-spot.mapSpotPosition, 0.5f);
        tweener_1.OnComplete(() =>
        {
            //启用ScrollRect
            if(time != 0)
                SetScrollRectEnable();
            GlobalController.lastQuestSpot = spot.mapSpotID;
        });

    }

    private void SetScrollRectDisable()
    {
        //禁用ScrollRect
        _scrollRect.enabled = false;
    }
    private void SetScrollRectEnable()
    {
        //启用ScrollRect
        _scrollRect.enabled = true;
    }

    public void InformPanelReload(int panelID)
    {
        _levelSelection.Reload(panelID);
    }







    // Update is called once per frame
    void Update()
    {
        //当ScrollRect被禁用时，跳过Update
        if (!_scrollRect.enabled)
        {
            return;
        }
        //鼠标滚轮滚动缩放content的大小
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            content.transform.localScale += new Vector3(0.1f, 0.1f);
            //最大缩放到2倍
            if (content.transform.localScale.x > 2)
            {
                content.transform.localScale = new Vector3(2, 2, 1);
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            content.transform.localScale -= new Vector3(0.1f, 0.1f);
            //最小缩放到1倍
            if (content.transform.localScale.x < 1)
            {
                content.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
