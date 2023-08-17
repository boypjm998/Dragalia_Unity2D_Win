using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class AudioBundlesTest : MonoBehaviour
{
    // Start is called before the first frame update
    GlobalController _globalController;
    StorySceneManager _storySceneManager;
    private AssetBundle _bgmBundle;
    private AssetBundle _voiceBundle;
    public bool loadingEnd = false;
    [SerializeField] private TextAsset storyVoiceInfo;
    JsonData currentLevelVoiceData;

    private void Awake()
    {
        _globalController = GlobalController.Instance;
        _storySceneManager = FindObjectOfType<StorySceneManager>();
        InitVoiceData();
        StartCoroutine(loadBundles());
        
    }

    private void Start()
    {
        
    }

    IEnumerator loadBundles()
    {
        //load asset bundles asynchronously
        var bgmBundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath+"/story/ms_bgm");
        yield return bgmBundleRequest;
        
        _bgmBundle = bgmBundleRequest.assetBundle;
        _globalController.loadedBundles.Add("story/ms_bgm", _bgmBundle);
        
        var voiceBundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath+"/story/ms_voice");
        yield return voiceBundleRequest;
        
        _voiceBundle = voiceBundleRequest.assetBundle;
        _globalController.loadedBundles.Add("story/ms_voice", _voiceBundle);
        loadingEnd = true;
    }
    
    public AudioClip GetBGM(string name)
    {
        if (loadingEnd)
        {
            return _bgmBundle.LoadAsset<AudioClip>(name);
        }
        else
        {
            return null;
        }
    }
    
    public AudioClip GetVoice(string name)
    {
        if (loadingEnd)
        {
            return _voiceBundle.LoadAsset<AudioClip>(name);
        }
        else
        {
            return null;
        }
    }

    private void InitVoiceData()
    {
        var levelName = _storySceneManager.GetLevelName();
        var voiceData = JsonMapper.ToObject(storyVoiceInfo.text);
        currentLevelVoiceData = voiceData[levelName]["voice_info"];
        print(currentLevelVoiceData["VO_STY_01_001"]["start"][0].ToString());
    }

    public void GetVoiceInfo(string voice_name,ref List<float> start_times, ref List<float> end_times)
    {
        var voice_info = currentLevelVoiceData[voice_name];
        foreach (var start_time in voice_info["start"])
        {
            start_times.Add(float.Parse(start_time.ToString()));
        }
        foreach (var end_time in voice_info["end"])
        {
            end_times.Add(float.Parse(end_time.ToString()));
        }
    }



}
