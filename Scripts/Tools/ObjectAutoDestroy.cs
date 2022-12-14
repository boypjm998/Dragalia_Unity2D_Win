using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAutoDestroy : MonoBehaviour
{
    Animator anim;
    private ParticleSystem _particleSystem;
    private float totalFrame;
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        totalFrame = (anim.GetCurrentAnimatorClipInfo(0)[0].clip.length*anim.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate);
    }

    // Update is called once per frame
    void Update()
    {
        float totalFrame = (anim.GetCurrentAnimatorClipInfo(0)[0].clip.length*anim.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate);
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1-(1/totalFrame))
        {
            Destroy(gameObject);
        }    
    }

    private void OnParticleSystemStopped()
    {
        print(_particleSystem.time);
    }
}
