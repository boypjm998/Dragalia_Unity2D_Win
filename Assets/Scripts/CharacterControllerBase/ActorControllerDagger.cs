using UnityEngine;
using GameMechanics;
using System.Collections;
public class ActorControllerDagger : ActorController
{
     public int comboStage = 0;
     

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
          CheckSkill();
     }

     protected override void CheckSkill()
     {
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
          //comboStage = 0;
          //anim.SetInteger("combo",0);
          Combo = 0;
          OnAttackInterrupt?.Invoke();
          
     }

     public override void onRollExit()
     {
          base.onRollExit();
          //comboStage = 0;
          //anim.SetInteger("combo",0);
          Combo = 0;
     }

     public override void OnHurtEnter()
     {
          base.OnHurtEnter();
          //comboStage = 0;
          //anim.SetInteger("combo",0);
          Combo = 0;
     }

     public override void OnStandardAttackEnter()
     {
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
          base.OnStandardAttackExit();
          pi.jumpEnabled = true;
          pi.moveEnabled = true;
          pi.attackEnabled = true;
          pi.jumpEnabled = true;
          pi.inputAttackEnabled = true;
          pi.inputRollEnabled = true;
          pi.rollEnabled = true;


          // if (Combo == 0)
          //      Combo = 0;
          //
          // if(anim.GetInteger("combo")==0)
          //      anim.SetInteger("combo",0);
     }

     void OnIdleEnter()
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

     /// <summary>
     /// 
     /// </summary>
     /// <param name="moveID">1:距离12,2:距离3,3：距离2,4：距离5</param>
     public override void FaceDirectionAutoFix(int moveID)
     {
          switch (moveID)
          {
               case 1:
               {
                    if (ta.GetNearestTargetInRangeDirection
                        (facedir, 12f, .5f,
                             LayerMask.GetMask("Enemies")) == null
                        &&
                        ta.GetNearestTargetInRangeDirection
                        (-facedir, 12f, .5f,
                             LayerMask.GetMask("Enemies")) != null)
                    {
                         SetFaceDir(-facedir);
                    }

                    break;
               }
               case 2:
               {
                    if (ta.GetNearestTargetInRangeDirection
                        (facedir, 3f, .5f,
                             LayerMask.GetMask("Enemies")) == null
                        &&
                        ta.GetNearestTargetInRangeDirection
                        (-facedir, 3f, .5f,
                             LayerMask.GetMask("Enemies")) != null)
                    {
                         SetFaceDir(-facedir);
                    }

                    break;
               }
               case 3:
               {
                    if (ta.GetNearestTargetInRangeDirection
                        (facedir, 2f, .5f,
                             LayerMask.GetMask("Enemies")) == null
                        &&
                        ta.GetNearestTargetInRangeDirection
                        (-facedir, 2f, .5f,
                             LayerMask.GetMask("Enemies")) != null)
                    {
                         SetFaceDir(-facedir);
                    }

                    break;
               }
               case 4:
               {
                    if (ta.GetNearestTargetInRangeDirection
                        (facedir, 5f, 1.5f,
                             LayerMask.GetMask("Enemies")) == null
                        &&
                        ta.GetNearestTargetInRangeDirection
                        (-facedir, 5f, 1.5f,
                             LayerMask.GetMask("Enemies")) != null)
                    {
                         SetFaceDir(-facedir);
                    }

                    break;
               }
          }
     }


     IEnumerator ResetComboStage(float time)
     {
          //print("reset_before");
          yield return new WaitForSeconds(time);
          //print("reset");
          //comboStage = 0;
          //anim.SetInteger("combo",0);
          Combo = 0;
          comboStageResetRoutine = null;
     }

     public void FaceDirectionAutoFix()
     {
          
     }
}
