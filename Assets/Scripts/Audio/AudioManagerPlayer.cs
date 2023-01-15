using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public abstract class AudioManagerPlayer : MonoBehaviour
{
    public int chara_id;
    
    protected AudioSource voice;
    protected Coroutine voiceCDRoutine;
    
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

    private GlobalController _globalController;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void LoadMyVoice()
    {
        _globalController = FindObjectOfType<GlobalController>();
        AssetBundle assetBundle = _globalController.GetBundle(GetVoicePath());
        //AssetBundle assetBundle = AssetBundle.LoadFromFile
            //(Path.Combine(Application.streamingAssetsPath, GetVoicePath()));
        var voicePack = assetBundle.LoadAllAssets<AudioClip>();
        DistributeMyVoice(voicePack);
    }

    protected string GetVoicePath()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("voice_c");
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
    public abstract void PlaySkillVoice(int id);
    public virtual void PlayHurtVoice(StatusManager statusManager)
    {
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
    public abstract void PlayAttackVoice(int id);

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

}
