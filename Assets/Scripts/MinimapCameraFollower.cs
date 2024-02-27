using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraFollower : MonoBehaviour
{
    private Transform player;
    private PolygonCollider2D cameraRange;
    private float maxHeight;
    private float minHeight;
    private float maxWidth;
    private float minWidth;
    private Camera mainCamera;
    private Camera myCamera;
    
    public static MinimapCameraFollower Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }


    IEnumerator Start()
    {
        myCamera = GetComponent<Camera>();
        yield return new WaitUntil(()=>BattleStageManager.Instance.GetPlayer() != null);
        BattleStageManager.Instance.OnMapInfoRefresh += RefreshBorderInfo;
        mainCamera = Camera.main;
        player = BattleStageManager.Instance.GetPlayer().transform;
        cameraRange = GameObject.FindGameObjectWithTag("CameraRange").GetComponent<PolygonCollider2D>();
        RefreshBorderInfo();

    }

    private void RefreshBorderInfo()
    {
        maxHeight = cameraRange.bounds.max.y - myCamera.orthographicSize;
        minHeight = cameraRange.bounds.min.y + myCamera.orthographicSize;
        maxWidth = cameraRange.bounds.max.x - myCamera.orthographicSize * myCamera.aspect;
        minWidth = cameraRange.bounds.min.x + myCamera.orthographicSize * myCamera.aspect;
    }

    public void SetSize(float size)
    {
        myCamera.orthographicSize = size;
        RefreshBorderInfo();
    }

    private void LateUpdate()
    {
        if (player == null)
            return;
        
        var posy = player.position.y;
        if(posy > maxHeight)
            posy = maxHeight;
        else if(posy < minHeight)
            posy = minHeight;
        
        var posx = mainCamera.transform.position.x;
        if (posx > maxWidth)
            posx = maxWidth;
        else if (posx < minWidth)
            posx = minWidth;

        transform.position = new Vector3(posx,posy, transform.position.z);
    }
}
