using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceController_C002 : AudioManagerPlayer
{
    // Start is called before the first frame update
    
    protected override void DistributeMyVoice(AudioClip[] clips)
    {
        return;
    }
    public override void PlayAttackVoice(int id)
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
                if (rand < 3)
                {
                    clip = Combo3[0];
                }
                else if (rand < 6)
                {
                    clip = Combo3[1];
                }
                else
                {
                    clip = Combo3[2];
                }

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
