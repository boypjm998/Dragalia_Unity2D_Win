using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public abstract  class AudioManagerGeneral : MonoBehaviour
{
    protected AudioSource voice;
    protected Coroutine voiceCDRoutine;
    public AudioClip[] myClips;

    public string voiceAssetBundlePath;
    
    
    
    protected virtual void LoadMyVoice()
    {
        AssetBundle assetBundle = AssetBundle.LoadFromFile
            (Path.Combine(Application.streamingAssetsPath, GetVoicePath(voiceAssetBundlePath)));
        var voicePack = assetBundle.LoadAllAssets<AudioClip>();
        DistributeMyVoice(voicePack);
    }

    protected virtual string GetVoicePath(string assetBundleName)
    {
        StringBuilder sb = new StringBuilder(assetBundleName);


        return sb.ToString();
    }

    protected abstract void DistributeMyVoice(AudioClip[] clips);

    public virtual void PlayMyVoice(int moveID)
    {
    }

    public virtual void PlayMyVoiceWithDialogBanner(int moveID)
    {
        PlayMyVoice(moveID);
        
    }
}
