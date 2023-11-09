using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceController_C010 : AudioManagerPlayer
{

    [Header("Metamorphosis Form")]
    public AudioClip transformClip;
    public AudioClip[] ComboDMode;
    public AudioClip[] SkillsDMode;
    
    // Start is called before the first frame update
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
                    clip = transformClip;
                    break;
                case 4:
                    clip = S4[0];
                    break;
                case 5:
                    clip = SkillsDMode[0];
                    break;
                case 6:
                    clip = SkillsDMode[1];
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
                clip = transformClip;
                break;
            }
            case 4:
            {
                if (rand < 7)
                {
                    clip = ComboDMode[0];
                }
                else clip = ComboDMode[0];
                break;
            }
            case 5:
            {
                if (rand < 3)
                {
                    clip = ComboDMode[1];
                }
                clip = ComboDMode[1];
                break;
            }
            case 6:
            {
                clip = ComboDMode[2];
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
