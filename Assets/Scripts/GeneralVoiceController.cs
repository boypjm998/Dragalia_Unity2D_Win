using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralVoiceController : AudioManagerPlayer, IVoice
{

    public List<AudioClip> myClips = new();
    // Start is called before the first frame update
    protected override void DistributeMyVoice(AudioClip[] clips)
    {
        
    }

    private void Start()
    {
        voice = GetComponentInChildren<AudioSource>();
    }

    public override void PlaySkillVoice(int id)
    {
        if(voice!=null)
            voice.Stop();
        if (id == 1)
            voice.clip = S1[0];
        if(id==2)
            voice.clip = S2[0];
        voice.Play();
    }
}
