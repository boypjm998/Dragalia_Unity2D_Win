using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StoryFXEmitter_01 : MonoBehaviour
{
    [SerializeField] private GameObject trailPrefab;
    public static StoryFXEmitter_01 Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void EmitPrefab(GameObject target)
    {
        var trail1 = Instantiate(trailPrefab, transform);
        var trail2 = Instantiate(trailPrefab,target.transform.position,Quaternion.identity, transform);
        var _tweener = trail1.transform.DOMove(target.transform.position, 0.15f);
        _tweener.SetEase(Ease.OutExpo);
        var _tweener2 = trail2.transform.DOMove(transform.position, 0.15f);
        _tweener2.SetEase(Ease.OutExpo);
        Destroy(trail1,0.2f);
        Destroy(trail2,0.2f);
    }
}
