using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
public class AdventurerSpecial_PortalTrail : MonoBehaviour
{

    private Vector3 target;
    private Tweener _tweener;

    public void InitAnim(Transform t)
    {
        
        target = t.position;
    }

    private void Start()
    {
        _tweener = transform.DOMove(target, 0.3f);
        _tweener.SetEase(Ease.OutExpo);
        _tweener.OnComplete(OnTweenComplete);
    }
    
    void OnTweenComplete()
    {
        Destroy(gameObject,0.2f);
    }

    


}
