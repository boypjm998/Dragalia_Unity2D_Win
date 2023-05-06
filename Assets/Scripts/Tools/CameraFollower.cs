using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private static float normalCameraSize = 8f;
    [SerializeField] private bool doScaleWithCamera = false;
    // Start is called before the first frame update
    private Camera currentCamera;
    void Start()
    {
        currentCamera = Camera.current;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (currentCamera == null)
        {
            if (Camera.current != null)
            {
                currentCamera = Camera.current;
            }
            else
            {
                currentCamera = Camera.main;
            }

        }
        transform.position = currentCamera.transform.position;

        if (doScaleWithCamera)
        {
            var scaleFactor = currentCamera.orthographicSize / normalCameraSize;
            if(scaleFactor < 1)
                scaleFactor = 1;
            transform.localScale = Vector3.one * scaleFactor;
        }

    }
}
