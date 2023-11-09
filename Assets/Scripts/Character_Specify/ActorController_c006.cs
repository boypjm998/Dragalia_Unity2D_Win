using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ActorController_c006 : ActorControllerMeeleWithFS
{
    public bool dodgeAttackUsed = false;
    private TimerBuff _dmgCutBuff;
    private float initialKBres;
    public int skill1Increment { get; private set; } = 0;
    private AttackManager_C006 _attackManagerC006;

    protected override void Awake()
    {
        base.Awake();
        initialKBres = _statusManager.knockbackRes;
        _dmgCutBuff = new TimerBuff((int)(BasicCalculation.BattleCondition.DamageCut),
            30, -1, 1, 100601);
        _dmgCutBuff.dispellable = false;
        _statusManager.SetReqDModeGauge(40);
        _attackManagerC006 = GetComponent<AttackManager_C006>();
    }

    private void Start()
    {
        _statusManager.ChargeDP(10);
    }

    protected override void CheckShapeShifting()
    {
        base.CheckShapeShifting();
        
        if (pi.buttonUp.OnPressed)
        {

            if(_statusManager.shapeshiftingCDTimer > 0)
                return;
            
            if(_statusManager.DModeGauge < _statusManager.ReqDModeGauge)
                return;

            if (pi.hurt == false && pi.isSkill == false)
            {
                _statusManager.DepleteDP(40);
                _statusManager.shapeshiftingCDTimer = _statusManager.shapeshiftingCD;
                pi.isSkill = true;
                pi.rollEnabled = false;
                pi.inputRollEnabled = false;
                
                //SetGravityScale(0);
                SetVelocity(0,0);
                _statusManager.knockbackRes = 999;
                //DModeIsOn = true;
                anim.Play("transform");
                voiceController.PlaySkillVoice(5);
                pi.moveEnabled = false;
                
                dodging = true;
            }
        }
        
    }

    public override void UseSkill(int id)
    {
        voiceController?.PlaySkillVoice(id);
        
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

            case 4:
                pi.isSkill = true;
                anim.Play("s4");
                _statusManager.currentSP[3] = 0;
                break;

            default:
                break;
        }
    }


    protected override void ResetCombo()
    {
        if(Combo == 7 || Combo == 8)
            voiceController?.PlayAttackVoice(Random.Range(4,6));
        
        
        if (Combo >= 9)
        {
            Combo = 0;
            //dodgeAttackUsed = false;
        }
        
        

        
    }

    protected override void Update()
    {
        base.Update();
        
        
    }
    
    

    private bool IsComboState(AnimatorStateInfo stateInfo)
    {
        if (stateInfo.IsName("combo1"))
            return true;
        if (stateInfo.IsName("combo2"))
            return true;
        if (stateInfo.IsName("combo3"))
            return true;
        if (stateInfo.IsName("combo4"))
            return true;
        if (stateInfo.IsName("combo5"))
            return true;
        if (stateInfo.IsName("combo6"))
            return true;
        if (stateInfo.IsName("combo7"))
            return true;
        if (stateInfo.IsName("combo8"))
            return true;
        if (stateInfo.IsName("combo9"))
            return true;
        

        return false;
    }

    public void EventRollMove()
    {
        dodgeAttackUsed = true;
        try
        {
            _tweener?.Kill();
        }
        catch
        {
        }
        if (pi.buttonLeft.IsPressing && !pi.buttonRight.IsPressing)
        {
            SetFaceDir(-1);
        }
        else if (!pi.buttonLeft.IsPressing && pi.buttonRight.IsPressing)
        {
            SetFaceDir(1);
        }

        StartCoroutine(HorizontalMove(rollspeed * 1.3f , 0.4f/1.2f, "roll_attack"));
        voiceController?.PlayAttackVoice(6,true);
    }

    public override void Roll()
    {
        if (pi.inputRollEnabled == false)
        {
            pi.roll = false;
            return;
        }

        if (forceLevel >= 0)
        {
            pi.roll = false;
            return;
        }

        


        if (pi.rollEnabled && !pi.hurt)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            
            if ((IsComboState(stateInfo) || Combo > 0) && dodgeAttackUsed == false)
            {
                
                pi.inputRollEnabled = false;
                pi.rollEnabled = false;
                pi.roll = false;
                print("IS COMBO STATE");
                //lastPositionAfterCombo6 = Vector2.zero;
                anim.Play("roll_attack");
            }else if(stateInfo.IsName("roll_attack"))
            {
                // if(dodgeAttackUsed)
                //     Combo = 0;
                print("IS DODGE COMBO STATE");
                anim.SetBool("roll",true);
                return;
            }
            
        }
        
        
        if(grounded)
            anim.SetBool("roll",true);
        
    }

    protected override void OnRollEnterBase()
    {
        base.OnRollEnterBase();
        //voiceController?.PlayAttackVoice(7);
        OnAttackInterrupt?.Invoke();
        pi.directionLock = false;
        dodgeAttackUsed = true;
    }

    public override void onRollEnter()
    {
        base.onRollEnter();
        Combo = 0;
        
    }

    public override void OnSkillEnter()
    {
        base.OnSkillEnter();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("s1"))
        {
            _statusManager.knockbackRes = 999;
            _statusManager.ObtainTimerBuff(_dmgCutBuff);
            skill1Increment = skill1Increment>=2?2:skill1Increment+1;
            _statusManager.requiredSP[0] = 2200 + 624 * (skill1Increment * skill1Increment);
        }
    }

    public override void OnSkillExit()
    {
        base.OnSkillExit();
        
        _statusManager.knockbackRes = (int)initialKBres;
        _statusManager.RemoveConditionWithoutLog(_dmgCutBuff);
        
        
    }

    public override void OnStandardAttackEnter()
    {
        base.OnStandardAttackEnter();
        dodgeAttackUsed = false;
    }






    public override bool BlockDPCharge(bool ability)
    {
        if (_attackManagerC006.summoned && !ability)
            return true;
        return false;
    }
    
    
    
    
    
}
