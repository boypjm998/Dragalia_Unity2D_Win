using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

public class BattleEffectManager : MonoBehaviour
{
    [SerializeField] private Color flashburnColor;
    [SerializeField] private GameObject flashburnFXPrefab;
    [SerializeField] private Color burnColor;
    [SerializeField] private GameObject burnFXPrefab;

    [SerializeField] private GameObject healFXPrefab;
    
    [SerializeField] private GameObject dispellFXPrefab;
    [SerializeField] private GameObject buffFXPrefab;
    [SerializeField] private GameObject debuffFXPrefab;

    [SerializeField] private GameObject targetLockPrefab;
    [SerializeField] private GameObject exclamationPrefab;


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


}
