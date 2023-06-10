using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class AudioManagerPlayer : MonoBehaviour, IVoice
{
    public int chara_id;
    
    protected bool isSkillVoice;
    
    protected AudioSource voice;
    protected Coroutine voiceCDRoutine;
    public bool voiceLoaded { get; protected set; } = false;

    [Header("Attack Part")]
    public AudioClip[] Combo1;
    public AudioClip[] Combo3;
    public AudioClip[] Combo5;

    public AudioClip[] Dash;

    public AudioClip[] S1;
    public AudioClip[] S2;
    public AudioClip[] S3;
    public AudioClip[] S4;

    [Header("Other Movement")] 
    public AudioClip[] Hurt;

    public AudioClip[] Dodge;

    protected GlobalController _globalController;
    
    private void Start()
    {
        voice = GetComponent<AudioSource>();
    }

    public void DebugLoadVoice()
    {
        LoadMyVoice();
    }

    protected virtual void LoadMyVoice()
    {

        _globalController = FindObjectOfType<GlobalController>();

        AssetBundle assetBundle = _globalController.GetBundle(GetVoicePath());
        //AssetBundle assetBundle = AssetBundle.LoadFromFile
            //(Path.Combine(Application.streamingAssetsPath, GetVoicePath()));
        var voicePack = assetBundle.LoadAllAssets<AudioClip>();
        DistributeMyVoice(voicePack);
        voiceLoaded = true;
    }

    protected string GetVoicePath()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("voice/voice_c");
        if (chara_id < 10)
        {
            sb.Append("00");
            sb.Append(chara_id.ToString());
        }
        else if (chara_id < 100)
        {
            sb.Append("0");
            sb.Append(chara_id.ToString());
        }
        else
        {
            sb.Append(chara_id.ToString());
        }

        return sb.ToString();
    }

    protected abstract void DistributeMyVoice(AudioClip[] clips);

    public virtual void PlaySkillVoice(int id)
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

                default:
                    clip = null;
                    break;
            }

            voice.clip = clip;
            voice.Play();
            //voice.PlayOneShot(clip);
        }
    }

    public virtual void PlayHurtVoice(StatusManager statusManager)
    {
        if(Hurt.Length==0)
            return;
        voice.Stop();
        var rand = Random.Range(0, 10);
        if (statusManager.currentHp > statusManager.maxHP * 0.8)
        {
            if (rand > 5)
            {
                voice.clip = Hurt[0];
            }
            else if(rand > 2)
            {
                voice.clip = Hurt[1];
            }
            else
            {
                voice.clip = Hurt[2];
            }
        }else if (statusManager.currentHp > statusManager.maxHP * 0.5)
        {
            if (rand > 5)
            {
                voice.clip = Hurt[2];
            }
            else if(rand>2)
            {
                voice.clip = Hurt[1];
            }
            else
            {
                voice.clip = Hurt[0];
            }
        }else if (statusManager.currentHp < statusManager.maxHP * 0.3)
        {
            if(rand > 7)
            {
                voice.clip = Hurt[2];
            }
            else
            {
                voice.clip = Hurt[3];
            }
        }
        else
        {
            if(rand < 7)
                voice.clip = Hurt[2];
            else
            {
                voice.clip = Hurt[3];
            }
        }
        voice.Play();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id = 0">Dash attack</param>
    /// <param name="id = 1">Combo1</param>
    /// <param name="id = 2">Combo3</param>
    /// <param name="id = 3">Combo5</param>
    /// <exception cref="NotImplementedException"></exception>
    public virtual void PlayAttackVoice(int id)
    {
    }

    public virtual void PlayDodgeVoice()
    {
        if (voice.isPlaying == true || voiceCDRoutine!=null)
        {
            return;
        }
        
        voiceCDRoutine = StartCoroutine("WaitForVoiceCooldown");

        voice.clip = Dodge[0];
        voice.Play();


    }
    
    protected IEnumerator WaitForVoiceCooldown()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        voiceCDRoutine = null;
    }
    

}
