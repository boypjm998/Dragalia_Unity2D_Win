
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoiceControllerEnemy : AudioManagerGeneral
{
    public int speakerID;
    protected StatusManager statusManager;
    
    [Serializable]
    /// <summary> The clipIndexID and clipTextID should be the same length. </summary>
    public class VoiceGroup
    {
        public List<int> clipIndexID = new();
        public List<int> clipTextID = new();
    }
    
    public List<VoiceGroup> voiceGroups = new();
    
    public int groupOfIntro = -1;
    public int groupOfDialogWhenHPBelow70 = -1;
    public int groupOfDialogWhenHPBelow40 = -1;
    public int groupOfDialogWhenHPBelow0 = -1;
    

    protected override void DistributeMyVoice(AudioClip[] clips)
    {
        
    }
    
    
    void Start()
    {
        

        statusManager = GetComponentInParent<StatusManager>();
        _dialogDisplayer = FindObjectOfType<UI_DialogDisplayer>();
        voice = gameObject.GetComponent<AudioSource>();
        BattleStageManager.Instance.OnGameStart += PlayIntroVoice;
        
        CheckVoiceVolume();
        
        if (speakerID <= 0)
            speakerID = statusManager.dialogIconID;
        
    }

    private void PlayIntroVoice()
    {
        if (groupOfIntro == -1)
        {
            BattleStageManager.Instance.OnGameStart -= PlayIntroVoice;
            return;
        }
        BroadCastMyVoice(groupOfIntro);
        BattleStageManager.Instance.OnGameStart -= PlayIntroVoice;
    }

    private void Update()
    {
        if(statusManager == null)
            return;
        
        
        if (statusManager.currentHp <= statusManager.maxHP * 0.7f && groupOfDialogWhenHPBelow70 != -1)
        {
            BroadCastMyVoice(groupOfDialogWhenHPBelow70);
            groupOfDialogWhenHPBelow70 = -1;
        }

        if (statusManager.currentHp <= statusManager.maxHP * 0.4f && groupOfDialogWhenHPBelow40 != -1)
        {
            BroadCastMyVoice(groupOfDialogWhenHPBelow40);
            groupOfDialogWhenHPBelow40 = -1;
        }
        
        if (statusManager.currentHp <= 0 && groupOfDialogWhenHPBelow0 != -1)
        {
            BroadCastMyVoice(groupOfDialogWhenHPBelow0);
            groupOfDialogWhenHPBelow0 = -1;
        }

        
    }

    public void BroadCastSpecificVoice(int voiceGroupID, int internalID)
    {
        var voiceGroup = voiceGroups[voiceGroupID];
        
        
        var moveID = voiceGroup.clipIndexID[internalID];
        
        

        if (voiceGroup.clipTextID[internalID] >= 0)
        {
            _dialogDisplayer.EnqueueDialog
                (speakerID, voiceGroup.clipTextID[internalID],voice, myClips[moveID]);
        }
    }


    public override void BroadCastMyVoice(int voiceGroupID)
    {
        if (voiceGroupID > voiceGroups.Count)
        {
            Debug.LogWarning("VoiceGroupID is out of range.");
            return;
        }

        

        var voiceGroup = voiceGroups[voiceGroupID];
        
        var randomIndex = Random.Range(0, voiceGroup.clipTextID.Count);
        
        var moveID = voiceGroup.clipIndexID[randomIndex];
        
        // if (myClips[moveID] != null)
        // {
        //     
        //     if (voiceCDRoutine == null)
        //     {
        //         voice.clip = myClips[moveID];
        //         voice.Play();
        //         voiceCDRoutine = StartCoroutine(WaitForVoiceCooldown());
        //     }
        // }

        if (voiceGroup.clipTextID[randomIndex] >= 0)
        {
            _dialogDisplayer.EnqueueDialog
                        (speakerID, voiceGroup.clipTextID[randomIndex],voice, myClips[moveID]);
        }

        
    }

    public void PlayMyVoice(int voiceGroupID)
    {
        if (voiceCDRoutine != null)
            return;

        var voiceGroup = voiceGroups[voiceGroupID];
        
        var randomIndex = Random.Range(0, voiceGroup.clipTextID.Count);
        
        var moveID = voiceGroup.clipIndexID[randomIndex];
        
        if (myClips[moveID] != null)
        {
            voice.clip = myClips[moveID];
            voice.Play();
            voiceCDRoutine = StartCoroutine(WaitForVoiceCooldown());
        }
    }



}
