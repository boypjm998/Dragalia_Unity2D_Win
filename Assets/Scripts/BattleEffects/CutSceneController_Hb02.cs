using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutSceneController_Hb02 : MonoBehaviour
{
    public PlayableDirector _director;
    public bool debug;
    public RenderTexture rt;
    public Material skyMat;
    public Color originColor; 
    protected Color worldColor1;
    protected Color worldColor2;

    protected PlayableDirector director;
    protected Animator anim;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
        skyMat = GetComponentInChildren<Skybox>().material;
        //originColor = skyMat.GetColor("_Tint");
        originColor = new Color(0.6f,0.6f,0.6f,0.5f);
        worldColor1 = new Color(141f/255f, 1, 134/255f,0.5f);
        worldColor2 = new Color(1, 91f/255f, 180/255f,0.5f);
        skyMat.SetColor("_Tint",originColor);
        director = GetComponent<PlayableDirector>();
        //anim = transform.Find("Actor/Model").GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        var signal_track = director.playableAsset.outputs.First(c => c.streamName == "Signal Track");
        director.SetGenericBinding(signal_track.sourceObject,gameObject.GetComponentInChildren<SignalReceiver>());
        
        //"Animation Track (1)"
        //var anim_track2 = director.playableAsset.outputs.First(c => c.streamName == "Animation Track (1)");
        //director.SetGenericBinding(signal_track.sourceObject,transform.Find("Actor/Model").GetComponentInChildren<Animator>());
        
    }


    public void Replay()
    {
        //anim.Play("start");
        _director.Stop();
        _director.Play();
    }

    public void SetSkyBoxColor(int id)
    {
        if (id == 0)
        {
            skyMat.SetColor("_Tint",originColor);
        }
        else if(id == 1)
        {
            
            skyMat.SetColor("_Tint",worldColor1);
        }
        else
        {
            skyMat.SetColor("_Tint",worldColor2);
        }

    }

}
