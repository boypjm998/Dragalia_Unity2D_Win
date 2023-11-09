using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoiceController_C004 : AudioManagerPlayer
{
    // Start is called before the first frame update
    private void Start()
    {
        voice = GetComponent<AudioSource>();
    }

    protected override void DistributeMyVoice(AudioClip[] clips)
    {
        return;
    }

    public override void PlayAttackVoice(int id, bool ignoreCD = false)
    {
        if (voice.isPlaying || voiceCDRoutine!=null)
        {
            return;
        }

        voiceCDRoutine = StartCoroutine("WaitForVoiceCooldown");
        AudioClip clip;
        var rand = Random.Range(0, 10);
        switch (id)
        {
            case 0:
            {
                if (rand < 5)
                {
                    clip = Dash[0];
                }
                else clip = Dash[1];
                break;
            }
            case 1:
            {
                if (rand < 5)
                {
                    clip = Combo1[0];
                }
                else clip = Combo1[1];
                break;
            }
            case 2:
            {
                if (rand < 5)
                {
                    clip = Combo3[0];
                }
                else clip = Combo3[1];
                break;
            }
            case 3:
            {
                if (rand < 5)
                {
                    clip = Combo5[0];
                }
                else clip = Combo5[1];
                break;
            }
            default:
                clip = null;
                break;
        }

        voice.clip = clip;
        voice.Play();
    }

    
}
