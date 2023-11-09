using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationSpeedModifier : MonoBehaviour, ISpeedControllable
{
    private Animator animator;
    public ActorController actorController;
    
    public void SetRate(int ComponentID, float rate)
    {
        if (rate > 1.5f)
            rate = 1.5f;
        animator.speed = rate;
    }

    public void GetTargetComponents()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        GetTargetComponents();
    }

    private void Update()
    {
        if(actorController)
            SetRate(0,actorController.attackRate * 0.01f + 1);
    }
}
