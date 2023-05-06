using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class VoiceController_C001 : AudioManagerPlayer
{
    


    // Start is called before the first frame update
    IEnumerator Start()
    {
        voice = gameObject.GetComponent<AudioSource>();
        chara_id = 1;
        yield return new WaitUntil(() => GlobalController.currentGameState == GlobalController.GameState.WaitForStart);
        yield return new WaitForSeconds(0.1f);
        LoadMyVoice();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void DistributeMyVoice(AudioClip[] clips)
    {
        //var clipList = new List<AudioClip>(clips);

        Hurt = new AudioClip[4];
        Combo1 = new AudioClip[2];
        S1 = new AudioClip[2];
        S2 = new AudioClip[2];
        S3 = new AudioClip[2];
        S4 = new AudioClip[1];
        Dash = new AudioClip[2];
        Dodge = new AudioClip[1];

        foreach (var clip in clips)
        {
            switch (clip.name)
            {
                case "hurt_1":
                    Hurt[0] = clip;
                    break;
                case "hurt_2":
                    Hurt[1] = clip;
                    break;
                case "hurt_3":
                    Hurt[2] = clip;
                    break;
                case "hurt_4":
                    Hurt[3] = clip;
                    break;
                
                case "combo_1":
                    Combo1[0] = clip;
                    break;
                case "combo_2":
                    Combo1[1] = clip;
                    break;
                case "dash_1":
                    Dash[0] = clip;
                    break;
                case "dash_2":
                    Dash[1] = clip;
                    break;
                
                case "roll":
                    Dodge[0] = clip;
                    break;
                
                case "skill_1":
                    S1[0] = clip;
                    break;
                case "skill_2":
                    S2[0] = clip;
                    break;
                case "skill_3":
                    S3[0] = clip;
                    break;
                case "skill_4":
                    S4[0] = clip;
                    break;
                case "skill_5":
                    S1[1] = clip;
                    break;
                case "skill_6":
                    S2[1] = clip;
                    break;
                case "skill_7":
                    S3[1] = clip;
                    break;
                

                //UnImplemented
                default:
                    break;

            }
        }




        //Not implemented
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
                case 7:
                    clip = S3[1];
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id = 0">Dash attack</param>
    /// <param name="id = 1">Combo1</param>
    /// <exception cref="NotImplementedException"></exception>
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
            default:
                clip = null;
                break;
        }

        voice.clip = clip;
        voice.Play();
        
    }

    
}
