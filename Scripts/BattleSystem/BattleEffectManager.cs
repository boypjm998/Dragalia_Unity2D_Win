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
    
    [SerializeField] private Color buffColor;
    [SerializeField] private GameObject buffFXPrefab;
    [SerializeField] private Color debuffColor;
    [SerializeField] private GameObject debuffFXPrefab;

    [SerializeField] private GameObject targetLockPrefab;


    public void SpawnEffect(GameObject target, BasicCalculation.BattleCondition cond)
    {
        SpriteRenderer spriteRenderer = target.GetComponentInChildren<SpriteRenderer>();
        Light2D light = spriteRenderer?.GetComponent<Light2D>();
        if (light == null)
        {
            return;
        }

        if ((int)cond <= 200)
        {
            //Buff Effect;
        }
        else if ((int)cond > 400)
        {
            switch (cond)
            {
                case BasicCalculation.BattleCondition.Flashburn:
                    if (light.intensity == 0)
                    {
                        StartCoroutine(LightEffectRoutine(light,1,0.6f));
                    }
                    SpawnAnimation(target, flashburnFXPrefab);
                    break;
            }
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

    private IEnumerator LightEffectRoutine(Light2D light, float intensity, float duration)
    {
        var time = 0f;
        light.color = flashburnColor;
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


}
