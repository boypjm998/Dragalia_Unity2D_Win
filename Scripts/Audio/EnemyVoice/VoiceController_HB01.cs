using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoiceController_HB01 : AudioManagerGeneral
{
    public enum myMoveList
    {
        SingleDodgeCombo,
        Combo,
        WarpAttack,
        CamineRush
        
    }
    void Start()
    {
        voice = gameObject.GetComponent<AudioSource>();
        voiceAssetBundlePath = "voice_c005";
        LoadMyVoice();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void DistributeMyVoice(AudioClip[] clips)
    {
        myClips = new AudioClip[30];
        
        foreach (var clip in clips)
        {
            switch (clip.name)
            {
                case "combo_1":
                    myClips[0] = clip;
                    break;
                case "combo_2":
                    myClips[1] = clip;
                    break;
                case "combo_3":
                    myClips[2] = clip;
                    break;
                case "combo_4":
                    myClips[3] = clip;
                    break;
                
                case "combo_5":
                    myClips[4] = clip;
                    break;
                case "combo_6":
                    myClips[5] = clip;
                    break;
                case "combo_7":
                    myClips[6] = clip;
                    break;
                
                case "dash_1":
                    myClips[7] = clip;
                    break;
                case "dash_2":
                    myClips[8] = clip;
                    break;
                case "roll":
                    myClips[9] = clip;
                    break;
                
                
                
                case "skill_1":
                    myClips[10] = clip;
                    break;
                case "skill_2":
                    myClips[11] = clip;
                    break;
                case "skill_3":
                    myClips[12] = clip;
                    break;
                case "skill_4":
                    myClips[13] = clip;
                    break;
                case "special_move_1":
                    myClips[20] = clip;
                    break;
                case "special_move_2":
                    myClips[21] = clip;
                    break;
                case "special_move_3":
                    myClips[22] = clip;
                    break;
                

                //UnImplemented
                default:
                    break;

            }
        }
    }

    public void PlayMyVoice(myMoveList moveName)
    {
        if (voice.isPlaying == true || voiceCDRoutine!=null)
        {
            return;
        }

        voiceCDRoutine = StartCoroutine("WaitForVoiceCooldown");
        AudioClip clip;
        int rand = 0;

        switch (moveName)
        {
            case myMoveList.Combo:
            {
                rand = Random.Range(0, 7);
                clip = myClips[rand];
                break;
            }
            case myMoveList.SingleDodgeCombo:
            {
                rand = Random.Range(0, 6);
                clip = myClips[rand];
                break;
            }
            case myMoveList.WarpAttack:
            {
                clip = myClips[20];
                break;
                    
            }
            case myMoveList.CamineRush:
            {
                clip = myClips[10];
                break;
            }
            default: clip = null;
                break;

        }
        voice.clip = clip;
        voice.Play();

    }
    
    IEnumerator WaitForVoiceCooldown()
    {
        yield return new WaitForSecondsRealtime(1f);
        voiceCDRoutine = null;
    }
    
}
