using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = System.Random;

public class ActorController_c019 : ActorController
{
     [SerializeField] private GameObject recoveryFX;

     private int healTimes = 0;
     private bool emergencyRecoverEnable = true;
     private Tween _abilityTween;
     public bool hasPowerOfBonds { get; private set; }
     
     protected override void Awake()
     {
          base.Awake();
          voiceController = GetComponentInChildren<VoiceControllerPlayer>();
          canPerformInAir[2] = true;
          canPerformInAir[3] = false;
     }

     private void Start()
     {
          _statusManager.OnBuffEventDelegate += CheckBoosted;
          _statusManager.OnBuffDispelledEventDelegate += CheckBoosted;
          _statusManager.OnBuffExpiredEventDelegate += CheckBoosted;
          //_statusManager.OnHPChange += EmergencyRecovery;
     }

     protected override void Update()
     {
          base.Update();
          CheckSkill();
          EmergencyRecovery();
     }

     private void OnDestroy()
     {
          _statusManager.OnBuffEventDelegate -= CheckBoosted;
          _statusManager.OnBuffDispelledEventDelegate -= CheckBoosted;
          _statusManager.OnBuffExpiredEventDelegate -= CheckBoosted;
          //_statusManager.OnHPChange -= EmergencyRecovery;
     } 
     
     public int Combo
     {
        get { return anim.GetInteger("combo"); }
        set { anim.SetInteger("combo",value); }
     }

    protected Coroutine comboStageResetRoutine = null;

    public override void UseSkill(int id)
    {
         if (!hasPowerOfBonds || id != 1)
         {
              if(id != 4)
                   voiceController?.PlaySkillVoice(id);
              else
              {
                   voiceController?.PlaySkillVoice(voiceController.S4[UnityEngine.Random.Range(0,2)]);
              }
         }
         else
         {
              voiceController?.PlaySkillVoice(voiceController.S1[1]);
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
                   if(hasPowerOfBonds)
                     anim.Play("s1_boost");
                   else anim.Play("s1");
                   _statusManager.currentSP[0] = 0;
                   break;

              case 2:
                   pi.isSkill = true;
                   if(hasPowerOfBonds)
                        anim.Play("s2_boost");
                   else anim.Play("s2");
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

              default:
                   break;
         }
    }

    public void RecoverHealTimes()
    {
         healTimes--;
         if (healTimes < 0)
              healTimes = 0;
    }

    public override bool CheckPowerOfBonds()
    {
         if(_statusManager.GetConditionsOfType((int)BasicCalculation.BattleCondition.PowerOfBonds).Count > 0)
         {
              float potency = Mathf.Clamp(100 - healTimes * 50, 1, 100);
              healTimes++;
              _statusManager.currentHp = 1;
              _statusManager.currentSP[1] = 0;
              _statusManager.HPRegenImmediately(potency,0);
              BattleEffectManager.Instance.SpawnHealEffect(gameObject);
              _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.PowerOfBonds,true);
              StartCoroutine(InvincibleRoutineWithoutRecover(1f));
              return true;
         }

         return false;
    }

    private void CheckBoosted(BattleCondition condition)
    {
        
        if (_statusManager.GetConditionsOfType((int)BasicCalculation.BattleCondition.PowerOfBonds).Count > 0)
        {
            hasPowerOfBonds = true;
            _statusManager.FillSP(1,100);
        }
        else hasPowerOfBonds = false;
    
    }

    private void EmergencyRecovery()
    {
         if(!emergencyRecoverEnable)
              return;
         
         float hpPercent = ((float)_statusManager.currentHp / (float)_statusManager.maxHP);
         //print(hpPercent);

         if (hpPercent <= 0.3f)
         {
              _statusManager.HPRegenImmediately(150,0,false);
              emergencyRecoverEnable = false;
              _abilityTween = DOVirtual.DelayedCall
                   (45, () => emergencyRecoverEnable = true,false);
              Instantiate(recoveryFX, transform.position, Quaternion.identity,
                   BattleStageManager.Instance.RangedAttackFXLayer.transform);
         }


    }
    
    public override void onRollEnter()
    {
        base.onRollEnter();
        pi.directionLock = false;
        //comboStage = 0;
        //anim.SetInteger("combo",0);
        //Combo = 0;
        OnAttackInterrupt?.Invoke();
          
    }

    public override void onRollExit()
    {
        base.onRollExit();
        //comboStage = 0;
        //anim.SetInteger("combo",0);
        //Combo = 0;
    }
    
    public override void OnHurtEnter()
     {
          //OnForceAttackExit();
          base.OnHurtEnter();
          Combo = 0;
          
     }

     public override void OnStandardAttackEnter()
     {
          StartAttack();
          base.OnStandardAttackEnter();
          pi.stdAtk = false;
          pi.moveEnabled = false;
          pi.attackEnabled = false;
          pi.jumpEnabled = false;
          pi.inputAttackEnabled = false;
          
          if(comboStageResetRoutine!=null)
               StopCoroutine(comboStageResetRoutine);
          comboStageResetRoutine = null;
          
          ClearBoolSignal("attack");
          
          
          Combo++;

          PlayComboVoice();
          
          ResetCombo();
     }
     

     protected override void PlayComboVoice()
     {
          if (Combo <= 1)
          {
               voiceController?.PlayAttackVoice(1);
          }else if (Combo <= 3)
          {
               voiceController?.PlayAttackVoice(2);
          }else if (Combo <= 5)
          {
               voiceController?.PlayAttackVoice(3);
          }
     }

     public override void ResetCombo()
     {
          if (Combo >= 5)
               Combo = 0;
     }

     public override void OnStandardAttackExit()
     {
          ExitAttack();
          base.OnStandardAttackExit();
          pi.jumpEnabled = true;
          pi.moveEnabled = true;
          pi.attackEnabled = true;
          pi.jumpEnabled = true;
          pi.inputAttackEnabled = true;
          pi.inputRollEnabled = true;
          pi.rollEnabled = true;
          print("ExitStandardAttack");

          SetWeaponVisibility(true);
     }

     protected void OnIdleEnter()
     {
          if (comboStageResetRoutine != null)
          {
               StopCoroutine(comboStageResetRoutine);
               comboStageResetRoutine = null;
               //print("interrupted");
          }

          comboStageResetRoutine = StartCoroutine(ResetComboStage(0.5f));
     }

     public override void OnSkillEnter()
     {
          base.OnSkillEnter();
          Combo = 0;
          OnAttackInterrupt?.Invoke();

          if (anim.GetCurrentAnimatorStateInfo(0).IsName("s1") ||
              anim.GetCurrentAnimatorStateInfo(0).IsName("s1_boost"))
          {
               _statusManager.knockbackRes = 999;
          }
          
          
     }

     public override void OnSkillExit()
     {
          base.OnSkillExit();
          _statusManager.ResetKBRes();
          ResetGravityScale();
          SetWeaponVisibility(true);
     }

     protected IEnumerator ResetComboStage(float time)
     {
          //print("reset_before");
          yield return new WaitForSeconds(time);
          //print("reset");
          //comboStage = 0;
          //anim.SetInteger("combo",0);
          Combo = 0;
          comboStageResetRoutine = null;
     }

     public override void FaceDirectionAutoFix(int moveID)
     {
          base.FaceDirectionAutoFix(moveID);
          switch (moveID)
          {
               case 1:
               {
                    if (ta.GetNearestTargetInRangeDirection
                        (facedir, 18f, 1.5f,
                             LayerMask.GetMask("Enemies"),1) == null
                        &&
                        ta.GetNearestTargetInRangeDirection
                        (-facedir, 18f, 1.5f,
                             LayerMask.GetMask("Enemies"),1) != null)
                    {
                         SetFaceDir(-facedir);
                    }

                    break;
               }
               case 2:
               {
                    if (ta.GetNearestTargetInRangeDirection
                        (facedir, 15f, 2f,
                             LayerMask.GetMask("Enemies"),1) == null
                        &&
                        ta.GetNearestTargetInRangeDirection
                        (-facedir, 15f, 2f,
                             LayerMask.GetMask("Enemies"),1) != null)
                    {
                         SetFaceDir(-facedir);
                    }

                    break;
               }
               case 3:
               {
                    if (ta.GetNearestTargetInRangeDirection
                        (facedir, 20f, 10f,
                             LayerMask.GetMask("Enemies"),1) == null
                        &&
                        ta.GetNearestTargetInRangeDirection
                        (-facedir, 20f, 10f,
                             LayerMask.GetMask("Enemies"),1) != null)
                    {
                         SetFaceDir(-facedir);
                    }

                    break;
               }
          }
     }
}
