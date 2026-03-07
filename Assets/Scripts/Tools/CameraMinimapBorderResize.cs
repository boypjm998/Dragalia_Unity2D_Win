using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraMinimapBorderResize : MonoBehaviour
{
    //CinemachineVirtualCamera cmMain;
    const float initOrthographicSize = 8.0f;
    Vector2 initialSize;
    SpriteRenderer borderSpriteRenderer;

    private void Awake()
    {
        initialSize = transform.localScale;
        borderSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        if (StageCameraController.Instance)
        {
            borderSpriteRenderer.transform.localScale = initialSize * (StageCameraController.Instance.currentMainCameraSize / initOrthographicSize);
        }
    }
}
