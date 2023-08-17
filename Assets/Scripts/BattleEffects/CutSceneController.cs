using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayableDirector _director;
    public RenderTexture rt;
    public Material skyMat;
    
    public void Replay()
    {
        //anim.Play("start");
        _director.Stop();
        _director.Play();
    }

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
        skyMat = GetComponentInChildren<Skybox>().material;
        
    }
}
