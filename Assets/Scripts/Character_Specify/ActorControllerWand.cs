using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorControllerWand : ActorController
{
    public int Combo
    {
        get { return anim.GetInteger("combo"); }
        set { anim.SetInteger("combo",value); }
    }
    protected Coroutine comboStageResetRoutine = null;


    protected override void Update()
    {
         base.Update();
         CheckSkill();
    }

    public override void onRollEnter()
    {
        base.onRollEnter();
        pi.directionLock = false;
        OnAttackInterrupt?.Invoke();
          
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

          //SetWeaponVisibility(true);
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

     }

     public override void OnSkillExit()
     {
          base.OnSkillExit();
          ResetGravityScale();
          //SetWeaponVisibility(true);
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
