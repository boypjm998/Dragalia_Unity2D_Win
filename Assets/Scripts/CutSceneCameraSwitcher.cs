using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CutSceneCameraSwitcher : MonoBehaviour
{
    CinemachineVirtualCamera vcam1;
    CinemachineVirtualCamera vcam2;
    void Start()
    {
        vcam1 = transform.Find("Center1").GetComponent<CinemachineVirtualCamera>();
        vcam2 = transform.Find("Center2").GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    public void SwitchCamera(int id)
    {
        if (id == 1)
        {
            vcam1.Priority = 10;
            vcam2.Priority = -1;
            //vcam2.enabled = false;
            vcam1.GetComponent<Camera>().enabled = true;
            vcam2.GetComponent<Camera>().enabled = false;
        }
        else if (id == 2)
        {
            vcam2.Priority = 10;
            vcam1.Priority = -1;
            //vcam1.enabled = false;
            vcam2.GetComponent<Camera>().enabled = true;
            vcam1.GetComponent<Camera>().enabled = false;
        }
    }

    public void CameraShake(float intensity)
    {
        CineMachineOperator.Instance.CamaraShake(intensity, 0.5f);
    }
}
