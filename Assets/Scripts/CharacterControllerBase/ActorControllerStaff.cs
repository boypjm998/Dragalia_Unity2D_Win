using System.Collections;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController;
using UnityEngine;


public class ActorControllerStaff : ActorController
{
     public float forcingRequireTime = 1f;
     public float forcingTime = 0;
     
     public int maxForceLevel = 1;
     public int forceLevel {
          get
          {
               return anim.GetInteger("force_level");
          }
          set
          {
               anim.SetInteger("force_level",value);
          }
     }
     public int Combo
     {
          get { return anim.GetInteger("combo"); }
          set { anim.SetInteger("combo",value); }
     }

     protected Coroutine comboStageResetRoutine = null;

     protected override void Awake()
     {
          base.Awake();
          voiceController = GetComponentInChildren<AudioManagerPlayer>();
     }

     protected override void Update()
     {
          base.Update();
          
          CheckRollWhenAttack();
          
          CheckSkill();
     }

     protected void CheckRollWhenAttack()
     {
          if (pi.rollEnabled == false && pi.inputRollEnabled == true && anim.GetBool("isAttack")
              && anim.GetBool("roll") == false)
          {
               anim.SetBool("roll",pi.buttonRoll.OnPressed);
          }


          if (anim.GetBool("roll") && anim.GetBool("isAttack"))
          {
               if (pi.rollEnabled && grounded)
               {
                    anim.Play("roll");
               }
          }
     }

     protected virtual void CheckForceStrike()
     {
          //TODO: 如果暮光之月增益存在的判定！！！
        
          //print(pi.buttonUp.isExtending?"正在Extending":"没有Extending");
        
          print((!pi.buttonAttack.isDelaying && pi.buttonAttack.IsPressing && pi.attackEnabled)?"正在蓄力":"没有蓄力");
        
          if(!pi.buttonAttack.isDelaying && pi.buttonAttack.IsPressing && pi.attackEnabled
             && !pi.hurt && grounded && !pi.isSkill)
          {
               if (forceLevel < 0)
               {
                    forceLevel = 0;
                    forcingTime = 0;
               }
               else
               {
                    forcingTime += Time.deltaTime * (1+_statusManager.fsSpeedBuff);
                    if(forcingTime > forcingRequireTime && forceLevel < maxForceLevel)
                    {
                         forceLevel++;
                         forcingTime = 0;
                    }

                    if (forceLevel >= maxForceLevel)
                    {
                         forcingTime = forcingRequireTime;
                    }
               }
          }
          else
          {
               if(forceLevel > 0)
               {
                    forceLevel = -1;
                    forcingTime = 0;
                    anim.Play("force_attack");
               }else if(forceLevel == 0)
               {
                    forceLevel = -1;
                    OnForceAttackExit();
                    anim.Play("idle");
                    forcingTime = 0;
               }
          }

     }

     protected override void CheckSkill()
     {
          if(pi.allowForceStrike && forceLevel>=0)
               return;
          
          if (pi.skill[0] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill)
          {
               UseSkill(1);
          }

          if (pi.skill[1] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill)
          {
               UseSkill(2);
          }
          
          if (pi.skill[2] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill)
          {
               UseSkill(3);
          }

          if (pi.skill[3] && !pi.hurt && !pi.isSkill)
          {
               UseSkill(4);
          }
          
          
     }

     public override void Roll()
     {
          anim.SetBool("roll",true);
          if (pi.rollEnabled && !pi.hurt)
          {
               anim.Play("roll");
          }
     }

     public override void StdAtk()
     {
          //anim.SetInteger("combo",comboStage);
          //anim.SetInteger("combo",anim.GetInteger("combo")+1);
          base.StdAtk();
          
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

     public override void IsGround()
     {
          anim.SetBool("isGround", true);
          pi.jumptime = 2;
          
          if(!anim.GetBool("isAttack"))
               pi.rollEnabled = true;
        
        
          
     }

     public override void OnHurtEnter()
     {
          OnForceAttackExit();
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


          // if (Combo == 0)
          //      Combo = 0;
          //
          // if(anim.GetInteger("combo")==0)
          //      anim.SetInteger("combo",0);
     }

     protected void OnIdleEnter()
     {
          if (comboStageResetRoutine != null)
          {
               StopCoroutine(comboStageResetRoutine);
               comboStageResetRoutine = null;
               print("interrupted");
          }

          comboStageResetRoutine = StartCoroutine(ResetComboStage(0.5f));
     }

     public override void OnSkillEnter()
     {
          base.OnSkillEnter();
          Combo = 0;
          OnAttackInterrupt?.Invoke();
     }

     public virtual void OnForceEnter()
     {
          print("onforceEnter");
          pi.inputRollEnabled = false;
          pi.stdAtk = false;
          pi.moveEnabled = false;
          pi.roll = false;
          pi.rollEnabled = false;
          
          pi.jumpEnabled = false;
          //pi.inputAttackEnabled = false;
          forcingTime = 0;
          forceLevel = 0;
          
          Combo = 0;
          
          ClearBoolSignal("attack");
     }

     public virtual void OnForcingMoveable()
     {
          pi.moveEnabled = true;
          speedModifier = 0.5f;
          
     }
     
     public virtual void OnForceAttackEnter()
     {
          speedModifier = 1;
          forceLevel = -1;
          pi.rollEnabled = false;
          pi.attackEnabled = false;
          pi.stdAtk = false;
          pi.inputRollEnabled = false;
          pi.directionLock = false;
          //dodging = true;
          pi.isSkill = true;

          ActionDisable((int)PlayerActionType.MOVE);
          ActionDisable((int)PlayerActionType.JUMP);
          ActionDisable((int)PlayerActionType.ROLL);
          ActionDisable((int)PlayerActionType.ATTACK);
          //pi.SetInputDisabled("attack");
          pi.SetInputDisabled("move");
     }
     
     public virtual void OnForceAttackExit()
     {
          forceLevel = -1;
          forcingTime = 0;
          pi.jumpEnabled = true;
          pi.moveEnabled = true;
          pi.attackEnabled = true;
          pi.jumpEnabled = true;
          pi.inputAttackEnabled = true;
          pi.inputRollEnabled = true;
          pi.rollEnabled = true;
          pi.roll = false;
          pi.isSkill = false;
          speedModifier = 1f;
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
}
