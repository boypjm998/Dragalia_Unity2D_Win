using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

public class BattleEffectManager : MonoBehaviour
{
    [Header("AfflictionEffect--异常状态特效")]
    [SerializeField] private Color flashburnColor;
    [SerializeField] private GameObject flashburnFXPrefab;
    [SerializeField] private Color burnColor;
    [SerializeField] private GameObject burnFXPrefab;
    [SerializeField] private Color schorchrendColor;
    [SerializeField] private GameObject scorchrendFXPrefab;

    [Header("Basic Effect")]
    [SerializeField] private GameObject healFXPrefab;
    [SerializeField] private GameObject dispellFXPrefab;
    [SerializeField] private GameObject buffFXPrefab;
    [SerializeField] private GameObject debuffFXPrefab;

    [Header("Enemy BattleHint Effect")]
    [SerializeField] private GameObject targetLockPrefab;
    [SerializeField] private GameObject exclamationPrefab;
    [SerializeField] private GameObject counterIconPrefab;
    
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip hitEffectClip;
    [SerializeField] private AudioClip reviveSEClip;
    
    
    
    
    
    private Camera camera;
    private AudioSource soundEffectSource;
    //private GameObject counterIcon;

    
    

    private void Start()
    {
        soundEffectSource = transform.GetChild(0).gameObject.GetComponent<AudioSource>();
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }


    public void SpawnHealEffect(GameObject target)
    {
        Instantiate(healFXPrefab, target.transform.position, target.transform.rotation,
            target.GetComponent<AttackManager>().BuffFXLayer.transform);
    }

    public void SpawnEffect(GameObject target, BasicCalculation.BattleCondition cond)
    {
        SpriteRenderer spriteRenderer = target.GetComponentInChildren<SpriteRenderer>();
        Light2D light = spriteRenderer?.GetComponent<Light2D>();
        
        
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
                case BasicCalculation.BattleCondition.Scorchrend:
                    if (light.intensity == 0)
                    {
                        StartCoroutine(LightEffectRoutine(light,schorchrendColor,1,0.6f));
                    }
                    SpawnAnimation(target, scorchrendFXPrefab);
                    break;
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
        var fx = Instantiate(prefab, target.transform.position, Quaternion.identity, layer.transform);
        //RESIZE
        if (fx.transform.GetComponentInChildren<Renderer>().localBounds.size.x > 10)
        {
            //Not Implement
        }
        
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
            Instantiate(targetLockPrefab, target.transform.position, Quaternion.identity, layer.transform);
        fx.GetComponent<ObjectInvokeDestroy>().destroyTime = lastTime;

    }

    public void SpawnExclamation(GameObject target, Vector3 position)
    {
        var layer = target.transform.Find("BuffLayer");
        var fx =
            Instantiate(exclamationPrefab, position, Quaternion.identity, layer.transform);
        
    }

    public void PlaySoundEffect(AudioClip clip, Vector2 position)
    {
        camera = Camera.current;
        AudioSource.PlayClipAtPoint(clip,new Vector3(position.x,position.y,camera.transform.position.z));
        
    }
    public void PlayHitSoundEffect(Vector2 position)
    {
        soundEffectSource.clip = hitEffectClip;
        if(!soundEffectSource.isPlaying)
            soundEffectSource.Play();
        
    }
    
    public void PlayReviveSoundEffect()
    {
        //camera = Camera.main;
        soundEffectSource.PlayOneShot(reviveSEClip);
        //AudioSource.PlayClipAtPoint(reviveSEClip, camera.transform.position);

    }

    public void PlayHealSoundEffect()
    {
        
    }

    public void DisplayCounterIcon(GameObject target, bool flag)
    {
        Transform bufflayer = target.transform.Find("BuffLayer");
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


}
