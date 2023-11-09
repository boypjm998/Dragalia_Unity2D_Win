using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoiceControllerPlayer : AudioManagerPlayer
{
    public List<AudioClip> forceStrikeClips = new();
    public List<AudioClip> myClips = new();

    public AudioClip transformClip;
    // Start is called before the first frame update
    protected override void DistributeMyVoice(AudioClip[] clips)
    {
        
    }

    private void Start()
    {
        voice = GetComponentInChildren<AudioSource>();
    }

    

    public AudioClip GetExtraClip(int clipID)
    {
        clipID -= 4;
        Debug.Log($"CLIP[{clipID}]");
        if(myClips.Count>clipID)
            return myClips[clipID];
        else
            return null;
        
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
                    clip = transformClip;
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


    public override void PlayAttackVoice(int id,bool ignoreCD = false)
    {

        if (ignoreCD && voiceCDRoutine!=null)
        {
            StopCoroutine(voiceCDRoutine);
            voiceCDRoutine = null;
        }

        if (voice.isPlaying || voiceCDRoutine!=null)
        {
            return;
        }

        voiceCDRoutine = StartCoroutine("WaitForVoiceCooldown");
        AudioClip clip;
        int randomInt;
        try
        {
            switch (id)
            {
                case 0:
                {
                    randomInt = Random.Range(0, Dash.Length);
                    clip = Dash[randomInt];
                    break;
                }
                case 1:
                {
                    randomInt = Random.Range(0, Combo1.Length);
                    clip = Combo1[randomInt];
                    break;
                }
                case 2:
                {
                    randomInt = Random.Range(0, Combo3.Length);
                    clip = Combo3[randomInt];
                    break;
                }
                case 3:
                {
                    randomInt = Random.Range(0, Combo5.Length);
                    clip = Combo5[randomInt];
                    break;
                }
                case 9:
                {
                    randomInt = Random.Range(0, forceStrikeClips.Count);
                    clip = forceStrikeClips[randomInt];
                    break;
                }
                default:
                {
                    clip = GetExtraClip(id);
                    print(clip.name);
                    break;
                }

            }
        }
        catch (NullReferenceException e)
        {
            clip = null;
        }

        voice.clip = clip;
        voice.Play();

    }
}
