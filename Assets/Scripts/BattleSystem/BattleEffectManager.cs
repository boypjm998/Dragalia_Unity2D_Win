using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;
using GameMechanics;
using UnityEngine.SceneManagement;

public class BattleEffectManager : MonoBehaviour
{
    public static BattleEffectManager Instance { get; private set; }
    
    [Header("AfflictionEffect--异常状态特效")]
    [SerializeField] private Color flashburnColor;
    [SerializeField] private GameObject flashburnFXPrefab;
    [SerializeField] private Color burnColor;
    [SerializeField] private GameObject burnFXPrefab;
    [SerializeField] private Color schorchrendColor;
    [SerializeField] private GameObject scorchrendFXPrefab;
    [SerializeField] private Color poisonColor;
    [SerializeField] private GameObject poisonFXPrefab;
    [SerializeField] private Color stormlashColor;
    [SerializeField] private GameObject stormlashFXPrefab;
    [SerializeField] private GameObject paralysisFXPrefab;
    [SerializeField] private GameObject frostbiteFXPrefab;
    
    [SerializeField] private GameObject stunFXPrefab;
    [SerializeField] private GameObject freezeFXPrefab;

    [Header("Basic Effect")]
    [SerializeField] private GameObject healFXPrefab;
    [SerializeField] private GameObject dispellFXPrefab;
    [SerializeField] private GameObject buffFXPrefab;
    [SerializeField] private GameObject debuffFXPrefab;
    [SerializeField] private GameObject reviveFXPrefab;
    [SerializeField] private GameObject breakFXPrefab;

    [Header("ShapeShifting Effect")] 
    public GameObject shapeShiftPurgeFXPrefab;
    [SerializeField] private GameObject shapeShiftFXWind;

    [Header("Enemy BattleHint Effect")]
    [SerializeField] private GameObject targetLockPrefab;
    [SerializeField] private GameObject exclamationPrefab;
    [SerializeField] private GameObject counterIconPrefab;
    
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip hitEffectClip;
    [SerializeField] private AudioClip reviveSEClip;
    [SerializeField] private AudioClip healSEClip;

    [Header("Special Boss Voices")]
    public List<AudioClip> notteHintClips = new();
    
    
    
    public Dictionary<string,AudioClip> SEClips = new();

    private Camera _camera;
    public AudioSource soundEffectSource;
    private int SEPlaying = 0;
    public AudioSource sharedVoiceSource;
    protected AudioSource bgmVoiceSource;
    public bool BGMHasSet { get => bgmVoiceSource.clip != null; }


    


    private void Awake()
    {
        Instance = this;
        soundEffectSource = transform.GetChild(0).gameObject.GetComponent<AudioSource>();
        sharedVoiceSource = transform.GetChild(1).gameObject.GetComponent<AudioSource>();
        bgmVoiceSource = GetComponent<AudioSource>();
    }

    private IEnumerator Start()
    {
        
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        //yield return new WaitUntil(() => GlobalController.currentGameState == GlobalController.GameState.WaitForStart);
        yield return new WaitUntil(() => GlobalController.Instance.loadingEnd);
        yield return new WaitForSeconds(1.25f);
        if(SceneManager.GetActiveScene().name=="BattleScenePrologue" || SceneManager.GetActiveScene().name=="BattleScenePrologue_EN")
            yield break;
        bgmVoiceSource.Play();
        //var soundbundle = GlobalController.Instance.GetBundle("soundeffect/soundeffect_common");
        //SEClips = soundbundle.LoadAllAssets<AudioClip>().ToList();
    }


    public void SpawnHealEffect(GameObject target)
    {
        Instantiate(healFXPrefab, target.transform.position, target.transform.rotation,
            target.transform.Find("BuffLayer"));
        soundEffectSource.clip = healSEClip;
        soundEffectSource.Play();
    }

    public void SpawnEffect(GameObject target, BasicCalculation.BattleCondition cond)
    {
        //SpriteRenderer spriteRenderer = target.GetComponentInChildren<SpriteRenderer>();
        Light2D light = target?.GetComponentInChildren<Light2D>();
        
        
        if (cond == BasicCalculation.BattleCondition.Dispell)
        {
            SpawnAnimation(target,dispellFXPrefab);
            return;
        }
        if (light == null && (int)cond > 400)
        {
            return;
        }

        if ((int)cond <= 200)
        {
            SpawnAnimation(target,buffFXPrefab);
        }
        else if ((int)cond > 400 && (int)cond<500)
        {
            switch (cond)
            {
                case BasicCalculation.BattleCondition.Flashburn:
                    if (light.intensity == 0)
                    {
                        StartCoroutine(LightEffectRoutine(light,flashburnColor,1,0.6f));
                    }
                    SpawnAnimation(target, flashburnFXPrefab);
                    break;
                case BasicCalculation.BattleCondition.Burn:
                    if (light.intensity == 0)
                    {
                        StartCoroutine(LightEffectRoutine(light,burnColor,1,0.6f));
                    }
                    SpawnAnimation(target, burnFXPrefab);
                    break;
                case BasicCalculation.BattleCondition.Poison:
                    if (light.intensity == 0)
                    {
                        StartCoroutine(LightEffectRoutine(light,poisonColor,1,0.6f));
                    }
                    SpawnAnimation(target, poisonFXPrefab);
                    break;
                case BasicCalculation.BattleCondition.Scorchrend:
                    if (light.intensity == 0)
                    {
                        StartCoroutine(LightEffectRoutine(light,schorchrendColor,1,0.6f));
                    }
                    SpawnAnimation(target, scorchrendFXPrefab);
                    break;
                case BasicCalculation.BattleCondition.Stormlash:
                    if (light.intensity == 0)
                    {
                        StartCoroutine(LightEffectRoutine(light,stormlashColor,1,0.6f));
                    }
                    SpawnAnimation(target, stormlashFXPrefab);
                    break;
                case BasicCalculation.BattleCondition.Paralysis:
                    SpawnAnimation(target, paralysisFXPrefab);
                    break;
                case BasicCalculation.BattleCondition.Frostbite:
                    SpawnAnimation(target, frostbiteFXPrefab);
                    break;
                
                
                case BasicCalculation.BattleCondition.Stun:
                {
                    SpawnAnimation(target,stunFXPrefab);
                    break;
                }
                case BasicCalculation.BattleCondition.Freeze:
                {
                    SpawnAnimation(target,freezeFXPrefab);
                    break;
                }
            }
        }
        else
        {
            SpawnAnimation(target,debuffFXPrefab);//Debuff Effect
        }
    }


    private void SpawnAnimation(GameObject target, GameObject prefab)
    {
        var layer = target.transform.Find("BuffLayer");
        
        var fx = Instantiate(prefab, layer.position, Quaternion.identity, layer.transform);
        //RESIZE
        var hitsensor = target.GetComponent<ActorBase>().HitSensor;
        if (hitsensor == null)
        {
            return;
        }

        var width = hitsensor.bounds.size.x;
        var height = hitsensor.bounds.size.y;
        var scaleFactor = Mathf.Min(width, height);
        if (scaleFactor > 2f)
        {
            fx.transform.localScale = new Vector3(1, 1, 1) * (scaleFactor*0.5f);
        }

    }
    
    private void SpawnAnimation(GameObject target, GameObject prefab, float time)
    {
        var layer = target.transform.Find("BuffLayer");
        
        var fx = Instantiate(prefab, layer.position, Quaternion.identity, layer.transform);
        //RESIZE
        var hitsensor = target.GetComponent<ActorBase>().HitSensor;
        if (hitsensor == null)
        {
            return;
        }

        var width = hitsensor.bounds.size.x;
        var height = hitsensor.bounds.size.y;
        var scaleFactor = Mathf.Max(width, height);
        if (scaleFactor > 2f)
        {
            fx.transform.localScale = new Vector3(1, 1, 1) * (scaleFactor*0.5f);
        }

        fx.GetComponent<ObjectInvokeDestroy>().destroyTime = time;

    }

    private IEnumerator LightEffectRoutine(Light2D light, Color color, float intensity, float duration)
    {
        var time = 0f;
        light.color = color;
        while (time < duration/2)
        {
            
            light.intensity += (2*intensity / duration) * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }
        while (time < duration)
        {
            light.intensity -= (2*intensity / duration) * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        light.intensity = 0;
    }

    public void SpawnTargetLockIndicator(GameObject target, float lastTime)
    {
        var layer = target.transform.Find("BuffLayer");
        var fx =
            Instantiate(targetLockPrefab, layer.position, Quaternion.identity, layer.transform);
        fx.GetComponent<ObjectInvokeDestroy>().destroyTime = lastTime;

    }

    public void SpawnTargetLockIndicator(Vector3 position, Transform _parent, float lastTime)
    {
        var fx =
            Instantiate(targetLockPrefab, position, Quaternion.identity, _parent.transform);
        fx.GetComponent<ObjectInvokeDestroy>().destroyTime = lastTime;
    }

    public void SpawnExclamation(GameObject target, Vector3 position)
    {
        var layer = target.transform.Find("BuffLayer");
        var fx =
            Instantiate(exclamationPrefab, position-new Vector3(0,0,10), Quaternion.identity, layer.transform);
        
    }

    public void PlayBreakEffect()
    {
        var targetTransform = GameObject.Find("UIFXContainer").transform;
        Instantiate(breakFXPrefab, targetTransform);
    }
    

    public void PlaySoundEffect(AudioClip clip, Vector2 position)
    {
        _camera = Camera.current;
        AudioSource.PlayClipAtPoint(clip,new Vector3(position.x,position.y,_camera.transform.position.z));
        
    }
    public void PlayHitSoundEffect(AudioClip clip,float volume=1f)
    {
        if(clip == null)
            return;


        if (SEPlaying == 0)
        {
            SEPlaying = 1;
            soundEffectSource.PlayOneShot(clip,volume);
            Invoke("SoundEffectLimitRoutine",0.05f);
        }

        
        
    }

    public void PlayOtherSE(string clipname,float volume=1f)
    {
        //在SEClip列表中找到clip并播放
        var seclip = SEClips[clipname];
        if (seclip == null)
        {
            print("SEClip not found");
            return;
        }

        soundEffectSource.PlayOneShot(seclip,volume);
        
    }

    public void PlayReviveSoundEffect()
    {
        //camera = Camera.main;
        soundEffectSource.PlayOneShot(reviveSEClip);
        //AudioSource.PlayClipAtPoint(reviveSEClip, camera.transform.position);

    }

    public void SpawnTargetLockParticleIndicator(Vector3 position)
    {
        var prefab = Resources.Load<GameObject>("UI/InBattle/General/BattleInfo/LockParticle");
        Instantiate(prefab, position, Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
    }

    public void SpawnReviveEffect(GameObject target)
    {
        Instantiate(reviveFXPrefab, target.transform.position, target.transform.rotation,
            target.GetComponent<AttackManager>().BuffFXLayer.transform);
    }
    

    public void DisplayCounterIcon(GameObject target, bool flag)
    {
        Transform bufflayer = target.transform.Find("BuffLayer");
        if(bufflayer == null)
            return;
        
        float height = target.transform.Find("HitSensor").GetComponent<Collider2D>().bounds.max.y;
        GameObject icon;

        if (flag)
        {
            var iconTransform = bufflayer.Find("CounterIcon");
            if (iconTransform == null)
            {
                icon = Instantiate(counterIconPrefab,
                        target.transform.position + Vector3.up*2,
                        Quaternion.identity, bufflayer.transform);
                icon.name = "CounterIcon";
                if (icon.transform.position.y < height)
                {
                    icon.transform.position = new Vector3(icon.transform.position.x, height+1, icon.transform.position.z);
                }
            }
            else
            {
                icon = iconTransform.gameObject;
            }
            icon.SetActive(true);
        }
        else
        {
            var iconTransform = bufflayer.Find("CounterIcon");
            if (iconTransform != null)
            {
                iconTransform.gameObject.SetActive(false);
            }
        }





    }

    public static void BWEffect()
    {
        var volume = GameObject.Find("Global Volume2").GetComponent<Volume>();

        volume.weight = 1;
        
        var tweenerCore = DOTween.To(() => volume.weight, x => volume.weight = x,
            0,
            3f).SetUpdate(true);
        
    }

    public void SetBGM(AudioClip clip)
    {
        bgmVoiceSource.clip = clip;
    }

    public void PlayBGM(bool flag = true)
    {
        if(flag)
            bgmVoiceSource.Play();
        else
        {
            bgmVoiceSource.Stop();
        }
    }

    protected void SoundEffectLimitRoutine()
    {
        SEPlaying = 0;
    }

    public GameObject GetShapeShiftingFX(int type)
    {
        switch (type)
        {
            case 3:
                return shapeShiftFXWind;
            default:
                return shapeShiftFXWind;
        }
    }




}
