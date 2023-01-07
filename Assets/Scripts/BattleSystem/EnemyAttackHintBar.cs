using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using DG.Tweening;
public class EnemyAttackHintBar : MonoBehaviour
{
    public float warningTime;

    //protected Tweener _tweener;
    protected GameObject Fill;
    protected GameObject MaxFill;

    protected EnemyController ac;
    public bool interruptable = true;
    
    

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if(interruptable)
            ac.OnAttackInterrupt -= DestroySelf;
    }

    protected virtual void Start()
    {
        if (ac == null)
        {
            interruptable = false;
            Debug.LogWarning("HintBar cannot find enemy source.");
        }

        if(interruptable)
            ac.OnAttackInterrupt += DestroySelf;
    }

    public void SetSource(EnemyController controller)
    {
        ac = controller;
    }
}
