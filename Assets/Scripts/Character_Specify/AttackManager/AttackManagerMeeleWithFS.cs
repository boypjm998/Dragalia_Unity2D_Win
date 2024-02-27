using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManagerMeeleWithFS : AttackManager
{
    [SerializeField] protected GameObject forceStrikeIndicatorPrefab;
    
    protected UI_ForceStrikeAimerMeele forceStrikeIndicator;
    
    [SerializeField] protected GameObject[] dashFX;
    [SerializeField] protected GameObject[] forceFX;
    [SerializeField] protected GameObject[] comboFX;
    [SerializeField] protected GameObject[] skillFX;
    
    public BasicCalculation.MeeleWeaponType weaponType;


    public override void DashAttack()
    {
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(dashFX[0],transform.position,container);
    }



    public void Combo1()
    {
        InstantiateMeele(comboFX[0], transform.position, InitContainer(true));
    }

    public void Combo2()
    {
        InstantiateMeele(comboFX[1], transform.position, InitContainer(true));
    }

    public void Combo3()
    {
        InstantiateMeele(comboFX[2], transform.position, InitContainer(true));
    }
    
    public void Combo4()
    {
        InstantiateMeele(comboFX[3], transform.position, InitContainer(true));
    }

    public void Combo5()
    {
        InstantiateMeele(comboFX[4], transform.position, InitContainer(true));
    }

    public void Combo5_Ranged()
    {
        InstantiateRanged(comboFX[4], transform.position, InitContainer(false),ac.facedir);
    }


    public virtual void ForceStrikeRelease(int currentFSLV){
        
        if(currentFSLV <= 0)
            return;
        
        ac.OnAttackInterrupt?.Invoke();

        ac.SetFaceDir(forceStrikeIndicator.forceDirection);
        
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(forceFX[0],transform.position,container);
        
        //base.ForceStrikeRelease(currentFSLV);
        if (weaponType == BasicCalculation.MeeleWeaponType.Blade)
        {
            (ac as ActorControllerMeeleWithFS).BladeForceStrikeMove();
        }else if (weaponType == BasicCalculation.MeeleWeaponType.Lance)
        {
            (ac as ActorControllerMeeleWithFS).LanceForceStrikeMove();
        }else if (weaponType == BasicCalculation.MeeleWeaponType.Sword)
        {
            var pi = (ac as ActorController).pi;
            if (pi.buttonLeft.IsPressing && !pi.buttonRight.IsPressing)
            {
                DOVirtual.DelayedCall(0.1f,
                    () =>
                    {
                        if(ac.anim.GetCurrentAnimatorStateInfo(0).IsName("force_attack"))
                            (ac as ActorControllerMeeleWithFS).GeneralHorizontalMovement(4, 0.1f);
                    },false);
            }
            else if (!pi.buttonLeft.IsPressing && pi.buttonRight.IsPressing)
            {
                DOVirtual.DelayedCall(0.1f,
                    () =>
                    {
                        if(ac.anim.GetCurrentAnimatorStateInfo(0).IsName("force_attack"))
                            (ac as ActorControllerMeeleWithFS).GeneralHorizontalMovement(4, 0.1f);
                    },false);
            }
            else
            {
                DOVirtual.DelayedCall(0.1f,
                    () =>
                    {
                        if(ac.anim.GetCurrentAnimatorStateInfo(0).IsName("force_attack"))
                            (ac as ActorControllerMeeleWithFS).GeneralHorizontalMovementWithEnemyCheck(4, 3, 2f, 0.1f);
                    }, false);

            }

        }
        (ac as ActorController).PlayAttackVoice(9);
    
    }
    
    public void OnForceStart()
    {
        
        
        
        if (forceStrikeIndicator == null)
        {
            var prefabIndicator = Instantiate(forceStrikeIndicatorPrefab, transform.position, Quaternion.identity,
                BuffFXLayer.gameObject.transform);
            forceStrikeIndicator = prefabIndicator.GetComponent<UI_ForceStrikeAimerMeele>();
            prefabIndicator.name = "ForceStrikeIndicator";
            forceStrikeIndicator.SetActorController(ac as ActorControllerMeeleWithFS);
            forceStrikeIndicator.SetMaxForceInfo(new float[] {(ac as ActorControllerMeeleWithFS).forcingRequireTime}.ToList());
            if ((ac as ActorControllerMeeleWithFS).maxForceLevel > 1)
            {
                List<float> forceInfo = new();
                for (int i = 0; i < (ac as ActorControllerMeeleWithFS).maxForceLevel; i++)
                {
                    forceInfo.Add((ac as ActorControllerMeeleWithFS).forcingRequireTime);
                }
                forceStrikeIndicator.SetMaxForceInfo(forceInfo);
            }
        }
        else
        {
            if(forceStrikeIndicator.gameObject.activeSelf)
                return;
            forceStrikeIndicator.gameObject.SetActive(true);
        }
        forceStrikeIndicator.SetForceDirection(ac.facedir);
        Debug.Log("Called OnForceStart");

    }
    
    
    
    protected virtual void OnForcingUpdate()
    {
        var ac = this.ac as ActorController;
        
        if(ac.pi.buttonLeft.IsPressing && !ac.pi.buttonRight.IsPressing)
            forceStrikeIndicator.SetForceDirection(-1);
        
        else if(ac.pi.buttonRight.IsPressing && !ac.pi.buttonLeft.IsPressing)
            forceStrikeIndicator.SetForceDirection(1);
        
        
    }

    public void Skill4()
    {
        _statusManager.HPRegenImmediately(0,10,true);
        BattleEffectManager.Instance.SpawnHealEffect(gameObject);
        //Instantiate(healbuff, transform.position, Quaternion.identity, BuffFXLayer.transform);
        _statusManager.ObtainHealOverTimeBuff(10,15,true);
        
    }

    protected void OnStandardAttackEnter()
    {
        if (weaponType == BasicCalculation.MeeleWeaponType.Lance)
        {
            _statusManager.knockbackRes = 99;
        }
        else if (weaponType == BasicCalculation.MeeleWeaponType.Sword &&
                 (ac.anim.GetCurrentAnimatorStateInfo(0).IsName("combo4") ||
                 ac.anim.GetCurrentAnimatorStateInfo(0).IsName("combo5"))   )
        {
            _statusManager.knockbackRes = 200;
        }
    }
    protected void OnStandardAttackExit()
    {
        if (weaponType == BasicCalculation.MeeleWeaponType.Lance ||
            weaponType == BasicCalculation.MeeleWeaponType.Sword)
        {
            _statusManager.ResetKBRes();
        }
    }
    
    
}
