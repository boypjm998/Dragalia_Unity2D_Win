using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
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
    }
}
