using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController_C007 : DragonController
{
    protected override void Awake()
    {
        pi = transform.GetComponentInParent<PlayerInput>();
        _statusManager = transform.GetComponentInParent<PlayerStatusManager>();
        ac = GetComponentInParent<ActorController_c007>();
        rigid = ac.rigid;
        //dAnim = GetComponentInChildren<Animator>();
        _statusManager.OnReviveOrDeath += DModeForcePurge;
        
    }

    private void Start()
    {
        //_statusManager.ChargeDP(70);
    }

    private void Update()
    {
        if(!ac.DModeIsOn)
            return;
        
        //ac.checkFaceDir();
        CheckShapeShifting();
        DModeGaugeTick(Time.deltaTime);
    }

    protected override void CheckShapeShifting()
    {
        if (pi.buttonUp.OnPressed)
        {
            if (pi.hurt == false && pi.isSkill == false && pi.attackEnabled &&
                ac.anim.GetCurrentAnimatorStateInfo(0).IsName("transform")==false)
            {
                print("解除龙化");
                DModePurged();
                //pi.moveEnabled = false;
                //pi.isSkill = true;
            }
        }
    }

    protected override void EnterShapeShifting()
    {
        ac.dodging = true;
        //ac.anim.enabled = false;
        pi.moveEnabled = false;
        pi.inputRollEnabled = true;
        pi.rollEnabled = false;

        _statusManager.knockbackRes = 999;
        _statusManager.ReliefAllAfflication();
        //_statusManager.ReliefDebuffExceptNilAndCorrosion();
        
        
        ac.SetGravityScale(gravityScale);
    }

    protected override void SetHitSensorScaleToHumanScale()
    {
        //pass
    }

    protected override void SetHitSensorScaleToDragonScale()
    {
        //pass
    }
}
