using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class ActorController_c007 : ActorControllerWand
{
    public bool SealRemoved { get; private set; }
    
    private static float skill2SpFractionNormal = 13333f;
    private static float skill2SpFractionBoost = 6170f;
    
    private TimerBuff demonSealDebuff;
    private TimerBuff sealReleasedBuff;
    private float currentSkill2SPFraction;
    
    
    
    protected override void Awake()
    {
        base.Awake();
        voiceController = GetComponentInChildren<VoiceControllerPlayer>();
        demonSealDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.DemonSeal,
            1, 120, 1);
        demonSealDebuff.dispellable = false;
        sealReleasedBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DemonSealReleased,
            1, -1, 1);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() =>
            GlobalController.currentGameState == GlobalController.GameState.Inbattle);

        _statusManager.ObtainTimerBuff(new TimerBuff(demonSealDebuff));
        _statusManager.OnBuffDispelledEventDelegate += CheckDemonSealRelief;
        _statusManager.OnBuffExpiredEventDelegate += CheckDemonSealRelief;
        _statusManager.SetSPChargeRate(1,150);
        _statusManager.currentSP[1] = 13333 * 0.3f;
        _statusManager.OnShapeshiftingExit += BlockAutoChargeOfSkill2;
        _statusManager.OnShapeshiftingEnter += ContinueAutoChargeOfSkill2;

    }


    public override void UseSkill(int id)
    {
        voiceController?.PlaySkillVoice(id);

        if (id == 1 && DModeIsOn)
        {
            id = 5;
        }else if (id == 2 && DModeIsOn)
        {
            id = 6;
        }
        
        if (isAttackSkill[id - 1])
        {
            pi.InvokeAttackSignal();
            AttackFromPlayer.CheckEnergyLevel(_statusManager);
            AttackFromPlayer.CheckInspirationLevel(_statusManager);
        }else if (isRecoverSkill[id - 1])
        {
            AttackFromPlayer.CheckEnergyLevel(_statusManager);
        }
        
        switch (id)
        {
            case 1:
                pi.isSkill = true;
                anim.Play("s1");
                _statusManager.currentSP[0] = 0;
                break;

            case 2:
                pi.isSkill = true;
                anim.Play("s2");
                _statusManager.currentSP[1] = 0;
                break;

            case 3:
                pi.isSkill = true;
                anim.Play("s3");
                _statusManager.currentSP[2] = 0;
                break;

            case 4:
                pi.isSkill = true;
                anim.Play("s4");
                _statusManager.currentSP[3] = 0;
                break;
            
            case 5:
                pi.isSkill = true;
                anim.Play("s1_boost");
                _statusManager.currentSP[0] = 0;
                break;

            case 6:
                pi.isSkill = true;
                anim.Play("s2_boost");
                _statusManager.currentSP[1] = 0;
                break;

            default:
                break;
        }
    }

    private void BlockAutoChargeOfSkill2()
    {
        currentSkill2SPFraction = _statusManager.currentSP[1] / skill2SpFractionBoost;
        _statusManager.SetRequiredSP(1, skill2SpFractionNormal);
        _statusManager.currentSP[1] /= 0.3f;
        _statusManager.SetSPChargeRate(1,60);

    }
    
    private void ContinueAutoChargeOfSkill2()
    {
        currentSkill2SPFraction = _statusManager.currentSP[1] / skill2SpFractionNormal;
        _statusManager.SetRequiredSP(1, skill2SpFractionBoost);
        // _statusManager.currentSP[1] = Mathf.Clamp(
        //     currentSkill2SPFraction * skill2SpFractionBoost, 0, skill2SpFractionBoost);
        _statusManager.SetSPChargeRate(1,400);
        _statusManager.currentSP[1] *= 0.3f;

    }
    
    
    private void CheckDemonSealRelief(BattleCondition condition)
    {
        if (condition.buffID == (int)BasicCalculation.BattleCondition.DemonSeal)
        {
            _statusManager.OnBuffDispelledEventDelegate -= CheckDemonSealRelief;
            _statusManager.OnBuffExpiredEventDelegate -= CheckDemonSealRelief;
            
            SealRemoved = true;
            _statusManager.ObtainTimerBuff(sealReleasedBuff);
            _statusManager.FlashburnRes = 100;
            _statusManager.BurnRes = 100;
            _statusManager.ScorchrendRes = 100;
            _statusManager.StormlashRes = 100;
            _statusManager.BogRes = 100;
            _statusManager.StunRes = 100;
            _statusManager.SleepRes = 100;
            _statusManager.BlindnessRes = 100;
            _statusManager.ShadowblightRes = 100;
            _statusManager.ParalysisRes = 100;
            _statusManager.PoisonRes = 100;
            _statusManager.FrostbiteRes = 200;
        }
    }

    public void OnShapeShiftingUpdate()
    {
        _statusManager.knockbackRes = 999;
        dodging = true;
    }
    protected override void CheckShapeShifting()
    {
        base.CheckShapeShifting();

        if(_statusManager.isShapeshifting)
            return;
        
        if(SealRemoved == false)
            return;
        
        _statusManager.ChargeDP(10*Time.deltaTime,true);


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
                voiceController.PlaySkillVoice(5);
                pi.moveEnabled = false;
                
                dodging = true;
            }
        }
    }

    protected override void OnShapeShiftEnter()
    {
        //dc = transform.Find("DModel").GetChild(0).GetComponent<DragonController>();
        if(dc == null)
            return;

        pi.moveEnabled = false;
        pi.isSkill = true;
        _statusManager.knockbackRes = 999;
        _statusManager.InvokeShapeshiftingEnter();
        //anim.Play("idle");
        ResetGravityScale();
        
    }

    protected void OnShapeShiftFinish()
    {
        pi.rollEnabled = true;
        pi.inputRollEnabled = true;
        pi.moveEnabled = true;
        pi.isSkill = false;
        dodging = false;
        _statusManager.ResetKBRes();
    }

    public override bool BlockDPCharge(bool abilityCharge)
    {
        if (abilityCharge)
            return false;
        return true;
    }
}
