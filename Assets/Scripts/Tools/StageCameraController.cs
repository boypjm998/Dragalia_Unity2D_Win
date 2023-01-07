using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class StageCameraController : MonoBehaviour
{
    private static GameObject overallCameraGameObject;

    private static GameObject mainCameraGameObject;
    // Start is called before the first frame update
    public bool testFlag = false;
    private static int currentCamera = 1;
    void Start()
    {
        mainCameraGameObject = gameObject;
        overallCameraGameObject = GameObject.Find("OverallCamera");
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

    }
    public static void SwitchMainCamera()
    {
        //mainCameraGameObject = GameObject.Find("Main Camera");
        //overallCameraGameObject = GameObject.Find("OverallCamera");
        TargetAimer ta = GameObject.Find("PlayerHandle").GetComponentInChildren<TargetAimer>();
        ta.enabled = true;
        overallCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 0;
        mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 10;
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
}
