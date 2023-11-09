using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class StageCameraController : MonoBehaviour
{
    public static StageCameraController Instance;

    public Vector2 startPosition = Vector2.zero;
    
    protected Tweener cameraTweener;

    protected CinemachineVirtualCamera cmMain;
    public static GameObject MainCameraGameObject
    {
        get => mainCameraGameObject;
        //set => mainCameraGameObject = value;
    }

    public float MainCameraSize
    {
        get => mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>().m_Lens.OrthographicSize;
    }
    public Transform MainCameraFollowObject
    {
        get => mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>().Follow;
    }

    private static GameObject overallCameraGameObject;

    private static GameObject mainCameraGameObject;
    // Start is called before the first frame update
    public bool testFlag = false;
    private static int currentCamera = 1;

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
        mainCameraGameObject = gameObject;
        yield return new WaitUntil(() => GlobalController.currentGameState == GlobalController.GameState.Inbattle);
        overallCameraGameObject = GameObject.Find("OverallCamera");
        cmMain = mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (testFlag == true && currentCamera==1)
        {
            currentCamera = 2;
            SwitchOverallCamera();
        }
        if (testFlag == false && currentCamera==2)
        {
            currentCamera = 1;
            SwitchMainCamera();
        }
    }

    public static void SwitchOverallCamera()
    {
        //mainCameraGameObject = GameObject.Find("Main Camera");
        //overallCameraGameObject = GameObject.Find("OverallCamera");
        TargetAimer ta = GameObject.Find("PlayerHandle").GetComponentInChildren<TargetAimer>();
        ta.enabled = false;
        mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 0;
        overallCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 10;
        CineMachineOperator.Instance.StopCameraShake();
        overallCameraGameObject.GetComponentInChildren<CineMachineOperator>()?.SetInstance();
    }
    public static void SwitchMainCamera()
    {
        //mainCameraGameObject = GameObject.Find("Main Camera");
        //overallCameraGameObject = GameObject.Find("OverallCamera");
        TargetAimer ta = GameObject.Find("PlayerHandle").GetComponentInChildren<TargetAimer>();
        ta.enabled = true;
        overallCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 0;
        mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 10;
        CineMachineOperator.Instance.StopCameraShake();
        mainCameraGameObject.GetComponentInChildren<CineMachineOperator>()?.SetInstance();
    }

    public static GameObject GetCurrentCamera()
    {
        if (currentCamera == 1)
        {
            return mainCameraGameObject;
        }
        else
        {
            return overallCameraGameObject;
        }
    }

    public static void SwitchMainCameraFollowObject(GameObject target)
    {
        var camera = mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        camera.Follow = target.transform;

    }

    public static void SwitchOverallCameraFollowObject(GameObject target)
    {
        var camera = overallCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        if(target == null)
            camera.Follow = null;
        else
        {
            camera.Follow = target.transform;
        }
        
    }

    public static void SetMainCameraSize(int size)
    {
        var camera = mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        camera.m_Lens.OrthographicSize = size;
    }

    public void ToShapeshiftingView()
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        var camera = cmMain;
        //0.5s内将摄像机的m_Lens.OrthographicSize变为9
        cameraTweener = DOTween.To(() => camera.m_Lens.OrthographicSize,
            x => camera.m_Lens.OrthographicSize = x, 9,
            0.5f);
    }
    
    public void ToNormalView()
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        var camera = cmMain;
        //0.5s内将摄像机的m_Lens.OrthographicSize变为9
        cameraTweener = DOTween.To(() => camera.m_Lens.OrthographicSize,
            x => camera.m_Lens.OrthographicSize = x, 8,
            0.5f);
    }

}
