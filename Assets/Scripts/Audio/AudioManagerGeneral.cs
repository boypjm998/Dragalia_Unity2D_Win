using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public abstract  class AudioManagerGeneral : MonoBehaviour, IVoice
{
    protected AudioSource voice;
    protected Coroutine voiceCDRoutine;
    public AudioClip[] myClips;

    [HideInInspector] public string voiceAssetBundlePath;
    private GlobalController _globalController;
    
    public enum MyMoveList
    {
        
    }
    
    protected virtual void LoadMyVoice()
    {
        _globalController = FindObjectOfType<GlobalController>();
        AssetBundle assetBundle = _globalController.GetBundle(voiceAssetBundlePath);
        print("voiceAssetBundlePath: " + voiceAssetBundlePath);
        //AssetBundle assetBundle = AssetBundle.LoadFromFile
        //    (Path.Combine(Application.streamingAssetsPath, GetVoicePath(voiceAssetBundlePath)));
        var voicePack = assetBundle.LoadAllAssets<AudioClip>();
        DistributeMyVoice(voicePack);
    }

    protected static string GetVoicePath(string assetBundleName)
    {
        StringBuilder sb = new StringBuilder(assetBundleName);


        return sb.ToString();
    }

    protected abstract void DistributeMyVoice(AudioClip[] clips);

    

    public virtual void BroadCastMyVoice(int moveID)
    {
        
        
    }
}
