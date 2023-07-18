using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceController_HB02 : AudioManagerGeneral
{
    // Start is called before the first frame update
    public enum myMoveList
    {
        ComboA,
        ComboC,
        ComboB,
        Dash,
        GloriousSanctuary,
        HolyCrown,
        CelestialPrayer,
        TwilightCrown,
        TwilightMoon,
        FaithEnhancement,
        MountainPrison,
        LastPhase,
        Defeat
    }

    protected override void DistributeMyVoice(AudioClip[] clips)
    {
        //Manual
    }
    
    void Start()
    {
        _dialogDisplayer = FindObjectOfType<UI_DialogDisplayer>();
        voice = gameObject.GetComponent<AudioSource>();
        
    }
    
    public void PlayMyVoice(myMoveList moveName,bool broadcast = false)
    {
        if(myClips.Length<=0)
            return;
        if (voice.isPlaying == true || voiceCDRoutine!=null)
        {
            return;
        }

        voiceCDRoutine = StartCoroutine("WaitForVoiceCooldown");
        AudioClip clip;
        int voiceID = 0;
        

        switch (moveName)
        {
            case myMoveList.ComboA:
            {
                voiceID = Random.Range(0, 3);
                voice.clip = myClips[voiceID];
                break;
            }
            case myMoveList.ComboB:
            {
                voiceID = Random.Range(1, 4);
                voice.clip = myClips[voiceID];
                break;
            }
            case myMoveList.ComboC:
            {
                voice.clip = myClips[7];
                break;
            }
            case myMoveList.Dash:
            {
                voiceID = Random.Range(5, 7);
                voice.clip = myClips[voiceID];
                break;
            }
            case myMoveList.GloriousSanctuary:
            {
                voice.clip = myClips[8];
                break;
            }
            case myMoveList.HolyCrown:
            {
                voice.clip = myClips[9];
                break;
            }
            case myMoveList.TwilightMoon:
            {
                voiceID = 10;
                voice.clip = myClips[10];
                break;
            }
            case myMoveList.TwilightCrown:
            {
                voiceID = 11;
                voice.clip = myClips[11];
                break;
            }
            case myMoveList.CelestialPrayer:
            {
                voice.clip = myClips[12];
                break;
            }
            case myMoveList.FaithEnhancement:
            {
                if (Random.Range(0, 2) == 1)
                {
                    voiceID = 13;
                    voice.clip = myClips[13];
                }
                else
                {
                    voiceID = 14;
                    voice.clip = myClips[14];
                }
                break;
            }
            case myMoveList.MountainPrison:
            {
                voiceID = 15;
                voice.clip = myClips[15];
                break;
            }
            case myMoveList.LastPhase:
            {
                voiceID = 16;
                voice.clip = myClips[16];
                break;
            }

            default:
                voiceID = -1;
                break;

        }

        if (voiceID >= 0)
        {
            clip = myClips[voiceID];
        }
        else
        {
            clip = null;
        }

        //voice.clip = clip;
        voice.Play();
        if (broadcast)
        {
            _dialogDisplayer.EnqueueDialog(1003, voiceID, voice, myClips[voiceID]);
        }

    }

    public override void BroadCastMyVoice(int moveID)
    {
        if(myClips.Length<=0)
            return;
        int clipID = -1;
        switch (moveID)
        {
            case 0:
            {
                clipID = 17;
                break;
            }
            default:
            {
                clipID = 17 + moveID;
                break;
            }
        }
        _dialogDisplayer.EnqueueDialog(1003, clipID, voice, myClips[clipID]);
    }
}
