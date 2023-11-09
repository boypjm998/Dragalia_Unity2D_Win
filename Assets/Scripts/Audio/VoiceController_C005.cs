using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceController_C005 : AudioManagerPlayer
{
    private void Start()
    {
        voice = GetComponent<AudioSource>();
    }

    protected override void DistributeMyVoice(AudioClip[] clips)
    {
        return;
    }

    public override void PlaySkillVoice(int id)
    {
        if (isSkillVoice == true)
        {
            return;
        }
        else
        {
            voice.Stop();
            AudioClip clip;
            switch (id)
            {
                case 1:
                    clip = S1[0];
                    break;
                case 2:
                    clip = S2[0];
                    break;
                case 3:
                    clip = S3[0];
                    break;
                case 4:
                    clip = S4[0];
                    break;
                case 5:
                    clip = S1[1];
                    break;
                case 6:
                    clip = S2[1];
                    break;

                default:
                    clip = null;
                    break;
            }

            voice.clip = clip;
            voice.Play();
            //voice.PlayOneShot(clip);
        }
    }

    public override void PlayAttackVoice(int id,bool ignoreCD=false)
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

    public override void PlayDodgeVoice()
    {
        if (voice.isPlaying || voiceCDRoutine!=null)
        {
            return;
        }
        
        var rand = Random.Range(0, 10);
        if (rand < 6)
        {
            voiceCDRoutine = StartCoroutine("WaitForVoiceCooldown");

            voice.clip = Dodge[0];
            voice.Play();
        }
        else
        {
            
        }
        
    }
}
