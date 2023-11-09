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
    private AssetBundle _seBundle;
    public bool loadingEnd = false;
    [SerializeField] private TextAsset storyVoiceInfo;
    JsonData currentLevelVoiceData;

    private void Awake()
    {
        _globalController = FindObjectOfType<GlobalController>();
        _storySceneManager = FindObjectOfType<StorySceneManager>();
        
        if(GlobalController.questID == "100001")
            StartCoroutine(loadBundles());
        else
        {
            if(_storySceneManager.isDebug)
                GlobalController.questID = _storySceneManager.currentStoryID;
            print(GlobalController.questID);
            StartCoroutine(LoadStoryBundle());
        }

        
        
    }

    private void Start()
    {
        
    }

    IEnumerator loadBundles()
    {
        if (_globalController == null)
        {
            _globalController = FindObjectOfType<GlobalController>();
        }

        //load asset bundles asynchronously
        var bgmBundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath+"/story/bgm/ms_bgm");
        yield return bgmBundleRequest;
        
        _bgmBundle = bgmBundleRequest.assetBundle;
        _globalController.loadedBundles.Add("story/bgm/ms_bgm", _bgmBundle);
        
        var voiceBundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath+"/story/voice/ms_voice");
        yield return voiceBundleRequest;
        
        _voiceBundle = voiceBundleRequest.assetBundle;
        _globalController.loadedBundles.Add("story/voice/ms_voice", _voiceBundle);
        
        var seBundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath+"/story/se");
        yield return seBundleRequest;
        
        _seBundle = seBundleRequest.assetBundle;
        _globalController.loadedBundles.Add("story/se", _seBundle);
        
        InitVoiceData();
        loadingEnd = true;
    }

    IEnumerator LoadStoryBundle()
    {
        if (_globalController == null)
        {
            _globalController = FindObjectOfType<GlobalController>();
        }
        string storyName = GlobalController.questID;
        
        var bgmbundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath+"/story/bgm/ms_bgm");
        yield return bgmbundleRequest;
        
        _bgmBundle = bgmbundleRequest.assetBundle;
        
        _globalController.loadedBundles.Add("story/bgm/ms_bgm", _bgmBundle);
        
        var voicebundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath+$"/story/voice/{storyName}");
        yield return voicebundleRequest;
        
        _voiceBundle = voicebundleRequest.assetBundle;
        
        _globalController.loadedBundles.Add("story/voice/"+storyName, _voiceBundle);
        
        var seBundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath+"/story/se");
        yield return seBundleRequest;
        
        _seBundle = seBundleRequest.assetBundle;
        _globalController.loadedBundles.Add("story/se", _seBundle);
        
        
        InitVoiceData();
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

    public AudioClip GetSE(string name)
    {
        if (loadingEnd)
        {
            return _seBundle.LoadAsset<AudioClip>(name);
        }
        else
        {
            return null;
        }
    }

    private void InitVoiceData()
    {
        var levelName = _storySceneManager.GetLevelName();


        if (levelName != "main_story_001")
        {
            storyVoiceInfo = _voiceBundle.LoadAsset<TextAsset>("data.json");
        }


        var voiceData = JsonMapper.ToObject(storyVoiceInfo.text);
        currentLevelVoiceData = voiceData[levelName]["voice_info"];
        //print(currentLevelVoiceData["VO_STY_01_001"]["start"][0].ToString());
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
