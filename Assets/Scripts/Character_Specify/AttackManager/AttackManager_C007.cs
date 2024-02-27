using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C007 : AttackManagerRanged
{
   [SerializeField] private GameObject transformFX;

   public bool IsDragondrive => (ac as ActorController_c007).DModeIsOn;

   public void InstantiateTransformWave()
   {
      InstantiateMeele(transformFX, transform.position + new Vector3(0, 1),
         InitContainer(true));
   }

   public void Combo1()
   {
      var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
         Quaternion.identity, RangedAttackFXLayer.transform);
      
      var proj = InstantiateRanged(combo1FX[0], transform.position,
         InitContainer(false),1);

      proj.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0);
      proj.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
         LayerMask.GetMask("Enemies"));

   }

   public void Combo1_Boost()
   {
      var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
         Quaternion.identity, RangedAttackFXLayer.transform);

      var container = InitContainer(false, 3);
      
      var projMain = InstantiateRanged(combo1FX[2], 
         transform.position+new Vector3(ac.facedir,0),
         container,ac.facedir);
      //projMain.GetComponent<DOTweenSimpleController>().moveDirection *= ac.facedir;
      int spGain = (int)projMain.GetComponent<AttackFromPlayer>().GetSpGain();
      int dir = ac.facedir;

      DOVirtual.DelayedCall(0.3f, () =>
      {

         Vector2[] offset = new[]
         {
            new Vector2(0.5f*ac.facedir,0.8f),
            new Vector2(0.5f*ac.facedir,-0.8f)
         };
         
         for (int i = 0; i < 2; i++)
         {
            var atk = InstantiateRanged(combo1FX[3],
               transform.position + (Vector3)offset[i], container, dir).GetComponent<AttackFromPlayer>();
            atk.SetSpGain(spGain);
            //atk.GetComponent<DOTweenSimpleController>().moveDirection *= ac.facedir;
         }
      }, false);


   }
   
   public void Combo2()
   {
      var muzzleFX = Instantiate(combo1FX[1], transform.position
                                              + new Vector3(ac.facedir, 0),
         Quaternion.identity, RangedAttackFXLayer.transform);
      
      var proj1 = InstantiateRanged(combo2FX[0], transform.position + new Vector3(0, 0.3f),
         InitContainer(false),1);

      proj1.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, -0.03f).normalized;
      proj1.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
         LayerMask.GetMask("Enemies"));
      
      var proj2 = InstantiateRanged(combo2FX[0], transform.position + new Vector3(0, -0.3f),
         InitContainer(false),1);

      proj2.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0.03f).normalized;
      proj2.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
         LayerMask.GetMask("Enemies"));
   }

   public void Combo2_Boost()
   {
      var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
         Quaternion.identity, RangedAttackFXLayer.transform);

      var container = InitContainer(false, 6);
      
      var projMain = InstantiateRanged(combo2FX[1], 
         transform.position + new Vector3(ac.facedir*0.6f,0.6f),
         container,ac.facedir);
      //projMain.GetComponent<DOTweenSimpleController>().moveDirection *= ac.facedir;
      int spGain = (int)projMain.GetComponent<AttackFromPlayer>().GetSpGain();
      
      var projMain2 = InstantiateRanged(combo2FX[1], 
         transform.position + new Vector3(ac.facedir*0.6f,-0.6f),
         container,ac.facedir);
      //projMain2.GetComponent<DOTweenSimpleController>().moveDirection *= ac.facedir;
      var dir = ac.facedir;

      DOVirtual.DelayedCall(0.35f, () =>
      {

         Vector2[] offset = new[]
         {
            new Vector2(0.5f*ac.facedir,0.8f),
            new Vector2(0.75f*ac.facedir,0.8f),
            new Vector2(0.5f*ac.facedir,-0.8f),
            new Vector2(0.75f*ac.facedir,-0.8f)
         };
         
         for (int i = 0; i < 4; i++)
         {
            var atk = InstantiateRanged(combo1FX[3],
               transform.position + (Vector3)offset[i], container, dir).GetComponent<AttackFromPlayer>();
            atk.SetSpGain(spGain);
            //atk.GetComponent<DOTweenSimpleController>().moveDirection *= ac.facedir;
         }
      }, false);
   }
   
   public void Combo3()
   {
      var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
         Quaternion.identity, RangedAttackFXLayer.transform);
      
      var proj1 = InstantiateRanged(combo3FX[0], transform.position + new Vector3(0, 0.5f),
         InitContainer(false),1);

      proj1.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, -0.03f).normalized;
      proj1.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
         LayerMask.GetMask("Enemies"));
      
      var proj2 = InstantiateRanged(combo3FX[0], transform.position + new Vector3(0, 0f),
         InitContainer(false),1);

      proj2.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0).normalized;
      proj2.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
         LayerMask.GetMask("Enemies"));
      
      var proj3 = InstantiateRanged(combo3FX[0], transform.position + new Vector3(0, -0.5f),
         InitContainer(false),1);

      proj3.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0.03f).normalized;
      proj3.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
         LayerMask.GetMask("Enemies"));
   }

   public void Combo3_Boost()
   {
      var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
         Quaternion.identity, RangedAttackFXLayer.transform);

      var container = InitContainer(false, 1);
      
      var projMain = InstantiateRanged(combo3FX[1], 
         transform.position + new Vector3(ac.facedir,0),
         container,ac.facedir);
      projMain.GetComponent<DOTweenSimpleController>().moveDirection *= ac.facedir;
   }
   
   public void Combo4()
   {
      var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
         Quaternion.identity, RangedAttackFXLayer.transform);
      
      var proj1 = InstantiateRanged(combo4FX[0], transform.position + new Vector3(0, 0.4f),
         InitContainer(false),1);

      proj1.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, -0.025f).normalized;
      proj1.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
         LayerMask.GetMask("Enemies"));
      
      var proj2 = InstantiateRanged(combo4FX[0], transform.position + new Vector3(0, -0.4f),
         InitContainer(false),1);

      proj2.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0.025f).normalized;
      proj2.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
         LayerMask.GetMask("Enemies"));
   }
   
   public void Combo4_Boost()
   {
      var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
         Quaternion.identity, RangedAttackFXLayer.transform);

      var container = InitContainer(false, 1);
      
      var projMain = InstantiateRanged(combo4FX[1], 
         transform.position + new Vector3(ac.facedir,0),
         container,ac.facedir);
      projMain.GetComponent<DOTweenSimpleController>().moveDirection *= ac.facedir;
   }
   
   public void Combo5()
   {
      var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
         Quaternion.identity, RangedAttackFXLayer.transform);

      GameObject container = InitContainer(false,5);
      
      var proj1 = InstantiateRanged(combo5FX[0], 
         transform.position + new Vector3(ac.facedir,0),
         container,1);

      proj1.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, 0).normalized;
      proj1.GetComponent<HomingAttack>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
         LayerMask.GetMask("Enemies"));

      var dir = ac.facedir;
      
      //List<GameObject> projectiles = new();
      Vector2[] pointOffsets = new[]
      {
         new Vector2(ac.facedir * 0.1f, -0.75f),
         new Vector2(ac.facedir * 0.6f, -0.4f),
         new Vector2(ac.facedir * 0.6f, 0.4f),
         new Vector2(ac.facedir * 0.1f, 0.75f),
      };

      Vector2[] startAngles = new[]
      {
         new Vector2(ac.facedir, 0.1f).normalized,
         new Vector2(ac.facedir, 0.04f).normalized,
         new Vector2(ac.facedir, -0.1f).normalized,
         new Vector2(ac.facedir, -0.04f).normalized
      };

      float[] angleZ = new[]
      {
         12f, 6f, -6f, -12f
      };

      for (int i = 0; i < 4; i++)
      {
         var proj = InstantiateRanged(combo5FX[1],
            transform.position + (Vector3)pointOffsets[i],container
            ,dir);

         proj.name = "combo5_" + i;
         //Mathf.Rad2Deg * Mathf.Atan2(startAngles[i].y,startAngles[i].x)
         print(proj.name);
      
         proj.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir,0);
         //proj.transform.rotation = Quaternion.Euler(0,0,angleZ[i]);
         proj.GetComponent<HomingAttack>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 22, 3,
            LayerMask.GetMask("Enemies"));
      }


      
   }
   
   public void Combo5_Boost()
   {
      var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
         Quaternion.identity, RangedAttackFXLayer.transform);

      var container = InitContainer(false, 7);
      
      var projMain = InstantiateRanged(combo5FX[2], 
         transform.position+new Vector3(ac.facedir,0),
         container,ac.facedir);
      projMain.GetComponent<DOTweenSimpleController>().moveDirection *= ac.facedir;
      int spGain = (int)projMain.GetComponent<AttackFromPlayer>().GetSpGain();

      DOVirtual.DelayedCall(0.3f, () =>
      {

         Vector2[] offset = new[]
         {
            new Vector2(0.5f*ac.facedir,1f),
            new Vector2(0.8f*ac.facedir,0.7f),
            new Vector2(0.5f*ac.facedir,-1f),
            new Vector2(0.8f*ac.facedir,-0.7f),
            new Vector2(1.1f*ac.facedir,0.2f),
            new Vector2(1.1f*ac.facedir,-0.2f)
         };
         
         for (int i = 0; i < 6; i++)
         {
            var atk = InstantiateRanged(combo1FX[3],
               transform.position + (Vector3)offset[i], container, ac.facedir).GetComponent<AttackFromPlayer>();
            atk.SetSpGain(spGain);
            //atk.GetComponent<DOTweenSimpleController>().moveDirection *= ac.facedir;
         }
      }, false);


   }

   public override void Skill1(int eventID)
   {
      if ((ac as ActorController).DModeIsOn)
      {
         var proj = Instantiate(skill1FX[1],gameObject.RaycastedPosition(),
            Quaternion.identity, RangedAttackFXLayer.transform);
         proj.GetComponent<Projectile_C007_1>().SetSource(gameObject);
      }
      else
      {
         var proj = InstantiateMeele(skill1FX[0], 
            gameObject.RaycastedPosition(), InitContainer(true, 1, true));
         var atk = proj.GetComponent<AttackFromPlayer>();
         atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,41,21,100),
            120);
         
         var checkConditionString = ((int)BasicCalculation.BattleCondition.Nihility).ToString();
         atk.AddConditionalAttackEffect(
            new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
               ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
               new string[] {"1", checkConditionString},
               new string[] {"0.5"})
            );
      }
   }

   public override void Skill2(int eventID)
   {
      if ((ac as ActorController).DModeIsOn)
      {
         (_statusManager as PlayerStatusManager).ChargeDP(16.67f,true);
         _statusManager.OnSpecialBuffDelegate?.
            Invoke(UI_BuffLogPopManager.SpecialConditionType.DragondriveCharge.ToString());
         
         var proj = InstantiateRanged(skill2FX[1], gameObject.RaycastedPosition()-new Vector2(0,1), InitContainer(false, 1, true),
            ac.facedir);
         var atk = proj.GetComponent<AttackFromPlayer>();
         
         atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Nihility,
            1,30,1),100);
         atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
            41,21,100,-1),120,1);
         
         var checkConditionString = ((int)BasicCalculation.BattleCondition.Nihility).ToString();
         atk.AddConditionalAttackEffect(
            new ConditionalAttackEffect(ConditionalAttackEffect.ConditionType.TargetHasCondition,
               ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
               new string[] {"1", checkConditionString},
               new string[] {"0.5"})
         );
         

      }
      else
      {
         _statusManager.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.DemonSeal,
            -1, true);
         (_statusManager as PlayerStatusManager).ChargeDP(100,true);
         _statusManager.OnSpecialBuffDelegate?.
            Invoke(UI_BuffLogPopManager.SpecialConditionType.DragondriveCharge.ToString());
         Instantiate(skill2FX[0], gameObject.RaycastedPosition(), Quaternion.identity,
            RangedAttackFXLayer.transform);

         var timerbuff = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
            10, -1, 1, 100701);
         timerbuff.dispellable = false;

         _statusManager.ObtainTimerBuff(timerbuff);
      }
      
      
      
      
   }

   
   
   
   
   
   
   
   
}
