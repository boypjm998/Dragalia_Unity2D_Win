using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;

public class AudioScheduleManager : MonoBehaviour
{
    //[SerializeField] private AudioClip introClip;
    [SerializeField] private AudioClip loopClip;
    [SerializeField] private string loopClipABName;
    [SerializeField] private string loopClipAssetName;
    private AudioSource extraBGMSource;
    
    
    IEnumerator Start()
    {
        yield return null;
        
        //1. If loopClip is null, then output the error message to a log file.
        
        var logPath = Application.dataPath + "/Log.txt";

        #if !UNITY_EDITOR

        while (loopClip == null && GlobalController.Instance.GetBundle(loopClipABName) == null)
        {
            yield return null;
            
            var bundle = GlobalController.Instance.GetBundle(loopClipABName);

            if (bundle != null)
            {
                loopClip = bundle.LoadAsset<AudioClip>(loopClipAssetName);
            }
            else
            {
                // using (StreamWriter sw = new StreamWriter(logPath, true))
                // {
                //     sw.WriteLine("AudioScheduleManager: loopClip is null");
                // }
            }

        }

        
        
        #endif
        

        yield return new WaitUntil(()=>loopClip != null);

        
        // if (loopClip == null)
        // {
        //     var ab = AssetBundle.LoadFromFile
        //         (Path.Combine(Application.streamingAssetsPath, loopClipABName));
        //     loopClip = ab.LoadAsset<AudioClip>(loopClipAssetName);
        // }
        
        
        
        SetNewBGMSource();
        BattleEffectManager.Instance.OnBGMClipPlay += SetLoopTime;
        
        
        
    }

    private void SetNewBGMSource()
    {
        print("SetNewBGMSourceA");
        if (loopClip != null)
        {
            print("SetNewBGMSourceB");
            var newSourceGO = new GameObject();
            
            newSourceGO.transform.SetParent(transform);
            newSourceGO.name = "ExtraBGMSource";
            extraBGMSource = newSourceGO.AddComponent<AudioSource>();
            extraBGMSource.loop = true;
            extraBGMSource.playOnAwake = false;
            newSourceGO.tag = "Music";
            if (GlobalController.Instance.gameOptions.soundSettings[0] == 0)
            {
                extraBGMSource.mute = true;
            }
        }
    }

    void SetLoopTime(AudioClip clip, float currentTime)
    {
        if(clip.name == loopClip.name)
            return;
        
        double introDuration = (double)clip.samples / clip.frequency;

        BattleEffectManager.Instance.OnBGMClipPlay -= SetLoopTime;

        double totalTime = introDuration;


        DOVirtual.DelayedCall(clip.length - 0.1f - currentTime, () =>
        {
            PlayLoop();
        });

        DOVirtual.DelayedCall(clip.length - currentTime, () =>
        {
            SetMainBGMSource();
        });
        
        // Invoke("PlayLoop",clip.length - 0.1f - currentTime);
        // Invoke("SetMainBGMSource", clip.length - currentTime);

    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    void PlayLoop()
    {
        extraBGMSource.clip = loopClip;
        extraBGMSource.Play();
    }

    void SetMainBGMSource()
    { //BattleEffectManager.Instance.PlayBGM(false);
        BattleEffectManager.Instance.SetOtherBGMSource(extraBGMSource);
    }


}
