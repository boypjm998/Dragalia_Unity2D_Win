using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneResourcesLoader : MonoBehaviour
{
    public string cutsceneName = "HB02";

    private static CutsceneResourcesLoader _instance;
    
    public static CutsceneResourcesLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CutsceneResourcesLoader>();
            }

            return _instance;
        }
    }

    public GameObject FullScreenRtuiGameObject { get; private set; }

    public GameObject RTSceneGameObject
    {
        get;
        private set;
    }
    
    void Awake()
    {
        if(_instance == null)
            _instance = this;
        else if(_instance != this)
            Destroy(this);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private IEnumerator Start()
    {
        if (transform.Find("RT") != null)
        {
            RTSceneGameObject = transform.Find("RT").gameObject;
            FullScreenRtuiGameObject = GameObject.Find("FullScreenEffect").transform.Find("RT").gameObject;
            yield break;
        }
        
        var rt = Resources.Load<GameObject>($"Timeline/{cutsceneName}_RT");
        var rtclone = Instantiate(rt, new Vector3(0,65,0), Quaternion.identity,transform);
        rtclone.name = "RT";


        RTSceneGameObject = rtclone;
        FullScreenRtuiGameObject = GameObject.Find("FullScreenEffect").transform.Find("RT").gameObject;
        
        yield break;
        
        // //debug
        // yield return new WaitUntil(() => GlobalController.currentGameState == GlobalController.GameState.Inbattle);
        // var fullscreen = GameObject.Find("FullScreenEffect").transform.Find("RT").gameObject;
        // fullscreen.SetActive(true);
        // FullScreenRtuiGameObject = fullscreen;
        // fullscreen.GetComponent<RawImage>().texture = 
        //     transform.Find("RT/Camera").GetComponent<Camera>().targetTexture;
        // rtclone.SetActive(true);
    }

    // Update is called once per frame
    
}
