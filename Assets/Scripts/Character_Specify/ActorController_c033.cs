using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using GameMechanics;
using UnityEngine;

public class ActorController_c033 : ActorControllerMeeleWithFS
{
    public int mode = 0;
    public bool sigilReleased = false;
    
    
    
    
    [SerializeField] private GameObject[] visualEffectsAfterSigilReleased;
    public float modeTime = 0f;
    private TimerBuff modeDefenseBuff;
    private TimerBuff modeRecoverBuff;
    private TimerBuff modePurifyBuff;

    private bool trapHasSet = false;
    public event Action<bool> OnTrapSetDelegate; 


    private float requiredSP;
    public event Action OnSigilReleaseDelegate;
    
    
    private IEnumerator Start()
    {
        requiredSP = _statusManager.requiredSP[2];
        SetWeaponVisibility(false);
        InitModeBuffs();
        yield return new WaitUntil(() => GlobalController.currentGameState == GlobalController.GameState.Inbattle);
        GrantSigilLocked();
    }

    public void SetTrap(bool flag)
    {
        
        if (flag == true)
        {
            _statusManager.requiredSP[2] = 200;
            _statusManager.FillSP(2,100);
            _statusManager.OnSpecialBuffDelegate?.Invoke
                (UI_BuffLogPopManager.SpecialConditionType.SPCharge.ToString());
        }
        else
        {
            //_statusManager.currentSP[2] = 0;
            _statusManager.requiredSP[2] = requiredSP;
        }

        trapHasSet = flag;
        OnTrapSetDelegate?.Invoke(flag);

    }


    protected override void CheckSkill()
    {
        if(pi.allowForceStrike && forceLevel>=0)
            return;


        if (pi.skill[2] && trapHasSet && !pi.hurt)
        {
            //TODO: Trap explode
            Projectile_C033_1.Instance.TriggerAttackThenDestroy(this);
            return;
        }


        if (pi.skill[0] && (anim.GetBool("isGround") || canPerformInAir[0]) && !pi.hurt && !pi.isSkill)
        {
            UseSkill(1);
        }

        if (pi.skill[1] && (anim.GetBool("isGround") || canPerformInAir[1]) && !pi.hurt && !pi.isSkill)
        {
            UseSkill(2);
        }

        if (pi.skill[2] && (anim.GetBool("isGround") || canPerformInAir[2]) && !pi.hurt && !pi.isSkill)
        {
            UseSkill(3);
        }

        if (pi.skill[3] && (anim.GetBool("isGround") || canPerformInAir[3]) && !pi.hurt && !pi.isSkill)
        {
            UseSkill(4);
        }
    }


    private void GrantSigilLocked()
    {
        TimerBuff lockedSigil = new TimerBuff((int)BasicCalculation.BattleCondition.LockedSigil,
            -1, 120, 1, 103301);
        lockedSigil.dispellable = false;
        _statusManager.ObtainTimerBuff(lockedSigil);
        _statusManager.OnBuffExpiredEventDelegate += SigilReleaseCheck;
        _statusManager.ObtainTimerBuff(modeDefenseBuff,false);

    }

    private void InitModeBuffs()
    {
        modeDefenseBuff = new TimerBuff((int)BasicCalculation.BattleCondition.ProtocolPreserve,
            -1, -1, 1, 103302);
        modePurifyBuff = new TimerBuff((int)BasicCalculation.BattleCondition.ProtocolPurify,
            -1, -1, 1, 103302);
        modeRecoverBuff = new TimerBuff((int)BasicCalculation.BattleCondition.ProtocolRestore,
            -1, -1, 1, 103302);
        
        modeDefenseBuff.dispellable = false;
        modePurifyBuff.dispellable = false;
        modeRecoverBuff.dispellable = false;
    }

    protected void SigilReleaseCheck(BattleCondition condition)
    {
        if (condition.buffID == (int)BasicCalculation.BattleCondition.LockedSigil)
        {
            //TODO: Sigil release
            _statusManager.ObtainTimerBuff(modeDefenseBuff, false);
            _statusManager.ObtainTimerBuff(modePurifyBuff, false);
            _statusManager.ObtainTimerBuff(modeRecoverBuff, false);
            
            TimerBuff sigilRelease = new TimerBuff((int)BasicCalculation.BattleCondition.SigilReleased,
                -1, -1, 1, 103301);
            sigilRelease.dispellable = false;
            _statusManager.ObtainTimerBuff(sigilRelease);
            _statusManager.OnBuffExpiredEventDelegate -= SigilReleaseCheck;
            sigilReleased = true;
            
            
            mode = 0;
            
            OnSigilReleaseDelegate?.Invoke();
            
            
            foreach (var eff in visualEffectsAfterSigilReleased)
            {
                eff.gameObject.SetActive(true);
            }
            
            

        }
    }
    
    

    

    public override void OnForceAttackExit()
    {
        base.OnForceAttackExit();
        modeTime = 0f;
        SetWeaponVisibility(false);
    }

    public override void OnForceAttackEnter()
    {
        base.OnForceAttackEnter();
        modeTime = 0f;
        SetWeaponVisibility(true);
    }

    public override void OnHurtExit()
    {
        base.OnHurtExit();
        SetWeaponVisibility(false);
    }

    public override void onRollEnter()
    {
        base.onRollEnter();
        SetWeaponVisibility(false);
    }

    public void SwitchMode(int mode)
    {
        _statusManager.RemoveAllConditionWithSpecialID(103302);
        switch (mode)
        {
            case 0:
            {
                _statusManager.ObtainTimerBuff(modeDefenseBuff);
                break;
            }
            case 2:
            {
                _statusManager.ObtainTimerBuff(modePurifyBuff);
                break;
            }
            case 1:
            {
                _statusManager.ObtainTimerBuff(modeRecoverBuff);
                break;
            }
        }
    }


}
