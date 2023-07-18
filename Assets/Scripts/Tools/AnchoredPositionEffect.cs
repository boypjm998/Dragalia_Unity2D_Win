using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchoredPositionEffect : MonoBehaviour
{
    private Camera currentCamera;
    private Vector3 startPosition;

    private void Awake()
    {
        currentCamera = Camera.current;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // if (currentCamera == null)
        // {
        //     if (Camera.current != null)
        //     {
        //         currentCamera = Camera.current;
        //         print("CurrentCamera is not null");
        //     }
        //     else
        //     {
        //         currentCamera = Camera.main;
        //     }
        //
        // }
        //
        // transform.position = currentCamera.ScreenToWorldPoint(startPosition, currentCamera.stereoActiveEye);
    }
}
