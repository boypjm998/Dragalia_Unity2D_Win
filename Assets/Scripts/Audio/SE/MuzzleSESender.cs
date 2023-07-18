using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MuzzleSESender : MonoBehaviour
{
    public AudioClip SEClip;
    public List<AudioClip> extraClips = new();
    public List<float> sendTimes = new();
    [Range(0,1)]public float volume = 1;
    public bool withContainer = false;
    AudioSource _audioSource;
    public bool playEvenNotInView = false;
    private void Start()
    {
        if (SEClip!=null)
        {
            SendVoiceToPlay(this.SEClip);
        }
        
        if (sendTimes.Count > 0)
        {
            foreach (var sendTime in sendTimes)
            {
                Invoke(nameof(PlayNextSound),sendTime);
            }
        }
        
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    public void SendVoiceToPlay(AudioClip SEClip)
    {
        if (withContainer)
        {
            var container = GetComponentInParent<AttackContainer>();
            _audioSource = container.GetComponent<AudioSource>();
        }
        else
        {
            _audioSource = BattleEffectManager.Instance.soundEffectSource;
        }

        if (playEvenNotInView)
        {
            _audioSource.PlayOneShot(SEClip,volume);
            return;
        }

        try
        {
            Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            {
                
                _audioSource.PlayOneShot(SEClip,volume);
                
            }
        }
        catch
        {
            _audioSource.PlayOneShot(SEClip,volume);
            //BattleEffectManager.Instance.PlayOtherSE(SEClip.name,volume);
            print("Play with exception");
        }
    }

    protected void PlayNextSound()
    {
        if (extraClips.Count > 0)
        {
            var clip = extraClips[0];
            extraClips.RemoveAt(0);
            SendVoiceToPlay(clip);
        }
        else
        {
            SendVoiceToPlay(SEClip);
        }
    }
}
