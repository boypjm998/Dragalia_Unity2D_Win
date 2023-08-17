using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneResourcesLoader : MonoBehaviour
{
    public string cutsceneName = "HB02";
    void Awake()
    {
        
    }

    private IEnumerator Start()
    {
        var rt = Resources.Load<GameObject>($"Timeline/{cutsceneName}_RT");
        var rtclone = Instantiate(rt, new Vector3(0,45,0), Quaternion.identity,transform);
        rtclone.name = "RT";
        yield break;
        yield return new WaitUntil(() => GlobalController.currentGameState == GlobalController.GameState.Inbattle);
        var fullscreen = GameObject.Find("FullScreenEffect").transform.Find("RT").gameObject;
        fullscreen.SetActive(true);
        fullscreen.GetComponent<RawImage>().texture = 
            transform.Find("RT/Camera").GetComponent<Camera>().targetTexture;
        rtclone.SetActive(true);
    }

    // Update is called once per frame
    
}
