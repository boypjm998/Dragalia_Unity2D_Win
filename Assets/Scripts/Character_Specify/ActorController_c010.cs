using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ActorController_c010 : ActorControllerDagger
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        voiceController = GetComponentInChildren<VoiceController_C010>();
        _statusManager.OnComboConnect += GainDPEvery20ComboHit;
    }

    protected override void CheckSkill()
    {
        if(DModeIsOn)
            return;
        
        if (pi.skill[0] && !pi.hurt && !pi.isSkill)
        {
            UseSkill(1);
        }

        if (pi.skill[1] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill)
        {
            UseSkill(2);
        }

        if (pi.skill[3] && !pi.hurt && !pi.isSkill)
        {
            UseSkill(4);
        }
    }

    private void Start()
    {
        //_statusManager.ChargeDP(50);
    }

    protected override void Update()
    {
        base.Update();
        //CheckAirRoll();
    }

    private void OnDestroy()
    {
        _statusManager.OnComboConnect -= GainDPEvery20ComboHit;
    }

    

    protected override void CheckShapeShifting()
    {
        base.CheckShapeShifting();
        if (pi.buttonUp.OnPressed)
        {
            
            if (!dc || !dc.CheckTransformCondition())
                return;
            
            if(_statusManager.shapeshiftingCDTimer > 0)
                return;

            if (pi.hurt == false && pi.isSkill == false)
            {
                pi.isSkill = true;
                pi.rollEnabled = false;
                pi.inputRollEnabled = false;
                
                SetGravityScale(0);
                SetVelocity(0,0);
                _statusManager.knockbackRes = 999;
                DModeIsOn = true;
                anim.Play("transform");
                (voiceController as VoiceController_C010).PlaySkillVoice(3);
                pi.moveEnabled = false;
                
                dodging = true;
            }
        }
    }

    public override void ResetCombo()
    {
        if (Combo >= 3)
            Combo = 0;
    }

    public void OnShapeShiftingUpdate()
    {
        _statusManager.knockbackRes = 999;
        dodging = true;
    }

    public override void OnSkillEnter()
    {
        ta.FaceDirectionAutofixWithMarking();
        speedModifier = 1;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("s1") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("s1_2"))
        {
            SetGravityScale(0);
            SetVelocity(0,0);
        }

        
        pi.isSkill = true;
        pi.rollEnabled = false;
        pi.inputRollEnabled = false;
        pi.directionLock = false;
        dodging = true;
        
        
        _tweener?.Kill();
        
        
        ActionDisable((int)PlayerActionType.MOVE);
        ActionDisable((int)PlayerActionType.JUMP);
        ActionDisable((int)PlayerActionType.ROLL);
        ActionDisable((int)PlayerActionType.ATTACK);
        //pi.SetInputDisabled("attack");
        pi.SetInputDisabled("move");
        Combo = 0;
    }

    public override void OnSkillExit()
    {
        base.OnSkillExit();
        ResetGravityScale();
    }

    public override void OnHurtEnter()
    {
        base.OnHurtEnter();
        AppearRenderer();
    }

    public override void OnDashEnter()
    {
        base.OnDashEnter();
        voiceController?.PlayAttackVoice(0);
    }

    public override void OnStandardAttackEnter()
    {
        base.OnStandardAttackEnter();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("combo3"))
        {
            Combo = 0;
        }
    }

    protected override void OnShapeShiftEnter()
    {
        base.OnShapeShiftEnter();
        transform.Find("Model").gameObject.SetActive(false);
        dc.transform.parent.gameObject.SetActive(true);
        //_statusManager.InvokeShapeshiftingEnter();
        //dc.dAnim.Rebind();
        
    }

    protected void GainDPEvery20ComboHit()
    {
        if(_statusManager.comboHitCount % 20 == 0)
            _statusManager.ChargeDP(2);
    }
}
