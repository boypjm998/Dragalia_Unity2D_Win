using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public abstract  class AudioManagerGeneral : MonoBehaviour, IVoice
{
    public AudioSource voice;
    protected UI_DialogDisplayer _dialogDisplayer;
    protected Coroutine voiceCDRoutine;
    public AudioClip[] myClips;

    [HideInInspector] public string voiceAssetBundlePath;
    private GlobalController _globalController;
    
    public enum MyMoveList
    {
        
    }

    protected void CheckVoiceVolume()
    {
        if (GlobalController.Instance.gameOptions.soundSettings[1] == 0)
        {
            voice.mute = true;
        }
        else
        {
            voice.mute = false;
        }
    }

    protected IEnumerator WaitForVoiceCooldown()
    {
        yield return new WaitForSecondsRealtime(1f);
        voiceCDRoutine = null;
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
