using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MainCameraStartPosition : MonoBehaviour
{
    public Vector2 startPosition = Vector2.zero;
    IEnumerator Start()
    {
        yield return new WaitUntil(()=>StageCameraController.MainCameraGameObject!=null);
        yield return new WaitUntil(()=>GlobalController.Instance.loadingEnd==true);
        if(StageCameraController.Instance.MainCameraFollowObject==null)
        {
            var cm = StageCameraController.MainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>();
            
            var newGameObject = Instantiate(new GameObject(), startPosition, Quaternion.identity);
            
            cm.Follow = newGameObject.transform;
            newGameObject.AddComponent<ObjectInvokeDestroy>();
            newGameObject.GetComponent<ObjectInvokeDestroy>().destroyTime = 1f;

            yield return new WaitForSeconds(1f);

            cm.Follow = null;
            
            // StageCameraController.MainCameraGameObject.transform.position =
            //     new Vector3(startPosition.x, startPosition.y,
            //         StageCameraController.MainCameraGameObject.transform.position.z);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
